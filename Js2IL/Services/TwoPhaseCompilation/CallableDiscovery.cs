using Acornima.Ast;
using Js2IL.SymbolTables;
using Js2IL.Utilities;

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
            var functionScopeName = $"{parentScopeName}/{funcName}";
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
            // IMPORTANT: scope name must match SymbolTableBuilder naming so nested DeclaringScopeName
            // values line up with Scope.GetQualifiedName() used across the pipeline.
            // SymbolTableBuilder may name anonymous function-expression scopes using an assignment target.
            var scopeName = funcName != null
                ? $"{parentScopeName}/{funcName}"
                : $"{parentScopeName}/{functionScope.Name}";
            DiscoverFromScope(functionScope, scopeName);
        }
        else if (astNode is ArrowFunctionExpression arrowExpr)
        {
            var location = SourceLocation.FromNode(arrowExpr);
            var paramCount = arrowExpr.Params.Count;
            
            var callableId = new CallableId
            {
                Kind = CallableKind.Arrow,
                DeclaringScopeName = parentScopeName,
                Name = null,
                Location = location,
                JsParamCount = paramCount,
                AstNode = arrowExpr
            };
            
            _discovered.Add(callableId);
            
            // Recurse into nested functions (arrows can contain nested arrows/functions)
            // IMPORTANT: scope name must match SymbolTableBuilder naming (1-based column).
            var col1Based = arrowExpr.Location.Start.Column + 1;
            var scopeName = $"{parentScopeName}/ArrowFunction_L{arrowExpr.Location.Start.Line}C{col1Based}";
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
                // For synthetic callables we still want a stable AST node for indexing,
                // but it must be unique per callable (CallableRegistry indexes Node -> CallableId).
                // Use ClassBody for the default ctor; use ClassDeclaration for .cctor.
                AstNode = classDecl.Body
            };
            
            _discovered.Add(ctorId);
        }
        
        // Check if the class has static fields with initializers -> needs a .cctor
        bool hasStaticFieldInits = classDecl.Body.Body.OfType<PropertyDefinition>()
            .Any(p => p.Static && p.Value != null);
        if (hasStaticFieldInits)
        {
            var cctorId = new CallableId
            {
                Kind = CallableKind.ClassStaticInitializer,
                DeclaringScopeName = parentScopeName,
                Name = className,
                JsParamCount = 0,
                AstNode = classDecl
            };
            _discovered.Add(cctorId);
        }
        
        // Discover methods
        foreach (var member in classDecl.Body.Body.OfType<MethodDefinition>().Where(m => m.Key is Identifier))
        {
            var methodKey = (Identifier)member.Key;
            
            var methodName = methodKey.Name;
            if (methodName == "constructor") continue; // Already handled
            
            var methodParamCount = (member.Value as FunctionExpression)?.Params.Count ?? 0;
            var location = SourceLocation.FromNode(member);

            // Distinguish methods vs accessors so CallableId keys remain unique.
            // Acornima models class members as MethodDefinition with a Kind (PropertyKind).
            // We keep CLR method naming concerns in the emitters; here we just classify callables.
            CallableKind kind;
            var memberKind = member.Kind;
            if (memberKind == PropertyKind.Get)
            {
                kind = member.Static ? CallableKind.ClassStaticGetter : CallableKind.ClassGetter;
            }
            else if (memberKind == PropertyKind.Set)
            {
                kind = member.Static ? CallableKind.ClassStaticSetter : CallableKind.ClassSetter;
            }
            else
            {
                kind = member.Static ? CallableKind.ClassStaticMethod : CallableKind.ClassMethod;
            }

            var callableName = kind switch
            {
                CallableKind.ClassGetter or CallableKind.ClassStaticGetter => JavaScriptCallableNaming.MakeClassAccessorCallableName(className, "get", methodName),
                CallableKind.ClassSetter or CallableKind.ClassStaticSetter => JavaScriptCallableNaming.MakeClassAccessorCallableName(className, "set", methodName),
                _ => JavaScriptCallableNaming.MakeClassMethodCallableName(className, methodName)
            };
            
            var methodId = new CallableId
            {
                Kind = kind,
                DeclaringScopeName = parentScopeName,
                Name = callableName,
                Location = location,
                JsParamCount = methodParamCount,
                AstNode = member
            };
            
            _discovered.Add(methodId);
        }
        
        // Recurse into class scope for any nested callables in method bodies
        // (e.g., arrows defined inside methods)
        var classScopeName = $"{parentScopeName}/{className}";
        foreach (var child in classScope.Children)
        {
            if (child.Kind == ScopeKind.Function && child.AstNode is FunctionExpression)
            {
                // This is a method body scope - check for nested callables
                // IMPORTANT: include the method scope name so nested callables end up with a
                // DeclaringScopeName that matches Scope.GetQualifiedName() (e.g. "C/m" or "C/constructor").
                var methodScopeName = $"{classScopeName}/{child.Name}";
                DiscoverFromScope(child, methodScopeName);
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
            ClassStaticMethods = _discovered.Count(c => c.Kind == CallableKind.ClassStaticMethod),
            ClassStaticInitializers = _discovered.Count(c => c.Kind == CallableKind.ClassStaticInitializer)
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
    public int ClassStaticInitializers { get; init; }
}
