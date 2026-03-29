using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakMap")]
    public sealed class WeakMap
    {
        internal static readonly object Prototype = CreatePrototype();
        // ConditionalWeakTable allows keys to be garbage collected when no other references exist
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();

        public WeakMap()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public object set(object? key, object? value)
        {
            if (key == null)
            {
                throw new Error("WeakMap key must be an object");
            }

            // In JavaScript, WeakMap keys must be objects (not primitives)
            // For simplicity, we allow any non-null reference type
            _table.AddOrUpdate(key, value!);
            return this;
        }

        public object? get(object? key)
        {
            if (key == null)
            {
                return null; // JavaScript undefined
            }

            if (_table.TryGetValue(key, out var value))
            {
                return value;
            }
            return null; // JavaScript undefined
        }

        public bool has(object? key)
        {
            if (key == null)
            {
                return false;
            }

            return _table.TryGetValue(key, out _);
        }

        public bool delete(object? key)
        {
            if (key == null)
            {
                return false;
            }

            return _table.Remove(key);
        }

        private static object CreatePrototype()
        {
            var prototype = new JsObject();
            DefinePrototypeMethod(prototype, "delete", PrototypeDelete);
            DefinePrototypeMethod(prototype, "get", PrototypeGet);
            DefinePrototypeMethod(prototype, "has", PrototypeHas);
            DefinePrototypeMethod(prototype, "set", PrototypeSet);
            return prototype;
        }

        private static void DefinePrototypeMethod(object prototype, string name, Func<object[], object?[]?, object?> method)
        {
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = method
            });
        }

        private static WeakMap GetThisWeakMap(string memberName)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is not WeakMap weakMap)
            {
                throw new TypeError($"WeakMap.prototype.{memberName} called on non-WeakMap");
            }

            return weakMap;
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("delete");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.delete(key);
        }

        private static object? PrototypeGet(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("get");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.get(key);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("has");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return weakMap.has(key);
        }

        private static object? PrototypeSet(object[] scopes, object?[]? args)
        {
            var weakMap = GetThisWeakMap("set");
            var key = args != null && args.Length > 0 ? args[0] : null;
            var value = args != null && args.Length > 1 ? args[1] : null;
            return weakMap.set(key, value);
        }
    }
}
