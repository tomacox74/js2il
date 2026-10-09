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

                    if (newUserClass.NeedsScopes)
                    {
                        if (newUserClass.ScopesArray is not { } scopesTemp)
                        {
                            return false;
                        }
                        EmitLoadTemp(scopesTemp, ilEncoder, allocation, methodDescriptor);
                    }

                    EmitPushUserClassConstructionContext(newUserClass, ilEncoder, allocation, methodDescriptor);

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

                    {
                        var popCurrentNewTarget = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PopCurrentNewTarget),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(popCurrentNewTarget);
                    }

                    // Restore the invocation frame arguments to their pre-construction state.
                    {
                        var popCurrentArguments = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.PopCurrentArguments),
                            parameterTypes: Type.EmptyTypes);
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(popCurrentArguments);
                        // Stack: [instance] (unchanged — PopCurrentArguments returns void)
                    }

                    void EmitAttachClassPrototype()
                    {
                        EmitLoadTempAsObject(
                            newUserClass.NewTarget,
                            ilEncoder,
                            allocation,
                            methodDescriptor);

                        ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString("prototype"));
                        var getProperty = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.ObjectRuntime),
                            nameof(JavaScriptRuntime.ObjectRuntime.GetProperty),
                            parameterTypes: new[] { typeof(object), typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(getProperty);

                        var setPrototype = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.PrototypeChain),
                            nameof(JavaScriptRuntime.PrototypeChain.SetPrototype),
                            parameterTypes: new[] { typeof(object), typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(setPrototype);
                    }

                    var ctorReturnField = default(FieldDefinitionHandle);
                    bool hasConstructorReturn = classRegistry != null
                        && classRegistry.TryGetPrivateField(
                            newUserClass.RegistryClassName,
                            "__jroc_ctorReturn",
                            out ctorReturnField);
                    if (!newUserClass.IsDerivedConstructor && !hasConstructorReturn)
                    {
                        if (resultUsed)
                        {
                            EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                            if (hasPrototype)
                            {
                                EmitLoadTemp(newUserClass.Result, ilEncoder, allocation, methodDescriptor);
                                EmitAttachClassPrototype();
                            }
                        }
                        else if (hasPrototype)
                        {
                            EmitAttachClassPrototype();
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Pop);
                        }
                        break;
                    }

                    if (hasPrototype)
                    {
                        // Attach the class prototype only to the receiver allocated by newobj.
                        // A function-valued base constructor may replace the derived result object.
                        ilEncoder.OpCode(ILOpCode.Dup);
                        EmitAttachClassPrototype();
                    }

                    if (newUserClass.IsDerivedConstructor || hasConstructorReturn)
                    {
                        // Read the return slot from the allocated receiver, before choosing
                        // a replacement object or resolving a possibly uninitialized `this`.
                        if (hasConstructorReturn)
                        {
                            ilEncoder.OpCode(ILOpCode.Dup);
                            ilEncoder.OpCode(ILOpCode.Ldfld);
                            ilEncoder.Token(ctorReturnField);
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        if (newUserClass.IsDerivedConstructor)
                        {
                            ilEncoder.Call(_memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.RuntimeServices),
                                nameof(JavaScriptRuntime.RuntimeServices.GetCurrentThis),
                                parameterTypes: Type.EmptyTypes));
                            ilEncoder.Call(_memberRefRegistry.GetOrAddMethod(
                                typeof(JavaScriptRuntime.RuntimeServices),
                                nameof(JavaScriptRuntime.RuntimeServices.PopDerivedConstructorThisBinding),
                                parameterTypes: Type.EmptyTypes));
                        }
                        else
                        {
                            ilEncoder.OpCode(ILOpCode.Ldnull);
                        }
                        ilEncoder.LoadConstantI4(newUserClass.IsDerivedConstructor ? 1 : 0);
                        ilEncoder.Call(_memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.RuntimeServices),
                            nameof(JavaScriptRuntime.RuntimeServices.ResolveClassConstructorResult),
                            parameterTypes: new[] { typeof(object), typeof(object), typeof(object), typeof(bool) }));
                    }

                    if (resultUsed)
                    {
                        EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);
                    }
                    else
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                    }
                    break;
                }

            default:
                return null;
        }

        return true;
    }

    private void EmitPushUserClassConstructionContext(
        LIRNewUserClass construction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // The invocation frame retains extra arguments even when the CLR signature cannot accept them.
        ilEncoder.LoadConstantI4(construction.Arguments.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);
        for (var index = 0; index < construction.Arguments.Count; index++)
        {
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(index);
            EmitLoadTempAsObject(construction.Arguments[index], ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
        ilEncoder.Call(_memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.PushCurrentArguments),
            parameterTypes: new[] { typeof(object[]) }));
        EmitLoadTempAsObject(construction.NewTarget, ilEncoder, allocation, methodDescriptor);
        ilEncoder.Call(_memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.PushCurrentNewTarget),
            parameterTypes: new[] { typeof(object) }));
    }
}
