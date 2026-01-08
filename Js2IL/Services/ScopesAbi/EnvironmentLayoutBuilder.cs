using Js2IL.IR;
using Js2IL.Services.VariableBindings;
using Js2IL.SymbolTables;

namespace Js2IL.Services.ScopesAbi;

/// <summary>
/// Callable kind for determining ABI shape.
/// </summary>
public enum CallableKind
{
    /// <summary>User-defined function or arrow function.</summary>
    Function,
    /// <summary>Class constructor.</summary>
    Constructor,
    /// <summary>Class instance method.</summary>
    ClassMethod,
    /// <summary>Module Main entry point.</summary>
    ModuleMain
}

/// <summary>
/// Builds EnvironmentLayout instances from SymbolTable/BindingInfo data.
/// This facade provides a clean abstraction for both legacy and IR pipelines.
/// </summary>
public class EnvironmentLayoutBuilder
{
    private readonly ScopeMetadataRegistry _scopeMetadata;

    public EnvironmentLayoutBuilder(ScopeMetadataRegistry scopeMetadata)
    {
        _scopeMetadata = scopeMetadata;
    }

    /// <summary>
    /// Builds an EnvironmentLayout for a callable scope.
    /// </summary>
    /// <param name="scope">The callable's scope.</param>
    /// <param name="kind">The kind of callable (function, constructor, method, main).</param>
    /// <param name="layoutKind">The scopes layout convention to use.</param>
    /// <returns>The computed EnvironmentLayout.</returns>
    public EnvironmentLayout Build(
        Scope scope,
        CallableKind kind,
        ScopesLayoutKind layoutKind = ScopesLayoutKind.GeneralizedScopesLayout)
    {
        // Count JS parameters (excluding destructured since those become fields)
        int jsParameterCount = scope.Parameters.Count - scope.DestructuredParameters.Count;

        // Determine if this callable needs parent scopes
        bool needsParentScopes = scope.ReferencesParentScopeVariables;

        // Build the callable ABI
        var abi = kind switch
        {
            CallableKind.Function => CallableAbi.ForFunction(jsParameterCount, needsParentScopes),
            CallableKind.Constructor => CallableAbi.ForConstructor(jsParameterCount, needsParentScopes),
            CallableKind.ClassMethod => CallableAbi.ForClassMethod(jsParameterCount, needsParentScopes),
            CallableKind.ModuleMain => CallableAbi.ForModuleMain(jsParameterCount),
            _ => throw new ArgumentException($"Unknown callable kind: {kind}", nameof(kind))
        };

        // Build scope chain layout (ancestor scopes from outermost to innermost)
        var scopeChain = BuildScopeChainLayout(scope, layoutKind);

        // Build binding storage map
        var storageByBinding = BuildStorageMap(scope, scopeChain, kind);

        return new EnvironmentLayout(abi, scopeChain, storageByBinding, layoutKind);
    }

    private static string GetModuleName(Scope scope)
    {
        var current = scope;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current.Name;
    }

    // Must match TypeGenerator's registry scope naming convention.
    // - Global scope uses the module name directly.
    // - Non-global scopes are module-qualified: {module}/{scopeName}
    private static string GetRegistryScopeName(Scope scope)
    {
        if (scope.Kind == ScopeKind.Global)
        {
            return scope.Name;
        }

        var moduleName = GetModuleName(scope);
        return $"{moduleName}/{scope.Name}";
    }

    /// <summary>
    /// Builds the scope chain layout for a callable.
    /// </summary>
    private ScopeChainLayout BuildScopeChainLayout(Scope scope, ScopesLayoutKind layoutKind)
    {
        // If this scope doesn't reference parent scopes, return empty chain
        if (!scope.ReferencesParentScopeVariables)
        {
            return ScopeChainLayout.Empty;
        }

        var ancestorScopes = new List<Scope>();
        
        // Walk ancestors from parent to root
        var current = scope.Parent;
        while (current != null)
        {
            ancestorScopes.Add(current);
            current = current.Parent;
        }

        if (layoutKind == ScopesLayoutKind.GeneralizedScopesLayout)
        {
            // Generalized layout: outermost (global) first
            ancestorScopes.Reverse();
        }
        else
        {
            // Legacy layout: for nested functions, typically just global + immediate parent
            // We still reverse to get global first, but legacy may have fewer slots
            ancestorScopes.Reverse();
            // For legacy, we might want to limit to just required ancestors
            // For now, use the same ordering but the IR pipeline uses GeneralizedScopesLayout
        }

        // Build slots
        var slots = new List<ScopeSlot>();
        for (int i = 0; i < ancestorScopes.Count; i++)
        {
            var ancestorScope = ancestorScopes[i];
            
            // Use ScopeId for IR-level abstraction (no direct handle references)
            var registryName = GetRegistryScopeName(ancestorScope);
            var scopeId = new ScopeId(registryName);

            slots.Add(new ScopeSlot(i, registryName, scopeId));
        }

        return new ScopeChainLayout(slots);
    }

    /// <summary>
    /// Builds the binding storage map for a callable.
    /// </summary>
    private Dictionary<BindingInfo, BindingStorage> BuildStorageMap(
        Scope scope,
        ScopeChainLayout scopeChain,
        CallableKind kind)
    {
        var storage = new Dictionary<BindingInfo, BindingStorage>();

        // Process bindings declared in this scope
        foreach (var (name, binding) in scope.Bindings)
        {
            var bindingStorage = ComputeBindingStorage(scope, binding, name, scopeChain, kind);
            storage[binding] = bindingStorage;
        }

        // Process free variables (bindings from parent scopes)
        // Walk up the scope chain and find bindings referenced by this scope
        var current = scope.Parent;
        while (current != null)
        {
            foreach (var (name, binding) in current.Bindings)
            {
                // Check if this binding is captured (referenced by this or child scopes)
                // and we haven't already added it
                if (binding.IsCaptured && !storage.ContainsKey(binding))
                {
                    // This is a parent scope field
                    var parentIndex = scopeChain.IndexOf(GetRegistryScopeName(current));
                    if (parentIndex >= 0)
                    {
                        // Use ScopeId and FieldId for IR-level abstraction (no direct handle references)
                        var registryName = GetRegistryScopeName(current);
                        var scopeId = new ScopeId(registryName);
                        var fieldId = new FieldId(registryName, name);

                        storage[binding] = BindingStorage.ForParentScopeField(
                            fieldId,
                            scopeId,
                            parentIndex
                        );
                    }
                }
            }
            current = current.Parent;
        }

        return storage;
    }

    /// <summary>
    /// Computes the storage location for a binding declared in the current scope.
    /// </summary>
    private BindingStorage ComputeBindingStorage(
        Scope scope,
        BindingInfo binding,
        string name,
        ScopeChainLayout scopeChain,
        CallableKind kind)
    {
        // Check if it's a parameter (and not destructured)
        if (scope.Parameters.Contains(name) && !scope.DestructuredParameters.Contains(name))
        {
            // Captured parameter - stored as a field on the leaf scope
            // Non-captured parameter - use IL argument
            var paramIndex = GetParameterIndex(scope, name);
            return binding.IsCaptured
                ? GetLeafScopeFieldStorage(scope, name)
                : BindingStorage.ForArgument(paramIndex);
        }

        // Local variable or function declaration
        return binding.IsCaptured
            ? GetLeafScopeFieldStorage(scope, name)
            : BindingStorage.ForLocal(-1);
    }

    private BindingStorage GetLeafScopeFieldStorage(Scope scope, string name)
    {
        // Use ScopeId and FieldId for IR-level abstraction (no direct handle references)
        var registryName = GetRegistryScopeName(scope);
        var scopeId = new ScopeId(registryName);
        var fieldId = new FieldId(registryName, name);

        return BindingStorage.ForLeafScopeField(fieldId, scopeId);
    }

    private int GetParameterIndex(Scope scope, string name)
    {
        int index = 0;
        foreach (var param in scope.Parameters.Where(p => !scope.DestructuredParameters.Contains(p)))
        {
            if (param == name)
                return index;
            index++;
        }
        return -1;
    }
}
