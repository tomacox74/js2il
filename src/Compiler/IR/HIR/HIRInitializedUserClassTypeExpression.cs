namespace Jroc.HIR;

public sealed class HIRInitializedUserClassTypeExpression : HIRExpression
{
    public HIRInitializedUserClassTypeExpression(
        string registryClassName,
        Jroc.SymbolTables.Scope classScope,
        IReadOnlyList<HIRStatement> initializationStatements,
        HIRExpression? superClass = null,
        bool isClassExpression = false,
        string? explicitName = null,
        IReadOnlyList<HIRExpression>? decorators = null)
    {
        RegistryClassName = registryClassName;
        ClassScope = classScope;
        InitializationStatements = initializationStatements;
        SuperClass = superClass;
        IsClassExpression = isClassExpression;
        ExplicitName = explicitName;
        Decorators = decorators ?? [];
    }

    public string RegistryClassName { get; }

    public Jroc.SymbolTables.Scope ClassScope { get; }

    public IReadOnlyList<HIRStatement> InitializationStatements { get; }

    public HIRExpression? SuperClass { get; }

    public bool IsClassExpression { get; }

    public string? ExplicitName { get; }

    public IReadOnlyList<HIRExpression> Decorators { get; }
}
