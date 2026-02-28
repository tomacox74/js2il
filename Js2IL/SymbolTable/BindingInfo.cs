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
    private Type? _clrType;
    private bool _isStableType;
    private Type? _stableElementClrType;

    public string Name { get; }
    public BindingKind Kind { get; }
    public Scope DeclaringScope { get; }
    public Node DeclarationNode { get; }
    // Optional: CLR runtime type known via static analysis (e.g., const fs = require('fs'))
    public Type? ClrType
    {
        get => _clrType;
        set
        {
            _clrType = value;
            if (_clrType != typeof(JavaScriptRuntime.Array))
            {
                _stableElementClrType = null;
            }
        }
    }
    
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
    public bool IsStableType
    {
        get => _isStableType;
        set
        {
            _isStableType = value;
            if (!_isStableType)
            {
                _stableElementClrType = null;
            }
        }
    }

    /// <summary>
    /// For stable JavaScriptRuntime.Array bindings, indicates a conservative stable CLR type
    /// for numeric element values (e.g., string when all observed indexed writes are strings).
    /// Null means unknown or unstable element type.
    /// </summary>
    public Type? StableElementClrType
    {
        get => _stableElementClrType;
        set
        {
            if (value != null && (!IsStableType || ClrType != typeof(JavaScriptRuntime.Array)))
            {
                _stableElementClrType = null;
                return;
            }

            _stableElementClrType = value;
        }
    }

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
