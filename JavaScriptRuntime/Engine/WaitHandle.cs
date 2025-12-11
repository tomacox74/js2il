namespace JavaScriptRuntime.EngineCore;

public class WaitHandle : IWaitHandle
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
}