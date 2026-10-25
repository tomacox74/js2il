namespace Jroc.HIR;

public sealed class HIRClassElementDecoratorsExpression(
    string registryClassName, string name, string kind, bool isStatic,
    IReadOnlyList<HIRExpression> decorators) : HIRExpression
{
    public string RegistryClassName { get; } = registryClassName;
    public string Name { get; } = name;
    public string Kind { get; } = kind;
    public bool IsStatic { get; } = isStatic;
    public IReadOnlyList<HIRExpression> Decorators { get; } = decorators;
}

public sealed class HIRApplyClassElementDecoratorsExpression(string registryClassName) : HIRExpression
{
    public string RegistryClassName { get; } = registryClassName;
}
