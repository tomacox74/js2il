namespace JavaScriptRuntime.EngineCore;

public interface IWaitHandle
{
    void Set();
    void WaitOne(int millisecondsTimeout);
}

