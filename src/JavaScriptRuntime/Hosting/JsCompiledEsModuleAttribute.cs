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
    public string ModuleId { get; } = moduleId;
    public bool HasTopLevelAwait { get; } = hasTopLevelAwait;
    public string[] Dependencies { get; } = dependencies;
    public string[] ExportNames { get; } = exportNames;
    public string? JsonSource { get; } = jsonSource;
    public string? LinkError { get; } = linkError;
    public string? ResolutionError { get; } = resolutionError;
}
