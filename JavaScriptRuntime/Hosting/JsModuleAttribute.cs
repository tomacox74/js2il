using System;

namespace Js2IL.Runtime;

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
