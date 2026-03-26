namespace JMW.MachineInfo;

public enum OutputType
{
    Json,
    Table,
    KeyValue,
}

public record DisplayOptions(OutputType OutputType, int? ConsoleWidth = null, string? Filter = null);
