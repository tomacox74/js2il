namespace JavaScriptRuntime;

public interface IAsyncScope
{
    int AsyncState { get; set; }

    PromiseWithResolvers? Deferred { get; set; }

    CompiledContinuation? MoveNext { get; set; }
}
