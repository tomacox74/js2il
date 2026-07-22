using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;
using System;

namespace Jroc.IR;

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

        if (expression is HIRAssignmentExpression assignmentExpr)
        {
            return TryLowerAssignmentExpression(assignmentExpr, out _, resultUsed: false);
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

        if (expression is HIRCallExpression
            {
                Callee: HIRVariableExpression { Name.Kind: BindingKind.Global, Name.Name: "Number" }
            } numberCall)
        {
            if (!TryEvaluateCallArguments(numberCall.Arguments, 1, out var args))
            {
                return false;
            }

            if (args.Count > 0)
            {
                _methodBodyIR.Instructions.Add(new LIRConvertToNumberDiscard(args[0]));
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

            case HIRThisExpression thisExpr:
                // PL3.5: ThisExpression.
                if (thisExpr.StaticClassRegistryName != null)
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(thisExpr.StaticClassRegistryName, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    return true;
                }

                if (_callableKind == CallableKind.ClassStaticInitializer
                    && TryGetEnclosingClassRegistryName(out var staticInitializerClass)
                    && staticInitializerClass != null)
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(staticInitializerClass, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    return true;
                }

                // Only supported for instance callables where IL arg0 is the receiver.
                if (_callableKind is not CallableKind.ClassMethod
                    and not CallableKind.ClassStaticMethod
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

            case HIRNewTargetExpression:
                // Function/arrow path: use hidden newTarget parameter.
                // Constructor path (class ctor): approximate new.target as this.GetType().
                if (_callableKind == CallableKind.Constructor)
                {
                    var thisTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadThis(thisTemp));
                    DefineTempStorage(thisTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                        thisTemp,
                        typeof(object),
                        nameof(object.GetType),
                        Array.Empty<TempVariable>(),
                        resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    return true;
                }

                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadNewTarget(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;

            case HIRImportMetaExpression:
                {
                    TempVariable moduleIdTemp;
                    if (!TryLowerCurrentModulePath(out moduleIdTemp))
                    {
                        return false;
                    }

                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                        MethodName: nameof(JavaScriptRuntime.RuntimeServices.GetImportMeta),
                        Arguments: new[] { moduleIdTemp },
                        Result: resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }

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

            case HIRImportExpression importExpr:
                if (!TryLowerImportExpression(importExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering ImportExpression");
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

            case HIRTaggedTemplateExpression taggedTemplateExpr:
                if (!TryLowerTaggedTemplateExpression(taggedTemplateExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering TaggedTemplateExpression");
                    return false;
                }
                return true;

            case HIRTemplateLiteralExpression templateLiteral:
                if (!TryLowerTemplateLiteralExpression(templateLiteral, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering TemplateLiteralExpression");
                    return false;
                }
                return true;

            case HIRAssignmentExpression assignExpr:
                return TryLowerAssignmentExpression(assignExpr, out resultTempVar);

            case HIRThrowTypeErrorExpression throwTypeErrorExpr:
                _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError(throwTypeErrorExpr.Message));
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;

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

            case HIRPrivateFieldAssignmentExpression privateFieldAssignExpr:
                if (!TryLowerExpression(privateFieldAssignExpr.Value, out var privateFieldValueTemp))
                {
                    return false;
                }

                privateFieldValueTemp = EnsureObject(privateFieldValueTemp);
                _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                    privateFieldAssignExpr.RegistryClassName,
                    privateFieldAssignExpr.FieldName,
                    IsPrivateField: true,
                    privateFieldValueTemp));

                resultTempVar = privateFieldValueTemp;
                return true;

            case HIRPrivateAccessorAssignmentExpression privateAccessorAssignExpr:
                if (_classRegistry == null
                    || !TryGetEnclosingClassRegistryName(out var privateAccessorClass)
                    || privateAccessorClass == null
                    || !_classRegistry.TryGetMethod(privateAccessorClass, privateAccessorAssignExpr.SetterMethodName, out var setterHandle, out _, out _, out _, out var hasSetterScopesParam, out _, out var setterMaxParamCount))
                {
                    return false;
                }

                if (!TryLowerExpression(privateAccessorAssignExpr.Value, out var privateAccessorValueTemp))
                {
                    return false;
                }

                var privateSetterResultTemp = CreateTempVariable();
                DefineTempStorage(privateSetterResultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                _methodBodyIR.Instructions.Add(new LIRCallUserClassInstanceMethod(
                    privateAccessorClass,
                    privateAccessorAssignExpr.SetterMethodName,
                    setterHandle,
                    hasSetterScopesParam,
                    RequiresPrivateBrandCheck: false,
                    setterMaxParamCount,
                    new[] { EnsureObject(privateAccessorValueTemp) },
                    privateSetterResultTemp));

                resultTempVar = privateAccessorValueTemp;
                return true;

            case HIRIndexAccessExpression indexAccessExpr:
                return TryLowerIndexAccessExpression(indexAccessExpr, out resultTempVar);

            case HIRVariableExpression varExpr:
                // Look up the binding using the Symbol's BindingInfo directly
                // This correctly resolves shadowed variables to the right binding
                var binding = varExpr.Name.BindingInfo;
                EmitWithBindingProbe(binding.Name);

                // Class declarations are compiled separately (as CLR types) and are not SSA-assigned.
                // Always lower a class identifier to a runtime System.Type so it can cross module boundaries
                // (e.g., `module.exports = { Counter }`).
                if (binding.DeclarationNode is ClassDeclaration classDecl)
                {
                    // Prefer the already-initialized binding value so class identity is stable across
                    // constructor/method/accessor reads. During evaluation-before-initialization, the
                    // scope-field load will still surface the TDZ sentinel as the correct runtime error.
                    if (_variableMap.TryGetValue(binding, out resultTempVar))
                    {
                        return true;
                    }

                    if (TryLoadVariable(binding, out resultTempVar))
                    {
                        return true;
                    }

                    if (!TryGetRegistryClassNameForClassDeclaration(classDecl, out var registryClassName))
                    {
                        return false;
                    }

                    if (_scope != null)
                    {
                        var rootScope = _scope;
                        while (rootScope.Parent != null)
                        {
                            rootScope = rootScope.Parent;
                        }

                        var classScope = FindScopeByDeclarationNode(classDecl, rootScope);
                        if (classScope != null && TryLowerClassConstructorValue(registryClassName, classScope, out resultTempVar))
                        {
                            return true;
                        }
                        // Fall through: TryLowerClassConstructorValue failed (e.g. static method with no
                        // scopes access), emit a simple type token instead.
                    }

                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(registryClassName, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    return true;
                }

                ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
                {
                    if (IsSafeInjectedCommonJsRequireParameter(b))
                    {
                        return new ValueStorage(ValueStorageKind.Reference, typeof(global::JavaScriptRuntime.CommonJS.RequireDelegate));
                    }

                    // var bindings can be observed as `undefined` before their initializer runs.
                    // Keep reads boxed to preserve that state shape across all control-flow paths.
                    if (b.Kind == BindingKind.Var
                        && !b.DeclaringScope.Parameters.Contains(b.Name)
                        && !b.CanUseUnboxedLocal)
                    {
                        return new ValueStorage(ValueStorageKind.Reference, typeof(object));
                    }

                    if (b.RequiresRuntimeTemporalDeadZoneChecks)
                    {
                        return new ValueStorage(ValueStorageKind.Reference, typeof(object));
                    }

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
                        if (b.ClrType == typeof(string) && CanUseStringLocalStorage(b))
                        {
                            return new ValueStorage(ValueStorageKind.Reference, typeof(string));
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

                TempVariable EmitTemporalDeadZoneReferenceError(BindingInfo b)
                {
                    var messageTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString($"Cannot access '{b.Name}' before initialization", messageTemp));
                    DefineTempStorage(messageTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    var errorTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRNewBuiltInError("ReferenceError", messageTemp, errorTemp));
                    DefineTempStorage(errorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    _methodBodyIR.Instructions.Add(new LIRThrow(errorTemp));

                    var result = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return result;
                }

                bool IsParameterTemporallyUninitialized(BindingInfo b)
                    => _scope?.HasParameterExpressions == true
                       && _currentDefaultParameterIndex is int defaultParameterIndex
                       && _parameterIndexMap.TryGetValue(b, out var referencedParameterIndex)
                       && referencedParameterIndex >= defaultParameterIndex;

                if (IsParameterTemporallyUninitialized(binding))
                {
                    resultTempVar = EmitTemporalDeadZoneReferenceError(binding);
                    return true;
                }

                if (_scope?.HasParameterExpressions == true
                    && _currentDefaultParameterIndex is int currentDefaultParameterIndex
                    && _parameterIndexMap.TryGetValue(binding, out var referencedParameterIndex)
                    && referencedParameterIndex < currentDefaultParameterIndex)
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadParameter(referencedParameterIndex, resultTempVar));
                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                    _tempBindingOrigin[resultTempVar] = binding;
                    return true;
                }

                // Per-iteration environments: if this binding lives in an active materialized scope instance
                // (e.g., `for (let/const ...)` loop-head scope), load directly from that scope field.
                if (binding.Kind != BindingKind.Global
                    && TryGetActiveScopeFieldStorage(binding, out var activeScopeTemp, out var activeScopeId, out var activeFieldId))
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadScopeField(activeScopeTemp, binding, activeFieldId, activeScopeId, resultTempVar));
                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                    _tempBindingOrigin[resultTempVar] = binding;
                    return true;
                }

                // Flow-sensitive numeric refinement: if this binding was previously proven to hold an
                // unboxed double (e.g. via an earlier Number(x) call or EnsureNumber coercion within
                // this basic block), return that temp directly to avoid a redundant ToNumber call.
                SyncNumericRefinementStateWithLabels();
                if (CanTrackNumericRefinement(binding) && _numericRefinements.TryGetValue(binding, out var numericRefined))
                {
                    resultTempVar = numericRefined;
                    return true;
                }
                
                // Check if this binding is stored in a scope field (captured variable)
                if (binding.Kind != BindingKind.Global && _environmentLayout != null)
                {
                    var storage = _environmentLayout.GetStorage(binding);
                    if (storage == null
                        && _scope != null
                        && binding.IsCaptured)
                    {
                        // Fallback: if the environment layout didn't include this binding (e.g., due to
                        // a BindingInfo identity mismatch or overly-conservative storage map), try to
                        // compute scope-field storage from the caller's scope chain.
                        //
                        // This is only valid for captured bindings that are stored as fields on their
                        // declaring scope type.
                        var declaringScope = binding.DeclaringScope;

                        if (declaringScope != null)
                        {
                            var declaringRegistryName = ScopeNaming.GetRegistryScopeName(declaringScope);
                            var scopeId = new ScopeId(declaringRegistryName);
                            var fieldId = new FieldId(declaringRegistryName, binding.Name);

                            if (!ReferenceEquals(declaringScope, _scope))
                            {
                                var parentIndex = _environmentLayout.ScopeChain.IndexOf(declaringRegistryName);
                                if (parentIndex < 0
                                    && declaringScope.Kind == ScopeKind.Global
                                    && _environmentLayout.Abi.ScopesSource is ScopesSource.Argument or ScopesSource.ThisField)
                                {
                                    parentIndex = 0;
                                }
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
                                        DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                        _tempBindingOrigin[resultTempVar] = binding;
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
                                    resultTempVar = EmitResolveWithBindingOrDefault(binding, resultTempVar);
                                    _tempBindingOrigin[resultTempVar] = binding;
                                    return true;
                                }
                                break;

                            case BindingStorageKind.ParentScopeField:
                                // Captured variable in parent scope - load from parent scope field
                                if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    var parentIndex = AdjustParentScopeFieldIndexForCurrentMethod(storage.ParentScopeIndex);
                                    _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, parentIndex, resultTempVar));
                                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                    resultTempVar = EmitResolveWithBindingOrDefault(binding, resultTempVar);
                                    _tempBindingOrigin[resultTempVar] = binding;
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
                        DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                        _tempBindingOrigin[resultTempVar] = binding;
                    }
                    return true;
                }
                
                if (TryMaterializeStringBuilderAccumulator(binding, out resultTempVar))
                {
                    return true;
                }

                if (!_variableMap.TryGetValue(binding, out resultTempVar))
                {
                    if (binding.RequiresTemporalDeadZoneChecks)
                    {
                        resultTempVar = EmitTemporalDeadZoneReferenceError(binding);
                        return true;
                    }

                    if (binding.Kind == BindingKind.Var && _environmentLayout != null)
                    {
                        var storage = _environmentLayout.GetStorage(binding);
                        if (storage != null)
                        {
                            if (storage.Kind == BindingStorageKind.LeafScopeField
                                && !storage.Field.IsNil
                                && !storage.DeclaringScope.IsNil)
                            {
                                resultTempVar = CreateTempVariable();
                                _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, resultTempVar));
                                DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                resultTempVar = EmitResolveWithBindingOrDefault(binding, resultTempVar);
                                _tempBindingOrigin[resultTempVar] = binding;
                                _variableMap[binding] = resultTempVar;
                                return true;
                            }

                            if (storage.Kind == BindingStorageKind.IlLocal)
                            {
                                resultTempVar = CreateTempVariable();
                                var defaultStorage = new ValueStorage(ValueStorageKind.Reference, typeof(object));
                                _methodBodyIR.Instructions.Insert(0, new LIRConstUndefined(resultTempVar));
                                DefineTempStorage(resultTempVar, defaultStorage);
                                var slot = GetOrCreateVariableSlot(binding, varExpr.Name.Name, defaultStorage);
                                SetTempVariableSlot(resultTempVar, slot);
                                _tempBindingOrigin[resultTempVar] = binding;
                                _variableMap[binding] = resultTempVar;
                                return true;
                            }
                        }
                    }

                    // Global bindings are resolved through the runtime global object so user code can
                    // observe mutations like `globalThis.Object = fakeObject`.
                    if (varExpr.Name.Kind == BindingKind.Global)
                    {
                        var globalName = varExpr.Name.Name;

                        var keyTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstString(globalName, keyTemp));
                        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                        resultTempVar = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                            nameof(JavaScriptRuntime.ObjectRuntime),
                            nameof(JavaScriptRuntime.ObjectRuntime.GetGlobalBindingValue),
                            new[] { EnsureObject(keyTemp) },
                            resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        return true;
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
                        var isAsync =
                            varExpr.Name.BindingInfo.DeclarationNode is FunctionDeclaration asyncFunctionDeclarationBinding
                            && asyncFunctionDeclarationBinding.Async;
                        var isAsyncGeneratorFunction =
                            varExpr.Name.BindingInfo.DeclarationNode is FunctionDeclaration functionDeclarationBinding
                            && functionDeclarationBinding.Async
                            && functionDeclarationBinding.Generator;

                        _methodBodyIR.Instructions.Add(new LIRCreateBoundFunctionExpression(
                            callableId,
                            scopesTemp,
                            resultTempVar,
                            IsAsyncGeneratorFunction: isAsyncGeneratorFunction,
                            IsAsync: isAsync));
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
            case HIRInitializedUserClassTypeExpression initializedUserClassType:
                if (TryLowerNamedClassExpressionInitialization(initializedUserClassType, out resultTempVar))
                {
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(_pendingAnonymousClassExpressionInferredName)
                    && initializedUserClassType.InitializationStatements.Count > 0)
                {
                    var classTypeTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(initializedUserClassType.RegistryClassName, classTypeTemp));
                    DefineTempStorage(classTypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));

                    var inferredNameTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString(_pendingAnonymousClassExpressionInferredName, inferredNameTemp));
                    DefineTempStorage(inferredNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    var namedClassTypeTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                        MethodName: nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorInferredName),
                        Arguments: new[] { EnsureObject(classTypeTemp), EnsureObject(inferredNameTemp) },
                        Result: namedClassTypeTemp));
                    DefineTempStorage(namedClassTypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    SetTempVariableSlot(
                        namedClassTypeTemp,
                        CreateAnonymousVariableSlot("$anon_class_type_with_inferred_name", new ValueStorage(ValueStorageKind.Reference, typeof(object))));
                }

                // Derived classes must evaluate and link their constructor before static
                // initialization. For ordinary classes, preserve the established ordering:
                // computed class keys can suspend, so construct the final class value after
                // their initialization has completed.
                if (initializedUserClassType.SuperClass != null)
                {
                    if (!TryLowerClassConstructorValue(
                            initializedUserClassType.RegistryClassName,
                            initializedUserClassType.ClassScope,
                            out resultTempVar))
                    {
                        resultTempVar = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetUserClassType(initializedUserClassType.RegistryClassName, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                    }

                    if (!TryLinkClassConstructorToSuperClass(initializedUserClassType, ref resultTempVar))
                    {
                        resultTempVar = default;
                        return false;
                    }
                }

                foreach (var initStatement in initializedUserClassType.InitializationStatements)
                {
                    if (!TryLowerStatement(initStatement))
                    {
                        resultTempVar = default;
                        return false;
                    }
                }

                if (initializedUserClassType.SuperClass == null
                    && !TryLowerClassConstructorValue(
                        initializedUserClassType.RegistryClassName,
                        initializedUserClassType.ClassScope,
                        out resultTempVar))
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRGetUserClassType(initializedUserClassType.RegistryClassName, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                }

                if (!string.IsNullOrWhiteSpace(_pendingAnonymousClassExpressionInferredName)
                    && initializedUserClassType.InitializationStatements.Count > 0)
                {
                    var inferredNameTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString(_pendingAnonymousClassExpressionInferredName, inferredNameTemp));
                    DefineTempStorage(inferredNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    var namedClassConstructorTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                        MethodName: nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorInferredName),
                        Arguments: new[] { EnsureObject(resultTempVar), EnsureObject(inferredNameTemp) },
                        Result: namedClassConstructorTemp));
                    DefineTempStorage(namedClassConstructorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    resultTempVar = namedClassConstructorTemp;
                }
                return true;
            case Jroc.HIR.HIRUserClassTypeExpression userClassType:
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetUserClassType(userClassType.RegistryClassName, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
                return true;
            case HIRClassHeritageValidationExpression validateClassHeritage:
                return TryLowerClassHeritageValidationExpression(validateClassHeritage, out resultTempVar);
            case HIRDefineClassDataPropertyExpression defineClassDataProperty:
                return TryLowerDefineClassDataPropertyExpression(defineClassDataProperty, out resultTempVar);
            case HIRDefineClassAccessorPropertyExpression defineClassAccessorProperty:
                return TryLowerDefineClassAccessorPropertyExpression(defineClassAccessorProperty, out resultTempVar);
            case HIRDefineClassAccessorMethodPropertyExpression defineClassAccessorMethodProperty:
                return TryLowerDefineClassAccessorMethodPropertyExpression(defineClassAccessorMethodProperty, out resultTempVar);
            case HIRDefineClassMethodDataPropertiesExpression defineClassMethodDataProperties:
                return TryLowerDefineClassMethodDataPropertiesExpression(defineClassMethodDataProperties, out resultTempVar);
            // Handle different expression types here
            default:
                // Unsupported expression type
                IRPipelineMetrics.RecordFailure($"HIR->LIR: unsupported expression type {expression.GetType().Name}");
                return false;
        }
    }

    private bool TryLowerNamedClassExpressionInitialization(HIRInitializedUserClassTypeExpression initializedUserClassType, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (initializedUserClassType.ClassScope.AstNode is not ClassExpression classExpression
            || classExpression.Id is not Identifier className
            || !initializedUserClassType.ClassScope.Bindings.TryGetValue(className.Name, out var classNameBinding))
        {
            return false;
        }

        var classNameBindingDeclarationIndex = -1;
        for (var index = 0; index < initializedUserClassType.InitializationStatements.Count; index++)
        {
            if (initializedUserClassType.InitializationStatements[index] is HIRVariableDeclaration variableDeclaration
                && ReferenceEquals(variableDeclaration.Name.BindingInfo, classNameBinding)
                && variableDeclaration.Initializer is HIRInitializedUserClassTypeExpression initializer
                && ReferenceEquals(initializer.ClassScope, initializedUserClassType.ClassScope)
                && initializer.InitializationStatements.Count == 0)
            {
                classNameBindingDeclarationIndex = index;
                break;
            }
        }

        if (classNameBindingDeclarationIndex < 0)
        {
            return false;
        }

        var classScopeName = ScopeNaming.GetRegistryScopeName(initializedUserClassType.ClassScope);
        var classScopeTemp = CreateTempVariable();
        DefineTempStorage(classScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: classScopeName));
        SetTempVariableSlot(classScopeTemp, CreateAnonymousVariableSlot($"$class_lexenv_{classScopeName}", new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: classScopeName)));
        _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(new ScopeId(classScopeName), classScopeTemp));

        var hadPreviousScope = _activeScopeTempsByScopeName.TryGetValue(classScopeName, out var previousScopeTemp);
        _activeScopeTempsByScopeName[classScopeName] = classScopeTemp;
        try
        {
            if (!string.IsNullOrWhiteSpace(_pendingAnonymousClassExpressionInferredName))
            {
                var classTypeTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetUserClassType(initializedUserClassType.RegistryClassName, classTypeTemp));
                DefineTempStorage(classTypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));

                var inferredNameTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstString(_pendingAnonymousClassExpressionInferredName, inferredNameTemp));
                DefineTempStorage(inferredNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                var namedClassTypeTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                    MethodName: nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorInferredName),
                    Arguments: new[] { EnsureObject(classTypeTemp), EnsureObject(inferredNameTemp) },
                    Result: namedClassTypeTemp));
                DefineTempStorage(namedClassTypeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                SetTempVariableSlot(
                    namedClassTypeTemp,
                    CreateAnonymousVariableSlot("$anon_class_type_with_inferred_name", new ValueStorage(ValueStorageKind.Reference, typeof(object))));
            }

            if (!TryLowerClassConstructorValue(initializedUserClassType.RegistryClassName, initializedUserClassType.ClassScope, out var classConstructorValue))
            {
                return false;
            }

            if (!TryLinkClassConstructorToSuperClass(initializedUserClassType, ref classConstructorValue))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_pendingAnonymousClassExpressionInferredName))
            {
                var inferredNameTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstString(_pendingAnonymousClassExpressionInferredName, inferredNameTemp));
                DefineTempStorage(inferredNameTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                var namedClassConstructorTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                    MethodName: nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorInferredName),
                    Arguments: new[] { EnsureObject(classConstructorValue), EnsureObject(inferredNameTemp) },
                    Result: namedClassConstructorTemp));
                DefineTempStorage(namedClassConstructorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                classConstructorValue = namedClassConstructorTemp;
            }

            for (var index = 0; index < initializedUserClassType.InitializationStatements.Count; index++)
            {
                if (index == classNameBindingDeclarationIndex)
                {
                    if (!TryStoreToBinding(classNameBinding, classConstructorValue, out _))
                    {
                        return false;
                    }

                    continue;
                }

                if (!TryLowerStatement(initializedUserClassType.InitializationStatements[index]))
                {
                    return false;
                }
            }

            resultTempVar = classConstructorValue;
            return true;
        }
        finally
        {
            if (hadPreviousScope)
            {
                _activeScopeTempsByScopeName[classScopeName] = previousScopeTemp;
            }
            else
            {
                _activeScopeTempsByScopeName.Remove(classScopeName);
            }
        }
    }

    private bool TryLowerClassConstructorValue(string registryClassName, Scope classScope, out TempVariable resultTempVar)
    {
        // Build scopes array first — if this fails, nothing has been emitted yet so the caller
        // can fall back to a simple LIRGetUserClassType without leaving orphaned IR instructions.
        // This happens in static CLR methods (property getters/setters) that have ScopesSource.None.
        var scopesTemp = CreateTempVariable();
        var allowEmptyOnUnmappedGlobal = !DoesClassNeedParentScopes(classScope);
        if (!TryBuildScopesArrayForClassConstructor(classScope, scopesTemp, allowEmptyOnUnmappedGlobal))
        {
            resultTempVar = default;
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        if (!_classMethodOwnerTempsByRegistryName.Remove(registryClassName, out var typeTemp))
        {
            typeTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetUserClassType(registryClassName, typeTemp));
            DefineTempStorage(typeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
        }

        // Look up the formal parameter count for Function.length semantics (params before defaults/rest).
        int minParamCount = 0;
        if (_classRegistry != null && _classRegistry.TryGetConstructor(registryClassName, out _, out _, out var minP, out _))
        {
            minParamCount = minP;
        }

        var paramCountTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber((double)minParamCount, paramCountTemp));
        DefineTempStorage(paramCountTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            MethodName: nameof(JavaScriptRuntime.RuntimeServices.CreateClassConstructorValue),
            Arguments: new[] { EnsureObject(typeTemp), EnsureObject(scopesTemp), EnsureObject(paramCountTemp) },
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return true;
    }

    private bool TryLinkClassConstructorToSuperClass(
        HIRInitializedUserClassTypeExpression classExpression,
        ref TempVariable classConstructorTemp)
    {
        if (classExpression.SuperClass == null)
        {
            return true;
        }

        if (!TryLowerExpression(classExpression.SuperClass, out var superClassTemp))
        {
            return false;
        }

        var linkedConstructorTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            MethodName: nameof(JavaScriptRuntime.RuntimeServices.SetClassConstructorPrototype),
            Arguments: new[] { EnsureObject(classConstructorTemp), EnsureObject(superClassTemp) },
            Result: linkedConstructorTemp));
        DefineTempStorage(linkedConstructorTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        classConstructorTemp = linkedConstructorTemp;
        return true;
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
            Result: resultTempVar,
            IsAsyncGeneratorFunction: funcScope.IsAsync && funcScope.IsGenerator,
            IsAsync: funcScope.IsAsync));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        if (funcExpr.IsNonConstructible)
        {
            resultTempVar = EmitMarkUndefinedPrototype(resultTempVar);
        }

        resultTempVar = EmitBindWithObjectIfNeeded(resultTempVar);

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
            IsAsync: arrowScope.IsAsync,
            RequiresLexicalSuperConstructorContext: arrowExpr.RequiresLexicalSuperConstructorContext,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        resultTempVar = EmitBindWithObjectIfNeeded(resultTempVar);
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

    private bool TryLowerTaggedTemplateExpression(HIRTaggedTemplateExpression taggedTemplate, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var template = taggedTemplate.Template;
        var quasis = template.Quasis;
        var rawQuasis = template.RawQuasis;
        var exprs = template.Expressions;

        // 1. Evaluate the tag expression
        if (!TryLowerExpression(taggedTemplate.Tag, out var tagTemp))
        {
            return false;
        }

        // 2. Create the template object (cooked + raw strings)
        var scopeName = _scope?.GetQualifiedName() ?? "UnknownScope";
        var callSiteId = taggedTemplate.Location is { } location
            ? $"{scopeName}:TaggedTemplate_{location}"
            : $"{scopeName}:TaggedTemplate";
        
        // Create cooked strings array
        var cookedStringTemps = new List<TempVariable>();
        for (int i = 0; i < quasis.Count; i++)
        {
            var stringTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(quasis[i], stringTemp));
            DefineTempStorage(stringTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            cookedStringTemps.Add(stringTemp);
        }

        var cookedArrayTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(cookedStringTemps, cookedArrayTemp));
        DefineTempStorage(cookedArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        // Create raw strings array (rawQuasis has same length as quasis when present)
        var rawStringTemps = new List<TempVariable>();
        var rawArrayCount = quasis.Count;
        for (int i = 0; i < rawArrayCount; i++)
        {
            var rawString = rawQuasis?[i] ?? quasis[i];
            var stringTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString(rawString, stringTemp));
            DefineTempStorage(stringTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            rawStringTemps.Add(stringTemp);
        }

        var rawArrayTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(rawStringTemps, rawArrayTemp));
        DefineTempStorage(rawArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        // Call RuntimeServices.CreateTemplateObject
        var callSiteIdTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(callSiteId, callSiteIdTemp));
        DefineTempStorage(callSiteIdTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var templateObjectTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.CreateTemplateObject),
            new[] { callSiteIdTemp, cookedArrayTemp, rawArrayTemp },
            templateObjectTemp));
        DefineTempStorage(templateObjectTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // 3. Evaluate substitution expressions left-to-right
        var substitutionTemps = new List<TempVariable>();
        foreach (var expr in exprs)
        {
            if (!TryLowerExpression(expr, out var exprTemp))
            {
                return false;
            }
            // Ensure substitution temps are object-compatible
            var objectTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToObject(exprTemp, typeof(object), objectTemp));
            DefineTempStorage(objectTemp, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            substitutionTemps.Add(objectTemp);
        }

        // 4. Build arguments array: [templateObject, ...substitutions]
        var allArgTemps = new List<TempVariable> { templateObjectTemp };
        allArgTemps.AddRange(substitutionTemps);

        var argsArrayTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(allArgTemps, argsArrayTemp));
        DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        // 5. Call the tag function with the arguments
        // Need to get the scopes array for the current context
        var scopesArrayTemp = CreateTempVariable();
        if (_callableKind is Services.ScopesAbi.CallableKind.Function)
        {
            // Load scopes from parameter
            _methodBodyIR.Instructions.Add(new LIRLoadScopesArgument(scopesArrayTemp));
            DefineTempStorage(scopesArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        }
        else
        {
            // No scopes available (e.g., in ModuleMain or class methods) - create empty array
            var emptyList = new List<TempVariable>();
            _methodBodyIR.Instructions.Add(new LIRBuildArray(emptyList, scopesArrayTemp));
            DefineTempStorage(scopesArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        }

        var callResultTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(tagTemp, scopesArrayTemp, argsArrayTemp, callResultTemp));
        DefineTempStorage(callResultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        resultTempVar = callResultTemp;
        return true;
    }

    private Jroc.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForFunctionDeclaration(Symbol symbol)
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
        var hasRestParameters = bodyScope?.HasRestParameters ?? false;
        var isStrictScope = bodyScope != null && Jroc.Utilities.ArgumentsObjectSemantics.IsStrictScope(bodyScope);

        switch (declNode)
        {
            case FunctionDeclaration funcDecl:
                return new Jroc.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Jroc.Services.TwoPhaseCompilation.CallableKind.FunctionDeclaration,
                    DeclaringScopeName = declaringScopeName,
                    Name = symbol.Name,
                    JsParamCount = CountNonRestParameters(funcDecl.Params),
                    NeedsArgumentsObject = needsArgumentsObject,
                    HasRestParameters = hasRestParameters,
                    UsesMappedArgumentsObject = bodyScope != null && Jroc.Utilities.ArgumentsObjectSemantics.UsesMappedArgumentsObject(bodyScope),
                    ArgumentsParameterNames = bodyScope != null ? Jroc.Utilities.ArgumentsObjectSemantics.GetMappedParameterNames(bodyScope) : Array.Empty<string>(),
                    IncludeCalleeInArgumentsObject = needsArgumentsObject && !isStrictScope,
                    HasRestrictedFunctionProperties = isStrictScope,
                    AstNode = funcDecl
                };

            case FunctionExpression funcExpr:
                return new Jroc.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Jroc.Services.TwoPhaseCompilation.CallableKind.FunctionExpression,
                    DeclaringScopeName = declaringScopeName,
                    Name = (funcExpr.Id as Identifier)?.Name,
                    Location = Jroc.Services.TwoPhaseCompilation.SourceLocation.FromNode(funcExpr),
                    JsParamCount = CountNonRestParameters(funcExpr.Params),
                    NeedsArgumentsObject = needsArgumentsObject,
                    HasRestParameters = hasRestParameters,
                    UsesMappedArgumentsObject = bodyScope != null && Jroc.Utilities.ArgumentsObjectSemantics.UsesMappedArgumentsObject(bodyScope),
                    ArgumentsParameterNames = bodyScope != null ? Jroc.Utilities.ArgumentsObjectSemantics.GetMappedParameterNames(bodyScope) : Array.Empty<string>(),
                    IncludeCalleeInArgumentsObject = needsArgumentsObject && !isStrictScope,
                    HasRestrictedFunctionProperties = isStrictScope,
                    AstNode = funcExpr
                };

            default:
                return null;
        }
    }

    private bool TryCreateCallableIdForConstInitializedArrow(
        Symbol symbol,
        out TwoPhase.CallableId callableId,
        out Scope bodyScope)
    {
        callableId = null!;
        bodyScope = null!;

        if (_scope == null
            || symbol.BindingInfo.Kind != BindingKind.Const
            || symbol.BindingInfo.RequiresRuntimeTemporalDeadZoneChecks
            || symbol.BindingInfo.DeclarationNode is not VariableDeclarator { Init: ArrowFunctionExpression arrowExpr })
        {
            return false;
        }

        if (ArrowRequiresRuntimeClosureInvocation(arrowExpr))
        {
            return false;
        }

        var root = _scope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var arrowScope = FindScopeByDeclarationNode(arrowExpr, root);
        if (arrowScope == null || arrowScope.Parent == null)
        {
            return false;
        }

        var declaringScope = arrowScope.Parent;
        var moduleName = root.Name;
        var declaringScopeName = declaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{declaringScope.GetQualifiedName()}";

        bodyScope = arrowScope;
        callableId = new TwoPhase.CallableId
        {
            Kind = TwoPhase.CallableKind.Arrow,
            DeclaringScopeName = declaringScopeName,
            Name = null,
            Location = TwoPhase.SourceLocation.FromNode(arrowExpr),
            JsParamCount = CountNonRestParameters(arrowExpr.Params),
            NeedsArgumentsObject = false,
            HasRestParameters = arrowScope.HasRestParameters,
            UsesMappedArgumentsObject = false,
            ArgumentsParameterNames = Array.Empty<string>(),
            IncludeCalleeInArgumentsObject = false,
            HasRestrictedFunctionProperties = false,
            AstNode = arrowExpr
        };
        return true;
    }

    private static bool ArrowRequiresRuntimeClosureInvocation(ArrowFunctionExpression arrowExpr)
    {
        var requiresRuntimeClosure = false;
        var walker = new AstWalker();
        walker.Visit(arrowExpr.Body, node =>
        {
            if (node is ThisExpression
                || node is MetaProperty
                || node is Identifier { Name: "arguments" })
            {
                requiresRuntimeClosure = true;
            }
        });
        return requiresRuntimeClosure;
    }

    private Scope? FindDeclaringScope(BindingInfo binding)
    {
        if (binding.DeclaringScope != null)
        {
            return binding.DeclaringScope;
        }

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

    /// <summary>
    /// Counts parameters excluding rest parameters.
    /// Rest parameters don't become IL method parameters.
    /// </summary>
    private static int CountNonRestParameters(Acornima.Ast.NodeList<Acornima.Ast.Node> parameters)
    {
        int count = 0;
        foreach (var param in parameters)
        {
            if (param is not Acornima.Ast.RestElement)
            {
                count++;
            }
        }
        return count;
    }
}
