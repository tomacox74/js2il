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
    private readonly MethodBodyIR _methodBodyIR = new MethodBodyIR();
    private readonly Scope? _scope;
    private readonly EnvironmentLayout? _environmentLayout;
    private readonly EnvironmentLayoutBuilder? _environmentLayoutBuilder;
    private readonly Js2IL.Services.ClassRegistry? _classRegistry;
    private readonly CallableKind _callableKind;
    private readonly bool _isAsync;
    private readonly bool _isDerivedConstructor;
    private bool _superConstructorCalled;

    // Source-level variables map to the current SSA value (TempVariable) at the current program point.
    // Keyed by BindingInfo reference to correctly handle shadowed variables with the same name.
    private readonly Dictionary<BindingInfo, TempVariable> _variableMap = new Dictionary<BindingInfo, TempVariable>();

    // Stable IL-local slot per JS variable declaration.
    // Keyed by BindingInfo reference to give each shadowed variable its own slot.
    private readonly Dictionary<BindingInfo, int> _variableSlots = new Dictionary<BindingInfo, int>();

    // Maps parameter bindings to their 0-based JS parameter index (not IL arg index)
    private readonly Dictionary<BindingInfo, int> _parameterIndexMap = new Dictionary<BindingInfo, int>();

    // Maps an active scope registry name to a temp holding that scope instance.
    // Used for nested lexical environments that are materialized within the current method
    // (e.g., per-iteration loop environments).
    private readonly Dictionary<string, TempVariable> _activeScopeTempsByScopeName = new(StringComparer.Ordinal);

    // Flow-sensitive numeric type refinement: maps a binding to the last proven unboxed-double
    // temp that holds its value.  Used to avoid redundant TypeUtilities.ToNumber calls when the
    // same variable is used in multiple numeric contexts without any intervening write.
    // Entries for writable bindings are cleared when the binding is assigned and at every
    // control-flow label (branch / loop header), keeping the optimisation safe.
    private readonly Dictionary<BindingInfo, TempVariable> _numericRefinements = new Dictionary<BindingInfo, TempVariable>();

    // Reverse map: records which binding a freshly-created load temp originated from so that
    // EnsureNumber can propagate the coercion result back into _numericRefinements.
    private readonly Dictionary<TempVariable, BindingInfo> _tempBindingOrigin = new Dictionary<TempVariable, BindingInfo>();

    // Track whether parameter initialization was successful (affects TryLower result)
    private bool _parameterInitSucceeded = true;

    private readonly bool _isGenerator;

    private HIRToLIRLowerer(Scope? scope, EnvironmentLayout? environmentLayout, EnvironmentLayoutBuilder? environmentLayoutBuilder, Js2IL.Services.ClassRegistry? classRegistry, CallableKind callableKind, IReadOnlyList<HIRPattern> parameters, bool isAsync = false, bool isGenerator = false, bool isDerivedConstructor = false)
    {
        _scope = scope;
        _environmentLayout = environmentLayout;
        _environmentLayoutBuilder = environmentLayoutBuilder;
        _classRegistry = classRegistry;
        _callableKind = callableKind;
        _isAsync = isAsync;
        _isGenerator = isGenerator;
        _isDerivedConstructor = isDerivedConstructor;
        InitializeParameters(parameters);
    }

    internal static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind callableKind, bool hasScopesParameter, Js2IL.Services.ClassRegistry? classRegistry, out MethodBodyIR? lirMethod, bool isAsync = false, bool isGenerator = false, TwoPhase.CallableId? callableId = null, bool isDerivedConstructor = false)
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
                var needsParentScopesOverride = callableKind switch
                {
                    Js2IL.Services.ScopesAbi.CallableKind.Constructor => hasScopesParameter,
                    Js2IL.Services.ScopesAbi.CallableKind.Function => hasScopesParameter,
                    _ => (bool?)null
                };
                environmentLayout = environmentLayoutBuilder.Build(scope, callableKind, needsParentScopesOverride: needsParentScopesOverride);
            }
            catch (Exception ex)
            {
                // If we can't build environment layout, fall back to legacy
                IRPipelineMetrics.RecordFailure($"EnvironmentLayout build failed for scope '{scope.GetQualifiedName()}' (kind={scope.Kind}) callableKind={callableKind}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        var lowerer = new HIRToLIRLowerer(scope, environmentLayout, environmentLayoutBuilder, classRegistry, callableKind, hirMethod.Parameters, isAsync, isGenerator, isDerivedConstructor);

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

        // Initialize generator state machine info if this is a generator function
        if (isGenerator)
        {
            lowerer._methodBodyIR.IsGenerator = true;
            lowerer._methodBodyIR.GeneratorInfo = new GeneratorStateMachineInfo();
        }

        // Emit scope instance creation if there are leaf scope fields
        // NOTE: This must come AFTER async info is initialized, because async functions
        // with awaits need a scope instance even if there are no captured variables.
        lowerer.EmitScopeInstanceCreationIfNeeded();

        // For generators, emit one-time parameter initialization guarded by GeneratorScope._started,
        // then emit a state switch based on GeneratorScope._genState.
        if (isGenerator)
        {
            if (!lowerer.EmitGeneratorParameterInitializationOnce(hirMethod.Parameters))
            {
                return false;
            }
            lowerer.EmitGeneratorStateSwitchIfNeeded();
        }
        
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

#if DEBUG
            // Run LIR invariant checks in debug builds to surface lowering bugs early.
            // Validation is skipped in release builds for performance.
            Js2IL.IL.LIRBodyValidator.Validate(lirMethod);
#endif

            return true;
        }

        return false;
    }

    // Backward compatibility overload for callers that don't provide callable kind
    internal static bool TryLower(HIRMethod hirMethod, Scope? scope, Services.VariableBindings.ScopeMetadataRegistry? scopeMetadataRegistry, out MethodBodyIR? lirMethod)
    {
        return TryLower(hirMethod, scope, scopeMetadataRegistry, Js2IL.Services.ScopesAbi.CallableKind.Function, hasScopesParameter: true, classRegistry: null, out lirMethod, isAsync: false, isGenerator: false, callableId: null);
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

    private bool TryGetEnclosingBaseClassRegistryName(out string? baseRegistryClassName)
    {
        baseRegistryClassName = null;

        if (_scope == null)
        {
            return false;
        }

        var current = _scope;
        while (current != null && current.Kind != ScopeKind.Class)
        {
            current = current.Parent;
        }

        if (current == null)
        {
            return false;
        }

        var superExpr = current.AstNode switch
        {
            ClassDeclaration cd => cd.SuperClass,
            ClassExpression ce => ce.SuperClass,
            _ => null
        };

        if (superExpr is not Identifier superId)
        {
            return false;
        }

        var superSymbol = current.FindSymbol(superId.Name);
        if (superSymbol.BindingInfo.DeclarationNode is not ClassDeclaration baseDecl || baseDecl.Id == null)
        {
            return false;
        }

        var declaringScope = FindDeclaringScope(superSymbol.BindingInfo);
        if (declaringScope == null)
        {
            return false;
        }

        var baseClassScope = declaringScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Class && string.Equals(s.Name, baseDecl.Id.Name, StringComparison.Ordinal));
        if (baseClassScope == null)
        {
            return false;
        }

        var ns = baseClassScope.DotNetNamespace ?? "Classes";
        var name = baseClassScope.DotNetTypeName ?? baseClassScope.Name;
        baseRegistryClassName = $"{ns}.{name}";
        return true;
    }

    private string? GetEnclosingSuperClassIntrinsicName()
    {
        if (_scope == null)
        {
            return null;
        }

        var current = _scope;
        while (current != null && current.Kind != ScopeKind.Class)
        {
            current = current.Parent;
        }

        if (current == null)
        {
            return null;
        }

        var superExpr = current.AstNode switch
        {
            ClassDeclaration cd => cd.SuperClass,
            ClassExpression ce => ce.SuperClass,
            _ => null
        };

        if (superExpr is not Identifier superId)
        {
            return null;
        }

        // Only support intrinsic bases that map cleanly to CLR base types.
        // Today the primary blocker is `extends Array` (issue #505).
        if (!string.Equals(superId.Name, "Array", StringComparison.Ordinal))
        {
            return null;
        }

        try
        {
            return JavaScriptRuntime.IntrinsicObjectRegistry.GetInfo(superId.Name) != null
                ? superId.Name
                : null;
        }
        catch
        {
            return null;
        }
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

        // Generator functions need a scope instance for generator state machine fields
        // (_genState, _started, _done, resume protocol fields) even if there are no captured variables.
        if (_scope != null
            && _methodBodyIR.IsGenerator
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
                // No available global slot in caller context (common for no-scopes optimized functions).
                // For non-capturing callees this is safe: runtime dispatch now honors RequiresScopesParameter
                // and will ignore/omit scopes for no-scopes delegates.
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(Array.Empty<ScopeSlotSource>(), resultTemp));
                return true;
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
        // If the class constructor ABI requires a scopes array, prefer building the full
        // callee layout so derived classes can pass through the scope chain needed by base classes.
        // (e.g., class Derived extends Base where Base captures outer variables).
        if (_environmentLayoutBuilder != null)
        {
            try
            {
                var calleeLayout = _environmentLayoutBuilder.Build(classScope, CallableKind.Constructor);
                // Only accept the callee layout when it actually includes non-global parent slots.
                // Some class scopes don't directly reference parent vars, but still need a scopes
                // array due to base-class captures; in that case, the class layout can be just
                // [global], which is insufficient.
                if (calleeLayout.ScopeChain.Slots.Count > 1
                    && TryBuildScopesArrayFromLayout(classScope, CallableKind.Constructor, resultTemp))
                {
                    return true;
                }
            }
            catch
            {
                // Fall through to the conservative fallback.
            }
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

        // If the caller has a leaf scope instance (e.g., we're inside a function and the class is
        // nested), include it as scopes[1]. This is required when base class methods access
        // captured variables via the stored _scopes array.
        var declaringScope = classScope.Parent;
        var declaringLeafScopeName = declaringScope != null
            ? ScopeNaming.GetRegistryScopeName(declaringScope)
            : moduleName;

        if (!string.Equals(declaringLeafScopeName, moduleName, StringComparison.Ordinal))
        {
            var leafSlot = new ScopeSlot(Index: 1, ScopeName: declaringLeafScopeName, ScopeId: new ScopeId(declaringLeafScopeName));
            if (TryMapScopeSlotToSource(leafSlot, out var leafSlotSource))
            {
                _methodBodyIR.Instructions.Add(new LIRBuildScopesArray(new[] { globalSlotSource, leafSlotSource }, resultTemp));
                return true;
            }
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

        // Check if this scope is currently materialized in a temp (e.g., loop iteration scope)
        if (_activeScopeTempsByScopeName.TryGetValue(slot.ScopeName, out var scopeTemp))
        {
            slotSource = new ScopeSlotSource(slot, ScopeInstanceSource.Temp, scopeTemp.Index);
            return true;
        }

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

    private bool TryGetActiveScopeFieldStorage(BindingInfo binding, out TempVariable scopeTemp, out ScopeId scopeId, out FieldId fieldId)
    {
        scopeTemp = default;
        scopeId = default;
        fieldId = default;

        if (!binding.IsCaptured)
        {
            return false;
        }

        var declaringScope = binding.DeclaringScope;
        if (declaringScope == null)
        {
            return false;
        }

        var scopeName = ScopeNaming.GetRegistryScopeName(declaringScope);
        if (!_activeScopeTempsByScopeName.TryGetValue(scopeName, out scopeTemp))
        {
            return false;
        }

        scopeId = new ScopeId(scopeName);
        fieldId = new FieldId(scopeName, binding.Name);
        return true;
    }

    // Backward compatibility overload for callers that don't provide scope
    public static bool TryLower(HIRMethod hirMethod, out MethodBodyIR? lirMethod)
    {
        return TryLower(hirMethod, null, null, out lirMethod);
    }

    // Clear all numeric refinements at a control-flow label (branch / loop header).
    // Refinements are only valid within a single basic block; once control flow can
    // arrive from multiple predecessors, a previously cached coercion result may no
    // longer reflect the current value of the variable, so we must discard them.
    private void ClearNumericRefinementsAtLabel()
    {
        _numericRefinements.Clear();
    }

}
