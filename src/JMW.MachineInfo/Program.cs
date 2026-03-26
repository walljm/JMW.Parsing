using System.Diagnostics;
using System.Runtime.InteropServices;
using CommandLine;
using JMW.MachineInfo;
using JMW.Parsing;

public class Program
{
    private static void Main(string[] args)
    {
        Options? options = null;

        Parser
           .Default
           .ParseArguments<Options>(args)
           .WithParsed(
                o =>
                {
                    options = o;
                }
            );

        if (options is null)
        {
            return;
        }

        if (options.UseIfconfig)
        {
            using var process = new Process();
            process.StartInfo.FileName = "ifconfig";
            process.StartInfo.Arguments = "-a -v";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            using var reader = new StringReader(output);
            switch (options.OutputType)
            {
                case OutputType.Json:
                    Ifconfig.OutputJson(reader, Console.Out);
                    break;
                case OutputType.KeyValue:
                    Ifconfig.OutputKeyValues(reader, Console.Out);
                    break;
                case OutputType.Table:
                    var displayOpts = new DisplayOptions(options.OutputType, Console.BufferWidth, options.Filter);
                    IfconfigTableRenderer.OutputTable(reader, Console.Out, displayOpts);
                    break;
            }
        }
        else if (options.UseScutilDns)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.Error.WriteLine("Unable to run scutil --dns on a non MacOS platform.");
                return;
            }

            using var process = new Process();
            process.StartInfo.FileName = "scutil";
            process.StartInfo.Arguments = "--dns";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            using var reader = new StringReader(output);
            switch (options.OutputType)
            {
                case OutputType.Json:
                    ScutilDns.OutputJson(reader, Console.Out);
                    break;
                case OutputType.KeyValue:
                    ScutilDns.OutputKeyValues(reader, Console.Out);
                    break;
                case OutputType.Table:
                    Console.Error.WriteLine("Table output is not supported for scutil --dns. Use Json or KeyValue.");
                    break;
            }
        }
        else
        {
            Console.Error.WriteLine("No command specified. Use --ifconfig or --scutildns.");
            Console.Error.WriteLine("Use --help for more information.");
        }
    }
}