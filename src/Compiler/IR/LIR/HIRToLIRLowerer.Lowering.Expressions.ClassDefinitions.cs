using Js2IL.HIR;
using Js2IL.Services;

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

    private bool TryLowerClassHeritageValidationExpression(HIRClassHeritageValidationExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(expression.Heritage, out var heritageTemp))
        {
            return false;
        }

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.ValidateClassHeritage),
            Arguments: new List<TempVariable> { EnsureObject(heritageTemp) },
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

    private bool TryLowerDefineClassAccessorMethodPropertyExpression(HIRDefineClassAccessorMethodPropertyExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(expression.Target, out var targetTemp)
            || !TryLowerExpression(expression.Owner, out var ownerTemp)
            || !TryLowerExpression(expression.Key, out var keyTemp))
        {
            return false;
        }

        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClassConstructor(expression.ClassScope, scopesTemp, allowEmptyOnUnmappedGlobal: true))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        var clrMethodNameTemp = CreateStringConstant(expression.ClrMethodName);
        var lengthTemp = CreateNumberConstant(expression.Length);
        var functionNameTemp = CreateStringConstant(expression.FunctionName);
        var isStaticTemp = CreateBooleanConstant(expression.IsStatic);
        var isPrivateTemp = CreateBooleanConstant(expression.IsPrivate);
        var isSetterTemp = CreateBooleanConstant(expression.IsSetter);
        var isGeneratorTemp = CreateBooleanConstant(expression.IsGenerator);
        var isAsyncTemp = CreateBooleanConstant(expression.IsAsync);

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
            MethodName: nameof(JavaScriptRuntime.ObjectRuntime.DefineClassMethodAccessorProperty),
            Arguments: new[]
            {
                EnsureObject(targetTemp),
                EnsureObject(keyTemp),
                EnsureObject(ownerTemp),
                EnsureObject(clrMethodNameTemp),
                EnsureObject(lengthTemp),
                EnsureObject(functionNameTemp),
                EnsureObject(isStaticTemp),
                EnsureObject(isPrivateTemp),
                EnsureObject(isSetterTemp),
                EnsureObject(isGeneratorTemp),
                EnsureObject(isAsyncTemp),
                EnsureObject(scopesTemp)
            },
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerDefineClassMethodDataPropertiesExpression(HIRDefineClassMethodDataPropertiesExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (expression.MethodDefinitions.Count == 0)
        {
            return false;
        }

        if (!TryLowerExpression(expression.Owner, out var ownerTemp))
        {
            return false;
        }

        if (expression.Owner is HIRUserClassTypeExpression ownerClassType)
        {
            _classMethodOwnerTempsByRegistryName[ownerClassType.RegistryClassName] = ownerTemp;
        }

        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClassConstructor(expression.ClassScope, scopesTemp, allowEmptyOnUnmappedGlobal: true))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        foreach (var methodDefinition in expression.MethodDefinitions)
        {
            var keyTemp = CreateStringConstant(methodDefinition.PropertyKey);
            var clrMethodNameTemp = CreateStringConstant(methodDefinition.ClrMethodName);
            var lengthTemp = CreateNumberConstant(methodDefinition.Length);
            var functionNameTemp = CreateStringConstant(methodDefinition.FunctionName);
            var isStaticTemp = CreateBooleanConstant(methodDefinition.IsStatic);
            var isPrivateTemp = CreateBooleanConstant(methodDefinition.IsPrivate);
            var isGeneratorTemp = CreateBooleanConstant(methodDefinition.IsGenerator);
            var isAsyncTemp = CreateBooleanConstant(methodDefinition.IsAsync);

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                IntrinsicName: nameof(JavaScriptRuntime.ObjectRuntime),
                MethodName: nameof(JavaScriptRuntime.ObjectRuntime.RegisterLazyClassMethodDataProperty),
                Arguments: new[]
                {
                    EnsureObject(ownerTemp),
                    EnsureObject(keyTemp),
                    EnsureObject(clrMethodNameTemp),
                    EnsureObject(lengthTemp),
                    EnsureObject(functionNameTemp),
                    EnsureObject(isStaticTemp),
                    EnsureObject(isPrivateTemp),
                    EnsureObject(isGeneratorTemp),
                    EnsureObject(isAsyncTemp),
                    EnsureObject(scopesTemp)
                },
                Result: resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        return true;
    }

    private TempVariable CreateStringConstant(string value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return temp;
    }

    private TempVariable CreateNumberConstant(double value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        return temp;
    }

    private TempVariable CreateBooleanConstant(bool value)
    {
        var temp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(value, temp));
        DefineTempStorage(temp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return temp;
    }
}
