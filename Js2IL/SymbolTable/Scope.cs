using Acornima.Ast;
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Stable inferred CLR types for JavaScript class instance fields.
    /// Populated by SymbolTableBuilder inference and consumed by class/code generators.
    /// Only includes fields where all observed assignments agree on a supported CLR type.
    /// </summary>
    public Dictionary<string, Type> StableInstanceFieldClrTypes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Stable inferred user-class types for JavaScript class instance fields.
    /// Value is the JavaScript class name (scope name) for the constructed instance.
    /// Only includes fields where all observed assignments agree on a single user class type.
    /// </summary>
    public Dictionary<string, string> StableInstanceFieldUserClassNames { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Stable inferred CLR return type for this callable scope.
    /// Only populated for very conservative cases (currently class methods with a single, top-level return).
    /// When null, the callable returns <see cref="object"/> (JavaScript value) in IL.
    /// </summary>
    public Type? StableReturnClrType { get; set; }
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

    /// <summary>
    /// Indicates whether this is an async function scope.
    /// Set during symbol table construction for function scopes.
    /// Used by TypeGenerator to add async state fields (_asyncState, _deferred).
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// The number of await expressions in this async function scope.
    /// Set during symbol table construction for async function scopes.
    /// Used by TypeGenerator to add awaited result fields (_awaited1, _awaited2, etc.).
    /// </summary>
    public int AwaitPointCount { get; set; }

    /// <summary>
    /// Indicates whether this is a generator function scope.
    /// Set during symbol table construction for function scopes.
    /// Used by TypeGenerator/IL emission to include generator state fields.
    /// </summary>
    public bool IsGenerator { get; set; }

    /// <summary>
    /// The number of yield expressions in this generator function scope.
    /// Set during symbol table construction for generator function scopes.
    /// Used for diagnostics and potential future storage decisions.
    /// </summary>
    public int YieldPointCount { get; set; }

    public Scope(string name, ScopeKind kind, Scope? parent, Node astNode)
    {
        Name = name;
        Kind = kind;
        Parent = parent;
        AstNode = astNode;
        Parent?.Children.Add(this);
    }

    /// <summary>
    /// Finds a symbol by name in this scope and parent scopes
    /// </summary>
    /// <remarks>
    /// This method throws if the symbol is not found.
    /// </remarks>
    public Symbol FindSymbol(string name)
    {
        if (this.Bindings.ContainsKey(name) == false)
        {
            if (Parent != null)
            {
                return Parent.FindSymbol(name);
            }
            
            // parent is null and we have undeclard symbol
            // this actually can occur.. for example "console" or "require"
            var globalBinding = new BindingInfo(name, BindingKind.Global, AstNode);
            Bindings[name] = globalBinding;
            return new Symbol(globalBinding);
        }
        
        var bindingInfo = Bindings[name];
        return new Symbol(bindingInfo);
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
