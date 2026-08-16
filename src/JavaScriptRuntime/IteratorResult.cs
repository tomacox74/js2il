namespace JavaScriptRuntime;

public interface IIteratorResult
{
    object? value { get; }
    bool done { get; }
}

/// <summary>
/// Strongly-typed iterator result object of the form: { value: any, done: boolean }.
///
/// The lower-case field names retain the typed iterator-result API, while the
/// constructor creates matching observable JavaScript own properties.
/// </summary>
public class IteratorResultObject<T> : JsObject, IIteratorResult
{
    public T? value;
    public bool done;

    public IteratorResultObject(T? value, bool done)
    {
        this.value = value;
        this.done = done;
        SetObject(nameof(value), value);
        SetBoolean(nameof(done), done);
        PrototypeChain.InitializePrototype(this, GlobalThis.ObjectPrototypeValue);
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
}
