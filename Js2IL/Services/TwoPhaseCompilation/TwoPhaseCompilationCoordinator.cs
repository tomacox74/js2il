using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System.Reflection;
using Acornima.Ast;
using Js2IL.Services.ILGenerators;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Js2IL.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Coordinates the two-phase compilation pipeline:
/// - Phase 1: Discovery + Declaration (create method tokens)
/// - Phase 2: Body compilation (in dependency order)
/// 
/// This is the entry point for the new compilation model. The coordinator is responsible for:
/// 1. Invoking CallableDiscovery to find all callables
/// 2. Populating CallableRegistry with signatures (Phase 1 - discovery)
/// 3. Invoking declaration APIs to create method tokens (Phase 1 - token allocation)
/// 4. Invoking body compilation in the appropriate order (Phase 2)
/// </summary>
/// <remarks>
/// Current implementation notes:
/// - Coordinator discovers callables and populates CallableRegistry with signatures.
/// - Phase 1 reserves callable tokens without compiling bodies.
/// - Strict mode is enabled before Phase 2 so expression emission is lookup-only.
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

    /// <summary>
    /// Last dependency plan computed for this compilation (if computed).
    /// </summary>
    public CompilationPlan? LastComputedPlan { get; private set; }

    // Used to validate that preallocated MethodDef row ids stay stable.
    private int? _methodDefRowCountAtPreallocation;
    private int? _expectedMethodDefsBeforeAnonymousCallables;

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
    /// Two-phase compilation entry point for the anonymous-callable preallocation sanity-check path.
    ///
    /// This runs:
    /// 1) Phase 1 discovery
    /// 2) Dependency plan computation (plan artifact only)
    /// 3) Phase 1 token preallocation for anonymous callables
    /// 4) Enable strict mode
    /// 5) Phase 2 body compilation via provided callbacks
    /// </summary>
    /// <remarks>
    /// We keep Phase 2 body compilation delegated to generators, but centralize the
    /// orchestration and the Phase 1 declaration here so MainGenerator
    /// stays thin and the coordinator matches the design doc responsibilities.
    /// </remarks>
    public void RunTwoPhaseWithAnonymousPreallocationCheck(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        Action<IReadOnlyList<CallableId>> compileAnonymousCallablesPhase2,
        Action compileClassesAndFunctionsPhase2)
    {
        RunPhase1Discovery(symbolTable);

        // Compute dependency graph + SCC/topo plan (plan artifact only).
        // This entry point does not change compilation order; planned ordering is used by the planned compilation path.
        ComputeDependencyPlan(symbolTable);

        if (_discoveredCallables != null)
        {
            PreallocatePhase1AnonymousCallablesMethodDefs(_discoveredCallables, metadataBuilder);
        }

        // Enable strict mode before any body compilation so expression emission cannot compile.
        EnableStrictMode();

        RunPhase2BodyCompilation(() =>
        {
            // Declare classes and functions FIRST so their constructors/methods are registered
            // in ClassRegistry before anonymous callables are compiled.
            // Arrow functions may contain `new ClassName()` which requires the class to exist.
            compileClassesAndFunctionsPhase2();

            // Sanity check: the number of MethodDef rows added by class/function declaration
            // must match what we assumed when preallocating anonymous callable MethodDef handles.
            if (_methodDefRowCountAtPreallocation.HasValue && _expectedMethodDefsBeforeAnonymousCallables.HasValue)
            {
                var expected = _methodDefRowCountAtPreallocation.Value + _expectedMethodDefsBeforeAnonymousCallables.Value;
                var actual = metadataBuilder.GetRowCount(TableIndex.MethodDef);
                if (actual != expected)
                {
                    throw new InvalidOperationException(
                        $"[TwoPhase] MethodDef preallocation mismatch: expected MethodDef row count {expected} after declaring classes/functions, but was {actual}. " +
                        "This indicates the preallocation offset is wrong (likely due to unexpected extra MethodDef emissions during class/function declaration). " +
                        "Common causes include new synthesized methods (for example, class static field initializers or helper methods) being emitted without updating CallableDiscovery/" +
                        "preallocation logic (_expectedMethodDefsBeforeAnonymousCallables). Verify that any additional MethodDef emissions during class/function declaration are either " +
                        "accounted for in the preallocation calculation or are moved to a phase that runs after anonymous callable preallocation."
                    );
                }
            }

            if (_discoveredCallables != null)
            {
                compileAnonymousCallablesPhase2(_discoveredCallables);
            }
        });
    }

    /// <summary>
    /// Two-phase compilation entry point for planned Phase 2 compilation.
    ///
    /// This will:
    /// - Run Phase 1 discovery
    /// - Compute a dependency plan (SCC + deterministic stage order)
    /// - Reserve MethodDef rows deterministically
    /// - Enable strict lookup-only mode
    /// - Compile callable bodies in plan order and finalize MethodDef rows deterministically
    /// </summary>
    internal void RunPlannedTwoPhaseCompilation(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        Action<IReadOnlyList<CallableId>> compileAnonymousCallablesPhase2,
        Action compileClassesAndFunctionsPhase2)
    {
        RunPhase1Discovery(symbolTable);

        // Compute dependency graph + SCC/topo plan.
        var plan = ComputeDependencyPlan(symbolTable);

        // Build a deterministic compilation order from the plan.
        var ordered = new List<CallableId>(_discoveredCallables?.Count ?? 0);
        var seen = new HashSet<CallableId>();
        foreach (var stage in plan.Stages)
        {
            foreach (var member in stage.Members.Where(member => seen.Add(member)))
            {
                ordered.Add(member);
            }
        }

        // Plan is authoritative.
        // If discovery produced callables that are missing from the plan (or duplicates exist), fail fast.
        if (_discoveredCallables != null)
        {
            if (seen.Count != _discoveredCallables.Count)
            {
                var missingCallables = _discoveredCallables.Where(c => !seen.Contains(c)).ToArray();
                var sample = missingCallables.Take(10).Select(c => c.UniqueKey).ToArray();
                throw new InvalidOperationException(
                    "[TwoPhase] Compilation plan did not include every discovered callable. " +
                    $"Discovered={_discoveredCallables.Count}, PlannedDistinct={seen.Count}. " +
                    (sample.Length > 0
                        ? ("Missing (sample): " + string.Join(", ", sample) + (missingCallables.Length > sample.Length ? $" (+{missingCallables.Length - sample.Length} more)" : ""))
                        : ""));
            }

            if (ordered.Count != _discoveredCallables.Count)
            {
                throw new InvalidOperationException(
                    "[TwoPhase] Compilation plan contained duplicate callables (after de-dup). " +
                    $"Discovered={_discoveredCallables.Count}, PlannedOrderedDistinct={ordered.Count}.");
            }
        }

        if (_discoveredCallables != null)
        {
            // Class callable MethodDefs must be preallocated BEFORE class type declarations so
            // TypeDef.MethodList can point at the correct first MethodDef row for each class.
            // We preallocate in class declaration order (TypeDef order) to satisfy ECMA-335 method list rules.
            PreallocatePhase1ClassCallablesMethodDefs(symbolTable, metadataBuilder);

            PreallocatePhase1AnonymousCallablesMethodDefsInOrder(_discoveredCallables, ordered, metadataBuilder);
        }

        // Enable strict mode before any body compilation so expression emission cannot compile.
        EnableStrictMode();

        // Compile Phase 2 bodies in plan order.
        // Today, only anonymous callables are compiled directly by the coordinator; classes and functions
        // remain delegated to the existing generators (which compile depth-first).
        RunPhase2BodyCompilation(() =>
        {
            // Keep existing safety invariant for now: classes/functions are emitted before anonymous callables.
            compileClassesAndFunctionsPhase2();

            // Compile class constructors/methods/accessors/.cctor bodies in planned Phase 2.
            // Compile bodies in plan order, then emit MethodDef rows grouped by class type (TypeDef order)
            // to satisfy ECMA-335 contiguous method list requirements.
            CompileAndFinalizePhase2ClassCallables(
                symbolTable,
                metadataBuilder,
                ordered,
                serviceProvider,
                rootVariables,
                bclReferences,
                methodBodyStreamEncoder,
                classRegistry);

            // Compile function declarations in planned Phase 2 order.
            // We compile bodies in plan order (IR-first, legacy fallback), but finalize MethodDefs in a single
            // deterministic block so metadata ordering stays valid.
            CompileAndFinalizePhase2FunctionDeclarations(
                symbolTable,
                metadataBuilder,
                ordered,
                serviceProvider,
                rootVariables,
                bclReferences,
                methodBodyStreamEncoder,
                classRegistry,
                functionRegistry);

            if (_methodDefRowCountAtPreallocation.HasValue && _expectedMethodDefsBeforeAnonymousCallables.HasValue)
            {
                var expected = _methodDefRowCountAtPreallocation.Value + _expectedMethodDefsBeforeAnonymousCallables.Value;
                var actual = metadataBuilder.GetRowCount(TableIndex.MethodDef);
                if (actual != expected)
                {
                    throw new InvalidOperationException(
                        $"[TwoPhase] MethodDef preallocation mismatch: expected MethodDef row count {expected} after declaring classes + finalizing class callables + finalizing function declarations, but was {actual}. " +
                        "This indicates the preallocation offset is wrong (likely due to unexpected extra MethodDef emissions during finalization)."
                    );
                }
            }

            if (_discoveredCallables != null)
            {
                compileAnonymousCallablesPhase2(ordered);
            }
        });
    }

    private void PreallocatePhase1ClassCallablesMethodDefs(SymbolTable symbolTable, MetadataBuilder metadataBuilder)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var nextRowId = metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1;

        foreach (var classScope in EnumerateClassScopesInDeclarationOrder(symbolTable.Root))
        {
            if (classScope.AstNode is not ClassDeclaration classDecl)
            {
                continue;
            }

            foreach (var callable in GetClassCallablesInDeclarationOrder(classScope, classDecl))
            {
                // Idempotent: if token already set, do not overwrite.
                if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
                {
                    continue;
                }

                var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
                _registry.SetToken(callable, preallocated);
            }
        }
    }

    private void CompileAndFinalizePhase2ClassCallables(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IReadOnlyList<CallableId> plannedOrder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var compiled = new Dictionary<CallableId, CompiledCallableBody>();
        var methodCompiler = serviceProvider.GetRequiredService<JsMethodCompiler>();

        // Phase 2: compile bodies in planned order (no MethodDef rows emitted here)
        foreach (var callable in plannedOrder)
        {
            if (callable.Kind is not (
                CallableKind.ClassConstructor or
                CallableKind.ClassMethod or
                CallableKind.ClassGetter or
                CallableKind.ClassSetter or
                CallableKind.ClassStaticMethod or
                CallableKind.ClassStaticGetter or
                CallableKind.ClassStaticSetter or
                CallableKind.ClassStaticInitializer))
            {
                continue;
            }

            if (!_registry.TryGetDeclaredToken(callable, out var tok) || tok.Kind != HandleKind.MethodDefinition)
            {
                throw new InvalidOperationException($"[TwoPhase] Class callable missing declared token: {callable.DisplayName}");
            }
            var expected = (MethodDefinitionHandle)tok;
            if (expected.IsNil)
            {
                throw new InvalidOperationException($"[TwoPhase] Class callable has nil token: {callable.DisplayName}");
            }

            var (classScope, classDecl, className) = ResolveClassScope(symbolTable, callable);
            var hasScopes = classRegistry.TryGetPrivateField(className, "_scopes", out var scopesField);

            CompiledCallableBody body;
            switch (callable.Kind)
            {
                case CallableKind.ClassConstructor:
                {
                    // Constructors need legacy emission for base ctor call + field initializers.
                    FunctionExpression? ctorFunc = null;
                    if (callable.AstNode is Acornima.Ast.MethodDefinition ctorDef)
                    {
                        ctorFunc = ctorDef.Value as FunctionExpression;
                    }
                    else
                    {
                        var ctorMember = classDecl.Body.Body
                            .OfType<Acornima.Ast.MethodDefinition>()
                            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
                        ctorFunc = ctorMember?.Value as FunctionExpression;
                    }

                    body = LegacyClassBodyCompiler.CompileConstructorBody(
                        serviceProvider,
                        metadataBuilder,
                        methodBodyStreamEncoder,
                        bclReferences,
                        classRegistry,
                        rootVariables,
                        symbolTable,
                        callable,
                        expected,
                        classScope,
                        classDecl,
                        ctorFunc,
                        needsScopes: hasScopes);
                    break;
                }

                case CallableKind.ClassStaticInitializer:
                {
                    body = LegacyClassBodyCompiler.CompileStaticInitializerBody(
                        serviceProvider,
                        metadataBuilder,
                        methodBodyStreamEncoder,
                        bclReferences,
                        classRegistry,
                        rootVariables,
                        symbolTable,
                        callable,
                        expected,
                        classScope,
                        classDecl);
                    break;
                }

                default:
                {
                    if (callable.AstNode is not Acornima.Ast.MethodDefinition methodDef)
                    {
                        throw new InvalidOperationException($"[TwoPhase] Class method callable missing AST node: {callable.DisplayName}");
                    }

                    var memberName = (methodDef.Key as Identifier)?.Name ?? "method";
                    var clrMethodName = methodDef.Kind switch
                    {
                        PropertyKind.Get => $"get_{memberName}",
                        PropertyKind.Set => $"set_{memberName}",
                        _ => memberName
                    };

                    // IR first (body-only), fallback to legacy emitter.
                    CompiledCallableBody? irBody = null;
                    try
                    {
                        var funcExpr = methodDef.Value as FunctionExpression;
                        if (funcExpr != null)
                        {
                            var methodScope = symbolTable.FindScopeByAstNode(funcExpr);
                            if (methodScope != null)
                            {
                                FieldDefinitionHandle? scopesFieldHandle = null;
                                if (!methodDef.Static && hasScopes)
                                {
                                    scopesFieldHandle = scopesField;
                                }

                                irBody = methodCompiler.TryCompileCallableBody(
                                    callable: callable,
                                    expectedMethodDef: expected,
                                    ilMethodName: clrMethodName,
                                    node: methodDef,
                                    scope: methodScope,
                                    methodBodyStreamEncoder: methodBodyStreamEncoder,
                                    isInstanceMethod: !methodDef.Static,
                                    hasScopesParameter: false,
                                    scopesFieldHandle: scopesFieldHandle,
                                    returnsVoid: false);
                            }
                        }
                    }
                    catch
                    {
                        // Fall back to legacy compilation.
                        irBody = null;
                    }

                    body = irBody ?? LegacyClassBodyCompiler.CompileMethodBody(
                        serviceProvider,
                        metadataBuilder,
                        methodBodyStreamEncoder,
                        bclReferences,
                        classRegistry,
                        rootVariables,
                        symbolTable,
                        callable,
                        expected,
                        classScope,
                        methodDef,
                        clrMethodName);
                    break;
                }
            }

            compiled[callable] = body;
            if (callable.AstNode != null)
            {
                _registry.MarkBodyCompiledForAstNode(callable.AstNode);
            }
        }

        // Finalize MethodDef/Param rows in TypeDef order (class declaration order), preserving per-type contiguity.
        foreach (var classScope in EnumerateClassScopesInDeclarationOrder(symbolTable.Root))
        {
            if (classScope.AstNode is not ClassDeclaration classDecl)
            {
                continue;
            }

            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(metadataBuilder, ns, name);

            foreach (var callable in GetClassCallablesInDeclarationOrder(classScope, classDecl))
            {
                if (!compiled.TryGetValue(callable, out var body))
                {
                    throw new InvalidOperationException($"[TwoPhase] Missing compiled body for class callable: {callable.DisplayName}");
                }

                _ = MethodDefinitionFinalizer.EmitMethod(metadataBuilder, tb, body);
            }
        }
    }

    private static IEnumerable<Scope> EnumerateClassScopesInDeclarationOrder(Scope scope)
    {
        foreach (var child in scope.Children)
        {
            if (child.Kind == ScopeKind.Class)
            {
                yield return child;
            }

            foreach (var nested in EnumerateClassScopesInDeclarationOrder(child))
            {
                yield return nested;
            }
        }
    }

    private IEnumerable<CallableId> GetClassCallablesInDeclarationOrder(Scope classScope, ClassDeclaration classDecl)
    {
        var className = classScope.Name;

        // Constructor (explicit or synthetic)
        var ctorMember = classDecl.Body.Body
            .OfType<Acornima.Ast.MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");

        if (ctorMember != null && _registry.TryGetCallableIdForAstNode(ctorMember, out var ctorCallable))
        {
            yield return ctorCallable;
        }
        else
        {
            var synthCtor = _discoveredCallables!.FirstOrDefault(c => c.Kind == CallableKind.ClassConstructor && string.Equals(c.Name, className, StringComparison.Ordinal));
            if (synthCtor != null)
            {
                yield return synthCtor;
            }
        }

        // Methods/accessors in source order
        foreach (var member in classDecl.Body.Body.OfType<Acornima.Ast.MethodDefinition>().Where(m => m.Key is Identifier))
        {
            var methodName = ((Identifier)member.Key).Name;
            if (methodName == "constructor") continue;

            if (_registry.TryGetCallableIdForAstNode(member, out var methodCallable))
            {
                yield return methodCallable;
            }
        }

        // Static initializer (.cctor) if needed (legacy ordering: after methods)
        bool hasStaticFieldInits = classDecl.Body.Body.OfType<Acornima.Ast.PropertyDefinition>()
            .Any(p => p.Static && p.Value != null);
        if (hasStaticFieldInits)
        {
            var cctor = _discoveredCallables!.FirstOrDefault(c => c.Kind == CallableKind.ClassStaticInitializer && string.Equals(c.Name, className, StringComparison.Ordinal));
            if (cctor != null)
            {
                yield return cctor;
            }
        }
    }

    private static (Scope ClassScope, ClassDeclaration ClassDecl, string ClassName) ResolveClassScope(SymbolTable symbolTable, CallableId callable)
    {
        if (string.IsNullOrEmpty(callable.Name))
        {
            throw new InvalidOperationException($"[TwoPhase] Class callable missing Name: {callable.DisplayName}");
        }

        var className = callable.Kind is CallableKind.ClassConstructor or CallableKind.ClassStaticInitializer
            ? callable.Name
            : (JavaScriptCallableNaming.TrySplitClassMethodCallableName(callable.Name, out var cn, out _) ? cn : "");

        if (string.IsNullOrEmpty(className))
        {
            throw new InvalidOperationException($"[TwoPhase] Invalid class callable name format: {callable.DisplayName}");
        }

        var declaringScope = ResolveScopeByPath(symbolTable, callable.DeclaringScopeName);
        var classScope = declaringScope.Children.FirstOrDefault(s => s.Kind == ScopeKind.Class && string.Equals(s.Name, className, StringComparison.Ordinal));
        if (classScope == null || classScope.AstNode is not ClassDeclaration classDecl)
        {
            throw new InvalidOperationException($"[TwoPhase] Class scope not found for callable: {callable.DisplayName} (DeclaringScope='{callable.DeclaringScopeName}', ClassName='{className}')");
        }

        // ClassRegistry keys use CLR full names (namespace + type) to avoid collisions across modules.
        var ns = classScope.DotNetNamespace ?? "Classes";
        var typeName = classScope.DotNetTypeName ?? classScope.Name;
        var registryClassName = $"{ns}.{typeName}";

        return (classScope, classDecl, registryClassName);
    }

    private static Scope ResolveScopeByPath(SymbolTable symbolTable, string scopePath)
    {
        if (string.IsNullOrWhiteSpace(scopePath))
        {
            return symbolTable.Root;
        }

        var parts = scopePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var idx = 0;
        if (parts.Length > 0 && string.Equals(parts[0], symbolTable.Root.Name, StringComparison.Ordinal))
        {
            idx = 1;
        }

        var current = symbolTable.Root;
        for (var i = idx; i < parts.Length; i++)
        {
            // Scope names are used as path segments; they must not contain '/'.
            if (current.Children.Any(s => s.Name.Contains('/')))
            {
                throw new InvalidOperationException("[TwoPhase] Invalid scope name: scope names must not contain '/'.");
            }

            var next = current.Children.FirstOrDefault(s => string.Equals(s.Name, parts[i], StringComparison.Ordinal));
            if (next == null)
            {
                throw new InvalidOperationException($"[TwoPhase] Declaring scope not found: '{scopePath}'");
            }
            current = next;
        }

        return current;
    }

    private void CompileAndFinalizePhase2FunctionDeclarations(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IReadOnlyList<CallableId> plannedOrder,
        IServiceProvider serviceProvider,
        Variables rootVariables,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var moduleName = symbolTable.Root.Name;

        // Build Variables for each function declaration with correct parent scope wiring.
        // This is required for nested function bodies compiled via the legacy emitter path.
        var variablesByFunctionDecl = new Dictionary<FunctionDeclaration, Variables>();
        void BuildVariablesForFunctionScopes(Scope currentScope, Variables currentFunctionVariables)
        {
            foreach (var child in currentScope.Children)
            {
                if (child.Kind == ScopeKind.Function && child.AstNode is FunctionDeclaration childFuncDecl)
                {
                    var fnName = (childFuncDecl.Id as Identifier)?.Name;
                    if (string.IsNullOrEmpty(fnName))
                    {
                        // FunctionDeclaration should always be named; keep defensive.
                        continue;
                    }

                    var registryScopeName = $"{moduleName}/{fnName}";
                    var paramNames = ILMethodGenerator.ExtractParameterNames(childFuncDecl.Params).ToArray();

                    // If we're inside another function, this is a nested function.
                    var isNestedFunction = currentFunctionVariables.GetCurrentScopeName() != rootVariables.GetCurrentScopeName();
                    var childVars = new Variables(currentFunctionVariables, registryScopeName, paramNames, isNestedFunction: isNestedFunction);

                    variablesByFunctionDecl[childFuncDecl] = childVars;

                    // Recurse: any function declarations underneath are nested within this function.
                    BuildVariablesForFunctionScopes(child, childVars);
                }
                else
                {
                    // Keep traversing to find function declarations nested in blocks, etc.
                    BuildVariablesForFunctionScopes(child, currentFunctionVariables);
                }
            }
        }

        // Seed with top-level functions under the module root.
        foreach (var topFuncScope in symbolTable.Root.Children.Where(c => c.Kind == ScopeKind.Function && c.AstNode is FunctionDeclaration))
        {
            var topFuncDecl = (FunctionDeclaration)topFuncScope.AstNode!;
            var topName = (topFuncDecl.Id as Identifier)?.Name;
            if (string.IsNullOrEmpty(topName))
            {
                continue;
            }

            var topRegistryScopeName = $"{moduleName}/{topName}";
            var topParamNames = ILMethodGenerator.ExtractParameterNames(topFuncDecl.Params).ToArray();
            var topVars = new Variables(rootVariables, topRegistryScopeName, topParamNames, isNestedFunction: false);
            variablesByFunctionDecl[topFuncDecl] = topVars;

            BuildVariablesForFunctionScopes(topFuncScope, topVars);
        }

        // Deterministic declaration order for MethodDef row allocation: discovery order.
        var functionDeclCallables = _discoveredCallables
            .Where(c => c.Kind == CallableKind.FunctionDeclaration)
            .ToList();
        if (functionDeclCallables.Count == 0)
        {
            return;
        }

        // Preallocate MethodDef handles for function declarations if not already present.
        var nextRowId = metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1;
        foreach (var callable in functionDeclCallables)
        {
            if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
            {
                continue;
            }

            var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
            _registry.SetToken(callable, preallocated);
            if (callable.AstNode is FunctionDeclaration fd)
            {
                _registry.SetDeclaredTokenForAstNode(fd, preallocated);
                var fnName = (fd.Id as Identifier)?.Name;
                if (!string.IsNullOrEmpty(fnName))
                {
                    var jsParamNames = ILMethodGenerator.ExtractParameterNames(fd.Params).ToArray();
                    functionRegistry.PreRegisterParameterCount(fnName, jsParamNames.Length);
                    functionRegistry.Register(fnName, preallocated, jsParamNames.Length);
                }
            }
        }

        // Compile bodies in planned order (no MethodDef rows emitted here).
        var compiled = new Dictionary<CallableId, CompiledCallableBody>();
        foreach (var callable in plannedOrder)
        {
            if (callable.Kind != CallableKind.FunctionDeclaration)
            {
                continue;
            }

            if (callable.AstNode is not FunctionDeclaration funcDecl)
            {
                continue;
            }

            if (!_registry.TryGetDeclaredToken(callable, out var tok) || tok.Kind != HandleKind.MethodDefinition)
            {
                throw new InvalidOperationException($"[TwoPhase] FunctionDeclaration missing declared token: {callable.DisplayName}");
            }
            var expected = (MethodDefinitionHandle)tok;
            if (expected.IsNil)
            {
                throw new InvalidOperationException($"[TwoPhase] FunctionDeclaration has nil token: {callable.DisplayName}");
            }

            var funcScope = symbolTable.FindScopeByAstNode(funcDecl);
            if (funcScope == null)
            {
                throw new InvalidOperationException($"[TwoPhase] FunctionDeclaration scope not found: {callable.DisplayName}");
            }

            if (!variablesByFunctionDecl.TryGetValue(funcDecl, out var functionVariables))
            {
                throw new InvalidOperationException($"[TwoPhase] Variables not found for function declaration: {callable.DisplayName}");
            }

            var registryScopeName = functionVariables.GetCurrentScopeName();
            var methodName = (funcDecl.Id as Identifier)?.Name ?? callable.Name ?? "anonymous";

            // IR first
            var methodCompiler = serviceProvider.GetRequiredService<JsMethodCompiler>();
            var irBody = methodCompiler.TryCompileCallableBody(
                callable: callable,
                expectedMethodDef: expected,
                ilMethodName: methodName,
                node: funcDecl,
                scope: funcScope,
                methodBodyStreamEncoder: methodBodyStreamEncoder,
                isInstanceMethod: false,
                hasScopesParameter: true,
                scopesFieldHandle: null,
                returnsVoid: false);

            CompiledCallableBody body;
            if (irBody != null)
            {
                body = irBody;
            }
            else
            {
                body = LegacyFunctionBodyCompiler.CompileFunctionDeclarationBody(
                    serviceProvider,
                    metadataBuilder,
                    methodBodyStreamEncoder,
                    bclReferences,
                    functionVariables,
                    classRegistry,
                    functionRegistry,
                    symbolTable,
                    callable,
                    expected,
                    funcDecl,
                    funcScope,
                    registryScopeName);
            }

            compiled[callable] = body;
            _registry.MarkBodyCompiledForAstNode(funcDecl);
        }

        // Finalize MethodDef/Param rows deterministically (discovery order) as a single contiguous block.
        var tb = new TypeBuilder(metadataBuilder, FunctionsNamespace, moduleName);
        foreach (var callable in functionDeclCallables)
        {
            if (!compiled.TryGetValue(callable, out var body))
            {
                // The plan is authoritative.
                throw new InvalidOperationException($"[TwoPhase] Missing compiled body for function declaration: {callable.DisplayName}");
            }
            _ = MethodDefinitionFinalizer.EmitMethod(metadataBuilder, tb, body);
        }

        tb.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
            bclReferences.ObjectType);
    }

    /// <summary>
    /// Builds the dependency graph and computes SCC/topo stages.
    /// </summary>
    public CompilationPlan ComputeDependencyPlan(SymbolTable symbolTable)
    {
        if (_discoveredCallables == null)
        {
            throw new InvalidOperationException("Phase 1 Discovery must be run before computing the dependency plan.");
        }

        var collector = new CallableDependencyCollector(symbolTable, _registry, _discoveredCallables);
        var graph = collector.Collect();
        var plan = CompilationPlanner.ComputePlan(graph);
        LastComputedPlan = plan;

        if (_verbose)
        {
            _logger.WriteLine($"[TwoPhase] Computed dependency plan (SCC stages: {plan.Stages.Count}).");
            _logger.WriteLine(plan.ToDebugString());
        }

        return plan;
    }

    /// <summary>
    /// Phase 2: Compile anonymous callables (arrows + function expressions).
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
                        // Note: body is marked as compiled by the generator (JavaScriptArrowFunctionGenerator)
                        // after it successfully emits the method body. No duplicate call here.
                    }
                    break;

                case CallableKind.FunctionExpression:
                    if (callable.AstNode is FunctionExpression funcExpr)
                    {
                        CompileFunctionExpression(callable, funcExpr, metadataBuilder, serviceProvider, rootVariables, bclReferences, methodBodyStreamEncoder, classRegistry, functionRegistry, symbolTable);
                        // Note: body is marked as compiled by the generator (ILMethodGenerator.GenerateFunctionExpressionMethod)
                        // after it successfully emits the method body. No duplicate call here.
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

    private void PreallocatePhase1AnonymousCallablesMethodDefs(IReadOnlyList<CallableId> callables, MetadataBuilder metadataBuilder)
    {
        // Preallocate MethodDef tokens for anonymous callables.
        //
        // Important: we are NOT emitting MethodDef rows here. We are reserving their row ids
        // deterministically so any IL emitted during class/function compilation can reference the
        // future MethodDef token. The actual MethodDef rows get emitted later during Phase 2 when
        // the callable bodies are compiled.
        //
        // This relies on the fact that MethodDef row ids are assigned sequentially. As long as the
        // number of MethodDef rows emitted before anonymous callables is deterministic (and matches
        // our computed offset), the predicted handles will match the real handles allocated later.

        _methodDefRowCountAtPreallocation = metadataBuilder.GetRowCount(TableIndex.MethodDef);

        // Phase 2 ordering (today): declare classes + function declarations first, then compile anonymous callables.
        // We assume those steps will emit one MethodDef per corresponding callable.
        _expectedMethodDefsBeforeAnonymousCallables = callables.Count(c => c.Kind is
            CallableKind.FunctionDeclaration or
            CallableKind.ClassConstructor or
            CallableKind.ClassMethod or
            CallableKind.ClassStaticMethod or
            CallableKind.ClassStaticInitializer);

        var nextRowId = _methodDefRowCountAtPreallocation.Value + _expectedMethodDefsBeforeAnonymousCallables.Value + 1;

        foreach (var callable in callables)
        {
            if (callable.Kind is not (CallableKind.Arrow or CallableKind.FunctionExpression))
            {
                continue;
            }

            // Idempotent: if token already set, do not overwrite.
            if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
            {
                continue;
            }

            var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
            _registry.SetToken(callable, preallocated);
        }
    }

    private void PreallocatePhase1AnonymousCallablesMethodDefsInOrder(
        IReadOnlyList<CallableId> discoveredCallables,
        IReadOnlyList<CallableId> compilationOrder,
        MetadataBuilder metadataBuilder)
    {
        _methodDefRowCountAtPreallocation = metadataBuilder.GetRowCount(TableIndex.MethodDef);

        // Phase 2 ordering baseline: classes + function declarations are still emitted before anonymous callables.
        _expectedMethodDefsBeforeAnonymousCallables = discoveredCallables.Count(c => c.Kind is
            CallableKind.FunctionDeclaration or
            CallableKind.ClassConstructor or
            CallableKind.ClassMethod or
            CallableKind.ClassGetter or
            CallableKind.ClassSetter or
            CallableKind.ClassStaticMethod or
            CallableKind.ClassStaticGetter or
            CallableKind.ClassStaticSetter or
            CallableKind.ClassStaticInitializer);

        var nextRowId = _methodDefRowCountAtPreallocation.Value + _expectedMethodDefsBeforeAnonymousCallables.Value + 1;

        foreach (var callable in compilationOrder)
        {
            if (callable.Kind is not (CallableKind.Arrow or CallableKind.FunctionExpression))
            {
                continue;
            }

            if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
            {
                continue;
            }

            var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
            _registry.SetToken(callable, preallocated);
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

        // Build O(1) lookup index from AST node to CallableId (stored in CallableRegistry)
        _registry.ResetAstNodeIndex(_discoveredCallables.Count);
        
        foreach (var callable in _discoveredCallables)
        {
            // Build CallableSignature from CallableId
            // Placeholder owner type handle (is set during token allocation)
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
                _registry.IndexAstNode(callable.AstNode, callable);
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
            CallableKind.ClassMethod or CallableKind.ClassStaticMethod => TryGetClassMemberName(callable.Name) ?? "method",
            CallableKind.ClassGetter or CallableKind.ClassStaticGetter => TryGetAccessorMethodName(callable.Name, "get") ?? "get",
            CallableKind.ClassSetter or CallableKind.ClassStaticSetter => TryGetAccessorMethodName(callable.Name, "set") ?? "set",
            _ => "unknown"
        };
    }

    private static string? TryGetClassMemberName(string? callableName)
    {
        // CallableId convention for class methods: "ClassName.methodName".
        if (string.IsNullOrWhiteSpace(callableName)) return null;
        var dot = callableName.IndexOf('.');
        if (dot < 0 || dot >= callableName.Length - 1) return null;
        var member = callableName[(dot + 1)..];
        return string.IsNullOrWhiteSpace(member) ? null : member;
    }

    private static string? TryGetAccessorMethodName(string? callableName, string accessorKind)
    {
        // CallableId convention for accessors: "ClassName.get:prop" / "ClassName.set:prop".
        if (string.IsNullOrWhiteSpace(callableName)) return null;
        var dot = callableName.IndexOf('.');
        if (dot < 0 || dot >= callableName.Length - 1) return null;
        var tail = callableName[(dot + 1)..];
        var colon = tail.IndexOf(':');
        if (colon < 0 || colon >= tail.Length - 1) return null;
        var kind = tail[..colon];
        var prop = tail[(colon + 1)..];
        if (!string.Equals(kind, accessorKind, StringComparison.Ordinal)) return null;
        if (string.IsNullOrWhiteSpace(prop)) return null;
        return $"{accessorKind}_{prop}";
    }
    
    /// <summary>
    /// Phase 1: Declare all discovered callables (create method tokens).
    /// This prepares the registry with signatures and tokens.
    /// 
    /// In the current implementation, declaration may include body compilation depending on the caller.
    /// Future iterations can split signature-only declaration from body compilation.
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

        // Delegate to the existing declaration code.
        declareAction();

        if (_verbose)
        {
            _logger.WriteLine($"[TwoPhase] Phase 1 Declaration complete. Tokens allocated: {_registry.TokensAllocated}/{_registry.Count}.");
        }
    }
    
    /// <summary>
    /// Enables strict mode after Phase 1 declaration is complete.
    /// This enforces the invariant: "expression emission never triggers compilation"
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
    /// This is a pass-through to the provided compilation logic.
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
    /// <param name="skipPhase2">If true, Phase 2 body compilation is skipped (when declareAction includes body compilation).</param>
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
        RunPhase1Declaration(declareAction);

        // Phase 2: Body Compilation
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
        // Use the same naming convention as ScopeNaming.GetRegistryScopeName():
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
