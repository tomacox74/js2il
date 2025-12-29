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
}

