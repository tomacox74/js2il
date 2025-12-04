namespace JavaScriptRuntime;

public interface ITickSource
{
    long GetTicks();
}

public class TickSource : ITickSource
{
    public long GetTicks()
    {
        return DateTime.UtcNow.Ticks;
    }
}

public interface IWaitHandle
{
    void Set();
    void WaitOne(int millisecondsTimeout);
}

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

public class NodeSychronizationContext : SynchronizationContext
{
    private readonly Queue<SendOrPostCallbackItem> _macro = new();
    private readonly PriorityQueue<(Action cb, long dueTicks), long> _timers = new();
    private readonly ITickSource _tickSource;

    // Wake-up signal for the loop (sleep until new work arrives or next timer is due)
    private readonly IWaitHandle _wakeup;

    public NodeSychronizationContext(ITickSource tickSource, IWaitHandle waitHandle)
    {
        _tickSource = tickSource;
        _wakeup = waitHandle;
    }

    static bool TryDequeue(Queue<SendOrPostCallbackItem> q, out SendOrPostCallbackItem item)
    {
        lock (q)
        {
            if (q.Count > 0) { item = q.Dequeue(); return true; }
        }
        item = default;
        return false;
    }

    private void TryPromoteDueTimerToMacro()
    {
        long now = DateTime.UtcNow.Ticks;
        lock (_timers)
        {
            if (_timers.TryPeek(out var item, out long due) && due <= now)
            {
                _timers.Dequeue();
                lock (_macro) _macro.Enqueue(new(_ => item.cb(), null));
            }
        }
    }

    public bool HasPendingWork()
    {
        return _macro.Count > 0 || _timers.Count > 0;
    }

    public void RunOneIteration()
    {
        // Fire any expired timers as macrotasks (only 1 per tick for simplicity)
        TryPromoteDueTimerToMacro();

        if (TryDequeue(_macro, out var m)) 
        {   
            m.D(m.S);
        }

        // TODO
        //int ticks = 0;
        //while (ticks++ < 1024 && TryDequeue(_nextTick, out var nt)) nt.D(nt.S);

        //while (TryDequeue(_micro, out var mi)) mi.D(mi.S);
    }

    public void WaitForWorkOrNextTimer(int maxWaitMs = 50)
    {
        int waitMs = maxWaitMs;
        lock (_timers)
        {
            if (_timers.TryPeek(out var _, out long due))
            {
                // We have to explicity use System.Math because there is also a JavascriptRuntime.Math type
                long deltaTicks = System.Math.Max(0, due - this._tickSource.GetTicks());
                long deltaMs = (long)TimeSpan.FromTicks(deltaTicks).TotalMilliseconds;
                waitMs = (int)System.Math.Min(maxWaitMs, System.Math.Max(0, deltaMs));
            }
        }
        _wakeup.WaitOne(waitMs);
    }

    private readonly record struct SendOrPostCallbackItem(SendOrPostCallback D, object? S);
}

/// <summary>
/// Configuration options for the JavaScript engine.
/// Allows mocks to be sustituted for testing.
/// </summary>
public sealed class EngineOptions
{
    public required ITickSource TickSource { get; set; }
    public required IWaitHandle WaitHandle { get; set; }

    public static Func<EngineOptions>? DefaultOptionsProvider { get; set; } = GetDefaultOptions;

    public static EngineOptions GetDefaultOptions()
    {
        return new EngineOptions
        {
            TickSource = new TickSource(),
            WaitHandle = new WaitHandle(),
        };
    }
}

/// <summary>
/// Entry point for executiing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    public void Execute(Action scriptEntryPoint)
    {
        var engineOptions = EngineOptions.DefaultOptionsProvider?.Invoke();
        if (engineOptions == null)
        {
            throw new InvalidOperationException("No default engine options has been configured.");
        }

        var ctx = new NodeSychronizationContext(engineOptions.TickSource, engineOptions.WaitHandle);
        SynchronizationContext.SetSynchronizationContext(ctx);

        scriptEntryPoint();

        while (ctx.HasPendingWork())
        {
            ctx.RunOneIteration();
            ctx.WaitForWorkOrNextTimer();
        }
    }
}