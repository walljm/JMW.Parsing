using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JMW.Parsing;

public static class Helpers
{
    public const string NewLine = "<newline>";
    public const string ArrayItem = "<arrayitem>";
    public record BlockOptions
    {
        public bool TrimInitialWhitespace { get; set; } = true;
        public bool TrimEndingWhitespace { get; set; } = false;
        public bool BlocksAreIndented { get; set; } = true;
    }

    public static IEnumerable<string> GetBlocks(TextReader output, bool trimInitialWhitespace = true, bool trimEndingWhitespace = false)
    {
        return GetBlocks(
            output,
            new BlockOptions { TrimEndingWhitespace = trimEndingWhitespace, TrimInitialWhitespace = trimInitialWhitespace, BlocksAreIndented = true }
        );
    }

    public static IEnumerable<string> GetBlocks(TextReader output, BlockOptions options)
    {
        void TrimWhitespace()
        {
            int? i;
            while ((i = output.Peek()) != -1)
            {
                var current = (char)i;
                if (char.IsWhiteSpace(current))
                {
                    output.Read();
                    continue;
                }

                break;
            }
        }

        bool IsNewBlock(char current, char last)
        {
            if (options.BlocksAreIndented)
            {
                return !char.IsWhiteSpace(current) && last == '\n';
            }
            else
            {
                return char.IsWhiteSpace(current) && last == '\n';
            }
        }

        var sb = new StringBuilder();
        var last = '\0';
        int? character;

        if (options.TrimInitialWhitespace)
        {
            // trim beginning whitespace.
            TrimWhitespace();
        }

        while ((character = output.Read()) != -1)
        {
            var current = (char)character;
            if (IsNewBlock(current, last))
            {
                if (sb.Length > 0)
                {
                    if (options.TrimEndingWhitespace)
                    {
                        while (char.IsWhiteSpace(sb[sb.Length - 1]))
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                    }

                    yield return sb.ToString();
                    sb.Clear();
                }

                if (char.IsWhiteSpace(current) && options.TrimInitialWhitespace)
                {
                    TrimWhitespace();
                }
                else
                {
                    sb.Append(current);
                }
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
            if (options.TrimEndingWhitespace)
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
    /// <summary>
    /// Counts the numbers of instances of the <paramref name="c"/> param in the <paramref
    /// name="test"/> string.
    /// </summary>
    /// <param name="test">string to search</param>
    /// <param name="c">char to search for</param>
    /// <param name="respect_word_boundaries">doesn't count "port" the same as "portindex"</param>
    /// <returns>int</returns>
    public static int CountInstances(this string test, string c, bool respect_word_boundaries = false)
    {
        var s = test + " "; // to avoid geriatrics with strings not ending in a word boundary.
        var cnt = 0;
        if (respect_word_boundaries) c = c + " ";
        while (s.IndexOf(c, StringComparison.Ordinal) != -1)
        {
            s = s.Substring(s.IndexOf(c, StringComparison.Ordinal) + c.Length);
            cnt++;
        }
        return cnt;
    }

    public static Dictionary<char, int> GetCharHistogram(this string line, bool collapse_whitespace = true)
    {
        var hist = new Dictionary<char, int>();

        var prev_char_is_ws = false;

        foreach (var c in line)
        {
            var my_char = c;
            if (collapse_whitespace && char.IsWhiteSpace(c))
            {
                my_char = ' ';
                if (prev_char_is_ws)
                {
                    continue;
                }
            }
            // Only the first whitespace counts we ignore the rest
            if (!hist.TryAdd(my_char, 1))
            {
                hist[my_char]++;
            }

            prev_char_is_ws = char.IsWhiteSpace(c);
        }

        return hist;
    }
    #region ParseToIndexOf

    public static string ParseToIndexOf(this string s, params string[] parms)
    {
        return s.ParseToIndexOf(false, parms);
    }

    /// <summary>
    ///   Returns a string from the begining to the index of one of the provided string parameters
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
    /// <param name="parms">The strings to parse to</param>
    /// <returns>Subsection of the string from begining to the index of the first matching parameter</returns>
    public static string ParseToIndexOf(this string s, bool case_insensitive = false, params string[] parms)
    {
        // return empty if string is empty
        if (s.Length == 0) return string.Empty;

        // return full string if no params were provided
        if (parms.Length == 0) return s;

        // lowercase the string if we're working with case insensitive search
        var str = case_insensitive ? s.ToLower() : s;

        var idx = -1;

        // look for earch parm, return first one found.
        foreach (var p in parms)
        {
            // get the index of our search parameter
            var tidx = IndexOf(str, p, case_insensitive);
            if (idx == -1)
                idx = tidx;
            else if (idx > tidx && tidx > -1)
                idx = tidx;
        }

        // if its greater than 0, return it.
        if (idx > -1)
        {
            return s.SafeSubstring(0, idx);
        }

        // worst case, return the string as is.
        return s;
    }
    /// <summary>
    /// Finds the index of a search string.  Optionally allows case insensitivity or option to search for last index.
    /// </summary>
    /// <param name="str">String to search</param>
    /// <param name="search_string">String to search for</param>
    /// <param name="case_insensitive">Ignore case</param>
    /// <param name="last">Search for last instance</param>
    public static int IndexOf(this string str, string search_string, bool case_insensitive, bool last = false)
    {
        // get the index of our search parameter
        int idx;
        if (case_insensitive)
            idx = last ? str.LastIndexOf(search_string.ToLower(), StringComparison.Ordinal) : str.IndexOf(search_string.ToLower(), StringComparison.Ordinal);
        else
            idx = last ? str.LastIndexOf(search_string, StringComparison.Ordinal) : str.IndexOf(search_string, StringComparison.Ordinal);
        return idx;
    }

    /// <summary>
    ///   Safely handles the cases where your start exceeds the length of the string, or your start+length exceeds the length of the string.
    /// </summary>
    /// <param name="s">The string to do the substring on</param>
    /// <param name="start">The index of the string to start parsing from</param>
    /// <param name="length">The number of characters to return</param>
    /// <returns>A subsection of the provided string</returns>
    public static string SafeSubstring(this string s, int start, int? length = null)
    {
        // return empty if the requested start index is past the length of the provided string
        if (start > s.Length) return string.Empty;

        // if length isn't provided, return without it.
        if (length is null) return s.Substring(start);

        // if the length is 0 or less, the string should be empty.
        if (length < 1) return string.Empty;

        var len = (int)length;
        if (start + length > s.Length)
        {
            len = s.Length - start;
        }

        return s.Substring(start, len);
    }

    #endregion ParseToIndexOf

    #region ParseAfterLastIndexOf_PlusLength

    public static string ParseAfterLastIndexOf_PlusLength(this string s, params string[] parms)
    {
        return s.ParseAfterLastIndexOf_PlusLength(false, parms);
    }

    /// <summary>
    ///   Returns a string starting after the last index of the first provided string parameter discovered to the end of the string
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="case_insensitive">Wheather to performs a case insensitive search or not</param>
    /// <param name="parms">The strings to parse to</param>
    public static string ParseAfterLastIndexOf_PlusLength(this string s, bool case_insensitive = false, params string[] parms)
    {
        // return empty if string is empty
        if (s.Length == 0) return string.Empty;

        // return full string if no params were provided
        if (parms.Length == 0) return s;

        // lowercase the string if we're working with case insensitive search
        var str = case_insensitive ? s.ToLower() : s;

        var idx = -1;
        var l = 0;

        // look for earch parm, return first one found.
        foreach (var p in parms)
        {
            // get the index of our search parameter
            var tidx = IndexOf(str, p, case_insensitive, last: true);
            if (idx == -1)
            {
                idx = tidx;
                l = p.Length;
            }
            else if (idx < tidx && tidx > -1)
            {
                idx = tidx;
                l = p.Length;
            }
        }

        // if its greater than 0, return it.
        if (idx > -1)
        {
            return s.SafeSubstring(idx + l);
        }

        // worst case, return the string as is.
        return s;
    }

    #endregion ParseAfterLastIndexOf_PlusLength



    public const string NewLine = "<newline>";

    public static IEnumerable<string> TokenizeTable(TextReader output)
    {
        int? c;
        var sb = new StringBuilder();
        while ((c = output.Read()) != -1)
        {
            if (!char.IsWhiteSpace((char)c))
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

            yield return sb.ToString();
            sb.Clear();
        }
    }
}