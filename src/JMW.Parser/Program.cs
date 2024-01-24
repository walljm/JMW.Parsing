using System.Diagnostics;
using JMW.Parsing;

internal class Program
{
    private static void Main(string[] args)
    {
        using (var process = new Process())
        {
            process.StartInfo.FileName = "ifconfig";
            process.StartInfo.Arguments = "-a -v";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            var reader = process.StandardOutput;

            Ifconfig.Parse(reader, new ParsingOptions(OutputType.KeyValue));

            process.WaitForExit();
        }

        Console.WriteLine("\n\nPress any key to exit.");
    }
}