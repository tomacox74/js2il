using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_AsyncAndGenerator(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor,
        Dictionary<int, LabelHandle> labelMap)
    {
        switch (instruction)
        {
            case LIRAsyncReject asyncReject:
                {
                    // Full async state machine: reject _deferred and return its promise.
                    // 1. Mark state as completed: _asyncState = -1
                    // 2. Call _deferred.reject(reason)
                    // 3. Return _deferred.promise
                    if (!(MethodBody.IsAsync && MethodBody.AsyncInfo is { HasAwaits: true }))
                    {
                        throw new InvalidOperationException("LIRAsyncReject is only valid for async methods with awaits.");
                    }

                    var scopeName = MethodBody.LeafScopeId.Name;

                    // _asyncState = -1 (completed)
                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(-1);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");

                    // Load _deferred.reject (it's a bound closure)
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                    var getRejectRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.PromiseWithResolvers),
                        $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.reject)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getRejectRef);

                    // Call it with the rejection reason: Closure.InvokeWithArgs(reject, scopes, argsArray)
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(0);
                    EmitLoadTempAsObject(asyncReject.Reason, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);

                    var invokeWithArgsRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeWithArgsRef);
                    ilEncoder.OpCode(ILOpCode.Pop); // discard result

                    // Return _deferred.promise
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                    var getPromiseRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.PromiseWithResolvers),
                        $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.promise)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getPromiseRef);
                    ilEncoder.OpCode(ILOpCode.Ret);
                    break;
                }

            case LIRGeneratorStateSwitch genSwitch:
                {
                    // Dispatch based on GeneratorScope._genState.
                    // State 0 falls through to DefaultLabel (emitted as a normal LIRLabel).
                    var scopeName = MethodBody.LeafScopeId.Name;

                    // Load _genState
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_genState");

                    if (!labelMap.TryGetValue(genSwitch.DefaultLabel, out var defaultLabel))
                    {
                        defaultLabel = ilEncoder.DefineLabel();
                        labelMap[genSwitch.DefaultLabel] = defaultLabel;
                    }

                    int maxStateId = 0;
                    foreach (var kvp in genSwitch.StateToLabel)
                    {
                        if (kvp.Key > maxStateId) maxStateId = kvp.Key;
                    }

                    int branchCount = maxStateId + 1;
                    var switchTargets = new LabelHandle[branchCount];
                    for (int i = 0; i < branchCount; i++)
                    {
                        switchTargets[i] = defaultLabel;
                    }

                    foreach (var kvp in genSwitch.StateToLabel.Where(kvp => kvp.Key > 0 && kvp.Key < branchCount))
                    {
                        var stateId = kvp.Key;
                        var labelId = kvp.Value;
                        if (!labelMap.TryGetValue(labelId, out var resumeLabel))
                        {
                            resumeLabel = ilEncoder.DefineLabel();
                            labelMap[labelId] = resumeLabel;
                        }
                        switchTargets[stateId] = resumeLabel;
                    }

                    var switchEncoder = ilEncoder.Switch(branchCount);
                    for (int i = 0; i < branchCount; i++)
                    {
                        switchEncoder.Branch(switchTargets[i]);
                    }

                    break;
                }

            case LIRYield yieldInstr:
                {
                    var scopeName = MethodBody.LeafScopeId.Name;

                    // Async generator yield: resolve the current _deferred with { value, done: false }
                    // and return _deferred.promise.
                    if (MethodBody.IsAsync && MethodBody.IsGenerator)
                    {
                        // _genState = ResumeStateId
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(yieldInstr.ResumeStateId);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_genState");

                        // _asyncState = -1 (completed for this next() call)
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(-1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");

                        var iterCreateAsync = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.IteratorResult),
                            nameof(JavaScriptRuntime.IteratorResult.Create),
                            parameterTypes: new[] { typeof(object), typeof(bool) });

                        var invokeWithArgsRefAsync = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                            parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });

                        // Load _deferred.resolve (it's a bound closure)
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                        var getResolveRefAsync = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.resolve)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getResolveRefAsync);

                        // Invoke resolve with [{ value: yielded, done: false }]
                        EmitLoadScopesArray(ilEncoder, methodDescriptor);
                        ilEncoder.LoadConstantI4(1);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadConstantI4(0);

                        // Create iterator result and store it into the args array
                        EmitLoadTempAsObject(yieldInstr.YieldedValue, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.LoadConstantI4(0);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(iterCreateAsync);
                        ilEncoder.OpCode(ILOpCode.Stelem_ref);

                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(invokeWithArgsRefAsync);
                        ilEncoder.OpCode(ILOpCode.Pop);

                        // Return _deferred.promise
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                        var getPromiseRefAsync = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.promise)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getPromiseRefAsync);
                        ilEncoder.OpCode(ILOpCode.Ret);

                        // Resume label
                        if (!labelMap.TryGetValue(yieldInstr.ResumeLabelId, out var resumeLabelAsync))
                        {
                            resumeLabelAsync = ilEncoder.DefineLabel();
                            labelMap[yieldInstr.ResumeLabelId] = resumeLabelAsync;
                        }
                        ilEncoder.MarkLabel(resumeLabelAsync);

                        if (yieldInstr.HandleThrowReturn)
                        {
                            // If _hasReturn: mark done and resolve deferred with { value: _returnValue, done: true }
                            var noReturnLabel = ilEncoder.DefineLabel();
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_hasReturn");
                            ilEncoder.Branch(ILOpCode.Brfalse, noReturnLabel);

                            ilEncoder.LoadLocal(0);
                            ilEncoder.LoadConstantI4(1);
                            EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                            // _asyncState = -1
                            ilEncoder.LoadLocal(0);
                            ilEncoder.LoadConstantI4(-1);
                            EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");

                            // resolve({ value: _returnValue, done: true })
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(getResolveRefAsync);

                            EmitLoadScopesArray(ilEncoder, methodDescriptor);
                            ilEncoder.LoadConstantI4(1);
                            ilEncoder.OpCode(ILOpCode.Newarr);
                            ilEncoder.Token(_bclReferences.ObjectType);
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(0);
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_returnValue");
                            ilEncoder.LoadConstantI4(1);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(iterCreateAsync);
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(invokeWithArgsRefAsync);
                            ilEncoder.OpCode(ILOpCode.Pop);

                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(getPromiseRefAsync);
                            ilEncoder.OpCode(ILOpCode.Ret);

                            ilEncoder.MarkLabel(noReturnLabel);

                            // If _hasResumeException: throw at yield site
                            var noThrowLabel = ilEncoder.DefineLabel();
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_hasResumeException");
                            ilEncoder.Branch(ILOpCode.Brfalse, noThrowLabel);

                            ilEncoder.LoadLocal(0);
                            ilEncoder.LoadConstantI4(0);
                            EmitStoreFieldByName(ilEncoder, scopeName, "_hasResumeException");

                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_resumeException");
                            var thrownCtor = _memberRefRegistry.GetOrAddConstructor(
                                typeof(JavaScriptRuntime.JsThrownValueException),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Newobj);
                            ilEncoder.Token(thrownCtor);
                            ilEncoder.OpCode(ILOpCode.Throw);

                            ilEncoder.MarkLabel(noThrowLabel);
                        }

                        if (IsMaterialized(yieldInstr.Result, allocation))
                        {
                            // yield expression result = _resumeValue
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_resumeValue");
                            EmitStoreTemp(yieldInstr.Result, ilEncoder, allocation);
                        }

                        break;
                    }

                    // _genState = ResumeStateId
                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(yieldInstr.ResumeStateId);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_genState");

                    // Return { value: yielded, done: false }
                    EmitLoadTempAsObject(yieldInstr.YieldedValue, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.LoadConstantI4(0);
                    var iterCreate = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.IteratorResult),
                        nameof(JavaScriptRuntime.IteratorResult.Create),
                        parameterTypes: new[] { typeof(object), typeof(bool) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(iterCreate);
                    ilEncoder.OpCode(ILOpCode.Ret);

                    // Resume label
                    if (!labelMap.TryGetValue(yieldInstr.ResumeLabelId, out var resumeLabel))
                    {
                        resumeLabel = ilEncoder.DefineLabel();
                        labelMap[yieldInstr.ResumeLabelId] = resumeLabel;
                    }
                    ilEncoder.MarkLabel(resumeLabel);

                    if (yieldInstr.HandleThrowReturn)
                    {
                        // If _hasReturn: mark done and return { value: _returnValue, done: true }
                        var noReturnLabel = ilEncoder.DefineLabel();
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_hasReturn");
                        ilEncoder.Branch(ILOpCode.Brfalse, noReturnLabel);

                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_returnValue");
                        ilEncoder.LoadConstantI4(1);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(iterCreate);
                        ilEncoder.OpCode(ILOpCode.Ret);

                        ilEncoder.MarkLabel(noReturnLabel);

                        // If _hasResumeException: throw at yield site
                        var noThrowLabel = ilEncoder.DefineLabel();
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_hasResumeException");
                        ilEncoder.Branch(ILOpCode.Brfalse, noThrowLabel);

                        // Clear the injected-exception flag before throwing so a caught exception
                        // doesn't get rethrown on the next resume.
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(0);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_hasResumeException");

                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_resumeException");
                        var thrownCtor = _memberRefRegistry.GetOrAddConstructor(
                            typeof(JavaScriptRuntime.JsThrownValueException),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(thrownCtor);
                        ilEncoder.OpCode(ILOpCode.Throw);

                        ilEncoder.MarkLabel(noThrowLabel);
                    }

                    if (IsMaterialized(yieldInstr.Result, allocation))
                    {
                        // yield expression result = _resumeValue
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_resumeValue");
                        EmitStoreTemp(yieldInstr.Result, ilEncoder, allocation);
                    }
                    break;
                }

            case LIRAwait awaitInstr:
                {
                    var asyncInfo = MethodBody.AsyncInfo;

                    // Check if we need full state machine or MVP approach
                    if (asyncInfo == null || !asyncInfo.HasAwaits)
                    {
                        // MVP await implementation: calls Promise.AwaitValue() helper
                        // which handles already-resolved promises synchronously.
                        // For pending promises, it throws NotSupportedException.

                        // Load the awaited value
                        EmitLoadTemp(awaitInstr.AwaitedValue, ilEncoder, allocation, methodDescriptor);

                        // Call Promise.AwaitValue(awaited) -> returns the resolved value
                        var awaitValueRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.AwaitValue),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(awaitValueRef);

                        // Store result to the result temp
                        EmitStoreTemp(awaitInstr.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        // Full state machine implementation:
                        // 1. Store state for resumption
                        // 2. Call SetupAwaitContinuation to schedule .then() callbacks
                        // 3. Load _deferred.promise and return
                        // 4. Resume label (jumped to when continuation fires)
                        // 5. Load awaited result from field

                        var scopeName = MethodBody.LeafScopeId.Name;
                        var resultFieldName = $"_awaited{awaitInstr.AwaitId}";

                        // Get the resume label
                        if (!labelMap.TryGetValue(awaitInstr.ResumeLabelId, out var resumeLabel))
                        {
                            resumeLabel = ilEncoder.DefineLabel();
                            labelMap[awaitInstr.ResumeLabelId] = resumeLabel;
                        }

                        // --- Step 1: Store state for resumption ---
                        // ldloc.0 (scope), ldc.i4 resumeStateId, stfld _asyncState
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(awaitInstr.ResumeStateId);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");

                        // --- Step 2: Call SetupAwaitContinuation ---
                        // Call is emitted as an instance method on AsyncScope.
                        // Stack: scope(this), awaited, scopesArray, resultFieldName, moveNext, [rejectStateId], [pendingExceptionFieldName]

                        // this: scope (ldloc.0)
                        ilEncoder.LoadLocal(0);

                        // awaited value
                        EmitLoadTemp(awaitInstr.AwaitedValue, ilEncoder, allocation, methodDescriptor);

                        // scopesArray
                        EmitLoadScopesArray(ilEncoder, methodDescriptor);

                        // resultFieldName
                        ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(resultFieldName));

                        // moveNext (ldloc.0, ldfld _moveNext)
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_moveNext");

                        if (awaitInstr.RejectResumeStateId.HasValue && !string.IsNullOrEmpty(awaitInstr.PendingExceptionFieldName))
                        {
                            ilEncoder.LoadConstantI4(awaitInstr.RejectResumeStateId.Value);
                            ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(awaitInstr.PendingExceptionFieldName));

                            var setupAwaitRef = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.AsyncScope),
                                nameof(JavaScriptRuntime.AsyncScope.SetupAwaitContinuationWithRejectResume),
                                parameterTypes: new[] { typeof(object), typeof(object[]), typeof(string), typeof(object), typeof(int), typeof(string) });
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(setupAwaitRef);
                        }

                        else
                        {
                            var setupAwaitRef = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.AsyncScope),
                                nameof(JavaScriptRuntime.AsyncScope.SetupAwaitContinuation),
                                parameterTypes: new[] { typeof(object), typeof(object[]), typeof(string), typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(setupAwaitRef);
                        }

                        // Persist variable locals across the suspension.
                        // The async continuation re-enters the method, so IL locals must be restored from scope storage.
                        EmitSpillVariableSlotsToAsyncLocalsArray(ilEncoder);

                        // --- Step 3: Return _deferred.promise ---
                        // ldloc.0, ldfld _deferred, callvirt get_promise, ret
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");

                        var getPromiseRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.promise)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getPromiseRef);
                        ilEncoder.OpCode(ILOpCode.Ret);

                        // --- Step 4: Resume label ---
                        ilEncoder.MarkLabel(resumeLabel);

                        // --- Step 5: Load awaited result from field ---
                        // ldloc.0, ldfld _awaitedN
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, resultFieldName);

                        // Store to result temp
                        EmitStoreTemp(awaitInstr.Result, ilEncoder, allocation);
                    }
                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
