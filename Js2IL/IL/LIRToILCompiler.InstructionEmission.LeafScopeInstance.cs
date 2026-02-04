using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private void EmitInitArgumentsObjectIfNeeded(InstructionEncoder ilEncoder, string scopeName)
    {
        var callableId = MethodBody.CallableId;
        if (callableId is null || !callableId.NeedsArgumentsObject)
        {
            return;
        }

        // scope.arguments = RuntimeServices.CreateArgumentsObject();
        ilEncoder.LoadLocal(0);
        var createArgsRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.CreateArgumentsObject),
            Type.EmptyTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(createArgsRef);
        EmitStoreFieldByName(ilEncoder, scopeName, "arguments");
    }

    private bool? TryCompileInstructionToIL_LeafScopeInstance(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor,
        Dictionary<int, LabelHandle> labelMap)
    {
        switch (instruction)
        {
            case LIRCreateLeafScopeInstance createScope:
                {
                    // Async generators are both async and generator resumables.
                    // - Factory call returns an AsyncGeneratorObject
                    // - Step calls run the method body, potentially awaiting and/or yielding
                    //
                    // Step calls are detected by scopes[0] being our leaf scope type.
                    if (MethodBody.IsAsync && MethodBody.IsGenerator)
                    {
                        if (!methodDescriptor.HasScopesParameter)
                        {
                            throw new InvalidOperationException("Async generator methods must use the js2il ABI and declare a leading scopes parameter.");
                        }

                        int scopesArgIndex = GetIlArgIndexForScopesArray(methodDescriptor);

                        var scopeTypeHandle = ResolveScopeTypeHandle(
                            createScope.Scope.Name,
                            "LIRCreateLeafScopeInstance instruction (async generator)");
                        var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                        var scopeName = createScope.Scope.Name;

                        // ldarg scopes, ldc.i4.0, ldelem.ref, isinst ScopeType
                        ilEncoder.LoadArgument(scopesArgIndex);
                        ilEncoder.LoadConstantI4(0);
                        ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                        ilEncoder.OpCode(ILOpCode.Isinst);
                        ilEncoder.Token(scopeTypeHandle);
                        ilEncoder.OpCode(ILOpCode.Dup);

                        var stepLabel = ilEncoder.DefineLabel();
                        ilEncoder.Branch(ILOpCode.Brtrue, stepLabel);

                        // --- Factory path ---
                        ilEncoder.OpCode(ILOpCode.Pop); // pop null

                        // Create leaf scope instance
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctorRef);
                        ilEncoder.StoreLocal(0);

                        EmitInitArgumentsObjectIfNeeded(ilEncoder, scopeName);

                        // Build modified scopes array with leaf at [0]
                        ilEncoder.LoadLocal(0); // leafScope
                        ilEncoder.LoadArgument(scopesArgIndex); // parent scopes
                        var prependRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.PrependScopeToArray),
                            parameterTypes: new[] { typeof(object), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(prependRef);
                        ilEncoder.StoreArgument(scopesArgIndex); // scopes = modified scopes array

                        // Initialize _moveNext = Closure.BindMoveNext(delegate, scopesArray, boundArgs)
                        // so await continuations can resume this step method.
                        ilEncoder.LoadLocal(0);  // for stfld _moveNext later

                        var callableId = MethodBody.CallableId
                            ?? throw new InvalidOperationException("Async generator method is missing CallableId.");
                        var reader = _serviceProvider.GetService<ICallableDeclarationReader>()
                            ?? throw new InvalidOperationException("ICallableDeclarationReader service is not available.");
                        if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                        {
                            throw new InvalidOperationException($"Failed to resolve MethodDefinitionHandle for async generator callable {callableId}.");
                        }

                        var methodHandle = (MethodDefinitionHandle)token;

                        int jsParamCount = callableId.JsParamCount;

                        if (methodDescriptor.IsStatic)
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Ldarg_0);
                        }
                        ilEncoder.OpCode(ILOpCode.Ldftn);
                        ilEncoder.Token(methodHandle);
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                        // Load modified scopes array
                        ilEncoder.LoadArgument(scopesArgIndex);

                        // Build boundArgs = new object[jsParamCount] filled from method arguments (excluding scopes).
                        ilEncoder.LoadConstantI4(jsParamCount);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);

                        for (var i = 0; i < jsParamCount; i++)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(i);
                            ilEncoder.LoadArgument(GetIlArgIndexForJsParameter(methodDescriptor, i));
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }

                        var bindMoveNextRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            nameof(JavaScriptRuntime.Closure.BindMoveNext),
                            parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(bindMoveNextRef);

                        EmitStoreFieldByName(ilEncoder, scopeName, "_moveNext");

                        // Return new AsyncGeneratorObject(scopes)
                        ilEncoder.LoadArgument(scopesArgIndex);
                        var asyncGenObjCtor = _memberRefRegistry.GetOrAddConstructor(
                            typeof(JavaScriptRuntime.AsyncGeneratorObject),
                            parameterTypes: new[] { typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(asyncGenObjCtor);
                        ilEncoder.OpCode(ILOpCode.Ret);

                        // --- Step path ---
                        ilEncoder.MarkLabel(stepLabel);
                        ilEncoder.StoreLocal(0);

                        // Restore async locals across awaits (only meaningful when there are awaits).
                        var asyncInfoForAsyncGen = MethodBody.AsyncInfo;
                        if (asyncInfoForAsyncGen != null && asyncInfoForAsyncGen.HasAwaits)
                        {
                            EmitEnsureAsyncLocalsArray(ilEncoder);

                            var skipRestoreLabel = ilEncoder.DefineLabel();
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_asyncState");
                            ilEncoder.LoadConstantI4(0);
                            ilEncoder.Branch(ILOpCode.Ble_s, skipRestoreLabel);
                            EmitRestoreVariableSlotsFromAsyncLocalsArray(ilEncoder);
                            ilEncoder.MarkLabel(skipRestoreLabel);

                            if (asyncInfoForAsyncGen.MaxResumeStateId > 0)
                            {
                                EmitAsyncStateSwitch(ilEncoder, labelMap, asyncInfoForAsyncGen);
                            }
                        }

                        // Generator state dispatch: resume from the last yield site.
                        var genInfo = MethodBody.GeneratorInfo;
                        if (genInfo != null && genInfo.ResumeLabels.Count > 0)
                        {
                            // Load _genState
                            ilEncoder.LoadLocal(0);
                            EmitLoadFieldByName(ilEncoder, scopeName, "_genState");

                            var defaultLabel = ilEncoder.DefineLabel();

                            int maxStateId = 0;
                            foreach (var kvp in genInfo.ResumeLabels)
                            {
                                if (kvp.Key > maxStateId) maxStateId = kvp.Key;
                            }

                            int branchCount = maxStateId + 1;
                            var switchTargets = new LabelHandle[branchCount];
                            for (int i = 0; i < branchCount; i++)
                            {
                                switchTargets[i] = defaultLabel;
                            }

                            foreach (var kvp in genInfo.ResumeLabels.Where(kvp => kvp.Key > 0 && kvp.Key < branchCount))
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

                            ilEncoder.MarkLabel(defaultLabel);
                        }

                        break;
                    }

                    // For async functions with awaits, we need special handling:
                    // - On initial call: create scope, build modified scopes array with scope at [0]
                    // - On resume: scopes array already has our scope at [0] (was built on initial call)
                    //
                    // We detect initial vs resume by checking if arg.0[0] is our scope type using isinst.
                    var asyncInfo = MethodBody.AsyncInfo;
                    if (MethodBody.IsAsync && asyncInfo != null && asyncInfo.HasAwaits)
                    {
                        var scopeTypeHandle = ResolveScopeTypeHandle(
                            createScope.Scope.Name,
                            "LIRCreateLeafScopeInstance instruction (async)");
                        var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                        var scopeName = createScope.Scope.Name;

                        // Check if scopes[0] is our scope type (isinst leaves typed ref or null)
                        // ldarg scopes, ldc.i4.0, ldelem.ref, isinst ScopeType
                        EmitLoadScopesArray(ilEncoder, methodDescriptor);
                        ilEncoder.LoadConstantI4(0);
                        ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                        ilEncoder.OpCode(ILOpCode.Isinst);
                        ilEncoder.Token(scopeTypeHandle);
                        ilEncoder.OpCode(ILOpCode.Dup); // duplicate for branch check

                        // brtrue -> resume path (use existing scope)
                        var resumeLabel = ilEncoder.DefineLabel();
                        var afterInitLabel = ilEncoder.DefineLabel();
                        ilEncoder.Branch(ILOpCode.Brtrue, resumeLabel);

                        // --- Initial path: create new scope and modified scopes array ---
                        ilEncoder.OpCode(ILOpCode.Pop); // pop the null from isinst

                        // Create new scope instance
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctorRef);
                        ilEncoder.StoreLocal(0);

                        EmitInitArgumentsObjectIfNeeded(ilEncoder, scopeName);

                        // Build modified scopes array using runtime helper:
                        // scopes = Promise.PrependScopeToArray(leafScope, scopes)
                        ilEncoder.LoadLocal(0);  // leafScope
                        EmitLoadScopesArray(ilEncoder, methodDescriptor); // parentScopes
                        var prependRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.PrependScopeToArray),
                            parameterTypes: new[] { typeof(object), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(prependRef);
                        ilEncoder.StoreArgument(GetIlArgIndexForScopesArray(methodDescriptor)); // scopes = modified scopes array

                        // Initialize _moveNext = Closure.Bind(delegate, scopesArray)
                        // Note: scopes now contains the modified scopes array with leaf scope at [0]
                        ilEncoder.LoadLocal(0);  // for stfld _moveNext later

                        var callableId = MethodBody.CallableId;
                        var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                        if (callableId != null && reader != null && reader.TryGetDeclaredToken(callableId, out var token) && token.Kind == HandleKind.MethodDefinition)
                        {
                            var methodHandle = (MethodDefinitionHandle)token;

                            // Create delegate: ldnull/ldthis, ldftn, newobj Func<object[], object, ...>::.ctor
                            // Use the JS parameter count so the delegate signature matches the async method.
                            int jsParamCount = callableId.JsParamCount;
                            if (methodDescriptor.IsStatic)
                            {
                                ilEncoder.OpCode(ILOpCode.Ldnull);
                            }
                            else
                            {
                                ilEncoder.LoadArgument(0);
                            }
                            ilEncoder.OpCode(ILOpCode.Ldftn);
                            ilEncoder.Token(methodHandle);
                            ilEncoder.OpCode(ILOpCode.Newobj);
                            ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                            // Load modified scopes array
                            var scopesArgIndex = GetIlArgIndexForScopesArray(methodDescriptor);
                            ilEncoder.LoadArgument(scopesArgIndex);

                            if (jsParamCount == 0)
                            {
                                // Call Closure.Bind(delegate, scopes)
                                var bindRef = _memberRefRegistry.GetOrAddMethod(
                                    typeof(JavaScriptRuntime.Closure),
                                    nameof(JavaScriptRuntime.Closure.Bind),
                                    parameterTypes: new[] { typeof(object), typeof(object[]) });
                                ilEncoder.OpCode(ILOpCode.Call);
                                ilEncoder.Token(bindRef);
                            }
                            else
                            {
                                // Call Closure.BindMoveNext(delegate, scopes, boundArgs)
                                // Build boundArgs = new object[jsParamCount] filled from method arguments (excluding scopes).
                                ilEncoder.LoadConstantI4(jsParamCount);
                                ilEncoder.OpCode(ILOpCode.Newarr);
                                ilEncoder.Token(_bclReferences.ObjectType);

                                var firstJsArgIndex = scopesArgIndex + 1;
                                for (var i = 0; i < jsParamCount; i++)
                                {
                                    ilEncoder.OpCode(ILOpCode.Dup);
                                    ilEncoder.LoadConstantI4(i);
                                    ilEncoder.LoadArgument(firstJsArgIndex + i);
                                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                                }

                                var bindMoveNextRef = _memberRefRegistry.GetOrAddMethod(
                                    typeof(JavaScriptRuntime.Closure),
                                    nameof(JavaScriptRuntime.Closure.BindMoveNext),
                                    parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });
                                ilEncoder.OpCode(ILOpCode.Call);
                                ilEncoder.Token(bindMoveNextRef);
                            }
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        EmitStoreFieldByName(ilEncoder, scopeName, "_moveNext");

                        ilEncoder.Branch(ILOpCode.Br, afterInitLabel);

                        // --- Resume path: use existing scope from scopes[0] ---
                        ilEncoder.MarkLabel(resumeLabel);
                        // Stack has the typed scope from isinst (dup'd before brtrue)
                        ilEncoder.StoreLocal(0);

                        ilEncoder.MarkLabel(afterInitLabel);

                        // Ensure we have persistent storage for variable locals and restore them on resumption.
                        // This is required because async MoveNext re-enters the method, so IL locals are re-initialized.
                        EmitEnsureAsyncLocalsArray(ilEncoder);

                        var skipRestoreLabel = ilEncoder.DefineLabel();
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_asyncState");
                        ilEncoder.LoadConstantI4(0);
                        ilEncoder.Branch(ILOpCode.Ble_s, skipRestoreLabel);
                        EmitRestoreVariableSlotsFromAsyncLocalsArray(ilEncoder);
                        ilEncoder.MarkLabel(skipRestoreLabel);

                        // Now emit the state switch to dispatch to resume points
                        // State 0 = initial entry (fall through)
                        // State 1, 2, ... = resume points after each await
                        if (asyncInfo.MaxResumeStateId > 0)
                        {
                            EmitAsyncStateSwitch(ilEncoder, labelMap, asyncInfo);
                        }
                    }
                    else if (MethodBody.IsGenerator)
                    {
                        // Generators: this method is both the factory (called as a JS function)
                        // and the step method (called by GeneratorObject on next/throw/return).
                        // Detect step calls by checking if scopes[0] is our leaf scope type.
                        if (!methodDescriptor.HasScopesParameter)
                        {
                            throw new InvalidOperationException("Generator methods must use the js2il ABI and declare a leading scopes parameter.");
                        }

                        int scopesArgIndex = GetIlArgIndexForScopesArray(methodDescriptor);

                        var scopeTypeHandle = ResolveScopeTypeHandle(
                            createScope.Scope.Name,
                            "LIRCreateLeafScopeInstance instruction (generator)");
                        var ctorRef = GetScopeConstructorRef(scopeTypeHandle);

                        // ldarg.0, ldc.i4.0, ldelem.ref, isinst ScopeType
                        ilEncoder.LoadArgument(scopesArgIndex);
                        ilEncoder.LoadConstantI4(0);
                        ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                        ilEncoder.OpCode(ILOpCode.Isinst);
                        ilEncoder.Token(scopeTypeHandle);
                        ilEncoder.OpCode(ILOpCode.Dup);

                        var stepLabel = ilEncoder.DefineLabel();
                        ilEncoder.Branch(ILOpCode.Brtrue, stepLabel);

                        // --- Factory path ---
                        ilEncoder.OpCode(ILOpCode.Pop); // pop null

                        // Create leaf scope instance
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctorRef);
                        ilEncoder.StoreLocal(0);

                        // Build modified scopes array with leaf at [0]
                        ilEncoder.LoadLocal(0); // leafScope
                        ilEncoder.LoadArgument(scopesArgIndex); // parent scopes
                        var prependRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.PrependScopeToArray),
                            parameterTypes: new[] { typeof(object), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(prependRef);
                        ilEncoder.StoreArgument(scopesArgIndex); // scopes = modified scopes array

                        // Build step delegate (to this method) and bind to the modified scopes array
                        var callableId = MethodBody.CallableId
                            ?? throw new InvalidOperationException("Generator method is missing CallableId.");
                        var reader = _serviceProvider.GetService<ICallableDeclarationReader>()
                            ?? throw new InvalidOperationException("ICallableDeclarationReader service is not available.");
                        if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                        {
                            throw new InvalidOperationException($"Failed to resolve MethodDefinitionHandle for generator callable {callableId}.");
                        }

                        var methodHandle = (MethodDefinitionHandle)token;
                        int jsParamCount = callableId.JsParamCount;

                        // ldnull, ldftn <method>, newobj FuncCtor
                        if (methodDescriptor.IsStatic)
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        else
                        {
                            // Closed delegate for instance generator methods: capture 'this'
                            ilEncoder.OpCode(ILOpCode.Ldarg_0);
                        }
                        ilEncoder.OpCode(ILOpCode.Ldftn);
                        ilEncoder.Token(methodHandle);
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                        // Closure.Bind(delegate, scopes)
                        ilEncoder.LoadArgument(scopesArgIndex);
                        var bindRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            nameof(JavaScriptRuntime.Closure.Bind),
                            parameterTypes: new[] { typeof(object), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(bindRef);

                        // Push scopes array (ctor arg 2)
                        ilEncoder.LoadArgument(scopesArgIndex);

                        // Build args array (ctor arg 3)
                        ilEncoder.LoadConstantI4(jsParamCount);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);
                        for (int i = 0; i < jsParamCount; i++)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(i);
                            ilEncoder.LoadArgument(GetIlArgIndexForJsParameter(methodDescriptor, i));
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }

                        // new GeneratorObject(step, scopes, args)
                        var genObjCtor = _memberRefRegistry.GetOrAddConstructor(
                            typeof(JavaScriptRuntime.GeneratorObject),
                            parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(genObjCtor);
                        ilEncoder.OpCode(ILOpCode.Ret);

                        // --- Step path ---
                        ilEncoder.MarkLabel(stepLabel);
                        ilEncoder.StoreLocal(0);
                    }
                    else
                    {
                        // Non-async or async without awaits: just create new scope
                        var scopeTypeHandle = ResolveScopeTypeHandle(
                            createScope.Scope.Name,
                            "LIRCreateLeafScopeInstance instruction");
                        var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                        var scopeName = createScope.Scope.Name;
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctorRef);
                        ilEncoder.StoreLocal(0);

                        EmitInitArgumentsObjectIfNeeded(ilEncoder, scopeName);
                    }
                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
