using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JMW.Parsing;

public static class Helpers
{
    public const string NewLine = "<newline>";
    public const string ArrayItem = "<arrayitem>";

    public static IEnumerable<string> GetBlocks(TextReader output, bool trimInitialWhitespace = true, bool trimEndingWhitespace = false)
    {
        var sb = new StringBuilder();
        var last = '\0';
        int? character;

        if (trimInitialWhitespace)
        {
            // trim beginning whitespace.
            while ((character = output.Peek()) != -1)
            {
                var current = (char)character;
                if (char.IsWhiteSpace(current))
                {
                    output.Read();
                    continue;
                }

                break;
            }
        }

        while ((character = output.Read()) != -1)
        {
            var current = (char)character;
            if (!char.IsWhiteSpace(current) && last == '\n')
            {
                if (sb.Length > 0)
                {
                    if (trimEndingWhitespace)
                    {
                        while (char.IsWhiteSpace(sb[sb.Length - 1]))
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }

                    yield return sb.ToString();
                    sb.Clear();
                }

                sb.Append(current);
            }
            else
            {
                sb.Append(current);
            }

            last = current;
        }

        // return the last block
        if (sb.Length > 0)
        {
            if (trimEndingWhitespace)
            {
                while (char.IsWhiteSpace(sb[sb.Length - 1]))
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }

            yield return sb.ToString();
        }
    }

    public static Dictionary<char, int> CountChars(StringBuilder sb, params char[] target)
    {
        var chars = target.ToDictionary(k => k, _ => 0);
        for (var i = 0; i < sb.Length; i++)
        {
            if (chars.TryGetValue(sb[i], out var value))
            {
                chars[sb[i]] = ++value;
            }
        }

        return chars;
    }

    public static string CleanKey(string key)
    {
        // remove non alphabet chars
        // convert to PascalCase
        var sb = new StringBuilder();
        char? last = null;
        foreach (var current in key)
        {
            if (char.IsDigit(current) || char.IsLetter(current))
            {
                sb.Append(
                    last is null || char.IsWhiteSpace(last.Value)
                        ? char.ToUpper(current)
                        : current
                );
            }

            last = current;
        }

        return sb.ToString();
    }

    public static bool TryGetValue(
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
}