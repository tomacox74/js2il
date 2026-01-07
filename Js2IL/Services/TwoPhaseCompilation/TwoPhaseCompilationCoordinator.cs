using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using Acornima.Ast;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Coordinates the two-phase compilation pipeline:
/// - Phase 1: Discovery + Declaration (create method tokens)
/// - Phase 2: Body compilation (in dependency order, when planner is added in Milestone 2)
/// 
/// This is the entry point for the new compilation model. The coordinator is responsible for:
/// 1. Invoking CallableDiscovery to find all callables
/// 2. Populating CallableRegistry with signatures (Phase 1 - discovery)
/// 3. Invoking declaration APIs to create method tokens (Phase 1 - token allocation)
/// 4. Invoking body compilation in the appropriate order (Phase 2)
/// </summary>
/// <remarks>
/// Milestone 1 Implementation:
/// - Coordinator discovers callables and populates CallableRegistry with signatures.
/// - Phase 1 declares callable tokens without compiling bodies (Option B: MemberRefs).
/// - Strict mode is enabled before Phase 2 so expression emission uses lookup-only.
/// - See docs/TwoPhaseCompilationPipeline.md for the full design.
/// </remarks>
public sealed class TwoPhaseCompilationCoordinator
{
    /// <summary>
    /// The namespace used for generated function types (arrows, function expressions, etc.).
    /// </summary>
    public const string FunctionsNamespace = "Functions";

    private readonly ILogger _logger;
    private readonly bool _verbose;
    
    // Registry for storing callable declarations (CallableId-keyed, per design doc)
    // Injected from DI - single instance per compilation
    private readonly CallableRegistry _registry;
    
    private IReadOnlyList<CallableId>? _discoveredCallables;
    
    // O(1) lookup index from AST node to CallableId (populated during discovery)
    private Dictionary<Node, CallableId>? _astNodeIndex;

    public TwoPhaseCompilationCoordinator(
        ILogger logger, 
        CompilerOptions compilerOptions,
        CallableRegistry registry)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verbose = compilerOptions?.Verbose ?? false;
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Gets the callable registry after Phase 1 completes.
    /// This is the single source of truth for callable declarations (per design doc).
    /// </summary>
    public CallableRegistry Registry => _registry;
    
    /// <summary>
    /// Gets the declaration reader interface for Phase 2 consumers.
    /// </summary>
    public ICallableDeclarationReader DeclarationReader => _registry;

    /// <summary>
    /// Gets the list of discovered callables after Phase 1 discovery.
    /// </summary>
    public IReadOnlyList<CallableId>? DiscoveredCallables => _discoveredCallables;

    /// <summary>
    /// Milestone 1 (Option B): Phase 1 is declare-only.
    ///
    /// This runs:
    /// 1) Phase 1 discovery
    /// 2) Phase 1 token declaration (MemberRefs for arrows/function expressions)
    /// 3) Enable strict mode
    /// 4) Phase 2 body compilation via provided callbacks
    /// </summary>
    /// <remarks>
    /// We keep Phase 2 body compilation delegated to generators, but centralize the
    /// orchestration and the Phase 1 declare-only MemberRef emission here so MainGenerator
    /// stays thin and the coordinator matches the design doc responsibilities.
    /// </remarks>
    public void RunMilestone1OptionB(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        Action<IReadOnlyList<CallableId>> compileAnonymousCallablesPhase2,
        Action compileClassesAndFunctionsPhase2)
    {
        RunPhase1Discovery(symbolTable);

        if (_discoveredCallables != null)
        {
            DeclarePhase1AnonymousCallablesTokens(_discoveredCallables, metadataBuilder);
        }

        // Enable strict mode before any body compilation so expression emission cannot compile.
        EnableStrictMode();

        RunPhase2BodyCompilation(() =>
        {
            // Declare classes and functions FIRST so their constructors/methods are registered
            // in ClassRegistry before anonymous callables are compiled.
            // Arrow functions may contain `new ClassName()` which requires the class to exist.
            compileClassesAndFunctionsPhase2();

            if (_discoveredCallables != null)
            {
                compileAnonymousCallablesPhase2(_discoveredCallables);
            }
        });
    }

    /// <summary>
    /// Phase 2 (Milestone 1): Compile anonymous callables (arrows + function expressions).
    ///
    /// This logic previously lived in MainGenerator; keeping it here makes MainGenerator a thin
    /// orchestration layer and keeps the two-phase implementation in one place.
    /// </summary>
    internal void CompilePhase2AnonymousCallables(
        IReadOnlyList<CallableId> callables,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        SymbolTable symbolTable)
    {
        foreach (var callable in callables)
        {
            switch (callable.Kind)
            {
                case CallableKind.Arrow:
                    if (callable.AstNode is ArrowFunctionExpression arrowExpr)
                    {
                        CompileArrowFunction(callable, arrowExpr, metadataBuilder, serviceProvider, rootVariables, bclReferences, methodBodyStreamEncoder, classRegistry, functionRegistry, symbolTable);
                    }
                    break;

                case CallableKind.FunctionExpression:
                    if (callable.AstNode is FunctionExpression funcExpr)
                    {
                        CompileFunctionExpression(callable, funcExpr, metadataBuilder, serviceProvider, rootVariables, bclReferences, methodBodyStreamEncoder, classRegistry, functionRegistry, symbolTable);
                    }
                    break;
            }
        }
    }

    private void CompileArrowFunction(
        CallableId callable,
        ArrowFunctionExpression arrowExpr,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        SymbolTable symbolTable)
    {
        var paramNames = ILMethodGenerator.ExtractParameterNames(arrowExpr.Params).ToArray();
        var moduleName = symbolTable.Root.Name;

        // Use 1-based column to match the CallableId.SourceLocation format used in Phase 1 declaration
        var col1Based = arrowExpr.Location.Start.Column + 1;
        var arrowBaseScopeName = callable.Name != null
            ? $"ArrowFunction_{callable.Name}"
            : $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{col1Based}";

        var registryScopeName = $"{moduleName}/{arrowBaseScopeName}";
        var ilMethodName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{col1Based}";

        // Find the arrow function's scope and build Variables representing its parent scope
        var arrowScope = symbolTable.FindScopeByAstNode(arrowExpr);
        var parentVariables = BuildParentVariablesForCallable(rootVariables, arrowScope, moduleName);

        var arrowGen = new JavaScriptArrowFunctionGenerator(
            serviceProvider,
            parentVariables,
            bclReferences,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry,
            symbolTable);

        arrowGen.GenerateArrowFunctionMethod(arrowExpr, registryScopeName, ilMethodName, paramNames);

        if (_verbose)
        {
            _logger.WriteLine($"[TwoPhase] Phase 2: Compiled arrow: {ilMethodName}");
        }
    }

    private void CompileFunctionExpression(
        CallableId callable,
        FunctionExpression funcExpr,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        SymbolTable symbolTable)
    {
        var paramNames = ILMethodGenerator.ExtractParameterNames(funcExpr.Params).ToArray();

        // Use 1-based column to match the CallableId.SourceLocation format used in Phase 1 declaration
        var col1Based = funcExpr.Location.Start.Column + 1;
        string baseScopeName;
        if (funcExpr.Id is Identifier fid && !string.IsNullOrEmpty(fid.Name))
        {
            baseScopeName = fid.Name;
        }
        else if (callable.Name != null)
        {
            baseScopeName = $"FunctionExpression_{callable.Name}";
        }
        else
        {
            baseScopeName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{col1Based}";
        }

        var moduleName = symbolTable.Root.Name;
        var registryScopeName = $"{moduleName}/{baseScopeName}";
        var ilMethodName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{col1Based}";

        // Find the function expression's scope and build Variables representing its parent scope
        var funcScope = symbolTable.FindScopeByAstNode(funcExpr);
        var parentVariables = BuildParentVariablesForCallable(rootVariables, funcScope, moduleName);

        var methodGen = new ILMethodGenerator(
            serviceProvider,
            parentVariables,
            bclReferences,
            metadataBuilder,
            methodBodyStreamEncoder,
            classRegistry,
            functionRegistry,
            symbolTable: symbolTable);

        methodGen.GenerateFunctionExpressionMethod(funcExpr, registryScopeName, ilMethodName, paramNames);

        if (_verbose)
        {
            _logger.WriteLine($"[TwoPhase] Phase 2: Compiled function expression: {ilMethodName}");
        }
    }

    private void DeclarePhase1AnonymousCallablesTokens(IReadOnlyList<CallableId> callables, MetadataBuilder metadataBuilder)
    {
        // Phase 1 declares MemberRefs for arrows and function expressions.
        // These are sufficient for ldftn/call token usage without compiling bodies.

        // Use Module scope for TypeRef resolution so we can reference types that will be emitted later.
        // The single module row is always index 1.
        var moduleHandle = MetadataTokens.EntityHandle(TableIndex.Module, 1);

        // Cache TypeRefs by "namespace/name" to avoid duplicates.
        var typeRefCache = new Dictionary<string, TypeReferenceHandle>(StringComparer.Ordinal);

        foreach (var callable in callables)
        {
            if (callable.Kind is not (CallableKind.Arrow or CallableKind.FunctionExpression))
            {
                continue;
            }

            // Skip if already has a token (idempotent); Phase 2 may overwrite MemberRef with MethodDef.
            if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
            {
                continue;
            }

            var signature = _registry.GetSignature(callable);
            if (signature == null)
            {
                continue;
            }

            var ilMethodName = signature.ILMethodName;
            var typeKey = $"{TwoPhaseCompilationCoordinator.FunctionsNamespace}/{ilMethodName}";
            if (!typeRefCache.TryGetValue(typeKey, out var typeRef))
            {
                typeRef = metadataBuilder.AddTypeReference(
                    moduleHandle,
                    metadataBuilder.GetOrAddString(TwoPhaseCompilationCoordinator.FunctionsNamespace),
                    metadataBuilder.GetOrAddString(ilMethodName));
                typeRefCache[typeKey] = typeRef;
            }

            var paramCount = 1 + callable.JsParamCount;
            var methodSig = ILGenerators.MethodBuilder.BuildMethodSignature(
                metadataBuilder,
                isInstance: false,
                paramCount: paramCount,
                hasScopesParam: true,
                returnsVoid: false);

            var memberRef = metadataBuilder.AddMemberReference(
                typeRef,
                metadataBuilder.GetOrAddString(ilMethodName),
                methodSig);

            _registry.SetToken(callable, (EntityHandle)memberRef);
        }
    }

    /// <summary>
    /// Phase 1: Discover all callables in the module and populate CallableRegistry.
    /// This must be called before RunPhase1Declaration.
    /// </summary>
    public void RunPhase1Discovery(SymbolTable symbolTable)
    {
        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Phase 1 Discovery: Starting callable discovery...");
        }

        var discovery = new CallableDiscovery(symbolTable);
        _discoveredCallables = discovery.DiscoverAll();

        // Build O(1) lookup index from AST node to CallableId
        _astNodeIndex = new Dictionary<Node, CallableId>(_discoveredCallables.Count);
        
        foreach (var callable in _discoveredCallables)
        {
            // Build CallableSignature from CallableId
            // For Milestone 1, we use a placeholder owner type handle (will be refined in future milestones)
            var signature = new CallableSignature
            {
                OwnerTypeHandle = default, // Will be set during token allocation
                RequiresScopesParameter = true, // All JS callables take scopes parameter
                JsParamCount = callable.JsParamCount,
                InvokeShape = CallableSignature.GetInvokeShape(callable.JsParamCount),
                IsInstanceMethod = callable.Kind == CallableKind.ClassMethod,
                ILMethodName = GetILMethodName(callable)
            };
            
            _registry.Declare(callable, signature);
            
            // Add to AST node index for O(1) lookup
            if (callable.AstNode != null)
            {
                _astNodeIndex[callable.AstNode] = callable;
            }
        }

        if (_verbose)
        {
            var stats = discovery.GetStats();
            _logger.WriteLine($"[TwoPhase] Discovered {stats.TotalCallables} callables:");
            _logger.WriteLine($"  - Function declarations: {stats.FunctionDeclarations}");
            _logger.WriteLine($"  - Function expressions: {stats.FunctionExpressions}");
            _logger.WriteLine($"  - Arrow functions: {stats.ArrowFunctions}");
            _logger.WriteLine($"  - Class constructors: {stats.ClassConstructors}");
            _logger.WriteLine($"  - Class methods: {stats.ClassMethods}");
            _logger.WriteLine($"  - Class static methods: {stats.ClassStaticMethods}");
            
            foreach (var callable in _discoveredCallables)
            {
                _logger.WriteLine($"  [{callable.Kind}] {callable.DisplayName} (params: {callable.JsParamCount})");
            }
        }
    }
    
    /// <summary>
    /// Derives the IL method name for a callable (used for CallableSignature).
    /// </summary>
    private static string GetILMethodName(CallableId callable)
    {
        return callable.Kind switch
        {
            CallableKind.FunctionDeclaration => callable.Name ?? "anonymous",
            CallableKind.FunctionExpression => callable.Location.HasValue 
                ? $"FunctionExpression_L{callable.Location.Value.Line}C{callable.Location.Value.Column}"
                : "FunctionExpression_anonymous",
            CallableKind.Arrow => callable.Location.HasValue
                ? $"ArrowFunction_L{callable.Location.Value.Line}C{callable.Location.Value.Column}"
                : "ArrowFunction_anonymous",
            CallableKind.ClassConstructor => ".ctor",
            CallableKind.ClassMethod or CallableKind.ClassStaticMethod => callable.Name ?? "method",
            _ => "unknown"
        };
    }
    
    /// <summary>
    /// Registers a method token for a callable by its AST node.
    /// Called by generators after creating a MethodDefinitionHandle.
    /// Uses O(1) dictionary lookup instead of linear search.
    /// </summary>
    public void RegisterToken(Node astNode, EntityHandle token)
    {
        if (_astNodeIndex == null)
        {
            return; // Discovery not run yet
        }
        
        // O(1) lookup using the AST node index
        if (_astNodeIndex.TryGetValue(astNode, out var callable))
        {
            _registry.SetToken(callable, token);
        }
    }
    
    /// <summary>
    /// Attempts to get a declared token for an AST node via the CallableRegistry.
    /// Uses O(1) dictionary lookup instead of linear search.
    /// </summary>
    public bool TryGetToken(Node astNode, out EntityHandle token)
    {
        token = default;
        if (_astNodeIndex == null)
        {
            return false; // Discovery not run yet
        }
        
        // O(1) lookup using the AST node index
        if (_astNodeIndex.TryGetValue(astNode, out var callable))
        {
            return _registry.TryGetDeclaredToken(callable, out token);
        }
        
        return false;
    }

    /// <summary>
    /// Phase 1: Declare all discovered callables (create method tokens).
    /// This prepares the registry with signatures and tokens.
    /// 
    /// For Milestone 1, declaration still includes body compilation.
    /// Future milestones will split into signature-only declaration + separate body compilation.
    /// </summary>
    /// <param name="declareAction">
    /// Action that performs the actual declaration using existing generators.
    /// This allows gradual migration: the coordinator orchestrates, but existing
    /// code does the work until we refactor it.
    /// </param>
    public void RunPhase1Declaration(Action declareAction)
    {
        if (_discoveredCallables == null)
        {
            throw new InvalidOperationException("Phase 1 Discovery must be run before Phase 1 Declaration.");
        }

        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Phase 1 Declaration: Creating method tokens...");
        }

        // For Milestone 1, we delegate to the existing declaration code
        // which still combines declaration and body compilation.
        // Future milestones will split this into signature-only + body compilation.
        declareAction();

        if (_verbose)
        {
            _logger.WriteLine($"[TwoPhase] Phase 1 Declaration complete. Tokens allocated: {_registry.TokensAllocated}/{_registry.Count}.");
        }
    }
    
    /// <summary>
    /// Enables strict mode after Phase 1 declaration is complete.
    /// This enforces the Milestone 1 invariant: "expression emission never triggers compilation"
    /// by making CallableRegistry throw if a lookup fails.
    /// </summary>
    public void EnableStrictMode()
    {
        _registry.StrictMode = true;
        
        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Strict mode enabled: expression emission will only lookup, not compile.");
        }
    }

    /// <summary>
    /// Phase 2: Compile callable bodies.
    /// 
    /// For Milestone 1, this is a pass-through to the existing compilation logic.
    /// Future milestones will compile in dependency order using the planner.
    /// </summary>
    /// <param name="compileAction">
    /// Action that performs the actual body compilation using existing generators.
    /// </param>
    public void RunPhase2BodyCompilation(Action compileAction)
    {
        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Phase 2: Compiling callable bodies...");
        }

        // For Milestone 1, we use existing compilation order.
        // Milestone 2 will introduce dependency-aware ordering.
        compileAction();

        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Phase 2: Body compilation complete.");
        }
    }

    /// <summary>
    /// Runs the complete two-phase compilation pipeline.
    /// This is a convenience method that runs all phases in order.
    /// </summary>
    /// <param name="symbolTable">The symbol table for the module.</param>
    /// <param name="declareAction">Action to declare callables (existing generators).</param>
    /// <param name="compileAction">Action to compile bodies (existing generators). Pass null to skip Phase 2.</param>
    /// <param name="skipPhase2">If true, Phase 2 body compilation is skipped (for Milestone 1 where declareAction includes body compilation).</param>
    public void RunFullPipeline(
        SymbolTable symbolTable,
        Action declareAction,
        Action? compileAction = null,
        bool skipPhase2 = false)
    {
        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Starting two-phase compilation pipeline...");
        }

        // Phase 1: Discovery
        RunPhase1Discovery(symbolTable);

        // Phase 1: Declaration
        // For Milestone 1, declaration is combined with body compilation in declareAction
        // Future milestones will separate these
        RunPhase1Declaration(declareAction);

        // Phase 2: Body Compilation
        // For Milestone 1, skipPhase2 should be true since declareAction includes body compilation
        // Future milestones will pass skipPhase2=false and a separate compileAction
        if (!skipPhase2 && compileAction != null)
        {
            RunPhase2BodyCompilation(compileAction);
        }

        if (_verbose)
        {
            _logger.WriteLine("[TwoPhase] Two-phase compilation pipeline complete.");
        }
    }

    /// <summary>
    /// Builds a Variables instance representing the parent scope of a nested callable.
    /// The generator will then create the actual callable's Variables from this parent.
    /// This ensures the parent scope chain is properly represented so captured variables can be resolved.
    /// </summary>
    private Variables BuildParentVariablesForCallable(Variables rootVariables, Scope? callableScope, string moduleName)
    {
        if (callableScope?.Parent == null)
        {
            // Callable is at global level, just use rootVariables
            return rootVariables;
        }

        var parentScope = callableScope.Parent;
        
        // If parent is the global scope, rootVariables already represents it
        if (parentScope.Kind == ScopeKind.Global)
        {
            return rootVariables;
        }

        // Build a Variables for the parent function scope with its own parent chain
        // Use the same naming convention as TypeGenerator.GetRegistryScopeName():
        // {moduleName}/{scopeName} for non-global scopes
        var parentScopeName = $"{moduleName}/{parentScope.Name}";
        
        // Collect grandparent scopes (parent's parents)
        var grandparentScopeNames = new List<string>();
        var current = parentScope.Parent;
        while (current != null)
        {
            // Use the correct registry scope name format
            if (current.Kind == ScopeKind.Global)
            {
                grandparentScopeNames.Insert(0, moduleName); // Global scope uses module name
            }
            else
            {
                grandparentScopeNames.Insert(0, $"{moduleName}/{current.Name}");
            }
            current = current.Parent;
        }

        // Create Variables for the parent scope
        // Empty parameter names since we're not compiling the parent, just representing its scope
        return new Variables(rootVariables, parentScopeName, Array.Empty<string>(), grandparentScopeNames, parameterStartIndex: 1);
    }
}
