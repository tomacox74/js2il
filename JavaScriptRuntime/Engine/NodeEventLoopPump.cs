namespace JavaScriptRuntime.EngineCore;

/// <summary>
/// Non-thread-safe message pump that drains a <see cref="NodeSchedulerState"/>.
///
/// This is intended to be owned by the single JS/runtime thread.
/// </summary>
public sealed class NodeEventLoopPump
{
    private readonly NodeSchedulerState _state;
    private readonly ITickSource _tickSource;
    private readonly IWaitHandle _wakeup;

    private readonly Queue<Action> _macro = new();

    private readonly int _ownerThreadId;

    public NodeEventLoopPump(NodeSchedulerState state, ITickSource tickSource, IWaitHandle waitHandle)
    {
        _state = state;
        _tickSource = tickSource;
        _wakeup = waitHandle;
        _ownerThreadId = Environment.CurrentManagedThreadId;
    }

    private void ThrowIfNotOwnerThread()
    {
        if (Environment.CurrentManagedThreadId != _ownerThreadId)
        {
            throw new InvalidOperationException(
                "This method may only be called from the JS runtime thread that owns this NodeEventLoopPump.");
        }
    }

    public bool HasPendingWork()
    {
        ThrowIfNotOwnerThread();
        return _macro.Count > 0 || _state.HasPendingWork();
    }

    public bool HasPendingWorkNow()
    {
        ThrowIfNotOwnerThread();
        if (_macro.Count > 0)
        {
            return true;
        }

        var now = _tickSource.GetTicks();
        return _state.HasPendingWorkNow(now);
    }

    public int GetWaitForWorkOrNextTimerMilliseconds(int maxWaitMs = 50)
    {
        ThrowIfNotOwnerThread();
        if (_macro.Count > 0)
        {
            return 0;
        }

        var now = _tickSource.GetTicks();
        return _state.GetWaitForWorkOrNextTimerMilliseconds(now, maxWaitMs);
    }

    /// <summary>
    /// Executes one scheduler turn using Node-compatible checkpoint ordering.
    ///
    /// Compatibility intent:
    /// - process.nextTick is drained before Promise microtasks at each checkpoint.
    /// - A pre-phase microtask checkpoint runs before immediates/timers so already-pending jobs
    ///   are not delayed behind macrotask phases.
    /// - Immediates run before timer promotion for this turn, then one due timer callback is
    ///   promoted to macro work.
    /// - A post-callback and end-of-turn checkpoint drains nextTick/microtasks created during
    ///   this iteration.
    ///
    /// This ordering is designed for practical Node event loop compatibility for the supported
    /// scheduling primitives in this runtime (nextTick, Promise jobs, setImmediate, setTimeout).
    /// It is intentionally bounded in each drain loop to preserve forward progress.
    /// </summary>
    public void RunOneIteration()
    {
        ThrowIfNotOwnerThread();

        DrainNextTicks();
        DrainMicrotasks();
        DrainNextTicks();
        DrainImmediatesOneTick();
        PromoteOneDueTimerToMacro();

        if (_macro.Count > 0)
        {
            _macro.Dequeue().Invoke();
            DrainNextTicks();
        }

        DrainNextTicks();
        DrainMicrotasks();
    }

    public void WaitForWorkOrNextTimer(int maxWaitMs = 50)
    {
        ThrowIfNotOwnerThread();

        int waitMs = GetWaitForWorkOrNextTimerMilliseconds(maxWaitMs);
        if (waitMs == 0)
        {
            return;
        }

        _wakeup.WaitOne(waitMs);
    }

    private void DrainImmediatesOneTick(int max = 1024)
    {
        int count = _state.GetImmediateCountSnapshot(max);
        for (int i = 0; i < count; i++)
        {
            if (!_state.TryDequeueImmediate(out var callback) || callback == null)
            {
                return;
            }

            callback.Invoke();

            // Node-compatible ordering: process.nextTick callbacks run before Promise jobs.
            DrainNextTicks();

            // Promise reactions are modeled as microtasks. Run a microtask checkpoint after each callback.
            DrainMicrotasks();
        }
    }

    private void DrainNextTicks(int max = 1024)
    {
        int count = _state.GetNextTickCountSnapshot(max);
        for (int i = 0; i < count; i++)
        {
            if (!_state.TryDequeueNextTick(out var callback) || callback == null)
            {
                return;
            }

            callback.Invoke();
        }
    }

    private void PromoteOneDueTimerToMacro()
    {
        var now = _tickSource.GetTicks();

        if (_state.TryDequeueDueTimer(now, out var entry))
        {
            _macro.Enqueue(entry.Callback);
            _state.RescheduleIntervalFromNow(entry, now);
        }
    }

    private void DrainMicrotasks(int max = 1024)
    {
        // We intentionally bound the number of microtasks drained in one checkpoint.
        // This preserves forward progress for timers/macrotasks and avoids starvation.
        int ticks = 0;
        while (ticks++ < max)
        {
            // Maintain Node-like priority for process.nextTick, including those queued from microtasks.
            DrainNextTicks();

            if (!_state.TryDequeueMicrotask(out var action) || action == null)
            {
                break;
            }

            action.Invoke();

            DrainNextTicks();
        }
    }
}
