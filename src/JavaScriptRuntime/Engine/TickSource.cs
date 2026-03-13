namespace JavaScriptRuntime.EngineCore;

public class TickSource : ITickSource
{
    public long GetTicks()
    {
        return DateTime.UtcNow.Ticks;
    }
}