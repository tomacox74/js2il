using Js2IL.IR;
using Js2IL.Services;
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
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();

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
                            EmitLoadTemp(newUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                            ilEncoder.OpCode(ILOpCode.Stelem_ref);
                        }

                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(pushCurrentArguments);
                        // Stack unchanged: [] or [scopes] (PushCurrentArguments is void)
                    }

                    // In JavaScript, extra constructor arguments are evaluated (side effects) but ignored.
                    // LIR lowering already evaluates all arguments; here we only pass the declared maximum.
                    int argsToPass = Math.Min(argc, newUserClass.MaxArgCount);
                    for (int i = 0; i < argsToPass; i++)
                    {
                        EmitLoadTemp(newUserClass.Arguments[i], ilEncoder, allocation, methodDescriptor);
                    }

                    int paddingNeeded = newUserClass.MaxArgCount - argsToPass;
                    for (int i = 0; i < paddingNeeded; i++)
                    {
                        ilEncoder.OpCode(ILOpCode.Ldnull);
                    }

                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctorDef);
                    // Stack: [instance]

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

                    var classTypeForPrototype = default(TypeDefinitionHandle);
                    bool hasPrototype = classRegistry != null
                        && classRegistry.TryGet(newUserClass.RegistryClassName, out classTypeForPrototype);

                    bool resultUsed = IsMaterialized(newUserClass.Result, allocation);

                    if (!resultUsed && !hasPrototype)
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                        break;
                    }

                    if (resultUsed)
                    {
                        // Store the constructed instance as the default result.
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
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
                        && classRegistry.TryGetPrivateField(newUserClass.RegistryClassName, "__js2il_ctorReturn", out var ctorReturnField)
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

                        // If not an object (primitive) => keep constructed instance.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        var isOverride = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.TypeUtilities),
                            nameof(JavaScriptRuntime.TypeUtilities.IsConstructorReturnOverride),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(isOverride);
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
