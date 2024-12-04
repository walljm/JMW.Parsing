using System.Diagnostics;
using System.Runtime.InteropServices;
using CommandLine;
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

        var parsingOpts = new DisplayOptions(options.OutputType, options.Filter);
        if (options.UseIfconfig)
        {
            using var process = new Process();
            process.StartInfo.FileName = "ifconfig";
            process.StartInfo.Arguments = "-a -v";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            Ifconfig.Parse(process.StandardOutput, Console.Out, parsingOpts);
            process.WaitForExit();
        }
        else if (options.UseScutilDns)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Unable to run scutil --dns on a non MacOS platform.");
            }

            using var process = new Process();
            process.StartInfo.FileName = "scutil";
            process.StartInfo.Arguments = "--dns";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            Ifconfig.Parse(process.StandardOutput, Console.Out, parsingOpts);
            process.WaitForExit();
        }
    }
}