using System.Text;
using System.Text.Json;

namespace JMW.Parsing;

public static class ScutilDns
{
    #region Public Parse/Output

    public static void Parse(TextReader inputReader, TextWriter outputWriter, ParsingOptions parsingOptions)
    {
        if (parsingOptions.OutputType == OutputType.KeyValue)
        {
            OutputKeyValues(inputReader, outputWriter);
        }
        else if (parsingOptions.OutputType == OutputType.Json)
        {
            OutputJson(inputReader, outputWriter);
        }
    }

    public static void OutputKeyValues(TextReader inputReader, TextWriter outputWriter)
    {
        var blocks = Helpers.GetBlocks(inputReader, trimInitialWhitespace: true, trimEndingWhitespace: false);

        var header = string.Empty;
        foreach (var block in blocks)
        {
            if (block.StartsWith("DNS"))
            {
                header = block.Trim();
                continue;
            }

            outputWriter.WriteLine(); // empty line for a spacer

            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Tokenize(blockReader), header);

            foreach (var pair in pairs)
            {
                pair.WriteKeyValues(outputWriter);
            }
        }
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        var blocks = Helpers.GetBlocks(inputReader, trimInitialWhitespace: true, trimEndingWhitespace: false);
        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true,
        };

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

        writer.WriteStartArray();

        var header = string.Empty;

        foreach (var block in blocks)
        {
            if (block.StartsWith("DNS"))
            {
                header = block.Trim();
                continue;
            }

            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Tokenize(blockReader), header).ToArray();
            if (pairs.Length == 0)
            {
                continue;
            }

            writer.WriteStartObject();
            foreach (var pair in pairs)
            {
                pair.WriteJson(writer);
            }

            writer.Flush();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());
        outputWriter.WriteLine(json);
    }

    #endregion

    #region Classes and Constants

    private record Pair(string Key, string Value, Pair[] Children, ChildType ChildType = ChildType.StringType)
    {
        public void WriteJson(Utf8JsonWriter writer, bool ignoreKey = false)
        {
            switch (ChildType)
            {
                case ChildType.StringType:

                    writer.WriteString(Helpers.CleanKey(Key), Value);
                    break;
                case ChildType.ObjectType:
                {
                    if (ignoreKey)
                    {
                        writer.WriteStartObject();
                    }
                    else
                    {
                        writer.WriteStartObject(Helpers.CleanKey(Key));
                    }

                    foreach (var child in Children)
                    {
                        child.WriteJson(writer);
                    }

                    writer.WriteEndObject();
                }
                    break;
                case ChildType.ArrayType:
                    if (ignoreKey)
                    {
                        writer.WriteStartArray();
                    }
                    else
                    {
                        writer.WriteStartArray(Helpers.CleanKey(Key));
                    }

                    foreach (var child in Children)
                    {
                        if (child.ChildType == ChildType.StringType)
                        {
                            writer.WriteStringValue(child.Value);
                        }
                        else
                        {
                            child.WriteJson(writer, true);
                        }
                    }

                    writer.WriteEndArray();
                    break;
            }
        }

        public void WriteKeyValues(TextWriter writer, int indent = 0)
        {
            writer.WriteLine($"{string.Empty.PadLeft(indent)}{Helpers.CleanKey(Key)}: {Value}");
            if (Children.Length > 0)
            {
                foreach (var child in Children)
                {
                    child.WriteKeyValues(writer, indent + 4);
                }
            }
        }
    }

    private enum TokenKind
    {
        NewLine,
        Next,
        Single,
        Options,
        Drop,
        Group,
    }

    private enum TokenType
    {
        Single,
        Multiple,
        Group,
        MultipleGroup,
    }

    private record TokenDef(
        TokenKind Kind,
        TokenType Type,
        HashSet<string>? MultipleKeywords = default,
        bool IsArray = false
    );

    private static readonly Dictionary<string, TokenDef> Tokens = new()
    {
        { "nameserver", new TokenDef(TokenKind.Next, TokenType.Single, IsArray: true) },
        { "if_index", new TokenDef(TokenKind.NewLine, TokenType.Single) },
        { "flags", new TokenDef(TokenKind.NewLine, TokenType.Single) },
        { "reach", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "domain", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "options", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "timeout", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "order", new TokenDef(TokenKind.Next, TokenType.Single) },

        // Multiple
        { "search", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "domain" ], IsArray: true) },
    };

    #endregion

    #region Private Functions

    private static IEnumerable<Pair> GetPairs(IEnumerable<string> tokens, string header)
    {
        var queue = new Queue<string>();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext(); // move to the first item.
        if (enumerator.Current == "resolver")
        {
            enumerator.MoveNext();
        }
        else
        {
            // something went wrong, exit.
            yield break;
        }

        // first token should always be the interface name.
        yield return new Pair("Type", header, []);
        yield return new Pair("Resolver", enumerator.Current.Trim('#'), []);


        while (Helpers.TryGetValue(enumerator, queue, out var token))
        {
            foreach (var pair in HandleChild(enumerator, queue, token))
            {
                yield return pair;
            }
        }
    }

    private static IEnumerable<Pair> HandleChild(IEnumerator<string> enumerator, Queue<string> queue, string token)
    {
        if (!Tokens.TryGetValue(token, out var tokenDef))
        {
            yield break;
        }

        if (tokenDef.Type == TokenType.Multiple)
        {
            token = HandleMultipleKeywords(queue, enumerator, tokenDef, token);
        }

        if (token is null)
        {
            throw new InvalidOperationException("Token can't be null");
        }

        if (tokenDef.IsArray)
        {
            foreach (var item in HandleArrayKeywords(queue, enumerator, tokenDef, token))
            {
                yield return item;
            }
        }
        else
        {
            foreach (var item in HandleSingleKeyword(queue, enumerator, tokenDef.Kind, token))
            {
                yield return item;
            }
        }
    }

    private static IEnumerable<Pair> HandleArrayKeywords(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        TokenDef tokenDef,
        string token
    )
    {
        var key = $"{token}s";
        var pairChildren = new List<Pair>();

        enumerator.MoveNext();
        pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, tokenDef.Kind, $"{token}{enumerator.Current}"));

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == Helpers.NewLine)
            {
                continue;
            }
            else if (nextToken == token)
            {
                enumerator.MoveNext(); // advance to the index.
                if (tokenDef.Type == TokenType.Multiple)
                {
                    var childToken = HandleMultipleKeywords(queue, enumerator, tokenDef, token);
                    pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, tokenDef.Kind, $"{childToken}{enumerator.Current}"));
                }
                else if (tokenDef.Type == TokenType.Single)
                {
                    pairChildren.AddRange(HandleSingleKeyword(queue, enumerator, tokenDef.Kind, $"{token}{enumerator.Current}"));
                }
            }
            else
            {
                queue.Enqueue(nextToken); // put it back on the stack.
                break;
            }
        }

        yield return new Pair(key, string.Empty, [.. pairChildren], ChildType.ArrayType);
    }

    private static string HandleMultipleKeywords(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        TokenDef tokenDef,
        string token
    )
    {
        if (tokenDef.MultipleKeywords is null)
        {
            throw new ArgumentNullException();
        }

        var key = token;
        foreach (var _ in tokenDef.MultipleKeywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                throw new InvalidOperationException();
            }

            if (!tokenDef.MultipleKeywords.Contains(nextItem))
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
        TokenKind kind,
        string token
    )
    {
        if (kind == TokenKind.NewLine)
        {
            // grab tokens until newline.
            var value = string.Empty;
            while (Helpers.TryGetValue(enumerator, queue, out var nextItem) && nextItem != Helpers.NewLine)
            {
                value += $" {nextItem}";
            }

            // last item is a newline, which we can ignore,
            yield return new Pair(token, value.Trim(), []);
        }
        else if (kind == TokenKind.Next)
        {
            if (Helpers.TryGetValue(enumerator, queue, out var value))
            {
                yield return new Pair(token, value, []);
            }
        }
        else if (kind == TokenKind.Single)
        {
            yield return new Pair(token, "true", []);
        }
        else if (kind == TokenKind.Options)
        {
            var value = string.Empty;
            while (Helpers.TryGetValue(enumerator, queue, out var nextItem) && nextItem != Helpers.NewLine)
            {
                value += $" {nextItem}";
            }

            var items = value.Split('(');
            var optionList = Array.Empty<Pair>();

            if (items.Length > 1)
            {
                optionList = items[1].Trim(')').Split(',').Select(o => new Pair($"{token} item", o, [])).ToArray();
            }

            var pairChildren = new []
            {
                new Pair("Bits", items[0], []),
                new Pair("Values", string.Empty, optionList, ChildType.ArrayType),
            };

            yield return new Pair(token, string.Empty, [.. pairChildren], ChildType.ObjectType);
        }
        else
        {
            throw new InvalidOperationException("Unknown Keyword Type");
        }
    }

    private static IEnumerable<string> Tokenize(TextReader output)
    {
        int? c;
        var sb = new StringBuilder();
        while ((c = output.Read()) != -1)
        {
            if (c == ':')
            {
                if (sb.Length == 0)
                {
                    continue;
                }

                yield return sb.ToString();
                sb.Clear();
                continue;
            }
            else if (c == '[')
            {
                yield return sb.ToString();
                sb.Clear();
                continue;
            }
            else if (c == ']')
            {
                yield return sb.ToString();
                sb.Clear();
                continue;
            }
            else if (!char.IsWhiteSpace((char)c))
            {
                sb.Append((char)c);
                continue;
            }
            else if (c == '\n')
            {
                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }

                yield return Helpers.NewLine;
                continue;
            }

            if (sb.Length == 0)
            {
                continue;
            }

            yield return sb.ToString();

            sb.Clear();
        }
    }

    #endregion
}