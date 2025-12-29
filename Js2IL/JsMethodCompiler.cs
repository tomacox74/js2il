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

    public MethodDefinitionHandle TryCompileMethod(string moduleName, Node node, Scope scope, MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (!HIRBuilder.TryParseMethod(node, scope, out var hirMethod))
        {
            // Nil
            return default;
        }
 
        if (!HIRToLIRLowerer.TryLower(hirMethod!, out var lirMethod))
        {
            // Nil
            return default;
        }        

        return TryCompileIRToIL(moduleName, lirMethod!, methodBodyStreamEncoder);
    }

    private MethodDefinitionHandle TryCompileIRToIL(string moduleName, MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder)
    { 
            // Get parameter info from shared ModuleParameters
            var paramCount = JavaScriptRuntime.CommonJS.ModuleParameters.Count;
            var parameterNames = JavaScriptRuntime.CommonJS.ModuleParameters.ParameterNames;

            // create the tools we need to generate the module type and method
            var programTypeBuilder = new TypeBuilder(_metadataBuilder, "Scripts", moduleName);
            
            // Create the method signature for the Main method with parameters
            var sigBuilder = new BlobBuilder();
            new BlobEncoder(sigBuilder)
                .MethodSignature()
                .Parameters(paramCount, returnType => returnType.Void(), parameters =>
                {
                    for (int i = 0; i < paramCount; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                parameters.AddParameter().Type().Object();
                                break;
                            case 1:
                                var requireDelegateReference = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.CommonJS.RequireDelegate)); 
                                parameters.AddParameter().Type().Type(requireDelegateReference, false);
                                break;
                            case 2:
                                parameters.AddParameter().Type().Object();
                                break;
                            case 3:
                            case 4:
                                parameters.AddParameter().Type().String();
                                break;
                        }
                    }
                });
            var methodSig = this._metadataBuilder.GetOrAddBlob(sigBuilder);

            // Compile the method body to IL
            if (!TryCompileMethodBodyToIL(methodBody, methodBodyStreamEncoder, out var bodyOffset))
            {
                // Failed to compile IL
                return default;
            }


            var methodDefinitionHandle = programTypeBuilder.AddMethodDefinition(
                MethodAttributes.Static | MethodAttributes.Public,
                "Main",
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

            // Define the Script main type via TypeBuilder
             programTypeBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                _bclReferences.ObjectType);

            return methodDefinitionHandle;
    }

    private bool TryCompileMethodBodyToIL(MethodBodyIR methodBody, MethodBodyStreamEncoder methodBodyStreamEncoder, out int bodyOffset)
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
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodMref);
        ilEncoder.OpCode(ILOpCode.Pop);
    }
}