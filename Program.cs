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
            var output = reader.ReadToEnd();

            //var ifcs = ParseInterfaces(output);

            var blocks = Parser.GetBlocks(output);
            foreach (var block in blocks)
            {
                Console.WriteLine("");
                using var blockReader = new StringReader(block);
                var tokens = Parser.Tokenize(blockReader).ToArray();
                var pairs = Parser.GetPairs(tokens);
                Console.WriteLine(block);
                foreach(var pair in pairs){
                    Console.WriteLine($"{pair.Key}: {pair.Value}");
                }
            }

            process.WaitForExit();
        }

        Console.WriteLine("\n\nPress any key to exit.");
    }
}