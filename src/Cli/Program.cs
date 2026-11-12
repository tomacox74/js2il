using Microsoft.Extensions.DependencyInjection;
using PowerArgs;
using System.Reflection;

namespace Jroc;

[ArgExceptionBehavior(ArgExceptionPolicy.DontHandleExceptions)]
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
    [ArgShortcut("a")]
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

    [ArgDescription("Target standalone output to net10.0 or net11.0 (default: the runtime running JROC)")]
    [ArgShortcut("--target-framework")]
    public string? TargetFramework { get; set; }

    [ArgDescription("Use a Microsoft.NETCore.App.Ref pack version directory (requires --target-framework)")]
    [ArgShortcut("--targeting-pack")]
    public string? TargetingPack { get; set; }

    [ArgDescription("Analyze and report unused properties and methods")]
    [ArgShortcut("--analyzeunused")]
    public bool AnalyzeUnused { get; set; }

    [ArgDescription("Emit Portable PDB debug symbols (.pdb) alongside the generated assembly")]
    [ArgShortcut("--pdb")]
    public bool EmitPdb { get; set; }

    [ArgDescription("Assume the host leaves built-in global bindings unchanged during execution (enables guard-free calls when source analysis permits)")]
    [ArgShortcut("--assume-unmodified-host-globals")]
    public bool AssumeUnmodifiedHostGlobals { get; set; }

    [ArgDescription("Analyze compilation coverage without writing or running the assembly")]
    [ArgShortcut("--coverage")]
    public bool Coverage { get; set; }

    [ArgDescription("Print machine-readable JSON for coverage analysis")]
    [ArgShortcut("--json")]
    public bool Json { get; set; }

    [ArgDescription("Write a compilation coverage JSON report (also supported during normal compilation)")]
    [ArgShortcut("--coverage-report")]
    public string? CoverageReport { get; set; }

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
            var coverageCommand = args.Length > 0 && args[0] == "coverage";
            if (coverageCommand)
            {
                args = args[1..];
            }
            if (args.Length == 1 && args[0] is "-h" or "-?" or "--help")
            {
                // Avoid PowerArgs' interactive console renderer for redirected output.
                PrintUsage(Console.WriteLine);
                return;
            }
            var remainingArgs = ExtractAdditionalInputs(args, out var additionalInputs);
            var parsed = Args.Parse<JrocArgs>(remainingArgs);
            if (parsed == null)
            {
                // PowerArgs hooks can cancel parsing without reporting an error.
                return;
            }

            if (parsed.AdditionalInput is not null)
            {
                throw new ArgException("Use -a <file> or --additional-input <file> for each additional entry.");
            }

            var coverageOnly = coverageCommand || parsed.Coverage;
            if (parsed.Json && !coverageOnly)
            {
                throw new ArgException("--json requires the coverage command or --coverage.");
            }
            if (parsed.Json && parsed.Verbose)
            {
                throw new ArgException("--json cannot be combined with verbose console diagnostics.");
            }
            if (parsed.Json && parsed.AnalyzeUnused)
            {
                throw new ArgException("--json cannot be combined with unused-analysis console output.");
            }
            if (coverageOnly && !string.IsNullOrWhiteSpace(parsed.OutputPath))
            {
                throw new ArgException("Coverage analysis does not write assemblies; use --coverage-report <file> for its JSON output.");
            }
            if (coverageOnly && (parsed.TargetFramework is not null || parsed.TargetingPack is not null))
            {
                throw new ArgException("--target-framework and --targeting-pack apply to standalone output, not coverage-only analysis.");
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
                EmitPdb = parsed.EmitPdb,
                CollectCompilationCoverage = !string.IsNullOrWhiteSpace(parsed.CoverageReport),
                AssumeUnmodifiedHostGlobals = parsed.AssumeUnmodifiedHostGlobals,
                TargetFrameworkReferences = ResolveTargetFrameworkReferences(parsed.TargetFramework, parsed.TargetingPack)
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
            if (coverageOnly)
            {
                var entries = new List<JrocInMemoryEntrySource>
                {
                    new(entryPath, RootModuleIdOverride: hasModuleId ? parsed.ModuleId : null)
                };
                entries.AddRange(additionalInputs.Select(path => new JrocInMemoryEntrySource(path)));
                var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(
                    new JrocInMemoryMultiEntryCompileRequest(entries)
                    {
                        AssemblyName = parsed.AssemblyName,
                        Verbose = parsed.Verbose,
                        AnalyzeUnused = parsed.AnalyzeUnused,
                        EmitPdb = parsed.EmitPdb,
                        AssumeUnmodifiedHostGlobals = parsed.AssumeUnmodifiedHostGlobals
                    }, logger);
                Console.WriteLine(parsed.Json ? analysis.Report.ToJson() : analysis.Report.ToText());
                var written = WriteCoverageReport(parsed.CoverageReport, analysis.Report, logger);
                Environment.ExitCode = analysis.Report.CompilationSucceeded && written ? 0 : 1;
                return;
            }
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
            var reportWritten = string.IsNullOrWhiteSpace(parsed.CoverageReport)
                || (compiler.CompilationCoverage is { } report
                    && WriteCoverageReport(parsed.CoverageReport, report, logger));
            Environment.ExitCode = success && reportWritten ? 0 : 1;
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

    private static AssemblyName[] ResolveTargetFrameworkReferences(string? targetFramework, string? targetingPack)
    {
        if (targetFramework is null)
        {
            if (targetingPack is not null)
            {
                throw new ArgException("--targeting-pack requires --target-framework net10.0 or net11.0.");
            }
            return [];
        }

        var major = targetFramework switch
        {
            "net10.0" => 10,
            "net11.0" => 11,
            _ => throw new ArgException(
                $"Unsupported target framework '{targetFramework}'. Use --target-framework net10.0 or net11.0.")
        };
        var remedy = $"Supply a Microsoft.NETCore.App.Ref pack for {targetFramework} " +
            $"with --targeting-pack <version-directory> containing ref/{targetFramework}/System.Runtime.dll.";

        try
        {
            string? pack = targetingPack is not null ? Path.GetFullPath(targetingPack) : null;
            if (targetingPack is null)
            {
                var runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                var dotnetRoot = Path.GetFullPath(Path.Combine(runtimeDirectory, "..", "..", ".."));
                var packsDirectory = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");
                if (Directory.Exists(packsDirectory))
                {
                    pack = Directory.EnumerateDirectories(packsDirectory)
                        .Select(path =>
                        {
                            Version.TryParse(Path.GetFileName(path).Split('-')[0], out var version);
                            return (Path: path, Version: version);
                        })
                        .Where(candidate => candidate.Version?.Major == major
                            && Directory.Exists(Path.Combine(candidate.Path, "ref", targetFramework)))
                        .OrderByDescending(candidate => candidate.Version)
                        .ThenByDescending(candidate => !Path.GetFileName(candidate.Path).Contains('-'))
                        .ThenByDescending(candidate => candidate.Path, StringComparer.Ordinal)
                        .Select(candidate => candidate.Path)
                        .FirstOrDefault();
                }
                if (pack is null)
                {
                    throw new ArgException(
                        $"No {targetFramework} targeting pack was found in '{packsDirectory}'. {remedy}");
                }
            }

            var referenceDirectory = Path.Combine(pack!, "ref", targetFramework);
            if (!Directory.Exists(referenceDirectory))
            {
                throw new ArgException(
                    $"The {targetFramework} targeting pack reference directory '{referenceDirectory}' does not exist. {remedy}");
            }
            var references = Directory.EnumerateFiles(referenceDirectory, "*.dll")
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(AssemblyName.GetAssemblyName)
                .ToArray();
            var systemRuntime = references.FirstOrDefault(reference => reference.Name == "System.Runtime");
            var expectedVersion = new Version(major, 0, 0, 0);
            if (systemRuntime?.Version != expectedVersion)
            {
                throw new ArgException(
                    $"Targeting pack '{pack}' must supply System.Runtime {expectedVersion} for {targetFramework}; " +
                    $"found {systemRuntime?.Version?.ToString() ?? "no System.Runtime reference"}. {remedy}");
            }
            var manifest = System.Xml.Linq.XDocument.Load(Path.Combine(pack!, "data", "FrameworkList.xml"));
            var framework = manifest.Root;
            if (framework?.Attribute("TargetFrameworkIdentifier")?.Value != ".NETCoreApp"
                || framework.Attribute("TargetFrameworkVersion")?.Value != $"{major}.0")
            {
                throw new ArgException($"Targeting pack '{pack}' does not describe the {targetFramework} framework. {remedy}");
            }
            // Missing framework DLLs would otherwise fall back to the executing CLR's identities.
            foreach (var file in framework.Elements("File").Where(file => file.Attribute("Type")?.Value == "Managed"))
            {
                var relativePath = file.Attribute("Path")?.Value;
                if (relativePath is null || !File.Exists(Path.Combine(pack!, relativePath)))
                {
                    throw new ArgException(
                        $"Targeting pack '{pack}' is incomplete: missing framework reference '{relativePath}'. {remedy}");
                }
            }
            return references;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
            or ArgumentException or BadImageFormatException or NotSupportedException or System.Xml.XmlException)
        {
            throw new ArgException($"Could not read the {targetFramework} targeting pack: {exception.Message} {remedy}");
        }
    }

    private static bool WriteCoverageReport(string? requestedPath, CompilationCoverageReport report, ICompilerOutput logger)
    {
        if (string.IsNullOrWhiteSpace(requestedPath))
        {
            return true;
        }
        try
        {
            var path = Path.GetFullPath(requestedPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, report.ToJson(), new System.Text.UTF8Encoding(false));
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            logger.WriteLineError($"Error writing compilation coverage report: {exception.Message}");
            return false;
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
                arg.Equals("-AdditionalInput", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-a", StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Length || args[index + 1].StartsWith('-') || string.IsNullOrWhiteSpace(args[index + 1]))
                {
                    throw new ArgException("--additional-input requires a file path.");
                }

                additionalInputs.Add(args[++index]);
            }
            else if (arg.StartsWith("--additional-input=", StringComparison.OrdinalIgnoreCase) ||
                     arg.StartsWith("-a=", StringComparison.OrdinalIgnoreCase) ||
                     arg.StartsWith("/AdditionalInput:", StringComparison.OrdinalIgnoreCase))
            {
                var prefixLength = arg[0] == '/'
                    ? "/AdditionalInput:".Length
                    : arg.StartsWith("-a=", StringComparison.OrdinalIgnoreCase)
                        ? "-a=".Length
                        : "--additional-input=".Length;
                var path = arg[prefixLength..];
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
        => PrintUsage(logger.WriteLineError);

    private static void PrintUsage(Action<string> writeLine)
    {
        writeLine("Usage: jroc <InputFile> [<OutputPath>] [options]");
        writeLine("   or: jroc --moduleid <ModuleId> [--output <directory>] [options]");
        writeLine("   or: jroc coverage <InputFile> [--json] [--coverage-report <file>]");
        writeLine("");
        writeLine("Option                 Description");
        writeLine("-i, --input            The JavaScript file to convert (positional supported)");
        writeLine("--moduleid             Compile an npm/CommonJS module id instead of a file path");
        writeLine("-a, --additional-input <file> Add another input (repeatable; incompatible with --moduleid)");
        writeLine("--assemblyname <name>   Set the assembly identity and artifact basename");
        writeLine("-o, --output           The output directory for the generated IL (created if missing)");
        writeLine("-v, --verbose          Enable diagnostics output to console");
        writeLine("--diagnostic-file <path> Write diagnostics output to a text file");
        writeLine("--target-framework <tfm> Target net10.0 or net11.0 (default: the runtime running JROC)");
        writeLine("--targeting-pack <path> Use a Microsoft.NETCore.App.Ref pack version directory (requires --target-framework)");
        writeLine("--analyzeunused        Report unused functions, properties, and variables");
        writeLine("--pdb                  Emit Portable PDB debug symbols (.pdb)");
        writeLine("--assume-unmodified-host-globals  Allow guard-free calls when analysis permits and the host preserves built-in global bindings");
        writeLine("--coverage             Analyze compilation modes without writing assemblies");
        writeLine("--json                 Print coverage analysis as JSON");
        writeLine("--coverage-report <file> Write compilation-mode JSON during analysis or normal compilation");
        writeLine("--version              Show version information and exit");
        writeLine("-h, -?, --help         Show help and exit");
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
