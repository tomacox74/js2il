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

/// <summary>
/// Identifies a stable documented type owned by a Node module.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class NodeModuleTypeAttribute : Attribute
{
    public NodeModuleTypeAttribute(string moduleName, string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleName);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        var normalizedModuleName = moduleName.Trim();
        if (normalizedModuleName.StartsWith("node:", StringComparison.OrdinalIgnoreCase))
        {
            normalizedModuleName = normalizedModuleName["node:".Length..];
        }

        if (string.IsNullOrWhiteSpace(normalizedModuleName))
        {
            throw new ArgumentException("Node module name must be non-empty.", nameof(moduleName));
        }

        ModuleName = normalizedModuleName;
        TypeName = typeName.Trim();
    }

    public string ModuleName { get; }

    public string TypeName { get; }
}

/// <summary>
/// Describes a documented nested shape accepted by an otherwise dynamically
/// represented JavaScript parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public sealed class NodeModuleParameterContractAttribute : Attribute
{
    public NodeModuleParameterContractAttribute(Type contractType)
    {
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
    }

    public Type ContractType { get; }
}

public enum NodeModuleResultKind
{
    Promise,
    Callback,
    Iterator,
    AsyncIterator
}

/// <summary>
/// Describes the documented value carried by a promise, callback, or iterator.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class NodeModuleResultContractAttribute : Attribute
{
    public NodeModuleResultContractAttribute(
        NodeModuleResultKind kind,
        Type contractType,
        string? callbackParameter = null)
    {
        Kind = kind;
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        CallbackParameter = callbackParameter;
    }

    public NodeModuleResultKind Kind { get; }

    public Type ContractType { get; }

    public string? CallbackParameter { get; }
}

/// <summary>
/// Lets generated contract adapters retain the original JavaScript value.
/// </summary>
public interface IJavaScriptValueHost
{
    object? JavaScriptValue { get; }
}

/// <summary>
/// Bridges generated contract hosts back to their JavaScript values without
/// using reflection or changing JavaScript property lookup semantics.
/// </summary>
public static class NodeModuleContractHosting
{
    public static object? Unwrap(object? value)
        => value is IJavaScriptValueHost host ? host.JavaScriptValue : value;
}
