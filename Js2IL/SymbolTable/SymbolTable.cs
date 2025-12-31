namespace Js2IL.SymbolTables;

/// <summary>
/// Represents the symbol table for JavaScript source code.
/// Contains the complete scope hierarchy and provides access to all symbols and functions.
/// </summary>
public class SymbolTable
{
    public Scope Root { get; }

    public SymbolTable(Scope root)
    {
        Root = root;
    }

    /// <summary>
    /// Retrieve a binding based on its specific name
    /// </summary>
    public BindingInfo? GetBindingInfo(string fqn)
    {
        var parts = fqn.Split('/');
        var currentScope = Root;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            var nextScope = currentScope.Children.FirstOrDefault(s => s.Name == part);
            if (nextScope == null)
            {
                return null; // Scope not found
            }
            currentScope = nextScope;
        }
        var bindingName = parts.Last();
        currentScope.Bindings.TryGetValue(bindingName, out var bindingInfo);
        return bindingInfo; 
    }

    /// <summary>
    /// Finds a scope by its AST node, searching recursively from the root.
    /// </summary>
    public Scope? FindScopeByAstNode(Acornima.Ast.Node astNode)
    {
        return FindScopeByAstNodeRecursive(Root, astNode);
    }

    private static Scope? FindScopeByAstNodeRecursive(Scope scope, Acornima.Ast.Node astNode)
    {
        if (scope.AstNode == astNode)
            return scope;

        foreach (var child in scope.Children)
        {
            var found = FindScopeByAstNodeRecursive(child, astNode);
            if (found != null)
                return found;
        }

        return null;
    }
}

