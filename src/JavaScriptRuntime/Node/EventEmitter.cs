using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime.Node
{
    public class EventEmitter
    {
        private readonly Dictionary<string, List<object?>> _listeners = new(StringComparer.Ordinal);
        private readonly Dictionary<object, object?> _onceListenerOriginals =
            new(ReferenceEqualityComparer.Instance);
        private double _maxListeners = 10;

        public EventEmitter on(object? eventName, object? listener)
        {
            if (!CallableOperations.IsCallable(listener))
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var key = GetEventKey(eventName);
            if (!_listeners.TryGetValue(key, out var handlers))
            {
                handlers = new List<object?>();
                _listeners[key] = handlers;
            }

            handlers.Add(listener);
            return this;
        }

        public EventEmitter addListener(object? eventName, object? listener)
            => on(eventName, listener);

        public EventEmitter once(object? eventName, object? listener)
        {
            if (!CallableOperations.IsCallable(listener))
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var emitter = this;
            var fired = false;
            Func<object[], object?[], object?>? wrapper = null;
            wrapper = (scopes, args) =>
            {
                if (fired)
                {
                    return null;
                }

                fired = true;
                emitter.off(eventName, wrapper);
                return InvokeListener(listener, args);
            };

            ObjectRuntime.SetProperty(wrapper, "listener", listener);
            _onceListenerOriginals[wrapper] = listener;
            return on(eventName, wrapper);
        }

        public EventEmitter off(object? eventName, object? listener)
        {
            if (!CallableOperations.IsCallable(listener))
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var key = GetEventKey(eventName);
            if (!_listeners.TryGetValue(key, out var handlers) || handlers.Count == 0)
            {
                return this;
            }

            for (var index = handlers.Count - 1; index >= 0; index--)
            {
                var handler = handlers[index];
                if (!ReferenceEquals(handler, listener)
                    && (handler == null
                        || !_onceListenerOriginals.TryGetValue(handler, out var original)
                        || !ReferenceEquals(original, listener)))
                {
                    continue;
                }

                handlers.RemoveAt(index);
                if (handler != null)
                {
                    _onceListenerOriginals.Remove(handler);
                }
                break;
            }

            if (handlers.Count == 0)
            {
                _listeners.Remove(key);
            }

            return this;
        }

        public EventEmitter removeListener(object? eventName, object? listener)
            => off(eventName, listener);

        public EventEmitter removeAllListeners()
        {
            _listeners.Clear();
            _onceListenerOriginals.Clear();
            return this;
        }

        public EventEmitter removeAllListeners(object? eventName)
        {
            var key = GetEventKey(eventName);
            if (_listeners.Remove(key, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    if (handler != null)
                    {
                        _onceListenerOriginals.Remove(handler);
                    }
                }
            }
            return this;
        }

        private bool EmitCore(object? eventName, in JsCallArguments args)
        {
            var key = GetEventKey(eventName);

            // Node-style special handling for 'error' events:
            // - invoke errorMonitor listeners first
            // - if no normal 'error' listeners are present, throw the error value
            if (string.Equals(key, "error", StringComparison.Ordinal))
            {
                var monitorKey = GetEventKey(Events.ErrorMonitorSymbol);
                if (_listeners.TryGetValue(monitorKey, out var monitorHandlers) && monitorHandlers.Count > 0)
                {
                    var monitorSnapshot = monitorHandlers.ToArray();
                    foreach (var monitorHandler in monitorSnapshot)
                    {
                        _ = InvokeListener(monitorHandler, args);
                    }
                }
            }

            if (!_listeners.TryGetValue(key, out var handlers) || handlers.Count == 0)
            {
                if (string.Equals(key, "error", StringComparison.Ordinal))
                {
                    var reason = args.Count > 0 ? args.GetArgument(0) : new Error("Unhandled error event");
                    if (reason is Exception ex)
                    {
                        throw ex;
                    }

                    throw new JsThrownValueException(reason);
                }

                return false;
            }

            var snapshot = handlers.ToArray();
            foreach (var handler in snapshot)
            {
                _ = InvokeListener(handler, args);
            }

            return true;
        }

        public bool emit(object? eventName)
        {
            var args = JsCallArguments.Empty;
            return EmitCore(eventName, args);
        }

        public bool emit(object? eventName, object? arg0)
        {
            var args = JsCallArguments.From(arg0);
            return EmitCore(eventName, args);
        }

        public bool emit(object? eventName, object? arg0, object? arg1)
        {
            var args = JsCallArguments.From(arg0, arg1);
            return EmitCore(eventName, args);
        }

        public bool emit(object? eventName, object? arg0, object? arg1, object? arg2)
        {
            var args = JsCallArguments.From(arg0, arg1, arg2);
            return EmitCore(eventName, args);
        }

        public bool emit(object? eventName, object? arg0, object? arg1, object? arg2, object? arg3)
        {
            var args = JsCallArguments.From(arg0, arg1, arg2, arg3);
            return EmitCore(eventName, args);
        }

        public double listenerCount(object? eventName)
        {
            var key = GetEventKey(eventName);
            if (_listeners.TryGetValue(key, out var handlers))
            {
                return handlers.Count;
            }

            return 0;
        }

        public object?[] eventNames()
        {
            var names = new List<object?>();
            foreach (var key in _listeners.Keys)
            {
                names.Add(key);
            }
            return names.ToArray();
        }

        public object?[] listeners(object? eventName)
        {
            var key = GetEventKey(eventName);
            if (_listeners.TryGetValue(key, out var handlers))
            {
                var listeners = new object?[handlers.Count];
                for (var index = 0; index < handlers.Count; index++)
                {
                    var handler = handlers[index];
                    listeners[index] = handler != null
                        && _onceListenerOriginals.TryGetValue(handler, out var original)
                            ? original
                            : handler;
                }
                return listeners;
            }
            return System.Array.Empty<object?>();
        }

        public object?[] rawListeners(object? eventName)
        {
            var key = GetEventKey(eventName);
            return _listeners.TryGetValue(key, out var handlers)
                ? handlers.ToArray()
                : System.Array.Empty<object?>();
        }

        public EventEmitter prependListener(object? eventName, object? listener)
        {
            if (!CallableOperations.IsCallable(listener))
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var key = GetEventKey(eventName);
            if (!_listeners.TryGetValue(key, out var handlers))
            {
                handlers = new List<object?>();
                _listeners[key] = handlers;
            }

            handlers.Insert(0, listener);
            return this;
        }

        public EventEmitter prependOnceListener(object? eventName, object? listener)
        {
            if (!CallableOperations.IsCallable(listener))
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var emitter = this;
            var fired = false;
            Func<object[], object?[], object?>? wrapper = null;
            wrapper = (scopes, args) =>
            {
                if (fired)
                {
                    return null;
                }

                fired = true;
                emitter.off(eventName, wrapper);
                return InvokeListener(listener, args);
            };

            ObjectRuntime.SetProperty(wrapper, "listener", listener);
            _onceListenerOriginals[wrapper] = listener;
            return prependListener(eventName, wrapper);
        }

        public EventEmitter setMaxListeners(object? n)
        {
            var value = TypeUtilities.ToNumber(n);
            
            if (double.IsNaN(value) || value < 0)
            {
                throw new RangeError("The value of \"n\" is out of range. It must be a non-negative number.");
            }
            
            _maxListeners = value;
            return this;
        }

        public double getMaxListeners()
        {
            return _maxListeners;
        }

        private object? InvokeListener(object? listener, object?[] args)
        {
            var callArguments = JsCallArguments.FromArray(args);
            return InvokeListener(listener, callArguments);
        }

        private object? InvokeListener(object? listener, in JsCallArguments args)
            => CallableOperations.Call(listener, this, args);

        private static string GetEventKey(object? eventName)
        {
            if (eventName == null || eventName is JsNull)
            {
                return string.Empty;
            }

            return DotNet2JSConversions.ToString(eventName);
        }
    }
}
