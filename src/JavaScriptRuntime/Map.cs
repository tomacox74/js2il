using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Map")]
    public sealed class Map : IEnumerable<object[]>
    {
        internal static readonly object Prototype = CreatePrototype();
        private static readonly object NullKeySentinel = new object();
        private readonly List<object[]> _entries = new List<object[]>(); // [key, value] pairs
        private readonly Dictionary<object, int> _keyIndex = new Dictionary<object, int>(new SameValueZeroKeyComparer());

        public Map()
        {
            PrototypeChain.SetPrototype(this, Prototype);
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

        // Iterator support - returns entries as [key, value] arrays
        public IEnumerator<object[]> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

        // JavaScript Map.prototype.keys()
        public IEnumerable<object> keys()
        {
            foreach (var entry in _entries)
            {
                yield return entry[0];
            }
        }

        // JavaScript Map.prototype.values()
        public IEnumerable<object> values()
        {
            foreach (var entry in _entries)
            {
                yield return entry[1];
            }
        }

        // JavaScript Map.prototype.entries()
        public IEnumerable<object[]> entries()
        {
            return _entries;
        }

        private static object CreatePrototype()
        {
            var prototype = new JsObject();
            DefinePrototypeMethod(prototype, "clear", PrototypeClear);
            DefinePrototypeMethod(prototype, "delete", PrototypeDelete);
            DefinePrototypeMethod(prototype, "entries", PrototypeEntries);
            DefinePrototypeMethod(prototype, "get", PrototypeGet);
            DefinePrototypeMethod(prototype, "has", PrototypeHas);
            DefinePrototypeMethod(prototype, "keys", PrototypeKeys);
            DefinePrototypeMethod(prototype, "set", PrototypeSet);
            DefinePrototypeMethod(prototype, "values", PrototypeValues);
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

        private static Map GetThisMap(string memberName)
        {
            var thisValue = RuntimeServices.GetCurrentThis();
            if (thisValue is not Map map)
            {
                throw new TypeError($"Map.prototype.{memberName} called on non-Map");
            }

            return map;
        }

        private static object? PrototypeClear(object[] scopes, object?[]? args)
        {
            GetThisMap("clear").clear();
            return null;
        }

        private static object? PrototypeDelete(object[] scopes, object?[]? args)
        {
            var map = GetThisMap("delete");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.delete(key);
        }

        private static object? PrototypeEntries(object[] scopes, object?[]? args)
            => GetThisMap("entries").entries();

        private static object? PrototypeGet(object[] scopes, object?[]? args)
        {
            var map = GetThisMap("get");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.get(key);
        }

        private static object? PrototypeHas(object[] scopes, object?[]? args)
        {
            var map = GetThisMap("has");
            var key = args != null && args.Length > 0 ? args[0] : null;
            return map.has(key);
        }

        private static object? PrototypeKeys(object[] scopes, object?[]? args)
            => GetThisMap("keys").keys();

        private static object? PrototypeSet(object[] scopes, object?[]? args)
        {
            var map = GetThisMap("set");
            var key = args != null && args.Length > 0 ? args[0] : null;
            var value = args != null && args.Length > 1 ? args[1] : null;
            return map.set(key, value);
        }

        private static object? PrototypeSizeGetter(object[] scopes, object?[]? args)
            => GetThisMap("size").size;

        private static object? PrototypeValues(object[] scopes, object?[]? args)
            => GetThisMap("values").values();

        private static object NormalizeKey(object? key)
        {
            return key ?? NullKeySentinel;
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
