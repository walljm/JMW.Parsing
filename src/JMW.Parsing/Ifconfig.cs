using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json;
using ConsoleTableExt;

namespace JMW.Parsing;

public static class Ifconfig
{
    #region Public Parse/Output

    public static void Parse(TextReader inputReader, TextWriter outputWriter, DisplayOptions displayOptions)
    {
        if (displayOptions.OutputType == OutputType.KeyValue)
        {
            OutputKeyValues(inputReader, outputWriter);
        }
        else if (displayOptions.OutputType == OutputType.Json)
        {
            OutputJson(inputReader, outputWriter);
        }
        else if (displayOptions.OutputType == OutputType.Table)
        {
            OutputTable(inputReader, outputWriter, displayOptions.Filter);
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

    public static void OutputTable(TextReader inputReader, TextWriter outputWriter, string? filter)
    {
        var blocks = Helpers.GetBlocks(inputReader, trimInitialWhitespace: true, trimEndingWhitespace: false);

        var columns = typeof(Ifc).GetProperties().Select(static o => o.Name).ToArray();
        var data = new List<Ifc>();
        foreach (var block in blocks)
        {
            outputWriter.WriteLine(); // empty line for a spacer

            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Tokenize(blockReader)).ToArray();

            var name = string.Empty;
            var status = string.Empty;
            var flags = string.Empty;
            var adminStatus = string.Empty;
            var operStatus = string.Empty;
            var index = -1;
            var type = string.Empty;
            var mac = string.Empty;
            var ip = string.Empty;
            var media = string.Empty;

            foreach (var pair in pairs)
            {
                if (pair.Key == "InterfaceName")
                {
                    name = pair.Value;
                }
                else if (pair.Key == "status")
                {
                    status = pair.Value;
                }
                else if (pair.Key == "flags")
                {
                    flags = string.Join(',', pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.Select(static o => o.Value) ?? []);
                    adminStatus =
                        pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.FirstOrDefault(static o => o.Value == "UP") is not null
                            ? "Up"
                            : "Down";
                    operStatus = pair.Children
                       .FirstOrDefault(static o => o.Key == "Values")
                      ?.Children
                       .FirstOrDefault(static o => o.Value == "RUNNING") is not null
                        ? "Up"
                        : "Down";
                }
                else if (pair.Key == "index")
                {
                    index = int.TryParse(pair.Value, out var idx) ? idx : -1;
                }
                else if (pair.Key == "type")
                {
                    type = pair.Value;
                }
                else if (pair.Key == "ether")
                {
                    mac = pair.Value;
                }
                else if (pair.Key == "inets")
                {
                    ip = string.Join(
                        ", ",
                        pair.Children.Select(
                            static o => string.Join(
                                    '/',
                                    o.Children.Select(
                                        static i =>
                                        {
                                            if (i.Key == "netmask")
                                            {
                                                return GetMaskLen(i.Value).ToString();
                                            }
                                            else if (i.Key == "inet")
                                            {
                                                return i.Value;
                                            }

                                            return string.Empty;
                                        }
                                    )
                                )
                               .TrimEnd('/')
                        )
                    );
                }
                else if (pair.Key == "media")
                {
                    media = pair.Value;
                }
                else
                {
                }
            }

            if (type.Length == 0)
            {
                if (flags.Contains("POINTOPOINT"))
                {
                    type = "Tunnel";
                }
                else if (name.StartsWith("bridge"))
                {
                    type = "Virtual Bridge";
                }
                else if (name.StartsWith("stf"))
                {
                    type = "6to4 Tunnel";
                }
                else if (flags.Contains("LOOPBACK"))
                {
                    type = "Loopback";
                }
            }

            data.Add(new (name, status, flags, adminStatus, operStatus, index, type, mac, ip, media));
        }

        if (filter is not null)
        {
            data = data.Where(
                    o => o.Status.Contains(filter, StringComparison.OrdinalIgnoreCase)
                      || o.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                      || o.IP.Contains(filter, StringComparison.OrdinalIgnoreCase)
                      || o.MAC.Contains(filter, StringComparison.OrdinalIgnoreCase)
                      || o.AdminStatus.Contains(filter, StringComparison.OrdinalIgnoreCase)
                      || o.OperStatus.Contains(filter, StringComparison.OrdinalIgnoreCase)
                )
               .ToList();
        }

        ConsoleTableBuilder
           .From(data.OrderBy(static o => o.Index).ThenBy(static o => o.Name).ToList())
           .WithColumn(columns)
           .WithFormat(ConsoleTableBuilderFormat.MarkDown)
           .ExportAndWriteLine();
    }

    private static int GetMaskLen(string ip)
    {
        var cidr = 0;
        if (IPAddress.TryParse(ip, out var ipAddress))
        {
            var bits = new BitArray(ipAddress.GetAddressBytes());
            for (var i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                {
                    cidr++;
                }
            }
        }

        return cidr;
    }

    private record Ifc(
        string Name,
        string Status,
        string Flags,
        string AdminStatus,
        string OperStatus,
        int Index,
        string Type,
        string MAC,
        string IP,
        string Media
    );

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

    private record GroupDefinition(
        string Kind,
        Dictionary<string, string> Keywords,
        Dictionary<string, MultipleDefinition> MultipleKeywords
    );

    private record MultipleDefinition(
        string Kind,
        Dictionary<string, string> Keywords
    );

    private static readonly HashSet<string> arrayPairs =
    [
        "inet",
        "inet6",
        "agent",
        "member",
    ];

    private static readonly HashSet<string> mergePairs =
    [
        "RX",
        "TX",
    ];

    private const string NewLine = "<newline>";
    private const string Next = "<next>";
    private const string Single = "<single>";
    private const string Options = "<options>";
    private const string Group = "<group>";
    private const string GroupNext = "<groupnext>";
    private const string Drop = "<drop>";

    private static readonly Dictionary<string, string> singleKeywords = new()
    {
        { "flags", Options },
        { "eflags", Options },
        { "xflags", Options },
        { "hwassist", Options },
        { "mtu", Next },
        { "ether", Next },
        { "media", NewLine },
        { "status", Next },
        { "priority", Next },
        { "type", NewLine },
        { "desc", NewLine },
        { "scheduler", Next },
        { "routermode4", Next },
        { "routermode6", Next },
        { "netif", Next },
        { "flowswitch", Next },
        { "options", Options },
        { "index", Next },
        { "txqueuelen", Next },
        { "unspec", Next },
        { "loop", Single },
    };

    private static readonly Dictionary<string, GroupDefinition> groupKeywords = new()
    {
        {
            "Configuration", new GroupDefinition(
                NewLine,
                new Dictionary<string, string>
                {
                    { "id", Next },
                    { "priority", Next },
                    { "hellotime", Next },
                    { "fwddelay", Next },
                    { "maxage", Next },
                    { "holdcnt", Next },
                    { "proto", Next },
                    { "maxaddr", Next },
                    { "timeout", Next },
                    { "ifcost", Next },
                    { "port", Next },
                    { "ipfilter", Next },
                    { "flags", Next },
                    { "root", Group },
                },
                []
            )
        },

        {
            "member", new GroupDefinition(
                GroupNext,
                new Dictionary<string, string>
                {
                    { "flags", Options },
                    { "ifmaxaddr", Next },
                    { "port", Next },
                    { "priority", Next },
                    { "hostfilter", Next },
                    { "hw", Next },
                    { "ip", Next },
                },
                new Dictionary<string, MultipleDefinition>
                {
                    { "path", new MultipleDefinition(Next, new Dictionary<string, string> { { "cost", Next } }) },
                }
            )
        },

        {
            "agent", new GroupDefinition(
                Group,
                new Dictionary<string, string>
                {
                    { "domain", Next },
                    { "type", Next },
                    { "flags", Next },
                    { "desc", NewLine },
                },
                []
            )
        },

        {
            "root", new GroupDefinition(
                Group,
                new Dictionary<string, string>
                {
                    { "id", Next },
                    { "priority", Next },
                    { "ifcost", Next },
                    { "port", Next },
                    { "ipfilter", Next },
                    { "flags", Options },
                    { "member", Group },
                },
                []
            )
        },

        {
            "inet6", new GroupDefinition(
                GroupNext,
                new Dictionary<string, string>
                {
                    { "prefixlen", Next },
                    { "scopeid", Next },
                    { "secured", Single },
                },
                []
            )
        },
        {
            "inet", new GroupDefinition(
                GroupNext,
                new Dictionary<string, string>
                {
                    { "netmask", Next },
                    { "broadcast", Next },
                },
                []
            )
        },
        {
            "RX", new GroupDefinition(
                Group,
                new Dictionary<string, string>
                {
                    { "packets", Next },
                    { "errors", Next },
                    { "dropped", Next },
                    { "overruns", Next },
                    { "carrier", Next },
                    { "frame", Next },
                    { "collisions", Next },
                    { "bytes", NewLine },
                },
                []
            )
        },
        {
            "TX", new GroupDefinition(
                Group,
                new Dictionary<string, string>
                {
                    { "packets", Next },
                    { "errors", Next },
                    { "dropped", Next },
                    { "overruns", Next },
                    { "carrier", Next },
                    { "frame", Next },
                    { "collisions", Next },
                    { "bytes", NewLine },
                },
                []
            )
        },
    };

    private static readonly Dictionary<string, MultipleDefinition> multipleKeywords = new()
    {
        { "nd6", new MultipleDefinition(Options, new Dictionary<string, string> { { "options", Options } }) },
        { "root", new MultipleDefinition(Next, new Dictionary<string, string> { { "id", Next } }) },
        { "path", new MultipleDefinition(Next, new Dictionary<string, string> { { "cost", Next } }) },
        { "state", new MultipleDefinition(Next, new Dictionary<string, string> { { "availability", Next } }) },
        { "qosmarking", new MultipleDefinition(Next, new Dictionary<string, string> { { "enabled", Next } }) },
        { "generation", new MultipleDefinition(Next, new Dictionary<string, string> { { "id", Next } }) },
        { "uplink", new MultipleDefinition(Next, new Dictionary<string, string> { { "rate", Next } }) },
        { "downlink", new MultipleDefinition(Next, new Dictionary<string, string> { { "rate", Next } }) },
        { "low", new MultipleDefinition(Next, new Dictionary<string, string> { { "power", Next }, { "mode", Next } }) },
        {
            "link",
            new MultipleDefinition(Next, new Dictionary<string, string> { { "rate", Next }, { "quality", Next } })
        },
        {
            "multi",
            new MultipleDefinition(
                Next,
                new Dictionary<string, string>
                    { { "layer", Next }, { "packet", Next }, { "logging", Next }, { "(mpklog)", Drop } }
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
            if (groupKeywords.TryGetValue(token, out var groupKeyword))
            {
                if (arrayPairs.Contains(token))
                {
                    foreach (var item in HandleArrayKeywords(queue, enumerator, token, groupKeyword))
                    {
                        yield return item;
                    }
                }
                else if (mergePairs.Contains(token))
                {
                    var children = HandleArrayKeywords(queue, enumerator, token, groupKeyword)
                       .SelectMany(static o => o.Children.SelectMany(static c => c.Children))
                       .ToArray();

                    // merge properties
                    yield return new Pair(token, string.Empty, children, ChildType.ObjectType);
                }
                else
                {
                    foreach (var item in HandleGroups(queue, enumerator, token, groupKeyword))
                    {
                        yield return item;
                    }
                }
            }
            else if (multipleKeywords.ContainsKey(token))
            {
                foreach (var item in HandleMultipleKeywords(queue, enumerator, multipleKeywords, token, token))
                {
                    yield return item;
                }
            }
            else if (singleKeywords.TryGetValue(token, out var kind))
            {
                if (token is null)
                {
                    throw new InvalidOperationException("Token can't be null");
                }

                foreach (var item in HandleSingleKeyword(queue, enumerator, kind, token))
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
        GroupDefinition groupDefinition
    )
    {
        var key = token;
        var pairChildren = new List<Pair>();
        var value = string.Empty;

        if (groupDefinition.Kind == GroupNext)
        {
            pairChildren.AddRange(
                HandleSingleKeyword(queue, enumerator, Next, token)
            );
        }

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (groupDefinition.Keywords.TryGetValue(nextToken, out var kind))
            {
                if (groupKeywords.TryGetValue(nextToken, out var childGroup))
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
        string kind,
        string token
    )
    {
        if (kind == NewLine)
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
        else if (kind == Next)
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
        else if (kind == Single)
        {
            yield return new Pair(token, "true", []);
        }
        else if (kind == Options)
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