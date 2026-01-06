using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.Services.ILGenerators;
using Js2IL.Services.VariableBindings;
using Js2IL.SymbolTables;
using Js2IL.Utilities.Ecma335;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Service responsible for declaring all callables during Phase 1.
/// This service ensures that all arrows and function expressions have their
/// MethodDefinitionHandles created BEFORE any body compilation that might
/// reference them, satisfying the Milestone 1 requirement that "expression
/// emission never triggers compilation."
/// </summary>
/// <remarks>
/// The service iterates through discovered callables and compiles them in
/// a deterministic order, populating DeclaredCallableStore. When expression
/// emission later encounters an arrow or function expression, it can lookup
/// the pre-declared handle instead of triggering compilation.
/// </remarks>
internal sealed class CallableDeclarationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DeclaredCallableStore _declaredCallableStore;
    private readonly ILogger _logger;
    private readonly bool _verbose;
    
    private MetadataBuilder? _metadataBuilder;
    private MethodBodyStreamEncoder? _methodBodyStreamEncoder;
    private BaseClassLibraryReferences? _bclReferences;
    private ClassRegistry? _classRegistry;
    private FunctionRegistry? _functionRegistry;
    private Variables? _rootVariables;
    private SymbolTable? _symbolTable;

    public CallableDeclarationService(
        IServiceProvider serviceProvider,
        DeclaredCallableStore declaredCallableStore,
        ILogger logger,
        CompilerOptions compilerOptions)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _declaredCallableStore = declaredCallableStore ?? throw new ArgumentNullException(nameof(declaredCallableStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verbose = compilerOptions?.Verbose ?? false;
    }

    /// <summary>
    /// Configures the service with the metadata context needed for compilation.
    /// Must be called before DeclareAllCallables.
    /// </summary>
    public void Configure(
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bclReferences,
        ClassRegistry classRegistry,
        FunctionRegistry functionRegistry,
        Variables rootVariables,
        SymbolTable symbolTable)
    {
        _metadataBuilder = metadataBuilder ?? throw new ArgumentNullException(nameof(metadataBuilder));
        _methodBodyStreamEncoder = methodBodyStreamEncoder;
        _bclReferences = bclReferences ?? throw new ArgumentNullException(nameof(bclReferences));
        _classRegistry = classRegistry ?? throw new ArgumentNullException(nameof(classRegistry));
        _functionRegistry = functionRegistry ?? throw new ArgumentNullException(nameof(functionRegistry));
        _rootVariables = rootVariables ?? throw new ArgumentNullException(nameof(rootVariables));
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
    }

    /// <summary>
    /// Declares all callable signatures and compiles bodies for the given discovered callables.
    /// This ensures all handles are available in DeclaredCallableStore before any subsequent
    /// expression emission that might reference them.
    /// </summary>
    /// <param name="callables">The callables discovered by CallableDiscovery.</param>
    public void DeclareAllCallables(IReadOnlyList<CallableId> callables)
    {
        if (_metadataBuilder == null || _methodBodyStreamEncoder == null || 
            _bclReferences == null || _classRegistry == null || 
            _functionRegistry == null || _rootVariables == null || _symbolTable == null)
        {
            throw new InvalidOperationException("Service must be configured before declaring callables.");
        }

        if (_verbose)
        {
            _logger.WriteLine($"[CallableDeclaration] Declaring {callables.Count} callables...");
        }

        // IMPORTANT: During this phase, strict mode must be OFF so that nested arrows
        // encountered during compilation can still be compiled on-demand.
        // We are building up the DeclaredCallableStore, not consuming it yet.
        var previousStrictMode = _declaredCallableStore.StrictMode;
        _declaredCallableStore.StrictMode = false;

        int arrowCount = 0;
        int funcExprCount = 0;

        try
        {
            foreach (var callable in callables)
            {
                switch (callable.Kind)
                {
                    case CallableKind.Arrow:
                        if (callable.AstNode is ArrowFunctionExpression arrowExpr)
                        {
                            // Check if already declared (may have been compiled during nested function processing)
                            if (_declaredCallableStore.TryGetHandle(arrowExpr, out _))
                            {
                                continue;
                            }
                            
                            // Skip arrows that are inside class scopes - they need class context
                            // (ClassesGenerator will handle them during class compilation)
                            if (IsInsideClassScope(callable))
                            {
                                if (_verbose)
                                {
                                    _logger.WriteLine($"[CallableDeclaration] Skipping arrow inside class: {callable.Location}");
                                }
                                continue;
                            }
                            
                            DeclareArrowFunction(callable, arrowExpr);
                            arrowCount++;
                        }
                        break;

                    case CallableKind.FunctionExpression:
                        if (callable.AstNode is FunctionExpression funcExpr)
                        {
                            // Check if already declared
                            if (_declaredCallableStore.TryGetHandle(funcExpr, out _))
                            {
                                continue;
                            }
                            
                            // Skip function expressions inside class scopes
                            if (IsInsideClassScope(callable))
                            {
                                if (_verbose)
                                {
                                    _logger.WriteLine($"[CallableDeclaration] Skipping function expression inside class: {callable.Location}");
                                }
                                continue;
                            }
                            
                            DeclareFunctionExpression(callable, funcExpr);
                            funcExprCount++;
                        }
                        break;

                    // Function declarations and class methods are handled by existing generators
                    // which already populate the DeclaredCallableStore
                    case CallableKind.FunctionDeclaration:
                    case CallableKind.ClassConstructor:
                    case CallableKind.ClassMethod:
                    case CallableKind.ClassStaticMethod:
                        // These are handled by JavaScriptFunctionGenerator and ClassesGenerator
                        break;
                }
            }
        }
        finally
        {
            // Restore strict mode (will be enabled by coordinator after all declarations complete)
            _declaredCallableStore.StrictMode = previousStrictMode;
        }

        if (_verbose)
        {
            _logger.WriteLine($"[CallableDeclaration] Declared {arrowCount} arrows, {funcExprCount} function expressions.");
        }
    }

    private void DeclareArrowFunction(CallableId callable, ArrowFunctionExpression arrowExpr)
    {
        // Find the scope for this arrow function
        var arrowScope = _symbolTable!.FindScopeByAstNode(arrowExpr);
        if (arrowScope == null)
        {
            if (_verbose)
            {
                _logger.WriteLine($"[CallableDeclaration] Warning: Could not find scope for arrow at {callable.Location}");
            }
            return;
        }

        // Build parameter names
        var paramNames = ILMethodGenerator.ExtractParameterNames(arrowExpr.Params).ToArray();
        
        // Construct registry scope name and IL method name
        var arrowBaseScopeName = callable.Name != null
            ? $"ArrowFunction_{callable.Name}"
            : $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column}";
        var moduleName = _symbolTable.Root.Name;
        var registryScopeName = $"{moduleName}/{arrowBaseScopeName}";
        var ilMethodName = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column}";

        // Find the parent variables context for this arrow
        var parentScope = arrowScope.Parent;
        var parentVariables = FindVariablesForScope(parentScope);
        
        if (parentVariables == null)
        {
            if (_verbose)
            {
                _logger.WriteLine($"[CallableDeclaration] Warning: Could not find parent variables for arrow at {callable.Location}");
            }
            return;
        }

        // Create the arrow function generator and compile
        var arrowGen = new JavaScriptArrowFunctionGenerator(
            _serviceProvider,
            parentVariables,
            _bclReferences!,
            _metadataBuilder!,
            _methodBodyStreamEncoder!.Value,
            _classRegistry!,
            _functionRegistry!,
            _symbolTable);

        var methodHandle = arrowGen.GenerateArrowFunctionMethod(arrowExpr, registryScopeName, ilMethodName, paramNames);
        
        // Handle is registered by the generator itself via DeclaredCallableStore.RegisterArrowFunction
        
        if (_verbose)
        {
            _logger.WriteLine($"[CallableDeclaration] Declared arrow: {ilMethodName}");
        }
    }

    private void DeclareFunctionExpression(CallableId callable, FunctionExpression funcExpr)
    {
        // Find the scope for this function expression
        var funcScope = _symbolTable!.FindScopeByAstNode(funcExpr);
        if (funcScope == null)
        {
            if (_verbose)
            {
                _logger.WriteLine($"[CallableDeclaration] Warning: Could not find scope for function expression at {callable.Location}");
            }
            return;
        }

        // Build parameter names
        var paramNames = ILMethodGenerator.ExtractParameterNames(funcExpr.Params).ToArray();

        // Construct registry scope name and IL method name
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
            baseScopeName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";
        }
        
        var moduleName = _symbolTable.Root.Name;
        var registryScopeName = $"{moduleName}/{baseScopeName}";
        var ilMethodName = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";

        // Find the parent variables context
        var parentScope = funcScope.Parent;
        var parentVariables = FindVariablesForScope(parentScope);
        
        if (parentVariables == null)
        {
            if (_verbose)
            {
                _logger.WriteLine($"[CallableDeclaration] Warning: Could not find parent variables for function expression at {callable.Location}");
            }
            return;
        }

        // Create an ILMethodGenerator to compile the function expression
        var methodGen = new ILMethodGenerator(
            _serviceProvider,
            parentVariables,
            _bclReferences!,
            _metadataBuilder!,
            _methodBodyStreamEncoder!.Value,
            _classRegistry,
            _functionRegistry,
            symbolTable: _symbolTable);

        var methodHandle = methodGen.GenerateFunctionExpressionMethod(funcExpr, registryScopeName, ilMethodName, paramNames);
        
        // Handle is registered by GenerateFunctionExpressionMethod via DeclaredCallableStore.RegisterFunctionExpression
        
        if (_verbose)
        {
            _logger.WriteLine($"[CallableDeclaration] Declared function expression: {ilMethodName}");
        }
    }

    /// <summary>
    /// Checks if a callable is declared inside a class scope (constructor, method, etc.)
    /// These callables need class context and should be handled by ClassesGenerator.
    /// </summary>
    private bool IsInsideClassScope(CallableId callable)
    {
        // Walk up the scope tree to see if we're inside a class
        var scope = _symbolTable!.FindScopeByAstNode(callable.AstNode);
        while (scope != null)
        {
            if (scope.Kind == ScopeKind.Class)
            {
                return true;
            }
            scope = scope.Parent;
        }
        return false;
    }

    /// <summary>
    /// Finds the Variables context for a given scope by walking up the scope tree.
    /// </summary>
    private Variables? FindVariablesForScope(Scope? scope)
    {
        if (scope == null) return _rootVariables;
        
        // For global scope, return the root variables
        if (scope.Kind == ScopeKind.Global)
        {
            return _rootVariables;
        }
        
        // For nested scopes, we need to construct Variables appropriately
        // This is complex because Variables instances are typically created during
        // the compilation process. For now, return root variables and let the
        // generator create child Variables as needed.
        return _rootVariables;
    }
}
