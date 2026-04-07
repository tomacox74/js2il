using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    [NodeModule("stream")]
    public sealed class Stream
    {
        public Type Readable => typeof(Readable);

        public Type Writable => typeof(Writable);

        public Type Duplex => typeof(Duplex);

        public Type Transform => typeof(Transform);

        public Type PassThrough => typeof(PassThrough);

        public object finished(object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new TypeError("stream.finished requires a stream instance.");
            }

            var stream = args[0];
            var callback = TryGetTrailingCallback(args, minimumArgumentCount: 1);
            var options = callback != null && args.Length > 2
                ? args[1]
                : callback == null && args.Length > 1
                    ? args[1]
                    : null;
            var deferred = Promise.withResolvers();
            StreamCompletionObserver? observer = null;
            var settled = false;
            var signal = GetSignal(options);
            Action unregisterAbort = static () => { };

            void ResolveSuccess()
            {
                if (settled)
                {
                    return;
                }

                settled = true;
                observer?.Dispose();
                unregisterAbort();
                if (callback != null)
                {
                    InvokeCallback(callback, JsNull.Null);
                }

                ResolvePromise(deferred, null);
            }

            void ResolveError(object? reason)
            {
                if (settled)
                {
                    return;
                }

                settled = true;
                observer?.Dispose();
                unregisterAbort();
                if (callback != null)
                {
                    InvokeCallback(callback, reason);
                }

                RejectPromise(deferred, reason);
            }

            TryRegisterAbortListener(signal, abortSignalReason =>
            {
                ResolveError(abortSignalReason is null or JsNull
                    ? GetAbortReason(signal) ?? new AbortError()
                    : CreateAbortError(abortSignalReason));
            }, out unregisterAbort);

            var abortReason = GetAbortReason(signal);
            if (abortReason != null)
            {
                ResolveError(abortReason);
                return callback != null ? stream! : deferred.promise;
            }

            observer = new StreamCompletionObserver(stream, ResolveSuccess, ResolveError);
            return callback != null ? stream! : deferred.promise;
        }

        public object pipeline(object[] args)
        {
            if (args == null || args.Length < 2)
            {
                throw new TypeError("stream.pipeline requires at least two streams.");
            }

            var callback = TryGetTrailingCallback(args, minimumArgumentCount: 2);
            var effectiveArgumentCount = callback != null ? args.Length - 1 : args.Length;
            object? options = null;
            if (effectiveArgumentCount > 2)
            {
                var optionsCandidate = args[effectiveArgumentCount - 1];
                if (optionsCandidate is not EventEmitter
                    && optionsCandidate is not Delegate
                    && NodeNetworkingCommon.LooksLikeOptionsObject(optionsCandidate))
                {
                    options = optionsCandidate;
                    effectiveArgumentCount--;
                }
            }

            var streamCount = effectiveArgumentCount;
            if (streamCount < 2)
            {
                throw new TypeError("stream.pipeline requires at least two streams.");
            }

            var streams = new object?[streamCount];
            System.Array.Copy(args, streams, streamCount);

            ValidatePipelineStreams(streams);

            var deferred = Promise.withResolvers();
            var observers = new List<StreamCompletionObserver>(streamCount);
            var settled = false;
            var remaining = streamCount;
            var signal = GetSignal(options);
            Action unregisterAbort = static () => { };

            void CleanupObservers()
            {
                foreach (var observer in observers)
                {
                    observer.Dispose();
                }

                observers.Clear();
                unregisterAbort();
            }

            void ResolveSuccess()
            {
                if (settled)
                {
                    return;
                }

                remaining--;
                if (remaining > 0)
                {
                    return;
                }

                settled = true;
                CleanupObservers();
                if (callback != null)
                {
                    InvokeCallback(callback, JsNull.Null);
                }

                ResolvePromise(deferred, null);
            }

            void ResolveError(object? reason)
            {
                if (settled)
                {
                    return;
                }

                settled = true;

                foreach (var stream in streams)
                {
                    DestroyStream(stream, reason);
                }

                CleanupObservers();

                if (callback != null)
                {
                    InvokeCallback(callback, reason);
                }

                RejectPromise(deferred, reason);
            }

            TryRegisterAbortListener(signal, abortSignalReason =>
            {
                ResolveError(abortSignalReason is null or JsNull
                    ? GetAbortReason(signal) ?? new AbortError()
                    : CreateAbortError(abortSignalReason));
            }, out unregisterAbort);

            var abortReason = GetAbortReason(signal);
            if (abortReason != null)
            {
                ResolveError(abortReason);
                return callback != null ? streams[^1]! : deferred.promise;
            }

            foreach (var stream in streams)
            {
                observers.Add(new StreamCompletionObserver(stream, ResolveSuccess, ResolveError));
            }

            try
            {
                for (var i = 0; i < streams.Length - 1; i++)
                {
                    PipeStreams(streams[i], streams[i + 1]);
                }
            }
            catch (Exception ex)
            {
                ResolveError(ex as Error ?? new Error(ex.Message, ex));
            }

            return callback != null ? streams[^1]! : deferred.promise;
        }

        private static void DestroyStream(object? stream, object? reason)
        {
            if (stream is JavaScriptRuntime.Node.Writable writable)
            {
                if (writable.destroyed)
                {
                    return;
                }

                AttachPipelineErrorGuard(writable, reason);
                writable.destroy(reason);
                return;
            }

            if (stream is JavaScriptRuntime.Node.Readable readable)
            {
                if (readable.destroyed)
                {
                    return;
                }

                AttachPipelineErrorGuard(readable, reason);
                readable.destroy(reason);
                return;
            }
        }

        private static void AttachPipelineErrorGuard(EventEmitter emitter, object? reason)
        {
            if (reason == null || reason is JsNull)
            {
                return;
            }

            Func<object[], object?[], object?> ignoreError = (_, _) => null;
            Func<object[], object?[], object?> removeGuard = (_, _) =>
            {
                emitter.off("error", ignoreError);
                return null;
            };

            emitter.on("error", ignoreError);
            emitter.once("close", removeGuard);
        }

        private static void InvokeCallback(Delegate callback, params object?[] args)
        {
            Closure.InvokeWithArgs(callback, System.Array.Empty<object>(), args);
        }

        private static AbortError CreateAbortError(object? signalReason)
        {
            if (signalReason == null || signalReason is JsNull)
            {
                return new AbortError();
            }

            return new AbortError("The operation was aborted", signalReason);
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

        private static object? GetSignal(object? options)
        {
            if (options == null || options is JsNull)
            {
                return null;
            }

            return ObjectRuntime.GetProperty(options, "signal");
        }

        private static void PipeStreams(object? source, object? destination)
        {
            switch (source)
            {
                case JavaScriptRuntime.Node.Duplex duplex:
                    duplex.pipe(destination);
                    return;
                case JavaScriptRuntime.Node.Readable readable:
                    readable.pipe(destination);
                    return;
                default:
                    throw new TypeError("stream.pipeline only supports Readable and Duplex sources in the current runtime.");
            }
        }

        private static void RejectPromise(PromiseWithResolvers deferred, object? reason)
        {
            if (deferred.reject is Delegate reject)
            {
                Closure.InvokeWithArgs(reject, System.Array.Empty<object>(), new object?[] { reason });
            }
        }

        private static void ResolvePromise(PromiseWithResolvers deferred, object? value)
        {
            if (deferred.resolve is Delegate resolve)
            {
                Closure.InvokeWithArgs(resolve, System.Array.Empty<object>(), new object?[] { value });
            }
        }

        private static Delegate? TryGetTrailingCallback(object[] args, int minimumArgumentCount)
        {
            if (args.Length <= minimumArgumentCount)
            {
                return null;
            }

            return args[^1] as Delegate;
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

        private static void ValidatePipelineStreams(object?[] streams)
        {
            for (var i = 0; i < streams.Length; i++)
            {
                if (streams[i] is not EventEmitter)
                {
                    throw new TypeError("stream.pipeline only supports EventEmitter-backed streams in the current runtime.");
                }

                if (i < streams.Length - 1
                    && streams[i] is not JavaScriptRuntime.Node.Readable
                    && streams[i] is not JavaScriptRuntime.Node.Duplex)
                {
                    throw new TypeError("stream.pipeline sources must be Readable or Duplex streams.");
                }

                if (i > 0 && streams[i] is not JavaScriptRuntime.Node.Writable)
                {
                    throw new TypeError("stream.pipeline destinations must be Writable or Duplex streams.");
                }
            }
        }

        private sealed class StreamCompletionObserver : IDisposable
        {
            private readonly EventEmitter _emitter;
            private readonly Action _onSuccess;
            private readonly Action<object?> _onError;
            private readonly bool _needsEnd;
            private readonly bool _needsFinish;

            private Func<object[], object?[], object?>? _onEnd;
            private Func<object[], object?[], object?>? _onFinish;
            private Func<object[], object?[], object?>? _onErrorEvent;
            private Func<object[], object?[], object?>? _onClose;
            private bool _sawEnd;
            private bool _sawFinish;
            private bool _settled;

            public StreamCompletionObserver(object? stream, Action onSuccess, Action<object?> onError)
            {
                if (stream is not EventEmitter emitter)
                {
                    throw new TypeError("stream helpers only support EventEmitter-backed streams in the current runtime.");
                }

                _emitter = emitter;
                _onSuccess = onSuccess;
                _onError = onError;
                _needsEnd = stream is JavaScriptRuntime.Node.Readable || stream is JavaScriptRuntime.Node.Duplex;
                _needsFinish = stream is JavaScriptRuntime.Node.Writable;
                _sawEnd = !_needsEnd;
                _sawFinish = !_needsFinish;

                if (stream is JavaScriptRuntime.Node.Readable readable && (readable.destroyed || !readable.readable))
                {
                    _sawEnd = !readable.destroyed;
                }

                if (stream is JavaScriptRuntime.Node.Duplex duplex && (duplex.destroyed || !duplex.readable))
                {
                    _sawEnd = !duplex.destroyed;
                }

                if (stream is JavaScriptRuntime.Node.Writable writable)
                {
                    if (writable.destroyed)
                    {
                        Fail(new Error("Premature close"));
                        return;
                    }

                    if (!writable.writable)
                    {
                        _sawFinish = true;
                    }
                }

                AttachHandlers();
                TryComplete();
            }

            public void Dispose()
            {
                Cleanup();
            }

            private void AttachHandlers()
            {
                _onEnd = (_, _) =>
                {
                    _sawEnd = true;
                    TryComplete();
                    return null;
                };

                _onFinish = (_, _) =>
                {
                    _sawFinish = true;
                    TryComplete();
                    return null;
                };

                _onErrorEvent = (_, args) =>
                {
                    Fail(args.Length > 0 ? args[0] : new Error("Unhandled stream error"));
                    return null;
                };

                _onClose = (_, _) =>
                {
                    if (_settled)
                    {
                        return null;
                    }

                    if (_sawEnd && _sawFinish)
                    {
                        return null;
                    }

                    Fail(new Error("Premature close"));
                    return null;
                };

                if (_needsEnd)
                {
                    _emitter.on("end", _onEnd);
                }

                if (_needsFinish)
                {
                    _emitter.on("finish", _onFinish);
                }

                _emitter.on("error", _onErrorEvent);
                _emitter.on("close", _onClose);
            }

            private void Cleanup()
            {
                if (_onEnd != null)
                {
                    _emitter.off("end", _onEnd);
                }

                if (_onFinish != null)
                {
                    _emitter.off("finish", _onFinish);
                }

                if (_onErrorEvent != null)
                {
                    _emitter.off("error", _onErrorEvent);
                }

                if (_onClose != null)
                {
                    _emitter.off("close", _onClose);
                }

                _onEnd = null;
                _onFinish = null;
                _onErrorEvent = null;
                _onClose = null;
            }

            private void Fail(object? reason)
            {
                if (_settled)
                {
                    return;
                }

                _settled = true;
                Cleanup();
                _onError(reason);
            }

            private void TryComplete()
            {
                if (_settled)
                {
                    return;
                }

                if (!_sawEnd || !_sawFinish)
                {
                    return;
                }

                _settled = true;
                Cleanup();
                _onSuccess();
            }
        }
    }
}
