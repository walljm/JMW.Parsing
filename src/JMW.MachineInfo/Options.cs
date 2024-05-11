using CommandLine;

public class Options
{
    [Option("json", Required = false)] public bool UseJson { get; set; }

    [Option("ifconfig", Required = false)] public bool UseIfconfig { get; set; }
    [Option("scutildns", Required = false)] public bool UseScutilDns { get; set; }
}