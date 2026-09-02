using Jroc.IR;
using Jroc.Services.ILGenerators;
using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Jroc.IL;

internal sealed partial class LIRToILCompiler
{
    #region Runtime/Operator Helpers

    private void EmitStringConcat(InstructionEncoder ilEncoder)
    {
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(_bclReferences.String_Concat_Ref);
    }

    private void EmitOperatorsAddObjectObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(object), typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsAddDoubleObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(double), typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsAddObjectDouble(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.Add),
            new[] { typeof(object), typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsMultiply(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Multiply));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitMathPow(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(System.Math),
            nameof(System.Math.Pow),
            new[] { typeof(double), typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitNormalizeExponentiationBase(
        InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.NormalizeExponentiationBase),
            new[] { typeof(double), typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyObject(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyDouble(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(double) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIsTruthyBool(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.IsTruthy), new[] { typeof(bool) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsIn(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.In));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsInstanceOf(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.InstanceOf));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.Equal));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.NotEqual));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.StrictEqual));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsStrictNotEqual(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(JavaScriptRuntime.Operators), nameof(JavaScriptRuntime.Operators.StrictNotEqual));
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsUnaryMinus(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.UnaryMinus),
            new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsBitwiseNot(InstructionEncoder ilEncoder)
    {
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.BitwiseNot),
            new[] { typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    /// <summary>
    /// Loads both operands of a dynamic binary operator and calls the matching
    /// <see cref="JavaScriptRuntime.Operators"/> overload. When exactly one operand is an unboxed
    /// double and the runtime exposes a mixed overload for the operator, the double is passed
    /// unboxed instead of allocating a box on every evaluation (e.g. <c>i &lt; limit</c> in loops).
    /// </summary>
    private void EmitOperatorsDynamicBinary(
        LIRBinaryDynamicOperator binaryDynamic,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        static bool IsUnboxedDouble(ValueStorage storage)
            => storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double);

        var leftIsDouble = IsUnboxedDouble(GetTempStorage(binaryDynamic.Left));
        var rightIsDouble = IsUnboxedDouble(GetTempStorage(binaryDynamic.Right));
        var hasMixedOverload = HasMixedDoubleOverload(binaryDynamic.Operator);

        if (hasMixedOverload && leftIsDouble && !rightIsDouble)
        {
            EmitLoadTempAsDouble(binaryDynamic.Left, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsObject(binaryDynamic.Right, ilEncoder, allocation, methodDescriptor);
            EmitOperatorsDynamicBinary(binaryDynamic.Operator, ilEncoder, typeof(double), typeof(object));
            return;
        }

        if (hasMixedOverload && !leftIsDouble && rightIsDouble)
        {
            EmitLoadTempAsObject(binaryDynamic.Left, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsDouble(binaryDynamic.Right, ilEncoder, allocation, methodDescriptor);
            EmitOperatorsDynamicBinary(binaryDynamic.Operator, ilEncoder, typeof(object), typeof(double));
            return;
        }

        EmitLoadTempAsObject(binaryDynamic.Left, ilEncoder, allocation, methodDescriptor);
        EmitLoadTempAsObject(binaryDynamic.Right, ilEncoder, allocation, methodDescriptor);
        EmitOperatorsDynamicBinary(binaryDynamic.Operator, ilEncoder);
    }

    private static bool HasMixedDoubleOverload(DynamicBinaryOperatorKind operatorKind)
        => operatorKind is DynamicBinaryOperatorKind.Subtract
            or DynamicBinaryOperatorKind.LessThan
            or DynamicBinaryOperatorKind.GreaterThan
            or DynamicBinaryOperatorKind.LessThanOrEqual
            or DynamicBinaryOperatorKind.GreaterThanOrEqual;

    private void EmitOperatorsDynamicBinary(
        DynamicBinaryOperatorKind operatorKind,
        InstructionEncoder ilEncoder,
        Type? leftParameterType = null,
        Type? rightParameterType = null)
    {
        var methodName = operatorKind switch
        {
            DynamicBinaryOperatorKind.Subtract => nameof(JavaScriptRuntime.Operators.Subtract),
            DynamicBinaryOperatorKind.Divide => nameof(JavaScriptRuntime.Operators.Divide),
            DynamicBinaryOperatorKind.Remainder => nameof(JavaScriptRuntime.Operators.Remainder),
            DynamicBinaryOperatorKind.Exponentiate => nameof(JavaScriptRuntime.Operators.Exponentiate),
            DynamicBinaryOperatorKind.BitwiseAnd => nameof(JavaScriptRuntime.Operators.BitwiseAnd),
            DynamicBinaryOperatorKind.BitwiseOr => nameof(JavaScriptRuntime.Operators.BitwiseOr),
            DynamicBinaryOperatorKind.BitwiseXor => nameof(JavaScriptRuntime.Operators.BitwiseXor),
            DynamicBinaryOperatorKind.LeftShift => nameof(JavaScriptRuntime.Operators.LeftShift),
            DynamicBinaryOperatorKind.SignedRightShift => nameof(JavaScriptRuntime.Operators.SignedRightShift),
            DynamicBinaryOperatorKind.UnsignedRightShift => nameof(JavaScriptRuntime.Operators.UnsignedRightShift),
            DynamicBinaryOperatorKind.LessThan => nameof(JavaScriptRuntime.Operators.LessThan),
            DynamicBinaryOperatorKind.GreaterThan => nameof(JavaScriptRuntime.Operators.GreaterThan),
            DynamicBinaryOperatorKind.LessThanOrEqual => nameof(JavaScriptRuntime.Operators.LessThanOrEqual),
            DynamicBinaryOperatorKind.GreaterThanOrEqual => nameof(JavaScriptRuntime.Operators.GreaterThanOrEqual),
            _ => throw new NotSupportedException($"Unsupported dynamic binary operator: {operatorKind}")
        };

        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            methodName,
            new[] { leftParameterType ?? typeof(object), rightParameterType ?? typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    private void EmitOperatorsAddAndToNumber(
        TempVariable left,
        TempVariable right,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        static bool IsUnboxedDouble(ValueStorage storage)
            => storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double);

        var leftStorage = GetTempStorage(left);
        var rightStorage = GetTempStorage(right);

        Type[] paramTypes;
        if (IsUnboxedDouble(leftStorage) && !IsUnboxedDouble(rightStorage))
        {
            EmitLoadTempAsDouble(left, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsObject(right, ilEncoder, allocation, methodDescriptor);
            paramTypes = new[] { typeof(double), typeof(object) };
        }
        else if (!IsUnboxedDouble(leftStorage) && IsUnboxedDouble(rightStorage))
        {
            EmitLoadTempAsObject(left, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsDouble(right, ilEncoder, allocation, methodDescriptor);
            paramTypes = new[] { typeof(object), typeof(double) };
        }
        else
        {
            EmitLoadTempAsObject(left, ilEncoder, allocation, methodDescriptor);
            EmitLoadTempAsObject(right, ilEncoder, allocation, methodDescriptor);
            paramTypes = new[] { typeof(object), typeof(object) };
        }

        ilEncoder.OpCode(ILOpCode.Call);
        var methodRef = _memberRefRegistry.GetOrAddMethod(
            typeof(JavaScriptRuntime.Operators),
            nameof(JavaScriptRuntime.Operators.AddAndToNumber),
            paramTypes);
        ilEncoder.Token(methodRef);
    }

    private bool TryEmitOperatorsAddAndToNumber(
        LIRCallRuntimeServicesStatic instruction,
        InstructionEncoder ilEncoder,
        TempLocalAllocation allocation,
        MethodDescriptor methodDescriptor)
    {
        if (!string.Equals(instruction.MethodName, nameof(JavaScriptRuntime.Operators.AddAndToNumber), System.StringComparison.Ordinal)
            || instruction.Arguments.Count != 2)
        {
            return false;
        }

        EmitOperatorsAddAndToNumber(
            instruction.Arguments[0],
            instruction.Arguments[1],
            ilEncoder,
            allocation,
            methodDescriptor);
        return true;
    }

    #endregion
}
