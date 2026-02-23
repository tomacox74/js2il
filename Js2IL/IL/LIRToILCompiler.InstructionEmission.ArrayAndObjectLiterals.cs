using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

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

            default:
                return null;
        }
    }
}
