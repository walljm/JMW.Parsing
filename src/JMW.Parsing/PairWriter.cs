using System.Text;
using System.Text.Json;

namespace JMW.Parsing;

public static class PairWriter
{
    public static void WriteJson(IEnumerable<IReadOnlyList<Pair>> blocksOfPairs, TextWriter output, Func<string, string>? keyTransform = null)
    {
        keyTransform ??= Helpers.CleanKey;
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
                WritePairJson(pair, writer, keyTransform: keyTransform);
            }

            writer.Flush();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();
        output.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position));
    }

    public static void WriteKeyValues(IEnumerable<IEnumerable<Pair>> blocksOfPairs, TextWriter output, Func<string, string>? keyTransform = null)
    {
        keyTransform ??= Helpers.CleanKey;
        foreach (var pairs in blocksOfPairs)
        {
            output.WriteLine(); // empty line for a spacer

            foreach (var pair in pairs)
            {
                WritePairKeyValues(pair, output, keyTransform: keyTransform);
            }
        }
    }

    public static void WritePairJson(Pair pair, Utf8JsonWriter writer, bool ignoreKey = false, Func<string, string>? keyTransform = null)
    {
        keyTransform ??= Helpers.CleanKey;
        switch (pair.ChildType)
        {
            case ChildType.StringType:
                writer.WriteString(keyTransform(pair.Key), pair.Value);
                break;

            case ChildType.ObjectType:
            {
                if (ignoreKey)
                {
                    writer.WriteStartObject();
                }
                else
                {
                    writer.WriteStartObject(keyTransform(pair.Key));
                }

                foreach (var child in pair.Children)
                {
                    WritePairJson(child, writer, keyTransform: keyTransform);
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
                    writer.WriteStartArray(keyTransform(pair.Key));
                }

                foreach (var child in pair.Children)
                {
                    if (child.ChildType == ChildType.StringType)
                    {
                        writer.WriteStringValue(child.Value);
                    }
                    else
                    {
                        WritePairJson(child, writer, true, keyTransform: keyTransform);
                    }
                }

                writer.WriteEndArray();
                break;
        }
    }

    public static void WritePairKeyValues(Pair pair, TextWriter writer, int indent = 0, Func<string, string>? keyTransform = null)
    {
        keyTransform ??= Helpers.CleanKey;
        writer.WriteLine($"{string.Empty.PadLeft(indent)}{keyTransform(pair.Key)}: {pair.Value}");
        if (pair.Children.Count > 0)
        {
            foreach (var child in pair.Children)
            {
                WritePairKeyValues(child, writer, indent + 4, keyTransform: keyTransform);
            }
        }
    }
}
