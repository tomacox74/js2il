using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool UsesStrictAssignmentSemantics()
        => _scope == null || Js2IL.Utilities.ArgumentsObjectSemantics.IsStrictScope(_scope);

    private bool TryLowerPropertyAssignmentTarget(HIRExpression objectExpr, string propertyName, TempVariable valueToStore, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_classRegistry != null
            && objectExpr is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, propertyName, out _))
        {
            var fieldValueToStore = EnsureObject(valueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                propertyName,
                IsPrivateField: false,
                fieldValueToStore));

            resultTempVar = fieldValueToStore;
            return true;
        }

        if (!TryLowerExpression(objectExpr, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        var keyTemp = EmitConstString(propertyName);
        var boxedKey = EnsureObject(keyTemp);

        var propertyValueStorage = GetTempStorage(valueToStore);
        bool canUseStringKeyDoubleValueSetItem =
            propertyValueStorage.Kind == ValueStorageKind.UnboxedValue &&
            propertyValueStorage.ClrType == typeof(double);

        if (!canUseStringKeyDoubleValueSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }

        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, boxedKey, valueToStore, setResult, UsesStrictAssignmentSemantics()));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
    }

    private bool TryLowerIndexAssignmentTarget(HIRExpression objectExpr, HIRExpression indexExpr, TempVariable valueToStore, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_classRegistry != null
            && objectExpr is HIRThisExpression
            && indexExpr is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
        {
            var fieldValueToStore = EnsureObject(valueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                literalFieldName,
                IsPrivateField: false,
                fieldValueToStore));

            resultTempVar = fieldValueToStore;
            return true;
        }

        if (!TryLowerExpression(objectExpr, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        if (!TryLowerExpression(indexExpr, out var indexTemp))
        {
            return false;
        }

        var indexStorage = GetTempStorage(indexTemp);
        var valueStorage = GetTempStorage(valueToStore);
        bool canUseNumericSetItem =
            indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double);
        bool canUseStringKeyDoubleValueSetItem =
            indexStorage.Kind == ValueStorageKind.Reference && indexStorage.ClrType == typeof(string) &&
            valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double);

        TempVariable indexForSet = canUseNumericSetItem ? indexTemp : EnsureObject(indexTemp);

        if (!canUseNumericSetItem && !canUseStringKeyDoubleValueSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }

        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, indexForSet, valueToStore, setResult, UsesStrictAssignmentSemantics()));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
    }

    private bool TryLowerPropertyAssignmentExpression(HIRPropertyAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (assignExpr.Operator == Acornima.Operator.NullishCoalescingAssignment)
        {
            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }
            objTemp = EnsureObject(objTemp);

            var keyTemp = EmitConstString(assignExpr.PropertyName);
            var boxedKey = EnsureObject(keyTemp);

            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, boxedKey, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            resultTempVar = CreateTempVariable();
            var evalRightLabel = CreateLabel();
            var endLabel = CreateLabel();

            var currentBoxed = EnsureObject(current);
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(currentBoxed, evalRightLabel));

            var isJsNullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), currentBoxed, isJsNullTemp));
            DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

            _methodBodyIR.Instructions.Add(new LIRCopyTemp(currentBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

            _methodBodyIR.Instructions.Add(new LIRLabel(evalRightLabel));
            ClearNumericRefinementsAtLabel();

            if (!TryLowerExpression(assignExpr.Value, out var rhsValue))
            {
                return false;
            }

            rhsValue = EnsureObject(rhsValue);
            var setResult = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, boxedKey, rhsValue, setResult, UsesStrictAssignmentSemantics()));
            DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(setResult, resultTempVar));

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: obj.prop += expr
            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }
            objTemp = EnsureObject(objTemp);

            var keyTemp = EmitConstString(assignExpr.PropertyName);
            var boxedKey = EnsureObject(keyTemp);

            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, boxedKey, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerExpression(assignExpr.Value, out var rhs))
            {
                return false;
            }

            if (!TryLowerCompoundOperation(assignExpr.Operator, current, rhs, out valueToStore))
            {
                return false;
            }
        }

        return TryLowerPropertyAssignmentTarget(assignExpr.Object, assignExpr.PropertyName, valueToStore, out resultTempVar);
    }

    private bool TryLowerIndexAssignmentExpression(HIRIndexAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (assignExpr.Operator == Acornima.Operator.NullishCoalescingAssignment)
        {
            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }
            objTemp = EnsureObject(objTemp);

            if (!TryLowerExpression(assignExpr.Index, out var indexTemp))
            {
                return false;
            }

            TempVariable? boxedIndex = null;
            var indexStorageForGet = GetTempStorage(indexTemp);
            TempVariable indexForGet;
            if (indexStorageForGet.Kind == ValueStorageKind.UnboxedValue && indexStorageForGet.ClrType == typeof(double))
            {
                indexForGet = indexTemp;
            }
            else
            {
                boxedIndex = EnsureObject(indexTemp);
                indexForGet = boxedIndex.Value;
            }

            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, indexForGet, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            resultTempVar = CreateTempVariable();
            var evalRightLabel = CreateLabel();
            var endLabel = CreateLabel();

            var currentBoxed = EnsureObject(current);
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(currentBoxed, evalRightLabel));

            var isJsNullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), currentBoxed, isJsNullTemp));
            DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

            _methodBodyIR.Instructions.Add(new LIRCopyTemp(currentBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

            _methodBodyIR.Instructions.Add(new LIRLabel(evalRightLabel));
            ClearNumericRefinementsAtLabel();

            if (!TryLowerExpression(assignExpr.Value, out var rhsValue))
            {
                return false;
            }

            rhsValue = EnsureObject(rhsValue);

            var indexStorageForSet = GetTempStorage(indexTemp);
            var indexForSet = indexStorageForSet.Kind == ValueStorageKind.UnboxedValue && indexStorageForSet.ClrType == typeof(double)
                ? indexTemp
                : boxedIndex ?? EnsureObject(indexTemp);

            var setResult = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, indexForSet, rhsValue, setResult, UsesStrictAssignmentSemantics()));
            DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(setResult, resultTempVar));

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: obj[index] += expr
            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }
            objTemp = EnsureObject(objTemp);

            if (!TryLowerExpression(assignExpr.Index, out var indexTemp))
            {
                return false;
            }

            TempVariable? boxedIndex = null;
            var indexStorageForGet = GetTempStorage(indexTemp);
            TempVariable indexForGet;
            if (indexStorageForGet.Kind == ValueStorageKind.UnboxedValue && indexStorageForGet.ClrType == typeof(double))
            {
                indexForGet = indexTemp;
            }
            else
            {
                boxedIndex = EnsureObject(indexTemp);
                indexForGet = boxedIndex.Value;
            }

            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, indexForGet, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerExpression(assignExpr.Value, out var rhs))
            {
                return false;
            }

            if (!TryLowerCompoundOperation(assignExpr.Operator, current, rhs, out valueToStore))
            {
                return false;
            }
        }

        return TryLowerIndexAssignmentTarget(assignExpr.Object, assignExpr.Index, valueToStore, out resultTempVar);
    }

    private bool TryLowerDestructuringAssignmentExpression(HIRDestructuringAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(assignExpr.Value, out var rhsTemp))
        {
            return false;
        }

        rhsTemp = EnsureObject(rhsTemp);

        var sourceNameForError = TryGetSimpleSourceNameForDestructuring(assignExpr.Value);
        if (!TryLowerDestructuringPattern(assignExpr.Pattern, rhsTemp, DestructuringWriteMode.Assignment, sourceNameForError))
        {
            return false;
        }

        // JS destructuring assignment evaluates to the RHS value.
        resultTempVar = rhsTemp;
        return true;
    }

}
