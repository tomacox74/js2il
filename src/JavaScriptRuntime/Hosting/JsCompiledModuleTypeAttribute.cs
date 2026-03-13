using System;

namespace Js2IL.Runtime;

/// <summary>
/// Assembly-level mapping emitted by the JS2IL compiler.
/// Maps a logical module id (e.g. "calculator/index" or "turndown") to the CLR type name
/// that implements the module (e.g. "Modules.calculator_index").
///
/// This allows runtime <c>require()</c> and hosts to resolve modules without re-implementing
/// Node.js filesystem/package.json rules.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class JsCompiledModuleTypeAttribute : Attribute
{
    public JsCompiledModuleTypeAttribute(string moduleId, string canonicalModuleId, string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalModuleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        ModuleId = moduleId;
        CanonicalModuleId = canonicalModuleId;
        TypeName = typeName;
    }

    public string ModuleId { get; }

    public string CanonicalModuleId { get; }
    public string TypeName { get; }
}
