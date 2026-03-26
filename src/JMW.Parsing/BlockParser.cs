using System.Text;

namespace JMW.Parsing;

/// <summary>
/// General-purpose parsing engine that converts raw command output into Pair trees
/// using a declarative ParserConfig. This is the core of the framework — command-specific
/// parsers are thin wrappers that supply a config and optional post-processing.
/// </summary>
public static class BlockParser
{
    /// <summary>
    /// Parse raw command output into block-level Pair collections using the given config.
    /// </summary>
    public static IEnumerable<IReadOnlyList<Pair>> ParseBlocks(TextReader input, ParserConfig config)
    {
        var blocks = config.BlockSplitter is not null
            ? config.BlockSplitter(input)
            : Helpers.GetBlocks(input, config.TrimInitialWhitespace, config.TrimEndingWhitespace);

        foreach (var block in blocks)
        {
            if (config.BlockFilter is not null && !config.BlockFilter(block))
            {
                config.OnBlockSkipped?.Invoke(block);
                continue;
            }

            yield return ParseBlock(block, config);
        }
    }

    /// <summary>
    /// Parse a single pre-split block of text into a Pair collection.
    /// Useful when callers already have blocks from their own splitting logic.
    /// </summary>
    public static IReadOnlyList<Pair> ParseBlock(string block, ParserConfig config)
    {
        using var blockReader = new StringReader(block);

        if (config.Tokenizer.MeasureIndentation)
        {
            var tokens = TokenizeWithDepth(blockReader, config.Tokenizer);
            return BuildTreeFromDepth(CollectLines(tokens));
        }

        var flatTokens = Tokenize(blockReader, config.Tokenizer);
        return WalkTokens(flatTokens, config).ToArray();
    }

    /// <summary>
    /// Tokenize raw text into a stream of string tokens using the given config.
    /// </summary>
    public static IEnumerable<string> Tokenize(TextReader output, TokenizerConfig config)
    {
        int? c;
        var sb = new StringBuilder();
        while ((c = output.Read()) != -1)
        {
            var ch = (char)c;

            if (config.Separators.Contains(ch))
            {
                if (sb.Length > 0)
                {
                    StripTrailingAndYield(sb, config);
                    foreach (var t in FlushBuffer(sb, config))
                        yield return t;
                }

                continue;
            }

            if (ch == '\n')
            {
                if (sb.Length > 0)
                {
                    StripTrailingAndYield(sb, config);
                    foreach (var t in FlushBuffer(sb, config))
                        yield return t;
                }

                if (config.EmitNewLineTokens)
                {
                    yield return Helpers.NewLine;
                }

                continue;
            }

            if (!char.IsWhiteSpace(ch))
            {
                sb.Append(ch);
                continue;
            }

            // whitespace - flush current token
            if (sb.Length == 0)
            {
                continue;
            }

            StripTrailingAndYield(sb, config);
            foreach (var t in FlushBuffer(sb, config))
                yield return t;
        }

        // flush remainder
        if (sb.Length > 0)
        {
            StripTrailingAndYield(sb, config);
            foreach (var t in FlushBuffer(sb, config))
                yield return t;
        }
    }

    /// <summary>
    /// Tokenize raw text into a stream of depth-annotated tokens.
    /// Leading whitespace on each line is measured and stored as <see cref="Token.Depth"/>.
    /// Requires <see cref="TokenizerConfig.MeasureIndentation"/> to be set (enforced by convention, not runtime check).
    /// </summary>
    public static IEnumerable<Token> TokenizeWithDepth(TextReader output, TokenizerConfig config)
    {
        int? c;
        var sb = new StringBuilder();
        var lineStart = true;
        var currentDepth = 0;

        while ((c = output.Read()) != -1)
        {
            var ch = (char)c;

            if (ch == '\n')
            {
                if (sb.Length > 0)
                {
                    StripTrailingAndYield(sb, config);
                    foreach (var t in FlushBuffer(sb, config))
                        yield return new Token(t, currentDepth);
                }

                if (config.EmitNewLineTokens)
                {
                    yield return new Token(Helpers.NewLine, 0);
                }

                lineStart = true;
                currentDepth = 0;
                continue;
            }

            // Count leading whitespace as depth
            if (lineStart && char.IsWhiteSpace(ch))
            {
                currentDepth++;
                continue;
            }

            lineStart = false;

            if (config.Separators.Contains(ch))
            {
                if (sb.Length > 0)
                {
                    StripTrailingAndYield(sb, config);
                    foreach (var t in FlushBuffer(sb, config))
                        yield return new Token(t, currentDepth);
                }

                continue;
            }

            if (!char.IsWhiteSpace(ch))
            {
                sb.Append(ch);
                continue;
            }

            // Whitespace mid-line — flush current token
            if (sb.Length == 0)
            {
                continue;
            }

            StripTrailingAndYield(sb, config);
            foreach (var t in FlushBuffer(sb, config))
                yield return new Token(t, currentDepth);
        }

        // flush remainder
        if (sb.Length > 0)
        {
            StripTrailingAndYield(sb, config);
            foreach (var t in FlushBuffer(sb, config))
                yield return new Token(t, currentDepth);
        }
    }

    private static void StripTrailingAndYield(StringBuilder sb, TokenizerConfig config)
    {
        while (sb.Length > 0 && config.StripTrailing.Contains(sb[^1]))
        {
            sb.Remove(sb.Length - 1, 1);
        }
    }

    private static IEnumerable<string> FlushBuffer(StringBuilder sb, TokenizerConfig config)
    {
        if (sb.Length == 0)
        {
            yield break;
        }

        if (config.SplitOnSingleColon)
        {
            var colonCount = Helpers.CountChars(sb, ':');
            if (colonCount[':'] == 1)
            {
                var tokens = sb.ToString().Split(':');
                yield return tokens[0];
                yield return tokens[1];
                sb.Clear();
                yield break;
            }
        }

        yield return sb.ToString();
        sb.Clear();
    }

    /// <summary>
    /// Walk a token stream against the keyword schema and produce Pair tree.
    /// </summary>
    private static IEnumerable<Pair> WalkTokens(IEnumerable<string> tokens, ParserConfig config)
    {
        var queue = new Queue<string>();
        using var enumerator = tokens.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            yield break;
        }

        // Handle first token(s) via config hook or default
        if (config.FirstTokenHandler is not null)
        {
            foreach (var pair in config.FirstTokenHandler(enumerator, queue))
            {
                yield return pair;
            }
        }
        else
        {
            yield return new Pair("Name", enumerator.Current, []);
        }

        while (Helpers.TryGetValue(enumerator, queue, out var token))
        {
            if (token == Helpers.NewLine)
            {
                continue;
            }

            foreach (var pair in HandleToken(token, enumerator, queue, config))
            {
                yield return pair;
            }
        }
    }

    private static IEnumerable<Pair> HandleToken(
        string token,
        IEnumerator<string> enumerator,
        Queue<string> queue,
        ParserConfig config)
    {
        if (!config.Keywords.TryGetValue(token, out var def))
        {
            if (config.UnknownTokenHandler is not null)
            {
                foreach (var pair in config.UnknownTokenHandler(token, enumerator, queue))
                {
                    yield return pair;
                }
            }

            yield break;
        }

        // Custom handler takes full control
        if (def.CustomHandler is not null)
        {
            foreach (var pair in def.CustomHandler(token, enumerator, queue))
            {
                yield return pair;
            }

            yield break;
        }

        // Multi-token keyword at top level (non-array, non-group)
        if (def.MultiToken is not null && !def.IsGroup && !def.IsArray)
        {
            foreach (var pair in HandleMultiTokenKeyword(queue, enumerator, def.MultiToken, token))
            {
                yield return pair;
            }

            yield break;
        }

        // Group keyword
        if (def.IsGroup)
        {
            if (def.IsArray)
            {
                foreach (var pair in HandleArrayKeyword(queue, enumerator, token, def, config))
                {
                    yield return pair;
                }
            }
            else if (def.MergeChildren)
            {
                var children = HandleArrayKeyword(queue, enumerator, token, def, config)
                   .SelectMany(static o => o.Children.SelectMany(static c => c.Children))
                   .ToArray();

                yield return new Pair(token, string.Empty, children, ChildType.ObjectType);
            }
            else
            {
                foreach (var pair in HandleGroup(queue, enumerator, token, def, config))
                {
                    yield return pair;
                }
            }

            yield break;
        }

        // Simple single keyword (possibly with array collection)
        if (def.IsArray)
        {
            foreach (var pair in HandleSimpleArrayKeyword(queue, enumerator, token, def, config))
            {
                yield return pair;
            }
        }
        else
        {
            foreach (var pair in ReadKeywordValue(queue, enumerator, def.Kind, token))
            {
                yield return pair;
            }
        }
    }

    private static IEnumerable<Pair> HandleGroup(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        string token,
        KeywordDef def,
        ParserConfig config)
    {
        var pairChildren = new List<Pair>();

        if (def.Kind == KeywordKind.GroupNext)
        {
            pairChildren.AddRange(ReadKeywordValue(queue, enumerator, KeywordKind.Next, token));
        }

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == Helpers.NewLine)
            {
                continue;
            }

            if (def.ChildKeywords!.TryGetValue(nextToken, out var childDef))
            {
                // If child has a custom handler, use it
                if (childDef.CustomHandler is not null)
                {
                    pairChildren.AddRange(childDef.CustomHandler(nextToken, enumerator, queue));
                }
                // If this is also a top-level group keyword, use the full top-level definition
                else if (config.Keywords.TryGetValue(nextToken, out var topLevelDef) && topLevelDef.IsGroup)
                {
                    if (topLevelDef.IsArray)
                    {
                        pairChildren.AddRange(HandleArrayKeyword(queue, enumerator, nextToken, topLevelDef, config));
                    }
                    else
                    {
                        pairChildren.AddRange(HandleGroup(queue, enumerator, nextToken, topLevelDef, config));
                    }
                }
                else
                {
                    pairChildren.AddRange(ReadKeywordValue(queue, enumerator, childDef.Kind, nextToken));
                }
            }
            else if (def.ChildMultiTokenKeywords is not null && def.ChildMultiTokenKeywords.ContainsKey(nextToken))
            {
                pairChildren.AddRange(HandleMultiTokenKeyword(queue, enumerator, def.ChildMultiTokenKeywords[nextToken], nextToken));
            }
            else
            {
                queue.Enqueue(nextToken);
                break;
            }
        }

        yield return new Pair(token, string.Empty, pairChildren, ChildType.ObjectType);
    }

    private static IEnumerable<Pair> HandleArrayKeyword(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        string token,
        KeywordDef def,
        ParserConfig config)
    {
        var key = def.ArrayKey ?? $"{token}s";
        var pairChildren = new List<Pair>();

        pairChildren.AddRange(HandleGroup(queue, enumerator, token, def, config));

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == token)
            {
                pairChildren.AddRange(HandleGroup(queue, enumerator, token, def, config));
            }
            else
            {
                queue.Enqueue(nextToken);
                break;
            }
        }

        yield return new Pair(key, string.Empty, pairChildren, ChildType.ArrayType);
    }

    /// <summary>
    /// Handles array collection for non-group (simple/single) keywords.
    /// Used by ScutilDns for "nameserver" and "search domain" array patterns.
    /// </summary>
    private static IEnumerable<Pair> HandleSimpleArrayKeyword(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        string token,
        KeywordDef def,
        ParserConfig config)
    {
        var pairChildren = new List<Pair>();

        // Handle multi-token compound first if applicable
        var currentToken = token;
        if (def.MultiToken is not null)
        {
            currentToken = BuildMultiTokenKey(queue, enumerator, def.MultiToken, token);
        }

        var key = def.ArrayKey ?? $"{currentToken}s";

        if (!enumerator.MoveNext())
        {
            yield break;
        }

        pairChildren.AddRange(ReadKeywordValue(queue, enumerator, def.Kind, $"{currentToken}{enumerator.Current}"));

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == Helpers.NewLine)
            {
                continue;
            }

            if (nextToken == token)
            {
                if (def.MultiToken is not null)
                {
                    var childToken = BuildMultiTokenKey(queue, enumerator, def.MultiToken, token);
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    pairChildren.AddRange(ReadKeywordValue(queue, enumerator, def.Kind, $"{childToken}{enumerator.Current}"));
                }
                else
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    pairChildren.AddRange(ReadKeywordValue(queue, enumerator, def.Kind, $"{token}{enumerator.Current}"));
                }
            }
            else
            {
                queue.Enqueue(nextToken);
                break;
            }
        }

        yield return new Pair(key, string.Empty, pairChildren, ChildType.ArrayType);
    }

    private static IEnumerable<Pair> HandleMultiTokenKeyword(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        MultiTokenDef multiToken,
        string token)
    {
        var key = token;
        var escape = false;

        foreach (var (expectedToken, expectedKind) in multiToken.Keywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                yield break;
            }

            if (nextItem != expectedToken)
            {
                queue.Enqueue(nextItem);
                escape = true;
                break;
            }

            if (expectedKind != KeywordKind.Drop)
            {
                key += $" {nextItem}";
            }
        }

        if (escape)
        {
            yield break;
        }

        // Use ReadKeywordValue for the final value read
        foreach (var pair in ReadKeywordValue(queue, enumerator, multiToken.Kind, key))
        {
            yield return pair;
        }
    }

    private static string BuildMultiTokenKey(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        MultiTokenDef multiToken,
        string token)
    {
        var key = token;
        foreach (var (expectedToken, _) in multiToken.Keywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                break;
            }

            if (nextItem != expectedToken)
            {
                queue.Enqueue(nextItem);
                break;
            }

            key += $" {nextItem}";
        }

        return key;
    }

    /// <summary>
    /// Read a keyword's value from the token stream based on the keyword kind.
    /// Handles NewLine, Next, and Single kinds.
    /// </summary>
    internal static IEnumerable<Pair> ReadKeywordValue(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        KeywordKind kind,
        string token)
    {
        if (kind == KeywordKind.NewLine)
        {
            var sb = new StringBuilder();
            while (Helpers.TryGetValue(enumerator, queue, out var nextItem) && nextItem != Helpers.NewLine)
            {
                sb.Append(' ').Append(nextItem);
            }

            yield return new Pair(token, sb.ToString().TrimStart(), []);
        }
        else if (kind == KeywordKind.Next)
        {
            if (Helpers.TryGetValue(enumerator, queue, out var value))
            {
                yield return new Pair(token, value, []);
            }
        }
        else if (kind == KeywordKind.Single)
        {
            yield return new Pair(token, "true", []);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported keyword kind: {kind}. Use CustomHandler for format-specific patterns.");
        }
    }

    /// <summary>
    /// Collect depth-annotated tokens into per-line entries (depth, key, value).
    /// Each line's first token becomes the key; remaining tokens are space-joined into the value.
    /// </summary>
    private static List<(int Depth, string Key, string Value)> CollectLines(IEnumerable<Token> tokens)
    {
        var lines = new List<(int Depth, string Key, string Value)>();
        var lineTokens = new List<string>();
        var lineDepth = 0;

        foreach (var token in tokens)
        {
            if (token.Value == Helpers.NewLine)
            {
                if (lineTokens.Count > 0)
                {
                    lines.Add((lineDepth, lineTokens[0],
                        lineTokens.Count > 1
                            ? string.Join(" ", lineTokens.Skip(1))
                            : string.Empty));
                    lineTokens.Clear();
                }

                continue;
            }

            if (lineTokens.Count == 0)
            {
                lineDepth = token.Depth;
            }

            lineTokens.Add(token.Value);
        }

        // Flush last line (input may not end with newline)
        if (lineTokens.Count > 0)
        {
            lines.Add((lineDepth, lineTokens[0],
                lineTokens.Count > 1
                    ? string.Join(" ", lineTokens.Skip(1))
                    : string.Empty));
        }

        return lines;
    }

    /// <summary>
    /// Build a Pair tree from line entries using indentation depth to determine parent/child relationships.
    /// Lines at greater depth become children of the nearest preceding line at lesser depth.
    /// </summary>
    private static IReadOnlyList<Pair> BuildTreeFromDepth(List<(int Depth, string Key, string Value)> lines)
    {
        if (lines.Count == 0)
        {
            return [];
        }

        // Stack of open frames. Each frame accumulates children until a same-or-lesser depth line closes it.
        var stack = new Stack<(int Depth, string Key, string Value, List<Pair> Children)>();

        // Sentinel root at depth -1 collects all top-level pairs.
        stack.Push((-1, string.Empty, string.Empty, new List<Pair>()));

        foreach (var (depth, key, value) in lines)
        {
            // Pop completed frames back to the appropriate parent.
            while (stack.Count > 1 && stack.Peek().Depth >= depth)
            {
                var completed = stack.Pop();
                var childType = completed.Children.Count > 0 ? ChildType.ObjectType : ChildType.StringType;
                stack.Peek().Children.Add(new Pair(completed.Key, completed.Value, completed.Children, childType));
            }

            stack.Push((depth, key, value, new List<Pair>()));
        }

        // Unwind remaining frames.
        while (stack.Count > 1)
        {
            var completed = stack.Pop();
            var childType = completed.Children.Count > 0 ? ChildType.ObjectType : ChildType.StringType;
            stack.Peek().Children.Add(new Pair(completed.Key, completed.Value, completed.Children, childType));
        }

        return stack.Pop().Children;
    }
}
