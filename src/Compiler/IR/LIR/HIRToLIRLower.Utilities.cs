using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private TempVariable EmitMarkUndefinedPrototype(TempVariable functionValueTemp)
    {
        var markedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.Function),
            MethodName: nameof(JavaScriptRuntime.Function.MarkUndefinedPrototype),
            Arguments: new List<TempVariable> { EnsureObject(functionValueTemp) },
            Result: markedTemp));
        DefineTempStorage(markedTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return markedTemp;
    }

    /// <summary>
    /// Attaches the active <c>with</c>-object to a newly created function/arrow value.
    /// This is the creation-time side of <c>with</c> support and only records callable metadata.
    /// Actual identifier shadow resolution is performed later during binding reads.
    /// </summary>
    private TempVariable EmitBindWithObjectIfNeeded(TempVariable functionValueTemp)
    {
        if (_activeWithObjects.Count == 0)
        {
            return functionValueTemp;
        }

        var boundTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.Function),
            MethodName: nameof(JavaScriptRuntime.Function.BindWithObject),
            Arguments: new[] { EnsureObject(functionValueTemp), _activeWithObjects.Peek() },
            Result: boundTemp));
        DefineTempStorage(boundTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return boundTemp;
    }

    /// <summary>
    /// Resolves a binding read against a bound <c>with</c>-object when present.
    /// If the current callee carries a bound <c>with</c>-object that shadows the binding name,
    /// returns that property value; otherwise returns the provided lexical value unchanged.
    /// </summary>
    private TempVariable EmitResolveWithBindingOrDefault(BindingInfo binding, TempVariable lexicalValueTemp)
    {
        // Only reads of bindings captured from an outer scope can be shadowed by a
        // creation-time `with` object. Current-scope locals/parameters must keep their
        // lexical value and skip runtime with-resolution.
        if (_scope is not null
            && binding.DeclaringScope is not null
            && ReferenceEquals(binding.DeclaringScope, _scope))
        {
            return lexicalValueTemp;
        }

        if (_scope?.MayUseBoundWithObject != true)
        {
            return lexicalValueTemp;
        }

        var nameTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(binding.Name, nameTemp));
        DefineTempStorage(nameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var resolvedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            MethodName: nameof(JavaScriptRuntime.RuntimeServices.ResolveWithBindingOrDefault),
            Arguments: new[] { EnsureObject(nameTemp), EnsureObject(lexicalValueTemp) },
            Result: resolvedTemp));
        DefineTempStorage(resolvedTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return resolvedTemp;
    }
} 