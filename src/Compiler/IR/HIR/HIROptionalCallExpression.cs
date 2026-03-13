namespace Js2IL.HIR;

public sealed class HIROptionalCallExpression : HIRExpression
{
    public HIROptionalCallExpression(HIRExpression callee, IReadOnlyList<HIRExpression> arguments)
    {
        Callee = callee;
        Arguments = arguments;
    }

    public HIRExpression Callee { get; init; }
    public IReadOnlyList<HIRExpression> Arguments { get; init; }
}
