namespace Jroc.HIR;

public sealed class HIRClassDecorationApplicationStatement(string registryClassName) : HIRStatement
{
    public string RegistryClassName { get; } = registryClassName;
}
