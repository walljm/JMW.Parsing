namespace JMW.Parsing;

public static class Ifconfig
{
    #region Public Parse/Output

    public static void OutputKeyValues(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteKeyValues(BlockParser.ParseBlocks(inputReader, Config), outputWriter);
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteJson(BlockParser.ParseBlocks(inputReader, Config), outputWriter);
    }

    #endregion

    #region Parser Configuration

    public static readonly ParserConfig Config = new()
    {
        Tokenizer = new TokenizerConfig
        {
            Separators = ['='],
            StripTrailing = [':'],
            SplitOnSingleColon = true,
            EmitNewLineTokens = true,
        },

        TrimInitialWhitespace = true,
        TrimEndingWhitespace = false,

        FirstTokenHandler = static (enumerator, _) =>
            [new Pair("InterfaceName", enumerator.Current, [])],

        Keywords = BuildKeywords(),
    };

    private static Dictionary<string, KeywordDef> BuildKeywords()
    {
        var keywords = new Dictionary<string, KeywordDef>();

        // Single keywords
        AddOptions(keywords, "flags");
        AddOptions(keywords, "eflags");
        AddOptions(keywords, "xflags");
        AddOptions(keywords, "hwassist");
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
        AddOptions(keywords, "options");
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
                { "id", Kw(KeywordKind.Next) },
                { "priority", Kw(KeywordKind.Next) },
                { "hellotime", Kw(KeywordKind.Next) },
                { "fwddelay", Kw(KeywordKind.Next) },
                { "maxage", Kw(KeywordKind.Next) },
                { "holdcnt", Kw(KeywordKind.Next) },
                { "proto", Kw(KeywordKind.Next) },
                { "maxaddr", Kw(KeywordKind.Next) },
                { "timeout", Kw(KeywordKind.Next) },
                { "ifcost", Kw(KeywordKind.Next) },
                { "port", Kw(KeywordKind.Next) },
                { "ipfilter", Kw(KeywordKind.Next) },
                { "flags", Kw(KeywordKind.Next) },
                { "root", Kw(KeywordKind.Group) },
            },
        };

        keywords["member"] = new KeywordDef
        {
            Kind = KeywordKind.GroupNext,
            IsArray = true,
            ChildKeywords = new()
            {
                { "flags", OptionsKw() },
                { "ifmaxaddr", Kw(KeywordKind.Next) },
                { "port", Kw(KeywordKind.Next) },
                { "priority", Kw(KeywordKind.Next) },
                { "hostfilter", Kw(KeywordKind.Next) },
                { "hw", Kw(KeywordKind.Next) },
                { "ip", Kw(KeywordKind.Next) },
            },
            ChildMultiTokenKeywords = new()
            {
                { "path", new MultiTokenDef { Kind = KeywordKind.Next, Keywords = [("cost", KeywordKind.Next)] } },
            },
        };

        keywords["agent"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            IsArray = true,
            ChildKeywords = new()
            {
                { "domain", Kw(KeywordKind.Next) },
                { "type", Kw(KeywordKind.Next) },
                { "flags", Kw(KeywordKind.Next) },
                { "desc", Kw(KeywordKind.NewLine) },
            },
        };

        keywords["root"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            ChildKeywords = new()
            {
                { "id", Kw(KeywordKind.Next) },
                { "priority", Kw(KeywordKind.Next) },
                { "ifcost", Kw(KeywordKind.Next) },
                { "port", Kw(KeywordKind.Next) },
                { "ipfilter", Kw(KeywordKind.Next) },
                { "flags", OptionsKw() },
                { "member", Kw(KeywordKind.Group) },
            },
        };

        keywords["inet6"] = new KeywordDef
        {
            Kind = KeywordKind.GroupNext,
            IsArray = true,
            ChildKeywords = new()
            {
                { "prefixlen", Kw(KeywordKind.Next) },
                { "scopeid", Kw(KeywordKind.Next) },
                { "secured", Kw(KeywordKind.Single) },
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
                { "netmask", Kw(KeywordKind.Next) },
                { "broadcast", Kw(KeywordKind.Next) },
            },
        };

        keywords["RX"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            MergeChildren = true,
            ChildKeywords = new()
            {
                { "packets", Kw(KeywordKind.Next) },
                { "errors", Kw(KeywordKind.Next) },
                { "dropped", Kw(KeywordKind.Next) },
                { "overruns", Kw(KeywordKind.Next) },
                { "carrier", Kw(KeywordKind.Next) },
                { "frame", Kw(KeywordKind.Next) },
                { "collisions", Kw(KeywordKind.Next) },
                { "bytes", Kw(KeywordKind.NewLine) },
            },
        };

        keywords["TX"] = new KeywordDef
        {
            Kind = KeywordKind.Group,
            MergeChildren = true,
            ChildKeywords = new()
            {
                { "packets", Kw(KeywordKind.Next) },
                { "errors", Kw(KeywordKind.Next) },
                { "dropped", Kw(KeywordKind.Next) },
                { "overruns", Kw(KeywordKind.Next) },
                { "carrier", Kw(KeywordKind.Next) },
                { "frame", Kw(KeywordKind.Next) },
                { "collisions", Kw(KeywordKind.Next) },
                { "bytes", Kw(KeywordKind.NewLine) },
            },
        };

        // Multi-token (compound) keywords
        keywords["nd6"] = new KeywordDef
        {
            CustomHandler = (token, enumerator, queue) =>
            {
                // Build compound key: consume "options" if it follows
                var key = token;
                if (Helpers.TryGetValue(enumerator, queue, out var next))
                {
                    if (next == "options")
                        key += $" {next}";
                    else
                        queue.Enqueue(next);
                }

                return KeywordHandlers.Options('<', '>')(key, enumerator, queue);
            },
        };
        AddMultiToken(keywords, "state", KeywordKind.Next, [("availability", KeywordKind.Next)]);
        AddMultiToken(keywords, "qosmarking", KeywordKind.Next, [("enabled", KeywordKind.Next)]);
        AddMultiToken(keywords, "generation", KeywordKind.Next, [("id", KeywordKind.Next)]);
        AddMultiToken(keywords, "uplink", KeywordKind.Next, [("rate", KeywordKind.Next)]);
        AddMultiToken(keywords, "downlink", KeywordKind.Next, [("rate", KeywordKind.Next)]);
        AddMultiToken(keywords, "low", KeywordKind.Next, [("power", KeywordKind.Next), ("mode", KeywordKind.Next)]);
        AddMultiToken(keywords, "link", KeywordKind.Next, [("rate", KeywordKind.Next), ("quality", KeywordKind.Next)]);
        AddMultiToken(keywords, "multi", KeywordKind.Next, [("layer", KeywordKind.Next), ("packet", KeywordKind.Next), ("logging", KeywordKind.Next), ("(mpklog)", KeywordKind.Drop)]);

        return keywords;
    }

    private static KeywordDef Kw(KeywordKind kind) => new() { Kind = kind };

    private static KeywordDef OptionsKw() => new() { CustomHandler = KeywordHandlers.Options('<', '>') };

    private static void AddOptions(Dictionary<string, KeywordDef> dict, string key)
    {
        dict[key] = OptionsKw();
    }

    private static void AddSingle(Dictionary<string, KeywordDef> dict, string key, KeywordKind kind)
    {
        dict[key] = new KeywordDef { Kind = kind };
    }

    private static void AddMultiToken(Dictionary<string, KeywordDef> dict, string key, KeywordKind kind, IReadOnlyList<(string Token, KeywordKind Kind)> following)
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
                children.AddRange(BlockParser.ReadKeywordValue(queue, enumerator, childKind, nextToken));
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
