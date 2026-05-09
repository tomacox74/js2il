namespace Js2IL.HIR;

public sealed class HIRThisExpression : HIRExpression
{
    public HIRThisExpression(string? staticClassRegistryName = null)
    {
        StaticClassRegistryName = staticClassRegistryName;
    }

    public string? StaticClassRegistryName { get; }
}
