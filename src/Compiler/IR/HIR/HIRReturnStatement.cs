namespace Js2IL.HIR;

/// <summary>
/// Represents a return statement in HIR.
/// </summary>
public sealed class HIRReturnStatement : HIRStatement
{
    public HIRReturnStatement(HIRExpression? expression = null)
    {
        Expression = expression;
    }

    /// <summary>
    /// The expression to return, or null for bare "return;" statements.
    /// </summary>
    public HIRExpression? Expression { get; init; }
}
