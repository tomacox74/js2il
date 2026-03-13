using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Set")]
    public sealed class Set : IEnumerable<object>
    {
        private readonly List<object> _items = new List<object>();
        private readonly HashSet<object> _set = new HashSet<object>();

        public Set() { }

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
    }
}
