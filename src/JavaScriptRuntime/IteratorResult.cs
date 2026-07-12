namespace JavaScriptRuntime;

public interface IIteratorResult
{
    object? value { get; }
    bool done { get; }
}

/// <summary>
/// Strongly-typed iterator result object of the form: { value: any, done: boolean }.
///
/// NOTE: Field names are intentionally lower-case to match JS property lookups
/// in this runtime's reflection-based `Object.GetProperty`.
/// </summary>
public class IteratorResultObject<T> : IIteratorResult
{
    public T? value;
    public bool done;

    public IteratorResultObject(T? value, bool done)
    {
        this.value = value;
        this.done = done;
    }

    object? IIteratorResult.value => value;
    bool IIteratorResult.done => done;
}

/// <summary>
/// Non-generic default for iterator results (equivalent to <see cref="IteratorResultObject{T}"/> with <c>T=object</c>).
/// </summary>
public sealed class IteratorResultObject : IteratorResultObject<object?>
{
    public IteratorResultObject(object? value, bool done)
        : base(value, done)
    {
    }
}

/// <summary>
/// Helper for creating iterator result objects of the form: { value: any, done: boolean }.
/// </summary>
public static class IteratorResult
{
    public static IteratorResultObject Create(object? value, bool done)
    {
        return new IteratorResultObject(value, done);
    }

    public static IteratorResultObject<T> Create<T>(T? value, bool done)
    {
        return new IteratorResultObject<T>(value, done);
    }

    /// <summary>
    /// Converts an internal, allocation-light iterator result into the ordinary
    /// JavaScript-visible object returned by <c>Iterator.prototype.next()</c> and the
    /// various built-in iterator "next" methods: a real ordinary object (backed by
    /// <see cref="JsObject"/>) with own data properties "value" then "done" in that
    /// order (per ECMA-262 7.4.7 CreateIteratorResultObject), writable/enumerable/
    /// configurable, and [[Prototype]] set to %Object.prototype%.
    /// <para>
    /// Internal consumers (for-of, spread, destructuring, generator delegation) must
    /// keep using <see cref="IJavaScriptIterator.Next"/> / <see cref="IIteratorResult"/>
    /// directly instead of this method, so the hot iteration path is not affected by
    /// the extra allocation this performs.
    /// </para>
    /// </summary>
    public static object ToOrdinaryObject(IIteratorResult result)
        => ToOrdinaryObject(result.value, result.done);

    public static object ToOrdinaryObject(object? value, bool done)
    {
        var obj = Object.CreateOrdinaryObject();
        obj.SetValue("value", value);
        obj.SetBoolean("done", done);
        return obj;
    }
}
