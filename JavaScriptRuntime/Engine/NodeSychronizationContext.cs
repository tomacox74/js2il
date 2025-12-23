namespace JavaScriptRuntime.EngineCore;

struct TimerEntry : IEquatable<TimerEntry>
{
    /// <summary>
    /// We have a id because someone could schedule something multiple times for the same callback
    /// </summary>
    public required long id;
    public required Action Callback;
    public required long DueTicks;

    public required bool IsRepeating;
    public required long IntervalTicks;

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
    private long _nextImmediateId = 0;
    
    private readonly object _immediateLock = new();
    private readonly Queue<ImmediateEntry> _immediate = new();
    private readonly HashSet<long> _immediateIds = new();
    private readonly HashSet<long> _canceledImmediates = new();
    private readonly HashSet<long> _canceledIntervals = new();
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

    private bool TryDequeueImmediate(out ImmediateEntry item, out bool canceled)
    {
        lock (_immediateLock)
        {
            if (_immediate.Count == 0)
            {
                item = default;
                canceled = false;
                return false;
            }

            item = _immediate.Dequeue();
            _immediateIds.Remove(item.id);
            canceled = _canceledImmediates.Remove(item.id);
            return true;
        }
    }

    private void TryPromoteDueTimerToMacro()
    {
        long now = _tickSource.GetTicks();
        lock (_timers)
        {
            // First, remove any canceled intervals from the front of the queue
            // (they may not be due yet, but we don't want to wait for them)
            while (_timers.TryPeek(out var peeked, out _) && 
                   peeked.IsRepeating && 
                   _canceledIntervals.Contains(peeked.id))
            {
                _timers.Dequeue();
                _canceledIntervals.Remove(peeked.id);
            }

            if (_timers.TryPeek(out var item, out long due) && due <= now)
            {
                _timers.Dequeue();

                // Check if this interval was canceled (might have been canceled while we were processing)
                if (item.IsRepeating && _canceledIntervals.Contains(item.id))
                {
                    // Don't execute and don't reschedule; clean up the cancel tracking
                    _canceledIntervals.Remove(item.id);
                    return;
                }

                lock (_macro) _macro.Enqueue(item.Callback);

                // Reschedule repeating timers from "now" (Node-like drift behavior)
                if (item.IsRepeating && item.IntervalTicks > 0)
                {
                    var nextEntry = new TimerEntry
                    {
                        id = item.id,
                        Callback = item.Callback,
                        DueTicks = now + item.IntervalTicks,
                        IsRepeating = true,
                        IntervalTicks = item.IntervalTicks
                    };
                    _timers.Enqueue(nextEntry, nextEntry.DueTicks);
                }
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
        lock (_immediateLock)
        {
            count = System.Math.Min(_immediate.Count, max);
        }

        for (int i = 0; i < count; i++)
        {
            if (!TryDequeueImmediate(out var entry, out var canceled))
            {
                return;
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
        lock (_micro)
        {
            if (_micro.Count > 0) return;
        }

        lock (_immediateLock)
        {
            if (_immediate.Count > 0) return;
        }

        lock (_macro)
        {
            if (_macro.Count > 0) return;
        }

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
        var entry = new TimerEntry
        {
            id = id,
            Callback = action,
            DueTicks = now + delay.Ticks,
            IsRepeating = false,
            IntervalTicks = 0
        };

        this._timers.Enqueue(entry, entry.DueTicks);

        _wakeup.Set();

        return entry;
    }

    void IScheduler.Cancel(object handle)
    {
        // javascript apis ignore if not expected type
        if (handle is TimerEntry entry)
        {
            this._timers.Remove(entry, out var _, out var _);
            _wakeup.Set();
        }
    }

    object IScheduler.ScheduleInterval(Action action, TimeSpan interval)
    {
        var now = this._tickSource.GetTicks();
        var id = System.Threading.Interlocked.Increment(ref _nextTimerId);
        var ticks = interval.Ticks;
        if (ticks < 0) ticks = 0;

        var entry = new TimerEntry
        {
            id = id,
            Callback = action,
            DueTicks = now + ticks,
            IsRepeating = true,
            IntervalTicks = ticks
        };

        this._timers.Enqueue(entry, entry.DueTicks);
        _wakeup.Set();
        return entry;
    }

    void IScheduler.CancelInterval(object handle)
    {
        // Mark the interval as canceled; rescheduling will check this
        if (handle is TimerEntry entry)
        {
            lock (_timers)
            {
                _canceledIntervals.Add(entry.id);
            }
            _wakeup.Set();
        }
    }

    object IScheduler.ScheduleImmediate(Action action)
    {
        var id = System.Threading.Interlocked.Increment(ref _nextImmediateId);
        var entry = new ImmediateEntry { id = id, Callback = action };
        lock (_immediateLock)
        {
            _immediate.Enqueue(entry);
            _immediateIds.Add(entry.id);
        }
        _wakeup.Set();
        return entry;
    }

    void IScheduler.CancelImmediate(object handle)
    {
        if (handle is ImmediateEntry entry)
        {
            lock (_immediateLock)
            {
                if (!_immediateIds.Contains(entry.id))
                {
                    return;
                }
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

