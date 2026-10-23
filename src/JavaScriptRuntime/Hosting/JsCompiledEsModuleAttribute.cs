namespace Jroc.Runtime;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsCompiledEsModuleAttribute(
    string moduleId,
    bool hasTopLevelAwait,
    string[] dependencies,
    string[] exportNames,
    string? jsonSource,
    string? linkError,
    string? resolutionError) : Attribute
{
    public JsCompiledEsModuleAttribute(
        string moduleId, bool hasTopLevelAwait, string[] dependencies, string[] exportNames,
        string? jsonSource, string? linkError, string? resolutionError,
        string? textSource, string? bytesSource, string[] dependencyPhases, bool isNativeEsModule)
        : this(moduleId, hasTopLevelAwait, dependencies, exportNames, jsonSource, linkError, resolutionError)
    {
        TextSource = textSource;
        BytesSource = bytesSource;
        DependencyPhases = dependencyPhases;
        IsNativeEsModule = isNativeEsModule;
    }

    public string ModuleId { get; } = moduleId;
    public bool HasTopLevelAwait { get; } = hasTopLevelAwait;
    public string[] Dependencies { get; } = dependencies;
    public string[] ExportNames { get; } = exportNames;
    public string? JsonSource { get; } = jsonSource;
    public string? LinkError { get; } = linkError;
    public string? ResolutionError { get; } = resolutionError;
    public string? TextSource { get; }
    public string? BytesSource { get; }
    public string[] DependencyPhases { get; } = [];
    public bool IsNativeEsModule { get; }
}
