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

        // Async try/finally (and try/catch/finally) cannot use IL exception regions when awaits occur
        // within the protected region, because awaits suspend MoveNext via 'ret'.
        if (_isAsync && hasFinally && awaitCount > 0)
        {
            return TryLowerAsyncTryWithFinallyWithAwait(tryStmt);
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
}
