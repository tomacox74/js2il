using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed partial class HIRToLIRLowerer
{
    private bool TryLowerUnaryExpression(HIRUnaryExpression unaryExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // void operator: evaluate operand for side-effects, then yield `undefined`.
        // This is commonly used by transpiled/compiled JS as `void 0`.
        if (unaryExpr.Operator == Acornima.Operator.Void)
        {
            if (!TryLowerExpressionDiscardResult(unaryExpr.Argument))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // delete operator requires lvalue semantics (delete obj[prop] / delete obj.prop)
        if (unaryExpr.Operator == Acornima.Operator.Delete)
        {
            switch (unaryExpr.Argument)
            {
                case HIRPropertyAccessExpression propAccess:
                {
                    if (!TryLowerExpression(propAccess.Object, out var recvTemp))
                    {
                        return false;
                    }

                    var keyTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString(propAccess.PropertyName, keyTemp));
                    DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    var deleted = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.DeleteProperty), new[] { EnsureObject(recvTemp), EnsureObject(keyTemp) }, deleted));
                    DefineTempStorage(deleted, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

                    // delete returns boolean
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(deleted, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                case HIRIndexAccessExpression indexAccess:
                {
                    if (!TryLowerExpression(indexAccess.Object, out var recvTemp))
                    {
                        return false;
                    }
                    if (!TryLowerExpression(indexAccess.Index, out var indexTemp))
                    {
                        return false;
                    }

                    var deleted = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(nameof(JavaScriptRuntime.Object), nameof(JavaScriptRuntime.Object.DeleteItem), new[] { EnsureObject(recvTemp), EnsureObject(indexTemp) }, deleted));
                    DefineTempStorage(deleted, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(deleted, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }
                default:
                    // Minimal semantics: delete of non-reference returns true.
                    _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
            }
        }

        if (!TryLowerExpression(unaryExpr.Argument, out var unaryArgTempVar))
        {
            return false;
        }

        if (unaryExpr.Operator == Acornima.Operator.TypeOf)
        {
            unaryArgTempVar = EnsureObject(unaryArgTempVar);
            _methodBodyIR.Instructions.Add(new LIRTypeof(unaryArgTempVar, resultTempVar));
            this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        if (unaryExpr.Operator == Acornima.Operator.LogicalNot)
        {
            // JS logical not: coerce to boolean (truthiness) then invert.
            // Prefer keeping typed/unboxed values when possible; IL emission will select
            // the appropriate TypeUtilities.ToBoolean overload to avoid boxing.
            _methodBodyIR.Instructions.Add(new LIRLogicalNot(unaryArgTempVar, resultTempVar));
            this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            return true;
        }

        if (unaryExpr.Operator == Acornima.Operator.UnaryNegation)
        {
            // Minimal: only support numeric (double) negation for now
            if (GetTempStorage(unaryArgTempVar).ClrType != typeof(double))
            {
                return false;
            }
            _methodBodyIR.Instructions.Add(new LIRNegateNumber(unaryArgTempVar, resultTempVar));
            this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        if (unaryExpr.Operator == Acornima.Operator.BitwiseNot)
        {
            // Minimal: ~x where x is numeric (double). Legacy pipeline coerces via ToNumber;
            // IR pipeline currently only supports number operands for this operator.
            if (GetTempStorage(unaryArgTempVar).ClrType != typeof(double))
            {
                return false;
            }
            _methodBodyIR.Instructions.Add(new LIRBitwiseNotNumber(unaryArgTempVar, resultTempVar));
            this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        return false;
    }
}
