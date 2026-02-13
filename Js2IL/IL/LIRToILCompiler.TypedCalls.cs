using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Typed Calls

    private void EmitCallTypedMemberNoFallback(
        LIRCallTypedMember instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        EmitCallTypedMemberNoFallbackCore(instruction, ilEncoder, allocation, methodDescriptor);

        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitBoxIfNeededForTypedCallResult(instruction.Result, instruction.ReturnClrType, ilEncoder);
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
    }

    private void EmitBoxIfNeededForTypedCallResult(TempVariable resultTemp, Type returnClrType, InstructionEncoder ilEncoder)
    {
        var resultStorage = GetTempStorage(resultTemp);
        if (resultStorage.Kind == ValueStorageKind.Reference
            && resultStorage.ClrType == typeof(object)
            && returnClrType != typeof(object)
            && returnClrType.IsValueType)
        {
            ilEncoder.OpCode(ILOpCode.Box);
            ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(returnClrType));
        }
    }

    private void EmitCallTypedMemberNoFallbackCore(
        LIRCallTypedMember instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Receiver: if already proven to be the resolved user-class type (e.g., stored in a typed local),
        // avoid the redundant cast.
        var receiverStorage = GetTempStorage(instruction.Receiver);
        if (receiverStorage.Kind == ValueStorageKind.Reference
            && !receiverStorage.TypeHandle.IsNil
            && receiverStorage.TypeHandle.Equals(instruction.ReceiverTypeHandle))
        {
            EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Castclass);
            ilEncoder.Token(instruction.ReceiverTypeHandle);
        }

        // Async class methods follow the js2il ABI and take a leading scopes array parameter.
        if (instruction.HasScopesParameter)
        {
            EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
        }

        // Match the declared signature (ignore extra args, pad missing args with null).
        int jsParamCount = instruction.MaxParamCount;
        int argsToPass = Math.Min(instruction.Arguments.Count, jsParamCount);

        for (int i = 0; i < argsToPass; i++)
        {
            EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
        }

        for (int i = argsToPass; i < jsParamCount; i++)
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }

        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(instruction.MethodHandle);
    }

    private void EmitCallTypedMemberWithFallback(
        LIRCallTypedMemberWithFallback instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var fallbackLabel = ilEncoder.DefineLabel();
        var doneLabel = ilEncoder.DefineLabel();

        // Receiver type-test
        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.OpCode(ILOpCode.Isinst);
        ilEncoder.Token(instruction.ReceiverTypeHandle);
        ilEncoder.OpCode(ILOpCode.Dup);
        ilEncoder.Branch(ILOpCode.Brfalse, fallbackLabel);

        // Direct call path (typed receiver on stack)
        if (instruction.HasScopesParameter)
        {
            EmitLoadScopesArrayOrEmpty(ilEncoder, methodDescriptor);
        }

        int jsParamCount = instruction.MaxParamCount;
        int argsToPass = Math.Min(instruction.Arguments.Count, jsParamCount);
        for (int i = 0; i < argsToPass; i++)
        {
            EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
        }

        for (int i = argsToPass; i < jsParamCount; i++)
        {
            ilEncoder.OpCode(ILOpCode.Ldnull);
        }

        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(instruction.MethodHandle);

        if (IsMaterialized(instruction.Result, allocation))
        {
            var resultStorage = GetTempStorage(instruction.Result);
            if (resultStorage.Kind == ValueStorageKind.Reference
                && resultStorage.ClrType == typeof(object)
                && instruction.ReturnClrType != typeof(object)
                && instruction.ReturnClrType.IsValueType)
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(instruction.ReturnClrType));
            }

            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }

        ilEncoder.Branch(ILOpCode.Br, doneLabel);

        // Fallback: pop null typed receiver and do runtime dispatch
        ilEncoder.MarkLabel(fallbackLabel);
        ilEncoder.OpCode(ILOpCode.Pop);

        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.MethodName);
        
        // Use arity-specific overload if possible (0-3 args)
        int argCount = instruction.Arguments.Count;
        if (argCount <= 3)
        {
            // Emit individual arguments
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
            
            // Select arity-specific overload
            Type[] paramTypes = argCount switch
            {
                0 => new[] { typeof(object), typeof(string) },
                1 => new[] { typeof(object), typeof(string), typeof(object) },
                2 => new[] { typeof(object), typeof(string), typeof(object), typeof(object) },
                3 => new[] { typeof(object), typeof(string), typeof(object), typeof(object), typeof(object) },
                _ => throw new InvalidOperationException("Unexpected arity")
            };
            
            string methodName = argCount == 0 ? "CallMember0" : $"CallMember{argCount}";
            var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Object),
                methodName,
                paramTypes);
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(callMemberRef);
        }
        else
        {
            // Fall back to array-based call for > 3 args
            EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);

            var callMemberRef = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.Object),
                nameof(JavaScriptRuntime.Object.CallMember),
                new[] { typeof(object), typeof(string), typeof(object[]) });
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(callMemberRef);
        }

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

    #endregion
}
