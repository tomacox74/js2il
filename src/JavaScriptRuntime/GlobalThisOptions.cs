namespace JavaScriptRuntime;

public sealed class GlobalThisOptions
{
    /// <summary>
    /// Gets a value indicating whether the non-standard <c>gc</c> testing helper is exposed on the global object.
    /// </summary>
    public bool ExposeGc { get; init; }
}
