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
            // Get the parameter count from the delegate's method
            var paramCount = del.Method.GetParameters().Length;
            
            // Build the arguments array
            // First parameter is always the scopes array (which bound closures ignore)
            // Remaining parameters are the extra args passed to setTimeout
            var invokeArgs = new object?[paramCount];
            invokeArgs[0] = null; // scopes placeholder (ignored by bound closures)
            
            // Fill in extra arguments (name, age, city, etc.)
            for (int i = 0; i < args.Length && i + 1 < paramCount; i++)
            {
                invokeArgs[i + 1] = args[i];
            }
            
            del.DynamicInvoke(invokeArgs);
        }, TimeSpan.FromMilliseconds(delayMs));

        return handle;
    }

    public object clearTimeout(object handle)
    {
        if (handle != null)
        {
            _scheduler.Cancel(handle);
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

            var invokeArgs = new object?[paramCount];
            invokeArgs[0] = null; // scopes placeholder (ignored by bound closures)

            for (int i = 0; i < args.Length && i + 1 < paramCount; i++)
            {
                invokeArgs[i + 1] = args[i];
            }

            del.DynamicInvoke(invokeArgs);
        });

        return handle;
    }

    public object clearImmediate(object handle)
    {
        if (handle != null)
        {
            _scheduler.CancelImmediate(handle);
        }
        return null;
    }
}