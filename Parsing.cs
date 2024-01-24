

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JMW.Parsing;

public record Pair(string Key, string Value);

public static class Parser
{
    private const string NewLine = "<newline>";
    private const string Next = "<next>";
    private const string Options = "<options>";
    private const string Group = "<group>";
    private const string GroupNext = "<groupnext>";

    private static readonly Dictionary<string, string> SingleKeywords = new(){
        {"flags", Options},
        {"eflags", Options},
        {"xflags", Options},
        {"hwassist", Options},
        {"mtu", Next},
        {"ether", Next},
        {"media", NewLine},
        {"status", Next},
        {"priority", Next},
        {"type", NewLine},
        {"desc", NewLine},
        {"scheduler", Next},
        {"routermode4", Next},
        {"routermode6", Next},
        {"netif", Next},
        {"flowswitch", Next},
        {"options", Options},
        {"inet", Next},
        {"netmask", Next},
        {"broadcast", Next},
        {"inet6", Next},
        {"prefixlen", Next},
        {"scopeid", Next},
        {"index", Next},
    };

    private record GroupDefinition(
        string Kind,
        Dictionary<string, string> Keywords,
        Dictionary<string, HashSet<string>> MultipleKeywords
        );

    private static readonly Dictionary<string, GroupDefinition> GroupKeywords = new()
    {
        {"Configuration:", new(NewLine, new(){
            {"id", Next},
            {"priority", Next},
            {"hellotime", Next},
            {"fwdelay", Next},
            {"maxage", Next},
            {"holdcnt", Next},
            {"proto", Next},
            {"maxaddr", Next},
            {"timeout", Next},
            {"ifcost", Next},
            {"port", Next},
            {"ipfilter", Next},
            {"flags", Next}
        }, new())},

        {"member", new(GroupNext, new () {
            {"flags", Next},
            {"ifmaxaddr", Next},
            {"port", Next},
            {"priority", Next},
            {"hostfilter", Next},
            {"hw", Next},
            {"ip", Next}
        }, new(){ {"path", new(){"cost"}}})},

        {"agent", new(Group, new() {
            {"domain", Next},
            {"type", Next},
            {"flags", Next},
            {"desc", NewLine}
        }, new())},

        {"root", new(Group, new(){
            {"id", Next},
            {"priority", Next},
            {"ifcost", Next},
            {"port", Next},
            {"ipfilter", Next},
            {"flags", Next}
        }, new())},
    };

    private static readonly Dictionary<string, HashSet<string>> MultipleKeywords = new()
    {
        {"nd6", new(){"options"}},
        {"root", new(){"id"}},
        {"path", new(){"cost"}},
        {"state", new(){"availability"}},
        {"link", new(){ "rate", "quality"}},
        {"qosmarking", new(){"enabled"}},
        {"low", new(){"power", "mode"}},
        {"multi", new(){"layer", "packet", "logging", "(mpklog)"}},
        {"generation", new(){"id"}},
        {"uplink", new(){"rate"}},
        {"downlink", new(){"rate"}},
    };

    private static bool TryGetValue(
        IEnumerator<string> enumerator,
        Queue<string> queue,
        [NotNullWhen(true)] out string? value
    )
    {
        // first try and get it from the queue.
        if (queue.TryDequeue(out value))
        {
            return true;
        }
        // not in the queue? get it from the enumerator.
        if (enumerator.MoveNext())
        {
            value = enumerator.Current;
            return true;
        }

        value = null;
        return false;
    }

    public static IEnumerable<Pair> GetPairs(IEnumerable<string> tokens)
    {
        var queue = new Queue<string>();
        var enumerator = tokens.GetEnumerator();
        // first token should always be the interface name.
        enumerator.MoveNext();
        yield return new Pair("interfacename", enumerator.Current);

        while (TryGetValue(enumerator, queue, out var token))
        {
            if (GroupKeywords.ContainsKey(token))
            {
                var group = GroupKeywords[token];
                var key = token;

                if (group.Kind == GroupNext)
                {
                    if (TryGetValue(enumerator, queue, out var value))
                    {
                        yield return new Pair(key, value);
                        key += $" {value}";
                    }
                }

                while (TryGetValue(enumerator, queue, out token))
                {
                    if (group.Keywords.ContainsKey(token))
                    {
                        foreach (var item in HandleSingleKeyword(queue, enumerator, group.Keywords[token], $"{key} {token}"))
                        {
                            yield return item;
                        }
                    }
                    else if (group.MultipleKeywords.ContainsKey(token))
                    {
                        foreach (var item in HandleMultipleKeywords(queue, enumerator, group.MultipleKeywords, token, $"{key} {token}"))
                        {
                            yield return item;
                        }
                    }
                    else if (token == NewLine)
                    {
                        continue;
                    }
                    else
                    {
                        queue.Enqueue(token); // put it back on the stack.
                        break;
                    }
                }
            }
            else if (MultipleKeywords.ContainsKey(token))
            {
                foreach (var item in HandleMultipleKeywords(queue, enumerator, MultipleKeywords, token, token))
                {
                    yield return item;
                }
            }
            else if (SingleKeywords.ContainsKey(token))
            {
                if (token is null)
                {
                    throw new InvalidOperationException("Token can't be null");
                }

                var kind = SingleKeywords[token];
                foreach (var item in HandleSingleKeyword(queue, enumerator, kind, token))
                {
                    yield return item;
                }
            }
        }
    }

    private static IEnumerable<Pair> HandleMultipleKeywords(
        Queue<string> queue,
        IEnumerator<string> enumerator,
        Dictionary<string, HashSet<string>> keywords,
        string token,
        string key)
    {
        var escape = false;
        foreach (var item in keywords[token])
        {
            if (!TryGetValue(enumerator, queue, out var next))
            {
                yield break;
            }

            if (!keywords[token].Contains(next))
            {
                queue.Enqueue(next);
                escape = true;
                break;
            }

            key += $" {next}";
        }

        if (escape)
        {
            yield break;
        }

        if (TryGetValue(enumerator, queue, out var value))
        {
            yield return new Pair(key, value);
        }
    }

    private static IEnumerable<Pair> HandleSingleKeyword(Queue<string> queue, IEnumerator<string> enumerator, string kind, string token)
    {
        if (kind == NewLine)
        {
            // grab tokens until newline.
            var value = string.Empty;
            while (TryGetValue(enumerator, queue, out var next) && next != NewLine)
            {
                value += $" {next}";
            }
            // last item is a newline, which we can ignore,
            yield return new Pair(token, value.Trim());
        }
        else if (kind == Next)
        {
            if (TryGetValue(enumerator, queue, out var value))
            {
                if (token == "inet6")
                {
                    var items = value.Split('%');
                    yield return new Pair(token, items[0]);
                    if (items.Length > 1)
                    {
                        yield return new Pair($"{token} ifc", items[1]);
                    }
                }
                else
                {

                    yield return new Pair(token, value);
                }
            }
        }
        else if (kind == Options)
        {
            if (TryGetValue(enumerator, queue, out var value))
            {
                var items = value.Split('<');
                yield return new Pair(token, items[0]);
                if (items.Length > 1)
                {
                    var lst = items[1].Trim('>');
                    yield return new Pair($"{token} list", lst);
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Unknown Keyword Type");
        }
    }

    public static IEnumerable<string> Tokenize(TextReader output)
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
            var colonCount = CountChars(sb, ':');
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

        yield break;
    }

    public static IEnumerable<string> GetBlocks(string output)
    {
        var sb = new StringBuilder();
        var last = '\0';
        foreach (var c in output)
        {
            if (!char.IsWhiteSpace(c) && last == '\n')
            {
                // new block
                yield return sb.ToString();
                sb.Clear();
                sb.Append(c);
            }
            else
            {
                sb.Append(c);
            }

            last = c;
        }
    }

    private static Dictionary<char, int> CountChars(StringBuilder sb, params char[] target)
    {
        var chars = target.ToDictionary(k => k, v => 0);
        for (int i = 0; i < sb.Length; i++)
        {
            if (chars.ContainsKey(sb[i]))
            {
                chars[sb[i]]++;
            }
        }
        return chars;
    }

}