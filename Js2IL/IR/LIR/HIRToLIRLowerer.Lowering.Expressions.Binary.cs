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
    private bool TryLowerBinaryExpression(HIRBinaryExpression binaryExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // IMPORTANT: logical operators must short-circuit; do not lower RHS eagerly.
        if (!TryLowerExpression(binaryExpr.Left, out var leftTempVar))
        {
            return false;
        }

        // Handle logical operators with correct short-circuit evaluation.
        // JavaScript logical operators (&&, ||) return one of the operand VALUES (not a boolean).
        if (binaryExpr.Operator == Acornima.Operator.LogicalAnd)
        {
            int falsyLabel = CreateLabel();
            int endLabel = CreateLabel();

            var leftBoxed = EnsureObject(leftTempVar);

            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(isTruthyTemp, falsyLabel));

            // Left was truthy; evaluate RHS and return it.
            if (!TryLowerExpression(binaryExpr.Right, out var andRightTempVar))
            {
                return false;
            }
            var rightBoxed = EnsureObject(andRightTempVar);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rightBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

            // Left was falsy; return left.
            _methodBodyIR.Instructions.Add(new LIRLabel(falsyLabel));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(leftBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.LogicalOr)
        {
            int truthyLabel = CreateLabel();
            int endLabel = CreateLabel();

            var leftBoxed = EnsureObject(leftTempVar);

            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isTruthyTemp, truthyLabel));

            // Left was falsy; evaluate RHS and return it.
            if (!TryLowerExpression(binaryExpr.Right, out var orRightTempVar))
            {
                return false;
            }
            var rightBoxed = EnsureObject(orRightTempVar);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rightBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

            // Left was truthy; return left.
            _methodBodyIR.Instructions.Add(new LIRLabel(truthyLabel));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(leftBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            return true;
        }

        // Nullish coalescing (??) short-circuits and returns one of the operand VALUES.
        // Semantics: evaluate LHS; if LHS is null or undefined, evaluate/return RHS; else return LHS.
        if (binaryExpr.Operator == Acornima.Operator.NullishCoalescing)
        {
            int evalRightLabel = CreateLabel();
            int endLabel = CreateLabel();

            // Ensure we branch on an object reference.
            var leftBoxed = EnsureObject(leftTempVar);

            // If left is null (undefined), evaluate RHS.
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(leftBoxed, evalRightLabel));

            // If left is boxed JsNull (null), evaluate RHS.
            var isJsNullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), leftBoxed, isJsNullTemp));
            DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isJsNullTemp, evalRightLabel));

            // Left is non-nullish; return it.
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(leftBoxed, resultTempVar));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

            // Left is nullish; evaluate RHS and return it.
            _methodBodyIR.Instructions.Add(new LIRLabel(evalRightLabel));
            if (!TryLowerExpression(binaryExpr.Right, out var coalesceRightTempVar))
            {
                return false;
            }

            var rightBoxed = EnsureObject(coalesceRightTempVar);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rightBoxed, resultTempVar));

            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            return true;
        }

        // Non-logical operators: evaluate RHS eagerly.
        if (!TryLowerExpression(binaryExpr.Right, out var rightTempVar))
        {
            return false;
        }

        var leftStorage = GetTempStorage(leftTempVar);
        var rightStorage = GetTempStorage(rightTempVar);
        var leftType = leftStorage.ClrType;
        var rightType = rightStorage.ClrType;

        // Handle 'instanceof'
        if (binaryExpr.Operator == Acornima.Operator.InstanceOf)
        {
            var leftBoxed = EnsureObject(leftTempVar);
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRInstanceOfOperator(leftBoxed, rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            return true;
        }

        // Handle addition
        if (binaryExpr.Operator == Acornima.Operator.Addition)
        {
            var leftIsUnboxedDouble = leftStorage.Kind == ValueStorageKind.UnboxedValue && leftType == typeof(double);
            var rightIsUnboxedDouble = rightStorage.Kind == ValueStorageKind.UnboxedValue && rightType == typeof(double);

            // Number + Number (ToNumber semantics). Only emit native numeric add when both operands
            // are truly unboxed doubles; otherwise, coerce first.
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                var leftNumber = leftIsUnboxedDouble ? leftTempVar : EnsureNumber(leftTempVar);
                var rightNumber = rightIsUnboxedDouble ? rightTempVar : EnsureNumber(rightTempVar);
                _methodBodyIR.Instructions.Add(new LIRAddNumber(leftNumber, rightNumber, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }

            // String + String
            if (leftType == typeof(string) && rightType == typeof(string))
            {
                _methodBodyIR.Instructions.Add(new LIRConcatStrings(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                return true;
            }

            // Dynamic addition (unknown types). Prefer avoiding boxing if exactly one side is already an unboxed double.
            if (leftIsUnboxedDouble && rightType != typeof(double))
            {
                var rightBoxedForAdd = EnsureObject(rightTempVar);
                _methodBodyIR.Instructions.Add(new LIRAddDynamicDoubleObject(leftTempVar, rightBoxedForAdd, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;
            }

            if (rightIsUnboxedDouble && leftType != typeof(double))
            {
                var leftBoxedForAdd = EnsureObject(leftTempVar);
                _methodBodyIR.Instructions.Add(new LIRAddDynamicObjectDouble(leftBoxedForAdd, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;
            }

            // General dynamic addition: box operands and call Operators.Add(object, object)
            var leftBoxed = EnsureObject(leftTempVar);
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRAddDynamic(leftBoxed, rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            return true;
        }

        // Handle multiplication
        // JS '*' always follows ToNumber semantics. Prefer emitting numeric coercion (only when needed)
        // and a native IL 'mul' rather than calling Operators.Multiply.
        if (binaryExpr.Operator == Acornima.Operator.Multiplication)
        {
            var leftIsUnboxedDouble = leftStorage.Kind == ValueStorageKind.UnboxedValue && leftType == typeof(double);
            var rightIsUnboxedDouble = rightStorage.Kind == ValueStorageKind.UnboxedValue && rightType == typeof(double);

            // Only emit native numeric ops when both operands are truly unboxed doubles.
            leftTempVar = leftIsUnboxedDouble ? leftTempVar : EnsureNumber(leftTempVar);
            rightTempVar = rightIsUnboxedDouble ? rightTempVar : EnsureNumber(rightTempVar);

            _methodBodyIR.Instructions.Add(new LIRMulNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle subtraction
        if (binaryExpr.Operator == Acornima.Operator.Subtraction)
        {
            // JS '-' operator always follows numeric coercion (ToNumber) semantics.
            // Support both the fast path (double - double) and the general path via EnsureNumber.
            var leftIsUnboxedDouble = leftStorage.Kind == ValueStorageKind.UnboxedValue && leftType == typeof(double);
            var rightIsUnboxedDouble = rightStorage.Kind == ValueStorageKind.UnboxedValue && rightType == typeof(double);

            // Only emit native numeric ops when both operands are truly unboxed doubles.
            leftTempVar = leftIsUnboxedDouble ? leftTempVar : EnsureNumber(leftTempVar);
            rightTempVar = rightIsUnboxedDouble ? rightTempVar : EnsureNumber(rightTempVar);

            _methodBodyIR.Instructions.Add(new LIRSubNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle division
        if (binaryExpr.Operator == Acornima.Operator.Division)
        {
            // JS '/' operator follows ToNumber semantics.
            var leftIsUnboxedDouble = leftStorage.Kind == ValueStorageKind.UnboxedValue && leftType == typeof(double);
            var rightIsUnboxedDouble = rightStorage.Kind == ValueStorageKind.UnboxedValue && rightType == typeof(double);

            // Only emit native numeric ops when both operands are truly unboxed doubles.
            leftTempVar = leftIsUnboxedDouble ? leftTempVar : EnsureNumber(leftTempVar);
            rightTempVar = rightIsUnboxedDouble ? rightTempVar : EnsureNumber(rightTempVar);

            _methodBodyIR.Instructions.Add(new LIRDivNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle remainder (modulo)
        if (binaryExpr.Operator == Acornima.Operator.Remainder)
        {
            // JS '%' operator follows ToNumber semantics.
            var leftIsUnboxedDouble = leftStorage.Kind == ValueStorageKind.UnboxedValue && leftType == typeof(double);
            var rightIsUnboxedDouble = rightStorage.Kind == ValueStorageKind.UnboxedValue && rightType == typeof(double);

            // Only emit native numeric ops when both operands are truly unboxed doubles.
            leftTempVar = leftIsUnboxedDouble ? leftTempVar : EnsureNumber(leftTempVar);
            rightTempVar = rightIsUnboxedDouble ? rightTempVar : EnsureNumber(rightTempVar);

            _methodBodyIR.Instructions.Add(new LIRModNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle exponentiation (** operator)
        if (binaryExpr.Operator == Acornima.Operator.Exponentiation)
        {
            // JS '**' operator follows ToNumber semantics.
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRExpNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle bitwise operators
        if (binaryExpr.Operator == Acornima.Operator.BitwiseAnd)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRBitwiseAnd(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.BitwiseOr)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRBitwiseOr(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.BitwiseXor)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRBitwiseXor(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle shift operators
        if (binaryExpr.Operator == Acornima.Operator.LeftShift)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRLeftShift(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.RightShift)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRRightShift(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.UnsignedRightShift)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRUnsignedRightShift(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle 'in' operator
        if (binaryExpr.Operator == Acornima.Operator.In)
        {
            // 'in' operator: checks if property exists in object
            var leftBoxed = EnsureObject(leftTempVar);
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRInOperator(leftBoxed, rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            return true;
        }

        // Handle comparison operators
        switch (binaryExpr.Operator)
        {
            case Acornima.Operator.LessThan:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    leftTempVar = EnsureNumber(leftTempVar);
                    rightTempVar = EnsureNumber(rightTempVar);
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.GreaterThan:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    leftTempVar = EnsureNumber(leftTempVar);
                    rightTempVar = EnsureNumber(rightTempVar);
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberGreaterThan(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.LessThanOrEqual:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    leftTempVar = EnsureNumber(leftTempVar);
                    rightTempVar = EnsureNumber(rightTempVar);
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThanOrEqual(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.GreaterThanOrEqual:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    leftTempVar = EnsureNumber(leftTempVar);
                    rightTempVar = EnsureNumber(rightTempVar);
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberGreaterThanOrEqual(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.Equality:
                // Support both number and boolean equality, with dynamic fallback
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareNumberEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else if (leftType == typeof(bool) && rightType == typeof(bool))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareBooleanEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else
                {
                    // Dynamic equality for unknown types
                    var leftBoxed = EnsureObject(leftTempVar);
                    var rightBoxed = EnsureObject(rightTempVar);
                    _methodBodyIR.Instructions.Add(new LIREqualDynamic(leftBoxed, rightBoxed, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

            case Acornima.Operator.StrictEquality:
                // Support both number and boolean strict equality, with dynamic fallback
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareNumberEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else if (leftType == typeof(bool) && rightType == typeof(bool))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareBooleanEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else
                {
                    // Dynamic strict equality for unknown types
                    var leftBoxed = EnsureObject(leftTempVar);
                    var rightBoxed = EnsureObject(rightTempVar);
                    _methodBodyIR.Instructions.Add(new LIRStrictEqualDynamic(leftBoxed, rightBoxed, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

            case Acornima.Operator.Inequality:
                // Support both number and boolean inequality, with dynamic fallback
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareNumberNotEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else if (leftType == typeof(bool) && rightType == typeof(bool))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareBooleanNotEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else
                {
                    // Dynamic inequality for unknown types
                    var leftBoxed = EnsureObject(leftTempVar);
                    var rightBoxed = EnsureObject(rightTempVar);
                    _methodBodyIR.Instructions.Add(new LIRNotEqualDynamic(leftBoxed, rightBoxed, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

            case Acornima.Operator.StrictInequality:
                // Support both number and boolean strict inequality, with dynamic fallback
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareNumberNotEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else if (leftType == typeof(bool) && rightType == typeof(bool))
                {
                    _methodBodyIR.Instructions.Add(new LIRCompareBooleanNotEqual(leftTempVar, rightTempVar, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                else
                {
                    // Dynamic strict inequality for unknown types
                    var leftBoxed = EnsureObject(leftTempVar);
                    var rightBoxed = EnsureObject(rightTempVar);
                    _methodBodyIR.Instructions.Add(new LIRStrictNotEqualDynamic(leftBoxed, rightBoxed, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

            default:
                return false;
        }
    }

    private TempVariable EmitConstString(string value)
    {
        var t = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(value, t));
        DefineTempStorage(t, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return t;
    }

    private TempVariable EmitConstNumber(double value)
    {
        var t = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(value, t));
        DefineTempStorage(t, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        return t;
    }

    private bool TryDeclareBinding(Symbol symbol, TempVariable value)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        var binding = symbol.BindingInfo;

        // Captured variable - store to leaf scope field
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null &&
                storage.Kind == BindingStorageKind.LeafScopeField &&
                !storage.Field.IsNil &&
                !storage.DeclaringScope.IsNil)
            {
                var boxedValue = EnsureObject(value);
                lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
                _variableMap[binding] = value;
                return true;
            }
        }

        // Non-captured variable - use SSA temp
        _variableMap[binding] = value;

        var storageInfo = GetTempStorage(value);
        var slot = GetOrCreateVariableSlot(binding, symbol.Name, storageInfo);
        SetTempVariableSlot(value, slot);
        _methodBodyIR.SingleAssignmentSlots.Add(slot);
        return true;
    }

    private void EmitDestructuringNullGuard(TempVariable sourceObject, string? sourceVariableName, string? targetVariableName)
    {
        // Inline the fast-path check to avoid a runtime helper call in the normal case.
        // Equivalent to:
        //   if (sourceValue is not null && sourceValue is not JsNull) return;
        //   ThrowDestructuringNullOrUndefined(...)

        var throwLabel = CreateLabel();
        var okLabel = CreateLabel();

        // Ensure we branch on an object reference (IL brfalse/brtrue work on object refs, not doubles).
        sourceObject = EnsureObject(sourceObject);

        // If sourceObject is null (undefined) => jump to throw.
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(sourceObject, throwLabel));

        // If sourceObject is boxed JsNull (null), fall through to throw; otherwise jump to ok.
        var isJsNullTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRIsInstanceOf(typeof(JavaScriptRuntime.JsNull), sourceObject, isJsNullTemp));
        DefineTempStorage(isJsNullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(isJsNullTemp, okLabel));

        // Shared throw block so we only emit one helper callsite.
        _methodBodyIR.Instructions.Add(new LIRLabel(throwLabel));
        EmitDestructuringNullOrUndefinedThrow(sourceObject, sourceVariableName, targetVariableName);

        _methodBodyIR.Instructions.Add(new LIRLabel(okLabel));
    }

    private void EmitDestructuringNullOrUndefinedThrow(TempVariable sourceObject, string? sourceVariableName, string? targetVariableName)
    {
        // Centralized throw helper so messages/types can match Node/V8 and be localized in the future.
        var sourceNameTemp = EmitConstString(sourceVariableName ?? string.Empty);
        var targetNameTemp = EmitConstString(targetVariableName ?? string.Empty);

        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStaticVoid(
            IntrinsicName: "Object",
            MethodName: nameof(JavaScriptRuntime.Object.ThrowDestructuringNullOrUndefined),
            Arguments: new[] { EnsureObject(sourceObject), EnsureObject(sourceNameTemp), EnsureObject(targetNameTemp) }));
    }

    private static string? TryGetSimpleSourceNameForDestructuring(HIRExpression source)
    {
        // Best-effort source name extraction:
        // - Handles only direct variable references, e.g. `const { a } = obj;` -> "obj".
        // - More complex sources such as member access (`obj.prop`) or calls (`getObj()`)
        //   intentionally return null here; the caller falls back to a generic/empty
        //   source name in the resulting error message.
        return source is HIRVariableExpression v ? v.Name.Name : null;
    }

    private static string GetFirstTargetNameForDestructuring(HIRObjectPattern obj)
    {
        if (obj.Properties.Count > 0)
        {
            return obj.Properties[0].Key;
        }
        return "<unknown>";
    }

    private static string GetFirstTargetNameForDestructuring(HIRArrayPattern arr)
    {
        for (int i = 0; i < arr.Elements.Count; i++)
        {
            if (arr.Elements[i] != null)
            {
                return i.ToString();
            }
        }
        // Even elided/rest-only patterns still require a coercible source.
        return "0";
    }

    private enum DestructuringWriteMode
    {
        Declaration,
        Assignment,
        ForDeclarationBindingInitialization
    }

}
