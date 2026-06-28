using Acornima.Ast;
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
            if (_scope is { Kind: ScopeKind.Function } functionScope)
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

        lirInstructions.Add(new LIRReturn(returnTempVar));
        return true;
    }

    private bool CanEmitTailPositionReturn()
        => _callableKind != CallableKind.Constructor
            && !_isAsync
            && !_isGenerator
            && _protectedControlFlowDepthStack.Count == 0
            && !_methodBodyIR.ReturnEpilogueLabelId.HasValue;

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

            case HIRCallExpression callExpr:
                return TryLowerTailCallFunctionReturn(callExpr);

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
        if (callExpr.Callee is not HIRVariableExpression { Name: var symbol }
            || symbol.Kind != BindingKind.Function
            || HasSpreadArguments(callExpr.Arguments)
            || !FunctionHasSimpleParams(symbol))
        {
            return false;
        }

        var callableId = TryCreateCallableIdForFunctionDeclaration(symbol);
        if (callableId == null || callableId.NeedsArgumentsObject || callableId.HasRestParameters)
        {
            return false;
        }

        var arguments = new List<TempVariable>(callExpr.Arguments.Length);
        foreach (var arg in callExpr.Arguments)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }

            arguments.Add(EnsureObject(argTemp));
        }

        var scopesTempVar = CreateTempVariable();
        if (!TryBuildScopesArrayForCallee(symbol, scopesTempVar))
        {
            return false;
        }

        DefineTempStorage(scopesTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        _methodBodyIR.Instructions.Add(new LIRTailCallFunctionReturn(symbol, scopesTempVar, arguments, callableId));
        return true;
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
        _methodBodyIR.Instructions.Add(new LIRReturn(returnTempVar));
        return true;
    }

    private TempVariable ApplyReturnTypeCoercion(TempVariable returnTempVar)
    {
        Type? stableReturnClrType = null;
        if (_scope is { Kind: ScopeKind.Function } functionScope)
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
