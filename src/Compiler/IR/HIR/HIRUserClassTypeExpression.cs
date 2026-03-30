namespace Js2IL.HIR;

/// <summary>
/// Represents a reference to a compiled user class type as a runtime value.
/// This is used when a class is used as an expression (e.g., module.exports = class ...).
/// Lowering produces a System.Type via LIRGetUserClassType.
/// </summary>
public sealed class HIRUserClassTypeExpression : HIRExpression
{
    public HIRUserClassTypeExpression(string registryClassName)
    {
        RegistryClassName = registryClassName;
    }

    public string RegistryClassName { get; init; }
}
