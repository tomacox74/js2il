namespace Js2IL.HIR;

/// <summary>
/// Represents an optional computed index access expression, e.g., obj?.[index].
/// </summary>
public sealed class HIROptionalIndexAccessExpression : HIRExpression
{
    public HIROptionalIndexAccessExpression(HIRExpression objectExpr, HIRExpression indexExpr)
    {
        Object = objectExpr;
        Index = indexExpr;
    }

    public HIRExpression Object { get; init; }
    public HIRExpression Index { get; init; }
}
