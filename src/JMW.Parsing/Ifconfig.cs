using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using table.lib;

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
            OutputTable(inputReader, outputWriter, displayOptions);
        }
    }

    public static void OutputKeyValues(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteKeyValues(GetBlockPairs(inputReader), outputWriter);
    }

    public static void OutputTable(TextReader inputReader, TextWriter outputWriter, DisplayOptions displayOptions)
    {
        #region Convert blocks to pocos

        var data = new List<Ifc>();
        foreach (var pairs in GetBlockPairs(inputReader))
        {

            var ifc = new Ifc();

            foreach (var pair in pairs)
            {
                if (pair.Key == "InterfaceName")
                {
                    ifc.Name = pair.Value;
                }
                else if (pair.Key == "status")
                {
                    ifc.Status = pair.Value;
                }
                else if (pair.Key == "flags")
                {
                    ifc.Flags = string.Join(
                        ',',
                        pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.Select(static o => o.Value) ?? []
                    );
                    ifc.AdminStatus =
                        pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.FirstOrDefault(static o => o.Value == "UP") is not null
                            ? "Up"
                            : "Down";
                    ifc.OperStatus = pair.Children
                       .FirstOrDefault(static o => o.Key == "Values")
                      ?.Children
                       .FirstOrDefault(static o => o.Value == "RUNNING") is not null
                        ? "Up"
                        : "Down";
                }
                else if (pair.Key == "index")
                {
                    ifc.Index = int.TryParse(pair.Value, out var idx) ? idx : -1;
                }
                else if (pair.Key == "type")
                {
                    ifc.Type = pair.Value;
                }
                else if (pair.Key == "ether")
                {
                    ifc.MAC = pair.Value;
                }
                else if (pair.Key == "inets")
                {
                    ifc.IP = string.Join(
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
                    ifc.Media = pair.Value;
                }
            }

            if (ifc.Type.Length == 0)
            {
                if (ifc.Flags.Contains("POINTOPOINT") )
                {
                    ifc.Type = "Tunnel";
                }
                else if (ifc.Name.StartsWith("bridge"))
                {
                    ifc.Type = "Virtual Bridge";
                }
                else if (ifc.Name.StartsWith("stf") )
                {
                    ifc.Type = "6to4 Tunnel";
                }
                else if (ifc.Flags.Contains("LOOPBACK"))
                {
                    ifc.Type = "Loopback";
                }
            }

            data.Add(ifc);
        }

        #endregion

        #region Filter data and handle display width

        if (displayOptions.Filter is not null)
        {
            Regex regex;
            try
            {
                var opts = RegexOptions.IgnoreCase | RegexOptions.NonBacktracking | RegexOptions.CultureInvariant;
                regex = new Regex(displayOptions.Filter, opts);
            }
            catch (RegexParseException ex)
            {
                Console.Error.WriteLine($"Invalid filter regex: {ex.Message}");
                return;
            }

            data = data.Where(
                    o => regex.IsMatch(o.Status)
                      || regex.IsMatch(o.Name)
                      || regex.IsMatch(o.IP)
                      || regex.IsMatch(o.MAC)
                      || regex.IsMatch(o.AdminStatus)
                      || regex.IsMatch(o.OperStatus)
                )
               .ToList();
        }

        data = data.OrderBy(static o => o.Index).ThenBy(static o => o.Name).ToList();
        var columns = new List<string>
        {
            nameof(Ifc.Name),
            nameof(Ifc.Status),
            nameof(Ifc.Flags),
            nameof(Ifc.AdminStatus),
            nameof(Ifc.OperStatus),
            nameof(Ifc.Index),
            nameof(Ifc.Type),
            nameof(Ifc.MAC),
            nameof(Ifc.IP),
            nameof(Ifc.Media),
        };

        if (displayOptions.ConsoleWidth < 225)
        {
            columns.Remove(nameof(Ifc.Media));
        }

        if (displayOptions.ConsoleWidth < 180)
        {
            columns.Remove(nameof(Ifc.Flags));
        }

        if (displayOptions.ConsoleWidth < 120)
        {
            columns.Remove(nameof(Ifc.Index));
        }

        if (displayOptions.ConsoleWidth < 100)
        {
            columns.Remove(nameof(Ifc.AdminStatus));
        }

        #endregion

        var tbl = Table<Ifc>.Add(data);
        tbl.FilterColumns(columns.ToArray(), FilterAction.Include);
        var result = tbl.ToMarkDown();
        outputWriter.Write(result);
    }

    private static int GetMaskLen(string mask)
    {
        byte[] bytes;

        if (mask.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
         && uint.TryParse(mask.AsSpan(2), NumberStyles.HexNumber, null, out var hex))
        {
            // macOS hex format: 0xffffff00
            bytes =
            [
                (byte)(hex >> 24),
                (byte)(hex >> 16),
                (byte)(hex >> 8),
                (byte)hex,
            ];
        }
        else if (IPAddress.TryParse(mask, out var ipAddress))
        {
            // Linux decimal format: 255.255.255.0
            bytes = ipAddress.GetAddressBytes();
        }
        else
        {
            return 0;
        }

        var cidr = 0;
        foreach (var b in bytes)
        {
            for (var bit = 7; bit >= 0; bit--)
            {
                if ((b & (1 << bit)) != 0)
                {
                    cidr++;
                }
                else
                {
                    return cidr;
                }
            }
        }

        return cidr;
    }

    private class Ifc
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Flags { get; set; } = string.Empty;
        public string AdminStatus { get; set; } = string.Empty;
        public string OperStatus { get; set; } = string.Empty;
        public int Index { get; set; } = -1;
        public string Type { get; set; } = string.Empty;
        public string MAC { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Media { get; set; } = string.Empty;
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteJson(GetBlockPairs(inputReader), outputWriter);
    }

    private static IEnumerable<IReadOnlyList<Pair>> GetBlockPairs(TextReader inputReader)
    {
        var blocks = Helpers.GetBlocks(inputReader, trimInitialWhitespace: true, trimEndingWhitespace: false);
        foreach (var block in blocks)
        {
            using var blockReader = new StringReader(block);
            yield return GetPairs(Tokenize(blockReader)).ToArray();
        }
    }

    #endregion

    #region Classes and Constants

    private record GroupDefinition(
        KeywordKind Kind,
        Dictionary<string, KeywordKind> Keywords,
        Dictionary<string, MultipleDefinition> MultipleKeywords
    );

    private record MultipleDefinition(
        KeywordKind Kind,
        Dictionary<string, KeywordKind> Keywords
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

    private static readonly Dictionary<string, KeywordKind> singleKeywords = new()
    {
        { "flags", KeywordKind.Options },
        { "eflags", KeywordKind.Options },
        { "xflags", KeywordKind.Options },
        { "hwassist", KeywordKind.Options },
        { "mtu", KeywordKind.Next },
        { "ether", KeywordKind.Next },
        { "media", KeywordKind.NewLine },
        { "status", KeywordKind.Next },
        { "priority", KeywordKind.Next },
        { "type", KeywordKind.NewLine },
        { "desc", KeywordKind.NewLine },
        { "scheduler", KeywordKind.Next },
        { "routermode4", KeywordKind.Next },
        { "routermode6", KeywordKind.Next },
        { "netif", KeywordKind.Next },
        { "flowswitch", KeywordKind.Next },
        { "options", KeywordKind.Options },
        { "index", KeywordKind.Next },
        { "txqueuelen", KeywordKind.Next },
        { "unspec", KeywordKind.Next },
        { "loop", KeywordKind.Single },
    };

    private static readonly Dictionary<string, GroupDefinition> groupKeywords = new()
    {
        {
            "Configuration", new GroupDefinition(
                KeywordKind.NewLine,
                new Dictionary<string, KeywordKind>
                {
                    { "id", KeywordKind.Next },
                    { "priority", KeywordKind.Next },
                    { "hellotime", KeywordKind.Next },
                    { "fwddelay", KeywordKind.Next },
                    { "maxage", KeywordKind.Next },
                    { "holdcnt", KeywordKind.Next },
                    { "proto", KeywordKind.Next },
                    { "maxaddr", KeywordKind.Next },
                    { "timeout", KeywordKind.Next },
                    { "ifcost", KeywordKind.Next },
                    { "port", KeywordKind.Next },
                    { "ipfilter", KeywordKind.Next },
                    { "flags", KeywordKind.Next },
                    { "root", KeywordKind.Group },
                },
                []
            )
        },

        {
            "member", new GroupDefinition(
                KeywordKind.GroupNext,
                new Dictionary<string, KeywordKind>
                {
                    { "flags", KeywordKind.Options },
                    { "ifmaxaddr", KeywordKind.Next },
                    { "port", KeywordKind.Next },
                    { "priority", KeywordKind.Next },
                    { "hostfilter", KeywordKind.Next },
                    { "hw", KeywordKind.Next },
                    { "ip", KeywordKind.Next },
                },
                new Dictionary<string, MultipleDefinition>
                {
                    { "path", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "cost", KeywordKind.Next } }) },
                }
            )
        },

        {
            "agent", new GroupDefinition(
                KeywordKind.Group,
                new Dictionary<string, KeywordKind>
                {
                    { "domain", KeywordKind.Next },
                    { "type", KeywordKind.Next },
                    { "flags", KeywordKind.Next },
                    { "desc", KeywordKind.NewLine },
                },
                []
            )
        },

        {
            "root", new GroupDefinition(
                KeywordKind.Group,
                new Dictionary<string, KeywordKind>
                {
                    { "id", KeywordKind.Next },
                    { "priority", KeywordKind.Next },
                    { "ifcost", KeywordKind.Next },
                    { "port", KeywordKind.Next },
                    { "ipfilter", KeywordKind.Next },
                    { "flags", KeywordKind.Options },
                    { "member", KeywordKind.Group },
                },
                []
            )
        },

        {
            "inet6", new GroupDefinition(
                KeywordKind.GroupNext,
                new Dictionary<string, KeywordKind>
                {
                    { "prefixlen", KeywordKind.Next },
                    { "scopeid", KeywordKind.Next },
                    { "secured", KeywordKind.Single },
                },
                []
            )
        },
        {
            "inet", new GroupDefinition(
                KeywordKind.GroupNext,
                new Dictionary<string, KeywordKind>
                {
                    { "netmask", KeywordKind.Next },
                    { "broadcast", KeywordKind.Next },
                },
                []
            )
        },
        {
            "RX", new GroupDefinition(
                KeywordKind.Group,
                new Dictionary<string, KeywordKind>
                {
                    { "packets", KeywordKind.Next },
                    { "errors", KeywordKind.Next },
                    { "dropped", KeywordKind.Next },
                    { "overruns", KeywordKind.Next },
                    { "carrier", KeywordKind.Next },
                    { "frame", KeywordKind.Next },
                    { "collisions", KeywordKind.Next },
                    { "bytes", KeywordKind.NewLine },
                },
                []
            )
        },
        {
            "TX", new GroupDefinition(
                KeywordKind.Group,
                new Dictionary<string, KeywordKind>
                {
                    { "packets", KeywordKind.Next },
                    { "errors", KeywordKind.Next },
                    { "dropped", KeywordKind.Next },
                    { "overruns", KeywordKind.Next },
                    { "carrier", KeywordKind.Next },
                    { "frame", KeywordKind.Next },
                    { "collisions", KeywordKind.Next },
                    { "bytes", KeywordKind.NewLine },
                },
                []
            )
        },
    };

    private static readonly Dictionary<string, MultipleDefinition> multipleKeywords = new()
    {
        { "nd6", new MultipleDefinition(KeywordKind.Options, new Dictionary<string, KeywordKind> { { "options", KeywordKind.Options } }) },
        { "root", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "id", KeywordKind.Next } }) },
        { "path", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "cost", KeywordKind.Next } }) },
        { "state", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "availability", KeywordKind.Next } }) },
        { "qosmarking", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "enabled", KeywordKind.Next } }) },
        { "generation", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "id", KeywordKind.Next } }) },
        { "uplink", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "rate", KeywordKind.Next } }) },
        { "downlink", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "rate", KeywordKind.Next } }) },
        { "low", new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "power", KeywordKind.Next }, { "mode", KeywordKind.Next } }) },
        {
            "link",
            new MultipleDefinition(KeywordKind.Next, new Dictionary<string, KeywordKind> { { "rate", KeywordKind.Next }, { "quality", KeywordKind.Next } })
        },
        {
            "multi",
            new MultipleDefinition(
                KeywordKind.Next,
                new Dictionary<string, KeywordKind>
                    { { "layer", KeywordKind.Next }, { "packet", KeywordKind.Next }, { "logging", KeywordKind.Next }, { "(mpklog)", KeywordKind.Drop } }
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

        if (groupDefinition.Kind == KeywordKind.GroupNext)
        {
            pairChildren.AddRange(
                HandleSingleKeyword(queue, enumerator, KeywordKind.Next, token)
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
            else if (nextToken == Helpers.NewLine)
            {
                continue;
            }
            else
            {
                queue.Enqueue(nextToken); // put it back on the stack.
                break;
            }
        }

        yield return new Pair(key, value, pairChildren, ChildType.ObjectType);
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

        yield return new Pair(key, value, pairChildren, ChildType.ArrayType);
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

            if (item.Value != KeywordKind.Drop)
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
        KeywordKind kind,
        string token
    )
    {
        // Special inet6 handling: split interface from address
        if (kind == KeywordKind.Next && token == "inet6")
        {
            if (Helpers.TryGetValue(enumerator, queue, out var value))
            {
                var items = value.Split('%');
                yield return new Pair(token, items[0], []);
                if (items.Length > 1)
                {
                    yield return new Pair("interface", items[1], []);
                }
            }

            yield break;
        }

        foreach (var pair in Helpers.HandleSingleKeyword(queue, enumerator, kind, token, '<', '>'))
        {
            yield return pair;
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
            else if (!char.IsWhiteSpace((char)c))
            {
                sb.Append((char)c);
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