using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerReturnStatement(HIRReturnStatement returnStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        TempVariable returnTempVar;
        if (_callableKind == CallableKind.Constructor && returnStmt.Expression != null)
        {
            // Constructors are void-returning in IL, but JavaScript allows `return <expr>`.
            // Stash the value so the `new` call site can apply JS override semantics.
            if (!TryLowerExpression(returnStmt.Expression, out var ctorReturnTemp))
            {
                return false;
            }

            ctorReturnTemp = EnsureObject(ctorReturnTemp);

            if (_isDerivedConstructor)
            {
                var overrideLabel = CreateLabel();
                var endReturnCheckLabel = CreateLabel();
                _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(ctorReturnTemp, overrideLabel));
                _methodBodyIR.Instructions.Add(new LIRBranch(endReturnCheckLabel));
                _methodBodyIR.Instructions.Add(new LIRLabel(overrideLabel));

                var isObjectReturnTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                    nameof(JavaScriptRuntime.Function),
                    nameof(JavaScriptRuntime.Function.IsConstructorReturnOverride),
                    new[] { ctorReturnTemp },
                    isObjectReturnTemp));
                DefineTempStorage(isObjectReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isObjectReturnTemp, endReturnCheckLabel));
                _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError("Derived constructors may only return object or undefined"));
                _methodBodyIR.Instructions.Add(new LIRLabel(endReturnCheckLabel));
            }

            if (!TryGetEnclosingClassRegistryName(out var registryClassName))
            {
                return false;
            }

            lirInstructions.Add(new LIRStoreUserClassInstanceField(
                RegistryClassName: registryClassName!,
                FieldName: "__jroc_ctorReturn",
                IsPrivateField: true,
                Value: ctorReturnTemp));

            // Control-flow return value is irrelevant for constructors.
            returnTempVar = CreateTempVariable();
            lirInstructions.Add(new LIRConstUndefined(returnTempVar));
            DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else if (returnStmt.Expression != null)
        {
            if (CanEmitTailPositionReturn()
                && TryLowerTailPositionReturnExpression(returnStmt.Expression))
            {
                return true;
            }

            // Lower the return expression
            // Special-case: if we inferred `return this` for a class method, keep it typed as the
            // user-defined class (metadata TypeDef handle) so the return matches the class-typed ABI.
            if (_scope?.StableReturnIsThis == true
                && returnStmt.Expression is HIRThisExpression
                && _classRegistry != null
                && TryGetEnclosingClassRegistryName(out var registryClassName)
                && registryClassName != null
                && _classRegistry.TryGet(registryClassName, out var thisTypeHandle)
                && !thisTypeHandle.IsNil)
            {
                returnTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadThis(returnTempVar));
                DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object), thisTypeHandle));
            }
            else
            {
                if (!TryLowerExpression(returnStmt.Expression, out returnTempVar))
                {
                    return false;
                }
            }

            // Default ABI returns object. Preserve typed returns only when the callable ABI actually
            // supports them:
            // - class methods/static methods may return stable primitive values directly
            // - function/arrow callables keep the historical string fast-path only
            Type? stableReturnClrType = null;
            if (_methodBodyIR.CallableId?.Semantics.MayReturnTailCall != true
                && _scope is { Kind: ScopeKind.Function } functionScope)
            {
                if (_callableKind is CallableKind.ClassMethod or CallableKind.ClassStaticMethod)
                {
                    stableReturnClrType = functionScope.StableReturnClrType;
                }
                else if (functionScope.StableReturnClrType == typeof(string)
                    || (functionScope.StableReturnClrType == typeof(double) && _preserveNonClassDoubleReturn))
                {
                    stableReturnClrType = functionScope.StableReturnClrType;
                }
            }
            if (stableReturnClrType == typeof(double))
            {
                returnTempVar = EnsureNumber(returnTempVar);
            }
            else if (stableReturnClrType == typeof(bool))
            {
                returnTempVar = EnsureBoolean(returnTempVar);
            }
            else if (stableReturnClrType != typeof(string) && _scope?.StableReturnIsThis != true)
            {
                returnTempVar = EnsureObject(returnTempVar);
            }
        }
        else
        {
            // Bare return - return undefined (null)
            returnTempVar = CreateTempVariable();
            lirInstructions.Add(new LIRConstUndefined(returnTempVar));
            DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        return TryEmitReturnCompletion(returnTempVar);
    }

    private bool TryEmitReturnCompletion(TempVariable returnTempVar)
    {
        // Async try/finally lowering: a return inside a protected region must flow through finally.
        if (_isAsync
            && _methodBodyIR.AsyncInfo?.HasAwaits == true
            && !_methodBodyIR.LeafScopeId.IsNil
            && _asyncTryFinallyStack.Count > 0)
        {
            var ctx = _asyncTryFinallyStack.Peek();
            var scopeName = _methodBodyIR.LeafScopeId.Name;

            returnTempVar = EnsureObject(returnTempVar);

            // pendingReturnValue = returnTempVar
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, returnTempVar));

            // hasPendingReturn = true
            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, hasReturnTemp));

            // hasPendingException = false; pendingException = null (return overrides)
            var clearHasExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, clearHasExTemp));
            DefineTempStorage(clearHasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, clearHasExTemp));

            var clearExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(clearExTemp));
            DefineTempStorage(clearExTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, clearExTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));
            return true;
        }

        // Generator try/finally lowering (when yields are present): route return through finally.
        if (_isGenerator && !_methodBodyIR.LeafScopeId.IsNil && _generatorTryCatchFinallyStack.Count > 0)
        {
            var ctx = _generatorTryCatchFinallyStack.Peek();
            var scopeName = _methodBodyIR.LeafScopeId.Name;

            returnTempVar = EnsureObject(returnTempVar);

            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, returnTempVar));

            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, trueTemp));

            // return overrides exception
            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, falseTemp));

            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, nullTemp));

            if (ctx.FinallyEntryLabelId != -1)
            {
                _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));
                return true;
            }

            // No finally in this explicit context. If there is an outer explicit context, route there;
            // otherwise return immediately.
            if (TryGetOuterGeneratorTryCatchFinallyContext(out var outer))
            {
                if (outer.FinallyEntryLabelId != -1)
                {
                    _methodBodyIR.Instructions.Add(new LIRBranch(outer.IsInFinally ? outer.FinallyExitLabelId : outer.FinallyEntryLabelId));
                    return true;
                }
            }

            _methodBodyIR.Instructions.Add(new LIRReturn(returnTempVar));
            return true;
        }

        if (TryEmitReturnThroughSyncFinally(returnTempVar))
        {
            return true;
        }

        // If we are inside a protected region with a finally handler, we must use leave
        // so finally runs before returning.
        if (_protectedControlFlowDepthStack.Count > 0 && _methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            if (!TryEmitReturnViaEpilogue(returnTempVar))
            {
                return false;
            }
            return true;
        }

        _methodBodyIR.Instructions.Add(new LIRReturn(returnTempVar));
        return true;
    }

    private bool CanEmitTailPositionReturn()
        => _callableKind != CallableKind.Constructor
            && !_isAsync
            && !_isGenerator
            && _tailPositionSuppressionDepth == 0
            && (_scope == null
                || ArgumentsObjectSemantics.IsStrictScope(_scope));

    private bool TryLowerTailPositionReturnExpression(HIRExpression expression)
    {
        switch (expression)
        {
            case HIRConditionalExpression conditionalExpr:
                return TryLowerTailPositionConditionalReturn(conditionalExpr);

            case HIRBinaryExpression { Operator: Acornima.Operator.LogicalAnd } logicalAnd:
                return TryLowerTailPositionLogicalAndReturn(logicalAnd);

            case HIRBinaryExpression { Operator: Acornima.Operator.LogicalOr } logicalOr:
                return TryLowerTailPositionLogicalOrReturn(logicalOr);

            case HIRBinaryExpression { Operator: Acornima.Operator.NullishCoalescing } coalesce:
                return TryLowerTailPositionNullishReturn(coalesce);

            case HIRSequenceExpression sequence:
                return TryLowerTailPositionSequenceReturn(sequence);

            case HIRCallExpression callExpr:
                return TryLowerTailCallFunctionReturn(callExpr);

            case HIRTaggedTemplateExpression taggedTemplate:
                return TryLowerTailTaggedTemplateReturn(taggedTemplate);

            default:
                return false;
        }
    }

    private bool TryLowerTailPositionConditionalReturn(HIRConditionalExpression conditionalExpr)
    {
        if (!TryLowerExpression(conditionalExpr.Test, out var conditionTemp))
        {
            return false;
        }

        var boolConditionTemp = EnsureBooleanCondition(conditionTemp);
        int elseLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boolConditionTemp, elseLabel));

        if (!TryLowerReturnExpressionOrTailReturn(conditionalExpr.Consequent))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(elseLabel));
        ClearNumericRefinementsAtLabel();

        return TryLowerReturnExpressionOrTailReturn(conditionalExpr.Alternate);
    }

    private bool TryLowerTailPositionLogicalAndReturn(HIRBinaryExpression logicalAnd)
    {
        if (!TryLowerExpression(logicalAnd.Left, out var leftTemp))
        {
            return false;
        }

        var leftBoxed = EnsureObject(leftTemp);
        var isTruthyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        int falsyLabel = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(isTruthyTemp, falsyLabel));

        if (!TryLowerReturnExpressionOrTailReturn(logicalAnd.Right))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(falsyLabel));
        ClearNumericRefinementsAtLabel();

        return EmitReturnTemp(leftBoxed);
    }

    private bool TryLowerTailPositionLogicalOrReturn(HIRBinaryExpression logicalOr)
    {
        if (!TryLowerExpression(logicalOr.Left, out var leftTemp))
        {
            return false;
        }

        var leftBoxed = EnsureObject(leftTemp);
        var isTruthyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        int truthyLabel = CreateLabel();
        _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isTruthyTemp, truthyLabel));

        if (!TryLowerReturnExpressionOrTailReturn(logicalOr.Right))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(truthyLabel));
        ClearNumericRefinementsAtLabel();

        return EmitReturnTemp(leftBoxed);
    }

    private bool TryLowerTailPositionNullishReturn(HIRBinaryExpression coalesce)
    {
        if (!TryLowerExpression(coalesce.Left, out var leftTemp))
        {
            return false;
        }

        var leftBoxed = EnsureObject(leftTemp);
        int evalRightLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(
            new LIRBranchIfFalse(leftBoxed, evalRightLabel));

        var isJsNullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(
            typeof(JavaScriptRuntime.JsNull),
            leftBoxed,
            isJsNullTemp));
        DefineTempStorage(
            isJsNullTemp,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(
            new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

        if (!EmitReturnTemp(leftBoxed))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(evalRightLabel));
        ClearNumericRefinementsAtLabel();
        return TryLowerReturnExpressionOrTailReturn(coalesce.Right);
    }

    private bool TryLowerTailPositionSequenceReturn(HIRSequenceExpression sequence)
    {
        if (sequence.Expressions.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < sequence.Expressions.Count - 1; i++)
        {
            if (!TryLowerExpressionDiscardResult(sequence.Expressions[i]))
            {
                return false;
            }
        }

        return TryLowerReturnExpressionOrTailReturn(
            sequence.Expressions[sequence.Expressions.Count - 1]);
    }

    private bool TryLowerReturnExpressionOrTailReturn(HIRExpression expression)
    {
        if (TryLowerTailPositionReturnExpression(expression))
        {
            return true;
        }

        if (!TryLowerExpression(expression, out var valueTemp))
        {
            return false;
        }

        return EmitReturnTemp(valueTemp);
    }

    private bool TryLowerTailCallFunctionReturn(HIRCallExpression callExpr)
    {
        if (TryPrepareSuperTailCallTarget(
                callExpr.Callee,
                out var superTarget,
                out var superThisArgument))
        {
            if (!TryLowerCallArgumentsToArgsArray(
                    callExpr.Arguments,
                    out var superArgumentsArray))
            {
                return false;
            }

            return EmitTailCallRequest(
                superTarget,
                superThisArgument,
                superArgumentsArray);
        }

        if (callExpr.StableDirectCallableTarget is { } stableTarget
            && callExpr.Callee is HIRVariableExpression stableCallee
            && !HasSpreadArguments(callExpr.Arguments)
            && !stableTarget.CallableId.NeedsArgumentsObject
            && !stableTarget.CallableId.HasRestParameters
            && _callableRegistry?.GetSignature(stableTarget.CallableId)
                ?.ReturnClrType is null)
        {
            var arguments = new List<TempVariable>(
                callExpr.Arguments.Length);
            foreach (var argumentExpression in callExpr.Arguments)
            {
                if (!TryLowerExpression(
                        argumentExpression,
                        out var argument))
                {
                    return false;
                }

                arguments.Add(EnsureObject(argument));
            }

            var scopes = CreateTempVariable();
            if (!TryBuildScopesArrayForClosureBinding(
                    stableTarget.CallableScope,
                    scopes))
            {
                return false;
            }
            DefineTempStorage(
                scopes,
                new ValueStorage(
                    ValueStorageKind.Reference,
                    typeof(object[])));
            _methodBodyIR.Instructions.Add(
                new LIRTailCallFunctionReturn(
                    stableCallee.Name,
                    scopes,
                    arguments,
                    stableTarget.CallableId));
            return true;
        }

        if (callExpr.Callee is HIRPropertyAccessExpression
            {
                Object: HIRVariableExpression globalVariable
            } stableGlobalMember
            && !HasSpreadArguments(callExpr.Arguments)
            && _activeWithObjects.Count == 0
            && globalVariable.Name.Kind == BindingKind.Global
            && !globalVariable.Name.BindingInfo.HasWrite
            && GlobalMemberIntrinsicRegistry.TryGet(
                globalVariable.Name.Name,
                stableGlobalMember.PropertyName,
                out _))
        {
            var result = CreateTempVariable();
            return TryLowerStableGlobalMemberCall(
                    callExpr,
                    stableGlobalMember,
                    hasSpreadArgs: false,
                    result)
                && EmitReturnTemp(result);
        }

        if (callExpr.Callee is HIRPropertyAccessExpression
            {
                Object: HIRVariableExpression specializedReceiver
            }
            && (TryGetNodeModuleContractType(
                    specializedReceiver.Name.BindingInfo.ClrType) != null
                || !specializedReceiver.Name.BindingInfo.HasWrite
                && specializedReceiver.Name.Kind == BindingKind.Global
                && _runtimeIntrinsicCatalog.TryGetIntrinsicObject(
                    specializedReceiver.Name.Name,
                    out _)))
        {
            if (!TryLowerExpression(callExpr, out var result))
            {
                return false;
            }

            return EmitReturnTemp(result);
        }

        TempVariable target;
        TempVariable thisArgument;
        var isMemberCall = false;

        if (callExpr.Callee is HIRPropertyAccessExpression propertyAccess)
        {
            if (!TryLowerExpression(propertyAccess.Object, out var receiver))
            {
                return false;
            }

            thisArgument = EnsureObject(receiver);
            var propertyKey = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRConstString(propertyAccess.PropertyName, propertyKey));
            DefineTempStorage(
                propertyKey,
                new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            target = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.PrepareTailCallMember),
                [thisArgument, propertyKey],
                target,
                [typeof(object), typeof(object)]));
            DefineTempStorage(
                target,
                new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            isMemberCall = true;
        }
        else if (callExpr.Callee is HIRIndexAccessExpression indexAccess)
        {
            if (!TryLowerExpression(indexAccess.Object, out var receiver)
                || !TryLowerExpression(indexAccess.Index, out var propertyKey))
            {
                return false;
            }

            thisArgument = EnsureObject(receiver);
            target = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.PrepareTailCallMember),
                [thisArgument, EnsureObject(propertyKey)],
                target,
                [typeof(object), typeof(object)]));
            DefineTempStorage(
                target,
                new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            isMemberCall = true;
        }
        else
        {
            if (!TryLowerExpression(callExpr.Callee, out target))
            {
                return false;
            }

            target = EnsureObject(target);
            thisArgument = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRConstUndefined(thisArgument));
            DefineTempStorage(
                thisArgument,
                new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        if (!TryLowerCallArgumentsToArgsArray(
                callExpr.Arguments,
                out var argumentsArray))
        {
            return false;
        }

        return isMemberCall
            ? EmitTailCallMemberRequest(target, argumentsArray)
            : EmitTailCallRequest(target, thisArgument, argumentsArray);
    }

    private bool TryLowerTailTaggedTemplateReturn(
        HIRTaggedTemplateExpression taggedTemplate)
    {
        if (!TryPrepareTailTaggedTemplateTarget(
                taggedTemplate.Tag,
                out var target,
                out var thisArgument,
                out var isMemberReference))
        {
            return false;
        }

        if (!TryPrepareTaggedTemplateInvocation(
                taggedTemplate,
                out _,
                out var argumentsArray,
                evaluateTagExpression: false))
        {
            return false;
        }

        return isMemberReference
            ? EmitTailCallMemberRequest(target, argumentsArray)
            : EmitTailCallRequest(
                EnsureObject(target),
                thisArgument,
                argumentsArray);
    }

    private bool TryPrepareTailTaggedTemplateTarget(
        HIRExpression tag,
        out TempVariable target,
        out TempVariable thisArgument,
        out bool isMemberReference)
    {
        target = default;
        thisArgument = default;
        isMemberReference = false;

        if (TryPrepareSuperTailCallTarget(
                tag,
                out target,
                out thisArgument))
        {
            return true;
        }

        if (tag is HIRPropertyAccessExpression propertyAccess)
        {
            if (!TryLowerExpression(propertyAccess.Object, out var receiver))
            {
                return false;
            }

            var propertyKey = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRConstString(propertyAccess.PropertyName, propertyKey));
            DefineTempStorage(
                propertyKey,
                new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            target = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.PrepareTailCallMember),
                [EnsureObject(receiver), propertyKey],
                target,
                [typeof(object), typeof(object)]));
            DefineTempStorage(
                target,
                new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            isMemberReference = true;
            return true;
        }

        if (tag is HIRIndexAccessExpression indexAccess)
        {
            if (!TryLowerExpression(indexAccess.Object, out var receiver)
                || !TryLowerExpression(indexAccess.Index, out var propertyKey))
            {
                return false;
            }

            target = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.PrepareTailCallMember),
                [EnsureObject(receiver), EnsureObject(propertyKey)],
                target,
                [typeof(object), typeof(object)]));
            DefineTempStorage(
                target,
                new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            isMemberReference = true;
            return true;
        }

        if (!TryLowerExpression(tag, out target))
        {
            return false;
        }

        thisArgument = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(thisArgument));
        DefineTempStorage(
            thisArgument,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryPrepareSuperTailCallTarget(
        HIRExpression callee,
        out TempVariable target,
        out TempVariable thisArgument)
    {
        target = default;
        thisArgument = default;

        TempVariable propertyName;
        if (callee is HIRPropertyAccessExpression
            {
                Object: HIRSuperExpression
            } propertyAccess)
        {
            propertyName = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRConstString(propertyAccess.PropertyName, propertyName));
            DefineTempStorage(
                propertyName,
                new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        }
        else if (callee is HIRIndexAccessExpression
            {
                Object: HIRSuperExpression
            } indexAccess)
        {
            if (!TryLowerExpression(indexAccess.Index, out var propertyKey))
            {
                return false;
            }

            propertyName = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.ToPropertyKeyString),
                [EnsureObject(propertyKey)],
                propertyName));
            DefineTempStorage(
                propertyName,
                new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        }
        else
        {
            return false;
        }

        target = CreateTempVariable();
        if (_classRegistry != null
            && TryGetEnclosingBaseClassRegistryName(out var baseClassRegistryName)
            && baseClassRegistryName != null)
        {
            var baseConstructor = CreateTempVariable();
            _methodBodyIR.Instructions.Add(
                new LIRGetUserClassType(baseClassRegistryName, baseConstructor));
            DefineTempStorage(
                baseConstructor,
                new ValueStorage(ValueStorageKind.Reference, typeof(Type)));

            var superObject = baseConstructor;
            if (!IsLexicallyEnclosedByStaticClassMethod())
            {
                var prototypeKey = CreateTempVariable();
                _methodBodyIR.Instructions.Add(
                    new LIRConstString("prototype", prototypeKey));
                DefineTempStorage(
                    prototypeKey,
                    new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                superObject = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRGetItem(
                    EnsureObject(baseConstructor),
                    prototypeKey,
                    superObject));
                DefineTempStorage(
                    superObject,
                    new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }

            _methodBodyIR.Instructions.Add(new LIRGetItem(
                EnsureObject(superObject),
                propertyName,
                target));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.GetSuperProperty),
                [propertyName],
                target));
        }
        DefineTempStorage(
            target,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return TryLowerExpression(
            new HIRThisExpression(),
            out thisArgument);
    }

    private bool IsLexicallyEnclosedByStaticClassMethod()
    {
        for (var scope = _scope; scope != null; scope = scope.Parent)
        {
            if (scope.Callable?.Kind == TwoPhase.CallableKind.ClassStaticMethod)
            {
                return true;
            }

            if (scope.Callable?.Kind == TwoPhase.CallableKind.ClassMethod)
            {
                return false;
            }
        }

        return _callableKind == CallableKind.ClassStaticMethod;
    }

    private bool EmitTailCallRequest(
        TempVariable target,
        TempVariable thisArgument,
        TempVariable argumentsArray)
    {
        var request = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.CreateTailCall),
            [target, thisArgument, argumentsArray],
            request,
            [typeof(object), typeof(object), typeof(object[])]));
        DefineTempStorage(
            request,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return TryEmitReturnCompletion(request);
    }

    private bool EmitTailCallMemberRequest(
        TempVariable memberReference,
        TempVariable argumentsArray)
    {
        var request = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.CreateTailCallMember),
            [memberReference, argumentsArray],
            request,
            [typeof(object), typeof(object[])]));
        DefineTempStorage(
            request,
            new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return TryEmitReturnCompletion(request);
    }

    private TempVariable EnsureBooleanCondition(TempVariable conditionTemp)
    {
        var conditionStorage = GetTempStorage(conditionTemp);
        if (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(bool))
        {
            return conditionTemp;
        }

        var conditionBoxed = EnsureObject(conditionTemp);
        var isTruthyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(conditionBoxed, isTruthyTemp));
        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return isTruthyTemp;
    }

    private bool EmitReturnTemp(TempVariable returnTempVar)
    {
        returnTempVar = ApplyReturnTypeCoercion(returnTempVar);
        return TryEmitReturnCompletion(returnTempVar);
    }

    private TempVariable ApplyReturnTypeCoercion(TempVariable returnTempVar)
    {
        Type? stableReturnClrType = null;
        if (_methodBodyIR.CallableId?.Semantics.MayReturnTailCall != true
            && _scope is { Kind: ScopeKind.Function } functionScope)
        {
            if (_callableKind is CallableKind.ClassMethod or CallableKind.ClassStaticMethod)
            {
                stableReturnClrType = functionScope.StableReturnClrType;
            }
            else if (functionScope.StableReturnClrType == typeof(string)
                || (functionScope.StableReturnClrType == typeof(double) && _preserveNonClassDoubleReturn))
            {
                stableReturnClrType = functionScope.StableReturnClrType;
            }
        }

        if (stableReturnClrType == typeof(double))
        {
            return EnsureNumber(returnTempVar);
        }

        if (stableReturnClrType == typeof(bool))
        {
            return EnsureBoolean(returnTempVar);
        }

        if (stableReturnClrType != typeof(string) && _scope?.StableReturnIsThis != true)
        {
            return EnsureObject(returnTempVar);
        }

        return returnTempVar;
    }
}
