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
    private readonly EnvironmentLayoutBuilder? _environmentLayoutBuilder;

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

    private HIRToLIRLowerer(Scope? scope, EnvironmentLayout? environmentLayout, EnvironmentLayoutBuilder? environmentLayoutBuilder)
    {
        _scope = scope;
        _environmentLayout = environmentLayout;
        _environmentLayoutBuilder = environmentLayoutBuilder;
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
        EnvironmentLayoutBuilder? environmentLayoutBuilder = null;
        if (scope != null && scopeMetadataRegistry != null)
        {
            try
            {
                environmentLayoutBuilder = new EnvironmentLayoutBuilder(scopeMetadataRegistry);
                environmentLayout = environmentLayoutBuilder.Build(scope, CallableKind.Function);
            }
            catch
            {
                // If we can't build environment layout, fall back to legacy
                return false;
            }
        }

        var lowerer = new HIRToLIRLowerer(scope, environmentLayout, environmentLayoutBuilder);

        // If default parameter initialization failed, fall back to legacy emitter
        if (!lowerer._parameterInitSucceeded)
        {
            return false;
        }

        // Emit scope instance creation if there are leaf scope fields
        lowerer.EmitScopeInstanceCreationIfNeeded();
        
        if (lowerer.TryLowerStatements(hirMethod.Body.Statements))
        {
            lirMethod = lowerer._methodBodyIR;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Emits LIRCreateLeafScopeInstance at the start of the method if any bindings
    /// are stored in leaf scope fields.
    /// </summary>
    private void EmitScopeInstanceCreationIfNeeded()
    {
        if (_environmentLayout == null) return;

        // Find the first binding that uses leaf scope field storage
        var leafScopeStorage = _environmentLayout.StorageByBinding.Values
            .FirstOrDefault(s => s.Kind == BindingStorageKind.LeafScopeField && !s.DeclaringScope.IsNil);

        if (leafScopeStorage != null)
        {
            // Found a leaf scope field - emit scope instance creation
            _methodBodyIR.Instructions.Insert(0, new LIRCreateLeafScopeInstance(leafScopeStorage.DeclaringScope));
            
            // Record that we need a scope local in the method
            _methodBodyIR.NeedsLeafScopeLocal = true;
            _methodBodyIR.LeafScopeId = leafScopeStorage.DeclaringScope;
        }
    }

    /// <summary>
    /// Builds a LIRBuildScopesArray instruction for calling a function.
    /// Determines the callee's required scope chain and maps each slot to a source in the caller.
    /// </summary>
    private bool TryBuildScopesArrayForCallee(Symbol calleeSymbol, TempVariable resultTemp)
    {
        // Find the callee's scope from its binding's declaration scope
        var calleeScope = FindCalleeScope(calleeSymbol);
        if (calleeScope == null)
        {
            // Can't determine callee's scope - fall back to empty scopes array
            // This happens for builtin functions or when scope info is unavailable
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
            return true;
        }

        // Check if the callee needs parent scopes
        if (!calleeScope.ReferencesParentScopeVariables)
        {
            // Callee doesn't need parent scopes - emit empty array
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
            return true;
        }

        // Build the callee's environment layout to get its scope chain
        if (_environmentLayoutBuilder == null)
        {
            // No layout builder available - fall back to legacy
            return false;
        }

        EnvironmentLayout calleeLayout;
        try
        {
            calleeLayout = _environmentLayoutBuilder.Build(calleeScope, CallableKind.Function);
        }
        catch
        {
            return false;
        }

        // Map each slot in the callee's scope chain to a source in the caller
        var slotSources = new List<ScopeSlotSource>();
        foreach (var slot in calleeLayout.ScopeChain.Slots)
        {
            if (!TryMapScopeSlotToSource(slot, out var slotSource))
            {
                return false;
            }
            slotSources.Add(slotSource);
        }

        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(slotSources.ToList(), resultTemp));
        return true;
    }

    /// <summary>
    /// Finds the scope associated with a function symbol.
    /// </summary>
    private Scope? FindCalleeScope(Symbol symbol)
    {
        if (_scope == null) return null;

        // Walk up to the root/global scope, since the callee may be declared
        // in an ancestor or sibling scope relative to the current scope.
        var rootScope = _scope;
        while (rootScope.Parent != null)
        {
            rootScope = rootScope.Parent;
        }

        // The function's scope is a child scope of the scope where it's declared
        // We need to find it by looking at child scopes whose AST node matches
        return FindScopeByDeclarationNode(symbol.BindingInfo.DeclarationNode, rootScope);
    }

    /// <summary>
    /// Recursively searches for a scope whose AST node matches the given declaration node.
    /// For function declarations, the scope's AstNode is the FunctionDeclaration itself.
    /// </summary>
    private static Scope? FindScopeByDeclarationNode(Acornima.Ast.Node declarationNode, Scope root)
    {
        // Check if this scope's AST node matches the declaration
        if (root.AstNode == declarationNode)
        {
            return root;
        }

        // Search child scopes
        foreach (var child in root.Children)
        {
            var found = FindScopeByDeclarationNode(declarationNode, child);
            if (found != null) return found;
        }

        return null;
    }

    /// <summary>
    /// Maps a callee scope slot to a source in the caller context.
    /// </summary>
    private bool TryMapScopeSlotToSource(ScopeSlot slot, out ScopeSlotSource slotSource)
    {
        slotSource = default;

        // The caller needs to provide this scope instance to the callee.
        // Determine where the caller can get this scope from:
        // 1. If it's the caller's leaf scope -> LeafLocal (ldloc.0)
        // 2. If it's in the caller's parent scopes -> ScopesArgument (ldarg scopesArg, ldelem.ref)
        // 3. If caller is a class method with _scopes -> ThisScopes (ldarg.0, ldfld _scopes, ldelem.ref)

        // Check if this is the caller's leaf scope
        if (_scope != null && _scope.Name == slot.ScopeName)
        {
            // The caller's own scope instance
            if (_methodBodyIR.NeedsLeafScopeLocal)
            {
                slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.LeafLocal);
                return true;
            }
            // If we don't have a leaf scope local, we can't provide this scope
            return false;
        }

        // Check if this scope is in the caller's environment layout (from parent scopes)
        if (_environmentLayout != null)
        {
            var callerSlotIndex = _environmentLayout.ScopeChain.IndexOf(slot.ScopeName);
            if (callerSlotIndex >= 0)
            {
                // Found in caller's scope chain
                var scopesSource = _environmentLayout.Abi.ScopesSource;
                if (scopesSource == ScopesSource.Argument)
                {
                    slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.ScopesArgument, callerSlotIndex);
                    return true;
                }
                else if (scopesSource == ScopesSource.ThisField)
                {
                    slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.ThisScopes, callerSlotIndex);
                    return true;
                }
            }
        }

        // Check if the caller's scope has a parent with this name
        // This handles the case where the caller is global scope and we need to pass it
        var currentScope = _scope;
        while (currentScope != null)
        {
            // This is an ancestor scope of the caller
            // In global/module main, the leaf scope local holds the global scope instance
            if (currentScope.Name == slot.ScopeName &&
                _methodBodyIR.NeedsLeafScopeLocal &&
                _scope == currentScope)
            {
                slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.LeafLocal);
                return true;
            }
            currentScope = currentScope.Parent;
        }

        // Can't find this scope in the caller's context
        // This might happen for scopes that don't have runtime instances
        // For now, we'll fail - the caller should fall back to legacy
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
                        // Captured variable - store to leaf scope field
                        if (storage != null && 
                            storage.Kind == BindingStorageKind.LeafScopeField &&
                            !storage.Field.IsNil && 
                            !storage.DeclaringScope.IsNil)
                        {
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, value));
                            // Also map in SSA for subsequent reads (though they'll use field load)
                            _variableMap[binding] = value;
                            return true;
                        }
                    }

                    // Non-captured variable - use SSA temp
                    _variableMap[binding] = value;

                    // Assign the declared variable a stable local slot and map this SSA temp to it.
                    // Track the storage type for the variable slot.
                    var storageInfo = GetTempStorage(value);
                    var slot = GetOrCreateVariableSlot(binding, exprStmt.Name.Name, storageInfo);
                    SetTempVariableSlot(value, slot);

                    // Mark all variable slots as single-assignment initially.
                    // This will be removed if the variable is reassigned later.
                    // const variables are always single-assignment by definition.
                    // let/var variables are single-assignment if never reassigned after initialization.
                    _methodBodyIR.SingleAssignmentSlots.Add(slot);
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

                    // If the condition is boxed or is an object reference, we need to
                    // convert it to a boolean using IsTruthy before branching.
                    // This is because brfalse on a boxed boolean checks for null, not false,
                    // and JavaScript has different truthiness rules (0, "", null, undefined, NaN are falsy).
                    var conditionStorage = GetTempStorage(conditionTemp);
                    bool needsTruthyCheck = conditionStorage.Kind == ValueStorageKind.BoxedValue ||
                        (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object));
                    
                    if (needsTruthyCheck)
                    {
                        var isTruthyTemp = CreateTempVariable();
                        lirInstructions.Add(new LIRCallIsTruthy(conditionTemp, isTruthyTemp));
                        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        conditionTemp = isTruthyTemp;
                    }

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
            case HIRForStatement forStmt:
                {
                    // For loop structure:
                    // init
                    // loop_start:
                    //   if (!test) goto end
                    //   body
                    //   update
                    //   goto loop_start
                    // end:

                    // Lower init statement (if present)
                    if (forStmt.Init != null && !TryLowerStatement(forStmt.Init))
                    {
                        return false;
                    }

                    int loopStartLabel = CreateLabel();
                    int loopEndLabel = CreateLabel();

                    // Loop start label
                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    // Test condition (if present)
                    if (forStmt.Test != null)
                    {
                        if (!TryLowerExpression(forStmt.Test, out var conditionTemp))
                        {
                            return false;
                        }

                        // If the condition is boxed or is an object reference, convert to boolean using IsTruthy
                        var conditionStorage = GetTempStorage(conditionTemp);
                        bool needsTruthyCheck = conditionStorage.Kind == ValueStorageKind.BoxedValue ||
                            (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object));
                        
                        if (needsTruthyCheck)
                        {
                            var isTruthyTemp = CreateTempVariable();
                            lirInstructions.Add(new LIRCallIsTruthy(conditionTemp, isTruthyTemp));
                            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                            conditionTemp = isTruthyTemp;
                        }

                        // Branch to end if condition is false
                        lirInstructions.Add(new LIRBranchIfFalse(conditionTemp, loopEndLabel));
                    }

                    // Loop body
                    if (!TryLowerStatement(forStmt.Body))
                    {
                        return false;
                    }

                    // Update expression (if present)
                    if (forStmt.Update != null && !TryLowerExpression(forStmt.Update, out _))
                    {
                        return false;
                    }
                    // Note: Update expression result is discarded (e.g., i++ side effect is what matters)

                    // Jump back to loop start
                    lirInstructions.Add(new LIRBranch(loopStartLabel));

                    // Loop end label
                    lirInstructions.Add(new LIRLabel(loopEndLabel));

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

            case HIRAssignmentExpression assignExpr:
                return TryLowerAssignmentExpression(assignExpr, out resultTempVar);

            case HIRArrayExpression arrayExpr:
                return TryLowerArrayExpression(arrayExpr, out resultTempVar);

            case HIRObjectExpression objectExpr:
                return TryLowerObjectExpression(objectExpr, out resultTempVar);

            case HIRPropertyAccessExpression propAccessExpr:
                return TryLowerPropertyAccessExpression(propAccessExpr, out resultTempVar);

            case HIRIndexAccessExpression indexAccessExpr:
                return TryLowerIndexAccessExpression(indexAccessExpr, out resultTempVar);

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
                                if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, resultTempVar));
                                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.ParentScopeField:
                                // Captured variable in parent scope - load from parent scope field
                                if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, resultTempVar));
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

            // Build the scopes array for the callee
            var scopesTempVar = CreateTempVariable();
            if (!TryBuildScopesArrayForCallee(symbol, scopesTempVar))
            {
                return false;
            }
            DefineTempStorage(scopesTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            // Emit the function call with arguments
            _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, scopesTempVar, arguments, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            return true;
        }

        // Case 2: Property access call (e.g., console.log, Array.isArray, Math.abs)
        if (callExpr.Callee is not HIRPropertyAccessExpression calleePropAccess)
        {
            return false;
        }

        // Case 2a: Typed Array instance method calls (e.g., arr.join(), arr.push(...)).
        // If we can lower the receiver expression and its CLR type is known to be JavaScriptRuntime.Array,
        // emit a general typed instance call.
        if (TryLowerExpression(calleePropAccess.Object, out var arrayReceiverTempVar))
        {
            var receiverStorage = GetTempStorage(arrayReceiverTempVar);
            if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Array))
            {
                var arrayArgTemps = new List<TempVariable>();
                foreach (var argExpr in callExpr.Arguments)
                {
                    if (!TryLowerExpression(argExpr, out var argTempVar))
                    {
                        return false;
                    }
                    arrayArgTemps.Add(EnsureObject(argTempVar));
                }

                _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                    arrayReceiverTempVar,
                    typeof(JavaScriptRuntime.Array),
                    calleePropAccess.PropertyName,
                    arrayArgTemps,
                    resultTempVar));

                // Track a more precise runtime type when we know it, so chained calls can lower.
                // Example: arr.slice(...).join(',') requires the result of slice() to be treated as an Array receiver.
                var returnClrType = string.Equals(calleePropAccess.PropertyName, "slice", StringComparison.OrdinalIgnoreCase)
                    ? typeof(JavaScriptRuntime.Array)
                    : typeof(object);
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, returnClrType));
                return true;
            }
        }

        // Case 2b: Intrinsic static method call (e.g., Array.isArray, Math.abs, JSON.parse)
        // Check if the object is a global variable that maps to an intrinsic type
        if (calleePropAccess.Object is HIRVariableExpression calleeGlobalVar &&
            calleeGlobalVar.Name.Kind == BindingKind.Global)
        {
            var intrinsicName = calleeGlobalVar.Name.Name;
            var methodName = calleePropAccess.PropertyName;

            // Try to resolve the intrinsic type via IntrinsicObjectRegistry
            var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(intrinsicName);
            if (intrinsicType != null)
            {
                // Check if there's a matching static method
                var staticMethods = intrinsicType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Where(mi => string.Equals(mi.Name, methodName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (staticMethods.Count > 0)
                {
                    // Lower all arguments
                    var staticArgTemps = new List<TempVariable>();
                    foreach (var argExpr in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(argExpr, out var argTempVar))
                        {
                            return false;
                        }
                        argTempVar = EnsureObject(argTempVar);
                        staticArgTemps.Add(argTempVar);
                    }

                    // Emit the intrinsic static call
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicName, methodName, staticArgTemps, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    return true;
                }
            }
        }

        // Case 2c: console.log (instance global, not a static intrinsic)
        // This is the legacy hardcoded path for console.log
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

        // Handle subtraction
        if (binaryExpr.Operator == Acornima.Operator.Subtraction)
        {
            // Number - Number - uses native IL sub instruction
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRSubNumber(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            // TODO: Add LIRSubDynamic for unknown types if needed
            return false;
        }

        // Handle division
        if (binaryExpr.Operator == Acornima.Operator.Division)
        {
            // Number / Number - uses native IL div instruction
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRDivNumber(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            // TODO: Add LIRDivDynamic for unknown types if needed
            return false;
        }

        // Handle remainder (modulo)
        if (binaryExpr.Operator == Acornima.Operator.Remainder)
        {
            // Number % Number - uses native IL rem instruction
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRModNumber(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            // TODO: Add LIRModDynamic for unknown types if needed
            return false;
        }

        // Handle exponentiation (** operator)
        if (binaryExpr.Operator == Acornima.Operator.Exponentiation)
        {
            // Number ** Number - calls Math.Pow
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRExpNumber(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            // TODO: Add LIRExpDynamic for unknown types if needed
            return false;
        }

        // Handle bitwise operators
        if (binaryExpr.Operator == Acornima.Operator.BitwiseAnd)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRBitwiseAnd(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        if (binaryExpr.Operator == Acornima.Operator.BitwiseOr)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRBitwiseOr(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        if (binaryExpr.Operator == Acornima.Operator.BitwiseXor)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRBitwiseXor(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        // Handle shift operators
        if (binaryExpr.Operator == Acornima.Operator.LeftShift)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRLeftShift(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        if (binaryExpr.Operator == Acornima.Operator.RightShift)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRRightShift(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        if (binaryExpr.Operator == Acornima.Operator.UnsignedRightShift)
        {
            if (leftType == typeof(double) && rightType == typeof(double))
            {
                _methodBodyIR.Instructions.Add(new LIRUnsignedRightShift(leftTempVar, rightTempVar, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
            }
            return false;
        }

        // Handle logical operators with short-circuit evaluation
        // These need to be decomposed into multiple LIR instructions
        // 
        // IMPORTANT: JavaScript logical operators (&&, ||) return one of the OPERAND VALUES,
        // not a boolean. For example: `1 && "hello"` returns "hello", `0 || "default"` returns "default".
        // This is why we store the result as BoxedValue (the actual operand) rather than UnboxedValue bool.
        // If the result is then used in a boolean context (like an if-statement condition), a separate
        // IsTruthy check will be applied - this is correct JS semantics, not redundant.
        if (binaryExpr.Operator == Acornima.Operator.LogicalAnd)
        {
            // Logical AND: if left is falsy, return left, otherwise return right
            // We need to evaluate left first, then conditionally evaluate right
            
            int falsyLabel = CreateLabel();
            int endLabel = CreateLabel();
            
            // Ensure left is boxed for truthiness check
            var leftBoxed = EnsureObject(leftTempVar);
            
            // Create temp to hold IsTruthy result (bool) - used only for branching
            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            
            // If left is falsy, branch to falsyLabel
            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(isTruthyTemp, falsyLabel));
            
            // Left was truthy, result is right (the actual VALUE, not boolean)
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
            
            // Falsy label: result is left (the actual VALUE, not boolean)
            _methodBodyIR.Instructions.Add(new LIRLabel(falsyLabel));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(leftBoxed, resultTempVar));
            
            // End label
            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            
            return true;
        }

        if (binaryExpr.Operator == Acornima.Operator.LogicalOr)
        {
            // Logical OR: if left is truthy, return left, otherwise return right
            
            int truthyLabel = CreateLabel();
            int endLabel = CreateLabel();
            
            // Ensure left is boxed for truthiness check
            var leftBoxed = EnsureObject(leftTempVar);
            
            // Create temp to hold IsTruthy result (bool) - used only for branching
            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(leftBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            
            // If left is truthy, branch to truthyLabel
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(isTruthyTemp, truthyLabel));
            
            // Left was falsy, result is right (the actual VALUE, not boolean)
            var rightBoxed = EnsureObject(rightTempVar);
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(rightBoxed, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
            
            // Truthy label: result is left (the actual VALUE, not boolean)
            _methodBodyIR.Instructions.Add(new LIRLabel(truthyLabel));
            _methodBodyIR.Instructions.Add(new LIRCopyTemp(leftBoxed, resultTempVar));
            
            // End label
            _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
            
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

    private bool TryLowerUpdateExpression(HIRUpdateExpression updateExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // Only support ++/-- on identifiers
        if (updateExpr.Operator != Acornima.Operator.Increment && updateExpr.Operator != Acornima.Operator.Decrement)
        {
            return false;
        }

        if (updateExpr.Argument is not HIRVariableExpression updateVarExpr)
        {
            return false;
        }

        var updateBinding = updateVarExpr.Name.BindingInfo;
        var isIncrement = updateExpr.Operator == Acornima.Operator.Increment;

        // Load the current value (handles both captured and non-captured variables)
        if (!TryLoadVariable(updateBinding, out var currentValue))
        {
            return false;
        }

        // For captured variables loaded from scope fields, the storage is always object (Reference).
        // For non-captured numeric locals, we only support double.
        var currentStorage = GetTempStorage(currentValue);
        
        // If the variable is from a scope field (Reference type), we need to handle it differently
        // as it comes boxed as object. For now, we only support numeric updates on locals.
        // TODO: Support dynamic increment/decrement for captured variables
        if (currentStorage.Kind == ValueStorageKind.Reference && currentStorage.ClrType == typeof(object))
        {
            // Captured variable - bail out for now, needs runtime support
            // In the future: call a runtime helper to increment/decrement
            return false;
        }

        // Only support numeric locals (double) for now
        if (currentStorage.ClrType != typeof(double))
        {
            return false;
        }

        // Get or create a variable slot for this non-captured variable
        // Note: Captured variables are rejected earlier (Reference/object check), so we only reach here
        // for IlLocal bindings or when there's no environment layout
        var slot = GetOrCreateVariableSlot(updateBinding, updateVarExpr.Name.Name, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

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

        // Store back to the appropriate location.
        // Note: Captured variables (LeafScopeField, ParentScopeField) are rejected earlier at line ~877
        // because they load as Reference/object type. Only IlLocal and no-environment-layout cases reach here.
        SetTempVariableSlot(updatedTemp, slot);
        _variableMap[updateBinding] = updatedTemp;

        // Update expressions (++/--) are reassignments, so the variable is not single-assignment.
        // Remove it from the single-assignment set to prevent incorrect inlining.
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);

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

    private bool TryLowerAssignmentExpression(HIRAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var binding = assignExpr.Target.BindingInfo;
        var lirInstructions = _methodBodyIR.Instructions;

        // For compound assignment (+=, -=, etc.), we need to load the current value first
        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            // Simple assignment: x = expr
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: x += expr, x -= expr, etc.
            // First, load the current value of the variable
            TempVariable currentValue;
            if (!TryLoadVariable(binding, out currentValue))
            {
                return false;
            }

            // Lower the RHS expression
            if (!TryLowerExpression(assignExpr.Value, out var rhsValue))
            {
                return false;
            }

            // Perform the compound operation
            if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhsValue, out valueToStore))
            {
                return false;
            }
        }

        // Store the value to the appropriate location
        // Check if this binding should be stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.LeafScopeField:
                        // Captured variable in current scope - store to leaf scope field
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
                            // Also update SSA map for subsequent reads
                            _variableMap[binding] = boxedValue;
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope - store to parent scope field
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, boxedValue));
                            // Also update SSA map for subsequent reads, mirroring leaf-scope behavior
                            _variableMap[binding] = boxedValue;
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlArgument:
                        // Storing to a parameter
                        if (storage.JsParameterIndex >= 0)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParameter(storage.JsParameterIndex, boxedValue));
                            resultTempVar = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - use SSA temp (fall through to default behavior)
                        break;
                }
            }
        }

        // Check parameter index map for parameters (fallback)
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreParameter(paramIndex, boxedValue));
            resultTempVar = boxedValue;
            return true;
        }

        // Non-captured local variable - update SSA map
        // Get or create a variable slot for this binding
        var storageInfo = GetTempStorage(valueToStore);
        var slot = GetOrCreateVariableSlot(binding, assignExpr.Target.Name, storageInfo);
        SetTempVariableSlot(valueToStore, slot);
        _variableMap[binding] = valueToStore;
        resultTempVar = valueToStore;

        // This is a reassignment (not initial declaration), so the variable is not single-assignment.
        // Remove it from the single-assignment set to prevent incorrect inlining.
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);
        return true;
    }

    private bool TryLowerArrayExpression(HIRArrayExpression arrayExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Check if there are any spread elements
        bool hasSpreadElements = arrayExpr.Elements.Any(e => e is HIRSpreadElement);

        if (!hasSpreadElements)
        {
            // Simple case: no spread elements, use LIRNewJsArray
            var elementTemps = new List<TempVariable>();
            foreach (var element in arrayExpr.Elements)
            {
                if (!TryLowerExpression(element, out var elementTemp))
                {
                    return false;
                }
                // Ensure each element is boxed as object for the array
                elementTemps.Add(EnsureObject(elementTemp));
            }

            // Emit the LIRNewJsArray instruction
            _methodBodyIR.Instructions.Add(new LIRNewJsArray(elementTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));
            return true;
        }

        // Complex case: has spread elements
        // First, collect all non-spread elements for initial capacity hint
        // Then emit array creation + individual Add/PushRange calls

        // Create the array with capacity 1 (minimum - will grow as needed)
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(Array.Empty<TempVariable>(), resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // Process each element
        foreach (var element in arrayExpr.Elements)
        {
            if (element is HIRSpreadElement spreadElement)
            {
                // Lower the spread argument
                if (!TryLowerExpression(spreadElement.Argument, out var spreadArgTemp))
                {
                    return false;
                }
                var boxedSpreadArg = EnsureObject(spreadArgTemp);
                // Emit PushRange to spread the elements
                _methodBodyIR.Instructions.Add(new LIRArrayPushRange(resultTempVar, boxedSpreadArg));
            }
            else
            {
                // Lower regular element
                if (!TryLowerExpression(element, out var elementTemp))
                {
                    return false;
                }
                var boxedElement = EnsureObject(elementTemp);
                // Emit Add for single element
                _methodBodyIR.Instructions.Add(new LIRArrayAdd(resultTempVar, boxedElement));
            }
        }

        return true;
    }

    private bool TryLowerObjectExpression(HIRObjectExpression objectExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Lower all property values first
        var properties = new List<ObjectProperty>();
        foreach (var prop in objectExpr.Properties)
        {
            if (!TryLowerExpression(prop.Value, out var valueTemp))
            {
                return false;
            }
            // Ensure each value is boxed as object for the dictionary
            var boxedValue = EnsureObject(valueTemp);
            properties.Add(new ObjectProperty(prop.Key, boxedValue));
        }

        // Emit the LIRNewJsObject instruction
        _methodBodyIR.Instructions.Add(new LIRNewJsObject(properties, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(System.Dynamic.ExpandoObject)));
        return true;
    }

    private bool TryLowerPropertyAccessExpression(HIRPropertyAccessExpression propAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Lower the object expression
        if (!TryLowerExpression(propAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        // Currently we only support the 'length' property
        if (propAccessExpr.PropertyName == "length")
        {
            var boxedObject = EnsureObject(objectTemp);
            _methodBodyIR.Instructions.Add(new LIRGetLength(boxedObject, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Unsupported property access
        return false;
    }

    private bool TryLowerIndexAccessExpression(HIRIndexAccessExpression indexAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Lower the object expression
        if (!TryLowerExpression(indexAccessExpr.Object, out var objectTemp))
        {
            return false;
        }

        // Lower the index expression
        if (!TryLowerExpression(indexAccessExpr.Index, out var indexTemp))
        {
            return false;
        }

        var boxedObject = EnsureObject(objectTemp);
        var boxedIndex = EnsureObject(indexTemp);
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, boxedIndex, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        return true;
    }

    /// <summary>
    /// Loads the current value of a variable, handling both captured and non-captured variables.
    /// </summary>
    private bool TryLoadVariable(BindingInfo binding, out TempVariable result)
    {
        result = default;

        // Check if this binding is stored in a scope field (captured variable)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.IlArgument:
                        // Non-captured parameter
                        if (storage.JsParameterIndex >= 0)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadParameter(storage.JsParameterIndex, result));
                            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            return true;
                        }
                        break;

                    case BindingStorageKind.LeafScopeField:
                        // Captured variable in current scope
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadLeafScopeField(binding, storage.Field, storage.DeclaringScope, result));
                            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, result));
                            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - use SSA map
                        break;
                }
            }
        }

        // Fallback: Check parameter index map
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            result = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(paramIndex, result));
            DefineTempStorage(result, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Non-captured local: look up in SSA map
        if (_variableMap.TryGetValue(binding, out result))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs a compound operation (+=, -=, *=, etc.) on two operands.
    /// </summary>
    private bool TryLowerCompoundOperation(Acornima.Operator op, TempVariable currentValue, TempVariable rhsValue, out TempVariable result)
    {
        result = CreateTempVariable();

        var leftType = GetTempStorage(currentValue).ClrType;
        var rightType = GetTempStorage(rhsValue).ClrType;

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
                // Dynamic addition
                var leftBoxed = EnsureObject(currentValue);
                var rightBoxed = EnsureObject(rhsValue);
                _methodBodyIR.Instructions.Add(new LIRAddDynamic(leftBoxed, rightBoxed, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;

            case Acornima.Operator.SubtractionAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRSubNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Subtraction requires numeric types
                return false;

            case Acornima.Operator.MultiplicationAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRMulNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Dynamic multiplication
                var leftMulBoxed = EnsureObject(currentValue);
                var rightMulBoxed = EnsureObject(rhsValue);
                _methodBodyIR.Instructions.Add(new LIRMulDynamic(leftMulBoxed, rightMulBoxed, result));
                DefineTempStorage(result, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;

            case Acornima.Operator.DivisionAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRDivNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Division requires numeric types
                return false;

            case Acornima.Operator.RemainderAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRModNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Remainder requires numeric types
                return false;

            case Acornima.Operator.ExponentiationAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRExpNumber(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Exponentiation requires numeric types
                return false;

            case Acornima.Operator.BitwiseAndAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseAnd(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise AND requires numeric types
                return false;

            case Acornima.Operator.BitwiseOrAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseOr(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise OR requires numeric types
                return false;

            case Acornima.Operator.BitwiseXorAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseXor(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise XOR requires numeric types
                return false;

            case Acornima.Operator.LeftShiftAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRLeftShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Left shift requires numeric types
                return false;

            case Acornima.Operator.RightShiftAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRRightShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Right shift requires numeric types
                return false;

            case Acornima.Operator.UnsignedRightShiftAssignment:
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRUnsignedRightShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Unsigned right shift requires numeric types
                return false;

            default:
                return false;
        }
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