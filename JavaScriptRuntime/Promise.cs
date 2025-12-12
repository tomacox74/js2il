using System;
namespace JavaScriptRuntime;

[IntrinsicObject("Promise")]
public sealed class Promise
{
    private enum State { Pending, Fulfilled, Rejected };

    private State _state = State.Pending;

    private object? _result;

    private readonly List<Reaction> _reactions = new();

    private struct Reaction
    {
        public readonly object? OnFulfilled;
        public readonly object? OnRejected;
        public readonly Promise NextPromise;

        public Reaction(object? onFulfilled, object? onRejected, Promise nextPromise)
        {
            OnFulfilled = onFulfilled;
            OnRejected = onRejected;
            NextPromise = nextPromise;
        }
    }

    public Promise(object? executor)
    {
        // as per the specification the delegate is called the executor
        // null is allowed
        // any value that is not a delegate will result in a TypeError being thrown
        InvokeExecutor(executor);
    }

    /// <summary>
    /// Private custructor for internal use (i.e. promise chaining)
    /// </summary>
    internal Promise()
    {
    }

    private void InvokeExecutor(object? executor)
    {
        if (executor is not Delegate jsFunction)
        {
            throw new JavaScriptRuntime.TypeError("Promise resolver is not a function");
        }

        // the first paramter is ignored.. only exists for consistency
        var unusedContext = System.Array.Empty<object>();
        var Resolve = new Func<object[]?, object?, object?>((_, value) =>
        {
            return Settle(State.Fulfilled, value);
        });

        var Reject = new Func<object[]?, object?, object?>((_, reason) =>
        {
            return Settle(State.Rejected, reason);
        });


        try 
        {
            // strongly typed fast path for the common cases
            if (executor is Func<object[]?, object?> func2)
            {
                func2(unusedContext);
            }
            else if (executor is Func<object[]?, object?, object?> func3)
            {
                func3(unusedContext, Resolve);
            }
            else if (executor is Func<object[]?, object?, object?, object?> func4)
            {
                func4(unusedContext, Resolve, Reject);
            }
            else
            {
                if (jsFunction.Method.ReturnType != typeof(object))
                {
                    // this would be unexpected.. and is a internal error
                    // all javascript functions return something of type object.. can be null
                    // technically we don't care.. this is to help catch internal bugs
                    throw new InvalidOperationException("Promise executor has an invalid signature. Unexpected return value.");
                }

                // get the number of parameters
                var paramCount = jsFunction.Method.GetParameters().Length;

                if (paramCount == 0)
                {
                    throw new InvalidOperationException("Promise executor has an invalid signature. Missing scopes parameter.");
                }

                var args = new object?[paramCount];
                args[0] = unusedContext;
                args[1] = Resolve;
                args[2] = Reject;
                jsFunction.DynamicInvoke(args);
            }
        } 
        catch (Exception ex)
        {
            Settle(State.Rejected, ex.InnerException ?? ex);
        }
    }

    public static object? resolve(object? value)
    {
        var promise = new Promise();
        promise.Settle(State.Fulfilled, value);
        return promise;
    }

    public static object? reject(object? reason)
    {
        var promise = new Promise();
        promise.Settle(State.Rejected, reason);
        return promise;
    }

    public object? @then(object? onFulfilled = null, object? onRejected = null)
    {
        var nextPromise = new Promise();
        var reaction = new Reaction(onFulfilled, onRejected, nextPromise);

        bool shouldEnqueue = false;

        lock (_reactions)
        {
            if (_state == State.Pending)
            {
                _reactions.Add(reaction);
            }
            else
            {
                shouldEnqueue = true;
            }
        }

        if (shouldEnqueue)
        {
            EnqueueReaction(reaction);
        }

        return nextPromise;
    }

    public object? @catch(object? onRejected)
    {
        return then(null, onRejected);
    }

    private object? Settle(State state, object? value)
    {
        List<Reaction> toSchedule;

        if (_state != State.Pending)
        {
            return null;
        }

        _state = state;
        _result = value;
        toSchedule = new List<Reaction>(_reactions);
        _reactions.Clear();

        foreach (var r in toSchedule)
        {
            EnqueueReaction(r);
        }

        return null;
    }

    private void EnqueueReaction(Reaction reaction)
    {
        JavaScriptRuntime.EngineCore.IMicrotaskScheduler? scheduler = GlobalThis.MicrotaskScheduler;
        if (scheduler == null)
        {
            throw new InvalidOperationException("No microtask scheduler available");
        }

        scheduler.QueueMicrotask(() =>
        {
            ProcessReaction(reaction);
        });
    }

    private void ProcessReaction(Reaction reaction)
    {
        try
        {
            object? handlerResult;
            if (_state == State.Fulfilled)
            {
                if (reaction.OnFulfilled != null)
                {
                    handlerResult = ExecuteHandler(reaction.OnFulfilled, _result);
                }
                else
                {
                    handlerResult = _result;
                }
            }
            else // Rejected
            {
                if (reaction.OnRejected != null)
                {
                    handlerResult = ExecuteHandler(reaction.OnRejected, _result);
                }
                else
                {
                    throw new Exception("Unhandled promise rejection", _result as Exception);
                }
            }

            reaction.NextPromise.Settle(State.Fulfilled, handlerResult);
        }
        catch (Exception ex)
        {
            reaction.NextPromise.Settle(State.Rejected, ex);
        }
    }

    /// <Summary>
    /// Execute the handler provided for "then", "catch", "finally"
    /// </Summary>
    /// <Remarks>
    /// Any value that is NOT a function is ignored.  Treated as a noop basically.
    /// see 27.2.5.4.1 in teh ECMA 262
    /// 
    /// </Remarks>
    private static object? ExecuteHandler(object? handler, object? value)
    {
        // for then, catch, finally. etc
        if (handler is not Delegate jsFunction)
        {
            return value;
        }

        // scopes are  always captured in closures (see Closure.Bind)
        // so the first parameter is always null
        if (jsFunction is Func<object[]?, object?> func1)
        {
            return func1(null);
        }
        else if (jsFunction is Func<object[]?, object?, object?> func2)
        {
            return func2(null, value);
        }

        // get the number of parameters
        var paramCount = jsFunction.Method.GetParameters().Length;

        var args = new object?[paramCount];
        args[1] = value;
        return jsFunction.DynamicInvoke(args);

    }
}