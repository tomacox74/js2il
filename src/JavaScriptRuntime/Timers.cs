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
        if (callback is not Delegate del)
        {
            throw new TypeError("First argument to setTimeout must be a function");
        }

        var delayMs = TypeUtilities.ToNumber(delay);
        if (delayMs < 0 || double.IsNaN(delayMs))
        {
            delayMs = 0;
        }

        var timeout = new Timeout(
            schedule: action => _scheduler.Schedule(action, TimeSpan.FromMilliseconds(delayMs)),
            cancel: _scheduler.Cancel,
            callback: () =>
            {
                // Parameter count includes the leading scopes array parameter.
                var paramCount = del.Method.GetParameters().Length;
                var expectedArgCount = System.Math.Max(0, paramCount - 1);

                // JS semantics: missing args -> undefined (null); extra args ignored.
                var invokeArgs = new object[expectedArgCount];
                for (int i = 0; i < expectedArgCount; i++)
                {
                    invokeArgs[i] = i < args.Length ? args[i] : null!;
                }

                // Provide a non-null scopes array; bound closures ignore it.
                Closure.InvokeWithArgs(del, System.Array.Empty<object>(), invokeArgs);
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
        if (callback is not Delegate del)
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
            var paramCount = del.Method.GetParameters().Length;
            var expectedArgCount = System.Math.Max(0, paramCount - 1);

            var invokeArgs = new object[expectedArgCount];
            for (int i = 0; i < expectedArgCount; i++)
            {
                invokeArgs[i] = i < args.Length ? args[i] : null!;
            }

            Closure.InvokeWithArgs(del, System.Array.Empty<object>(), invokeArgs);
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
        if (callback is not Delegate del)
        {
            throw new TypeError("First argument to setImmediate must be a function");
        }

        var handle = _scheduler.ScheduleImmediate(() =>
        {
            var paramCount = del.Method.GetParameters().Length;
            var expectedArgCount = System.Math.Max(0, paramCount - 1);

            var invokeArgs = new object[expectedArgCount];
            for (int i = 0; i < expectedArgCount; i++)
            {
                invokeArgs[i] = i < args.Length ? args[i] : null!;
            }

            Closure.InvokeWithArgs(del, System.Array.Empty<object>(), invokeArgs);
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
    private readonly object _syncRoot = new();
    private readonly Func<Action, object> _schedule;
    private readonly Action<object> _cancel;
    private readonly Action _callback;
    private object? _schedulerHandle;
    private long _generation;
    private bool _active;

    internal Timeout(Func<Action, object> schedule, Action<object> cancel, Action callback)
    {
        _schedule = schedule;
        _cancel = cancel;
        _callback = callback;
    }

    /// <summary>
    /// Reschedules this timeout using its original delay and returns the same handle.
    /// </summary>
    public Timeout refresh()
    {
        lock (_syncRoot)
        {
            CancelScheduledHandle();
            _active = true;
            var generation = ++_generation;
            _schedulerHandle = _schedule(() => Invoke(generation));
        }

        return this;
    }

    internal void Cancel()
    {
        lock (_syncRoot)
        {
            _active = false;
            ++_generation;
            CancelScheduledHandle();
        }
    }

    private void Invoke(long generation)
    {
        lock (_syncRoot)
        {
            if (!_active || generation != _generation)
            {
                return;
            }

            _active = false;
            _schedulerHandle = null;
        }

        _callback();
    }

    private void CancelScheduledHandle()
    {
        if (_schedulerHandle == null)
        {
            return;
        }

        _cancel(_schedulerHandle);
        _schedulerHandle = null;
    }
}