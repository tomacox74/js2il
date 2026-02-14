using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_Calls(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor,
        StackifyResult stackifyResult)
    {
        switch (instruction)
        {
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

                    // If the callee needs an `arguments` object or has rest parameters, preserve the full runtime args list.
                    // We route through Closure.InvokeDirectWithArgs which sets the ambient arguments context.
                    if (callableId.NeedsArgumentsObject || callableId.HasRestParameters)
                    {
                        // Create delegate: ldnull, ldftn, newobj Func<object[], [object, ...], object>::.ctor
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                        ilEncoder.OpCode(ILOpCode.Ldftn);
                        ilEncoder.Token(methodHandle);
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(_bclReferences.GetFuncCtorRef(callableId.JsParamCount));

                        // Load scopes array
                        EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);

                        // Build args array from call-site arguments (no truncation)
                        ilEncoder.LoadConstantI4(callFunc.Arguments.Count);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);

                        for (int i = 0; i < callFunc.Arguments.Count; i++)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(i);
                            EmitLoadTemp(callFunc.Arguments[i], ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }

                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(_bclReferences.GetInvokeDirectWithArgsRef(callableId.JsParamCount));

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
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, [args], call Closure.InvokeWithArgs
                    // Try to use arity-specific overload if the argument count is known (0-3).
                    if (TryGetBuildArraySource(callValue.ArgumentsArray, out var argElements) && argElements.Count <= 3)
                    {
                        // Emit target
                        EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                        
                        // Emit scopes array
                        EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                        
                        // Emit individual arguments
                        foreach (var arg in argElements)
                        {
                            EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
                        }
                        
                        // Select arity-specific overload
                        Type[] paramTypes = argElements.Count switch
                        {
                            0 => new[] { typeof(object), typeof(object[]) },
                            1 => new[] { typeof(object), typeof(object[]), typeof(object) },
                            2 => new[] { typeof(object), typeof(object[]), typeof(object), typeof(object) },
                            3 => new[] { typeof(object), typeof(object[]), typeof(object), typeof(object), typeof(object) },
                            _ => throw new InvalidOperationException("Unexpected arity")
                        };
                        
                        string methodName = argElements.Count == 0 ? "InvokeWithArgs0" : $"InvokeWithArgs{argElements.Count}";
                        var invokeRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            methodName,
                            paramTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(invokeRef);
                    }
                    else
                    {
                        // Fall back to standard array-based call
                        EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                        EmitLoadTemp(callValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                        var invokeRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Closure),
                            nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                            new[] { typeof(object), typeof(object[]), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(invokeRef);
                    }

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

            case LIRCallRequire callRequire:
                {
                    // Emit: (RequireDelegate)requireValue(moduleId)
                    // This avoids the generic Closure.InvokeWithArgs dispatcher.
                    var requireStorage = GetTempStorage(callRequire.RequireValue);
                    if (requireStorage.Kind == ValueStorageKind.Reference
                        && requireStorage.ClrType == typeof(JavaScriptRuntime.CommonJS.RequireDelegate))
                    {
                        // Already typed (e.g., a typed local). No castclass needed.
                        EmitLoadTemp(callRequire.RequireValue, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        // Object-typed temp: cast to the delegate type before callvirt.
                        EmitLoadTempAsObject(callRequire.RequireValue, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.CommonJS.RequireDelegate)));
                    }

                    EmitLoadTemp(callRequire.ModuleId, ilEncoder, allocation, methodDescriptor);
                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.CommonJS.RequireDelegate),
                        nameof(JavaScriptRuntime.CommonJS.RequireDelegate.Invoke),
                        new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callRequire.Result, allocation))
                    {
                        EmitStoreTemp(callRequire.Result, ilEncoder, allocation);
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

            case LIRCallIntrinsicBaseConstructor callIntrinsicBaseCtor:
                {
                    EmitIntrinsicBaseConstructorCallCore(callIntrinsicBaseCtor, ilEncoder, allocation, methodDescriptor);
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
                    // Try to use arity-specific overload if the argument count is known (0-3).
                    if (TryGetBuildArraySource(callMember.ArgumentsArray, out var argElements) && argElements.Count <= 3)
                    {
                        // Emit receiver
                        EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                        
                        // Emit method name
                        ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                        
                        // Emit individual arguments
                        foreach (var arg in argElements)
                        {
                            EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
                        }
                        
                        // Select arity-specific overload
                        Type[] paramTypes = argElements.Count switch
                        {
                            0 => new[] { typeof(object), typeof(string) },
                            1 => new[] { typeof(object), typeof(string), typeof(object) },
                            2 => new[] { typeof(object), typeof(string), typeof(object), typeof(object) },
                            3 => new[] { typeof(object), typeof(string), typeof(object), typeof(object), typeof(object) },
                            _ => throw new InvalidOperationException("Unexpected arity")
                        };
                        
                        string methodName = argElements.Count == 0 ? "CallMember0" : $"CallMember{argElements.Count}";
                        var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            methodName,
                            paramTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(callMemberRef);
                    }
                    else
                    {
                        // Fall back to standard array-based call
                        EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                        EmitLoadTemp(callMember.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                        var callMemberRefDefault = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.CallMember),
                            new[] { typeof(object), typeof(string), typeof(object[]) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(callMemberRefDefault);
                    }

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
                    // If Stackify marked this result as stackable, defer emission to the single use site.
                    // This avoids spilling the call result into an object local and then re-casting it.
                    if (!IsMaterialized(callTyped.Result, allocation) && stackifyResult.IsStackable(callTyped.Result))
                    {
                        break;
                    }

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

                    // Bind delegate to scopes array AND lexical 'this': Closure.BindArrow(object, object[], object)
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Capture lexical 'this' at arrow creation time.
                    // - In instance methods: ldarg.0
                    // - In static methods: RuntimeServices.GetCurrentThis()
                    if (methodDescriptor.IsStatic)
                    {
                        var getThisRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.RuntimeServices), nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis));
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThisRef);
                    }
                    else
                    {
                        ilEncoder.LoadArgument(0);
                    }

                    ilEncoder.OpCode(ILOpCode.Call);
                    var bindRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Closure), nameof(JavaScriptRuntime.Closure.BindArrow), new[] { typeof(object), typeof(object[]), typeof(object) });
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

            case LIRCallRuntimeServicesStatic callRuntimeServices:
                {
                    if (!IsMaterialized(callRuntimeServices.Result, allocation))
                    {
                        break;
                    }

                    // Emit call to JavaScriptRuntime.RuntimeServices static method
                    var runtimeServicesType = typeof(JavaScriptRuntime.RuntimeServices);
                    
                    // Load arguments and box if necessary
                    foreach (var arg in callRuntimeServices.Arguments)
                    {
                        EmitLoadTemp(arg, ilEncoder, allocation, methodDescriptor);
                        
                        // Check if we need to box the value
                        if (GetTempStorage(arg) is { } storage && 
                            storage.Kind == ValueStorageKind.UnboxedValue &&
                            storage.ClrType != null)
                        {
                            // Box the unboxed value to object
                            ilEncoder.OpCode(ILOpCode.Box);
                            if (storage.ClrType == typeof(double))
                            {
                                ilEncoder.Token(_bclReferences.DoubleType);
                            }
                            else if (storage.ClrType == typeof(bool))
                            {
                                ilEncoder.Token(_bclReferences.BooleanType);
                            }
                            else if (storage.ClrType == typeof(int))
                            {
                                ilEncoder.Token(_bclReferences.Int32Type);
                            }
                            else
                            {
                                throw new NotSupportedException($"Unsupported unboxed type for RuntimeServices call: {storage.ClrType}");
                            }
                        }
                    }

                    // Emit call - use explicit parameter types to ensure correct method resolution
                    ilEncoder.OpCode(ILOpCode.Call);
                    var paramTypes = new Type[callRuntimeServices.Arguments.Count];
                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        paramTypes[i] = typeof(object); // RuntimeServices methods take object parameters
                    }
                    var methodRef = _memberRefRegistry.GetOrAddMethod(
                        runtimeServicesType, 
                        callRuntimeServices.MethodName,
                        paramTypes);
                    ilEncoder.Token(methodRef);

                    // Store result
                    EmitStoreTemp(callRuntimeServices.Result, ilEncoder, allocation);
                    break;
                }

            default:
                return null;
        }

        return true;
    }

    /// <summary>
    /// Tries to find the LIRBuildArray instruction that defines the given temp variable.
    /// Returns true if found and the array has a fixed size, with the element temps in the out parameter.
    /// </summary>
    private bool TryGetBuildArraySource(TempVariable arrayTemp, out IReadOnlyList<TempVariable> elements)
    {
        elements = System.Array.Empty<TempVariable>();
        
        // Search backwards through instructions to find the LIRBuildArray that defines this temp
        foreach (var instr in MethodBody.Instructions)
        {
            if (instr is LIRBuildArray buildArray && buildArray.Result.Index == arrayTemp.Index)
            {
                elements = buildArray.Elements;
                return true;
            }
        }
        
        return false;
    }
}
