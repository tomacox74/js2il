using System;
using System.Collections.Generic;

namespace JavaScriptRuntime.Node
{
    [NodeModule("events")]
    public sealed class Events
    {
        internal static readonly Symbol ErrorMonitorSymbol = new("events.errorMonitor");

        public Type EventEmitter => typeof(EventEmitter);
        public Symbol errorMonitor => ErrorMonitorSymbol;

        public object on(object? emitter, object? eventName)
        {
            return on(emitter, eventName, null);
        }

        public object on(object? emitter, object? eventName, object? options)
        {
            if (emitter is not EventEmitter eventEmitter)
            {
                throw new TypeError("events.on only supports EventEmitter in this runtime");
            }

            return new EventEmitterAsyncOnIterator(eventEmitter, eventName);
        }

        public object once(object? emitter, object? eventName)
        {
            return once(emitter, eventName, null);
        }

        public object once(object? emitter, object? eventName, object? options)
        {
            if (emitter is not EventEmitter eventEmitter)
            {
                throw new TypeError("events.once only supports EventEmitter in this runtime");
            }

            var deferred = Promise.withResolvers();
            var observeError = !IsErrorEvent(eventName);

            Func<object[], object?[], object?>? eventHandler = null;
            Func<object[], object?[], object?>? errorHandler = null;

            eventHandler = (_, args) =>
            {
                eventEmitter.off(eventName, eventHandler);
                if (observeError)
                {
                    eventEmitter.off("error", errorHandler);
                }

                ResolveDeferred(deferred, new JavaScriptRuntime.Array(args ?? System.Array.Empty<object?>()));
                return null;
            };

            if (observeError)
            {
                errorHandler = (_, args) =>
                {
                    eventEmitter.off(eventName, eventHandler);
                    eventEmitter.off("error", errorHandler);

                    RejectDeferred(deferred, FirstArgOrDefaultError(args));
                    return null;
                };

                eventEmitter.on("error", errorHandler);
            }

            eventEmitter.on(eventName, eventHandler);
            return deferred.promise;
        }

        private static bool IsErrorEvent(object? eventName)
        {
            return string.Equals(NormalizeEventKey(eventName), "error", StringComparison.Ordinal);
        }

        private static string NormalizeEventKey(object? eventName)
        {
            if (eventName == null || eventName is JsNull)
            {
                return string.Empty;
            }

            return DotNet2JSConversions.ToString(eventName);
        }

        private static object? FirstArgOrDefaultError(object?[]? args)
        {
            if (args != null && args.Length > 0)
            {
                return args[0];
            }

            return new Error("Unhandled error event");
        }

        private static void ResolveDeferred(PromiseWithResolvers deferred, object? value)
        {
            if (deferred.resolve is Delegate resolve)
            {
                Closure.InvokeWithArgs(resolve, System.Array.Empty<object>(), new object?[] { value });
            }
        }

        private static void RejectDeferred(PromiseWithResolvers deferred, object? reason)
        {
            if (deferred.reject is Delegate reject)
            {
                Closure.InvokeWithArgs(reject, System.Array.Empty<object>(), new object?[] { reason });
            }
        }

        private sealed class EventEmitterAsyncOnIterator : IJavaScriptAsyncIterator
        {
            private readonly EventEmitter _emitter;
            private readonly object? _eventName;
            private readonly bool _observeError;
            private readonly Queue<JavaScriptRuntime.Array> _queuedValues = new();
            private readonly Queue<PromiseWithResolvers> _pending = new();
            private readonly Func<object[], object?[], object?> _eventHandler;
            private readonly Func<object[], object?[], object?>? _errorHandler;

            private object? _terminalError;
            private bool _closed;

            public EventEmitterAsyncOnIterator(EventEmitter emitter, object? eventName)
            {
                _emitter = emitter;
                _eventName = eventName;
                _observeError = !IsErrorEvent(eventName);

                _eventHandler = (_, args) =>
                {
                    if (_closed)
                    {
                        return null;
                    }

                    var payload = new JavaScriptRuntime.Array(args ?? System.Array.Empty<object?>());

                    if (_pending.Count > 0)
                    {
                        var deferred = _pending.Dequeue();
                        ResolveDeferred(deferred, IteratorResult.Create(payload, done: false));
                    }
                    else
                    {
                        _queuedValues.Enqueue(payload);
                    }

                    return null;
                };

                if (_observeError)
                {
                    _errorHandler = (_, args) =>
                    {
                        if (_closed)
                        {
                            return null;
                        }

                        _terminalError = FirstArgOrDefaultError(args);
                        Close();

                        while (_pending.Count > 0)
                        {
                            var deferred = _pending.Dequeue();
                            RejectDeferred(deferred, _terminalError);
                        }

                        return null;
                    };

                    _emitter.on("error", _errorHandler);
                }

                _emitter.on(_eventName, _eventHandler);
            }

            public bool HasReturn => true;

            public object? Next()
            {
                if (_terminalError != null)
                {
                    var reason = _terminalError;
                    _terminalError = null;
                    return Promise.reject(reason);
                }

                if (_queuedValues.Count > 0)
                {
                    return Promise.resolve(IteratorResult.Create(_queuedValues.Dequeue(), done: false));
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
                Close();

                while (_pending.Count > 0)
                {
                    var deferred = _pending.Dequeue();
                    ResolveDeferred(deferred, IteratorResult.Create(null, done: true));
                }

                return Promise.resolve(IteratorResult.Create(null, done: true));
            }

            private void Close()
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;
                _emitter.off(_eventName, _eventHandler);

                if (_observeError)
                {
                    _emitter.off("error", _errorHandler);
                }
            }
        }
    }
}
