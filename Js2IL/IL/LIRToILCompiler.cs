using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Utilities.Ecma335;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

/// <summary>
/// Compiles LIR (Low-level IR) to IL bytecode.
/// </summary>
/// <remarks>
/// This class is responsible for the final stage of the JS to IL compilation pipeline:
/// LIR -> IL.
/// This class is designed for single-use; create a new instance for each method compilation.
/// </remarks>
internal sealed class LIRToILCompiler
{
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberRefRegistry;
    private readonly ConsoleLogPeepholeOptimizer _consoleLogOptimizer;
    private readonly CompiledMethodCache _compiledMethodCache;
    private MethodBodyIR? _methodBody;
    private bool _compiled;

    // Flag to enable/disable console.log peephole optimization.
    // TODO(#211): Remove this flag once Stackify can fully replace ConsoleLogPeepholeOptimizer.
    // Set to false to test Stackify in isolation (currently causes 152 test failures due to
    // missing inline emission support for some instruction types).
    private const bool EnableConsoleLogPeephole = true;

    /// <summary>
    /// Gets the method body, throwing if not yet set.
    /// </summary>
    private MethodBodyIR MethodBody => _methodBody ?? throw new InvalidOperationException("MethodBody has not been set.");

    public LIRToILCompiler(
        MetadataBuilder metadataBuilder,
        TypeReferenceRegistry typeReferenceRegistry,
        MemberReferenceRegistry memberReferenceRegistry,
        BaseClassLibraryReferences bclReferences,
        CompiledMethodCache compiledMethodCache)
    {
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberReferenceRegistry;
        _compiledMethodCache = compiledMethodCache;
        _consoleLogOptimizer = new ConsoleLogPeepholeOptimizer(metadataBuilder, bclReferences, memberReferenceRegistry, typeReferenceRegistry);
    }

    #region Public API

    public MethodDefinitionHandle TryCompile(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        var (methodDef, _) = TryCompileWithSignature(methodDescriptor, methodBody, methodBodyStreamEncoder);
        return methodDef;
    }

    public (MethodDefinitionHandle MethodDef, BlobHandle Signature) TryCompileWithSignature(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (_compiled)
        {
            throw new InvalidOperationException("LIRToILCompiler can only compile a single method. Create a new instance for each method.");
        }
        _compiled = true;
        _methodBody = methodBody;

        var programTypeBuilder = methodDescriptor.TypeBuilder;
        var methodParameters = methodDescriptor.Parameters;

        // Create the method signature for the Main method with parameters
        var sigBuilder = new BlobBuilder();
        new BlobEncoder(sigBuilder)
            .MethodSignature(isInstanceMethod: !methodDescriptor.IsStatic)
            .Parameters(methodParameters.Count, returnType =>
            {
                if (methodDescriptor.ReturnsVoid)
                    returnType.Void();
                else
                    returnType.Type().Object();
            }, parameters =>
            {
                for (int i = 0; i < methodParameters.Count; i++)
                {
                    var parameterDefinition = methodParameters[i];

                    if (parameterDefinition.ParameterType == typeof(object))
                    {
                        parameters.AddParameter().Type().Object();
                    }
                    else if (parameterDefinition.ParameterType == typeof(string))
                    {
                        parameters.AddParameter().Type().String();
                    }
                    else if (parameterDefinition.ParameterType.IsArray && parameterDefinition.ParameterType.GetElementType() == typeof(object))
                    {
                        parameters.AddParameter().Type().SZArray().Object();
                    }
                    else
                    {
                        // Assume it's a type reference
                        var typeRef = _typeReferenceRegistry.GetOrAdd(parameterDefinition.ParameterType!);
                        parameters.AddParameter().Type().Type(typeRef, false);
                    }
                }
            });
        var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

        // Compile the method body to IL
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            // Failed to compile IL
            return default;
        }

        var parameterNames = methodParameters.Select(p => p.Name).ToArray();

        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        if (methodDescriptor.IsStatic)
        {
            methodAttributes |= MethodAttributes.Static;
        }

        var methodDefinitionHandle = programTypeBuilder.AddMethodDefinition(
            methodAttributes,
            methodDescriptor.Name,
            methodSig,
            bodyOffset);

        // Add parameter names to metadata (sequence starts at 1 for first parameter)
        int sequence = 1;
        foreach (var paramName in parameterNames)
        {
            _metadataBuilder.AddParameter(
                ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(paramName),
                sequence++);
        }

        return (methodDefinitionHandle, methodSig);
    }

    #endregion

    #region Method Body Compilation

    private bool TryCompileMethodBodyToIL(MethodDescriptor methodDescriptor, MethodBodyStreamEncoder methodBodyStreamEncoder, out int bodyOffset)
    {
        bodyOffset = -1;
        var methodBlob = new BlobBuilder();
        var controlFlowBuilder = new ControlFlowBuilder();
        var ilEncoder = new InstructionEncoder(methodBlob, controlFlowBuilder);

        // Pre-pass: find console.log(oneArg) sequences that we will emit stack-only, and avoid
        // allocating IL locals for temps that are only used within those sequences.
        var peepholeReplaced = EnableConsoleLogPeephole 
            ? _consoleLogOptimizer.ComputeStackOnlyMask(MethodBody)
            : new bool[MethodBody.Temps.Count];

        // Build map of temp → defining instruction for branch condition inlining
        var tempDefinitions = BranchConditionOptimizer.BuildTempDefinitionMap(MethodBody);

        // Mark comparison temps only used by branches as non-materialized
        BranchConditionOptimizer.MarkBranchOnlyComparisonTemps(MethodBody, peepholeReplaced, tempDefinitions);

        // Stackify analysis: identify temps that can stay on the stack
        var stackifyResult = Stackify.Analyze(MethodBody);
        MarkStackifiableTemps(stackifyResult, peepholeReplaced);

        var allocation = TempLocalAllocator.Allocate(MethodBody, peepholeReplaced);

        // Pre-create IL labels for all LIR labels
        var labelMap = new Dictionary<int, LabelHandle>();
        foreach (var lirLabel in MethodBody.Instructions
            .OfType<LIRLabel>()
            .Where(l => !labelMap.ContainsKey(l.LabelId)))
        {
            labelMap[lirLabel.LabelId] = ilEncoder.DefineLabel();
        }

        bool hasExplicitReturn = false;
        for (int i = 0; i < MethodBody.Instructions.Count; i++)
        {
            // Peephole: console.log(<singleArg>) emitted stack-only
            if (EnableConsoleLogPeephole && _consoleLogOptimizer.TryEmitPeephole(
                MethodBody, i, ilEncoder, allocation,
                IsMaterialized,
                EmitStoreTemp,
                methodDescriptor.HasScopesParameter,
                !methodDescriptor.IsStatic,
                out var consumed))
            {
                i += consumed - 1;
                continue;
            }

            var instruction = MethodBody.Instructions[i];

            // Handle control flow instructions directly
            switch (instruction)
            {
                case LIRLabel lirLabel:
                    ilEncoder.MarkLabel(labelMap[lirLabel.LabelId]);
                    continue;

                case LIRBranch branch:
                    ilEncoder.Branch(ILOpCode.Br, labelMap[branch.TargetLabel]);
                    continue;

                case LIRBranchIfFalse branchFalse:
                    EmitBranchCondition(branchFalse.Condition, ilEncoder, allocation, tempDefinitions, methodDescriptor);
                    ilEncoder.Branch(ILOpCode.Brfalse, labelMap[branchFalse.TargetLabel]);
                    continue;

                case LIRBranchIfTrue branchTrue:
                    EmitBranchCondition(branchTrue.Condition, ilEncoder, allocation, tempDefinitions, methodDescriptor);
                    ilEncoder.Branch(ILOpCode.Brtrue, labelMap[branchTrue.TargetLabel]);
                    continue;
            }

            if (!TryCompileInstructionToIL(instruction, ilEncoder, allocation, methodDescriptor))
            {
                // Failed to compile instruction
                IRPipelineMetrics.RecordFailure($"IL compile failed: unsupported LIR instruction {instruction.GetType().Name}");
                return false;
            }
            if (instruction is LIRReturn)
            {
                hasExplicitReturn = true;
            }
        }

        // Only emit implicit return if no explicit return was found
        if (!hasExplicitReturn)
        {
            if (!methodDescriptor.ReturnsVoid)
            {
                if (methodDescriptor.IsStatic)
                {
                    // For static methods implicit return is undefined (null in dotnet)
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                }
                else
                {
                    // For instance methods, load 'this' reference
                    ilEncoder.OpCode(ILOpCode.Ldarg_0);
                }
            }
            ilEncoder.OpCode(ILOpCode.Ret);
        }

        var localVariablesSignature = CreateLocalVariablesSignature(allocation);

        var bodyAttributes = MethodBodyAttributes.None;
        if (MethodBody.VariableNames.Count > 0 || allocation.SlotStorages.Count > 0)
        {
            bodyAttributes |= MethodBodyAttributes.InitLocals;
        }

        bodyOffset = methodBodyStreamEncoder.AddMethodBody(
                ilEncoder,
                maxStack: 32,
                localVariablesSignature: localVariablesSignature,
                attributes: bodyAttributes);

        return true;
    }

    #endregion

    #region Instruction Emission

    private bool TryCompileInstructionToIL(LIRInstruction instruction, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRAddNumber addNumber:
                EmitLoadTemp(addNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Add);
                EmitStoreTemp(addNumber.Result, ilEncoder, allocation);
                break;
            case LIRConcatStrings concatStrings:
                EmitLoadTemp(concatStrings.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(concatStrings.Right, ilEncoder, allocation, methodDescriptor);
                EmitStringConcat(ilEncoder);
                EmitStoreTemp(concatStrings.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamic addDynamic:
                EmitLoadTemp(addDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAdd(ilEncoder);
                EmitStoreTemp(addDynamic.Result, ilEncoder, allocation);
                break;
            case LIRSubNumber subNumber:
                EmitLoadTemp(subNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(subNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Sub);
                EmitStoreTemp(subNumber.Result, ilEncoder, allocation);
                break;
            case LIRMulNumber mulNumber:
                EmitLoadTemp(mulNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Mul);
                EmitStoreTemp(mulNumber.Result, ilEncoder, allocation);
                break;
            case LIRMulDynamic mulDynamic:
                EmitLoadTemp(mulDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsMultiply(ilEncoder);
                EmitStoreTemp(mulDynamic.Result, ilEncoder, allocation);
                break;
            case LIRBeginInitArrayElement:
                // Pure SSA LIR lowering does not rely on stack tricks; this is a no-op hint.
                break;
            case LIRConstNumber constNumber:
                if (!IsMaterialized(constNumber.Result, allocation))
                {
                    break;
                }
                ilEncoder.LoadConstantR8(constNumber.Value);
                EmitStoreTemp(constNumber.Result, ilEncoder, allocation);
                break;
            case LIRConstString constString:
                if (!IsMaterialized(constString.Result, allocation))
                {
                    break;
                }
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constString.Value));
                EmitStoreTemp(constString.Result, ilEncoder, allocation);
                break;
            case LIRConstBoolean constBoolean:
                if (!IsMaterialized(constBoolean.Result, allocation))
                {
                    break;
                }
                ilEncoder.LoadConstantI4(constBoolean.Value ? 1 : 0);
                EmitStoreTemp(constBoolean.Result, ilEncoder, allocation);
                break;
            case LIRConstUndefined:
                if (!IsMaterialized(((LIRConstUndefined)instruction).Result, allocation))
                {
                    break;
                }
                ilEncoder.OpCode(ILOpCode.Ldnull);
                EmitStoreTemp(((LIRConstUndefined)instruction).Result, ilEncoder, allocation);
                break;
            case LIRConstNull:
                if (!IsMaterialized(((LIRConstNull)instruction).Result, allocation))
                {
                    break;
                }
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                EmitStoreTemp(((LIRConstNull)instruction).Result, ilEncoder, allocation);
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                if (!IsMaterialized(getIntrinsicGlobal.Result, allocation))
                {
                    break;
                }
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                EmitStoreTemp(getIntrinsicGlobal.Result, ilEncoder, allocation);
                break;
            case LIRCallIntrinsic callIntrinsic:
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodDescriptor);
                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);

                if (IsMaterialized(callIntrinsic.Result, allocation))
                {
                    EmitStoreTemp(callIntrinsic.Result, ilEncoder, allocation);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Pop);
                }
                break;
            case LIRConvertToObject convertToObject:
                if (!IsMaterialized(convertToObject.Result, allocation))
                {
                    break;
                }

                EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Box);
                if (convertToObject.SourceType == typeof(bool))
                {
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                else if (convertToObject.SourceType == typeof(JavaScriptRuntime.JsNull))
                {
                    ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                }
                else
                {
                    ilEncoder.Token(_bclReferences.DoubleType);
                }

                EmitStoreTemp(convertToObject.Result, ilEncoder, allocation);
                break;
            case LIRTypeof:
                if (!IsMaterialized(((LIRTypeof)instruction).Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(((LIRTypeof)instruction).Value, ilEncoder, allocation, methodDescriptor);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                EmitStoreTemp(((LIRTypeof)instruction).Result, ilEncoder, allocation);
                break;
            case LIRNegateNumber:
                if (!IsMaterialized(((LIRNegateNumber)instruction).Result, allocation))
                {
                    break;
                }

                EmitLoadTemp(((LIRNegateNumber)instruction).Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Neg);
                EmitStoreTemp(((LIRNegateNumber)instruction).Result, ilEncoder, allocation);
                break;
            case LIRBitwiseNotNumber:
                if (!IsMaterialized(((LIRBitwiseNotNumber)instruction).Result, allocation))
                {
                    break;
                }

                EmitLoadTemp(((LIRBitwiseNotNumber)instruction).Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(((LIRBitwiseNotNumber)instruction).Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberLessThan cmpLt:
                if (!IsMaterialized(cmpLt.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpLt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                EmitStoreTemp(cmpLt.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberGreaterThan cmpGt:
                if (!IsMaterialized(cmpGt.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpGt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                EmitStoreTemp(cmpGt.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberLessThanOrEqual cmpLe:
                if (!IsMaterialized(cmpLe.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpLe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpLe.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                if (!IsMaterialized(cmpGe.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpGe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpGe.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberEqual cmpEq:
                if (!IsMaterialized(cmpEq.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpEq.Result, ilEncoder, allocation);
                break;
            case LIRCompareNumberNotEqual cmpNe:
                if (!IsMaterialized(cmpNe.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpNe.Result, ilEncoder, allocation);
                break;
            case LIRCompareBooleanEqual cmpBoolEq:
                if (!IsMaterialized(cmpBoolEq.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolEq.Result, ilEncoder, allocation);
                break;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                if (!IsMaterialized(cmpBoolNe.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolNe.Result, ilEncoder, allocation);
                break;
            case LIRNewObjectArray newObjectArray:
                if (!IsMaterialized(newObjectArray.Result, allocation))
                {
                    break;
                }
                ilEncoder.LoadConstantI4(newObjectArray.ElementCount);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                EmitStoreTemp(newObjectArray.Result, ilEncoder, allocation);
                break;
            case LIRReturn lirReturn:
                EmitLoadTemp(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ret);
                break;
            case LIRStoreElementRef:
                {
                    var store = (LIRStoreElementRef)instruction;
                    EmitLoadTemp(store.Array, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.LoadConstantI4(store.Index);
                    EmitLoadTemp(store.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    break;
                }
            case LIRCreateScopesArray createScopes:
                {
                    if (!IsMaterialized(createScopes.Result, allocation))
                    {
                        break;
                    }
                    // Create scopes array with 1 element containing a null placeholder.
                    // The called function expects a scopes array but when no variables are
                    // captured, the IR pipeline doesn't create scope instances, so we pass null.
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    // Store null as the global scope placeholder
                    // (The function doesn't use scopes[0] in our test case)
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(0);
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    EmitStoreTemp(createScopes.Result, ilEncoder, allocation);
                    break;
                }
            case LIRLoadParameter loadParam:
                {
                    if (!IsMaterialized(loadParam.Result, allocation))
                    {
                        break;
                    }
                    // JS parameter index is 0-based. IL arg index depends on method type:
                    // - User functions (static): arg0 is scopes array, so JS param 0 -> IL arg 1
                    // - Instance methods: arg0 is 'this', so JS param 0 -> IL arg 1
                    // - Module Main (static, no scopes): JS param 0 -> IL arg 0
                    int ilArgIndex = (methodDescriptor.HasScopesParameter || !methodDescriptor.IsStatic)
                        ? loadParam.ParameterIndex + 1
                        : loadParam.ParameterIndex;
                    ilEncoder.LoadArgument(ilArgIndex);
                    EmitStoreTemp(loadParam.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreParameter storeParam:
                {
                    // JS parameter index is 0-based. IL arg index depends on method type:
                    // - User functions (static): arg0 is scopes array, so JS param 0 -> IL arg 1
                    // - Instance methods: arg0 is 'this', so JS param 0 -> IL arg 1
                    // - Module Main (static, no scopes): JS param 0 -> IL arg 0
                    int ilArgIndex = (methodDescriptor.HasScopesParameter || !methodDescriptor.IsStatic)
                        ? storeParam.ParameterIndex + 1
                        : storeParam.ParameterIndex;
                    EmitLoadTemp(storeParam.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.StoreArgument(ilArgIndex);
                    break;
                }
            case LIRCallFunction callFunc:
                {
                    // Look up the method handle from CompiledMethodCache
                    if (!_compiledMethodCache.TryGet(callFunc.FunctionSymbol.BindingInfo, out var methodHandle))
                    {
                        return false; // Fall back to legacy emitter
                    }

                    int jsParamCount = callFunc.Arguments.Count;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Load scopes array
                    EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Load all arguments
                    foreach (var arg in callFunc.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    // Invoke: callvirt Func<object[], [object, ...], object>::Invoke
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(_bclReferences.GetFuncInvokeRef(jsParamCount));

                    if (IsMaterialized(callFunc.Result, allocation))
                    {
                        EmitStoreTemp(callFunc.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }
            case LIRLoadLeafScopeField loadLeafField:
                {
                    if (!IsMaterialized(loadLeafField.Result, allocation))
                    {
                        break;
                    }
                    
                    // Emit: ldloc.0 (scope instance), ldfld (field handle)
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(loadLeafField.FieldHandle);
                    EmitStoreTemp(loadLeafField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreLeafScopeField storeLeafField:
                {
                    // Emit: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    EmitLoadTemp(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(storeLeafField.FieldHandle);
                    break;
                }
            case LIRLoadParentScopeField loadParentField:
                {
                    if (!IsMaterialized(loadParentField.Result, allocation))
                    {
                        break;
                    }
                    
                    // Emit: ldarg scopes, ldc.i4 index, ldelem.ref, castclass (scope type), ldfld (field handle)
                    // Scopes array is arg 0 for static functions with captured variables
                    ilEncoder.LoadArgument(0);  // TODO: This assumes scopes is always arg 0 - needs refinement
                    ilEncoder.LoadConstantI4(loadParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(loadParentField.ScopeType);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(loadParentField.FieldHandle);
                    EmitStoreTemp(loadParentField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreParentScopeField storeParentField:
                {
                    // Emit: ldarg scopes, ldc.i4 index, ldelem.ref, castclass (scope type), ldarg/ldloc Value, stfld (field handle)
                    ilEncoder.LoadArgument(0);  // TODO: This assumes scopes is always arg 0 - needs refinement
                    ilEncoder.LoadConstantI4(storeParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(storeParentField.ScopeType);
                    EmitLoadTemp(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(storeParentField.FieldHandle);
                    break;
                }
            default:
                return false;
        }

        return true;
    }

    #endregion

    #region Branch Condition Handling

    /// <summary>
    /// Emits the condition for a branch instruction. If the condition is a non-materialized
    /// comparison, emits the comparison inline. Otherwise loads the temp normally.
    /// </summary>
    private void EmitBranchCondition(
        TempVariable condition,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        Dictionary<int, LIRInstruction> tempDefinitions,
        MethodDescriptor methodDescriptor)
    {
        // Check if the condition is a non-materialized comparison that we should inline
        if (!IsMaterialized(condition, allocation) &&
            condition.Index >= 0 &&
            tempDefinitions.TryGetValue(condition.Index, out var definingInstruction) &&
            BranchConditionOptimizer.IsComparisonInstruction(definingInstruction))
        {
            // Emit the comparison inline without storing to a local
            BranchConditionOptimizer.EmitInlineComparison(
                definingInstruction,
                ilEncoder,
                (temp, encoder) => EmitLoadTemp(temp, encoder, allocation, methodDescriptor));
        }
        else
        {
            // Load the condition from its local normally
            EmitLoadTemp(condition, ilEncoder, allocation, methodDescriptor);
        }
    }

    #endregion

    #region Temp/Local Variable Management

    private StandaloneSignatureHandle CreateLocalVariablesSignature(TempLocalAllocation allocation)
    {
        if (MethodBody.VariableNames.Count == 0 && allocation.SlotStorages.Count == 0)
        {
            return default;
        }

        int varCount = MethodBody.VariableNames.Count;
        int totalLocals = varCount + allocation.SlotStorages.Count;

        var localSig = new BlobBuilder();
        var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(totalLocals);

        // Variable locals first
        for (int i = 0; i < varCount; i++)
        {
            var typeEncoder = localEncoder.AddVariable().Type();

            if (i < MethodBody.VariableStorages.Count)
            {
                var storage = MethodBody.VariableStorages[i];
                if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
                {
                    typeEncoder.Boolean();
                }
                else if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
                {
                    typeEncoder.Double();
                }
                else if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
                {
                    typeEncoder.String();
                }
                else
                {
                    typeEncoder.Object();
                }
            }
            else
            {
                typeEncoder.Double();
            }
        }

        // Then temp locals
        for (int i = 0; i < allocation.SlotStorages.Count; i++)
        {
            var storage = allocation.SlotStorages[i];
            var typeEncoder = localEncoder.AddVariable().Type();

            if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
            {
                typeEncoder.Double();
            }
            else if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
            {
                typeEncoder.Boolean();
            }
            else if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(JavaScriptRuntime.JsNull))
            {
                var typeRef = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull));
                typeEncoder.Type(typeRef, false);
            }
            else if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
            {
                typeEncoder.String();
            }
            else if (storage.Kind == ValueStorageKind.Reference && storage.ClrType != null && storage.ClrType.IsArray && storage.ClrType.GetElementType() == typeof(object))
            {
                typeEncoder.SZArray().Object();
            }
            else if (storage.Kind == ValueStorageKind.Reference && storage.ClrType != null && storage.ClrType != typeof(object))
            {
                var typeRef = _typeReferenceRegistry.GetOrAdd(storage.ClrType);
                typeEncoder.Type(typeRef, false);
            }
            else
            {
                typeEncoder.Object();
            }
        }

        var signature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
        return signature;
    }

    private void EmitLoadTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        // Check if materialized - if so, load from local
        if (IsMaterialized(temp, allocation))
        {
            var slot = GetSlotForTemp(temp, allocation);
            ilEncoder.LoadLocal(slot);
            return;
        }

        // Not materialized - try to emit inline
        var def = TryFindDefInstruction(temp);
        if (def == null)
        {
            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - no definition found");
        }

        // Emit the constant/expression inline
        switch (def)
        {
            case LIRConstNumber constNum:
                ilEncoder.LoadConstantR8(constNum.Value);
                break;
            case LIRConstString constStr:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constStr.Value));
                break;
            case LIRConstBoolean constBool:
                ilEncoder.LoadConstantI4(constBool.Value ? 1 : 0);
                break;
            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                break;
            case LIRConstNull:
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                break;
            case LIRLoadParameter loadParam:
                // Emit ldarg.X inline - no local slot needed
                // For instance methods, arg0 is 'this', so JS param 0 -> IL arg 1
                int ilArgIndex = (methodDescriptor.HasScopesParameter || !methodDescriptor.IsStatic)
                    ? loadParam.ParameterIndex + 1
                    : loadParam.ParameterIndex;
                ilEncoder.LoadArgument(ilArgIndex);
                break;
            case LIRConvertToObject convertToObject:
                // Emit the source inline and box it
                EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Box);
                if (convertToObject.SourceType == typeof(bool))
                {
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                else if (convertToObject.SourceType == typeof(JavaScriptRuntime.JsNull))
                {
                    ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                }
                else
                {
                    ilEncoder.Token(_bclReferences.DoubleType);
                }
                break;
            case LIRMulDynamic mulDynamic:
                // Emit inline dynamic multiplication
                EmitLoadTemp(mulDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsMultiply(ilEncoder);
                break;
            case LIRAddDynamic addDynamic:
                // Emit inline dynamic addition
                EmitLoadTemp(addDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAdd(ilEncoder);
                break;
            default:
                throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - unsupported instruction {def.GetType().Name}");
        }
    }

    private bool IsMaterialized(TempVariable temp, TempLocalAllocation allocation)
    {
        // Variable-mapped temps always materialize into their stable variable local slot.
        if (temp.Index >= 0 &&
            temp.Index < MethodBody.TempVariableSlots.Count &&
            MethodBody.TempVariableSlots[temp.Index] >= 0)
        {
            return true;
        }

        return allocation.IsMaterialized(temp);
    }

    private void EmitStoreTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation)
    {
        if (!IsMaterialized(temp, allocation))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
            return;
        }

        var slot = GetSlotForTemp(temp, allocation);
        ilEncoder.StoreLocal(slot);
    }

    private int GetSlotForTemp(TempVariable temp, TempLocalAllocation allocation)
    {
        // Variable-mapped temps always go to their stable variable slot.
        if (temp.Index >= 0 && temp.Index < MethodBody.TempVariableSlots.Count)
        {
            int varSlot = MethodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                return varSlot;
            }
        }

        // Other temps go after variable locals.
        var slot = allocation.GetSlot(temp);
        return MethodBody.VariableNames.Count + slot;
    }

    private LIRInstruction? TryFindDefInstruction(TempVariable temp)
    {
        foreach (var instr in MethodBody.Instructions
            .Where(i => TempLocalAllocator.TryGetDefinedTemp(i, out var defined) && defined == temp))
        {
            return instr;
        }
        return null;
    }

    /// <summary>
    /// Marks stackifiable temps as non-materialized in the peephole mask.
    /// This prevents TempLocalAllocator from allocating IL local slots for temps that can stay on the stack.
    /// </summary>
    private void MarkStackifiableTemps(StackifyResult stackifyResult, bool[]? shouldMaterializeTemp)
    {
        if (shouldMaterializeTemp == null || stackifyResult.CanStackify.Length == 0)
        {
            return;
        }

        for (int i = 0; i < Math.Min(stackifyResult.CanStackify.Length, shouldMaterializeTemp.Length); i++)
        {
            if (stackifyResult.CanStackify[i])
            {
                // This temp can stay on the stack - mark it as not needing materialization
                shouldMaterializeTemp[i] = false;
            }
        }
    }

    #endregion

    #region Intrinsic/Runtime Helpers

    /// <summary>
    /// Loads a value onto the the stack for a given intrinsic global variable.
    /// </summary>
    public void EmitLoadIntrinsicGlobalVariable(string variableName, InstructionEncoder ilEncoder)
    {
        var gvType = typeof(JavaScriptRuntime.GlobalThis);
        var gvProp = gvType.GetProperty(variableName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
        var getterDecl = gvProp?.GetMethod?.DeclaringType!;
        var getterMref = _memberRefRegistry.GetOrAddMethod(getterDecl!, gvProp!.GetMethod!.Name);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(getterMref);
    }

    public void EmitInvokeIntrinsicMethod(Type declaringType, string methodName, InstructionEncoder ilEncoder)
    {
        var methodMref = _memberRefRegistry.GetOrAddMethod(declaringType, methodName);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodMref);
    }

    private void EmitStringConcat(InstructionEncoder ilEncoder)
    {
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_bclReferences.String_Concat_Ref);
    }

    private void EmitOperatorsAdd(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Add");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsMultiply(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Multiply");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    #endregion
}
