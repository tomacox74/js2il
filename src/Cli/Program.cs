using Microsoft.Extensions.DependencyInjection;
using PowerArgs;
using System.Reflection;

namespace Jroc;

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class JrocArgs
{
    [ArgPosition(0)]
    [ArgDescription("The JavaScript file to convert")]
    [ArgShortcut("--input")]
    [ArgShortcut("i")]
    public string? InputFile { get; set; }

    [ArgDescription("Compile an npm/CommonJS module id (e.g. 'turndown' or '@scope/pkg') instead of a file path")]
    [ArgShortcut("--moduleid")]
    public string? ModuleId { get; set; }

    [ArgDescription("Add a JavaScript input file to the assembly (repeat for each additional file)")]
    [ArgShortcut("--additional-input")]
    public string? AdditionalInput { get; set; }

    [ArgDescription("Set the generated assembly identity and artifact basename")]
    [ArgShortcut("--assemblyname")]
    public string? AssemblyName { get; set; }

    [ArgPosition(1)]
    [ArgDescription("The output directory for the generated IL")]
    [ArgShortcut("--output")]
    [ArgShortcut("o")]
    public string? OutputPath { get; set; }

    [ArgDescription("Enable diagnostics output to console")]
    [ArgShortcut("v")]
    public bool Verbose { get; set; }

    [ArgDescription("Write diagnostics to a text file (opt-in)")]
    [ArgShortcut("--diagnostic-file")]
    public string? DiagnosticFile { get; set; }

    [ArgDescription("Analyze and report unused properties and methods")]
    [ArgShortcut("a")]
    public bool AnalyzeUnused { get; set; }

    [ArgDescription("Emit Portable PDB debug symbols (.pdb) alongside the generated assembly")]
    [ArgShortcut("--pdb")]
    public bool EmitPdb { get; set; }

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
            var remainingArgs = ExtractAdditionalInputs(args, out var additionalInputs);
            var parsed = Args.Parse<JrocArgs>(remainingArgs);
            if (parsed == null)
            {
                // HelpHook likely handled output; treat as successful exit
                return;
            }

            if (parsed.AdditionalInput is not null)
            {
                throw new ArgException("Use --additional-input <file> for each additional entry.");
            }

            // Version handling (PowerArgs default alias is -Version from property name)
            if (parsed.Version)
            {
                using var versionProvider = CompilerServices.BuildServiceProvider(new CompilerOptions());
                var versionLogger = versionProvider.GetRequiredService<ICompilerOutput>();
                var asm = Assembly.GetExecutingAssembly();
                var name = asm.GetName();
                var version = name.Version?.ToString() ?? "unknown";
                versionLogger.WriteLine($"jroc {version}");
                return;
            }

            if (!TryValidateDiagnosticFilePath(parsed.DiagnosticFile, out var diagnosticFilePath, out var diagnosticFileError))
            {
                Console.Error.WriteLine($"Error: {diagnosticFileError}");
                Environment.ExitCode = 1;
                return;
            }

            using var servicesProvider = CompilerServices.BuildServiceProvider(new CompilerOptions
            {
                AssemblyName = parsed.AssemblyName,
                OutputDirectory = parsed.OutputPath,
                Verbose = parsed.Verbose,
                DiagnosticFilePath = diagnosticFilePath,
                AnalyzeUnused = parsed.AnalyzeUnused,
                EmitPdb = parsed.EmitPdb
            });
            var logger = servicesProvider.GetRequiredService<ICompilerOutput>();

            var hasInputFile = !string.IsNullOrWhiteSpace(parsed.InputFile);
            var hasModuleId = !string.IsNullOrWhiteSpace(parsed.ModuleId);

            if (hasInputFile && hasModuleId)
            {
                logger.WriteLineError("Error: Provide either <InputFile> or --moduleid, not both.");
                PrintUsage(logger);
                Environment.ExitCode = 1;
                return;
            }

            if (hasModuleId && additionalInputs.Count > 0)
            {
                logger.WriteLineError("Error: --additional-input cannot be used with --moduleid.");
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

            foreach (var additionalInput in additionalInputs)
            {
                if (!File.Exists(additionalInput))
                {
                    logger.WriteLineError($"Error: Additional input file '{additionalInput}' does not exist.");
                    Environment.ExitCode = 1;
                    return;
                }
            }

            var compiler = servicesProvider.GetRequiredService<Compiler>();
            bool success;
            if (additionalInputs.Count == 0)
            {
                success = compiler.Compile(entryPath, rootModuleIdOverride: hasModuleId ? parsed.ModuleId : null);
            }
            else
            {
                var entries = new List<JrocCompileEntry> { new(entryPath) };
                entries.AddRange(additionalInputs.Select(path => new JrocCompileEntry(path)));
                success = compiler.Compile(entries);
            }
            Environment.ExitCode = success ? 0 : 1;
        }
        catch (ArgException ex)
        {
            // Args.Parse failed, so we need to create a minimal service provider for logging
            using var errorProvider = CompilerServices.BuildServiceProvider(new CompilerOptions());
            var errorLogger = errorProvider.GetRequiredService<ICompilerOutput>();
            errorLogger.WriteLineError(ex.Message);
            PrintUsage(errorLogger);
            Environment.ExitCode = 1;
            return;
        }
    }

    private static string[] ExtractAdditionalInputs(string[] args, out List<string> additionalInputs)
    {
        var remaining = new List<string>(args.Length);
        additionalInputs = new List<string>();

        // PowerArgs rejects repeated option keys and consumes trailing positional values for list properties.
        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            if (arg.Equals("--additional-input", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-AdditionalInput", StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Length || args[index + 1].StartsWith('-') || string.IsNullOrWhiteSpace(args[index + 1]))
                {
                    throw new ArgException("--additional-input requires a file path.");
                }

                additionalInputs.Add(args[++index]);
            }
            else if (arg.StartsWith("--additional-input=", StringComparison.OrdinalIgnoreCase) ||
                     arg.StartsWith("/AdditionalInput:", StringComparison.OrdinalIgnoreCase))
            {
                var path = arg[(arg[0] == '/' ? "/AdditionalInput:".Length : "--additional-input=".Length)..];
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgException("--additional-input requires a file path.");
                }

                additionalInputs.Add(path);
            }
            else
            {
                remaining.Add(arg);
            }
        }

        return remaining.ToArray();
    }

    // Print usage information using the logger (outputs to stderr for error scenarios)
    private static void PrintUsage(ICompilerOutput logger)
    {
        logger.WriteLineError("Usage: jroc <InputFile> [<OutputPath>] [options]");
        logger.WriteLineError("   or: jroc --moduleid <ModuleId> [<OutputPath>] [options]");
        logger.WriteLineError("");
        logger.WriteLineError("Option                 Description");
        logger.WriteLineError("-i, --input            The JavaScript file to convert (positional supported)");
        logger.WriteLineError("--moduleid             Compile an npm/CommonJS module id instead of a file path");
        logger.WriteLineError("--additional-input <file> Add another input to the assembly (repeatable; incompatible with --moduleid)");
        logger.WriteLineError("--assemblyname <name>   Set the assembly identity and artifact basename");
        logger.WriteLineError("-o, --output           The output directory for the generated IL (created if missing)");
        logger.WriteLineError("-v, --verbose          Enable diagnostics output to console");
        logger.WriteLineError("--diagnostic-file <path> Write diagnostics output to a text file");
        logger.WriteLineError("-a, --analyzeunused    Analyze and report unused properties and methods");
        logger.WriteLineError("--pdb                  Emit Portable PDB debug symbols (.pdb)");
        logger.WriteLineError("--version              Show version information and exit");
        logger.WriteLineError("-h, -?, --help         Show help and exit");
    }

    private static bool TryValidateDiagnosticFilePath(
        string? diagnosticFilePath,
        out string? normalizedDiagnosticFilePath,
        out string? error)
    {
        normalizedDiagnosticFilePath = null;
        error = null;

        if (string.IsNullOrWhiteSpace(diagnosticFilePath))
        {
            return true;
        }

        try
        {
            var fullPath = Path.GetFullPath(diagnosticFilePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var _ = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            normalizedDiagnosticFilePath = fullPath;
            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException
                                    || ex is ArgumentException
                                    || ex is PathTooLongException
                                    || ex is NotSupportedException
                                    || ex is DriveNotFoundException
                                    || ex is IOException)
        {
            error = $"Cannot write diagnostics file '{diagnosticFilePath}': {ex.Message}";
            return false;
        }
    }
}
