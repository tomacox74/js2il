using System;
using System.Collections.Generic;
using System.Dynamic;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    [NodeModule("timers/promises")]
    public sealed class TimersPromises
    {
        private static readonly Action NoOp = () => { };
        private readonly IScheduler _scheduler;

        public TimersPromises()
        {
            _scheduler = GlobalThis.ServiceProvider?.Resolve<IScheduler>()
                ?? throw new InvalidOperationException("IScheduler is not available for timers/promises.");
        }

        public Promise setTimeout(object delay, object? value = null, object? options = null)
        {
            var deferred = Promise.withResolvers();
            object? signal;
            try
            {
                signal = TryGetOption(options, "signal");
            }
            catch (Exception ex)
            {
                RejectDeferred(deferred, ex);
                return deferred.promise;
            }

            var abortReason = GetAbortReason(signal);
            if (abortReason != null)
            {
                RejectDeferred(deferred, abortReason);
                return deferred.promise;
            }

            int settled = 0;
            int abortRequested = 0;
            Action unregisterAbort = NoOp;
            object? handle = null;

            var abortListener = new Action<object?>(abortSignalReason =>
            {
                System.Threading.Volatile.Write(ref abortRequested, 1);

                if (System.Threading.Interlocked.Exchange(ref settled, 1) != 0)
                {
                    return;
                }

                if (handle != null)
                {
                    _scheduler.Cancel(handle);
                }

                unregisterAbort();
                RejectDeferred(deferred, ResolveAbortError(signal, abortSignalReason));
            });

            TryRegisterAbortListener(signal, abortListener, out unregisterAbort);

            handle = _scheduler.Schedule(() =>
            {
                if (System.Threading.Interlocked.Exchange(ref settled, 1) != 0)
                {
                    return;
                }

                unregisterAbort();
                ResolveDeferred(deferred, value);
            }, TimeSpan.FromMilliseconds(CoerceDelay(delay)));

            if (System.Threading.Volatile.Read(ref abortRequested) != 0 && handle != null)
            {
                _scheduler.Cancel(handle);
            }

            var immediateAbortReason = GetAbortReason(signal);
            if (immediateAbortReason != null && System.Threading.Interlocked.Exchange(ref settled, 1) == 0)
            {
                if (handle != null)
                {
                    _scheduler.Cancel(handle);
                }

                unregisterAbort();
                RejectDeferred(deferred, immediateAbortReason);
            }

            return deferred.promise;
        }

        public Promise setImmediate(object? value = null, object? options = null)
        {
            var deferred = Promise.withResolvers();
            object? signal;
            try
            {
                signal = TryGetOption(options, "signal");
            }
            catch (Exception ex)
            {
                RejectDeferred(deferred, ex);
                return deferred.promise;
            }

            var abortReason = GetAbortReason(signal);
            if (abortReason != null)
            {
                RejectDeferred(deferred, abortReason);
                return deferred.promise;
            }

            int settled = 0;
            int abortRequested = 0;
            Action unregisterAbort = NoOp;
            object? handle = null;

            var abortListener = new Action<object?>(abortSignalReason =>
            {
                System.Threading.Volatile.Write(ref abortRequested, 1);

                if (System.Threading.Interlocked.Exchange(ref settled, 1) != 0)
                {
                    return;
                }

                if (handle != null)
                {
                    _scheduler.CancelImmediate(handle);
                }

                unregisterAbort();
                RejectDeferred(deferred, ResolveAbortError(signal, abortSignalReason));
            });

            TryRegisterAbortListener(signal, abortListener, out unregisterAbort);

            handle = _scheduler.ScheduleImmediate(() =>
            {
                if (System.Threading.Interlocked.Exchange(ref settled, 1) != 0)
                {
                    return;
                }

                unregisterAbort();
                ResolveDeferred(deferred, value);
            });

            if (System.Threading.Volatile.Read(ref abortRequested) != 0 && handle != null)
            {
                _scheduler.CancelImmediate(handle);
            }

            var immediateAbortReason = GetAbortReason(signal);
            if (immediateAbortReason != null && System.Threading.Interlocked.Exchange(ref settled, 1) == 0)
            {
                if (handle != null)
                {
                    _scheduler.CancelImmediate(handle);
                }

                unregisterAbort();
                RejectDeferred(deferred, immediateAbortReason);
            }

            return deferred.promise;
        }

        public object? setInterval(object? delay = null, object? value = null, object? options = null)
        {
            object? signal;
            object? startupFailure;
            double delayMs;

            try
            {
                delayMs = CoerceIntervalDelay(delay);
                signal = TryGetOption(options, "signal");
                startupFailure = GetAbortReason(signal);
            }
            catch (Exception ex)
            {
                delayMs = 1;
                signal = null;
                startupFailure = ex;
            }

            return new TimersPromisesIntervalIterator(_scheduler, delayMs, value, signal, startupFailure);
        }

        private static double CoerceDelay(object? delay)
        {
            var delayMs = TypeUtilities.ToNumber(delay);
            if (delayMs < 0 || double.IsNaN(delayMs))
            {
                return 0;
            }

            return delayMs;
        }

        // The underlying scheduler only reschedules repeating timers with a positive interval,
        // and Node clamps sub-1ms interval delays instead of treating them as zero-delay loops.
        private static double CoerceIntervalDelay(object? delay)
        {
            var delayMs = TypeUtilities.ToNumber(delay);
            if (delayMs < 1 || double.IsNaN(delayMs))
            {
                return 1;
            }

            return delayMs;
        }

        private static object? TryGetOption(object? options, string name)
        {
            if (options == null || options is JsNull)
            {
                return null;
            }

            if (options is ExpandoObject expando)
            {
                var dict = (System.Collections.Generic.IDictionary<string, object?>)expando;
                if (dict.TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            return ObjectRuntime.GetProperty(options, name);
        }

        private static AbortError? GetAbortReason(object? signal)
        {
            if (signal == null || signal is JsNull)
            {
                return null;
            }

            var aborted = TypeUtilities.ToBoolean(ObjectRuntime.GetProperty(signal, "aborted"));
            if (!aborted)
            {
                return null;
            }

            return CreateAbortError(ObjectRuntime.GetProperty(signal, "reason"));
        }

        private static AbortError CreateAbortError(object? signalReason)
        {
            if (signalReason == null || signalReason is JsNull)
            {
                return new AbortError();
            }

            return new AbortError("The operation was aborted", signalReason);
        }

        private static AbortError ResolveAbortError(object? signal, object? abortSignalReason)
        {
            return abortSignalReason is null or JsNull
                ? GetAbortReason(signal) ?? new AbortError()
                : CreateAbortError(abortSignalReason);
        }

        private static bool TryRegisterAbortListener(object? signal, Action<object?> abortListener, out Action unregister)
        {
            unregister = NoOp;

            if (signal == null || signal is JsNull)
            {
                return false;
            }

            if (signal is AbortSignal abortSignal)
            {
                return abortSignal.TryRegisterInternalListener(abortListener, out unregister);
            }

            var addListener = ObjectRuntime.GetProperty(signal, "addEventListener");
            if (addListener is not Delegate addDelegate)
            {
                return false;
            }

            Func<object[], object?[], object?> listenerDelegate = (_, _) =>
            {
                abortListener(null);
                return null;
            };

            Function.Call(addDelegate, signal, new object?[] { "abort", listenerDelegate });

            var removeListener = ObjectRuntime.GetProperty(signal, "removeEventListener");
            if (removeListener is Delegate removeDelegate)
            {
                unregister = () => Function.Call(removeDelegate, signal, new object?[] { "abort", listenerDelegate });
            }

            return true;
        }

        private static void ResolveDeferred(PromiseWithResolvers deferred, object? value)
        {
            if (deferred.resolve is Delegate resolve)
            {
                Closure.InvokeWithArgs(resolve, System.Array.Empty<object>(), value);
            }
        }

        private static void RejectDeferred(PromiseWithResolvers deferred, object? reason)
        {
            if (deferred.reject is Delegate reject)
            {
                Closure.InvokeWithArgs(reject, System.Array.Empty<object>(), reason);
            }
        }

        private sealed class TimersPromisesIntervalIterator : IJavaScriptAsyncIterator
        {
            private readonly IScheduler _scheduler;
            private readonly double _delayMs;
            private readonly object? _value;
            private readonly object? _signal;
            private readonly Queue<PromiseWithResolvers> _pending = new();

            private object? _handle;
            private object? _startupFailure;
            private object? _terminalError;
            private Action _unregisterAbort = NoOp;
            private bool _started;
            private bool _closed;
            private int _notYielded;

            public TimersPromisesIntervalIterator(IScheduler scheduler, double delayMs, object? value, object? signal, object? startupFailure)
            {
                _scheduler = scheduler;
                _delayMs = delayMs;
                _value = value;
                _signal = signal;
                _startupFailure = startupFailure;
                JavaScriptRuntime.AsyncIterator.InitializeAsyncIteratorSurface(this);
            }

            public bool HasReturn => true;

            public object? Next()
            {
                if (_startupFailure != null)
                {
                    var reason = _startupFailure;
                    _startupFailure = null;
                    _closed = true;
                    return Promise.reject(reason);
                }

                EnsureStarted();

                if (_terminalError != null)
                {
                    var reason = _terminalError;
                    _terminalError = null;
                    return Promise.reject(reason);
                }

                if (_notYielded > 0)
                {
                    _notYielded--;
                    return Promise.resolve(IteratorResult.Create(_value, done: false));
                }

                if (_closed)
                {
                    return Promise.resolve(IteratorResult.Create(null, done: true));
                }

                var deferred = Promise.withResolvers();
                _pending.Enqueue(deferred);
                return deferred.promise;
            }

            public object? Return()
            {
                _startupFailure = null;
                _terminalError = null;
                Close();

                while (_pending.Count > 0)
                {
                    ResolveDeferred(_pending.Dequeue(), IteratorResult.Create(null, done: true));
                }

                return Promise.resolve(IteratorResult.Create(null, done: true));
            }

            private void EnsureStarted()
            {
                if (_started || _closed)
                {
                    return;
                }

                _started = true;

                var abortReason = GetAbortReason(_signal);
                if (abortReason != null)
                {
                    AbortWithError(abortReason);
                    return;
                }

                TryRegisterAbortListener(_signal, abortSignalReason =>
                {
                    AbortWithError(ResolveAbortError(_signal, abortSignalReason));
                }, out _unregisterAbort);

                _handle = _scheduler.ScheduleInterval(OnTick, TimeSpan.FromMilliseconds(_delayMs));

                if (_closed && _handle != null)
                {
                    _scheduler.CancelInterval(_handle);
                }
            }

            private void OnTick()
            {
                // Match Node's async-generator contract: if a consumer is already awaiting next(),
                // fulfill it immediately; otherwise count the elapsed interval so a later next()
                // can synchronously drain queued ticks one by one.
                if (_closed)
                {
                    return;
                }

                if (_pending.Count > 0)
                {
                    ResolveDeferred(_pending.Dequeue(), IteratorResult.Create(_value, done: false));
                    return;
                }

                _notYielded++;
            }

            private void AbortWithError(object? error)
            {
                if (_closed)
                {
                    return;
                }

                _terminalError = error;
                Close();

                while (_pending.Count > 0)
                {
                    RejectDeferred(_pending.Dequeue(), error);
                }
            }

            private void Close()
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;
                _notYielded = 0;
                _unregisterAbort();

                if (_handle != null)
                {
                    _scheduler.CancelInterval(_handle);
                    _handle = null;
                }
            }
        }
    }
}
