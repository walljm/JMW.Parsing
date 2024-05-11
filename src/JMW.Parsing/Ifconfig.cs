using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using JMW.Parsing.Models;
using VAE.Common.Parsing.Tables;

namespace JMW.Parsing;

public static class Ifconfig
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

        foreach (var block in blocks)
        {
            outputWriter.WriteLine(); // empty line for a spacer

            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Tokenize(blockReader));

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
            Indented = true
        };

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

        writer.WriteStartArray();

        foreach (var block in blocks)
        {
            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Tokenize(blockReader)).ToArray();
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
        MultipleGroup
    }

    private record TokenDef(
        TokenKind Kind,
        TokenType Type,
        HashSet<string>? MultipleKeywords = default,
        Dictionary<string, TokenDef>? GroupKeywords = default,
        bool IsArray = false,
        bool ShouldMerge = false
    );

    private static readonly Dictionary<string, TokenDef> Tokens = new()
    {
        { "flags", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "eflags", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "xflags", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "hwassist", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "mtu", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "ether", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "media", new TokenDef(TokenKind.NewLine, TokenType.Single) },
        { "status", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "priority", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "type", new TokenDef(TokenKind.NewLine, TokenType.Single) },
        { "desc", new TokenDef(TokenKind.NewLine, TokenType.Single) },
        { "scheduler", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "routermode4", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "routermode6", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "netif", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "flowswitch", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "options", new TokenDef(TokenKind.Options, TokenType.Single) },
        { "index", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "txqueuelen", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "unspec", new TokenDef(TokenKind.Next, TokenType.Single) },
        { "loop", new TokenDef(TokenKind.Single, TokenType.Single) },

        // Multiple
        { "nd6", new TokenDef( TokenKind.Options, TokenType.Multiple,  [ "options" ]) },
        { "root", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "id" ]) },
        { "path", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "cost" ]) },
        { "state", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "availability" ]) },
        { "qosmarking", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "enabled" ]) },
        { "generation", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "id" ]) },
        { "uplink", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "rate" ]) },
        { "downlink", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "rate" ]) },
        { "low", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "power", "mode" ]) },
        { "link", new TokenDef( TokenKind.Next, TokenType.Multiple,  [ "rate", "quality" ]) },
        { "multi", new TokenDef( TokenKind.Next, TokenType.Multiple, [ "layer", "packet", "logging", "(mpklog)" ]) },

        // groups
        {
            "Configuration:",
            new TokenDef(
                TokenKind.NewLine,
                TokenType.Group,
                [],
                new ()
                {
                    { "id", new (TokenKind.Next, TokenType.Single) },
                    { "priority", new (TokenKind.Next, TokenType.Single) },
                    { "hellotime", new (TokenKind.Next, TokenType.Single) },
                    { "fwddelay", new (TokenKind.Next, TokenType.Single) },
                    { "maxage", new (TokenKind.Next, TokenType.Single) },
                    { "holdcnt", new (TokenKind.Next, TokenType.Single) },
                    { "proto", new (TokenKind.Next, TokenType.Single) },
                    { "maxaddr", new (TokenKind.Next, TokenType.Single) },
                    { "timeout", new (TokenKind.Next, TokenType.Single) },
                    { "ifcost", new (TokenKind.Next, TokenType.Single) },
                    { "port", new (TokenKind.Next, TokenType.Single) },
                    { "ipfilter", new (TokenKind.Next, TokenType.Single) },
                    { "flags", new (TokenKind.Next, TokenType.Single) },
                    { "root", new (TokenKind.Single, TokenType.Group) },
                },
                false,
                false
            )
        },
        {
            "member",
            new TokenDef(
                TokenKind.Next,
                TokenType.MultipleGroup,
                [
                    "path", "cost",
                ],
                new ()
                {
                    { "flags", new (TokenKind.Options, TokenType.Single) },
                    { "ifmaxaddr", new (TokenKind.Next, TokenType.Single) },
                    { "port", new (TokenKind.Next, TokenType.Single) },
                    { "priority", new (TokenKind.Next, TokenType.Single) },
                    { "hostfilter", new (TokenKind.Next, TokenType.Single) },
                    { "hw", new (TokenKind.Next, TokenType.Single) },
                    { "ip", new (TokenKind.Next, TokenType.Single) },
                },
                true,
                false
            )
        },
        {
            "agent",
            new TokenDef(
                TokenKind.Single,
                TokenType.Group,
                [],
                new ()
                {
                    { "domain", new (TokenKind.Next, TokenType.Single) },
                    { "type", new (TokenKind.Next, TokenType.Single) },
                    { "flags", new (TokenKind.Next, TokenType.Single) },
                    { "desc", new (TokenKind.NewLine, TokenType.Single) },
                },
                true,
                false
            )
        },
        {
            "root",
            new TokenDef(
                TokenKind.Single,
                TokenType.Group,
                [],
                new ()
                {
                    { "id", new(TokenKind.Next, TokenType.Single) },
                    { "priority", new(TokenKind.Next, TokenType.Single) },
                    { "ifcost", new(TokenKind.Next, TokenType.Single) },
                    { "port", new(TokenKind.Next, TokenType.Single) },
                    { "ipfilter", new(TokenKind.Next, TokenType.Single) },
                    { "flags", new(TokenKind.Options, TokenType.Single) },
                    { "member", new(TokenKind.Single, TokenType.Group) },
                },
                false,
                false
            )
        },
        {
            "inet6",
            new TokenDef(
                TokenKind.Next,
                TokenType.Group,
                [],
                new ()
                {
                    { "prefixlen", new(TokenKind.Next, TokenType.Single) },
                    { "scopeid", new(TokenKind.Next, TokenType.Single) },
                    { "secured", new(TokenKind.Single, TokenType.Single) },
                },
                true,
                false
            )
        },
        {
            "inet",
            new TokenDef(
                TokenKind.Next,
                TokenType.Group,
                [],
                new ()
                {
                    { "netmask", new(TokenKind.Next, TokenType.Single) },
                    { "broadcast", new(TokenKind.Next, TokenType.Single) },
                },
                true,
                false
            )
        },
        {
            "RX",
            new TokenDef(
                TokenKind.Single,
                TokenType.Group,
                [],
                new ()
                {
                    { "packets", new(TokenKind.Next, TokenType.Single) },
                    { "errors", new(TokenKind.Next, TokenType.Single) },
                    { "dropped", new(TokenKind.Next, TokenType.Single) },
                    { "overruns", new(TokenKind.Next, TokenType.Single) },
                    { "carrier", new(TokenKind.Next, TokenType.Single) },
                    { "frame", new(TokenKind.Next, TokenType.Single) },
                    { "collisions", new(TokenKind.Next, TokenType.Single) },
                    { "bytes", new(TokenKind.NewLine, TokenType.Single) },
                },
                false,
                true
            )
        },
        {
            "TX",
            new TokenDef(
                TokenKind.Single,
                TokenType.Group,
                [],
                new ()
                {
                    { "packets", new(TokenKind.Next, TokenType.Single) },
                    { "errors", new(TokenKind.Next, TokenType.Single) },
                    { "dropped", new(TokenKind.Next, TokenType.Single) },
                    { "overruns", new(TokenKind.Next, TokenType.Single) },
                    { "carrier", new(TokenKind.Next, TokenType.Single) },
                    { "frame", new(TokenKind.Next, TokenType.Single) },
                    { "collisions", new(TokenKind.Next, TokenType.Single) },
                    { "bytes", new(TokenKind.NewLine, TokenType.Single) },
                },
                false,
                true
            )
        },
    };

    #endregion

    #region Private Functions

    private static IEnumerable<Pair> GetPairs(IEnumerable<string> tokens)
    {
        var queue = new Queue<string>();
        using var enumerator = tokens.GetEnumerator();
        var hasLine = enumerator.MoveNext();
        if (!hasLine)
        {
            yield break;
        }

        // first token should always be the interface name.
        yield return new Pair("InterfaceName", enumerator.Current, []);

        while (Helpers.TryGetValue(enumerator, queue, out var token))
        {
            if (!Tokens.TryGetValue(token, out var tokenDef))
            {
                continue;
            }

            if (tokenDef.Type == TokenType.Group)
            {
                if (tokenDef.IsArray)
                {
                    foreach (var item in HandleArrayKeywords(queue, enumerator, token, tokenDef))
                    {
                        yield return item;
                    }
                }
                else if (tokenDef.ShouldMerge)
                {
                    var children = HandleArrayKeywords(queue, enumerator, token, tokenDef)
                       .SelectMany(o => o.Children.SelectMany(c => c.Children))
                       .ToArray();

                    // merge properties
                    yield return new Pair(token, string.Empty, children, ChildType.ObjectType);
                }
                else
                {
                    foreach (var item in HandleGroups(queue, enumerator, token, tokenDef))
                    {
                        yield return item;
                    }
                }
            }
            else if (tokenDef.Type == TokenType.Multiple)
            {
                foreach (var item in HandleMultipleKeywords(queue, enumerator, token, tokenDef))
                {
                    yield return item;
                }
            }
            else if (tokenDef.Type == TokenType.Single)
            {
                if (token is null) throw new InvalidOperationException("Token can't be null");
                foreach (var item in HandleSingleKeyword(queue, enumerator, token, tokenDef))
                {
                    yield return item;
                }
            }
        }
    }

    private static IEnumerable<Pair> HandleGroups(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        string token,
        TokenDef tokenDef
    )
    {
        var key = token;
        var pairChildren = new List<Pair>();
        var value = string.Empty;
        if (tokenDef.Kind == TokenKind.Next)
        {
            pairChildren.AddRange(
                HandleSingleKeyword(queue, enumerator, TokenKind.Next, token)
            );
        }

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (tokenDef.GroupKeywords.TryGetValue(nextToken, out var kind))
            {
                if (tokenDef.GroupKeywords.TryGetValue(nextToken, out var childGroup))
                {
                    if (arrayPairs.Contains(nextToken))
                    {
                        pairChildren.AddRange(HandleArrayKeywords(queue, enumerator, nextToken, childGroup));
                    }
                    else
                    {
                        pairChildren.AddRange(HandleGroups(queue, enumerator, nextToken, childGroup));
                    }
                }
                else
                {
                    pairChildren.AddRange(
                        HandleSingleKeyword(queue, enumerator, kind, nextToken)
                    );
                }
            }
            else if (groupDefinition.MultipleKeywords.ContainsKey(nextToken))
            {
                pairChildren.AddRange(
                    HandleMultipleKeywords(queue, enumerator, groupDefinition.MultipleKeywords, nextToken, nextToken)
                );
            }
            else if (nextToken == NewLine)
            {
                continue;
            }
            else
            {
                queue.Enqueue(nextToken); // put it back on the stack.
                break;
            }
        }

        yield return new Pair(key, value, [.. pairChildren], ChildType.ObjectType);
    }

    private static IEnumerable<Pair> HandleArrayKeywords(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        string token,
        GroupDefinition groupDefinition
    )
    {
        var key = $"{token}s";
        var pairChildren = new List<Pair>();
        var value = string.Empty;
        pairChildren.AddRange(HandleGroups(queue, enumerator, token, groupDefinition));

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == token)
            {
                pairChildren.AddRange(HandleGroups(queue, enumerator, token, groupDefinition));
            }
            else
            {
                queue.Enqueue(nextToken); // put it back on the stack.
                break;
            }
        }

        yield return new Pair(key, value, [.. pairChildren], ChildType.ArrayType);
    }

    private static IEnumerable<Pair> HandleMultipleKeywords(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        Dictionary<string, MultipleDefinition> definition,
        string token,
        string key
    )
    {
        var escape = false;
        foreach (var item in definition[token].Keywords)
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var nextItem))
            {
                yield break;
            }

            if (!definition[token].Keywords.ContainsKey(nextItem))
            {
                queue.Enqueue(nextItem);
                escape = true;
                break;
            }

            if (item.Value != Drop)
            {
                key += $" {nextItem}";
            }
        }

        if (escape)
        {
            yield break;
        }

        foreach (var pair in HandleSingleKeyword(queue, enumerator, definition[token].Kind, key))
        {
            yield return pair;
        }
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
            while (Helpers.TryGetValue(enumerator, queue, out var nextItem) && nextItem != NewLine)
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
                if (token == "inet6")
                {
                    var items = value.Split('%');
                    yield return new Pair(token, items[0], []);
                    if (items.Length > 1)
                    {
                        yield return new Pair("interface", items[1], []);
                    }
                }
                else
                {
                    yield return new Pair(token, value, []);
                }
            }
        }
        else if (kind == TokenKind.Single)
        {
            yield return new Pair(token, "true", []);
        }
        else if (kind == TokenKind.Options)
        {
            if (Helpers.TryGetValue(enumerator, queue, out var value))
            {
                var items = value.Split('<');
                var optionList = Array.Empty<Pair>();
                if (items.Length > 1)
                {
                    optionList = items[1].Trim('>').Split(',').Select(o => new Pair($"{token} item", o, [])).ToArray();
                }

                var pairChildren = new []
                {
                    new Pair("Bits", items[0], []),
                    new Pair("Values", string.Empty, optionList, ChildType.ArrayType),
                };
                yield return new Pair(token, string.Empty, [.. pairChildren], ChildType.ObjectType);
            }
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
            if (c == '=')
            {
                if (sb[^1] == ':')
                {
                    sb.Remove(sb.Length - 1, 1);
                }

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

                yield return NewLine;
                continue;
            }

            if (sb.Length == 0)
            {
                continue;
            }

            if (sb[^1] == ':')
            {
                sb.Remove(sb.Length - 1, 1);
            }

            // sometimes the key:value pairs are separated
            //  by a single colon.  split these.
            var colonCount = Helpers.CountChars(sb, ':');
            if (colonCount[':'] == 1)
            {
                var tokens = sb.ToString().Split(':');
                yield return tokens[0];
                yield return tokens[1];
            }
            else
            {
                yield return sb.ToString();
            }

            sb.Clear();
        }
    }

    #endregion
}