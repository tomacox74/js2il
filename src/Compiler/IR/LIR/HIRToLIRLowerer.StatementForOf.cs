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
    private bool TryLowerForOfStatement(HIRForOfStatement forOfStmt)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        if (forOfStmt.IsAwait)
        {
            // Desugar for await..of using async iterator protocol (ECMA-262 14.7.5.6):
            // iterator = Object.GetAsyncIterator(rhs)
            // completed = false; closed = false
            // try {
            //   while (true) {
            //     result = await Object.AsyncIteratorNext(iterator)
            //     if (ToBoolean(result.done)) break
            //     value = result.value
            //     target = value
            //     body
            //   }
            //   completed = true
            // } finally {
            //   // AsyncIteratorClose on abrupt completion (await return())
            //   if (!completed && !closed) await Object.AsyncIteratorClose(iterator)
            // }

            if (!_isAsync || _methodBodyIR.AsyncInfo == null || _methodBodyIR.LeafScopeId.IsNil)
            {
                return false;
            }

            var asyncInfo = _methodBodyIR.AsyncInfo;
            var scopeName = _methodBodyIR.LeafScopeId.Name;

            const string pendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._pendingException);
            const string hasPendingExceptionField = nameof(JavaScriptRuntime.AsyncScope._hasPendingException);
            const string pendingReturnField = nameof(JavaScriptRuntime.AsyncScope._pendingReturnValue);
            const string hasPendingReturnField = nameof(JavaScriptRuntime.AsyncScope._hasPendingReturn);

            // Spec: CreatePerIterationEnvironment for for..of with lexical declarations.
            var perIterationBindings = (forOfStmt.IsDeclaration && (forOfStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
                ? forOfStmt.LoopHeadBindings.Where(b => b.Kind is BindingKind.Let or BindingKind.Const).Where(b => b.IsCaptured).ToList()
                : new List<BindingInfo>();

            bool useTempPerIterationScope = false;
            TempVariable loopScopeTemp = default;
            ScopeId loopScopeId = default;
            string? loopScopeName = null;

            // Current implementation only supports the "typed temp scope" optimization for non-async methods.
            // Keep behavior consistent with sync for..of lowering.
            if (perIterationBindings.Count > 0
                && !_methodBodyIR.IsAsync
                && !_methodBodyIR.IsGenerator)
            {
                var declaringScope = perIterationBindings[0].DeclaringScope;
                if (declaringScope != null
                    && declaringScope.Kind == ScopeKind.Block
                    && perIterationBindings.All(b => b.DeclaringScope == declaringScope))
                {
                    useTempPerIterationScope = true;
                    loopScopeName = ScopeNaming.GetRegistryScopeName(declaringScope);
                    loopScopeId = new ScopeId(loopScopeName);

                    loopScopeTemp = CreateTempVariable();
                    DefineTempStorage(loopScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName));
                    SetTempVariableSlot(loopScopeTemp, CreateAnonymousVariableSlot("$forAwaitOf_lexenv", new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName)));
                    _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(loopScopeId, loopScopeTemp));
                    _activeScopeTempsByScopeName[loopScopeName] = loopScopeTemp;
                }
            }

            try
            {
                if (!TryLowerExpression(forOfStmt.Iterable, out var rhsTemp))
                {
                    return false;
                }

                var rhsBoxed = EnsureObject(rhsTemp);

                // iterator = Object.GetAsyncIterator(rhs)
                var iterTemp = CreateTempVariable();
                lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetAsyncIterator), new[] { rhsBoxed }, iterTemp));
                DefineTempStorage(iterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptAsyncIterator)));
                SetTempVariableSlot(iterTemp, CreateAnonymousVariableSlot("$forAwaitOf_iter", new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptAsyncIterator))));

                var completedTemp = CreateTempVariable();
                DefineTempStorage(completedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                SetTempVariableSlot(completedTemp, CreateAnonymousVariableSlot("$forAwaitOf_completed", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

                var closedTemp = CreateTempVariable();
                DefineTempStorage(closedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                SetTempVariableSlot(closedTemp, CreateAnonymousVariableSlot("$forAwaitOf_closed", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

                var falseTemp = CreateTempVariable();
                lirInstructions.Add(new LIRConstBoolean(false, falseTemp));
                DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                lirInstructions.Add(new LIRCopyTemp(falseTemp, completedTemp));
                lirInstructions.Add(new LIRCopyTemp(falseTemp, closedTemp));

                var trueTemp = CreateTempVariable();
                lirInstructions.Add(new LIRConstBoolean(true, trueTemp));
                DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

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

                // Reset pending completion fields on entry.
                {
                    var nullTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
                    DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingReturnField, nullTemp));

                    var resetFalseTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, resetFalseTemp));
                    DefineTempStorage(resetFalseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, resetFalseTemp));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, resetFalseTemp));
                }

                int loopStartLabel = CreateLabel();
                int loopUpdateLabel = CreateLabel();
                int breakCleanupLabel = CreateLabel();
                int normalCompleteLabel = CreateLabel();

                _controlFlowStack.Push(new ControlFlowContext(breakCleanupLabel, loopUpdateLabel, forOfStmt.Label));
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
                    // Loop start
                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    // awaitedNext = await Object.AsyncIteratorNext(iterator)
                    var nextCallTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.AsyncIteratorNext), new[] { EnsureObject(iterTemp) }, nextCallTemp));
                    DefineTempStorage(nextCallTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    // Emit await of nextCallTemp
                    var awaitId = asyncInfo.AllocateAwaitId();
                    var resumeStateId = asyncInfo.AllocateResumeStateId();
                    var resumeLabelId = CreateLabel();
                    var iterResult = CreateTempVariable();
                    DefineTempStorage(iterResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    asyncInfo.AwaitPoints.Add(new AwaitPointInfo
                    {
                        AwaitId = awaitId,
                        ResumeStateId = resumeStateId,
                        ResumeLabelId = resumeLabelId,
                        ResultTemp = iterResult
                    });
                    asyncInfo.RegisterResumeLabel(resumeStateId, resumeLabelId);

                    int? rejectStateId = null;
                    string? rejectPendingExceptionField = null;
                    if (_asyncTryCatchStack.Count > 0)
                    {
                        var ctx = _asyncTryCatchStack.Peek();
                        rejectStateId = ctx.CatchStateId;
                        rejectPendingExceptionField = ctx.PendingExceptionFieldName;
                    }

                    _methodBodyIR.Instructions.Add(new LIRAwait(
                        nextCallTemp,
                        awaitId,
                        resumeStateId,
                        resumeLabelId,
                        iterResult,
                        rejectStateId,
                        rejectPendingExceptionField));

                    // done = Object.IteratorResultDone(result)
                    var doneBool = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultDone), new[] { EnsureObject(iterResult) }, doneBool));
                    DefineTempStorage(doneBool, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    lirInstructions.Add(new LIRBranchIfTrue(doneBool, normalCompleteLabel));

                    // value = Object.IteratorResultValue(result)
                    var itemTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultValue), new[] { EnsureObject(iterResult) }, itemTemp));
                    DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    var writeMode = (forOfStmt.IsDeclaration && (forOfStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
                        ? DestructuringWriteMode.ForDeclarationBindingInitialization
                        : DestructuringWriteMode.Assignment;

                    if (!TryLowerDestructuringPattern(forOfStmt.Target, itemTemp, writeMode, sourceNameForError: null))
                    {
                        return false;
                    }

                    if (!TryLowerStatement(forOfStmt.Body))
                    {
                        return false;
                    }

                    // Continue target
                    lirInstructions.Add(new LIRLabel(loopUpdateLabel));
                    if (useTempPerIterationScope)
                    {
                        EmitRecreatePerIterationScopeFromTemp(loopScopeTemp, loopScopeId, loopScopeName!, perIterationBindings);
                    }
                    lirInstructions.Add(new LIRBranch(loopStartLabel));

                    // Break target: exit via finally (which performs AsyncIteratorClose on abrupt completion).
                    lirInstructions.Add(new LIRLabel(breakCleanupLabel));
                    _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));

                    // Normal completion: mark completed then exit via finally.
                    lirInstructions.Add(new LIRLabel(normalCompleteLabel));
                    lirInstructions.Add(new LIRCopyTemp(trueTemp, completedTemp));
                    _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));
                }
                finally
                {
                    _asyncTryCatchStack.Pop();
                    _asyncTryFinallyStack.Pop();
                    _controlFlowStack.Pop();
                }

                // --- Exception path into finally (synthetic; used for await rejection / throw) ---
                _methodBodyIR.Instructions.Add(new LIRLabel(exceptionToFinallyLabel));
                {
                    var setHasExTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, setHasExTemp));
                    DefineTempStorage(setHasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, setHasExTemp));

                    var clearHasReturnTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, clearHasReturnTemp));
                    DefineTempStorage(clearHasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, clearHasReturnTemp));

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
                    int finallySkipClose = CreateLabel();
                    _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(completedTemp, finallySkipClose));
                    _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(closedTemp, finallySkipClose));

                    // Await Object.AsyncIteratorClose(iterator)
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(trueTemp, closedTemp));
                    var closeCallTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.AsyncIteratorClose), new[] { EnsureObject(iterTemp) }, closeCallTemp));
                    DefineTempStorage(closeCallTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    var finallyAwaitId = asyncInfo.AllocateAwaitId();
                    var finallyResumeStateId = asyncInfo.AllocateResumeStateId();
                    var finallyResumeLabelId = CreateLabel();
                    var finallyCloseResultTemp = CreateTempVariable();
                    DefineTempStorage(finallyCloseResultTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    asyncInfo.AwaitPoints.Add(new AwaitPointInfo
                    {
                        AwaitId = finallyAwaitId,
                        ResumeStateId = finallyResumeStateId,
                        ResumeLabelId = finallyResumeLabelId,
                        ResultTemp = finallyCloseResultTemp
                    });
                    asyncInfo.RegisterResumeLabel(finallyResumeStateId, finallyResumeLabelId);

                    int? finallyRejectStateId = null;
                    string? finallyRejectPendingExceptionField = null;
                    if (_asyncTryCatchStack.Count > 0)
                    {
                        var ctx = _asyncTryCatchStack.Peek();
                        finallyRejectStateId = ctx.CatchStateId;
                        finallyRejectPendingExceptionField = ctx.PendingExceptionFieldName;
                    }

                    _methodBodyIR.Instructions.Add(new LIRAwait(
                        closeCallTemp,
                        finallyAwaitId,
                        finallyResumeStateId,
                        finallyResumeLabelId,
                        finallyCloseResultTemp,
                        finallyRejectStateId,
                        finallyRejectPendingExceptionField));

                    _methodBodyIR.Instructions.Add(new LIRLabel(finallySkipClose));
                    _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));
                }
                finally
                {
                    _asyncTryCatchStack.Pop();
                    _asyncTryFinallyStack.Pop();
                }

                // --- Exception inside finally overrides prior completion ---
                _methodBodyIR.Instructions.Add(new LIRLabel(exceptionInFinallyLabel));
                {
                    var setHasExTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, setHasExTemp));
                    DefineTempStorage(setHasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, setHasExTemp));

                    var clearHasReturnTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, clearHasReturnTemp));
                    DefineTempStorage(clearHasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, clearHasReturnTemp));

                    _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));
                }

                // --- After finally: dispatch based on completion ---
                _methodBodyIR.Instructions.Add(new LIRLabel(finallyExitLabel));

                var checkReturnLabel = CreateLabel();
                {
                    var hasExTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingExceptionField, hasExTemp));
                    DefineTempStorage(hasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    SetTempVariableSlot(hasExTemp, CreateAnonymousVariableSlot("$forAwaitOf_hasEx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

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
                    SetTempVariableSlot(hasReturnTemp, CreateAnonymousVariableSlot("$forAwaitOf_hasReturn", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

                    _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, afterTryLabel));

                    var retTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingReturnField, retTemp));
                    DefineTempStorage(retTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    _methodBodyIR.Instructions.Add(new LIRReturn(retTemp));
                }

                _methodBodyIR.Instructions.Add(new LIRLabel(afterTryLabel));
                return true;
            }
            finally
            {
                if (useTempPerIterationScope && loopScopeName != null)
                {
                    _activeScopeTempsByScopeName.Remove(loopScopeName);
                }
            }
        }
        else
        {
            // Desugar for..of using iterator protocol (ECMA-262 14.7.5.5-.7):
            // iterator = Object.GetIterator(rhs)
            // try {
            //   while (true) {
            //     result = iterator.next()
            //     if (ToBoolean(result.done)) break
            //     value = result.value
            //     target = value
            //     body
            //   }
            // } finally {
            //   // IteratorClose on abrupt completion
            //   if (!completed && !closedExplicitly) Object.IteratorClose(iterator)
            // }

            // Spec: CreatePerIterationEnvironment for for..of with lexical declarations.
            var perIterationBindings = (forOfStmt.IsDeclaration && (forOfStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
                ? forOfStmt.LoopHeadBindings.Where(b => b.Kind is BindingKind.Let or BindingKind.Const).Where(b => b.IsCaptured).ToList()
                : new List<BindingInfo>();

            bool useTempPerIterationScope = false;
            TempVariable loopScopeTemp = default;
            ScopeId loopScopeId = default;
            string? loopScopeName = null;

            if (perIterationBindings.Count > 0
                && !_methodBodyIR.IsAsync
                && !_methodBodyIR.IsGenerator)
            {
                var declaringScope = perIterationBindings[0].DeclaringScope;
                if (declaringScope != null
                    && declaringScope.Kind == ScopeKind.Block
                    && perIterationBindings.All(b => b.DeclaringScope == declaringScope))
                {
                    useTempPerIterationScope = true;
                    loopScopeName = ScopeNaming.GetRegistryScopeName(declaringScope);
                    loopScopeId = new ScopeId(loopScopeName);

                    loopScopeTemp = CreateTempVariable();
                    DefineTempStorage(loopScopeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName));
                    SetTempVariableSlot(loopScopeTemp, CreateAnonymousVariableSlot("$forOf_lexenv", new ValueStorage(ValueStorageKind.Reference, typeof(object), ScopeName: loopScopeName)));
                    _methodBodyIR.Instructions.Add(new LIRCreateScopeInstance(loopScopeId, loopScopeTemp));
                    _activeScopeTempsByScopeName[loopScopeName] = loopScopeTemp;
                }
            }

            try
            {
                if (!TryLowerExpression(forOfStmt.Iterable, out var rhsTemp))
                {
                    return false;
                }

                var rhsBoxed = EnsureObject(rhsTemp);

                // iterator = Object.GetIterator(rhs)
                var iterTemp = CreateTempVariable();
                lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.GetIterator), new[] { rhsBoxed }, iterTemp));
                DefineTempStorage(iterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptIterator)));
                // NOTE: temp-local allocation is linear and does not account for loop back-edges.
                // Pin loop-carry temps to stable variable slots so values remain correct across iterations.
                SetTempVariableSlot(iterTemp, CreateAnonymousVariableSlot("$forOf_iter", new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.IJavaScriptIterator))));

                var completedTemp = CreateTempVariable();
                DefineTempStorage(completedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                SetTempVariableSlot(completedTemp, CreateAnonymousVariableSlot("$forOf_completed", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

                var closedTemp = CreateTempVariable();
                DefineTempStorage(closedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                SetTempVariableSlot(closedTemp, CreateAnonymousVariableSlot("$forOf_closed", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

                var falseTemp = CreateTempVariable();
                lirInstructions.Add(new LIRConstBoolean(false, falseTemp));
                DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                lirInstructions.Add(new LIRCopyTemp(falseTemp, completedTemp));
                lirInstructions.Add(new LIRCopyTemp(falseTemp, closedTemp));

                var trueTemp = CreateTempVariable();
                lirInstructions.Add(new LIRConstBoolean(true, trueTemp));
                DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

                int outerTryStart = CreateLabel();
                int outerTryEnd = CreateLabel();
                int finallyStart = CreateLabel();
                int finallyEnd = CreateLabel();

                int loopStartLabel = CreateLabel();
                int loopUpdateLabel = CreateLabel();
                int breakCleanupLabel = CreateLabel();
                int normalCompleteLabel = CreateLabel();
                int loopEndLabel = CreateLabel();

                // Track current control-flow depth so we can decide when break/continue exits the protected region.
                _protectedControlFlowDepthStack.Push(_controlFlowStack.Count);

                // Any return inside a protected region must use 'leave' to an epilogue outside the region.
                if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
                {
                    _methodBodyIR.ReturnEpilogueLabelId = CreateLabel();
                }

                _controlFlowStack.Push(new ControlFlowContext(breakCleanupLabel, loopUpdateLabel, forOfStmt.Label));
                try
                {
                    lirInstructions.Add(new LIRLabel(outerTryStart));

                    // Loop start
                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    // result = Object.IteratorNext(iterator)
                    var iterResult = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorNext), new[] { EnsureObject(iterTemp) }, iterResult));
                    DefineTempStorage(iterResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    // done = Object.IteratorResultDone(result)
                    var doneBool = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultDone), new[] { EnsureObject(iterResult) }, doneBool));
                    DefineTempStorage(doneBool, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    lirInstructions.Add(new LIRBranchIfTrue(doneBool, normalCompleteLabel));

                    // value = Object.IteratorResultValue(result)
                    var itemTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.IteratorResultValue), new[] { EnsureObject(iterResult) }, itemTemp));
                    DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    var writeMode = (forOfStmt.IsDeclaration && (forOfStmt.DeclarationKind is BindingKind.Let or BindingKind.Const))
                        ? DestructuringWriteMode.ForDeclarationBindingInitialization
                        : DestructuringWriteMode.Assignment;

                    if (!TryLowerDestructuringPattern(forOfStmt.Target, itemTemp, writeMode, sourceNameForError: null))
                    {
                        return false;
                    }

                    if (!TryLowerStatement(forOfStmt.Body))
                    {
                        return false;
                    }

                    // Continue target
                    lirInstructions.Add(new LIRLabel(loopUpdateLabel));

                    if (useTempPerIterationScope)
                    {
                        EmitRecreatePerIterationScopeFromTemp(loopScopeTemp, loopScopeId, loopScopeName!, perIterationBindings);
                    }

                    lirInstructions.Add(new LIRBranch(loopStartLabel));

                    // Break target: close iterator then leave loop.
                    lirInstructions.Add(new LIRLabel(breakCleanupLabel));
                    lirInstructions.Add(new LIRCallIntrinsicStaticVoid("Object", "IteratorClose", new[] { EnsureObject(iterTemp) }));
                    lirInstructions.Add(new LIRCopyTemp(trueTemp, closedTemp));
                    lirInstructions.Add(new LIRLeave(loopEndLabel));

                    // Normal completion: leave loop.
                    lirInstructions.Add(new LIRLabel(normalCompleteLabel));
                    lirInstructions.Add(new LIRCopyTemp(trueTemp, completedTemp));
                    lirInstructions.Add(new LIRLeave(loopEndLabel));

                    lirInstructions.Add(new LIRLabel(outerTryEnd));

                    lirInstructions.Add(new LIRLabel(finallyStart));
                    // Only close on abrupt completion.
                    int finallySkipClose = CreateLabel();
                    lirInstructions.Add(new LIRBranchIfTrue(completedTemp, finallySkipClose));
                    lirInstructions.Add(new LIRBranchIfTrue(closedTemp, finallySkipClose));
                    lirInstructions.Add(new LIRCallIntrinsicStaticVoid("Object", "IteratorClose", new[] { EnsureObject(iterTemp) }));
                    lirInstructions.Add(new LIRLabel(finallySkipClose));
                    lirInstructions.Add(new LIREndFinally());
                    lirInstructions.Add(new LIRLabel(finallyEnd));

                    lirInstructions.Add(new LIRLabel(loopEndLabel));

                    _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
                        ExceptionRegionKind.Finally,
                        TryStartLabelId: outerTryStart,
                        TryEndLabelId: outerTryEnd,
                        HandlerStartLabelId: finallyStart,
                        HandlerEndLabelId: finallyEnd));

                    return true;
                }
                finally
                {
                    _controlFlowStack.Pop();
                    _protectedControlFlowDepthStack.Pop();
                }
            }
            finally
            {
                if (useTempPerIterationScope && loopScopeName != null)
                {
                    _activeScopeTempsByScopeName.Remove(loopScopeName);
                }
            }
        }
    }
}
