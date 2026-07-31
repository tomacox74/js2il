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
                        EmitSchedulerOwnedStackValueIfNeeded(
                            buildArray,
                            buildArray.Result,
                            ilEncoder,
                            allocation,
                            methodDescriptor);
                        return true;
                    }

                    EmitBuildArrayValue(
                        buildArray,
                        ilEncoder,
                        allocation,
                        methodDescriptor);

                    EmitStoreTemp(buildArray.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRNewJsArray newJsArray:
                {
                    if (!IsMaterialized(newJsArray.Result, allocation))
                    {
                        EmitSchedulerOwnedStackValueIfNeeded(
                            newJsArray,
                            newJsArray.Result,
                            ilEncoder,
                            allocation,
                            methodDescriptor);
                        return true;
                    }

                    EmitNewJsArrayValue(
                        newJsArray,
                        ilEncoder,
                        allocation,
                        methodDescriptor);

                    EmitStoreTemp(newJsArray.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRNewJsObject newJsObject:
                {
                    if (!IsMaterialized(newJsObject.Result, allocation))
                    {
                        EmitSchedulerOwnedStackValueIfNeeded(
                            newJsObject,
                            newJsObject.Result,
                            ilEncoder,
                            allocation,
                            methodDescriptor);
                        return true;
                    }

                    EmitNewJsObjectValue(
                        newJsObject,
                        ilEncoder,
                        allocation,
                        methodDescriptor);

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

    private void EmitBuildArrayValue(
                LIRBuildArray buildArray,
                InstructionEncoder ilEncoder,
                TempLocalAllocation allocation,
                MethodDescriptor methodDescriptor)
    {
        ilEncoder.LoadConstantI4(buildArray.Elements.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        for (var index = 0; index < buildArray.Elements.Count; index++)
        {
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(index);
            EmitLoadTempAsObject(
                buildArray.Elements[index],
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    private void EmitNewJsArrayValue(
            LIRNewJsArray newJsArray,
            InstructionEncoder ilEncoder,
            TempLocalAllocation allocation,
            MethodDescriptor methodDescriptor)
    {
        ilEncoder.LoadConstantI4(
            newJsArray.CapacityHint ?? newJsArray.Elements.Count);
        ilEncoder.OpCode(ILOpCode.Newobj);
        ilEncoder.Token(_memberRefRegistry.GetOrAddConstructor(
            typeof(JavaScriptRuntime.Array),
            parameterTypes: new[] { typeof(int) }));

        var addMethod = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Array),
            nameof(JavaScriptRuntime.Array.Add),
            parameterTypes: new[] { typeof(object) });
        var addNumberMethod = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Array),
            nameof(JavaScriptRuntime.Array.AddNumber),
            parameterTypes: new[] { typeof(double) });
        foreach (var element in newJsArray.Elements)
        {
            var elementStorage = GetTempStorage(element);
            ilEncoder.OpCode(ILOpCode.Dup);
            if (elementStorage.Kind == ValueStorageKind.UnboxedValue
                && elementStorage.ClrType == typeof(double))
            {
                EmitLoadTemp(
                    element,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Callvirt);
                ilEncoder.Token(addNumberMethod);
                continue;
            }

            EmitLoadTempAsObject(
                element,
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Callvirt);
            ilEncoder.Token(addMethod);
        }
    }

    private void EmitNewJsObjectValue(
            LIRNewJsObject newJsObject,
            InstructionEncoder ilEncoder,
            TempLocalAllocation allocation,
            MethodDescriptor methodDescriptor)
    {
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.CreateObjectLiteral),
            parameterTypes: Type.EmptyTypes));

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
        foreach (var property in newJsObject.Properties)
        {
            var valueStorage = GetTempStorage(property.Value);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.Ldstr(_metadataBuilder, property.Key);
            if (valueStorage.Kind == ValueStorageKind.UnboxedValue
                && valueStorage.ClrType == typeof(double))
            {
                EmitLoadTemp(
                    property.Value,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Callvirt);
                ilEncoder.Token(setNumberMethod);
            }
            else if (valueStorage.Kind == ValueStorageKind.UnboxedValue
                && valueStorage.ClrType == typeof(bool))
            {
                EmitLoadTemp(
                    property.Value,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Callvirt);
                ilEncoder.Token(setBooleanMethod);
            }
            else
            {
                EmitLoadTempAsObject(
                    property.Value,
                    ilEncoder,
                    allocation,
                    methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Callvirt);
                ilEncoder.Token(setObjectMethod);
            }
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
