using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.SymbolTables;
using Js2IL.IR;
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

    public JsMethodCompiler(MetadataBuilder metadataBuilder, TypeReferenceRegistry typeReferenceRegistry, MemberReferenceRegistry memberReferenceRegistry, BaseClassLibraryReferences bclReferences)
    {
        _metadataBuilder = metadataBuilder;
        _typeReferenceRegistry = typeReferenceRegistry;
        _bclReferences = bclReferences;
        _memberRefRegistry = memberReferenceRegistry;
    }

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
        var ilEncoder = new InstructionEncoder(methodBlob);

        // Pre-pass: find console.log(oneArg) sequences that we will emit stack-only, and avoid
        // allocating IL locals for temps that are only used within those sequences.
        var peepholeReplaced = ComputeStackOnlyConsoleLogPeepholeMask(methodBody);
        var allocation = TempLocalAllocator.Allocate(methodBody, peepholeReplaced);
        var varCount = methodBody.VariableNames.Count;

        bool hasExplicitReturn = false;
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            // Peephole: console.log(<singleArg>) emitted stack-only
            if (TryEmitConsoleLogPeephole(methodBody, i, ilEncoder, allocation, out var consumed))
            {
                i += consumed - 1;
                continue;
            }

            var instruction = methodBody.Instructions[i];
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

    private bool[]? ComputeStackOnlyConsoleLogPeepholeMask(MethodBodyIR methodBody)
    {
        int tempCount = methodBody.Temps.Count;
        if (tempCount == 0)
        {
            return null;
        }

        var replaced = new bool[methodBody.Instructions.Count];

        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (TryMatchConsoleLogMultiArgSequence(methodBody, i, out var lastStoreIndex, out var storeInfos))
            {
                int callIndex = lastStoreIndex + 1;
                
                // Build set of temps defined in this sequence
                var definedInSequence = new HashSet<TempVariable>();
                for (int j = i; j <= lastStoreIndex; j++)
                {
                    if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[j], out var def))
                    {
                        definedInSequence.Add(def);
                    }
                }

                // For single-arg case, check update expression pattern
                if (storeInfos.Count == 1)
                {
                    int idx = i + 2;
                    if (methodBody.Instructions[idx] is LIRBeginInitArrayElement)
                    {
                        idx++;
                    }
                    if (TryMatchUpdateExpressionForConsoleArg(methodBody, idx, storeInfos[0].StoreIndex, storeInfos[0].StoredValue, out _, out _, out _))
                    {
                        for (int j = i; j <= callIndex; j++)
                        {
                            replaced[j] = true;
                        }
                        i = callIndex;
                        continue;
                    }
                }

                // Check if ALL arguments can be emitted stack-only
                bool allArgsStackOnly = true;
                foreach (var (_, storedValue) in storeInfos)
                {
                    if (!CanEmitTempStackOnly(methodBody, storedValue, definedInSequence))
                    {
                        allArgsStackOnly = false;
                        break;
                    }
                }

                if (allArgsStackOnly)
                {
                    for (int j = i; j <= callIndex; j++)
                    {
                        replaced[j] = true;
                    }
                    i = callIndex;
                }
            }
        }

        // Determine which temps are used outside replaced regions.
        var usedOutside = new bool[tempCount];
        for (int i = 0; i < methodBody.Instructions.Count; i++)
        {
            if (replaced[i])
            {
                continue;
            }

            foreach (var used in TempLocalAllocator.EnumerateUsedTemps(methodBody.Instructions[i]))
            {
                if (used.Index >= 0 && used.Index < tempCount)
                {
                    usedOutside[used.Index] = true;
                }
            }
        }

        return usedOutside;
    }

    /// <summary>
    /// Matches a console.log sequence with N arguments (N >= 1).
    /// Pattern:
    ///   GetIntrinsicGlobal("console") -> tConsole
    ///   NewObjectArray(N) -> tArr
    ///   For each arg i in 0..N-1:
    ///     (optional) BeginInitArrayElement(tArr, i)
    ///     ... (value computation) ...
    ///     StoreElementRef(tArr, i, tVal_i)
    ///   CallIntrinsic(tConsole, "log", tArr) -> tRes
    /// </summary>
    private static bool TryMatchConsoleLogMultiArgSequence(
        MethodBodyIR methodBody,
        int startIndex,
        out int lastStoreIndex,
        out List<(int StoreIndex, TempVariable StoredValue)> storeInfos)
    {
        lastStoreIndex = -1;
        storeInfos = new List<(int StoreIndex, TempVariable StoredValue)>();

        if (startIndex + 3 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex] is not LIRGetIntrinsicGlobal g || !string.Equals(g.Name, "console", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 1] is not LIRNewObjectArray a || a.ElementCount < 1)
        {
            return false;
        }

        int argCount = a.ElementCount;
        int searchIdx = startIndex + 2;

        for (int argIdx = 0; argIdx < argCount; argIdx++)
        {
            // Skip optional BeginInitArrayElement
            if (searchIdx < methodBody.Instructions.Count &&
                methodBody.Instructions[searchIdx] is LIRBeginInitArrayElement begin &&
                begin.Array == a.Result && begin.Index == argIdx)
            {
                searchIdx++;
            }

            // Find the StoreElementRef for this argument index
            int storeIndex = -1;
            for (int j = searchIdx; j < methodBody.Instructions.Count; j++)
            {
                if (methodBody.Instructions[j] is LIRStoreElementRef s && s.Array == a.Result && s.Index == argIdx)
                {
                    storeIndex = j;
                    storeInfos.Add((j, s.Value));
                    searchIdx = j + 1;
                    break;
                }

                // Bail out if we hit another intrinsic/global/array init
                if (methodBody.Instructions[j] is LIRGetIntrinsicGlobal or LIRNewObjectArray)
                {
                    return false;
                }
            }

            if (storeIndex < 0)
            {
                return false;
            }
        }

        if (storeInfos.Count != argCount)
        {
            return false;
        }

        lastStoreIndex = storeInfos[^1].StoreIndex;

        // Check that CallIntrinsic immediately follows the last store
        if (lastStoreIndex + 1 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[lastStoreIndex + 1] is not LIRCallIntrinsic call ||
            call.IntrinsicObject != g.Result ||
            call.ArgumentsArray != a.Result ||
            !string.Equals(call.Name, "log", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private bool CanEmitTempStackOnly(MethodBodyIR methodBody, TempVariable temp, HashSet<TempVariable> definedInSequence)
    {
        // Check for variable-mapped temps FIRST.
        // Variable-mapped temps CAN be emitted stack-only by loading from the variable local.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            if (methodBody.TempVariableSlots[temp.Index] >= 0)
            {
                return true;
            }
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            // No definition and not variable-mapped - can't emit
            return false;
        }

        return def switch
        {
            LIRConstNumber => true,
            LIRConstString => true,
            LIRConstBoolean => true,
            LIRConstUndefined => true,
            LIRConstNull => true,
            LIRConvertToObject conv => CanEmitTempStackOnly(methodBody, conv.Source, definedInSequence),
            LIRTypeof t => CanEmitTempStackOnly(methodBody, t.Value, definedInSequence),
            LIRNegateNumber neg => CanEmitTempStackOnly(methodBody, neg.Value, definedInSequence),
            LIRBitwiseNotNumber not => CanEmitTempStackOnly(methodBody, not.Value, definedInSequence),
            // Array ops that are part of this sequence are fine
            LIRGetIntrinsicGlobal g when definedInSequence.Contains(g.Result) => true,
            LIRNewObjectArray a when definedInSequence.Contains(a.Result) => true,
            // Anything else (variable reads, calls, etc.) cannot be emitted stack-only
            _ => false,
        };
    }

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
                    // Pure and unused
                    break;
                }
                ilEncoder.LoadConstantR8(constNumber.Value);
                EmitStoreTemp(constNumber.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstString constString:
                if (!IsMaterialized(constString.Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constString.Value));
                EmitStoreTemp(constString.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstBoolean constBoolean:
                if (!IsMaterialized(constBoolean.Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                ilEncoder.LoadConstantI4(constBoolean.Value ? 1 : 0);
                EmitStoreTemp(constBoolean.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstUndefined:
                if (!IsMaterialized(((LIRConstUndefined)instruction).Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                ilEncoder.OpCode(ILOpCode.Ldnull);
                EmitStoreTemp(((LIRConstUndefined)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRConstNull:
                if (!IsMaterialized(((LIRConstNull)instruction).Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                // JavaScript 'null' raw value (boxing handled by LIRConvertToObject)
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                EmitStoreTemp(((LIRConstNull)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                if (!IsMaterialized(getIntrinsicGlobal.Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                EmitStoreTemp(getIntrinsicGlobal.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRCallIntrinsic callIntrinsic:
                // Stack: [this, args] -> callvirt -> [object?]
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodBody);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodBody);
                EmitInvokeInstrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);

                if (IsMaterialized(callIntrinsic.Result, allocation, methodBody))
                {
                    EmitStoreTemp(callIntrinsic.Result, ilEncoder, allocation, methodBody);
                }
                else
                {
                    // Side effects needed, but result unused
                    ilEncoder.OpCode(ILOpCode.Pop);
                }
                break;
            case LIRConvertToObject convertToObject:
                if (!IsMaterialized(convertToObject.Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }

                EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodBody);
                // Box the value type using the source type
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
                    // Pure and unused
                    break;
                }
                // Stack: [value(object)] -> call TypeUtilities.Typeof(object) -> [string]
                EmitLoadTemp(((LIRTypeof)instruction).Value, ilEncoder, allocation, methodBody);
                var typeofMref = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMref);
                EmitStoreTemp(((LIRTypeof)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRNegateNumber:
                if (!IsMaterialized(((LIRNegateNumber)instruction).Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }

                EmitLoadTemp(((LIRNegateNumber)instruction).Value, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Neg);
                EmitStoreTemp(((LIRNegateNumber)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRBitwiseNotNumber:
                if (!IsMaterialized(((LIRBitwiseNotNumber)instruction).Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }

                // ~x = (double)(~(int)x)
                EmitLoadTemp(((LIRBitwiseNotNumber)instruction).Value, ilEncoder, allocation, methodBody);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                EmitStoreTemp(((LIRBitwiseNotNumber)instruction).Result, ilEncoder, allocation, methodBody);
                break;
            case LIRNewObjectArray newObjectArray:
                if (!IsMaterialized(newObjectArray.Result, allocation, methodBody))
                {
                    // Pure and unused
                    break;
                }
                ilEncoder.LoadConstantI4(newObjectArray.ElementCount);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                EmitStoreTemp(newObjectArray.Result, ilEncoder, allocation, methodBody);
                break;
            case LIRReturn lirReturn:
                // Load the return value temp onto the stack, then ret
                // The temp should already be boxed to object if needed
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
            // Current IR pipeline only uses numeric locals for ++/-- tests.
            // Default to float64 for variable slots.
            localEncoder.AddVariable().Type().Double();
        }

        // Then temp locals
        for (int i = 0; i < allocation.SlotStorages.Count; i++)
        {
            var storage = allocation.SlotStorages[i];

            var typeEncoder = localEncoder.AddVariable().Type();

            // Keep LIR independent of IL locals; we only use locals to materialize SSA values.
            // Unknown/boxed values are stored as object.
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

    private static void EmitLoadTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        var slot = GetSlotForTemp(temp, allocation, methodBody);
        ilEncoder.LoadLocal(slot);
    }

    private static bool IsMaterialized(TempVariable temp, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        // Variable-mapped temps always materialize into their stable variable local slot.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            if (methodBody.TempVariableSlots[temp.Index] >= 0)
            {
                return true;
            }
        }

        return allocation.IsMaterialized(temp);
    }

    private static void EmitStoreTemp(TempVariable temp, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodBodyIR methodBody)
    {
        if (!IsMaterialized(temp, allocation, methodBody))
        {
            // If a value is on the stack but not materialized, discard.
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

    private bool TryEmitConsoleLogPeephole(MethodBodyIR methodBody, int startIndex, InstructionEncoder ilEncoder, TempLocalAllocation allocation, out int consumed)
    {
        consumed = 0;

        // Pattern:
        //   GetIntrinsicGlobal("console") -> tConsole
        //   NewObjectArray(N) -> tArr
        //   For each arg i in 0..N-1:
        //     (optional) BeginInitArrayElement(tArr, i)
        //     ... (value computation) ...
        //     StoreElementRef(tArr, i, tVal_i)
        //   CallIntrinsic(tConsole, "log", tArr) -> tRes

        if (startIndex + 3 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[startIndex] is not LIRGetIntrinsicGlobal g || !string.Equals(g.Name, "console", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodBody.Instructions[startIndex + 1] is not LIRNewObjectArray a || a.ElementCount < 1)
        {
            return false;
        }

        int argCount = a.ElementCount;
        int idx = startIndex + 2;

        // Collect all StoreElementRef instructions for this array
        var storeInfos = new List<(int StoreIndex, TempVariable StoredValue)>();
        int searchIdx = idx;
        
        for (int argIdx = 0; argIdx < argCount; argIdx++)
        {
            // Skip optional BeginInitArrayElement
            if (searchIdx < methodBody.Instructions.Count && 
                methodBody.Instructions[searchIdx] is LIRBeginInitArrayElement begin &&
                begin.Array == a.Result && begin.Index == argIdx)
            {
                searchIdx++;
            }

            // Find the StoreElementRef for this argument index
            int storeIndex = -1;
            for (int j = searchIdx; j < methodBody.Instructions.Count; j++)
            {
                if (methodBody.Instructions[j] is LIRStoreElementRef s && s.Array == a.Result && s.Index == argIdx)
                {
                    storeIndex = j;
                    storeInfos.Add((j, s.Value));
                    searchIdx = j + 1;
                    break;
                }

                // Bail out if we hit another intrinsic/global/array init
                if (methodBody.Instructions[j] is LIRGetIntrinsicGlobal or LIRNewObjectArray)
                {
                    return false;
                }
            }

            if (storeIndex < 0)
            {
                return false;
            }
        }

        if (storeInfos.Count != argCount)
        {
            return false;
        }

        int lastStoreIndex = storeInfos[^1].StoreIndex;

        // Check that CallIntrinsic immediately follows the last store
        if (lastStoreIndex + 1 >= methodBody.Instructions.Count)
        {
            return false;
        }

        if (methodBody.Instructions[lastStoreIndex + 1] is not LIRCallIntrinsic call || 
            call.IntrinsicObject != g.Result || 
            call.ArgumentsArray != a.Result || 
            !string.Equals(call.Name, "log", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Build the set of temps defined in this sequence (for stack-only analysis)
        var definedInSequence = new HashSet<TempVariable>();
        for (int i = startIndex; i <= lastStoreIndex; i++)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(methodBody.Instructions[i], out var def))
            {
                definedInSequence.Add(def);
            }
        }

        // For single-arg case, check for update expression pattern first
        if (argCount == 1)
        {
            var (storeIndex, storedValue) = storeInfos[0];
            if (TryMatchUpdateExpressionForConsoleArg(methodBody, idx, storeIndex, storedValue, out var updatedVarSlot, out var isDecrement, out var isPrefix))
            {
                EmitLoadIntrinsicGlobalVariable("console", ilEncoder);
                ilEncoder.LoadConstantI4(1);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(0);

                EmitStackOnlyUpdateExpressionValue(ilEncoder, updatedVarSlot, isDecrement, isPrefix);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);

                EmitInvokeInstrinsicMethod(typeof(JavaScriptRuntime.Console), "log", ilEncoder);

                if (IsMaterialized(call.Result, allocation, methodBody))
                {
                    EmitStoreTemp(call.Result, ilEncoder, allocation, methodBody);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Pop);
                }

                consumed = (lastStoreIndex + 1) - startIndex + 1;
                return true;
            }
        }

        // Check if ALL arguments can be emitted stack-only
        bool allArgsStackOnly = true;
        foreach (var (_, storedValue) in storeInfos)
        {
            if (!CanEmitTempStackOnly(methodBody, storedValue, definedInSequence))
            {
                allArgsStackOnly = false;
                break;
            }
        }

        if (allArgsStackOnly)
        {
            EmitLoadIntrinsicGlobalVariable("console", ilEncoder);
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int argIdx = 0; argIdx < argCount; argIdx++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(argIdx);
                EmitTempStackOnly(methodBody, storeInfos[argIdx].StoredValue, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }

            EmitInvokeInstrinsicMethod(typeof(JavaScriptRuntime.Console), "log", ilEncoder);

            if (IsMaterialized(call.Result, allocation, methodBody))
            {
                EmitStoreTemp(call.Result, ilEncoder, allocation, methodBody);
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }

            consumed = (lastStoreIndex + 1) - startIndex + 1;
            return true;
        }

        // If we can't handle it stack-only, don't consume the sequence.
        // Let normal instruction-by-instruction compilation handle it (with proper temp allocation).
        return false;
    }

    private void EmitStackOnlyUpdateExpressionValue(InstructionEncoder ilEncoder, int updatedVarSlot, bool isDecrement, bool isPrefix)
    {
        // Emit stack-only update and value production, boxed.
        // We intentionally avoid any temp locals here.
        if (!isPrefix)
        {
            // postfix: value is old
            ilEncoder.LoadLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantR8(1.0);
            ilEncoder.OpCode(isDecrement ? ILOpCode.Sub : ILOpCode.Add);
            ilEncoder.StoreLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
        else
        {
            // prefix: value is new
            ilEncoder.LoadLocal(updatedVarSlot);
            ilEncoder.LoadConstantR8(1.0);
            ilEncoder.OpCode(isDecrement ? ILOpCode.Sub : ILOpCode.Add);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.StoreLocal(updatedVarSlot);
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
    }

    /// <summary>
    /// Emits a pure expression chain stack-only (no locals). The value ends up boxed on the stack.
    /// Only call this after CanEmitTempStackOnly returns true.
    /// </summary>
    private void EmitTempStackOnly(MethodBodyIR methodBody, TempVariable temp, InstructionEncoder ilEncoder)
    {
        // Handle variable-mapped temps FIRST.
        // Variable-mapped temps are loaded directly from their local slot.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            var varSlot = methodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                // Load the variable and box it.
                ilEncoder.LoadLocal(varSlot);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                return;
            }
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            throw new InvalidOperationException($"EmitTempStackOnly: temp {temp.Index} has no definition and is not variable-mapped");
        }

        switch (def)
        {
            case LIRConstNumber cn:
                ilEncoder.LoadConstantR8(cn.Value);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRConstString cs:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(cs.Value));
                // Strings are already reference types, no boxing needed.
                break;

            case LIRConstBoolean cb:
                ilEncoder.LoadConstantI4(cb.Value ? 1 : 0);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.BooleanType);
                break;

            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                break;

            case LIRConstNull:
                // Load the JsNull enum value (0) and box it
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                break;

            case LIRTypeof t:
                // Emit typeof: push the boxed value, call TypeUtilities.Typeof, result is string (already object)
                // t.Value is already boxed (object), so use EmitTempStackOnly which produces boxed values.
                EmitTempStackOnly(methodBody, t.Value, ilEncoder);
                var typeofMethod = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.TypeUtilities), nameof(JavaScriptRuntime.TypeUtilities.Typeof));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(typeofMethod);
                // Result is a string, which is already an object reference.
                break;

            case LIRNegateNumber neg:
                EmitTempStackOnlyUnboxed(methodBody, neg.Value, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Neg);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRBitwiseNotNumber not:
                EmitTempStackOnlyUnboxed(methodBody, not.Value, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;

            case LIRConvertToObject conv:
                // Emit the source and box it.
                EmitTempStackOnlyUnboxed(methodBody, conv.Source, ilEncoder);
                if (conv.SourceType == typeof(bool))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.BooleanType);
                }
                else if (conv.SourceType == typeof(JavaScriptRuntime.JsNull))
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Box);
                    ilEncoder.Token(_bclReferences.DoubleType);
                }
                break;

            default:
                throw new InvalidOperationException($"EmitTempStackOnly: unexpected instruction {def?.GetType().Name}");
        }
    }

    /// <summary>
    /// Emits a pure expression chain stack-only, leaving the raw (unboxed) value on the stack.
    /// Used for intermediate values in expression chains.
    /// </summary>
    private void EmitTempStackOnlyUnboxed(MethodBodyIR methodBody, TempVariable temp, InstructionEncoder ilEncoder)
    {
        // Handle variable-mapped temps FIRST.
        // Variable-mapped temps are loaded directly from their local slot.
        if (temp.Index >= 0 && temp.Index < methodBody.TempVariableSlots.Count)
        {
            var varSlot = methodBody.TempVariableSlots[temp.Index];
            if (varSlot >= 0)
            {
                // Load the variable (unboxed, already double).
                ilEncoder.LoadLocal(varSlot);
                return;
            }
        }

        var def = TryFindDefInstruction(methodBody, temp);
        if (def == null)
        {
            throw new InvalidOperationException($"EmitTempStackOnlyUnboxed: temp {temp.Index} has no definition and is not variable-mapped");
        }

        switch (def)
        {
            case LIRConstNumber cn:
                ilEncoder.LoadConstantR8(cn.Value);
                break;

            case LIRConstBoolean cb:
                ilEncoder.LoadConstantI4(cb.Value ? 1 : 0);
                break;

            case LIRConstNull:
                // Load unboxed JsNull enum value (0)
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                break;

            case LIRNegateNumber neg:
                EmitTempStackOnlyUnboxed(methodBody, neg.Value, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Neg);
                break;

            case LIRBitwiseNotNumber not:
                EmitTempStackOnlyUnboxed(methodBody, not.Value, ilEncoder);
                ilEncoder.OpCode(ILOpCode.Conv_i4);
                ilEncoder.OpCode(ILOpCode.Not);
                ilEncoder.OpCode(ILOpCode.Conv_r8);
                break;

            default:
                throw new InvalidOperationException($"EmitTempStackOnlyUnboxed: unexpected instruction {def?.GetType().Name}");
        }
    }

    private bool TryMatchUpdateExpressionForConsoleArg(
        MethodBodyIR methodBody,
        int start,
        int storeIndex,
        TempVariable storedValue,
        out int updatedVarSlot,
        out bool isDecrement,
        out bool isPrefix)
    {
        updatedVarSlot = -1;
        isDecrement = false;
        isPrefix = false;

        // Detect pattern produced by HIRToLIRLowerer for x++/x-- used as a single console.log arg:
        //   (postfix) ConvertToObject(original) -> tObj
        //            ConstNumber(1) -> tDelta
        //            AddNumber/SubNumber(original, tDelta) -> tUpdated (mapped to var slot)
        //            StoreElementRef(arr,0,tObj)
        //   (prefix)  ConstNumber(1) -> tDelta
        //            AddNumber/SubNumber(original, tDelta) -> tUpdated (mapped to var slot)
        //            ConvertToObject(tUpdated) -> tObj
        //            StoreElementRef(arr,0,tObj)

        if (TryFindDefInstruction(methodBody, storedValue) is not LIRConvertToObject valueConv)
        {
            return false;
        }

        // Find the AddNumber or SubNumber that produces the updated value in the intervening range.
        TempVariable updateResult;
        TempVariable updateLeft;
        TempVariable updateRight;
        bool foundUpdate = false;
        updateResult = default!;
        updateLeft = default!;
        updateRight = default!;

        for (int i = start; i < storeIndex; i++)
        {
            if (methodBody.Instructions[i] is LIRAddNumber add)
            {
                updateResult = add.Result;
                updateLeft = add.Left;
                updateRight = add.Right;
                isDecrement = false;
                foundUpdate = true;
                break;
            }
            if (methodBody.Instructions[i] is LIRSubNumber sub)
            {
                updateResult = sub.Result;
                updateLeft = sub.Left;
                updateRight = sub.Right;
                isDecrement = true;
                foundUpdate = true;
                break;
            }
        }
        if (!foundUpdate)
        {
            return false;
        }

        if (updateResult.Index >= 0 && updateResult.Index < methodBody.TempVariableSlots.Count)
        {
            updatedVarSlot = methodBody.TempVariableSlots[updateResult.Index];
        }
        if (updatedVarSlot < 0)
        {
            return false;
        }

        var rightDef = TryFindDefInstruction(methodBody, updateRight!);
        if (rightDef is not LIRConstNumber cn)
        {
            return false;
        }
        if (cn.Value is not 1.0)
        {
            return false;
        }

        bool prefix = valueConv.Source == updateResult;
        bool postfix = valueConv.Source == updateLeft;
        if (!prefix && !postfix)
        {
            return false;
        }

        isPrefix = prefix;
        return true;
    }

    private void EmitTempAsObject(MethodBodyIR methodBody, TempVariable valueTemp, InstructionEncoder ilEncoder, TempLocalAllocation allocation)
    {
        // If this temp was produced by an explicit convert-to-object, inline it.
        var def = TryFindDefInstruction(methodBody, valueTemp);
        if (def is LIRConvertToObject conv)
        {
            // Emit source value then box.
            EmitLoadTemp(conv.Source, ilEncoder, allocation, methodBody);
            ilEncoder.OpCode(ILOpCode.Box);
            if (conv.SourceType == typeof(bool))
            {
                ilEncoder.Token(_bclReferences.BooleanType);
            }
            else if (conv.SourceType == typeof(JavaScriptRuntime.JsNull))
            {
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull)));
            }
            else
            {
                ilEncoder.Token(_bclReferences.DoubleType);
            }
            return;
        }

        // Otherwise assume already object-compatible in this IR subset.
        EmitLoadTemp(valueTemp, ilEncoder, allocation, methodBody);
    }

    private static LIRInstruction? TryFindDefInstruction(MethodBodyIR methodBody, TempVariable temp)
    {
        // Cheap linear scan; IR pipeline methods are small today.
        // If this becomes hot, build an index map once per method.
        foreach (var instr in methodBody.Instructions)
        {
            if (TempLocalAllocator.TryGetDefinedTemp(instr, out var defined) && defined == temp)
            {
                return instr;
            }
        }
        return null;
    }

    private readonly record struct TempLocalAllocation(int[] TempToSlot, IReadOnlyList<ValueStorage> SlotStorages)
    {
        public bool IsMaterialized(TempVariable temp)
            => temp.Index >= 0 && temp.Index < TempToSlot.Length && TempToSlot[temp.Index] >= 0;

        public int GetSlot(TempVariable temp)
        {
            if (temp.Index < 0 || temp.Index >= TempToSlot.Length)
            {
                throw new InvalidOperationException($"Temp index out of range: {temp.Index}");
            }

            var slot = TempToSlot[temp.Index];
            if (slot < 0)
            {
                throw new InvalidOperationException($"Temp {temp.Index} was not materialized into an IL local slot.");
            }

            return slot;
        }
    }

    private static class TempLocalAllocator
    {
        private readonly record struct StorageKey(ValueStorageKind Kind, Type? ClrType);

        public static TempLocalAllocation Allocate(MethodBodyIR methodBody, bool[]? shouldMaterializeTemp = null)
        {
            int tempCount = methodBody.Temps.Count;
            if (tempCount == 0)
            {
                return new TempLocalAllocation(Array.Empty<int>(), Array.Empty<ValueStorage>());
            }

            var lastUse = new int[tempCount];
            Array.Fill(lastUse, -1);

            // First pass: determine last use for each temp.
            for (int i = 0; i < methodBody.Instructions.Count; i++)
            {
                foreach (var used in EnumerateUsedTemps(methodBody.Instructions[i]))
                {
                    if (used.Index >= 0 && used.Index < tempCount)
                    {
                        if (shouldMaterializeTemp is not null && !shouldMaterializeTemp[used.Index])
                        {
                            continue;
                        }
                        lastUse[used.Index] = i;
                    }
                }
            }

            // Second pass: linear-scan allocation with reuse after last use.
            var tempToSlot = new int[tempCount];
            Array.Fill(tempToSlot, -1);

            var slotStorages = new List<ValueStorage>();
            var freeByKey = new Dictionary<StorageKey, Stack<int>>();

            for (int i = 0; i < methodBody.Instructions.Count; i++)
            {
                var instruction = methodBody.Instructions[i];

                // Free dead operands before allocating the result so we can reuse within the same instruction.
                foreach (var used in EnumerateUsedTemps(instruction))
                {
                    if (used.Index < 0 || used.Index >= tempCount)
                    {
                        continue;
                    }

                    if (lastUse[used.Index] != i)
                    {
                        continue;
                    }

                    var usedSlot = tempToSlot[used.Index];
                    if (usedSlot < 0)
                    {
                        continue;
                    }

                    var usedStorage = GetTempStorage(methodBody, used);
                    var key = new StorageKey(usedStorage.Kind, usedStorage.ClrType);
                    if (!freeByKey.TryGetValue(key, out var stack))
                    {
                        stack = new Stack<int>();
                        freeByKey[key] = stack;
                    }
                    stack.Push(usedSlot);
                }

                // Allocate a slot for result if it will be used later.
                if (TryGetDefinedTemp(instruction, out var defined))
                {
                    if (defined.Index >= 0 && defined.Index < tempCount && lastUse[defined.Index] >= 0 && (shouldMaterializeTemp is null || shouldMaterializeTemp[defined.Index]))
                    {
                        var storage = GetTempStorage(methodBody, defined);
                        var key = new StorageKey(storage.Kind, storage.ClrType);

                        int slot;
                        if (freeByKey.TryGetValue(key, out var stack) && stack.Count > 0)
                        {
                            slot = stack.Pop();
                        }
                        else
                        {
                            slot = slotStorages.Count;
                            slotStorages.Add(storage);
                        }

                        tempToSlot[defined.Index] = slot;
                    }
                }
            }

            return new TempLocalAllocation(tempToSlot, slotStorages);
        }

        private static ValueStorage GetTempStorage(MethodBodyIR methodBody, TempVariable temp)
        {
            if (temp.Index >= 0 && temp.Index < methodBody.TempStorages.Count)
            {
                return methodBody.TempStorages[temp.Index];
            }

            return new ValueStorage(ValueStorageKind.Unknown);
        }

        internal static IEnumerable<TempVariable> EnumerateUsedTemps(LIRInstruction instruction)
        {
            switch (instruction)
            {
                case LIRAddNumber add:
                    yield return add.Left;
                    yield return add.Right;
                    break;
                case LIRSubNumber sub:
                    yield return sub.Left;
                    yield return sub.Right;
                    break;
                case LIRBeginInitArrayElement begin:
                    yield return begin.Array;
                    break;
                case LIRCallIntrinsic call:
                    yield return call.IntrinsicObject;
                    yield return call.ArgumentsArray;
                    break;
                case LIRConvertToObject conv:
                    yield return conv.Source;
                    break;
                case LIRTypeof t:
                    yield return t.Value;
                    break;
                case LIRNegateNumber neg:
                    yield return neg.Value;
                    break;
                case LIRBitwiseNotNumber not:
                    yield return not.Value;
                    break;
                case LIRStoreElementRef store:
                    yield return store.Array;
                    yield return store.Value;
                    break;
                case LIRReturn ret:
                    yield return ret.ReturnValue;
                    break;
            }
        }

        internal static bool TryGetDefinedTemp(LIRInstruction instruction, out TempVariable defined)
        {
            switch (instruction)
            {
                case LIRConstNumber c:
                    defined = c.Result;
                    return true;
                case LIRConstString c:
                    defined = c.Result;
                    return true;
                case LIRConstBoolean c:
                    defined = c.Result;
                    return true;
                case LIRConstUndefined c:
                    defined = c.Result;
                    return true;
                case LIRConstNull c:
                    defined = c.Result;
                    return true;
                case LIRGetIntrinsicGlobal g:
                    defined = g.Result;
                    return true;
                case LIRNewObjectArray n:
                    defined = n.Result;
                    return true;
                case LIRAddNumber add:
                    defined = add.Result;
                    return true;
                case LIRSubNumber sub:
                    defined = sub.Result;
                    return true;
                case LIRCallIntrinsic call:
                    defined = call.Result;
                    return true;
                case LIRConvertToObject conv:
                    defined = conv.Result;
                    return true;
                case LIRTypeof t:
                    defined = t.Result;
                    return true;
                case LIRNegateNumber neg:
                    defined = neg.Result;
                    return true;
                case LIRBitwiseNotNumber not:
                    defined = not.Result;
                    return true;
                default:
                    defined = default;
                    return false;
            }
        }
    }

    /// <summary>
    /// Loads a value onto the the stack for a given intrinsic global variable.
    /// </summary>
    /// <param name="variableName">The name of the intrinsic global variable.. i.e. 'console'</param>
    /// <remarks>
    /// When GlobalThis is changed to be instance-based rather than static-based, this method will need to be updated
    /// </remarks>
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
}