using Js2IL.IR;
using Js2IL.Services;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Services.VariableBindings;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private bool? TryCompileInstructionToIL_TempsAndExceptions(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            // Copy temp variable
            case LIRCopyTemp copyTemp:
                if (TryGetSameILLocalSlot(copyTemp.Source, copyTemp.Destination, allocation, out _))
                {
                    return true;
                }
                EmitLoadTemp(copyTemp.Source, ilEncoder, allocation, methodDescriptor);
                EmitStoreTemp(copyTemp.Destination, ilEncoder, allocation);
                return true;

            case LIRStoreException storeException:
                // Exception object is on stack at catch handler entry.
                EmitStoreTemp(storeException.Result, ilEncoder, allocation);
                return true;

            case LIRUnwrapCatchException unwrapCatch:
                {
                    // Unwrap CLR exception to JS catch value.
                    // Stack discipline: ensure stack is empty on all paths.
                    var isThrownValue = ilEncoder.DefineLabel();
                    var isJsError = ilEncoder.DefineLabel();
                    var done = ilEncoder.DefineLabel();

                    var thrownType = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.JsThrownValueException));
                    var errorType = _typeReferenceRegistry.GetOrAdd(typeof(JavaScriptRuntime.Error));

                    // Load exception (object)
                    EmitLoadTemp(unwrapCatch.Exception, ilEncoder, allocation, methodDescriptor);

                    // dup; isinst JsThrownValueException
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(thrownType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, isThrownValue);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // dup; isinst Error
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(errorType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, isJsError);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // Unknown exception: discard original and rethrow.
                    ilEncoder.OpCode(ILOpCode.Pop);
                    ilEncoder.OpCode(ILOpCode.Rethrow);

                    ilEncoder.MarkLabel(isThrownValue);
                    // Stack: ex, (JsThrownValueException)
                    ilEncoder.OpCode(ILOpCode.Pop); // pop ex
                    var getValue = _memberRefRegistry.GetOrAddMethod(
                        typeof(JavaScriptRuntime.JsThrownValueException),
                        $"get_{nameof(JavaScriptRuntime.JsThrownValueException.Value)}");
                    ilEncoder.OpCode(ILOpCode.Callvirt);
                    ilEncoder.Token(getValue);
                    EmitStoreTemp(unwrapCatch.Result, ilEncoder, allocation);
                    ilEncoder.Branch(ILOpCode.Br, done);

                    ilEncoder.MarkLabel(isJsError);
                    // Stack: ex, (Error)
                    ilEncoder.OpCode(ILOpCode.Pop); // pop ex
                    EmitStoreTemp(unwrapCatch.Result, ilEncoder, allocation);
                    ilEncoder.MarkLabel(done);
                    return true;
                }

            case LIRThrow throwInstr:
                {
                    // Throw JS value: if already a CLR Exception, throw it; otherwise wrap.
                    var throwException = ilEncoder.DefineLabel();
                    var exceptionType = _typeReferenceRegistry.GetOrAdd(typeof(System.Exception));
                    var wrapperCtor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.JsThrownValueException),
                        parameterTypes: new[] { typeof(object) });

                    EmitLoadTempAsObject(throwInstr.Value, ilEncoder, allocation, methodDescriptor);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.OpCode(ILOpCode.Isinst);
                    ilEncoder.Token(exceptionType);
                    ilEncoder.OpCode(ILOpCode.Dup);
                    ilEncoder.Branch(ILOpCode.Brtrue, throwException);
                    ilEncoder.OpCode(ILOpCode.Pop); // pop null

                    // Wrap and throw.
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(wrapperCtor);
                    ilEncoder.OpCode(ILOpCode.Throw);

                    ilEncoder.MarkLabel(throwException);
                    // Stack: value, exception
                    ilEncoder.OpCode(ILOpCode.Pop); // pop original value
                    ilEncoder.OpCode(ILOpCode.Throw);
                    return true;
                }

            case LIRThrowNewTypeError throwTypeError:
                {
                    // throw new JavaScriptRuntime.TypeError(message)
                    var ctor = _memberRefRegistry.GetOrAddConstructor(
                        typeof(JavaScriptRuntime.TypeError),
                        parameterTypes: new[] { typeof(string) });
                    ilEncoder.LoadString(_metadataBuilder.GetOrAddUserString(throwTypeError.Message));
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(ctor);
                    ilEncoder.OpCode(ILOpCode.Throw);
                    return true;
                }

            case LIRNewBuiltInError newError:
                {
                    if (!IsMaterialized(newError.Result, allocation))
                    {
                        return true;
                    }

                    var errorClrType = Js2IL.IR.BuiltInErrorTypes.GetRuntimeErrorClrType(newError.ErrorTypeName);

                    if (newError.Message.HasValue)
                    {
                        // JS: Error(message) stringifies message.
                        EmitLoadTempAsObject(newError.Message.Value, ilEncoder, allocation, methodDescriptor);
                        var toString = _memberRefRegistry.GetOrAddMethod(
                            typeof(JavaScriptRuntime.DotNet2JSConversions),
                            nameof(JavaScriptRuntime.DotNet2JSConversions.ToString),
                            parameterTypes: new[] { typeof(object) });
                        ilEncoder.OpCode(ILOpCode.Call);
                        ilEncoder.Token(toString);

                        var ctor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: new[] { typeof(string) });
                        ilEncoder.OpCode(ILOpCode.Newobj);
                        ilEncoder.Token(ctor);
                        EmitStoreTemp(newError.Result, ilEncoder, allocation);
                        return true;
                    }

                    var defaultCtor = _memberRefRegistry.GetOrAddConstructor(errorClrType, parameterTypes: System.Type.EmptyTypes);
                    ilEncoder.OpCode(ILOpCode.Newobj);
                    ilEncoder.Token(defaultCtor);
                    EmitStoreTemp(newError.Result, ilEncoder, allocation);
                    return true;
                }

            case LIRNewIntrinsicObject newIntrinsic:
                {
                    if (!IsMaterialized(newIntrinsic.Result, allocation))
                    {
                        return true;
                    }

                    EmitNewIntrinsicObjectCore(newIntrinsic, ilEncoder, allocation, methodDescriptor);
                    EmitStoreTemp(newIntrinsic.Result, ilEncoder, allocation);
                    return true;
                }

            default:
                return null;
        }
    }
}
