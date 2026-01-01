namespace Js2IL.SymbolTables;

/// <summary>
/// A reference to a symbol in the symbol table.
/// </summary>
public sealed class Symbol
{
    public Symbol(BindingInfo bindingInfo)
    {
        Name = bindingInfo.Name;
        _bindingInfo = bindingInfo;
    }

    public string Name { get; }
    private readonly BindingInfo _bindingInfo;

    public BindingKind Kind => _bindingInfo.Kind;
}