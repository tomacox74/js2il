namespace JavaScriptRuntime;

/// <summary>
/// Public contract for JavaScript array values exposed across host boundaries.
/// </summary>
public interface IJavaScriptArray : IReadOnlyList<object?>
{
    new object? this[int index] { get; set; }

    object? this[double index] { get; set; }

    double length { get; set; }
}
