using System.Reflection.Metadata;
using Js2IL.SymbolTables;

namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Metadata about a compiled callable, including its method handle, environment layout,
/// and scopes layout convention.
/// </summary>
/// <param name="MethodHandle">The compiled method's handle in the assembly being built.</param>
/// <param name="Layout">The environment layout specifying how bindings are accessed.</param>
/// <param name="LayoutKind">The scopes layout convention (legacy vs generalized).</param>
public sealed record CallableMetadata(
    MethodDefinitionHandle MethodHandle,
    EnvironmentLayout Layout,
    ScopesLayoutKind LayoutKind
);

/// <summary>
/// Registry of callable metadata keyed by BindingInfo.
/// This is the forward path for IR-compiled callables, replacing the role of FunctionRegistry
/// (which is string-keyed and lacks scope metadata) during Case A migration.
/// </summary>
public class CallableMetadataRegistry
{
    private readonly Dictionary<BindingInfo, CallableMetadata> _callables = new();

    /// <summary>
    /// Registers metadata for a compiled callable.
    /// Called after compiling each function/method body.
    /// </summary>
    public void Add(BindingInfo binding, CallableMetadata metadata)
    {
        _callables[binding] = metadata;
    }

    /// <summary>
    /// Registers metadata for a compiled callable using individual components.
    /// </summary>
    public void Add(
        BindingInfo binding,
        MethodDefinitionHandle methodHandle,
        EnvironmentLayout layout,
        ScopesLayoutKind layoutKind = ScopesLayoutKind.GeneralizedScopesLayout)
    {
        _callables[binding] = new CallableMetadata(methodHandle, layout, layoutKind);
    }

    /// <summary>
    /// Attempts to retrieve metadata for a callable binding.
    /// Called during IL emission when generating function call sites.
    /// </summary>
    public bool TryGet(BindingInfo binding, out CallableMetadata? metadata)
    {
        return _callables.TryGetValue(binding, out metadata);
    }

    /// <summary>
    /// Checks if a binding has been registered.
    /// </summary>
    public bool Contains(BindingInfo binding) => _callables.ContainsKey(binding);

    /// <summary>
    /// Gets the environment layout for a callable, or null if not registered.
    /// Convenience method for scope chain lookup.
    /// </summary>
    public EnvironmentLayout? GetLayout(BindingInfo binding)
    {
        return _callables.TryGetValue(binding, out var metadata) ? metadata.Layout : null;
    }

    /// <summary>
    /// Gets the scopes layout kind for a callable, defaulting to generalized if not found.
    /// </summary>
    public ScopesLayoutKind GetLayoutKind(BindingInfo binding)
    {
        return _callables.TryGetValue(binding, out var metadata)
            ? metadata.LayoutKind
            : ScopesLayoutKind.GeneralizedScopesLayout;
    }
}
