using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed class HIRToLIRLowerer
{
    private readonly MethodBodyIR _methodBodyIR = new MethodBodyIR();
    private readonly Scope? _scope;

    private int _tempVarCounter = 0;
    private int _labelCounter = 0;

    // Source-level variables map to the current SSA value (TempVariable) at the current program point.
    // Keyed by BindingInfo reference to correctly handle shadowed variables with the same name.
    private readonly Dictionary<BindingInfo, TempVariable> _variableMap = new Dictionary<BindingInfo, TempVariable>();

    // Stable IL-local slot per JS variable declaration.
    // Keyed by BindingInfo reference to give each shadowed variable its own slot.
    private readonly Dictionary<BindingInfo, int> _variableSlots = new Dictionary<BindingInfo, int>();

    // Maps parameter bindings to their 0-based JS parameter index (not IL arg index)
    private readonly Dictionary<BindingInfo, int> _parameterIndexMap = new Dictionary<BindingInfo, int>();

    private HIRToLIRLowerer(Scope? scope)
    {
        _scope = scope;
        InitializeParameters();
    }

    private void InitializeParameters()
    {
        if (_scope == null) return;

        // Build ordered parameter list from scope.Parameters
        // Parameters are simple identifiers, so scope.Parameters contains the names in order
        int paramIndex = 0;
        foreach (var paramName in _scope.Parameters)
        {
            if (_scope.Bindings.TryGetValue(paramName, out var binding))
            {
                _parameterIndexMap[binding] = paramIndex;
                _methodBodyIR.Parameters.Add(paramName);
            }
            paramIndex++;
        }
    }

    public static bool TryLower(HIRMethod hirMethod, Scope? scope, out MethodBodyIR? lirMethod)
    {
        lirMethod = null;

        var lowerer = new HIRToLIRLowerer(scope);
        
        if (lowerer.TryLowerStatements(hirMethod.Body.Statements))
        {
            lirMethod = lowerer._methodBodyIR;
            return true;
        }

        return false;
    }

    // Backward compatibility overload for callers that don't provide scope
    public static bool TryLower(HIRMethod hirMethod, out MethodBodyIR? lirMethod)
    {
        return TryLower(hirMethod, null, out lirMethod);
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
                {
                    // Variable declarations define a new binding in the current scope.
                    // In SSA-style LIR we simply map the name to the produced value.
                    TempVariable value;

                    if (exprStmt.Initializer != null)
                    {
                        if (!TryLowerExpression(exprStmt.Initializer, out value))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // No initializer means 'undefined'
                        value = CreateTempVariable();
                        lirInstructions.Add(new LIRConstUndefined(value));
                        DefineTempStorage(value, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }

                    // Use BindingInfo as key for correct shadowing behavior
                    var binding = exprStmt.Name.BindingInfo;
                    _variableMap[binding] = value;

                    // Assign the declared variable a stable local slot and map this SSA temp to it.
                    // Track the storage type for the variable slot.
                    var storage = GetTempStorage(value);
                    var slot = GetOrCreateVariableSlot(binding, exprStmt.Name.Name, storage);
                    SetTempVariableSlot(value, slot);
                    return true;
                }
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

                        // IR pipeline methods currently return object; ensure boxing/conversion.
                        returnTempVar = EnsureObject(returnTempVar);
                    }
                    else
                    {
                        // Bare return - return undefined (null)
                        returnTempVar = CreateTempVariable();
                        lirInstructions.Add(new LIRConstUndefined(returnTempVar));
                        DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }
                    lirInstructions.Add(new LIRReturn(returnTempVar));
                    return true;
                }
            case HIRIfStatement ifStmt:
                {
                    // Evaluate the test condition
                    if (!TryLowerExpression(ifStmt.Test, out var conditionTemp))
                    {
                        return false;
                    }

                    int elseLabel = CreateLabel();

                    // Branch to else if condition is false
                    lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, elseLabel));

                    // Consequent block (then)
                    if (!TryLowerStatement(ifStmt.Consequent))
                    {
                        return false;
                    }

                    // Alternate block (else) - if present
                    if (ifStmt.Alternate != null)
                    {
                        // Jump over else block
                        int endLabel = CreateLabel();
                        lirInstructions.Add(new LIRBranch(endLabel));

                        // Else label
                        lirInstructions.Add(new LIRLabel(elseLabel));

                        if (!TryLowerStatement(ifStmt.Alternate))
                        {
                            return false;
                        }

                        // End label
                        lirInstructions.Add(new LIRLabel(endLabel));
                    }
                    else
                    {
                        // No else block - just emit the else label (which is effectively the end)
                        lirInstructions.Add(new LIRLabel(elseLabel));
                    }

                    return true;
                }
            case HIRBlock block:
                // Lower each statement in the block - return false on first failure
                return block.Statements.All(TryLowerStatement);
            default:
                // Unsupported statement type
                return false;
        }
    }

    private int CreateLabel() => _labelCounter++;

    private bool TryLowerExpression(HIRExpression expression, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        switch (expression)
        {
            case HIRLiteralExpression literal:
                // All literals allocate a new SSA value.
                resultTempVar = CreateTempVariable();
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
                return TryLowerBinaryExpression(binaryExpr, out resultTempVar);

            case HIRCallExpression callExpr:
                return TryLowerCallExpression(callExpr, out resultTempVar);

            case HIRUnaryExpression unaryExpr:
                return TryLowerUnaryExpression(unaryExpr, out resultTempVar);

            case HIRUpdateExpression updateExpr:
                return TryLowerUpdateExpression(updateExpr, out resultTempVar);

            case HIRVariableExpression varExpr:
                // Look up the binding using the Symbol's BindingInfo directly
                // This correctly resolves shadowed variables to the right binding
                var binding = varExpr.Name.BindingInfo;
                
                // Check if this is a parameter - emit LIRLoadParameter
                if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
                {
                    resultTempVar = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRLoadParameter(paramIndex, resultTempVar));
                    // Parameters are always type object (unknown type)
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }
                
                if (!_variableMap.TryGetValue(binding, out resultTempVar))
                {
                    return false;
                }

                // Variable reads are SSA value lookups (no load instruction).
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

        // Case 1: User-defined function call (callee is a variable referencing a function)
        if (callExpr.Callee is HIRVariableExpression funcVarExpr)
        {
            var symbol = funcVarExpr.Name;
            
            // Only handle function bindings for now (BindingKind.Function)
            if (symbol.Kind != BindingKind.Function)
            {
                return false;
            }

            // Check if the function has simple identifier parameters (no defaults, destructuring, rest).
            // If the function uses complex params, it will be compiled via traditional generator
            // with a different calling convention, so we must bail out to ensure Main is also
            // compiled traditionally to maintain calling convention consistency.
            if (!FunctionHasSimpleParams(symbol))
            {
                return false;
            }
            
            // Lower all arguments first
            var arguments = new List<TempVariable>();
            foreach (var arg in callExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                // Ensure arguments are boxed as object for function calls
                arguments.Add(EnsureObject(argTemp));
            }

            // Create scopes array placeholder.
            // In the legacy direct IL emitter, the global scope instance was typically stored
            // in local 0 of the main method, and that local was used to populate this array.
            // In the IR-based pipeline used here, we currently pass 'default' (effectively null)
            // and let the downstream IL emitter decide how (or whether) to materialize scopes.
            var scopesTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCreateScopesArray(default, scopesTempVar));
            DefineTempStorage(scopesTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            // Emit the function call with arguments
            _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, scopesTempVar, arguments, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            return true;
        }

        // Case 2: Property access call (e.g., console.log)
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
        this.DefineTempStorage(arrayTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

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

        // console.log returns undefined (null)
        this.DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return true;
    }

    private bool TryLowerUnaryExpression(HIRUnaryExpression unaryExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

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

    private bool TryLowerBinaryExpression(HIRBinaryExpression binaryExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        if (!TryLowerExpression(binaryExpr.Left, out var leftTempVar))
        {
            return false;
        }

        if (!TryLowerExpression(binaryExpr.Right, out var rightTempVar))
        {
            return false;
        }

        var leftType = GetTempStorage(leftTempVar).ClrType;
        var rightType = GetTempStorage(rightTempVar).ClrType;

        // Handle addition
        if (binaryExpr.Operator == Acornima.Operator.Addition)
        {
            // Number + Number
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRAddNumber(leftTempVar, rightTempVar, resultTempVar));
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

            // Dynamic addition (unknown types) - box operands and call Operators.Add
            var leftBoxed = EnsureObject(leftTempVar);
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRAddDynamic(leftBoxed, rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            return true;
        }

        // Handle multiplication
        if (binaryExpr.Operator == Acornima.Operator.Multiplication)
        {
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRMulNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle comparison operators
        switch (binaryExpr.Operator)
        {
            case Acornima.Operator.LessThan:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.GreaterThan:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberGreaterThan(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.LessThanOrEqual:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThanOrEqual(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.GreaterThanOrEqual:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    return false;
                }
                _methodBodyIR.Instructions.Add(new LIRCompareNumberGreaterThanOrEqual(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                return true;

            case Acornima.Operator.Equality:
            case Acornima.Operator.StrictEquality:
                // Support both number and boolean equality
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
                return false;

            case Acornima.Operator.Inequality:
            case Acornima.Operator.StrictInequality:
                // Support both number and boolean inequality
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
                return false;

            default:
                return false;
        }
    }

    private bool TryLowerUpdateExpression(HIRUpdateExpression updateExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // Minimal: only support ++/-- on local variables (identifiers)
        if (updateExpr.Operator != Acornima.Operator.Increment && updateExpr.Operator != Acornima.Operator.Decrement)
        {
            return false;
        }

        if (updateExpr.Argument is not HIRVariableExpression updateVarExpr)
        {
            return false;
        }

        var updateBinding = updateVarExpr.Name.BindingInfo;
        if (!_variableMap.TryGetValue(updateBinding, out var currentValue))
        {
            return false;
        }

        // Only support numeric locals (double) for now
        if (GetTempStorage(currentValue).ClrType != typeof(double))
        {
            return false;
        }

        var slot = GetOrCreateVariableSlot(updateBinding, updateVarExpr.Name.Name, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var isIncrement = updateExpr.Operator == Acornima.Operator.Increment;

        // In SSA: ++/-- produces a new value and updates the variable binding.
        // Prefix returns updated value; postfix returns original value.
        var originalTemp = currentValue;

        // Make sure the current value is associated with the variable slot.
        SetTempVariableSlot(originalTemp, slot);

        // For postfix, capture/box the original value *before* we emit the update that overwrites
        // the stable variable local slot. Otherwise, later loads of originalTemp would observe the
        // updated value.
        TempVariable? boxedOriginalForPostfix = null;
        if (!updateExpr.Prefix)
        {
            boxedOriginalForPostfix = EnsureObject(originalTemp);
        }

        var deltaTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaTemp));
        this.DefineTempStorage(deltaTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        var updatedTemp = CreateTempVariable();
        if (isIncrement)
        {
            _methodBodyIR.Instructions.Add(new LIRAddNumber(originalTemp, deltaTemp, updatedTemp));
        }
        else
        {
            _methodBodyIR.Instructions.Add(new LIRSubNumber(originalTemp, deltaTemp, updatedTemp));
        }
        this.DefineTempStorage(updatedTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

        // The updated SSA value represents the same JS variable slot.
        SetTempVariableSlot(updatedTemp, slot);

        // Update variable mapping to the new SSA value.
        _variableMap[updateBinding] = updatedTemp;

        if (updateExpr.Prefix)
        {
            // Prefix returns the updated value, boxed to object so we can store/emit without extra locals.
            resultTempVar = EnsureObject(updatedTemp);
            return true;
        }

        // Postfix returns the original value.
        resultTempVar = boxedOriginalForPostfix!.Value;
        return true;
    }

    private TempVariable CreateTempVariable()
    {
        var tempVar = new TempVariable(_tempVarCounter);
        _tempVarCounter++;
        _methodBodyIR.Temps.Add(tempVar);
        _methodBodyIR.TempStorages.Add(new ValueStorage(ValueStorageKind.Unknown));
        _methodBodyIR.TempVariableSlots.Add(-1);
        return tempVar;
    }

    private int GetOrCreateVariableSlot(BindingInfo binding, string displayName, ValueStorage storage)
    {
        if (_variableSlots.TryGetValue(binding, out var slot))
        {
            return slot;
        }

        slot = _methodBodyIR.VariableNames.Count;
        _variableSlots[binding] = slot;
        _methodBodyIR.VariableNames.Add(displayName);
        _methodBodyIR.VariableStorages.Add(storage);
        return slot;
    }

    private void SetTempVariableSlot(TempVariable temp, int slot)
    {
        if (temp.Index < 0 || temp.Index >= _methodBodyIR.TempVariableSlots.Count)
        {
            return;
        }
        _methodBodyIR.TempVariableSlots[temp.Index] = slot;
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
        
        // Check that all params are simple identifiers
        return parameters.Value.All(param => param is Acornima.Ast.Identifier);
    }
}