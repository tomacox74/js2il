using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using Js2IL.Utilities;
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

    private readonly Stack<ControlFlowContext> _controlFlowStack = new();

    private readonly Stack<int> _protectedControlFlowDepthStack = new();
    private int? _returnEpilogueReturnSlot;
    private TempVariable? _returnEpilogueLoadTemp;
    private bool _needsReturnEpilogueBlock;

    private readonly struct ControlFlowContext
    {
        public ControlFlowContext(int breakLabel, int? continueLabel, string? labelName)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
            LabelName = labelName;
        }

        public int BreakLabel { get; }
        public int? ContinueLabel { get; }
        public string? LabelName { get; }
    }

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

        // For captured identifier parameters, initialize the corresponding leaf-scope fields.
        // This must happen after default parameter initialization so the final value is stored.
        // Without this, nested functions reading captured parameters will observe null.
        if (_parameterInitSucceeded)
        {
            EmitCapturedParameterFieldInitializers();
        }
    }

    private void EmitCapturedParameterFieldInitializers()
    {
        if (_scope == null || _environmentLayout == null) return;

        foreach (var (binding, jsParamIndex) in _parameterIndexMap)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage == null) continue;

            if (storage.Kind == BindingStorageKind.LeafScopeField && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
            {
                // Load the parameter (object) and store to leaf scope field.
                var paramTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadParameter(jsParamIndex, paramTemp));
                DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                var boxed = EnsureObject(paramTemp);
                _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxed));
            }
        }
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

    public static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind callableKind, out MethodBodyIR? lirMethod)
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
                environmentLayout = environmentLayoutBuilder.Build(scope, callableKind);
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
            // If a return epilogue is needed (for try/finally), emit it at the end.
            if (lowerer._needsReturnEpilogueBlock && lowerer._methodBodyIR.ReturnEpilogueLabelId.HasValue)
            {
                // Ensure epilogue storage exists and then emit: label + return <slotValue>
                lowerer.EnsureReturnEpilogueStorage();
                lowerer._methodBodyIR.Instructions.Add(new LIRLabel(lowerer._methodBodyIR.ReturnEpilogueLabelId.Value));
                if (!lowerer._returnEpilogueLoadTemp.HasValue)
                {
                    return false;
                }
                lowerer._methodBodyIR.Instructions.Add(new LIRReturn(lowerer._returnEpilogueLoadTemp.Value));
            }

            lirMethod = lowerer._methodBodyIR;
            return true;
        }

        return false;
    }

    // Backward compatibility overload for callers that don't provide callable kind
    public static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, out MethodBodyIR? lirMethod)
    {
        return TryLower(hirMethod, scope, scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind.Function, out lirMethod);
    }

    /// <summary>
    /// Emits LIRCreateLeafScopeInstance at the start of the method if any bindings
    /// are stored in leaf scope fields.
    /// </summary>
    private void EmitScopeInstanceCreationIfNeeded()
    {
        if (_environmentLayout == null) return;

        // Special-case: the global/module scope instance is needed to satisfy the ABI expectation
        // that scopes[0] is the module/global scope when lowering direct user-defined calls from the
        // entry point. Even if the global scope has no leaf fields, creating the instance is cheap
        // and enables IR lowering to build scopes arrays without falling back.
        if (_scope != null && _scope.Kind == ScopeKind.Global && !_methodBodyIR.NeedsLeafScopeLocal)
        {
            EnsureLeafScopeInstance(new ScopeId(_scope.Name));
            return;
        }

        // Find the first binding that uses leaf scope field storage
        var leafScopeStorage = _environmentLayout.StorageByBinding.Values
            .FirstOrDefault(s => s.Kind == BindingStorageKind.LeafScopeField && !s.DeclaringScope.IsNil);

        if (leafScopeStorage != null)
        {
            // Found a leaf scope field - ensure scope instance creation
            EnsureLeafScopeInstance(leafScopeStorage.DeclaringScope);
        }
    }

    private bool EnsureLeafScopeInstance(ScopeId scopeId)
    {
        if (_methodBodyIR.NeedsLeafScopeLocal)
        {
            // If we already decided on a leaf scope id, it must match.
            if (!_methodBodyIR.LeafScopeId.IsNil && _methodBodyIR.LeafScopeId != scopeId)
            {
                return false;
            }
            if (_methodBodyIR.LeafScopeId.IsNil)
            {
                _methodBodyIR.LeafScopeId = scopeId;
            }
            return true;
        }

        _methodBodyIR.Instructions.Insert(0, new LIRCreateLeafScopeInstance(scopeId));
        _methodBodyIR.NeedsLeafScopeLocal = true;
        _methodBodyIR.LeafScopeId = scopeId;
        return true;
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

        // Even if the callee doesn't directly reference parent scope variables, it may need the global
        // scope to correctly construct closures for nested functions that capture globals.
        // To preserve the historic ABI expectation that scopes[0] is the global scope (when available),
        // ensure we at least pass the global scope slot.
        if (!calleeScope.ReferencesParentScopeVariables)
        {
            if (_scope == null)
            {
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
                return true;
            }

            // Global slot is always index 0 and uses the module name.
            var root = _scope;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            var moduleName = root.Name;
            var globalSlot = new ScopeSlot(Index: 0, ScopeName: moduleName, ScopeId: new ScopeId(moduleName));
            if (!TryMapScopeSlotToSource(globalSlot, out var globalSlotSource))
            {
                return false;
            }
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(new[] { globalSlotSource }, resultTemp));
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

    private bool TryBuildScopesArrayForClassConstructor(Scope classScope, TempVariable resultTemp)
    {
        if (_environmentLayoutBuilder == null)
        {
            return false;
        }

        EnvironmentLayout calleeLayout;
        try
        {
            calleeLayout = _environmentLayoutBuilder.Build(classScope, CallableKind.Constructor);
        }
        catch
        {
            return false;
        }

        var slotSources = new List<ScopeSlotSource>();
        foreach (var slot in calleeLayout.ScopeChain.Slots)
        {
            if (!TryMapScopeSlotToSource(slot, out var slotSource))
            {
                return false;
            }
            slotSources.Add(slotSource);
        }

        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(slotSources, resultTemp));
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
        if (_scope != null && ScopeNaming.GetRegistryScopeName(_scope) == slot.ScopeName)
        {
            // The caller's own scope instance
            if (!EnsureLeafScopeInstance(slot.ScopeId))
            {
                return false;
            }
            slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.LeafLocal);
            return true;
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
                            // Scope fields are always object-typed; box value types before storing.
                            var boxedValue = EnsureObject(value);
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
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

                    // If we are inside a protected region with a finally handler, we must use leave
                    // so finally runs before returning.
                    if (_protectedControlFlowDepthStack.Count > 0 && _methodBodyIR.ReturnEpilogueLabelId.HasValue)
                    {
                        if (!TryEmitReturnViaEpilogue(returnTempVar))
                        {
                            return false;
                        }
                        return true;
                    }

                    lirInstructions.Add(new LIRReturn(returnTempVar));
                    return true;
                }
            case HIRLabeledStatement labeledStmt:
                {
                    var endLabel = CreateLabel();
                    _controlFlowStack.Push(new ControlFlowContext(endLabel, null, labeledStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(labeledStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    lirInstructions.Add(new LIRLabel(endLabel));
                    return true;
                }
            case HIRSwitchStatement switchStmt:
                {
                    if (!TryLowerExpression(switchStmt.Discriminant, out var discriminantTemp))
                    {
                        return false;
                    }

                    discriminantTemp = EnsureObject(discriminantTemp);

                    var endLabel = CreateLabel();
                    _controlFlowStack.Push(new ControlFlowContext(endLabel, null, null));

                    try
                    {
                        // Create a label for each case start.
                        var caseLabels = new int[switchStmt.Cases.Length];
                        for (int i = 0; i < caseLabels.Length; i++)
                        {
                            caseLabels[i] = CreateLabel();
                        }

                        int? defaultCaseIndex = null;
                        for (int i = 0; i < switchStmt.Cases.Length; i++)
                        {
                            if (switchStmt.Cases[i].Test == null)
                            {
                                defaultCaseIndex = i;
                                break;
                            }
                        }

                        // Dispatch: compare discriminant === caseTest in order.
                        for (int i = 0; i < switchStmt.Cases.Length; i++)
                        {
                            var sc = switchStmt.Cases[i];
                            if (sc.Test == null)
                            {
                                continue;
                            }

                            if (!TryLowerExpression(sc.Test, out var testTemp))
                            {
                                return false;
                            }

                            testTemp = EnsureObject(testTemp);
                            var cmpTemp = CreateTempVariable();
                            lirInstructions.Add(new LIRStrictEqualDynamic(discriminantTemp, testTemp, cmpTemp));
                            DefineTempStorage(cmpTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                            lirInstructions.Add(new LIRBranchIfTrue(cmpTemp, caseLabels[i]));
                        }

                        // No match: jump to default if present, else end.
                        lirInstructions.Add(new LIRBranch(defaultCaseIndex.HasValue ? caseLabels[defaultCaseIndex.Value] : endLabel));

                        // Emit case bodies in order; fallthrough is natural.
                        for (int i = 0; i < switchStmt.Cases.Length; i++)
                        {
                            lirInstructions.Add(new LIRLabel(caseLabels[i]));
                            foreach (var cons in switchStmt.Cases[i].Consequent)
                            {
                                if (!TryLowerStatement(cons))
                                {
                                    return false;
                                }
                            }
                        }

                        // End of switch.
                        lirInstructions.Add(new LIRLabel(endLabel));
                        return true;
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }
                }
            case HIRTryStatement tryStmt:
                {
                    var hasCatch = tryStmt.CatchBody != null;
                    var hasFinally = tryStmt.FinallyBody != null;
                    if (!hasCatch && !hasFinally)
                    {
                        return TryLowerStatement(tryStmt.TryBlock);
                    }

                    // Track current control-flow depth so we can decide when break/continue exits the try.
                    _protectedControlFlowDepthStack.Push(_controlFlowStack.Count);

                    // Any return inside a protected region must use 'leave' to an epilogue outside the region.
                    if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
                    {
                        _methodBodyIR.ReturnEpilogueLabelId = CreateLabel();
                    }

                    try
                    {
                        var outerTryStart = CreateLabel();
                        var outerTryEnd = CreateLabel();
                        var endLabel = CreateLabel();

                        int innerTryStart = outerTryStart;
                        int innerTryEnd = outerTryEnd;

                        int catchStart = 0;
                        int catchEnd = 0;
                        if (hasCatch)
                        {
                            innerTryStart = CreateLabel();
                            innerTryEnd = CreateLabel();
                            catchStart = CreateLabel();
                            catchEnd = CreateLabel();
                        }

                        int finallyStart = 0;
                        int finallyEnd = 0;
                        if (hasFinally)
                        {
                            finallyStart = CreateLabel();
                            finallyEnd = CreateLabel();
                        }

                        // Outer try label (used for finally when present)
                        lirInstructions.Add(new LIRLabel(outerTryStart));

                        // Inner try/catch (if catch present) or direct try.
                        if (hasCatch)
                        {
                            lirInstructions.Add(new LIRLabel(innerTryStart));
                        }

                        if (!TryLowerStatement(tryStmt.TryBlock))
                        {
                            return false;
                        }

                        lirInstructions.Add(new LIRLeave(endLabel));

                        if (hasCatch)
                        {
                            lirInstructions.Add(new LIRLabel(innerTryEnd));
                            lirInstructions.Add(new LIRLabel(catchStart));

                            // Catch handler starts with the exception object on the stack.
                            var exTemp = CreateTempVariable();
                            lirInstructions.Add(new LIRStoreException(exTemp));
                            DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(System.Exception)));
                            SetTempVariableSlot(exTemp, CreateAnonymousVariableSlot("$catch_ex", new ValueStorage(ValueStorageKind.Reference, typeof(System.Exception))));

                            if (tryStmt.CatchParamBinding != null)
                            {
                                var jsCatchValue = CreateTempVariable();
                                lirInstructions.Add(new LIRUnwrapCatchException(exTemp, jsCatchValue));
                                DefineTempStorage(jsCatchValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                SetTempVariableSlot(jsCatchValue, CreateAnonymousVariableSlot("$catch_value", new ValueStorage(ValueStorageKind.Reference, typeof(object))));

                                if (!TryStoreToBinding(tryStmt.CatchParamBinding, jsCatchValue, out _))
                                {
                                    return false;
                                }
                            }

                            if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
                            {
                                return false;
                            }

                            lirInstructions.Add(new LIRLeave(endLabel));
                            lirInstructions.Add(new LIRLabel(catchEnd));
                        }

                        lirInstructions.Add(new LIRLabel(outerTryEnd));

                        if (hasFinally)
                        {
                            lirInstructions.Add(new LIRLabel(finallyStart));
                            if (tryStmt.FinallyBody != null && !TryLowerStatement(tryStmt.FinallyBody))
                            {
                                return false;
                            }
                            lirInstructions.Add(new LIREndFinally());
                            lirInstructions.Add(new LIRLabel(finallyEnd));
                        }

                        lirInstructions.Add(new LIRLabel(endLabel));

                        // Register EH regions.
                        if (hasCatch)
                        {
                            _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
                                ExceptionRegionKind.Catch,
                                TryStartLabelId: innerTryStart,
                                TryEndLabelId: innerTryEnd,
                                HandlerStartLabelId: catchStart,
                                HandlerEndLabelId: catchEnd,
                                CatchType: typeof(System.Exception)));
                        }

                        if (hasFinally)
                        {
                            _methodBodyIR.ExceptionRegions.Add(new ExceptionRegionInfo(
                                ExceptionRegionKind.Finally,
                                TryStartLabelId: outerTryStart,
                                TryEndLabelId: outerTryEnd,
                                HandlerStartLabelId: finallyStart,
                                HandlerEndLabelId: finallyEnd));
                        }

                        return true;
                    }
                    finally
                    {
                        _protectedControlFlowDepthStack.Pop();
                    }
                }
            case HIRThrowStatement throwStmt:
                {
                    if (!TryLowerExpression(throwStmt.Argument, out var argTemp))
                    {
                        return false;
                    }

                    argTemp = EnsureObject(argTemp);
                    lirInstructions.Add(new LIRThrow(argTemp));
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
                    int loopUpdateLabel = CreateLabel();
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
                    _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopUpdateLabel, forStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(forStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    // Continue target (for-loops continue runs update, then loops)
                    lirInstructions.Add(new LIRLabel(loopUpdateLabel));

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

            case Js2IL.HIR.HIRForOfStatement forOfStmt:
                {
                    // Desugar for..of:
                    // iter = Object.NormalizeForOfIterable(rhs)
                    // len = iter.length
                    // idx = 0
                    // loop_start:
                    //   if (!(idx < len)) goto end
                    //   target = iter[idx]
                    //   body
                    // loop_update:
                    //   idx = idx + 1
                    //   goto loop_start
                    // end:

                    if (!TryLowerExpression(forOfStmt.Iterable, out var rhsTemp))
                    {
                        return false;
                    }

                    var rhsBoxed = EnsureObject(rhsTemp);

                    var iterTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic("Object", "NormalizeForOfIterable", new[] { rhsBoxed }, iterTemp));
                    DefineTempStorage(iterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    // NOTE: temp-local allocation is linear and does not account for loop back-edges.
                    // Pin loop-carry temps to stable variable slots so values remain correct across iterations.
                    SetTempVariableSlot(iterTemp, CreateAnonymousVariableSlot("$forOf_iter", new ValueStorage(ValueStorageKind.Reference, typeof(object))));

                    var lenTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRGetLength(iterTemp, lenTemp));
                    DefineTempStorage(lenTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    SetTempVariableSlot(lenTemp, CreateAnonymousVariableSlot("$forOf_len", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

                    var idxTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRConstNumber(0.0, idxTemp));
                    DefineTempStorage(idxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    SetTempVariableSlot(idxTemp, CreateAnonymousVariableSlot("$forOf_idx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

                    int loopStartLabel = CreateLabel();
                    int loopUpdateLabel = CreateLabel();
                    int loopEndLabel = CreateLabel();

                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    var condTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCompareNumberLessThan(idxTemp, lenTemp, condTemp));
                    DefineTempStorage(condTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    lirInstructions.Add(new LIRBranchIfFalse(condTemp, loopEndLabel));

                    // target = iter[idx]
                    var itemTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRGetItem(EnsureObject(iterTemp), EnsureObject(idxTemp), itemTemp));
                    DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    if (!TryStoreToBinding(forOfStmt.Target.BindingInfo, itemTemp, out _))
                    {
                        return false;
                    }

                    _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopUpdateLabel, forOfStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(forOfStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    lirInstructions.Add(new LIRLabel(loopUpdateLabel));
                    var oneTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRConstNumber(1.0, oneTemp));
                    DefineTempStorage(oneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    var updatedIdx = CreateTempVariable();
                    lirInstructions.Add(new LIRAddNumber(idxTemp, oneTemp, updatedIdx));
                    DefineTempStorage(updatedIdx, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    lirInstructions.Add(new LIRCopyTemp(updatedIdx, idxTemp));
                    lirInstructions.Add(new LIRBranch(loopStartLabel));
                    lirInstructions.Add(new LIRLabel(loopEndLabel));
                    return true;
                }

            case Js2IL.HIR.HIRForInStatement forInStmt:
                {
                    // Desugar for..in:
                    // keys = Object.GetEnumerableKeys(rhs)
                    // len = keys.length
                    // idx = 0
                    // loop_start:
                    //   if (!(idx < len)) goto end
                    //   target = keys[idx]
                    //   body
                    // loop_update:
                    //   idx = idx + 1
                    //   goto loop_start
                    // end:

                    if (!TryLowerExpression(forInStmt.Enumerable, out var rhsTemp))
                    {
                        return false;
                    }

                    var rhsBoxed = EnsureObject(rhsTemp);

                    var keysTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCallIntrinsicStatic("Object", "GetEnumerableKeys", new[] { rhsBoxed }, keysTemp));
                    DefineTempStorage(keysTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    // Pin loop-carry temps to stable variable slots (see note in for..of lowering).
                    SetTempVariableSlot(keysTemp, CreateAnonymousVariableSlot("$forIn_keys", new ValueStorage(ValueStorageKind.Reference, typeof(object))));

                    var lenTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRGetLength(keysTemp, lenTemp));
                    DefineTempStorage(lenTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    SetTempVariableSlot(lenTemp, CreateAnonymousVariableSlot("$forIn_len", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

                    var idxTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRConstNumber(0.0, idxTemp));
                    DefineTempStorage(idxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    SetTempVariableSlot(idxTemp, CreateAnonymousVariableSlot("$forIn_idx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

                    int loopStartLabel = CreateLabel();
                    int loopUpdateLabel = CreateLabel();
                    int loopEndLabel = CreateLabel();

                    lirInstructions.Add(new LIRLabel(loopStartLabel));
                    var condTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRCompareNumberLessThan(idxTemp, lenTemp, condTemp));
                    DefineTempStorage(condTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    lirInstructions.Add(new LIRBranchIfFalse(condTemp, loopEndLabel));

                    var keyTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRGetItem(EnsureObject(keysTemp), EnsureObject(idxTemp), keyTemp));
                    DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    if (!TryStoreToBinding(forInStmt.Target.BindingInfo, keyTemp, out _))
                    {
                        return false;
                    }

                    _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopUpdateLabel, forInStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(forInStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    lirInstructions.Add(new LIRLabel(loopUpdateLabel));
                    var oneTemp = CreateTempVariable();
                    lirInstructions.Add(new LIRConstNumber(1.0, oneTemp));
                    DefineTempStorage(oneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    var updatedIdx = CreateTempVariable();
                    lirInstructions.Add(new LIRAddNumber(idxTemp, oneTemp, updatedIdx));
                    DefineTempStorage(updatedIdx, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    lirInstructions.Add(new LIRCopyTemp(updatedIdx, idxTemp));
                    lirInstructions.Add(new LIRBranch(loopStartLabel));
                    lirInstructions.Add(new LIRLabel(loopEndLabel));
                    return true;
                }
            case HIRWhileStatement whileStmt:
                {
                    // While loop structure:
                    // loop_start:
                    //   if (!test) goto end
                    //   body
                    //   goto loop_start
                    // end:

                    int loopStartLabel = CreateLabel();
                    int loopEndLabel = CreateLabel();

                    // Loop start label
                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    // Test condition
                    if (!TryLowerExpression(whileStmt.Test, out var conditionTemp))
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

                    // Loop body
                    _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopStartLabel, whileStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(whileStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    // Jump back to loop start
                    lirInstructions.Add(new LIRBranch(loopStartLabel));

                    // Loop end label
                    lirInstructions.Add(new LIRLabel(loopEndLabel));

                    return true;
                }
            case HIRDoWhileStatement doWhileStmt:
                {
                    // Do/while loop structure:
                    // loop_start:
                    //   body
                    // loop_test:
                    //   if (!test) goto end
                    //   goto loop_start
                    // end:

                    int loopStartLabel = CreateLabel();
                    int loopTestLabel = CreateLabel();
                    int loopEndLabel = CreateLabel();

                    // Loop start label
                    lirInstructions.Add(new LIRLabel(loopStartLabel));

                    // Loop body (always executes at least once)
                    _controlFlowStack.Push(new ControlFlowContext(loopEndLabel, loopTestLabel, doWhileStmt.Label));
                    try
                    {
                        if (!TryLowerStatement(doWhileStmt.Body))
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        _controlFlowStack.Pop();
                    }

                    // Continue target (do/while continue should skip remainder of body and go to test)
                    lirInstructions.Add(new LIRLabel(loopTestLabel));

                    // Test condition
                    if (!TryLowerExpression(doWhileStmt.Test, out var conditionTemp))
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

                    // Jump back to loop start
                    lirInstructions.Add(new LIRBranch(loopStartLabel));

                    // Loop end label
                    lirInstructions.Add(new LIRLabel(loopEndLabel));

                    return true;
                }
            case HIRBreakStatement breakStmt:
                {
                    if (!TryResolveControlFlowTarget(breakStmt.Label, out var target, out var matchedAbsoluteIndex, isBreak: true))
                    {
                        return false;
                    }

                    if (_protectedControlFlowDepthStack.Count > 0 && matchedAbsoluteIndex < _protectedControlFlowDepthStack.Peek())
                    {
                        lirInstructions.Add(new LIRLeave(target));
                    }
                    else
                    {
                        lirInstructions.Add(new LIRBranch(target));
                    }
                    return true;
                }
            case HIRContinueStatement continueStmt:
                {
                    if (!TryResolveControlFlowTarget(continueStmt.Label, out var target, out var matchedAbsoluteIndex, isBreak: false))
                    {
                        return false;
                    }

                    if (_protectedControlFlowDepthStack.Count > 0 && matchedAbsoluteIndex < _protectedControlFlowDepthStack.Peek())
                    {
                        lirInstructions.Add(new LIRLeave(target));
                    }
                    else
                    {
                        lirInstructions.Add(new LIRBranch(target));
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

    private bool TryResolveControlFlowTarget(string? label, out int targetLabel, out int matchedAbsoluteIndex, bool isBreak)
    {
        targetLabel = default;
        matchedAbsoluteIndex = -1;

        // Enumerate from top to bottom; Stack<T>.ToArray() returns top-first.
        var contexts = _controlFlowStack.ToArray();
        var total = contexts.Length;

        if (string.IsNullOrEmpty(label))
        {
            if (isBreak)
            {
                if (total == 0)
                {
                    return false;
                }

                targetLabel = contexts[0].BreakLabel;
                matchedAbsoluteIndex = total - 1;
                return true;
            }

            // continue without label targets nearest loop context
            for (int i = 0; i < total; i++)
            {
                if (contexts[i].ContinueLabel is int continueLabel)
                {
                    targetLabel = continueLabel;
                    matchedAbsoluteIndex = total - 1 - i;
                    return true;
                }
            }

            return false;
        }

        for (int i = 0; i < total; i++)
        {
            var ctx = contexts[i];
            if (!string.Equals(ctx.LabelName, label, global::System.StringComparison.Ordinal))
            {
                continue;
            }

            if (isBreak)
            {
                targetLabel = ctx.BreakLabel;
                matchedAbsoluteIndex = total - 1 - i;
                return true;
            }

            if (ctx.ContinueLabel is int continueLabel)
            {
                targetLabel = continueLabel;
                matchedAbsoluteIndex = total - 1 - i;
                return true;
            }

            // Labeled continue targeting a non-loop labeled statement is invalid; do not fall through
            // to outer contexts with the same label.
            return false;
        }

        return false;
    }

    private bool TryEmitReturnViaEpilogue(TempVariable returnValue)
    {
        if (!_methodBodyIR.ReturnEpilogueLabelId.HasValue)
        {
            return false;
        }

        EnsureReturnEpilogueStorage();

        // Store return value into the dedicated slot.
        var storeTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(returnValue, storeTemp));
        DefineTempStorage(storeTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(storeTemp, _returnEpilogueReturnSlot!.Value);

        // Leave to epilogue (outside of try/finally so finally executes).
        _methodBodyIR.Instructions.Add(new LIRLeave(_methodBodyIR.ReturnEpilogueLabelId.Value));
        _needsReturnEpilogueBlock = true;
        return true;
    }

    private void EnsureReturnEpilogueStorage()
    {
        if (_returnEpilogueReturnSlot.HasValue && _returnEpilogueLoadTemp.HasValue)
        {
            return;
        }

        // Reserve a stable slot for the return value.
        var slot = CreateAnonymousVariableSlot("$return", new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _returnEpilogueReturnSlot = slot;

        // Create a load temp mapped to the slot so epilogue can return it.
        var loadTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstUndefined(loadTemp));
        DefineTempStorage(loadTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        SetTempVariableSlot(loadTemp, slot);
        _returnEpilogueLoadTemp = loadTemp;
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

            case HIRConditionalExpression conditionalExpr:
                return TryLowerConditionalExpression(conditionalExpr, out resultTempVar);

            case HIRCallExpression callExpr:
                return TryLowerCallExpression(callExpr, out resultTempVar);

            case HIRNewExpression newExpr:
                return TryLowerNewExpression(newExpr, out resultTempVar);

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

    private bool TryLowerNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (newExpr.Callee is not HIRVariableExpression calleeVar)
        {
            return false;
        }

        // User-defined class: `new ClassName(...)`
        if (calleeVar.Name.Kind != BindingKind.Global)
        {
            if (calleeVar.Name.BindingInfo.DeclarationNode is not ClassDeclaration classDecl)
            {
                return false;
            }

            return TryLowerNewUserDefinedClass(classDecl, newExpr.Arguments, out resultTempVar);
        }

        var ctorName = calleeVar.Name.Name;

        // PL3.3a: built-in Error types
        if (BuiltInErrorTypes.IsBuiltInErrorTypeName(ctorName))
        {
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            TempVariable? messageTemp = null;
            if (newExpr.Arguments.Count == 1)
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var loweredMessage))
                {
                    return false;
                }
                messageTemp = EnsureObject(loweredMessage);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(ctorName, messageTemp, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // PL3.3d: Array constructor semantics
        if (string.Equals(ctorName, "Array", StringComparison.Ordinal))
        {
            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic("Array", "Construct", argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // PL3.3e: String constructor sugar
        if (string.Equals(ctorName, "String", StringComparison.Ordinal))
        {
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            TempVariable source;
            if (newExpr.Arguments.Count == 0)
            {
                source = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(source));
                DefineTempStorage(source, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            else
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var argTemp))
                {
                    return false;
                }
                source = EnsureObject(argTemp);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToString(source, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            return true;
        }

        // PL3.3f: Boolean/Number constructor sugar
        if (string.Equals(ctorName, "Boolean", StringComparison.Ordinal))
        {
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            TempVariable source;
            if (newExpr.Arguments.Count == 0)
            {
                source = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(source));
                DefineTempStorage(source, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            else
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var argTemp))
                {
                    return false;
                }
                source = EnsureObject(argTemp);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(source, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            return true;
        }

        if (string.Equals(ctorName, "Number", StringComparison.Ordinal))
        {
            if (newExpr.Arguments.Count > 1)
            {
                return false;
            }

            TempVariable source;
            if (newExpr.Arguments.Count == 0)
            {
                source = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstUndefined(source));
                DefineTempStorage(source, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            }
            else
            {
                if (!TryLowerExpression(newExpr.Arguments[0], out var argTemp))
                {
                    return false;
                }
                source = EnsureObject(argTemp);
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToNumber(source, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // PL3.3g: generic intrinsic constructor support (Date/RegExp/Set/Promise/Int32Array/etc.)
        var intrinsicType = JavaScriptRuntime.IntrinsicObjectRegistry.Get(ctorName);
        if (intrinsicType != null)
        {
            bool isStaticClass = intrinsicType.IsAbstract && intrinsicType.IsSealed;
            if (isStaticClass)
            {
                return false;
            }

            if (newExpr.Arguments.Count > 2)
            {
                return false;
            }

            var argTemps = new List<TempVariable>(newExpr.Arguments.Count);
            foreach (var arg in newExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                argTemps.Add(EnsureObject(argTemp));
            }

            resultTempVar = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(ctorName, argTemps, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        return false;
    }

    private bool TryLowerNewUserDefinedClass(ClassDeclaration classDecl, IReadOnlyList<HIRExpression> args, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Resolve the class scope to determine whether it needs parent scopes.
        var rootScope = _scope;
        while (rootScope.Parent != null)
        {
            rootScope = rootScope.Parent;
        }

        var classScope = FindScopeByDeclarationNode(classDecl, rootScope);
        if (classScope == null)
        {
            return false;
        }

        bool needsScopes = classScope.ReferencesParentScopeVariables;
        TempVariable? scopesTemp = null;
        if (needsScopes)
        {
            scopesTemp = CreateTempVariable();
            if (!TryBuildScopesArrayForClassConstructor(classScope, scopesTemp.Value))
            {
                return false;
            }
            DefineTempStorage(scopesTemp.Value, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
        }

        // Lower arguments (boxed)
        var argTemps = new List<TempVariable>(args.Count);
        foreach (var arg in args)
        {
            if (!TryLowerExpression(arg, out var argTemp))
            {
                return false;
            }
            argTemps.Add(EnsureObject(argTemp));
        }

        // Compute ctor arg range from AST (min required vs max including defaults)
        var ctorMember = classDecl.Body.Body
            .OfType<MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
        Node ctorNodeForToken = (Node?)ctorMember ?? classDecl.Body;

        int minArgs = 0;
        int maxArgs = 0;
        if (ctorMember?.Value is FunctionExpression ctorFunc)
        {
            foreach (var p in ctorFunc.Params)
            {
                switch (p)
                {
                    case RestElement:
                        return false;
                    case AssignmentPattern:
                        maxArgs++;
                        break;
                    default:
                        minArgs++;
                        maxArgs++;
                        break;
                }
            }
        }

        var className = classDecl.Id is Identifier cid ? cid.Name : classScope.Name;

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewUserClass(
            ClassName: className,
            ConstructorNode: ctorNodeForToken,
            NeedsScopes: needsScopes,
            ScopesArray: scopesTemp,
            MinArgCount: minArgs,
            MaxArgCount: maxArgs,
            Arguments: argTemps,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
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
            var callableId = TryCreateCallableIdForFunctionDeclaration(symbol);
            _methodBodyIR.Instructions.Add(new LIRCallFunction(symbol, scopesTempVar, arguments, resultTempVar, callableId));
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

    private Js2IL.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForFunctionDeclaration(Symbol symbol)
    {
        if (_scope == null)
        {
            return null;
        }

        // IR lowering currently only supports direct calls where the callee is a function binding.
        // For now, only attach CallableId for function declarations (named function foo() {}).
        if (symbol.BindingInfo.Kind != BindingKind.Function)
        {
            return null;
        }

        if (symbol.BindingInfo.DeclarationNode is not FunctionDeclaration funcDecl)
        {
            return null;
        }

        var declaringScope = FindDeclaringScope(symbol.BindingInfo);
        if (declaringScope == null)
        {
            return null;
        }

        var root = declaringScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var declaringScopeName = declaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{declaringScope.GetQualifiedName()}";

        return new Js2IL.Services.TwoPhaseCompilation.CallableId
        {
            Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.FunctionDeclaration,
            DeclaringScopeName = declaringScopeName,
            Name = symbol.Name,
            JsParamCount = funcDecl.Params.Count,
            AstNode = funcDecl
        };
    }

    private Scope? FindDeclaringScope(BindingInfo binding)
    {
        var current = _scope;
        while (current != null)
        {
            if (current.Bindings.TryGetValue(binding.Name, out var candidate) && ReferenceEquals(candidate, binding))
            {
                return current;
            }
            current = current.Parent;
        }
        return null;
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

        if (unaryExpr.Operator == Acornima.Operator.LogicalNot)
        {
            // JS logical not: coerce to boolean (truthiness) then invert.
            // Minimal implementation uses runtime TypeUtilities.ToBoolean(object).
            unaryArgTempVar = EnsureObject(unaryArgTempVar);
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

        // Non-logical operators: evaluate RHS eagerly.
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

        // Updating a const is a runtime TypeError.
        if (updateBinding.Kind == BindingKind.Const)
        {
            _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
            resultTempVar = CreateTempVariable();
            return true;
        }

        // Load the current value (handles both captured and non-captured variables)
        if (!TryLoadVariable(updateBinding, out var currentValue))
        {
            return false;
        }

        // For captured variables loaded from scope fields, the storage is always object (Reference).
        // For non-captured numeric locals, we only support double.
        var currentStorage = GetTempStorage(currentValue);

        // Captured variable update: value is boxed (object) and stored in a scope field.
        // Implement numeric coercion via runtime TypeUtilities.ToNumber(object?) and then store
        // the boxed updated value back to the appropriate scope field.
        if (currentStorage.Kind == ValueStorageKind.Reference && currentStorage.ClrType == typeof(object))
        {
            if (_environmentLayout == null)
            {
                return false;
            }

            var storage = _environmentLayout.GetStorage(updateBinding);
            if (storage == null)
            {
                return false;
            }

            // For postfix, we must capture the old value before the field store happens.
            // Use LIRCopyTemp so Stackify will materialize the captured value.
            TempVariable? originalSnapshotForPostfix = null;
            if (!updateExpr.Prefix)
            {
                var snapshot = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(currentValue, snapshot));
                DefineTempStorage(snapshot, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                originalSnapshotForPostfix = snapshot;
            }

            var currentNumber = EnsureNumber(currentValue);

            var deltaOneTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, deltaOneTemp));
            DefineTempStorage(deltaOneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedNumber = CreateTempVariable();
            if (isIncrement)
            {
                _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, deltaOneTemp, updatedNumber));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, deltaOneTemp, updatedNumber));
            }
            DefineTempStorage(updatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedBoxed = EnsureObject(updatedNumber);

            switch (storage.Kind)
            {
                case BindingStorageKind.LeafScopeField:
                    if (storage.Field.IsNil || storage.DeclaringScope.IsNil)
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(updateBinding, storage.Field, storage.DeclaringScope, updatedBoxed));
                    break;

                case BindingStorageKind.ParentScopeField:
                    if (storage.ParentScopeIndex < 0 || storage.Field.IsNil || storage.DeclaringScope.IsNil)
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreParentScopeField(updateBinding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, updatedBoxed));
                    break;

                default:
                    // Not a captured storage - fall back to local update path.
                    return false;
            }

            // Update SSA map for subsequent reads.
            _variableMap[updateBinding] = updatedBoxed;

            if (updateExpr.Prefix)
            {
                resultTempVar = updatedBoxed;
                return true;
            }

            resultTempVar = originalSnapshotForPostfix!.Value;
            return true;
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

    private TempVariable EnsureNumber(TempVariable tempVar)
    {
        var storage = GetTempStorage(tempVar);

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(double))
        {
            return tempVar;
        }

        // Dynamic numeric coercion: object -> double via runtime helper.
        var numberTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConvertToNumber(EnsureObject(tempVar), numberTempVar));
        DefineTempStorage(numberTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        return numberTempVar;
    }

    private bool TryStoreToBinding(BindingInfo binding, TempVariable valueToStore, out TempVariable storedValue)
    {
        storedValue = default;

        var lirInstructions = _methodBodyIR.Instructions;

        // Store via environment layout (captured vars, parameters)
        if (_environmentLayout != null)
        {
            var storage = _environmentLayout.GetStorage(binding);
            if (storage != null)
            {
                switch (storage.Kind)
                {
                    case BindingStorageKind.LeafScopeField:
                        if (!storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, boxedValue));
                            _variableMap[binding] = boxedValue;
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, boxedValue));
                            _variableMap[binding] = boxedValue;
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlArgument:
                        if (storage.JsParameterIndex >= 0)
                        {
                            var boxedValue = EnsureObject(valueToStore);
                            lirInstructions.Add(new LIRStoreParameter(storage.JsParameterIndex, boxedValue));
                            storedValue = boxedValue;
                            return true;
                        }
                        break;

                    case BindingStorageKind.IlLocal:
                        // Non-captured local - fall through to local-slot behavior
                        break;
                }
            }
        }

        // Fallback parameter index map (when environment layout isn't present)
        if (_parameterIndexMap.TryGetValue(binding, out var paramIndex))
        {
            var boxedValue = EnsureObject(valueToStore);
            lirInstructions.Add(new LIRStoreParameter(paramIndex, boxedValue));
            storedValue = boxedValue;
            return true;
        }

        // Non-captured variable - use stable variable slot
        _variableMap[binding] = valueToStore;

        var storageInfo = GetTempStorage(valueToStore);
        var slot = GetOrCreateVariableSlot(binding, binding.Name, storageInfo);
        SetTempVariableSlot(valueToStore, slot);
        // for..of/in assigns each iteration; do not treat as single-assignment
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);

        storedValue = valueToStore;
        return true;
    }

    private bool TryLowerAssignmentExpression(HIRAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var binding = assignExpr.Target.BindingInfo;
        var lirInstructions = _methodBodyIR.Instructions;

        // Assigning to a const is a runtime TypeError.
        if (binding.Kind == BindingKind.Const)
        {
            lirInstructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
            resultTempVar = CreateTempVariable();
            return true;
        }

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

    private bool TryLowerConditionalExpression(HIRConditionalExpression conditionalExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Evaluate the test condition, then branch to either consequent or alternate.
        if (!TryLowerExpression(conditionalExpr.Test, out var conditionTemp))
        {
            return false;
        }

        var conditionBoxed = EnsureObject(conditionTemp);
        var isTruthyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(conditionBoxed, isTruthyTemp));
        DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));

        int elseLabel = CreateLabel();
        int endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(isTruthyTemp, elseLabel));

        // Consequent branch
        if (!TryLowerExpression(conditionalExpr.Consequent, out var consequentTemp))
        {
            return false;
        }

        // For now, always box the result so branches can join safely.
        var consequentBoxed = EnsureObject(consequentTemp);
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(consequentBoxed, resultTempVar));
        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Alternate branch
        _methodBodyIR.Instructions.Add(new LIRLabel(elseLabel));

        if (!TryLowerExpression(conditionalExpr.Alternate, out var alternateTemp))
        {
            return false;
        }

        var alternateBoxed = EnsureObject(alternateTemp);
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(alternateBoxed, resultTempVar));

        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
        return true;
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

    private int CreateAnonymousVariableSlot(string displayName, ValueStorage storage)
    {
        var slot = _methodBodyIR.VariableNames.Count;
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