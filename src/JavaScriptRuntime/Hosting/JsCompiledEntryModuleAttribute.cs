namespace Jroc.Runtime;

/// <summary>
/// Identifies the single canonical entry module in a compiled JROC assembly.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class JsCompiledEntryModuleAttribute : Attribute
{
    public JsCompiledEntryModuleAttribute(string moduleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}
