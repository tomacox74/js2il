using Jroc.SymbolTables;
namespace Jroc.HIR;
public sealed class HIRVariableExpression : HIRExpression
{
    public HIRVariableExpression(Symbol name)
    {
        Name = name;
    }

    public Symbol Name { get; init; }
}