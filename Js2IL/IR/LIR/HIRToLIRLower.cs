using Acornima.Ast;
using Js2IL.HIR;
using Js2IL.Services;
using Js2IL.Services.ScopesAbi;
using TwoPhase = Js2IL.Services.TwoPhaseCompilation;
using Js2IL.Utilities;
using Js2IL.SymbolTables;

namespace Js2IL.IR;

public sealed class HIRToLIRLowerer
{
    private readonly MethodBodyIR _methodBodyIR = new MethodBodyIR();
    private readonly Scope? _scope;
    private readonly EnvironmentLayout? _environmentLayout;
    private readonly EnvironmentLayoutBuilder? _environmentLayoutBuilder;
    private readonly Js2IL.Services.ClassRegistry? _classRegistry;
    private readonly CallableKind _callableKind;
    private readonly bool _isAsync;

    private int _tempVarCounter = 0;
    private int _labelCounter = 0;

    private readonly Stack<ControlFlowContext> _controlFlowStack = new();

    private readonly Stack<int> _protectedControlFlowDepthStack = new();
    private int? _returnEpilogueReturnSlot;
    private TempVariable? _returnEpilogueLoadTemp;
    private bool _needsReturnEpilogueBlock;

    private readonly Stack<AsyncTryCatchContext> _asyncTryCatchStack = new();
    private readonly Stack<AsyncTryFinallyContext> _asyncTryFinallyStack = new();

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

    private sealed record AsyncTryCatchContext(int CatchStateId, int CatchLabelId, string PendingExceptionFieldName);

    private sealed record AsyncTryFinallyContext(
        int FinallyEntryLabelId,
        int FinallyExitLabelId,
        string PendingExceptionFieldName,
        string HasPendingExceptionFieldName,
        string PendingReturnFieldName,
        string HasPendingReturnFieldName,
        bool IsInFinally);

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

    private HIRToLIRLowerer(Scope? scope, EnvironmentLayout? environmentLayout, EnvironmentLayoutBuilder? environmentLayoutBuilder, Js2IL.Services.ClassRegistry? classRegistry, CallableKind callableKind, IReadOnlyList<HIRPattern> parameters, bool isAsync = false)
    {
        _scope = scope;
        _environmentLayout = environmentLayout;
        _environmentLayoutBuilder = environmentLayoutBuilder;
        _classRegistry = classRegistry;
        _callableKind = callableKind;
        _isAsync = isAsync;
        InitializeParameters(parameters);
    }

    private Type? TryGetStableThisFieldClrType(string fieldName)
    {
        // Find the nearest enclosing class scope.
        var current = _scope;
        while (current != null && current.Kind != ScopeKind.Class)
        {
            current = current.Parent;
        }

        if (current == null)
        {
            return null;
        }

        return current.StableInstanceFieldClrTypes.TryGetValue(fieldName, out var t) ? t : null;
    }

    private static ValueStorage GetPreferredFieldReadStorage(Type? fieldClrType)
    {
        if (fieldClrType == typeof(double))
        {
            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
        }
        if (fieldClrType == typeof(bool))
        {
            return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
        }
        if (fieldClrType == typeof(string))
        {
            return new ValueStorage(ValueStorageKind.Reference, typeof(string));
        }

        return new ValueStorage(ValueStorageKind.Reference, typeof(object));
    }

    private void InitializeParameters(IReadOnlyList<HIRPattern> parameters)
    {
        // Build ordered parameter names from HIR. (No AST peeking in lowering.)
        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];

            var ilParamName = p switch
            {
                HIRIdentifierPattern id => id.Symbol.Name,
                HIRDefaultPattern def when def.Target is HIRIdentifierPattern id => id.Symbol.Name,
                _ => $"$p{i}"
            };
            _methodBodyIR.Parameters.Add(ilParamName);
        }

        // If we don't have scope info (e.g., legacy callers), we can't build binding maps or emit
        // default/destructuring initializers. Parameter names are still populated above.
        if (_scope == null) return;

        // Map identifier parameters to their 0-based JS parameter index.
        // Destructuring parameters bind their properties, not the parameter object itself.
        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];
            BindingInfo? binding = p switch
            {
                HIRIdentifierPattern id => id.Symbol.BindingInfo,
                HIRDefaultPattern def when def.Target is HIRIdentifierPattern id => id.Symbol.BindingInfo,
                _ => null
            };

            if (binding != null)
            {
                _parameterIndexMap[binding] = i;
            }
        }

        // Emit default parameter initializers for identifier parameters with defaults.
        _parameterInitSucceeded = EmitDefaultParameterInitializers(parameters);
        if (!_parameterInitSucceeded)
        {
            return;
        }

        // Emit parameter destructuring initializers (object/array patterns).
        if (!EmitDestructuredParameterInitializers(parameters))
        {
            _parameterInitSucceeded = false;
            return;
        }

        // For captured identifier parameters, initialize the corresponding leaf-scope fields.
        // This must happen after default parameter initialization so the final value is stored.
        // Without this, nested functions reading captured parameters will observe null.
        EmitCapturedParameterFieldInitializers();
    }

    private bool EmitDestructuredParameterInitializers(IReadOnlyList<HIRPattern> parameters)
    {
        if (_scope == null)
        {
            return false;
        }

        for (int i = 0; i < parameters.Count; i++)
        {
            var p = parameters[i];

            // Only handle direct Object/Array patterns for now.
            // (Param-level defaults like ({a} = {}) are not yet supported by the IR pipeline gate.)
            if (p is not HIRObjectPattern && p is not HIRArrayPattern)
            {
                if (p is HIRDefaultPattern def && (def.Target is HIRObjectPattern or HIRArrayPattern))
                {
                    return false;
                }
                continue;
            }

            // Load the incoming parameter value and destructure into declared bindings.
            var paramTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(i, paramTemp));
            DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerDestructuringPattern(p, paramTemp, isDeclaration: true, sourceNameForError: null))
            {
                return false;
            }
        }

        return true;
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
    /// Emits LIR instructions to initialize default parameter values.
    /// For each parameter with a default (AssignmentPattern), emits:
    /// - Load parameter, check if null
    /// - If null, evaluate default expression and store back to parameter
    /// </summary>
    /// <returns>True if all default parameters were successfully lowered, false if any failed (method should fall back to legacy)</returns>
    private bool EmitDefaultParameterInitializers(IReadOnlyList<HIRPattern> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            if (parameters[i] is not HIRDefaultPattern def)
            {
                continue;
            }

            // Only support top-level default parameters for identifier params.
            if (def.Target is not HIRIdentifierPattern)
            {
                return false;
            }

            // Record instruction count before we start, so we can roll back if lowering fails
            var instructionCountBefore = _methodBodyIR.Instructions.Count;

            // Emit: load parameter, check if null, if so evaluate default and store back
            var notNullLabel = CreateLabel();

            // Load parameter value
            var paramTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadParameter(i, paramTemp));
            DefineTempStorage(paramTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            // Branch if not null (brtrue)
            _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(paramTemp, notNullLabel));

            // Evaluate default value expression
            if (!TryLowerExpression(def.Default, out var defaultValueTemp))
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
            _methodBodyIR.Instructions.Add(new LIRStoreParameter(i, defaultValueTemp));

            // Not-null label
            _methodBodyIR.Instructions.Add(new LIRLabel(notNullLabel));
        }

        return true; // All default parameters successfully lowered
    }

    /// <summary>
    /// Counts the number of await expressions in an HIR block.
    /// Used to determine if an async function needs full state machine support.
    /// </summary>
    private static int CountAwaitExpressions(HIRBlock block)
    {
        int count = 0;
        foreach (var statement in block.Statements)
        {
            count += CountAwaitExpressionsInStatement(statement);
        }
        return count;
    }
    
    private static int CountAwaitExpressionsInStatement(HIRStatement statement)
    {
        int count = 0;
        switch (statement)
        {
            case HIRExpressionStatement exprStmt:
                count += CountAwaitExpressionsInExpression(exprStmt.Expression);
                break;
            case HIRVariableDeclaration varDecl:
                if (varDecl.Initializer != null)
                    count += CountAwaitExpressionsInExpression(varDecl.Initializer);
                break;
            case HIRReturnStatement returnStmt:
                if (returnStmt.Expression != null)
                    count += CountAwaitExpressionsInExpression(returnStmt.Expression);
                break;
            case HIRIfStatement ifStmt:
                count += CountAwaitExpressionsInExpression(ifStmt.Test);
                count += CountAwaitExpressionsInStatement(ifStmt.Consequent);
                if (ifStmt.Alternate != null)
                    count += CountAwaitExpressionsInStatement(ifStmt.Alternate);
                break;
            case HIRWhileStatement whileStmt:
                count += CountAwaitExpressionsInExpression(whileStmt.Test);
                count += CountAwaitExpressionsInStatement(whileStmt.Body);
                break;
            case HIRForStatement forStmt:
                if (forStmt.Init != null)
                    count += CountAwaitExpressionsInStatement(forStmt.Init);
                if (forStmt.Test != null)
                    count += CountAwaitExpressionsInExpression(forStmt.Test);
                if (forStmt.Update != null)
                    count += CountAwaitExpressionsInExpression(forStmt.Update);
                count += CountAwaitExpressionsInStatement(forStmt.Body);
                break;
            case HIRTryStatement tryStmt:
                count += CountAwaitExpressionsInStatement(tryStmt.TryBlock);
                if (tryStmt.CatchBody != null)
                    count += CountAwaitExpressionsInStatement(tryStmt.CatchBody);
                if (tryStmt.FinallyBody != null)
                    count += CountAwaitExpressionsInStatement(tryStmt.FinallyBody);
                break;
            case HIRBlock blockStmt:
                count += CountAwaitExpressions(blockStmt);
                break;
            case HIRThrowStatement throwStmt:
                count += CountAwaitExpressionsInExpression(throwStmt.Argument);
                break;
        }
        return count;
    }
    
    private static int CountAwaitExpressionsInExpression(HIRExpression expression)
    {
        int count = 0;
        switch (expression)
        {
            case HIRAwaitExpression awaitExpr:
                count = 1; // Found one!
                count += CountAwaitExpressionsInExpression(awaitExpr.Argument);
                break;
            case HIRBinaryExpression binExpr:
                count += CountAwaitExpressionsInExpression(binExpr.Left);
                count += CountAwaitExpressionsInExpression(binExpr.Right);
                break;
            case HIRUnaryExpression unaryExpr:
                count += CountAwaitExpressionsInExpression(unaryExpr.Argument);
                break;
            case HIRCallExpression callExpr:
                count += CountAwaitExpressionsInExpression(callExpr.Callee);
                foreach (var arg in callExpr.Arguments)
                    count += CountAwaitExpressionsInExpression(arg);
                break;
            case HIRPropertyAccessExpression propAccessExpr:
                count += CountAwaitExpressionsInExpression(propAccessExpr.Object);
                break;
            case HIRConditionalExpression condExpr:
                count += CountAwaitExpressionsInExpression(condExpr.Test);
                count += CountAwaitExpressionsInExpression(condExpr.Consequent);
                count += CountAwaitExpressionsInExpression(condExpr.Alternate);
                break;
            case HIRArrayExpression arrayExpr:
                foreach (var elem in arrayExpr.Elements)
                    if (elem != null)
                        count += CountAwaitExpressionsInExpression(elem);
                break;
            case HIRObjectExpression objExpr:
                foreach (var member in objExpr.Members)
                {
                    switch (member)
                    {
                        case HIRObjectProperty prop:
                            count += CountAwaitExpressionsInExpression(prop.Value);
                            break;
                        case HIRObjectComputedProperty computed:
                            count += CountAwaitExpressionsInExpression(computed.KeyExpression);
                            count += CountAwaitExpressionsInExpression(computed.Value);
                            break;
                        case HIRObjectSpreadProperty spread:
                            count += CountAwaitExpressionsInExpression(spread.Argument);
                            break;
                    }
                }
                break;
            case HIRAssignmentExpression assignExpr:
                count += CountAwaitExpressionsInExpression(assignExpr.Value);
                break;
            case HIRNewExpression newExpr:
                count += CountAwaitExpressionsInExpression(newExpr.Callee);
                foreach (var arg in newExpr.Arguments)
                    count += CountAwaitExpressionsInExpression(arg);
                break;
        }
        return count;
    }

    internal static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind callableKind, bool hasScopesParameter, Js2IL.Services.ClassRegistry? classRegistry, out MethodBodyIR? lirMethod, bool isAsync = false, TwoPhase.CallableId? callableId = null)
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
                var needsParentScopesOverride = callableKind == Js2IL.Services.ScopesAbi.CallableKind.Constructor
                    ? hasScopesParameter
                    : (bool?)null;
                environmentLayout = environmentLayoutBuilder.Build(scope, callableKind, needsParentScopesOverride: needsParentScopesOverride);
            }
            catch (Exception ex)
            {
                // If we can't build environment layout, fall back to legacy
                IRPipelineMetrics.RecordFailure($"EnvironmentLayout build failed for scope '{scope.GetQualifiedName()}' (kind={scope.Kind}) callableKind={callableKind}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        var lowerer = new HIRToLIRLowerer(scope, environmentLayout, environmentLayoutBuilder, classRegistry, callableKind, hirMethod.Parameters, isAsync);

        // If default parameter initialization failed, fall back to legacy emitter
        if (!lowerer._parameterInitSucceeded)
        {
            return false;
        }

        // Set CallableId for async functions (needed to create self-referencing closure)
        if (callableId != null)
        {
            lowerer._methodBodyIR.CallableId = callableId;
        }

        // Initialize async state machine info if this is an async function
        if (isAsync)
        {
            lowerer._methodBodyIR.IsAsync = true;
            lowerer._methodBodyIR.AsyncInfo = new AsyncStateMachineInfo();
            
            // Pre-scan for await points to set up the state machine.
            int awaitCount = CountAwaitExpressions(hirMethod.Body);
            
            // If there are await points, enable full state machine infrastructure
            if (awaitCount > 0)
            {
                lowerer._methodBodyIR.AsyncInfo.HasAwaits = true;
                
                // Reserve state IDs for each await point (they'll be assigned during lowering)
                // State 0 = initial entry
                // State 1, 2, ... = resume points after each await
                // State -1 = completed
            }
        }

        // Emit scope instance creation if there are leaf scope fields
        // NOTE: This must come AFTER async info is initialized, because async functions
        // with awaits need a scope instance even if there are no captured variables.
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
    internal static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, out MethodBodyIR? lirMethod)
    {
        return TryLower(hirMethod, scope, scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind.Function, hasScopesParameter: true, classRegistry: null, out lirMethod);
    }

    private bool TryGetEnclosingClassRegistryName(out string? registryClassName)
    {
        registryClassName = null;

        if (_scope == null)
        {
            return false;
        }

        var current = _scope;
        while (current != null)
        {
            if (current.Kind == ScopeKind.Class)
            {
                var ns = current.DotNetNamespace ?? "Classes";
                var name = current.DotNetTypeName ?? current.Name;
                registryClassName = $"{ns}.{name}";
                return true;
            }

            current = current.Parent;
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

        // Special-case: the global/module scope instance is needed to satisfy the ABI expectation
        // that scopes[0] is the module/global scope when lowering direct user-defined calls from the
        // entry point. Even if the global scope has no leaf fields, creating the instance is cheap
        // and enables IR lowering to build scopes arrays without falling back.
        if (_scope != null && _scope.Kind == ScopeKind.Global && !_methodBodyIR.NeedsLeafScopeLocal)
        {
            EnsureLeafScopeInstance(new ScopeId(ScopeNaming.GetRegistryScopeName(_scope)));
            return;
        }

        // If a function contains nested functions/classes, it still needs a concrete leaf scope
        // instance so closures can include a stable parent slot in their scopes array.
        // Without this, nested Promise executors/callbacks can IndexOutOfRange when they expect
        // scopes[1] (or deeper) to exist even if the parent has no captured fields.
        if (_scope != null
            && _scope.Kind == ScopeKind.Function
            && _scope.Children.Any(c => c.Kind == ScopeKind.Function || c.Kind == ScopeKind.Class)
            && !_methodBodyIR.NeedsLeafScopeLocal)
        {
            EnsureLeafScopeInstance(new ScopeId(ScopeNaming.GetRegistryScopeName(_scope)));
            return;
        }

        // Async functions with awaits need a scope instance for state machine fields
        // (_asyncState, _deferred, _moveNext, _awaited*) even if there are no captured variables.
        if (_scope != null
            && _methodBodyIR.IsAsync
            && _methodBodyIR.AsyncInfo?.HasAwaits == true
            && !_methodBodyIR.NeedsLeafScopeLocal)
        {
            EnsureLeafScopeInstance(new ScopeId(ScopeNaming.GetRegistryScopeName(_scope)));
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

        return TryBuildScopesArrayFromLayout(calleeScope, CallableKind.Function, resultTemp);
    }

    private bool TryBuildScopesArrayFromLayout(Scope calleeScope, CallableKind callableKind, TempVariable resultTemp)
    {
        // Build the callee's environment layout to get its scope chain.
        if (_environmentLayoutBuilder == null)
        {
            // No layout builder available - fall back to legacy
            return false;
        }

        EnvironmentLayout calleeLayout;
        try
        {
            calleeLayout = _environmentLayoutBuilder.Build(calleeScope, callableKind);
        }
        catch
        {
            return false;
        }

        // Map each slot in the callee's scope chain to a source in the caller.
        var slotSources = new List<ScopeSlotSource>();
        foreach (var slot in calleeLayout.ScopeChain.Slots)
        {
            if (!TryMapScopeSlotToSource(slot, out var slotSource))
            {
                var caller = _scope != null ? _scope.GetQualifiedName() : "<null>";
                var callerScopesSource = _environmentLayout?.Abi.ScopesSource.ToString() ?? "<null>";
                var callerChain = _environmentLayout != null
                    ? string.Join(",", _environmentLayout.ScopeChain.Slots.Select(s => s.ScopeName))
                    : "<null>";
                IRPipelineMetrics.RecordFailure(
                    $"HIR->LIR: failed mapping scope slot '{slot.ScopeName}' for callee '{calleeScope.GetQualifiedName()}' ({callableKind}); caller='{caller}', callerScopesSource={callerScopesSource}, callerChain=[{callerChain}]"
                );
                return false;
            }
            slotSources.Add(slotSource);
        }

        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(slotSources, resultTemp));
        return true;

    }


    private bool TryBuildScopesArrayForClassConstructor(Scope classScope, TempVariable resultTemp)
    {
        if (classScope.ReferencesParentScopeVariables)
        {
            return TryBuildScopesArrayFromLayout(classScope, CallableKind.Constructor, resultTemp);
        }

        // Preserve ABI expectation that scopes[0] is the global/module scope when available.
        if (_scope == null)
        {
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
            return true;
        }

        var root = _scope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var globalSlot = new ScopeSlot(Index: 0, ScopeName: moduleName, ScopeId: new ScopeId(moduleName));
        if (!TryMapScopeSlotToSource(globalSlot, out var globalSlotSource))
        {
            var caller = _scope != null ? _scope.GetQualifiedName() : "<null>";
            var callerScopesSource = _environmentLayout?.Abi.ScopesSource.ToString() ?? "<null>";
            var callerChain = _environmentLayout != null
                ? string.Join(",", _environmentLayout.ScopeChain.Slots.Select(s => s.ScopeName))
                : "<null>";
            IRPipelineMetrics.RecordFailure(
                $"HIR->LIR: failed mapping global scope '{moduleName}' for class ctor scopes; calleeClass='{classScope.GetQualifiedName()}', caller='{caller}', callerScopesSource={callerScopesSource}, callerChain=[{callerChain}]"
            );
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(new[] { globalSlotSource }, resultTemp));
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
        static bool NodesMatch(Acornima.Ast.Node a, Acornima.Ast.Node b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a.GetType() != b.GetType())
            {
                return false;
            }

            // Acornima nodes can be re-parsed between phases, so reference equality can fail.
            // Fall back to a stable match using source location.
            var al = a.Location;
            var bl = b.Location;

            if (al.Start.Line <= 0 || bl.Start.Line <= 0)
            {
                return false;
            }

            return al.Start.Line == bl.Start.Line
                && al.Start.Column == bl.Start.Column
                && al.End.Line == bl.End.Line
                && al.End.Column == bl.End.Column;
        }

        // Check if this scope's AST node matches the declaration
        if (NodesMatch(root.AstNode, declarationNode))
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
                IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering statement {statement.GetType().Name}");
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
            case HIRStoreUserClassInstanceFieldStatement storeInstanceField:
                {
                    if (!TryLowerExpression(storeInstanceField.Value, out var valueTemp))
                    {
                        return false;
                    }

                    // Only force object boxing when the declared field type is unknown/object.
                    // For stable typed fields (double/bool/string), keep the value in its preferred form.
                    var stableFieldType = TryGetStableThisFieldClrType(storeInstanceField.FieldName);
                    if (stableFieldType == null || stableFieldType == typeof(object))
                    {
                        valueTemp = EnsureObject(valueTemp);
                    }
                    lirInstructions.Add(new LIRStoreUserClassInstanceField(
                        storeInstanceField.RegistryClassName,
                        storeInstanceField.FieldName,
                        storeInstanceField.IsPrivateField,
                        valueTemp));
                    return true;
                }

            case HIRStoreUserClassStaticFieldStatement storeStaticField:
                {
                    if (!TryLowerExpression(storeStaticField.Value, out var valueTemp))
                    {
                        return false;
                    }
                    valueTemp = EnsureObject(valueTemp);
                    lirInstructions.Add(new LIRStoreUserClassStaticField(
                        storeStaticField.RegistryClassName,
                        storeStaticField.FieldName,
                        valueTemp));
                    return true;
                }

            case HIRVariableDeclaration exprStmt:
                {
                    // Variable declarations define a new binding in the current scope.
                    TempVariable value;

                    if (exprStmt.Initializer != null)
                    {
                        if (!TryLowerExpression(exprStmt.Initializer, out value))
                        {
                            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering variable initializer expression {exprStmt.Initializer.GetType().Name} for '{exprStmt.Name.Name}'");
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
                            // Captured variable - store to leaf scope field.
                            // Most scope fields are object-typed, but stable inferred primitive fields can be typed.
                            // For typed bool fields, do NOT box (stfld bool cannot consume object).
                            var fieldValue = value;
                            if (!(binding.IsStableType && binding.ClrType == typeof(bool)))
                            {
                                fieldValue = EnsureObject(value);
                            }

                            lirInstructions.Add(new LIRStoreLeafScopeField(binding, storage.Field, storage.DeclaringScope, fieldValue));
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
            case HIRDestructuringVariableDeclaration destructDecl:
                {
                    // PL4.1: Destructuring variable declarators.
                    if (!TryLowerExpression(destructDecl.Initializer, out var sourceTemp))
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering destructuring initializer expression {destructDecl.Initializer.GetType().Name}");
                        return false;
                    }

                    // Destructuring operates on JS values (boxed as object).
                    sourceTemp = EnsureObject(sourceTemp);

                    var sourceNameForError = TryGetSimpleSourceNameForDestructuring(destructDecl.Initializer);
                    if (!TryLowerDestructuringPattern(destructDecl.Pattern, sourceTemp, isDeclaration: true, sourceNameForError))
                    {
                        IRPipelineMetrics.RecordFailureIfUnset("HIR->LIR: failed lowering destructuring variable declaration");
                        return false;
                    }

                    return true;
                }
            case HIRExpressionStatement exprStmt:
                {
                    // Lower the expression and discard the result
                    if (!TryLowerExpression(exprStmt.Expression, out var _))
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering expression statement {exprStmt.Expression.GetType().Name}");
                        return false;
                    }
                    return true;
                }
            case HIRReturnStatement returnStmt:
                {
                    TempVariable returnTempVar;
                    if (_callableKind == CallableKind.Constructor && returnStmt.Expression != null)
                    {
                        // Constructors are void-returning in IL, but JavaScript allows `return <expr>`.
                        // Stash the value so the `new` call site can apply JS override semantics.
                        if (!TryLowerExpression(returnStmt.Expression, out var ctorReturnTemp))
                        {
                            return false;
                        }

                        ctorReturnTemp = EnsureObject(ctorReturnTemp);

                        if (!TryGetEnclosingClassRegistryName(out var registryClassName) || registryClassName == null)
                        {
                            return false;
                        }

                        lirInstructions.Add(new LIRStoreUserClassInstanceField(
                            RegistryClassName: registryClassName,
                            FieldName: "__js2il_ctorReturn",
                            IsPrivateField: true,
                            Value: ctorReturnTemp));

                        // Control-flow return value is irrelevant for constructors.
                        returnTempVar = CreateTempVariable();
                        lirInstructions.Add(new LIRConstUndefined(returnTempVar));
                        DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }
                    else if (returnStmt.Expression != null)
                    {
                        // Lower the return expression
                        if (!TryLowerExpression(returnStmt.Expression, out returnTempVar))
                        {
                            return false;
                        }

                        // Default ABI returns object. If we inferred a stable return type for this callable,
                        // preserve/produce the matching unboxed or typed value.
                        // Typed/unboxed returns are only supported for class methods currently.
                        // Keep ABI consistent: other callables must return object.
                        var stableReturnClrType = (_scope?.Kind == ScopeKind.Function && _scope?.Parent?.Kind == ScopeKind.Class)
                            ? _scope.StableReturnClrType
                            : null;
                        if (stableReturnClrType == typeof(double))
                        {
                            returnTempVar = EnsureNumber(returnTempVar);
                        }
                        else if (stableReturnClrType == typeof(bool))
                        {
                            returnTempVar = EnsureBoolean(returnTempVar);
                        }
                        else
                        {
                            returnTempVar = EnsureObject(returnTempVar);
                        }
                    }
                    else
                    {
                        // Bare return - return undefined (null)
                        returnTempVar = CreateTempVariable();
                        lirInstructions.Add(new LIRConstUndefined(returnTempVar));
                        DefineTempStorage(returnTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }

                    // Async try/finally lowering: a return inside a protected region must flow through finally.
                    if (_isAsync
                        && _methodBodyIR.AsyncInfo?.HasAwaits == true
                        && !_methodBodyIR.LeafScopeId.IsNil
                        && _asyncTryFinallyStack.Count > 0)
                    {
                        var ctx = _asyncTryFinallyStack.Peek();
                        var scopeName = _methodBodyIR.LeafScopeId.Name;

                        returnTempVar = EnsureObject(returnTempVar);

                        // pendingReturnValue = returnTempVar
                        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingReturnFieldName, returnTempVar));

                        // hasPendingReturn = true
                        var hasReturnTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, hasReturnTemp));
                        DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingReturnFieldName, hasReturnTemp));

                        // hasPendingException = false; pendingException = null (return overrides)
                        var clearHasExTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, clearHasExTemp));
                        DefineTempStorage(clearHasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.HasPendingExceptionFieldName, clearHasExTemp));

                        var clearExTemp = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRConstNull(clearExTemp));
                        DefineTempStorage(clearExTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, clearExTemp));

                        _methodBodyIR.Instructions.Add(new LIRBranch(ctx.IsInFinally ? ctx.FinallyExitLabelId : ctx.FinallyEntryLabelId));
                        return true;
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
                    var awaitCountTry = CountAwaitExpressionsInStatement(tryStmt.TryBlock);
                    var awaitCountCatch = tryStmt.CatchBody != null ? CountAwaitExpressionsInStatement(tryStmt.CatchBody) : 0;
                    var awaitCountFinally = tryStmt.FinallyBody != null ? CountAwaitExpressionsInStatement(tryStmt.FinallyBody) : 0;
                    var awaitCount = awaitCountTry + awaitCountCatch + awaitCountFinally;

                    // Async try/finally (and try/catch/finally) cannot use IL exception regions when awaits occur
                    // within the protected region, because awaits suspend MoveNext via 'ret'.
                    if (_isAsync && hasFinally && awaitCount > 0)
                    {
                        return TryLowerAsyncTryWithFinallyWithAwait(tryStmt);
                    }
                    if (_isAsync && hasCatch && !hasFinally && awaitCount > 0)
                    {
                        return TryLowerAsyncTryCatchWithAwait(tryStmt);
                    }
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

                    // In async MoveNext with awaits, do not emit CLR throws (they won't be caught by the runtime).
                    // Instead reject the deferred promise unless we are inside an async try/catch/finally routing context.
                    if (_isAsync
                        && _methodBodyIR.AsyncInfo?.HasAwaits == true
                        && _asyncTryCatchStack.Count == 0
                        && !_methodBodyIR.LeafScopeId.IsNil)
                    {
                        _methodBodyIR.Instructions.Add(new LIRAsyncReject(argTemp));
                        return true;
                    }

                    if (_asyncTryCatchStack.Count > 0 && !_methodBodyIR.LeafScopeId.IsNil)
                    {
                        var ctx = _asyncTryCatchStack.Peek();
                        var scopeName = _methodBodyIR.LeafScopeId.Name;
                        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, ctx.PendingExceptionFieldName, argTemp));
                        _methodBodyIR.Instructions.Add(new LIRBranch(ctx.CatchLabelId));
                        return true;
                    }
                    lirInstructions.Add(new LIRThrow(argTemp));
                    return true;
                }
            case HIRIfStatement ifStmt:
                {
                    // Evaluate the test condition
                    if (!TryLowerExpression(ifStmt.Test, out var conditionTemp))
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if test expression {ifStmt.Test.GetType().Name}");
                        return false;
                    }

                    int elseLabel = CreateLabel();

                    // If the condition is boxed or is an object reference, we need to
                    // convert it to a boolean using IsTruthy before branching.
                    // This is because brfalse on a boxed boolean checks for null, not false,
                    // and JavaScript has different truthiness rules (0, "", null, undefined, NaN are falsy).
                    var conditionStorage = GetTempStorage(conditionTemp);
                    bool needsTruthyCheck = conditionStorage.Kind == ValueStorageKind.BoxedValue ||
                        (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
                        (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));
                    
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
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if consequent {ifStmt.Consequent.GetType().Name}");
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
                            IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering if alternate {ifStmt.Alternate.GetType().Name}");
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
                            (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
                            (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));
                        
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
                    lirInstructions.Add(new LIRGetItem(EnsureObject(iterTemp), idxTemp, itemTemp));
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
                    lirInstructions.Add(new LIRGetItem(EnsureObject(keysTemp), idxTemp, keyTemp));
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
                        (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
                        (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));

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
                        (conditionStorage.Kind == ValueStorageKind.Reference && conditionStorage.ClrType == typeof(object)) ||
                        (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(double));

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

    private bool TryLowerAsyncTryCatchWithAwait(HIRTryStatement tryStmt)
    {
        if (_methodBodyIR.AsyncInfo == null || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        var asyncInfo = _methodBodyIR.AsyncInfo;
        var scopeName = _methodBodyIR.LeafScopeId.Name;
        const string pendingExceptionField = "_pendingException";

        var catchStateId = asyncInfo.AllocateResumeStateId();
        var catchLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(catchStateId, catchLabel);

        var endLabel = CreateLabel();

        // Clear pending exception before entering try
        var clearTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNull(clearTemp));
        DefineTempStorage(clearTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, clearTemp));

        _asyncTryCatchStack.Push(new AsyncTryCatchContext(catchStateId, catchLabel, pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
        }

        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

        // Catch label (used for both rejected awaits and explicit throws in try)
        _methodBodyIR.Instructions.Add(new LIRLabel(catchLabel));

        var pendingTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, pendingTemp));
        DefineTempStorage(pendingTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

        // Clear pending exception after loading
        var clearAfterTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNull(clearAfterTemp));
        DefineTempStorage(clearAfterTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, clearAfterTemp));

        if (tryStmt.CatchParamBinding != null &&
            !TryStoreToBinding(tryStmt.CatchParamBinding, pendingTemp, out _))
        {
            return false;
        }

        if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
        {
            return false;
        }

        _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));
        return true;
    }

    private bool TryLowerAsyncTryWithFinallyWithAwait(HIRTryStatement tryStmt)
    {
        if (_methodBodyIR.AsyncInfo == null || _methodBodyIR.LeafScopeId.IsNil)
        {
            return false;
        }

        if (tryStmt.FinallyBody == null)
        {
            return false;
        }

        var asyncInfo = _methodBodyIR.AsyncInfo;
        var scopeName = _methodBodyIR.LeafScopeId.Name;

        const string pendingExceptionField = "_pendingException";
        const string hasPendingExceptionField = "_hasPendingException";
        const string pendingReturnField = "_pendingReturnValue";
        const string hasPendingReturnField = "_hasPendingReturn";

        bool hasCatch = tryStmt.CatchBody != null;

        // Synthetic labels used by the async state machine.
        var afterTryLabel = CreateLabel();
        var finallyEntryLabel = CreateLabel();
        var finallyExitLabel = CreateLabel();

        // Rejection/exception routing labels (used as resume targets for await rejection).
        var exceptionToFinallyStateId = asyncInfo.AllocateResumeStateId();
        var exceptionToFinallyLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(exceptionToFinallyStateId, exceptionToFinallyLabel);

        var exceptionInFinallyStateId = asyncInfo.AllocateResumeStateId();
        var exceptionInFinallyLabel = CreateLabel();
        asyncInfo.RegisterResumeLabel(exceptionInFinallyStateId, exceptionInFinallyLabel);

        int catchStateId = 0;
        int catchLabel = 0;
        if (hasCatch)
        {
            catchStateId = asyncInfo.AllocateResumeStateId();
            catchLabel = CreateLabel();
            asyncInfo.RegisterResumeLabel(catchStateId, catchLabel);
        }

        // Reset pending completion fields on entry.
        {
            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingReturnField, nullTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, falseTemp));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));
        }

        // --- Try block ---
        _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: pendingExceptionField,
            HasPendingExceptionFieldName: hasPendingExceptionField,
            PendingReturnFieldName: pendingReturnField,
            HasPendingReturnFieldName: hasPendingReturnField,
            IsInFinally: false));
        _asyncTryCatchStack.Push(new AsyncTryCatchContext(
            CatchStateId: hasCatch ? catchStateId : exceptionToFinallyStateId,
            CatchLabelId: hasCatch ? catchLabel : exceptionToFinallyLabel,
            PendingExceptionFieldName: pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.TryBlock))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
            _asyncTryFinallyStack.Pop();
        }

        // Normal completion flows into finally.
        _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));

        // --- Catch handler (synthetic) ---
        if (hasCatch)
        {
            _methodBodyIR.Instructions.Add(new LIRLabel(catchLabel));

            // Mark that we arrived due to an exception/rejection.
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            // Load pending exception into temp and clear it (the catch is handling it).
            var pendingTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, pendingTemp));
            DefineTempStorage(pendingTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            var nullTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNull(nullTemp));
            DefineTempStorage(nullTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, pendingExceptionField, nullTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, falseTemp));

            if (tryStmt.CatchParamBinding != null &&
                !TryStoreToBinding(tryStmt.CatchParamBinding, pendingTemp, out _))
            {
                return false;
            }

            _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
                FinallyEntryLabelId: finallyEntryLabel,
                FinallyExitLabelId: finallyExitLabel,
                PendingExceptionFieldName: pendingExceptionField,
                HasPendingExceptionFieldName: hasPendingExceptionField,
                PendingReturnFieldName: pendingReturnField,
                HasPendingReturnFieldName: hasPendingReturnField,
                IsInFinally: false));
            _asyncTryCatchStack.Push(new AsyncTryCatchContext(
                CatchStateId: exceptionToFinallyStateId,
                CatchLabelId: exceptionToFinallyLabel,
                PendingExceptionFieldName: pendingExceptionField));
            try
            {
                if (tryStmt.CatchBody != null && !TryLowerStatement(tryStmt.CatchBody))
                {
                    return false;
                }
            }
            finally
            {
                _asyncTryCatchStack.Pop();
                _asyncTryFinallyStack.Pop();
            }

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));
        }

        // --- Exception path into finally (synthetic; used for await rejection / throw) ---
        _methodBodyIR.Instructions.Add(new LIRLabel(exceptionToFinallyLabel));
        {
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyEntryLabel));
        }

        // --- Finally block ---
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyEntryLabel));
        _asyncTryFinallyStack.Push(new AsyncTryFinallyContext(
            FinallyEntryLabelId: finallyEntryLabel,
            FinallyExitLabelId: finallyExitLabel,
            PendingExceptionFieldName: pendingExceptionField,
            HasPendingExceptionFieldName: hasPendingExceptionField,
            PendingReturnFieldName: pendingReturnField,
            HasPendingReturnFieldName: hasPendingReturnField,
            IsInFinally: true));
        _asyncTryCatchStack.Push(new AsyncTryCatchContext(
            CatchStateId: exceptionInFinallyStateId,
            CatchLabelId: exceptionInFinallyLabel,
            PendingExceptionFieldName: pendingExceptionField));
        try
        {
            if (!TryLowerStatement(tryStmt.FinallyBody))
            {
                return false;
            }
        }
        finally
        {
            _asyncTryCatchStack.Pop();
            _asyncTryFinallyStack.Pop();
        }
        _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));

        // --- Exception inside finally overrides prior completion ---
        _methodBodyIR.Instructions.Add(new LIRLabel(exceptionInFinallyLabel));
        {
            var trueTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(true, trueTemp));
            DefineTempStorage(trueTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingExceptionField, trueTemp));

            var falseTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, falseTemp));
            DefineTempStorage(falseTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, hasPendingReturnField, falseTemp));

            _methodBodyIR.Instructions.Add(new LIRBranch(finallyExitLabel));
        }

        // --- After finally: dispatch based on completion ---
        _methodBodyIR.Instructions.Add(new LIRLabel(finallyExitLabel));

        var checkReturnLabel = CreateLabel();
        {
            var hasExTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingExceptionField, hasExTemp));
            DefineTempStorage(hasExTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            SetTempVariableSlot(hasExTemp, CreateAnonymousVariableSlot("$finally_hasEx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasExTemp, checkReturnLabel));

            var exTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingExceptionField, exTemp));
            DefineTempStorage(exTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (_asyncTryCatchStack.Count > 0)
            {
                var outer = _asyncTryCatchStack.Peek();
                _methodBodyIR.Instructions.Add(new LIRStoreScopeFieldByName(scopeName, outer.PendingExceptionFieldName, exTemp));
                _methodBodyIR.Instructions.Add(new LIRBranch(outer.CatchLabelId));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRAsyncReject(exTemp));
            }
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(checkReturnLabel));
        {
            var hasReturnTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, hasPendingReturnField, hasReturnTemp));
            DefineTempStorage(hasReturnTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            SetTempVariableSlot(hasReturnTemp, CreateAnonymousVariableSlot("$finally_hasReturn", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool))));

            _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(hasReturnTemp, afterTryLabel));

            var retTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRLoadScopeFieldByName(scopeName, pendingReturnField, retTemp));
            DefineTempStorage(retTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            _methodBodyIR.Instructions.Add(new LIRReturn(retTemp));
        }

        _methodBodyIR.Instructions.Add(new LIRLabel(afterTryLabel));
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
            case HIRAwaitExpression awaitExpr:
                {
                    // await is only supported inside async functions
                    if (!_isAsync || _methodBodyIR.AsyncInfo == null)
                    {
                        IRPipelineMetrics.RecordFailure("await expression found outside async function context");
                        return false;
                    }

                    // Lower the awaited expression first
                    if (!TryLowerExpression(awaitExpr.Argument, out var awaitedValueTemp))
                    {
                        return false;
                    }

                    // Ensure the awaited value is boxed to object
                    awaitedValueTemp = EnsureObject(awaitedValueTemp);

                    // Allocate state ID and label for resumption
                    var asyncInfo = _methodBodyIR.AsyncInfo;
                    var awaitId = asyncInfo.AllocateAwaitId();
                    var resumeStateId = asyncInfo.AllocateResumeStateId();
                    var resumeLabel = CreateLabel();

                    // Create result temp for the await expression
                    resultTempVar = CreateTempVariable();
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    // Record the await point
                    asyncInfo.AwaitPoints.Add(new AwaitPointInfo
                    {
                        AwaitId = awaitId,
                        ResumeStateId = resumeStateId,
                        ResumeLabelId = resumeLabel,
                        ResultTemp = resultTempVar
                    });

                    asyncInfo.RegisterResumeLabel(resumeStateId, resumeLabel);

                    int? rejectStateId = null;
                    string? pendingExceptionField = null;
                    if (_asyncTryCatchStack.Count > 0)
                    {
                        var ctx = _asyncTryCatchStack.Peek();
                        rejectStateId = ctx.CatchStateId;
                        pendingExceptionField = ctx.PendingExceptionFieldName;
                    }

                    // Emit the await instruction
                    _methodBodyIR.Instructions.Add(new LIRAwait(
                        awaitedValueTemp,
                        awaitId,
                        resumeStateId,
                        resumeLabel,
                        resultTempVar,
                        rejectStateId,
                        pendingExceptionField));

                    return true;
                }

            case HIRScopesArrayExpression:
                // Only currently emitted for constructors that receive a scopes argument.
                if (_callableKind != CallableKind.Constructor)
                {
                    return false;
                }
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadScopesArgument(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));
                return true;

            case HIRThisExpression:
                // PL3.5: ThisExpression.
                // Only supported for instance callables where IL arg0 is the receiver.
                if (_callableKind is not CallableKind.ClassMethod
                    and not CallableKind.Constructor
                    and not CallableKind.Function
                    and not CallableKind.ModuleMain)
                {
                    return false;
                }

                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadThis(resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;

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
                if (!TryLowerCallExpression(callExpr, out resultTempVar))
                {
                    if (callExpr.Callee is HIRPropertyAccessExpression pa)
                    {
                        var recv = pa.Object;
                        var recvDesc = recv switch
                        {
                            HIRVariableExpression ve => $"{ve.Name.Name} ({ve.Name.Kind})",
                            _ => recv.GetType().Name
                        };
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering CallExpression (property '{pa.PropertyName}' on {recvDesc})");
                    }
                    else
                    {
                        IRPipelineMetrics.RecordFailureIfUnset($"HIR->LIR: failed lowering CallExpression (callee={callExpr.Callee.GetType().Name})");
                    }
                    return false;
                }
                return true;

            case HIRNewExpression newExpr:
                if (!TryLowerNewExpression(newExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure($"HIR->LIR: failed lowering NewExpression (callee={newExpr.Callee.GetType().Name})");
                    return false;
                }
                return true;

            case HIRUnaryExpression unaryExpr:
                return TryLowerUnaryExpression(unaryExpr, out resultTempVar);

            case HIRUpdateExpression updateExpr:
                return TryLowerUpdateExpression(updateExpr, out resultTempVar);

            case HIRTemplateLiteralExpression templateLiteral:
                if (!TryLowerTemplateLiteralExpression(templateLiteral, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering TemplateLiteralExpression");
                    return false;
                }
                return true;

            case HIRAssignmentExpression assignExpr:
                return TryLowerAssignmentExpression(assignExpr, out resultTempVar);

            case HIRPropertyAssignmentExpression propAssignExpr:
                return TryLowerPropertyAssignmentExpression(propAssignExpr, out resultTempVar);

            case HIRIndexAssignmentExpression indexAssignExpr:
                return TryLowerIndexAssignmentExpression(indexAssignExpr, out resultTempVar);

            case HIRDestructuringAssignmentExpression destructAssignExpr:
                return TryLowerDestructuringAssignmentExpression(destructAssignExpr, out resultTempVar);

            case HIRArrayExpression arrayExpr:
                if (!TryLowerArrayExpression(arrayExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure("HIR->LIR: failed lowering ArrayExpression");
                    return false;
                }
                return true;

            case HIRObjectExpression objectExpr:
                return TryLowerObjectExpression(objectExpr, out resultTempVar);

            case HIRPropertyAccessExpression propAccessExpr:
                if (!TryLowerPropertyAccessExpression(propAccessExpr, out resultTempVar))
                {
                    IRPipelineMetrics.RecordFailure($"HIR->LIR: failed lowering PropertyAccessExpression (property='{propAccessExpr.PropertyName}')");
                    return false;
                }
                return true;

            case HIRLoadUserClassInstanceFieldExpression loadUserField:
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    loadUserField.RegistryClassName,
                    loadUserField.FieldName,
                    loadUserField.IsPrivateField,
                    resultTempVar));
                var stableFieldType = TryGetStableThisFieldClrType(loadUserField.FieldName);
                DefineTempStorage(resultTempVar, GetPreferredFieldReadStorage(stableFieldType));
                return true;

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
                    if (storage == null && _scope != null)
                    {
                        // Fallback: if the environment layout didn't include this binding (e.g., due to
                        // a BindingInfo identity mismatch or overly-conservative storage map), try to
                        // compute scope-field storage from the caller's scope chain.
                        //
                        // This is only valid for captured bindings that are stored as fields on their
                        // declaring scope type.
                        if (binding.IsCaptured)
                        {
                            var declaringScope = _scope;
                            while (declaringScope != null)
                            {
                                if (declaringScope.Bindings.TryGetValue(binding.Name, out var candidate)
                                    && ReferenceEquals(candidate, binding))
                                {
                                    break;
                                }
                                declaringScope = declaringScope.Parent;
                            }

                            if (declaringScope != null)
                            {
                                var declaringRegistryName = ScopeNaming.GetRegistryScopeName(declaringScope);
                                var scopeId = new ScopeId(declaringRegistryName);
                                var fieldId = new FieldId(declaringRegistryName, binding.Name);

                                if (!ReferenceEquals(declaringScope, _scope))
                                {
                                    var parentIndex = _environmentLayout.ScopeChain.IndexOf(declaringRegistryName);
                                    if (parentIndex >= 0)
                                    {
                                        storage = BindingStorage.ForParentScopeField(fieldId, scopeId, parentIndex);
                                    }
                                }
                                else
                                {
                                    storage = BindingStorage.ForLeafScopeField(fieldId, scopeId);
                                }
                            }
                        }
                    }
                    if (storage != null)
                    {
                        static ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
                        {
                            // Propagate unboxed primitives for stable inferred types. This matches the current
                            // typed-scope-field support in TypeGenerator/VariableRegistry.
                            //
                            // Note: We only use unboxed storage when the runtime semantics are identical:
                            // - bool: JS ToBoolean(bool) is the identity (so conditionals can branch directly).
                            // - double: used for numeric IL paths (e.g., add) and boxed on demand.
                            if (b.IsStableType)
                            {
                                if (b.ClrType == typeof(double))
                                {
                                    return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
                                }
                                if (b.ClrType == typeof(bool))
                                {
                                    return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool));
                                }
                                if (b.ClrType == typeof(JavaScriptRuntime.Array))
                                {
                                    // Keep as a typed reference so downstream lowering can emit direct instance calls
                                    // (e.g., arr.join(), arr.push(...)) without generic dispatch.
                                    return new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array));
                                }
                            }

                            return new ValueStorage(ValueStorageKind.Reference, typeof(object));
                        }

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
                                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
                                    return true;
                                }
                                break;

                            case BindingStorageKind.ParentScopeField:
                                // Captured variable in parent scope - load from parent scope field
                                if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                                {
                                    resultTempVar = CreateTempVariable();
                                    _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, resultTempVar));
                                    DefineTempStorage(resultTempVar, GetPreferredBindingReadStorage(binding));
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
                    // Intrinsic globals (e.g., console, process, Infinity, NaN) are exposed via JavaScriptRuntime.GlobalThis.
                    // If this identifier is a Global binding and maps to a GlobalThis static property, emit a load.
                    if (varExpr.Name.Kind == BindingKind.Global)
                    {
                        var globalName = varExpr.Name.Name;
                        var gvType = typeof(JavaScriptRuntime.GlobalThis);
                        var gvProp = gvType.GetProperty(globalName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                        if (gvProp != null)
                        {
                            resultTempVar = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRGetIntrinsicGlobal(globalName, resultTempVar));
                            // Track the concrete CLR type when known (e.g., console -> JavaScriptRuntime.Console)
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, gvProp.PropertyType));
                            return true;
                        }
                    }

                    // Function declarations are compiled separately and are not SSA-assigned in the HIR body.
                    // When a function declaration identifier is used as a value (e.g., returned or assigned),
                    // create a delegate and bind it to the appropriate scopes array.
                    if (varExpr.Name.BindingInfo.Kind == BindingKind.Function)
                    {
                        var callableId = TryCreateCallableIdForFunctionDeclaration(varExpr.Name);
                        if (callableId == null)
                        {
                            return false;
                        }

                        var scopesTemp = CreateTempVariable();
                        if (!TryBuildScopesArrayForCallee(varExpr.Name, scopesTemp))
                        {
                            return false;
                        }
                        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                        resultTempVar = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCreateBoundFunctionExpression(callableId, scopesTemp, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        return true;
                    }

                    IRPipelineMetrics.RecordFailureIfUnset(
                        $"HIR->LIR: no storage for variable '{binding.Name}' (kind={binding.Kind}, captured={binding.IsCaptured}, hasEnv={_environmentLayout != null}, scope='{_scope?.GetQualifiedName() ?? "<null>"}')");
                    return false;
                }

                // Variable reads are SSA value lookups (no load instruction).
                return true;

            case HIRArrowFunctionExpression arrowExpr:
                return TryLowerArrowFunctionExpression(arrowExpr, out resultTempVar);
            case HIRFunctionExpression funcExpr:
                return TryLowerFunctionExpression(funcExpr, out resultTempVar);
            // Handle different expression types here
            default:
                // Unsupported expression type
                IRPipelineMetrics.RecordFailure($"HIR->LIR: unsupported expression type {expression.GetType().Name}");
                return false;
        }
    }

    private bool TryLowerFunctionExpression(HIRFunctionExpression funcExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Scope is resolved during HIR construction (avoid keeping AST nodes in HIR).
        var funcScope = funcExpr.FunctionScope;

        // Build scopes[] to bind for closure semantics.
        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClosureBinding(funcScope, scopesTemp))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCreateBoundFunctionExpression(
            CallableId: funcExpr.CallableId,
            ScopesArray: scopesTemp,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryLowerArrowFunctionExpression(HIRArrowFunctionExpression arrowExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (_scope == null)
        {
            return false;
        }

        // Scope is resolved during HIR construction (avoid keeping AST nodes in HIR).
        var arrowScope = arrowExpr.FunctionScope;

        // Build scopes[] to bind for closure semantics.
        var scopesTemp = CreateTempVariable();
        if (!TryBuildScopesArrayForClosureBinding(arrowScope, scopesTemp))
        {
            return false;
        }
        DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCreateBoundArrowFunction(
            CallableId: arrowExpr.CallableId,
            ScopesArray: scopesTemp,
            Result: resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryBuildScopesArrayForClosureBinding(Scope calleeScope, TempVariable resultTemp)
    {
        // Mirror the call-site logic: even when the callee doesn't reference parent variables,
        // preserve the ABI expectation that scopes[0] is the global scope when available.
        if (!calleeScope.ReferencesParentScopeVariables)
        {
            if (_scope == null)
            {
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
                return true;
            }

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

        return TryBuildScopesArrayFromLayout(calleeScope, CallableKind.Function, resultTemp);
    }

    private static Scope? FindScopeByAstNode(Node astNode, Scope current)
    {
        if (ReferenceEquals(current.AstNode, astNode))
        {
            return current;
        }

        foreach (var child in current.Children)
        {
            var found = FindScopeByAstNode(astNode, child);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private bool TryLowerTemplateLiteralExpression(HIRTemplateLiteralExpression templateLiteral, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        var quasis = templateLiteral.Quasis;
        var exprs = templateLiteral.Expressions;

        // Start with the first quasi (or empty string if missing).
        var current = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(quasis.Count > 0 ? quasis[0] : string.Empty, current));
        DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        for (int i = 0; i < exprs.Count; i++)
        {
            if (!TryLowerExpression(exprs[i], out var exprTemp))
            {
                return false;
            }

            // Interpolations are converted to string using JS semantics.
            var exprAsString = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConvertToString(exprTemp, exprAsString));
            DefineTempStorage(exprAsString, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

            var concat1 = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConcatStrings(current, exprAsString, concat1));
            DefineTempStorage(concat1, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
            current = concat1;

            // Append the tail quasi (if present). Missing quasis are treated as empty string.
            var tail = (i + 1) < quasis.Count ? quasis[i + 1] : string.Empty;
            if (!string.IsNullOrEmpty(tail))
            {
                var tailTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstString(tail, tailTemp));
                DefineTempStorage(tailTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                var concat2 = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConcatStrings(current, tailTemp, concat2));
                DefineTempStorage(concat2, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                current = concat2;
            }
        }

        resultTempVar = current;
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
        return true;
    }

    private bool TryLowerNewExpression(HIRNewExpression newExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (newExpr.Callee is not HIRVariableExpression calleeVar)
        {
            return false;
        }

        // User-defined class: `new ClassName(...)`
        // Note: top-level classes live in the global scope but still have a declaration node.
        if (calleeVar.Name.BindingInfo.DeclarationNode is ClassDeclaration declaredClass)
        {
            return TryLowerNewUserDefinedClass(declaredClass, newExpr.Arguments, out resultTempVar);
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
                // JS semantics: new Number() defaults to +0 (like Number())
                resultTempVar = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                return true;
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

        // Match ClassesGenerator registry key convention: "{ns}.{typeName}".
        // This allows IL emission to look up type/field handles for the class.
        var registryClassName = $"{(classScope.DotNetNamespace ?? "Classes")}.{(classScope.DotNetTypeName ?? classScope.Name)}";

        bool needsScopes = DoesClassNeedParentScopes(classDecl, classScope);
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

        int minArgs = 0;
        int maxArgs = 0;
        int jsParamCount = 0;
        if (ctorMember?.Value is FunctionExpression ctorFunc)
        {
            jsParamCount = ctorFunc.Params.Count;
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

        // Build a stable CallableId for the constructor so LIR remains AST-free.
        // This mirrors CallableDiscovery.DiscoverClass.
        var moduleName = rootScope.Name;
        string declaringScopeName;
        if (classScope.Parent == null || classScope.Parent.Kind == ScopeKind.Global)
        {
            declaringScopeName = moduleName;
        }
        else
        {
            declaringScopeName = $"{moduleName}/{classScope.Parent.GetQualifiedName()}";
        }

        var className = classDecl.Id is Identifier cid ? cid.Name : classScope.Name;
        var ctorCallableId = new TwoPhase.CallableId
        {
            Kind = TwoPhase.CallableKind.ClassConstructor,
            DeclaringScopeName = declaringScopeName,
            Name = className,
            JsParamCount = jsParamCount,
            AstNode = null
        };

        resultTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewUserClass(
            ClassName: className,
            RegistryClassName: registryClassName,
            ConstructorCallableId: ctorCallableId,
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

            // PL8.1: Primitive conversion callables: String(x), Number(x), Boolean(x).
            // These are CallExpression forms (not NewExpression) and should lower to runtime conversions.
            // Semantics:
            // - No args: String() => "", Number() => 0, Boolean() => false
            // - Extra args are evaluated for side-effects and ignored
            if (symbol.Kind == BindingKind.Global)
            {
                var name = symbol.Name;
                if (string.Equals(name, "String", StringComparison.Ordinal)
                    || string.Equals(name, "Number", StringComparison.Ordinal)
                    || string.Equals(name, "Boolean", StringComparison.Ordinal))
                {
                    if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var conversionArgs))
                    {
                        return false;
                    }

                    var firstArg = conversionArgs.Count > 0 ? conversionArgs[0] : (TempVariable?)null;

                    if (string.Equals(name, "String", StringComparison.Ordinal))
                    {
                        if (firstArg == null)
                        {
                            _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                            return true;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConvertToString(firstArg.Value, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;
                    }

                    if (string.Equals(name, "Number", StringComparison.Ordinal))
                    {
                        if (firstArg == null)
                        {
                            _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, resultTempVar));
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                            return true;
                        }

                        _methodBodyIR.Instructions.Add(new LIRConvertToNumber(firstArg.Value, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;
                    }

                    // Boolean
                    if (firstArg == null)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;
                    }

                    _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(firstArg.Value, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

                var intrinsicInfo = JavaScriptRuntime.IntrinsicObjectRegistry.GetInfo(name);
                if (intrinsicInfo != null && intrinsicInfo.CallKind != JavaScriptRuntime.IntrinsicCallKind.None)
                {
                    switch (intrinsicInfo.CallKind)
                    {
                        case JavaScriptRuntime.IntrinsicCallKind.BuiltInError:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var errorArgs))
                                {
                                    return false;
                                }

                                var messageTemp = errorArgs.Count > 0 ? errorArgs[0] : (TempVariable?)null;

                                _methodBodyIR.Instructions.Add(new LIRNewBuiltInError(intrinsicInfo.Name, messageTemp, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ArrayConstruct:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, callExpr.Arguments.Length, out var argTemps))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicInfo.Name, "Construct", argTemps, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ObjectConstruct:
                            {
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 1, out var objectArgs))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(intrinsicInfo.Name, "Construct", objectArgs, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.DateToString:
                            {
                                // ECMAScript Date() called as a function ignores all arguments and
                                // returns the current date/time as a string. Arguments are still
                                // evaluated for side effects, but none are passed to the constructor.
                                if (!TryEvaluateCallArguments(callExpr.Arguments, 0, out var _))
                                {
                                    return false;
                                }

                                var dateTemp = CreateTempVariable();
                                _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(intrinsicInfo.Name, Array.Empty<TempVariable>(), dateTemp));
                                DefineTempStorage(dateTemp, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Date)));

                                _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                                    dateTemp,
                                    typeof(JavaScriptRuntime.Date),
                                    nameof(JavaScriptRuntime.Date.toISOString),
                                    Array.Empty<TempVariable>(),
                                    resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                                return true;
                            }

                        case JavaScriptRuntime.IntrinsicCallKind.ConstructorLike:
                            {
                                var maxUsed = Math.Min(callExpr.Arguments.Length, 2);
                                if (!TryEvaluateCallArguments(callExpr.Arguments, maxUsed, out var argTemps))
                                {
                                    return false;
                                }

                                _methodBodyIR.Instructions.Add(new LIRNewIntrinsicObject(intrinsicInfo.Name, argTemps, resultTempVar));
                                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                                return true;
                            }
                    }
                }
            }

            // Case 1.0: Intrinsic global function call (e.g., setTimeout(...)).
            // These are exposed as public static methods on JavaScriptRuntime.GlobalThis.
            // We lower them directly rather than trying to load them as a value.
            if (symbol.Kind == BindingKind.Global)
            {
                var globalFunctionName = symbol.Name;

                // PL8.1: Primitive conversion callables: String(x), Number(x), Boolean(x)
                // Distinct from `new String(...)` sugar handled in NewExpression lowering.
                if (string.Equals(globalFunctionName, "String", StringComparison.OrdinalIgnoreCase))
                {
                    if (callExpr.Arguments.Length > 1)
                    {
                        return false;
                    }

                    // String() with no args returns empty string.
                    if (callExpr.Arguments.Length == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstString(string.Empty, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                        return true;
                    }

                    if (!TryLowerExpression(callExpr.Arguments[0], out var argTemp))
                    {
                        return false;
                    }

                    var source = EnsureObject(argTemp);
                    _methodBodyIR.Instructions.Add(new LIRConvertToString(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Number", StringComparison.OrdinalIgnoreCase))
                {
                    if (callExpr.Arguments.Length > 1)
                    {
                        return false;
                    }

                    // Number() with no args returns +0.
                    if (callExpr.Arguments.Length == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstNumber(0.0, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                        return true;
                    }

                    if (!TryLowerExpression(callExpr.Arguments[0], out var argTemp))
                    {
                        return false;
                    }

                    var source = EnsureObject(argTemp);
                    _methodBodyIR.Instructions.Add(new LIRConvertToNumber(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }

                if (string.Equals(globalFunctionName, "Boolean", StringComparison.OrdinalIgnoreCase))
                {
                    if (callExpr.Arguments.Length > 1)
                    {
                        return false;
                    }

                    // Boolean() with no args returns false.
                    if (callExpr.Arguments.Length == 0)
                    {
                        _methodBodyIR.Instructions.Add(new LIRConstBoolean(false, resultTempVar));
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                        return true;
                    }

                    if (!TryLowerExpression(callExpr.Arguments[0], out var argTemp))
                    {
                        return false;
                    }

                    var source = EnsureObject(argTemp);
                    _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(source, resultTempVar));
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

                var gvType = typeof(JavaScriptRuntime.GlobalThis);
                var gvMethod = gvType.GetMethod(
                    globalFunctionName,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);

                if (gvMethod != null)
                {
                    var argTemps = new List<TempVariable>();
                    foreach (var arg in callExpr.Arguments)
                    {
                        if (!TryLowerExpression(arg, out var argTemp))
                        {
                            return false;
                        }
                        argTemps.Add(EnsureObject(argTemp));
                    }

                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicGlobalFunction(globalFunctionName, argTemps, resultTempVar));
                    if (gvMethod.ReturnType == typeof(double))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    }
                    else if (gvMethod.ReturnType == typeof(bool))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    }
                    else if (gvMethod.ReturnType == typeof(string))
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                    }
                    else
                    {
                        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    }
                    return true;
                }
            }
            
            // If the callee is not a direct function binding (e.g., it's a local/const holding a closure),
            // invoke via runtime dispatch (Closure.InvokeWithArgs).
            if (symbol.Kind != BindingKind.Function)
            {
                // Lower callee value
                if (!TryLowerExpression(funcVarExpr, out var calleeTemp))
                {
                    return false;
                }
                calleeTemp = EnsureObject(calleeTemp);

                // Lower arguments and pack into an object[]
                var callArgTemps = new List<TempVariable>();
                foreach (var arg in callExpr.Arguments)
                {
                    if (!TryLowerExpression(arg, out var argTemp))
                    {
                        return false;
                    }
                    callArgTemps.Add(EnsureObject(argTemp));
                }

                var argsArrayTemp = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRBuildArray(callArgTemps, argsArrayTemp));
                DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                // Build a scopes array for the current context. Bound closures ignore the passed scopes,
                // but unbound function values still require a scopes array.
                var scopesTemp = CreateTempVariable();
                if (!TryBuildCurrentScopesArray(scopesTemp))
                {
                    return false;
                }
                DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(calleeTemp, scopesTemp, argsArrayTemp, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
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

        // Case 1b: Indirect call where the callee is an expression value (e.g., IIFE:
        // (function() { ... })(), (() => 1)(), or getFn()()).
        // Exclude property-access calls here to avoid accidentally breaking method-call semantics
        // that are handled by intrinsic/typed-member lowering below.
        if (callExpr.Callee is not HIRVariableExpression && callExpr.Callee is not HIRPropertyAccessExpression)
        {
            if (!TryLowerExpression(callExpr.Callee, out var calleeTemp))
            {
                return false;
            }
            calleeTemp = EnsureObject(calleeTemp);

            var callArgTemps = new List<TempVariable>();
            foreach (var arg in callExpr.Arguments)
            {
                if (!TryLowerExpression(arg, out var argTemp))
                {
                    return false;
                }
                callArgTemps.Add(EnsureObject(argTemp));
            }

            var argsArrayTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRBuildArray(callArgTemps, argsArrayTemp));
            DefineTempStorage(argsArrayTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            var scopesTemp = CreateTempVariable();
            if (!TryBuildCurrentScopesArray(scopesTemp))
            {
                return false;
            }
            DefineTempStorage(scopesTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

            _methodBodyIR.Instructions.Add(new LIRCallFunctionValue(calleeTemp, scopesTemp, argsArrayTemp, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

        // Case 2: Property access call (e.g., console.log, Array.isArray, Math.abs)
        if (callExpr.Callee is not HIRPropertyAccessExpression calleePropAccess)
        {
            return false;
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
                    // Choose the same overload we will emit in IL (see LIRToILCompiler.EmitIntrinsicStaticCall)
                    var argCount = callExpr.Arguments.Count();
                    System.Reflection.MethodInfo? chosen = null;
                    foreach (var mi in staticMethods)
                    {
                        if (mi.GetParameters().Length == argCount)
                        {
                            chosen = mi;
                            break;
                        }
                    }

                    if (chosen == null)
                    {
                        foreach (var mi in staticMethods)
                        {
                            var ps = mi.GetParameters();
                            if (ps.Length == 1 && ps[0].ParameterType == typeof(object[]))
                            {
                                chosen = mi;
                                break;
                            }
                        }
                    }
                    // If we can't select a compatible overload for the intrinsic static call,
                    // fall back to generic member-dispatch below.
                    if (chosen != null)
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

                        // Track the correct CLR type to prevent invalid IL (e.g., storing bool into an object local).
                        var retType = chosen.ReturnType;
                        if (retType == typeof(void))
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                        }
                        else if (retType.IsValueType)
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, retType));
                        }
                        else
                        {
                            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, retType));
                        }
                        return true;
                    }
                }
            }
        }

        // Case 2b.2: User-defined class static method call (e.g., Greeter.helloWorld()).
        // If the receiver is a class identifier (ClassDeclaration binding) and the member is a static method,
        // emit a direct call to the declared method token via CallableRegistry.
        if (calleePropAccess.Object is HIRVariableExpression classVarExpr &&
            classVarExpr.Name.BindingInfo.DeclarationNode is ClassDeclaration classDecl)
        {
            var className = classVarExpr.Name.Name;
            var memberName = calleePropAccess.PropertyName;

            var member = classDecl.Body.Body
                .OfType<MethodDefinition>()
                .FirstOrDefault(m =>
                    m.Static &&
                    m.Key is Identifier kid &&
                    string.Equals(kid.Name, memberName, StringComparison.Ordinal));

            if (member?.Value is FunctionExpression memberFunc)
            {
                // Create a CallableId that matches CallableDiscovery conventions.
                var callableId = TryCreateCallableIdForClassStaticMethod(classVarExpr.Name, member, memberName, memberFunc.Params.Count);
                if (callableId == null)
                {
                    return false;
                }

                // Lower all arguments (evaluate extras for side effects, but only pass up to declared param count).
                var declaredParamCount = memberFunc.Params.Count;
                var callArgTemps = new List<TempVariable>(declaredParamCount);

                for (int i = 0; i < callExpr.Arguments.Length; i++)
                {
                    if (!TryLowerExpression(callExpr.Arguments[i], out var argTemp))
                    {
                        return false;
                    }

                    argTemp = EnsureObject(argTemp);

                    if (i < declaredParamCount)
                    {
                        callArgTemps.Add(argTemp);
                    }
                    // else: evaluated for side effects, result intentionally ignored
                }

                // Pad missing args with undefined (null) to match the declared signature.
                while (callArgTemps.Count < declaredParamCount)
                {
                    var undefTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstUndefined(undefTemp));
                    DefineTempStorage(undefTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    callArgTemps.Add(undefTemp);
                }

                _methodBodyIR.Instructions.Add(new LIRCallDeclaredCallable(callableId, callArgTemps, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }
        }

        // Lower receiver once for instance/member call cases below.
        // IMPORTANT: Do not lower the receiver more than once; it may have side effects (e.g. promise chaining).
        if (!TryLowerExpression(calleePropAccess.Object, out var receiverTempVar))
        {
            return false;
        }

        // Case 2a: Typed Array instance method calls (e.g., arr.join(), arr.push(...)).
        // If the receiver CLR type is known to be JavaScriptRuntime.Array, emit a typed instance call.
        {
            var receiverStorage = GetTempStorage(receiverTempVar);
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
                    receiverTempVar,
                    typeof(JavaScriptRuntime.Array),
                    calleePropAccess.PropertyName,
                    arrayArgTemps,
                    resultTempVar));

                // Methods with stable unboxed primitive returns.
                if (string.Equals(calleePropAccess.PropertyName, "some", StringComparison.OrdinalIgnoreCase))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                    return true;
                }

                // Track a more precise runtime type when we know it, so chained calls can lower.
                // Example: arr.slice(...).join(',') requires the result of slice() to be treated as an Array receiver.
                var returnClrType = (string.Equals(calleePropAccess.PropertyName, "slice", StringComparison.OrdinalIgnoreCase)
                                     || string.Equals(calleePropAccess.PropertyName, "map", StringComparison.OrdinalIgnoreCase))
                    ? typeof(JavaScriptRuntime.Array)
                    : typeof(object);
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, returnClrType));
                return true;
            }

            // Case 2a.2: Typed Console instance method calls (e.g., console.log(...)).
            // The console intrinsic is a known runtime type and exposes instance methods; calling them directly
            // avoids generic dispatch and keeps generator output stable.
            if (receiverStorage.Kind == ValueStorageKind.Reference && receiverStorage.ClrType == typeof(JavaScriptRuntime.Console))
            {
                var consoleArgTemps = new List<TempVariable>();
                foreach (var argExpr in callExpr.Arguments)
                {
                    if (!TryLowerExpression(argExpr, out var argTempVar))
                    {
                        return false;
                    }
                    consoleArgTemps.Add(EnsureObject(argTempVar));
                }

                _methodBodyIR.Instructions.Add(new LIRCallInstanceMethod(
                    receiverTempVar,
                    typeof(JavaScriptRuntime.Console),
                    calleePropAccess.PropertyName,
                    consoleArgTemps,
                    resultTempVar));

                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                return true;
            }

            // Case 2a.3: Direct calls to known instance methods on the current user-defined class.
            // Example: `this.setBitTrue(x)` inside a class method can be emitted as a direct callvirt
            // rather than runtime dispatch through Object.CallMember.
            if (_classRegistry != null
                && calleePropAccess.Object is HIRThisExpression
                && TryGetEnclosingClassRegistryName(out var currentClass)
                && currentClass != null
                && _classRegistry.TryGetMethod(currentClass, calleePropAccess.PropertyName, out var methodHandle, out _, out var methodReturnClrType, out _, out var maxParamCount))
            {
                var argTemps = new List<TempVariable>();
                foreach (var argExpr in callExpr.Arguments)
                {
                    if (!TryLowerExpression(argExpr, out var argTempVar))
                    {
                        return false;
                    }

                    argTemps.Add(EnsureObject(argTempVar));
                }

                _methodBodyIR.Instructions.Add(new LIRCallUserClassInstanceMethod(
                    currentClass,
                    calleePropAccess.PropertyName,
                    methodHandle,
                    maxParamCount,
                    argTemps,
                    resultTempVar));

                // Propagate typed return storage when available.
                if (methodReturnClrType == typeof(double))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                }
                else if (methodReturnClrType == typeof(bool))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
                }
                else if (methodReturnClrType == typeof(string))
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(string)));
                }
                else
                {
                    DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                }
                return true;
            }
        }

        // Case 2c: Generic member call via runtime dispatcher.
        // This is a catch-all for calls like `output.join(',')` where `output` may be boxed as object,
        // so typed receiver lowering can't prove the receiver type.
        receiverTempVar = EnsureObject(receiverTempVar);

        var argTempsGeneric = new List<TempVariable>();
        foreach (var argExpr in callExpr.Arguments)
        {
            if (!TryLowerExpression(argExpr, out var argTempVar))
            {
                return false;
            }

            argTempsGeneric.Add(EnsureObject(argTempVar));
        }

        var argsArrayTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRBuildArray(argTempsGeneric, argsArrayTempVar));
        DefineTempStorage(argsArrayTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

        _methodBodyIR.Instructions.Add(new LIRCallMember(receiverTempVar, calleePropAccess.PropertyName, argsArrayTempVar, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryEvaluateCallArguments(IReadOnlyList<HIRExpression> arguments, int usedCount, out List<TempVariable> usedTemps)
    {
        usedTemps = new List<TempVariable>(Math.Max(usedCount, 0));

        var maxUsed = Math.Max(0, usedCount);
        if (arguments.Count < maxUsed)
        {
            maxUsed = arguments.Count;
        }

        for (int i = 0; i < arguments.Count; i++)
        {
            if (!TryLowerExpression(arguments[i], out var argTemp))
            {
                return false;
            }

            if (i < maxUsed)
            {
                usedTemps.Add(EnsureObject(argTemp));
            }
        }

        return true;
    }

    private Js2IL.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForClassStaticMethod(
        Symbol classSymbol,
        MethodDefinition methodDef,
        string methodName,
        int declaredParamCount)
    {
        if (_scope == null)
        {
            return null;
        }

        var declaringScope = FindDeclaringScope(classSymbol.BindingInfo);
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

        var callableName = JavaScriptCallableNaming.MakeClassMethodCallableName(classSymbol.Name, methodName);
        var location = Js2IL.Services.TwoPhaseCompilation.SourceLocation.FromNode(methodDef);

        return new Js2IL.Services.TwoPhaseCompilation.CallableId
        {
            Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.ClassStaticMethod,
            DeclaringScopeName = declaringScopeName,
            Name = callableName,
            Location = location,
            JsParamCount = declaredParamCount,
            AstNode = methodDef
        };
    }

    /// <summary>
    /// Builds an object[] scopes array for the current caller context.
    /// This is used for indirect calls where we cannot statically determine the callee scope chain.
    /// </summary>
    private bool TryBuildCurrentScopesArray(TempVariable resultTemp)
    {
        if (_environmentLayout == null || _environmentLayout.ScopeChain.Slots.Count == 0)
        {
            _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
            return true;
        }

        var slotSources = new List<ScopeSlotSource>(_environmentLayout.ScopeChain.Slots.Count);
        foreach (var slot in _environmentLayout.ScopeChain.Slots)
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

    private Js2IL.Services.TwoPhaseCompilation.CallableId? TryCreateCallableIdForFunctionDeclaration(Symbol symbol)
    {
        if (_scope == null)
        {
            return null;
        }

        // IR lowering currently only supports direct calls where the callee is a function binding.
        if (symbol.BindingInfo.Kind != BindingKind.Function)
        {
            return null;
        }

        var declNode = symbol.BindingInfo.DeclarationNode;
        var declaringScope = FindDeclaringScope(symbol.BindingInfo);
        if (declaringScope == null)
        {
            return null;
        }

        // For named function expressions, the function name is bound inside the function scope,
        // but the callable itself is declared in the parent scope (Phase 1 discovery uses the parent).
        // If we detect that pattern, shift the DeclaringScopeName to the parent scope.
        var callableDeclaringScope = declaringScope;
        if (declNode is FunctionExpression funcExprDecl &&
            declaringScope.AstNode is FunctionExpression scopeFuncExpr &&
            ReferenceEquals(funcExprDecl, scopeFuncExpr) &&
            declaringScope.Parent != null)
        {
            callableDeclaringScope = declaringScope.Parent;
        }

        var root = callableDeclaringScope;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        var moduleName = root.Name;
        var declaringScopeName = callableDeclaringScope.Kind == ScopeKind.Global
            ? moduleName
            : $"{moduleName}/{callableDeclaringScope.GetQualifiedName()}";

        switch (declNode)
        {
            case FunctionDeclaration funcDecl:
                return new Js2IL.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.FunctionDeclaration,
                    DeclaringScopeName = declaringScopeName,
                    Name = symbol.Name,
                    JsParamCount = funcDecl.Params.Count,
                    AstNode = funcDecl
                };

            case FunctionExpression funcExpr:
                return new Js2IL.Services.TwoPhaseCompilation.CallableId
                {
                    Kind = Js2IL.Services.TwoPhaseCompilation.CallableKind.FunctionExpression,
                    DeclaringScopeName = declaringScopeName,
                    Name = (funcExpr.Id as Identifier)?.Name,
                    Location = Js2IL.Services.TwoPhaseCompilation.SourceLocation.FromNode(funcExpr),
                    JsParamCount = funcExpr.Params.Count,
                    AstNode = funcExpr
                };

            default:
                return null;
        }
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

            // Dynamic addition (unknown types). Prefer avoiding boxing if exactly one side is already an unboxed double.
            if (leftType == typeof(double) && rightType != typeof(double))
            {
                var rightBoxedForAdd = EnsureObject(rightTempVar);
                _methodBodyIR.Instructions.Add(new LIRAddDynamicDoubleObject(leftTempVar, rightBoxedForAdd, resultTempVar));
                DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.BoxedValue, typeof(object)));
                return true;
            }

            if (leftType != typeof(double) && rightType == typeof(double))
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
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRMulNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle subtraction
        if (binaryExpr.Operator == Acornima.Operator.Subtraction)
        {
            // JS '-' operator always follows numeric coercion (ToNumber) semantics.
            // Support both the fast path (double - double) and the general path via EnsureNumber.
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRSubNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle division
        if (binaryExpr.Operator == Acornima.Operator.Division)
        {
            // JS '/' operator follows ToNumber semantics.
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

            _methodBodyIR.Instructions.Add(new LIRDivNumber(leftTempVar, rightTempVar, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
            return true;
        }

        // Handle remainder (modulo)
        if (binaryExpr.Operator == Acornima.Operator.Remainder)
        {
            // JS '%' operator follows ToNumber semantics.
            if (leftType != typeof(double) || rightType != typeof(double))
            {
                leftTempVar = EnsureNumber(leftTempVar);
                rightTempVar = EnsureNumber(rightTempVar);
            }

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

    private bool TryLowerDestructuringPattern(HIRPattern pattern, TempVariable sourceValue, bool isDeclaration, string? sourceNameForError)
    {
        sourceValue = EnsureObject(sourceValue);

        switch (pattern)
        {
            case HIRIdentifierPattern id:
                if (isDeclaration)
                {
                    return TryDeclareBinding(id.Symbol, sourceValue);
                }
                else
                {
                    // Assignment to const is a runtime TypeError.
                    if (id.Symbol.BindingInfo.Kind == BindingKind.Const)
                    {
                        _methodBodyIR.Instructions.Add(new LIRThrowNewTypeError("Assignment to constant variable."));
                        return true;
                    }

                    return TryStoreToBinding(id.Symbol.BindingInfo, sourceValue, out _);
                }

            case HIRDefaultPattern def:
                {
                    // Apply default only when the incoming value is undefined (null).
                    var notNullLabel = CreateLabel();
                    var endLabel = CreateLabel();

                    var selected = CreateTempVariable();
                    DefineTempStorage(selected, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                    _methodBodyIR.Instructions.Add(new LIRBranchIfTrue(sourceValue, notNullLabel));

                    if (!TryLowerExpression(def.Default, out var defaultTemp))
                    {
                        return false;
                    }
                    defaultTemp = EnsureObject(defaultTemp);
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(defaultTemp, selected));
                    _methodBodyIR.Instructions.Add(new LIRBranch(endLabel));

                    _methodBodyIR.Instructions.Add(new LIRLabel(notNullLabel));
                    _methodBodyIR.Instructions.Add(new LIRCopyTemp(sourceValue, selected));
                    _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

                    return TryLowerDestructuringPattern(def.Target, selected, isDeclaration, sourceNameForError);
                }

            case HIRRestPattern rest:
                // Rest patterns are materialized by the containing object/array pattern.
                return TryLowerDestructuringPattern(rest.Target, sourceValue, isDeclaration, sourceNameForError);

            case HIRObjectPattern obj:
                {
                    EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(obj));

                    // Collect excluded keys for object rest.
                    var excludedKeyTemps = new List<TempVariable>(obj.Properties.Count);

                    foreach (var prop in obj.Properties)
                    {
                        var keyTemp = EmitConstString(prop.Key);
                        excludedKeyTemps.Add(keyTemp);
                        var getResult = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceValue, EnsureObject(keyTemp), getResult));
                        DefineTempStorage(getResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(prop.Value, getResult, isDeclaration, prop.Key))
                        {
                            return false;
                        }
                    }

                    if (obj.Rest != null)
                    {
                        var excludedArray = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRBuildArray(excludedKeyTemps, excludedArray));
                        DefineTempStorage(excludedArray, new ValueStorage(ValueStorageKind.Reference, typeof(object[])));

                        var restObj = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                            IntrinsicName: "Object",
                            MethodName: nameof(JavaScriptRuntime.Object.Rest),
                            Arguments: new[] { EnsureObject(sourceValue), excludedArray },
                            Result: restObj));
                        DefineTempStorage(restObj, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(obj.Rest.Target, restObj, isDeclaration, "rest"))
                        {
                            return false;
                        }
                    }

                    return true;
                }

            case HIRArrayPattern arr:
                {
                    EmitDestructuringNullGuard(sourceValue, sourceNameForError, GetFirstTargetNameForDestructuring(arr));

                    for (int i = 0; i < arr.Elements.Count; i++)
                    {
                        var elementPattern = arr.Elements[i];
                        if (elementPattern == null)
                        {
                            continue;
                        }

                        var indexTemp = EmitConstNumber(i);
                        var getResult = CreateTempVariable();
                        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceValue, indexTemp, getResult));
                        DefineTempStorage(getResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                        if (!TryLowerDestructuringPattern(elementPattern, getResult, isDeclaration, i.ToString()))
                        {
                            return false;
                        }
                    }

                    if (arr.Rest != null)
                    {
                        if (!TryBuildArrayRest(sourceValue, arr.Elements.Count, out var restArray))
                        {
                            return false;
                        }
                        if (!TryLowerDestructuringPattern(arr.Rest.Target, restArray, isDeclaration, "rest"))
                        {
                            return false;
                        }
                    }

                    return true;
                }

            default:
                return false;
        }
    }

    private bool TryBuildArrayRest(TempVariable sourceObject, int startIndex, out TempVariable restArray)
    {
        restArray = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRNewJsArray(System.Array.Empty<TempVariable>(), restArray));
        DefineTempStorage(restArray, new ValueStorage(ValueStorageKind.Reference, typeof(JavaScriptRuntime.Array)));

        // len = Object.GetLength(source)
        var lenTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetLength(sourceObject, lenTemp));
        DefineTempStorage(lenTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        // NOTE: temp-local allocation is linear and does not account for loop back-edges.
        // Pin loop-carry temps to stable variable slots so values remain correct across iterations.
        SetTempVariableSlot(lenTemp, CreateAnonymousVariableSlot("$arrayRest_len", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

        var idxTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber((double)startIndex, idxTemp));
        DefineTempStorage(idxTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        SetTempVariableSlot(idxTemp, CreateAnonymousVariableSlot("$arrayRest_idx", new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double))));

        var loopLabel = CreateLabel();
        var endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRLabel(loopLabel));

        var condTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCompareNumberLessThan(idxTemp, lenTemp, condTemp));
        DefineTempStorage(condTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(condTemp, endLabel));

        var itemTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRGetItem(sourceObject, idxTemp, itemTemp));
        DefineTempStorage(itemTemp, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        _methodBodyIR.Instructions.Add(new LIRArrayAdd(restArray, EnsureObject(itemTemp)));

        // idx = idx + 1
        var oneTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, oneTemp));
        DefineTempStorage(oneTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        var updatedIdx = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRAddNumber(idxTemp, oneTemp, updatedIdx));
        DefineTempStorage(updatedIdx, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(updatedIdx, idxTemp));

        _methodBodyIR.Instructions.Add(new LIRBranch(loopLabel));
        _methodBodyIR.Instructions.Add(new LIRLabel(endLabel));

        return true;
    }

    private bool TryLowerPropertyAssignmentExpression(HIRPropertyAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // If this is an assignment to a known instance field on the current user-defined class,
        // lower to direct field store (stfld) instead of dynamic property set (Object.SetItem).
        if (_classRegistry != null
            && assignExpr.Object is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, assignExpr.PropertyName, out _))
        {
            TempVariable fieldValueToStore;

            if (assignExpr.Operator == Acornima.Operator.Assignment)
            {
                if (!TryLowerExpression(assignExpr.Value, out fieldValueToStore))
                {
                    return false;
                }
            }
            else
            {
                // Compound assignment: this.field += expr
                var currentValue = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    currentClass,
                    assignExpr.PropertyName,
                    IsPrivateField: false,
                    currentValue));
                DefineTempStorage(currentValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (!TryLowerExpression(assignExpr.Value, out var rhs))
                {
                    return false;
                }

                if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhs, out fieldValueToStore))
                {
                    return false;
                }
            }

            fieldValueToStore = EnsureObject(fieldValueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                assignExpr.PropertyName,
                IsPrivateField: false,
                fieldValueToStore));

            // Assignment expression result is the value assigned.
            resultTempVar = fieldValueToStore;
            return true;
        }

        if (!TryLowerExpression(assignExpr.Object, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        var keyTemp = EmitConstString(assignExpr.PropertyName);
        var boxedKey = EnsureObject(keyTemp);

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: obj.prop += expr
            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, boxedKey, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerExpression(assignExpr.Value, out var rhs))
            {
                return false;
            }

            if (!TryLowerCompoundOperation(assignExpr.Operator, current, rhs, out valueToStore))
            {
                return false;
            }
        }

        valueToStore = EnsureObject(valueToStore);
        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, boxedKey, valueToStore, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
    }

    private bool TryLowerIndexAssignmentExpression(HIRIndexAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        // User-defined class instance field access via bracket notation (e.g., this["wordArray"] = ...).
        // If the receiver is `this` and the index is a constant string that matches a known field on the
        // generated CLR type, lower directly to an instance field store (stfld) instead of dynamic SetItem.
        if (_classRegistry != null
            && assignExpr.Object is HIRThisExpression
            && assignExpr.Index is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
        {
            TempVariable fieldValueToStore;

            if (assignExpr.Operator == Acornima.Operator.Assignment)
            {
                if (!TryLowerExpression(assignExpr.Value, out fieldValueToStore))
                {
                    return false;
                }
            }
            else
            {
                // Compound assignment: this["field"] += expr
                var currentValue = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                    currentClass,
                    literalFieldName,
                    IsPrivateField: false,
                    currentValue));
                DefineTempStorage(currentValue, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

                if (!TryLowerExpression(assignExpr.Value, out var rhs))
                {
                    return false;
                }

                if (!TryLowerCompoundOperation(assignExpr.Operator, currentValue, rhs, out fieldValueToStore))
                {
                    return false;
                }
            }

            fieldValueToStore = EnsureObject(fieldValueToStore);
            _methodBodyIR.Instructions.Add(new LIRStoreUserClassInstanceField(
                currentClass,
                literalFieldName,
                IsPrivateField: false,
                fieldValueToStore));

            // Assignment expression result is the value assigned.
            resultTempVar = fieldValueToStore;
            return true;
        }

        if (!TryLowerExpression(assignExpr.Object, out var objTemp))
        {
            return false;
        }
        objTemp = EnsureObject(objTemp);

        if (!TryLowerExpression(assignExpr.Index, out var indexTemp))
        {
            return false;
        }
        TempVariable boxedIndex = default;
        bool hasBoxedIndex = false;

        TempVariable valueToStore;
        if (assignExpr.Operator == Acornima.Operator.Assignment)
        {
            if (!TryLowerExpression(assignExpr.Value, out valueToStore))
            {
                return false;
            }
        }
        else
        {
            // Compound assignment: obj[index] += expr
            var indexStorageForGet = GetTempStorage(indexTemp);
            TempVariable indexForGet;
            if (indexStorageForGet.Kind == ValueStorageKind.UnboxedValue && indexStorageForGet.ClrType == typeof(double))
            {
                indexForGet = indexTemp;
            }
            else
            {
                boxedIndex = EnsureObject(indexTemp);
                hasBoxedIndex = true;
                indexForGet = boxedIndex;
            }

            var current = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRGetItem(objTemp, indexForGet, current));
            DefineTempStorage(current, new ValueStorage(ValueStorageKind.Reference, typeof(object)));

            if (!TryLowerExpression(assignExpr.Value, out var rhs))
            {
                return false;
            }

            if (!TryLowerCompoundOperation(assignExpr.Operator, current, rhs, out valueToStore))
            {
                return false;
            }
        }

        var indexStorage = GetTempStorage(indexTemp);
        var valueStorage = GetTempStorage(valueToStore);
        bool canUseNumericSetItem =
            indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double) &&
            valueStorage.Kind == ValueStorageKind.UnboxedValue && valueStorage.ClrType == typeof(double);

        TempVariable indexForSet;
        if (canUseNumericSetItem)
        {
            indexForSet = indexTemp;
        }
        else
        {
            if (!hasBoxedIndex)
            {
                boxedIndex = EnsureObject(indexTemp);
                hasBoxedIndex = true;
            }
            indexForSet = boxedIndex;
        }

        if (!canUseNumericSetItem)
        {
            valueToStore = EnsureObject(valueToStore);
        }
        var setResult = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRSetItem(objTemp, indexForSet, valueToStore, setResult));
        DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        resultTempVar = setResult;
        return true;
    }

    private bool TryLowerDestructuringAssignmentExpression(HIRDestructuringAssignmentExpression assignExpr, out TempVariable resultTempVar)
    {
        resultTempVar = default;

        if (!TryLowerExpression(assignExpr.Value, out var rhsTemp))
        {
            return false;
        }

        rhsTemp = EnsureObject(rhsTemp);

        var sourceNameForError = TryGetSimpleSourceNameForDestructuring(assignExpr.Value);
        if (!TryLowerDestructuringPattern(assignExpr.Pattern, rhsTemp, isDeclaration: false, sourceNameForError))
        {
            return false;
        }

        // JS destructuring assignment evaluates to the RHS value.
        resultTempVar = rhsTemp;
        return true;
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

        // Environment-stored update path:
        // - Captured variables live in scope fields
        // - Parameters live in IL arguments
        // Do not rely on the temp storage kind here, since other lowering steps may propagate
        // stable unboxed types for captured fields.
        var updateStorage = _environmentLayout?.GetStorage(updateBinding);
        var isEnvironmentStored = updateStorage != null && updateStorage.Kind != BindingStorageKind.IlLocal;

        // Implement numeric coercion via runtime TypeUtilities.ToNumber(object?) and then store
        // the boxed updated value back to the appropriate storage location.
        if (isEnvironmentStored)
        {
            if (_environmentLayout == null || updateStorage == null)
            {
                return false;
            }

            var currentNumber = EnsureNumber(currentValue);

            // For postfix, we must capture the old (ToNumber-coerced) value before the store happens.
            // Use LIRCopyTemp so Stackify will materialize the captured value.
            TempVariable? originalSnapshotForPostfix = null;
            if (!updateExpr.Prefix)
            {
                var snapshotValue = EnsureObject(currentNumber);
                var snapshot = CreateTempVariable();
                _methodBodyIR.Instructions.Add(new LIRCopyTemp(snapshotValue, snapshot));
                DefineTempStorage(snapshot, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                originalSnapshotForPostfix = snapshot;
            }

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

            switch (updateStorage.Kind)
            {
                case BindingStorageKind.IlArgument:
                    if (updateStorage.JsParameterIndex < 0)
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreParameter(updateStorage.JsParameterIndex, updatedBoxed));
                    break;

                case BindingStorageKind.LeafScopeField:
                    if (updateStorage.Field.IsNil || updateStorage.DeclaringScope.IsNil)
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreLeafScopeField(updateBinding, updateStorage.Field, updateStorage.DeclaringScope, updatedBoxed));
                    break;

                case BindingStorageKind.ParentScopeField:
                    if (updateStorage.ParentScopeIndex < 0 || updateStorage.Field.IsNil || updateStorage.DeclaringScope.IsNil)
                    {
                        return false;
                    }
                    _methodBodyIR.Instructions.Add(new LIRStoreParentScopeField(updateBinding, updateStorage.Field, updateStorage.DeclaringScope, updateStorage.ParentScopeIndex, updatedBoxed));
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

        // For non-captured locals, support both numeric (double) and boxed/object paths.
        var currentStorage = GetTempStorage(currentValue);

        if (currentStorage.ClrType != typeof(double))
        {
            // Boxed/local update path (e.g., object-typed locals).
            var currentNumber = EnsureNumber(currentValue);

            TempVariable? originalSnapshotForPostfix = null;
            if (!updateExpr.Prefix)
            {
                originalSnapshotForPostfix = EnsureObject(currentNumber);
            }

            var boxedDeltaTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRConstNumber(1.0, boxedDeltaTemp));
            DefineTempStorage(boxedDeltaTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var boxedUpdatedNumber = CreateTempVariable();
            if (isIncrement)
            {
                _methodBodyIR.Instructions.Add(new LIRAddNumber(currentNumber, boxedDeltaTemp, boxedUpdatedNumber));
            }
            else
            {
                _methodBodyIR.Instructions.Add(new LIRSubNumber(currentNumber, boxedDeltaTemp, boxedUpdatedNumber));
            }
            DefineTempStorage(boxedUpdatedNumber, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));

            var updatedBoxed = EnsureObject(boxedUpdatedNumber);

            if (!TryStoreToBinding(updateBinding, updatedBoxed, out var storedValue))
            {
                return false;
            }

            if (updateExpr.Prefix)
            {
                resultTempVar = EnsureObject(storedValue);
                return true;
            }

            resultTempVar = originalSnapshotForPostfix!.Value;
            return true;
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

    private TempVariable EnsureBoolean(TempVariable tempVar)
    {
        var storage = GetTempStorage(tempVar);

        if (storage.Kind == ValueStorageKind.UnboxedValue && storage.ClrType == typeof(bool))
        {
            return tempVar;
        }

        var boolTempVar = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConvertToBoolean(EnsureObject(tempVar), boolTempVar));
        DefineTempStorage(boolTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
        return boolTempVar;
    }

    private TempVariable CoerceToVariableSlotStorage(int slot, TempVariable value)
    {
        if (slot < 0 || slot >= _methodBodyIR.VariableStorages.Count)
        {
            return value;
        }

        var slotStorage = _methodBodyIR.VariableStorages[slot];
        if (slotStorage.Kind == ValueStorageKind.UnboxedValue && slotStorage.ClrType == typeof(double))
        {
            return EnsureNumber(value);
        }

        if (slotStorage.Kind == ValueStorageKind.UnboxedValue && slotStorage.ClrType == typeof(bool))
        {
            return EnsureBoolean(value);
        }

        if (slotStorage.Kind is ValueStorageKind.Reference or ValueStorageKind.BoxedValue)
        {
            return EnsureObject(value);
        }

        return value;
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
        var storageInfo = GetTempStorage(valueToStore);
        var slot = GetOrCreateVariableSlot(binding, binding.Name, storageInfo);
        valueToStore = CoerceToVariableSlotStorage(slot, valueToStore);
        _variableMap[binding] = valueToStore;
        valueToStore = EnsureTempMappedToSlot(slot, valueToStore);
        _variableMap[binding] = valueToStore;
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
        valueToStore = CoerceToVariableSlotStorage(slot, valueToStore);
        valueToStore = EnsureTempMappedToSlot(slot, valueToStore);
        _variableMap[binding] = valueToStore;
        resultTempVar = valueToStore;

        // This is a reassignment (not initial declaration), so the variable is not single-assignment.
        // Remove it from the single-assignment set to prevent incorrect inlining.
        _methodBodyIR.SingleAssignmentSlots.Remove(slot);
        return true;
    }

    private bool DoesClassNeedParentScopes(ClassDeclaration classDecl, Scope classScope)
    {
        if (classScope.ReferencesParentScopeVariables)
        {
            return true;
        }

        // Match ClassesGenerator's heuristic for when a class must capture parent scopes:
        // if any constructor/method contains nested functions or news a class that itself
        // requires parent scopes.
        var ctor = classDecl.Body.Body.OfType<MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

        if (ctor?.Value is FunctionExpression ctorExpr && MethodBodyRequiresParentScopes(ctorExpr.Body, classScope))
        {
            return true;
        }

        foreach (var method in classDecl.Body.Body
            .OfType<MethodDefinition>()
            .Where(m => m.Value is FunctionExpression && (m.Key as Identifier)?.Name != "constructor"))
        {
            if (method.Value is FunctionExpression funcExpr && MethodBodyRequiresParentScopes(funcExpr.Body, classScope))
            {
                return true;
            }
        }

        return false;
    }

    private bool MethodBodyRequiresParentScopes(Node? body, Scope classScope)
    {
        bool found = false;

        void Walk(Node? n)
        {
            if (n == null || found) return;

            switch (n)
            {
                case BlockStatement bs:
                    foreach (var st in bs.Body) Walk(st);
                    break;
                case ExpressionStatement es:
                    Walk(es.Expression);
                    break;
                case VariableDeclaration vd:
                    foreach (var d in vd.Declarations)
                    {
                        Walk(d.Init as Node);
                    }
                    break;
                case IfStatement ifs:
                    Walk(ifs.Test);
                    Walk(ifs.Consequent);
                    Walk(ifs.Alternate);
                    break;
                case WhileStatement ws:
                    Walk(ws.Test);
                    Walk(ws.Body);
                    break;
                case DoWhileStatement dws:
                    Walk(dws.Body);
                    Walk(dws.Test);
                    break;
                case ForStatement fs:
                    Walk(fs.Init as Node);
                    Walk(fs.Test);
                    Walk(fs.Update);
                    Walk(fs.Body);
                    break;
                case ForInStatement fi:
                    Walk(fi.Left as Node);
                    Walk(fi.Right as Node);
                    Walk(fi.Body);
                    break;
                case ForOfStatement fof:
                    Walk(fof.Left as Node);
                    Walk(fof.Right as Node);
                    Walk(fof.Body);
                    break;
                case ReturnStatement rs:
                    Walk(rs.Argument);
                    break;
                case AssignmentExpression ae:
                    Walk(ae.Left);
                    Walk(ae.Right);
                    break;
                case CallExpression ce:
                    Walk(ce.Callee);
                    foreach (var a in ce.Arguments) Walk(a as Node);
                    break;
                case NewExpression ne:
                    if (ne.Callee is Identifier classId)
                    {
                        var foundClassScope = FindClassScopeByName(classScope, classId.Name);
                        if (foundClassScope != null && foundClassScope.ReferencesParentScopeVariables)
                        {
                            found = true;
                            return;
                        }
                    }
                    foreach (var a in ne.Arguments) Walk(a as Node);
                    break;

                case FunctionDeclaration:
                case FunctionExpression:
                case ArrowFunctionExpression:
                    found = true;
                    return;

                default:
                    // Conservative: keep walking common container nodes we know about.
                    break;
            }
        }

        Walk(body);
        return found;
    }

    private static Scope? FindClassScopeByName(Scope startScope, string className)
    {
        var current = startScope;
        while (current != null)
        {
            foreach (var child in current.Children)
            {
                if (child.Kind == ScopeKind.Class && string.Equals(child.Name, className, StringComparison.Ordinal))
                {
                    return child;
                }
            }

            current = current.Parent;
        }

        return null;
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

        // Fast path: simple object literal with only non-computed properties.
        // Preserve the existing LIRNewJsObject initialization pattern for minimal IL/snapshot churn.
        bool allSimple = true;
        foreach (var member in objectExpr.Members)
        {
            if (member is not HIRObjectProperty)
            {
                allSimple = false;
                break;
            }
        }

        if (allSimple)
        {
            var properties = new List<ObjectProperty>();
            foreach (HIRObjectProperty prop in objectExpr.Members)
            {
                if (!TryLowerExpression(prop.Value, out var valueTemp))
                {
                    return false;
                }

                var boxedValue = EnsureObject(valueTemp);
                properties.Add(new ObjectProperty(prop.Key, boxedValue));
            }

            _methodBodyIR.Instructions.Add(new LIRNewJsObject(properties, resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(System.Dynamic.ExpandoObject)));
            return true;
        }

        // Create an empty object first, then apply members in source evaluation order.
        // This preserves side-effect order for computed keys and spread members.
        _methodBodyIR.Instructions.Add(new LIRNewJsObject(new List<ObjectProperty>(), resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(System.Dynamic.ExpandoObject)));

        foreach (var member in objectExpr.Members)
        {
            switch (member)
            {
                case HIRObjectProperty prop:
                {
                    var keyTemp = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRConstString(prop.Key, keyTemp));
                    DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

                    if (!TryLowerExpression(prop.Value, out var valueTemp))
                    {
                        return false;
                    }

                    var boxedKey = EnsureObject(keyTemp);
                    var boxedValue = EnsureObject(valueTemp);
                    var setResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRSetItem(resultTempVar, boxedKey, boxedValue, setResult));
                    DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                case HIRObjectComputedProperty computed:
                {
                    // Evaluate key expression before value expression (ECMA-262 order).
                    if (!TryLowerExpression(computed.KeyExpression, out var keyExprTemp))
                    {
                        return false;
                    }

                    if (!TryLowerExpression(computed.Value, out var valueTemp))
                    {
                        return false;
                    }

                    var boxedKey = EnsureObject(keyExprTemp);
                    var boxedValue = EnsureObject(valueTemp);
                    var setResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRSetItem(resultTempVar, boxedKey, boxedValue, setResult));
                    DefineTempStorage(setResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                case HIRObjectSpreadProperty spread:
                {
                    if (!TryLowerExpression(spread.Argument, out var spreadTemp))
                    {
                        return false;
                    }

                    var boxedTarget = EnsureObject(resultTempVar);
                    var boxedSource = EnsureObject(spreadTemp);
                    var spreadResult = CreateTempVariable();
                    _methodBodyIR.Instructions.Add(new LIRCallIntrinsicStatic(
                        IntrinsicName: "Object",
                        MethodName: "SpreadInto",
                        Arguments: new List<TempVariable> { boxedTarget, boxedSource },
                        Result: spreadResult));
                    DefineTempStorage(spreadResult, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
                    break;
                }

                default:
                    return false;
            }
        }
        return true;
    }

    private bool TryLowerPropertyAccessExpression(HIRPropertyAccessExpression propAccessExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // User-defined class instance field access (e.g., this.wordArray).
        // If the receiver is `this` and we know the generated CLR type has a field with this name,
        // lower directly to an instance field load (ldfld) instead of dynamic property access.
        if (_classRegistry != null
            && propAccessExpr.Object is HIRThisExpression
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, propAccessExpr.PropertyName, out _))
        {
            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: propAccessExpr.PropertyName,
                IsPrivateField: false,
                Result: resultTempVar));
            var fieldClrType = typeof(object);
            _classRegistry.TryGetFieldClrType(currentClass, propAccessExpr.PropertyName, out fieldClrType);
            var storageKind = (fieldClrType == typeof(double)
                || fieldClrType == typeof(bool)
                || fieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, fieldClrType));
            return true;
        }

        // User-defined class static field access (e.g., Greeter.message).
        // Classes are compiled as .NET types, and static class fields are emitted as CLR static fields.
        // When the receiver is the class identifier, lower directly to a static field load.
        if (propAccessExpr.Object is HIRVariableExpression classVarExpr &&
            classVarExpr.Name.BindingInfo.DeclarationNode is ClassDeclaration)
        {
            if (!TryGetRegistryClassNameForClassSymbol(classVarExpr.Name, out var registryClassName))
            {
                return false;
            }

            _methodBodyIR.Instructions.Add(new LIRLoadUserClassStaticField(
                RegistryClassName: registryClassName,
                FieldName: propAccessExpr.PropertyName,
                Result: resultTempVar));
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
            return true;
        }

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

        // General property access: treat as an item access with a string key (obj[propName]).
        // This enables lowering for cases like `console.log(s.n)` without falling back to legacy.
        var keyTemp = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRConstString(propAccessExpr.PropertyName, keyTemp));
        DefineTempStorage(keyTemp, new ValueStorage(ValueStorageKind.Reference, typeof(string)));

        var boxedObjectGeneric = EnsureObject(objectTemp);
        var boxedKey = EnsureObject(keyTemp);
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObjectGeneric, boxedKey, resultTempVar));
        DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        return true;
    }

    private bool TryGetRegistryClassNameForClassSymbol(Symbol classSymbol, out string registryClassName)
    {
        registryClassName = string.Empty;

        if (_scope == null)
        {
            return false;
        }

        var declaringScope = FindDeclaringScope(classSymbol.BindingInfo);
        if (declaringScope == null)
        {
            return false;
        }

        // The class scope is expected to be a child scope of the declaring scope.
        var classScope = declaringScope.Children.FirstOrDefault(s =>
            s.Kind == ScopeKind.Class &&
            string.Equals(s.Name, classSymbol.Name, StringComparison.Ordinal));

        if (classScope == null)
        {
            return false;
        }

        // Match TwoPhaseCompilationCoordinator registry naming (namespace + type name).
        var ns = classScope.DotNetNamespace ?? "Classes";
        var typeName = classScope.DotNetTypeName ?? classScope.Name;
        registryClassName = $"{ns}.{typeName}";
        return true;
    }

    private bool TryLowerConditionalExpression(HIRConditionalExpression conditionalExpr, out TempVariable resultTempVar)
    {
        resultTempVar = CreateTempVariable();

        // Evaluate the test condition, then branch to either consequent or alternate.
        if (!TryLowerExpression(conditionalExpr.Test, out var conditionTemp))
        {
            return false;
        }

        // If the test is already a boolean, we can branch directly.
        // Otherwise, apply JS truthiness semantics via Operators.IsTruthy(object).
        TempVariable boolConditionTemp;
        var conditionStorage = GetTempStorage(conditionTemp);
        if (conditionStorage.Kind == ValueStorageKind.UnboxedValue && conditionStorage.ClrType == typeof(bool))
        {
            boolConditionTemp = conditionTemp;
        }
        else
        {
            var conditionBoxed = EnsureObject(conditionTemp);
            var isTruthyTemp = CreateTempVariable();
            _methodBodyIR.Instructions.Add(new LIRCallIsTruthy(conditionBoxed, isTruthyTemp));
            DefineTempStorage(isTruthyTemp, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(bool)));
            boolConditionTemp = isTruthyTemp;
        }

        int elseLabel = CreateLabel();
        int endLabel = CreateLabel();

        _methodBodyIR.Instructions.Add(new LIRBranchIfFalse(boolConditionTemp, elseLabel));

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

        // User-defined class instance field access via bracket notation (e.g., this["wordArray"]).
        // If the receiver is `this` and the index is a constant string that matches a known field on the
        // generated CLR type, lower directly to an instance field load (ldfld) instead of dynamic GetItem.
        if (_classRegistry != null
            && indexAccessExpr.Object is HIRThisExpression
            && indexAccessExpr.Index is HIRLiteralExpression literalIndex
            && literalIndex.Kind == JavascriptType.String
            && literalIndex.Value is string literalFieldName
            && TryGetEnclosingClassRegistryName(out var currentClass)
            && currentClass != null
            && _classRegistry.TryGetField(currentClass, literalFieldName, out _))
        {
            _methodBodyIR.Instructions.Add(new LIRLoadUserClassInstanceField(
                RegistryClassName: currentClass,
                FieldName: literalFieldName,
                IsPrivateField: false,
                Result: resultTempVar));
            var fieldClrType = typeof(object);
            _classRegistry.TryGetFieldClrType(currentClass, literalFieldName, out fieldClrType);
            var storageKind = (fieldClrType == typeof(double)
                || fieldClrType == typeof(bool)
                || fieldClrType == typeof(JavaScriptRuntime.JsNull))
                ? ValueStorageKind.UnboxedValue
                : ValueStorageKind.Reference;
            DefineTempStorage(resultTempVar, new ValueStorage(storageKind, fieldClrType));
            return true;
        }

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
        var indexStorage = GetTempStorage(indexTemp);
        TempVariable indexForGet;
        if (indexStorage.Kind == ValueStorageKind.UnboxedValue && indexStorage.ClrType == typeof(double))
        {
            indexForGet = indexTemp;
        }
        else
        {
            indexForGet = EnsureObject(indexTemp);
        }
        _methodBodyIR.Instructions.Add(new LIRGetItem(boxedObject, indexForGet, resultTempVar));

        // If the receiver is statically known to be an Int32Array and the index is numeric,
        // lower the result as an unboxed double. This allows IL emission to use the typed
        // `Int32Array.get_Item(double)` fast-path without boxing, and only box later if
        // `EnsureObject` is required by usage.
        var receiverStorage = GetTempStorage(boxedObject);
        if (receiverStorage.Kind == ValueStorageKind.Reference
            && receiverStorage.ClrType == typeof(JavaScriptRuntime.Int32Array)
            && indexStorage.Kind == ValueStorageKind.UnboxedValue
            && indexStorage.ClrType == typeof(double))
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
        }
        else
        {
            DefineTempStorage(resultTempVar, new ValueStorage(ValueStorageKind.Reference, typeof(object)));
        }

        return true;
    }

    /// <summary>
    /// Loads the current value of a variable, handling both captured and non-captured variables.
    /// </summary>
    private bool TryLoadVariable(BindingInfo binding, out TempVariable result)
    {
        result = default;

        static ValueStorage GetPreferredBindingReadStorage(BindingInfo b)
        {
            // Only propagate unboxed doubles for stable types. This matches the current
            // typed-scope-field support in TypeGenerator/VariableRegistry.
            if (b.IsStableType && b.ClrType == typeof(double))
            {
                return new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double));
            }

            return new ValueStorage(ValueStorageKind.Reference, typeof(object));
        }

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
                            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
                            return true;
                        }
                        break;

                    case BindingStorageKind.ParentScopeField:
                        // Captured variable in parent scope
                        if (storage.ParentScopeIndex >= 0 && !storage.Field.IsNil && !storage.DeclaringScope.IsNil)
                        {
                            result = CreateTempVariable();
                            _methodBodyIR.Instructions.Add(new LIRLoadParentScopeField(binding, storage.Field, storage.DeclaringScope, storage.ParentScopeIndex, result));
                            DefineTempStorage(result, GetPreferredBindingReadStorage(binding));
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
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseAnd(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise AND requires numeric types
                return false;

            case Acornima.Operator.BitwiseOrAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseOr(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise OR requires numeric types
                return false;

            case Acornima.Operator.BitwiseXorAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRBitwiseXor(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Bitwise XOR requires numeric types
                return false;

            case Acornima.Operator.LeftShiftAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRLeftShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Left shift requires numeric types
                return false;

            case Acornima.Operator.RightShiftAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
                if (leftType == typeof(double) && rightType == typeof(double))
                {
                    _methodBodyIR.Instructions.Add(new LIRRightShift(currentValue, rhsValue, result));
                    DefineTempStorage(result, new ValueStorage(ValueStorageKind.UnboxedValue, typeof(double)));
                    return true;
                }
                // Right shift requires numeric types
                return false;

            case Acornima.Operator.UnsignedRightShiftAssignment:
                if (leftType != typeof(double) || rightType != typeof(double))
                {
                    EnsureNumericOperands();
                }
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

    private int GetTempVariableSlot(TempVariable temp)
    {
        if (temp.Index < 0 || temp.Index >= _methodBodyIR.TempVariableSlots.Count)
        {
            return -1;
        }

        return _methodBodyIR.TempVariableSlots[temp.Index];
    }

    private TempVariable EnsureTempMappedToSlot(int slot, TempVariable value)
    {
        var currentSlot = GetTempVariableSlot(value);
        if (currentSlot == -1 || currentSlot == slot)
        {
            SetTempVariableSlot(value, slot);
            return value;
        }

        // Avoid retroactively changing earlier IL for this temp by remapping.
        var copy = CreateTempVariable();
        _methodBodyIR.Instructions.Add(new LIRCopyTemp(value, copy));
        DefineTempStorage(copy, GetTempStorage(value));
        SetTempVariableSlot(copy, slot);
        return copy;
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
        
        // Allow identifier params, simple defaults, and destructuring patterns.
        // Disallow top-level RestElement (...args) for now.
        return parameters.Value.All(param => param switch
        {
            Acornima.Ast.Identifier => true,
            Acornima.Ast.AssignmentPattern ap => ap.Left is Acornima.Ast.Identifier,
            Acornima.Ast.ObjectPattern => true,
            Acornima.Ast.ArrayPattern => true,
            Acornima.Ast.RestElement => false,
            _ => false
        });
    }
}
