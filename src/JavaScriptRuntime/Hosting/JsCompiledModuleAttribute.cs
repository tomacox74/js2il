using System;

namespace Js2IL.Runtime;

/// <summary>
/// Assembly-level manifest entry emitted by the JS2IL compiler.
/// Used by hosts to discover which module ids exist inside a compiled assembly.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsCompiledModuleAttribute : Attribute
{
    public JsCompiledModuleAttribute(string moduleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ModuleId = moduleId;
    }

    public string ModuleId { get; }
}
