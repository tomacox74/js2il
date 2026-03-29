using System;
using System.Collections.Generic;
using System.Dynamic;

namespace JavaScriptRuntime
{
    // Minimal Proxy implementation supporting the currently implemented trap surface.
    [IntrinsicObject("Proxy")]
    public sealed class Proxy
    {
        private object? _target;
        private object? _handler;

        public Proxy(object target, object handler)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        internal object Target => _target!;

        internal object Handler => _handler!;

        internal object GetTarget(string operation)
        {
            EnsureNotRevoked(operation);
            return _target!;
        }

        internal void EnsureNotRevoked(string operation)
        {
            if (_target is null || _handler is null)
            {
                throw new TypeError($"Cannot perform '{operation}' on a proxy that has been revoked");
            }
        }

        internal bool TryInvokeTrap(string trapName, string operation, object?[] args, out object? result)
        {
            EnsureNotRevoked(operation);

            var trap = ObjectRuntime.GetProperty(_handler!, trapName);
            if (trap is null || trap is JsNull)
            {
                result = null;
                return false;
            }

            var previousThis = RuntimeServices.SetCurrentThis(_handler);
            try
            {
                result = Closure.InvokeWithArgs(trap, Array.Empty<object>(), args);
                return true;
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        internal void Revoke()
        {
            _target = null;
            _handler = null;
        }

        public static object revocable(object target, object handler)
        {
            var proxy = new Proxy(target, handler);
            var result = new ExpandoObject();
            var dict = (IDictionary<string, object?>)result;
            dict["proxy"] = proxy;
            dict["revoke"] = (Func<object[], object?[]?, object?>)((_, __) =>
            {
                proxy.Revoke();
                return null;
            });
            return result;
        }
    }
}
