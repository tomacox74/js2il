using Js2IL.HIR;
using Js2IL.Services;

namespace Js2IL.IR;

public sealed class HIRToLIRLowerer
{
    private MethodBodyIR _methodBodyIR = new MethodBodyIR();

    private int _tempVarCounter = 0;

    private int _localVarCounter = 0;

    private Dictionary<string, LocalVariable> _variableMap = new Dictionary<string, LocalVariable>();

    private Dictionary<TempVariable, ValueStorage> _tempVarTypes = new Dictionary<TempVariable, ValueStorage>();

    private Dictionary<LocalVariable, ValueStorage> _localVarTypes = new Dictionary<LocalVariable, ValueStorage>();


    public static bool TryLower(HIRMethod hirMethod, out MethodBodyIR lirMethod)
    {
        var lowerer = new HIRToLIRLowerer();
        if (lowerer.TryLowerStatements(hirMethod.Body.Statements))
        {
            lirMethod = lowerer._methodBodyIR;
            return true;
        }
        lirMethod = null;
        return false;
    } 

    public bool TryLowerStatements(IEnumerable<HIRStatement> statements)
    {
        foreach (var statement in statements)
        {
            if (!TryLowerStatement(statement))
            {
                return false;
            }
        }

        return true;
    }

    private bool TryLowerStatement(HIRStatement statement)
    {
        var lirInstructions = _methodBodyIR.Instructions;

        switch (statement)
        {
            case HRIVariableDeclaration exprStmt:
                // we need to create instructions for the initializer expression
                TempVariable valueTempVar;
                if (exprStmt.Initializer != null)
                {
                    if (!TryLowerExpression(exprStmt.Initializer, out valueTempVar))
                    {
                        return false;
                    }
                }
                else
                {
                    // no initializer means 'undefined'
                    valueTempVar = CreateTempVariable();
                    lirInstructions.Add(new LIRConstUndefined(valueTempVar));
                }

                var localVar = CreateLocalVariable(exprStmt.Name.Name);
                lirInstructions.Add(new LIRStoreLocal(valueTempVar, localVar));

                // make the type transitive from temp to local
                var storage = GetTempStorage(valueTempVar);
                _localVarTypes[localVar] = storage;

                return true;
            case HIRExpressionStatement exprStmt:
                {
                    // Lower the expression and discard the result
                    if (!TryLowerExpression(exprStmt.Expression, out var _))
                    {
                        return false;
                    }
                    return true;
                }
            default:
                // Unsupported statement type
                return false;
        }
    }

    private bool TryLowerExpression(HIRExpression expression, out TempVariable resultTempVar)
    {
        // all expressions produce a result
        resultTempVar = CreateTempVariable();

        switch (expression)
        {
            case HIRLiteralExpression literal:
                switch (literal.Kind)
                {
                    case JavascriptType.String:
                        _methodBodyIR.Instructions.Add(new LIRConstString((string)literal.Value!, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;

                    case JavascriptType.Number:
                        double value = 0;
                        if (literal.Value != null)
                        {
                            value = (double)literal.Value;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConstNumber(value, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;

                    default:
                        // Unsupported literal type
                        return false;
                }

            case HIRBinaryExpression binaryExpr:
                if (binaryExpr.Operator != Acornima.Operator.Addition)
                {
                    // Unsupported binary operator
                    return false;
                }

                if (!TryLowerExpression(binaryExpr.Left, out var leftTempVar))
                {
                    return false;
                }

                if (!TryLowerExpression(binaryExpr.Right, out var rightTempVar))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRAddNumber(leftTempVar, rightTempVar, resultTempVar));
                this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;

            case HIRCallExpression callExpr:
                return TryLowerCallExpression(callExpr, out resultTempVar);

            case HIRVariableExpression varExpr:
                if (!_variableMap.TryGetValue(varExpr.Name.Name, out var localVar))
                {
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRLoadLocal(localVar, resultTempVar));
                var storage = GetLocalStorage(localVar);
                this.DefineTempStorage(resultTempVar, storage);
                return true;
            // Handle different expression types here
            default:
                // Unsupported expression type
                return false;
        }
    }

    private bool TryLowerCallExpression(HIRCallExpression callExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (callExpr.Callee is not HIRPropertyAccessExpression calleePropAccess)
        {
            return false;
        }

        // so basically at this time we are hardcoded to only support console.log
        // this draft is proof of concept
        if (calleePropAccess.Object is not HIRVariableExpression calleeObject ||
            calleeObject.Name.Name != "console")
        {
            return false;
        }

        if (calleePropAccess.PropertyName != "log")
        {
            return false;
        }

        var consoleTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetIntrinsicGlobal("console", consoleTempVar));
        this.DefineTempStorage(consoleTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Console)));

        // console.log takes its arguments as a array of type object
        var arrayTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewObjectArray(callExpr.Arguments.Count(), arrayTempVar));

        foreach (var (argExpr, index) in callExpr.Arguments.Select((expr, idx) => (expr, idx)))
        {
            _methodBodyIR.Instructions.Add(new LIRBeginInitArrayElement(arrayTempVar, index));

            if (!TryLowerExpression(argExpr, out var argTempVar))
            {
                return false;
            }

            argTempVar = EnsureObject(argTempVar);
            
            // store argTempVar into arrayTempVar at index
            _methodBodyIR.Instructions.Add(new LIRStoreElementRef(arrayTempVar, index, argTempVar));
        }

        _methodBodyIR.Instructions.Add(new LIRCallIntrinsic(consoleTempVar, "log", arrayTempVar, resultTempVar));

        return true;
    }

    private TempVariable CreateTempVariable()
    {
        var tempVar = new TempVariable(_tempVarCounter);
        _tempVarCounter++;
        _methodBodyIR.Temps.Add(tempVar);
        return tempVar;
    }

    private LocalVariable CreateLocalVariable(string name)
    {
        var localVar = new LocalVariable(_localVarCounter);
        _localVarCounter++;
        _methodBodyIR.Locals.Add(localVar);
        _variableMap[name] = localVar;
        return localVar;
    }

    /// <summary>
    /// Uses temp type inforamtion to determine if the temp variable is compatiable with the object type.
    /// </summary>
    /// <param name="tempVar">tempVar that needs to be checkedfor compatilbity</param>
    /// <returns>if not compatible, returns a converrted tempVar</returns>
    private TempVariable EnsureObject(TempVariable tempVar)
    {
        if (!IsObjectCompatible(tempVar))
        {
            var objectTempVar = CreateTempVariable();
            var storage = this.GetTempStorage(tempVar);
            _methodBodyIR.Instructions.Add(new LIRConvertToObject(tempVar, objectTempVar));
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
        _tempVarTypes[tempVar] = storage;
    }

    private ValueStorage GetTempStorage(TempVariable tempVar)
    {
        if (_tempVarTypes.TryGetValue(tempVar, out var storage))
        {
            return storage;
        }
        return new ValueStorage(ValueStorageKind.Unknown);
    }

    private void DefineLocalStorage(LocalVariable localVar, ValueStorage storage)
    {
        _localVarTypes[localVar] = storage;
    }

    private ValueStorage GetLocalStorage(LocalVariable localVar)
    {
        if (_localVarTypes.TryGetValue(localVar, out var storage))
        {
            return storage;
        }
        return new ValueStorage(ValueStorageKind.Unknown);
    }
}