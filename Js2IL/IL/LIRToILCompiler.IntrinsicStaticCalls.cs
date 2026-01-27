using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Intrinsic Static Calls

    /// <summary>
    /// Emits a static method call on an intrinsic type (e.g., Array.isArray, Math.abs).
    /// Uses the same method resolution strategy as the legacy pipeline.
    /// </summary>
    private void EmitIntrinsicStaticCall(
        LIRCallIntrinsicStatic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

        // Resolve the static method using the same heuristics as the legacy pipeline:
        // 1. Exact arity match first
        // 2. Fallback to params object[] signature
        var allMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        var methods = allMethods.Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase)).ToList();

        var argCount = instruction.Arguments.Count;
        var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == argCount);
        if (chosen == null)
        {
            // Try params object[] signature
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching static method found: {intrinsicType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            // Build an object[] array with all arguments
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Load each argument directly (boxing handled if needed based on target parameter type)
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        // Emit the static call
        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);

        // Store or pop result
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            // If the method returns void, don't pop
            if (chosen.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    private void EmitIntrinsicStaticVoidCall(
        LIRCallIntrinsicStaticVoid instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

        var allMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        var methods = allMethods.Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase)).ToList();

        var argCount = instruction.Arguments.Count;
        var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == argCount);
        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching static method found: {intrinsicType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);

        // Statement-level call: ensure no value is left on stack.
        if (chosen.ReturnType != typeof(void))
        {
            ilEncoder.OpCode(ILOpCode.Pop);
        }
    }

    /// <summary>
    /// Emits intrinsic static call for inline (unmaterialized) temps - leaves result on stack.
    /// </summary>
    private void EmitIntrinsicStaticCallInline(
        LIRCallIntrinsicStatic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(instruction.IntrinsicName);
        if (intrinsicType == null)
        {
            throw new InvalidOperationException($"Unknown intrinsic type: {instruction.IntrinsicName}");
        }

        // Resolve the static method (same logic as EmitIntrinsicStaticCall)
        var allMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        var methods = allMethods.Where(mi => string.Equals(mi.Name, instruction.MethodName, StringComparison.OrdinalIgnoreCase)).ToList();

        var argCount = instruction.Arguments.Count;
        var chosen = methods.FirstOrDefault(mi => mi.GetParameters().Length == argCount);
        if (chosen == null)
        {
            chosen = methods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType == typeof(object[]);
            });
        }

        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"No matching static method found: {intrinsicType.FullName}.{instruction.MethodName} with {argCount} argument(s)");
        }

        var parameters = chosen.GetParameters();
        var expectsParamsArray = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]);

        if (expectsParamsArray)
        {
            // Build an object[] array with all arguments
            ilEncoder.LoadConstantI4(argCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < argCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Load each argument directly (boxing handled if needed based on target parameter type)
            foreach (var arg in instruction.Arguments)
            {
                EmitLoadTempAsObject(arg, ilEncoder, allocation, methodDescriptor);
            }
        }

        // Emit the static call - result stays on stack
        var paramTypes = chosen.GetParameters().Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(intrinsicType, chosen.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    #endregion
}
