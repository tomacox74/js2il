using Jroc.IR;
using Jroc.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private void EmitGuardedStringIntrinsicCall(
        LIRCallGuardedStringIntrinsic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (instruction.Arguments.Count > 5
            || instruction.IntrinsicParameterTypes.Count
                != instruction.Arguments.Count + 1
            || instruction.IntrinsicParameterTypes[0] != typeof(string)
            || instruction.IntrinsicReturnClrType == typeof(void)
            || (instruction.FallbackResultConversion
                    == LIRGuardedStringFallbackResultConversion.ToNumber
                && instruction.IntrinsicReturnClrType != typeof(double)))
        {
            throw new InvalidOperationException(
                $"Invalid guarded String intrinsic call shape for '{instruction.MemberName}'.");
        }

        var fallbackLabel = ilEncoder.DefineLabel();
        var typeFallbackLabel = instruction.ReceiverIsProvenString
            ? default
            : ilEncoder.DefineLabel();
        var doneLabel = ilEncoder.DefineLabel();

        ilEncoder.LoadConstantI4(
            (int)JavaScriptRuntime.IntrinsicPrototypeFamily.String);
        var isPristine = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.IntrinsicPrototypeEpochs),
            nameof(JavaScriptRuntime.IntrinsicPrototypeEpochs.IsPristine),
            new[]
            {
                typeof(JavaScriptRuntime.IntrinsicPrototypeFamily)
            });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(isPristine);
        ilEncoder.Branch(ILOpCode.Brfalse, fallbackLabel);

        if (instruction.ReceiverIsProvenString)
        {
            EmitLoadTempAsString(
                instruction.Receiver,
                ilEncoder,
                allocation,
                methodDescriptor);
        }
        else
        {
            EmitLoadTempAsObject(
                instruction.Receiver,
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Isinst);
            ilEncoder.Token(_bclReferences.StringType);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.Branch(ILOpCode.Brfalse, typeFallbackLabel);
        }

        for (var i = 0; i < instruction.Arguments.Count; i++)
        {
            EmitLoadTempForParameter(
                instruction.Arguments[i],
                instruction.IntrinsicParameterTypes[i + 1],
                ilEncoder,
                allocation,
                methodDescriptor);
        }

        var intrinsicMethod = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.String),
            instruction.IntrinsicMethodName,
            instruction.IntrinsicParameterTypes.ToArray());
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(intrinsicMethod);
        EmitStoreGuardedStringFastResult(
            instruction,
            ilEncoder,
            allocation);
        ilEncoder.Branch(ILOpCode.Br, doneLabel);

        if (!instruction.ReceiverIsProvenString)
        {
            ilEncoder.MarkLabel(typeFallbackLabel);
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.MarkLabel(fallbackLabel);
        EmitGuardedStringFallbackCall(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);
        EmitStoreGuardedStringFallbackResult(
            instruction,
            ilEncoder,
            allocation);

        ilEncoder.MarkLabel(doneLabel);
    }

    private void EmitStoreGuardedStringFastResult(
        LIRCallGuardedStringIntrinsic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation)
    {
        if (!IsMaterialized(instruction.Result, allocation))
        {
            if (instruction.IntrinsicReturnClrType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }

            return;
        }

        var resultStorage = GetMaterializedTempStorage(
            instruction.Result,
            allocation);
        if (resultStorage.Kind == ValueStorageKind.Reference
            && resultStorage.ClrType == typeof(object)
            && instruction.IntrinsicReturnClrType.IsValueType)
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(
                _memberRefRegistry.GetOrAddTypeHandle(
                    instruction.IntrinsicReturnClrType));
        }

        EmitStoreTemp(
            instruction.Result,
            ilEncoder,
            allocation);
    }

    private void EmitGuardedStringFallbackCall(
        LIRCallGuardedStringIntrinsic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        EmitLoadTempAsObject(
            instruction.Receiver,
            ilEncoder,
            allocation,
            methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.MemberName);
        foreach (var argument in instruction.Arguments)
        {
            EmitLoadTempAsObject(
                argument,
                ilEncoder,
                allocation,
                methodDescriptor);
        }

        var parameterTypes = new Type[instruction.Arguments.Count + 2];
        parameterTypes[0] = typeof(object);
        parameterTypes[1] = typeof(string);
        global::System.Array.Fill(
            parameterTypes,
            typeof(object),
            startIndex: 2,
            count: instruction.Arguments.Count);
        var callMember = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.ObjectRuntime),
            $"CallMember{instruction.Arguments.Count}",
            parameterTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(callMember);

        if (instruction.FallbackResultConversion
            == LIRGuardedStringFallbackResultConversion.ToNumber)
        {
            var toNumber = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.TypeUtilities),
                nameof(JavaScriptRuntime.TypeUtilities.ToNumber),
                new[] { typeof(object) });
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(toNumber);
        }
    }

    private void EmitStoreGuardedStringFallbackResult(
        LIRCallGuardedStringIntrinsic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation)
    {
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(
                instruction.Result,
                ilEncoder,
                allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
    }
}
