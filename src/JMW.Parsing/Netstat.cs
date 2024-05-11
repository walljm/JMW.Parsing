using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using JMW.Parsing.Models;

namespace JMW.Parsing;

public static class Netstat
{
    public static void Parse(TextReader inputReader, TextWriter outputWriter, ParsingOptions parsingOptions)
    {
        if (parsingOptions.OutputType == OutputType.KeyValue)
        {
            OutputKeyValues(inputReader, outputWriter);
        }
        else if (parsingOptions.OutputType == OutputType.Json)
        {
            OutputJson(inputReader, outputWriter);
        }
    }

    public static void OutputKeyValues(TextReader inputReader, TextWriter outputWriter)
    {
        var blocks = FilterBlocks(Parser.GetBlocks(inputReader, trimInitialWhitespace: true, trimEndingWhitespace: false));

        foreach (var block in blocks)
        {
            outputWriter.WriteLine(); // empty line for a spacer

            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Parser.TokenizeTable(blockReader));

            foreach (var pair in pairs)
            {
                pair.WriteKeyValues(outputWriter);
            }
        }
    }


    public static IEnumerable<string> FilterBlocks(IEnumerable<string> blocks)
    {
        foreach (var block in blocks)
        {
            if (block.StartsWith("internet", StringComparison.OrdinalIgnoreCase))
            {
                yield return block;
            }
        }
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        var blocks = FilterBlocks(Parser.GetBlocks(inputReader, new Parser.BlockOptions { BlocksAreIndented = false }));

        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true
        };

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

        writer.WriteStartArray();

        foreach (var block in blocks)
        {
            using var blockReader = new StringReader(block);
            var pairs = GetPairs(Parser.TokenizeTable(blockReader)).ToArray();
            if (pairs.Length == 0)
            {
                continue;
            }

            writer.WriteStartObject();
            foreach (var pair in pairs)
            {
                pair.WriteJson(writer);
            }

            writer.Flush();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());
        outputWriter.WriteLine(json);
    }


    private static IEnumerable<Pair> GetPairs(IEnumerable<string> tokens)
    {
        var queue = new Queue<string>();
        using var enumerator = tokens.GetEnumerator();
        enumerator.MoveNext(); // first line is the name of the table.
        var table = enumerator.Current;

        enumerator.MoveNext(); // skip newline.
        enumerator.MoveNext(); // first column.

        var columns = new List<string>();
        while (enumerator.Current != Parser.NewLine)
        {
            columns.Add(enumerator.Current);
            enumerator.MoveNext();
        }

        var fields = new List<Pair>();
        var i = 0;
        while (enumerator.MoveNext())
        {
            if (enumerator.Current == Parser.NewLine)
            {
                yield return new Pair("row", String.Empty, fields.ToArray(), ChildType.ArrayType);
                fields = [];
                i = 0;
            }

            fields.Add(new Pair(columns[i], enumerator.Current, []));
        }

    }
}