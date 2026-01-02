using System.Reflection.Metadata;
using Js2IL.SymbolTables;

namespace Js2IL.IL;

/// <summary>
/// Cache of compiled method handles, used during IL emission phase (LIR → IL).
/// This is NOT a semantic analysis artifact - it stores the output of function compilation
/// for later use when emitting call sites.
/// </summary>
public class CompiledMethodCache
{
    private readonly Dictionary<BindingInfo, MethodDefinitionHandle> _methods = new();

    /// <summary>
    /// Registers a compiled function's method handle.
    /// Called by JavaScriptFunctionGenerator after compiling each function body.
    /// </summary>
    public void Add(BindingInfo binding, MethodDefinitionHandle handle)
    {
        _methods[binding] = handle;
    }

    /// <summary>
    /// Attempts to retrieve the method handle for a function binding.
    /// Called during IL emission when generating function call sites.
    /// </summary>
    public bool TryGet(BindingInfo binding, out MethodDefinitionHandle handle)
    {
        return _methods.TryGetValue(binding, out handle);
    }
}
