namespace JavaScriptRuntime;

/// <summary>
/// Strongly-typed iterator used by for..of and runtime helpers.
/// This avoids ExpandoObject/dynamic member dispatch for built-in iterables.
/// </summary>
public interface IJavaScriptIterator
{
    IteratorResultObject Next();

    bool HasReturn { get; }

    void Return();
}

/// <summary>
/// Generic iterator interface for cases where the yielded value type is known.
///
/// When consumed via the non-generic <see cref="IJavaScriptIterator"/>, implementations should
/// also provide an explicit <see cref="IJavaScriptIterator.Next"/> that returns an object-typed
/// result.
/// </summary>
public interface IJavaScriptIterator<T> : IJavaScriptIterator
{
    new IteratorResultObject<T> Next();
}
