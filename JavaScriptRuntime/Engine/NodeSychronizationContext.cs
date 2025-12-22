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

struct ImmediateEntry : IEquatable<ImmediateEntry>
{
    public required long id;
    public required Action Callback;

    public bool Equals(ImmediateEntry other)
    {
        return id == other.id;
    }

    public override bool Equals(object? obj)
    {
        return obj is ImmediateEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}

public class NodeSychronizationContext : SynchronizationContext, IScheduler, IMicrotaskScheduler
{
    private static long _nextTimerId = 0;
    private static long _nextImmediateId = 0;
    
    private readonly Queue<ImmediateEntry> _immediate = new();
    private readonly HashSet<long> _canceledImmediates = new();
    private readonly Queue<Action> _macro = new();
    private readonly PriorityQueue<TimerEntry, long> _timers = new();
    private readonly Queue<Action> _micro = new();
    private readonly ITickSource _tickSource;

    // Wake-up signal for the loop (sleep until new work arrives or next timer is due)
    private readonly IWaitHandle _wakeup;

    public NodeSychronizationContext(ITickSource tickSource, IWaitHandle waitHandle)
    {
        _tickSource = tickSource;
        _wakeup = waitHandle;
    }

    static bool TryDequeue(Queue<Action> q, out Action? item)
    {
        lock (q)
        {
            if (q.Count > 0) { item = q.Dequeue(); return true; }
        }
        item = default;
        return false;
    }

    static bool TryDequeue(Queue<ImmediateEntry> q, out ImmediateEntry item)
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
                lock (_macro) _macro.Enqueue(item.Callback);
            }
        }
    }

    public bool HasPendingWork()
    {
        lock (_micro)
        {
            if (_micro.Count > 0) return true;
        }

        lock (_immediate)
        {
            if (_immediate.Count > 0) return true;
        }

        lock (_macro)
        {
            if (_macro.Count > 0) return true;
        }

        lock (_timers)
        {
            return _timers.Count > 0;
        }
    }

    private void DrainImmediatesOneTick(int max = 1024)
    {
        // Snapshot count to ensure newly-queued immediates run on the next iteration.
        int count;
        lock (_immediate)
        {
            count = System.Math.Min(_immediate.Count, max);
        }

        for (int i = 0; i < count; i++)
        {
            if (!TryDequeue(_immediate, out var entry))
            {
                return;
            }

            bool canceled;
            lock (_canceledImmediates)
            {
                canceled = _canceledImmediates.Remove(entry.id);
            }

            if (canceled)
            {
                continue;
            }

            entry.Callback?.Invoke();
        }
    }

    public void RunOneIteration()
    {
        // Execute immediates first (higher priority than timers/macrotasks)
        DrainImmediatesOneTick();

        // Fire any expired timers as macrotasks (only 1 per tick for simplicity)
        TryPromoteDueTimerToMacro();

        if (TryDequeue(_macro, out var action)) 
        {   
            action?.Invoke();
        }

        DrainMicrotasks();
    }

    public void WaitForWorkOrNextTimer(int maxWaitMs = 50)
    {
        if (_micro.Count() > 0) return;
        if (_immediate.Count() > 0) return;
        if (_macro.Count() > 0) return;

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

    object IScheduler.ScheduleImmediate(Action action)
    {
        var id = System.Threading.Interlocked.Increment(ref _nextImmediateId);
        var entry = new ImmediateEntry { id = id, Callback = action };
        lock (_immediate)
        {
            _immediate.Enqueue(entry);
        }
        _wakeup.Set();
        return entry;
    }

    void IScheduler.CancelImmediate(object handle)
    {
        if (handle is ImmediateEntry entry)
        {
            lock (_canceledImmediates)
            {
                _canceledImmediates.Add(entry.id);
            }
            _wakeup.Set();
        }
    }

    void IMicrotaskScheduler.QueueMicrotask(Action task)
    {
        lock (_micro)
        {
            _micro.Enqueue(task);
        }
    }
    
    /// <summary>
    /// Executes microtasks
    /// </summary>
    /// <param name="max">Maximum number of microtasks to execute in one call</param>
    /// <remarks>
    /// We limit how many we execute in a loop to ensure we don't starve execution of macro tasks and timers
    /// </remarks>
    private void DrainMicrotasks(int max = 1024)
    {
            int ticks = 0;
            while (ticks++ < max && TryDequeue(_micro, out var action))
            {
                action?.Invoke();
            }
    }
}

