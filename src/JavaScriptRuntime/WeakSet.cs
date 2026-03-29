using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakSet")]
    public sealed class WeakSet
    {
        internal static readonly object Prototype = CreatePrototype();
        // Use ConditionalWeakTable with a dummy value to track membership
        // The presence of a key in the table indicates it's in the set
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();
        private static readonly object _dummyValue = new object();

        public WeakSet()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public object add(object? value)
        {
            if (value == null)
            {
                throw new Error("WeakSet value must be an object");
            }

            // In JavaScript, WeakSet values must be objects (not primitives)
            _table.AddOrUpdate(value, _dummyValue);
            return this;
        }

        public bool has(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return _table.TryGetValue(value, out _);
        }

        public bool delete(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return _table.Remove(value);
        }

        private static object CreatePrototype()
        {
            var prototype = new JsObject();
            DefinePrototypeMethod(prototype, "add", PrototypeAdd);
            DefinePrototypeMethod(prototype, "delete", PrototypeDelete);
            DefinePrototypeMethod(prototype, "has", PrototypeHas);
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

        private static WeakSet GetThisWeakSet(string memberName)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is not WeakSet weakSet)
            {
                throw new TypeError($"WeakSet.prototype.{memberName} called on non-WeakSet");
            }

            return weakSet;
        }

        private static object? PrototypeAdd(object[] scopes, object?[]? args)
        {
            var weakSet = GetThisWeakSet("add");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return weakSet.add(value);
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var weakSet = GetThisWeakSet("delete");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return weakSet.delete(value);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var weakSet = GetThisWeakSet("has");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return weakSet.has(value);
        }
    }
}
