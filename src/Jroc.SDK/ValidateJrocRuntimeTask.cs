using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Jroc.SDK.BuildTasks;

public sealed class ValidateJrocRuntimeTask : Microsoft.Build.Utilities.Task
{
    private const string RuntimeAssemblyName = "JavaScriptRuntime";

    [Required]
    public ITaskItem[] GeneratedAssemblies { get; set; } = [];

    public ITaskItem[] RuntimeAssemblies { get; set; } = [];

    public override bool Execute()
    {
        var requiredVersions = new HashSet<Version>();
        foreach (var generatedAssembly in GeneratedAssemblies)
        {
            if (!TryReadRuntimeReference(generatedAssembly.ItemSpec, requiredVersions))
            {
                return false;
            }
        }

        if (requiredVersions.Count == 0)
        {
            return true;
        }

        if (requiredVersions.Count > 1)
        {
            LogSdkError(
                "JROCSDK1003",
                $"Jroc generated assemblies require incompatible {RuntimeAssemblyName} versions: " +
                $"{string.Join(", ", requiredVersions.Order())}. Compile all JrocCompile items with one compatible Jroc.SDK package.");
            return false;
        }

        var requiredVersion = requiredVersions.Single();
        var runtimePaths = RuntimeAssemblies
            .Select(item => item.ItemSpec)
            .Where(path => string.Equals(Path.GetFileName(path), RuntimeAssemblyName + ".dll", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (runtimePaths.Length == 0)
        {
            LogMissingRuntime(requiredVersion);
            return false;
        }

        var resolvedVersions = new Dictionary<Version, List<string>>();
        foreach (var runtimePath in runtimePaths)
        {
            if (!File.Exists(runtimePath))
            {
                LogSdkError(
                    "JROCSDK1001",
                    $"Jroc requires {RuntimeAssemblyName} {requiredVersion}, but the resolved runtime asset '{runtimePath}' is missing. " +
                    "Restore Jroc.SDK without excluding compile or runtime assets, then rebuild.");
                return false;
            }

            if (!TryReadAssemblyIdentity(runtimePath, out var name, out var version, out var error))
            {
                LogSdkError(
                    "JROCSDK1002",
                    $"Jroc could not inspect runtime asset '{runtimePath}': {error} " +
                    "Restore a compatible Jroc.Runtime package and rebuild.");
                return false;
            }

            if (!string.Equals(name, RuntimeAssemblyName, StringComparison.Ordinal))
            {
                LogSdkError(
                    "JROCSDK1002",
                    $"Jroc expected runtime assembly '{RuntimeAssemblyName}', but '{runtimePath}' contains assembly '{name}'. " +
                    "Restore a compatible Jroc.Runtime package and rebuild.");
                return false;
            }

            if (!resolvedVersions.TryGetValue(version, out var paths))
            {
                paths = [];
                resolvedVersions.Add(version, paths);
            }

            paths.Add(runtimePath);
        }

        if (resolvedVersions.Count != 1 || !resolvedVersions.ContainsKey(requiredVersion))
        {
            var resolved = string.Join(
                "; ",
                resolvedVersions.OrderBy(pair => pair.Key)
                    .Select(pair => $"{pair.Key} at {string.Join(", ", pair.Value)}"));
            LogSdkError(
                "JROCSDK1002",
                $"Jroc generated assemblies require {RuntimeAssemblyName} {requiredVersion}, but MSBuild resolved {resolved}. " +
                "Align the Jroc.SDK and Jroc.Runtime package versions, remove conflicting runtime references, then restore and rebuild.");
            return false;
        }

        return true;
    }

    private bool TryReadRuntimeReference(string generatedAssemblyPath, HashSet<Version> requiredVersions)
    {
        if (!File.Exists(generatedAssemblyPath))
        {
            LogSdkError(
                "JROCSDK1004",
                $"Jroc generated assembly '{generatedAssemblyPath}' is missing before reference validation. Clean and rebuild the project.");
            return false;
        }

        try
        {
            using var stream = File.OpenRead(generatedAssemblyPath);
            using var peReader = new PEReader(stream);
            var metadata = peReader.GetMetadataReader();
            foreach (var handle in metadata.AssemblyReferences)
            {
                var reference = metadata.GetAssemblyReference(handle);
                if (string.Equals(metadata.GetString(reference.Name), RuntimeAssemblyName, StringComparison.Ordinal))
                {
                    requiredVersions.Add(reference.Version);
                    return true;
                }
            }

            LogSdkError(
                "JROCSDK1004",
                $"Jroc generated assembly '{generatedAssemblyPath}' does not reference {RuntimeAssemblyName}. Clean and rebuild with a compatible Jroc.SDK package.");
            return false;
        }
        catch (Exception ex) when (ex is IOException or BadImageFormatException)
        {
            LogSdkError(
                "JROCSDK1004",
                $"Jroc could not inspect generated assembly '{generatedAssemblyPath}': {ex.Message}");
            return false;
        }
    }

    private static bool TryReadAssemblyIdentity(
        string assemblyPath,
        out string name,
        out Version version,
        out string error)
    {
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            var metadata = peReader.GetMetadataReader();
            var definition = metadata.GetAssemblyDefinition();
            name = metadata.GetString(definition.Name);
            version = definition.Version;
            error = string.Empty;
            return true;
        }
        catch (Exception ex) when (ex is IOException or BadImageFormatException)
        {
            name = string.Empty;
            version = new Version();
            error = ex.Message;
            return false;
        }
    }

    private void LogMissingRuntime(Version requiredVersion)
    {
        LogSdkError(
            "JROCSDK1001",
            $"Jroc generated assemblies require {RuntimeAssemblyName} {requiredVersion}, but no compatible runtime reference was resolved. " +
            "Restore Jroc.SDK without excluding compile or runtime assets, remove stale obj files, then rebuild.");
    }

    private void LogSdkError(string code, string message)
    {
        Log.LogError(
            subcategory: "Jroc SDK",
            errorCode: code,
            helpKeyword: null,
            file: null,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            message);
    }
}
