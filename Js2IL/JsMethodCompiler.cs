using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.SymbolTables;
using Js2IL.IR;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Utilities.Ecma335;
using Js2IL.Services;

namespace Js2IL;

sealed record MethodParameterDescriptor
{
    public MethodParameterDescriptor(string name, Type parameterType)
    {
        Name = name;
        ParameterType = parameterType;
    }

    public string Name { get; init; }
    public Type ParameterType { get; init; }
}

sealed record MethodDescriptor
{
    public MethodDescriptor(string name, TypeBuilder typeBuilder, IReadOnlyList<MethodParameterDescriptor> parameters)
    {
        Name = name;
        TypeBuilder = typeBuilder;
        Parameters = parameters;
    }

    public string Name { get; init; }
    public TypeBuilder TypeBuilder { get; init; }
    public IReadOnlyList<MethodParameterDescriptor> Parameters { get; set; }

    /// <summary>
    ///  Default is to return an object
    /// </summary>
    public bool ReturnsVoid { get; set; } = false;

    /// <summary>
    /// Only class instance methods are not static currently, so we default to static.
    /// </summary>
    public bool IsStatic {get; set; } = true;
}

/// <summary>
/// Per method compiling from JS to IL
/// </summary>
/// <remarks>
/// AST -> HIR -> LIR -> IL
/// </remarks>
internal sealed class JsMethodCompiler
{
    private readonly MetadataBuilder _metadataBuilder;
    private readonly TypeReferenceRegistry _typeReferenceRegistry;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MemberReferenceRegistry _memberRefRegistry;
    private readonly ConsoleLogPeepholeOptimizer _consoleLogOptimizer;

    public JsMethodCompiler(MetadataBuilder metadataBuilder, TypeReferenceRegistry typeReferenceRegistry, MemberReferenceRegistry memberReferenceRegistry, BaseClassLibraryReferences bclReferences)
    {
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberReferenceRegistry;
        _consoleLogOptimizer = new ConsoleLogPeepholeOptimizer(metadataBuilder, bclReferences, memberReferenceRegistry, typeReferenceRegistry);
    }

    #region Public API - Entry Points

    public MethodDefinitionHandle TryCompileMethod(TypeBuilder typeBuilder, string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (!TryLowerASTToLIR(node, scope, out var lirMethod))
        {
            return default;
        }

        var methodDescriptor = new MethodDescriptor(
            methodName,
            typeBuilder,
            [new MethodParameterDescriptor("scopes", typeof(object[]))]);

        if (node is Acornima.Ast.MethodDefinition methodDef)
        {
            methodDescriptor.IsStatic = methodDef.Static;
            methodDescriptor.Parameters = Array.Empty<MethodParameterDescriptor>();
        }

        return TryCompileIRToIL(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileArrowFunction(string methodName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (!TryLowerASTToLIR(node, scope, out var lirMethod))
        {
            return default;
        }

        // Create the type builder for the arrow function
        var arrowTypeBuilder = new TypeBuilder(_metadataBuilder, "Functions", methodName);

        var methodDescriptor = new MethodDescriptor(
            methodName,
            arrowTypeBuilder,
            [new MethodParameterDescriptor("scopes", typeof(object[]))]);

        var methodDefinitionHandle = TryCompileIRToIL(methodDescriptor, lirMethod!, methodBodyStreamEncoder);

        // Define the arrow function type
        arrowTypeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
            _bclReferences.ObjectType);

        return methodDefinitionHandle;
    }

    /// <summary>
    /// Attempts to compile a class constructor using the IR pipeline.
    /// Falls back to legacy emitter if IR compilation fails.
    /// Note: TypeBuilder is shared and managed by ClassesGenerator, not created here.
    /// </summary>
    /// <returns>A tuple of (MethodDefinitionHandle, BlobHandle signature), or default values if compilation fails.</returns>
    public (MethodDefinitionHandle MethodDef, BlobHandle Signature) TryCompileClassConstructor(
        TypeBuilder typeBuilder, 
        FunctionExpression ctorFunc, 
        Scope constructorScope, 
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        bool needsScopes)
    {
        // IR pipeline doesn't yet handle:
        // - Base constructor calls (required for all constructors)
        // - Field initializations
        // - Scope parameter storage
        // - Constructor parameters
        // Fall back to legacy emitter for all these cases
        if (needsScopes || ctorFunc.Params.Count > 0)
        {
            return default;
        }

        if (!TryLowerASTToLIR(ctorFunc, constructorScope, out var lirMethod))
        {
            return default;
        }

        // For constructors: instance method, returns void, no parameters for now
        var methodDescriptor = new MethodDescriptor(
            ".ctor",
            typeBuilder,
            Array.Empty<MethodParameterDescriptor>());

        methodDescriptor.IsStatic = false;
        methodDescriptor.ReturnsVoid = true;

        // Note: This won't produce valid constructor IL yet because we need:
        // 1. Base constructor call (ldarg.0 + call System.Object::.ctor)
        // 2. Field initializations
        // The IR pipeline needs to be extended to handle these in future PRs.
        // For now, this will compile but produce incomplete constructor IL,
        // so the fail-fast guards above should prevent reaching here.
        return TryCompileIRToILWithSignature(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
    }

    public MethodDefinitionHandle TryCompileMainMethod(string moduleName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (!TryLowerASTToLIR(node, scope, out var lirMethod))
        {
            return default;
        }

        // create the tools we need to generate the module type and method
        var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Scripts", moduleName);

        MethodParameterDescriptor[] parameters = [
            new MethodParameterDescriptor("exports", typeof(object)),
            new MethodParameterDescriptor("require", typeof(JavaScriptRuntime.CommonJS.RequireDelegate)),
            new MethodParameterDescriptor("module", typeof(object)),
            new MethodParameterDescriptor("__filename", typeof(string)),
            new MethodParameterDescriptor("__dirname", typeof(string))
        ];
        var methodDescriptor = new MethodDescriptor(
            "Main",
            programTypeBuilder,
            parameters);

        methodDescriptor.ReturnsVoid = true;

        var methodDefinitionHandle = TryCompileIRToIL(methodDescriptor, lirMethod!, methodBodyStreamEncoder);

        // Define the Script main type via TypeBuilder
        programTypeBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
            _bclReferences.ObjectType);

        return methodDefinitionHandle;
    }

    #endregion

    #region Core Pipeline - AST to LIR to IL

    private bool TryLowerASTToLIR(Node node, Scope scope, out MethodBodyIR? methodBody)
    {
        methodBody = null;

        if (!HIRBuilder.TryParseMethod(node, scope, out var hirMethod))
        {
            IR.IRPipelineMetrics.RecordFailure($"HIR parse failed for node type {node.Type}");
            return false;
        }

        if (!HIRToLIRLowerer.TryLower(hirMethod!, out var lirMethod))
        {
            IR.IRPipelineMetrics.RecordFailure("HIR->LIR lowering failed");
            return false;
        }

        methodBody = lirMethod!;
        return true;
    }

    private MethodDefinitionHandle TryCompileIRToIL(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        var (methodDef, _) = TryCompileIRToILWithSignature(methodDescriptor, methodBody, methodBodyStreamEncoder);
        return methodDef;
    }

    private (MethodDefinitionHandle MethodDef, BlobHandle Signature) TryCompileIRToILWithSignature(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    { 
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
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);

            // Compile the method body to IL
            if (!TryCompileMethodBodyToIL(methodDescriptor, methodBody, methodBodyStreamEncoder, out var bodyOffset))
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

    private bool TryCompileMethodBodyToIL(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder, out int bodyOffset)
    {
        bodyOffset = -1;
        var methodBlob = new BlobBuilder();
        var controlFlowBuilder = new ControlFlowBuilder();
        var ilEncoder = new InstructionEncoder(methodBlob, controlFlowBuilder);

        // Pre-pass: find console.log(oneArg) sequences that we will emit stack-only, and avoid
        // allocating IL locals for temps that are only used within those sequences.
        var peepholeReplaced = _consoleLogOptimizer.ComputeStackOnlyMask(methodBody);
        
        // Build map of temp → defining instruction for branch condition inlining
        var tempDefinitions = BranchConditionOptimizer.BuildTempDefinitionMap(methodBody);
        
        // Mark comparison temps only used by branches as non-materialized
        BranchConditionOptimizer.MarkBranchOnlyComparisonTemps(methodBody, peepholeReplaced, tempDefinitions);
        
        var allocation = TempLocalAllocator.Allocate(methodBody, peepholeReplaced);

        // Pre-create IL labels for all LIR labels
        var labelMap = new Dictionary<int, LabelHandle>();
        foreach (var lirLabel in methodBody.Instructions
            .OfType<LIRLabel>()
            .Where(l => !labelMap.ContainsKey(l.LabelId)))
        {
            labelMap[lirLabel.LabelId] = ilEncoder.DefineLabel();
        }

        bool hasExplicitReturn = false;
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            // Peephole: console.log(<singleArg>) emitted stack-only
            if (_consoleLogOptimizer.TryEmitPeephole(
                methodBody, i, ilEncoder, allocation,
                IsMaterialized,
                EmitStoreTemp,
                out var consumed))
            {
                i += consumed - 1;
                continue;
            }

            var instruction = methodBody.Instructions[i];
            
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
                    EmitBranchCondition(branchFalse.Condition, ilEncoder, allocation, methodBody, tempDefinitions);
                    ilEncoder.Branch(ILOpCode.Brfalse, labelMap[branchFalse.TargetLabel]);
                    continue;

                case LIRBranchIfTrue branchTrue:
                    EmitBranchCondition(branchTrue.Condition, ilEncoder, allocation, methodBody, tempDefinitions);
                    ilEncoder.Branch(ILOpCode.Brtrue, labelMap[branchTrue.TargetLabel]);
                    continue;
            }

            if (!TryCompileInstructionToIL(instruction, ilEncoder, allocation, methodBody))
            {
                // Failed to compile instruction
                IR.IRPipelineMetrics.RecordFailure($"IL compile failed: unsupported LIR instruction {instruction.GetType().Name}");
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

        var localVariablesSignature = CreateLocalVariablesSignature(methodBody, allocation);

        var bodyAttributes = MethodBodyAttributes.None;
        if (methodBody.VariableNames.Count > 0 || allocation.SlotStorages.Count > 0)
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

    private bool TryCompileInstructionToIL(LIRInstruction instruction, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        switch (instruction)
        {
            case LIRAddNumber addNumber:
                EmitLoadTemp(addNumber.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(addNumber.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Add);
                EmitStoreTemp(addNumber.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRSubNumber subNumber:
                EmitLoadTemp(subNumber.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(subNumber.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Sub);
                EmitStoreTemp(subNumber.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRBeginInitArrayElement beginInitArrayElement:
                // Pure SSA LIR lowering does not rely on stack tricks; this is a no-op hint.
                break;
            case LIRConstNumber constNumber:
                if (!IsMaterialized(constNumber.Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.LoadConstantR8(constNumber.Value);
                EmitStoreTemp(constNumber.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstString constString:
                if (!IsMaterialized(constString.Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constString.Value));
                EmitStoreTemp(constString.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstBoolean constBoolean:
                if (!IsMaterialized(constBoolean.Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.LoadConstantI4(constBoolean.Value ? 1 : 0);
                EmitStoreTemp(constBoolean.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstUndefined:
                if (!IsMaterialized(((LIRConstUndefined)instruction).Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.OpCode(ILOpCode.Ldnull);
                EmitStoreTemp(((LIRConstUndefined)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstNull:
                if (!IsMaterialized(((LIRConstNull)instruction).Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                EmitStoreTemp(((LIRConstNull)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                if (!IsMaterialized(getIntrinsicGlobal.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                EmitStoreTemp(getIntrinsicGlobal.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCallIntrinsic callIntrinsic:
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodBody);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodBody);
                EmitInvokeInstrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);

                if (IsMaterialized(callIntrinsic.Result, allocation, methodBody))
                {
                    EmitStoreTemp(callIntrinsic.Result, ilEncoder, allocation, methodBody);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Pop);
                }
                break;
            case LIRConvertToObject convertToObject:
                if (!IsMaterialized(convertToObject.Result, allocation, methodBody))
                {
                    break;
                }

                EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodBody);
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

                EmitStoreTemp(convertToObject.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRTypeof:
                if (!IsMaterialized(((LIRTypeof)instruction).Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(((LIRTypeof)instruction).Value, ilEncoder, allocation, methodBody);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                EmitStoreTemp(((LIRTypeof)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRNegateNumber:
                if (!IsMaterialized(((LIRNegateNumber)instruction).Result, allocation, methodBody))
                {
                    break;
                }

                EmitLoadTemp(((LIRNegateNumber)instruction).Value, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Neg);
                EmitStoreTemp(((LIRNegateNumber)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRBitwiseNotNumber:
                if (!IsMaterialized(((LIRBitwiseNotNumber)instruction).Result, allocation, methodBody))
                {
                    break;
                }

                EmitLoadTemp(((LIRBitwiseNotNumber)instruction).Value, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(((LIRBitwiseNotNumber)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberLessThan cmpLt:
                if (!IsMaterialized(cmpLt.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpLt.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpLt.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Clt);
                EmitStoreTemp(cmpLt.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberGreaterThan cmpGt:
                if (!IsMaterialized(cmpGt.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpGt.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpGt.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Cgt);
                EmitStoreTemp(cmpGt.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberLessThanOrEqual cmpLe:
                if (!IsMaterialized(cmpLe.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpLe.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpLe.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpLe.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                if (!IsMaterialized(cmpGe.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpGe.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpGe.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpGe.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberEqual cmpEq:
                if (!IsMaterialized(cmpEq.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpEq.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpEq.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpEq.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareNumberNotEqual cmpNe:
                if (!IsMaterialized(cmpNe.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpNe.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpNe.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpNe.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareBooleanEqual cmpBoolEq:
                if (!IsMaterialized(cmpBoolEq.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolEq.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpBoolEq.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolEq.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                if (!IsMaterialized(cmpBoolNe.Result, allocation, methodBody))
                {
                    break;
                }
                EmitLoadTemp(cmpBoolNe.Left, ilEncoder, allocation, methodBody);
                EmitLoadTemp(cmpBoolNe.Right, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(cmpBoolNe.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRNewObjectArray newObjectArray:
                if (!IsMaterialized(newObjectArray.Result, allocation, methodBody))
                {
                    break;
                }
                ilEncoder.LoadConstantI4(newObjectArray.ElementCount);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                EmitStoreTemp(newObjectArray.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRReturn lirReturn:
                EmitLoadTemp(lirReturn.ReturnValue, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Ret);
                break;
            case LIRStoreElementRef:
                {
                    var store = (LIRStoreElementRef)instruction;
                    EmitLoadTemp(store.Array, ilEncoder, allocation, methodBody);
                    ilEncoder.LoadConstantI4(store.Index);
                    EmitLoadTemp(store.Value, ilEncoder, allocation, methodBody);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
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
        MethodBodyIR methodBody,
        Dictionary<int, LIRInstruction> tempDefinitions)
    {
        // Check if the condition is a non-materialized comparison that we should inline
        if (!IsMaterialized(condition, allocation, methodBody) &&
            condition.Index >= 0 &&
            tempDefinitions.TryGetValue(condition.Index, out var definingInstruction) &&
            BranchConditionOptimizer.IsComparisonInstruction(definingInstruction))
        {
            // Emit the comparison inline without storing to a local
            BranchConditionOptimizer.EmitInlineComparison(
                definingInstruction,
                ilEncoder,
                (temp, encoder) => EmitLoadTemp(temp, encoder, allocation, methodBody));
        }
        else
        {
            // Load the condition from its local normally
            EmitLoadTemp(condition, ilEncoder, allocation, methodBody);
        }
    }

    #endregion

    #region Temp/Local Variable Management

    private StandaloneSignatureHandle CreateLocalVariablesSignature(MethodBodyIR methodBody, TempLocalAllocation allocation)
    {
        if (methodBody.VariableNames.Count == 0 && allocation.SlotStorages.Count == 0)
        {
            return default;
        }

        int varCount = methodBody.VariableNames.Count;
        int totalLocals = varCount + allocation.SlotStorages.Count;

        var localSig = new BlobBuilder();
        var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(totalLocals);

        // Variable locals first
        for (int i = 0; i < varCount; i++)
        {
            var typeEncoder = localEncoder.AddVariable().Type();
            
            if (i < methodBody.VariableStorages.Count)
            {
                var storage = methodBody.VariableStorages[i];
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

    private void EmitLoadTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        // Check if materialized - if so, load from local
        if (IsMaterialized(temp, allocation, methodBody))
        {
            var slot = GetSlotForTemp(temp, allocation, methodBody);
            ilEncoder.LoadLocal(slot);
            return;
        }

        // Not materialized - try to emit inline
        var def = TryFindDefInstruction(methodBody, temp);
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
            default:
                throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - unsupported instruction {def.GetType().Name}");
        }
    }

    private static bool IsMaterialized(TempVariable temp, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        // Variable-mapped temps always materialize into their stable variable local slot.
        if (temp.Index >= 0 &&
            temp.Index < methodBody.TempVariableSlots.Count &&
            methodBody.TempVariableSlots[temp.Index] >= 0)
        {
            return true;
        }

        return allocation.IsMaterialized(temp);
    }

    private static void EmitStoreTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        if (!IsMaterialized(temp, allocation, methodBody))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
            return;
        }

        var slot = GetSlotForTemp(temp, allocation, methodBody);
        ilEncoder.StoreLocal(slot);
    }

    private static int GetSlotForTemp(TempVariable temp, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        // Variable-mapped temps always go to their stable variable slot.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            int varSlot = methodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                return varSlot;
            }
        }

        // Other temps go after variable locals.
        var slot = allocation.GetSlot(temp);
        return methodBody.VariableNames.Count + slot;
    }

    private static LIRInstruction? TryFindDefInstruction(MethodBodyIR methodBody, TempVariable temp)
    {
        foreach (var instr in methodBody.Instructions
            .Where(i => TempLocalAllocator.TryGetDefinedTemp(i, out var defined) && defined == temp))
        {
            return instr;
        }
        return null;
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

    public void EmitInvokeInstrinsicMethod(Type declaringType, string methodName, InstructionEncoder ilEncoder)
    {
        var methodMref = _memberRefRegistry.GetOrAddMethod(declaringType, methodName);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodMref);
    }

    #endregion
}
