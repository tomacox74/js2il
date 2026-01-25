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

    // Useful for debugging, but keep ToString() JS-like.
    public string DebugId => $"Symbol({_id})";
}
