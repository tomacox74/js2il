using Js2IL.HIR;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerDefineClassDataPropertyExpression(HIRDefineClassDataPropertyExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(expression.Key, out var keyTemp)
            || !TryLowerExpression(expression.Target, out var targetTemp)
            || !TryLowerExpression(expression.Value, out var valueTemp))
        {
            return false;
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.DefineClassElementDataProperty),
            Arguments: new List<TempVariable> { EnsureObject(targetTemp), EnsureObject(keyTemp), EnsureObject(valueTemp) },
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerDefineClassAccessorPropertyExpression(HIRDefineClassAccessorPropertyExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(expression.Key, out var keyTemp)
            || !TryLowerExpression(expression.Target, out var targetTemp))
        {
            return false;
        }

        TempVariable getterTemp;
        if (expression.Getter == null)
        {
            getterTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(getterTemp));
            DefineTempStorage(getterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else if (!TryLowerExpression(expression.Getter, out getterTemp))
        {
            return false;
        }

        TempVariable setterTemp;
        if (expression.Setter == null)
        {
            setterTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstUndefined(setterTemp));
            DefineTempStorage(setterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }
        else if (!TryLowerExpression(expression.Setter, out setterTemp))
        {
            return false;
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.DefineClassElementAccessorProperty),
            Arguments: new List<TempVariable>
            {
                EnsureObject(targetTemp),
                EnsureObject(keyTemp),
                EnsureObject(getterTemp),
                EnsureObject(setterTemp)
            },
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }
}
