using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime.Node
{
    public class EventEmitter
    {
        private readonly Dictionary<string, List<object?>> _listeners = new(StringComparer.Ordinal);
        private double _maxListeners = 10;

        public EventEmitter on(object? eventName, object? listener)
        {
            if (listener is not Delegate)
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
            if (listener is not Delegate)
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var emitter = this;
            Func<object[], object?[], object?>? wrapper = null;
            wrapper = (scopes, args) =>
            {
                emitter.off(eventName, wrapper);
                return InvokeListener(listener, args);
            };

            return on(eventName, wrapper);
        }

        public EventEmitter off(object? eventName, object? listener)
        {
            var key = GetEventKey(eventName);
            if (!_listeners.TryGetValue(key, out var handlers) || handlers.Count == 0)
            {
                return this;
            }

            handlers.RemoveAll(h => ReferenceEquals(h, listener));
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
            return this;
        }

        public EventEmitter removeAllListeners(object? eventName)
        {
            var key = GetEventKey(eventName);
            _listeners.Remove(key);
            return this;
        }

        private bool EmitCore(object? eventName, object?[] args)
        {
            var key = GetEventKey(eventName);
            if (!_listeners.TryGetValue(key, out var handlers) || handlers.Count == 0)
            {
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
            => EmitCore(eventName, System.Array.Empty<object?>());

        public bool emit(object? eventName, object? arg0)
            => EmitCore(eventName, new object?[] { arg0 });

        public bool emit(object? eventName, object? arg0, object? arg1)
            => EmitCore(eventName, new object?[] { arg0, arg1 });

        public bool emit(object? eventName, object? arg0, object? arg1, object? arg2)
            => EmitCore(eventName, new object?[] { arg0, arg1, arg2 });

        public bool emit(object? eventName, object? arg0, object? arg1, object? arg2, object? arg3)
            => EmitCore(eventName, new object?[] { arg0, arg1, arg2, arg3 });

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
                if (string.IsNullOrEmpty(key))
                {
                    names.Add(null);
                }
                else
                {
                    names.Add(key);
                }
            }
            return names.ToArray();
        }

        public object?[] listeners(object? eventName)
        {
            var key = GetEventKey(eventName);
            if (_listeners.TryGetValue(key, out var handlers))
            {
                return handlers.ToArray();
            }
            return System.Array.Empty<object?>();
        }

        public object?[] rawListeners(object? eventName)
        {
            // For now, rawListeners is the same as listeners
            // In full Node.js implementation, this would return wrapper functions for once listeners
            return listeners(eventName);
        }

        public EventEmitter prependListener(object? eventName, object? listener)
        {
            if (listener is not Delegate)
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
            if (listener is not Delegate)
            {
                throw new TypeError("EventEmitter listener must be a function");
            }

            var emitter = this;
            Func<object[], object?[], object?>? wrapper = null;
            wrapper = (scopes, args) =>
            {
                emitter.off(eventName, wrapper);
                return InvokeListener(listener, args);
            };

            return prependListener(eventName, wrapper);
        }

        public EventEmitter setMaxListeners(object? n)
        {
            if (n is double d)
            {
                _maxListeners = d;
            }
            return this;
        }

        public double getMaxListeners()
        {
            return _maxListeners;
        }

        private object? InvokeListener(object? listener, object?[] args)
        {
            if (listener is not Delegate)
            {
                return null;
            }

            var previousThis = RuntimeServices.SetCurrentThis(this);
            try
            {
                return Closure.InvokeWithArgs(listener, System.Array.Empty<object>(), args);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

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
