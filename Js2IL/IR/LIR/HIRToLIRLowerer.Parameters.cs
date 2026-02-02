using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private Type? TryGetStableThisFieldClrType(string fieldName)
    {
        // Find the nearest enclosing class scope.
        var current = _scope;
        while (current != null && current.Kind != ScopeKind.Class)
        {
            current = current.Parent;
        }

        if (current == null)
        {
            return null;
        }

        return current.StableInstanceFieldClrTypes.TryGetValue(fieldName, out var t) ? t : null;
    }

    private static ValueStorage GetPreferredFieldReadStorage(Type? fieldClrType)
    {
        if (fieldClrType == typeof(double))
        {
            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
        }
        if (fieldClrType == typeof(bool))
        {
            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
        }
        if (fieldClrType == typeof(string))
        {
            return new ValueStorage(ValueStorageKind.Reference, typeof(string));
        }

        return new ValueStorage(ValueStorageKind.Reference, typeof(object));
    }

    private void InitializeParameters(IReadOnlyList<HIRPattern> parameters)
    {
        // Build ordered parameter names from HIR. (No AST peeking in lowering.)
        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];

            var ilParamName = p switch
            {
                HIRIdentifierPattern identifier => identifier.Symbol.Name,
                HIRDefaultPattern def when def.Target is HIRIdentifierPattern defaultIdentifier => defaultIdentifier.Symbol.Name,
                _ => $"$p{i}"
            };
            _methodBodyIR.Parameters.Add(ilParamName);
        }

        // If we don't have scope info (e.g., legacy callers), we can't build binding maps or emit
        // default/destructuring initializers. Parameter names are still populated above.
        if (_scope == null) return;

        // Map identifier parameters to their 0-based JS parameter index.
        // Destructuring parameters bind their properties, not the parameter object itself.
        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];
            BindingInfo? binding = p switch
            {
                HIRIdentifierPattern identifier => identifier.Symbol.BindingInfo,
                HIRDefaultPattern def when def.Target is HIRIdentifierPattern defaultIdentifier => defaultIdentifier.Symbol.BindingInfo,
                _ => null
            };

            if (binding != null)
            {
                _parameterIndexMap[binding] = i;
            }
        }

        // For generator functions, parameter initialization must run only when the generator is first started
        // (i.e., on the first .next()), not when the generator object is created and not on each resumption.
        // We'll emit this as a one-time guarded block later (see EmitGeneratorParameterInitializationOnce).
        if (_isGenerator)
        {
            return;
        }

        // Emit default parameter initializers for identifier parameters with defaults.
        _parameterInitSucceeded = EmitDefaultParameterInitializers(parameters);
        if (!_parameterInitSucceeded)
        {
            return;
        }

        // Emit parameter destructuring initializers (object/array patterns).
        if (!EmitDestructuredParameterInitializers(parameters))
        {
            _parameterInitSucceeded = false;
            return;
        }

        // For captured identifier parameters, initialize the corresponding leaf-scope fields.
        // This must happen after default parameter initialization so the final value is stored.
        // Without this, nested functions reading captured parameters will observe null.
        EmitCapturedParameterFieldInitializers();
    }

    private bool EmitGeneratorParameterInitializationOnce(IReadOnlyList<HIRPattern> parameters)
    {
        if (!_isGenerator)
        {
            return true;
        }

        if (_scope == null)
        {
            return false;
        }

        if (!_methodBodyIR.NeedsLeafScopeLocal || _methodBodyIR.LeafScopeId.IsNil)
        {
            // Generator lowering requires a leaf scope local.
            return false;
        }

        var scopeName = _methodBodyIR.LeafScopeId.Name;

        var afterInitLabel = CreateLabel();

        // if (_started) goto afterInit;
        var startedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._started), startedTemp));
        DefineTempStorage(startedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        SetTempVariableSlot(startedTemp, CreateAnonymousVariableSlot("$gen_started", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(startedTemp, afterInitLabel));

        // Default parameter initialization
        _parameterInitSucceeded = EmitDefaultParameterInitializers(parameters);
        if (!_parameterInitSucceeded)
        {
            return false;
        }

        // Destructuring parameter initialization
        if (!EmitDestructuredParameterInitializers(parameters))
        {
            return false;
        }

        // Store captured parameter fields (so parameter bindings live on the leaf scope)
        EmitCapturedParameterFieldInitializers();

        // _started = true
        var trueTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
        DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._started), trueTemp));

        _methodBodyIR.Instructions.Add(new LIRLabel(afterInitLabel));
        return true;
    }

    private bool EmitDestructuredParameterInitializers(IReadOnlyList<HIRPattern> parameters)
    {
        if (_scope == null)
        {
            return false;
        }

        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];

            // Only handle direct Object/Array patterns for now.
            // (Param-level defaults like ({a} = {}) are not yet supported by the IR pipeline gate.)
            if (p is not HIRObjectPattern && p is not HIRArrayPattern)
            {
                if (p is HIRDefaultPattern def && (def.Target is HIRObjectPattern or HIRArrayPattern))
                {
                    return false;
                }
                continue;
            }

            // Load the incoming parameter value and destructure into declared bindings.
            var paramTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(i, paramTemp));
            DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerDestructuringPattern(p, paramTemp, DestructuringWriteMode.Declaration, sourceNameForError: null))
            {
                return false;
            }
        }

        return true;
    }

    private void EmitCapturedParameterFieldInitializers()
    {
        if (_scope == null || _environmentLayout == null) return;

        foreach (var (binding, jsParamIndex) in _parameterIndexMap)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage == null) continue;

            if (storage.Kind == BindingStorageKind.LeafScopeField && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
            {
                // Load the parameter (object) and store to leaf scope field.
                var paramTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadParameter(jsParamIndex, paramTemp));
                DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                var boxed = EnsureObject(paramTemp);
                _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxed));
            }
        }
    }

    /// <summary>
    /// Emits LIR instructions to initialize default parameter values.
    /// For each parameter with a default (AssignmentPattern), emits:
    /// - Load parameter, check if null
    /// - If null, evaluate default expression and store back to parameter
    /// </summary>
    /// <returns>True if all default parameters were successfully lowered, false if any failed (method should fall back to legacy)</returns>
    private bool EmitDefaultParameterInitializers(IReadOnlyList<HIRPattern> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            if (parameters[i] is not HIRDefaultPattern def)
            {
                continue;
            }

            // Only support top-level default parameters for identifier params.
            if (def.Target is not HIRIdentifierPattern)
            {
                return false;
            }

            // Record instruction count before we start, so we can roll back if lowering fails
            var instructionCountBefore = _methodBodyIR.Instructions.Count;

            // Emit: load parameter, check if null, if so evaluate default and store back
            var notNullLabel = CreateLabel();

            // Load parameter value
            var paramTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(i, paramTemp));
            DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Branch if not null (brtrue)
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(paramTemp, notNullLabel));

            // Evaluate default value expression
            if (!TryLowerExpression(def.Default, out var defaultValueTemp))
            {
                // If we can't lower the default expression, roll back all instructions
                // and signal that the entire method should fall back to legacy.
                // Note: We only rollback instructions here, not temp variables or labels.
                // This is acceptable because when we return false, the entire MethodBodyIR is discarded
                // and the method falls back to legacy compilation - the orphaned temps/labels are never used.
                while (_methodBodyIR.Instructions.Count > instructionCountBefore)
                {
                    _methodBodyIR.Instructions.RemoveAt(_methodBodyIR.Instructions.Count - 1);
                }
                return false;
            }

            // Ensure the default value is boxed to object
            defaultValueTemp = EnsureObject(defaultValueTemp);

            // Store back to parameter
            _methodBodyIR.Instructions.Add(new LIRStoreParameter(i, defaultValueTemp));

            // Not-null label
            _methodBodyIR.Instructions.Add(new LIRLabel(notNullLabel));
        }

        return true; // All default parameters successfully lowered
    }
}
