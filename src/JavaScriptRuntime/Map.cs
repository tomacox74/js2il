using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Map")]
    public sealed class Map : IEnumerable<object[]>
    {
        private static readonly object NullKeySentinel = new object();
        private readonly List<object[]> _entries = new List<object[]>(); // [key, value] pairs
        private readonly Dictionary<object, int> _keyIndex = new Dictionary<object, int>(new SameValueZeroKeyComparer());

        public Map() { }

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
