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
