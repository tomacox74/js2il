using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HRIVariableDeclaration : HIRStatement
{
    public HRIVariableDeclaration(Symbol name, HIRExpression? initializer = null)
    {
        Name = name;
        Initializer = initializer;
    }

    public Symbol Name { get; init; }
    public HIRExpression? Initializer { get; init; }
}