using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_Collections(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
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

            case LIRGetItemAsNumber getItemAsNumber:
                {
                    if (!IsMaterialized(getItemAsNumber.Result, allocation))
                    {
                        break;
                    }

                    var indexStorage = GetTempStorage(getItemAsNumber.Index);
                    if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double))
                    {
                        // Emit: call float64 JavaScriptRuntime.Object.GetItemAsNumber(object, double)
                        EmitLoadTempAsObject(getItemAsNumber.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(getItemAsNumber.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemAsNumberMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItemAsNumber),
                            parameterTypes: new[] { typeof(object), typeof(double) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemAsNumberMethod);
                    }
                    else
                    {
                        // Emit: call float64 JavaScriptRuntime.Object.GetItemAsNumber(object, object)
                        EmitLoadTempAsObject(getItemAsNumber.Object, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTempAsObject(getItemAsNumber.Index, ilEncoder, allocation, methodDescriptor);
                        var getItemAsNumberMethod = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetItemAsNumber),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getItemAsNumberMethod);
                    }

                    EmitStoreTemp(getItemAsNumber.Result, ilEncoder, allocation);
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

                    EmitLoadTempAsDouble(setI32.Index, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsDouble(setI32.Value, ilEncoder, allocation, methodDescriptor);

                    var int32ArraySetter = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "set_Item",
                        parameterTypes: new[] { typeof(double), typeof(double) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArraySetter);

                    // If the assignment expression result is used, return the assigned value.
                    if (IsMaterialized(setI32.Result, allocation))
                    {
                        var resultStorage = GetTempStorage(setI32.Result);
                        if (resultStorage.Kind == ValueStorageKind.UnboxedValue && resultStorage.ClrType == typeof(double))
                        {
                            EmitLoadTempAsDouble(setI32.Value, ilEncoder, allocation, methodDescriptor);
                        }
                        else
                        {
                            EmitLoadTemp(setI32.Value, ilEncoder, allocation, methodDescriptor);
                            var valueStorage = GetTempStorage(setI32.Value);
                            if (valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double))
                            {
                                ilEncoder.OpCode(ILOpCode.Box);
                                ilEncoder.Token(_bclReferences.DoubleType);
                            }
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

            case LIRGetInt32ArrayElementInt getI32Int:
                {
                    if (!IsMaterialized(getI32Int.Result, allocation))
                    {
                        break;
                    }

                    // Load receiver as Int32Array (cast only if needed)
                    var receiverStorageI32Int = GetTempStorage(getI32Int.Receiver);
                    if (receiverStorageI32Int.Kind == ValueStorageKind.Reference && receiverStorageI32Int.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(getI32Int.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(getI32Int.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    // Index is int32 - load directly
                    EmitLoadTemp(getI32Int.Index, ilEncoder, allocation, methodDescriptor);

                    var int32ArrayGetterInt = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "get_ItemInt",
                        parameterTypes: new[] { typeof(int) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArrayGetterInt);

                    EmitStoreTemp(getI32Int.Result, ilEncoder, allocation);
                    break;
                }

            case LIRSetInt32ArrayElementInt setI32Int:
                {
                    // Load receiver as Int32Array (cast only if needed)
                    var receiverStorageSetI32Int = GetTempStorage(setI32Int.Receiver);
                    if (receiverStorageSetI32Int.Kind == ValueStorageKind.Reference && receiverStorageSetI32Int.ClrType == typeof(JavaScriptRuntime.Int32Array))
                    {
                        EmitLoadTemp(setI32Int.Receiver, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        EmitLoadTempAsObject(setI32Int.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Int32Array)));
                    }

                    EmitLoadTemp(setI32Int.Index, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(setI32Int.Value, ilEncoder, allocation, methodDescriptor);

                    var int32ArraySetterInt = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Int32Array),
                        "set_ItemInt",
                        parameterTypes: new[] { typeof(int), typeof(int) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(int32ArraySetterInt);

                    // If the assignment expression result is used, return the assigned value.
                    if (IsMaterialized(setI32Int.Result, allocation))
                    {
                        EmitLoadTemp(setI32Int.Value, ilEncoder, allocation, methodDescriptor);
                        EmitStoreTemp(setI32Int.Result, ilEncoder, allocation);
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

            default:
                return null;
        }

        return true;
    }
}
