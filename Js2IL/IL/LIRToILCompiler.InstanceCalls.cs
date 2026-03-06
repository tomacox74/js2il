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
        var receiverType = instruction.ReceiverClrType;
        var argCount = instruction.Arguments.Count;
        var chosen = ResolveTypedInstanceMethodOverload(receiverType, instruction.MethodName, argCount);

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
        var argCount = instruction.Arguments.Count;
        var chosen = ResolveTypedInstanceMethodOverload(receiverType, instruction.MethodName, argCount);

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

    internal static System.Reflection.MethodInfo? ResolveTypedInstanceMethodOverload(
        Type receiverType,
        string methodName,
        int argCount)
    {
        var allMethods = receiverType
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .ToList();

        var namedMethods = allMethods
            .Where(mi => string.Equals(mi.Name, methodName, StringComparison.Ordinal))
            .ToList();

        // Prefer exact JS casing, but keep a case-insensitive fallback for CLR surfaces
        // that only expose PascalCase method names.
        if (namedMethods.Count == 0)
        {
            namedMethods = allMethods
                .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var methods = namedMethods.Where(mi => mi.DeclaringType == receiverType).ToList();
        if (methods.Count == 0)
        {
            methods = namedMethods;
        }

        return methods
            .Select(mi => new { Method = mi, Parameters = mi.GetParameters() })
            .Where(static x =>
                (x.Parameters.Length == 1 && x.Parameters[0].ParameterType == typeof(object[]))
                || x.Parameters.All(p => p.ParameterType == typeof(object)))
            .Select(x => new
            {
                x.Method,
                x.Parameters,
                IsVariadicFallback = x.Parameters.Length == 1 && x.Parameters[0].ParameterType == typeof(object[])
            })
            .Where(x => x.IsVariadicFallback || x.Parameters.Length == argCount)
            .OrderBy(x => x.IsVariadicFallback ? 1 : 0)
            .ThenBy(x => x.Parameters.Length)
            .ThenBy(x => x.Method.ToString(), StringComparer.Ordinal)
            .Select(x => x.Method)
            .FirstOrDefault();
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
