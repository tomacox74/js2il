using Js2IL.SymbolTables;
namespace Js2IL.HIR;
public sealed class HIRVariableExpression : HIRExpression
{
    public HIRVariableExpression(Symbol name)
    {
        Name = name;
    }

    public Symbol Name { get; init; }
}