using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Instance Calls

    private void EmitInstanceMethodCall(
        LIRCallInstanceMethod instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Resolve the instance method using heuristics aligned with intrinsic static calls.
        // Prefer object[] signature (variadic JS-style), else exact arity match with object parameters.
        var receiverType = instruction.ReceiverClrType;

        var allMethods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var methods = allMethods
            .Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var argCount = instruction.Arguments.Count;

        var chosen = methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
        });

        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == argCount && ps.All(p => p.ParameterType == typeof(object));
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching instance method found: {receiverType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        // Load receiver
        EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(receiverType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodRef);

        if (IsMaterialized(instruction.Result, allocation))
        {
            // If the CLR method returns void but JS expects a value, treat it as `undefined`.
            if (chosen.ReturnType == typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Ldnull);
            }

            // If the result temp is object-typed but the CLR call returns a value type,
            // box it before storing to avoid invalid IL (e.g., bool -> object).
            var resultStorage = GetTempStorage(instruction.Result);
            if (chosen.ReturnType != typeof(void)
                && chosen.ReturnType.IsValueType
                && resultStorage.Kind == ValueStorageKind.Reference
                && resultStorage.ClrType == typeof(object))
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(chosen.ReturnType));
            }

            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            if (chosen.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    private void EmitInstanceMethodCallInline(
        LIRCallInstanceMethod instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var receiverType = instruction.ReceiverClrType;

        var allMethods = receiverType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var methods = allMethods
            .Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var argCount = instruction.Arguments.Count;

        var chosen = methods.FirstOrDefault(mi =>
        {
            var ps = mi.GetParameters();
            return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
        });

        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == argCount && ps.All(p => p.ParameterType == typeof(object));
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching instance method found: {receiverType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        EmitLoadTemp(instruction.Receiver, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            EmitObjectArrayFromTemps(instruction.Arguments, ilEncoder, allocation, methodDescriptor);
        }
        else
        {
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(receiverType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodRef);
    }

    private void EmitObjectArrayFromTemps(
        IReadOnlyList<TempVariable> args,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        ilEncoder.LoadConstantI4(args.Count);
        ilEncoder.OpCode(ILOpCode.Newarr);
        ilEncoder.Token(_bclReferences.ObjectType);

        for (int i = 0; i < args.Count; i++)
        {
            ilEncoder.OpCode(ILOpCode.Dup);
            ilEncoder.LoadConstantI4(i);
            EmitLoadTempAsObject(args[i], ilEncoder, allocation, methodDescriptor);
            ilEncoder.OpCode(ILOpCode.Stelem_ref);
        }
    }

    #endregion
}
