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
    public string? InputFile { get; set; }

    [ArgDescription("Compile an npm/CommonJS module id (e.g. 'turndown' or '@scope/pkg') instead of a file path")]
    [ArgShortcut("--moduleid")]
    public string? ModuleId { get; set; }

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

    [ArgDescription("Emit Portable PDB debug symbols (.pdb) alongside the generated assembly")]
    [ArgShortcut("--pdb")]
    public bool EmitPdb { get; set; }

    [ArgDescription("Strict-mode directive prologue enforcement: Error, Warn, or Ignore")]
    [ArgShortcut("--strictMode")]
    public StrictModeDirectivePrologueMode StrictMode { get; set; } = StrictModeDirectivePrologueMode.Error;

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
                AnalyzeUnused = parsed.AnalyzeUnused,
                EmitPdb = parsed.EmitPdb,
                StrictMode = parsed.StrictMode
            });
            var logger = servicesProvider.GetRequiredService<ILogger>();

            var hasInputFile = !string.IsNullOrWhiteSpace(parsed.InputFile);
            var hasModuleId = !string.IsNullOrWhiteSpace(parsed.ModuleId);

            if (hasInputFile && hasModuleId)
            {
                logger.WriteLineError("Error: Provide either <InputFile> or --moduleid, not both.");
                PrintUsage(logger);
                Environment.ExitCode = 1;
                return;
            }

            if (!hasInputFile && !hasModuleId)
            {
                logger.WriteLineError("Error: Provide <InputFile> or --moduleid.");
                PrintUsage(logger);
                Environment.ExitCode = 1;
                return;
            }

            string entryPath;
            if (hasModuleId)
            {
                // Resolve module id to a physical .js entry file at compile time.
                // Base directory is the current working directory.
                var resolver = servicesProvider.GetRequiredService<Services.NodeModuleResolver>();
                var baseDir = Environment.CurrentDirectory;
                if (!resolver.TryResolve(parsed.ModuleId!, baseDir, out var resolved, out var resolveError))
                {
                    logger.WriteLineError($"Error: Failed to resolve --moduleid '{parsed.ModuleId}': {resolveError}");
                    Environment.ExitCode = 1;
                    return;
                }
                entryPath = resolved;
            }
            else
            {
                entryPath = parsed.InputFile!;
            }

            // Validate entry file exists (covers cases where user provides a non-existent file)
            if (!File.Exists(entryPath))
            {
                logger.WriteLineError($"Error: Input file '{entryPath}' does not exist.");
                Environment.ExitCode = 1;
                return;
            }

            var compiler = servicesProvider.GetRequiredService<Compiler>();
            var success = compiler.Compile(entryPath, rootModuleIdOverride: hasModuleId ? parsed.ModuleId : null);
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
        logger.WriteLineError("   or: js2il --moduleid <ModuleId> [<OutputPath>] [options]");
        logger.WriteLineError("");
        logger.WriteLineError("Option                 Description");
        logger.WriteLineError("-i, --input            The JavaScript file to convert (positional supported)");
        logger.WriteLineError("--moduleid             Compile an npm/CommonJS module id instead of a file path");
        logger.WriteLineError("-o, --output           The output directory for the generated IL (created if missing)");
        logger.WriteLineError("-v, --verbose          Enable verbose output");
        logger.WriteLineError("-a, --analyzeunused    Analyze and report unused properties and methods");
        logger.WriteLineError("--pdb                  Emit Portable PDB debug symbols (.pdb)");
        logger.WriteLineError("--strictMode           Strict-mode directive prologue enforcement: Error, Warn, or Ignore");
        logger.WriteLineError("--version              Show version information and exit");
        logger.WriteLineError("-h, -?, --help         Show help and exit");
    }
}
