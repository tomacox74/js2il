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
    private bool? TryCompileInstructionToIL_Intrinsics(
        LIRInstruction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        switch (instruction)
        {
            case LIRGetUserClassType getUserClassType:
                if (!IsMaterialized(getUserClassType.Result, allocation))
                {
                    break;
                }
                {
                    var classRegistry = _serviceProvider.GetService<Js2IL.Services.ClassRegistry>();
                    if (classRegistry == null || !classRegistry.TryGet(getUserClassType.RegistryClassName, out var typeDef))
                    {
                        throw new InvalidOperationException($"Class not found in registry: '{getUserClassType.RegistryClassName}'");
                    }

                    ilEncoder.OpCode(ILOpCode.Ldtoken);
                    ilEncoder.Token(typeDef);

                    var getTypeFromHandle = _memberRefRegistry.GetOrAddMethod(
                        typeof(Type),
                        nameof(Type.GetTypeFromHandle),
                        parameterTypes: new[] { typeof(RuntimeTypeHandle) });
                    ilEncoder.OpCode(ILOpCode.Call);
                    ilEncoder.Token(getTypeFromHandle);

                    EmitStoreTemp(getUserClassType.Result, ilEncoder, allocation);
                }
                break;
            case LIRGetIntrinsicGlobal getIntrinsicGlobal:
                if (!IsMaterialized(getIntrinsicGlobal.Result, allocation))
                {
                    break;
                }
                EmitLoadIntrinsicGlobalVariable(getIntrinsicGlobal.Name, ilEncoder);
                EmitStoreTemp(getIntrinsicGlobal.Result, ilEncoder, allocation);
                break;
            case LIRCallIntrinsic callIntrinsic:
                EmitLoadTemp(callIntrinsic.IntrinsicObject, ilEncoder, allocation, methodDescriptor);
                EmitLoadTemp(callIntrinsic.ArgumentsArray, ilEncoder, allocation, methodDescriptor);
                EmitInvokeIntrinsicMethod(typeof(JavaScriptRuntime.Console), callIntrinsic.Name, ilEncoder);

                if (IsMaterialized(callIntrinsic.Result, allocation))
                {
                    EmitStoreTemp(callIntrinsic.Result, ilEncoder, allocation);
                }
                else
                {
                    ilEncoder.OpCode(ILOpCode.Pop);
                }
                break;

            case LIRCallIntrinsicGlobalFunction callGlobalFunc:
                EmitIntrinsicGlobalFunctionCall(callGlobalFunc, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallInstanceMethod callInstance:
                EmitInstanceMethodCall(callInstance, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStatic callIntrinsicStatic:
                EmitIntrinsicStaticCall(callIntrinsicStatic, ilEncoder, allocation, methodDescriptor);
                break;
            case LIRCallIntrinsicStaticVoid callIntrinsicStaticVoid:
                EmitIntrinsicStaticVoidCall(callIntrinsicStaticVoid, ilEncoder, allocation, methodDescriptor);
                break;

            default:
                return null;
        }

        return true;
    }
}
