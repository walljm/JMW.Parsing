﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using JMW.Parsing;

internal partial class Program
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Options))]
    private static void Main(string[] args)
    {
        Options? options = null;

        Parser.Default.ParseArguments<Options>(args)
               .WithParsed(o =>
               {
                   options = o;
               });

        if (options is null)
        {
            return;
        }

        var parsingOpts = new ParsingOptions(options.UseJson ? OutputType.Json : OutputType.KeyValue);
        if (options.UseIfconfig)
        {
            using var process = new Process();
            process.StartInfo.FileName = "ifconfig";
            process.StartInfo.Arguments = "-a -v";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            var reader = process.StandardOutput;
            Ifconfig.Parse(reader, parsingOpts);
            process.WaitForExit();
        }

    }
}