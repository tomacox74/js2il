using Jroc.HIR;
using Jroc.Services;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryEmitCompileTimeConstant(BindingInfo binding, out TempVariable result)
    {
        result = default;
        if (!CanPropagateCompileTimeConstant(binding))
        {
            return false;
        }

        result = CreateTempVariable();
        EmitCompileTimeConstant(
            binding.CompileTimeConstantType,
            binding.CompileTimeConstantValue,
            result);
        return true;
    }

    private bool CanPropagateCompileTimeConstant(BindingInfo binding)
    {
        if (!binding.IsCompileTimeConstant)
        {
            return false;
        }

        // Reads in the declaring callable remain flow-sensitive so a pre-declaration read
        // still takes the existing TDZ path.
        if (ReferenceEquals(_scope, binding.DeclaringScope)
            || _scope == null
            || !IsDescendantOf(_scope, binding.DeclaringScope))
        {
            return _variableMap.ContainsKey(binding);
        }

        return true;
    }

    private static bool IsDescendantOf(Scope scope, Scope ancestor)
    {
        for (var current = scope.Parent; current != null; current = current.Parent)
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryFoldCompileTimeConstantExpression(
        HIRBinaryExpression expression,
        TempVariable result)
    {
        if (_activeWithObjects.Count > 0 || _scope?.MayUseBoundWithObject == true)
        {
            return false;
        }

        if (!TryEvaluateCompileTimeConstant(
                expression,
                out var type,
                out var value,
                out var usesBinding)
            || !usesBinding)
        {
            return false;
        }

        EmitCompileTimeConstant(type, value, result);
        return true;
    }

    private bool TryEvaluateCompileTimeConstant(
        HIRExpression expression,
        out JavascriptType type,
        out object? value,
        out bool usesBinding)
    {
        switch (expression)
        {
            case HIRLiteralExpression literal
                when literal.Kind is JavascriptType.Number
                    or JavascriptType.String
                    or JavascriptType.Boolean
                    or JavascriptType.Null:
                type = literal.Kind;
                value = literal.Value;
                usesBinding = false;
                return true;

            case HIRVariableExpression variable
                when CanPropagateCompileTimeConstant(variable.Name.BindingInfo):
                type = variable.Name.BindingInfo.CompileTimeConstantType;
                value = variable.Name.BindingInfo.CompileTimeConstantValue;
                usesBinding = true;
                return true;

            case HIRBinaryExpression binary:
                if (!TryEvaluateCompileTimeConstant(
                        binary.Left,
                        out var leftType,
                        out var leftValue,
                        out var leftUsesBinding)
                    || !TryEvaluateCompileTimeConstant(
                        binary.Right,
                        out var rightType,
                        out var rightValue,
                        out var rightUsesBinding)
                    || !TryFoldBinaryConstant(
                        binary.Operator,
                        leftType,
                        leftValue,
                        rightType,
                        rightValue,
                        out type,
                        out value))
                {
                    usesBinding = false;
                    type = JavascriptType.Unknown;
                    value = null;
                    return false;
                }

                usesBinding = leftUsesBinding || rightUsesBinding;
                return true;

            default:
                type = JavascriptType.Unknown;
                value = null;
                usesBinding = false;
                return false;
        }
    }

    private static bool TryFoldBinaryConstant(
        Acornima.Operator op,
        JavascriptType leftType,
        object? leftValue,
        JavascriptType rightType,
        object? rightValue,
        out JavascriptType resultType,
        out object? resultValue)
    {
        if (leftType == JavascriptType.Number
            && rightType == JavascriptType.Number
            && leftValue is double left
            && rightValue is double right)
        {
            resultType = JavascriptType.Number;
            switch (op)
            {
                case Acornima.Operator.Addition:
                    resultValue = left + right;
                    return true;
                case Acornima.Operator.Subtraction:
                    resultValue = left - right;
                    return true;
                case Acornima.Operator.Multiplication:
                    resultValue = left * right;
                    return true;
                case Acornima.Operator.Division:
                    resultValue = left / right;
                    return true;
                case Acornima.Operator.Remainder:
                    resultValue = left % right;
                    return true;
            }
        }

        if (op == Acornima.Operator.Addition
            && leftType == JavascriptType.String
            && rightType == JavascriptType.String
            && leftValue is string leftString
            && rightValue is string rightString)
        {
            resultType = JavascriptType.String;
            resultValue = leftString + rightString;
            return true;
        }

        resultType = JavascriptType.Unknown;
        resultValue = null;
        return false;
    }

    private void EmitCompileTimeConstant(
        JavascriptType type,
        object? value,
        TempVariable result)
    {
        switch (type)
        {
            case JavascriptType.Number:
                _methodBodyIR.Instructions.Add(new LIRConstNumber((double)value!, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                break;
            case JavascriptType.String:
                _methodBodyIR.Instructions.Add(new LIRConstString((string)value!, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                break;
            case JavascriptType.Boolean:
                _methodBodyIR.Instructions.Add(new LIRConstBoolean((bool)value!, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                break;
            case JavascriptType.Null:
                _methodBodyIR.Instructions.Add(new LIRConstNull(result));
                DefineTempStorage(
                    result,
                    new ValueStorage(ValueStorageKind.UnboxedValue, typeof(JavaScriptRuntime.JsNull)));
                break;
            default:
                throw new InvalidOperationException(
                    $"Unsupported compile-time constant type '{type}'.");
        }
    }
}
