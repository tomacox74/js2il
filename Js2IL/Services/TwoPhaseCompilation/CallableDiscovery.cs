using Acornima.Ast;
using Js2IL.SymbolTables;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Discovers all callables in a module by walking the AST and symbol table.
/// This is Phase 1 step 1: identify every callable that will need declaration.
/// </summary>
/// <remarks>
/// CallableDiscovery produces a complete list of CallableIds without compiling anything.
/// The discovery is deterministic: the same AST always produces the same set of CallableIds
/// in the same order.
/// </remarks>
public sealed class CallableDiscovery
{
    private readonly SymbolTable _symbolTable;
    private readonly string _moduleName;
    private readonly List<CallableId> _discovered = new();

    public CallableDiscovery(SymbolTable symbolTable)
    {
        _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
        _moduleName = symbolTable.Root.Name;
    }

    /// <summary>
    /// Discovers all callables in the module.
    /// Returns callables in a deterministic order suitable for declaration.
    /// </summary>
    public IReadOnlyList<CallableId> DiscoverAll()
    {
        _discovered.Clear();
        
        // Discover callables from the symbol table scope tree
        DiscoverFromScope(_symbolTable.Root, _moduleName);
        
        return _discovered.AsReadOnly();
    }

    private void DiscoverFromScope(Scope scope, string currentScopeName)
    {
        foreach (var child in scope.Children)
        {
            switch (child.Kind)
            {
                case ScopeKind.Function:
                    DiscoverFunction(child, currentScopeName);
                    break;
                    
                case ScopeKind.Class:
                    DiscoverClass(child, currentScopeName);
                    break;
                    
                case ScopeKind.Block:
                    // Recurse into block scopes (may contain nested functions/classes)
                    var blockScopeName = $"{currentScopeName}/{child.Name}";
                    DiscoverFromScope(child, blockScopeName);
                    break;
            }
        }
    }

    private void DiscoverFunction(Scope functionScope, string parentScopeName)
    {
        var astNode = functionScope.AstNode;
        
        if (astNode is FunctionDeclaration funcDecl)
        {
            var funcName = (funcDecl.Id as Identifier)?.Name ?? functionScope.Name;
            var paramCount = funcDecl.Params.Count;
            
            var callableId = new CallableId
            {
                Kind = CallableKind.FunctionDeclaration,
                DeclaringScopeName = parentScopeName,
                Name = funcName,
                JsParamCount = paramCount,
                AstNode = funcDecl
            };
            
            _discovered.Add(callableId);
            
            // Recurse into nested functions
            var functionScopeName = $"{_moduleName}/{funcName}";
            DiscoverFromScope(functionScope, functionScopeName);
        }
        else if (astNode is FunctionExpression funcExpr)
        {
            var location = SourceLocation.FromNode(funcExpr);
            var funcName = (funcExpr.Id as Identifier)?.Name;
            var paramCount = funcExpr.Params.Count;
            
            var callableId = new CallableId
            {
                Kind = CallableKind.FunctionExpression,
                DeclaringScopeName = parentScopeName,
                Name = funcName, // May be null for anonymous
                Location = location,
                JsParamCount = paramCount,
                AstNode = funcExpr
            };
            
            _discovered.Add(callableId);
            
            // Recurse into nested functions
            var scopeName = funcName != null 
                ? $"{_moduleName}/{funcName}" 
                : $"{_moduleName}/FunctionExpression_{location}";
            DiscoverFromScope(functionScope, scopeName);
        }
        else if (astNode is ArrowFunctionExpression arrowExpr)
        {
            var location = SourceLocation.FromNode(arrowExpr);
            var paramCount = arrowExpr.Params.Count;
            
            // Try to get assignment target name from scope
            string? assignmentTarget = null;
            if (functionScope.Name.StartsWith("ArrowFunction_") && 
                !functionScope.Name.StartsWith("ArrowFunction_L"))
            {
                assignmentTarget = functionScope.Name.Substring("ArrowFunction_".Length);
            }
            
            var callableId = new CallableId
            {
                Kind = CallableKind.Arrow,
                DeclaringScopeName = parentScopeName,
                Name = assignmentTarget, // May be null for inline arrows
                Location = location,
                JsParamCount = paramCount,
                AstNode = arrowExpr
            };
            
            _discovered.Add(callableId);
            
            // Recurse into nested functions (arrows can contain nested arrows/functions)
            var scopeName = assignmentTarget != null 
                ? $"{_moduleName}/ArrowFunction_{assignmentTarget}" 
                : $"{_moduleName}/ArrowFunction_{location}";
            DiscoverFromScope(functionScope, scopeName);
        }
    }

    private void DiscoverClass(Scope classScope, string parentScopeName)
    {
        var astNode = classScope.AstNode;
        
        if (astNode is not ClassDeclaration classDecl)
            return;
            
        var className = (classDecl.Id as Identifier)?.Name ?? classScope.Name;
        
        // Discover constructor
        var ctor = classDecl.Body.Body
            .OfType<MethodDefinition>()
            .FirstOrDefault(m => (m.Key as Identifier)?.Name == "constructor");
            
        if (ctor != null)
        {
            var ctorParamCount = (ctor.Value as FunctionExpression)?.Params.Count ?? 0;
            
            var ctorId = new CallableId
            {
                Kind = CallableKind.ClassConstructor,
                DeclaringScopeName = parentScopeName,
                Name = className,
                JsParamCount = ctorParamCount,
                AstNode = ctor
            };
            
            _discovered.Add(ctorId);
        }
        else
        {
            // Default constructor
            var ctorId = new CallableId
            {
                Kind = CallableKind.ClassConstructor,
                DeclaringScopeName = parentScopeName,
                Name = className,
                JsParamCount = 0,
                AstNode = classDecl
            };
            
            _discovered.Add(ctorId);
        }
        
        // Discover methods
        foreach (var member in classDecl.Body.Body.OfType<MethodDefinition>())
        {
            var methodKey = member.Key as Identifier;
            if (methodKey == null) continue;
            
            var methodName = methodKey.Name;
            if (methodName == "constructor") continue; // Already handled
            
            var methodParamCount = (member.Value as FunctionExpression)?.Params.Count ?? 0;
            var location = SourceLocation.FromNode(member);
            
            var methodId = new CallableId
            {
                Kind = member.Static ? CallableKind.ClassStaticMethod : CallableKind.ClassMethod,
                DeclaringScopeName = parentScopeName,
                Name = $"{className}.{methodName}",
                Location = location,
                JsParamCount = methodParamCount,
                AstNode = member
            };
            
            _discovered.Add(methodId);
        }
        
        // Recurse into class scope for any nested callables in method bodies
        // (e.g., arrows defined inside methods)
        var classScopeName = $"{_moduleName}/{className}";
        foreach (var child in classScope.Children)
        {
            if (child.Kind == ScopeKind.Function && child.AstNode is FunctionExpression)
            {
                // This is a method body scope - check for nested callables
                DiscoverFromScope(child, classScopeName);
            }
        }
    }

    /// <summary>
    /// Gets statistics about the discovered callables.
    /// </summary>
    public DiscoveryStats GetStats()
    {
        return new DiscoveryStats
        {
            TotalCallables = _discovered.Count,
            FunctionDeclarations = _discovered.Count(c => c.Kind == CallableKind.FunctionDeclaration),
            FunctionExpressions = _discovered.Count(c => c.Kind == CallableKind.FunctionExpression),
            ArrowFunctions = _discovered.Count(c => c.Kind == CallableKind.Arrow),
            ClassConstructors = _discovered.Count(c => c.Kind == CallableKind.ClassConstructor),
            ClassMethods = _discovered.Count(c => c.Kind == CallableKind.ClassMethod),
            ClassStaticMethods = _discovered.Count(c => c.Kind == CallableKind.ClassStaticMethod)
        };
    }
}

/// <summary>
/// Statistics about discovered callables.
/// </summary>
public record struct DiscoveryStats
{
    public int TotalCallables { get; init; }
    public int FunctionDeclarations { get; init; }
    public int FunctionExpressions { get; init; }
    public int ArrowFunctions { get; init; }
    public int ClassConstructors { get; init; }
    public int ClassMethods { get; init; }
    public int ClassStaticMethods { get; init; }
}
