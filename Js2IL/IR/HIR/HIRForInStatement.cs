using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRForInStatement : HIRStatement
{
    public HIRForInStatement(Symbol target, HIRExpression enumerable, HIRStatement body, string? label = null)
    {
        Target = target;
        Enumerable = enumerable;
        Body = body;
        Label = label;
    }

    public Symbol Target { get; }
    public HIRExpression Enumerable { get; }
    public HIRStatement Body { get; }
    public string? Label { get; }
}
