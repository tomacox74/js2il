namespace JavaScriptRuntime;

/// <summary>
/// Minimal Symbol callable intrinsic support.
///
/// Symbols are represented as opaque unique reference objects.
/// This is sufficient for typeof/equality semantics used by tests.
/// </summary>
[IntrinsicObject("Symbol")]
public sealed class Symbol
{
    private static long _nextId;
    private static readonly Dictionary<string, Symbol> _globalRegistry = new(StringComparer.Ordinal);
    private static readonly Dictionary<Symbol, string> _symbolToRegistryKey = new(ReferenceEqualityComparer.Instance);
    private static readonly object _registryLock = new();

    // Well-known symbols used by core language features.
    // These are singletons so identity comparisons work as expected.
    private static readonly Symbol _iterator = new Symbol("Symbol.iterator");
    private static readonly Symbol _asyncIterator = new Symbol("Symbol.asyncIterator");
    private static readonly Symbol _hasInstance = new Symbol("Symbol.hasInstance");
    private static readonly Symbol _isConcatSpreadable = new Symbol("Symbol.isConcatSpreadable");
    private static readonly Symbol _match = new Symbol("Symbol.match");
    private static readonly Symbol _matchAll = new Symbol("Symbol.matchAll");
    private static readonly Symbol _replace = new Symbol("Symbol.replace");
    private static readonly Symbol _search = new Symbol("Symbol.search");
    private static readonly Symbol _species = new Symbol("Symbol.species");
    private static readonly Symbol _split = new Symbol("Symbol.split");
    private static readonly Symbol _toPrimitive = new Symbol("Symbol.toPrimitive");
    private static readonly Symbol _toStringTag = new Symbol("Symbol.toStringTag");
    private static readonly Symbol _unscopables = new Symbol("Symbol.unscopables");

    private readonly long _id;

    public string? Description { get; }
    public string? description => Description;

    public Symbol()
    {
        _id = System.Threading.Interlocked.Increment(ref _nextId);
        Description = null;
    }

    public Symbol(object? description)
    {
        _id = System.Threading.Interlocked.Increment(ref _nextId);

        // JS: undefined => no description; otherwise ToString.
        if (description is null)
        {
            Description = null;
        }
        else
        {
            Description = DotNet2JSConversions.ToString(description);
        }
    }

    // Callable form: Symbol([description])
    public static object Call()
    {
        return new Symbol();
    }

    public static object Call(object? description)
    {
        return new Symbol(description);
    }

    public override string ToString()
    {
        return Description == null ? "Symbol()" : $"Symbol({Description})";
    }

    public string toString() => ToString();

    public Symbol valueOf() => this;

    // Well-known symbol: Symbol.iterator
    public static Symbol iterator => _iterator;

    // Well-known symbol: Symbol.asyncIterator
    public static Symbol asyncIterator => _asyncIterator;

    // Well-known symbol: Symbol.hasInstance
    public static Symbol hasInstance => _hasInstance;

    // Well-known symbol: Symbol.isConcatSpreadable
    public static Symbol isConcatSpreadable => _isConcatSpreadable;

    // Well-known symbol: Symbol.match
    public static Symbol match => _match;

    // Well-known symbol: Symbol.matchAll
    public static Symbol matchAll => _matchAll;

    // Well-known symbol: Symbol.replace
    public static Symbol replace => _replace;

    // Well-known symbol: Symbol.search
    public static Symbol search => _search;

    // Well-known symbol: Symbol.species
    public static Symbol species => _species;

    // Well-known symbol: Symbol.split
    public static Symbol split => _split;

    // Well-known symbol: Symbol.toPrimitive
    public static Symbol toPrimitive => _toPrimitive;

    // Well-known symbol: Symbol.toStringTag
    public static Symbol toStringTag => _toStringTag;

    // Well-known symbol: Symbol.unscopables
    public static Symbol unscopables => _unscopables;

    // Symbol.for(key)
    public static object @for(object? key)
    {
        var registryKey = DotNet2JSConversions.ToString(key);
        lock (_registryLock)
        {
            if (_globalRegistry.TryGetValue(registryKey, out var existing))
            {
                return existing;
            }

            var created = new Symbol(registryKey);
            _globalRegistry[registryKey] = created;
            _symbolToRegistryKey[created] = registryKey;
            return created;
        }
    }

    // Symbol.keyFor(sym)
    public static object? keyFor(object? sym)
    {
        if (sym is not Symbol symbol)
        {
            throw new TypeError("Symbol.keyFor requires a symbol");
        }

        lock (_registryLock)
        {
            return _symbolToRegistryKey.TryGetValue(symbol, out var key) ? key : null;
        }
    }

    // Access well-known symbols via property-read lowering (e.g., Symbol.iterator).
    // Returns null (JS undefined) when the well-known symbol is not supported.
    public static object? GetWellKnown(string name)
    {
        return name switch
        {
            "iterator" => iterator,
            "asyncIterator" => asyncIterator,
            "hasInstance" => hasInstance,
            "isConcatSpreadable" => isConcatSpreadable,
            "match" => match,
            "matchAll" => matchAll,
            "replace" => replace,
            "search" => search,
            "species" => species,
            "split" => split,
            "toPrimitive" => toPrimitive,
            "toStringTag" => toStringTag,
            "unscopables" => unscopables,
            _ => null
        };
    }

    // Useful for debugging, but keep ToString() JS-like.
    public string DebugId => $"Symbol({_id})";
}
