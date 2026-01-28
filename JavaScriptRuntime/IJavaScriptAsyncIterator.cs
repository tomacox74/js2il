namespace JavaScriptRuntime;

/// <summary>
/// Async iterator interface used by for await..of lowering.
///
/// Next/Return return a JavaScript value that may be a Promise (or a plain value).
/// The compiler always awaits these results.
/// </summary>
public interface IJavaScriptAsyncIterator
{
    object? Next();

    bool HasReturn { get; }

    object? Return();
}
