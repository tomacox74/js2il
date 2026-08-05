using Jroc.IR;
using Jroc.Runtime.Node.Contracts;
using Jroc.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    #region Instance Calls

    private void EmitNodeModuleContractMemberCall(
        LIRCallNodeModuleContractMember instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var chosen = ResolveTypedInstanceMethodOverload(
            instruction.ContractType,
            instruction.ClrMethodName,
            instruction.Arguments.Count);
        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching Node contract member found: {instruction.ContractType.FullName}.{instruction.ClrMethodName}");
        }

        if (!instruction.RequiresOverrideGuard)
        {
            EmitDirectNodeModuleContractMemberCall(
                instruction,
                chosen,
                ilEncoder,
                allocation,
                methodDescriptor);
            return;
        }

        var fallbackLabel = ilEncoder.DefineLabel();
        var doneLabel = ilEncoder.DefineLabel();
        var overrideGuardType = instruction.OverrideGuardType ?? typeof(JavaScriptRuntime.ObjectRuntime);
        var overrideGuardMethodName = instruction.OverrideGuardMethodName
            ?? nameof(JavaScriptRuntime.ObjectRuntime.HasOwnPropertyOverride);

        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.JavaScriptMemberName);
        var hasOverride = _memberRefRegistry.GetOrAddMethod(
            overrideGuardType,
            overrideGuardMethodName,
            new[] { typeof(object), typeof(string) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(hasOverride);
        ilEncoder.Branch(ILOpCode.Brtrue, fallbackLabel);

        EmitDirectNodeModuleContractMemberCall(
            instruction,
            chosen,
            ilEncoder,
            allocation,
            methodDescriptor);
        ilEncoder.Branch(ILOpCode.Br, doneLabel);

        ilEncoder.MarkLabel(fallbackLabel);
        EmitLoadTempAsObject(instruction.Receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.Ldstr(_metadataBuilder, instruction.JavaScriptMemberName);
        if (instruction.IsPropertyGet)
        {
            var getProperty = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.GetProperty),
                new[] { typeof(object), typeof(string) });
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(getProperty);
        }
        else
        {
            EmitObjectArrayFromTemps(
                instruction.Arguments,
                ilEncoder,
                allocation,
                methodDescriptor);
            var callOwnPropertyMember = _memberRefRegistry.GetOrAddMethod(
                typeof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.CallOwnPropertyMember),
                new[] { typeof(object), typeof(string), typeof(object[]) });
            ilEncoder.OpCode(ILOpCode.Call);
            ilEncoder.Token(callOwnPropertyMember);
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

    private void EmitDirectNodeModuleContractMemberCall(
        LIRCallNodeModuleContractMember instruction,
        System.Reflection.MethodInfo method,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        EmitLoadInstanceMethodReceiver(
            instruction.Receiver,
            instruction.ContractType,
            ilEncoder,
            allocation,
            methodDescriptor);
        var parameters = method.GetParameters();
        EmitInstanceMethodArguments(
            instruction.Arguments,
            parameters,
            ilEncoder,
            allocation,
            methodDescriptor);
        var methodReference = _memberRefRegistry.GetOrAddMethod(
            instruction.ContractType,
            method.Name,
            parameters.Select(static parameter => parameter.ParameterType).ToArray());
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodReference);
        EmitStoreNodeContractResult(method.ReturnType, instruction.Result, ilEncoder, allocation);
    }

    private void EmitStoreNodeContractResult(
        Type returnType,
        TempVariable result,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation)
    {
        if (IsMaterialized(result, allocation))
        {
            if (returnType == typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Ldnull);
            }
            else if (returnType.IsValueType)
            {
                ilEncoder.OpCode(ILOpCode.Box);
                ilEncoder.Token(_memberRefRegistry.GetOrAddTypeHandle(returnType));
            }

            EmitStoreTemp(result, ilEncoder, allocation);
        }
        else if (returnType != typeof(void))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
    }

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

        EmitLoadInstanceMethodReceiver(instruction.Receiver, receiverType, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        EmitInstanceMethodArguments(
            instruction.Arguments,
            parameters,
            ilEncoder,
            allocation,
            methodDescriptor);

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

        EmitLoadInstanceMethodReceiver(instruction.Receiver, receiverType, ilEncoder, allocation, methodDescriptor);

        var parameters = chosen.GetParameters();
        EmitInstanceMethodArguments(
            instruction.Arguments,
            parameters,
            ilEncoder,
            allocation,
            methodDescriptor);

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
        var isNodeModuleContract = receiverType.GetCustomAttributes(
                typeof(NodeModuleInterfaceAttribute),
                inherit: false)
            .Length == 1;
        var allMethods = receiverType
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .ToList();

        var namedMethods = allMethods
            .Where(mi => string.Equals(GetJavaScriptMethodName(mi), methodName, StringComparison.Ordinal))
            .ToList();

        // Prefer exact JS casing, but keep a case-insensitive fallback for CLR surfaces
        // that only expose PascalCase method names.
        if (namedMethods.Count == 0)
        {
            namedMethods = allMethods
                .Where(mi => string.Equals(GetJavaScriptMethodName(mi), methodName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var methods = namedMethods.Where(mi => mi.DeclaringType == receiverType).ToList();
        if (methods.Count == 0)
        {
            methods = namedMethods;
        }

        return methods
            .Select(mi => new { Method = mi, Parameters = mi.GetParameters() })
            .Where(x => isNodeModuleContract
                || (x.Parameters.Length == 1 && x.Parameters[0].ParameterType == typeof(object[]))
                || x.Parameters.All(parameter => parameter.ParameterType == typeof(object)))
            .Select(x => new
            {
                x.Method,
                x.Parameters,
                IsVariadic = x.Parameters.Length > 0
                    && (x.Parameters[^1].GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false).Length > 0
                        || (x.Parameters.Length == 1 && x.Parameters[0].ParameterType == typeof(object[])))
            })
            .Where(x => x.IsVariadic
                ? argCount >= x.Parameters.Length - 1
                : x.Parameters.Length == argCount)
            .OrderBy(x => x.IsVariadic ? 1 : 0)
            .ThenByDescending(x => x.IsVariadic ? x.Parameters.Length - 1 : x.Parameters.Length)
            .ThenBy(x => x.Method.ToString(), StringComparer.Ordinal)
            .Select(x => x.Method)
            .FirstOrDefault();
    }

    private static string GetJavaScriptMethodName(System.Reflection.MethodInfo method)
    {
        var attribute = method.GetCustomAttributes(typeof(NodeModuleMemberAttribute), inherit: false)
            .OfType<NodeModuleMemberAttribute>()
            .SingleOrDefault();
        return attribute?.MemberName ?? method.Name;
    }

    private void EmitInstanceMethodArguments(
        IReadOnlyList<TempVariable> arguments,
        IReadOnlyList<System.Reflection.ParameterInfo> parameters,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var hasParamsArray = parameters.Count > 0
            && (parameters[^1].GetCustomAttributes(typeof(ParamArrayAttribute), inherit: false).Length > 0
                || (parameters.Count == 1 && parameters[0].ParameterType == typeof(object[])));
        var fixedParameterCount = hasParamsArray ? parameters.Count - 1 : parameters.Count;

        for (var i = 0; i < fixedParameterCount; i++)
        {
            EmitLoadTempAsParameterType(
                arguments[i],
                parameters[i].ParameterType,
                ilEncoder,
                allocation,
                methodDescriptor);
        }

        if (hasParamsArray)
        {
            EmitObjectArrayFromTemps(
                arguments.Skip(fixedParameterCount).ToArray(),
                ilEncoder,
                allocation,
                methodDescriptor);
        }
    }

    private void EmitLoadInstanceMethodReceiver(
        TempVariable receiver,
        Type receiverType,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var receiverStorage = GetTempStorage(receiver);
        if (receiverStorage.Kind == ValueStorageKind.Reference
            && receiverStorage.ClrType == receiverType)
        {
            EmitLoadTemp(receiver, ilEncoder, allocation, methodDescriptor);
            return;
        }

        EmitLoadTempAsObject(receiver, ilEncoder, allocation, methodDescriptor);
        ilEncoder.OpCode(ILOpCode.Castclass);
        ilEncoder.Token(_typeReferenceRegistry.GetOrAdd(receiverType));
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
