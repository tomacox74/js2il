namespace Js2IL.HIR;

/// <summary>
/// Represents a spread element in an array literal, e.g., [...arr].
/// </summary>
public sealed class HIRSpreadElement : HIRExpression
{
    public HIRSpreadElement(HIRExpression argument)
    {
        Argument = argument;
    }

    /// <summary>
    /// The expression being spread (e.g., the array to spread).
    /// </summary>
    public HIRExpression Argument { get; init; }
}
