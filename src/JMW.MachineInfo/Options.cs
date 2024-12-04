using CommandLine;
using JMW.Parsing;

public class Options
{
    [Option('t', "type", Default = OutputType.Table, HelpText = $"Type of output: Json, Table, KeyValue", Required = false)]
    public OutputType OutputType { get; set; }

    [Option('f', "filter", HelpText = $"If using the Table type, this option will allow you to filter results", Required = false)]
    public string? Filter { get; set; }

    [Option("ifconfig", Required = false)]
    public bool UseIfconfig { get; set; }

    [Option("scutildns", Required = false)]
    public bool UseScutilDns { get; set; }
}