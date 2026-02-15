using Js2IL.DebugSymbols;
using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Method Body Compilation

    private bool TryCompileMethodBodyToIL(MethodDescriptor methodDescriptor, MethodBodyStreamEncoder methodBodyStreamEncoder, out int bodyOffset)
    {
        bodyOffset = -1;
        var methodBlob = new BlobBuilder();
        var controlFlowBuilder = new ControlFlowBuilder();
        var ilEncoder = new InstructionEncoder(methodBlob, controlFlowBuilder);

        // Capture sequence points as (IL offset -> source span) markers.
        // These are later consumed by Portable PDB emission at the assembly writer layer.
        _sequencePoints = new List<MethodSequencePoint>();
        _localVariablesSignature = default;
        _ilLength = 0;
        _locals = null;

        // All temps start as "needs materialization". Stackify will mark which ones can stay on stack.
        var shouldMaterialize = new bool[MethodBody.Temps.Count];
        Array.Fill(shouldMaterialize, true);

        // Build map of temp ÃƒÆ’Ã†â€™Ãƒâ€šÃ‚Â¢ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬Ãƒâ€šÃ‚Â ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â‚¬Å¾Ã‚Â¢ defining instruction for branch condition inlining
        var tempDefinitions = BranchConditionOptimizer.BuildTempDefinitionMap(MethodBody);

        // Mark comparison temps only used by branches as non-materialized
        BranchConditionOptimizer.MarkBranchOnlyComparisonTemps(MethodBody, shouldMaterialize, tempDefinitions);

        // Stackify analysis: identify temps that can stay on the stack
        var stackifyResult = Stackify.Analyze(MethodBody);
        MarkStackifiableTemps(stackifyResult, shouldMaterialize);

        // Constructor return override (PL5.4a) requires post-construction logic that overwrites
        // the result temp. If a class constructor can return a value, force materialization for
        // `new C()` results so we have a stable local slot.
        var classRegistryForCtorReturn = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
        if (classRegistryForCtorReturn != null)
        {
            foreach (var instr in MethodBody.Instructions.OfType<LIRNewUserClass>())
            {
                if (instr.Result.Index >= 0
                    && instr.Result.Index < shouldMaterialize.Length
                    && classRegistryForCtorReturn.TryGetPrivateField(instr.RegistryClassName, "__js2il_ctorReturn", out _))
                {
                    shouldMaterialize[instr.Result.Index] = true;
                }
            }
        }

        var allocation = TempLocalAllocator.Allocate(MethodBody, shouldMaterialize);

        // Pre-create IL labels for all LIR labels
        var labelMap = new Dictionary<int, LabelHandle>();
        foreach (var lirLabel in MethodBody.Instructions
            .OfType<LIRLabel>()
            .Where(l => !labelMap.ContainsKey(l.LabelId)))
        {
            labelMap[lirLabel.LabelId] = ilEncoder.DefineLabel();
        }

        // For constructors, emit base System.Object::.ctor() call before any body instructions.
        // Derived constructors (class extends ...) must call the base constructor via `super(...)`.
        if (methodDescriptor.IsConstructor && !methodDescriptor.IsDerivedConstructor)
        {
            ilEncoder.OpCode(ILOpCode.Ldarg_0);
            ilEncoder.Call(_bclReferences.Object_Ctor_Ref);
        }

        // Opt-in prototype chain support: enable runtime behavior only when the compiler detected
        // prototype-related usage (or the user forced it via options). Emit this once per module init.
        if (string.Equals(methodDescriptor.Name, "__js_module_init__", StringComparison.Ordinal))
        {
            var options = _serviceProvider.GetService<CompilerOptions>();
            if (options?.PrototypeChainEnabled == true)
            {
                var enableProto = _memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.PrototypeChain),
                    nameof(JavaScriptRuntime.PrototypeChain.Enable),
                    parameterTypes: Type.EmptyTypes);

                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(enableProto);
            }
        }

        // NOTE: For async functions, the state switch is emitted AFTER LIRCreateLeafScopeInstance
        // because we need the scope instance to be in local 0 first (either newly created or
        // loaded from arg.0[0] on resume). The state switch is emitted inline with that instruction.

        for (int i = 0; i < MethodBody.Instructions.Count; i++)
        {
            var instruction = MethodBody.Instructions[i];

            // Peephole optimization: fuse `new C(...); this.<field> = <temp>` into a single sequence
            // `ldarg.0; newobj C(..); stfld <field>`.
            // This avoids materializing the freshly-constructed instance into an `object` local and
            // then immediately `castclass`-ing it back to the declared user-class type for `stfld`.
            //
            // Only apply when we can preserve JS semantics without requiring PL5.4a ctor return override handling.
            if (instruction is LIRNewUserClass newUserClass
                && i + 1 < MethodBody.Instructions.Count
                && MethodBody.Instructions[i + 1] is LIRStoreUserClassInstanceField storeInstanceField
                && storeInstanceField.Value.Equals(newUserClass.Result))
            {
                var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                var reader = _serviceProvider.GetService<ICallableDeclarationReader>();

                if (classRegistry != null
                    && reader != null
                    && (!classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__js2il_ctorReturn", out _))
                    && reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token)
                    && token.Kind == HandleKind.MethodDefinition)
                {
                    // Resolve the field handle.
                    FieldDefinitionHandle fieldHandle;
                    if (storeInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(storeInstanceField.RegistryClassName, storeInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(storeInstanceField.RegistryClassName, storeInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }

                    // Emit: ldarg.0
                    ilEncoder.LoadArgument(0);

                    // Emit the constructor call inline (leave the constructed instance on stack).
                    var ctorDef = (MethodDefinitionHandle)token;

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount || argc > newUserClass.MaxArgCount)
                    {
                        var expectedMinArgs = newUserClass.MinArgCount;
                        var expectedMaxArgs = newUserClass.MaxArgCount;

                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            ILEmitHelpers.ThrowNotSupported(
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.");
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.");
                    }

                    if (newUserClass.NeedsScopes)
                    {
                        if (newUserClass.ScopesArray is not { } scopesTemp)
                        {
                            return false;
                        }
                        EmitLoadTemp(scopesTemp, ilEncoder, allocation, methodDescriptor);
                    }

                    foreach (var arg in newUserClass.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argc;
                    for (int p = 0; p < paddingNeeded; p++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);

                    // If the field is declared as a specific user-class type and it matches the constructed type,
                    // omit the cast. Otherwise, preserve the cast to keep IL verification correct.
                    if (TryGetDeclaredUserClassFieldTypeHandle(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false,
                        out var declaredFieldTypeHandle)
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out var constructedTypeHandle)
                        && !declaredFieldTypeHandle.Equals(constructedTypeHandle))
                    {
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(declaredFieldTypeHandle);
                    }

                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);

                    // Skip the following LIRStoreUserClassInstanceField (we just emitted it).
                    i++;
                    continue;
                }
            }

            // Peephole optimization: fuse `new <Intrinsic>(...); this.<field> = <temp>` into a single sequence
            // `ldarg.0; newobj <Intrinsic>(..); stfld <field>`.
            // This avoids materializing the freshly-constructed intrinsic instance into an `object` local and
            // then immediately `castclass`-ing it back to the declared intrinsic type for `stfld`.
            if (instruction is LIRNewIntrinsicObject newIntrinsic
                && i + 1 < MethodBody.Instructions.Count
                && MethodBody.Instructions[i + 1] is LIRStoreUserClassInstanceField storeIntrinsicField
                && storeIntrinsicField.Value.Equals(newIntrinsic.Result))
            {
                var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                if (classRegistry != null)
                {
                    // Resolve the field handle.
                    FieldDefinitionHandle fieldHandle;
                    if (storeIntrinsicField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(storeIntrinsicField.RegistryClassName, storeIntrinsicField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(storeIntrinsicField.RegistryClassName, storeIntrinsicField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }

                    // Emit: ldarg.0
                    ilEncoder.LoadArgument(0);

                    // Emit the intrinsic constructor call inline (leave the constructed instance on stack).
                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);

                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);

                    // Skip the following LIRStoreUserClassInstanceField (we just emitted it).
                    i++;
                    continue;
                }
            }

            // Handle control flow instructions directly
            switch (instruction)
            {
                case LIRLabel lirLabel:
                    ilEncoder.MarkLabel(labelMap[lirLabel.LabelId]);
                    continue;

                case LIRSequencePoint sp:
                    // Marker emits no IL. Associate the next real IL instruction with this span.
                    // IL offset is the current method IL byte length.
                    _sequencePoints.Add(new MethodSequencePoint(methodBlob.Count, sp.Span));
                    continue;

                case LIRBranch branch:
                    ilEncoder.Branch(ILOpCode.Br, labelMap[branch.TargetLabel]);
                    continue;

                case LIRLeave leave:
                    ilEncoder.Branch(ILOpCode.Leave, labelMap[leave.TargetLabel]);
                    continue;

                case LIRBranchIfFalse branchFalse:
                    EmitBranchCondition(branchFalse.Condition, ilEncoder, allocation, tempDefinitions, methodDescriptor);
                    ilEncoder.Branch(ILOpCode.Brfalse, labelMap[branchFalse.TargetLabel]);
                    continue;

                case LIRBranchIfTrue branchTrue:
                    EmitBranchCondition(branchTrue.Condition, ilEncoder, allocation, tempDefinitions, methodDescriptor);
                    ilEncoder.Branch(ILOpCode.Brtrue, labelMap[branchTrue.TargetLabel]);
                    continue;

                case LIREndFinally:
                    ilEncoder.OpCode(ILOpCode.Endfinally);
                    continue;
            }

            if (!TryCompileInstructionToIL(instruction, ilEncoder, allocation, methodDescriptor, labelMap, stackifyResult))
            {
                // Failed to compile instruction
                IRPipelineMetrics.RecordFailureIfUnset($"IL compile failed: unsupported LIR instruction {instruction.GetType().Name}");
                return false;
            }
        }

        // Ensure the method body always ends with a return.
        // Even if there are explicit returns in some branches, a JS function body can still fall through.
        // If IR lowering didn't produce an explicit return on all paths, we must add a default return here.
        if (MethodBody.Instructions.Count == 0 || MethodBody.Instructions[^1] is not LIRReturn)
        {
            if (!methodDescriptor.ReturnsVoid)
            {
                if (MethodBody.IsAsync && MethodBody.IsGenerator)
                {
                    // Async generator fallthrough: mark done and resolve _deferred with { value: undefined, done: true }
                    var scopeName = MethodBody.LeafScopeId.Name;

                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(1);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(-1);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");

                    // Load _deferred.resolve
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                    var getResolveRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.PromiseWithResolvers),
                        $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.resolve)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getResolveRef);

                    var iterCreate = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.IteratorResult),
                        nameof(JavaScriptRuntime.IteratorResult.Create),
                        parameterTypes: new[] { typeof(object), typeof(bool) });

                    var invokeWithArgsRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        parameterTypes: new[] { typeof(object), typeof(object[]), typeof(object[]) });

                    // resolve(IteratorResult.Create(undefined, true))
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(0);
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(iterCreate);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeWithArgsRef);
                    ilEncoder.OpCode(ILOpCode.Pop);

                    // Return _deferred.promise
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                    var getPromiseRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.PromiseWithResolvers),
                        $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.promise)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getPromiseRef);
                }
                else if (MethodBody.IsGenerator)
                {
                    // Generator fallthrough: mark done and return { value: undefined, done: true }
                    var scopeName = MethodBody.LeafScopeId.Name;

                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(1);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.LoadConstantI4(1);
                    var iterCreate = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.IteratorResult),
                        nameof(JavaScriptRuntime.IteratorResult.Create),
                        parameterTypes: new[] { typeof(object), typeof(bool) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(iterCreate);
                }
                else if (MethodBody.IsAsync && MethodBody.AsyncInfo is { HasAwaits: true })
                {
                    // Full async state machine: resolve _deferred with undefined and return its promise.
                    var scopeName = MethodBody.LeafScopeId.Name;
                    
                    // _asyncState = -1 (completed)
                    ilEncoder.LoadLocal(0);
                    ilEncoder.LoadConstantI4(-1);
                    EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");
                    
                    // Load _deferred.resolve
                    ilEncoder.LoadLocal(0);
                    EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                    var getResolveRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.PromiseWithResolvers),
                        $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.resolve)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getResolveRef);
                    
                    // Call it with undefined: Closure.InvokeWithArgs(resolve, scopes, [null])
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    // Build 1-element array with null (undefined)
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    // Array is initialized to null by default, no need to explicitly set element
                    
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
                }
                else
                {
                    // JavaScript: functions/methods fallthrough returns 'undefined'
                    // (modeled as CLR null in our runtime).
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                }
            }
            ilEncoder.OpCode(ILOpCode.Ret);
        }

        // Register exception regions (try/catch/finally) for the method body.
        foreach (var region in MethodBody.ExceptionRegions)
        {
            switch (region.Kind)
            {
                case Js2IL.IR.ExceptionRegionKind.Catch:
                    {
                        var catchType = region.CatchType ?? typeof(System.Exception);
                        var catchTypeRef = _typeReferenceRegistry.GetOrAdd(catchType);
                        controlFlowBuilder.AddCatchRegion(
                            labelMap[region.TryStartLabelId],
                            labelMap[region.TryEndLabelId],
                            labelMap[region.HandlerStartLabelId],
                            labelMap[region.HandlerEndLabelId],
                            catchTypeRef);
                        break;
                    }
                case Js2IL.IR.ExceptionRegionKind.Finally:
                    controlFlowBuilder.AddFinallyRegion(
                        labelMap[region.TryStartLabelId],
                        labelMap[region.TryEndLabelId],
                        labelMap[region.HandlerStartLabelId],
                        labelMap[region.HandlerEndLabelId]);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported exception region kind: {region.Kind}");
            }
        }

        var localVariablesSignature = CreateLocalVariablesSignature(allocation);
        _localVariablesSignature = localVariablesSignature;

        // Emit local variable names for source-level variable slots (exclude leaf-scope local and temp locals).
        // IL local layout is:
        // - [0] leaf scope local (optional)
        // - [scopeOffset..scopeOffset+VariableNames.Count-1] source locals
        // - remaining locals are temps (unnamed)
        int scopeOffset = (MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil) ? 1 : 0;
        if (MethodBody.VariableNames.Count > 0)
        {
            _locals = MethodBody.VariableNames
                .Select((name, i) => new DebugSymbolRegistry.MethodLocal(scopeOffset + i, name))
                .ToArray();
        }

        var bodyAttributes = MethodBodyAttributes.None;
        if (MethodBody.VariableNames.Count > 0 || allocation.SlotStorages.Count > 0)
        {
            bodyAttributes |= MethodBodyAttributes.InitLocals;
        }

        // Default maxstack of 8 matches MethodBodyStreamEncoder's safe default and is
        // sufficient for many simple methods, but js2il emits inline array/object construction
        // (e.g. building object[] argument arrays containing object literals) that can exceed 8.
        // We estimate maxstack using LIR patterns and (when temps are not materialized) account
        // for the peak stack usage of inline temp emission.
        int maxStack = 8;

        // Build a quick map from temp -> defining instruction so we can estimate inlined temp emission.
        var defByTemp = new Dictionary<int, LIRInstruction>();
        for (int i = 0; i < MethodBody.Instructions.Count; i++)
        {
            var instr = MethodBody.Instructions[i];
            if (TempLocalAllocator.TryGetDefinedTemp(instr, out var defined) && defined.Index >= 0)
            {
                defByTemp[defined.Index] = instr;
            }
        }

        var tempLoadPeakCache = new Dictionary<int, int>();
        var tempConstructionPeakCache = new Dictionary<int, int>();

        int EstimateTempConstructionPeak(TempVariable temp)
        {
            if (temp.Index < 0)
            {
                return 1;
            }

            if (tempConstructionPeakCache.TryGetValue(temp.Index, out var cached))
            {
                return cached;
            }

            // Default: loading a simple value produces a single stack item.
            var peak = 1;

            if (defByTemp.TryGetValue(temp.Index, out var def))
            {
                peak = def switch
                {
                    // Inline object[] construction: dup + index + element
                    LIRBuildArray buildArray => 3 + (buildArray.Elements.Count == 0 ? 0 : buildArray.Elements.Max(EstimateTempConstructionPeak)),

                    // Inline scopes array construction: dup + index + scopeInstance
                    LIRBuildScopesArray buildScopes => 3 + (buildScopes.Slots.Count == 0 ? 0 : 1),

                    // Inline JavaScriptRuntime.Array construction: dup + element
                    LIRNewJsArray newJsArray => 2 + (newJsArray.Elements.Count == 0 ? 0 : newJsArray.Elements.Max(EstimateTempConstructionPeak)),

                    // Inline ExpandoObject/object literal: dup + key + value
                    LIRNewJsObject newJsObject => 3 + (newJsObject.Properties.Count == 0 ? 0 : newJsObject.Properties.Max(p => EstimateTempConstructionPeak(p.Value))),

                    // Inline item get: receiver + index
                    // Load order matters because receiver remains on stack while index is evaluated.
                    LIRGetItem getItem => Math.Max(
                        EstimateTempConstructionPeak(getItem.Object),
                        1 + EstimateTempConstructionPeak(getItem.Index)),

                    // Inline item set: receiver + index + value
                    // Load order matters because earlier values remain on stack while later ones are evaluated.
                    LIRSetItem setItem => new[]
                    {
                        EstimateTempConstructionPeak(setItem.Object),
                        1 + EstimateTempConstructionPeak(setItem.Index),
                        2 + EstimateTempConstructionPeak(setItem.Value)
                    }.Max(),

                    // Inline length get: receiver
                    LIRGetLength getLength => EstimateTempConstructionPeak(getLength.Object),

                    // Leaf scope/global loads/constants/etc.
                    LIRConstNumber or LIRConstString or LIRConstBoolean or LIRConstUndefined or LIRConstNull or LIRGetIntrinsicGlobal or LIRLoadParameter or LIRLoadThis
                        => 1,

                    _ => 1
                };
            }

            // Clamp to at least 1.
            if (peak < 1)
            {
                peak = 1;
            }

            tempConstructionPeakCache[temp.Index] = peak;
            return peak;
        }

        int EstimateTempLoadPeak(TempVariable temp)
        {
            if (temp.Index < 0)
            {
                return 1;
            }

            if (IsMaterialized(temp, allocation))
            {
                return 1;
            }

            if (tempLoadPeakCache.TryGetValue(temp.Index, out var cached))
            {
                return cached;
            }

            var peak = EstimateTempConstructionPeak(temp);
            tempLoadPeakCache[temp.Index] = peak;
            return peak;
        }

        foreach (var instr in MethodBody.Instructions)
        {
            int estimated = instr switch
            {
                // Direct user-defined function call:
                //   ldnull, ldftn, newobj (delegate) -> +1
                //   scopes + newTarget + declared JS params (including ldnull padding)
                // Peak stack before callvirt: delegate + scopes + newTarget + jsParamCount
                LIRCallFunction callFunction => 3 + (callFunction.CallableId?.JsParamCount ?? callFunction.Arguments.Count),

                // Direct user-defined function call with args array (e.g. spread calls):
                //   delegate + scopes + argsArray (argsArray may be inlined)
                LIRCallFunctionWithArgsArray callWithArgsArray => 2 + EstimateTempLoadPeak(callWithArgsArray.ArgumentsArray),

                // Function value call via runtime dispatch: target + scopesArray + argsArray (argsArray may be inlined)
                LIRCallFunctionValue callValue => 2 + EstimateTempLoadPeak(callValue.ArgumentsArray),

                // Member call via runtime dispatch: receiver + methodName + argsArray (argsArray may be inlined)
                LIRCallMember callMember => 2 + EstimateTempLoadPeak(callMember.ArgumentsArray),

                // Known CLR instance method calls: receiver + args (no padding)
                LIRCallInstanceMethod callInstance => 1 + callInstance.Arguments.Count,

                // Intrinsic static calls with a pre-built args array (argsArray may be inlined)
                LIRCallIntrinsicStaticWithArgsArray callStaticWithArgsArray => EstimateTempLoadPeak(callStaticWithArgsArray.ArgumentsArray),
                LIRCallIntrinsicStaticVoidWithArgsArray callStaticVoidWithArgsArray => EstimateTempLoadPeak(callStaticVoidWithArgsArray.ArgumentsArray),

                // Early-bound typed member calls: receiver + optional scopes + optional newTarget + declared JS params (including padding)
                LIRCallTypedMember callTypedMember => 1 + (callTypedMember.HasScopesParameter ? 2 : 0) + callTypedMember.MaxParamCount,

                // Typed member calls with fallback have an additional 'dup' on the typed receiver during the type-test.
                // Be conservative and account for 2 receivers on the stack at the peak.
                LIRCallTypedMemberWithFallback callTypedFallback => 2 + (callTypedFallback.HasScopesParameter ? 2 : 0) + callTypedFallback.MaxParamCount,

                // Direct user-class instance method calls: receiver + optional scopes + optional newTarget + declared JS params (including padding)
                LIRCallUserClassInstanceMethod callUserInstance => 1 + (callUserInstance.HasScopesParameter ? 2 : 0) + callUserInstance.MaxParamCount,
                LIRCallUserClassBaseConstructor callBaseCtor => 1 + (callBaseCtor.HasScopesParameter ? 1 : 0) + callBaseCtor.MaxParamCount,
                LIRCallUserClassBaseInstanceMethod callBaseInstance => 1 + (callBaseInstance.HasScopesParameter ? 2 : 0) + callBaseInstance.MaxParamCount,

                // Declared callable call: delegate instance + args list (already signature-shaped at call site).
                LIRCallDeclaredCallable callDeclared => 1 + callDeclared.Arguments.Count,

                // New user class: optional scopes + declared ctor param count.
                // Use the declared max arg count since emission pads missing args.
                LIRNewUserClass newUserClass => (newUserClass.NeedsScopes ? 1 : 0) + newUserClass.MaxArgCount,

                _ => 0
            };

            // Also account for deep stack usage inside the instruction that defines a temp
            // (e.g. nested object/array literals), regardless of whether the temp is materialized.
            if (TempLocalAllocator.TryGetDefinedTemp(instr, out var definedTemp) && definedTemp.Index >= 0)
            {
                var constructionPeak = EstimateTempConstructionPeak(definedTemp);
                if (constructionPeak > estimated)
                {
                    estimated = constructionPeak;
                }
            }

            if (estimated > maxStack)
            {
                maxStack = estimated;
            }
        }

        bodyOffset = methodBodyStreamEncoder.AddMethodBody(
            ilEncoder,
            maxStack: maxStack,
            localVariablesSignature: localVariablesSignature,
            attributes: bodyAttributes);

        // IL length for LocalScope ranges (relative to start of method IL stream).
        _ilLength = methodBlob.Count;

        return true;
    }

    #endregion
}