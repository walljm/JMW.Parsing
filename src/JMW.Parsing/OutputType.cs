namespace JMW.Parsing;


public enum OutputType
{
    Json,
    KeyValue
}

internal enum ChildType
{
    StringType,
    ArrayType,
    ObjectType,
}

public record ParsingOptions(OutputType OutputType);