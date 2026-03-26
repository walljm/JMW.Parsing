using System.Text;
using System.Text.Json;

namespace JMW.Parsing;

internal static class PairWriter
{
    public static void WriteJson(IEnumerable<IReadOnlyList<Pair>> blocksOfPairs, TextWriter output)
    {
        var jsonWriterOptions = new JsonWriterOptions { Indented = true };

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

        writer.WriteStartArray();

        foreach (var pairs in blocksOfPairs)
        {
            if (pairs.Count == 0)
            {
                continue;
            }

            writer.WriteStartObject();
            foreach (var pair in pairs)
            {
                WritePairJson(pair, writer);
            }

            writer.Flush();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();
        output.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position));
    }

    public static void WriteKeyValues(IEnumerable<IEnumerable<Pair>> blocksOfPairs, TextWriter output)
    {
        foreach (var pairs in blocksOfPairs)
        {
            output.WriteLine(); // empty line for a spacer

            foreach (var pair in pairs)
            {
                WritePairKeyValues(pair, output);
            }
        }
    }

    public static void WritePairJson(Pair pair, Utf8JsonWriter writer, bool ignoreKey = false)
    {
        switch (pair.ChildType)
        {
            case ChildType.StringType:
                writer.WriteString(Helpers.CleanKey(pair.Key), pair.Value);
                break;

            case ChildType.ObjectType:
            {
                if (ignoreKey)
                {
                    writer.WriteStartObject();
                }
                else
                {
                    writer.WriteStartObject(Helpers.CleanKey(pair.Key));
                }

                foreach (var child in pair.Children)
                {
                    WritePairJson(child, writer);
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
                    writer.WriteStartArray(Helpers.CleanKey(pair.Key));
                }

                foreach (var child in pair.Children)
                {
                    if (child.ChildType == ChildType.StringType)
                    {
                        writer.WriteStringValue(child.Value);
                    }
                    else
                    {
                        WritePairJson(child, writer, true);
                    }
                }

                writer.WriteEndArray();
                break;
        }
    }

    public static void WritePairKeyValues(Pair pair, TextWriter writer, int indent = 0)
    {
        writer.WriteLine($"{string.Empty.PadLeft(indent)}{Helpers.CleanKey(pair.Key)}: {pair.Value}");
        if (pair.Children.Count > 0)
        {
            foreach (var child in pair.Children)
            {
                WritePairKeyValues(child, writer, indent + 4);
            }
        }
    }
}
