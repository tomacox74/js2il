using System;
using System.Dynamic;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node
{
    [NodeModule("timers/promises")]
    public sealed class TimersPromises
    {
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
            Action unregisterAbort = static () => { };
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
                RejectDeferred(deferred, abortSignalReason is null or JsNull
                    ? GetAbortReason(signal) ?? new AbortError()
                    : CreateAbortError(abortSignalReason));
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
            Action unregisterAbort = static () => { };
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
                RejectDeferred(deferred, abortSignalReason is null or JsNull
                    ? GetAbortReason(signal) ?? new AbortError()
                    : CreateAbortError(abortSignalReason));
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
            _ = delay;
            _ = value;
            _ = options;
            return (Promise)Promise.reject(new Error("node:timers/promises.setInterval is not supported yet."))!;
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

        private static bool TryRegisterAbortListener(object? signal, Action<object?> abortListener, out Action unregister)
        {
            unregister = static () => { };

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
    }
}
