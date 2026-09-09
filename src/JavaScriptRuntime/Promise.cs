using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using JavaScriptRuntime.EngineCore;
namespace JavaScriptRuntime;

[IntrinsicObject("Promise")]
public sealed partial class Promise : JsObject, IJavaScriptPromise
{
    private static readonly BuiltinFunction2 PrototypeThenValue = PrototypeThen;
    private static readonly BuiltinFunction1 PrototypeCatchValue = PrototypeCatch;
    private static readonly BuiltinFunction1 PrototypeFinallyValue = PrototypeFinally;
    /// <summary>Realm-owned <c>Promise.prototype</c> intrinsic (issue #1824).</summary>
    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.PromisePrototype,
            static () => new JsObject(),
            static prototype => InitializePrototype(prototype));

    // Nested types
    private enum State { Pending, Fulfilled, Rejected }

    private struct Reaction
    {
        public readonly object? OnFulfilled;
        public readonly object? OnRejected;
        public readonly Promise? NextPromise;
        public readonly JavaScriptRuntime.Node.AsyncContextSnapshot? Context;

        /// <summary>
        /// When set, the reaction settles a caller-provided PromiseCapability by
        /// invoking these resolving functions directly in the reaction job (used by
        /// Promise.prototype.then's SpeciesConstructor path). When null, the reaction
        /// settles <see cref="NextPromise"/> directly (the intrinsic fast path).
        /// </summary>
        public readonly object? CapabilityResolve;
        public readonly object? CapabilityReject;

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
            CapabilityResolve = null;
            CapabilityReject = null;
            IsFinally = isFinally;
            Context = JavaScriptRuntime.Node.AsyncContextRuntime.CaptureCurrentSnapshot();
        }

        private Reaction(object? onFulfilled, object? onRejected, object? capabilityResolve, object? capabilityReject)
        {
            OnFulfilled = onFulfilled;
            OnRejected = onRejected;
            NextPromise = null;
            CapabilityResolve = capabilityResolve;
            CapabilityReject = capabilityReject;
            IsFinally = false;
            Context = JavaScriptRuntime.Node.AsyncContextRuntime.CaptureCurrentSnapshot();
        }

        public static Reaction ForCapability(
            object? onFulfilled,
            object? onRejected,
            object? capabilityResolve,
            object? capabilityReject)
            => new Reaction(onFulfilled, onRejected, capabilityResolve, capabilityReject);

        public bool HasCapability => CapabilityResolve is not null;
    }

    private sealed record PromiseCapability(
        object Promise,
        object Resolve,
        object Reject);

    // Fields
    private State _state = State.Pending;

    private object? _result;
    private bool _isHandled;
    private readonly JavaScriptRuntime.Node.AsyncResourceState? _asyncResourceState;

    private readonly List<Reaction> _reactions = new();

    private static void InitializePrototype(JsObject prototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        Function.InitializeFunctionInstance(
            PrototypeThenValue,
            2d,
            "then",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(PrototypeThenValue));
        Function.MarkUndefinedPrototype(PrototypeThenValue);
        Function.InitializeFunctionInstance(
            PrototypeCatchValue,
            1d,
            "catch",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(PrototypeCatchValue));
        Function.MarkUndefinedPrototype(PrototypeCatchValue);
        Function.InitializeFunctionInstance(
            PrototypeFinallyValue,
            1d,
            "finally",
            requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(PrototypeFinallyValue));
        Function.MarkUndefinedPrototype(PrototypeFinallyValue);

        DefineDataProperty(prototype, "then", PrototypeThenValue);
        DefineDataProperty(prototype, "catch", PrototypeCatchValue);
        DefineDataProperty(prototype, "finally", PrototypeFinallyValue);
        PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = false,
            Value = "Promise"
        });
    }

    private static void DefineDataProperty(object target, string key, object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    private static Promise GetPromiseReceiver(object? thisValue, string methodName)
    {
        if (thisValue is not Promise promise)
        {
            throw new TypeError($"Promise.prototype.{methodName} called on incompatible receiver");
        }

        return promise;
    }

    private static object? PrototypeThen(object? thisArgument, object? onFulfilledArgument, object? onRejectedArgument)
    {
        // 27.2.5.4 Promise.prototype.then: the this value must be a Promise, and the
        // result promise is created from SpeciesConstructor(promise, %Promise%).
        return GetPromiseReceiver(thisArgument, "then")
            .ThenWithSpeciesConstructor(onFulfilledArgument, onRejectedArgument);
    }

    private static object? PrototypeCatch(object? thisArgument, object? onRejectedArgument)
    {
        // 27.2.5.1 Promise.prototype.catch ( onRejected )
        // 1. Let promise be the this value.
        // 2. Return ? Invoke(promise, "then", « undefined, onRejected »).
        // This must be fully generic (works on any receiver whose "then" property
        // is invoked with the original receiver as `this`), not limited to actual
        // Promise instances.
        return ObjectRuntime.CallMember2(thisArgument!, "then", null, onRejectedArgument);
    }

    private static object? PrototypeFinally(object? thisArgument, object? onFinallyArgument)
    {
        // 27.2.5.3 Promise.prototype.finally ( onFinally )
        // 1. Let promise be the this value.
        // 2. If Type(promise) is not Object, throw a TypeError exception.
        if (!Proxy.IsObjectLikeValue(thisArgument))
        {
            throw new TypeError("Promise.prototype.finally called on non-object");
        }

        // 3. Let C be ? SpeciesConstructor(promise, %Promise%). Performed immediately,
        //    before building the wrappers or invoking "then", so a throwing/poisoned
        //    constructor or @@species is observed here.
        // 4. Assert: IsConstructor(C).
        var constructor = SpeciesConstructor(thisArgument);

        object? thenFinallyValue;
        object? catchFinallyValue;

        if (!CallableOperations.IsCallable(onFinallyArgument))
        {
            // 5. If IsCallable(onFinally) is false, then
            //    a. Let thenFinally be onFinally.
            //    b. Let catchFinally be onFinally.
            thenFinallyValue = onFinallyArgument;
            catchFinallyValue = onFinallyArgument;
        }
        else
        {
            var onFinally = onFinallyArgument;
            var capturedConstructor = constructor;

            // Then Finally Function: result = onFinally(); promise = PromiseResolve(C, result);
            // return Invoke(promise, "then", « (value) => value »).
            BuiltinFunction1 thenFinally = (_, value) =>
            {
                var result = CallableOperations.Call0(onFinally, null);
                var resultPromise = ResolveForConstructor(capturedConstructor, result);
                BuiltinFunction1 valueThunk = (_, _) => value;
                Function.InitializeFunctionInstance(valueThunk, 0d, string.Empty, requiresInvocationContext: false);
                Function.MarkUndefinedPrototype(valueThunk);
                var valueThunkValue = BuiltinDelegateFunctionAdapter.FromDelegate(valueThunk);
                return ObjectRuntime.CallMember1(resultPromise!, "then", valueThunkValue);
            };
            Function.InitializeFunctionInstance(thenFinally, 1d, string.Empty, requiresInvocationContext: false);
            Function.MarkUndefinedPrototype(thenFinally);
            thenFinallyValue = BuiltinDelegateFunctionAdapter.FromDelegate(thenFinally);

            // Catch Finally Function: result = onFinally(); promise = PromiseResolve(C, result);
            // return Invoke(promise, "then", « thrower »).
            BuiltinFunction1 catchFinally = (_, reason) =>
            {
                var result = CallableOperations.Call0(onFinally, null);
                var resultPromise = ResolveForConstructor(capturedConstructor, result);
                // The spec thrower rethrows `reason`; returning an already-rejected
                // promise propagates `reason` verbatim through thenable assimilation
                // without wrapping it in a CLR exception.
                BuiltinFunction1 thrower = (_, _) => Promise.reject(reason);
                Function.InitializeFunctionInstance(thrower, 0d, string.Empty, requiresInvocationContext: false);
                Function.MarkUndefinedPrototype(thrower);
                var throwerValue = BuiltinDelegateFunctionAdapter.FromDelegate(thrower);
                return ObjectRuntime.CallMember1(resultPromise!, "then", throwerValue);
            };
            Function.InitializeFunctionInstance(catchFinally, 1d, string.Empty, requiresInvocationContext: false);
            Function.MarkUndefinedPrototype(catchFinally);
            catchFinallyValue = BuiltinDelegateFunctionAdapter.FromDelegate(catchFinally);
        }

        // 7. Return ? Invoke(promise, "then", « thenFinally, catchFinally »).
        // Invoke performs Get(promise, "then") and then Call, which throws a
        // TypeError when the resolved "then" property is not callable. Resolve the
        // property explicitly (respecting own-property shadowing) so a non-callable
        // (or overridden) "then" is handled per spec rather than falling back to the
        // prototype implementation.
        var thenMethod = ObjectRuntime.GetProperty(thisArgument!, "then");
        if (!CallableOperations.IsCallable(thenMethod))
        {
            throw new TypeError("Promise.prototype.finally: then is not a function");
        }

        return CallableOperations.Call2(
            thenMethod,
            thisArgument,
            thenFinallyValue,
            catchFinallyValue);
    }

    /// <summary>
    /// Determines whether a member lookup would dispatch differently than the
    /// intrinsic Promise prototype implementation.
    /// </summary>
    public static bool HasMemberOverride(object target, string memberName)
    {
        if (target is not Promise || !string.Equals(memberName, "then", StringComparison.Ordinal))
        {
            return true;
        }

        return !ReferenceEquals(ObjectRuntime.GetProperty(target, memberName), PrototypeThenValue);
    }

    private void InitializeIntrinsicSurface()
    {
        PrototypeChain.InitializePrototype(this, Prototype);
    }

    // Constructors
    public Promise(object? executor)
    {
        InitializeIntrinsicSurface();
        _asyncResourceState =
            JavaScriptRuntime.Node.AsyncContextRuntime.TryCreatePromiseResource(this);
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
        InitializeIntrinsicSurface();
        _asyncResourceState =
            JavaScriptRuntime.Node.AsyncContextRuntime.TryCreatePromiseResource(this);
    }

    // Public methods
    public static object? resolve(object? value)
    {
        // PromiseResolve(%Promise%, value): return the value unchanged only when it is
        // already a promise whose own "constructor" is the intrinsic %Promise%. A
        // promise whose "constructor" was overridden must not be blindly reused.
        if (value is Promise && PromiseResolveConstructorMatches(value, null))
        {
            return value;
        }

        var promise = new Promise();
        promise.ResolveValue(value);
        return promise;
    }

    /// <summary>
    /// Implements the SameValue(xConstructor, C) test of PromiseResolve (ECMA-262
    /// 27.2.4.7.1). When <paramref name="constructor"/> is the intrinsic %Promise%
    /// (or <c>null</c>, meaning the intrinsic), identity is decided by
    /// <see cref="GlobalThis.IsPromiseConstructorValue"/> so that any canonical
    /// representation of %Promise% matches; otherwise a strict SameValue comparison
    /// against the supplied constructor is used.
    /// </summary>
    private static bool PromiseResolveConstructorMatches(object? value, object? constructor)
    {
        var valueConstructor = ObjectRuntime.GetProperty(value!, "constructor");
        if (constructor is null || GlobalThis.IsPromiseConstructorValue(constructor))
        {
            return GlobalThis.IsPromiseConstructorValue(valueConstructor);
        }

        return Operators.SameValue(valueConstructor, constructor);
    }

    internal static object? ResolveForConstructor(object? constructor, object? value)
    {
        if (constructor is null || constructor is JsNull)
        {
            return Promise.resolve(value);
        }

        if (!ObjectRuntime.IsConstructibleValue(constructor))
        {
            throw new TypeError("Promise.resolve requires a constructor receiver");
        }

        // PromiseResolve identity (ECMA-262 27.2.4.7.1): when the value is already a
        // promise whose own "constructor" is SameValue with C, return it unchanged.
        // This must run even for the intrinsic Promise constructor so that a promise
        // whose "constructor" was overridden is not blindly reused.
        if (value is Promise && PromiseResolveConstructorMatches(value, constructor))
        {
            return value;
        }

        if (GlobalThis.IsPromiseConstructorValue(constructor))
        {
            // Intrinsic constructor fast path: allocate a fresh native promise and
            // run the resolution procedure directly instead of the capability dance.
            var intrinsicPromise = new Promise();
            intrinsicPromise.ResolveValue(value);
            return intrinsicPromise;
        }

        object? capabilityResolve = null;
        object? capabilityReject = null;
        Func<object[], object?, object?, object?> executor = (scopes, resolveArg, rejectArg) =>
        {
            if (capabilityResolve is not null || capabilityReject is not null)
            {
                throw new TypeError("Promise capability executor already initialized");
            }

            capabilityResolve = resolveArg;
            capabilityReject = rejectArg;
            return null;
        };

        Function.InitializeFunctionInstance(executor, 2d, string.Empty, requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(executor);
        var executorValue =
            BuiltinDelegateFunctionAdapter.FromDelegate(executor);

        var promise = ObjectRuntime.ConstructValue(
            constructor,
            new object[] { executorValue });
        if (!CallableOperations.IsCallable(capabilityResolve)
            || !CallableOperations.IsCallable(capabilityReject))
        {
            throw new TypeError("Promise constructor did not supply resolving functions");
        }

        CallableOperations.Call1(capabilityResolve, null, value);
        return promise;
    }

    internal static object? TryForConstructor(object? constructor, object? callback, object?[]? args)
    {
        if (constructor is null || constructor is JsNull || !ObjectRuntime.IsConstructibleValue(constructor))
        {
            throw new TypeError("Promise.try requires a constructor receiver");
        }

        object? capabilityResolve = null;
        object? capabilityReject = null;
        Func<object[], object?, object?, object?> executor = (scopes, resolveArg, rejectArg) =>
        {
            if (capabilityResolve is not null || capabilityReject is not null)
            {
                throw new TypeError("Promise capability executor already initialized");
            }

            capabilityResolve = resolveArg;
            capabilityReject = rejectArg;
            return null;
        };

        Function.InitializeFunctionInstance(executor, 2d, string.Empty, requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(executor);
        var executorValue =
            BuiltinDelegateFunctionAdapter.FromDelegate(executor);

        var promise = ObjectRuntime.ConstructValue(
            constructor,
            new object[] { executorValue });
        if (!CallableOperations.IsCallable(capabilityResolve)
            || !CallableOperations.IsCallable(capabilityReject))
        {
            throw new TypeError("Promise constructor did not supply resolving functions");
        }

        try
        {
            var callbackResult = CallableOperations.Call(
                callback,
                null,
                args ?? System.Array.Empty<object?>());
            CallableOperations.Call1(capabilityResolve, null, callbackResult);
        }
        catch (Exception ex)
        {
            CallableOperations.Call1(capabilityReject, null, ex.InnerException ?? ex);
        }

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

    public static void AwaitTopLevel(object? awaited)
    {
        if (awaited is not Promise promise)
        {
            return;
        }

        var eventLoop = GlobalThis.ServiceProvider?.Resolve<NodeEventLoopPump>()
            ?? throw new InvalidOperationException("Top-level await requires an active JROC runtime event loop.");

        while (true)
        {
            lock (promise._reactions)
            {
                switch (promise._state)
                {
                    case State.Fulfilled:
                        return;
                    case State.Rejected:
                        throw new JsThrownValueException(promise._result);
                }
            }

            eventLoop.RunOneIteration();
            eventLoop.WaitForWorkOrNextTimer();
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

    public object? @then()
        => @then(null, null);

    public object? @then(object? onFulfilled)
        => @then(onFulfilled, null);

    public object? @then(object? onFulfilled, object? onRejected)
    {
        MarkHandled();
        var nextPromise = new Promise();
        var reaction = new Reaction(
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(onFulfilled),
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(onRejected),
            nextPromise,
            false);

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

    /// <summary>
    /// Implements the SpeciesConstructor portion of Promise.prototype.then
    /// (27.2.5.4 steps 3-5). When the resolved constructor is the intrinsic
    /// %Promise%, the fast default reaction path is used; otherwise the result
    /// promise is created via NewPromiseCapability(C) and PerformPromiseThen
    /// targets that capability directly.
    /// </summary>
    internal object? ThenWithSpeciesConstructor(object? onFulfilled, object? onRejected)
    {
        var constructor = ResolveThenSpeciesConstructor();
        if (constructor is null)
        {
            // Default %Promise%: preserve the existing, allocation-light reaction path.
            return then(onFulfilled, onRejected);
        }

        // 4. resultCapability = NewPromiseCapability(C).
        var capability = NewPromiseCapability(constructor);

        // 5. PerformPromiseThen(promise, onFulfilled, onRejected, resultCapability):
        // register a reaction that invokes the generic capability's resolving
        // functions directly in the original reaction job.
        MarkHandled();
        var reaction = Reaction.ForCapability(
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(onFulfilled),
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(onRejected),
            capability.Resolve,
            capability.Reject);

        var shouldEnqueue = false;
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

        return capability.Promise;
    }

    /// <summary>
    /// SpeciesConstructor(this, %Promise%). Returns <c>null</c> when the resolved
    /// constructor is the intrinsic %Promise% (or an unset/undefined species), so
    /// callers can use the default fast path. Throws a TypeError for an invalid
    /// constructor/species, matching ECMA-262 SpeciesConstructor.
    /// </summary>
    private object? ResolveThenSpeciesConstructor()
    {
        var constructor = SpeciesConstructor(this);
        return GlobalThis.IsPromiseConstructorValue(constructor) ? null : constructor;
    }

    /// <summary>
    /// ECMA-262 SpeciesConstructor(O, %Promise%). Returns the resolved constructor,
    /// defaulting to the intrinsic %Promise% constructor when the "constructor"
    /// property is undefined or its @@species is undefined/null.
    /// </summary>
    private static object SpeciesConstructor(object? promise)
    {
        var constructor = ObjectRuntime.GetProperty(promise!, "constructor");
        if (constructor is null)
        {
            // constructor is undefined -> default constructor.
            return DefaultPromiseConstructorValue;
        }

        if (constructor is JsNull || TypeUtilities.IsPrimitive(constructor))
        {
            throw new TypeError("Promise constructor property is not an object");
        }

        var species = ObjectRuntime.GetProperty(constructor, Symbol.species.DebugId);
        if (species is null || species is JsNull)
        {
            // @@species is undefined or null -> default constructor.
            return DefaultPromiseConstructorValue;
        }

        if (!CallableOperations.IsConstructor(species))
        {
            throw new TypeError("Promise @@species is not a constructor");
        }

        return species;
    }

    private static object DefaultPromiseConstructorValue
        => BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise);

    public object? @finally(object? onFinally)
    {
        MarkHandled();
        var nextPromise = new Promise();
        var normalizedFinally =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(onFinally);
        var reaction = new Reaction(
            normalizedFinally,
            normalizedFinally,
            nextPromise,
            true);

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

    IJavaScriptPromise IJavaScriptPromise.then(object? onFulfilled, object? onRejected)
        => (IJavaScriptPromise)then(onFulfilled, onRejected)!;

    IJavaScriptPromise IJavaScriptPromise.@catch(object? onRejected)
        => (IJavaScriptPromise)@catch(onRejected)!;

    IJavaScriptPromise IJavaScriptPromise.@finally(object? onFinally)
        => (IJavaScriptPromise)@finally(onFinally)!;

    public static object? all(object? iterable)
        => AllForConstructor(
            BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise),
            iterable);

    internal static object? AllForConstructor(object? constructor, object? iterable)
        => PerformCombinator(CombinatorKind.All, constructor, iterable);

    public static object? allSettled(object? iterable)
        => AllSettledForConstructor(
            BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise),
            iterable);

    internal static object? AllSettledForConstructor(object? constructor, object? iterable)
        => PerformCombinator(CombinatorKind.AllSettled, constructor, iterable);

    public static object? any(object? iterable)
        => AnyForConstructor(
            BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise),
            iterable);

    internal static object? AnyForConstructor(object? constructor, object? iterable)
        => PerformCombinator(CombinatorKind.Any, constructor, iterable);

    public static object? allKeyed(object? dictionary)
        => AllKeyedForConstructor(
            BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise),
            dictionary);

    internal static object? AllKeyedForConstructor(object? constructor, object? dictionary)
        => PerformKeyedCombinator(settled: false, constructor, dictionary);

    public static object? allSettledKeyed(object? dictionary)
        => AllSettledKeyedForConstructor(
            BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Promise),
            dictionary);

    internal static object? AllSettledKeyedForConstructor(object? constructor, object? dictionary)
        => PerformKeyedCombinator(settled: true, constructor, dictionary);

    /// <summary>
    /// Constructor-aware implementation of the await-dictionary combinators
    /// (Promise.allKeyed and Promise.allSettledKeyed). Follows ECMA-262
    /// PerformPromiseAllKeyed: it iterates the dictionary's own enumerable
    /// string and symbol keys in [[OwnPropertyKeys]] order, resolves each value,
    /// and fulfils with CreateKeyedPromiseCombinatorResultObject — a
    /// null-prototype object whose keys follow the original source order
    /// regardless of settlement order (values for allKeyed, settlement records
    /// for allSettledKeyed).
    /// </summary>
    private static object? PerformKeyedCombinator(bool settled, object? constructor, object? dictionary)
    {
        var capability = NewPromiseCapability(constructor);

        try
        {
            var promiseResolve = GetPromiseResolve(constructor);

            if (dictionary is null || dictionary is JsNull || TypeUtilities.IsPrimitive(dictionary))
            {
                throw new TypeError("Promise keyed combinator requires an object dictionary");
            }

            // 1. allKeys = ? dictionary.[[OwnPropertyKeys]](): snapshot ALL own keys
            // (string and symbol) in source order. Enumerability is intentionally NOT
            // filtered here; it is re-checked inside the loop immediately before each
            // value is read so that a callback triggered by an earlier key can affect
            // the enumerability (or presence) of a later key.
            var keys = ObjectRuntime.GetOwnPropertyKeysInOrder(
                dictionary,
                includeEncodedSymbolKeys: true);

            // entries preserves source order; each value slot starts as undefined and
            // is filled in place as its promise settles.
            var entries = new List<(string Key, StrongBox<object?> Value)>(keys.Count);
            var remaining = new StrongBox<int>(1);

            void ResolveWhenComplete()
            {
                // CreateKeyedPromiseCombinatorResultObject: OrdinaryObjectCreate(null),
                // then CreateDataPropertyOrThrow for each entry in source order.
                var result = ObjectRuntime.CreateOrdinaryObject();
                PrototypeChain.SetPrototype(result, JsNull.Null);
                foreach (var (entryKey, entryValue) in entries)
                {
                    ObjectRuntime.CreateDataProperty(result, entryKey, entryValue.Value);
                }

                CallableOperations.Call1(capability.Resolve, null, result);
            }

            foreach (var key in keys)
            {
                // Re-check the descriptor immediately before reading the value: skip
                // keys that an earlier callback deleted or made non-enumerable.
                if (!ObjectRuntime.IsOwnPropertyPresentAndEnumerable(dictionary, key))
                {
                    continue;
                }

                var value = ObjectRuntime.GetProperty(dictionary, key);
                var entryValue = new StrongBox<object?>(null);
                entries.Add((key, entryValue));

                var nextPromise = CallableOperations.Call1(promiseResolve, constructor, value);
                var alreadyCalled = new StrongBox<bool>(false);
                remaining.Value++;

                BuiltinFunction1 resolveElement = (_, v) =>
                {
                    if (alreadyCalled.Value)
                    {
                        return null;
                    }

                    alreadyCalled.Value = true;
                    entryValue.Value = settled
                        ? CreateSettledRecord("fulfilled", "value", v)
                        : v;
                    remaining.Value--;
                    if (remaining.Value == 0)
                    {
                        ResolveWhenComplete();
                    }

                    return null;
                };

                object? onRejectedValue;
                if (settled)
                {
                    BuiltinFunction1 rejectElement = (_, reason) =>
                    {
                        if (alreadyCalled.Value)
                        {
                            return null;
                        }

                        alreadyCalled.Value = true;
                        entryValue.Value = CreateSettledRecord("rejected", "reason", reason);
                        remaining.Value--;
                        if (remaining.Value == 0)
                        {
                            ResolveWhenComplete();
                        }

                        return null;
                    };
                    onRejectedValue = CreateElementFunctionValue(rejectElement);
                }
                else
                {
                    onRejectedValue = capability.Reject;
                }

                var then = ObjectRuntime.GetProperty(nextPromise!, "then");
                CallableOperations.Call2(
                    then,
                    nextPromise,
                    CreateElementFunctionValue(resolveElement),
                    onRejectedValue);
            }

            remaining.Value--;
            if (remaining.Value == 0)
            {
                ResolveWhenComplete();
            }

            return capability.Promise;
        }
        catch (Exception ex)
        {
            CallableOperations.Call1(
                capability.Reject,
                null,
                GetThrownJsValue(ex));
            return capability.Promise;
        }
    }

    private enum CombinatorKind
    {
        All,
        AllSettled,
        Any,
    }

    /// <summary>
    /// Shared, constructor-aware implementation of the Promise combinators
    /// (Promise.all, Promise.allSettled, Promise.any). Implements
    /// NewPromiseCapability, GetPromiseResolve (evaluated once), per-element
    /// resolving functions with the "already called" guard, and the
    /// iterator/IteratorClose completion ordering required by ECMA-262.
    /// </summary>
    private static object? PerformCombinator(CombinatorKind kind, object? constructor, object? iterable)
    {
        // 1-2. NewPromiseCapability(C) is a throwing (?) completion: it propagates
        // synchronously rather than rejecting the returned promise.
        var capability = NewPromiseCapability(constructor);
        IJavaScriptIterator? iterator = null;
        var iteratorDone = false;

        try
        {
            // 3-4. GetPromiseResolve(C) — evaluated exactly once, before iteration.
            var promiseResolve = GetPromiseResolve(constructor);

            // 5-6. GetIterator(iterable, sync).
            iterator = ObjectRuntime.GetIterator(iterable);

            // The list of per-element results (values for all/allSettled, errors for any).
            var values = new JavaScriptRuntime.Array();
            var remaining = new StrongBox<int>(1);
            var index = 0;

            void ResolveWhenComplete()
            {
                if (kind == CombinatorKind.Any)
                {
                    CallableOperations.Call1(
                        capability.Reject,
                        null,
                        new AggregateError(values, "All promises were rejected"));
                }
                else
                {
                    CallableOperations.Call1(capability.Resolve, null, values);
                }
            }

            while (true)
            {
                object? nextValue;
                try
                {
                    var next = ObjectRuntime.IteratorNext(iterator);
                    if (ObjectRuntime.IteratorResultDone(next))
                    {
                        iteratorDone = true;
                        remaining.Value--;
                        if (remaining.Value == 0)
                        {
                            ResolveWhenComplete();
                        }

                        return capability.Promise;
                    }

                    nextValue = ObjectRuntime.IteratorResultValue(next);
                }
                catch
                {
                    // Errors while stepping the iterator (IteratorStep/IteratorValue)
                    // leave the iterator in a "done" state; it must not be closed.
                    iteratorDone = true;
                    throw;
                }

                values.Add(null);
                var elementIndex = index++;
                remaining.Value++;

                var nextPromise = CallableOperations.Call1(
                    promiseResolve,
                    constructor,
                    nextValue);

                var (onFulfilledValue, onRejectedValue) = CreateElementHandlers(
                    kind,
                    elementIndex,
                    values,
                    remaining,
                    capability,
                    ResolveWhenComplete);

                var then = ObjectRuntime.GetProperty(nextPromise!, "then");
                CallableOperations.Call2(
                    then,
                    nextPromise,
                    onFulfilledValue,
                    onRejectedValue);
            }
        }
        catch (Exception ex)
        {
            if (iterator is not null && !iteratorDone)
            {
                ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
            }

            CallableOperations.Call1(
                capability.Reject,
                null,
                GetThrownJsValue(ex));
            return capability.Promise;
        }
    }

    /// <summary>
    /// GetPromiseResolve(constructor): reads the constructor's <c>resolve</c>
    /// method once and asserts it is callable.
    /// </summary>
    private static object? GetPromiseResolve(object? constructor)
    {
        var promiseResolve = ObjectRuntime.GetProperty(constructor!, "resolve");
        if (!CallableOperations.IsCallable(promiseResolve))
        {
            throw new TypeError("Promise resolve is not callable");
        }

        return promiseResolve;
    }

    /// <summary>
    /// Builds the per-element resolving functions passed to each element promise's
    /// <c>then</c>. The functions carry the spec-mandated metadata (length 1,
    /// anonymous name, no <c>prototype</c>, not a constructor) and share a single
    /// "already called" guard so only the first settlement is observed.
    /// </summary>
    private static (object? OnFulfilled, object? OnRejected) CreateElementHandlers(
        CombinatorKind kind,
        int elementIndex,
        JavaScriptRuntime.Array values,
        StrongBox<int> remaining,
        PromiseCapability capability,
        Action resolveWhenComplete)
    {
        var alreadyCalled = new StrongBox<bool>(false);

        void RecordSettlement(object? entry)
        {
            values[elementIndex] = entry;
            remaining.Value--;
            if (remaining.Value == 0)
            {
                resolveWhenComplete();
            }
        }

        switch (kind)
        {
            case CombinatorKind.All:
            {
                BuiltinFunction1 resolveElement = (_, value) =>
                {
                    if (alreadyCalled.Value)
                    {
                        return null;
                    }

                    alreadyCalled.Value = true;
                    RecordSettlement(value);
                    return null;
                };
                return (
                    CreateElementFunctionValue(resolveElement),
                    capability.Reject);
            }

            case CombinatorKind.AllSettled:
            {
                BuiltinFunction1 resolveElement = (_, value) =>
                {
                    if (alreadyCalled.Value)
                    {
                        return null;
                    }

                    alreadyCalled.Value = true;
                    RecordSettlement(CreateSettledRecord("fulfilled", "value", value));
                    return null;
                };
                BuiltinFunction1 rejectElement = (_, reason) =>
                {
                    if (alreadyCalled.Value)
                    {
                        return null;
                    }

                    alreadyCalled.Value = true;
                    RecordSettlement(CreateSettledRecord("rejected", "reason", reason));
                    return null;
                };
                return (
                    CreateElementFunctionValue(resolveElement),
                    CreateElementFunctionValue(rejectElement));
            }

            case CombinatorKind.Any:
            {
                BuiltinFunction1 rejectElement = (_, reason) =>
                {
                    if (alreadyCalled.Value)
                    {
                        return null;
                    }

                    alreadyCalled.Value = true;
                    RecordSettlement(reason);
                    return null;
                };
                return (
                    capability.Resolve,
                    CreateElementFunctionValue(rejectElement));
            }

            default:
                throw new InvalidOperationException("Unknown Promise combinator kind");
        }
    }

    private static object? CreateElementFunctionValue(BuiltinFunction1 element)
    {
        Function.InitializeFunctionInstance(
            element,
            1d,
            string.Empty,
            requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(element);
        return BuiltinDelegateFunctionAdapter.FromDelegate(element);
    }

    private static JsObject CreateSettledRecord(string status, string valueKey, object? value)
    {
        var record = ObjectRuntime.CreateOrdinaryObject();
        ObjectRuntime.CreateDataProperty(record, "status", status);
        ObjectRuntime.CreateDataProperty(record, valueKey, value);
        return record;
    }

    private static object? GetThrownJsValue(Exception ex)
    {
        if (ex is JsThrownValueException thrown)
        {
            return thrown.Value;
        }

        if (ex.InnerException is JsThrownValueException innerThrown)
        {
            return innerThrown.Value;
        }

        return ex.InnerException ?? ex;
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
        if (!CallableOperations.IsCallable(executor))
        {
            throw new JavaScriptRuntime.TypeError("Promise resolver is not a function");
        }

        var Resolve = new Func<object[]?, object?, object?>((_, value) =>
        {
            return ResolveValue(value);
        });

        var Reject = new Func<object[]?, object?, object?>((_, reason) =>
        {
            return Settle(State.Rejected, reason);
        });

        var resolveValue =
            BuiltinDelegateFunctionAdapter.FromDelegate(Resolve);
        var rejectValue =
            BuiltinDelegateFunctionAdapter.FromDelegate(Reject);

        try 
        {
            CallableOperations.Call2(
                executor,
                null,
                resolveValue,
                rejectValue);
        }
        catch (Exception ex)
        {
            Settle(State.Rejected, ex.InnerException ?? ex);
        }
    }

    private object? Settle(State state, object? value)
    {
        if (state == State.Rejected && value is ScriptProcessExitException exit)
        {
            throw exit;
        }

        List<Reaction> toSchedule;

        if (_state != State.Pending)
        {
            return null;
        }

        _state = state;
        _result = value;
        if (state == State.Rejected && !_isHandled)
        {
            GlobalThis.ServiceProvider?
                .Resolve<UnhandledPromiseRejectionTracker>()
                .Track(this, value);
        }
        JavaScriptRuntime.Node.AsyncContextRuntime.EmitPromiseResolve(
            _asyncResourceState);
        toSchedule = new List<Reaction>(_reactions);
        _reactions.Clear();

        foreach (var r in toSchedule)
        {
            EnqueueReaction(r);
        }

        return null;
    }

    private void MarkHandled()
    {
        _isHandled = true;
        GlobalThis.ServiceProvider?
            .Resolve<UnhandledPromiseRejectionTracker>()
            .MarkHandled(this);
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
            JavaScriptRuntime.Node.AsyncContextRuntime.RunJobSnapshot(
                reaction.Context,
                reaction.NextPromise?._asyncResourceState,
                jobCallback);
        });
    }

    private void ProcessReaction(Reaction reaction)
    {
        // select the appropriate handler
        var handler = _state == State.Fulfilled ? reaction.OnFulfilled : reaction.OnRejected;

        // Promise.prototype.finally reactions retain the original settlement
        // semantics and never use a caller-provided capability.
        if (reaction.IsFinally && handler != null)
        {
            try
            {
                var cleanupResult = ExecuteHandler(handler, _result, isFinally: true);

                // If finally returns a Promise/thenable, we must wait for it.
                // On fulfillment: preserve the original state/result.
                // On rejection: override with the cleanup error.
                if (TryWaitFinally(cleanupResult, reaction.NextPromise!, _state, _result))
                {
                    return;
                }

                reaction.NextPromise!.Settle(_state, _result);
            }
            catch (Exception ex)
            {
                reaction.NextPromise!.Settle(State.Rejected, ex);
            }

            return;
        }

        // Per NewPromiseReactionJob (27.2.2.1), only the handler invocation is a
        // catchable completion. Compute the handler result (normal or abrupt) here;
        // the selected resolving callback is invoked afterwards so that any throw it
        // raises propagates as an abrupt completion of the job instead of triggering
        // a second (double) reject on the same capability.
        object? handlerValue = _result;
        var handlerAbrupt = false;
        Exception? handlerError = null;

        if (handler != null)
        {
            try
            {
                handlerValue = ExecuteHandler(handler, _result, isFinally: false);
            }
            catch (Exception ex)
            {
                handlerAbrupt = true;
                handlerError = ex;
            }
        }

        if (reaction.HasCapability)
        {
            if (handlerAbrupt)
            {
                CallableOperations.Call1(
                    reaction.CapabilityReject,
                    null,
                    GetThrownJsValue(handlerError!));
                return;
            }

            if (handler != null)
            {
                // then/catch handler present: resolve the caller-provided capability
                // with the handler result (assimilation is performed by the
                // capability's resolving function).
                CallableOperations.Call1(reaction.CapabilityResolve, null, handlerValue);
                return;
            }

            // No handler: forward the settlement (pass-through) to the capability's
            // resolving functions in the same reaction job.
            if (_state == State.Fulfilled)
            {
                CallableOperations.Call1(reaction.CapabilityResolve, null, handlerValue);
            }
            else
            {
                CallableOperations.Call1(reaction.CapabilityReject, null, handlerValue);
            }

            return;
        }

        // Intrinsic fast path: settle the internally-created next promise.
        if (handlerAbrupt)
        {
            reaction.NextPromise!.Settle(State.Rejected, handlerError);
            return;
        }

        if (handler != null)
        {
            // then/catch: handler exists -> state becomes Fulfilled
            if (TryAssimilateThenable(handlerValue, reaction.NextPromise!))
            {
                return;
            }

            reaction.NextPromise!.Settle(State.Fulfilled, handlerValue);
            return;
        }

        // No handler: pass the current settlement through unchanged.
        reaction.NextPromise!.Settle(_state, handlerValue);
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
        if (!CallableOperations.IsCallable(handler))
        {
            return previousResult;
        }

        return isFinally
            ? CallableOperations.Call0(handler, null)
            : CallableOperations.Call1(handler, null, previousResult);
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
            thenProp = JavaScriptRuntime.ObjectRuntime.GetProperty(value, "then");
        }
        catch (Exception ex)
        {
            targetPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            return true;
        }

        if (!CallableOperations.IsCallable(thenProp))
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
        resolve =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(resolve)!;
        reject =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(reject)!;

        try
        {
            CallableOperations.Call2(thenProp, value, resolve, reject);
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
            thenProp = JavaScriptRuntime.ObjectRuntime.GetProperty(cleanupResult, "then");
        }
        catch (Exception ex)
        {
            nextPromise.Settle(State.Rejected, ex.InnerException ?? ex);
            return true;
        }

        if (!CallableOperations.IsCallable(thenProp))
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
        resolve =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(resolve)!;
        reject =
            BuiltinDelegateFunctionAdapter.WrapJavaScriptVisibleValue(reject)!;

        try
        {
            CallableOperations.Call2(thenProp, cleanupResult, resolve, reject);
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

    private static PromiseCapability NewPromiseCapability(object? constructor)
    {
        if (!ObjectRuntime.IsConstructibleValue(constructor))
        {
            throw new TypeError("Promise constructor receiver is not a constructor");
        }

        object? capabilityResolve = null;
        object? capabilityReject = null;
        Func<object[], object?, object?, object?> executor = (_, resolve, reject) =>
        {
            if (capabilityResolve is not null || capabilityReject is not null)
            {
                throw new TypeError("Promise capability executor already initialized");
            }

            capabilityResolve = resolve;
            capabilityReject = reject;
            return null;
        };

        Function.InitializeFunctionInstance(
            executor,
            2d,
            string.Empty,
            requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(executor);
        var executorValue =
            BuiltinDelegateFunctionAdapter.FromDelegate(executor);
        var promise = ObjectRuntime.ConstructValue(
            constructor!,
            new object[] { executorValue });

        if (!CallableOperations.IsCallable(capabilityResolve)
            || !CallableOperations.IsCallable(capabilityReject))
        {
            throw new TypeError(
                "Promise constructor did not supply resolving functions");
        }

        if (TypeUtilities.IsPrimitive(promise))
        {
            throw new TypeError("Promise constructor returned a non-object");
        }

        return new PromiseCapability(
            promise!,
            capabilityResolve!,
            capabilityReject!);
    }

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

}
