using Jroc.IR;
using Jroc.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    private void EmitGuardedIntrinsicMemberCall(
        LIRCallGuardedIntrinsicMember instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicMethod = ResolveTypedInstanceMethodOverload(
            instruction.ReceiverClrType,
            instruction.MemberName,
            instruction.Arguments.Count)
            ?? throw new InvalidOperationException(
                $"No guarded intrinsic method found: "
                + $"{instruction.ReceiverClrType.FullName}."
                + $"{instruction.MemberName} with "
                + $"{instruction.Arguments.Count} argument(s).");
        var fallbackLabel = ilEncoder.DefineLabel();
        var typeFallbackLabel = instruction.ReceiverIsProvenType
            ? default
            : ilEncoder.DefineLabel();
        var doneLabel = ilEncoder.DefineLabel();

        ilEncoder.LoadConstantI4((int)instruction.PrototypeFamily);
        var isPristine = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.IntrinsicPrototypeEpochs),
            nameof(JavaScriptRuntime.IntrinsicPrototypeEpochs.IsPristine),
            [typeof(JavaScriptRuntime.IntrinsicPrototypeFamily)]);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(isPristine);
        ilEncoder.Branch(ILOpCode.Brfalse, fallbackLabel);

        if (!instruction.ReceiverIsProvenType)
        {
            EmitLoadTempAsObject(
                instruction.Receiver,
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Isinst);
            ilEncoder.Token(
                _typeReferenceRegistry.GetOrAdd(
                    instruction.ReceiverClrType));
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.Branch(ILOpCode.Brfalse, typeFallbackLabel);
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        EmitLoadTempAsObject(
            instruction.Receiver,
            ilEncoder,
            allocation,
            methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.MemberName);
        var hasOwnOverride = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.HasOwnIntrinsicMemberOverride),
            [typeof(object), typeof(string)]);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(hasOwnOverride);
        ilEncoder.Branch(ILOpCode.Brtrue, fallbackLabel);

        EmitLoadTempAsObject(
            instruction.Receiver,
            ilEncoder,
            allocation,
            methodDescriptor);
        ilEncoder.LoadConstantI4((int)instruction.PrototypeFamily);
        var hasDefaultPrototype = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.IntrinsicPrototypeEpochs),
            nameof(JavaScriptRuntime.IntrinsicPrototypeEpochs.HasDefaultPrototype),
            [typeof(object), typeof(JavaScriptRuntime.IntrinsicPrototypeFamily)]);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(hasDefaultPrototype);
        ilEncoder.Branch(ILOpCode.Brfalse, fallbackLabel);

        EmitLoadInstanceMethodReceiver(
            instruction.Receiver,
            instruction.ReceiverClrType,
            ilEncoder,
            allocation,
            methodDescriptor);

        var parameters = intrinsicMethod.GetParameters();
        EmitInstanceMethodArguments(
            instruction.Arguments,
            parameters,
            ilEncoder,
            allocation,
            methodDescriptor);
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            instruction.ReceiverClrType,
            intrinsicMethod.Name,
            parameters
                .Select(static parameter => parameter.ParameterType)
                .ToArray());
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodRef);

        if (intrinsicMethod.ReturnType == typeof(void))
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }
        else if (intrinsicMethod.ReturnType.IsValueType)
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(
                _typeReferenceRegistry.GetOrAdd(
                    intrinsicMethod.ReturnType));
        }

        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
        ilEncoder.Branch(ILOpCode.Br, doneLabel);

        if (!instruction.ReceiverIsProvenType)
        {
            ilEncoder.MarkLabel(typeFallbackLabel);
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.MarkLabel(fallbackLabel);
        EmitGuardedIntrinsicMemberFallback(
            instruction,
            ilEncoder,
            allocation,
            methodDescriptor);
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.MarkLabel(doneLabel);
    }

    private void EmitGuardedIntrinsicMemberFallback(
        LIRCallGuardedIntrinsicMember instruction,
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
    }

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
        var stringObjectFallbackLabel = instruction.ReceiverIsProvenString
            ? default
            : ilEncoder.DefineLabel();
        var fastPathLabel = instruction.ReceiverIsProvenString
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
            ilEncoder.Branch(ILOpCode.Brtrue, fastPathLabel);
            ilEncoder.OpCode(ILOpCode.Pop);

            EmitLoadTempAsObject(
                instruction.Receiver,
                ilEncoder,
                allocation,
                methodDescriptor);
            ilEncoder.Ldstr(
                _metadataBuilder,
                instruction.MemberName);
            var tryUnwrap = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.IntrinsicPrototypeEpochs),
                nameof(JavaScriptRuntime.IntrinsicPrototypeEpochs
                    .TryUnwrapStringObjectReceiver),
                [typeof(object), typeof(string)]);
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(tryUnwrap);
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.Branch(
                ILOpCode.Brfalse,
                stringObjectFallbackLabel);
            ilEncoder.MarkLabel(fastPathLabel);
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
            ilEncoder.MarkLabel(stringObjectFallbackLabel);
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
