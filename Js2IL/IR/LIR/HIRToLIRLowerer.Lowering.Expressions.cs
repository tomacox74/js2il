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
    private bool TryLowerExpressionDiscardResult(HIRExpression expression)
    {
        // Expression statements and for-loop update clauses do not consume the expression result.
        // Special-case update expressions so we can avoid materializing the postfix return value
        // (which otherwise becomes a dead box/pop sequence in the generated IL).
        if (expression is HIRUpdateExpression updateExpr)
        {
            return TryLowerUpdateExpression(updateExpr, out _, resultUsed: false);
        }

        if (expression is HIRSequenceExpression seqExpr)
        {
            for (int i = 0; i < seqExpr.Expressions.Count; i++)
            {
                if (!TryLowerExpressionDiscardResult(seqExpr.Expressions[i]))
                {
                    return false;
                }
            }
            return true;
        }

        return TryLowerExpression(expression, out _);
    }

    private bool TryLowerExpression(HIRExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        switch (expression)
        {
            case HIRSequenceExpression seqExpr:
                {
                    if (seqExpr.Expressions.Count == 0)
                    {
                        return false;
                    }

                    // Evaluate each expression left-to-right; the sequence's value is the last.
                    for (int i = 0; i < seqExpr.Expressions.Count - 1; i++)
                    {
                        if (!TryLowerExpressionDiscardResult(seqExpr.Expressions[i]))
                        {
                            return false;
                        }
                    }

                    return TryLowerExpression(seqExpr.Expressions[seqExpr.Expressions.Count - 1], out resultTempVar);
                }

            case HIROptionalPropertyAccessExpression optionalPropAccessExpr:
                return TryLowerOptionalPropertyAccessExpression(optionalPropAccessExpr, out resultTempVar);

            case HIROptionalIndexAccessExpression optionalIndexAccessExpr:
                return TryLowerOptionalIndexAccessExpression(optionalIndexAccessExpr, out resultTempVar);

            case HIROptionalCallExpression optionalCallExpr:
                return TryLowerOptionalCallExpression(optionalCallExpr, out resultTempVar);

            case HIRAwaitExpression awaitExpr:
                return TryLowerAwaitExpression(awaitExpr, out resultTempVar);

            case HIRYieldExpression yieldExpr:
                return TryLowerYieldExpression(yieldExpr, out resultTempVar);

            case HIRScopesArrayExpression:
                // Only currently emitted for constructors that receive a scopes argument.
                if (_callableKind != CallableKind.Constructor)
                {
                    return false;
                }
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopesArgument(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                return true;

            case HIRThisExpression:
                // PL3.5: ThisExpression.
                // Only supported for instance callables where IL arg0 is the receiver.
                if (_callableKind is not CallableKind.ClassMethod
                    and not CallableKind.Constructor
                    and not CallableKind.Function
                    and not CallableKind.ModuleMain)
                {
                    return false;
                }

                // Derived class constructors must call super() before accessing `this`.
                if (_callableKind == CallableKind.Constructor && _isDerivedConstructor && !_superConstructorCalled)
                {
                    var errTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRNewBuiltInError("ReferenceError", Message: null, errTemp));
                    DefineTempStorage(errTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    _methodBodyIR.Instructions.Add(new LIRThrow(errTemp));

                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }

                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadThis(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;

            case HIRSuperExpression:
                // `super` is only meaningful as the callee of a call expression (super(...))
                // or as the receiver in property access (super.m).
                return false;

            case HIRLiteralExpression literal:
                // All literals allocate a new SSA value.
                resultTempVar = CreateTempVariable();
                switch (literal.Kind)
                {
                    case JavascriptType.String:
                        _methodBodyIR.Instructions.Add(new LIRConstString((string)literal.Value!, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;

                    case JavascriptType.Number:
                        double value = 0;
                        if (literal.Value != null)
                        {
                            value = (double)literal.Value;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConstNumber(value, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;

                    case JavascriptType.Boolean:
                        bool boolValue = literal.Value != null && (bool)literal.Value;
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(boolValue, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;

                    case JavascriptType.Null:
                        // JavaScript 'null' literal - raw value, boxing added by EnsureObject when needed
                        _methodBodyIR.Instructions.Add(new LIRConstNull(resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(JavaScriptRuntime.JsNull)));
                        return true;

                    case JavascriptType.Undefined:
                        // JavaScript 'undefined' - represented as CLR null
                        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        return true;

                    default:
                        // Unsupported literal type
                        return false;
                }

            case HIRBinaryExpression binaryExpr:
                return TryLowerBinaryExpression(binaryExpr, out resultTempVar);

            case HIRConditionalExpression conditionalExpr:
                return TryLowerConditionalExpression(conditionalExpr, out resultTempVar);

            case HIRCallExpression callExpr:
                if (!TryLowerCallExpression(callExpr, out resultTempVar))
                {
                    if (callExpr.Callee is HIRPropertyAccessExpression pa)
                    {
                        var recv = pa.Object;
                        var recvDesc = recv switch
                        {
                            HIRVariableExpression ve => $"{ve.Name.Name} ({ve.Name.Kind})",
                            _ => recv.GetType().Name
                        };
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering CallExpression (property '{pa.PropertyName}' on {recvDesc})");
                    }
                    else
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering CallExpression (callee={callExpr.Callee.GetType().Name})");
                    }
                    return false;
                }
                return true;

            case HIRNewExpression newExpr:
                if (!TryLowerNewExpression(newExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering NewExpression (callee={newExpr.Callee.GetType().Name})");
                    return false;
                }
                return true;

            case HIRUnaryExpression unaryExpr:
                return TryLowerUnaryExpression(unaryExpr, out resultTempVar);

            case HIRUpdateExpression updateExpr:
                return TryLowerUpdateExpression(updateExpr, out resultTempVar);

            case HIRTemplateLiteralExpression templateLiteral:
                if (!TryLowerTemplateLiteralExpression(templateLiteral, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering TemplateLiteralExpression");
                    return false;
                }
                return true;

            case HIRAssignmentExpression assignExpr:
                return TryLowerAssignmentExpression(assignExpr, out resultTempVar);

            case HIRPropertyAssignmentExpression propAssignExpr:
                return TryLowerPropertyAssignmentExpression(propAssignExpr, out resultTempVar);

            case HIRIndexAssignmentExpression indexAssignExpr:
                return TryLowerIndexAssignmentExpression(indexAssignExpr, out resultTempVar);

            case HIRDestructuringAssignmentExpression destructAssignExpr:
                return TryLowerDestructuringAssignmentExpression(destructAssignExpr, out resultTempVar);

            case HIRArrayExpression arrayExpr:
                if (!TryLowerArrayExpression(arrayExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering ArrayExpression");
                    return false;
                }
                return true;

            case HIRObjectExpression objectExpr:
                return TryLowerObjectExpression(objectExpr, out resultTempVar);

            case HIRPropertyAccessExpression propAccessExpr:
                if (!TryLowerPropertyAccessExpression(propAccessExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure($"HIR->LIR: failed lowering PropertyAccessExpression (property='{propAccessExpr.PropertyName}')");
                    return false;
                }
                return true;

            case HIRLoadUserClassInstanceFieldExpression loadUserField:
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    loadUserField.RegistryClassName,
                    loadUserField.FieldName,
                    loadUserField.IsPrivateField,
                    resultTempVar));
                var stableFieldType = TryGetStableThisFieldClrType(loadUserField.FieldName);
                DefineTempStorage(resultTempVar, GetPreferredFieldReadStorage(stableFieldType));
                return true;

            case HIRIndexAccessExpression indexAccessExpr:
                return TryLowerIndexAccessExpression(indexAccessExpr, out resultTempVar);

            case HIRVariableExpression varExpr:
                // Look up the binding using the Symbol's BindingInfo directly
                // This correctly resolves shadowed variables to the right binding
                var binding = varExpr.Name.BindingInfo;

                // Class declarations are compiled separately (as CLR types) and are not SSA-assigned.
                // Always lower a class identifier to a runtime System.Type so it can cross module boundaries
                // (e.g., `module.exports = { Counter }`).
                if (binding.DeclarationNode is ClassDeclaration classDecl)
                {
                    if (!TryGetRegistryClassNameForClassDeclaration(classDecl, out var registryClassName))
                    {
                        return false;
                    }

                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(registryClassName, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    return true;
                }

                static ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
                {
                    // Propagate unboxed primitives for stable inferred types. This matches the current
                    // typed-scope-field support in TypeGenerator/VariableRegistry.
                    if (b.IsStableType)
                    {
                        if (b.ClrType == typeof(double))
                        {
                            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                        }
                        if (b.ClrType == typeof(bool))
                        {
                            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
                        }
                        if (b.ClrType == typeof(JavaScriptRuntime.Array))
                        {
                            return new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array));
                        }
                    }

                    return new ValueStorage(ValueStorageKind.Reference, typeof(object));
                }

                static bool IsSafeInjectedCommonJsRequireParameter(BindingInfo b)
                {
                    // CommonJS module wrapper parameter.
                    // Only treat it as strongly-typed RequireDelegate when we can conservatively prove it is
                    // the injected parameter binding and is never written to (not reassigned).
                    return string.Equals(b.Name, "require", StringComparison.Ordinal)
                        && b.DeclaringScope.Kind == ScopeKind.Global
                        && b.DeclaringScope.Parameters.Contains("require")
                        && ReferenceEquals(b.DeclarationNode, b.DeclaringScope.AstNode)
                        && !b.HasWrite;
                }

                // Per-iteration environments: if this binding lives in an active materialized scope instance
                // (e.g., `for (let/const ...)` loop-head scope), load directly from that scope field.
                if (TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, resultTempVar));
                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                    return true;
                }
                
                // Check if this binding is stored in a scope field (captured variable)
                if (_environmentLayout != null)
                {
                    var storage = _environmentLayout.GetStorage(binding);
                    if (storage == null && _scope != null && binding.IsCaptured)
                    {
                        // Fallback: if the environment layout didn't include this binding (e.g., due to
                        // a BindingInfo identity mismatch or overly-conservative storage map), try to
                        // compute scope-field storage from the caller's scope chain.
                        //
                        // This is only valid for captured bindings that are stored as fields on their
                        // declaring scope type.
                        var declaringScope = _scope;
                        while (declaringScope != null)
                        {
                            if (declaringScope.Bindings.TryGetValue(binding.Name, out var candidate)
                                && ReferenceEquals(candidate, binding))
                            {
                                break;
                            }
                            declaringScope = declaringScope.Parent;
                        }

                        if (declaringScope != null)
                        {
                            var declaringRegistryName = ScopeNaming.GetRegistryScopeName(declaringScope);
                            var scopeId = new ScopeId(declaringRegistryName);
                            var fieldId = new FieldId(declaringRegistryName, binding.Name);

                            if (!ReferenceEquals(declaringScope, _scope))
                            {
                                var parentIndex = _environmentLayout.ScopeChain.IndexOf(declaringRegistryName);
                                if (parentIndex >= 0)
                                {
                                    storage = BindingStorage.ForParentScopeField(fieldId, scopeId, parentIndex);
                                }
                            }
                            else
                            {
                                storage = BindingStorage.ForLeafScopeField(fieldId, scopeId);
                            }
                        }
                    }
                    if (storage != null)
                    {
                        switch (storage.Kind)
                        {
                            case BindingStorageKind.IlArgument:
                                // Non-captured parameter - use LIRLoadParameter
                                if (storage.JsParameterIndex >= 0)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadParameter(storage.JsParameterIndex, resultTempVar));
                                    if (IsSafeInjectedCommonJsRequireParameter(binding))
                                    {
                                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(global::JavaScriptRuntime.CommonJS.RequireDelegate)));
                                    }
                                    else
                                    {
                                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    }
                                    return true;
                                }
                                break;

                            case BindingStorageKind.LeafScopeField:
                                // Captured variable in current scope - load from leaf scope field
                                if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, resultTempVar));
                                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.ParentScopeField:
                                // Captured variable in parent scope - load from parent scope field
                                if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    var parentIndex = storage.ParentScopeIndex;
                                    if ((_methodBodyIR.IsAsync && _methodBodyIR.AsyncInfo?.HasAwaits == true)
                                        || (_methodBodyIR.IsGenerator && (_methodBodyIR.GeneratorInfo?.YieldPointCount ?? 0) > 0))
                                    {
                                        // Resumables prepend leaf scope at scopes[0], shifting parents right by one.
                                        parentIndex += 1;
                                    }
                                    _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, parentIndex, resultTempVar));
                                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.IlLocal:
                                // Non-captured local - use SSA temp (fall through to default behavior)
                                break;
                        }
                    }
                }
                
                // Fallback: Check if this is a parameter (legacy behavior)
                if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadParameter(paramIndex, resultTempVar));
                    if (IsSafeInjectedCommonJsRequireParameter(binding))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(global::JavaScriptRuntime.CommonJS.RequireDelegate)));
                    }
                    else
                    {
                        // Parameters are typically treated as object (unknown type)
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }
                    return true;
                }
                
                if (!_variableMap.TryGetValue(binding, out resultTempVar))
                {
                    // Intrinsic globals (e.g., console, process, Infinity, NaN) are exposed via JavaScriptRuntime.GlobalThis.
                    // If this identifier is a Global binding and maps to a GlobalThis static property, emit a load.
                    if (varExpr.Name.Kind == BindingKind.Global)
                    {
                        var globalName = varExpr.Name.Name;
                        var gvType = typeof(JavaScriptRuntime.GlobalThis);
                        var gvProp = gvType.GetProperty(globalName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                        if (gvProp != null)
                        {
                            resultTempVar = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRGetIntrinsicGlobal(globalName, resultTempVar));
                            // Track the concrete CLR type when known (e.g., console -> JavaScriptRuntime.Console)
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, gvProp.PropertyType));
                            return true;
                        }

                        // Global functions (GlobalThis static methods) may also be referenced as values.
                        // e.g., window.setTimeout = setTimeout
                        var gvMethod = gvType.GetMethod(
                            globalName,
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                        if (gvMethod != null)
                        {
                            resultTempVar = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRGetIntrinsicGlobalFunction(globalName, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            return true;
                        }
                    }

                    // Function declarations are compiled separately and are not SSA-assigned in the HIR body.
                    // When a function declaration identifier is used as a value (e.g., returned or assigned),
                    // create a delegate and bind it to the appropriate scopes array.
                    if (varExpr.Name.BindingInfo.Kind == BindingKind.Function)
                    {
                        var callableId = TryCreateCallableIdForFunctionDeclaration(varExpr.Name);
                        if (callableId == null)
                        {
                            return false;
                        }

                        var scopesTemp = CreateTempVariable();
                        if (!TryBuildScopesArrayForCallee(varExpr.Name, scopesTemp))
                        {
                            return false;
                        }
                        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                        resultTempVar = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCreateBoundFunctionExpression(callableId, scopesTemp, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        // Cache the function value so repeated reads of the identifier within the same
                        // method return the same object identity (required for `.prototype` mutations).
                        _variableMap[binding] = resultTempVar;
                        return true;
                    }

                    IRPipelineMetrics.RecordFailureIfUnset(
                        $"HIR->LIR: no storage for variable '{binding.Name}' (kind={binding.Kind}, captured={binding.IsCaptured}, hasEnv={_environmentLayout != null}, scope='{_scope?.GetQualifiedName() ?? "<null>"}')");
                    return false;
                }

                // Variable reads are SSA value lookups (no load instruction).
                return true;

            case HIRArrowFunctionExpression arrowExpr:
                return TryLowerArrowFunctionExpression(arrowExpr, out resultTempVar);
            case HIRFunctionExpression funcExpr:
                return TryLowerFunctionExpression(funcExpr, out resultTempVar);
            case Js2IL.HIR.HIRUserClassTypeExpression userClassType:
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetUserClassType(userClassType.RegistryClassName, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                return true;
            // Handle different expression types here
            default:
                // Unsupported expression type
                IRPipelineMetrics.RecordFailure($"HIR->LIR: unsupported expression type {expression.GetType().Name}");
                return false;
        }
    }

    private bool TryLowerFunctionExpression(HIRFunctionExpression funcExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Scope is resolved during HIR construction (avoid keeping AST nodes in HIR).
        var funcScope = funcExpr.FunctionScope;

        // Build scopes[] to bind for closure semantics.
        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClosureBinding(funcScope, scopesTemp))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCreateBoundFunctionExpression(
            CallableId: funcExpr.CallableId,
            ScopesArray: scopesTemp,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerArrowFunctionExpression(HIRArrowFunctionExpression arrowExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Scope is resolved during HIR construction (avoid keeping AST nodes in HIR).
        var arrowScope = arrowExpr.FunctionScope;

        // Build scopes[] to bind for closure semantics.
        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClosureBinding(arrowScope, scopesTemp))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCreateBoundArrowFunction(
            CallableId: arrowExpr.CallableId,
            ScopesArray: scopesTemp,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryBuildScopesArrayForClosureBinding(Scope calleeScope, TempVariable resultTemp)
    {
        // Mirror the call-site logic: even when the callee doesn't reference parent variables,
        // preserve the ABI expectation that scopes[0] is the global scope when available.
        if (!calleeScope.ReferencesParentScopeVariables)
        {
            if (_scope == null)
            {
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
                return true;
            }

            var root = _scope;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            var moduleName = root.Name;
            var globalSlot = new ScopeSlot(Index: 0, ScopeName: moduleName, ScopeId: new ScopeId(moduleName));
            if (!TryMapScopeSlotToSource(globalSlot, out var globalSlotSource))
            {
                // Some call sites (notably synchronous class methods/constructors) may not have
                // access to a global scope instance when no scopes parameter/_scopes field is present.
                // For callables that don't reference parent-scope variables, fall back to the ABI-compatible
                // empty scopes array (1-element null) rather than failing compilation.
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
                return true;
            }

            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(new[] { globalSlotSource }, resultTemp));
            return true;
        }

        return TryBuildScopesArrayFromLayout(calleeScope, CallableKind.Function, resultTemp);
    }

    private static Scope? FindScopeByAstNode(Node astNode, Scope current)
    {
        if (ReferenceEquals(current.AstNode, astNode))
        {
            return current;
        }

        foreach (var child in current.Children)
        {
            if (FindScopeByAstNode(astNode, child) is { } found)
            {
                return found;
            }
        }

        return null;
    }

    private bool TryLowerTemplateLiteralExpression(HIRTemplateLiteralExpression templateLiteral, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var quasis = templateLiteral.Quasis;
        var exprs = templateLiteral.Expressions;

        // Start with the first quasi (or empty string if missing).
        var current = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(quasis.Count > 0 ? quasis[0] : string.Empty, current));
        DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        for (int i = 0; i < exprs.Count; i++)
        {
            if (!TryLowerExpression(exprs[i], out var exprTemp))
            {
                return false;
            }

            // Interpolations are converted to string using JS semantics.
            var exprAsString = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToString(exprTemp, exprAsString));
            DefineTempStorage(exprAsString, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var concat1 = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConcatStrings(current, exprAsString, concat1));
            DefineTempStorage(concat1, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            current = concat1;

            // Append the tail quasi (if present). Missing quasis are treated as empty string.
            var tail = (i + 1) < quasis.Count ? quasis[i + 1] : string.Empty;
            if (!string.IsNullOrEmpty(tail))
            {
                var tailTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstString(tail, tailTemp));
                DefineTempStorage(tailTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                var concat2 = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConcatStrings(current, tailTemp, concat2));
                DefineTempStorage(concat2, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                current = concat2;
            }
        }

        resultTempVar = current;
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return true;
    }

    private Js2IL.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForFunctionDeclaration(Symbol symbol)
    {
        if (_scope == null)
        {
            return null;
        }

        // IR lowering currently only supports direct calls where the callee is a function binding.
        if (symbol.BindingInfo.Kind != BindingKind.Function)
        {
            return null;
        }

        var declNode = symbol.BindingInfo.DeclarationNode;
        var declaringScope = FindDeclaringScope(symbol.BindingInfo);
        if (declaringScope == null)
        {
            return null;
        }

        static Scope? FindCallableBodyScope(Scope scope, Node decl)
        {
            if (scope.Kind == ScopeKind.Function && scope.AstNode != null && ReferenceEquals(scope.AstNode, decl))
            {
                return scope;
            }

            foreach (var child in scope.Children)
            {
                var match = FindCallableBodyScope(child, decl);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        // For named function expressions, the function name is bound inside the function scope,
        // but the callable itself is declared in the parent scope (Phase 1 discovery uses the parent).
        // If we detect that pattern, shift the DeclaringScopeName to the parent scope.
        var callableDeclaringScope = declaringScope;
        if (declNode is FunctionExpression funcExprDecl &&
            declaringScope.AstNode is FunctionExpression scopeFuncExpr &&
            ReferenceEquals(funcExprDecl, scopeFuncExpr) &&
            declaringScope.Parent != null)
        {
            callableDeclaringScope = declaringScope.Parent;
        }

        var root = callableDeclaringScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var declaringScopeName = callableDeclaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{callableDeclaringScope.GetQualifiedName()}";

        var bodyScope = FindCallableBodyScope(declaringScope, declNode);
        var needsArgumentsObject = bodyScope?.NeedsArgumentsObject ?? false;

        switch (declNode)
        {
            case FunctionDeclaration funcDecl:
                return new Js2IL.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.FunctionDeclaration,
                    DeclaringScopeName = declaringScopeName,
                    Name = symbol.Name,
                    JsParamCount = funcDecl.Params.Count,
                    NeedsArgumentsObject = needsArgumentsObject,
                    AstNode = funcDecl
                };

            case FunctionExpression funcExpr:
                return new Js2IL.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.FunctionExpression,
                    DeclaringScopeName = declaringScopeName,
                    Name = (funcExpr.Id as Identifier)?.Name,
                    Location = Js2IL.Services.TwoPhaseCompilation.SourceLocation.FromNode(funcExpr),
                    JsParamCount = funcExpr.Params.Count,
                    NeedsArgumentsObject = needsArgumentsObject,
                    AstNode = funcExpr
                };

            default:
                return null;
        }
    }

    private Scope? FindDeclaringScope(BindingInfo binding)
    {
        var current = _scope;
        while (current != null)
        {
            if (current.Bindings.TryGetValue(binding.Name, out var candidate) && ReferenceEquals(candidate, binding))
            {
                return current;
            }
            current = current.Parent;
        }
        return null;
    }

}
