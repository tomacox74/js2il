using Jroc.IR;
using Jroc.Services.ILGenerators;
using Jroc.Utilities.Ecma335;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

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

        void EmitReceiver()
        {
            if (callIntrinsicBaseCtor.UsesLexicalReceiver)
            {
                var getSuperReceiver = _memberRefRegistry.GetOrAddMethod(
                    typeof(JavaScriptRuntime.RuntimeServices),
                    nameof(JavaScriptRuntime.RuntimeServices.GetCurrentLexicalSuperReceiver));
                ilEncoder.OpCode(ILOpCode.Call);
                ilEncoder.Token(getSuperReceiver);
                ilEncoder.OpCode(ILOpCode.Castclass);
                ilEncoder.Token(_memberRefRegistry.GetOrAddTypeHandle(typeof(JavaScriptRuntime.Array)));
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Ldarg_0);
            }
        }

        // Base constructor must be invoked before instance usage.
        EmitReceiver();
        var baseCtor = _memberRefRegistry.GetOrAddConstructor(typeof(JavaScriptRuntime.Array), Type.EmptyTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(baseCtor);

        EmitReceiver();

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

        var initializeDerivedThis = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.RuntimeServices),
            nameof(JavaScriptRuntime.RuntimeServices.InitializeDerivedConstructorThisBinding),
            parameterTypes: new[] { typeof(object) });
        EmitReceiver();
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(initializeDerivedThis);
    }
}
