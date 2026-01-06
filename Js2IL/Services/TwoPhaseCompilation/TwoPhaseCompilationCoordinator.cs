using Acornima.Ast;
using Js2IL.SymbolTables;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Coordinates the two-phase compilation pipeline:
/// - Phase 1: Discovery + Declaration (no body compilation for main entry point)
/// - Phase 2: Body compilation (in dependency order, when planner is added in Milestone 2)
/// 
/// This is the entry point for the new compilation model. The coordinator is responsible for:
/// 1. Invoking CallableDiscovery to find all callables
/// 2. Invoking declaration APIs to create method tokens (Phase 1)
/// 3. Invoking body compilation in the appropriate order (Phase 2)
/// </summary>
/// <remarks>
/// Milestone 1 Implementation:
/// - Coordinator discovers and logs callables
/// - All arrow functions and function expressions are compiled upfront during Phase 1
/// - This ensures expression emission never triggers compilation (the key Milestone 1 invariant)
/// - Body compilation still uses existing emitters (no ordering changes yet)
/// </remarks>
public sealed class TwoPhaseCompilationCoordinator
{
    private readonly ILogger _logger;
    private readonly DeclaredCallableStore _declaredCallableStore;
    private readonly bool _verbose;
    
    // Registry for storing callable declarations
    private CallableRegistry? _registry;
    private IReadOnlyList<CallableId>? _discoveredCallables;

    public TwoPhaseCompilationCoordinator(
        ILogger logger, 
        CompilerOptions compilerOptions,
        DeclaredCallableStore declaredCallableStore)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verbose = compilerOptions?.Verbose ?? false;
        _declaredCallableStore = declaredCallableStore ?? throw new ArgumentNullException(nameof(declaredCallableStore));
    }

    /// <summary>
    /// Gets the callable registry after Phase 1 completes.
    /// </summary>
    public CallableRegistry? Registry => _registry;

    /// <summary>
    /// Gets the list of discovered callables after Phase 1 discovery.
    /// </summary>
    public IReadOnlyList<CallableId>? DiscoveredCallables => _discoveredCallables;

    /// <summary>
    /// Phase 1: Discover all callables in the module.
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

        // Initialize the registry with discovered callables
        _registry = new CallableRegistry();
    }

    /// <summary>
    /// Phase 1: Declare all discovered callables (create method tokens).
    /// This prepares the registry with signatures but does NOT compile bodies.
    /// 
    /// For Milestone 1, this is a skeleton that prepares the registry.
    /// The actual declaration/token allocation will be refined as we split
    /// the existing generators into declare-only and compile-body phases.
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

        // NOTE: We do NOT enable strict mode during Phase 1 declaration.
        // Arrows/function expressions inside classes are compiled during ClassesGenerator.DeclareClasses,
        // and they haven't been pre-declared yet. Strict mode will be enabled for Phase 2 (main body
        // compilation) in future milestones.
        
        // For Milestone 1, we delegate to the existing declaration code
        // which still combines declaration and body compilation.
        // Future milestones will split this.
        declareAction();

        if (_verbose)
        {
            var stats = _declaredCallableStore.GetStats();
            _logger.WriteLine($"[TwoPhase] Phase 1 Declaration complete. Declared: {stats.ByAstNode} by node, {stats.ByScopeName} by scope name.");
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
}
