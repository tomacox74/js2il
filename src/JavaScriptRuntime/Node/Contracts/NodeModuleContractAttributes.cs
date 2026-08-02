namespace Jroc.Runtime.Node.Contracts;

[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class NodeModuleInterfaceAttribute : Attribute
{
    public NodeModuleInterfaceAttribute(string moduleName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleName);

        var normalizedName = moduleName.Trim();
        if (normalizedName.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
        {
            normalizedName = normalizedName["node:".Length..];
        }

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Node module name must be non-empty.", nameof(moduleName));
        }

        ModuleName = normalizedName;
    }

    public string ModuleName { get; }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
public sealed class NodeModuleMemberAttribute : Attribute
{
    public NodeModuleMemberAttribute(string memberName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        MemberName = memberName.Trim();
    }

    public string MemberName { get; }
}
