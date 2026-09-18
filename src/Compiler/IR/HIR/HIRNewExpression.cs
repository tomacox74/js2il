using Jroc.SymbolTables;

namespace Jroc.HIR;

public sealed class HIRNewExpression : HIRExpression
{
    public HIRNewExpression(
        HIRExpression callee,
        IReadOnlyList<HIRExpression> arguments,
        bool isRegExpLiteral = false)
    {
        Callee = callee;
        Arguments = arguments;
        IsRegExpLiteral = isRegExpLiteral;
    }

    public HIRExpression Callee { get; }

    public IReadOnlyList<HIRExpression> Arguments { get; }

    public bool IsRegExpLiteral { get; }
}
