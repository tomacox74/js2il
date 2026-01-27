namespace JavaScriptRuntime;

/// <summary>
/// Strongly-typed iterator result object of the form: { value: any, done: boolean }.
///
/// NOTE: Field names are intentionally lower-case to match JS property lookups
/// in this runtime's reflection-based `Object.GetProperty`.
/// </summary>
public sealed class IteratorResultObject
{
    public object? value;
    public bool done;

    public IteratorResultObject(object? value, bool done)
    {
        this.value = value;
        this.done = done;
    }
}

/// <summary>
/// Helper for creating iterator result objects of the form: { value: any, done: boolean }.
/// </summary>
public static class IteratorResult
{
    public static object Create(object? value, bool done)
    {
        return new IteratorResultObject(value, done);
    }
}
