namespace Jroc.HIR;

/// <summary>
/// Represents a dynamic import() expression per ECMA-262 §13.3.10.
/// </summary>
public sealed class HIRImportExpression : HIRExpression
{
    public HIRImportExpression(HIRExpression specifier, HIRExpression? options = null, string phase = "None")
    {
        Specifier = specifier;
        Options = options;
        Phase = phase;
    }

    /// <summary>
    /// The module specifier expression, converted to a string by the runtime.
    /// </summary>
    public HIRExpression Specifier { get; init; }
    public HIRExpression? Options { get; init; }
    public string Phase { get; init; }
}
