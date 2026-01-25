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

    private static void EmitReturnType(ReturnTypeEncoder returnType, Type clrReturnType, EntityHandle returnTypeHandle = default)
    {
        if (!returnTypeHandle.IsNil)
        {
            returnType.Type().Type(returnTypeHandle, isValueType: false);
            return;
        }

        if (clrReturnType == typeof(double))
        {
            returnType.Type().Double();
            return;
        }

        if (clrReturnType == typeof(bool))
        {
            returnType.Type().Boolean();
            return;
        }

        if (clrReturnType == typeof(string))
        {
            returnType.Type().String();
            return;
        }

        // Default ABI: JavaScript value as object.
        returnType.Type().Object();
    }

    private BlobHandle BuildMethodSignature(MethodDescriptor methodDescriptor)
    {
        var methodParameters = methodDescriptor.Parameters;

        var sigBuilder = new BlobBuilder();
        new BlobEncoder(sigBuilder)
            .MethodSignature(isInstanceMethod: !methodDescriptor.IsStatic)
            .Parameters(methodParameters.Count, returnType =>
            {
                if (methodDescriptor.ReturnsVoid)
                {
                    returnType.Void();
                }
                else
                {
                    EmitReturnType(returnType, methodDescriptor.ReturnClrType, methodDescriptor.ReturnTypeHandle);
                }
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
                    else if (parameterDefinition.ParameterType.IsArray
                        && parameterDefinition.ParameterType.GetElementType() == typeof(object))
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

        return _metadataBuilder.GetOrAddBlob(sigBuilder);
    }

    private static MethodAttributes ComputeMethodAttributes(MethodDescriptor methodDescriptor)
    {
        MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;

        if (string.Equals(methodDescriptor.Name, ".ctor", StringComparison.Ordinal))
        {
            methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        }
        else if (string.Equals(methodDescriptor.Name, ".cctor", StringComparison.Ordinal))
        {
            methodAttributes = MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        }
        else if (methodDescriptor.IsStatic)
        {
            methodAttributes |= MethodAttributes.Static;
        }

        return methodAttributes;
    }

    private static int GetIlArgIndexForScopesArray(MethodDescriptor methodDescriptor)
    {
        if (!methodDescriptor.HasScopesParameter)
        {
            throw new InvalidOperationException("Expected a scopes parameter but methodDescriptor.HasScopesParameter is false");
        }

        // Static: arg0 = scopes. Instance: arg0 = this, arg1 = scopes.
        return methodDescriptor.IsStatic ? 0 : 1;
    }

    private Type GetDeclaredScopeFieldClrType(string scopeName, string fieldName)
    {
        if (TryGetAsyncScopeBaseFieldClrType(fieldName, out var asyncFieldType))
            return asyncFieldType;

        if (TryGetGeneratorScopeBaseFieldClrType(fieldName, out var generatorFieldType))
            return generatorFieldType;

        return _scopeMetadataRegistry.TryGetFieldClrType(scopeName, fieldName, out var t)
            ? t
            : typeof(object);
    }

    private static bool TryGetAsyncScopeBaseFieldClrType(string fieldName, out Type fieldClrType)
    {
        fieldClrType = fieldName switch
        {
            "_asyncState" => typeof(int),
            "_deferred" => typeof(JavaScriptRuntime.PromiseWithResolvers),
            "_moveNext" => typeof(object),
            "_pendingException" => typeof(object),
            "_hasPendingException" => typeof(bool),
            "_pendingReturnValue" => typeof(object),
            "_hasPendingReturn" => typeof(bool),
            _ => typeof(object)
        };

        return fieldName is "_asyncState"
            or "_deferred"
            or "_moveNext"
            or "_pendingException"
            or "_hasPendingException"
            or "_pendingReturnValue"
            or "_hasPendingReturn";
    }

    private static bool TryGetGeneratorScopeBaseFieldClrType(string fieldName, out Type fieldClrType)
    {
        fieldClrType = fieldName switch
        {
            "_genState" => typeof(int),
            "_started" => typeof(bool),
            "_done" => typeof(bool),
            "_resumeValue" => typeof(object),
            "_resumeException" => typeof(object),
            "_hasResumeException" => typeof(bool),
            "_returnValue" => typeof(object),
            "_hasReturn" => typeof(bool),
            _ => typeof(object)
        };

        return fieldName is "_genState"
            or "_started"
            or "_done"
            or "_resumeValue"
            or "_resumeException"
            or "_hasResumeException"
            or "_returnValue"
            or "_hasReturn";
    }

    private bool TryResolveAsyncScopeBaseFieldToken(string fieldName, out EntityHandle token)
    {
        if (!TryGetAsyncScopeBaseFieldClrType(fieldName, out _))
        {
            token = default;
            return false;
        }

        token = _memberRefRegistry.GetOrAddField(typeof(JavaScriptRuntime.AsyncScope), fieldName);
        return true;
    }

    private bool TryResolveGeneratorScopeBaseFieldToken(string fieldName, out EntityHandle token)
    {
        if (!TryGetGeneratorScopeBaseFieldClrType(fieldName, out _))
        {
            token = default;
            return false;
        }

        token = _memberRefRegistry.GetOrAddField(typeof(JavaScriptRuntime.GeneratorScope), fieldName);
        return true;
    }

    private void EmitCastOrBoxAfterTypedFieldLoad(
        Type fieldClrType,
        ValueStorage targetStorage,
        InstructionEncoder ilEncoder,
        bool allowReferenceNarrowingCast)
    {
        // If the loaded value is reference-typed (often object) but the consumer expects a more specific
        // reference type (e.g., JavaScriptRuntime.Array), insert a castclass.
        if (allowReferenceNarrowingCast
            && !fieldClrType.IsValueType
            && targetStorage.Kind == ValueStorageKind.Reference
            && targetStorage.ClrType != null
            && targetStorage.ClrType != typeof(object)
            && fieldClrType != targetStorage.ClrType)
        {
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(targetStorage.ClrType));
            return;
        }

        if (!fieldClrType.IsValueType)
            return;

        // If the consumer expects an unboxed value with matching CLR type, do not box.
        if (targetStorage.Kind == ValueStorageKind.UnboxedValue && targetStorage.ClrType == fieldClrType)
            return;

        // Otherwise, box the loaded value type so downstream code (which is often object-based) continues to work.
        if (fieldClrType == typeof(double))
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.DoubleType);
        }
        else if (fieldClrType == typeof(bool))
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_bclReferences.BooleanType);
        }
    }

    private void EmitBoxIfNeededForTypedScopeFieldLoad(Type fieldClrType, ValueStorage targetStorage, InstructionEncoder ilEncoder)
    {
        EmitCastOrBoxAfterTypedFieldLoad(fieldClrType, targetStorage, ilEncoder, allowReferenceNarrowingCast: true);
    }

    private static Type GetDeclaredUserClassFieldClrType(
        Js2IL.Services.ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField)
    {
        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldClrType(registryClassName, fieldName, out var t)
                ? t
                : typeof(object);
        }

        return classRegistry.TryGetFieldClrType(registryClassName, fieldName, out var t2)
            ? t2
            : typeof(object);
    }

    private static bool TryGetDeclaredUserClassFieldTypeHandle(
        Js2IL.Services.ClassRegistry classRegistry,
        string registryClassName,
        string fieldName,
        bool isPrivateField,
        bool isStaticField,
        out EntityHandle typeHandle)
    {
        typeHandle = default;

        if (isStaticField)
        {
            return classRegistry.TryGetStaticFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        if (isPrivateField)
        {
            return classRegistry.TryGetPrivateFieldTypeHandle(registryClassName, fieldName, out typeHandle);
        }

        return classRegistry.TryGetFieldTypeHandle(registryClassName, fieldName, out typeHandle);
    }

    private void EmitBoxIfNeededForTypedUserClassFieldLoad(Type fieldClrType, ValueStorage targetStorage, InstructionEncoder ilEncoder)
    {
        EmitCastOrBoxAfterTypedFieldLoad(fieldClrType, targetStorage, ilEncoder, allowReferenceNarrowingCast: false);
    }

    private void EmitLoadTempAsDouble(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        // Peephole: avoid boxing a known double just to immediately coerce it back to double.
        // This happens when lowering inserts ConvertToObject around numeric literals/constants.
        if (!IsMaterialized(value, allocation) && TryFindDefInstruction(value) is LIRConvertToObject convertToObject && convertToObject.SourceType == typeof(double))
        {
            EmitLoadTemp(convertToObject.Source, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        var toNumberMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toNumberMref);
    }

    private void EmitLoadTempAsBoolean(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        var toBooleanMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toBooleanMref);
    }

    private void EmitLoadTempAsString(TempVariable value, InstructionEncoder ilEncoder, TempLocalAllocation allocation, MethodDescriptor methodDescriptor)
    {
        var storage = GetTempStorage(value);
        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
        {
            EmitLoadTemp(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
        ilEncoder.OpCode(ILOpCode.Castclass);
        ilEncoder.Token(_bclReferences.StringType);
    }

    private static int GetIlArgIndexForJsParameter(MethodDescriptor methodDescriptor, int jsParameterIndex)
    {
        // Base IL-argument index for JS parameter 0:
        // - static without scopes: base=0
        // - static with scopes: base=1 (arg0=scopes)
        // - instance without scopes: base=1 (arg0=this)
        // - instance with scopes: base=2 (arg0=this, arg1=scopes)
        int baseIndex = (methodDescriptor.IsStatic ? 0 : 1) + (methodDescriptor.HasScopesParameter ? 1 : 0);
        return baseIndex + jsParameterIndex;
    }

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

        var methodSig = BuildMethodSignature(methodDescriptor);

        // Compile body
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            return null;
        }

        var methodAttributes = ComputeMethodAttributes(methodDescriptor);

        var result = new CompiledCallableBody
        {
            Callable = callable,
            MethodName = methodDescriptor.Name,
            ExpectedMethodDef = expectedMethodDef,
            Attributes = methodAttributes,
            Signature = methodSig,
            BodyOffset = bodyOffset,
            ParameterNames = methodDescriptor.Parameters.Select(p => p.Name).ToArray()
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

        var methodSig = BuildMethodSignature(methodDescriptor);

        // Compile the method body to IL
        if (!TryCompileMethodBodyToIL(methodDescriptor, methodBodyStreamEncoder, out var bodyOffset))
        {
            // Failed to compile IL
            return default;
        }

        var parameterNames = methodDescriptor.Parameters.Select(p => p.Name).ToArray();

        var methodAttributes = ComputeMethodAttributes(methodDescriptor);

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

            if (!TryCompileInstructionToIL(instruction, ilEncoder, allocation, methodDescriptor, labelMap))
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
                if (MethodBody.IsAsync && MethodBody.AsyncInfo is { HasAwaits: true })
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

    private bool TryCompileInstructionToIL(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor,
        Dictionary<int, LabelHandle> labelMap)
    {
        switch (instruction)
        {
            case LIRAddNumber addNumber:
                TryEmitStackValueInstruction(addNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(addNumber.Result, ilEncoder, allocation);
                break;
            case LIRConcatStrings concatStrings:
                if (!IsMaterialized(concatStrings.Result, allocation))
                {
                    // Stackify will re-emit concat inline at the single use site.
                    break;
                }
                TryEmitStackValueInstruction(concatStrings, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(concatStrings.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamic addDynamic:
                EmitLoadTemp(addDynamic.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamic.Right, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectObject(ilEncoder);
                EmitStoreTemp(addDynamic.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamicDoubleObject addDynamicDoubleObject:
                EmitLoadTemp(addDynamicDoubleObject.LeftDouble, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicDoubleObject.RightObject, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddDoubleObject(ilEncoder);
                EmitStoreTemp(addDynamicDoubleObject.Result, ilEncoder, allocation);
                break;
            case LIRAddDynamicObjectDouble addDynamicObjectDouble:
                EmitLoadTemp(addDynamicObjectDouble.LeftObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicObjectDouble.RightDouble, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectDouble(ilEncoder);
                EmitStoreTemp(addDynamicObjectDouble.Result, ilEncoder, allocation);
                break;
            case LIRSubNumber subNumber:
                TryEmitStackValueInstruction(subNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(subNumber.Result, ilEncoder, allocation);
                break;
            case LIRMulNumber mulNumber:
                TryEmitStackValueInstruction(mulNumber, ilEncoder, allocation, methodDescriptor);
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
                TryEmitStackValueInstruction(constNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constNumber.Result, ilEncoder, allocation);
                break;
            case LIRConstString constString:
                if (!IsMaterialized(constString.Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction(constString, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constString.Result, ilEncoder, allocation);
                break;
            case LIRConstBoolean constBoolean:
                if (!IsMaterialized(constBoolean.Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction(constBoolean, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(constBoolean.Result, ilEncoder, allocation);
                break;
            case LIRConstUndefined:
                if (!IsMaterialized(((LIRConstUndefined)instruction).Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction((LIRConstUndefined)instruction, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(((LIRConstUndefined)instruction).Result, ilEncoder, allocation);
                break;
            case LIRConstNull:
                if (!IsMaterialized(((LIRConstNull)instruction).Result, allocation))
                {
                    break;
                }
                TryEmitStackValueInstruction((LIRConstNull)instruction, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(((LIRConstNull)instruction).Result, ilEncoder, allocation);
                break;

            case LIRGetUserClassType getUserClassType:
                if (!IsMaterialized(getUserClassType.Result, allocation))
                {
                    break;
                }
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null || !classRegistry.TryGet(getUserClassType.RegistryClassName, out var typeDef))
                    {
                        throw new InvalidOperationException($"Class not found in registry: '{getUserClassType.RegistryClassName}'");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldtoken);
                    ilEncoder.Token(typeDef);

                    var getTypeFromHandle = _memberRefRegistry.GetOrAddMethod(
                        typeof(Type),
                        nameof(Type.GetTypeFromHandle),
                        parameterTypes: new[] { typeof(RuntimeTypeHandle) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getTypeFromHandle);

                    EmitStoreTemp(getUserClassType.Result, ilEncoder, allocation);
                }
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

            case LIRCallIntrinsicGlobalFunction callGlobalFunc:
                EmitIntrinsicGlobalFunctionCall(callGlobalFunc, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallInstanceMethod callInstance:
                EmitInstanceMethodCall(callInstance, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStatic callIntrinsicStatic:
                EmitIntrinsicStaticCall(callIntrinsicStatic, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStaticVoid callIntrinsicStaticVoid:
                EmitIntrinsicStaticVoidCall(callIntrinsicStaticVoid, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRConvertToObject convertToObject:
                if (!IsMaterialized(convertToObject.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToObject, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToObject.Result, ilEncoder, allocation);
                break;

            case LIRConvertToNumber convertToNumber:
                if (!IsMaterialized(convertToNumber.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToNumber.Result, ilEncoder, allocation);
                break;

            case LIRConvertToBoolean convertToBoolean:
                if (!IsMaterialized(convertToBoolean.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToBoolean, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(convertToBoolean.Result, ilEncoder, allocation);
                break;

            case LIRConvertToString convertToString:
                if (!IsMaterialized(convertToString.Result, allocation))
                {
                    break;
                }

                TryEmitStackValueInstruction(convertToString, ilEncoder, allocation, methodDescriptor);
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

                // Emit JS ToBoolean(value) and then invert.
                EmitConvertToBooleanCore(logicalNot.Value, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                EmitStoreTemp(logicalNot.Result, ilEncoder, allocation);
                break;

            case LIRIsInstanceOf isInstanceOf:
                if (!IsMaterialized(isInstanceOf.Result, allocation))
                {
                    break;
                }

                EmitLoadTempAsObject(isInstanceOf.Value, ilEncoder, allocation, methodDescriptor);
                {
                    var targetType = _typeReferenceRegistry.GetOrAdd(isInstanceOf.TargetType);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(targetType);
                }
                EmitStoreTemp(isInstanceOf.Result, ilEncoder, allocation);
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
                TryEmitStackValueInstruction(divNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(divNumber.Result, ilEncoder, allocation);
                break;

            // Remainder (modulo)
            case LIRModNumber modNumber:
                TryEmitStackValueInstruction(modNumber, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(modNumber.Result, ilEncoder, allocation);
                break;

            // Exponentiation (Math.Pow)
            case LIRExpNumber expNumber:
                TryEmitStackValueInstruction(expNumber, ilEncoder, allocation, methodDescriptor);
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
                var truthyInputStorage = GetTempStorage(callIsTruthy.Value);
                EmitLoadTemp(callIsTruthy.Value, ilEncoder, allocation, methodDescriptor);

                if (truthyInputStorage.Kind == ValueStorageKind.UnboxedValue && truthyInputStorage.ClrType == typeof(double))
                {
                    EmitOperatorsIsTruthyDouble(ilEncoder);
                }
                else if (truthyInputStorage.Kind == ValueStorageKind.UnboxedValue && truthyInputStorage.ClrType == typeof(bool))
                {
                    EmitOperatorsIsTruthyBool(ilEncoder);
                }
                else
                {
                    EmitOperatorsIsTruthyObject(ilEncoder);
                }
                EmitStoreTemp(callIsTruthy.Result, ilEncoder, allocation);
                break;

            // Copy temp variable
            case LIRCopyTemp copyTemp:
                if (TryGetSameILLocalSlot(copyTemp.Source, copyTemp.Destination, allocation, out _))
                {
                    break;
                }
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
                        $"get_{nameof(JavaScriptRuntime.JsThrownValueException.Value)}");
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
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    if (!reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var ctorDef = (MethodDefinitionHandle)token;

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount)
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

                    // In JavaScript, extra constructor arguments are evaluated (side effects) but ignored.
                    // LIR lowering already evaluates all arguments; here we only pass the declared maximum.
                    int argsToPass = Math.Min(argc, newUserClass.MaxArgCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(newUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argsToPass;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);

                    if (!IsMaterialized(newUserClass.Result, allocation))
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                        break;
                    }

                    // Store the constructed instance as the default result.
                    EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);

                    // PL5.4a: If the JS constructor explicitly returned an object, new-expr evaluates to that object;
                    // if it returned a primitive/null/undefined, the constructed instance is used.
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry != null
                        && classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__js2il_ctorReturn", out var ctorReturnField)
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out var classTypeHandle))
                    {
                        var keepThis = ilEncoder.DefineLabel();
                        var done = ilEncoder.DefineLabel();

                        // Load the hidden ctor return field from the constructed instance.
                        EmitLoadTemp(newUserClass.Result, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(classTypeHandle);
                        ilEncoder.OpCode(ILOpCode.Ldfld);
                        ilEncoder.Token(ctorReturnField);

                        // If null/undefined => keep constructed instance.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Branch(ILOpCode.Brfalse, keepThis);

                        // If not an object (primitive) => keep constructed instance.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        var isOverride = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.IsConstructorReturnOverride),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(isOverride);
                        ilEncoder.Branch(ILOpCode.Brfalse, keepThis);

                        // Override result with the returned object.
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                        ilEncoder.Branch(ILOpCode.Br, done);

                        // Keep constructed instance; discard the return value.
                        ilEncoder.MarkLabel(keepThis);
                        ilEncoder.OpCode(ILOpCode.Pop);
                        ilEncoder.MarkLabel(done);
                    }
                    break;
                }

            case LIRStoreUserClassInstanceField storeInstanceField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

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

                    ilEncoder.LoadArgument(0);
                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false);

                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(bool))
                    {
                        EmitLoadTempAsBoolean(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else if (fieldClrType == typeof(string))
                    {
                        EmitLoadTempAsString(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(storeInstanceField.Value, ilEncoder, allocation, methodDescriptor);
                    }

                    // If the field is declared as a user class type (not object/string), cast before stfld.
                    // This keeps IL verification correct since `object` is not assignable to a specific class type.
                    if (TryGetDeclaredUserClassFieldTypeHandle(
                        classRegistry,
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        isStaticField: false,
                        out var declaredTypeHandle))
                    {
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(declaredTypeHandle);
                    }
                    else if (fieldClrType != typeof(object)
                        && fieldClrType != typeof(string)
                        && fieldClrType != typeof(double)
                        && fieldClrType != typeof(bool)
                        && !fieldClrType.IsValueType)
                    {
                        // Typed CLR reference field (e.g., JavaScriptRuntime.Int32Array). If the value is currently
                        // flowing as object (common in our temps), cast to keep IL verification correct.
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(fieldClrType));
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRStoreUserClassStaticField storeStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    if (!classRegistry.TryGetStaticField(storeStaticField.RegistryClassName, storeStaticField.FieldName, out var fieldHandle))
                    {
                        return false;
                    }

                    EmitLoadTemp(storeStaticField.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Stsfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }

            case LIRLoadUserClassStaticField loadStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    if (!classRegistry.TryGetStaticField(loadStaticField.RegistryClassName, loadStaticField.FieldName, out var fieldHandle))
                    {
                        return false;
                    }

                    ilEncoder.OpCode(ILOpCode.Ldsfld);
                    ilEncoder.Token(fieldHandle);

                    if (IsMaterialized(loadStaticField.Result, allocation))
                    {
                        EmitStoreTemp(loadStaticField.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }

                    break;
                }

            case LIRLoadUserClassInstanceField loadInstanceField:
                {
                    if (!IsMaterialized(loadInstanceField.Result, allocation))
                    {
                        // This temp will be emitted inline at its use site (EmitLoadTemp).
                        break;
                    }

                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        return false;
                    }

                    FieldDefinitionHandle fieldHandle;
                    if (loadInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            return false;
                        }
                    }

                    ilEncoder.LoadArgument(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        loadInstanceField.RegistryClassName,
                        loadInstanceField.FieldName,
                        loadInstanceField.IsPrivateField,
                        isStaticField: false);
                    EmitBoxIfNeededForTypedUserClassFieldLoad(fieldClrType, GetTempStorage(loadInstanceField.Result), ilEncoder);

                    EmitStoreTemp(loadInstanceField.Result, ilEncoder, allocation);

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
                        nameof(System.Collections.Generic.List<object>.Add));
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

            case LIRGetJsArrayLength getJsArrayLength:
                {
                    if (!IsMaterialized(getJsArrayLength.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used.
                        break;
                    }

                    // Load receiver as Array (cast only if needed)
                    var receiverStorage = GetTempStorage(getJsArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    // Emit: callvirt int32 List<object>.get_Count; conv.r8
                    var getCountMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "get_Count",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getCountMethod);
                    ilEncoder.OpCode(ILOpCode.Conv_r8);

                    EmitStoreTemp(getJsArrayLength.Result, ilEncoder, allocation);
                    break;
                }

            case LIRGetInt32ArrayLength getInt32ArrayLength:
                {
                    if (!IsMaterialized(getInt32ArrayLength.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used.
                        break;
                    }

                    // Load receiver as Int32Array (cast only if needed)
                    var receiverStorage = GetTempStorage(getInt32ArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    // Emit: callvirt float64 get_length
                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_length",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getLengthMethod);

                    EmitStoreTemp(getInt32ArrayLength.Result, ilEncoder, allocation);
                    break;
                }
            case LIRGetItem getItem:
                {
                    if (!IsMaterialized(getItem.Result, allocation))
                    {
                        break;
                    }

                    var indexStorage = GetTempStorage(getItem.Index);
                    var resultStorage = GetTempStorage(getItem.Result);
                    if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double))
                    {
                        // Emit: call JavaScriptRuntime.Object.GetItem(object, double)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(double) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp is typed as an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }
                    else
                    {
                        // Emit: call JavaScriptRuntime.Object.GetItem(object, object)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp is typed as an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }

                    EmitStoreTemp(getItem.Result, ilEncoder, allocation);
                    break;
                }

            case LIRGetJsArrayElement getArray:
                {
                    if (!IsMaterialized(getArray.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used.
                        break;
                    }

                    // Load receiver as Array (cast only if needed)
                    var receiverStorage = GetTempStorage(getArray.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    // Index must be numeric double
                    EmitLoadTemp(getArray.Index, ilEncoder, allocation, methodDescriptor);

                    var arrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(arrayGetter);

                    // If the temp expects an unboxed double, coerce object result to a number.
                    var resultStorage = GetTempStorage(getArray.Result);
                    if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                    {
                        var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toNumberMref);
                    }

                    EmitStoreTemp(getArray.Result, ilEncoder, allocation);
                    break;
                }

            case LIRGetInt32ArrayElement getI32:
                {
                    if (!IsMaterialized(getI32.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used.
                        break;
                    }

                    // Load receiver as Int32Array (cast only if needed)
                    var receiverStorage = GetTempStorage(getI32.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    // Index must be numeric double
                    EmitLoadTemp(getI32.Index, ilEncoder, allocation, methodDescriptor);

                    var int32ArrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArrayGetter);

                    // Store result (box only if the temp expects object)
                    var resultStorage = GetTempStorage(getI32.Result);
                    if (!(resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double)))
                    {
                        ilEncoder.OpCode(ILOpCode.Box);
                        ilEncoder.Token(_bclReferences.DoubleType);
                    }

                    EmitStoreTemp(getI32.Result, ilEncoder, allocation);
                    break;
                }

            case LIRSetInt32ArrayElement setI32:
                {
                    // Load receiver as Int32Array (cast only if needed)
                    var receiverStorage = GetTempStorage(setI32.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(setI32.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(setI32.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    EmitLoadTemp(setI32.Index, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(setI32.Value, ilEncoder, allocation, methodDescriptor);

                    var int32ArraySetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "set_Item",
                        parameterTypes: new[] { typeof(double), typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArraySetter);

                    // If the assignment expression result is used, return the assigned value.
                    if (IsMaterialized(setI32.Result, allocation))
                    {
                        EmitLoadTemp(setI32.Value, ilEncoder, allocation, methodDescriptor);
                        var resultStorage = GetTempStorage(setI32.Result);
                        if (!(resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double)))
                        {
                            ilEncoder.OpCode(ILOpCode.Box);
                            ilEncoder.Token(_bclReferences.DoubleType);
                        }
                        EmitStoreTemp(setI32.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        // Ensure stack is balanced if an unmaterialized result was produced.
                        // (We do not push the value unless needed.)
                    }

                    break;
                }

            case LIRSetJsArrayElement setArray:
                {
                    // Load receiver as Array (cast only if needed)
                    var receiverStorage = GetTempStorage(setArray.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(setArray.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(setArray.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    EmitLoadTemp(setArray.Index, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(setArray.Value, ilEncoder, allocation, methodDescriptor);

                    var arraySetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        "set_Item",
                        parameterTypes: new[] { typeof(double), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(arraySetter);

                    // If the assignment expression result is used, return the assigned value.
                    if (IsMaterialized(setArray.Result, allocation))
                    {
                        var valueStorage = GetTempStorage(setArray.Value);
                        var resultStorage = GetTempStorage(setArray.Result);
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
                            {
                                // Directly reuse the numeric RHS as the expression result.
                                EmitLoadTemp(setArray.Value, ilEncoder, allocation, methodDescriptor);
                            }
                            else
                            {
                                EmitLoadTempAsObject(setArray.Value, ilEncoder, allocation, methodDescriptor);
                                var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                    typeof(JavaScriptRuntime.TypeUtilities),
                                    nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                    parameterTypes: new[] { typeof(object) });
                                ilEncoder.OpCode(ILOpCode.Call);
                                ilEncoder.Token(toNumberMref);
                            }
                        }
                        else
                        {
                            EmitLoadTempAsObject(setArray.Value, ilEncoder, allocation, methodDescriptor);
                        }

                        EmitStoreTemp(setArray.Result, ilEncoder, allocation);
                    }

                    break;
                }
            case LIRSetItem setItem:
                {
                    var indexStorage = GetTempStorage(setItem.Index);
                    var valueStorage = GetTempStorage(setItem.Value);

                    bool isResultMaterialized = IsMaterialized(setItem.Result, allocation);

                    if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double) &&
                        valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
                    {
                        // Emit: call JavaScriptRuntime.Object.SetItem(object, double, double)
                        EmitLoadTempAsObject(setItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(setItem.Index, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(setItem.Value, ilEncoder, allocation, methodDescriptor);
                        var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.SetItem),
                            parameterTypes: new[] { typeof(object), typeof(double), typeof(double) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(setItemMethod);
                    }
                    else
                    {
                        // Emit: call JavaScriptRuntime.Object.SetItem(object, object, object)
                        EmitLoadTempAsObject(setItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(setItem.Index, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(setItem.Value, ilEncoder, allocation, methodDescriptor);
                        var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.SetItem),
                            parameterTypes: new[] { typeof(object), typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(setItemMethod);
                    }

                    if (isResultMaterialized)
                    {
                        EmitStoreTemp(setItem.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
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
                        nameof(System.Collections.Generic.List<object>.Add));
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(addMethod);
                    break;
                }
            case LIRReturn lirReturn:
                // Constructors are void-returning - don't load any value before ret
                if (!methodDescriptor.ReturnsVoid)
                {
                    if (MethodBody.IsGenerator)
                    {
                        // Generator completion: mark done and return { value, done: true }
                        var scopeName = MethodBody.LeafScopeId.Name;

                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_done");

                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
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
                        // Full async state machine: resolve _deferred and return its promise.
                        // 1. Mark state as completed: _asyncState = -1
                        // 2. Call _deferred.resolve(value)
                        // 3. Return _deferred.promise
                        
                        var scopeName = MethodBody.LeafScopeId.Name;
                        
                        // _asyncState = -1 (completed)
                        ilEncoder.LoadLocal(0);
                        ilEncoder.LoadConstantI4(-1);
                        EmitStoreFieldByName(ilEncoder, scopeName, "_asyncState");
                        
                        // Load _deferred.resolve (it's a bound closure)
                        ilEncoder.LoadLocal(0);
                        EmitLoadFieldByName(ilEncoder, scopeName, "_deferred");
                        var getResolveRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PromiseWithResolvers),
                            $"get_{nameof(JavaScriptRuntime.PromiseWithResolvers.resolve)}");
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(getResolveRef);
                        
                        // Call it with the return value: Closure.InvokeWithArgs(resolve, scopes, argsArray)
                        // Build a 1-element array containing the return value
                        EmitLoadScopesArray(ilEncoder, methodDescriptor);
                        ilEncoder.LoadConstantI4(1);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.LoadConstantI4(0);
                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
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
                    }
                    else if (MethodBody.IsAsync)
                    {
                        // async function without awaits (MVP): return Promise.resolve(value)
                        EmitLoadTempAsObject(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                        var resolveRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Promise),
                            nameof(JavaScriptRuntime.Promise.resolve),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(resolveRef);
                    }
                    else
                    {
                        EmitLoadTemp(lirReturn.ReturnValue, ilEncoder, allocation, methodDescriptor);
                    }
                }
                ilEncoder.OpCode(ILOpCode.Ret);
                break;

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
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.RuntimeServices), nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);
                        EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
                        break;
                    }

                    ilEncoder.LoadArgument(0);
                    EmitStoreTemp(loadThis.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadScopesArgument loadScopesArg:
                {
                    if (!IsMaterialized(loadScopesArg.Result, allocation))
                    {
                        break;
                    }

                    if (!methodDescriptor.HasScopesParameter)
                    {
                        return false;
                    }

                    // Static functions: scopes is arg0. Instance constructors: scopes is arg1.
                    ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1);
                    EmitStoreTemp(loadScopesArg.Result, ilEncoder, allocation);
                    break;
                }

            case LIRLoadParameter loadParam:
                {
                    if (!IsMaterialized(loadParam.Result, allocation))
                    {
                        break;
                    }

                    int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, loadParam.ParameterIndex);
                    ilEncoder.LoadArgument(ilArgIndex);
                    EmitStoreTemp(loadParam.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreParameter storeParam:
                {
                    int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, storeParam.ParameterIndex);
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

                    // IMPORTANT: use the callee's declared parameter count, not the call-site argument count.
                    // The call-site may omit args (default parameters), but the delegate signature must match
                    // the target method signature, otherwise the JIT can crash the process.
                    int jsParamCount = callableId.JsParamCount;
                    int argsToPass = Math.Min(callFunc.Arguments.Count, jsParamCount);

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Load scopes array
                    EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Load all arguments
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callFunc.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    // Pad missing parameters with null (supports default parameter initialization).
                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
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

            case LIRCallFunctionValue callValue:
                {
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, ldarg/ldloc argsArray, call Closure.InvokeWithArgs
                    EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        new[] { typeof(object), typeof(object[]), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue.Result, allocation))
                    {
                        EmitStoreTemp(callValue.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRConstructValue constructValue:
                {
                    EmitLoadTempAsObject(constructValue.ConstructorValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(constructValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var mref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.ConstructValue),
                        new[] { typeof(object), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(mref);

                    if (IsMaterialized(constructValue.Result, allocation))
                    {
                        EmitStoreTemp(constructValue.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallUserClassInstanceMethod callUserClass:
                {
                    if (callUserClass.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit direct instance call for '{callUserClass.RegistryClassName}.{callUserClass.MethodName}' - missing method token");
                    }

                    // Receiver is implicit 'this'
                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    // Async class methods use the standard js2il calling convention and expect a leading scopes array.
                    if (callUserClass.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    // Match the declared signature (ignore extra args, pad missing args with null).
                    int jsParamCount = callUserClass.MaxParamCount;
                    int argsToPass = Math.Min(callUserClass.Arguments.Count, jsParamCount);

                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(callUserClass.MethodHandle);

                    if (IsMaterialized(callUserClass.Result, allocation))
                    {
                        EmitStoreTemp(callUserClass.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallUserClassBaseConstructor callBaseCtor:
                {
                    if (callBaseCtor.ConstructorHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit base constructor call for '{callBaseCtor.BaseRegistryClassName}' - missing ctor token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseCtor.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseCtor.MaxParamCount;
                    int argsToPass = Math.Min(callBaseCtor.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseCtor.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseCtor.ConstructorHandle);
                    break;
                }

            case LIRCallUserClassBaseInstanceMethod callBaseMethod:
                {
                    if (callBaseMethod.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit base method call for '{callBaseMethod.BaseRegistryClassName}.{callBaseMethod.MethodName}' - missing method token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseMethod.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseMethod.MaxParamCount;
                    int argsToPass = Math.Min(callBaseMethod.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseMethod.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseMethod.MethodHandle);

                    if (IsMaterialized(callBaseMethod.Result, allocation))
                    {
                        EmitStoreTemp(callBaseMethod.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }

                    break;
                }

            case LIRCallMember callMember:
                {
                    // Runtime dispatcher member call.
                    EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                    EmitLoadTemp(callMember.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var callMemberRefDefault = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember),
                        new[] { typeof(object), typeof(string), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRefDefault);

                    if (IsMaterialized(callMember.Result, allocation))
                    {
                        EmitStoreTemp(callMember.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallTypedMember callTyped:
                {
                    EmitCallTypedMemberNoFallback(callTyped, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRCallTypedMemberWithFallback callTypedFallback:
                {
                    EmitCallTypedMemberWithFallback(callTypedFallback, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRCallDeclaredCallable callDeclared:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    if (!reader.TryGetDeclaredToken(callDeclared.CallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false; // Fall back to legacy emitter
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

                    foreach (var arg in callDeclared.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(methodHandle);

                    if (IsMaterialized(callDeclared.Result, allocation))
                    {
                        EmitStoreTemp(callDeclared.Result, ilEncoder, allocation);
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

                    var reader = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    var callableId = createArrow.CallableId;
                    if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    var jsParamCount = createArrow.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
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

                    var reader = _serviceProvider.GetService<Js2IL.Services.TwoPhaseCompilation.ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    var callableId = createFunc.CallableId;
                    if (!reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    var jsParamCount = createFunc.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
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
                    var fieldHandle = ResolveFieldToken(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "LIRLoadLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadLeafField.Field.ScopeName, loadLeafField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadLeafField.Result), ilEncoder);
                    EmitStoreTemp(loadLeafField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRLoadScopeFieldByName loadScopeField:
                {
                    if (!IsMaterialized(loadScopeField.Result, allocation))
                    {
                        break;
                    }

                    var fieldHandle = ResolveFieldToken(
                        loadScopeField.ScopeName,
                        loadScopeField.FieldName,
                        "LIRLoadScopeFieldByName instruction");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeField.ScopeName, loadScopeField.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadScopeField.Result), ilEncoder);
                    EmitStoreTemp(loadScopeField.Result, ilEncoder, allocation);
                    break;
                }
            case LIRStoreLeafScopeField storeLeafField:
                {
                    // Emit: ldloc.0 (scope instance), ldarg/ldloc Value, stfld (field handle)
                    var fieldHandle = ResolveFieldToken(
                        storeLeafField.Field.ScopeName,
                        storeLeafField.Field.FieldName,
                        "LIRStoreLeafScopeField instruction");
                    ilEncoder.LoadLocal(0); // Scope instance is always in local 0

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeLeafField.Field.ScopeName, storeLeafField.Field.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTemp(storeLeafField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }
            case LIRStoreScopeFieldByName storeScopeField:
                {
                    var fieldHandle = ResolveFieldToken(
                        storeScopeField.ScopeName,
                        storeScopeField.FieldName,
                        "LIRStoreScopeFieldByName instruction");
                    ilEncoder.LoadLocal(0);

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeScopeField.ScopeName, storeScopeField.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTemp(storeScopeField.Value, ilEncoder, allocation, methodDescriptor);
                    }
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
                    var fieldHandle = ResolveFieldToken(
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

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadParentField.Field.ScopeName, loadParentField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(loadParentField.Result), ilEncoder);
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
                    var fieldHandle = ResolveFieldToken(
                        storeParentField.Field.ScopeName,
                        storeParentField.Field.FieldName,
                        "LIRStoreParentScopeField instruction");
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    ilEncoder.LoadConstantI4(storeParentField.ParentScopeIndex);
                    ilEncoder.OpCode(ILOpCode.Ldelem_ref);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(scopeTypeHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(storeParentField.Field.ScopeName, storeParentField.Field.FieldName);
                    if (fieldClrType == typeof(double))
                    {
                        EmitLoadTempAsDouble(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTemp(storeParentField.Value, ilEncoder, allocation, methodDescriptor);
                    }
                    ilEncoder.OpCode(ILOpCode.Stfld);
                    ilEncoder.Token(fieldHandle);
                    break;
                }
            case LIRCreateLeafScopeInstance createScope:
                {
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
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctorRef);
                        ilEncoder.StoreLocal(0);
                    }
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

    private bool TryGetSameILLocalSlot(TempVariable source, TempVariable destination, TempLocalAllocation allocation, out int slot)
    {
        slot = -1;

        // Variable-slot backed temps.
        int srcVarSlot = (source.Index >= 0 && source.Index < MethodBody.TempVariableSlots.Count)
            ? MethodBody.TempVariableSlots[source.Index]
            : -1;
        int dstVarSlot = (destination.Index >= 0 && destination.Index < MethodBody.TempVariableSlots.Count)
            ? MethodBody.TempVariableSlots[destination.Index]
            : -1;

        if (srcVarSlot >= 0 || dstVarSlot >= 0)
        {
            if (srcVarSlot >= 0 && dstVarSlot >= 0 && srcVarSlot == dstVarSlot)
            {
                slot = srcVarSlot;
                return true;
            }

            return false;
        }

        // Temp-local backed temps.
        if (allocation.IsMaterialized(source) && allocation.IsMaterialized(destination))
        {
            var srcTempSlot = allocation.GetSlot(source);
            var dstTempSlot = allocation.GetSlot(destination);
            if (srcTempSlot == dstTempSlot)
            {
                slot = srcTempSlot;
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Temp/Local Variable Management

    private void EmitLocalType(SignatureTypeEncoder typeEncoder, ValueStorage storage, bool allowUnboxedJsNull)
    {
        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            typeEncoder.Boolean();
            return;
        }

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            typeEncoder.Double();
            return;
        }

        if (allowUnboxedJsNull && storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(JavaScriptRuntime.JsNull))
        {
            var typeRef = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsNull));
            typeEncoder.Type(typeRef, false);
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType == typeof(string))
        {
            typeEncoder.String();
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference
            && storage.ClrType != null
            && storage.ClrType.IsArray
            && storage.ClrType.GetElementType() == typeof(object))
        {
            typeEncoder.SZArray().Object();
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && !storage.TypeHandle.IsNil)
        {
            // Preserve known reference types represented only by metadata handles
            // (e.g., user-defined JS classes compiled as TypeDefinitionHandles).
            typeEncoder.Type(storage.TypeHandle, false);
            return;
        }

        if (storage.Kind == ValueStorageKind.Reference && storage.ClrType != null && storage.ClrType != typeof(object))
        {
            // Preserve known runtime reference types for declared variables (e.g., JavaScriptRuntime.Array)
            // so later lowering/emission can take advantage of typed locals.
            var typeRef = _typeReferenceRegistry.GetOrAdd(storage.ClrType);
            typeEncoder.Type(typeRef, false);
            return;
        }

        typeEncoder.Object();
    }

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
            var storage = MethodBody.VariableStorages[i];
            EmitLocalType(localEncoder.AddVariable().Type(), storage, allowUnboxedJsNull: false);
        }

        // Then temp locals
        for (int i = 0; i < allocation.SlotStorages.Count; i++)
        {
            var storage = allocation.SlotStorages[i];
            EmitLocalType(localEncoder.AddVariable().Type(), storage, allowUnboxedJsNull: true);
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
            // Try to include a little more context: which instruction attempted to use this temp.
            // This typically indicates a lowering/SSA bug where a temp is referenced without being defined.
            var firstUse = MethodBody.Instructions
                .Select((instr, idx) => (instr, idx))
                .FirstOrDefault(t => TempLocalAllocator.EnumerateUsedTemps(t.instr).Any(u => u == temp));

            if (firstUse.instr != null)
            {
                throw new InvalidOperationException(
                    $"Cannot emit unmaterialized temp {temp.Index} - no definition found. " +
                    $"First use at instruction #{firstUse.idx}: {firstUse.instr.GetType().Name}");
            }

            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - no definition found (and no uses found in method body?)");
        }

        // Emit the constant/expression inline
        if (TryEmitStackValueInstruction(def, ilEncoder, allocation, methodDescriptor))
        {
            return;
        }

        switch (def)
        {
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
            case LIRCompareNumberLessThan cmpLt:
                EmitLoadTemp(cmpLt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                break;
            case LIRCompareNumberGreaterThan cmpGt:
                EmitLoadTemp(cmpGt.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGt.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                break;
            case LIRCompareNumberLessThanOrEqual cmpLe:
                EmitLoadTemp(cmpLe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpLe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Cgt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberGreaterThanOrEqual cmpGe:
                EmitLoadTemp(cmpGe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpGe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Clt);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberEqual cmpEq:
                EmitLoadTemp(cmpEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareNumberNotEqual cmpNe:
                EmitLoadTemp(cmpNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanEqual cmpBoolEq:
                EmitLoadTemp(cmpBoolEq.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolEq.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;
            case LIRCompareBooleanNotEqual cmpBoolNe:
                EmitLoadTemp(cmpBoolNe.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(cmpBoolNe.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Ceq);
                ilEncoder.OpCode(ILOpCode.Ldc_i4_0);
                ilEncoder.OpCode(ILOpCode.Ceq);
                break;

            case LIRNewIntrinsicObject newIntrinsic:
                {
                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRNewUserClass newUserClass:
                {
                    // Emit inline user-defined class construction (newobj) with optional scopes array.
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing ICallableDeclarationReader");
                    }

                    if (!reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
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
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.");
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.");
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

            case LIRLoadUserClassStaticField loadStaticField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing ClassRegistry for static field load {loadStaticField.RegistryClassName}::{loadStaticField.FieldName}");
                    }

                    if (!classRegistry.TryGetStaticField(loadStaticField.RegistryClassName, loadStaticField.FieldName, out var fieldHandle))
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing registered static field {loadStaticField.RegistryClassName}::{loadStaticField.FieldName}");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldsfld);
                    ilEncoder.Token(fieldHandle);
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
                EmitOperatorsAddObjectObject(ilEncoder);
                break;
            case LIRAddDynamicDoubleObject addDynamicDoubleObject:
                // Mixed dynamic addition: left is unboxed double, right is boxed object
                EmitLoadTemp(addDynamicDoubleObject.LeftDouble, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicDoubleObject.RightObject, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddDoubleObject(ilEncoder);
                break;
            case LIRAddDynamicObjectDouble addDynamicObjectDouble:
                // Mixed dynamic addition: left is boxed object, right is unboxed double
                EmitLoadTemp(addDynamicObjectDouble.LeftObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addDynamicObjectDouble.RightDouble, ilEncoder, allocation, methodDescriptor);
                EmitOperatorsAddObjectDouble(ilEncoder);
                break;
            case LIRLoadLeafScopeField loadLeafField:
                // Emit inline: ldloc.0 (scope instance), ldfld (field handle)
                {
                    var fieldHandle = ResolveFieldToken(
                        loadLeafField.Field.ScopeName,
                        loadLeafField.Field.FieldName,
                        "inline LIRLoadLeafScopeField emission");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadLeafField.Field.ScopeName, loadLeafField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRLoadScopeFieldByName loadScopeField:
                // Emit inline: ldloc.0 (scope instance), ldfld (field handle)
                {
                    var fieldHandle = ResolveFieldToken(
                        loadScopeField.ScopeName,
                        loadScopeField.FieldName,
                        "inline LIRLoadScopeFieldByName emission");
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadScopeField.ScopeName, loadScopeField.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                }
                break;
            case LIRLoadParentScopeField loadParentField:
                // Emit inline: load scopes array, index, cast, ldfld
                {
                    var scopeTypeHandle = ResolveScopeTypeHandle(
                        loadParentField.Scope.Name,
                        "inline LIRLoadParentScopeField emission (castclass)");
                    var fieldHandle = ResolveFieldToken(
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

                    var fieldClrType = GetDeclaredScopeFieldClrType(loadParentField.Field.ScopeName, loadParentField.Field.FieldName);
                    EmitBoxIfNeededForTypedScopeFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
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
                        nameof(System.Collections.Generic.List<object>.Add));
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

            case LIRGetJsArrayLength getJsArrayLength:
                {
                    // Inline: receiver, callvirt get_Count, conv.r8
                    var receiverStorage = GetTempStorage(getJsArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getJsArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    var getCountMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(System.Collections.Generic.List<object>),
                        "get_Count",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getCountMethod);
                    ilEncoder.OpCode(ILOpCode.Conv_r8);
                    break;
                }

            case LIRGetInt32ArrayLength getInt32ArrayLength:
                {
                    // Inline: receiver, callvirt get_length
                    var receiverStorage = GetTempStorage(getInt32ArrayLength.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getInt32ArrayLength.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    var getLengthMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_length",
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getLengthMethod);
                    break;
                }
            case LIRGetItem getItem:
                {
                    var indexStorage = GetTempStorage(getItem.Index);
                    var resultStorage = GetTempStorage(getItem.Result);
                    if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double))
                    {
                        // Emit inline: call JavaScriptRuntime.Object.GetItem(object, double)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(double) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp storage expects an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }
                    else
                    {
                        // Emit inline: call JavaScriptRuntime.Object.GetItem(object, object)
                        EmitLoadTempAsObject(getItem.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(getItem.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItem),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemMethod);

                        // If the temp storage expects an unboxed double, coerce the object result to a number.
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.TypeUtilities),
                                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                                parameterTypes: new[] { typeof(object) });
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(toNumberMref);
                        }
                    }
                }
                break;

            case LIRGetJsArrayElement getArray:
                {
                    // Inline: receiver, index, callvirt Array.get_Item(double)
                    var receiverStorage = GetTempStorage(getArray.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
                    {
                        EmitLoadTemp(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getArray.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
                    }

                    EmitLoadTemp(getArray.Index, ilEncoder, allocation, methodDescriptor);

                    var arrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(arrayGetter);

                    // If the temp storage expects an unboxed double, coerce the object result to a number.
                    var resultStorage = GetTempStorage(getArray.Result);
                    if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                    {
                        var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toNumberMref);
                    }

                    break;
                }

            case LIRGetInt32ArrayElement getI32:
                {
                    // Inline: receiver, index, callvirt get_Item(double)
                    var receiverStorage = GetTempStorage(getI32.Receiver);
                    if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getI32.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    EmitLoadTemp(getI32.Index, ilEncoder, allocation, methodDescriptor);

                    var int32ArrayGetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_Item",
                        parameterTypes: new[] { typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArrayGetter);

                    // Leave as double on stack; caller will box if it needs object.
                    break;
                }
            case LIRCallIntrinsic callIntrinsic:
                // Emit inline intrinsic call (e.g., console.log)
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodDescriptor);
                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);
                // Result stays on stack (caller will handle it)
                break;

            case LIRCallIntrinsicGlobalFunction callGlobalFunc:
                EmitIntrinsicGlobalFunctionCallInline(callGlobalFunc, ilEncoder, allocation, methodDescriptor);
                // Result stays on stack
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

                    // IMPORTANT: use the callee's declared parameter count, not the call-site argument count.
                    // The call-site may omit args (default parameters), but the delegate signature must match
                    // the target method signature, otherwise the JIT can crash the process.
                    int jsParamCount = callableId.JsParamCount;
                    int argsToPass = Math.Min(callFunc.Arguments.Count, jsParamCount);

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Load scopes array
                    EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Load all arguments
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callFunc.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    // Pad missing parameters with null (supports default parameter initialization).
                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    // Invoke: callvirt Func<object[], [object, ...], object>::Invoke
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(_bclReferences.GetFuncInvokeRef(jsParamCount));
                    // Result stays on stack
                    break;
                }

            case LIRCallFunctionValue callValue:
                {
                    // Inline emission uses the same lowering as the main pass for calls.
                    EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        new[] { typeof(object), typeof(object[]), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue.Result, allocation))
                    {
                        EmitStoreTemp(callValue.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallMember callMember:
                {
                    // Inline emission of member call via runtime dispatcher.
                    EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                    EmitLoadTemp(callMember.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember),
                        new[] { typeof(object), typeof(string), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);
                    break;
                }

            case LIRCallTypedMember callTyped:
                {
                    EmitCallTypedMemberNoFallback(callTyped, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRCallTypedMemberWithFallback callTypedFallback:
                {
                    EmitCallTypedMemberWithFallback(callTypedFallback, ilEncoder, allocation, methodDescriptor);
                    break;
                }

            case LIRCallUserClassInstanceMethod callUserClass:
                {
                    if (callUserClass.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing method token for '{callUserClass.RegistryClassName}.{callUserClass.MethodName}'");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callUserClass.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callUserClass.MaxParamCount;
                    int argsToPass = Math.Min(callUserClass.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(callUserClass.MethodHandle);
                    break;
                }

            case LIRCallUserClassBaseConstructor callBaseCtor:
                {
                    if (callBaseCtor.ConstructorHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized base constructor call for '{callBaseCtor.BaseRegistryClassName}' - missing ctor token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseCtor.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseCtor.MaxParamCount;
                    int argsToPass = Math.Min(callBaseCtor.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseCtor.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseCtor.ConstructorHandle);
                    break;
                }

            case LIRCallUserClassBaseInstanceMethod callBaseMethod:
                {
                    if (callBaseMethod.MethodHandle.IsNil)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized base method call for '{callBaseMethod.BaseRegistryClassName}.{callBaseMethod.MethodName}' - missing method token");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldarg_0);

                    if (callBaseMethod.HasScopesParameter)
                    {
                        EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
                    }

                    int jsParamCount = callBaseMethod.MaxParamCount;
                    int argsToPass = Math.Min(callBaseMethod.Arguments.Count, jsParamCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(callBaseMethod.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callBaseMethod.MethodHandle);
                    break;
                }

            case LIRCallDeclaredCallable callDeclared:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null || !reader.TryGetDeclaredToken(callDeclared.CallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callDeclared.CallableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;

                    foreach (var arg in callDeclared.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(methodHandle);
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundArrowFunction createArrow:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    var callableId = createArrow.CallableId;
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createArrow.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }

            case LIRCreateBoundFunctionExpression createFunc:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    var callableId = createFunc.CallableId;
                    if (reader == null || !reader.TryGetDeclaredToken(callableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing declared token for callable {callableId.DisplayName}");
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    int jsParamCount = createFunc.CallableId.JsParamCount;

                    // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                    ilEncoder.OpCode(ILOpCode.Ldnull);
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount));

                    // Bind delegate to scopes array: Closure.Bind(object, object[])
                    EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.Bind), new[] { typeof(object), typeof(object[]) });
                    ilEncoder.Token(bindRef);
                    // Result stays on stack
                    break;
                }

            case LIRLoadUserClassInstanceField loadInstanceField:
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null)
                    {
                        throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - ClassRegistry service missing");
                    }

                    FieldDefinitionHandle fieldHandle;
                    if (loadInstanceField.IsPrivateField)
                    {
                        if (!classRegistry.TryGetPrivateField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing private field '{loadInstanceField.FieldName}' on '{loadInstanceField.RegistryClassName}'");
                        }
                    }
                    else
                    {
                        if (!classRegistry.TryGetField(loadInstanceField.RegistryClassName, loadInstanceField.FieldName, out fieldHandle))
                        {
                            throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - missing field '{loadInstanceField.FieldName}' on '{loadInstanceField.RegistryClassName}'");
                        }
                    }

                    // Inline: ldarg.0; ldfld <field>
                    ilEncoder.LoadArgument(0);
                    ilEncoder.OpCode(ILOpCode.Ldfld);
                    ilEncoder.Token(fieldHandle);

                    var fieldClrType = GetDeclaredUserClassFieldClrType(
                        classRegistry,
                        loadInstanceField.RegistryClassName,
                        loadInstanceField.FieldName,
                        loadInstanceField.IsPrivateField,
                        isStaticField: false);
                    EmitBoxIfNeededForTypedUserClassFieldLoad(fieldClrType, GetTempStorage(temp), ilEncoder);
                    break;
                }

            case LIRIsInstanceOf isInstanceOf:
                {
                    // Inline: <value as object>; isinst <TargetType>
                    EmitLoadTempAsObject(isInstanceOf.Value, ilEncoder, allocation, methodDescriptor);
                    var targetType = _typeReferenceRegistry.GetOrAdd(isInstanceOf.TargetType);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(targetType);
                    break;
                }
            default:
                throw new InvalidOperationException($"Cannot emit unmaterialized temp {temp.Index} - unsupported instruction {def.GetType().Name}");
        }
    }

    private bool TryEmitStackValueInstruction(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRConstNumber constNum:
                ilEncoder.LoadConstantR8(constNum.Value);
                return true;
            case LIRConstString constStr:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constStr.Value));
                return true;
            case LIRConstBoolean constBool:
                ilEncoder.LoadConstantI4(constBool.Value ? 1 : 0);
                return true;
            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                return true;
            case LIRConstNull:
                ilEncoder.LoadConstantI4((int)JavaScriptRuntime.JsNull.Null);
                return true;
            case LIRLoadThis:
                if (methodDescriptor.IsStatic)
                {
                    var getThisRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getThisRef);
                    return true;
                }
                ilEncoder.LoadArgument(0);
                return true;
            case LIRLoadScopesArgument:
                if (!methodDescriptor.HasScopesParameter)
                {
                    throw new InvalidOperationException("Cannot emit scopes argument when method has no scopes parameter");
                }
                // Static functions: scopes is arg0. Instance constructors: scopes is arg1.
                ilEncoder.LoadArgument(methodDescriptor.IsStatic ? 0 : 1);
                return true;
            case LIRLoadParameter loadParam:
                // Emit ldarg.X inline - no local slot needed
                int ilArgIndex = GetIlArgIndexForJsParameter(methodDescriptor, loadParam.ParameterIndex);
                ilEncoder.LoadArgument(ilArgIndex);
                return true;
            case LIRConvertToObject convertToObject:
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
                return true;

            case LIRConvertToNumber convertToNumber:
                // If already an unboxed numeric value, skip boxing + ToNumber.
                // In this compiler pipeline, the only non-numeric unboxed values are bool and JsNull.
                if (GetTempStorage(convertToNumber.Source) is { Kind: ValueStorageKind.UnboxedValue, ClrType: var clrType }
                    && clrType != typeof(bool)
                    && clrType != typeof(JavaScriptRuntime.JsNull))
                {
                    EmitLoadTemp(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                    return true;
                }

                EmitLoadTempAsObject(convertToNumber.Source, ilEncoder, allocation, methodDescriptor);
                {
                    var toNumberMref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.TypeUtilities),
                        nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(toNumberMref);
                }
                return true;

            case LIRConvertToBoolean convertToBoolean:
                EmitConvertToBooleanCore(convertToBoolean.Source, ilEncoder, allocation, methodDescriptor);
                return true;

            case LIRConvertToString convertToString:
                EmitConvertToStringCore(convertToString.Source, ilEncoder, allocation, methodDescriptor);
                return true;

            case LIRConcatStrings concatStrings:
                EmitLoadTemp(concatStrings.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(concatStrings.Right, ilEncoder, allocation, methodDescriptor);
                EmitStringConcat(ilEncoder);
                return true;

            case LIRAddNumber addNumber:
                EmitLoadTemp(addNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(addNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Add);
                return true;
            case LIRSubNumber subNumber:
                EmitLoadTemp(subNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(subNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Sub);
                return true;
            case LIRMulNumber mulNumber:
                EmitLoadTemp(mulNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(mulNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Mul);
                return true;
            case LIRDivNumber divNumber:
                EmitLoadTemp(divNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(divNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Div);
                return true;
            case LIRModNumber modNumber:
                EmitLoadTemp(modNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(modNumber.Right, ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Rem);
                return true;
            case LIRExpNumber expNumber:
                EmitLoadTemp(expNumber.Left, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(expNumber.Right, ilEncoder, allocation, methodDescriptor);
                EmitMathPow(ilEncoder);
                return true;
            default:
                return false;
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
        var sourceStorage = GetTempStorage(source);

        // If the value is already a typed primitive, avoid boxing.
        if (sourceStorage.Kind == ValueStorageKind.UnboxedValue)
        {
            if (sourceStorage.ClrType == typeof(bool))
            {
                // JS ToBoolean(bool) is identity.
                EmitLoadTemp(source, ilEncoder, allocation, methodDescriptor);
                return;
            }

            if (sourceStorage.ClrType == typeof(double))
            {
                EmitLoadTemp(source, ilEncoder, allocation, methodDescriptor);
                var toBooleanDoubleMref = _memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.TypeUtilities),
                    nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
                    parameterTypes: new[] { typeof(double) });
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(toBooleanDoubleMref);
                return;
            }
        }

        // Fallback: box and call object-based coercion.
        EmitLoadTempAsObject(source, ilEncoder, allocation, methodDescriptor);
        var toBooleanObjectMref = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.TypeUtilities),
            nameof(JavaScriptRuntime.TypeUtilities.ToBoolean),
            parameterTypes: new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(toBooleanObjectMref);
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
    /// For instance methods with scopes parameter: ldarg.1 (scopes array is second parameter, after this)
    /// For instance methods: ldarg.0 (this), ldfld _scopes (scopes stored in instance field)
    /// </summary>
    private void EmitLoadScopesArray(InstructionEncoder ilEncoder, MethodDescriptor methodDescriptor)
    {
        if (methodDescriptor.IsStatic && methodDescriptor.HasScopesParameter)
        {
            // Static function with scopes parameter - scopes is arg 0
            ilEncoder.LoadArgument(0);
        }
        else if (!methodDescriptor.IsStatic && methodDescriptor.HasScopesParameter)
        {
            // Instance method (e.g., constructor) with scopes parameter - scopes is arg 1
            ilEncoder.LoadArgument(1);
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

            private void EmitLoadScopesArrayOrEmpty(InstructionEncoder ilEncoder, MethodDescriptor methodDescriptor)
            {
                if (methodDescriptor.HasScopesParameter || (!methodDescriptor.IsStatic && methodDescriptor.ScopesFieldHandle.HasValue))
                {
                    EmitLoadScopesArray(ilEncoder, methodDescriptor);
                    return;
                }

                // If this method has a leaf scope instance in local 0, use that as the parent
                // scopes array. This is important for resumables (async/generators) because
                // their leaf scope is prepended to the parent scopes, and the runtime expects
                // at least the global/module scope to be present at index 1 after prepending.
                if (MethodBody.NeedsLeafScopeLocal)
                {
                    ilEncoder.LoadConstantI4(1);
                    ilEncoder.OpCode(ILOpCode.Newarr);
                    ilEncoder.Token(_bclReferences.ObjectType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.LoadConstantI4(0);
                    ilEncoder.LoadLocal(0);
                    ilEncoder.OpCode(ILOpCode.Stelem_ref);
                    return;
                }

                // ABI fallback: 1-element array with null.
                EmitEmptyScopesArray(ilEncoder);
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

    private void EmitIntrinsicGlobalFunctionCall(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        EmitIntrinsicGlobalFunctionCallCore(instruction, ilEncoder, allocation, methodDescriptor);

        // Store or pop result
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            // If the method returns void, don't pop
            var methodInfo = ResolveGlobalThisMethod(instruction.FunctionName);
            if (methodInfo.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    private void EmitIntrinsicGlobalFunctionCallInline(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Leaves the result on the stack.
        EmitIntrinsicGlobalFunctionCallCore(instruction, ilEncoder, allocation, methodDescriptor);
    }

    private System.Reflection.MethodInfo ResolveGlobalThisMethod(string functionName)
    {
        var gvType = typeof(JavaScriptRuntime.GlobalThis);
        var methodInfo = gvType.GetMethod(
            functionName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Unknown GlobalThis intrinsic function: {functionName}");
        }
        return methodInfo;
    }

    private void EmitIntrinsicGlobalFunctionCallCore(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var methodInfo = ResolveGlobalThisMethod(instruction.FunctionName);
        var parameters = methodInfo.GetParameters();

        var hasParamsArray = parameters.Length > 0
            && Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute));

        var regularParamCount = hasParamsArray ? parameters.Length - 1 : parameters.Length;

        // Load regular args (missing args become null/undefined at runtime; we push ldnull)
        for (int i = 0; i < regularParamCount; i++)
        {
            if (i < instruction.Arguments.Count)
            {
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Ldnull);
            }
        }

        if (hasParamsArray)
        {
            var paramsCount = Math.Max(0, instruction.Arguments.Count - regularParamCount);
            ilEncoder.LoadConstantI4(paramsCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < paramsCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[regularParamCount + i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Evaluate extra args for side effects (already evaluated in lowering), but do not pass them.
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.GlobalThis), methodInfo.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
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

    private void EmitCallTypedMemberNoFallback(
        LIRCallTypedMember instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Receiver: if already proven to be the resolved user-class type (e.g., stored in a typed local),
        // avoid the redundant cast.
        var receiverStorage = GetTempStorage(instruction.Receiver);
        if (receiverStorage.Kind == ValueStorageKind.Reference
            && !receiverStorage.TypeHandle.IsNil
            && receiverStorage.TypeHandle.Equals(instruction.ReceiverTypeHandle))
        {
            EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(instruction.ReceiverTypeHandle);
        }

        // Async class methods follow the js2il ABI and take a leading scopes array parameter.
        if (instruction.HasScopesParameter)
        {
            EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
        }

        // Match the declared signature (ignore extra args, pad missing args with null).
        int jsParamCount = instruction.MaxParamCount;
        int argsToPass = Math.Min(instruction.Arguments.Count, jsParamCount);

        for (int i = 0; i < argsToPass; i++)
        {
            EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
        }

        for (int i = argsToPass; i < jsParamCount; i++)
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }

        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(instruction.MethodHandle);

        if (IsMaterialized(instruction.Result, allocation))
        {
            var resultStorage = GetTempStorage(instruction.Result);
            if (resultStorage.Kind == ValueStorageKind.Reference
                && resultStorage.ClrType == typeof(object)
                && instruction.ReturnClrType != typeof(object)
                && instruction.ReturnClrType.IsValueType)
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(instruction.ReturnClrType));
            }

            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
    }

    private void EmitCallTypedMemberWithFallback(
        LIRCallTypedMemberWithFallback instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var fallbackLabel = ilEncoder.DefineLabel();
        var doneLabel = ilEncoder.DefineLabel();

        // Receiver type-test
        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.OpCode(ILOpCode.Isinst);
        ilEncoder.Token(instruction.ReceiverTypeHandle);
        ilEncoder.OpCode(ILOpCode.Dup);
        ilEncoder.Branch(ILOpCode.Brfalse, fallbackLabel);

        // Direct call path (typed receiver on stack)
        if (instruction.HasScopesParameter)
        {
            EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
        }

        int jsParamCount = instruction.MaxParamCount;
        int argsToPass = Math.Min(instruction.Arguments.Count, jsParamCount);
        for (int i = 0; i < argsToPass; i++)
        {
            EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
        }

        for (int i = argsToPass; i < jsParamCount; i++)
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }

        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(instruction.MethodHandle);

        if (IsMaterialized(instruction.Result, allocation))
        {
            var resultStorage = GetTempStorage(instruction.Result);
            if (resultStorage.Kind == ValueStorageKind.Reference
                && resultStorage.ClrType == typeof(object)
                && instruction.ReturnClrType != typeof(object)
                && instruction.ReturnClrType.IsValueType)
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(instruction.ReturnClrType));
            }

            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.Branch(ILOpCode.Br, doneLabel);

        // Fallback: pop null typed receiver and do runtime dispatch
        ilEncoder.MarkLabel(fallbackLabel);
        ilEncoder.OpCode(ILOpCode.Pop);

        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.MethodName);
        EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);

        var callMemberRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Object),
            nameof(JavaScriptRuntime.Object.CallMember),
            new[] { typeof(object), typeof(string), typeof(object[]) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(callMemberRef);

        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.MarkLabel(doneLabel);
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

    private void EmitIntrinsicStaticVoidCall(
        LIRCallIntrinsicStaticVoid instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

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
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);

        // Statement-level call: ensure no value is left on stack.
        if (chosen.ReturnType != typeof(void))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
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

    private void EmitOperatorsAddObjectObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(object), typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsAddDoubleObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(double), typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsAddObjectDouble(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(object), typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsMultiply(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Multiply));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitMathPow(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(System.Math), nameof(System.Math.Pow));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyDouble(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyBool(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(bool) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIn(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.In));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Equal));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.NotEqual));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.StrictEqual));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.StrictNotEqual));
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
    private EntityHandle ResolveFieldToken(string scopeName, string fieldName, string context)
    {
        try
        {
            if (TryResolveAsyncScopeBaseFieldToken(fieldName, out var token))
                return token;

            if (TryResolveGeneratorScopeBaseFieldToken(fieldName, out token))
                return token;

            return _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve field token for '{fieldName}' in scope '{scopeName}' during {context}.",
                ex);
        }
    }

    /// <summary>
    /// Emits IL to load a field by name from the scope instance (local 0).
    /// Assumes the scope instance is already on the stack.
    /// </summary>
    private void EmitLoadFieldByName(InstructionEncoder ilEncoder, string scopeName, string fieldName)
    {
        var fieldHandle = TryResolveAsyncScopeBaseFieldToken(fieldName, out var token)
            ? token
            : TryResolveGeneratorScopeBaseFieldToken(fieldName, out token)
                ? token
                : _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        ilEncoder.OpCode(ILOpCode.Ldfld);
        ilEncoder.Token(fieldHandle);
    }

    /// <summary>
    /// Emits IL to store to a field by name on the scope instance.
    /// Assumes the scope instance and value are on the stack (scope, value).
    /// </summary>
    private void EmitStoreFieldByName(InstructionEncoder ilEncoder, string scopeName, string fieldName)
    {
        var fieldHandle = TryResolveAsyncScopeBaseFieldToken(fieldName, out var token)
            ? token
            : TryResolveGeneratorScopeBaseFieldToken(fieldName, out token)
                ? token
                : _scopeMetadataRegistry.GetFieldHandle(scopeName, fieldName);
        ilEncoder.OpCode(ILOpCode.Stfld);
        ilEncoder.Token(fieldHandle);
    }

    /// <summary>
    /// Emits the state switch at the entry of an async function.
    /// This dispatches to the appropriate resume point based on _asyncState.
    /// State 0 = initial entry (fall through to function body)
    /// State 1, 2, 3, ... = resume points after each await
    /// </summary>
    private void EmitAsyncStateSwitch(
        InstructionEncoder ilEncoder,
        Dictionary<int, LabelHandle> labelMap,
        AsyncStateMachineInfo asyncInfo)
    {
        var scopeName = MethodBody.LeafScopeId.Name;
        
        // Load _asyncState from scope instance (local 0)
        ilEncoder.LoadLocal(0);
        EmitLoadFieldByName(ilEncoder, scopeName, "_asyncState");
        
        // Build switch table for resume states.
        // The switch instruction expects targets for cases 0, 1, 2, ...
        // Case 0 = initial entry (fall through - we'll use a label that goes right after the switch)
        // Case 1, 2, ... = resume points
        
        var fallThroughLabel = ilEncoder.DefineLabel();
        int branchCount = asyncInfo.MaxResumeStateId + 1;
        
        // Collect switch targets
        var switchTargets = new LabelHandle[branchCount];
        
        // Default all cases to fall through to function body
        for (int i = 0; i < branchCount; i++)
        {
            switchTargets[i] = fallThroughLabel;
        }
        
        // Cases 1, 2, 3, ...: jump to resume labels
        foreach (var kvp in asyncInfo.ResumeLabels)
        {
            var stateId = kvp.Key;
            var labelId = kvp.Value;
            if (stateId <= 0 || stateId >= branchCount)
            {
                continue;
            }
            if (!labelMap.TryGetValue(labelId, out var resumeLabel))
            {
                resumeLabel = ilEncoder.DefineLabel();
                labelMap[labelId] = resumeLabel;
            }
            switchTargets[stateId] = resumeLabel;
        }
        
        // Emit switch instruction using SwitchInstructionEncoder
        var switchEncoder = ilEncoder.Switch(branchCount);
        for (int i = 0; i < branchCount; i++)
        {
            switchEncoder.Branch(switchTargets[i]);
        }
        
        // Mark the fall-through label (for state 0 or values > max state)
        ilEncoder.MarkLabel(fallThroughLabel);
    }

    #endregion
}
