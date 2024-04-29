using System.Text;

namespace JMW.Parsing;

public static class Parser
{
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
}