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

        internal static bool IsObjectLikeValue(object? value)
        {
            if (value is null || value is JsNull)
            {
                return false;
            }

            var valueType = TypeUtilities.Typeof(value);
            return valueType == "object" || valueType == "function";
        }

        internal static bool IsCallableValue(object? value)
        {
            if (value is JavaScriptRuntime.Proxy proxy)
            {
                return IsCallableValue(proxy.GetTarget("apply"));
            }

            return value is Delegate;
        }

        public Proxy(object target, object handler)
        {
            if (!IsObjectLikeValue(target))
            {
                throw new TypeError("Proxy target must be an object");
            }

            if (!IsObjectLikeValue(handler))
            {
                throw new TypeError("Proxy handler must be an object");
            }

            _target = target;
            _handler = handler;
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

            var handler = _handler;
            if (handler is null)
            {
                throw new TypeError($"Cannot perform '{operation}' on a proxy that has been revoked");
            }

            var trap = ObjectRuntime.GetProperty(handler, trapName);
            if (trap is null || trap is JsNull)
            {
                result = null;
                return false;
            }

            var previousThis = RuntimeServices.SetCurrentThis(handler);
            try
            {
                result = Closure.InvokeWithArgs(trap, System.Array.Empty<object>(), args);
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
            dict["revoke"] = (Func<object[], object?[]?, object?>)((object[] scopes, object?[]? args) =>
            {
                proxy.Revoke();
                return null;
            });
            return result;
        }
    }
}
