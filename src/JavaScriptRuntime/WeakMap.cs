using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakMap")]
    public sealed class WeakMap
    {
        // ConditionalWeakTable allows keys to be garbage collected when no other references exist
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();

        public WeakMap() { }

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
    }
}
