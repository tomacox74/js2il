using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

internal sealed partial class LIRToILCompiler
{
    #region Global Intrinsics

    /// <summary>
    /// Loads a value onto the the stack for a given intrinsic global variable.
    /// </summary>
    public void EmitLoadIntrinsicGlobalVariable(string variableName, InstructionEncoder ilEncoder)
    {
        var gvType = typeof(JavaScriptRuntime.GlobalThis);
        var gvProp = gvType.GetProperty(variableName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
        var getterDecl = gvProp?.GetMethod?.DeclaringType!;
        var getterMref = _memberRefRegistry.GetOrAddMethod(getterDecl!, gvProp!.GetMethod!.Name);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(getterMref);
    }

    public void EmitInvokeIntrinsicMethod(Type declaringType, string methodName, InstructionEncoder ilEncoder)
    {
        var methodMref = _memberRefRegistry.GetOrAddMethod(declaringType, methodName);
        ilEncoder.OpCode(ILOpCode.Callvirt);
        ilEncoder.Token(methodMref);
    }

    private void EmitIntrinsicGlobalFunctionCall(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        EmitIntrinsicGlobalFunctionCallCore(instruction, ilEncoder, allocation, methodDescriptor);

        // Store or pop result
        if (IsMaterialized(instruction.Result, allocation))
        {
            EmitStoreTemp(instruction.Result, ilEncoder, allocation);
        }
        else
        {
            // If the method returns void, don't pop
            var methodInfo = ResolveGlobalThisMethod(instruction.FunctionName);
            if (methodInfo.ReturnType != typeof(void))
            {
                ilEncoder.OpCode(ILOpCode.Pop);
            }
        }
    }

    private void EmitIntrinsicGlobalFunctionCallInline(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        // Leaves the result on the stack.
        EmitIntrinsicGlobalFunctionCallCore(instruction, ilEncoder, allocation, methodDescriptor);
    }

    private System.Reflection.MethodInfo ResolveGlobalThisMethod(string functionName)
    {
        var gvType = typeof(JavaScriptRuntime.GlobalThis);
        var methodInfo = gvType.GetMethod(
            functionName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
        if (methodInfo == null)
        {
            throw new InvalidOperationException($"Unknown GlobalThis intrinsic function: {functionName}");
        }
        return methodInfo;
    }

    private void EmitIntrinsicGlobalFunctionCallCore(
        LIRCallIntrinsicGlobalFunction instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        var methodInfo = ResolveGlobalThisMethod(instruction.FunctionName);
        var parameters = methodInfo.GetParameters();

        var hasParamsArray = parameters.Length > 0
            && Attribute.IsDefined(parameters[^1], typeof(ParamArrayAttribute));

        var regularParamCount = hasParamsArray ? parameters.Length - 1 : parameters.Length;

        // Load regular args (missing args become null/undefined at runtime; we push ldnull)
        for (int i = 0; i < regularParamCount; i++)
        {
            if (i < instruction.Arguments.Count)
            {
                EmitLoadTempAsObject(instruction.Arguments[i], ilEncoder, allocation, methodDescriptor);
            }
            else
            {
                ilEncoder.OpCode(ILOpCode.Ldnull);
            }
        }

        if (hasParamsArray)
        {
            var paramsCount = Math.Max(0, instruction.Arguments.Count - regularParamCount);
            ilEncoder.LoadConstantI4(paramsCount);
            ilEncoder.OpCode(ILOpCode.Newarr);
            ilEncoder.Token(_bclReferences.ObjectType);

            for (int i = 0; i < paramsCount; i++)
            {
                ilEncoder.OpCode(ILOpCode.Dup);
                ilEncoder.LoadConstantI4(i);
                EmitLoadTempAsObject(instruction.Arguments[regularParamCount + i], ilEncoder, allocation, methodDescriptor);
                ilEncoder.OpCode(ILOpCode.Stelem_ref);
            }
        }
        else
        {
            // Evaluate extra args for side effects (already evaluated in lowering), but do not pass them.
        }

        var paramTypes = parameters.Select(p => p.ParameterType).ToArray();
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.GlobalThis), methodInfo.Name, paramTypes);
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    #endregion
}
