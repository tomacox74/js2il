namespace JavaScriptRuntime.EngineCore;

interface IIOScheduler
{
    void BeginIo();
    void EndIo(global::JavaScriptRuntime.PromiseWithResolvers promiseWithResolvers, object? result, bool isError = false);
}
