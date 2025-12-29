using Js2IL.SymbolTables;

namespace Js2IL.HIR;

public sealed class HIRVariableDeclaration : HIRStatement
{
    public HIRVariableDeclaration(Symbol name, HIRExpression? initializer = null)
    {
        Name = name;
        Initializer = initializer;
    }

    public Symbol Name { get; init; }
    public HIRExpression? Initializer { get; init; }
}