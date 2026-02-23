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
    private bool TryLowerCompoundOperation(Acornima.Operator op, TempVariable currentValue, TempVariable rhsValue, out TempVariable result)
    {
        result = CreateTempVariable();

        var leftType = GetTempStorage(currentValue).ClrType;
        var rightType = GetTempStorage(rhsValue).ClrType;

        // Most compound operators follow JS numeric semantics (ToNumber / ToInt32 / ToUint32 depending on op).
        // In IR lowering, index/property reads come back as object, so we must support numeric coercion here.
        bool EnsureNumericOperands()
        {
            currentValue = EnsureNumber(currentValue);
            rhsValue = EnsureNumber(rhsValue);
            leftType = typeof(double);
            rightType = typeof(double);
            return true;
        }

        switch (op)
        {
            case Acornima.Operator.AdditionAssignment:
                // Number + Number
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRAddNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // String + String
                if (leftType == typeof(string) && rightType == typeof(string))
                {
                    _methodBodyIR.Instructions.Add(new LIRConcatStrings(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                    return true;
                }
                // Dynamic addition (unknown types). Prefer avoiding boxing if exactly one side is already an unboxed double.
                if (leftType == typeof(double) && rightType != typeof(double))
                {
                    var rightBoxedForAdd = EnsureObject(rhsValue);
                    _methodBodyIR.Instructions.Add(new LIRAddDynamicDoubleObject(currentValue, rightBoxedForAdd, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                    return true;
                }

                if (leftType != typeof(double) && rightType == typeof(double))
                {
                    var leftBoxedForAdd = EnsureObject(currentValue);
                    _methodBodyIR.Instructions.Add(new LIRAddDynamicObjectDouble(leftBoxedForAdd, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                    return true;
                }

                // General dynamic addition: box operands and call Operators.Add(object, object)
                var leftBoxed = EnsureObject(currentValue);
                var rightBoxed = EnsureObject(rhsValue);
                _methodBodyIR.Instructions.Add(new LIRAddDynamic(leftBoxed, rightBoxed, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;

            case Acornima.Operator.SubtractionAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRSubNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Subtraction requires numeric types
                return false;

            case Acornima.Operator.MultiplicationAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }

                _methodBodyIR.Instructions.Add(new LIRMulNumber(currentValue, rhsValue, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;

            case Acornima.Operator.DivisionAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRDivNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Division requires numeric types
                return false;

            case Acornima.Operator.RemainderAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRModNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Remainder requires numeric types
                return false;

            case Acornima.Operator.ExponentiationAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRExpNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Exponentiation requires numeric types
                return false;

            case Acornima.Operator.BitwiseAndAssignment:
                {
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRBitwiseAnd(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int)));
                    return true;
                }

            case Acornima.Operator.BitwiseOrAssignment:
                {
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRBitwiseOr(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int)));
                    return true;
                }

            case Acornima.Operator.BitwiseXorAssignment:
                {
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRBitwiseXor(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int)));
                    return true;
                }

            case Acornima.Operator.LeftShiftAssignment:
                {
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRLeftShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int)));
                    return true;
                }

            case Acornima.Operator.RightShiftAssignment:
                {
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRRightShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(int)));
                    return true;
                }

            case Acornima.Operator.UnsignedRightShiftAssignment:
                {
                    // >>> always produces a ToUint32 result (0..4294967295); use double to avoid losing values >= 2^31.
                    bool leftIsNumeric = (leftType == typeof(double) || leftType == typeof(int)) && GetTempStorage(currentValue).Kind == ValueStorageKind.UnboxedValue;
                    bool rightIsNumeric = (rightType == typeof(double) || rightType == typeof(int)) && GetTempStorage(rhsValue).Kind == ValueStorageKind.UnboxedValue;
                    if (!leftIsNumeric) { currentValue = EnsureNumber(currentValue); }
                    if (!rightIsNumeric) { rhsValue = EnsureNumber(rhsValue); }
                    _methodBodyIR.Instructions.Add(new LIRUnsignedRightShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }

            default:
                return false;
        }
    }

    /// <summary>
    /// Uses temp type information to determine if the temp variable is compatible with the object type.
    /// </summary>
    /// <param name="tempVar">Temp variable that needs to be checked for compatibility</param>
    /// <returns>If not compatible, returns a converted temp variable</returns>
    private TempVariable EnsureObject(TempVariable tempVar)
    {
        if (!IsObjectCompatible(tempVar))
        {
            var objectTempVar = CreateTempVariable();
            var storage = this.GetTempStorage(tempVar);
            _methodBodyIR.Instructions.Add(new LIRConvertToObject(tempVar, storage.ClrType ?? typeof(object), objectTempVar));
            this.DefineTempStorage(objectTempVar, new ValueStorage(ValueStorageKind.BoxedValue, storage.ClrType));
            return objectTempVar;
        }

        // For now, we assume all types are objects
        return tempVar;
    }

    private bool IsObjectCompatible(TempVariable tempVar)
    {
        var s = GetTempStorage(tempVar);
        return s.Kind is ValueStorageKind.BoxedValue or ValueStorageKind.Reference;
    }

    private void DefineTempStorage(TempVariable tempVar, ValueStorage storage)
    {
        if (tempVar.Index < 0 || tempVar.Index >= _methodBodyIR.TempStorages.Count)
        {
            // Should never happen; indicates a temp was used without registration.
            return;
        }
        _methodBodyIR.TempStorages[tempVar.Index] = storage;
    }

    private ValueStorage GetTempStorage(TempVariable tempVar)
    {
        if (tempVar.Index < 0 || tempVar.Index >= _methodBodyIR.TempStorages.Count)
        {
            return new ValueStorage(ValueStorageKind.Unknown);
        }
        return _methodBodyIR.TempStorages[tempVar.Index];
    }

    /// <summary>
    /// Returns true if the function binding has only simple identifier parameters
    /// (no defaults, destructuring, or rest patterns).
    /// </summary>
    private static bool FunctionHasSimpleParams(Symbol functionSymbol)
    {
        // Get the declaration node for the function binding
        var declarationNode = functionSymbol.BindingInfo.DeclarationNode;
        
        Acornima.Ast.NodeList<Acornima.Ast.Node>? parameters = null;
        
        if (declarationNode is Acornima.Ast.FunctionDeclaration funcDecl)
        {
            parameters = funcDecl.Params;
        }
        else if (declarationNode is Acornima.Ast.FunctionExpression funcExpr)
        {
            parameters = funcExpr.Params;
        }
        else if (declarationNode is Acornima.Ast.ArrowFunctionExpression arrowFunc)
        {
            parameters = arrowFunc.Params;
        }
        
        // If we couldn't find parameters, bail out conservatively
        if (parameters == null)
        {
            return false;
        }
        
        // Allow identifier params, simple defaults, destructuring patterns, and rest parameters.
        return parameters.Value.All(param => param switch
        {
            Acornima.Ast.Identifier => true,
            Acornima.Ast.AssignmentPattern ap => ap.Left is Acornima.Ast.Identifier,
            Acornima.Ast.ObjectPattern => true,
            Acornima.Ast.ArrayPattern => true,
            Acornima.Ast.RestElement => true,
            _ => false
        });
    }
}
