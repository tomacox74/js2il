using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Set")]
    public sealed class Set : IEnumerable<object>
    {
        private static readonly Func<object[], object?[]?, object?> _prototypeValuesValue = PrototypeValues;
        internal static readonly ExpandoObject Prototype = CreatePrototype();
        private readonly List<object> _items = new List<object>();
        private readonly HashSet<object> _set = new HashSet<object>();

        private static ExpandoObject CreatePrototype()
        {
            var exp = new ExpandoObject();
            DefinePrototypeMethod(exp, "add", PrototypeAdd);
            DefinePrototypeMethod(exp, "has", PrototypeHas);
            DefinePrototypeMethod(exp, "delete", PrototypeDelete);
            DefinePrototypeMethod(exp, "clear", PrototypeClear);
            DefinePrototypeMethod(exp, "entries", PrototypeEntries);
            DefinePrototypeMethod(exp, "forEach", PrototypeForEach);
            DefinePrototypeMethod(exp, "keys", _prototypeValuesValue);
            DefinePrototypeMethod(exp, "values", _prototypeValuesValue);
            DefinePrototypeMethod(exp, "difference", PrototypeDifference);
            DefinePrototypeMethod(exp, "intersection", PrototypeIntersection);
            DefinePrototypeMethod(exp, "isDisjointFrom", PrototypeIsDisjointFrom);
            DefinePrototypeMethod(exp, "isSubsetOf", PrototypeIsSubsetOf);
            DefinePrototypeMethod(exp, "isSupersetOf", PrototypeIsSupersetOf);
            DefinePrototypeMethod(exp, "symmetricDifference", PrototypeSymmetricDifference);
            DefinePrototypeMethod(exp, "union", PrototypeUnion);
            PropertyDescriptorStore.DefineOrUpdate(exp, Symbol.iterator.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _prototypeValuesValue
            });
            PropertyDescriptorStore.DefineOrUpdate(exp, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Set"
            });
            return exp;
        }

        private static void DefinePrototypeMethod(ExpandoObject prototype, string name, Func<object[], object?[]?, object?> method)
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

        private static Set GetSetReceiver(string methodName)
        {
            var receiver = RuntimeServices.GetCurrentThis();
            if (receiver is not Set set)
            {
                throw new TypeError($"Set.prototype.{methodName} called on incompatible receiver");
            }

            return set;
        }

        private static object? PrototypeAdd(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("add");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return set.add(value);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("has");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return set.has(value);
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("delete");
            var value = args != null && args.Length > 0 ? args[0] : null;
            return set.delete(value);
        }

        private static object? PrototypeClear(object[] scopes, object?[]? args)
        {
            GetSetReceiver("clear").clear();
            return null;
        }

        private static object? PrototypeEntries(object[] scopes, object?[]? args)
        {
            return GetSetReceiver("entries").entries();
        }

        private static object? PrototypeValues(object[] scopes, object?[]? args)
        {
            return GetSetReceiver("values").values();
        }

        private static object? PrototypeForEach(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("forEach");
            var callback = args != null && args.Length > 0 ? args[0] : null;
            var thisArg = args != null && args.Length > 1 ? args[1] : null;
            set.forEach(callback, thisArg);
            return null;
        }

        private static object? PrototypeDifference(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("difference");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.difference(other);
        }

        private static object? PrototypeIntersection(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("intersection");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.intersection(other);
        }

        private static object? PrototypeIsDisjointFrom(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("isDisjointFrom");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.isDisjointFrom(other);
        }

        private static object? PrototypeIsSubsetOf(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("isSubsetOf");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.isSubsetOf(other);
        }

        private static object? PrototypeIsSupersetOf(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("isSupersetOf");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.isSupersetOf(other);
        }

        private static object? PrototypeSymmetricDifference(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("symmetricDifference");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.symmetricDifference(other);
        }

        private static object? PrototypeUnion(object[] scopes, object?[]? args)
        {
            var set = GetSetReceiver("union");
            var other = args != null && args.Length > 0 ? args[0] : null;
            return set.union(other);
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public Set()
        {
            InitializeIntrinsicSurface();
        }

        public Set(object? iterable)
        {
            InitializeIntrinsicSurface();
            if (iterable is null || iterable is JsNull)
            {
                return;
            }

            AddValuesFromIterable(iterable);
        }

        private void AddValuesFromIterable(object iterable)
        {
            var iterator = ObjectRuntime.GetIterator(iterable);
            try
            {
                while (true)
                {
                    var step = JavaScriptRuntime.Object.IteratorNext(iterator);
                    if (JavaScriptRuntime.Object.IteratorResultDone(step))
                    {
                        break;
                    }

                    add(JavaScriptRuntime.Object.IteratorResultValue(step));
                }
            }
            finally
            {
                JavaScriptRuntime.Object.IteratorClose(iterator);
            }
        }

        private static Set NormalizeOtherSet(object? other, string methodName)
        {
            if (other is null || other is JsNull)
            {
                throw new TypeError($"Set.prototype.{methodName} called with null or undefined other");
            }

            return other as Set ?? new Set(other);
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

        public bool delete(object? value)
        {
            var v = value!;
            if (!_set.Remove(v))
            {
                return false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (Equals(_items[i], v))
                {
                    _items.RemoveAt(i);
                    break;
                }
            }

            return true;
        }

        public void clear()
        {
            _set.Clear();
            _items.Clear();
        }

        public void forEach(object? callback)
        {
            forEach(callback, null);
        }

        public void forEach(object? callback, object? thisArg)
        {
            if (callback is not Delegate del)
            {
                throw new TypeError("Set.prototype.forEach callback must be a function");
            }

            for (int i = 0; i < _items.Count; i++)
            {
                var value = _items[i];
                var previousThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    Closure.InvokeWithArgs(del, System.Array.Empty<object>(), new object?[] { value, value, this });
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }
        }

        public IJavaScriptIterator values() => new SetIterator(this, SetIteratorKind.Values);

        public IJavaScriptIterator keys() => new SetIterator(this, SetIteratorKind.Values);

        public IJavaScriptIterator entries() => new SetIterator(this, SetIteratorKind.Entries);

        public Set difference(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(difference));
            var result = new Set();
            foreach (var value in _items)
            {
                if (!otherSet._set.Contains(value))
                {
                    result.add(value);
                }
            }

            return result;
        }

        public Set intersection(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(intersection));
            var result = new Set();
            foreach (var value in _items)
            {
                if (otherSet._set.Contains(value))
                {
                    result.add(value);
                }
            }

            return result;
        }

        public bool isDisjointFrom(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(isDisjointFrom));
            foreach (var value in _items)
            {
                if (otherSet._set.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        public bool isSubsetOf(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(isSubsetOf));
            foreach (var value in _items)
            {
                if (!otherSet._set.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        public bool isSupersetOf(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(isSupersetOf));
            foreach (var value in otherSet._items)
            {
                if (!_set.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        public Set symmetricDifference(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(symmetricDifference));
            var result = new Set(this);
            foreach (var value in otherSet._items)
            {
                if (result._set.Contains(value))
                {
                    result.delete(value);
                }
                else
                {
                    result.add(value);
                }
            }

            return result;
        }

        public Set union(object? other)
        {
            var otherSet = NormalizeOtherSet(other, nameof(union));
            var result = new Set(this);
            foreach (var value in otherSet._items)
            {
                result.add(value);
            }

            return result;
        }

        public IEnumerator<object> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        private enum SetIteratorKind
        {
            Values,
            Entries
        }

        private sealed class SetIterator : IJavaScriptIterator
        {
            private readonly Set _set;
            private readonly SetIteratorKind _kind;
            private int _index;
            private bool _isClosed;

            public SetIterator(Set set, SetIteratorKind kind)
            {
                _set = set;
                _kind = kind;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed)
                {
                    return new IteratorResultObject(null, done: true);
                }

                if (_index >= _set._items.Count)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var value = _set._items[_index++];
                object? result = _kind == SetIteratorKind.Entries
                    ? new JavaScriptRuntime.Array(new object?[] { value, value })
                    : value;

                return new IteratorResultObject(result, done: false);
            }

            public object next(object? value = null) => Next();

            public void Return()
            {
                _isClosed = true;
            }
        }
    }
}
