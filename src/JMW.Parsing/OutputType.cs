namespace JMW.Parsing;

public enum OutputType
{
    Json,
    Table,
    KeyValue,
}

public enum ChildType
{
    StringType,
    ArrayType,
    ObjectType,
}

public record DisplayOptions(OutputType OutputType, int? ConsoleWidth = null, string? Filter = null);