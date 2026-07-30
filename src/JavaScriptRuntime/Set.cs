using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Set")]
    public sealed class Set : IEnumerable<object>
    {
        private static readonly Func<object[], object?[]?, object?> _prototypeValuesValue = PrototypeValues;
        internal static readonly JsObject IteratorPrototype = CreateIteratorPrototype();
        internal static readonly JsObject Prototype = CreatePrototype();
        private readonly List<object> _items = new List<object>();
        private readonly HashSet<object> _set = new HashSet<object>();

        private static JsObject CreatePrototype()
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            var exp = new JsObject();
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
            PropertyDescriptorStore.DefineOrUpdate(exp, "size", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = (Func<object[], object?[]?, object?>)PrototypeSizeGetter
            });
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

        private static JsObject CreateIteratorPrototype()
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            var prototype = new JsObject();
            PrototypeChain.SetPrototype(prototype, Iterator.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Set Iterator"
            });
            return prototype;
        }

        private static void DefinePrototypeMethod(JsObject prototype, string name, Func<object[], object?[]?, object?> method)
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

        private static object? PrototypeSizeGetter(object[] scopes, object?[]? args)
        {
            return GetSetReceiver("size").size;
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
            var adder = GetCallableAdder("add");
            var iterator = ObjectRuntime.GetIterator(iterable);
            var completedNormally = false;
            try
            {
                while (true)
                {
                    var step = JavaScriptRuntime.ObjectRuntime.IteratorNext(iterator);
                    if (JavaScriptRuntime.ObjectRuntime.IteratorResultDone(step))
                    {
                        break;
                    }

                    CallAdder(adder, JavaScriptRuntime.ObjectRuntime.IteratorResultValue(step));
                }

                completedNormally = true;
            }
            finally
            {
                if (!completedNormally)
                {
                    JavaScriptRuntime.ObjectRuntime.IteratorClose(iterator);
                }
            }
        }

        private Delegate GetCallableAdder(string name)
        {
            var adder = ObjectRuntime.GetProperty(this, name);
            if (adder is not Delegate del)
            {
                throw new TypeError($"Set.prototype.{name} is not callable");
            }

            return del;
        }

        private object? CallAdder(Delegate adder, object? value)
        {
            var previousThis = RuntimeServices.SetCurrentThis(this);
            try
            {
                return Closure.InvokeWithArgs(adder, System.Array.Empty<object>(), new object?[] { value });
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }
        }

        /// <summary>
        /// ECMA-262 Set Record, the result of GetSetRecord. Captures the set-like object together
        /// with its already-coerced size and its <c>has</c> / <c>keys</c> methods so that the set
        /// operations observe each of them exactly once, in spec order.
        /// </summary>
        private readonly struct SetRecord
        {
            internal SetRecord(object setObject, double size, object has, object keys)
            {
                SetObject = setObject;
                Size = size;
                Has = has;
                Keys = keys;
            }

            internal object SetObject { get; }

            internal double Size { get; }

            internal object Has { get; }

            internal object Keys { get; }
        }

        /// <summary>
        /// ECMA-262 GetSetRecord. Set-like objects only need a numeric <c>size</c> plus callable
        /// <c>has</c> and <c>keys</c> properties, so plain objects and classes qualify while
        /// arrays and other iterables deliberately do not.
        /// </summary>
        private static SetRecord GetSetRecord(object? other, string methodName)
        {
            if (!Proxy.IsObjectLikeValue(other))
            {
                throw new TypeError($"Set.prototype.{methodName} called with a non-object argument");
            }

            var rawSize = ObjectRuntime.GetItem(other!, "size");
            var numSize = TypeUtilities.ToNumber(rawSize);
            if (double.IsNaN(numSize))
            {
                throw new TypeError($"Set.prototype.{methodName} argument has a non-numeric size");
            }

            var intSize = ToIntegerOrInfinity(numSize);
            if (intSize < 0)
            {
                throw new RangeError($"Set.prototype.{methodName} argument has a negative size");
            }

            var has = ObjectRuntime.GetItem(other!, "has");
            if (!IsCallableValue(has))
            {
                throw new TypeError($"Set.prototype.{methodName} argument has a non-callable has method");
            }

            var keys = ObjectRuntime.GetItem(other!, "keys");
            if (!IsCallableValue(keys))
            {
                throw new TypeError($"Set.prototype.{methodName} argument has a non-callable keys method");
            }

            return new SetRecord(other!, intSize, has!, keys!);
        }

        /// <summary>
        /// Copies the receiver's set data directly, matching the spec's "copy of O.[[SetData]]"
        /// step. Going through the constructor instead would observably call <c>Set.prototype.add</c>.
        /// </summary>
        private static Set CopyOf(Set source)
        {
            var copy = new Set();
            copy._items.AddRange(source._items);
            foreach (var value in source._items)
            {
                copy._set.Add(value);
            }

            return copy;
        }

        private static bool IsCallableValue(object? value)
            => value is Delegate || (value is Proxy proxy && proxy.IsCallableTarget);

        /// <summary>ECMA-262 ToIntegerOrInfinity, applied to an already-coerced number.</summary>
        private static double ToIntegerOrInfinity(double number)
        {
            if (double.IsNaN(number))
            {
                return 0;
            }

            if (double.IsInfinity(number))
            {
                return number;
            }

            return global::System.Math.Truncate(number);
        }

        /// <summary>ECMA-262 CanonicalizeKeyedCollectionKey: normalizes -0 to +0.</summary>
        private static object? CanonicalizeKey(object? value)
            => value is double d && d == 0 ? 0d : value;

        private static bool SetRecordHas(in SetRecord record, object? value)
            => TypeUtilities.ToBoolean(Function.Apply(record.Has, record.SetObject, new object?[] { value }));

        private static IJavaScriptIterator SetRecordKeys(in SetRecord record)
            => ObjectRuntime.GetIteratorFromMethod(record.SetObject, record.Keys);

        /// <summary>
        /// Walks the keys iterator of a set-like object, canonicalizing each value. When
        /// <paramref name="visit"/> returns false the iterator is closed early, matching the
        /// IteratorClose steps the short-circuiting set predicates perform.
        /// </summary>
        private static void ForEachOtherKey(in SetRecord record, Func<object?, bool> visit)
        {
            var iterator = SetRecordKeys(record);
            var closeIterator = true;
            try
            {
                while (true)
                {
                    var step = ObjectRuntime.IteratorNext(iterator);
                    if (ObjectRuntime.IteratorResultDone(step))
                    {
                        closeIterator = false;
                        break;
                    }

                    if (!visit(CanonicalizeKey(ObjectRuntime.IteratorResultValue(step))))
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (closeIterator)
                {
                    ObjectRuntime.IteratorClose(iterator);
                }
            }
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
            var otherRec = GetSetRecord(other, nameof(difference));
            var result = CopyOf(this);
            if (_items.Count <= otherRec.Size)
            {
                for (var index = 0; index < _items.Count; index++)
                {
                    var value = _items[index];
                    if (SetRecordHas(otherRec, value))
                    {
                        result.delete(value);
                    }
                }
            }
            else
            {
                ForEachOtherKey(otherRec, value =>
                {
                    result.delete(value);
                    return true;
                });
            }

            return result;
        }

        public Set intersection(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(intersection));
            var result = new Set();
            if (_items.Count <= otherRec.Size)
            {
                for (var index = 0; index < _items.Count; index++)
                {
                    var value = _items[index];
                    if (SetRecordHas(otherRec, value))
                    {
                        result.add(value);
                    }
                }
            }
            else
            {
                ForEachOtherKey(otherRec, value =>
                {
                    if (_set.Contains(value!))
                    {
                        result.add(value);
                    }

                    return true;
                });
            }

            return result;
        }

        public bool isDisjointFrom(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(isDisjointFrom));
            var disjoint = true;
            if (_items.Count <= otherRec.Size)
            {
                for (var index = 0; index < _items.Count; index++)
                {
                    if (SetRecordHas(otherRec, _items[index]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                ForEachOtherKey(otherRec, value =>
                {
                    if (!_set.Contains(value!))
                    {
                        return true;
                    }

                    disjoint = false;
                    return false;
                });
            }

            return disjoint;
        }

        public bool isSubsetOf(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(isSubsetOf));
            if (_items.Count > otherRec.Size)
            {
                return false;
            }

            for (var index = 0; index < _items.Count; index++)
            {
                if (!SetRecordHas(otherRec, _items[index]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool isSupersetOf(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(isSupersetOf));
            if (_items.Count < otherRec.Size)
            {
                return false;
            }

            var superset = true;
            ForEachOtherKey(otherRec, value =>
            {
                if (_set.Contains(value!))
                {
                    return true;
                }

                superset = false;
                return false;
            });

            return superset;
        }

        public Set symmetricDifference(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(symmetricDifference));
            var result = CopyOf(this);
            ForEachOtherKey(otherRec, value =>
            {
                if (_set.Contains(value!))
                {
                    result.delete(value);
                }
                else
                {
                    result.add(value);
                }

                return true;
            });

            return result;
        }

        public Set union(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(union));
            var result = CopyOf(this);
            ForEachOtherKey(otherRec, value =>
            {
                result.add(value);
                return true;
            });

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
                PrototypeChain.SetPrototype(this, IteratorPrototype);
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
