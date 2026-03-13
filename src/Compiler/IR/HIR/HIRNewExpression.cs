using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRNewExpression : HIRExpression
{
    public HIRNewExpression(HIRExpression callee, IReadOnlyList<HIRExpression> arguments)
    {
        Callee = callee;
        Arguments = arguments;
    }

    public HIRExpression Callee { get; }

    public IReadOnlyList<HIRExpression> Arguments { get; }
}
