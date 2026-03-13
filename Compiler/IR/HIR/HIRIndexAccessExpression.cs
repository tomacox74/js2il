namespace Js2IL.HIR;

/// <summary>
/// Represents a computed index access expression, e.g., obj[index] or arr[0].
/// </summary>
public sealed class HIRIndexAccessExpression : HIRExpression
{
    public HIRIndexAccessExpression(HIRExpression objectExpr, HIRExpression indexExpr)
    {
        Object = objectExpr;
        Index = indexExpr;
    }

    /// <summary>
    /// The object being indexed.
    /// </summary>
    public HIRExpression Object { get; init; }

    /// <summary>
    /// The index expression.
    /// </summary>
    public HIRExpression Index { get; init; }
}
