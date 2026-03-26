using System.Globalization;
using System.Net;
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
        PairWriter.WriteKeyValues(BlockParser.ParseBlocks(inputReader, Config), outputWriter);
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteJson(BlockParser.ParseBlocks(inputReader, Config), outputWriter);
    }

    #endregion

    #region Table Output (ifconfig-specific)

    public static void OutputTable(TextReader inputReader, TextWriter outputWriter, DisplayOptions displayOptions)
    {
        #region Convert blocks to pocos

        var data = new List<Ifc>();
        foreach (var pairs in BlockParser.ParseBlocks(inputReader, Config))
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

    #endregion

    #region Parser Configuration

    internal static readonly ParserConfig Config = new()
    {
        Tokenizer = new TokenizerConfig
        {
            Separators = ['='],
            StripTrailing = [':'],
            SplitOnSingleColon = true,
            EmitNewLineTokens = true,
        },

        OptionDelimiters = ('<', '>'),
        OptionsReadUntilNewLine = false,
        TrimInitialWhitespace = true,
        TrimEndingWhitespace = false,
        SupportsTableOutput = true,

        FirstTokenHandler = static (enumerator, _) =>
            [new Pair("InterfaceName", enumerator.Current, [])],

        Keywords = BuildKeywords(),
    };

    private static Dictionary<string, KeywordDef> BuildKeywords()
    {
        var keywords = new Dictionary<string, KeywordDef>();

        // Single keywords
        AddSingle(keywords, "flags", KeywordKind.Options);
        AddSingle(keywords, "eflags", KeywordKind.Options);
        AddSingle(keywords, "xflags", KeywordKind.Options);
        AddSingle(keywords, "hwassist", KeywordKind.Options);
        AddSingle(keywords, "mtu", KeywordKind.Next);
        AddSingle(keywords, "ether", KeywordKind.Next);
        AddSingle(keywords, "media", KeywordKind.NewLine);
        AddSingle(keywords, "status", KeywordKind.Next);
        AddSingle(keywords, "priority", KeywordKind.Next);
        AddSingle(keywords, "type", KeywordKind.NewLine);
        AddSingle(keywords, "desc", KeywordKind.NewLine);
        AddSingle(keywords, "scheduler", KeywordKind.Next);
        AddSingle(keywords, "routermode4", KeywordKind.Next);
        AddSingle(keywords, "routermode6", KeywordKind.Next);
        AddSingle(keywords, "netif", KeywordKind.Next);
        AddSingle(keywords, "flowswitch", KeywordKind.Next);
        AddSingle(keywords, "options", KeywordKind.Options);
        AddSingle(keywords, "index", KeywordKind.Next);
        AddSingle(keywords, "txqueuelen", KeywordKind.Next);
        AddSingle(keywords, "unspec", KeywordKind.Next);
        AddSingle(keywords, "loop", KeywordKind.Single);

        // Group keywords
        keywords["Configuration"] = new KeywordDef
        {
            Kind = KeywordKind.NewLine,
            ChildKeywords = new()
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
        };

        keywords["member"] = new KeywordDef
        {
            Kind = KeywordKind.GroupNext,
            IsArray = true,
            ChildKeywords = new()
            {
                { "flags", KeywordKind.Options },
                { "ifmaxaddr", KeywordKind.Next },
                { "port", KeywordKind.Next },
                { "priority", KeywordKind.Next },
                { "hostfilter", KeywordKind.Next },
                { "hw", KeywordKind.Next },
                { "ip", KeywordKind.Next },
            },
            ChildMultiTokenKeywords = new()
            {
                { "path", new MultiTokenDef { Kind = KeywordKind.Next, Keywords = new() { { "cost", KeywordKind.Next } } } },
            },
        };

        keywords["agent"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            IsArray = true,
            ChildKeywords = new()
            {
                { "domain", KeywordKind.Next },
                { "type", KeywordKind.Next },
                { "flags", KeywordKind.Next },
                { "desc", KeywordKind.NewLine },
            },
        };

        keywords["root"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            ChildKeywords = new()
            {
                { "id", KeywordKind.Next },
                { "priority", KeywordKind.Next },
                { "ifcost", KeywordKind.Next },
                { "port", KeywordKind.Next },
                { "ipfilter", KeywordKind.Next },
                { "flags", KeywordKind.Options },
                { "member", KeywordKind.Group },
            },
        };

        keywords["inet6"] = new KeywordDef
        {
            Kind = KeywordKind.GroupNext,
            IsArray = true,
            ChildKeywords = new()
            {
                { "prefixlen", KeywordKind.Next },
                { "scopeid", KeywordKind.Next },
                { "secured", KeywordKind.Single },
            },
            CustomHandler = (token, enumerator, queue) =>
            {
                // inet6 needs special GroupNext handling where the value is split on '%'
                // We still need to collect the group's children, so we do it inline.
                return HandleInet6Group(token, enumerator, queue);
            },
        };

        keywords["inet"] = new KeywordDef
        {
            Kind = KeywordKind.GroupNext,
            IsArray = true,
            ChildKeywords = new()
            {
                { "netmask", KeywordKind.Next },
                { "broadcast", KeywordKind.Next },
            },
        };

        keywords["RX"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            MergeChildren = true,
            ChildKeywords = new()
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
        };

        keywords["TX"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            MergeChildren = true,
            ChildKeywords = new()
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
        };

        // Multi-token (compound) keywords
        AddMultiToken(keywords, "nd6", KeywordKind.Options, new() { { "options", KeywordKind.Options } });
        AddMultiToken(keywords, "state", KeywordKind.Next, new() { { "availability", KeywordKind.Next } });
        AddMultiToken(keywords, "qosmarking", KeywordKind.Next, new() { { "enabled", KeywordKind.Next } });
        AddMultiToken(keywords, "generation", KeywordKind.Next, new() { { "id", KeywordKind.Next } });
        AddMultiToken(keywords, "uplink", KeywordKind.Next, new() { { "rate", KeywordKind.Next } });
        AddMultiToken(keywords, "downlink", KeywordKind.Next, new() { { "rate", KeywordKind.Next } });
        AddMultiToken(keywords, "low", KeywordKind.Next, new() { { "power", KeywordKind.Next }, { "mode", KeywordKind.Next } });
        AddMultiToken(keywords, "link", KeywordKind.Next, new() { { "rate", KeywordKind.Next }, { "quality", KeywordKind.Next } });
        AddMultiToken(keywords, "multi", KeywordKind.Next, new() { { "layer", KeywordKind.Next }, { "packet", KeywordKind.Next }, { "logging", KeywordKind.Next }, { "(mpklog)", KeywordKind.Drop } });

        return keywords;
    }

    private static void AddSingle(Dictionary<string, KeywordDef> dict, string key, KeywordKind kind)
    {
        dict[key] = new KeywordDef { Kind = kind };
    }

    private static void AddMultiToken(Dictionary<string, KeywordDef> dict, string key, KeywordKind kind, Dictionary<string, KeywordKind> following)
    {
        dict[key] = new KeywordDef
        {
            Kind = kind,
            MultiToken = new MultiTokenDef { Kind = kind, Keywords = following },
        };
    }

    /// <summary>
    /// Custom handler for inet6 groups that splits "addr%interface" on '%'
    /// and then collects the group's remaining child keywords.
    /// </summary>
    private static IEnumerable<Pair> HandleInet6Group(string token, IEnumerator<string> enumerator, Queue<string> queue)
    {
        // This is an array keyword — collect all occurrences
        var key = $"{token}s";
        var pairChildren = new List<Pair>();

        pairChildren.Add(CollectOneInet6(token, enumerator, queue));

        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == token)
            {
                pairChildren.Add(CollectOneInet6(token, enumerator, queue));
            }
            else
            {
                queue.Enqueue(nextToken);
                break;
            }
        }

        yield return new Pair(key, string.Empty, pairChildren, ChildType.ArrayType);
    }

    private static readonly Dictionary<string, KeywordKind> Inet6ChildKeywords = new()
    {
        { "prefixlen", KeywordKind.Next },
        { "scopeid", KeywordKind.Next },
        { "secured", KeywordKind.Single },
    };

    private static Pair CollectOneInet6(string token, IEnumerator<string> enumerator, Queue<string> queue)
    {
        var children = new List<Pair>();

        // Read the value and split on '%'
        if (Helpers.TryGetValue(enumerator, queue, out var value))
        {
            var items = value.Split('%');
            children.Add(new Pair(token, items[0], []));
            if (items.Length > 1)
            {
                children.Add(new Pair("interface", items[1], []));
            }
        }

        // Read remaining child keywords
        while (Helpers.TryGetValue(enumerator, queue, out var nextToken))
        {
            if (nextToken == Helpers.NewLine)
            {
                continue;
            }

            if (Inet6ChildKeywords.TryGetValue(nextToken, out var childKind))
            {
                children.AddRange(Helpers.HandleSingleKeyword(queue, enumerator, childKind, nextToken, '<', '>'));
            }
            else
            {
                queue.Enqueue(nextToken);
                break;
            }
        }

        return new Pair(token, string.Empty, children, ChildType.ObjectType);
    }

    #endregion
}
