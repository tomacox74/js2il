using Js2IL.IR;
using Js2IL.Services.ILGenerators;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Js2IL.IL;

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
        var methodRef = _memberRefRegistry.GetOrAddMethod(typeof(System.Math), nameof(System.Math.Pow));
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

    private void EmitOperatorsDynamicBinary(DynamicBinaryOperatorKind operatorKind, InstructionEncoder ilEncoder)
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
            new[] { typeof(object), typeof(object) });
        ilEncoder.OpCode(ILOpCode.Call);
        ilEncoder.Token(methodRef);
    }

    #endregion
}
