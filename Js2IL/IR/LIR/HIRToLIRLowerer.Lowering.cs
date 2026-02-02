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
    private bool TryLowerStatement(HIRStatement statement)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        switch (statement)
        {
            case HIRSequencePointStatement sequencePoint:
                {
                    lirInstructions.Add(new LIRSequencePoint(sequencePoint.Span));
                    return true;
                }
            case HIRStoreUserClassInstanceFieldStatement storeInstanceField:
                {
                    if (!TryLowerExpression(storeInstanceField.Value, out var valueTemp))
                    {
                        return false;
                    }

                    // Only force object boxing when the declared field type is unknown/object.
                    // For stable typed fields (double/bool/string), keep the value in its preferred form.
                    var stableFieldType = TryGetStableThisFieldClrType(storeInstanceField.FieldName);
                    if (stableFieldType == null || stableFieldType == typeof(object))
                    {
                        valueTemp = EnsureObject(valueTemp);
                    }
                    lirInstructions.Add(new LIRStoreUserClassInstanceField(
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        valueTemp));
                    return true;
                }

            case HIRStoreUserClassStaticFieldStatement storeStaticField:
                {
                    if (!TryLowerExpression(storeStaticField.Value, out var valueTemp))
                    {
                        return false;
                    }
                    valueTemp = EnsureObject(valueTemp);
                    lirInstructions.Add(new LIRStoreUserClassStaticField(
                        storeStaticField.RegistryClassName,
                        storeStaticField.FieldName,
                        valueTemp));
                    return true;
                }

            case HIRVariableDeclaration exprStmt:
                return TryLowerVariableDeclaration(exprStmt);
            case HIRDestructuringVariableDeclaration destructDecl:
                return TryLowerDestructuringVariableDeclaration(destructDecl);
            case HIRExpressionStatement exprStmt:
                {
                    // Lower the expression and discard the result
                    if (!TryLowerExpressionDiscardResult(exprStmt.Expression))
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering expression statement {exprStmt.Expression.GetType().Name}");
                        return false;
                    }
                    return true;
                }
            case HIRReturnStatement returnStmt:
                return TryLowerReturnStatement(returnStmt);
            case HIRLabeledStatement labeledStmt:
                {
                    var endLabel = CreateLabel();
                    _controlFlowStack.Push(new ControlFlowContext(endLabel, null, labeledStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(labeledStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    lirInstructions.Add(new LIRLabel(endLabel));
                    return true;
                }
            case HIRSwitchStatement switchStmt:
                return TryLowerSwitchStatement(switchStmt);
            case HIRTryStatement tryStmt:
                return TryLowerTryStatement(tryStmt);
            case HIRThrowStatement throwStmt:
                return TryLowerThrowStatement(throwStmt);
            case HIRIfStatement ifStmt:
                return TryLowerIfStatement(ifStmt);
            case HIRForStatement forStmt:
                return TryLowerForStatement(forStmt);

            case Js2IL.HIR.HIRForOfStatement forOfStmt:
                return TryLowerForOfStatement(forOfStmt);

            case Js2IL.HIR.HIRForInStatement forInStmt:
                return TryLowerForInStatement(forInStmt);
            case HIRWhileStatement whileStmt:
                return TryLowerWhileStatement(whileStmt);
            case HIRDoWhileStatement doWhileStmt:
                return TryLowerDoWhileStatement(doWhileStmt);
            case HIRBreakStatement breakStmt:
                return TryLowerBreakStatement(breakStmt);
            case HIRContinueStatement continueStmt:
                return TryLowerContinueStatement(continueStmt);
            case HIRBlock block:
                // Lower each statement in the block - return false on first failure
                return block.Statements.All(TryLowerStatement);
            default:
                // Unsupported statement type
                return false;
        }
    }

    private bool TryLowerAsyncTryCatchWithAwait(HIRTryStatement tryStmt)
    {
        if (_methodBodyIR.AsyncInfo == null || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        var asyncInfo = _methodBodyIR.AsyncInfo;
        var scopeName = _methodBodyIR.LeafScopeId.Name;
        const string pendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._pendingException);

        var catchStateId = asyncInfo.AllocateResumeStateId();
        var catchLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(catchStateId, catchLabel);

        var endLabel = CreateLabel();

        // Clear pending exception before entering try
        var clearTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNull(clearTemp));
        DefineTempStorage(clearTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, clearTemp));

        _asyncTryCatchStack.Push(new AsyncTryCatchContext(catchStateId, catchLabel, pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
        }

        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Catch label (used for both rejected awaits and explicit throws in try)
        _methodBodyIR.Instructions.Add(new LIRLabel(catchLabel));

        var pendingTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, pendingTemp));
        DefineTempStorage(pendingTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        // Clear pending exception after loading
        var clearAfterTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNull(clearAfterTemp));
        DefineTempStorage(clearAfterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, clearAfterTemp));

        if (tryStmt.CatchParamBinding != null &&
            !TryStoreToBinding(tryStmt.CatchParamBinding, pendingTemp, out _))
        {
            return false;
        }

        if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        return true;
    }

    private bool TryLowerAsyncTryWithFinallyWithAwait(HIRTryStatement tryStmt)
    {
        if (_methodBodyIR.AsyncInfo == null || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        if (tryStmt.FinallyBody == null)
        {
            return false;
        }

        var asyncInfo = _methodBodyIR.AsyncInfo;
        var scopeName = _methodBodyIR.LeafScopeId.Name;

        const string pendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._pendingException);
        const string hasPendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._hasPendingException);
        const string pendingReturnField = nameof(JavaScriptRuntime.AsyncScope._pendingReturnValue);
        const string hasPendingReturnField = nameof(JavaScriptRuntime.AsyncScope._hasPendingReturn);

        bool hasCatch = tryStmt.CatchBody != null;

        // Synthetic labels used by the async state machine.
        var afterTryLabel = CreateLabel();
        var finallyEntryLabel = CreateLabel();
        var finallyExitLabel = CreateLabel();

        // Rejection/exception routing labels (used as resume targets for await rejection).
        var exceptionToFinallyStateId = asyncInfo.AllocateResumeStateId();
        var exceptionToFinallyLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(exceptionToFinallyStateId, exceptionToFinallyLabel);

        var exceptionInFinallyStateId = asyncInfo.AllocateResumeStateId();
        var exceptionInFinallyLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(exceptionInFinallyStateId, exceptionInFinallyLabel);

        int catchStateId = 0;
        int catchLabel = 0;
        if (hasCatch)
        {
            catchStateId = asyncInfo.AllocateResumeStateId();
            catchLabel = CreateLabel();
            asyncInfo.RegisterResumeLabel(catchStateId, catchLabel);
        }

        // Reset pending completion fields on entry.
        {
            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingReturnField, nullTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, falseTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));
        }

        // --- Try block ---
        _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: pendingExceptionField,
            HasPendingExceptionFieldName: hasPendingExceptionField,
            PendingReturnFieldName: pendingReturnField,
            HasPendingReturnFieldName: hasPendingReturnField,
            IsInFinally: false));
        _asyncTryCatchStack.Push(new AsyncTryCatchContext(
            CatchStateId: hasCatch ? catchStateId : exceptionToFinallyStateId,
            CatchLabelId: hasCatch ? catchLabel : exceptionToFinallyLabel,
            PendingExceptionFieldName: pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
            _asyncTryFinallyStack.Pop();
        }

        // Normal completion flows into finally.
        _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));

        // --- Catch handler (synthetic) ---
        if (hasCatch)
        {
            _methodBodyIR.Instructions.Add(new LIRLabel(catchLabel));

            // Mark that we arrived due to an exception/rejection.
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            // Load pending exception into temp and clear it (the catch is handling it).
            var pendingTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, pendingTemp));
            DefineTempStorage(pendingTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, falseTemp));

            if (tryStmt.CatchParamBinding != null &&
                !TryStoreToBinding(tryStmt.CatchParamBinding, pendingTemp, out _))
            {
                return false;
            }

            _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
                FinallyEntryLabelId: finallyEntryLabel,
                FinallyExitLabelId: finallyExitLabel,
                PendingExceptionFieldName: pendingExceptionField,
                HasPendingExceptionFieldName: hasPendingExceptionField,
                PendingReturnFieldName: pendingReturnField,
                HasPendingReturnFieldName: hasPendingReturnField,
                IsInFinally: false));
            _asyncTryCatchStack.Push(new AsyncTryCatchContext(
                CatchStateId: exceptionToFinallyStateId,
                CatchLabelId: exceptionToFinallyLabel,
                PendingExceptionFieldName: pendingExceptionField));
            try
            {
                if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
                {
                    return false;
                }
            }
            finally
            {
                _asyncTryCatchStack.Pop();
                _asyncTryFinallyStack.Pop();
            }

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));
        }

        // --- Exception path into finally (synthetic; used for await rejection / throw) ---
        _methodBodyIR.Instructions.Add(new LIRLabel(exceptionToFinallyLabel));
        {
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));
        }

        // --- Finally block ---
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyEntryLabel));
        _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: pendingExceptionField,
            HasPendingExceptionFieldName: hasPendingExceptionField,
            PendingReturnFieldName: pendingReturnField,
            HasPendingReturnFieldName: hasPendingReturnField,
            IsInFinally: true));
        _asyncTryCatchStack.Push(new AsyncTryCatchContext(
            CatchStateId: exceptionInFinallyStateId,
            CatchLabelId: exceptionInFinallyLabel,
            PendingExceptionFieldName: pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.FinallyBody))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
            _asyncTryFinallyStack.Pop();
        }
        _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));

        // --- Exception inside finally overrides prior completion ---
        _methodBodyIR.Instructions.Add(new LIRLabel(exceptionInFinallyLabel));
        {
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));
        }

        // --- After finally: dispatch based on completion ---
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyExitLabel));

        var checkReturnLabel = CreateLabel();
        {
            var hasExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingExceptionField, hasExTemp));
            DefineTempStorage(hasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            SetTempVariableSlot(hasExTemp, CreateAnonymousVariableSlot("$finally_hasEx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasExTemp, checkReturnLabel));

            var exTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, exTemp));
            DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (_asyncTryCatchStack.Count > 0)
            {
                var outer = _asyncTryCatchStack.Peek();
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, outer.PendingExceptionFieldName, exTemp));
                _methodBodyIR.Instructions.Add(new LIRBranch(outer.CatchLabelId));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRAsyncReject(exTemp));
            }
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(checkReturnLabel));
        {
            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingReturnField, hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            SetTempVariableSlot(hasReturnTemp, CreateAnonymousVariableSlot("$finally_hasReturn", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, afterTryLabel));

            var retTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingReturnField, retTemp));
            DefineTempStorage(retTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRReturn(retTemp));
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(afterTryLabel));
        return true;
    }

}
