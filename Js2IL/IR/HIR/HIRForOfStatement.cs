using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRForOfStatement : HIRStatement
{
    public HIRForOfStatement(Symbol target, HIRExpression iterable, HIRStatement body, string? label = null)
    {
        Target = target;
        Iterable = iterable;
        Body = body;
        Label = label;
    }

    public Symbol Target { get; }
    public HIRExpression Iterable { get; }
    public HIRStatement Body { get; }
    public string? Label { get; }
}
