using Acornima.Ast;
using Jroc.HIR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using TwoPhase = Jroc.Services.TwoPhaseCompilation;
using Jroc.Utilities;
using Jroc.SymbolTables;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool UsesStrictAssignmentSemantics()
        => _scope == null || Jroc.Utilities.ArgumentsObjectSemantics.IsStrictScope(_scope);

    private bool TryLowerPropertyAssignmentTarget(HIRExpression objectExpr, string propertyName, TempVariable valueToStore, out TempVariable resultTempVar, bool resultUsed = true)
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

            resultTempVar = resultUsed ? fieldValueToStore : default;
            return true;
        }

        if (!TryLowerExpression(objectExpr, out var objTemp))
        {
            return false;
        }

        return TryLowerPropertyAssignmentTarget(objTemp, propertyName, valueToStore, out resultTempVar, resultUsed);
    }

    private bool TryLowerPropertyAssignmentTarget(TempVariable objectTemp, string propertyName, TempVariable valueToStore, out TempVariable resultTempVar, bool resultUsed = true)
    {
        resultTempVar = default;

        objectTemp = EnsureObject(objectTemp);

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
        _methodBodyIR.Instructions.Add(new LIRSetItem(objectTemp, boxedKey, valueToStore, setResult, UsesStrictAssignmentSemantics()));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = resultUsed ? setResult : default;
        return true;
    }

    private bool TryLowerIndexAssignmentTarget(HIRExpression objectExpr, HIRExpression indexExpr, TempVariable valueToStore, out TempVariable resultTempVar, bool resultUsed = true)
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

            resultTempVar = resultUsed ? fieldValueToStore : default;
            return true;
        }

        if (!TryLowerExpression(objectExpr, out var objTemp))
        {
            return false;
        }

        if (!TryLowerExpression(indexExpr, out var indexTemp))
        {
            return false;
        }

        return TryLowerIndexAssignmentTarget(objTemp, indexTemp, valueToStore, out resultTempVar, resultUsed);
    }

    private bool TryLowerIndexAssignmentTarget(TempVariable objectTemp, TempVariable indexTemp, TempVariable valueToStore, out TempVariable resultTempVar, bool resultUsed = true)
    {
        resultTempVar = default;

        objectTemp = EnsureObject(objectTemp);

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
        _methodBodyIR.Instructions.Add(new LIRSetItem(objectTemp, indexForSet, valueToStore, setResult, UsesStrictAssignmentSemantics()));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = resultUsed ? setResult : default;
        return true;
    }

    private bool TryLowerPropertyAssignmentExpression(HIRPropertyAssignmentExpression assignExpr, out TempVariable resultTempVar, bool resultUsed = true)
    {
        resultTempVar = default;

        // Early-bound object-literal member write (phase 4, #1432). All write forms must go
        // through the generated setter so the typed backing field and JsObject storage stay
        // in sync; a generic SetItem would leave the backing field stale.
        if (TryGetInferredObjectLiteralMember(assignExpr.Object, assignExpr.PropertyName, out var inferredShape, out var inferredMember))
        {
            return TryLowerInferredMemberAssignment(assignExpr, inferredShape, inferredMember, out resultTempVar);
        }

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

            var evalRightLabel = CreateLabel();
            var endLabel = CreateLabel();

            var currentBoxed = EnsureObject(current);
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(currentBoxed, evalRightLabel));

            var isJsNullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), currentBoxed, isJsNullTemp));
            DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

            if (resultUsed)
            {
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(currentBoxed, resultTempVar));
            }
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
            if (resultUsed)
            {
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(setResult, resultTempVar));
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            if (resultUsed)
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            return true;
        }

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (_classRegistry != null
                && assignExpr.Object is HIRThisExpression
                && TryGetEnclosingClassRegistryName(out var currentClass)
                && currentClass != null
                && _classRegistry.TryGetField(currentClass, assignExpr.PropertyName, out _))
            {
                if (!TryLowerExpression(assignExpr.Value, out valueToStore))
                {
                    return false;
                }

                return TryLowerPropertyAssignmentTarget(assignExpr.Object, assignExpr.PropertyName, valueToStore, out resultTempVar, resultUsed);
            }

            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }

            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }

            return TryLowerPropertyAssignmentTarget(objTemp, assignExpr.PropertyName, valueToStore, out resultTempVar, resultUsed);
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

        return TryLowerPropertyAssignmentTarget(assignExpr.Object, assignExpr.PropertyName, valueToStore, out resultTempVar, resultUsed);
    }

    /// <summary>
    /// Lowers simple, compound, and nullish-coalescing assignments to a declared member of an
    /// eligible object-literal binding through the generated typed accessors (phase 4, #1432).
    /// </summary>
    private bool TryLowerInferredMemberAssignment(
        HIRPropertyAssignmentExpression assignExpr,
        ObjectLiteralShapeInfo shape,
        ObjectLiteralMemberInfo member,
        out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(assignExpr.Object, out var receiverTemp))
        {
            return false;
        }
        var receiver = EnsureObject(receiverTemp);

        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (!TryLowerExpression(assignExpr.Value, out var valueTemp))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRSetInferredMember(shape, member.Name, receiver, valueTemp));
            resultTempVar = valueTemp;
            return true;
        }

        if (assignExpr.Operator == Acornima.Operator.NullishCoalescingAssignment)
        {
            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetInferredMember(shape, member.Name, receiver, current));
            DefineTempStorage(current, GetInferredMemberStorage(member));

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

            var rhsBoxed = EnsureObject(rhsValue);
            _methodBodyIR.Instructions.Add(new LIRSetInferredMember(shape, member.Name, receiver, rhsBoxed));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rhsBoxed, resultTempVar));

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Compound assignment: read-modify-write through the early-bound accessors.
        var currentValue = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetInferredMember(shape, member.Name, receiver, currentValue));
        DefineTempStorage(currentValue, GetInferredMemberStorage(member));

        if (!TryLowerExpression(assignExpr.Value, out var rhs))
        {
            return false;
        }

        if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhs, out var computedValue))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRSetInferredMember(shape, member.Name, receiver, computedValue));
        resultTempVar = computedValue;
        return true;
    }

    private bool TryLowerIndexAssignmentExpression(HIRIndexAssignmentExpression assignExpr, out TempVariable resultTempVar, bool resultUsed = true)
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

            var evalRightLabel = CreateLabel();
            var endLabel = CreateLabel();

            var currentBoxed = EnsureObject(current);
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(currentBoxed, evalRightLabel));

            var isJsNullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), currentBoxed, isJsNullTemp));
            DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

            if (resultUsed)
            {
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(currentBoxed, resultTempVar));
            }
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
            if (resultUsed)
            {
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(setResult, resultTempVar));
            }

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            if (resultUsed)
            {
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            return true;
        }

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (_classRegistry != null
                && assignExpr.Object is HIRThisExpression
                && assignExpr.Index is HIRLiteralExpression literalIndex
                && literalIndex.Kind == JavascriptType.String
                && literalIndex.Value is string literalFieldName
                && TryGetEnclosingClassRegistryName(out var currentClass)
                && currentClass != null
                && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
            {
                if (!TryLowerExpression(assignExpr.Value, out valueToStore))
                {
                    return false;
                }

                return TryLowerIndexAssignmentTarget(assignExpr.Object, assignExpr.Index, valueToStore, out resultTempVar, resultUsed);
            }

            if (!TryLowerExpression(assignExpr.Object, out var objTemp))
            {
                return false;
            }

            if (!TryLowerExpression(assignExpr.Index, out var indexTemp))
            {
                return false;
            }

            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }

            return TryLowerIndexAssignmentTarget(objTemp, indexTemp, valueToStore, out resultTempVar, resultUsed);
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

        return TryLowerIndexAssignmentTarget(assignExpr.Object, assignExpr.Index, valueToStore, out resultTempVar, resultUsed);
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
