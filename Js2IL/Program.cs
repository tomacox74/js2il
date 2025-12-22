using Microsoft.Extensions.DependencyInjection;
using PowerArgs;
using System.Reflection;

namespace Js2IL;

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class Js2ILArgs
{
    [ArgPosition(0)]
    [ArgDescription("The JavaScript file to convert")]
    [ArgShortcut("--input")]
    [ArgShortcut("i")]
    public required string InputFile { get; set; }

    [ArgPosition(1)]
    [ArgDescription("The output directory for the generated IL")]
    [ArgShortcut("--output")]
    [ArgShortcut("o")]
    public string? OutputPath { get; set; }

    [ArgDescription("Enable verbose output")]
    [ArgShortcut("v")]
    public bool Verbose { get; set; }

    [ArgDescription("Analyze and report unused properties and methods")]
    [ArgShortcut("a")]
    public bool AnalyzeUnused { get; set; }

    [ArgDescription("Show version information and exit")]
    [ArgShortcut("--version")]
    public bool Version { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var parsed = Args.Parse<Js2ILArgs>(args);
            if (parsed == null)
            {
                // HelpHook likely handled output; treat as successful exit
                return;
            }

            // Version handling (PowerArgs default alias is -Version from property name)
            if (parsed.Version)
            {
                var asm = Assembly.GetExecutingAssembly();
                var name = asm.GetName();
                var version = name.Version?.ToString() ?? "unknown";
                Console.WriteLine($"js2il {version}");
                return;
            }

            // Require InputFile unless in help/version mode (PowerArgs won't enforce without [ArgRequired])
            if (string.IsNullOrWhiteSpace(parsed.InputFile))
            {
                Logger.WriteLineError("Error: InputFile is required.");
                PrintUsage(Console.Error);
                Environment.ExitCode = 1;
                return;
            }

            // Validate input file exists (covers cases where user provides a non-existent file)
            if (!File.Exists(parsed.InputFile))
            {
                Logger.WriteLineError($"Error: Input file '{parsed.InputFile}' does not exist.");
                Environment.ExitCode = 1;
                return;
            }

            var servicesProvider = CompilerServices.BuildServiceProvider(new CompilerOptions
            {
                OutputDirectory = parsed.OutputPath,
                Verbose = parsed.Verbose,
                AnalyzeUnused = parsed.AnalyzeUnused
            });
            var compiler = servicesProvider.GetRequiredService<Compiler>();
            var success = compiler.Compile(parsed.InputFile);
            Environment.ExitCode = success ? 0 : 1;
        }
        catch (ArgException ex)
        {
            Logger.WriteLineError(ex.Message);
            // Avoid potential template generation issues by printing a simple usage message
            PrintUsage(Console.Error);
            Environment.ExitCode = 1;
            return;
        }
    }

    // Overload to print usage to a chosen writer (used for error cases to stderr)
    private static void PrintUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: js2il <InputFile> [<OutputPath>] [options]");
        writer.WriteLine();
        writer.WriteLine("Option                 Description");
        writer.WriteLine("-i, --input            The JavaScript file to convert (positional supported)");
        writer.WriteLine("-o, --output           The output directory for the generated IL (created if missing)");
        writer.WriteLine("-v, --verbose          Enable verbose output");
        writer.WriteLine("-a, --analyzeunused    Analyze and report unused properties and methods");
        writer.WriteLine("--version              Show version information and exit");
        writer.WriteLine("-h, -?, --help         Show help and exit");
    }
}
