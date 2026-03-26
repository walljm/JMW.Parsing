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
        var blocks = Helpers.GetBlocks(input, config.TrimInitialWhitespace, config.TrimEndingWhitespace);
        foreach (var block in blocks)
        {
            if (config.BlockFilter is not null && !config.BlockFilter(block))
            {
                config.OnBlockSkipped?.Invoke(block);
                continue;
            }

            using var blockReader = new StringReader(block);
            var tokens = Tokenize(blockReader, config.Tokenizer);
            yield return WalkTokens(tokens, config).ToArray();
        }
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
            foreach (var pair in HandleSingleKeyword(queue, enumerator, def.Kind, token, config))
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
            pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, KeywordKind.Next, token, config));
        }

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == Helpers.NewLine)
            {
                continue;
            }

            if (def.ChildKeywords!.TryGetValue(nextToken, out var childKind))
            {
                // Is this child also a top-level group keyword?
                if (config.Keywords.TryGetValue(nextToken, out var childDef) && childDef.IsGroup)
                {
                    if (childDef.IsArray)
                    {
                        pairChildren.AddRange(HandleArrayKeyword(queue, enumerator, nextToken, childDef, config));
                    }
                    else
                    {
                        pairChildren.AddRange(HandleGroup(queue, enumerator, nextToken, childDef, config));
                    }
                }
                else
                {
                    pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, childKind, nextToken, config));
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
        var key = $"{token}s";
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

        var key = $"{currentToken}s";

        enumerator.MoveNext();
        pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, def.Kind, $"{currentToken}{enumerator.Current}", config));

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
                    enumerator.MoveNext();
                    pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, def.Kind, $"{childToken}{enumerator.Current}", config));
                }
                else
                {
                    enumerator.MoveNext();
                    pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, def.Kind, $"{token}{enumerator.Current}", config));
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

        foreach (var item in multiToken.Keywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                yield break;
            }

            if (!multiToken.Keywords.ContainsKey(nextItem))
            {
                queue.Enqueue(nextItem);
                escape = true;
                break;
            }

            if (item.Value != KeywordKind.Drop)
            {
                key += $" {nextItem}";
            }
        }

        if (escape)
        {
            yield break;
        }

        // Use Helpers.HandleSingleKeyword for the final value read
        foreach (var pair in Helpers.HandleSingleKeyword(queue, enumerator, multiToken.Kind, key, '<', '>'))
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
        foreach (var _ in multiToken.Keywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                break;
            }

            if (!multiToken.Keywords.ContainsKey(nextItem))
            {
                queue.Enqueue(nextItem);
                break;
            }

            key += $" {nextItem}";
        }

        return key;
    }

    private static IEnumerable<Pair> HandleSingleKeyword(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        KeywordKind kind,
        string token,
        ParserConfig config)
    {
        return Helpers.HandleSingleKeyword(
            queue, enumerator, kind, token,
            config.OptionDelimiters.Open, config.OptionDelimiters.Close,
            config.OptionsReadUntilNewLine);
    }
}
