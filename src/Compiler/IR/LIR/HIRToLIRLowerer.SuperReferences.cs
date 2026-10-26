using Jroc.HIR;

namespace Jroc.IR;

public sealed partial class HIRToLIRLowerer
{
    private readonly record struct SuperPropertyReference(
        TempVariable Prototype,
        TempVariable Receiver,
        TempVariable Key);

    private bool TryLowerSuperPropertyReference(
        HIRExpression? computedKey,
        string? propertyName,
        bool read,
        out SuperPropertyReference reference,
        string? staticOwner = null)
    {
        reference = default;
        var receiver = CreateTempVariable();
        if (staticOwner != null)
        {
            if (!TryLowerExpression(new HIRUserClassTypeExpression(staticOwner), out receiver))
                return false;
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
            nameof(JavaScriptRuntime.RuntimeServices.GetCurrentLexicalSuperPropertyReceiver),
            [],
            receiver));
            DefineTempStorage(receiver, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        TempVariable key;
        if (computedKey != null)
        {
            if (!TryLowerExpression(computedKey, out key))
            {
                return false;
            }
        }
        else
        {
            key = EmitConstString(propertyName
                ?? throw new InvalidOperationException("A super reference requires a property key."));
        }

        var prototype = CreateTempVariable();
        if (staticOwner != null)
        {
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime), nameof(JavaScriptRuntime.ObjectRuntime.getPrototypeOf),
                [EnsureObject(receiver)], prototype));
        }
        else if (_callableKind == Services.ScopesAbi.CallableKind.Constructor
            && TryGetEnclosingClassRegistryName(out var className)
            && className != null)
        {
            var owner = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetUserClassType(className, owner));
            DefineTempStorage(owner, new ValueStorage(ValueStorageKind.Reference, typeof(Type)));
            _methodBodyIR.Instructions.Add(new LIRCallRuntimeServicesStatic(
                nameof(JavaScriptRuntime.RuntimeServices.GetConstructorSuperPropertyBase), [owner], prototype, [typeof(Type)]));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                nameof(JavaScriptRuntime.ObjectRuntime),
                nameof(JavaScriptRuntime.ObjectRuntime.GetSuperPropertyBase),
                [],
                prototype));
        }
        DefineTempStorage(prototype, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        if (read)
        {
            prototype = RequireObjectCoercible(prototype);
            key = PrepareAssignmentPropertyKey(ref prototype, key);
        }

        reference = new SuperPropertyReference(prototype, receiver, key);
        return true;
    }

    private TempVariable EmitGetSuperProperty(SuperPropertyReference reference)
    {
        var result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.GetSuperProperty),
            [reference.Prototype, reference.Receiver, EnsureObject(reference.Key)],
            result));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return result;
    }

    private TempVariable EmitSetSuperProperty(SuperPropertyReference reference, TempVariable value)
    {
        var strict = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstBoolean(UsesStrictAssignmentSemantics(), strict));
        DefineTempStorage(strict, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        var result = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
            nameof(JavaScriptRuntime.ObjectRuntime),
            nameof(JavaScriptRuntime.ObjectRuntime.SetSuperProperty),
            [reference.Prototype, reference.Receiver, EnsureObject(reference.Key), EnsureObject(value), strict],
            result));
        DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return result;
    }

    private bool TryLowerSuperAssignment(
        HIRExpression? key,
        string? propertyName,
        Acornima.Operator op,
        HIRExpression value,
        out TempVariable result)
    {
        result = default;
        if (!TryLowerSuperPropertyReference(key, propertyName, read: op != Acornima.Operator.Assignment, out var reference))
        {
            return false;
        }

        var current = op == Acornima.Operator.Assignment ? default : EmitGetSuperProperty(reference);
        if (op is Acornima.Operator.NullishCoalescingAssignment
            or Acornima.Operator.LogicalAndAssignment
            or Acornima.Operator.LogicalOrAssignment)
        {
            result = CreateTempVariable();
            var assignLabel = CreateLabel();
            var endLabel = CreateLabel();
            EmitLogicalAssignmentEvaluationBranch(op, current, assignLabel);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(current, result));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
            _methodBodyIR.Instructions.Add(new LIRLabel(assignLabel));
            ClearNumericRefinementsAtLabel();
            if (!TryLowerExpression(value, out var right))
            {
                return false;
            }
            var stored = EmitSetSuperProperty(reference, right);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(stored, result));
            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            ClearNumericRefinementsAtLabel();
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        if (!TryLowerExpression(value, out var assigned))
        {
            return false;
        }
        if (op != Acornima.Operator.Assignment
            && !TryLowerCompoundOperation(op, current, assigned, out assigned))
        {
            return false;
        }
        result = EmitSetSuperProperty(reference, assigned);
        return true;
    }

    private bool TryLowerSuperUpdate(
        HIRExpression? key,
        string? propertyName,
        bool increment,
        bool prefix,
        bool needsPostfixValue,
        out TempVariable result)
    {
        result = default;
        if (!TryLowerSuperPropertyReference(key, propertyName, read: true, out var reference))
        {
            return false;
        }
        var original = EnsureNumeric(EmitGetSuperProperty(reference));
        var updated = EmitNumericUpdate(original, increment);
        EmitSetSuperProperty(reference, updated);
        result = prefix || !needsPostfixValue ? updated : original;
        return true;
    }
}
