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
    private bool TryLowerPropertyAssignmentExpression(HIRPropertyAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // If this is an assignment to a known instance field on the current user-defined class,
        // lower to direct field store (stfld) instead of dynamic property set (Object.SetItem).
        if (_classRegistry != null
            && assignExpr.Object is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, assignExpr.PropertyName, out _))
        {
            TempVariable fieldValueToStore;

            if (assignExpr.Operator == Acornima.Operator.Assignment)
            {
                if (!TryLowerExpression(assignExpr.Value, out fieldValueToStore))
                {
                    return false;
                }
            }
            else
            {
                // Compound assignment: this.field += expr
                var currentValue = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    currentClass,
                    assignExpr.PropertyName,
                    IsPrivateField: false,
                    currentValue));
                DefineTempStorage(currentValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (!TryLowerExpression(assignExpr.Value, out var rhs))
                {
                    return false;
                }

                if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhs, out fieldValueToStore))
                {
                    return false;
                }
            }

            fieldValueToStore = EnsureObject(fieldValueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                assignExpr.PropertyName,
                IsPrivateField: false,
                fieldValueToStore));

            // Assignment expression result is the value assigned.
            resultTempVar = fieldValueToStore;
            return true;
        }

        if (!TryLowerExpression(assignExpr.Object, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        var keyTemp = EmitConstString(assignExpr.PropertyName);
        var boxedKey = EnsureObject(keyTemp);

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

        var propertyValueStorage = GetTempStorage(valueToStore);
        bool canUseStringKeyDoubleValueSetItem =
            propertyValueStorage.Kind == ValueStorageKind.UnboxedValue &&
            propertyValueStorage.ClrType == typeof(double);

        if (!canUseStringKeyDoubleValueSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }
        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, boxedKey, valueToStore, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
    }

    private bool TryLowerIndexAssignmentExpression(HIRIndexAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // User-defined class instance field access via bracket notation (e.g., this["wordArray"] = ...).
        // If the receiver is `this` and the index is a constant string that matches a known field on the
        // generated CLR type, lower directly to an instance field store (stfld) instead of dynamic SetItem.
        if (_classRegistry != null
            && assignExpr.Object is HIRThisExpression
            && assignExpr.Index is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
        {
            TempVariable fieldValueToStore;

            if (assignExpr.Operator == Acornima.Operator.Assignment)
            {
                if (!TryLowerExpression(assignExpr.Value, out fieldValueToStore))
                {
                    return false;
                }
            }
            else
            {
                // Compound assignment: this["field"] += expr
                var currentValue = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    currentClass,
                    literalFieldName,
                    IsPrivateField: false,
                    currentValue));
                DefineTempStorage(currentValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (!TryLowerExpression(assignExpr.Value, out var rhs))
                {
                    return false;
                }

                if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhs, out fieldValueToStore))
                {
                    return false;
                }
            }

            fieldValueToStore = EnsureObject(fieldValueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                literalFieldName,
                IsPrivateField: false,
                fieldValueToStore));

            // Assignment expression result is the value assigned.
            resultTempVar = fieldValueToStore;
            return true;
        }

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

        var indexStorage = GetTempStorage(indexTemp);
        var valueStorage = GetTempStorage(valueToStore);
        bool canUseNumericSetItem =
            indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double) &&
            valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double);
        bool canUseStringKeyDoubleValueSetItem =
            indexStorage.Kind == ValueStorageKind.Reference && indexStorage.ClrType == typeof(string) &&
            valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double);

        TempVariable indexForSet;
        if (canUseNumericSetItem)
        {
            indexForSet = indexTemp;
        }
        else
        {
            boxedIndex ??= EnsureObject(indexTemp);
            indexForSet = boxedIndex.Value;
        }

        if (!canUseNumericSetItem && !canUseStringKeyDoubleValueSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }
        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, indexForSet, valueToStore, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
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
