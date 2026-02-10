using Acornima.Ast;

namespace Js2IL.SymbolTables;

public enum BindingKind
{
    Var,
    Let,
    Const,
    Function,
    Global
}

public class BindingInfo
{
    public string Name { get; }
    public BindingKind Kind { get; }
    public Scope DeclaringScope { get; }
    public Node DeclarationNode { get; }
    // Optional: CLR runtime type known via static analysis (e.g., const fs = require('fs'))
    public Type? ClrType { get; set; }
    
    /// <summary>
    /// Indicates whether this variable is captured (referenced) by any child scope.
    /// When false, the variable can be optimized to use local variables instead of fields.
    /// </summary>
    public bool IsCaptured { get; set; }

    /// <summary>
    /// Indicates whether the variable's type has been inferred during static analysis
    /// and is known to never change during the variable's lifetime.
    /// When true, any attempt to change ClrType is a bug.
    /// </summary>
    public bool IsStableType { get; set; }

    /// <summary>
    /// True when this binding is the target of any write operation (assignment/update/initializer).
    /// This is used for conservative optimizations that require proving a binding is never reassigned.
    /// </summary>
    public bool HasWrite { get; set; }

    public BindingInfo(string name, BindingKind kind, Scope declaringScope, Node declarationNode)
    {
        Name = name;
        Kind = kind;
        DeclaringScope = declaringScope;
        DeclarationNode = declarationNode;
    }
}