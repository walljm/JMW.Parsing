

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JMW.Parsing;

public enum OutputType
{
    Json,
    KeyValue,
}

public record ParsingOptions(OutputType OutputType);

public static class Ifconfig
{
    public static void Parse(TextReader output, ParsingOptions options)
    {
        var blocks = GetBlocks(output);

        foreach (var block in blocks)
        {
            using var blockReader = new StringReader(block);
            var tokens = Tokenize(blockReader);
            var pairs = GetPairs(tokens);
            if (options.OutputType == OutputType.KeyValue)
            {
                foreach (var pair in pairs)
                {
                    Console.WriteLine($"{CleanKey(pair.Key)}: {pair.Value}");
                }
            }
            else if (options.OutputType == OutputType.Json)
            {

            }
        }
    }

    private static string CleanKey(string key)
    {
        // remove non alphabet chars
        // convert to PascalCase
        var sb = new StringBuilder();
        foreach (var c in key)
        {
            if (char.IsDigit(c) || char.IsLetter(c))
            {
                sb.Append(c);
            }
            else if (char.IsWhiteSpace(c))
            {
                if (sb.Length > 0)
                {
                    sb[^1] = char.ToUpper(sb[^1]);
                }
            }
        }
        return sb.ToString();
    }


    private const string newLine = "<newline>";
    private const string next = "<next>";
    private const string options = "<options>";
    private const string group = "<group>";
    private const string groupNext = "<groupnext>";

    private record GroupDefinition(
        string Kind,
        Dictionary<string, string> Keywords,
        Dictionary<string, HashSet<string>> MultipleKeywords
        );

    private record Pair(string Key, string Value);

    private static readonly Dictionary<string, string> singleKeywords = new(){
        {"flags", options},
        {"eflags", options},
        {"xflags", options},
        {"hwassist", options},
        {"mtu", next},
        {"ether", next},
        {"media", newLine},
        {"status", next},
        {"priority", next},
        {"type", newLine},
        {"desc", newLine},
        {"scheduler", next},
        {"routermode4", next},
        {"routermode6", next},
        {"netif", next},
        {"flowswitch", next},
        {"options", options},
        {"inet", next},
        {"netmask", next},
        {"broadcast", next},
        {"inet6", next},
        {"prefixlen", next},
        {"scopeid", next},
        {"index", next},
    };

    private static readonly Dictionary<string, GroupDefinition> groupKeywords = new()
    {
        {"Configuration:", new(newLine, new(){
            {"id", next},
            {"priority", next},
            {"hellotime", next},
            {"fwdelay", next},
            {"maxage", next},
            {"holdcnt", next},
            {"proto", next},
            {"maxaddr", next},
            {"timeout", next},
            {"ifcost", next},
            {"port", next},
            {"ipfilter", next},
            {"flags", next}
        }, [])},

        {"member", new(groupNext, new () {
            {"flags", next},
            {"ifmaxaddr", next},
            {"port", next},
            {"priority", next},
            {"hostfilter", next},
            {"hw", next},
            {"ip", next}
        }, new(){ {"path", new(){"cost"}}})},

        {"agent", new(group, new() {
            {"domain", next},
            {"type", next},
            {"flags", next},
            {"desc", newLine}
        }, [])},

        {"root", new(group, new(){
            {"id", next},
            {"priority", next},
            {"ifcost", next},
            {"port", next},
            {"ipfilter", next},
            {"flags", next}
        }, [])},
    };

    private static readonly Dictionary<string, HashSet<string>> multipleKeywords = new()
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

    private static IEnumerable<Pair> GetPairs(IEnumerable<string> tokens)
    {
        var queue = new Queue<string>();
        var enumerator = tokens.GetEnumerator();
        // first token should always be the interface name.
        enumerator.MoveNext();
        yield return new Pair("interfacename", enumerator.Current);

        while (TryGetValue(enumerator, queue, out var token))
        {
            if (groupKeywords.TryGetValue(token, out var group))
            {
                var key = token;

                if (group.Kind == groupNext)
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
                    else if (token == newLine)
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
        if (kind == newLine)
        {
            // grab tokens until newline.
            var value = string.Empty;
            while (TryGetValue(enumerator, queue, out var next) && next != newLine)
            {
                value += $" {next}";
            }
            // last item is a newline, which we can ignore,
            yield return new Pair(token, value.Trim());
        }
        else if (kind == next)
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
        else if (kind == options)
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
                yield return newLine;
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

    public static IEnumerable<string> GetBlocks(TextReader output)
    {
        var sb = new StringBuilder();
        var last = '\0';
        int? c;
        while ((c = output.Read()) != -1)
        {
            if (!char.IsWhiteSpace((char)c) && last == '\n')
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

            last = (char)c;
        }
    }

    private static Dictionary<char, int> CountChars(StringBuilder sb, params char[] target)
    {
        var chars = target.ToDictionary(k => k, v => 0);
        for (int i = 0; i < sb.Length; i++)
        {
            if (chars.TryGetValue(sb[i], out var value))
            {
                chars[sb[i]] = ++value;
            }
        }
        return chars;
    }

}