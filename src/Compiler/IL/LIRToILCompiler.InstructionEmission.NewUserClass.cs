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
                    if (reader == null)
                    {
                        return false;
                    }

                    if (!reader.TryGetDeclaredToken(newUserClass.ConstructorCallableId, out var token) || token.Kind != HandleKind.MethodDefinition)
                    {
                        return false;
                    }

                    var ctorDef = (MethodDefinitionHandle)token;

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

                    if (!IsMaterialized(newUserClass.Result, allocation))
                    {
                        ilEncoder.OpCode(ILOpCode.Pop);
                        break;
                    }

                    // Store the constructed instance as the default result.
                    EmitStoreTemp(newUserClass.Result, ilEncoder, allocation);

                    // PL5.4a: If the JS constructor explicitly returned an object, new-expr evaluates to that object;
                    // if it returned a primitive/null/undefined, the constructed instance is used.
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
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
