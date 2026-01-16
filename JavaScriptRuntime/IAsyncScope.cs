namespace JavaScriptRuntime;

public interface IAsyncScope
{
    int AsyncState { get; set; }

    PromiseWithResolvers? Deferred { get; set; }

    object? MoveNext { get; set; }
}
