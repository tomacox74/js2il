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
                var versionProvider = CompilerServices.BuildServiceProvider(new CompilerOptions());
                var versionLogger = versionProvider.GetRequiredService<ILogger>();
                var asm = Assembly.GetExecutingAssembly();
                var name = asm.GetName();
                var version = name.Version?.ToString() ?? "unknown";
                versionLogger.WriteLine($"js2il {version}");
                return;
            }

            var servicesProvider = CompilerServices.BuildServiceProvider(new CompilerOptions
            {
                OutputDirectory = parsed.OutputPath,
                Verbose = parsed.Verbose,
                AnalyzeUnused = parsed.AnalyzeUnused
            });
            var logger = servicesProvider.GetRequiredService<ILogger>();

            // Require InputFile unless in help/version mode (PowerArgs won't enforce without [ArgRequired])
            if (string.IsNullOrWhiteSpace(parsed.InputFile))
            {
                logger.WriteLineError("Error: InputFile is required.");
                PrintUsage(logger);
                Environment.ExitCode = 1;
                return;
            }

            // Validate input file exists (covers cases where user provides a non-existent file)
            if (!File.Exists(parsed.InputFile))
            {
                logger.WriteLineError($"Error: Input file '{parsed.InputFile}' does not exist.");
                Environment.ExitCode = 1;
                return;
            }

            var compiler = servicesProvider.GetRequiredService<Compiler>();
            var success = compiler.Compile(parsed.InputFile);
            Environment.ExitCode = success ? 0 : 1;
        }
        catch (ArgException ex)
        {
            // Args.Parse failed, so we need to create a minimal service provider for logging
            var errorProvider = CompilerServices.BuildServiceProvider(new CompilerOptions());
            var errorLogger = errorProvider.GetRequiredService<ILogger>();
            errorLogger.WriteLineError(ex.Message);
            PrintUsage(errorLogger);
            Environment.ExitCode = 1;
            return;
        }
    }

    // Print usage information using the logger (outputs to stderr for error scenarios)
    private static void PrintUsage(ILogger logger)
    {
        logger.WriteLineError("Usage: js2il <InputFile> [<OutputPath>] [options]");
        logger.WriteLineError("");
        logger.WriteLineError("Option                 Description");
        logger.WriteLineError("-i, --input            The JavaScript file to convert (positional supported)");
        logger.WriteLineError("-o, --output           The output directory for the generated IL (created if missing)");
        logger.WriteLineError("-v, --verbose          Enable verbose output");
        logger.WriteLineError("-a, --analyzeunused    Analyze and report unused properties and methods");
        logger.WriteLineError("--version              Show version information and exit");
        logger.WriteLineError("-h, -?, --help         Show help and exit");
    }
}
