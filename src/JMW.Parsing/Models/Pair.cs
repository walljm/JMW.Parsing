using System.Text;
using System.Text.Json;

namespace JMW.Parsing.Models;

internal enum ChildType
{
    StringType,
    ArrayType,
    ObjectType,
}

internal record Pair(string Key, string Value, Pair[] Children, ChildType ChildType = ChildType.StringType)
{
    public void WriteJson(Utf8JsonWriter writer, bool ignoreKey = false)
    {
        switch (ChildType)
        {
            case ChildType.StringType:

                writer.WriteString(CleanKey(Key), Value);
                break;
            case ChildType.ObjectType:
            {
                if (ignoreKey)
                {
                    writer.WriteStartObject();
                }
                else
                {
                    writer.WriteStartObject(CleanKey(Key));
                }

                foreach (var child in Children)
                {
                    child.WriteJson(writer);
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
                    writer.WriteStartArray(CleanKey(Key));
                }

                foreach (var child in Children)
                {
                    if (child.ChildType == ChildType.StringType)
                    {
                        writer.WriteStringValue(child.Value);
                    }
                    else
                    {
                        child.WriteJson(writer, true);
                    }
                }

                writer.WriteEndArray();
                break;
        }
    }

    public void WriteKeyValues(TextWriter writer, int indent = 0)
    {
        writer.WriteLine($"{string.Empty.PadLeft(indent)}{CleanKey(Key)}: {Value}");
        if (Children.Length > 0)
        {
            foreach (var child in Children)
            {
                child.WriteKeyValues(writer, indent + 4);
            }
        }
    }


    private static string CleanKey(string key)
    {
        // remove non alphabet chars
        // convert to PascalCase
        var sb = new StringBuilder();
        char? last = null;
        foreach (var current in key)
        {
            if (char.IsDigit(current) || char.IsLetter(current))
                sb.Append(
                    last is null || char.IsWhiteSpace(last.Value)
                        ? char.ToUpper(current)
                        : current
                );
            last = current;
        }

        return sb.ToString();
    }
}