using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime
{
    [IntrinsicObject("Map")]
    public sealed class Map : IEnumerable<object[]>
    {
        private readonly List<object[]> _entries = new List<object[]>(); // [key, value] pairs
        private readonly Dictionary<object, int> _keyIndex = new Dictionary<object, int>();

        public Map() { }

        // JavaScript Map.prototype.size property
        public double size
        {
            get { return _entries.Count; }
        }

        public object set(object? key, object? value)
        {
            var k = key!; // JS allows undefined/null as keys
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                // Update existing key
                _entries[idx] = new object[] { k, value! };
            }
            else
            {
                // Add new key
                _keyIndex[k] = _entries.Count;
                _entries.Add(new object[] { k, value! });
            }
            return this;
        }

        public object? get(object? key)
        {
            var k = key!;
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                return _entries[idx][1];
            }
            return null; // JavaScript undefined, represented as null in .NET
        }

        public bool has(object? key)
        {
            return _keyIndex.ContainsKey(key!);
        }

        public bool delete(object? key)
        {
            var k = key!;
            if (_keyIndex.TryGetValue(k, out var idx))
            {
                _keyIndex.Remove(k);
                _entries.RemoveAt(idx);
                
                // Rebuild index for entries after removed item
                for (int i = idx; i < _entries.Count; i++)
                {
                    _keyIndex[_entries[i][0]] = i;
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
    }
}
