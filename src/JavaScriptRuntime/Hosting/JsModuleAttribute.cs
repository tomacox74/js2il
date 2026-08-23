using System;

namespace Jroc.Runtime;

/// <summary>
/// Associates a generated exports contract type with a compiled module id.
/// This enables <see cref="JsEngine.LoadModule{TExports}()"/> to resolve the module id without an explicit parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class JsModuleAttribute : Attribute
{
    public JsModuleAttribute(string moduleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}

/// <summary>
/// Records the exact JavaScript export name represented by a generated contract member.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class JsExportNameAttribute : Attribute
{
    public JsExportNameAttribute(string exportName)
    {
        ArgumentNullException.ThrowIfNull(exportName);
        ExportName = exportName;
    }

    public string ExportName { get; }
}

/// <summary>
/// Marks a generated fallback contract member as operating on the complete exports value.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class JsExportValueAttribute : Attribute
{
}
