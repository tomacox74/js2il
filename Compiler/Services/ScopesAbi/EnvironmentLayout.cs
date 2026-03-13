using Js2IL.SymbolTables;

namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// The complete environment specification attached to a compiled callable.
/// Contains everything needed to emit loads/stores for bindings and construct scopes arrays.
/// </summary>
/// <param name="Abi">The ABI shape of the callable (signature, scopes source).</param>
/// <param name="ScopeChain">The layout of the scopes array.</param>
/// <param name="StorageByBinding">Maps each referenced binding to its storage location.</param>
/// <param name="LayoutKind">The scopes layout convention (legacy vs generalized).</param>
public sealed record EnvironmentLayout(
    CallableAbi Abi,
    ScopeChainLayout ScopeChain,
    IReadOnlyDictionary<BindingInfo, BindingStorage> StorageByBinding,
    ScopesLayoutKind LayoutKind = ScopesLayoutKind.GeneralizedScopesLayout
)
{
    /// <summary>
    /// Gets the storage for a binding, or null if not found.
    /// </summary>
    public BindingStorage? GetStorage(BindingInfo binding)
    {
        return StorageByBinding.TryGetValue(binding, out var storage) ? storage : null;
    }

    /// <summary>
    /// Returns true if this callable needs access to parent scopes.
    /// </summary>
    public bool NeedsParentScopes => Abi.ScopesSource != ScopesSource.None;

    /// <summary>
    /// Returns the set of parent scope indices actually accessed by this callable.
    /// Useful for optimization: slots not in this set may be left null.
    /// </summary>
    public IReadOnlySet<int> GetRequiredParentScopeIndices()
    {
        var indices = new HashSet<int>();
        foreach (var (_, storage) in StorageByBinding)
        {
            if (storage.Kind == BindingStorageKind.ParentScopeField)
            {
                indices.Add(storage.ParentScopeIndex);
            }
        }
        return indices;
    }

    /// <summary>
    /// Creates an empty environment layout for callables that don't access any bindings.
    /// </summary>
    public static EnvironmentLayout Empty(CallableAbi abi)
    {
        return new EnvironmentLayout(
            abi,
            ScopeChainLayout.Empty,
            new Dictionary<BindingInfo, BindingStorage>()
        );
    }
}
