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
using Microsoft.Extensions.Logging;

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
/// - See docs/compiler/TwoPhaseCompilationPipeline.md for the full design.
/// </remarks>
public sealed class TwoPhaseCompilationCoordinator
{
    /// <summary>
    /// The namespace used for generated function types (arrows, function expressions, etc.).
    /// </summary>
    public const string FunctionsNamespace = "Functions";

    private readonly Microsoft.Extensions.Logging.ILogger<TwoPhaseCompilationCoordinator> _diagnosticLogger;
    private readonly bool _diagnosticsEnabled;
    
    // Registry for storing callable declarations (CallableId-keyed, per design doc)
    // Injected from DI - single instance per compilation
    private readonly CallableRegistry _registry;

    private readonly FunctionTypeMetadataRegistry _functionTypeMetadataRegistry;
    private readonly AnonymousCallableTypeMetadataRegistry _anonymousCallableTypeMetadataRegistry;
    
    private IReadOnlyList<CallableId>? _discoveredCallables;

    /// <summary>
    /// Last dependency plan computed for this compilation (if computed).
    /// </summary>
    public CompilationPlan? LastComputedPlan { get; private set; }

    // Used to validate that preallocated MethodDef row ids stay stable.
    private int? _methodDefRowCountAtPreallocation;
    private int? _expectedMethodDefsBeforeAnonymousCallables;

    public TwoPhaseCompilationCoordinator(
        CompilerOptions compilerOptions,
        CallableRegistry registry,
        Microsoft.Extensions.Logging.ILogger<TwoPhaseCompilationCoordinator>? diagnosticLogger = null,
        FunctionTypeMetadataRegistry? functionTypeMetadataRegistry = null,
        AnonymousCallableTypeMetadataRegistry? anonymousCallableTypeMetadataRegistry = null)
    {
        _diagnosticLogger = diagnosticLogger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<TwoPhaseCompilationCoordinator>.Instance;
        _diagnosticsEnabled = compilerOptions?.DiagnosticsEnabled ?? false;
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _functionTypeMetadataRegistry = functionTypeMetadataRegistry ?? new FunctionTypeMetadataRegistry();
        _anonymousCallableTypeMetadataRegistry = anonymousCallableTypeMetadataRegistry ?? new AnonymousCallableTypeMetadataRegistry();
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
            PreallocatePhase1AnonymousCallablesMethodDefsInOrder(_discoveredCallables, _discoveredCallables, metadataBuilder);
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
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        Action<IReadOnlyList<CallableId>> compileAnonymousCallablesPhase2,
        Action compileClassesAndFunctionsPhase2)
    {
        RunPhase1Discovery(symbolTable);

        IReadOnlyList<CallableId>? orderedNonClassCallables = null;

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
            // Deterministic MethodDef-row allocation:
            // Allocate non-class callables (FunctionDeclaration/FunctionExpression/Arrow) in discovery
            // (scope-tree) order so enclosing callable owner types always precede nested callables.
            //
            // This avoids CoreCLR load-time BadImageFormatException when a nested callable owner type
            // appears before its enclosing owner type in the TypeDef table.
            var methodDefRowCountBefore = metadataBuilder.GetRowCount(TableIndex.MethodDef);
            var nextRowId = methodDefRowCountBefore + 1;

            static bool IsNonClassCallable(CallableId c) =>
                c.Kind is CallableKind.FunctionDeclaration or CallableKind.FunctionExpression or CallableKind.Arrow;

            static int GetCallableDeclaringScopeDepth(SymbolTable symbolTable, CallableId callable)
            {
                if (callable.AstNode == null)
                {
                    return 0;
                }

                // For function declarations/expressions/arrows, FindScopeByAstNode returns the callable's own scope.
                // The declaring scope is the parent scope where the binding occurs.
                var callableScope = symbolTable.FindScopeByAstNode(callable.AstNode);
                var declaringScope = callableScope?.Parent;

                var depth = 0;
                while (declaringScope != null && declaringScope.Parent != null)
                {
                    depth++;
                    declaringScope = declaringScope.Parent;
                }

                return depth;
            }

            // Allocate MethodDef handles so enclosing callables (shallower declaring scope) come first.
            // This guarantees the owner TypeDef RID ordering respects nesting constraints when we later
            // declare owner types in MethodDef-row order.
            orderedNonClassCallables = _discoveredCallables
                .Where(IsNonClassCallable)
                .Select((c, index) => (Callable: c, Index: index, Depth: GetCallableDeclaringScopeDepth(symbolTable, c)))
                .OrderBy(x => x.Depth)
                .ThenBy(x => x.Index)
                .Select(x => x.Callable)
                .ToList();

            foreach (var callable in orderedNonClassCallables)
            {
                if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
                {
                    continue;
                }

                var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
                _registry.SetToken(callable, preallocated);
                if (callable.AstNode != null)
                {
                    _registry.SetDeclaredTokenForAstNode(callable.AstNode, preallocated);
                }
            }

            _ = PreallocatePhase1ClassCallablesMethodDefsFromRowId(symbolTable, nextRowId);

            // Predeclare callable owner types (function declarations + anonymous callables) now that
            // MethodDef tokens are allocated.
            var moduleTypeRegistry = serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>();
            if (!moduleTypeRegistry.TryGet(symbolTable.Root.Name, out var moduleTypeHandle) || moduleTypeHandle.IsNil)
            {
                throw new InvalidOperationException(
                    $"Missing module type handle for module '{symbolTable.Root.Name}' during callable-owner predeclaration.");
            }

            DeclareFunctionAndAnonymousOwnerTypesForNesting(
                symbolTable,
                metadataBuilder,
                bclReferences,
                moduleTypeHandleForNesting: moduleTypeHandle,
                nestedTypeRelationshipRegistry: serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>(),
                declareFunctionDeclarationOwnerTypes: true,
                declareAnonymousOwnerTypes: true);
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

            // Compile and finalize ALL non-class callables (FunctionDeclaration + Arrow + FunctionExpression)
            // in the exact order used for Phase 1 MethodDef preallocation.
            CompileAndFinalizePhase2NonClassCallablesInDiscoveryOrder(
                symbolTable,
                metadataBuilder,
                serviceProvider,
                bclReferences,
                methodBodyStreamEncoder,
                classRegistry,
                orderedNonClassCallables);

            // Compile class constructors/methods/accessors/.cctor bodies in planned Phase 2.
            // Compile bodies in plan order, then emit MethodDef rows grouped by class type (TypeDef order)
            // to satisfy ECMA-335 contiguous method list requirements.
            CompileAndFinalizePhase2ClassCallables(
                symbolTable,
                metadataBuilder,
                ordered,
                serviceProvider,
                methodBodyStreamEncoder,
                classRegistry);
        });
    }

    private void CompileAndFinalizePhase2NonClassCallablesInDiscoveryOrder(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry,
        IReadOnlyList<CallableId>? orderedNonClassCallables = null)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var callables = orderedNonClassCallables ?? _discoveredCallables;
        foreach (var callable in callables)
        {
            switch (callable.Kind)
            {
                case CallableKind.FunctionDeclaration:
                {
                    if (callable.AstNode is FunctionDeclaration fd)
                    {
                        CompileAndFinalizeSingleFunctionDeclaration(
                            callable,
                            fd,
                            symbolTable,
                            metadataBuilder,
                            serviceProvider,
                            bclReferences,
                            methodBodyStreamEncoder);
                    }
                    break;
                }

                case CallableKind.Arrow:
                {
                    if (callable.AstNode is ArrowFunctionExpression arrowExpr)
                    {
                        CompileArrowFunction(
                            callable,
                            arrowExpr,
                            metadataBuilder,
                            serviceProvider,
                            bclReferences,
                            methodBodyStreamEncoder,
                            symbolTable);
                    }
                    break;
                }

                case CallableKind.FunctionExpression:
                {
                    if (callable.AstNode is FunctionExpression funcExpr)
                    {
                        CompileFunctionExpression(
                            callable,
                            funcExpr,
                            metadataBuilder,
                            serviceProvider,
                            bclReferences,
                            methodBodyStreamEncoder,
                            symbolTable);
                    }
                    break;
                }
            }
        }
    }

    private void CompileAndFinalizeSingleFunctionDeclaration(
        CallableId callable,
        FunctionDeclaration funcDecl,
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder)
    {
        if (_registry.IsBodyCompiledForAstNode(funcDecl))
        {
            return;
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

        var moduleName = symbolTable.Root.Name;
        var funcScope = symbolTable.FindScopeByAstNode(funcDecl);
        if (funcScope == null)
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionDeclaration scope not found: {callable.DisplayName}");
        }

        const string ilMethodName = "__js_call__";

        // Get the signature to determine if scopes parameter is required
        if (!_registry.TryGetSignature(callable, out var signature))
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionDeclaration signature not found: {callable.DisplayName}");
        }

        var methodCompiler = serviceProvider.GetRequiredService<JsMethodCompiler>();
        var body = methodCompiler.TryCompileCallableBody(
            callable: callable,
            expectedMethodDef: expected,
            ilMethodName: ilMethodName,
            node: funcDecl,
            scope: funcScope,
            methodBodyStreamEncoder: methodBodyStreamEncoder,
            isInstanceMethod: false,
            hasScopesParameter: signature.RequiresScopesParameter,
            scopesFieldHandle: null,
            returnsVoid: false);

        if (body == null)
        {
            var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
            var extra = string.IsNullOrWhiteSpace(lastFailure) ? string.Empty : $" (IR failure: {lastFailure})";
            var functionName = (funcDecl.Id as Identifier)?.Name ?? callable.Name ?? "anonymous";
            throw new NotSupportedException(
                $"[TwoPhase] IR pipeline could not compile function declaration '{functionName}' in scope '{funcScope.GetQualifiedName()}'.{extra}");
        }

        // Ensure owner type is registered (it should have been predeclared in Phase 1).
        var functionNameForRegistry = (funcDecl.Id as Identifier)?.Name ?? callable.Name ?? "anonymous";
        if (!_functionTypeMetadataRegistry.TryGet(moduleName, callable.DeclaringScopeName, functionNameForRegistry, out var ownerType) || ownerType.IsNil)
        {
            var declTb = new TypeBuilder(metadataBuilder, string.Empty, functionNameForRegistry);
            ownerType = declTb.AddTypeDefinition(
                TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                bclReferences.ObjectType,
                firstFieldOverride: null,
                firstMethodOverride: expected);
            _functionTypeMetadataRegistry.Add(moduleName, callable.DeclaringScopeName, functionNameForRegistry, ownerType);

            var moduleTypeRegistry = serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>();
            if (moduleTypeRegistry.TryGet(moduleName, out var moduleTypeHandle) && !moduleTypeHandle.IsNil)
            {
                serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>().Add(ownerType, moduleTypeHandle);
            }
        }

        var irTb = new TypeBuilder(metadataBuilder, string.Empty, functionNameForRegistry);
        _ = MethodDefinitionFinalizer.EmitMethod(metadataBuilder, irTb, body);

        _registry.MarkBodyCompiledForAstNode(funcDecl);
    }

    /// <summary>
    /// Predeclares function-declaration owner types (Modules.&lt;ModuleName&gt;.&lt;FunctionName&gt;) before
    /// scope TypeDefs are emitted. This is required for CLR loadability when the function's scope type
    /// is nested under the function owner type (Modules.&lt;Module&gt;.&lt;FunctionName&gt;/Scope).
    ///
    /// IMPORTANT: caller must pass the MethodDef row count that will exist AFTER scope constructors
    /// are emitted for this module. Scope type generation emits one .ctor MethodDef per scope type.
    /// We preallocate callable MethodDef handles relative to that future row count so TypeDef.MethodList
    /// pointers remain correct.
    /// </summary>
    internal void PredeclareFunctionDeclarationOwnerTypesForNesting(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        int? methodDefBaseRowOverride = null)
    {
        if (symbolTable == null) throw new ArgumentNullException(nameof(symbolTable));
        if (metadataBuilder == null) throw new ArgumentNullException(nameof(metadataBuilder));
        if (bclReferences == null) throw new ArgumentNullException(nameof(bclReferences));

        // Backward-compatible wrapper: allocate callable MethodDef handles first, then declare
        // function/anonymous owner TypeDefs. NOTE: caller is responsible for declaring class
        // TypeDefs before calling this if they want TypeDef.MethodList to remain monotonic.
        PreallocateCallableMethodDefsForNesting(symbolTable, metadataBuilder, methodDefBaseRowOverride);
        DeclareFunctionAndAnonymousOwnerTypesForNesting(symbolTable, metadataBuilder, bclReferences);
    }

    /// <summary>
    /// Phase 1 helper: preallocate MethodDef handles for all callables in a module.
    /// This does NOT emit any TypeDefs.
    /// </summary>
    internal void PreallocateCallableMethodDefsForNesting(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        int? methodDefBaseRowOverride = null)
    {
        // Ensure callables are discovered and declared in the registry.
        RunPhase1Discovery(symbolTable);

        var discoveredCallables = _discoveredCallables;
        if (discoveredCallables == null)
        {
            return;
        }

        var nextRowId = methodDefBaseRowOverride ?? (metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1);

        static bool IsNonClassCallable(CallableId c) =>
            c.Kind is CallableKind.FunctionDeclaration or CallableKind.FunctionExpression or CallableKind.Arrow;

        static int GetCallableDeclaringScopeDepth(SymbolTable symbolTable, CallableId callable)
        {
            if (callable.AstNode == null)
            {
                return 0;
            }

            var callableScope = symbolTable.FindScopeByAstNode(callable.AstNode);
            var declaringScope = callableScope?.Parent;

            var depth = 0;
            while (declaringScope != null && declaringScope.Parent != null)
            {
                depth++;
                declaringScope = declaringScope.Parent;
            }

            return depth;
        }

        // Allocate MethodDef handles so that enclosing callables (shallower declaring scope) come first.
        // This ensures TypeDef RID ordering respects nesting constraints when we later declare owner types
        // in MethodDef-row order.
        var orderedCallables = discoveredCallables
            .Where(IsNonClassCallable)
            .Select((c, index) => (Callable: c, Index: index, Depth: GetCallableDeclaringScopeDepth(symbolTable, c)))
            .OrderBy(x => x.Depth)
            .ThenBy(x => x.Index)
            .Select(x => x.Callable);

        foreach (var callable in orderedCallables)
        {
            if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
            {
                continue;
            }

            var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
            _registry.SetToken(callable, preallocated);
            if (callable.AstNode != null)
            {
                _registry.SetDeclaredTokenForAstNode(callable.AstNode, preallocated);
            }
        }

        _ = PreallocatePhase1ClassCallablesMethodDefsFromRowId(symbolTable, nextRowId);
    }

    /// <summary>
    /// Phase 1 helper: declare callable owner TypeDefs (Modules.* function owners and Functions.* anonymous owners).
    /// Requires callable MethodDef tokens to already be allocated in <see cref="CallableRegistry"/>.
    /// </summary>
    internal void DeclareFunctionAndAnonymousOwnerTypesForNesting(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        BaseClassLibraryReferences bclReferences,
        TypeDefinitionHandle moduleTypeHandleForNesting = default,
        NestedTypeRelationshipRegistry? nestedTypeRelationshipRegistry = null,
        bool declareFunctionDeclarationOwnerTypes = true,
        bool declareAnonymousOwnerTypes = true)
    {
        if (_discoveredCallables == null)
        {
            // Caller must run discovery first.
            RunPhase1Discovery(symbolTable);
        }

        var discoveredCallables = _discoveredCallables;
        if (discoveredCallables == null)
        {
            return;
        }

        var moduleName = symbolTable.Root.Name;

        var canNest = !moduleTypeHandleForNesting.IsNil && nestedTypeRelationshipRegistry != null;

        // When scope types are emitted as NestedPrivate, any nested callable that references an
        // outer callable's Scope type must be nested under that outer callable-owner type.
        // Otherwise the CLR will throw TypeAccessException at runtime.
        //
        // To make nesting robust regardless of discovery/declaration order, we:
        // 1) declare/register owner TypeDefs
        // 2) in a second pass, emit NestedClass relationships once all enclosing types exist
        var pendingNest = new List<(TypeDefinitionHandle OwnerType, string DeclaringScopeName)>();

        static string GetFunctionDeclarationName(CallableId callable)
        {
            if (callable.AstNode is FunctionDeclaration fd)
            {
                var name = (fd.Id as Identifier)?.Name;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
            }

            return callable.Name ?? "anonymous";
        }

        TypeDefinitionHandle ResolveEnclosingType(string declaringScopeName)
        {
            if (!canNest)
            {
                throw new InvalidOperationException("ResolveEnclosingType requires a moduleTypeHandleForNesting and nestedTypeRelationshipRegistry.");
            }

            if (string.Equals(declaringScopeName, moduleName, StringComparison.Ordinal))
            {
                return moduleTypeHandleForNesting;
            }

            var segments = declaringScopeName
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = segments.Length - 1; i >= 1; i--)
            {
                var candidateName = segments[i];

                var candidateDeclaringScope = string.Join("/", segments.Take(i));

                if (_functionTypeMetadataRegistry.TryGet(moduleName, candidateDeclaringScope, candidateName, out var functionOwner) && !functionOwner.IsNil)
                {
                    return functionOwner;
                }

                if (_anonymousCallableTypeMetadataRegistry.TryGetOwnerTypeHandle(moduleName, candidateDeclaringScope, candidateName, out var anonOwner)
                    && !anonOwner.IsNil)
                {
                    return anonOwner;
                }
            }

            return moduleTypeHandleForNesting;
        }

        // IMPORTANT: declare ALL callable owner types in MethodDef-row order.
        // - Keeps TypeDef.MethodList monotonic (TypeBuilder firstMethodOverride constraints)
        // - Ensures enclosing owner types are created before nested owner types (CoreCLR loader requirement)
        if (declareAnonymousOwnerTypes || declareFunctionDeclarationOwnerTypes)
        {
            bool ShouldDeclare(CallableId c) => c.Kind switch
            {
                CallableKind.FunctionDeclaration => declareFunctionDeclarationOwnerTypes,
                CallableKind.FunctionExpression or CallableKind.Arrow => declareAnonymousOwnerTypes,
                _ => false
            };

            var ordered = discoveredCallables
                .Where(ShouldDeclare)
                .Select(c =>
                {
                    if (!_registry.TryGetDeclaredToken(c, out var tok) || tok.Kind != HandleKind.MethodDefinition)
                    {
                        throw new InvalidOperationException($"[TwoPhase] Callable missing declared token during predeclare: {c.DisplayName}");
                    }

                    var mdh = (MethodDefinitionHandle)tok;
                    if (mdh.IsNil)
                    {
                        throw new InvalidOperationException($"[TwoPhase] Callable has nil token during predeclare: {c.DisplayName}");
                    }

                    return (Callable: c, Method: mdh);
                })
                .OrderBy(x => MetadataTokens.GetRowNumber(x.Method))
                .ToList();

            foreach (var (callable, expected) in ordered)
            {
                TypeDefinitionHandle ownerTypeHandle;

                if (callable.Kind == CallableKind.FunctionDeclaration)
                {
                    var functionName = GetFunctionDeclarationName(callable);

                    if (!_functionTypeMetadataRegistry.TryGet(moduleName, callable.DeclaringScopeName, functionName, out ownerTypeHandle) || ownerTypeHandle.IsNil)
                    {
                        var tb = new TypeBuilder(metadataBuilder, string.Empty, functionName);
                        ownerTypeHandle = tb.AddTypeDefinition(
                            TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                            bclReferences.ObjectType,
                            firstFieldOverride: null,
                            firstMethodOverride: expected);

                        _functionTypeMetadataRegistry.Add(moduleName, callable.DeclaringScopeName, functionName, ownerTypeHandle);
                    }
                }
                else
                {
                    string ilMethodName;
                    switch (callable.Kind)
                    {
                        case CallableKind.Arrow:
                        {
                            if (callable.AstNode is not ArrowFunctionExpression arrowExpr)
                            {
                                throw new InvalidOperationException($"[TwoPhase] Expected ArrowFunctionExpression node for {callable.DisplayName}");
                            }
                            // Prefer the actual scope name authored by SymbolTableBuilder.
                            var scope = symbolTable.FindScopeByAstNode(arrowExpr);
                            if (scope != null && !string.IsNullOrWhiteSpace(scope.Name))
                            {
                                ilMethodName = scope.Name;
                            }
                            else
                            {
                                var col1Based = arrowExpr.Location.Start.Column + 1;
                                ilMethodName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{col1Based}";
                            }
                            break;
                        }
                        case CallableKind.FunctionExpression:
                        {
                            if (callable.AstNode is not FunctionExpression funcExpr)
                            {
                                throw new InvalidOperationException($"[TwoPhase] Expected FunctionExpression node for {callable.DisplayName}");
                            }
                            var scope = symbolTable.FindScopeByAstNode(funcExpr);
                            if (scope != null && !string.IsNullOrWhiteSpace(scope.Name))
                            {
                                ilMethodName = scope.Name;
                            }
                            else
                            {
                                var col0Based = funcExpr.Location.Start.Column;
                                ilMethodName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{col0Based}";
                            }
                            break;
                        }
                        default:
                            continue;
                    }

                    if (!_anonymousCallableTypeMetadataRegistry.TryGetOwnerTypeHandle(moduleName, callable.DeclaringScopeName, ilMethodName, out ownerTypeHandle)
                        || ownerTypeHandle.IsNil)
                    {
                        var tb = new TypeBuilder(metadataBuilder, string.Empty, ilMethodName);
                        ownerTypeHandle = tb.AddTypeDefinition(
                            TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                            bclReferences.ObjectType,
                            firstFieldOverride: null,
                            firstMethodOverride: expected);

                        _anonymousCallableTypeMetadataRegistry.Add(moduleName, callable.DeclaringScopeName, ilMethodName, ownerTypeHandle);
                    }
                }

                if (canNest)
                {
                    pendingNest.Add((ownerTypeHandle, callable.DeclaringScopeName));
                }
            }
        }

        if (canNest)
        {
            foreach (var (ownerType, declaringScopeName) in pendingNest)
            {
                var enclosing = ResolveEnclosingType(declaringScopeName);
                nestedTypeRelationshipRegistry!.Add(ownerType, enclosing);
            }
        }
    }

    private int PreallocatePhase1ClassCallablesMethodDefsFromRowId(SymbolTable symbolTable, int startRowId)
    {
        if (_discoveredCallables == null)
        {
            return startRowId;
        }

        var nextRowId = startRowId;

        foreach (var classScope in EnumerateClassScopesInDeclarationOrder(symbolTable.Root))
        {
            if (!TryGetClassBody(classScope, out var classBody))
            {
                continue;
            }

            foreach (var callable in GetClassCallablesInDeclarationOrder(classScope, classBody))
            {
                if (_registry.TryGetDeclaredToken(callable, out var existingToken) && !existingToken.IsNil)
                {
                    continue;
                }

                var preallocated = MetadataTokens.MethodDefinitionHandle(nextRowId++);
                _registry.SetToken(callable, preallocated);
            }
        }

        return nextRowId;
    }

    private int PreallocatePhase1FunctionDeclarationMethodDefsFromRowId(SymbolTable symbolTable, int startRowId)
    {
        if (_discoveredCallables == null)
        {
            return startRowId;
        }

        var nextRowId = startRowId;
        var functionDeclCallables = _discoveredCallables
            .Where(c => c.Kind == CallableKind.FunctionDeclaration)
            .ToList();

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
            }
        }

        return nextRowId;
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
            if (!TryGetClassBody(classScope, out var classBody))
            {
                continue;
            }

            foreach (var callable in GetClassCallablesInDeclarationOrder(classScope, classBody))
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

    private void PreallocatePhase1FunctionDeclarationMethodDefs(SymbolTable symbolTable, MetadataBuilder metadataBuilder)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var startRowId = metadataBuilder.GetRowCount(TableIndex.MethodDef) + 1;
        _ = PreallocatePhase1FunctionDeclarationMethodDefsFromRowId(symbolTable, startRowId);
    }

    private void CompileAndFinalizePhase2ClassCallables(
        SymbolTable symbolTable,
        MetadataBuilder metadataBuilder,
        IReadOnlyList<CallableId> plannedOrder,
        IServiceProvider serviceProvider,
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

            var (classScope, classNode, className) = ResolveClassScope(symbolTable, callable);
            var classBody = GetClassBodyOrThrow(classNode);
            var hasScopes = classRegistry.TryGetPrivateField(className, "_scopes", out var scopesField);

            CompiledCallableBody body;
            switch (callable.Kind)
            {
                case CallableKind.ClassConstructor:
                {
                    Node? ctorNodeOverride = callable.AstNode as Acornima.Ast.MethodDefinition;
                    if (ctorNodeOverride == null)
                    {
                        // For synthetic constructor callables, prefer the actual constructor MethodDefinition if present.
                        ctorNodeOverride = classBody.Body
                            .OfType<Acornima.Ast.MethodDefinition>()
                            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
                    }

                    body = methodCompiler.CompileClassConstructorBodyTwoPhase(
                        callable: callable,
                        expectedMethodDef: expected,
                        methodBodyStreamEncoder: methodBodyStreamEncoder,
                        symbolTable: symbolTable,
                        classScope: classScope,
                        classNode: classNode,
                        ctorNodeOverride: ctorNodeOverride,
                        needsScopes: hasScopes);
                    break;
                }

                case CallableKind.ClassStaticInitializer:
                {
                    body = methodCompiler.CompileClassStaticInitializerBodyTwoPhase(
                        callable: callable,
                        expectedMethodDef: expected,
                        methodBodyStreamEncoder: methodBodyStreamEncoder,
                        classScope: classScope,
                        classNode: classNode);
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

                    body = methodCompiler.CompileClassMethodBodyTwoPhase(
                        callable: callable,
                        expectedMethodDef: expected,
                        methodBodyStreamEncoder: methodBodyStreamEncoder,
                        classRegistry: classRegistry,
                        symbolTable: symbolTable,
                        classScope: classScope,
                        methodDef: methodDef,
                        clrMethodName: clrMethodName,
                        hasScopes: hasScopes);
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
            if (!TryGetClassBody(classScope, out var classBody))
            {
                continue;
            }

            var ns = classScope.DotNetNamespace ?? "Classes";
            var name = classScope.DotNetTypeName ?? classScope.Name;
            var tb = new TypeBuilder(metadataBuilder, ns, name);

            foreach (var callable in GetClassCallablesInDeclarationOrder(classScope, classBody))
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

    private IEnumerable<CallableId> GetClassCallablesInDeclarationOrder(Scope classScope, ClassBody classBody)
    {
        var className = classScope.Name;

        // Constructor (explicit or synthetic)
        var ctorMember = classBody.Body
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
        foreach (var member in classBody.Body.OfType<Acornima.Ast.MethodDefinition>().Where(m => m.Key is Identifier))
        {
            var methodName = ((Identifier)member.Key).Name;
            if (methodName == "constructor") continue;

            if (_registry.TryGetCallableIdForAstNode(member, out var methodCallable))
            {
                yield return methodCallable;
            }
        }

        // Static initializer (.cctor) if needed (legacy ordering: after methods)
        bool hasStaticFieldInits = classBody.Body.OfType<Acornima.Ast.PropertyDefinition>()
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

    private static (Scope ClassScope, Node ClassNode, string ClassName) ResolveClassScope(SymbolTable symbolTable, CallableId callable)
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
        if (classScope == null || classScope.AstNode is not (ClassDeclaration or ClassExpression))
        {
            throw new InvalidOperationException($"[TwoPhase] Class scope not found for callable: {callable.DisplayName} (DeclaringScope='{callable.DeclaringScopeName}', ClassName='{className}')");
        }

        var classNode = (Node)classScope.AstNode;

        // ClassRegistry keys use CLR full names (namespace + type) to avoid collisions across modules.
        var ns = classScope.DotNetNamespace ?? "Classes";
        var typeName = classScope.DotNetTypeName ?? classScope.Name;
        var registryClassName = $"{ns}.{typeName}";

        return (classScope, classNode, registryClassName);
    }

    private static bool TryGetClassBody(Scope classScope, out ClassBody classBody)
    {
        if (classScope.AstNode is ClassDeclaration classDecl)
        {
            classBody = classDecl.Body;
            return true;
        }

        if (classScope.AstNode is ClassExpression classExpr)
        {
            classBody = classExpr.Body;
            return true;
        }

        classBody = null!;
        return false;
    }

    private static ClassBody GetClassBodyOrThrow(Node classNode)
    {
        return classNode switch
        {
            ClassDeclaration classDecl => classDecl.Body,
            ClassExpression classExpr => classExpr.Body,
            _ => throw new InvalidOperationException($"[TwoPhase] Unsupported class node type: {classNode.Type}")
        };
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
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        ClassRegistry classRegistry)
    {
        if (_discoveredCallables == null)
        {
            return;
        }

        var moduleName = symbolTable.Root.Name;

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
                    var jsParamNames = JavaScriptParameterNameExtractor.ExtractParameterNames(fd.Params).ToArray();
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

            var functionName = (funcDecl.Id as Identifier)?.Name ?? callable.Name ?? "anonymous";
            const string ilMethodName = "__js_call__";

            // IR first
            var methodCompiler = serviceProvider.GetRequiredService<JsMethodCompiler>();
            var irBody = methodCompiler.TryCompileCallableBody(
                callable: callable,
                expectedMethodDef: expected,
                ilMethodName: ilMethodName,
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
                var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
                var extra = string.IsNullOrWhiteSpace(lastFailure) ? string.Empty : $" (IR failure: {lastFailure})";
                throw new NotSupportedException(
                    $"[TwoPhase] IR pipeline could not compile function declaration '{functionName}' in scope '{funcScope.GetQualifiedName()}'.{extra}");
            }

            compiled[callable] = body;
            _registry.MarkBodyCompiledForAstNode(funcDecl);
        }

        // Finalize MethodDef/Param rows deterministically (discovery order).
        // Owner TypeDefs are predeclared earlier (before scope TypeDefs) so CLR loaders accept
        // nesting layouts like Modules.<Module>+<FunctionName>/Scope.
        foreach (var callable in functionDeclCallables)
        {
            if (!compiled.TryGetValue(callable, out var body))
            {
                // The plan is authoritative.
                throw new InvalidOperationException($"[TwoPhase] Missing compiled body for function declaration: {callable.DisplayName}");
            }

            var functionName = (callable.AstNode as FunctionDeclaration)?.Id is Identifier id && !string.IsNullOrWhiteSpace(id.Name)
                ? id.Name
                : callable.Name ?? "anonymous";

            // Safety fallback: if the owner type wasn't predeclared for some reason, declare it now.
            // This may produce less ideal TypeDef ordering, but keeps compilation functional.
            if (!_functionTypeMetadataRegistry.TryGet(moduleName, callable.DeclaringScopeName, functionName, out var ownerType) || ownerType.IsNil)
            {
                // Owner type will be nested under the module TypeDef later via NestedClass rows.
                var declTb = new TypeBuilder(metadataBuilder, string.Empty, functionName);
                ownerType = declTb.AddTypeDefinition(
                    TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                    bclReferences.ObjectType,
                    firstFieldOverride: null,
                    firstMethodOverride: body.ExpectedMethodDef);
                _functionTypeMetadataRegistry.Add(moduleName, callable.DeclaringScopeName, functionName, ownerType);

                // IMPORTANT: a TypeDef with Nested* visibility MUST have a NestedClass table row.
                // If we had to declare the owner type late, ensure it is still nested under the
                // correct enclosing type so the assembly remains loadable.
                var moduleTypeRegistry = serviceProvider.GetRequiredService<ModuleTypeMetadataRegistry>();
                if (!moduleTypeRegistry.TryGet(moduleName, out var moduleTypeHandle) || moduleTypeHandle.IsNil)
                {
                    throw new InvalidOperationException($"[TwoPhase] Missing module type handle for module '{moduleName}' when nesting function owner type '{functionName}'.");
                }

                var nestedTypeRegistry = serviceProvider.GetRequiredService<NestedTypeRelationshipRegistry>();
                nestedTypeRegistry.Add(ownerType, moduleTypeHandle);
            }

            // Emit the MethodDef (TypeDef was already created).
            var functionTb = new TypeBuilder(metadataBuilder, $"Modules.{moduleName}", functionName);
            _ = MethodDefinitionFinalizer.EmitMethod(metadataBuilder, functionTb, body);
        }
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

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Computed dependency plan (SCC stages: {StageCount}).", plan.Stages.Count);
            _diagnosticLogger.LogInformation("{DependencyPlan}", plan.ToDebugString());
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
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        SymbolTable symbolTable)
    {
        foreach (var callable in callables)
        {
            switch (callable.Kind)
            {
                case CallableKind.Arrow:
                    if (callable.AstNode is ArrowFunctionExpression arrowExpr)
                    {
                        CompileArrowFunction(callable, arrowExpr, metadataBuilder, serviceProvider, bclReferences, methodBodyStreamEncoder, symbolTable);
                        // Note: body is marked as compiled by the generator (JavaScriptArrowFunctionGenerator)
                        // after it successfully emits the method body. No duplicate call here.
                    }
                    break;

                case CallableKind.FunctionExpression:
                    if (callable.AstNode is FunctionExpression funcExpr)
                    {
                        CompileFunctionExpression(callable, funcExpr, metadataBuilder, serviceProvider, bclReferences, methodBodyStreamEncoder, symbolTable);
                        // Note: body is marked as compiled by the coordinator after successful IR compilation.
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
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        SymbolTable symbolTable)
    {
        var moduleName = symbolTable.Root.Name;

        var arrowScope = symbolTable.FindScopeByAstNode(arrowExpr);
        var arrowTypeName = arrowScope?.Name;
        if (string.IsNullOrWhiteSpace(arrowTypeName))
        {
            // Fallback: location-based naming (1-based column) to match SymbolTableBuilder.
            var col1Based = arrowExpr.Location.Start.Column + 1;
            arrowTypeName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{col1Based}";
        }

        var arrowGen = new JavaScriptArrowFunctionGenerator(
            serviceProvider,
            bclReferences,
            metadataBuilder,
            methodBodyStreamEncoder,
            symbolTable);

        arrowGen.GenerateArrowFunctionMethod(arrowExpr, arrowTypeName);

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 2: Compiled arrow: {ArrowTypeName}", arrowTypeName);
        }
    }

    private void CompileFunctionExpression(
        CallableId callable,
        FunctionExpression funcExpr,
        MetadataBuilder metadataBuilder,
        IServiceProvider serviceProvider,
        BaseClassLibraryReferences bclReferences,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        SymbolTable symbolTable)
    {
        if (_registry.TryGetDeclaredToken(callable, out var existingToken) &&
            existingToken.Kind == HandleKind.MethodDefinition &&
            _registry.IsBodyCompiledForAstNode(funcExpr))
        {
            return;
        }

        if (!_registry.TryGetDeclaredToken(callable, out var tok) || tok.Kind != HandleKind.MethodDefinition)
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionExpression missing declared token: {callable.DisplayName}");
        }
        var expected = (MethodDefinitionHandle)tok;
        if (expected.IsNil)
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionExpression has nil token: {callable.DisplayName}");
        }

        var moduleName = symbolTable.Root.Name;
        const string ilMethodName = "__js_call__";
        var funcScope = symbolTable.FindScopeByAstNode(funcExpr);
        if (funcScope == null)
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionExpression scope not found: {callable.DisplayName}");
        }

        var funcTypeName = funcScope.Name;

        // Get the signature to determine if scopes parameter is required
        if (!_registry.TryGetSignature(callable, out var signature))
        {
            throw new InvalidOperationException($"[TwoPhase] FunctionExpression signature not found: {callable.DisplayName}");
        }

        var methodCompiler = serviceProvider.GetRequiredService<JsMethodCompiler>();
        var compiledBody = methodCompiler.TryCompileCallableBody(
            callable: callable,
            expectedMethodDef: expected,
            ilMethodName: ilMethodName,
            node: funcExpr,
            scope: funcScope,
            methodBodyStreamEncoder: methodBodyStreamEncoder,
            isInstanceMethod: false,
            hasScopesParameter: signature.RequiresScopesParameter,
            scopesFieldHandle: null,
            returnsVoid: false);

        if (compiledBody == null)
        {
            var lastFailure = IR.IRPipelineMetrics.GetLastFailure();
            var extra = string.IsNullOrWhiteSpace(lastFailure) ? string.Empty : $"\nIR failure: {lastFailure}";

            var location = funcExpr.Location;
            var locText = $"(line {location.Start.Line}, col {location.Start.Column})";

            throw new NotSupportedException(
                $"[TwoPhase] IR pipeline could not compile function expression '{funcTypeName}' in module '{moduleName}' {locText} in scope '{funcScope.GetQualifiedName()}'.{extra}");
        }

        // Two-phase: the IR compiler returns a body-only representation. We must finalize it into
        // a MethodDef/TypeDef so existing call sites (ldftn / delegate creation) reference a real
        // method body at the preallocated token.
        var irTb = new TypeBuilder(metadataBuilder, string.Empty, funcTypeName);
        _ = MethodDefinitionFinalizer.EmitMethod(metadataBuilder, irTb, compiledBody);

        _registry.SetDeclaredTokenForAstNode(funcExpr, expected);
        _registry.MarkBodyCompiledForAstNode(funcExpr);

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 2: Compiled function expression: {FunctionTypeName}", funcTypeName);
        }
    }

    private void PreallocatePhase1AnonymousCallablesMethodDefsInOrder(
        IReadOnlyList<CallableId> discoveredCallables,
        IReadOnlyList<CallableId> compilationOrder,
        MetadataBuilder metadataBuilder,
        int? methodDefRowCountAtPreallocationOverride = null)
    {
        _methodDefRowCountAtPreallocation = methodDefRowCountAtPreallocationOverride ?? metadataBuilder.GetRowCount(TableIndex.MethodDef);

        // Phase 2 ordering baseline:
        // - Function declarations are finalized before anonymous callables.
        // - Class callables are finalized after anonymous callables.
        _expectedMethodDefsBeforeAnonymousCallables = discoveredCallables.Count(c => c.Kind is CallableKind.FunctionDeclaration);

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
        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 1 Discovery: Starting callable discovery...");
        }

        var discovery = new CallableDiscovery(symbolTable);
        _discoveredCallables = discovery.DiscoverAll();

        // Build O(1) lookup index from AST node to CallableId (stored in CallableRegistry)
        _registry.ResetAstNodeIndex(_discoveredCallables.Count);
        
        foreach (var callable in _discoveredCallables)
        {
            // Compute whether this callable requires a scopes parameter.
            // For class methods, this is handled differently (they use this._scopes field).
            // For functions and arrows, we check if the scope references parent variables.
            bool requiresScopesParameter = callable.Kind switch
            {
                CallableKind.ClassMethod or 
                CallableKind.ClassGetter or 
                CallableKind.ClassSetter or
                CallableKind.ClassStaticMethod or
                CallableKind.ClassStaticGetter or
                CallableKind.ClassStaticSetter => false, // Class methods don't use scopes parameter
                _ => ComputeRequiresScopesParameter(callable, symbolTable)
            };

            // Build CallableSignature from CallableId
            // Placeholder owner type handle (is set during token allocation)
            var signature = new CallableSignature
            {
                OwnerTypeHandle = default, // Will be set during token allocation
                RequiresScopesParameter = requiresScopesParameter,
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

        if (_diagnosticsEnabled)
        {
            var stats = discovery.GetStats();
            _diagnosticLogger.LogInformation("[TwoPhase] Discovered {TotalCallables} callables:", stats.TotalCallables);
            _diagnosticLogger.LogInformation("  - Function declarations: {Count}", stats.FunctionDeclarations);
            _diagnosticLogger.LogInformation("  - Function expressions: {Count}", stats.FunctionExpressions);
            _diagnosticLogger.LogInformation("  - Arrow functions: {Count}", stats.ArrowFunctions);
            _diagnosticLogger.LogInformation("  - Class constructors: {Count}", stats.ClassConstructors);
            _diagnosticLogger.LogInformation("  - Class methods: {Count}", stats.ClassMethods);
            _diagnosticLogger.LogInformation("  - Class static methods: {Count}", stats.ClassStaticMethods);
            
            foreach (var callable in _discoveredCallables)
            {
                _diagnosticLogger.LogInformation(
                    "  [{CallableKind}] {DisplayName} (params: {ParameterCount})",
                    callable.Kind,
                    callable.DisplayName,
                    callable.JsParamCount);
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
            CallableKind.FunctionDeclaration => "__js_call__",
            CallableKind.FunctionExpression => callable.Location.HasValue 
                ? "__js_call__"
                : "__js_call__",
            CallableKind.Arrow => callable.Location.HasValue
                ? "__js_call__"
                : "__js_call__",
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
    /// Computes whether a callable requires a scopes parameter based on scope analysis.
    /// Symbol-table analysis is authoritative for parent-scope references; this method
    /// applies ABI policy on top (e.g., resumable callables remain conservative).
    /// </summary>
    private static bool ComputeRequiresScopesParameter(CallableId callable, SymbolTable symbolTable)
    {
        // Resumable callables (async/generator) currently rely on scopes plumbing in leaf-scope
        // creation and resume paths. Keep scopes parameter enabled for these callables.
        if (callable.AstNode is FunctionDeclaration fd && (fd.Async || fd.Generator))
        {
            return true;
        }

        if (callable.AstNode is FunctionExpression fe && (fe.Async || fe.Generator))
        {
            return true;
        }

        if (callable.AstNode is ArrowFunctionExpression af && af.Async)
        {
            return true;
        }

        // Try to find the scope for this callable
        if (callable.AstNode == null)
        {
            // No AST node - conservatively assume scopes are required
            return true;
        }

        var scope = symbolTable.FindScopeByAstNode(callable.AstNode);
        if (scope == null)
        {
            // Scope not found - conservatively assume scopes are required
            return true;
        }

        return scope.ReferencesParentScopeVariables;
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

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 1 Declaration: Creating method tokens...");
        }

        // Delegate to the existing declaration code.
        declareAction();

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation(
                "[TwoPhase] Phase 1 Declaration complete. Tokens allocated: {TokensAllocated}/{TokenCount}.",
                _registry.TokensAllocated,
                _registry.Count);
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
        
        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Strict mode enabled: expression emission will only lookup, not compile.");
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
        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 2: Compiling callable bodies...");
        }

        compileAction();

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Phase 2: Body compilation complete.");
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
        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Starting two-phase compilation pipeline...");
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

        if (_diagnosticsEnabled)
        {
            _diagnosticLogger.LogInformation("[TwoPhase] Two-phase compilation pipeline complete.");
        }
    }

}
