using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Set")]
    public sealed class Set : IEnumerable<object>
    {
        internal static readonly object Prototype = CreatePrototype();
        private readonly List<object> _items = new List<object>();
        private readonly HashSet<object> _set = new HashSet<object>();

        public Set()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        // JavaScript Set.prototype.size property
        public double size
        {
            get { return _items.Count; }
        }

        public object add(object? value)
        {
            var v = value!; // JS allows undefined/null; store as null reference
            if (!_set.Contains(v))
            {
                _set.Add(v);
                _items.Add(v);
            }
            return this;
        }

        public object has(object? value)
        {
            return _set.Contains(value!);
        }

        public IEnumerator<object> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        private static object CreatePrototype()
        {
            var prototype = new JsObject();
            DefinePrototypeMethod(prototype, "add", PrototypeAdd);
            DefinePrototypeMethod(prototype, "has", PrototypeHas);
            PropertyDescriptorStore.DefineOrUpdate(prototype, "size", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = (Func<object[], object?[]?, object?>)PrototypeSizeGetter
            });
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

        private static Set GetThisSet(string memberName)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is not Set set)
            {
                throw new TypeError($"Set.prototype.{memberName} called on non-Set");
            }

            return set;
        }

        private static object? PrototypeAdd(object[] scopes, object?[]? args)
        {
            var set = GetThisSet("add");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return set.add(value);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var set = GetThisSet("has");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return set.has(value);
        }

        private static object? PrototypeSizeGetter(object[] scopes, object?[]? args)
            => GetThisSet("size").size;
    }
}
