namespace JavaScriptRuntime.EngineCore;

public class WaitHandle : IWaitHandle, IDisposable
{
    private readonly AutoResetEvent _event = new(false);

    public void Set()
    {
        _event.Set();
    }

    public void WaitOne(int millisecondsTimeout)
    {
        _event.WaitOne(millisecondsTimeout);
    }

    public void Dispose()
    {
        _event.Dispose();
    }
}