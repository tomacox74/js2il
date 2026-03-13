using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("WeakSet")]
    public sealed class WeakSet
    {
        // Use ConditionalWeakTable with a dummy value to track membership
        // The presence of a key in the table indicates it's in the set
        private readonly ConditionalWeakTable<object, object> _table = new ConditionalWeakTable<object, object>();
        private static readonly object _dummyValue = new object();

        public WeakSet() { }

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
    }
}
