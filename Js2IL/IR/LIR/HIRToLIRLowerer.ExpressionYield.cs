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
    private bool TryLowerYieldExpression(HIRYieldExpression yieldExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!_isGenerator || _methodBodyIR.GeneratorInfo == null)
        {
            IRPipelineMetrics.RecordFailure("yield expression found outside generator function context");
            return false;
        }

        if (yieldExpr.IsDelegate)
        {
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
            var generatorInfo = _methodBodyIR.GeneratorInfo;

            // Evaluate the yield* argument once.
            TempVariable yieldedStarArg;
            if (yieldExpr.Argument == null)
            {
                yieldedStarArg = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(yieldedStarArg));
                DefineTempStorage(yieldedStarArg, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            else
            {
                if (!TryLowerExpression(yieldExpr.Argument, out yieldedStarArg))
                {
                    return false;
                }
                yieldedStarArg = EnsureObject(yieldedStarArg);
            }

            // yield* expression result temp (final completion value from delegated iterator)
            resultTempVar = CreateTempVariable();
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Type check: if argument is a js2il GeneratorObject, delegate via next/throw/return forwarding.
            var isGenObjTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.GeneratorObject), yieldedStarArg, isGenObjTemp));
            DefineTempStorage(isGenObjTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            int iteratorSetupLabel = CreateLabel();
            int indexSetupLabel = CreateLabel();

            // If isGenObjTemp != null -> iterator path
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isGenObjTemp, iteratorSetupLabel));
            _methodBodyIR.Instructions.Add(new LIRLabel(indexSetupLabel));

            // ---------------------------
            // Indexable delegation path (arrays/strings/typed arrays via NormalizeForOfIterable)
            // ---------------------------
            var indexIterTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.NormalizeForOfIterable), new[] { yieldedStarArg }, indexIterTemp));
            DefineTempStorage(indexIterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Persist delegation state on GeneratorScope so it survives suspension.
            var oneTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, oneTemp));
            DefineTempStorage(oneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarMode), oneTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), indexIterTemp));

            var idxTempInit = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, idxTempInit));
            DefineTempStorage(idxTempInit, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarIndex), idxTempInit));

            var lenTempInit = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetLength(indexIterTemp, lenTempInit));
            DefineTempStorage(lenTempInit, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarLength), lenTempInit));

            int indexLoopStart = CreateLabel();
            int indexLoopEnd = CreateLabel();

            // Allocate a distinct resume point for the indexable yield.
            var indexResumeStateId = generatorInfo.AllocateResumeStateId();
            var indexResumeLabel = CreateLabel();
            var indexYieldResultTemp = CreateTempVariable();
            DefineTempStorage(indexYieldResultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            generatorInfo.YieldPoints.Add(new YieldPointInfo
            {
                ResumeStateId = indexResumeStateId,
                ResumeLabelId = indexResumeLabel,
                ResultTemp = indexYieldResultTemp
            });
            generatorInfo.RegisterResumeLabel(indexResumeStateId, indexResumeLabel);

            _methodBodyIR.Instructions.Add(new LIRLabel(indexLoopStart));

            var idxTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarIndex), idxTemp));
            DefineTempStorage(idxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var lenTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarLength), lenTemp));
            DefineTempStorage(lenTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var condTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(idxTemp, lenTemp, condTemp));
            DefineTempStorage(condTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(condTemp, indexLoopEnd));

            var iterObjTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), iterObjTemp));
            DefineTempStorage(iterObjTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var itemTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(iterObjTemp), idxTemp, itemTemp));
            DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // idx = idx + 1
            var oneIdxTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, oneIdxTemp));
            DefineTempStorage(oneIdxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            var nextIdxTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRAddNumber(idxTemp, oneIdxTemp, nextIdxTemp));
            DefineTempStorage(nextIdxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarIndex), nextIdxTemp));

            // Yield the current item.
            _methodBodyIR.Instructions.Add(new LIRYield(
                itemTemp,
                indexResumeStateId,
                indexResumeLabel,
                indexYieldResultTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(indexLoopStart));

            _methodBodyIR.Instructions.Add(new LIRLabel(indexLoopEnd));

            // Clear delegation state and set yield* expression result = undefined.
            var zeroModeTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, zeroModeTemp));
            DefineTempStorage(zeroModeTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarMode), zeroModeTemp));

            var nullTargetTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTargetTemp));
            DefineTempStorage(nullTargetTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), nullTargetTemp));

            _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));

            // Jump past iterator path code.
            int afterYieldStarLabel = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranch(afterYieldStarLabel));

            // ---------------------------
            // Iterator delegation path (js2il GeneratorObject)
            // ---------------------------
            _methodBodyIR.Instructions.Add(new LIRLabel(iteratorSetupLabel));

            var twoTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(2.0, twoTemp));
            DefineTempStorage(twoTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarMode), twoTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), isGenObjTemp));

            // Local flag: whether the current delegated step was triggered by generator.return(...)
            var wasReturnTemp = CreateTempVariable();
            DefineTempStorage(wasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            SetTempVariableSlot(wasReturnTemp, CreateAnonymousVariableSlot("$yieldStar_wasReturn", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(falseTemp, wasReturnTemp));

            int iterLoopStart = CreateLabel();
            int iterCallNext = CreateLabel();
            int iterCallThrow = CreateLabel();
            int iterCallReturn = CreateLabel();
            int iterAfterCall = CreateLabel();
            int iterDone = CreateLabel();
            int iterReturnComplete = CreateLabel();
            int iterNormalComplete = CreateLabel();

            // Allocate a distinct resume point for iterator yields.
            var iterResumeStateId = generatorInfo.AllocateResumeStateId();
            var iterResumeLabel = CreateLabel();
            var iterYieldResultTemp = CreateTempVariable();
            DefineTempStorage(iterYieldResultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            generatorInfo.YieldPoints.Add(new YieldPointInfo
            {
                ResumeStateId = iterResumeStateId,
                ResumeLabelId = iterResumeLabel,
                ResultTemp = iterYieldResultTemp
            });
            generatorInfo.RegisterResumeLabel(iterResumeStateId, iterResumeLabel);

            _methodBodyIR.Instructions.Add(new LIRLabel(iterLoopStart));

            var iterObj = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), iterObj));
            DefineTempStorage(iterObj, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasReturn), hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(hasReturnTemp, iterCallReturn));

            var hasThrowTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasResumeException), hasThrowTemp));
            DefineTempStorage(hasThrowTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(hasThrowTemp, iterCallThrow));

            _methodBodyIR.Instructions.Add(new LIRBranch(iterCallNext));

            var iterResult = CreateTempVariable();
            DefineTempStorage(iterResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            _methodBodyIR.Instructions.Add(new LIRLabel(iterCallReturn));
            {
                var trueTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
                DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, wasReturnTemp));

                var returnArg = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._returnValue), returnArg));
                DefineTempStorage(returnArg, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                // Clear outer return flag before forwarding.
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasReturn), falseTemp));

                var argsArr = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRBuildArray(new[] { EnsureObject(returnArg) }, argsArr));
                DefineTempStorage(argsArr, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallMember(iterObj, "return", argsArr, iterResult));
                _methodBodyIR.Instructions.Add(new LIRBranch(iterAfterCall));
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(iterCallThrow));
            {
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(falseTemp, wasReturnTemp));

                var throwArg = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._resumeException), throwArg));
                DefineTempStorage(throwArg, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                // Clear outer throw flag before forwarding.
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasResumeException), falseTemp));

                var argsArr = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRBuildArray(new[] { EnsureObject(throwArg) }, argsArr));
                DefineTempStorage(argsArr, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallMember(iterObj, "throw", argsArr, iterResult));
                _methodBodyIR.Instructions.Add(new LIRBranch(iterAfterCall));
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(iterCallNext));
            {
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(falseTemp, wasReturnTemp));

                var nextArg = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._resumeValue), nextArg));
                DefineTempStorage(nextArg, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                var argsArr = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRBuildArray(new[] { EnsureObject(nextArg) }, argsArr));
                DefineTempStorage(argsArr, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallMember(iterObj, "next", argsArr, iterResult));
                _methodBodyIR.Instructions.Add(new LIRBranch(iterAfterCall));
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(iterAfterCall));

            // done = ToBoolean(result.done)
            var doneKey = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString("done", doneKey));
            DefineTempStorage(doneKey, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var valueKey = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstString("value", valueKey));
            DefineTempStorage(valueKey, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var doneObj = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(iterResult), EnsureObject(doneKey), doneObj));
            DefineTempStorage(doneObj, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var doneBool = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(doneObj, doneBool));
            DefineTempStorage(doneBool, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(doneBool, iterDone));

            // yield result.value
            var yieldedVal = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(iterResult), EnsureObject(valueKey), yieldedVal));
            DefineTempStorage(yieldedVal, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            _methodBodyIR.Instructions.Add(new LIRYield(
                yieldedVal,
                iterResumeStateId,
                iterResumeLabel,
                iterYieldResultTemp,
                HandleThrowReturn: false));

            _methodBodyIR.Instructions.Add(new LIRBranch(iterLoopStart));

            _methodBodyIR.Instructions.Add(new LIRLabel(iterDone));

            var finalVal = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(EnsureObject(iterResult), EnsureObject(valueKey), finalVal));
            DefineTempStorage(finalVal, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Clear delegation state.
            var iterZeroModeTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, iterZeroModeTemp));
            DefineTempStorage(iterZeroModeTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarMode), iterZeroModeTemp));

            var iterNullTargetTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(iterNullTargetTemp));
            DefineTempStorage(iterNullTargetTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._yieldStarTarget), iterNullTargetTemp));

            // If this step was triggered by generator.return(...), complete the outer generator immediately.
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(wasReturnTemp, iterReturnComplete));
            _methodBodyIR.Instructions.Add(new LIRBranch(iterNormalComplete));

            _methodBodyIR.Instructions.Add(new LIRLabel(iterReturnComplete));
            _methodBodyIR.Instructions.Add(new LIRReturn(EnsureObject(finalVal)));

            _methodBodyIR.Instructions.Add(new LIRLabel(iterNormalComplete));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(EnsureObject(finalVal), resultTempVar));

            _methodBodyIR.Instructions.Add(new LIRLabel(afterYieldStarLabel));
            return true;
        }

        TempVariable yieldedValueTemp;
        if (yieldExpr.Argument == null)
        {
            yieldedValueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(yieldedValueTemp));
            DefineTempStorage(yieldedValueTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else
        {
            if (!TryLowerExpression(yieldExpr.Argument, out yieldedValueTemp))
            {
                return false;
            }
            yieldedValueTemp = EnsureObject(yieldedValueTemp);
        }

        var genInfo = _methodBodyIR.GeneratorInfo;
        var resumeStateId = genInfo.AllocateResumeStateId();
        var resumeLabel = CreateLabel();

        resultTempVar = CreateTempVariable();
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        genInfo.YieldPoints.Add(new YieldPointInfo
        {
            ResumeStateId = resumeStateId,
            ResumeLabelId = resumeLabel,
            ResultTemp = resultTempVar
        });

        genInfo.RegisterResumeLabel(resumeStateId, resumeLabel);

        // If we are inside a generator try/finally that was lowered without CLR EH regions,
        // we must not use the built-in yield-site return/throw handling. Instead, we route
        // generator.return/throw through the enclosing finally via pending completion fields.
        var routeThrowReturnToFinally = _isGenerator
            && !_methodBodyIR.LeafScopeId.IsNil
            && _generatorTryFinallyStack.Count > 0;

        _methodBodyIR.Instructions.Add(new LIRYield(
            yieldedValueTemp,
            resumeStateId,
            resumeLabel,
            resultTempVar,
            HandleThrowReturn: !routeThrowReturnToFinally));

        if (routeThrowReturnToFinally)
        {
            var ctx = _generatorTryFinallyStack.Peek();
            var scopeName = _methodBodyIR.LeafScopeId.Name;

            // Shared constants used by the routing logic.
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // if (_hasReturn) { pendingReturn = _returnValue; hasPendingReturn=true; clear pending exception; clear _hasReturn; goto finally; }
            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasReturn), hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var noReturnLabel = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, noReturnLabel));

            var returnValueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._returnValue), returnValueTemp));
            DefineTempStorage(returnValueTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, returnValueTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, trueTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, falseTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasReturn), falseTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, nullTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));

            _methodBodyIR.Instructions.Add(new LIRLabel(noReturnLabel));

            // if (_hasResumeException) { pendingException = _resumeException; hasPendingException=true; clear pending return; clear _hasResumeException; goto finally; }
            var hasThrowTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasResumeException), hasThrowTemp));
            DefineTempStorage(hasThrowTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            var noThrowLabel = CreateLabel();
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasThrowTemp, noThrowLabel));

            var resumeExceptionTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._resumeException), resumeExceptionTemp));
            DefineTempStorage(resumeExceptionTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, resumeExceptionTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, nullTemp));

            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, trueTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, falseTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, nameof(JavaScriptRuntime.GeneratorScope._hasResumeException), falseTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));

            _methodBodyIR.Instructions.Add(new LIRLabel(noThrowLabel));
        }

        return true;
    }
}
