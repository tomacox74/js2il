using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed class HIRToLIRLowerer
{
    private readonly MethodBodyIR _methodBodyIR = new MethodBodyIR();
    private readonly Scope? _scope;
    private readonly EnvironmentLayout? _environmentLayout;

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

    // Track whether parameter initialization was successful (affects TryLower result)
    private bool _parameterInitSucceeded = true;

    private HIRToLIRLowerer(Scope? scope, EnvironmentLayout? environmentLayout)
    {
        _scope = scope;
        _environmentLayout = environmentLayout;
        InitializeParameters();
    }

    private void InitializeParameters()
    {
        if (_scope == null) return;

        // Build ordered parameter list from scope.Parameters
        // Parameters are simple identifiers or AssignmentPatterns, so scope.Parameters contains the names in order
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

        // Emit default parameter initializers for parameters with defaults
        // If any fail, mark the initialization as failed (will cause TryLower to return false)
        _parameterInitSucceeded = EmitDefaultParameterInitializers();
    }

    /// <summary>
    /// Extracts the AST parameter list from the scope's AST node (if it's a function).
    /// Returns null if the scope doesn't have accessible parameters.
    /// </summary>
    private NodeList<Node>? GetAstParameters()
    {
        if (_scope?.AstNode == null) return null;

        return _scope.AstNode switch
        {
            FunctionDeclaration funcDecl => funcDecl.Params,
            FunctionExpression funcExpr => funcExpr.Params,
            ArrowFunctionExpression arrowFunc => arrowFunc.Params,
            _ => null
        };
    }

    /// <summary>
    /// Emits LIR instructions to initialize default parameter values.
    /// For each parameter with a default (AssignmentPattern), emits:
    /// - Load parameter, check if null
    /// - If null, evaluate default expression and store back to parameter
    /// </summary>
    /// <returns>True if all default parameters were successfully lowered, false if any failed (method should fall back to legacy)</returns>
    private bool EmitDefaultParameterInitializers()
    {
        var astParams = GetAstParameters();
        if (astParams == null) return true; // No parameters, success

        var parameters = astParams.Value;
        for (int i = 0; i < parameters.Count; i++)
        {
            if (parameters[i] is not AssignmentPattern ap) continue;
            if (ap.Left is not Identifier paramId) continue;

            // Get the binding for this parameter
            if (!_scope!.Bindings.TryGetValue(paramId.Name, out var binding)) continue;
            if (!_parameterIndexMap.TryGetValue(binding, out var paramIndex)) continue;

            // Convert AST default expression to HIR first to check if we can lower it
            var hirDefaultExpr = ConvertAstToHIRExpression(ap.Right);
            if (hirDefaultExpr == null)
            {
                // Can't convert this default expression - entire method should fall back to legacy
                return false;
            }

            // Record instruction count before we start, so we can roll back if lowering fails
            var instructionCountBefore = _methodBodyIR.Instructions.Count;

            // Emit: load parameter, check if null, if so evaluate default and store back
            var notNullLabel = CreateLabel();

            // Load parameter value
            var paramTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(paramIndex, paramTemp));
            DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Branch if not null (brtrue)
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(paramTemp, notNullLabel));

            // Evaluate default value expression
            if (!TryLowerExpression(hirDefaultExpr, out var defaultValueTemp))
            {
                // If we can't lower the default expression, roll back all instructions
                // and signal that the entire method should fall back to legacy.
                // Note: We only rollback instructions here, not temp variables or labels.
                // This is acceptable because when we return false, the entire MethodBodyIR is discarded
                // and the method falls back to legacy compilation - the orphaned temps/labels are never used.
                while (_methodBodyIR.Instructions.Count > instructionCountBefore)
                {
                    _methodBodyIR.Instructions.RemoveAt(_methodBodyIR.Instructions.Count - 1);
                }
                return false;
            }

            // Ensure the default value is boxed to object
            defaultValueTemp = EnsureObject(defaultValueTemp);

            // Store back to parameter
            _methodBodyIR.Instructions.Add(new LIRStoreParameter(paramIndex, defaultValueTemp));

            // Not-null label
            _methodBodyIR.Instructions.Add(new LIRLabel(notNullLabel));
        }

        return true; // All default parameters successfully lowered
    }

    /// <summary>
    /// Converts an AST Expression to an HIR Expression for lowering.
    /// This is a simplified conversion for default parameter expressions.
    /// </summary>
    private HIRExpression? ConvertAstToHIRExpression(Expression expr)
    {
        return expr switch
        {
            NumericLiteral lit => new HIRLiteralExpression(JavascriptType.Number, lit.Value),
            StringLiteral lit => new HIRLiteralExpression(JavascriptType.String, lit.Value),
            BooleanLiteral lit => new HIRLiteralExpression(JavascriptType.Boolean, lit.Value),
            Literal lit when lit.Value is null => new HIRLiteralExpression(JavascriptType.Null, null),
            Identifier id => ConvertIdentifierToHIRExpression(id),
            BinaryExpression binExpr => ConvertBinaryExpressionToHIR(binExpr),
            _ => null
        };
    }

    private HIRExpression? ConvertIdentifierToHIRExpression(Identifier id)
    {
        if (_scope == null) return null;
        var symbol = _scope.FindSymbol(id.Name);
        return new HIRVariableExpression(symbol);
    }

    private HIRExpression? ConvertBinaryExpressionToHIR(BinaryExpression binExpr)
    {
        var left = ConvertAstToHIRExpression(binExpr.Left);
        var right = ConvertAstToHIRExpression(binExpr.Right);
        if (left == null || right == null) return null;
        return new HIRBinaryExpression(binExpr.Operator, left, right);
    }

    public static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, out MethodBodyIR? lirMethod)
    {
        lirMethod = null;

        // Build EnvironmentLayout for this method if scope is provided
        EnvironmentLayout? environmentLayout = null;
        if (scope != null && scopeMetadataRegistry != null)
        {
            try
            {
                var builder = new EnvironmentLayoutBuilder(scopeMetadataRegistry);
                environmentLayout = builder.Build(scope, CallableKind.Function);
            }
            catch
            {
                // If we can't build environment layout, fall back to legacy
                return false;
            }
        }

        var lowerer = new HIRToLIRLowerer(scope, environmentLayout);

        // If default parameter initialization failed, fall back to legacy emitter
        if (!lowerer._parameterInitSucceeded)
        {
            return false;
        }
        
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
        return TryLower(hirMethod, null, null, out lirMethod);
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

                    // Check if this binding should be stored in a scope field (captured variable)
                    if (_environmentLayout != null)
                    {
                        var storage = _environmentLayout.GetStorage(binding);
                        if (storage != null && storage.Kind == BindingStorageKind.LeafScopeField)
                        {
                            // Captured variable - store to leaf scope field
                            if (!storage.FieldHandle.IsNil && !storage.DeclaringScopeType.IsNil)
                            {
                                lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.FieldHandle, storage.DeclaringScopeType, value));
                                // Also map in SSA for subsequent reads (though they'll use field load)
                                _variableMap[binding] = value;
                                return true;
                            }
                        }
                    }

                    // Non-captured variable - use SSA temp
                    _variableMap[binding] = value;

                    // Assign the declared variable a stable local slot and map this SSA temp to it.
                    // Track the storage type for the variable slot.
                    var storageInfo = GetTempStorage(value);
                    var slot = GetOrCreateVariableSlot(binding, exprStmt.Name.Name, storageInfo);
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
                
                // Check if this binding is stored in a scope field (captured variable)
                if (_environmentLayout != null)
                {
                    var storage = _environmentLayout.GetStorage(binding);
                    if (storage != null)
                    {
                        switch (storage.Kind)
                        {
                            case BindingStorageKind.IlArgument:
                                // Non-captured parameter - use LIRLoadParameter
                                if (storage.JsParameterIndex >= 0)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadParameter(storage.JsParameterIndex, resultTempVar));
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.LeafScopeField:
                                // Captured variable in current scope - load from leaf scope field
                                if (!storage.FieldHandle.IsNil && !storage.DeclaringScopeType.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.FieldHandle, storage.DeclaringScopeType, resultTempVar));
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.ParentScopeField:
                                // Captured variable in parent scope - load from parent scope field
                                if (storage.ParentScopeIndex >= 0 && !storage.FieldHandle.IsNil && !storage.DeclaringScopeType.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.FieldHandle, storage.DeclaringScopeType, storage.ParentScopeIndex, resultTempVar));
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.IlLocal:
                                // Non-captured local - use SSA temp (fall through to default behavior)
                                break;
                        }
                    }
                }
                
                // Fallback: Check if this is a parameter (legacy behavior)
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

        // console.log takes its arguments as an array of type object
        // First, lower all argument expressions to temps
        var argTemps = new List<TempVariable>();
        foreach (var argExpr in callExpr.Arguments)
        {
            if (!TryLowerExpression(argExpr, out var argTempVar))
            {
                return false;
            }

            argTempVar = EnsureObject(argTempVar);
            argTemps.Add(argTempVar);
        }

        // Create the arguments array with all elements in one instruction
        var arrayTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(argTemps, arrayTempVar));
        this.DefineTempStorage(arrayTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

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
        // LIRMulNumber emitted when both operands are known to be double (uses native IL mul instruction).
        // LIRMulDynamic emitted otherwise (calls Operators.Multiply at runtime for type coercion).
        // Type inference could be improved to track numeric types through more expressions to prefer LIRMulNumber.
        if (binaryExpr.Operator == Acornima.Operator.Multiplication)
        {
            // Number * Number - uses native IL mul instruction (optimal path)
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRMulNumber(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }

            // Dynamic multiplication - types unknown at compile time, box operands and call Operators.Multiply
            // This has runtime overhead but handles mixed types correctly (e.g., string to number coercion)
            var leftBoxed = EnsureObject(leftTempVar);
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRMulDynamic(leftBoxed, rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
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