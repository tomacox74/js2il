using Acornima.Ast;

namespace Js2IL.SymbolTables;

public enum BindingKind
{
    Var,
    Let,
    Const,
    Function
}

public class BindingInfo
{
    public string Name { get; }
    public BindingKind Kind { get; }
    public Node DeclarationNode { get; }
    // Optional: CLR runtime type known via static analysis (e.g., const fs = require('fs'))
    public Type? ClrType { get; set; }
    
    /// <summary>
    /// Indicates whether this variable is captured (referenced) by any child scope.
    /// When false, the variable can be optimized to use local variables instead of fields.
    /// </summary>
    public bool IsCaptured { get; set; }

    public BindingInfo(string name, BindingKind kind, Node declarationNode)
    {
        Name = name;
        Kind = kind;
        DeclarationNode = declarationNode;
    }
}