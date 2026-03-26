using System.Text;

namespace JMW.Parsing;

/// <summary>
/// Factory methods for creating common keyword handler patterns.
/// Use these with KeywordDef.CustomHandler to compose parser behavior.
/// </summary>
public static class KeywordHandlers
{
    /// <summary>
    /// Creates a handler for flag/options-style values like "1234&lt;FLAG1,FLAG2,FLAG3&gt;".
    /// Produces a Pair with children: { Bits: "1234", Values: ["FLAG1", "FLAG2", "FLAG3"] }.
    /// </summary>
    public static Func<string, IEnumerator<string>, Queue<string>, IEnumerable<Pair>>
        Options(char open, char close, bool readUntilNewLine = false)
    {
        return (token, enumerator, queue) => ReadOptions(token, enumerator, queue, open, close, readUntilNewLine);
    }

    private static IEnumerable<Pair> ReadOptions(
        string token,
        IEnumerator<string> enumerator,
        Queue<string> queue,
        char open,
        char close,
        bool readUntilNewLine)
    {
        string value;
        if (readUntilNewLine)
        {
            var sb = new StringBuilder();
            while (Helpers.TryGetValue(enumerator, queue, out var nextItem) && nextItem != Helpers.NewLine)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(nextItem);
            }

            value = sb.ToString();
        }
        else
        {
            if (!Helpers.TryGetValue(enumerator, queue, out var v))
            {
                yield break;
            }

            value = v;
        }

        var items = value.Split(open);
        var optionList = Array.Empty<Pair>();

        if (items.Length > 1)
        {
            optionList = items[1].Trim(close).Split(',')
               .Select(o => new Pair($"{token} item", o, []))
               .ToArray();
        }

        var pairChildren = new[]
        {
            new Pair("Bits", items[0], []),
            new Pair("Values", string.Empty, optionList, ChildType.ArrayType),
        };

        yield return new Pair(token, string.Empty, pairChildren, ChildType.ObjectType);
    }
}
