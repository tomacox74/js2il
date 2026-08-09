namespace JavaScriptRuntime;

public sealed class PromiseWithResolvers
{
    public PromiseWithResolvers(Promise promise, object resolve, object reject)
    {
        this.promise = promise;
        this.resolve = Node.AsyncContextRuntime.BindCurrentCallback(resolve);
        this.reject = Node.AsyncContextRuntime.BindCurrentCallback(reject);
    }

    // Note: These member names are intentionally lowercase to match JS property access
    // via ObjectRuntime.GetItem/GetProperty (which is case-sensitive for host properties).
    public Promise promise { get; }

    public object resolve { get; }

    public object reject { get; }
}
