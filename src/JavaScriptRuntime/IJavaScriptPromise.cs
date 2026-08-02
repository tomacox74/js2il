namespace JavaScriptRuntime;

/// <summary>
/// Public contract for JavaScript promise values exposed across host boundaries.
/// </summary>
public interface IJavaScriptPromise
{
    IJavaScriptPromise then(object? onFulfilled = null, object? onRejected = null);

    IJavaScriptPromise @catch(object? onRejected);

    IJavaScriptPromise @finally(object? onFinally);
}
