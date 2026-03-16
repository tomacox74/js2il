using System;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    public sealed class AbortController
    {
        public AbortController()
        {
            signal = new AbortSignal();
        }

        public AbortSignal signal { get; }

        public object? abort(object? reason = null)
        {
            signal.Abort(reason);
            return null;
        }
    }

    public sealed class AbortSignal
    {
        private readonly object _syncRoot = new();
        private readonly List<Delegate> _eventListeners = new();
        private readonly List<Action<object?>> _internalListeners = new();

        public bool aborted { get; private set; }

        public object? reason { get; private set; }

        public object? addEventListener(object? eventName, object? listener, object? options = null)
        {
            _ = options;

            if (!IsAbortEventName(eventName) || listener is not Delegate del)
            {
                return null;
            }

            lock (_syncRoot)
            {
                if (!aborted && !_eventListeners.Contains(del))
                {
                    _eventListeners.Add(del);
                }
            }

            return null;
        }

        public object? removeEventListener(object? eventName, object? listener, object? options = null)
        {
            _ = options;

            if (!IsAbortEventName(eventName) || listener is not Delegate del)
            {
                return null;
            }

            lock (_syncRoot)
            {
                _eventListeners.Remove(del);
            }

            return null;
        }

        internal bool TryRegisterInternalListener(Action<object?> listener, out Action unregister)
        {
            lock (_syncRoot)
            {
                if (aborted)
                {
                    unregister = static () => { };
                    return false;
                }

                _internalListeners.Add(listener);
            }

            unregister = () =>
            {
                lock (_syncRoot)
                {
                    _internalListeners.Remove(listener);
                }
            };

            return true;
        }

        internal void Abort(object? abortReason = null)
        {
            Delegate[] listeners;
            Action<object?>[] internalListeners;
            object? resolvedReason;

            lock (_syncRoot)
            {
                if (aborted)
                {
                    return;
                }

                aborted = true;
                reason = abortReason ?? new AbortError("This operation was aborted");
                resolvedReason = reason;
                listeners = _eventListeners.ToArray();
                internalListeners = _internalListeners.ToArray();
                _eventListeners.Clear();
                _internalListeners.Clear();
            }

            foreach (var listener in listeners)
            {
                Closure.InvokeWithArgs(listener, System.Array.Empty<object>(), System.Array.Empty<object>());
            }

            foreach (var listener in internalListeners)
            {
                listener(resolvedReason);
            }
        }

        private static bool IsAbortEventName(object? eventName)
        {
            return string.Equals(DotNet2JSConversions.ToString(eventName), "abort", StringComparison.Ordinal);
        }
    }

    public sealed class AbortError : Error
    {
        public AbortError() : this("The operation was aborted")
        {
        }

        public AbortError(string? message) : base(message)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, object? cause) : base(message, cause)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, Exception? innerException) : base(message, innerException)
        {
            Name = "AbortError";
        }

        public AbortError(string? message, Exception? innerException, object? cause) : base(message, innerException, cause)
        {
            Name = "AbortError";
        }

        public string code => "ABORT_ERR";
    }
}
