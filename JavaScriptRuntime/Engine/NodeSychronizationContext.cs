namespace JavaScriptRuntime.EngineCore;

struct TimerEntry : IEquatable<TimerEntry>
{
    /// <summary>
    /// We have a id because someone could schedule something multiple times for the same callback
    /// </summary>
    public required long id;
    public required Action Callback;
    public required long DueTicks;

    public bool Equals(TimerEntry other)
    {
        return id == other.id;
    }

    public override bool Equals(object? obj)
    {
        return obj is TimerEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}

public class NodeSychronizationContext : SynchronizationContext, IScheduler
{
    private static long _nextTimerId = 0;
    
    private readonly Queue<SendOrPostCallbackItem> _macro = new();
    private readonly PriorityQueue<TimerEntry, long> _timers = new();
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
        long now = _tickSource.GetTicks();
        lock (_timers)
        {
            if (_timers.TryPeek(out var item, out long due) && due <= now)
            {
                _timers.Dequeue();
                lock (_macro) _macro.Enqueue(new(_ => item.Callback(), null));
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

    object IScheduler.Schedule(Action action, TimeSpan delay)
    {
        var now = this._tickSource.GetTicks();

        var id = System.Threading.Interlocked.Increment(ref _nextTimerId);

        // struct so this is allocated on the stact
        var entry = new TimerEntry { id = id, Callback = action, DueTicks =  now + delay.Ticks };

        this._timers.Enqueue(entry, entry.DueTicks);

        return entry;
    }

    void IScheduler.Cancel(object handle)
    {
        // javascript apis ignore if not expected type
        if (handle is TimerEntry entry)
        {
            this._timers.Remove(entry, out var _, out var _);
        }
    }

    private readonly record struct SendOrPostCallbackItem(SendOrPostCallback D, object? S);
}

