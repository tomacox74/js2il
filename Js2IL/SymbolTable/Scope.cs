using Acornima.Ast;

namespace Js2IL.SymbolTables;

public enum ScopeKind
{
    Global,
    Function,
    Block,
    Class
}

public class Scope
{
    /// <summary>
    /// The name of the scope (used as the class name in .NET codegen).
    /// </summary>
    public string Name { get; }
    public Scope? Parent { get; }
    public List<Scope> Children { get; } = new();
    public Dictionary<string, BindingInfo> Bindings { get; } = new();
    // Names of parameters (for function scopes) so we can avoid generating backing fields for them.
    public HashSet<string> Parameters { get; } = new();
    /// <summary>
    /// Names of parameters that come from destructuring patterns (e.g., {x, y} in function foo({x, y})).
    /// These require fields because they're extracted from the incoming object, not passed directly as IL arguments.
    /// </summary>
    public HashSet<string> DestructuredParameters { get; } = new();
    public ScopeKind Kind { get; }
    public Node AstNode { get; }

    /// <summary>
    /// Authoritative .NET namespace for this scope's generated type (if any).
    /// When null, generators may apply a default.
    /// </summary>
    public string? DotNetNamespace { get; set; }

    /// <summary>
    /// Authoritative .NET simple type name for this scope's generated type (if any).
    /// When null, generators may use <see cref="Name"/>.
    /// </summary>
    public string? DotNetTypeName { get; set; }

    /// <summary>
    /// Indicates whether this scope (or its child scopes) references variables declared in parent scopes.
    /// Set during symbol table construction for classes and functions.
    /// Used by code generators to determine if scope arrays need to be passed/stored.
    /// </summary>
    public bool ReferencesParentScopeVariables { get; set; }

    public Scope(string name, ScopeKind kind, Scope? parent, Node astNode)
    {
        Name = name;
        Kind = kind;
        Parent = parent;
        AstNode = astNode;
        Parent?.Children.Add(this);
    }

    /// <summary>
    /// Gets the fully qualified scope name by walking up the parent chain.
    /// E.g., "Point/constructor" for a constructor in the Point class.
    /// For the global scope, returns its name directly.
    /// </summary>
    public string GetQualifiedName()
    {
        // Special case: global scope returns its name
        if (Kind == ScopeKind.Global)
        {
            return Name;
        }
        
        var parts = new System.Collections.Generic.List<string>();
        var current = this;
        while (current != null)
        {
            // Don't include the global scope in the qualified path
            if (current.Kind != ScopeKind.Global)
            {
                parts.Add(current.Name);
            }
            current = current.Parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }
}
