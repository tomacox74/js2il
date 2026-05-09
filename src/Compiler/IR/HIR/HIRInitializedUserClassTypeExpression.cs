namespace Js2IL.HIR;

public sealed class HIRInitializedUserClassTypeExpression : HIRExpression
{
    public HIRInitializedUserClassTypeExpression(string registryClassName, Js2IL.SymbolTables.Scope classScope, IReadOnlyList<HIRStatement> initializationStatements)
    {
        RegistryClassName = registryClassName;
        ClassScope = classScope;
        InitializationStatements = initializationStatements;
    }

    public string RegistryClassName { get; }

    public Js2IL.SymbolTables.Scope ClassScope { get; }

    public IReadOnlyList<HIRStatement> InitializationStatements { get; }
}
