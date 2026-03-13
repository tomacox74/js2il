namespace Js2IL.HIR;

/// <summary>
/// Represents a dynamic import() expression per ECMA-262 §13.3.10.
/// </summary>
public sealed class HIRImportExpression : HIRExpression
{
    public HIRImportExpression(HIRExpression specifier)
    {
        Specifier = specifier;
    }

    /// <summary>
    /// The module specifier expression (validated to be a string literal during AST validation).
    /// </summary>
    public HIRExpression Specifier { get; init; }
}
