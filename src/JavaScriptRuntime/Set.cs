using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Set")]
    public sealed class Set : JsObject, IEnumerable<object>
    {
        private static readonly object _emptySlot = new object();
        private static readonly object _iteratorRegistration = new object();
        private static readonly BuiltinFunction0 _prototypeSizeGetterValue = PrototypeSizeGetter;
        private static readonly BuiltinFunction0 _prototypeValuesValue = PrototypeValues;
        /// <summary>Realm-owned <c>Set Iterator prototype</c> intrinsic (issue #1824).</summary>
        internal static JsObject IteratorPrototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.SetIteratorPrototype,
                static () => new JsObject(),
                static prototype => InitializeIteratorPrototype(prototype));
        /// <summary>Realm-owned <c>Set.prototype</c> intrinsic (issue #1824).</summary>
        internal static JsObject Prototype
            => RuntimeIntrinsics.Current.GetOrCreate(
                RuntimeIntrinsicSlot.SetPrototype,
                static () => new JsObject(),
                static exp => InitializePrototype(exp));
        private readonly List<object> _items = new List<object>();
        private readonly HashSet<object> _set = new HashSet<object>();
        private readonly ConditionalWeakTable<SetIterator, object> _iterators =
            new ConditionalWeakTable<SetIterator, object>();
        // Reentrant callbacks need stable indexes; otherwise removals compact and adjust iterator cursors.
        private int _activeIndexedTraversals;
        private int _emptySlotCount;

        private static void InitializePrototype(JsObject exp)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            DefinePrototypeMethod(exp, "add", (BuiltinFunction1)PrototypeAdd);
            DefinePrototypeMethod(exp, "has", (BuiltinFunction1)PrototypeHas);
            DefinePrototypeMethod(exp, "delete", (BuiltinFunction1)PrototypeDelete);
            DefinePrototypeMethod(exp, "clear", (BuiltinFunction0)PrototypeClear);
            DefinePrototypeMethod(exp, "entries", (BuiltinFunction0)PrototypeEntries);
            DefinePrototypeMethod(exp, "forEach", (BuiltinFunction2)PrototypeForEach, 1d);
            DefinePrototypeMethod(exp, "keys", _prototypeValuesValue);
            DefinePrototypeMethod(exp, "values", _prototypeValuesValue);
            DefinePrototypeMethod(exp, "difference", (BuiltinFunction1)PrototypeDifference);
            DefinePrototypeMethod(exp, "intersection", (BuiltinFunction1)PrototypeIntersection);
            DefinePrototypeMethod(exp, "isDisjointFrom", (BuiltinFunction1)PrototypeIsDisjointFrom);
            DefinePrototypeMethod(exp, "isSubsetOf", (BuiltinFunction1)PrototypeIsSubsetOf);
            DefinePrototypeMethod(exp, "isSupersetOf", (BuiltinFunction1)PrototypeIsSupersetOf);
            DefinePrototypeMethod(exp, "symmetricDifference", (BuiltinFunction1)PrototypeSymmetricDifference);
            DefinePrototypeMethod(exp, "union", (BuiltinFunction1)PrototypeUnion);
            Function.InitializeFunctionInstance(
                _prototypeSizeGetterValue,
                0d,
                "get size",
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(_prototypeSizeGetterValue));
            Function.MarkUndefinedPrototype(_prototypeSizeGetterValue);
            PropertyDescriptorStore.DefineOrUpdate(exp, "size", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = _prototypeSizeGetterValue
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
        }

        private static void InitializeIteratorPrototype(JsObject prototype)
        {
            using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

            PrototypeChain.SetPrototype(prototype, Iterator.Prototype);
            PropertyDescriptorStore.DefineOrUpdate(prototype, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Set Iterator"
            });
        }

        private static void DefinePrototypeMethod(JsObject prototype, string name, Delegate method)
            => DefinePrototypeMethod(prototype, name, method, Function.GetLength(method));

        private static void DefinePrototypeMethod(
            JsObject prototype,
            string name,
            Delegate method,
            double length)
        {
            Function.InitializeFunctionInstance(
                method,
                length,
                name,
                requiresInvocationContext: !BuiltinFunctionDelegates.IsReceiverAware(method));
            Function.MarkUndefinedPrototype(method);
            PropertyDescriptorStore.DefineOrUpdate(prototype, name, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = method
            });
        }

        private static Set GetSetReceiver(object? thisArgument, string methodName)
        {
            if (thisArgument is not Set set)
            {
                throw new TypeError($"Set.prototype.{methodName} called on incompatible receiver");
            }

            return set;
        }

        private static object? PrototypeAdd(object? thisArgument, object? value)
        {
            var set = GetSetReceiver(thisArgument, "add");
            return set.add(value);
        }

        private static object? PrototypeHas(object? thisArgument, object? value)
        {
            var set = GetSetReceiver(thisArgument, "has");
            return set.has(value);
        }

        private static object? PrototypeDelete(object? thisArgument, object? value)
        {
            var set = GetSetReceiver(thisArgument, "delete");
            return set.delete(value);
        }

        private static object? PrototypeClear(object? thisArgument)
        {
            GetSetReceiver(thisArgument, "clear").clear();
            return null;
        }

        private static object? PrototypeEntries(object? thisArgument)
        {
            return GetSetReceiver(thisArgument, "entries").entries();
        }

        private static object? PrototypeValues(object? thisArgument)
        {
            return GetSetReceiver(thisArgument, "values").values();
        }

        private static object? PrototypeForEach(object? thisArgument, object? callback, object? forEachThisArg)
        {
            var set = GetSetReceiver(thisArgument, "forEach");
            set.forEach(callback, forEachThisArg);
            return null;
        }

        private static object? PrototypeDifference(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "difference");
            return set.difference(other);
        }

        private static object? PrototypeIntersection(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "intersection");
            return set.intersection(other);
        }

        private static object? PrototypeIsDisjointFrom(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "isDisjointFrom");
            return set.isDisjointFrom(other);
        }

        private static object? PrototypeIsSubsetOf(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "isSubsetOf");
            return set.isSubsetOf(other);
        }

        private static object? PrototypeIsSupersetOf(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "isSupersetOf");
            return set.isSupersetOf(other);
        }

        private static object? PrototypeSymmetricDifference(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "symmetricDifference");
            return set.symmetricDifference(other);
        }

        private static object? PrototypeUnion(object? thisArgument, object? other)
        {
            var set = GetSetReceiver(thisArgument, "union");
            return set.union(other);
        }

        private static object? PrototypeSizeGetter(object? thisArgument)
        {
            return GetSetReceiver(thisArgument, "size").size;
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.InitializePrototype(this, Prototype);
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
                    JavaScriptRuntime.ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                }
            }
        }

        private object GetCallableAdder(string name)
        {
            var adder = ObjectRuntime.GetProperty(this, name);
            if (!CallableOperations.IsCallable(adder))
            {
                throw new TypeError($"Set.prototype.{name} is not callable");
            }

            return adder!;
        }

        private object? CallAdder(object adder, object? value)
            => CallableOperations.Call1(adder, this, value);

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
            foreach (var value in source._items)
            {
                if (IsEmptySlot(value))
                {
                    continue;
                }

                copy._items.Add(value);
                copy._set.Add(value);
            }

            return copy;
        }

        private static bool IsCallableValue(object? value)
            => CallableOperations.IsCallable(value);

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
            => value switch
            {
                double number when number == 0d => 0d,
                float number when number == 0f => 0d,
                _ => value
            };

        private static bool IsEmptySlot(object value)
            => ReferenceEquals(value, _emptySlot);

        private bool VisitLiveValues(Func<object, bool> visit)
        {
            _activeIndexedTraversals++;
            try
            {
                for (var index = 0; index < _items.Count; index++)
                {
                    var value = _items[index];
                    if (!IsEmptySlot(value) && !visit(value))
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                _activeIndexedTraversals--;
                if (_activeIndexedTraversals == 0)
                {
                    CompactEmptySlots();
                }
            }
        }

        private void ForEachActiveIterator(Action<SetIterator> action)
        {
            foreach (var registration in _iterators)
            {
                if (!registration.Key.IsClosed)
                {
                    action(registration.Key);
                }
            }
        }

        private void RegisterIterator(SetIterator iterator)
            => _iterators.Add(iterator, _iteratorRegistration);

        private void UnregisterIterator(SetIterator iterator)
            => _iterators.Remove(iterator);

        private void CompactEmptySlots()
        {
            if (_emptySlotCount == 0)
            {
                return;
            }

            ForEachActiveIterator(iterator =>
            {
                var liveIndex = 0;
                var oldIndex = global::System.Math.Min(iterator.Index, _items.Count);
                for (var index = 0; index < oldIndex; index++)
                {
                    if (!IsEmptySlot(_items[index]))
                    {
                        liveIndex++;
                    }
                }

                iterator.Index = liveIndex;
            });

            _items.RemoveAll(IsEmptySlot);
            _emptySlotCount = 0;
        }

        private void RemoveItemAt(int index)
        {
            if (_activeIndexedTraversals > 0)
            {
                _items[index] = _emptySlot;
                _emptySlotCount++;
                return;
            }

            _items.RemoveAt(index);
            ForEachActiveIterator(iterator =>
            {
                if (iterator.Index > index)
                {
                    iterator.Index--;
                }
            });
        }

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
            get { return _set.Count; }
        }

        public object add(object? value)
        {
            var v = CanonicalizeKey(value)!; // JS allows undefined/null; store as null reference
            if (!_set.Contains(v))
            {
                _set.Add(v);
                _items.Add(v);
            }
            return this;
        }

        public object has(object? value)
        {
            return _set.Contains(CanonicalizeKey(value)!);
        }

        public bool delete(object? value)
        {
            var v = CanonicalizeKey(value)!;
            if (!_set.Remove(v))
            {
                return false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (!IsEmptySlot(_items[i]) && Equals(_items[i], v))
                {
                    RemoveItemAt(i);
                    break;
                }
            }

            return true;
        }

        public void clear()
        {
            _set.Clear();
            if (_activeIndexedTraversals == 0)
            {
                _items.Clear();
                _emptySlotCount = 0;
                ForEachActiveIterator(iterator => iterator.Index = 0);
                return;
            }

            for (var index = 0; index < _items.Count; index++)
            {
                if (!IsEmptySlot(_items[index]))
                {
                    _items[index] = _emptySlot;
                    _emptySlotCount++;
                }
            }
        }

        public void forEach(object? callback)
        {
            forEach(callback, null);
        }

        public void forEach(object? callback, object? thisArg)
        {
            if (!CallableOperations.IsCallable(callback))
            {
                throw new TypeError("Set.prototype.forEach callback must be a function");
            }

            VisitLiveValues(value =>
            {
                CallableOperations.Call3(callback, thisArg, value, value, this);
                return true;
            });
        }

        public IJavaScriptIterator values() => new SetIterator(this, SetIteratorKind.Values);

        public IJavaScriptIterator keys() => new SetIterator(this, SetIteratorKind.Values);

        public IJavaScriptIterator entries() => new SetIterator(this, SetIteratorKind.Entries);

        public Set difference(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(difference));
            var result = CopyOf(this);
            if (_set.Count <= otherRec.Size)
            {
                result.VisitLiveValues(value =>
                {
                    if (SetRecordHas(otherRec, value))
                    {
                        result.delete(value);
                    }

                    return true;
                });
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
            if (_set.Count <= otherRec.Size)
            {
                VisitLiveValues(value =>
                {
                    if (SetRecordHas(otherRec, value))
                    {
                        result.add(value);
                    }

                    return true;
                });
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
            if (_set.Count <= otherRec.Size)
            {
                return VisitLiveValues(value => !SetRecordHas(otherRec, value));
            }

            var disjoint = true;
            ForEachOtherKey(otherRec, value =>
            {
                if (!_set.Contains(value!))
                {
                    return true;
                }

                disjoint = false;
                return false;
            });

            return disjoint;
        }

        public bool isSubsetOf(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(isSubsetOf));
            if (_set.Count > otherRec.Size)
            {
                return false;
            }

            return VisitLiveValues(value => SetRecordHas(otherRec, value));
        }

        public bool isSupersetOf(object? other)
        {
            var otherRec = GetSetRecord(other, nameof(isSupersetOf));
            if (_set.Count < otherRec.Size)
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

        public new IEnumerator<object> GetEnumerator()
        {
            _activeIndexedTraversals++;
            try
            {
                for (var index = 0; index < _items.Count; index++)
                {
                    var value = _items[index];
                    if (!IsEmptySlot(value))
                    {
                        yield return value;
                    }
                }
            }
            finally
            {
                _activeIndexedTraversals--;
                if (_activeIndexedTraversals == 0)
                {
                    CompactEmptySlots();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        private enum SetIteratorKind
        {
            Values,
            Entries
        }

        private sealed class SetIterator : JsObject, IJavaScriptIterator
        {
            private readonly Set _set;
            private readonly SetIteratorKind _kind;
            private int _index;
            private bool _isClosed;

            internal int Index
            {
                get => _index;
                set => _index = value;
            }

            internal bool IsClosed => _isClosed;

            public SetIterator(Set set, SetIteratorKind kind)
            {
                _set = set;
                _kind = kind;
                set.RegisterIterator(this);
                PrototypeChain.InitializePrototype(this, IteratorPrototype);
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed)
                {
                    return new IteratorResultObject(null, done: true);
                }

                while (_index < _set._items.Count)
                {
                    var value = _set._items[_index++];
                    if (IsEmptySlot(value))
                    {
                        continue;
                    }

                    object? result = _kind == SetIteratorKind.Entries
                        ? new JavaScriptRuntime.Array(new object?[] { value, value })
                        : value;

                    return new IteratorResultObject(result, done: false);
                }

                Close();
                return new IteratorResultObject(null, done: true);
            }

            public object next(object? value = null) => Next();

            public void Return()
            {
                Close();
            }

            private void Close()
            {
                if (_isClosed)
                {
                    return;
                }

                _isClosed = true;
                _set.UnregisterIterator(this);
            }
        }
    }
}
