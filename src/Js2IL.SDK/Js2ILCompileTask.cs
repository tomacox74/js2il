using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Js2IL.Services;

namespace Js2IL.SDK.BuildTasks;

public sealed class Js2ILCompileTask : Microsoft.Build.Utilities.Task
{
    private static readonly string[] KnownJavaScriptSourceExtensions = [".js", ".mjs", ".cjs"];

    [Required]
    public ITaskItem[] Sources { get; set; } = [];

    [Output]
    public ITaskItem[] GeneratedAssemblies { get; private set; } = [];

    [Output]
    public ITaskItem[] GeneratedOutputs { get; private set; } = [];

    public override bool Execute()
    {
        var generatedAssemblies = new List<ITaskItem>();
        var generatedOutputs = new List<ITaskItem>();

        foreach (var source in Sources)
        {
            if (!TryCompileSource(source, generatedAssemblies, generatedOutputs))
            {
                continue;
            }
        }

        GeneratedAssemblies = generatedAssemblies.ToArray();
        GeneratedOutputs = generatedOutputs.ToArray();
        return !Log.HasLoggedErrors;
    }

    private bool TryCompileSource(
        ITaskItem source,
        List<ITaskItem> generatedAssemblies,
        List<ITaskItem> generatedOutputs)
    {
        var sourceSpecifier = source.ItemSpec;
        var sourcePath = GetMetadataOrFallback(source, "ResolvedSourcePath", () => Path.GetFullPath(source.ItemSpec));
        var moduleResolutionBaseDirectory = GetMetadataOrFallback(source, "ResolvedModuleResolutionBaseDirectory", () => Environment.CurrentDirectory);
        var rootModuleId = source.GetMetadata("RootModuleId");

        if (!File.Exists(sourcePath))
        {
            if (!LooksLikeModuleIdSpecifier(sourceSpecifier))
            {
                Log.LogError($"Js2IL source '{sourcePath}' does not exist.");
                return false;
            }

            var resolver = new NodeModuleResolver(new FileSystem());
            if (!resolver.TryResolve(sourceSpecifier, moduleResolutionBaseDirectory, out var resolvedSourcePath, out var resolutionError))
            {
                Log.LogError($"Js2IL could not resolve module id '{sourceSpecifier}' from '{moduleResolutionBaseDirectory}': {resolutionError}");
                return false;
            }

            sourcePath = resolvedSourcePath;
            if (string.IsNullOrWhiteSpace(rootModuleId))
            {
                rootModuleId = sourceSpecifier;
            }
        }

        if (!File.Exists(sourcePath))
        {
            Log.LogError($"Js2IL source '{sourcePath}' does not exist.");
            return false;
        }

        if (!TryGetRequiredMetadata(source, "ResolvedOutputDirectory", out var outputDirectory))
        {
            return false;
        }

        if (!TryGetBooleanMetadata(source, "Verbose", out var verbose)
            || !TryGetBooleanMetadata(source, "AnalyzeUnused", out var analyzeUnused)
            || !TryGetBooleanMetadata(source, "EmitPdb", out var emitPdb)
            || !TryGetBooleanMetadata(source, "ReferenceOutputAssembly", out var referenceOutputAssembly)
            || !TryGetBooleanMetadata(source, "CopyToOutputDirectory", out var copyToOutputDirectory))
        {
            return false;
        }

        if (!TryGetStrictMode(source, out var strictMode))
        {
            return false;
        }

        var diagnosticFilePath = source.GetMetadata("ResolvedDiagnosticFilePath");
        if (!EnsureDiagnosticDirectoryExists(diagnosticFilePath))
        {
            return false;
        }

        var compilerOptions = new CompilerOptions
        {
            OutputDirectory = outputDirectory,
            Verbose = verbose,
            DiagnosticFilePath = string.IsNullOrWhiteSpace(diagnosticFilePath) ? null : diagnosticFilePath,
            AnalyzeUnused = analyzeUnused,
            EmitPdb = emitPdb,
            StrictMode = strictMode
        };

        var logger = new MsBuildCompilerOutput(Log, sourcePath);

        using var services = CompilerServices.BuildServiceProvider(compilerOptions, fileSystem: null, compilerOutput: logger);
        var compiler = services.GetRequiredService<Compiler>();
        var success = compiler.Compile(
            sourcePath,
            rootModuleIdOverride: string.IsNullOrWhiteSpace(rootModuleId) ? null : rootModuleId);

        if (!success)
        {
            return false;
        }

        var assemblyName = Path.GetFileNameWithoutExtension(sourcePath);
        var assemblyPath = Path.Combine(outputDirectory, assemblyName + ".dll");
        var assemblyPdbPath = Path.Combine(outputDirectory, assemblyName + ".pdb");
        var runtimeConfigPath = Path.Combine(outputDirectory, assemblyName + ".runtimeconfig.json");
        var runtimeAssemblyPath = Path.Combine(outputDirectory, "JavaScriptRuntime.dll");
        var runtimePdbPath = Path.Combine(outputDirectory, "JavaScriptRuntime.pdb");

        if (!File.Exists(assemblyPath))
        {
            Log.LogError($"Js2IL did not produce the expected assembly '{assemblyPath}' for source '{sourcePath}'.");
            return false;
        }

        if (!File.Exists(runtimeConfigPath))
        {
            Log.LogError($"Js2IL did not produce the expected runtime config '{runtimeConfigPath}' for source '{sourcePath}'.");
            return false;
        }

        if (!File.Exists(runtimeAssemblyPath))
        {
            Log.LogError($"Js2IL did not produce the expected runtime support assembly '{runtimeAssemblyPath}' for source '{sourcePath}'.");
            return false;
        }

        var assemblyItem = new TaskItem(assemblyPath);
        PopulateSharedMetadata(
            assemblyItem,
            sourcePath,
            outputDirectory,
            rootModuleId,
            referenceOutputAssembly,
            copyToOutputDirectory);

        assemblyItem.SetMetadata("AssemblyName", assemblyName);
        assemblyItem.SetMetadata("RuntimeConfigPath", runtimeConfigPath);
        assemblyItem.SetMetadata("RuntimeAssemblyPath", runtimeAssemblyPath);
        assemblyItem.SetMetadata("PdbPath", File.Exists(assemblyPdbPath) ? assemblyPdbPath : string.Empty);
        generatedAssemblies.Add(assemblyItem);

        generatedOutputs.Add(CreateOutputItem(
            assemblyPath,
            kind: "Assembly",
            sourcePath,
            outputDirectory,
            rootModuleId,
            referenceOutputAssembly,
            copyToOutputDirectory));

        generatedOutputs.Add(CreateOutputItem(
            runtimeConfigPath,
            kind: "RuntimeConfig",
            sourcePath,
            outputDirectory,
            rootModuleId,
            referenceOutputAssembly,
            copyToOutputDirectory));

        generatedOutputs.Add(CreateOutputItem(
            runtimeAssemblyPath,
            kind: "RuntimeAssembly",
            sourcePath,
            outputDirectory,
            rootModuleId,
            referenceOutputAssembly,
            copyToOutputDirectory));

        if (File.Exists(assemblyPdbPath))
        {
            generatedOutputs.Add(CreateOutputItem(
                assemblyPdbPath,
                kind: "AssemblyPdb",
                sourcePath,
                outputDirectory,
                rootModuleId,
                referenceOutputAssembly,
                copyToOutputDirectory));
        }

        if (File.Exists(runtimePdbPath))
        {
            generatedOutputs.Add(CreateOutputItem(
                runtimePdbPath,
                kind: "RuntimePdb",
                sourcePath,
                outputDirectory,
                rootModuleId,
                referenceOutputAssembly,
                copyToOutputDirectory));
        }

        return true;
    }

    private static bool LooksLikeModuleIdSpecifier(string specifier)
    {
        if (string.IsNullOrWhiteSpace(specifier))
        {
            return false;
        }

        var trimmed = specifier.Trim();
        if (trimmed.StartsWith("./", StringComparison.Ordinal)
            || trimmed.StartsWith("../", StringComparison.Ordinal)
            || trimmed.StartsWith("/", StringComparison.Ordinal)
            || Path.IsPathRooted(trimmed)
            || trimmed.Contains('\\'))
        {
            return false;
        }

        var extension = Path.GetExtension(trimmed);
        return !KnownJavaScriptSourceExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private bool EnsureDiagnosticDirectoryExists(string? diagnosticFilePath)
    {
        if (string.IsNullOrWhiteSpace(diagnosticFilePath))
        {
            return true;
        }

        try
        {
            var directory = Path.GetDirectoryName(diagnosticFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return true;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException
                                    || ex is ArgumentException
                                    || ex is PathTooLongException
                                    || ex is NotSupportedException
                                    || ex is DriveNotFoundException
                                    || ex is IOException)
        {
            Log.LogError($"Cannot create the diagnostic output directory for '{diagnosticFilePath}': {ex.Message}");
            return false;
        }
    }

    private bool TryGetStrictMode(ITaskItem source, out StrictModeDirectivePrologueMode strictMode)
    {
        var raw = source.GetMetadata("StrictMode");
        if (string.IsNullOrWhiteSpace(raw))
        {
            strictMode = StrictModeDirectivePrologueMode.Error;
            return true;
        }

        if (Enum.TryParse(raw, ignoreCase: true, out strictMode))
        {
            return true;
        }

        Log.LogError($"Invalid StrictMode value '{raw}' for Js2IL source '{source.ItemSpec}'. Use Error, Warn, or Ignore.");
        return false;
    }

    private bool TryGetBooleanMetadata(ITaskItem item, string metadataName, out bool value)
    {
        var raw = item.GetMetadata(metadataName);
        if (string.IsNullOrWhiteSpace(raw))
        {
            value = false;
            return true;
        }

        if (bool.TryParse(raw, out value))
        {
            return true;
        }

        Log.LogError($"Invalid boolean value '{raw}' for Js2IL source '{item.ItemSpec}' metadata '{metadataName}'.");
        return false;
    }

    private bool TryGetRequiredMetadata(ITaskItem item, string metadataName, out string value)
    {
        value = item.GetMetadata(metadataName);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        Log.LogError($"Missing required Js2IL metadata '{metadataName}' for source '{item.ItemSpec}'.");
        return false;
    }

    private static string GetMetadataOrFallback(ITaskItem item, string metadataName, Func<string> fallback)
    {
        var value = item.GetMetadata(metadataName);
        return string.IsNullOrWhiteSpace(value) ? fallback() : value;
    }

    private static ITaskItem CreateOutputItem(
        string path,
        string kind,
        string sourcePath,
        string outputDirectory,
        string rootModuleId,
        bool referenceOutputAssembly,
        bool copyToOutputDirectory)
    {
        var item = new TaskItem(path);
        PopulateSharedMetadata(item, sourcePath, outputDirectory, rootModuleId, referenceOutputAssembly, copyToOutputDirectory);
        item.SetMetadata("Kind", kind);
        return item;
    }

    private static void PopulateSharedMetadata(
        ITaskItem item,
        string sourcePath,
        string outputDirectory,
        string rootModuleId,
        bool referenceOutputAssembly,
        bool copyToOutputDirectory)
    {
        item.SetMetadata("SourcePath", sourcePath);
        item.SetMetadata("OutputDirectory", outputDirectory);
        item.SetMetadata("RootModuleId", rootModuleId ?? string.Empty);
        item.SetMetadata("ReferenceOutputAssembly", referenceOutputAssembly.ToString());
        item.SetMetadata("CopyToOutputDirectory", copyToOutputDirectory.ToString());
    }
}
