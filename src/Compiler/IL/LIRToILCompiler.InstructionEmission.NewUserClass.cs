using Jroc.IR;
using Jroc.Services;
using Jroc.Services.ILGenerators;
using Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_NewUserClass(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRNewUserClass newUserClass:
                {
                    var reader = _serviceProvider.GetService<ICallableDeclarationReader>();
                    var classRegistry = _serviceProvider.GetService<Jroc.Services.ClassRegistry>();

                    MethodDefinitionHandle ctorDef;
                    if (reader != null
                        && reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token)
                        && token.Kind == HandleKind.MethodDefinition)
                    {
                        ctorDef = (MethodDefinitionHandle)token;
                    }
                    else if (classRegistry != null
                        && classRegistry.TryGetConstructor(newUserClass.RegistryClassName, out var registeredCtorDef, out _, out _, out _))
                    {
                        ctorDef = registeredCtorDef;
                    }
                    else
                    {
                        return false;
                    }

                    int argc = newUserClass.Arguments.Count;
                    if (argc < newUserClass.MinArgCount)
                    {
                        var expectedMinArgs = newUserClass.MinArgCount;
                        var expectedMaxArgs = newUserClass.MaxArgCount;

                        if (expectedMinArgs == expectedMaxArgs)
                        {
                            ILEmitHelpers.ThrowNotSupported(
                                $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs} argument(s) but call site has {argc}.");
                        }

                        ILEmitHelpers.ThrowNotSupported(
                            $"Constructor for class '{newUserClass.ClassName}' expects {expectedMinArgs}-{expectedMaxArgs} argument(s) but call site has {argc}.");
                    }

                    if (newUserClass.NeedsScopes)
                    {
                        if (newUserClass.ScopesArray is not { } scopesTemp)
                        {
                            return false;
                        }
                        EmitLoadTemp(scopesTemp, ilEncoder, allocation, methodDescriptor);
                    }

                    // Before the newobj call, push all actual call-site arguments into _currentArguments so that
                    // the 'arguments' keyword inside the constructor chain (including base constructors) reflects
                    // the actual values passed by the caller — even when MaxArgCount is 0 (default derived ctor).
                    {
                        var pushCurrentArguments = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PushCurrentArguments),
                            parameterTypes: new[] { typeof(object[]) });

                        // Build object[] of all argc arguments. The array itself is consumed by PushCurrentArguments
                        // (void return), so this block does not perturb the eval stack (scopes may already be pushed).
                        ilEncoder.LoadConstantI4(argc);
                        ilEncoder.OpCode(ILOpCode.Newarr);
                        ilEncoder.Token(_bclReferences.ObjectType);

                        for (int i = 0; i < argc; i++)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.LoadConstantI4(i);
                            EmitLoadTempAsObject(newUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }

                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(pushCurrentArguments);
                        // Stack unchanged: [] or [scopes] (PushCurrentArguments is void)
                    }

                    if (newUserClass.IsDerivedConstructor)
                    {
                        var pushDerivedThis = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PushDerivedConstructorThisBinding),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(pushDerivedThis);
                        // Stack unchanged: [] or [scopes]. The binding is mutable so arrows created
                        // before super() can observe initialization after the super() call.
                    }

                    // In JavaScript, extra constructor arguments are evaluated (side effects) but ignored.
                    // LIR lowering already evaluates all arguments; here we only pass the declared maximum.
                    int argsToPass = Math.Min(argc, newUserClass.MaxArgCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        var parameterClrType = i < newUserClass.ParameterClrTypes.Count
                            ? newUserClass.ParameterClrTypes[i]
                            : null;
                        EmitLoadTempAsParameterType(
                            newUserClass.Arguments[i],
                            parameterClrType,
                            ilEncoder,
                            allocation,
                            methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argsToPass;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);
                    // Stack: [instance]

                    var classTypeForPrototype = default(TypeDefinitionHandle);
                    bool hasPrototype = classRegistry != null
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out classTypeForPrototype);

                    bool resultUsed = IsMaterialized(newUserClass.Result, allocation);
                    bool resultAlreadyStored = false;

                    // Restore _currentArguments to its pre-construction state.
                    {
                        var popCurrentArguments = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PopCurrentArguments),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(popCurrentArguments);
                        // Stack: [instance] (unchanged — PopCurrentArguments returns void)
                    }

                    if (newUserClass.IsDerivedConstructor && resultUsed)
                    {
                        // The derived constructor's result is the initialized lexical `this` binding,
                        // which may be a replacement object returned by a function-valued base constructor.
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                        resultAlreadyStored = true;

                        var getThis = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getThis);

                        var resolveThis = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.ResolveLexicalThis),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(resolveThis);
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                    }

                    if (newUserClass.IsDerivedConstructor)
                    {
                        var popDerivedThis = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PopDerivedConstructorThisBinding),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(popDerivedThis);
                        // Stack: [instance] (unchanged — PopDerivedConstructorThisBinding returns void)
                    }

                    if (!resultUsed && !hasPrototype)
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                        break;
                    }

                    if (resultUsed)
                    {
                        if (!resultAlreadyStored)
                        {
                            // Store the constructed instance as the default result.
                            EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                        }
                    }

                    if (hasPrototype)
                    {
                        if (resultUsed)
                        {
                            // Reload instance from result local.
                            EmitLoadTemp(newUserClass.Result, ilEncoder, allocation, methodDescriptor);
                        }
                        // Else: instance is still on the stack from newobj — use it directly.
                        // Stack: [instance]

                        ilEncoder.OpCode(ILOpCode.Ldtoken);
                        ilEncoder.Token(classTypeForPrototype);
                        var getTypeFromHandleForPrototype = _memberRefRegistry.GetOrAddMethod(
                            typeof(Type),
                            nameof(Type.GetTypeFromHandle),
                            parameterTypes: new[] { typeof(RuntimeTypeHandle) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getTypeFromHandleForPrototype);

                        ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString("prototype"));
                        var getProperty = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.Object),
                            nameof(JavaScriptRuntime.Object.GetProperty),
                            parameterTypes: new[] { typeof(object), typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getProperty);

                        var setPrototype = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PrototypeChain),
                            nameof(JavaScriptRuntime.PrototypeChain.SetPrototype),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(setPrototype);
                        // Stack: [] (instance consumed by SetPrototype)
                    }

                    if (!resultUsed)
                    {
                        // Prototype was set; instance was consumed. Nothing left to do.
                        break;
                    }

                    // PL5.4a: If the JS constructor explicitly returned an object, new-expr evaluates to that object;
                    // if it returned a primitive/null/undefined, the constructed instance is used.
                    if (classRegistry != null
                        && classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__jroc_ctorReturn", out var ctorReturnField)
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out var classTypeHandle))
                    {
                        var keepThis = ilEncoder.DefineLabel();
                        var done = ilEncoder.DefineLabel();

                        // Load the hidden ctor return field from the constructed instance.
                        EmitLoadTemp(newUserClass.Result, ilEncoder, allocation, methodDescriptor);
                        ilEncoder.OpCode(ILOpCode.Castclass);
                        ilEncoder.Token(classTypeHandle);
                        ilEncoder.OpCode(ILOpCode.Ldfld);
                        ilEncoder.Token(ctorReturnField);

                        // If null/undefined => keep constructed instance.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        ilEncoder.Branch(ILOpCode.Brfalse, keepThis);

                        // If not an object (primitive) => base constructors keep the instance,
                        // but derived constructors must reject explicit primitive returns.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        var isOverride = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.IsConstructorReturnOverride),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(isOverride);
                        if (newUserClass.IsDerivedConstructor)
                        {
                            var overrideObject = ilEncoder.DefineLabel();
                            ilEncoder.Branch(ILOpCode.Brtrue, overrideObject);

                            ilEncoder.OpCode(ILOpCode.Pop);
                            var typeErrorCtor = _memberRefRegistry.GetOrAddConstructor(
                                typeof(JavaScriptRuntime.TypeError),
                                parameterTypes: new[] { typeof(string) });
                            ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString("Derived constructors may only return object or undefined"));
                            ilEncoder.OpCode(ILOpCode.Newobj);
                            ilEncoder.Token(typeErrorCtor);
                            ilEncoder.OpCode(ILOpCode.Throw);

                            ilEncoder.MarkLabel(overrideObject);
                        }
                        ilEncoder.Branch(ILOpCode.Brfalse, keepThis);

                        // Override result with the returned object.
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                        ilEncoder.Branch(ILOpCode.Br, done);

                        // Keep constructed instance; discard the return value.
                        ilEncoder.MarkLabel(keepThis);
                        ilEncoder.OpCode(ILOpCode.Pop);
                        ilEncoder.MarkLabel(done);
                    }
                    break;
                }

            default:
                return null;
        }

        return true;
    }
}
