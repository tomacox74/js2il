using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Map")]
    public sealed class Map : IEnumerable<object[]>
    {
        private static readonly Func<object[], object?[]?, object?> _prototypeEntriesValue = PrototypeEntries;
        internal static readonly ExpandoObject Prototype = CreatePrototype();
        private static readonly object NullKeySentinel = new object();
        private readonly List<object[]> _entries = new List<object[]>(); // [key, value] pairs
        private readonly Dictionary<object, int> _keyIndex = new Dictionary<object, int>(new SameValueZeroKeyComparer());

        private static ExpandoObject CreatePrototype()
        {
            var exp = new ExpandoObject();
            DefinePrototypeMethod(exp, "set", PrototypeSet);
            DefinePrototypeMethod(exp, "get", PrototypeGet);
            DefinePrototypeMethod(exp, "has", PrototypeHas);
            DefinePrototypeMethod(exp, "delete", PrototypeDelete);
            DefinePrototypeMethod(exp, "clear", PrototypeClear);
            DefinePrototypeMethod(exp, "keys", PrototypeKeys);
            DefinePrototypeMethod(exp, "values", PrototypeValues);
            DefinePrototypeMethod(exp, "entries", _prototypeEntriesValue);
            DefinePrototypeMethod(exp, "forEach", PrototypeForEach);
            PropertyDescriptorStore.DefineOrUpdate(exp, Symbol.iterator.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = _prototypeEntriesValue
            });
            PropertyDescriptorStore.DefineOrUpdate(exp, "size", new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Enumerable = false,
                Configurable = true,
                Get = (Func<object[], object?[]?, object?>)PrototypeSizeGetter
            });
            PropertyDescriptorStore.DefineOrUpdate(exp, Symbol.toStringTag.DebugId, new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = false,
                Value = "Map"
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

        private static Map GetMapReceiver(string methodName)
        {
            var receiver = RuntimeServices.GetCurrentThis();
            if (receiver is not Map map)
            {
                throw new TypeError($"Map.prototype.{methodName} called on incompatible receiver");
            }

            return map;
        }

        private static object? PrototypeSet(object[] scopes, object?[]? args)
        {
            var map = GetMapReceiver("set");
            var key = args != null && args.Length > 0 ? args[0] : null;
            var value = args != null && args.Length > 1 ? args[1] : null;
            return map.set(key, value);
        }

        private static object? PrototypeGet(object[] scopes, object?[]? args)
        {
            var map = GetMapReceiver("get");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.get(key);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var map = GetMapReceiver("has");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.has(key);
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var map = GetMapReceiver("delete");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.delete(key);
        }

        private static object? PrototypeClear(object[] scopes, object?[]? args)
        {
            GetMapReceiver("clear").clear();
            return null;
        }

        private static object? PrototypeKeys(object[] scopes, object?[]? args)
        {
            return GetMapReceiver("keys").keys();
        }

        private static object? PrototypeValues(object[] scopes, object?[]? args)
        {
            return GetMapReceiver("values").values();
        }

        private static object? PrototypeEntries(object[] scopes, object?[]? args)
        {
            return GetMapReceiver("entries").entries();
        }

        private static object? PrototypeForEach(object[] scopes, object?[]? args)
        {
            var map = GetMapReceiver("forEach");
            var callback = args != null && args.Length > 0 ? args[0] : null;
            var thisArg = args != null && args.Length > 1 ? args[1] : null;
            map.forEach(callback, thisArg);
            return null;
        }

        private static object? PrototypeSizeGetter(object[] scopes, object?[]? args)
        {
            return GetMapReceiver("size").size;
        }

        private void InitializeIntrinsicSurface()
        {
            PrototypeChain.SetPrototype(this, Prototype);
        }

        public Map()
        {
            InitializeIntrinsicSurface();
        }

        public Map(object? iterable)
        {
            InitializeIntrinsicSurface();
            if (iterable is null || iterable is JsNull)
            {
                return;
            }

            AddEntriesFromIterable(iterable);
        }

        private void AddEntriesFromIterable(object iterable)
        {
            var iterator = ObjectRuntime.GetIterator(iterable);
            var completedNormally = false;
            try
            {
                while (true)
                {
                    var step = JavaScriptRuntime.Object.IteratorNext(iterator);
                    if (JavaScriptRuntime.Object.IteratorResultDone(step))
                    {
                        break;
                    }

                    var (key, value) = ExtractEntry(JavaScriptRuntime.Object.IteratorResultValue(step));
                    set(key, value);
                }

                completedNormally = true;
            }
            finally
            {
                if (!completedNormally)
                {
                    JavaScriptRuntime.Object.IteratorClose(iterator);
                }
            }
        }

        private static (object? Key, object? Value) ExtractEntry(object? entry)
        {
            if (entry is null || entry is JsNull)
            {
                throw new TypeError("Iterator value must be an object");
            }

            var entryType = TypeUtilities.Typeof(entry);
            if (entryType != "object" && entryType != "function")
            {
                throw new TypeError("Iterator value is not an entry object");
            }

            if (entry is JavaScriptRuntime.Array arrayEntry)
            {
                return (
                    arrayEntry.Count > 0 ? arrayEntry[0] : null,
                    arrayEntry.Count > 1 ? arrayEntry[1] : null
                );
            }

            if (entry is System.Collections.IList listEntry)
            {
                return (
                    listEntry.Count > 0 ? listEntry[0] : null,
                    listEntry.Count > 1 ? listEntry[1] : null
                );
            }

            return (ObjectRuntime.GetItem(entry, 0.0), ObjectRuntime.GetItem(entry, 1.0));
        }

        // JavaScript Map.prototype.size property
        public double size
        {
            get { return _entries.Count; }
        }

        public object set(object? key, object? value)
        {
            var k = NormalizeKey(key);
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                // Update existing key
                _entries[idx] = new object[] { key!, value! };
            }
            else
            {
                // Add new key
                _keyIndex[k] = _entries.Count;
                _entries.Add(new object[] { key!, value! });
            }
            return this;
        }

        public object? get(object? key)
        {
            var k = NormalizeKey(key);
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                return _entries[idx][1];
            }
            return null; // JavaScript undefined, represented as null in .NET
        }

        public bool has(object? key)
        {
            return _keyIndex.ContainsKey(NormalizeKey(key));
        }

        public bool delete(object? key)
        {
            // Current implementation preserves insertion order by compacting the list,
            // which requires re-indexing following entries (O(n) delete).
            var k = NormalizeKey(key);
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                _keyIndex.Remove(k);
                _entries.RemoveAt(idx);
                
                // Rebuild index for entries after removed item
                for (int i = idx; i < _entries.Count; i++)
                {
                    _keyIndex[NormalizeKey(_entries[i][0])] = i;
                }
                return true;
            }
            return false;
        }

        public void clear()
        {
            _keyIndex.Clear();
            _entries.Clear();
        }

        public void forEach(object? callback)
        {
            forEach(callback, null);
        }

        public void forEach(object? callback, object? thisArg)
        {
            if (callback is not Delegate del)
            {
                throw new TypeError("Map.prototype.forEach callback must be a function");
            }

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                var previousThis = RuntimeServices.SetCurrentThis(thisArg);
                try
                {
                    Closure.InvokeWithArgs(del, System.Array.Empty<object>(), new object?[] { entry[1], entry[0], this });
                }
                finally
                {
                    RuntimeServices.SetCurrentThis(previousThis);
                }
            }
        }

        // Iterator support - returns entries as [key, value] arrays
        public IEnumerator<object[]> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

        // JavaScript Map.prototype.keys()
        public IJavaScriptIterator keys() => new MapIterator(this, MapIteratorKind.Keys);

        // JavaScript Map.prototype.values()
        public IJavaScriptIterator values() => new MapIterator(this, MapIteratorKind.Values);

        // JavaScript Map.prototype.entries()
        public IJavaScriptIterator entries() => new MapIterator(this, MapIteratorKind.Entries);

        private static object NormalizeKey(object? key)
        {
            return key ?? NullKeySentinel;
        }

        private enum MapIteratorKind
        {
            Keys,
            Values,
            Entries
        }

        private sealed class MapIterator : IJavaScriptIterator
        {
            private readonly Map _map;
            private readonly MapIteratorKind _kind;
            private int _index;
            private bool _isClosed;

            public MapIterator(Map map, MapIteratorKind kind)
            {
                _map = map;
                _kind = kind;
            }

            public bool HasReturn => true;

            public IteratorResultObject Next()
            {
                if (_isClosed)
                {
                    return new IteratorResultObject(null, done: true);
                }

                if (_index >= _map._entries.Count)
                {
                    return new IteratorResultObject(null, done: true);
                }

                var entry = _map._entries[_index++];
                object? value = _kind switch
                {
                    MapIteratorKind.Keys => entry[0],
                    MapIteratorKind.Values => entry[1],
                    MapIteratorKind.Entries => new JavaScriptRuntime.Array(new object?[] { entry[0], entry[1] }),
                    _ => null
                };

                return new IteratorResultObject(value, done: false);
            }

            public object next(object? value = null) => Next();

            public void Return()
            {
                _isClosed = true;
            }
        }

        private sealed class SameValueZeroKeyComparer : IEqualityComparer<object>
        {
            public new bool Equals(object? x, object? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                if (x is double dx && y is double dy)
                {
                    return (double.IsNaN(dx) && double.IsNaN(dy)) || dx == dy;
                }

                if (x is float fx && y is float fy)
                {
                    return (float.IsNaN(fx) && float.IsNaN(fy)) || fx == fy;
                }

                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                if (obj is null) return 0;

                if (obj is double d)
                {
                    if (double.IsNaN(d)) return 0x7ff80000;
                    if (d == 0d) return 0;
                    return d.GetHashCode();
                }

                if (obj is float f)
                {
                    if (float.IsNaN(f)) return 0x7fc00000;
                    if (f == 0f) return 0;
                    return f.GetHashCode();
                }

                return obj.GetHashCode();
            }
        }
    }
}
