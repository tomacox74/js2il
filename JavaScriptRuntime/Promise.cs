using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using JavaScriptRuntime.EngineCore;
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
        if (value is Promise existing)
        {
            return existing;
        }

        var promise = new Promise();
        promise.ResolveValue(value);
        return promise;
    }

    /// <summary>
    /// Returns an object containing a new promise and its associated resolve/reject functions.
    /// Equivalent to the TC39 Promise.withResolvers() proposal (now part of ECMA-262).
    /// </summary>
    public static PromiseWithResolvers withResolvers()
    {
        var promise = new Promise();

        var resolve = new Func<object[]?, object?, object?>((_, value) =>
        {
            promise.ResolveValue(value);
            return null;
        });

        var reject = new Func<object[]?, object?, object?>((_, reason) =>
        {
            promise.Settle(State.Rejected, reason);
            return null;
        });

        return new PromiseWithResolvers(promise, resolve, reject);
    }

    /// <summary>
    /// Synchronous helper for lowering JavaScript <c>await</c> when HasAwaits=false.
    /// 
    /// This is a fallback path used only when the compiler determines no actual awaits
    /// exist in an async function. For already-resolved promises, it extracts the value
    /// synchronously. For pending promises, it throws (should not happen in practice
    /// since the full state machine path handles that case).
    /// 
    /// The primary await implementation uses <see cref="AsyncScope.SetupAwaitContinuation"/> which
    /// generates proper suspension/resumption via promise.then() callbacks.
    /// </summary>
    /// <param name="awaited">The value being awaited (typically a Promise)</param>
    /// <returns>The resolved value if the promise is already settled</returns>
    /// <exception cref="NotSupportedException">If the promise is still pending</exception>
    /// <exception cref="JavaScriptRuntime.JsThrownValueException">If the promise was rejected</exception>
    public static object? AwaitValue(object? awaited)
    {
        // First, ensure we have a promise (Promise.resolve() behavior)
        if (awaited is not Promise promise)
        {
            promise = (Promise)resolve(awaited)!;
        }
        
        // Check the promise state
        lock (promise._reactions)
        {
            switch (promise._state)
            {
                case State.Fulfilled:
                    return promise._result;
                    
                case State.Rejected:
                    throw new JsThrownValueException(promise._result);
                    
                case State.Pending:
                    throw new NotSupportedException(
                        "Cannot await a pending promise using the synchronous AwaitValue helper. " +
                        "This code path is only used when HasAwaits=false. For pending promises, " +
                        "the compiler should generate a full state machine with AsyncScope.SetupAwaitContinuation.");
                    
                default:
                    throw new InvalidOperationException($"Unknown promise state: {promise._state}");
            }
        }
    }

    /// <summary>
    /// Creates a new scopes array with the async function's leaf scope prepended.
    /// This is called on initial invocation of an async function to create the scopes array
    /// that will be used for resumption.
    /// </summary>
    /// <param name="leafScope">The async function's scope instance</param>
    /// <param name="parentScopes">The original scopes array containing parent scopes</param>
    /// <returns>A new array: [leafScope, ...parentScopes]</returns>
    public static object[] PrependScopeToArray(object leafScope, object[] parentScopes)
    {
        var result = new object[parentScopes.Length + 1];
        result[0] = leafScope;
        System.Array.Copy(parentScopes, 0, result, 1, parentScopes.Length);
        return result;
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

        // The first parameter is the scopes array.
        // JS2IL's scopes ABI expects at least one slot (even when there are no captured scopes).
        // Using an empty array can cause IndexOutOfRange when nested closures access parent scopes.
        var unusedContext = new object[1];
        var Resolve = new Func<object[]?, object?, object?>((_, value) =>
        {
            return ResolveValue(value);
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

        var jobCallback = JavaScriptRuntime.EngineCore.HostJobCallbacks.HostMakeJobCallback(() => ProcessReaction(reaction));

        scheduler.QueueMicrotask(() =>
        {
            JavaScriptRuntime.EngineCore.HostJobCallbacks.HostCallJobCallback(jobCallback, v: null, argumentsList: System.Array.Empty<object?>());
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
                if (reaction.IsFinally)
                {
                    var cleanupResult = ExecuteHandler(handler, _result, isFinally: true);

                    // If finally returns a Promise/thenable, we must wait for it.
                    // On fulfillment: preserve the original state/result.
                    // On rejection: override with the cleanup error.
                    if (TryWaitFinally(cleanupResult, reaction.NextPromise, _state, _result))
                    {
                        return;
                    }

                    reaction.NextPromise.Settle(_state, _result);
                    return;
                }

                // then/catch: handler exists -> state becomes Fulfilled
                newState = State.Fulfilled;
                var handlerResult = ExecuteHandler(handler, _result, isFinally: false);
                newResult = handlerResult;

                if (TryAssimilateThenable(handlerResult, reaction.NextPromise))
                {
                    return;
                }

                reaction.NextPromise.Settle(newState, newResult);
                return;
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
            // Reflection fallback for non-standard delegate types.
            // JS2IL delegates normally take a leading scopes array parameter.
            var paramCount = jsFunction.Method.GetParameters().Length;
            if (paramCount == 0)
            {
                result = jsFunction.DynamicInvoke(null);
            }
            else
            {
                var args = new object?[paramCount];
                args[0] = null; // scopes
                if (!isFinally && paramCount > 1)
                {
                    args[1] = handlerParameter;
                }
                result = jsFunction.DynamicInvoke(args);
            }
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

    /// <summary>
    /// Resolves this promise with <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// This implements the Promise Resolution Procedure behavior:
    /// if <paramref name="value"/> is a Promise, adopt its state; if it is a thenable
    /// (an object with a callable <c>then</c> property), invoke <c>then</c> with
    /// resolving functions; otherwise fulfill with <paramref name="value"/>.
    /// </remarks>
    private object? ResolveValue(object? value)
    {
        if (TryAssimilateThenable(value, this))
        {
            return null;
        }

        return Settle(State.Fulfilled, value);
    }

    /// <summary>
    /// Attempts to assimilate a promise/thenable into <paramref name="targetPromise"/>.
    /// </summary>
    /// <param name="value">The value being resolved.</param>
    /// <param name="targetPromise">The promise whose state should be adopted.</param>
    /// <returns>
    /// True if <paramref name="value"/> was a Promise or thenable and assimilation was started;
    /// false if <paramref name="value"/> should be treated as a non-thenable fulfillment value.
    /// </returns>
    private static bool TryAssimilateThenable(object? value, Promise targetPromise)
    {
        if (value is Promise promise)
        {
            if (ReferenceEquals(promise, targetPromise))
            {
                targetPromise.Settle(State.Rejected, new TypeError("Promise cannot resolve itself"));
                return true;
            }

            promise.then(
                new Func<object?[], object?, object?>((_, res) =>
                {
                    targetPromise.ResolveValue(res);
                    return null;
                }),
                new Func<object?[], object?, object?>((_, err) =>
                {
                    targetPromise.Settle(State.Rejected, err);
                    return null;
                })
            );
            return true;
        }

        if (value == null || value is JsNull)
        {
            return false;
        }

        if (value is string || value.GetType().IsValueType)
        {
            return false;
        }

        object? thenProp;
        try
        {
            thenProp = JavaScriptRuntime.Object.GetProperty(value, "then");
        }
        catch (Exception ex)
        {
            targetPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            return true;
        }

        if (thenProp is not Delegate thenDelegate)
        {
            return false;
        }

        int alreadyCalled = 0;
        object resolve = new Func<object[]?, object?, object?>((_, res) =>
        {
            if (System.Threading.Interlocked.Exchange(ref alreadyCalled, 1) == 1) return null;
            targetPromise.ResolveValue(res);
            return null;
        });

        object reject = new Func<object[]?, object?, object?>((_, err) =>
        {
            if (System.Threading.Interlocked.Exchange(ref alreadyCalled, 1) == 1) return null;
            targetPromise.Settle(State.Rejected, err);
            return null;
        });

        try
        {
            // Invoke the previously retrieved 'then' value, preserving 'this' binding.
            var previousThis = RuntimeServices.SetCurrentThis(value);
            try
            {
                Closure.InvokeWithArgs(thenDelegate, System.Array.Empty<object>(), resolve, reject);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }
        catch (Exception ex)
        {
            if (System.Threading.Volatile.Read(ref alreadyCalled) == 0)
            {
                targetPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            }
        }

        return true;
    }

    /// <summary>
    /// Implements Promise.prototype.finally pass-through semantics when the finally handler
    /// returns a Promise/thenable: wait for it, then preserve the original state/result.
    /// </summary>
    private static bool TryWaitFinally(object? cleanupResult, Promise nextPromise, State originalState, object? originalResult)
    {
        if (cleanupResult is Promise cleanupPromise)
        {
            cleanupPromise.then(
                new Func<object?[], object?, object?>((_, _) =>
                {
                    nextPromise.Settle(originalState, originalResult);
                    return null;
                }),
                new Func<object?[], object?, object?>((_, err) =>
                {
                    nextPromise.Settle(State.Rejected, err);
                    return null;
                })
            );
            return true;
        }

        if (cleanupResult == null || cleanupResult is JsNull)
        {
            return false;
        }

        if (cleanupResult is string || cleanupResult.GetType().IsValueType)
        {
            return false;
        }

        object? thenProp;
        try
        {
            thenProp = JavaScriptRuntime.Object.GetProperty(cleanupResult, "then");
        }
        catch (Exception ex)
        {
            nextPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            return true;
        }

        if (thenProp is not Delegate thenDelegate)
        {
            return false;
        }

        int alreadyCalled = 0;
        object resolve = new Func<object[]?, object?, object?>((_, __) =>
        {
            if (System.Threading.Interlocked.Exchange(ref alreadyCalled, 1) == 1) return null;
            nextPromise.Settle(originalState, originalResult);
            return null;
        });

        object reject = new Func<object[]?, object?, object?>((_, err) =>
        {
            if (System.Threading.Interlocked.Exchange(ref alreadyCalled, 1) == 1) return null;
            nextPromise.Settle(State.Rejected, err);
            return null;
        });

        try
        {
            var previousThis = RuntimeServices.SetCurrentThis(cleanupResult);
            try
            {
                Closure.InvokeWithArgs(thenDelegate, System.Array.Empty<object>(), resolve, reject);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }
        catch (Exception ex)
        {
            if (System.Threading.Volatile.Read(ref alreadyCalled) == 0)
            {
                nextPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            }
        }

        return true;
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