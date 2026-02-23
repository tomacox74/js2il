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

    // Well-known symbols used by core language features.
    // These are singletons so identity comparisons work as expected.
    private static readonly Symbol _iterator = new Symbol("Symbol.iterator");
    private static readonly Symbol _asyncIterator = new Symbol("Symbol.asyncIterator");
    private static readonly Symbol _toStringTag = new Symbol("Symbol.toStringTag");

    private readonly long _id;

    public string? Description { get; }

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

    // Well-known symbol: Symbol.iterator
    public static Symbol iterator => _iterator;

    // Well-known symbol: Symbol.asyncIterator
    public static Symbol asyncIterator => _asyncIterator;

    // Well-known symbol: Symbol.toStringTag
    public static Symbol toStringTag => _toStringTag;

    // Access well-known symbols via property-read lowering (e.g., Symbol.iterator).
    // Returns null (JS undefined) when the well-known symbol is not supported.
    public static object? GetWellKnown(string name)
    {
        return name switch
        {
            "iterator" => iterator,
            "asyncIterator" => asyncIterator,
            "toStringTag" => toStringTag,
            _ => null
        };
    }

    // Useful for debugging, but keep ToString() JS-like.
    public string DebugId => $"Symbol({_id})";
}
