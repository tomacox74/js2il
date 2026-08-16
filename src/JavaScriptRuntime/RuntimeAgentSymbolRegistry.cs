namespace JavaScriptRuntime;

public sealed class RuntimeAgentSymbolRegistry : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);
    private readonly Dictionary<Symbol, string> _keys = new();
    private bool _disposed;

    internal RuntimeAgentSymbolRegistry()
    {
    }

    internal Symbol GetOrCreate(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_gate)
        {
            ThrowIfDisposed();
            if (_symbols.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var symbol = new Symbol(key);
            _symbols.Add(key, symbol);
            _keys.Add(symbol, key);
            return symbol;
        }
    }

    internal string? GetKey(Symbol symbol)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        lock (_gate)
        {
            ThrowIfDisposed();
            return _keys.GetValueOrDefault(symbol);
        }
    }

    internal bool Contains(Symbol symbol)
    {
        ArgumentNullException.ThrowIfNull(symbol);

        lock (_gate)
        {
            return !_disposed && _keys.ContainsKey(symbol);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _symbols.Clear();
            _keys.Clear();
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RuntimeAgentSymbolRegistry));
        }
    }
}
