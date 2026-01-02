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

        var handle = _scheduler.Schedule(() =>
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
        }, TimeSpan.FromMilliseconds(delayMs));

        return handle;
    }

    public object? clearTimeout(object handle)
    {
        if (handle != null)
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