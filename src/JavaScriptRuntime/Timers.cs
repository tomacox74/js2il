using JavaScriptRuntime;

internal class Timers
{
    private JavaScriptRuntime.EngineCore.IScheduler  _scheduler;

    public Timers(JavaScriptRuntime.EngineCore.IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public object setTimeout(object callback, object delay, params object[] args)
    {
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("First argument to setTimeout must be a function");
        }

        var delayMs = TypeUtilities.ToNumber(delay);
        if (delayMs < 0 || double.IsNaN(delayMs))
        {
            delayMs = 0;
        }

        var timeout = new Timeout(
            scheduler: _scheduler,
            delay: TimeSpan.FromMilliseconds(delayMs),
            callback: () =>
            {
                CallableOperations.Call(callback, null, args);
            });

        return timeout.refresh();
    }

    public object? clearTimeout(object handle)
    {
        if (handle is Timeout timeout)
        {
            timeout.Cancel();
        }
        else if (handle != null)
        {
            _scheduler.Cancel(handle);
        }
        return null;
    }

    public object setInterval(object callback, object delay, params object[] args)
    {
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("First argument to setInterval must be a function");
        }

        var delayMs = TypeUtilities.ToNumber(delay);
        if (delayMs < 0 || double.IsNaN(delayMs))
        {
            delayMs = 0;
        }

        var handle = _scheduler.ScheduleInterval(() =>
        {
            CallableOperations.Call(callback, null, args);
        }, TimeSpan.FromMilliseconds(delayMs));

        return handle;
    }

    public object? clearInterval(object handle)
    {
        if (handle != null)
        {
            _scheduler.CancelInterval(handle);
        }
        return null;
    }

    public object setImmediate(object callback, params object[] args)
    {
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError("First argument to setImmediate must be a function");
        }

        var handle = _scheduler.ScheduleImmediate(() =>
        {
            CallableOperations.Call(callback, null, args);
        });

        return handle;
    }

    public object? clearImmediate(object handle)
    {
        if (handle != null)
        {
            _scheduler.CancelImmediate(handle);
        }
        return null;
    }
}

/// <summary>
/// Node-compatible one-shot timer handle.
/// </summary>
public sealed class Timeout
{
    private readonly JavaScriptRuntime.EngineCore.IScheduler _scheduler;
    private readonly TimeSpan _delay;
    private readonly Action _callback;
    private object? _schedulerHandle;
    private bool _canceled;

    internal Timeout(
        JavaScriptRuntime.EngineCore.IScheduler scheduler,
        TimeSpan delay,
        Action callback)
    {
        _scheduler = scheduler;
        _delay = delay;
        _callback = callback;
    }

    /// <summary>
    /// Reschedules this timeout using its original delay and returns the same handle.
    /// </summary>
    public Timeout refresh()
    {
        if (_canceled)
        {
            return this;
        }

        CancelScheduledHandle();
        _schedulerHandle = _scheduler.Schedule(Invoke, _delay);
        return this;
    }

    internal void Cancel()
    {
        _canceled = true;
        CancelScheduledHandle();
    }

    private void Invoke()
    {
        _schedulerHandle = null;
        if (_canceled)
        {
            return;
        }

        _callback();
    }

    private void CancelScheduledHandle()
    {
        if (_schedulerHandle == null)
        {
            return;
        }

        _scheduler.Cancel(_schedulerHandle);
        _schedulerHandle = null;
    }
}