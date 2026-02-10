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
    private bool TryLowerTryStatement(HIRTryStatement tryStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        var hasCatch = tryStmt.CatchBody != null;
        var hasFinally = tryStmt.FinallyBody != null;
        var awaitCountTry = CountAwaitExpressionsInStatement(tryStmt.TryBlock);
        var awaitCountCatch = tryStmt.CatchBody != null ? CountAwaitExpressionsInStatement(tryStmt.CatchBody) : 0;
        var awaitCountFinally = tryStmt.FinallyBody != null ? CountAwaitExpressionsInStatement(tryStmt.FinallyBody) : 0;
        var awaitCount = awaitCountTry + awaitCountCatch + awaitCountFinally;

        var yieldCountTry = _isGenerator ? CountYieldExpressionsInStatement(tryStmt.TryBlock) : 0;
        var yieldCountCatch = _isGenerator && tryStmt.CatchBody != null ? CountYieldExpressionsInStatement(tryStmt.CatchBody) : 0;
        var yieldCountFinally = _isGenerator && tryStmt.FinallyBody != null ? CountYieldExpressionsInStatement(tryStmt.FinallyBody) : 0;
        var yieldCount = yieldCountTry + yieldCountCatch + yieldCountFinally;

        // Async try/finally (and try/catch/finally) cannot use IL exception regions when awaits occur
        // within the protected region, because awaits suspend MoveNext via 'ret'.
        if (_isAsync && hasFinally && awaitCount > 0)
        {
            return TryLowerAsyncTryWithFinallyWithAwait(tryStmt);
        }

        // Generator suspension via 'yield' cannot occur within CLR EH regions (try/finally), because
        // our yield lowering suspends via 'ret'. CLR requires protected regions to exit via 'leave'.
        // When yields appear within a try/catch/finally in a generator, lower it as an explicit
        // state-machine routing (similar to async-with-await try/finally lowering).
        if (_isGenerator && !_isAsync && (hasCatch || hasFinally) && yieldCount > 0)
        {
            return TryLowerGeneratorTryWithYield(tryStmt);
        }
        if (_isAsync && hasCatch && !hasFinally && awaitCount > 0)
        {
            return TryLowerAsyncTryCatchWithAwait(tryStmt);
        }
        if (!hasCatch && !hasFinally)
        {
            return TryLowerStatement(tryStmt.TryBlock);
        }

        // Track current control-flow depth so we can decide when break/continue exits the try.
        _protectedControlFlowDepthStack.Push(_controlFlowStack.Count);

        // Any return inside a protected region must use 'leave' to an epilogue outside the region.
        if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            _methodBodyIR.ReturnEpilogueLabelId = CreateLabel();
        }

        try
        {
            var outerTryStart = CreateLabel();
            var outerTryEnd = CreateLabel();
            var endLabel = CreateLabel();
            int innerTryStart = outerTryStart;
            int innerTryEnd = outerTryEnd;

            int catchStart = 0;
            int catchEnd = 0;
            if (hasCatch)
            {
                innerTryStart = CreateLabel();
                innerTryEnd = CreateLabel();
                catchStart = CreateLabel();
                catchEnd = CreateLabel();
            }

            int finallyStart = 0;
            int finallyEnd = 0;
            if (hasFinally)
            {
                finallyStart = CreateLabel();
                finallyEnd = CreateLabel();
            }

            // Outer try label (used for finally when present)
            lirInstructions.Add(new LIRLabel(outerTryStart));

            // Inner try/catch (if catch present) or direct try.
            if (hasCatch)
            {
                lirInstructions.Add(new LIRLabel(innerTryStart));
            }

            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }

            lirInstructions.Add(new LIRLeave(endLabel));

            if (hasCatch)
            {
                lirInstructions.Add(new LIRLabel(innerTryEnd));
                lirInstructions.Add(new LIRLabel(catchStart));

                // Catch handler starts with the exception object on the stack.
                var exTemp = CreateTempVariable();
                lirInstructions.Add(new LIRStoreException(exTemp));
                DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(System.Exception)));
                SetTempVariableSlot(exTemp, CreateAnonymousVariableSlot("$catch_ex", new ValueStorage(ValueStorageKind.Reference, typeof(System.Exception))));

                if (tryStmt.CatchParamBinding != null)
                {
                    var jsCatchValue = CreateTempVariable();
                    lirInstructions.Add(new LIRUnwrapCatchException(exTemp, jsCatchValue));
                    DefineTempStorage(jsCatchValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    SetTempVariableSlot(jsCatchValue, CreateAnonymousVariableSlot("$catch_value", new ValueStorage(ValueStorageKind.Reference, typeof(object))));

                    if (!TryStoreToBinding(tryStmt.CatchParamBinding, jsCatchValue, out _))
                    {
                        return false;
                    }
                }

                if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
                {
                    return false;
                }

                lirInstructions.Add(new LIRLeave(endLabel));
                lirInstructions.Add(new LIRLabel(catchEnd));
            }

            lirInstructions.Add(new LIRLabel(outerTryEnd));

            if (hasFinally)
            {
                lirInstructions.Add(new LIRLabel(finallyStart));
                if (tryStmt.FinallyBody != null && !TryLowerStatement(tryStmt.FinallyBody))
                {
                    return false;
                }
                lirInstructions.Add(new LIREndFinally());
                lirInstructions.Add(new LIRLabel(finallyEnd));
            }

            lirInstructions.Add(new LIRLabel(endLabel));

            // Register EH regions.
            if (hasCatch)
            {
                _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
                    ExceptionRegionKind.Catch,
                    TryStartLabelId: innerTryStart,
                    TryEndLabelId: innerTryEnd,
                    HandlerStartLabelId: catchStart,
                    HandlerEndLabelId: catchEnd,
                    CatchType: typeof(System.Exception)));
            }

            if (hasFinally)
            {
                _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
                    ExceptionRegionKind.Finally,
                    TryStartLabelId: outerTryStart,
                    TryEndLabelId: outerTryEnd,
                    HandlerStartLabelId: finallyStart,
                    HandlerEndLabelId: finallyEnd));
            }

            return true;
        }
        finally
        {
            _protectedControlFlowDepthStack.Pop();
        }
    }

    private bool TryLowerGeneratorTryWithYield(HIRTryStatement tryStmt)
    {
        if (!_isGenerator || _isAsync || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        var hasCatch = tryStmt.CatchBody != null;
        var hasFinally = tryStmt.FinallyBody != null;
        if (!hasCatch && !hasFinally)
        {
            return false;
        }

        var scopeName = _methodBodyIR.LeafScopeId.Name;

        const string pendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._genPendingException);
        const string hasPendingExceptionField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingException);
        const string pendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._genPendingReturnValue);
        const string hasPendingReturnField = nameof(JavaScriptRuntime.GeneratorScope._hasGenPendingReturn);

        var afterTryLabel = CreateLabel();

        var catchEntryLabel = hasCatch ? CreateLabel() : -1;
        var finallyEntryLabel = hasFinally ? CreateLabel() : -1;
        var finallyExitLabel = hasFinally ? CreateLabel() : -1;

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

        var ctx = new GeneratorTryCatchFinallyContext(
            HasCatch: hasCatch,
            CatchEntryLabelId: catchEntryLabel,
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: pendingExceptionField,
            HasPendingExceptionFieldName: hasPendingExceptionField,
            PendingReturnFieldName: pendingReturnField,
            HasPendingReturnFieldName: hasPendingReturnField,
            IsInFinally: false,
            IsInCatch: false);

        _generatorTryCatchFinallyStack.Push(ctx);
        try
        {
            // --- Try block ---
            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }

            // Normal completion flows into finally (if present) or directly after try.
            _methodBodyIR.Instructions.Add(new LIRBranch(hasFinally ? finallyEntryLabel : afterTryLabel));

            // --- Catch block ---
            if (hasCatch)
            {
                _methodBodyIR.Instructions.Add(new LIRLabel(catchEntryLabel));

                _generatorTryCatchFinallyStack.Pop();
                _generatorTryCatchFinallyStack.Push(ctx with { IsInCatch = true });

                // Load the pending exception as the JS catch value.
                var catchValueTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, catchValueTemp));
                DefineTempStorage(catchValueTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                // Clear pending exception before executing catch (it is now handled unless re-thrown).
                {
                    var falseTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
                    DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, falseTemp));

                    var nullTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
                    DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));
                }

                if (tryStmt.CatchParamBinding != null
                    && !TryStoreToBinding(tryStmt.CatchParamBinding, catchValueTemp, out _))
                {
                    return false;
                }

                if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRBranch(hasFinally ? finallyEntryLabel : afterTryLabel));

                // Restore the context state to 'try' for subsequent lowering.
                _generatorTryCatchFinallyStack.Pop();
                _generatorTryCatchFinallyStack.Push(ctx);
            }

            // --- Finally block ---
            if (hasFinally)
            {
                _methodBodyIR.Instructions.Add(new LIRLabel(finallyEntryLabel));

                _generatorTryCatchFinallyStack.Pop();
                _generatorTryCatchFinallyStack.Push(ctx with { IsInFinally = true });

                if (tryStmt.FinallyBody != null && !TryLowerStatement(tryStmt.FinallyBody))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));

                // --- After finally: dispatch based on completion ---
                _methodBodyIR.Instructions.Add(new LIRLabel(finallyExitLabel));
            }

            // If we are nested within another generator try/catch/finally routing context, propagate
            // the pending completion outward by jumping to the appropriate outer handler.
            GeneratorTryCatchFinallyContext? outerCtx = null;
            if (TryGetOuterGeneratorTryCatchFinallyContext(out var outer))
            {
                outerCtx = outer;
            }

            int? outerHandlerTarget = null;
            if (outerCtx != null)
            {
                if (outerCtx.HasCatch && !outerCtx.IsInCatch && !outerCtx.IsInFinally)
                {
                    outerHandlerTarget = outerCtx.CatchEntryLabelId;
                }
                else if (outerCtx.FinallyEntryLabelId != -1)
                {
                    outerHandlerTarget = outerCtx.IsInFinally ? outerCtx.FinallyExitLabelId : outerCtx.FinallyEntryLabelId;
                }
            }

            var checkReturnLabel = CreateLabel();
            {
                var hasExTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingExceptionField, hasExTemp));
                DefineTempStorage(hasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasExTemp, checkReturnLabel));

                var exTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, exTemp));
                DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (outerHandlerTarget.HasValue)
                {
                    _methodBodyIR.Instructions.Add(new LIRBranch(outerHandlerTarget.Value));
                }
                else
                {
                    _methodBodyIR.Instructions.Add(new LIRThrow(exTemp));
                }
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(checkReturnLabel));
            {
                var hasReturnTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingReturnField, hasReturnTemp));
                DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, afterTryLabel));

                var retTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingReturnField, retTemp));
                DefineTempStorage(retTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (outerHandlerTarget.HasValue)
                {
                    _methodBodyIR.Instructions.Add(new LIRBranch(outerHandlerTarget.Value));
                }
                else
                {
                    _methodBodyIR.Instructions.Add(new LIRReturn(retTemp));
                }
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(afterTryLabel));
            return true;
        }
        finally
        {
            // Ensure the current context is popped even if lowering fails.
            if (_generatorTryCatchFinallyStack.Count > 0)
            {
                _generatorTryCatchFinallyStack.Pop();
            }
        }
    }
}
