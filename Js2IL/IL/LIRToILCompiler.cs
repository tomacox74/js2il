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
    private readonly IServiceProvider _serviceProvider;
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberRefRegistry;
    private readonly ScopeMetadataRegistry _scopeMetadataRegistry;
    private MethodBodyIR? _methodBody;
    private bool _compiled;

    /// <summary>
    /// Gets the method body, throwing if not yet set.
    /// </summary>
    private MethodBodyIR MethodBody => _methodBody ?? throw new InvalidOperationException("MethodBody has not been set.");

    public LIRToILCompiler(
        MetadataBuilder metadataBuilder,
        TypeReferenceRegistry typeReferenceRegistry,
        MemberReferenceRegistry memberReferenceRegistry,
        BaseClassLibraryReferences bclReferences,
        ScopeMetadataRegistry scopeMetadataRegistry,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberReferenceRegistry;
        _scopeMetadataRegistry = scopeMetadataRegistry;
    }

    #region Public API

    /// <summary>
    /// Two-phase API: compile a callable body to IL and return the resulting body metadata
    /// without emitting a MethodDef row. The MethodDef row is emitted later in a deterministic
    /// per-type order.
    /// </summary>
    public CompiledCallableBody? TryCompileCallableBody(
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        MethodDescriptor methodDescriptor,
        MethodBodyIR methodBody,
        MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (_compiled)
        {
            throw new InvalidOperationException("LIRToILCompiler can only compile a single method. Create a new instance for each method.");
        }
        _compiled = true;
        _methodBody = methodBody;

        var methodParameters = methodDescriptor.Parameters;

        // Build method signature
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
                        var typeRef = _typeReferenceRegistry.GetOrAdd(parameterDefinition.ParameterType!);
                        parameters.AddParameter().Type().Type(typeRef, false);
                    }
                }
            });
        var methodSig = _metadataBuilder.GetOrAddBlob(sigBuilder);

        // Compile body
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            return null;
        }

        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        if (methodDescriptor.IsStatic)
        {
            methodAttributes |= MethodAttributes.Static;
        }

        var result = new CompiledCallableBody
        {
            Callable = callable,
            MethodName = methodDescriptor.Name,
            ExpectedMethodDef = expectedMethodDef,
            Attributes = methodAttributes,
            Signature = methodSig,
            BodyOffset = bodyOffset,
            ParameterNames = methodParameters.Select(p => p.Name).ToArray()
        };
        result.Validate();
        return result;
    }

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

        // All temps start as "needs materialization". Stackify will mark which ones can stay on stack.
        var shouldMaterialize = new bool[MethodBody.Temps.Count];
        Array.Fill(shouldMaterialize, true);

        // Build map of temp → defining instruction for branch condition inlining
        var tempDefinitions = BranchConditionOptimizer.BuildTempDefinitionMap(MethodBody);

        // Mark comparison temps only used by branches as non-materialized
        BranchConditionOptimizer.MarkBranchOnlyComparisonTemps(MethodBody, shouldMaterialize, tempDefinitions);

        // Stackify analysis: identify temps that can stay on the stack
        var stackifyResult = Stackify.Analyze(MethodBody);
        MarkStackifiableTemps(stackifyResult, shouldMaterialize);

        var allocation = TempLocalAllocator.Allocate(MethodBody, shouldMaterialize);

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
                if (!IsMaterialized(concatStrings.Result, allocation))
                {
                    // Stackify will re-emit concat inline at the single use site.
                    break;
                }
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
            case LIRCallInstanceMethod callInstance:
                EmitInstanceMethodCall(callInstance, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStatic callIntrinsicStatic:
                EmitIntrinsicStaticCall(callIntrinsicStatic, ilEncoder, allocation, methodDescriptor);
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

            case LIRConvertToNumber convertToNumber:
                if (!IsMaterialized(convertToNumber.Result, allocation))
                {
                    break;
                }

                // Convert boxed/object value to JS number (double) using runtime coercion.
                EmitLoadTempAsObject(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                {
                    var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toNumberMref);
                }
                EmitStoreTemp(convertToNumber.Result, ilEncoder, allocation);
                break;

            case LIRConvertToBoolean convertToBoolean:
                if (!IsMaterialized(convertToBoolean.Result, allocation))
                {
                    break;
                }

                EmitConvertToBooleanCore(convertToBoolean.Source, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToBoolean.Result, ilEncoder, allocation);
                break;

            case LIRConvertToString convertToString:
                if (!IsMaterialized(convertToString.Result, allocation))
                {
                    break;
                }

                EmitConvertToStringCore(convertToString.Source, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToString.Result, ilEncoder, allocation);
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
            case LIRLogicalNot logicalNot:
                if (!IsMaterialized(logicalNot.Result, allocation))
                {
                    break;
                }

                EmitLoadTempAsObject(logicalNot.Value, ilEncoder, allocation, methodDescriptor);
                {
                    var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toBooleanMref);
                }
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(logicalNot.Result, ilEncoder, allocation);
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
            
            // Division
            case LIRDivNumber divNumber:
                EmitLoadTemp(divNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(divNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Div);
                EmitStoreTemp(divNumber.Result, ilEncoder, allocation);
                break;

            // Remainder (modulo)
            case LIRModNumber modNumber:
                EmitLoadTemp(modNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(modNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Rem);
                EmitStoreTemp(modNumber.Result, ilEncoder, allocation);
                break;

            // Exponentiation (Math.Pow)
            case LIRExpNumber expNumber:
                EmitLoadTemp(expNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(expNumber.Right, ilEncoder, allocation, methodDescriptor);
                EmitMathPow(ilEncoder);
                EmitStoreTemp(expNumber.Result, ilEncoder, allocation);
                break;

            // Bitwise AND: convert to int32, and, convert back to double
            case LIRBitwiseAnd bitwiseAnd:
                EmitLoadTemp(bitwiseAnd.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTemp(bitwiseAnd.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.And);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseAnd.Result, ilEncoder, allocation);
                break;

            // Bitwise OR: convert to int32, or, convert back to double
            case LIRBitwiseOr bitwiseOr:
                EmitLoadTemp(bitwiseOr.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTemp(bitwiseOr.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Or);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseOr.Result, ilEncoder, allocation);
                break;

            // Bitwise XOR: convert to int32, xor, convert back to double
            case LIRBitwiseXor bitwiseXor:
                EmitLoadTemp(bitwiseXor.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTemp(bitwiseXor.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Xor);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(bitwiseXor.Result, ilEncoder, allocation);
                break;

            // Left shift: convert to int32, shift, convert back to double
            case LIRLeftShift leftShift:
                EmitLoadTemp(leftShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTemp(leftShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shl);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(leftShift.Result, ilEncoder, allocation);
                break;

            // Right shift (signed): convert to int32, shift, convert back to double
            case LIRRightShift rightShift:
                EmitLoadTemp(rightShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                EmitLoadTemp(rightShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shr);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(rightShift.Result, ilEncoder, allocation);
                break;

            // Unsigned right shift: convert to int32 (to preserve negative values), reinterpret as uint32, shift, convert back to double
            case LIRUnsignedRightShift unsignedRightShift:
                EmitLoadTemp(unsignedRightShift.Left, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);  // Convert to int32 first (handles negatives)
                ilEncoder.OpCode(ILOpCode.Conv_u4);  // Then reinterpret as uint32 (no value change, just type)
                EmitLoadTemp(unsignedRightShift.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Shr_un);
                ilEncoder.OpCode(ILOpCode.Conv_r_un); // Convert unsigned to double
                EmitStoreTemp(unsignedRightShift.Result, ilEncoder, allocation);
                break;

            // Call Operators.IsTruthy
            case LIRCallIsTruthy callIsTruthy:
                if (!IsMaterialized(callIsTruthy.Result, allocation))
                {
                    break;
                }
                EmitLoadTemp(callIsTruthy.Value, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsIsTruthy(ilEncoder);
                EmitStoreTemp(callIsTruthy.Result, ilEncoder, allocation);
                break;

            // Copy temp variable
            case LIRCopyTemp copyTemp:
                EmitLoadTemp(copyTemp.Source, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(copyTemp.Destination, ilEncoder, allocation);
                break;

            case LIRStoreException storeException:
                // Exception object is on stack at catch handler entry.
                EmitStoreTemp(storeException.Result, ilEncoder, allocation);
                break;

            case LIRUnwrapCatchException unwrapCatch:
                {
                    // Unwrap CLR exception to JS catch value.
                    // Stack discipline: ensure stack is empty on all paths.
                    var isThrownValue = ilEncoder.DefineLabel();
                    var isJsError = ilEncoder.DefineLabel();
                    var done = ilEncoder.DefineLabel();

                    var thrownType = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsThrownValueException));
                    var errorType = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Error));

                    // Load exception (object)
                    EmitLoadTemp(unwrapCatch.Exception, ilEncoder, allocation, methodDescriptor);

                    // dup; isinst JsThrownValueException
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(thrownType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, isThrownValue);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // dup; isinst Error
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(errorType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, isJsError);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // Unknown exception: discard original and rethrow.
                    ilEncoder.OpCode(ILOpCode.Pop);
                    ilEncoder.OpCode(ILOpCode.Rethrow);

                    ilEncoder.MarkLabel(isThrownValue);
                    // Stack: ex, (JsThrownValueException)
                    ilEncoder.OpCode(ILOpCode.Pop); // pop ex
                    var getValue = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.JsThrownValueException),
                        "get_Value");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getValue);
                    EmitStoreTemp(unwrapCatch.Result, ilEncoder, allocation);
                    ilEncoder.Branch(ILOpCode.Br, done);

                    ilEncoder.MarkLabel(isJsError);
                    // Stack: ex, (Error)
                    ilEncoder.OpCode(ILOpCode.Pop); // pop ex
                    EmitStoreTemp(unwrapCatch.Result, ilEncoder, allocation);
                    ilEncoder.MarkLabel(done);
                    break;
                }

            case LIRThrow throwInstr:
                {
                    // Throw JS value: if already a CLR Exception, throw it; otherwise wrap.
                    var throwException = ilEncoder.DefineLabel();
                    var exceptionType = _typeReferenceRegistry.GetOrAdd(typeof(System.Exception));
                    var wrapperCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.JsThrownValueException),
                        parameterTypes: new[] { typeof(object) });

                    EmitLoadTempAsObject(throwInstr.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(exceptionType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, throwException);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // Wrap and throw.
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(wrapperCtor);
                    ilEncoder.OpCode(ILOpCode.Throw);

                    ilEncoder.MarkLabel(throwException);
                    // Stack: value, exception
                    ilEncoder.OpCode(ILOpCode.Pop); // pop original value
                    ilEncoder.OpCode(ILOpCode.Throw);
                    break;
                }

            case LIRThrowNewTypeError throwTypeError:
                {
                    // throw new JavaScriptRuntime.TypeError(message)
                    var ctor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.TypeError),
                        parameterTypes: new[] { typeof(string) });
                    ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(throwTypeError.Message));
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctor);
                    ilEncoder.OpCode(ILOpCode.Throw);
                    break;
                }

            case LIRNewBuiltInError newError:
                {
                    if (!IsMaterialized(newError.Result, allocation))
                    {
                        break;
                    }

                    var errorClrType = Js2IL.IR.BuiltInErrorTypes.GetRuntimeErrorClrType(newError.ErrorTypeName);

                    if (newError.Message.HasValue)
                    {
                        // JS: Error(message) stringifies message.
                        EmitLoadTempAsObject(newError.Message.Value, ilEncoder, allocation, methodDescriptor);
                        var toString = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.DotNet2JSConversions),
                            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toString);

                        var ctor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: new[] { typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctor);
                        EmitStoreTemp(newError.Result, ilEncoder, allocation);
                        break;
                    }

                    var defaultCtor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(defaultCtor);
                    EmitStoreTemp(newError.Result, ilEncoder, allocation);
                    break;
                }

            case LIRNewIntrinsicObject newIntrinsic:
                {
                    if (!IsMaterialized(newIntrinsic.Result, allocation))
                    {
                        break;
                    }

                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);
                    EmitStoreTemp(newIntrinsic.Result, ilEncoder, allocation);
                    break;
                }

            case LIRNewUserClass newUserClass:
                {
                    // Resolve constructor token via CallableRegistry (two-phase declared tokens)
                    var registry = _serviceProvider.GetService<CallableRegistry>();
                    if (registry == null)
                    {
                        return false;
                    }

                    if (!registry.TryGetDeclaredTokenForAstNode(newUserClass.ConstructorNode, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var ctorDef = (MethodDefinitionHandle)token;

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount || argc > newUserClass.MaxArgCount)
                    {
                        var expectedMinArgs = newUserClass.MinArgCount;
                        var expectedMaxArgs = newUserClass.MaxArgCount;

                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            ILEmitHelpers.ThrowNotSupported(
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.",
                                newUserClass.ConstructorNode);
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.",
                            newUserClass.ConstructorNode);
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
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);

                    if (IsMaterialized(newUserClass.Result, allocation))
                    {
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            // 'in' operator - calls Operators.In
            case LIRInOperator inOp:
                EmitLoadTemp(inOp.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(inOp.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsIn(ilEncoder);
                EmitStoreTemp(inOp.Result, ilEncoder, allocation);
                break;

            // Dynamic equality - calls Operators.Equal
            case LIREqualDynamic equalDynamic:
                EmitLoadTemp(equalDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(equalDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsEqual(ilEncoder);
                EmitStoreTemp(equalDynamic.Result, ilEncoder, allocation);
                break;

            // Dynamic inequality - calls Operators.NotEqual
            case LIRNotEqualDynamic notEqualDynamic:
                EmitLoadTemp(notEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(notEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsNotEqual(ilEncoder);
                EmitStoreTemp(notEqualDynamic.Result, ilEncoder, allocation);
                break;

            // Dynamic strict equality - calls Operators.StrictEqual
            case LIRStrictEqualDynamic strictEqualDynamic:
                EmitLoadTemp(strictEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(strictEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictEqual(ilEncoder);
                EmitStoreTemp(strictEqualDynamic.Result, ilEncoder, allocation);
                break;

            // Dynamic strict inequality - calls Operators.StrictNotEqual
            case LIRStrictNotEqualDynamic strictNotEqualDynamic:
                EmitLoadTemp(strictNotEqualDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(strictNotEqualDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsStrictNotEqual(ilEncoder);
                EmitStoreTemp(strictNotEqualDynamic.Result, ilEncoder, allocation);
                break;

            case LIRBuildArray buildArray:
                {
                    if (!IsMaterialized(buildArray.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        break;
                    }
                    
                    // Emit: newarr Object
                    ilEncoder.LoadConstantI4(buildArray.Elements.Count);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    
                    // For each element: dup, ldc.i4 index, load element value (boxed), stelem.ref
                    for (int i = 0; i < buildArray.Elements.Count; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadConstantI4(i);
                        EmitLoadTempAsObject(buildArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    }
                    
                    EmitStoreTemp(buildArray.Result, ilEncoder, allocation);
                    break;
                }
            case LIRNewJsArray newJsArray:
                {
                    if (!IsMaterialized(newJsArray.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        break;
                    }

                    // Emit: ldc.i4 capacity, newobj JavaScriptRuntime.Array::.ctor(int)
                    ilEncoder.LoadConstantI4(newJsArray.Elements.Count);
                    var arrayCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.Array),
                        parameterTypes: new[] { typeof(int) });
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(arrayCtor);

                    // For each element: dup, load element value (boxed), callvirt Add
                    var addMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "Add");
                    for (int i = 0; i < newJsArray.Elements.Count; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        EmitLoadTempAsObject(newJsArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(addMethod);
                    }

                    EmitStoreTemp(newJsArray.Result, ilEncoder, allocation);
                    break;
                }
            case LIRNewJsObject newJsObject:
                {
                    if (!IsMaterialized(newJsObject.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        break;
                    }

                    // Emit: newobj ExpandoObject::.ctor()
                    var expandoCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(System.Dynamic.ExpandoObject),
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(expandoCtor);

                    // For each property: dup, ldstr key, load value, callvirt IDictionary.set_Item
                    var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.IDictionary<string, object>),
                        "set_Item");
                    foreach (var prop in newJsObject.Properties)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Ldstr(_metadataBuilder, prop.Key);
                        EmitLoadTempAsObject(prop.Value, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(setItemMethod);
                    }

                    EmitStoreTemp(newJsObject.Result, ilEncoder, allocation);
                    break;
                }
            case LIRGetLength getLength:
                {
                    if (!IsMaterialized(getLength.Result, allocation))
                    {
                        break;
                    }

                    // Emit: call JavaScriptRuntime.Object.GetLength(object)
                    EmitLoadTempAsObject(getLength.Object, ilEncoder, allocation, methodDescriptor);
                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetLength),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getLengthMethod);
                    EmitStoreTemp(getLength.Result, ilEncoder, allocation);
                    break;
                }
            case LIRGetItem getItem:
                {
                    if (!IsMaterialized(getItem.Result, allocation))
                    {
                        break;
                    }

                    // Emit: call JavaScriptRuntime.Object.GetItem(object, object)
                    EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(getItem.Index, ilEncoder, allocation, methodDescriptor);
                    var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetItem),
                        parameterTypes: new[] { typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getItemMethod);
                    EmitStoreTemp(getItem.Result, ilEncoder, allocation);
                    break;
                }
            case LIRArrayPushRange arrayPushRange:
                {
                    // Emit: ldtemp target, ldtemp source, callvirt PushRange
                    EmitLoadTemp(arrayPushRange.TargetArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(arrayPushRange.SourceArray, ilEncoder, allocation, methodDescriptor);
                    var pushRangeMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        nameof(JavaScriptRuntime.Array.PushRange),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(pushRangeMethod);
                    break;
                }
            case LIRArrayAdd arrayAdd:
                {
                    // Emit: ldtemp target, ldtemp element, callvirt Add
                    EmitLoadTemp(arrayAdd.TargetArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(arrayAdd.Element, ilEncoder, allocation, methodDescriptor);
                    var addMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "Add");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(addMethod);
                    break;
                }
            case LIRReturn lirReturn:
                EmitLoadTemp(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ret);
                break;
            case LIRBuildScopesArray buildScopes:
                {
                    if (!IsMaterialized(buildScopes.Result, allocation))
                    {
                        break;
                    }
                    
                    if (buildScopes.Slots.Count == 0)
                    {
                        // Empty scopes array - create 1-element array with null for ABI compatibility
                        // (Functions always expect at least a 1-element array)
                        EmitEmptyScopesArray(ilEncoder);
                    }
                    else
                    {
                        EmitPopulateScopesArray(ilEncoder, buildScopes.Slots, methodDescriptor);
                    }
                    
                    EmitStoreTemp(buildScopes.Result, ilEncoder, allocation);
                    break;
                }
            case LIRLoadThis loadThis:
                {
                    if (!IsMaterialized(loadThis.Result, allocation))
                    {
                        break;
                    }

                    if (methodDescriptor.IsStatic)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    ilEncoder.LoadArgument(0);
                    EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
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
                    if (callFunc.CallableId is not { } callableId)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

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

            case LIRCreateBoundArrowFunction createArrow:
                {
                    if (!IsMaterialized(createArrow.Result, allocation))
                    {
                        break;
                    }

                    // Resolve CallableId from AST node and then the declared MethodDef token.
                    var registry = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.CallableRegistry>();
                    if (registry == null)
                    {
                        return false;
                    }

                    if (!registry.TryGetCallableIdForAstNode(createArrow.ArrowNode, out var callableId))
                    {
                        return false;
                    }

                    var reader = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    var jsParamCount = createArrow.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "Bind", new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);

                    EmitStoreTemp(createArrow.Result, ilEncoder, allocation);
                    break;
                }

            case LIRCreateBoundFunctionExpression createFunc:
                {
                    if (!IsMaterialized(createFunc.Result, allocation))
                    {
                        break;
                    }

                    var registry = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.CallableRegistry>();
                    if (registry == null)
                    {
                        return false;
                    }

                    if (!registry.TryGetCallableIdForAstNode(createFunc.FunctionNode, out var callableId))
                    {
                        return false;
                    }

                    var reader = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    var jsParamCount = createFunc.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "Bind", new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);

                    EmitStoreTemp(createFunc.Result, ilEncoder, allocation);
                    break;
                }
            case LIRLoadLeafScopeField loadLeafField:
                {
                    if (!IsMaterialized(loadLeafField.Result, allocation))
                    {
                        break;
                    }
                    
                    // Emit: ldloc.0 (scope instance), ldfld (field handle)
                    var fieldHandle = ResolveFieldHandle(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "LIRLoadLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);
                    EmitStoreTemp(loadLeafField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreLeafScopeField storeLeafField:
                {
                    // Emit: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
                    var fieldHandle = ResolveFieldHandle(
                        storeLeafField.Field.ScopeName,
                        storeLeafField.Field.FieldName,
                        "LIRStoreLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    EmitLoadTemp(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }
            case LIRLoadParentScopeField loadParentField:
                {
                    if (!IsMaterialized(loadParentField.Result, allocation))
                    {
                        break;
                    }
                    
                    // Emit IL to load parent scope field
                    // For static methods with scopes parameter: ldarg.0 (scopes array)
                    // For instance methods: ldarg.0 (this), ldfld _scopes
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadParentField.Scope.Name,
                        "LIRLoadParentScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldHandle(
                        loadParentField.Field.ScopeName,
                        loadParentField.Field.FieldName,
                        "LIRLoadParentScopeField instruction");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(loadParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);
                    EmitStoreTemp(loadParentField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreParentScopeField storeParentField:
                {
                    // Emit IL to store to parent scope field
                    // For static methods with scopes parameter: ldarg.0 (scopes array)
                    // For instance methods: ldarg.0 (this), ldfld _scopes
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        storeParentField.Scope.Name,
                        "LIRStoreParentScopeField instruction (castclass)");
                    var fieldHandle = ResolveFieldHandle(
                        storeParentField.Field.ScopeName,
                        storeParentField.Field.FieldName,
                        "LIRStoreParentScopeField instruction");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(storeParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    EmitLoadTemp(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }
            case LIRCreateLeafScopeInstance createScope:
                {
                    // Emit: newobj instance void ScopeType::.ctor(), stloc.0
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        createScope.Scope.Name,
                        "LIRCreateLeafScopeInstance instruction");
                    var ctorRef = GetScopeConstructorRef(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorRef);
                    ilEncoder.StoreLocal(0); // Scope instance is always in local 0
                    break;
                }
            default:
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a member reference handle for the default constructor of a scope type.
    /// </summary>
    private MemberReferenceHandle GetScopeConstructorRef(TypeDefinitionHandle scopeType)
    {
        // The scope constructor is a parameterless instance method
        // Signature: void .ctor()
        var ctorSignature = new BlobBuilder();
        new BlobEncoder(ctorSignature)
            .MethodSignature(SignatureCallingConvention.Default, 0, isInstanceMethod: true)
            .Parameters(0, returnType => returnType.Void(), parameters => { });

        return _metadataBuilder.AddMemberReference(
            scopeType,
            _metadataBuilder.GetOrAddString(".ctor"),
            _metadataBuilder.GetOrAddBlob(ctorSignature));
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
        int varCount = MethodBody.VariableNames.Count;
        int tempLocals = allocation.SlotStorages.Count;
        bool hasLeafScope = MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil;
        
        // If we need a leaf scope local, it goes in slot 0
        int scopeLocalCount = hasLeafScope ? 1 : 0;
        int totalLocals = scopeLocalCount + varCount + tempLocals;

        if (totalLocals == 0)
        {
            return default;
        }

        var localSig = new BlobBuilder();
        var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(totalLocals);

        // Local 0: Scope instance (if needed)
        if (hasLeafScope)
        {
            var typeEncoder = localEncoder.AddVariable().Type();
            var leafScopeTypeHandle = ResolveScopeTypeHandle(
                MethodBody.LeafScopeId.Name,
                "local variable signature creation (leaf scope local)");
            typeEncoder.Type(leafScopeTypeHandle, false);
        }

        // Variable locals (shifted by scope local count)
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
            case LIRLoadThis:
                if (methodDescriptor.IsStatic)
                {
                    throw new InvalidOperationException("Cannot emit 'this' in a static method");
                }
                ilEncoder.LoadArgument(0);
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

            case LIRNewBuiltInError newError:
                {
                    var errorClrType = Js2IL.IR.BuiltInErrorTypes.GetRuntimeErrorClrType(newError.ErrorTypeName);

                    if (newError.Message.HasValue)
                    {
                        EmitLoadTempAsObject(newError.Message.Value, ilEncoder, allocation, methodDescriptor);
                        var toString = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.DotNet2JSConversions),
                            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toString);
                        var ctor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: new[] { typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctor);
                        break;
                    }

                    var defaultCtor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(defaultCtor);
                    break;
                }
            case LIRConvertToNumber convertToNumber:
                // Emit inline: load as object, call TypeUtilities.ToNumber(object)
                EmitLoadTempAsObject(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                {
                    var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toNumberMref);
                }
                break;

            case LIRConvertToBoolean convertToBoolean:
                EmitConvertToBooleanCore(convertToBoolean.Source, ilEncoder, allocation, methodDescriptor);
                break;

            case LIRConvertToString convertToString:
                EmitConvertToStringCore(convertToString.Source, ilEncoder, allocation, methodDescriptor);
                break;

            case LIRConcatStrings concatStrings:
                EmitLoadTemp(concatStrings.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(concatStrings.Right, ilEncoder, allocation, methodDescriptor);
                EmitStringConcat(ilEncoder);
                break;

            case LIRNewIntrinsicObject newIntrinsic:
                {
                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRNewUserClass newUserClass:
                {
                    // Emit inline user-defined class construction (newobj) with optional scopes array.
                    // Constructor token is resolved via CallableRegistry (two-phase declared tokens).
                    var registry = _serviceProvider.GetService<CallableRegistry>();
                    if (registry == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing CallableRegistry");
                    }

                    if (!registry.TryGetDeclaredTokenForAstNode(newUserClass.ConstructorNode, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared constructor token for class {newUserClass.ClassName}");
                    }

                    var ctorDef = (MethodDefinitionHandle)token;

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount || argc > newUserClass.MaxArgCount)
                    {
                        var expectedMinArgs = newUserClass.MinArgCount;
                        var expectedMaxArgs = newUserClass.MaxArgCount;

                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            ILEmitHelpers.ThrowNotSupported(
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.",
                                newUserClass.ConstructorNode);
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.",
                            newUserClass.ConstructorNode);
                    }

                    if (newUserClass.NeedsScopes)
                    {
                        if (newUserClass.ScopesArray is not { } scopesTemp)
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing scopes array temp for class {newUserClass.ClassName}");
                        }
                        EmitLoadTemp(scopesTemp, ilEncoder, allocation, methodDescriptor);
                    }

                    foreach (var arg in newUserClass.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argc;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);
                    // Result stays on stack
                    break;
                }

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
            case LIRLoadLeafScopeField loadLeafField:
                // Emit inline: ldloc.0 (scope instance), ldfld (field handle)
                {
                    var fieldHandle = ResolveFieldHandle(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "inline LIRLoadLeafScopeField emission");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);
                }
                break;
            case LIRLoadParentScopeField loadParentField:
                // Emit inline: load scopes array, index, cast, ldfld
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadParentField.Scope.Name,
                        "inline LIRLoadParentScopeField emission (castclass)");
                    var fieldHandle = ResolveFieldHandle(
                        loadParentField.Field.ScopeName,
                        loadParentField.Field.FieldName,
                        "inline LIRLoadParentScopeField emission");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(loadParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);
                }
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                // Emit inline: call IntrinsicObjectRegistry.GetOrDefault
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                break;
            case LIRBuildScopesArray buildScopes:
                // Emit inline: create scopes array with scope instances
                if (buildScopes.Slots.Count == 0)
                {
                    EmitEmptyScopesArray(ilEncoder);
                }
                else
                {
                    EmitPopulateScopesArray(ilEncoder, buildScopes.Slots, methodDescriptor);
                }
                // Array reference stays on stack
                break;
            case LIRBitwiseNotNumber bitwiseNot:
                // Emit inline: load value, convert to int, bitwise not, convert back to double
                EmitLoadTemp(bitwiseNot.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                break;
            case LIRLogicalNot logicalNot:
                // Emit inline: load as object, call ToBoolean, invert
                EmitLoadTempAsObject(logicalNot.Value, ilEncoder, allocation, methodDescriptor);
                {
                    var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toBooleanMref);
                }
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRTypeof typeofInstr:
                // Emit inline: load value, call TypeUtilities.Typeof
                EmitLoadTemp(typeofInstr.Value, ilEncoder, allocation, methodDescriptor);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                break;
            case LIRBuildArray buildArray:
                // Emit inline array construction using dup pattern
                ilEncoder.LoadConstantI4(buildArray.Elements.Count);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                for (int i = 0; i < buildArray.Elements.Count; i++)
                {
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(i);
                    EmitLoadTempAsObject(buildArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                }
                // Array reference stays on stack
                break;
            case LIRNewJsArray newJsArray:
                {
                    // Emit inline JavaScriptRuntime.Array construction using dup pattern
                    ilEncoder.LoadConstantI4(newJsArray.Elements.Count);
                    var arrayCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.Array),
                        parameterTypes: new[] { typeof(int) });
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(arrayCtor);

                    // For each element: dup, load element value, callvirt Add
                    var addMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "Add");
                    for (int i = 0; i < newJsArray.Elements.Count; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        EmitLoadTemp(newJsArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(addMethod);
                    }
                    // Array reference stays on stack
                }
                break;
            case LIRNewJsObject newJsObject:
                {
                    // Emit inline ExpandoObject construction
                    var expandoCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(System.Dynamic.ExpandoObject),
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(expandoCtor);

                    // For each property: dup, ldstr key, load value, callvirt IDictionary.set_Item
                    var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.IDictionary<string, object>),
                        "set_Item");
                    foreach (var prop in newJsObject.Properties)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Ldstr(_metadataBuilder, prop.Key);
                        EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(setItemMethod);
                    }
                    // Object reference stays on stack
                }
                break;
            case LIRGetLength getLength:
                // Emit inline: call JavaScriptRuntime.Object.GetLength(object)
                EmitLoadTempAsObject(getLength.Object, ilEncoder, allocation, methodDescriptor);
                {
                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetLength),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getLengthMethod);
                }
                break;
            case LIRGetItem getItem:
                // Emit inline: call JavaScriptRuntime.Object.GetItem(object, object)
                EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                EmitLoadTempAsObject(getItem.Index, ilEncoder, allocation, methodDescriptor);
                {
                    var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.GetItem),
                        parameterTypes: new[] { typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getItemMethod);
                }
                break;
            case LIRCallIntrinsic callIntrinsic:
                // Emit inline intrinsic call (e.g., console.log)
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodDescriptor);
                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);
                // Result stays on stack (caller will handle it)
                break;
            case LIRCallInstanceMethod callInstance:
                // Emit inline instance method call - result stays on stack
                EmitInstanceMethodCallInline(callInstance, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStatic callIntrinsicStatic:
                // Emit inline intrinsic static call (e.g., Array.isArray)
                // We reuse the main EmitIntrinsicStaticCall but need to handle unmaterialized result
                EmitIntrinsicStaticCallInline(callIntrinsicStatic, ilEncoder, allocation, methodDescriptor);
                // Result stays on stack (caller will handle it)
                break;
            case LIRNegateNumber negateNumber:
                // Emit inline: load value, negate
                EmitLoadTemp(negateNumber.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Neg);
                break;
            case LIRCallFunction callFunc:
                {
                    if (callFunc.CallableId is not { } callableId)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing CallableId for LIRCallFunction");
                    }

                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

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
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundArrowFunction createArrow:
                {
                    var registry = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.CallableRegistry>();
                    if (registry == null || !registry.TryGetCallableIdForAstNode(createArrow.ArrowNode, out var callableId))
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing CallableId for arrow function AST node");
                    }

                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createArrow.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "Bind", new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundFunctionExpression createFunc:
                {
                    var registry = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.CallableRegistry>();
                    if (registry == null || !registry.TryGetCallableIdForAstNode(createFunc.FunctionNode, out var callableId))
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing CallableId for function expression AST node");
                    }

                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createFunc.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), "Bind", new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }
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

    /// <summary>
    /// Emits IL to load a temp value as an object reference.
    /// If the temp's storage is an unboxed value type, emits a box instruction.
    /// </summary>
    private void EmitLoadTempAsObject(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        // Load the temp value
        EmitLoadTemp(temp, ilEncoder, allocation, methodDescriptor);

        // Check if boxing is needed based on storage type
        var storage = GetTempStorage(temp);
        if (storage.Kind == ValueStorageKind.UnboxedValue)
        {
            ilEncoder.OpCode(ILOpCode.Box);
            if (storage.ClrType == typeof(double))
            {
                ilEncoder.Token(_bclReferences.DoubleType);
            }
            else if (storage.ClrType == typeof(bool))
            {
                ilEncoder.Token(_bclReferences.BooleanType);
            }
            else if (storage.ClrType == typeof(JavaScriptRuntime.JsNull))
            {
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
            }
            else
            {
                // Default to double for unknown numeric types
                ilEncoder.Token(_bclReferences.DoubleType);
            }
        }
    }

    private void EmitConvertToBooleanCore(TempVariable source, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        EmitLoadTempAsObject(source, ilEncoder, allocation, methodDescriptor);
        var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toBooleanMref);
    }

    private void EmitConvertToStringCore(TempVariable source, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        EmitLoadTempAsObject(source, ilEncoder, allocation, methodDescriptor);
        var toStringMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.DotNet2JSConversions),
            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toStringMref);
    }

    private void EmitNewIntrinsicObjectCore(LIRNewIntrinsicObject newIntrinsic, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(newIntrinsic.IntrinsicName)
            ?? throw new InvalidOperationException($"Unknown intrinsic type: {newIntrinsic.IntrinsicName}");

        bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
        if (isStaticClass)
        {
            throw new InvalidOperationException($"Intrinsic '{newIntrinsic.IntrinsicName}' is not constructible (static class). ");
        }

        var argc = newIntrinsic.Arguments.Count;
        ConstructorInfo? chosenCtor = argc switch
        {
            0 => intrinsicType.GetConstructor(Type.EmptyTypes),
            1 => intrinsicType.GetConstructor(new[] { typeof(object) }),
            2 => intrinsicType.GetConstructor(new[] { typeof(object), typeof(object) }),
            _ => null
        };

        if (chosenCtor == null)
        {
            throw new InvalidOperationException(
                $"No matching intrinsic constructor found: {intrinsicType.FullName} with {argc} argument(s)");
        }

        foreach (var arg in newIntrinsic.Arguments)
        {
            EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
        }

        var ctorParamTypes = chosenCtor.GetParameters().Select(p => p.ParameterType).ToArray();
        var ctorRef = _memberRefRegistry.GetOrAddConstructor(intrinsicType, ctorParamTypes);
        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(ctorRef);
    }

    /// <summary>
    /// Gets the storage type for a temp variable.
    /// </summary>
    private ValueStorage GetTempStorage(TempVariable temp)
    {
        if (temp.Index >= 0 && temp.Index < MethodBody.TempStorages.Count)
        {
            return MethodBody.TempStorages[temp.Index];
        }
        return new ValueStorage(ValueStorageKind.Unknown);
    }

    /// <summary>
    /// Emits IL to load the scopes array onto the stack.
    /// For static methods with scopes parameter: ldarg.0 (scopes array is first parameter)
    /// For instance methods: ldarg.0 (this), ldfld _scopes (scopes stored in instance field)
    /// </summary>
    private void EmitLoadScopesArray(InstructionEncoder ilEncoder, MethodDescriptor methodDescriptor)
    {
        if (methodDescriptor.IsStatic && methodDescriptor.HasScopesParameter)
        {
            // Static function with scopes parameter - scopes is arg 0
            ilEncoder.LoadArgument(0);
        }
        else if (!methodDescriptor.IsStatic && methodDescriptor.ScopesFieldHandle.HasValue)
        {
            // Instance method with _scopes field
            // ldarg.0 (this), ldfld _scopes
            ilEncoder.LoadArgument(0);
            ilEncoder.OpCode(ILOpCode.Ldfld);
            ilEncoder.Token(methodDescriptor.ScopesFieldHandle.Value);
        }
        else
        {
            // Static method without scopes parameter (e.g., module Main) - shouldn't have parent scope access
            throw new InvalidOperationException("Cannot load scopes array - method has no scopes parameter and no _scopes field");
        }
    }

    /// <summary>
    /// Emits IL to load a scope instance from the specified source.
    /// </summary>
    private void EmitLoadScopeInstance(InstructionEncoder ilEncoder, ScopeSlotSource slotSource, MethodDescriptor methodDescriptor)
    {
        switch (slotSource.Source)
        {
            case ScopeInstanceSource.LeafLocal:
                // Load from local 0 (the leaf scope instance)
                ilEncoder.LoadLocal(0);
                break;

            case ScopeInstanceSource.ScopesArgument:
                // Load from scopes argument: ldarg.0 (scopes), ldc.i4 index, ldelem.ref
                if (!methodDescriptor.HasScopesParameter)
                {
                    throw new InvalidOperationException("Cannot load from ScopesArgument - method has no scopes parameter");
                }
                ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1); // scopes arg position
                ilEncoder.LoadConstantI4(slotSource.SourceIndex);
                ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                break;

            case ScopeInstanceSource.ThisScopes:
                // Load from this._scopes: ldarg.0 (this), ldfld _scopes, ldc.i4 index, ldelem.ref
                if (methodDescriptor.IsStatic || !methodDescriptor.ScopesFieldHandle.HasValue)
                {
                    throw new InvalidOperationException("Cannot load from ThisScopes - method is static or has no _scopes field");
                }
                ilEncoder.LoadArgument(0); // this
                ilEncoder.OpCode(ILOpCode.Ldfld);
                ilEncoder.Token(methodDescriptor.ScopesFieldHandle.Value);
                ilEncoder.LoadConstantI4(slotSource.SourceIndex);
                ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                break;

            default:
                throw new ArgumentException($"Unknown ScopeInstanceSource: {slotSource.Source}");
        }
    }

    /// <summary>
    /// Emits IL to create a 1-element scopes array with null.
    /// Used for ABI compatibility when callee doesn't need scopes.
    /// </summary>
    private void EmitEmptyScopesArray(InstructionEncoder ilEncoder)
    {
        ilEncoder.LoadConstantI4(1);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        ilEncoder.OpCode(ILOpCode.Dup);
        ilEncoder.LoadConstantI4(0);
        ilEncoder.OpCode(ILOpCode.Ldnull);
        ilEncoder.OpCode(ILOpCode.Stelem_ref);
    }

    /// <summary>
    /// Emits IL to create and populate a scopes array with scope instances.
    /// </summary>
    private void EmitPopulateScopesArray(InstructionEncoder ilEncoder, IReadOnlyList<ScopeSlotSource> slots, MethodDescriptor methodDescriptor)
    {
        // Create array with proper size
        ilEncoder.LoadConstantI4(slots.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        
        // Populate each slot
        foreach (var slotSource in slots)
        {
            ilEncoder.OpCode(ILOpCode.Dup); // Keep array reference for next stelem
            ilEncoder.LoadConstantI4(slotSource.Slot.Index);
            
            // Load the scope instance from the appropriate source
            EmitLoadScopeInstance(ilEncoder, slotSource, methodDescriptor);
            
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    private int GetSlotForTemp(TempVariable temp, TempLocalAllocation allocation)
    {
        // Calculate offset for scope local (if present)
        int scopeLocalOffset = (MethodBody.NeedsLeafScopeLocal && !MethodBody.LeafScopeId.IsNil) ? 1 : 0;
        
        // Variable-mapped temps always go to their stable variable slot (after scope local).
        if (temp.Index >= 0 && temp.Index < MethodBody.TempVariableSlots.Count)
        {
            int varSlot = MethodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                return scopeLocalOffset + varSlot;
            }
        }

        // Other temps go after variable locals (and scope local).
        var slot = allocation.GetSlot(temp);
        return scopeLocalOffset + MethodBody.VariableNames.Count + slot;
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

    private void EmitInstanceMethodCall(
        LIRCallInstanceMethod instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Resolve the instance method using heuristics aligned with intrinsic static calls.
        // Prefer object[] signature (variadic JS-style), else exact arity match with object parameters.
        var receiverType = instruction.ReceiverClrType;

        var allMethods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var methods = allMethods
            .Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var argCount = instruction.Arguments.Count;

        var chosen = methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
        });

        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == argCount && ps.All(p => p.ParameterType == typeof(object));
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching instance method found: {receiverType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        // Load receiver
        EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(receiverType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodRef);

        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            if (chosen.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    private void EmitInstanceMethodCallInline(
        LIRCallInstanceMethod instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var receiverType = instruction.ReceiverClrType;

        var allMethods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var methods = allMethods
            .Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var argCount = instruction.Arguments.Count;

        var chosen = methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
        });

        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == argCount && ps.All(p => p.ParameterType == typeof(object));
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching instance method found: {receiverType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(receiverType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodRef);
    }

    private void EmitObjectArrayFromTemps(
        IReadOnlyList<TempVariable> args,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        ilEncoder.LoadConstantI4(args.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);

        for (int i = 0; i < args.Count; i++)
        {
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(i);
            EmitLoadTempAsObject(args[i], ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    /// <summary>
    /// Emits a static method call on an intrinsic type (e.g., Array.isArray, Math.abs).
    /// Uses the same method resolution strategy as the legacy pipeline.
    /// </summary>
    private void EmitIntrinsicStaticCall(
        LIRCallIntrinsicStatic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

        // Resolve the static method using the same heuristics as the legacy pipeline:
        // 1. Exact arity match first
        // 2. Fallback to params object[] signature
        var allMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        var methods = allMethods.Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase)).ToList();

        var argCount = instruction.Arguments.Count;
        var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == argCount);
        if (chosen == null)
        {
            // Try params object[] signature
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching static method found: {intrinsicType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            // Build an object[] array with all arguments
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Load each argument directly (boxing handled if needed based on target parameter type)
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        // Emit the static call
        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);

        // Store or pop result
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            // If the method returns void, don't pop
            if (chosen.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    /// <summary>
    /// Emits intrinsic static call for inline (unmaterialized) temps - leaves result on stack.
    /// </summary>
    private void EmitIntrinsicStaticCallInline(
        LIRCallIntrinsicStatic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

        // Resolve the static method (same logic as EmitIntrinsicStaticCall)
        var allMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        var methods = allMethods.Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase)).ToList();

        var argCount = instruction.Arguments.Count;
        var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == argCount);
        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching static method found: {intrinsicType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            // Build an object[] array with all arguments
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Load each argument directly (boxing handled if needed based on target parameter type)
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        // Emit the static call - result stays on stack
        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
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

    private void EmitMathPow(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(System.Math), "Pow");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthy(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "IsTruthy");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIn(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "In");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "Equal");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "NotEqual");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "StrictEqual");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), "StrictNotEqual");
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    #endregion

    #region Handle Resolution Helpers

    /// <summary>
    /// Resolves a scope type handle from the registry with improved error context.
    /// </summary>
    private TypeDefinitionHandle ResolveScopeTypeHandle(string scopeName, string context)
    {
        try
        {
            return _scopeMetadataRegistry.GetScopeTypeHandle(scopeName);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve scope type handle for '{scopeName}' during {context}.",
                ex);
        }
    }

    /// <summary>
    /// Resolves a field handle from the registry with improved error context.
    /// </summary>
    private FieldDefinitionHandle ResolveFieldHandle(string scopeName, string fieldName, string context)
    {
        try
        {
            return _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve field handle for '{fieldName}' in scope '{scopeName}' during {context}.",
                ex);
        }
    }

    #endregion
}
