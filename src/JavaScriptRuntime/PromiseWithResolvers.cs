namespace JavaScriptRuntime;

public sealed class PromiseWithResolvers
{
    public PromiseWithResolvers(Promise promise, object resolve, object reject)
    {
        this.promise = promise;
        var resolveFunction =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(resolve)
            ?? throw new ArgumentNullException(nameof(resolve));
        var rejectFunction =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(reject)
            ?? throw new ArgumentNullException(nameof(reject));
        this.resolve = Node.AsyncContextRuntime.BindCurrentCallback(
            resolveFunction);
        this.reject = Node.AsyncContextRuntime.BindCurrentCallback(
            rejectFunction);
    }

    // Note: These member names are intentionally lowercase to match JS property access
    // via ObjectRuntime.GetItem/GetProperty (which is case-sensitive for host properties).
    public Promise promise { get; }

    public object resolve { get; }

    public object reject { get; }
}
