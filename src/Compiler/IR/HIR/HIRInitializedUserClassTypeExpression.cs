namespace Jroc.HIR;

public sealed class HIRInitializedUserClassTypeExpression : HIRExpression
{
    public HIRInitializedUserClassTypeExpression(string registryClassName, Jroc.SymbolTables.Scope classScope, IReadOnlyList<HIRStatement> initializationStatements)
    {
        RegistryClassName = registryClassName;
        ClassScope = classScope;
        InitializationStatements = initializationStatements;
    }

    public string RegistryClassName { get; }

    public Jroc.SymbolTables.Scope ClassScope { get; }

    public IReadOnlyList<HIRStatement> InitializationStatements { get; }
}
