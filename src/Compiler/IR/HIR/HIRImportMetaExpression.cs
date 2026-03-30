namespace Js2IL.HIR;

/// <summary>
/// Represents an import.meta meta property expression.
/// In CommonJS mode this lowers to a host-defined object with stable identity per module.
/// </summary>
public sealed class HIRImportMetaExpression : HIRExpression
{
}
