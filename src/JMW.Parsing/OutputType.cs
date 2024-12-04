namespace JMW.Parsing;

public enum OutputType
{
    Json,
    Table,
    KeyValue,
}

internal enum ChildType
{
    StringType,
    ArrayType,
    ObjectType,
}

public record DisplayOptions(OutputType OutputType, string? Filter = null);