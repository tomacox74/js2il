using System.Collections.Concurrent;
using System.Reflection.Metadata;
using Acornima.Ast;

namespace Js2IL.Services.TwoPhaseCompilation;

/// <summary>
/// Stores pre-declared method handles for callables, keyed by their AST node or location.
/// This enables Phase 2 expression emission to lookup handles without triggering compilation.
/// 
/// The store is populated during Phase 1 (declaration) and consumed during Phase 2 (body compilation
/// and expression emission).
/// </summary>
/// <remarks>
/// Milestone 1: This store allows ILExpressionGenerator to lookup pre-declared handles for
/// arrow functions and function expressions instead of compiling them on-demand.
/// 
/// Keys used:
/// - ArrowFunctionExpression AST node → MethodDefinitionHandle
/// - FunctionExpression AST node → MethodDefinitionHandle  
/// - Location string (for fallback lookups) → MethodDefinitionHandle
/// </remarks>
public sealed class DeclaredCallableStore
{
    // Keyed by AST node reference for exact matching
    private readonly ConcurrentDictionary<Node, MethodDefinitionHandle> _byAstNode = new();
    
    // Keyed by location string for fallback/diagnostic lookups
    private readonly ConcurrentDictionary<string, MethodDefinitionHandle> _byLocation = new();
    
    // Keyed by scope name for function declarations
    private readonly ConcurrentDictionary<string, MethodDefinitionHandle> _byScopeName = new();

    /// <summary>
    /// Registers a method handle for an arrow function expression.
    /// </summary>
    public void RegisterArrowFunction(ArrowFunctionExpression arrowExpr, MethodDefinitionHandle handle)
    {
        _byAstNode[arrowExpr] = handle;
        
        var location = $"ArrowFunction_L{arrowExpr.Location.Start.Line}C{arrowExpr.Location.Start.Column}";
        _byLocation[location] = handle;
    }

    /// <summary>
    /// Registers a method handle for a function expression.
    /// </summary>
    public void RegisterFunctionExpression(FunctionExpression funcExpr, MethodDefinitionHandle handle)
    {
        _byAstNode[funcExpr] = handle;
        
        var location = $"FunctionExpression_L{funcExpr.Location.Start.Line}C{funcExpr.Location.Start.Column}";
        _byLocation[location] = handle;
    }

    /// <summary>
    /// Registers a method handle by scope name (for function declarations).
    /// </summary>
    public void RegisterByScopeName(string scopeName, MethodDefinitionHandle handle)
    {
        _byScopeName[scopeName] = handle;
    }

    /// <summary>
    /// Attempts to get the pre-declared method handle for an AST node.
    /// </summary>
    public bool TryGetHandle(Node node, out MethodDefinitionHandle handle)
    {
        return _byAstNode.TryGetValue(node, out handle);
    }

    /// <summary>
    /// Attempts to get the pre-declared method handle by location string.
    /// </summary>
    public bool TryGetHandleByLocation(string location, out MethodDefinitionHandle handle)
    {
        return _byLocation.TryGetValue(location, out handle);
    }

    /// <summary>
    /// Attempts to get the pre-declared method handle by scope name.
    /// </summary>
    public bool TryGetHandleByScopeName(string scopeName, out MethodDefinitionHandle handle)
    {
        return _byScopeName.TryGetValue(scopeName, out handle);
    }

    /// <summary>
    /// Checks if a handle exists for the given AST node.
    /// </summary>
    public bool Contains(Node node)
    {
        return _byAstNode.ContainsKey(node);
    }

    /// <summary>
    /// Gets statistics about the store contents.
    /// </summary>
    public (int ByAstNode, int ByLocation, int ByScopeName) GetStats()
    {
        return (_byAstNode.Count, _byLocation.Count, _byScopeName.Count);
    }

    /// <summary>
    /// Clears all stored handles (for testing or recompilation scenarios).
    /// </summary>
    public void Clear()
    {
        _byAstNode.Clear();
        _byLocation.Clear();
        _byScopeName.Clear();
    }
}
