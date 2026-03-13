namespace Js2IL.SymbolTables;

/// <summary>
/// A reference to a symbol in the symbol table.
/// </summary>
public sealed class Symbol
{
    public Symbol(BindingInfo bindingInfo)
    {
        Name = bindingInfo.Name;
        BindingInfo = bindingInfo;
    }

    public string Name { get; }
    
    /// <summary>
    /// The underlying binding info for this symbol. Useful for identity comparison
    /// when the same variable name exists in different scopes (shadowing).
    /// </summary>
    public BindingInfo BindingInfo { get; }

    public BindingKind Kind => BindingInfo.Kind;
}