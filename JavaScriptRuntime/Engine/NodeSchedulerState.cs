namespace JavaScriptRuntime.EngineCore;

internal struct TimerEntry : IEquatable<TimerEntry>
{
    /// <summary>
    /// We have an id because someone could schedule something multiple times for the same callback.
    /// For repeating timers, the id is stable across reschedules.
    /// </summary>
    public required long id;
    public required Action Callback;
    public required long DueTicks;

    public required bool IsRepeating;
    public required long IntervalTicks;

    public bool Equals(TimerEntry other) => id == other.id;

    public override bool Equals(object? obj) => obj is TimerEntry other && Equals(other);

    public override int GetHashCode() => id.GetHashCode();
}

internal struct ImmediateEntry : IEquatable<ImmediateEntry>
{
    public required long id;
    public required Action Callback;

    public bool Equals(ImmediateEntry other) => id == other.id;

    public override bool Equals(object? obj) => obj is ImmediateEntry other && Equals(other);

    public override int GetHashCode() => id.GetHashCode();
}

/// <summary>
/// Thread-safe scheduler state for the runtime.
///
/// This is the shared state mutated by:
/// - timers (setTimeout/setInterval)
/// - immediates (setImmediate)
/// - microtasks (Promise reactions)
///
/// The event loop/message pump is responsible for draining and executing work.
/// </summary>
public sealed class NodeSchedulerState : IScheduler, IMicrotaskScheduler
{
    private long _nextTimerId = 0;
    private long _nextImmediateId = 0;

    private readonly object _nextTickLock = new();
    private readonly Queue<Action> _nextTick = new();

    private readonly object _immediateLock = new();
    private readonly Queue<ImmediateEntry> _immediate = new();
    private readonly HashSet<long> _immediateIds = new();
    private readonly HashSet<long> _canceledImmediates = new();

    private readonly object _timerLock = new();
    private readonly HashSet<long> _canceledIntervals = new();
    private readonly PriorityQueue<TimerEntry, long> _timers = new();

    private readonly Queue<Action> _micro = new();

    private readonly ITickSource _tickSource;
    private readonly IWaitHandle _wakeup;

    public NodeSchedulerState(ITickSource tickSource, IWaitHandle waitHandle)
    {
        _tickSource = tickSource;
        _wakeup = waitHandle;
    }

    internal bool HasPendingWork()
    {
        lock (_micro)
        {
            if (_micro.Count > 0) return true;
        }

        lock (_nextTickLock)
        {
            if (_nextTick.Count > 0) return true;
        }

        lock (_immediateLock)
        {
            if (_immediate.Count > 0) return true;
        }

        lock (_timerLock)
        {
            return _timers.Count > 0;
        }
    }

    internal bool HasPendingWorkNow(long nowTicks)
    {
        lock (_micro)
        {
            if (_micro.Count > 0) return true;
        }

        lock (_nextTickLock)
        {
            if (_nextTick.Count > 0) return true;
        }

        lock (_immediateLock)
        {
            if (_immediate.Count > 0) return true;
        }

        lock (_timerLock)
        {
            RemoveCanceledIntervalsFromHead_NoWake();
            return _timers.TryPeek(out _, out long due) && due <= nowTicks;
        }
    }

    internal int GetWaitForWorkOrNextTimerMilliseconds(long nowTicks, int maxWaitMs = 50)
    {
        if (HasPendingWorkNow(nowTicks))
        {
            return 0;
        }

        int waitMs = maxWaitMs;
        lock (_timerLock)
        {
            RemoveCanceledIntervalsFromHead_NoWake();

            if (_timers.TryPeek(out _, out long due))
            {
                long deltaTicks = System.Math.Max(0, due - nowTicks);
                long deltaMs = (long)TimeSpan.FromTicks(deltaTicks).TotalMilliseconds;
                waitMs = (int)System.Math.Min(maxWaitMs, System.Math.Max(0, deltaMs));
            }
        }

        return waitMs;
    }

    internal int GetImmediateCountSnapshot(int max)
    {
        lock (_immediateLock)
        {
            return System.Math.Min(_immediate.Count, max);
        }
    }

    internal int GetNextTickCountSnapshot(int max)
    {
        lock (_nextTickLock)
        {
            return System.Math.Min(_nextTick.Count, max);
        }
    }

    internal bool TryDequeueNextTick(out Action? callback)
    {
        lock (_nextTickLock)
        {
            if (_nextTick.Count > 0)
            {
                callback = _nextTick.Dequeue();
                return true;
            }
        }

        callback = null;
        return false;
    }

    internal bool TryDequeueImmediate(out Action? callback)
    {
        lock (_immediateLock)
        {
            while (_immediate.Count > 0)
            {
                var entry = _immediate.Dequeue();
                _immediateIds.Remove(entry.id);
                if (_canceledImmediates.Remove(entry.id))
                {
                    continue;
                }

                callback = entry.Callback;
                return true;
            }
        }

        callback = null;
        return false;
    }

    internal bool TryDequeueMicrotask(out Action? callback)
    {
        lock (_micro)
        {
            if (_micro.Count > 0)
            {
                callback = _micro.Dequeue();
                return true;
            }
        }

        callback = null;
        return false;
    }

    internal bool TryDequeueDueTimer(long nowTicks, out TimerEntry entry)
    {
        lock (_timerLock)
        {
            RemoveCanceledIntervalsFromHead_NoWake();

            if (!_timers.TryPeek(out var peeked, out long due) || due > nowTicks)
            {
                entry = default;
                return false;
            }

            // Consume the due entry.
            entry = _timers.Dequeue();

            // If this interval was canceled, drop it and do not execute/reschedule.
            if (entry.IsRepeating && _canceledIntervals.Contains(entry.id))
            {
                _canceledIntervals.Remove(entry.id);
                entry = default;
                return false;
            }

            return true;
        }
    }

    internal void RescheduleIntervalFromNow(TimerEntry previous, long nowTicks)
    {
        if (!previous.IsRepeating || previous.IntervalTicks <= 0)
        {
            return;
        }

        var nextEntry = new TimerEntry
        {
            id = previous.id,
            Callback = previous.Callback,
            DueTicks = nowTicks + previous.IntervalTicks,
            IsRepeating = true,
            IntervalTicks = previous.IntervalTicks,
        };

        lock (_timerLock)
        {
            _timers.Enqueue(nextEntry, nextEntry.DueTicks);
        }

        _wakeup.Set();
    }

    private void RemoveCanceledIntervalsFromHead_NoWake()
    {
        while (_timers.TryPeek(out var peeked, out _) &&
               peeked.IsRepeating &&
               _canceledIntervals.Contains(peeked.id))
        {
            _timers.Dequeue();
            _canceledIntervals.Remove(peeked.id);
        }
    }

    object IScheduler.Schedule(Action action, TimeSpan delay)
    {
        var now = _tickSource.GetTicks();
        var id = System.Threading.Interlocked.Increment(ref _nextTimerId);

        var entry = new TimerEntry
        {
            id = id,
            Callback = action,
            DueTicks = now + delay.Ticks,
            IsRepeating = false,
            IntervalTicks = 0,
        };

        lock (_timerLock)
        {
            _timers.Enqueue(entry, entry.DueTicks);
        }

        _wakeup.Set();
        return entry;
    }

    void IScheduler.Cancel(object handle)
    {
        if (handle is TimerEntry entry)
        {
            lock (_timerLock)
            {
                _timers.Remove(entry, out _, out _);
            }
            _wakeup.Set();
        }
    }

    object IScheduler.ScheduleInterval(Action action, TimeSpan interval)
    {
        var now = _tickSource.GetTicks();
        var id = System.Threading.Interlocked.Increment(ref _nextTimerId);

        var ticks = interval.Ticks;
        if (ticks < 0) ticks = 0;

        var entry = new TimerEntry
        {
            id = id,
            Callback = action,
            DueTicks = now + ticks,
            IsRepeating = true,
            IntervalTicks = ticks,
        };

        lock (_timerLock)
        {
            _timers.Enqueue(entry, entry.DueTicks);
        }

        _wakeup.Set();
        return entry;
    }

    void IScheduler.CancelInterval(object handle)
    {
        if (handle is TimerEntry entry)
        {
            lock (_timerLock)
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

        _wakeup.Set();
    }

    internal void QueueNextTick(Action task)
    {
        lock (_nextTickLock)
        {
            _nextTick.Enqueue(task);
        }

        _wakeup.Set();
    }
}
