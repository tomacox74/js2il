using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Jroc.Services.ILGenerators;
using Jroc.Services.VariableBindings;
using Jroc.Utilities.Ecma335;

namespace Jroc.Services.TwoPhaseCompilation;

internal sealed class GeneratedFunctionObjectEmitter
{
    private readonly MetadataBuilder _metadataBuilder;
    private readonly BaseClassLibraryReferences _bclReferences;
    private readonly MethodBodyStreamEncoder _methodBodyStream;
    private readonly ScopeMetadataRegistry _scopeMetadata;
    private readonly CallableRegistry _callableRegistry;
    private readonly GeneratedFunctionObjectRegistry _functionObjectRegistry;
    private readonly FunctionTypeMetadataRegistry _functionTypeRegistry;
    private readonly AnonymousCallableTypeMetadataRegistry _anonymousTypeRegistry;
    private readonly ClassRegistry _classRegistry;
    private readonly NestedTypeRelationshipRegistry _nestedTypeRegistry;

    public GeneratedFunctionObjectEmitter(
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStream,
        ScopeMetadataRegistry scopeMetadata,
        CallableRegistry callableRegistry,
        GeneratedFunctionObjectRegistry functionObjectRegistry,
        FunctionTypeMetadataRegistry functionTypeRegistry,
        AnonymousCallableTypeMetadataRegistry anonymousTypeRegistry,
        ClassRegistry classRegistry,
        NestedTypeRelationshipRegistry nestedTypeRegistry)
    {
        _metadataBuilder = metadataBuilder;
        _bclReferences = bclReferences;
        _methodBodyStream = methodBodyStream;
        _scopeMetadata = scopeMetadata;
        _callableRegistry = callableRegistry;
        _functionObjectRegistry = functionObjectRegistry;
        _functionTypeRegistry = functionTypeRegistry;
        _anonymousTypeRegistry = anonymousTypeRegistry;
        _classRegistry = classRegistry;
        _nestedTypeRegistry = nestedTypeRegistry;
    }

    public void DeclareTypes(int firstMethodRow)
    {
        var nextMethodRow = firstMethodRow;
        foreach (var plan in _functionObjectRegistry.GetPlansInStableOrder())
        {
            if (_functionObjectRegistry.TryGetMetadata(plan.Callable, out _))
            {
                continue;
            }

            var typeBuilder = new TypeBuilder(_metadataBuilder, plan.Namespace, plan.TypeName);
            var fieldHandles = new Dictionary<string, FieldDefinitionHandle>(StringComparer.Ordinal);

            foreach (var capture in plan.Captures)
            {
                if (!_scopeMetadata.TryGetScopeTypeHandle(capture.ScopeName, out var scopeType)
                    || scopeType.IsNil)
                {
                    throw new InvalidOperationException(
                        $"Missing scope type '{capture.ScopeName}' while declaring generated function object '{plan.TypeName}'.");
                }

                fieldHandles[capture.FieldName] = typeBuilder.AddFieldDefinition(
                    FieldAttributes.Private | FieldAttributes.InitOnly,
                    capture.FieldName,
                    CreateFieldSignature(scopeType));
            }

            foreach (var state in plan.StateFields)
            {
                var stateType = GetStateTypeHandle(state.Kind);
                fieldHandles[state.FieldName] = typeBuilder.AddFieldDefinition(
                    FieldAttributes.Private | FieldAttributes.InitOnly,
                    state.FieldName,
                    CreateFieldSignature(stateType));
            }

            var constructor = MetadataTokens.MethodDefinitionHandle(nextMethodRow++);
            var isConstructorGetter = MetadataTokens.MethodDefinitionHandle(nextMethodRow++);
            var requiresContextGetter = MetadataTokens.MethodDefinitionHandle(nextMethodRow++);
            var ordinaryThisResolver = plan.UsesNonStrictThisBinding
                ? MetadataTokens.MethodDefinitionHandle(nextMethodRow++)
                : default;
            var stateAccessorHandles =
                new Dictionary<GeneratedFunctionStateKind, MethodDefinitionHandle>();
            foreach (var state in plan.StateFields)
            {
                if (TryGetStateAccessorName(state.Kind, out _))
                {
                    stateAccessorHandles[state.Kind] =
                        MetadataTokens.MethodDefinitionHandle(nextMethodRow++);
                }
            }
            var arrayCallAdapter = plan.RequiresArrayCallAdapter
                && CanEmitArrayCallAdapter(plan)
                ? MetadataTokens.MethodDefinitionHandle(nextMethodRow++)
                : default;
            var callAdapter = plan.Callable.Kind == CallableKind.ClassConstructor
                ? default
                : MetadataTokens.MethodDefinitionHandle(nextMethodRow++);
            var constructBodyAdapter = plan.Callable.Kind is
                    CallableKind.FunctionDeclaration
                    or CallableKind.FunctionExpression
                && !plan.Callable.IsMethodDefinition
                && plan.IsConstructable
                ? MetadataTokens.MethodDefinitionHandle(nextMethodRow++)
                : default;
            var constructAdapter = plan.IsConstructable
                && plan.Callable.Kind != CallableKind.ClassConstructor
                ? MetadataTokens.MethodDefinitionHandle(nextMethodRow++)
                : default;

            var typeHandle = typeBuilder.AddTypeDefinition(
                TypeAttributes.NestedPublic
                | TypeAttributes.Class
                | TypeAttributes.Sealed
                | TypeAttributes.BeforeFieldInit,
                plan.Callable.Kind == CallableKind.ClassConstructor
                    ? _bclReferences.JsClassConstructorObjectType
                    : plan.ReturnKind == GeneratedFunctionReturnKind.Promise
                        ? _bclReferences.JsAsyncFunctionObjectType
                        : _bclReferences.JsFunctionObjectType,
                firstFieldOverride: null,
                firstMethodOverride: constructor);

            if (!_callableRegistry.TryGetDeclaredToken(plan.Callable, out var bodyToken)
                || bodyToken.Kind != HandleKind.MethodDefinition)
            {
                throw new InvalidOperationException(
                    $"Missing canonical body token for generated function object '{plan.Callable.DisplayName}'.");
            }

            var canonicalBody = (MethodDefinitionHandle)bodyToken;
            var ownerType = ResolveCanonicalOwnerType(plan);
            _nestedTypeRegistry.Add(typeHandle, ownerType);
            _functionObjectRegistry.SetMetadata(new GeneratedFunctionObjectMetadata
            {
                Plan = plan,
                TypeHandle = typeHandle,
                CanonicalOwnerTypeHandle = ownerType,
                ConstructorHandle = constructor,
                IsConstructorGetterHandle = isConstructorGetter,
                RequiresInvocationContextGetterHandle = requiresContextGetter,
                OrdinaryThisResolverHandle = ordinaryThisResolver,
                StateAccessorHandles = stateAccessorHandles,
                ArrayCallAdapterHandle = arrayCallAdapter,
                CallAdapterHandle = callAdapter,
                ConstructBodyAdapterHandle = constructBodyAdapter,
                ConstructAdapterHandle = constructAdapter,
                FieldHandles = fieldHandles,
                EntryPoints =
                [
                    new GeneratedFunctionEntryPointPlan(
                        plan.Signature.ILMethodName,
                        plan.Signature.ParameterClrTypes,
                        plan.Signature.ReturnClrType,
                        canonicalBody)
                ]
            });
        }
    }

    public void EmitMethods()
    {
        foreach (var metadata in _functionObjectRegistry.GetMetadataInStableOrder())
        {
            EmitAndValidate(
                metadata.ConstructorHandle,
                EmitConstructor(metadata),
                metadata.Plan,
                ".ctor");
            EmitAndValidate(
                metadata.IsConstructorGetterHandle,
                EmitBooleanGetter(metadata, "get_IsConstructor", metadata.Plan.IsConstructable),
                metadata.Plan,
                "get_IsConstructor");
            EmitAndValidate(
                metadata.RequiresInvocationContextGetterHandle,
                EmitBooleanGetter(
                    metadata,
                    "get_RequiresInvocationContext",
                    metadata.Plan.RequiresInvocationContext),
                metadata.Plan,
                "get_RequiresInvocationContext");
            if (!metadata.OrdinaryThisResolverHandle.IsNil)
            {
                EmitAndValidate(
                    metadata.OrdinaryThisResolverHandle,
                    EmitOrdinaryThisResolver(metadata),
                    metadata.Plan,
                    "ResolveThisArgumentCore");
            }
            foreach (var state in metadata.Plan.StateFields)
            {
                if (!metadata.StateAccessorHandles.TryGetValue(
                        state.Kind,
                        out var expectedAccessor))
                {
                    continue;
                }

                EmitAndValidate(
                    expectedAccessor,
                    EmitStateAccessor(metadata, state),
                    metadata.Plan,
                    GetStateAccessorName(state.Kind));
            }
            if (!metadata.ArrayCallAdapterHandle.IsNil)
            {
                EmitAndValidate(
                    metadata.ArrayCallAdapterHandle,
                    EmitArrayCallAdapter(metadata),
                    metadata.Plan,
                    "__js_call_with_arguments__");
            }
            if (!metadata.CallAdapterHandle.IsNil)
            {
                EmitAndValidate(
                    metadata.CallAdapterHandle,
                    EmitCallAdapter(metadata),
                    metadata.Plan,
                    metadata.Plan.ReturnKind
                        == GeneratedFunctionReturnKind.Promise
                        ? "CallCoreAsync"
                        : "CallCore");
            }

            if (!metadata.ConstructBodyAdapterHandle.IsNil)
            {
                EmitAndValidate(
                    metadata.ConstructBodyAdapterHandle,
                    EmitConstructBodyAdapter(metadata),
                    metadata.Plan,
                    "ConstructBodyCore");
            }

            if (!metadata.ConstructAdapterHandle.IsNil)
            {
                EmitAndValidate(
                    metadata.ConstructAdapterHandle,
                    EmitConstructAdapter(metadata),
                    metadata.Plan,
                    "ConstructCore");
            }
        }
    }

    private MethodDefinitionHandle EmitConstructor(GeneratedFunctionObjectMetadata metadata)
    {
        var parameterTypes = GetConstructorParameterTypes(metadata).ToArray();
        var signature = CreateConstructorSignature(parameterTypes);
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        encoder.OpCode(ILOpCode.Ldarg_0);
        encoder.Call(
            metadata.Plan.Callable.Kind == CallableKind.ClassConstructor
                ? _bclReferences.JsClassConstructorObject_Ctor_Ref
                : metadata.Plan.ReturnKind == GeneratedFunctionReturnKind.Promise
                    ? _bclReferences.JsAsyncFunctionObject_Ctor_Ref
                    : _bclReferences.JsFunctionObject_Ctor_Ref);

        for (var index = 0; index < parameterTypes.Length; index++)
        {
            var fieldName = index < metadata.Plan.Captures.Count
                ? metadata.Plan.Captures[index].FieldName
                : metadata.Plan.StateFields[index - metadata.Plan.Captures.Count].FieldName;
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.LoadArgument(index + 1);
            encoder.OpCode(ILOpCode.Stfld);
            encoder.Token(metadata.FieldHandles[fieldName]);
        }

        encoder.OpCode(ILOpCode.Ret);
        var bodyOffset = AddMethodBody(encoder);
        var parameters = parameterTypes
            .Select((_, index) => $"capture{index}")
            .ToArray();
        return AddMethod(
            metadata,
            MethodAttributes.Public
            | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName
            | MethodAttributes.RTSpecialName,
            ".ctor",
            signature,
            bodyOffset,
            parameters);
    }

    private MethodDefinitionHandle EmitBooleanGetter(
        GeneratedFunctionObjectMetadata metadata,
        string name,
        bool value)
    {
        var signature = CreateBooleanGetterSignature();
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        encoder.LoadConstantI4(value ? 1 : 0);
        encoder.OpCode(ILOpCode.Ret);
        return AddMethod(
            metadata,
            MethodAttributes.Public
            | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName
            | MethodAttributes.Virtual,
            name,
            signature,
            AddMethodBody(encoder),
            Array.Empty<string>());
    }

    private MethodDefinitionHandle EmitCallAdapter(GeneratedFunctionObjectMetadata metadata)
    {
        var returnsPromise = metadata.Plan.ReturnKind
            == GeneratedFunctionReturnKind.Promise;
        var signature = CreateCallAdapterSignature(returnsPromise);
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);

        if (metadata.Plan.Callable.Kind == CallableKind.ClassConstructor)
        {
            EmitTypeError(encoder, "Class constructor cannot be invoked without 'new'");
        }
        else
        {
            EmitCanonicalCall(metadata, encoder);
            if (returnsPromise)
            {
                encoder.OpCode(ILOpCode.Castclass);
                encoder.Token(_bclReferences.PromiseType);
            }
            if (metadata.Plan.ReturnKind
                    == GeneratedFunctionReturnKind.AsyncGenerator
                || metadata.Plan.ReturnKind
                    == GeneratedFunctionReturnKind.Generator
                    && HasSimpleGeneratorParameters(
                        metadata.Plan.Callable.AstNode))
            {
                encoder.OpCode(ILOpCode.Ldarg_0);
                encoder.Call(
                    _bclReferences
                        .GeneratorObject_InitializeInstanceFromFunction_Ref);
            }
            encoder.OpCode(ILOpCode.Ret);
        }

        return AddMethod(
            metadata,
            MethodAttributes.Family
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual,
            metadata.Plan.ReturnKind
                == GeneratedFunctionReturnKind.Promise
                ? "CallCoreAsync"
                : "CallCore",
            signature,
            AddMethodBody(
                encoder,
                maxStack: System.Math.Max(
                    8,
                    metadata.Plan.Signature.JsParamCount + 3)),
            ["thisArgument", "arguments"],
            inParameterIndex: 1);
    }

    private MethodDefinitionHandle EmitArrayCallAdapter(
        GeneratedFunctionObjectMetadata metadata)
    {
        return AddMethod(
            metadata,
            MethodAttributes.Assembly
            | MethodAttributes.Static
            | MethodAttributes.HideBySig,
            "__js_call_with_arguments__",
            CreateArrayCallAdapterSignature(),
            EmitArrayCallAdapterWithInvocationContext(metadata),
            ["functionObject", "scopes", "arguments"]);
    }

    private int EmitArrayCallAdapterWithInvocationContext(
        GeneratedFunctionObjectMetadata metadata)
    {
        var il = new BlobBuilder();
        var controlFlow = new ControlFlowBuilder();
        var encoder = new InstructionEncoder(il, controlFlow);
        var tryStart = encoder.DefineLabel();
        var tryEnd = encoder.DefineLabel();
        var finallyStart = encoder.DefineLabel();
        var finallyEnd = encoder.DefineLabel();
        var returnLabel = encoder.DefineLabel();

        encoder.OpCode(ILOpCode.Ldarg_0);
        encoder.OpCode(ILOpCode.Ldarg_2);
        encoder.Call(
            _bclReferences
                .RuntimeServices_PushGeneratedFunctionDirectCall_Ref);

        encoder.MarkLabel(tryStart);
        EmitCanonicalArrayCall(metadata, encoder);
        encoder.StoreLocal(0);
        encoder.Branch(ILOpCode.Leave, returnLabel);
        encoder.MarkLabel(tryEnd);

        encoder.MarkLabel(finallyStart);
        encoder.Call(
            _bclReferences
                .RuntimeServices_PopGeneratedFunctionDirectCall_Ref);
        encoder.OpCode(ILOpCode.Endfinally);
        encoder.MarkLabel(finallyEnd);

        encoder.MarkLabel(returnLabel);
        encoder.LoadLocal(0);
        encoder.OpCode(ILOpCode.Ret);

        controlFlow.AddFinallyRegion(
            tryStart,
            tryEnd,
            finallyStart,
            finallyEnd);

        return AddMethodBody(
            encoder,
            maxStack: System.Math.Max(
                8,
                metadata.Plan.Signature.JsParamCount + 3),
            localVariablesSignature: CreateArrayCallAdapterLocalSignature());
    }

    private void EmitCanonicalArrayCall(
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder encoder)
    {
        var plan = metadata.Plan;
        if (!CanEmitArrayCallAdapter(plan))
        {
            throw new InvalidOperationException(
                $"Callable '{plan.Callable.DisplayName}' does not support the static array-call adapter.");
        }

        switch (plan.Signature.ScopeAbiKind)
        {
            case Jroc.Runtime.CallableScopeAbiKind.NoScopes:
                break;

            case Jroc.Runtime.CallableScopeAbiKind.SingleScope:
                var capture = plan.Captures.Single();
                encoder.OpCode(ILOpCode.Ldarg_1);
                encoder.LoadConstantI4(capture.ScopeIndex);
                encoder.OpCode(ILOpCode.Ldelem_ref);
                encoder.OpCode(ILOpCode.Castclass);
                encoder.Token(
                    _scopeMetadata.GetScopeTypeHandle(
                        capture.ScopeName));
                break;

            case Jroc.Runtime.CallableScopeAbiKind.ScopeArray:
                encoder.OpCode(ILOpCode.Ldarg_1);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported callable scope ABI '{plan.Signature.ScopeAbiKind}'.");
        }

        // Direct function calls supply undefined new.target. Arrow lexical new.target is
        // carried by the generated function object's ambient invocation context.
        encoder.OpCode(ILOpCode.Ldnull);
        for (var index = 0;
             index < plan.Signature.JsParamCount;
             index++)
        {
            encoder.OpCode(ILOpCode.Ldarg_2);
            encoder.LoadConstantI4(index);
            encoder.Call(
                _bclReferences
                    .RuntimeServices_GetArgumentOrUndefined_Ref);
            EmitDynamicArgumentConversion(
                encoder,
                plan.Signature.ParameterClrTypes.ElementAtOrDefault(index));
        }

        encoder.OpCode(ILOpCode.Call);
        encoder.Token(metadata.EntryPoints[0].MethodHandle);
        EmitReturnAdaptation(
            encoder,
            plan.Signature.ReturnClrType);
    }

    private static bool CanEmitArrayCallAdapter(
        GeneratedFunctionObjectPlan plan)
        => !plan.Signature.IsInstanceMethod
            && plan.Callable.Kind is
                CallableKind.FunctionDeclaration
                or CallableKind.FunctionExpression
                or CallableKind.Arrow;

    private static bool HasSimpleGeneratorParameters(Node? callableNode)
    {
        var parameters = callableNode switch
        {
            FunctionDeclaration function => function.Params,
            FunctionExpression function => function.Params,
            Acornima.Ast.MethodDefinition
            {
                Value: FunctionExpression function
            } => function.Params,
            _ => default
        };

        return parameters.Count == 0
            || parameters.All(parameter => parameter is Identifier);
    }

    private MethodDefinitionHandle EmitStateAccessor(
        GeneratedFunctionObjectMetadata metadata,
        GeneratedFunctionStatePlan state)
    {
        var name = GetStateAccessorName(state.Kind);
        var returnType = GetStateTypeHandle(state.Kind);
        var hasThisArgument = state.Kind == GeneratedFunctionStateKind.LexicalThis;
        var signature = CreateStateAccessorSignature(returnType, hasThisArgument);
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        encoder.OpCode(ILOpCode.Ldarg_0);
        encoder.OpCode(ILOpCode.Ldfld);
        encoder.Token(metadata.FieldHandles[state.FieldName]);
        encoder.OpCode(ILOpCode.Ret);

        return AddMethod(
            metadata,
            MethodAttributes.Family
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual,
            name,
            signature,
            AddMethodBody(encoder),
            hasThisArgument ? ["thisArgument"] : Array.Empty<string>());
    }

    private MethodDefinitionHandle EmitOrdinaryThisResolver(
        GeneratedFunctionObjectMetadata metadata)
    {
        var signature = CreateStateAccessorSignature(
            _bclReferences.ObjectType,
            hasThisArgument: true);
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        encoder.OpCode(ILOpCode.Ldarg_1);
        encoder.Call(_bclReferences.Function_ResolveOrdinaryThisArgument_Ref);
        encoder.OpCode(ILOpCode.Ret);

        return AddMethod(
            metadata,
            MethodAttributes.Family
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual,
            "ResolveThisArgumentCore",
            signature,
            AddMethodBody(encoder),
            ["thisArgument"]);
    }

    private MethodDefinitionHandle EmitConstructAdapter(
        GeneratedFunctionObjectMetadata metadata)
    {
        var signature = CreateConstructAdapterSignature();
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        if (metadata.Plan.Callable.Kind is CallableKind.FunctionDeclaration
            or CallableKind.FunctionExpression)
        {
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldarg_1);
            encoder.OpCode(ILOpCode.Ldobj);
            encoder.Token(_bclReferences.JsCallArgumentsType);
            encoder.OpCode(ILOpCode.Ldarg_2);
            encoder.Call(_bclReferences.Function_ConstructGeneratedFunctionObject_Ref);
            encoder.OpCode(ILOpCode.Ret);
        }
        else
        {
            EmitTypeError(
                encoder,
                "Generated construction adapter is reserved for the callable-family construction migration");
        }

        return AddMethod(
            metadata,
            MethodAttributes.Family
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual,
            "ConstructCore",
            signature,
            AddMethodBody(encoder),
            ["arguments", "newTarget"],
            inParameterIndex: 0);
    }

    private MethodDefinitionHandle EmitConstructBodyAdapter(
        GeneratedFunctionObjectMetadata metadata)
    {
        var signature = CreateConstructBodyAdapterSignature();
        var il = new BlobBuilder();
        var encoder = new InstructionEncoder(il);
        EmitCanonicalCall(
            metadata,
            encoder,
            thisArgumentIndex: 1,
            argumentsIndex: 2,
            newTargetIndex: 3);
        encoder.OpCode(ILOpCode.Ret);

        return AddMethod(
            metadata,
            MethodAttributes.Family
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual,
            "ConstructBodyCore",
            signature,
            AddMethodBody(
                encoder,
                maxStack: System.Math.Max(
                    8,
                    metadata.Plan.Signature.JsParamCount + 3)),
            ["receiver", "arguments", "newTarget"],
            inParameterIndex: 1);
    }

    private void EmitCanonicalCall(
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder encoder,
        int thisArgumentIndex = 1,
        int argumentsIndex = 2,
        int? newTargetIndex = null)
    {
        var plan = metadata.Plan;
        if (plan.Callable.Kind is CallableKind.ClassStaticMethod
                or CallableKind.ClassStaticGetter
                or CallableKind.ClassStaticSetter
            && plan.StateFields.Any(state =>
                state.Kind == GeneratedFunctionStateKind.PrivateBrand))
        {
            encoder.LoadArgument(thisArgumentIndex);
            encoder.OpCode(ILOpCode.Ldtoken);
            encoder.Token(metadata.CanonicalOwnerTypeHandle);
            encoder.Call(_bclReferences.Type_GetTypeFromHandle_Ref);
            EmitPrivateBrand(metadata, encoder);
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.Call(
                _bclReferences.RuntimeServices_ValidateGeneratedStaticMethodReceiver_Ref);
            encoder.OpCode(ILOpCode.Pop);
        }

        if (plan.Signature.IsInstanceMethod)
        {
            if (plan.Callable.Kind is CallableKind.ClassMethod
                or CallableKind.ClassGetter
                or CallableKind.ClassSetter)
            {
                encoder.LoadArgument(thisArgumentIndex);
                encoder.OpCode(ILOpCode.Ldtoken);
                encoder.Token(metadata.CanonicalOwnerTypeHandle);
                encoder.Call(_bclReferences.Type_GetTypeFromHandle_Ref);
                EmitScopeArray(metadata, encoder);
                EmitPrivateBrand(metadata, encoder);
                encoder.OpCode(ILOpCode.Ldarg_0);
                encoder.Call(
                    _bclReferences.RuntimeServices_ResolveGeneratedClassMethodReceiver_Ref);
                encoder.OpCode(ILOpCode.Castclass);
                encoder.Token(metadata.CanonicalOwnerTypeHandle);
            }
            else
            {
                encoder.LoadArgument(thisArgumentIndex);
                encoder.OpCode(ILOpCode.Castclass);
                encoder.Token(metadata.CanonicalOwnerTypeHandle);
            }
        }

        EmitScopePayload(metadata, encoder);

        if (plan.Callable.Kind is CallableKind.FunctionDeclaration
            or CallableKind.FunctionExpression
            or CallableKind.Arrow)
        {
            var lexicalNewTarget = plan.StateFields.FirstOrDefault(
                state => state.Kind == GeneratedFunctionStateKind.LexicalNewTarget);
            if (plan.Callable.Kind == CallableKind.Arrow
                && lexicalNewTarget is not null)
            {
                encoder.OpCode(ILOpCode.Ldarg_0);
                encoder.OpCode(ILOpCode.Ldfld);
                encoder.Token(metadata.FieldHandles[lexicalNewTarget.FieldName]);
            }
            else if (plan.Callable.Kind is CallableKind.FunctionDeclaration
                or CallableKind.FunctionExpression)
            {
                if (newTargetIndex.HasValue)
                {
                    encoder.LoadArgument(newTargetIndex.Value);
                }
                else
                {
                    encoder.Call(_bclReferences.RuntimeServices_GetCurrentNewTarget_Ref);
                }
            }
            else
            {
                encoder.OpCode(ILOpCode.Ldnull);
            }
        }
        else if ((plan.Callable.Kind is CallableKind.ClassMethod
                or CallableKind.ClassGetter
                or CallableKind.ClassSetter
                or CallableKind.ClassStaticMethod
                or CallableKind.ClassStaticGetter
                or CallableKind.ClassStaticSetter)
            && plan.Signature.ScopeAbiKind
                != Jroc.Runtime.CallableScopeAbiKind.NoScopes)
        {
            encoder.OpCode(ILOpCode.Ldnull);
        }

        for (var index = 0; index < plan.Signature.JsParamCount; index++)
        {
            encoder.LoadArgument(argumentsIndex);
            encoder.LoadConstantI4(index);
            encoder.Call(_bclReferences.JsCallArguments_GetArgument_Ref);
            EmitDynamicArgumentConversion(encoder, plan.Signature.ParameterClrTypes.ElementAtOrDefault(index));
        }

        encoder.OpCode(ILOpCode.Call);
        encoder.Token(metadata.EntryPoints[0].MethodHandle);
        EmitReturnAdaptation(encoder, plan.Signature.ReturnClrType);
    }

    private void EmitPrivateBrand(
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder encoder)
    {
        var privateBrand = metadata.Plan.StateFields.FirstOrDefault(
            state => state.Kind == GeneratedFunctionStateKind.PrivateBrand);
        if (privateBrand is null)
        {
            encoder.OpCode(ILOpCode.Ldnull);
            return;
        }

        encoder.OpCode(ILOpCode.Ldarg_0);
        encoder.OpCode(ILOpCode.Ldfld);
        encoder.Token(metadata.FieldHandles[privateBrand.FieldName]);
    }

    private void EmitScopeArray(
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder encoder)
    {
        var transitionalScopes = metadata.Plan.StateFields.FirstOrDefault(
            state => state.Kind == GeneratedFunctionStateKind.TransitionalScopeArray);
        if (transitionalScopes is not null)
        {
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldfld);
            encoder.Token(metadata.FieldHandles[transitionalScopes.FieldName]);
            return;
        }

        encoder.LoadConstantI4(
            System.Math.Max(1, metadata.Plan.ScopeChainSlotCount));
        encoder.OpCode(ILOpCode.Newarr);
        encoder.Token(_bclReferences.ObjectType);
        foreach (var capture in metadata.Plan.Captures)
        {
            encoder.OpCode(ILOpCode.Dup);
            encoder.LoadConstantI4(capture.ScopeIndex);
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldfld);
            encoder.Token(metadata.FieldHandles[capture.FieldName]);
            encoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    private void EmitScopePayload(
        GeneratedFunctionObjectMetadata metadata,
        InstructionEncoder encoder)
    {
        var plan = metadata.Plan;
        if (plan.Signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.NoScopes)
        {
            return;
        }

        if (plan.Signature.ScopeAbiKind == Jroc.Runtime.CallableScopeAbiKind.SingleScope)
        {
            var capture = plan.Captures.Single();
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldfld);
            encoder.Token(metadata.FieldHandles[capture.FieldName]);
            return;
        }

        var transitionalScopes = plan.StateFields.FirstOrDefault(
            state => state.Kind == GeneratedFunctionStateKind.TransitionalScopeArray);
        if (transitionalScopes is not null)
        {
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldfld);
            encoder.Token(metadata.FieldHandles[transitionalScopes.FieldName]);
            return;
        }

        encoder.LoadConstantI4(plan.ScopeChainSlotCount);
        encoder.OpCode(ILOpCode.Newarr);
        encoder.Token(_bclReferences.ObjectType);
        foreach (var capture in plan.Captures)
        {
            encoder.OpCode(ILOpCode.Dup);
            encoder.LoadConstantI4(capture.ScopeIndex);
            encoder.OpCode(ILOpCode.Ldarg_0);
            encoder.OpCode(ILOpCode.Ldfld);
            encoder.Token(metadata.FieldHandles[capture.FieldName]);
            encoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    private void EmitDynamicArgumentConversion(InstructionEncoder encoder, Type? targetType)
    {
        if (targetType == typeof(double))
        {
            encoder.Call(_bclReferences.TypeUtilities_ToNumber_Object_Ref);
        }
        else if (targetType == typeof(bool))
        {
            encoder.Call(_bclReferences.TypeUtilities_ToBoolean_Object_Ref);
        }
        else if (targetType == typeof(string))
        {
            encoder.Call(_bclReferences.DotNet2JSConversions_ToString_Ref);
        }
        else if (targetType == typeof(JavaScriptRuntime.Array))
        {
            encoder.OpCode(ILOpCode.Castclass);
            encoder.Token(_bclReferences.TypeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Array)));
        }
    }

    private void EmitReturnAdaptation(InstructionEncoder encoder, Type? returnType)
    {
        if (returnType == typeof(double))
        {
            encoder.OpCode(ILOpCode.Box);
            encoder.Token(_bclReferences.DoubleType);
        }
        else if (returnType == typeof(bool))
        {
            encoder.OpCode(ILOpCode.Box);
            encoder.Token(_bclReferences.BooleanType);
        }
    }

    private void EmitTypeError(InstructionEncoder encoder, string message)
    {
        encoder.Ldstr(_metadataBuilder, message);
        encoder.OpCode(ILOpCode.Newobj);
        encoder.Token(_bclReferences.TypeError_Ctor_String_Ref);
        encoder.OpCode(ILOpCode.Throw);
    }

    private IEnumerable<EntityHandle> GetConstructorParameterTypes(
        GeneratedFunctionObjectMetadata metadata)
    {
        foreach (var capture in metadata.Plan.Captures)
        {
            yield return _scopeMetadata.GetScopeTypeHandle(capture.ScopeName);
        }

        foreach (var state in metadata.Plan.StateFields)
        {
            yield return GetStateTypeHandle(state.Kind);
        }
    }

    private TypeDefinitionHandle ResolveCanonicalOwnerType(
        GeneratedFunctionObjectPlan plan)
    {
        var callable = plan.Callable;
        var moduleName = plan.ModuleName;
        if (callable.Kind == CallableKind.FunctionDeclaration)
        {
            var functionName = plan.CanonicalOwnerTypeName;
            if (_functionTypeRegistry.TryGet(
                    moduleName,
                    callable.DeclaringScopeName,
                    functionName,
                    out var owner))
            {
                return owner;
            }
        }
        else if (callable.Kind is CallableKind.FunctionExpression or CallableKind.Arrow)
        {
            var ownerName = plan.CanonicalOwnerTypeName;
            if (_anonymousTypeRegistry.TryGetOwnerTypeHandle(
                    moduleName,
                    callable.DeclaringScopeName,
                    ownerName,
                    out var owner))
            {
                return owner;
            }
        }
        else
        {
            var className = plan.CanonicalOwnerTypeName;
            if (!string.IsNullOrWhiteSpace(className)
                && (_classRegistry.TryGet(className!, out var owner)
                    || _classRegistry.TryGetBySimpleTypeName(
                        className!.Split('.').Last(),
                        out owner)))
            {
                return owner;
            }
        }

        throw new InvalidOperationException(
            $"Could not resolve canonical owner type for '{callable.DisplayName}'.");
    }

    private BlobHandle CreateFieldSignature(EntityHandle typeHandle)
    {
        var blob = new BlobBuilder();
        var encoder = new BlobEncoder(blob).Field().Type();
        if (typeHandle == _bclReferences.ObjectArrayType)
        {
            encoder.SZArray().Object();
        }
        else
        {
            encoder.Type(typeHandle, isValueType: false);
        }
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateFieldSignature(Type type)
    {
        var blob = new BlobBuilder();
        var encoder = new BlobEncoder(blob).Field().Type();
        if (type == typeof(object))
        {
            encoder.Object();
        }
        else
        {
            encoder.Type(
                _bclReferences.TypeReferenceRegistry.GetOrAdd(type),
                isValueType: type.IsValueType);
        }
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateConstructorSignature(IReadOnlyList<EntityHandle> parameterTypes)
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                parameterTypes.Count,
                returnType => returnType.Void(),
                parameters =>
                {
                    foreach (var parameterType in parameterTypes)
                    {
                        var parameterEncoder = parameters.AddParameter().Type();
                        if (parameterType == _bclReferences.ObjectArrayType)
                        {
                            parameterEncoder.SZArray().Object();
                        }
                        else
                        {
                            parameterEncoder.Type(parameterType, isValueType: false);
                        }
                    }
                });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateBooleanGetterSignature()
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                0,
                returnType => returnType.Type().Boolean(),
                _ => { });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateStateAccessorSignature(
        EntityHandle returnTypeHandle,
        bool hasThisArgument)
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                hasThisArgument ? 1 : 0,
                returnType =>
                {
                    if (returnTypeHandle == _bclReferences.ObjectType)
                    {
                        returnType.Type().Object();
                    }
                    else if (returnTypeHandle == _bclReferences.ObjectArrayType)
                    {
                        returnType.Type().SZArray().Object();
                    }
                    else
                    {
                        returnType.Type().Type(
                            returnTypeHandle,
                            isValueType: false);
                    }
                },
                parameters =>
                {
                    if (hasThisArgument)
                    {
                        parameters.AddParameter().Type().Object();
                    }
                });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private EntityHandle GetStateTypeHandle(GeneratedFunctionStateKind kind)
        => kind is GeneratedFunctionStateKind.LexicalSuperScopes
            or GeneratedFunctionStateKind.TransitionalScopeArray
            ? _bclReferences.ObjectArrayType
            : _bclReferences.ObjectType;

    private static bool TryGetStateAccessorName(
        GeneratedFunctionStateKind kind,
        out string name)
    {
        name = kind switch
        {
            GeneratedFunctionStateKind.LexicalThis => "ResolveThisArgumentCore",
            GeneratedFunctionStateKind.LexicalNewTarget => "ResolveCallNewTargetCore",
            GeneratedFunctionStateKind.HomeObject => "GetLexicalSuperReceiverCore",
            GeneratedFunctionStateKind.LexicalSuperScopes => "GetLexicalSuperScopesCore",
            _ => string.Empty
        };
        return name.Length != 0;
    }

    private static string GetStateAccessorName(GeneratedFunctionStateKind kind)
        => TryGetStateAccessorName(kind, out var name)
            ? name
            : throw new InvalidOperationException(
                $"State kind '{kind}' does not define an invocation accessor.");

    private BlobHandle CreateArrayCallAdapterSignature()
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: false)
            .Parameters(
                3,
                returnType => returnType.Type().Object(),
                parameters =>
                {
                    parameters.AddParameter().Type().Type(
                        _bclReferences.JsFunctionObjectType,
                        isValueType: false);
                    parameters.AddParameter().Type().SZArray().Object();
                    parameters.AddParameter().Type().SZArray().Object();
                });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private StandaloneSignatureHandle CreateArrayCallAdapterLocalSignature()
    {
        var blob = new BlobBuilder();
        var locals = new BlobEncoder(blob).LocalVariableSignature(1);
        locals.AddVariable().Type().Object();
        return _metadataBuilder.AddStandaloneSignature(
            _metadataBuilder.GetOrAddBlob(blob));
    }

    private BlobHandle CreateCallAdapterSignature(bool returnsPromise)
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                2,
                returnType =>
                {
                    if (returnsPromise)
                    {
                        returnType.Type().Type(
                            _bclReferences.PromiseType,
                            isValueType: false);
                    }
                    else
                    {
                        returnType.Type().Object();
                    }
                },
                parameters =>
                {
                    parameters.AddParameter().Type().Object();
                    EncodeInCallArguments(parameters.AddParameter());
                });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateConstructAdapterSignature()
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(
                2,
                returnType => returnType.Type().Object(),
                parameters =>
                {
                    EncodeInCallArguments(parameters.AddParameter());
                    parameters.AddParameter().Type().Object();
                });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private BlobHandle CreateConstructBodyAdapterSignature()
    {
        var blob = new BlobBuilder();
        new BlobEncoder(blob)
            .MethodSignature(isInstanceMethod: true)
            .Parameters(3, returnType => returnType.Type().Object(), parameters =>
            {
                parameters.AddParameter().Type().Object();
                EncodeInCallArguments(parameters.AddParameter());
                parameters.AddParameter().Type().Object();
            });
        return _metadataBuilder.GetOrAddBlob(blob);
    }

    private void EncodeInCallArguments(ParameterTypeEncoder parameter)
    {
        parameter.CustomModifiers().AddModifier(
            _bclReferences.InAttributeType,
            isOptional: false);
        parameter.Type(isByRef: true).Type(
            _bclReferences.JsCallArgumentsType,
            isValueType: true);
    }

    private int AddMethodBody(
        InstructionEncoder encoder,
        int maxStack = 8,
        StandaloneSignatureHandle localVariablesSignature = default)
    {
        return _methodBodyStream.AddMethodBody(
            encoder,
            maxStack,
            localVariablesSignature,
            attributes: localVariablesSignature.IsNil
                ? MethodBodyAttributes.None
                : MethodBodyAttributes.InitLocals);
    }

    private MethodDefinitionHandle AddMethod(
        GeneratedFunctionObjectMetadata metadata,
        MethodAttributes attributes,
        string name,
        BlobHandle signature,
        int bodyOffset,
        IReadOnlyList<string> parameterNames,
        int? inParameterIndex = null)
    {
        ParameterHandle firstParameter = default;
        for (var index = 0; index < parameterNames.Count; index++)
        {
            var parameter = _metadataBuilder.AddParameter(
                inParameterIndex == index ? ParameterAttributes.In : ParameterAttributes.None,
                _metadataBuilder.GetOrAddString(parameterNames[index]),
                (ushort)(index + 1));
            if (index == 0)
            {
                firstParameter = parameter;
            }
        }

        var typeBuilder = new TypeBuilder(
            _metadataBuilder,
            metadata.Plan.Namespace,
            metadata.Plan.TypeName);
        return typeBuilder.AddMethodDefinition(
            attributes,
            name,
            signature,
            bodyOffset,
            firstParameter);
    }

    private static void EmitAndValidate(
        MethodDefinitionHandle expected,
        MethodDefinitionHandle actual,
        GeneratedFunctionObjectPlan plan,
        string methodName)
    {
        if (actual != expected)
        {
            throw new InvalidOperationException(
                $"Generated function-object MethodDef mismatch for '{plan.Callable.DisplayName}.{methodName}'. " +
                $"Expected 0x{MetadataTokens.GetToken(expected):X8}, got 0x{MetadataTokens.GetToken(actual):X8}.");
        }
    }
}
