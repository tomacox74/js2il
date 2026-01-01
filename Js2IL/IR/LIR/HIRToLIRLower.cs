using Js2IL.HIR;
using Js2IL.Services;

namespace Js2IL.IR;

public sealed class HIRToLIRLowerer
{
    private readonly MethodBodyIR _methodBodyIR = new MethodBodyIR();

    private int _tempVarCounter = 0;

    private int _localVarCounter = 0;

    private readonly Dictionary<string, LocalVariable> _variableMap = new Dictionary<string, LocalVariable>();

    private readonly Dictionary<TempVariable, ValueStorage> _tempVarTypes = new Dictionary<TempVariable, ValueStorage>();

    private readonly Dictionary<LocalVariable, ValueStorage> _localVarTypes = new Dictionary<LocalVariable, ValueStorage>();

    private LocalVariable CreateScratchLocal(ValueStorage storage)
    {
        var localVar = new LocalVariable(_localVarCounter);
        _localVarCounter++;
        _methodBodyIR.Locals.Add(localVar);
        _localVarTypes[localVar] = storage;
        return localVar;
    }

    public static bool TryLower(HIRMethod hirMethod, out MethodBodyIR? lirMethod)
    {
        lirMethod = null;

        var lowerer = new HIRToLIRLowerer();
        if (lowerer.TryLowerStatements(hirMethod.Body.Statements))
        {
            lirMethod = lowerer._methodBodyIR;
            return true;
        }

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
            case HIRVariableDeclaration exprStmt:
                // Create instructions for the initializer expression
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
                    // No initializer means 'undefined'
                    valueTempVar = CreateTempVariable();
                    lirInstructions.Add(new LIRConstUndefined(valueTempVar));
                }

                var localVar = CreateLocalVariable(exprStmt.Name.Name);
                lirInstructions.Add(new LIRStoreLocal(valueTempVar, localVar));

                // Make the type transitive from temp to local
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
            case HIRReturnStatement returnStmt:
                {
                    TempVariable returnTempVar;
                    if (returnStmt.Expression != null)
                    {
                        // Lower the return expression
                        if (!TryLowerExpression(returnStmt.Expression, out returnTempVar))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Bare return - return undefined (null)
                        returnTempVar = CreateTempVariable();
                        lirInstructions.Add(new LIRConstUndefined(returnTempVar));
                    }
                    lirInstructions.Add(new LIRReturn(returnTempVar));
                    return true;
                }
            default:
                // Unsupported statement type
                return false;
        }
    }

    private bool TryLowerExpression(HIRExpression expression, out TempVariable resultTempVar)
    {
        // All expressions produce a result
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

                    case JavascriptType.Boolean:
                        bool boolValue = literal.Value != null && (bool)literal.Value;
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(boolValue, resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;

                    case JavascriptType.Null:
                        // JavaScript 'null' literal - raw value, boxing added by EnsureObject when needed
                        _methodBodyIR.Instructions.Add(new LIRConstNull(resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(JavaScriptRuntime.JsNull)));
                        return true;

                    case JavascriptType.Undefined:
                        // JavaScript 'undefined' - represented as CLR null
                        _methodBodyIR.Instructions.Add(new LIRConstUndefined(resultTempVar));
                        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
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

                if (GetTempStorage(leftTempVar).ClrType != typeof(double) ||
                    GetTempStorage(rightTempVar).ClrType != typeof(double))
                {
                    // For now, only support number addition
                    return false;
                }

                _methodBodyIR.Instructions.Add(new LIRAddNumber(leftTempVar, rightTempVar, resultTempVar));
                this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;

            case HIRCallExpression callExpr:
                return TryLowerCallExpression(callExpr, out resultTempVar);

            case HIRUnaryExpression unaryExpr:
                return TryLowerUnaryExpression(unaryExpr, resultTempVar);

            case HIRUpdateExpression updateExpr:
                return TryLowerUpdateExpression(updateExpr, resultTempVar);

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

        // At this time we are hardcoded to only support console.log
        // This is proof of concept code
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
            
            // Store argTempVar into arrayTempVar at index
            _methodBodyIR.Instructions.Add(new LIRStoreElementRef(arrayTempVar, index, argTempVar));
        }

        _methodBodyIR.Instructions.Add(new LIRCallIntrinsic(consoleTempVar, "log", arrayTempVar, resultTempVar));

        return true;
    }

    private bool TryLowerUnaryExpression(HIRUnaryExpression unaryExpr, TempVariable resultTempVar)
    {
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

    private bool TryLowerUpdateExpression(HIRUpdateExpression updateExpr, TempVariable resultTempVar)
    {
        // Minimal: only support ++/-- on local variables (identifiers)
        if (updateExpr.Operator != Acornima.Operator.Increment && updateExpr.Operator != Acornima.Operator.Decrement)
        {
            return false;
        }

        if (updateExpr.Argument is not HIRVariableExpression updateVarExpr)
        {
            return false;
        }

        if (!_variableMap.TryGetValue(updateVarExpr.Name.Name, out var targetLocal))
        {
            return false;
        }

        // Only support numeric locals (double) for now
        if (GetLocalStorage(targetLocal).ClrType != typeof(double))
        {
            return false;
        }

        var delta = updateExpr.Operator == Acornima.Operator.Increment ? 1.0 : -1.0;

        LocalVariable? scratchLocal = null;

        if (!updateExpr.Prefix)
        {
            // Postfix: result is original value.
            scratchLocal = CreateScratchLocal(new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var originalTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadLocal(targetLocal, originalTemp));
            this.DefineTempStorage(originalTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            _methodBodyIR.Instructions.Add(new LIRStoreLocal(originalTemp, scratchLocal.Value));
        }

        // Compute updated value and store back
        var currentTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadLocal(targetLocal, currentTemp));
        this.DefineTempStorage(currentTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var deltaTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(delta, deltaTemp));
        this.DefineTempStorage(deltaTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var updatedTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRAddNumber(currentTemp, deltaTemp, updatedTemp));
        this.DefineTempStorage(updatedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        _methodBodyIR.Instructions.Add(new LIRStoreLocal(updatedTemp, targetLocal));

        if (updateExpr.Prefix)
        {
            // Prefix returns the updated value
            _methodBodyIR.Instructions.Add(new LIRLoadLocal(targetLocal, resultTempVar));
            this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Postfix returns the original value from scratch local
        if (scratchLocal is null)
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRLoadLocal(scratchLocal.Value, resultTempVar));
        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
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