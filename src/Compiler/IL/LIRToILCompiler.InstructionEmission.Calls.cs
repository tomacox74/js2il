using Acornima.Ast;
using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private static bool IsGeneratorCallable(CallableId callableId)
        => callableId.AstNode switch
        {
            FunctionDeclaration { Generator: true } => true,
            FunctionExpression { Generator: true } => true,
            Acornima.Ast.MethodDefinition { Value: FunctionExpression { Generator: true } } => true,
            _ => false
        };

    private static int GetExpectedFunctionLength(CallableId callableId)
        => callableId.AstNode switch
        {
            FunctionDeclaration functionDeclaration => CountExpectedFunctionLength(functionDeclaration.Params),
            FunctionExpression functionExpression => CountExpectedFunctionLength(functionExpression.Params),
            ArrowFunctionExpression arrowFunctionExpression => CountExpectedFunctionLength(arrowFunctionExpression.Params),
            Acornima.Ast.MethodDefinition { Value: FunctionExpression methodFunction } => CountExpectedFunctionLength(methodFunction.Params),
            _ => callableId.JsParamCount
        };

    private static int CountExpectedFunctionLength(NodeList<Node> parameters)
    {
        var count = 0;
        foreach (var parameter in parameters)
        {
            if (parameter is RestElement or AssignmentPattern)
            {
                break;
            }

            count++;
        }

        return count;
    }

    private static string GetFunctionName(CallableId callableId)
        => callableId.AstNode switch
        {
            FunctionDeclaration { Id: Identifier id } => id.Name,
            FunctionExpression { Id: Identifier id } => id.Name,
            _ => callableId.Name ?? string.Empty
        };

    private static bool RequiresInvocationContext(CallableId callableId)
    {
        return callableId.NeedsArgumentsObject
            || callableId.HasRestParameters
            || callableId.AstNode is FunctionExpression { Id: not null }
            || ContainsMetaProperty(callableId.AstNode);
    }

    private static bool ContainsMetaProperty(Node? node)
    {
        if (node is null)
        {
            return false;
        }

        var found = false;
        var walker = new AstWalker();
        walker.Visit(node, visited =>
        {
            if (visited is MetaProperty)
            {
                found = true;
            }
        });
        return found;
    }

    private void EmitInitializeFunctionInstance(CallableId callableId, bool isAsync, InstructionEncoder ilEncoder)
    {
        ilEncoder.LoadConstantI4(GetExpectedFunctionLength(callableId));
        ilEncoder.OpCode(ILOpCode.Conv_r8);
        ilEncoder.Ldstr(_metadataBuilder, GetFunctionName(callableId));
        ilEncoder.LoadConstantI4(RequiresInvocationContext(callableId) ? 1 : 0);
        ilEncoder.LoadConstantI4(callableId.HasRestrictedFunctionProperties ? 1 : 0);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
            isAsync ? typeof(JavaScriptRuntime.AsyncFunction) : typeof(JavaScriptRuntime.Function),
            nameof(JavaScriptRuntime.Function.InitializeFunctionInstance),
            new[] { typeof(object), typeof(double), typeof(string), typeof(bool), typeof(bool) }));

        if (callableId.Kind == CallableKind.Arrow)
        {
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(_memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Function),
                nameof(JavaScriptRuntime.Function.MarkUndefinedPrototype),
                new[] { typeof(object) }));
        }
    }

    private void EmitInitializeGeneratorFunctionSurfaceIfNeeded(CallableId callableId, InstructionEncoder ilEncoder)
    {
        if (!IsGeneratorCallable(callableId))
        {
            return;
        }

        var initRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.GeneratorObject),
            nameof(JavaScriptRuntime.GeneratorObject.InitializeGeneratorFunctionSurface),
            new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(initRef);
    }

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

                    // Look up the callable's signature to determine if scopes parameter is required
                    bool requiresScopes = true; // Default to true for safety
                    var callableSignature = reader.GetSignature(callableId);
                    if (callableSignature != null)
                    {
                        requiresScopes = callableSignature.RequiresScopesParameter;
                    }

                    bool usesSingleScope = UsesSingleScopeAbi(callableSignature);
                    bool delegateRequiresScopeArray = requiresScopes && !usesSingleScope;

                    // If the callee needs an `arguments` object or has rest parameters, preserve the full runtime args list.
                    // We route through Closure.InvokeDirectWithArgs which sets the ambient arguments context.
                    if (callableId.NeedsArgumentsObject || callableId.HasRestParameters)
                    {
                        if (usesSingleScope)
                        {
                            EmitLoadSingleScopeFromScopesArray(
                                callFunc.ScopesArray,
                                ilEncoder,
                                allocation,
                                methodDescriptor,
                                callableSignature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {callableId.DisplayName}."));
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        ilEncoder.OpCode(ILOpCode.Ldftn);
                        ilEncoder.Token(methodHandle);
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(_bclReferences.GetFuncCtorRef(callableId.JsParamCount, delegateRequiresScopeArray));

                        if (delegateRequiresScopeArray)
                        {
                            EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                        }

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
                        ilEncoder.Token(_bclReferences.GetInvokeDirectWithArgsRef(callableId.JsParamCount, delegateRequiresScopeArray));

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

                    if (usesSingleScope)
                    {
                        EmitLoadSingleScopeFromScopesArray(
                            callFunc.ScopesArray,
                            ilEncoder,
                            allocation,
                            methodDescriptor,
                            callableSignature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {callableId.DisplayName}."));
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount, delegateRequiresScopeArray));

                    if (delegateRequiresScopeArray)
                    {
                        // Load scopes array only when required by callee ABI.
                        EmitLoadTemp(callFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    }

                    // Normal function call path: new.target is undefined.
                    ilEncoder.OpCode(ILOpCode.Ldnull);

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
                    ilEncoder.Token(_bclReferences.GetFuncInvokeRef(jsParamCount, delegateRequiresScopeArray));

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

            case LIRTailCallFunctionReturn tailCall:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    if (reader == null)
                    {
                        return false;
                    }

                    if (!reader.TryGetDeclaredToken(tailCall.CallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    if (tailCall.CallableId.NeedsArgumentsObject || tailCall.CallableId.HasRestParameters)
                    {
                        return false;
                    }

                    var methodHandle = (MethodDefinitionHandle)token;
                    var callableSignature = reader.GetSignature(tailCall.CallableId);
                    bool requiresScopes = callableSignature?.RequiresScopesParameter ?? true;
                    bool usesSingleScope = UsesSingleScopeAbi(callableSignature);
                    int jsParamCount = tailCall.CallableId.JsParamCount;
                    int argsToPass = Math.Min(tailCall.Arguments.Count, jsParamCount);

                    if (usesSingleScope)
                    {
                        EmitLoadSingleScopeFromScopesArray(
                            tailCall.ScopesArray,
                            ilEncoder,
                            allocation,
                            methodDescriptor,
                            callableSignature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {tailCall.CallableId.DisplayName}."));
                    }
                    else
                    {
                        if (requiresScopes)
                        {
                            EmitLoadTemp(tailCall.ScopesArray, ilEncoder, allocation, methodDescriptor);
                        }
                    }

                    // new.target is undefined for ordinary function tail calls.
                    ilEncoder.OpCode(ILOpCode.Ldnull);

                    for (int i = 0; i < argsToPass; i++)
                    {
                        var parameterClrType = callableSignature?.ParameterClrTypes != null && i < callableSignature.ParameterClrTypes.Count
                            ? callableSignature.ParameterClrTypes[i]
                            : null;
                        EmitLoadTempAsParameterType(tailCall.Arguments[i], parameterClrType, ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Tail);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Ret);
                    break;
                }

            case LIRCallFunctionWithArgsArray callFuncArray:
                {
                    if (callFuncArray.CallableId is not { } callableId)
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
                    int jsParamCount = callableId.JsParamCount;

                    bool requiresScopes = true;
                    var callableSignature = reader.GetSignature(callableId);
                    if (callableSignature != null)
                    {
                        requiresScopes = callableSignature.RequiresScopesParameter;
                    }

                    bool usesSingleScope = UsesSingleScopeAbi(callableSignature);
                    bool delegateRequiresScopeArray = requiresScopes && !usesSingleScope;

                    if (usesSingleScope)
                    {
                        EmitLoadSingleScopeFromScopesArray(
                            callFuncArray.ScopesArray,
                            ilEncoder,
                            allocation,
                            methodDescriptor,
                            callableSignature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {callableId.DisplayName}."));
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount, delegateRequiresScopeArray));

                    // Load scopes array
                    EmitLoadTemp(callFuncArray.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Load runtime argument array (already expanded for spread)
                    EmitLoadTemp(callFuncArray.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    // Dispatch through Closure.InvokeWithArgs(target, scopes, argsArray)
                    var invokeWithArgsRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeWithArgs),
                        new[] { typeof(object), typeof(object[]), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeWithArgsRef);

                    if (IsMaterialized(callFuncArray.Result, allocation))
                    {
                        EmitStoreTemp(callFuncArray.Result, ilEncoder, allocation);
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
                    // This instruction is only used for >3 args or spread calls now.
                    EmitLoadTemp(callValue.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeFunctionCallWithArgs),
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

            case LIRCallFunctionValue0 callValue0:
                {
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, call Closure.InvokeWithArgs0
                    EmitLoadTemp(callValue0.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue0.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeFunctionCallWithArgs0),
                        new[] { typeof(object), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue0.Result, allocation))
                    {
                        EmitStoreTemp(callValue0.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallFunctionValue1 callValue1:
                {
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, ldarg/ldloc a0, call Closure.InvokeWithArgs1
                    EmitLoadTemp(callValue1.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue1.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue1.A0, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeFunctionCallWithArgs1),
                        new[] { typeof(object), typeof(object[]), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue1.Result, allocation))
                    {
                        EmitStoreTemp(callValue1.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallFunctionValue2 callValue2:
                {
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, ldarg/ldloc a0, ldarg/ldloc a1, call Closure.InvokeWithArgs2
                    EmitLoadTemp(callValue2.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue2.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue2.A0, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue2.A1, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeFunctionCallWithArgs2),
                        new[] { typeof(object), typeof(object[]), typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue2.Result, allocation))
                    {
                        EmitStoreTemp(callValue2.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallFunctionValue3 callValue3:
                {
                    // Emit: ldarg/ldloc target, ldarg/ldloc scopesArray, ldarg/ldloc a0, ldarg/ldloc a1, ldarg/ldloc a2, call Closure.InvokeWithArgs3
                    EmitLoadTemp(callValue3.FunctionValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callValue3.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue3.A0, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue3.A1, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callValue3.A2, ilEncoder, allocation, methodDescriptor);

                    var invokeRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Closure),
                        nameof(JavaScriptRuntime.Closure.InvokeFunctionCallWithArgs3),
                        new[] { typeof(object), typeof(object[]), typeof(object), typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(invokeRef);

                    if (IsMaterialized(callValue3.Result, allocation))
                    {
                        EmitStoreTemp(callValue3.Result, ilEncoder, allocation);
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

            case LIRCallImport callImport:
                {
                    // Emit: JavaScriptRuntime.CommonJS.DynamicImport(specifier, currentModuleId)
                    EmitLoadTemp(callImport.ModuleSpecifier, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callImport.CurrentModuleId, ilEncoder, allocation, methodDescriptor);
                    
                    var importRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.CommonJS.DynamicImport),
                        nameof(JavaScriptRuntime.CommonJS.DynamicImport.Import),
                        new[] { typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(importRef);

                    if (IsMaterialized(callImport.Result, allocation))
                    {
                        EmitStoreTemp(callImport.Result, ilEncoder, allocation);
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

            case LIRCallFunctionBaseConstructor callFunctionBase:
                {
                    ilEncoder.OpCode(ILOpCode.Ldarg_0);
                    EmitLoadTempAsObject(callFunctionBase.ConstructorValue, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTemp(callFunctionBase.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var mref = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.ConstructDerivedFunctionBase),
                        new[] { typeof(object), typeof(object), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(mref);
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
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    // Match the declared signature (ignore extra args, pad missing args with null).
                    int jsParamCount = callUserClass.MaxParamCount;
                    int argsToPass = Math.Min(callUserClass.Arguments.Count, jsParamCount);
                    IReadOnlyList<Type?> parameterClrTypes = Array.Empty<Type?>();
                    if (_serviceProvider.GetService<ClassRegistry>() is { } classRegistry)
                    {
                        classRegistry.TryGetMethodParameterClrTypes(callUserClass.RegistryClassName, callUserClass.MethodName, out parameterClrTypes);
                    }

                    for (int i = 0; i < argsToPass; i++)
                    {
                        var parameterClrType = i < parameterClrTypes.Count ? parameterClrTypes[i] : null;
                        EmitLoadTempAsParameterType(callUserClass.Arguments[i], parameterClrType, ilEncoder, allocation, methodDescriptor);
                    }

                    for (int i = argsToPass; i < jsParamCount; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(callUserClass.MethodHandle);

                    if (IsMaterialized(callUserClass.Result, allocation))
                    {
                        if (GetMaterializedTempStorage(callUserClass.Result, allocation) is { Kind: ValueStorageKind.Reference, ClrType: Type clrType }
                            && clrType == typeof(object)
                            && GetTempStorage(callUserClass.Result) is { Kind: ValueStorageKind.UnboxedValue, ClrType: Type resultClrType })
                        {
                            ilEncoder.OpCode(ILOpCode.Box);
                            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(resultClrType));
                        }

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

                    // Push all actual JS arguments into _currentArguments so that the 'arguments' keyword
                    // inside the base constructor reflects the values passed to super(...), not the outer ctor args.
                    // Only push when there are explicit JS args; if no args are passed (e.g., synthesized default
                    // constructor calls super() with 0 args), the outer _currentArguments remains visible to Base.
                    int allArgc = callBaseCtor.AllJsArguments.Count;
                    if (allArgc > 0)
                    {
                        var pushCurrentArguments = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PushCurrentArguments),
                            parameterTypes: new[] { typeof(object[]) });

                        ilEncoder.LoadConstantI4(allArgc);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);
                        for (int i = 0; i < allArgc; i++)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(i);
                            EmitLoadTemp(callBaseCtor.AllJsArguments[i], ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(pushCurrentArguments);
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

                    var initializeDerivedThis = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.RuntimeServices),
                        nameof(JavaScriptRuntime.RuntimeServices.InitializeDerivedConstructorThisBinding),
                        parameterTypes: new[] { typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Ldarg_0);
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(initializeDerivedThis);

                    // Restore _currentArguments to its pre-super-call state (only if we pushed above).
                    if (allArgc > 0)
                    {
                        var popCurrentArguments = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PopCurrentArguments),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(popCurrentArguments);
                    }
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
                        ilEncoder.OpCode(ILOpCode.Ldnull);
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
                    // Runtime dispatcher member call (>3 args or spread).
                    // Emit: ldarg/ldloc receiver, ldstr methodName, ldarg/ldloc argsArray, call Object.CallMember
                    EmitLoadTempAsObject(callMember.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember.MethodName);
                    EmitLoadTemp(callMember.ArgumentsArray, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember),
                        new[] { typeof(object), typeof(string), typeof(object[]) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);

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

            case LIRCallMember0 callMember0:
                {
                    // Emit: ldarg/ldloc receiver, ldstr methodName, call Object.CallMember0
                    EmitLoadTempAsObject(callMember0.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember0.MethodName);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember0),
                        new[] { typeof(object), typeof(string) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);

                    if (IsMaterialized(callMember0.Result, allocation))
                    {
                        EmitStoreTemp(callMember0.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallMember1 callMember1:
                {
                    // Emit: ldarg/ldloc receiver, ldstr methodName, ldarg/ldloc a0, call Object.CallMember1
                    EmitLoadTempAsObject(callMember1.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember1.MethodName);
                    EmitLoadTempAsObject(callMember1.A0, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember1),
                        new[] { typeof(object), typeof(string), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);

                    if (IsMaterialized(callMember1.Result, allocation))
                    {
                        EmitStoreTemp(callMember1.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallMember2 callMember2:
                {
                    // Emit: ldarg/ldloc receiver, ldstr methodName, ldarg/ldloc a0, ldarg/ldloc a1, call Object.CallMember2
                    EmitLoadTempAsObject(callMember2.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember2.MethodName);
                    EmitLoadTempAsObject(callMember2.A0, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callMember2.A1, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember2),
                        new[] { typeof(object), typeof(string), typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);

                    if (IsMaterialized(callMember2.Result, allocation))
                    {
                        EmitStoreTemp(callMember2.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            case LIRCallMember3 callMember3:
                {
                    // Emit: ldarg/ldloc receiver, ldstr methodName, ldarg/ldloc a0, ldarg/ldloc a1, ldarg/ldloc a2, call Object.CallMember3
                    EmitLoadTempAsObject(callMember3.Receiver, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.Ldstr(_metadataBuilder, callMember3.MethodName);
                    EmitLoadTempAsObject(callMember3.A0, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callMember3.A1, ilEncoder, allocation, methodDescriptor);
                    EmitLoadTempAsObject(callMember3.A2, ilEncoder, allocation, methodDescriptor);

                    var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.Object),
                        nameof(JavaScriptRuntime.Object.CallMember3),
                        new[] { typeof(object), typeof(string), typeof(object), typeof(object), typeof(object) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(callMemberRef);

                    if (IsMaterialized(callMember3.Result, allocation))
                    {
                        EmitStoreTemp(callMember3.Result, ilEncoder, allocation);
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

                    var signature = reader.GetSignature(callDeclared.CallableId);
                    var jsArgumentOffset = (signature?.RequiresScopesParameter == true ? 1 : 0) + 1;
                    for (var i = 0; i < callDeclared.Arguments.Count; i++)
                    {
                        var jsParameterIndex = i - jsArgumentOffset;
                        var parameterClrType = signature?.ParameterClrTypes != null && jsParameterIndex >= 0 && jsParameterIndex < signature.ParameterClrTypes.Count
                            ? signature.ParameterClrTypes[jsParameterIndex]
                            : null;
                        EmitLoadTempAsParameterType(callDeclared.Arguments[i], parameterClrType, ilEncoder, allocation, methodDescriptor);
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

                    bool requiresScopes = true;
                    var signature = reader.GetSignature(callableId);
                    if (signature != null)
                    {
                        requiresScopes = signature.RequiresScopesParameter;
                    }

                    bool usesSingleScope = UsesSingleScopeAbi(signature);
                    bool delegateRequiresScopeArray = requiresScopes && !usesSingleScope;

                    if (usesSingleScope)
                    {
                        EmitLoadSingleScopeFromScopesArray(
                            createArrow.ScopesArray,
                            ilEncoder,
                            allocation,
                            methodDescriptor,
                            signature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {callableId.DisplayName}."));
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount, delegateRequiresScopeArray));

                    // Bind delegate to scopes array AND lexical 'this': Closure.BindArrow(object, object[], object)
                    EmitLoadTemp(createArrow.ScopesArray, ilEncoder, allocation, methodDescriptor);

                    // Capture lexical 'this' at arrow creation time.
                    // - In derived constructors: capture the mutable derived-this binding
                    //   so arrows created before super() observe TDZ first and the receiver after super().
                    // - In instance methods: ldarg.0
                    // - In static methods: RuntimeServices.GetCurrentThis()
                    if (methodDescriptor.IsDerivedConstructor || methodDescriptor.IsStatic)
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
                    EmitInitializeFunctionInstance(createArrow.CallableId, createArrow.IsAsync, ilEncoder);

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

                    bool requiresScopes = true;
                    var signature = reader.GetSignature(callableId);
                    if (signature != null)
                    {
                        requiresScopes = signature.RequiresScopesParameter;
                    }

                    bool usesSingleScope = UsesSingleScopeAbi(signature);

                    // Create a JsFuncNoScopesN delegate.
                    // - requiresScopes: close over scopes as delegate target (binds first static arg object[] scopes)
                    // - no scopes: regular static delegate target = null
                    if (usesSingleScope)
                    {
                        EmitLoadSingleScopeFromScopesArray(
                            createFunc.ScopesArray,
                            ilEncoder,
                            allocation,
                            methodDescriptor,
                            signature ?? throw new InvalidOperationException($"Missing SingleScope signature metadata for callable {callableId.DisplayName}."));
                    }
                    else if (requiresScopes)
                    {
                        EmitLoadTemp(createFunc.ScopesArray, ilEncoder, allocation, methodDescriptor);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }
                    ilEncoder.OpCode(ILOpCode.Ldftn);
                    ilEncoder.Token(methodHandle);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(_bclReferences.GetFuncCtorRef(jsParamCount, requiresScopes: false));
                    EmitInitializeFunctionInstance(createFunc.CallableId, createFunc.IsAsync, ilEncoder);
                    EmitInitializeGeneratorFunctionSurfaceIfNeeded(callableId, ilEncoder);

                    if (createFunc.IsAsyncGeneratorFunction)
                    {
                        var initAsyncGeneratorFunctionRef = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.AsyncGeneratorFunction),
                            nameof(JavaScriptRuntime.AsyncGeneratorFunction.InitializeFunctionObject),
                            new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(initAsyncGeneratorFunctionRef);
                    }

                    EmitStoreTemp(createFunc.Result, ilEncoder, allocation);
                    break;
                }

            case LIRCallRuntimeServicesStatic callRuntimeServices:
                {
                    if (!IsMaterialized(callRuntimeServices.Result, allocation))
                    {
                        break;
                    }

                    if (TryEmitOperatorsAddAndToNumber(callRuntimeServices, ilEncoder, allocation, methodDescriptor))
                    {
                        EmitStoreTemp(callRuntimeServices.Result, ilEncoder, allocation);
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
}
