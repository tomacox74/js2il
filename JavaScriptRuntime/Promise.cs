using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace JavaScriptRuntime;

[IntrinsicObject("Promise")]
public sealed class Promise
{
    // Nested types
    private enum State { Pending, Fulfilled, Rejected }

    private struct Reaction
    {
        public readonly object? OnFulfilled;
        public readonly object? OnRejected;
        public readonly Promise NextPromise;

        /// <Summary>
        /// True if this reaction is for a finally handler,
        /// i.e., for Promise.prototype.finally.
        /// </Summary>
        /// <Remarks>
        /// Final handlers have the following characteristics:
        /// * The handler does not accept any parameters
        /// * incoming promise state is copied to the newly returned promise
        /// </Remarks>
        public readonly bool IsFinally;

        public Reaction(object? onFulfilled, object? onRejected, Promise nextPromise, bool isFinally)
        {
            OnFulfilled = onFulfilled;
            OnRejected = onRejected;
            NextPromise = nextPromise;
            IsFinally = isFinally;
        }
    }

    // Fields
    private State _state = State.Pending;

    private object? _result;

    private readonly List<Reaction> _reactions = new();

    // Constructors
    public Promise(object? executor)
    {
        // as per the specification the delegate is called the executor
        // null is allowed
        // any value that is not a delegate will result in a TypeError being thrown
        InvokeExecutor(executor);
    }

    /// <summary>
    /// Private constructor for internal use (i.e. promise chaining)
    /// </summary>
    internal Promise()
    {
    }

    // Public methods
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
        var reaction = new Reaction(onFulfilled, onRejected, nextPromise, false);

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

    public object? @finally(object? onFinally)
    {
        var nextPromise = new Promise();
        var reaction = new Reaction(onFinally, onFinally, nextPromise, true);

        bool shouldDispatch = false;

        lock (_reactions)
        {
            if (_state == State.Pending)
            {
                _reactions.Add(reaction);
            }
            else
            {
                shouldDispatch = true;
            }
        }

        if (shouldDispatch)
        {
            EnqueueReaction(reaction);
        }

        return nextPromise;
    }

    public static object? all(object? iterable)
    {

        JavaScriptRuntime.Array? results = null;
        int resolvedCount = 0;
        Promise? allPromise = null;

        Promise InitializeState()
        {
            results = new JavaScriptRuntime.Array();
            allPromise = new Promise();
            return allPromise;
        }

        void CheckForAllCompleted()
        {
            if (resolvedCount == results!.Count)
            {
                allPromise!.Settle(State.Fulfilled, results);
            }
        }

        AddPromiseResult AddPromise(Promise p)
        {
            results!.Add(null);
            var index = results.Count - 1;
            return new AddPromiseResult(
                onFulfilled: (value) =>
                {
                    results[index] = value;
                    resolvedCount++;
                    CheckForAllCompleted();
                },
                onRejected: (reason) =>
                {
                    allPromise!.Settle(State.Rejected, reason);
                });
        }

        return Combine(
            initializeState: InitializeState,
            iterable: iterable,
            addPromise: AddPromise,
            finalizeState: CheckForAllCompleted);
    }

    public static object? allSettled(object? iterable)
    {
        JavaScriptRuntime.Array? results = null;
        var settledCount = 0;
        Promise? allSettledPromise = null;

        Promise InitializeState()
        {
            results = new JavaScriptRuntime.Array();
            allSettledPromise = new Promise();
            return allSettledPromise;
        }

        void CheckForAllCompleted()
        {
            if (settledCount == results!.Count)
            {
                allSettledPromise!.Settle(State.Fulfilled, results);
            }
        }

        AddPromiseResult AddPromise(Promise p)
        {
            results!.Add(null);
            var index = results.Count - 1;
            return new AddPromiseResult(
                onFulfilled: (value) =>
                {
                    results[index] = new FulfilledResult(value);
                    settledCount++;
                    CheckForAllCompleted();
                },
                onRejected: (reason) =>
                {
                    results[index] = new RejectedResult(reason);
                    settledCount++;
                    CheckForAllCompleted();
                });
        }

        return Combine(
            initializeState: InitializeState,
            iterable: iterable,
            addPromise: AddPromise,
            finalizeState: CheckForAllCompleted);
    }

    public static object? any(object? iterable)
    {
        JavaScriptRuntime.Array? rejectionReasons = null;
        var rejectedCount = 0;
        Promise? anyPromise = null;
        var totalCount = 0;

        Promise InitializeState()
        {
            rejectionReasons = new JavaScriptRuntime.Array();
            anyPromise = new Promise();
            return anyPromise;
        }

        AddPromiseResult AddPromise(Promise p)
        {
            totalCount++;
            return new AddPromiseResult(
                onFulfilled: (value) =>
                {
                    anyPromise!.Settle(State.Fulfilled, value);
                },
                onRejected: (reason) =>
                {
                    rejectionReasons!.Add(reason);
                    rejectedCount++;
                    if (rejectedCount == totalCount)
                    {
                        anyPromise!.Settle(State.Rejected, new AggregateError(rejectionReasons, "All promises were rejected"));
                    }
                });
        }

        void FinalizeState()
        {
            // handle the case of an empty iterable
            if (totalCount == 0)
            {
                // this is the same error message nodejs returns for Promise.any with an empty iterable
                anyPromise!.Settle(State.Rejected, new AggregateError(rejectionReasons!, "All promises were rejected"));
            }
        }

        return Combine(
            initializeState: InitializeState,
            iterable: iterable,
            addPromise: AddPromise,
            finalizeState: FinalizeState);
    }

    public static object? race(object? iterable)
    {
        Promise? racePromise = null;

        Promise InitializeState()
        {
            racePromise = new Promise();
            return racePromise;
        }

        AddPromiseResult AddPromise(Promise p)
        {
            return new AddPromiseResult(
                onFulfilled: (value) =>
                {
                    racePromise!.Settle(State.Fulfilled, value);
                },
                onRejected: (reason) =>
                {
                    racePromise!.Settle(State.Rejected, reason);
                });
        }

        return Combine(
            initializeState: InitializeState,
            iterable: iterable,
            addPromise: AddPromise,
            finalizeState: () => { });
    }

    // Private methods
    private void InvokeExecutor(object? executor)
    {
        if (executor is not Delegate jsFunction)
        {
            throw new JavaScriptRuntime.TypeError("Promise resolver is not a function");
        }

        // the first parameter is ignored.. only exists for consistency
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
        var scheduler = GlobalThis.ServiceProvider?.Resolve<JavaScriptRuntime.EngineCore.IMicrotaskScheduler>(); 
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
            // select the appropriate handler
            var handler = _state == State.Fulfilled ? reaction.OnFulfilled : reaction.OnRejected;

            // by default the state and result are passed on
            var newState = _state;
            object? newResult = _result;

            if (handler != null)
            {
                // then and catch change the state if a handler exists to Fulfilled
                // finally does not change the state
                // currently "thenable" scenarios are not yet supported (handler returns something promiselike)
                newState = reaction.IsFinally ? newState : State.Fulfilled;
                var handlerResult = ExecuteHandler(handler, _result, reaction.IsFinally);
                newResult = reaction.IsFinally ? _result : handlerResult;

                // is handler result a Promise?
                // then we need inject it into the chain
                if (handlerResult is Promise handlerPromise)
                {
                    handlerPromise.then(
                        new Func<object?[], object?, object?>((_, res) =>
                        {
                            // for then/catch.. pass along what ever the result was
                            // if it is finally.. pass along the previous result.. finally is just an observer
                            reaction.NextPromise.Settle(newState,  reaction.IsFinally ? _result : res);
                            return null;
                        }),
                        new Func<object?[], object?, object?>((_, err) =>
                        {
                            // for then, catch and even finally.. return the error state
                            // something failed
                            reaction.NextPromise.Settle(State.Rejected, err);
                            return null;
                        })
                    );

                    // we are done.. lets bail
                    return;
                }

            }

            reaction.NextPromise.Settle(newState, newResult);
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
    /// see 27.2.5.4.1 in the ECMA 262
    /// </Remarks>
    private static object? ExecuteHandler(object? handler, object? previousResult, bool isFinally)
    {
        // for then, catch, finally. etc
        if (handler is not Delegate jsFunction)
        {
            return previousResult;
        }

        // for then and catch 1 value is passed to the delegate
        // for finally no parameters are passed
        object? handlerParameter = isFinally ? null : previousResult;
        object? result;

        // scopes are  always captured in closures (see Closure.Bind)
        // so the first parameter is always null
        if (jsFunction is Func<object[]?, object?> func1)
        {
            result = func1(null);
        }
        else if (jsFunction is Func<object[]?, object?, object?> func2)
        {
            result = func2(null, handlerParameter);
        }
        else
        {
            // get the number of parameters
            var paramCount = jsFunction.Method.GetParameters().Length;

            var args = new object?[paramCount];
            args[1] = handlerParameter;
            result = jsFunction.DynamicInvoke(args);
        }

        // For finally handlers: we need to return the actual result if it's a Promise
        // (so that ProcessReaction can wait on it), but for non-Promise values,
        // finally is only an observer and cannot alter the result
        if (isFinally && result is not Promise)
        {
            return previousResult;
        }
        
        return result;
    }

    private static System.Collections.IEnumerable? ToEnumerableOrThrow(object? obj, out TypeError? typeError)
    {
        // in javascript strings are iterable
        if (obj is string interableAsString)
        {
            obj = new JavaScriptRuntime.Array(interableAsString.ToCharArray().Select(c => c.ToString()));
        }

        if (obj is not System.Collections.IEnumerable enumerable)
        {
            typeError = new JavaScriptRuntime.TypeError("Promise method requires an iterable");
            return null;
        }
        else
        {
            typeError = null!;
        }

        return enumerable;
    }

    private delegate void CombinePromiseHandler(object? value);
    private record AddPromiseResult(CombinePromiseHandler onFulfilled, CombinePromiseHandler onRejected);
    private delegate AddPromiseResult AddPromise(Promise p);

    private static Promise Combine(
        Func<Promise> initializeState,
        object? iterable,
        AddPromise addPromise,
        Action finalizeState)
    {
        var enumerable = ToEnumerableOrThrow(iterable, out TypeError? typeError);
        if (typeError != null)
        {
            return (Promise)Promise.reject(typeError)!;
        }
        
        Promise combinedPromise = initializeState();

        foreach (var item in enumerable!)
        {
            Promise? p = item as Promise;
            if (p == null)
            {
                p = (Promise)Promise.resolve(item)!;
            }

            var handlers = addPromise(p);
            p.then(
                new Func<object?[], object?, object?>((_, value) =>
                {
                    handlers.onFulfilled(value);
                    return null;
                }),
                new Func<object?[], object?, object?>((_, reason) =>
                {
                    handlers.onRejected(reason);
                    return null;
                })
            );

        }

        finalizeState();

        return combinedPromise;
    }

    private abstract class SettledResult
    {
        public readonly string status;

        protected SettledResult(string status)
        {
            this.status = status;
        }
    }

    private sealed class FulfilledResult : SettledResult
    {
        public readonly object? value;

        public FulfilledResult(object? value) : base("fulfilled")
        {
            this.value = value;
        }
    }

    private sealed class RejectedResult : SettledResult
    {
        public readonly object? reason;

        public RejectedResult(object? reason) : base("rejected")
        {
            this.reason = reason;
        }
    }

}