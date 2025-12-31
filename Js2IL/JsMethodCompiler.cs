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
    public IReadOnlyList<MethodParameterDescriptor> Parameters { get; init; }

    /// <summary>
    ///  default is to return object
    /// </summary>
    public bool ReturnsVoid { get; set; } = false;
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

        return TryCompileIRToIL(methodDescriptor, lirMethod!, methodBodyStreamEncoder);
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
            return false;
        }

        if (!HIRToLIRLowerer.TryLower(hirMethod!, out var lirMethod))
        {
            return false;
        }

        methodBody = lirMethod!;
        return true;
    }

    private MethodDefinitionHandle TryCompileIRToIL(MethodDescriptor methodDescriptor, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    { 
            var programTypeBuilder = methodDescriptor.TypeBuilder;
            var methodParameters = methodDescriptor.Parameters;
            
            // Create the method signature for the Main method with parameters
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
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
            if (!TryCompileMethodBodyToIL(methodBody, methodDescriptor.ReturnsVoid, methodBodyStreamEncoder, out var bodyOffset))
            {
                // Failed to compile IL
                return default;
            }

            var parameterNames = methodParameters.Select(p => p.Name).ToArray();

            var methodDefinitionHandle = programTypeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig,
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

            return methodDefinitionHandle;
    }

    private bool TryCompileMethodBodyToIL(MethodBodyIR methodBody, bool returnsVoid, MethodBodyStreamEncoder methodBodyStreamEncoder, out int bodyOffset)
    {
        bodyOffset = -1;
        var methodBlob = new BlobBuilder();
        var ilEncoder = new InstructionEncoder(methodBlob);

        foreach (var instruction in methodBody.Instructions)
        {
            if (!TryCompileInstructionToIL(instruction, ilEncoder))
            {
                // Failed to compile instruction
                return false;
            }
        }

        if (!returnsVoid)
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }
        ilEncoder.OpCode(ILOpCode.Ret);

        var LocalVariablesSignature = CreateLocalVariablesSignature(methodBody);

        var bodyAttributes = MethodBodyAttributes.None;
        if (methodBody.Locals.Count > 0)
        {
            bodyAttributes |= MethodBodyAttributes.InitLocals;
        }

        bodyOffset = methodBodyStreamEncoder.AddMethodBody(
                ilEncoder,
                maxStack: 32,
                localVariablesSignature: LocalVariablesSignature,
                attributes: bodyAttributes);

        return true;
    }

    private bool TryCompileInstructionToIL(LIRInstruction instruction, InstructionEncoder ilEncoder)
    {
        switch (instruction)
        {
            case LIRAddNumber:
                ilEncoder.OpCode(ILOpCode.Add);
                break;
            case LIRBeginInitArrayElement beginInitArrayElement:
                // set the table to store a array element reference
                ilEncoder.OpCode(ILOpCode.Dup);

                // set the index to store at
                ilEncoder.LoadConstantI4(beginInitArrayElement.Index);
                break;
            case LIRConstNumber constNumber:
                ilEncoder.LoadConstantR8(constNumber.Value);
                break;
            case LIRConstString constString:
                ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(constString.Value));
                break;
            case LIRConstUndefined:
                ilEncoder.OpCode(ILOpCode.Ldnull);
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                break;
            case LIRCallIntrinsic callIntrinsic:
                EmitInvokeInstrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);
                break;
            case LIRConvertToObject convertToObject:
                // temporary hardcode to double
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_bclReferences.DoubleType);
                break;
            case LIRLoadLocal loadLocal:
                ilEncoder.LoadLocal(loadLocal.Source.Index);
                break;
            case LIRNewObjectArray newObjectArray:
                ilEncoder.LoadConstantI4(newObjectArray.ElementCount);
                ilEncoder.OpCode(ILOpCode.Newarr);
                ilEncoder.Token(_bclReferences.ObjectType);
                break;
            case LIRStoreElementRef:
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
                break;
            case LIRStoreLocal storeLocal:
                ilEncoder.StoreLocal(storeLocal.Destination.Index);
                break;
            default:
                return false;
        }

        return true;
    }

    private StandaloneSignatureHandle CreateLocalVariablesSignature(MethodBodyIR methodBody)
    {
        if (methodBody.Locals.Count == 0)
        {
            return default;
        }

        var localSig = new BlobBuilder();
        var localEncoder = new BlobEncoder(localSig).LocalVariableSignature(methodBody.Locals.Count);

        foreach (var local in methodBody.Locals)
        {
            localEncoder.AddVariable().Type().Double();
        }

        var signature = _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(localSig));
        return signature;
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
        ilEncoder.OpCode(ILOpCode.Pop);
    }
}