using Jroc.IR;
using Jroc.Services.ILGenerators;
using Jroc.Services.TwoPhaseCompilation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_ArrayAndObjectLiterals(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRBuildArray buildArray:
                {
                    if (!IsMaterialized(buildArray.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        return true;
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
                    return true;
                }

            case LIRNewJsArray newJsArray:
                {
                    if (!IsMaterialized(newJsArray.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        return true;
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
                        typeof(JavaScriptRuntime.Array),
                        nameof(JavaScriptRuntime.Array.Add),
                        parameterTypes: new[] { typeof(object) });
                    for (int i = 0; i < newJsArray.Elements.Count; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Dup);
                        EmitLoadTempAsObject(newJsArray.Elements[i], ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Callvirt);
                        ilEncoder.Token(addMethod);
                    }

                    EmitStoreTemp(newJsArray.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRNewJsObject newJsObject:
                {
                    if (!IsMaterialized(newJsObject.Result, allocation))
                    {
                        // Will be emitted inline via EmitLoadTemp when the temp is used
                        return true;
                    }

                    // Emit: call RuntimeServices.CreateObjectLiteral() -> object
                    var createObjectLiteral = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.CreateObjectLiteral),
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(createObjectLiteral);

                    // For each property emit a typed JsObject setter when possible to avoid boxing.
                    var setNumberMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.JsObject),
                        nameof(JavaScriptRuntime.JsObject.SetNumber),
                        parameterTypes: new[] { typeof(string), typeof(double) });
                    var setBooleanMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.JsObject),
                        nameof(JavaScriptRuntime.JsObject.SetBoolean),
                        parameterTypes: new[] { typeof(string), typeof(bool) });
                    var setObjectMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.JsObject),
                        nameof(JavaScriptRuntime.JsObject.SetObject),
                        parameterTypes: new[] { typeof(string), typeof(object) });
                    foreach (var prop in newJsObject.Properties)
                    {
                        var valueStorage = GetTempStorage(prop.Value);
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Ldstr(_metadataBuilder, prop.Key);
                        if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
                        {
                            EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(setNumberMethod);
                        }
                        else if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(bool))
                        {
                            EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(setBooleanMethod);
                        }
                        else
                        {
                            EmitLoadTempAsObject(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Callvirt);
                            ilEncoder.Token(setObjectMethod);
                        }
                    }

                    EmitStoreTemp(newJsObject.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRNewInferredJsObject newInferredJsObject:
                {
                    if (!IsMaterialized(newInferredJsObject.Result, allocation))
                    {
                        return true;
                    }

                    EmitNewInferredJsObject(newInferredJsObject, ilEncoder, allocation, methodDescriptor);
                    EmitStoreTemp(newInferredJsObject.Result, ilEncoder, allocation);
                    EmitInitializeInferredJsObject(newInferredJsObject, ilEncoder, allocation, methodDescriptor);
                    return true;
                }

            case LIRGetInferredMember getInferredMember:
                {
                    // The generated getter is pure; skip entirely when the result is unused.
                    if (!IsMaterialized(getInferredMember.Result, allocation))
                    {
                        return true;
                    }

                    var metadata = GetObjectLiteralTypeMetadata(getInferredMember.Shape);
                    if (!metadata.GetterHandlesByMemberName.TryGetValue(getInferredMember.MemberName, out var getterHandle))
                    {
                        throw new InvalidOperationException(
                            $"Missing generated object-literal getter metadata for member '{getInferredMember.MemberName}'.");
                    }

                    EmitLoadTempAsObject(getInferredMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(metadata.TypeHandle);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getterHandle);
                    EmitStoreTemp(getInferredMember.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRSetInferredMember setInferredMember:
                {
                    var metadata = GetObjectLiteralTypeMetadata(setInferredMember.Shape);
                    if (!metadata.SetterHandlesByMemberName.TryGetValue(setInferredMember.MemberName, out var setterHandle))
                    {
                        throw new InvalidOperationException(
                            $"Missing generated object-literal setter metadata for member '{setInferredMember.MemberName}'.");
                    }
                    if (!metadata.FieldClrTypesByMemberName.TryGetValue(setInferredMember.MemberName, out var memberClrType))
                    {
                        throw new InvalidOperationException(
                            $"Missing generated object-literal CLR type metadata for member '{setInferredMember.MemberName}'.");
                    }

                    EmitLoadTempAsObject(setInferredMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Castclass);
                    ilEncoder.Token(metadata.TypeHandle);
                    EmitLoadTempAsClrType(setInferredMember.Value, memberClrType, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(setterHandle);
                    return true;
                }

            default:
                return null;
        }
    }

    private Jroc.Services.VariableBindings.ObjectLiteralTypeMetadata GetObjectLiteralTypeMetadata(Jroc.SymbolTables.ObjectLiteralShapeInfo shape)
    {
        if (!_variableRegistry.TryGetObjectLiteralType(shape, out var metadata))
        {
            throw new InvalidOperationException(
                $"Missing generated object-literal type metadata for binding '{shape.Binding.Name}'.");
        }

        return metadata;
    }

    private void EmitNewInferredJsObject(
        LIRNewInferredJsObject newInferredJsObject,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (!_variableRegistry.TryGetObjectLiteralType(newInferredJsObject.Shape, out var metadata))
        {
            throw new InvalidOperationException(
                $"Missing generated object-literal type metadata for binding '{newInferredJsObject.Shape.Binding.Name}'.");
        }

        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(metadata.ConstructorHandle);
    }

    private void EmitInitializeInferredJsObject(
        LIRNewInferredJsObject newInferredJsObject,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (!_variableRegistry.TryGetObjectLiteralType(newInferredJsObject.Shape, out var metadata))
        {
            throw new InvalidOperationException(
                $"Missing generated object-literal type metadata for binding '{newInferredJsObject.Shape.Binding.Name}'.");
        }

        foreach (var prop in newInferredJsObject.Properties)
        {
            if (!metadata.SetterHandlesByMemberName.TryGetValue(prop.Key, out var setterHandle))
            {
                throw new InvalidOperationException(
                    $"Missing generated object-literal setter metadata for member '{prop.Key}'.");
            }
            if (!metadata.FieldClrTypesByMemberName.TryGetValue(prop.Key, out var fieldClrType))
            {
                throw new InvalidOperationException(
                    $"Missing generated object-literal CLR type metadata for member '{prop.Key}'.");
            }

            // The generated setter stores the typed backing field and mirrors the value
            // into JsObject storage, so a single call keeps both views in sync.
            EmitLoadTemp(newInferredJsObject.Result, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsClrType(prop.Value, fieldClrType, ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Callvirt);
            ilEncoder.Token(setterHandle);
        }
    }

    private void EmitLoadTempAsClrType(
        TempVariable value,
        Type fieldClrType,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (fieldClrType == typeof(double))
        {
            EmitLoadTempAsNumber(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        if (fieldClrType == typeof(bool))
        {
            EmitLoadTempAsBoolean(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        if (fieldClrType == typeof(string))
        {
            EmitLoadTempAsString(value, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(value, ilEncoder, allocation, methodDescriptor);
    }
}
