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

                    // Emit: call RuntimeServices.CreateObjectLiteral() -> JsObject (as object)
                    var createObjectLiteral = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.CreateObjectLiteral),
                        parameterTypes: Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(createObjectLiteral);

                    // For each property, choose the most specific typed setter to avoid 'box' instructions.
                    // - UnboxedValue + double  -> SetPropertyNumber(object, string, double)  (void)
                    // - UnboxedValue + bool    -> SetPropertyBoolean(object, string, bool)   (void)
                    // - Reference  + string   -> SetPropertyString(object, string, string?)  (void)
                    // - Otherwise             -> SetItem(object, object, object)             (object, pop)
                    var setPropertyNumberMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.SetPropertyNumber),
                        parameterTypes: new[] { typeof(object), typeof(string), typeof(double) });
                    var setPropertyBooleanMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.SetPropertyBoolean),
                        parameterTypes: new[] { typeof(object), typeof(string), typeof(bool) });
                    var setPropertyStringMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.SetPropertyString),
                        parameterTypes: new[] { typeof(object), typeof(string), typeof(string) });
                    var setItemMethod = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.SetItem),
                        parameterTypes: new[] { typeof(object), typeof(object), typeof(object) });

                    foreach (var prop in newJsObject.Properties)
                    {
                        var valueStorage = GetTempStorage(prop.Value);
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Ldstr(_metadataBuilder, prop.Key);

                        if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
                        {
                            // Fast path: emit unboxed double directly - no 'box' instruction
                            EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(setPropertyNumberMethod);
                            // SetPropertyNumber returns void - no pop needed
                        }
                        else if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(bool))
                        {
                            // Fast path: emit unboxed bool directly - no 'box' instruction
                            EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(setPropertyBooleanMethod);
                            // SetPropertyBoolean returns void - no pop needed
                        }
                        else if (valueStorage.Kind == ValueStorageKind.Reference && valueStorage.ClrType == typeof(string))
                        {
                            // Fast path: emit string reference directly - no boxing
                            EmitLoadTemp(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(setPropertyStringMethod);
                            // SetPropertyString returns void - no pop needed
                        }
                        else
                        {
                            // General path: box value as object, call SetItem, pop return value
                            EmitLoadTempAsObject(prop.Value, ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Call);
                            ilEncoder.Token(setItemMethod);
                            ilEncoder.OpCode(ILOpCode.Pop);
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
