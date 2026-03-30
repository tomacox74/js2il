using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using Js2IL.Utilities.Ecma335;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    private void EmitIntrinsicBaseConstructorCallCore(
        LIRCallIntrinsicBaseConstructor callIntrinsicBaseCtor,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (!string.Equals(callIntrinsicBaseCtor.IntrinsicName, "Array", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported intrinsic base constructor: '{callIntrinsicBaseCtor.IntrinsicName}'");
        }

        // Base constructor must be invoked before instance usage.
        // Emit: ldarg.0; call instance void JavaScriptRuntime.Array::.ctor()
        ilEncoder.OpCode(ILOpCode.Ldarg_0);
        var baseCtor = _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Array), Type.EmptyTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(baseCtor);

        // Emit: ldarg.0; callvirt instance void JavaScriptRuntime.Array::ConstructInto(object[])
        ilEncoder.OpCode(ILOpCode.Ldarg_0);

        int argc = callIntrinsicBaseCtor.Arguments.Count;
        ilEncoder.LoadConstantI4(argc);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);

        for (int i = 0; i < argc; i++)
        {
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(i);
            EmitLoadTempAsObject(callIntrinsicBaseCtor.Arguments[i], ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }

        var initRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Array),
            nameof(JavaScriptRuntime.Array.ConstructInto),
            new[] { typeof(object[]) });

        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(initRef);
    }
}
