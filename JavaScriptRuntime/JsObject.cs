using System.Collections;
using System.Collections.Generic;

namespace JavaScriptRuntime;

/// <summary>
/// Lightweight JavaScript object storage that keeps common scalar values unboxed.
/// </summary>
public sealed class JsObject : IDictionary<string, object?>
{
    private readonly Dictionary<string, JsValue> _properties = new(StringComparer.Ordinal);

    public void SetNumber(string key, double value) => _properties[key] = JsValue.FromNumber(value);

    public void SetBoolean(string key, bool value) => _properties[key] = JsValue.FromBoolean(value);

    public void SetObject(string key, object? value) => _properties[key] = JsValue.FromObject(value);

    public object? this[string key]
    {
        get => _properties.TryGetValue(key, out var value) ? value.ToObject() : null;
        set => SetObject(key, value);
    }

    public ICollection<string> Keys => _properties.Keys;

    public ICollection<object?> Values
    {
        get
        {
            var values = new List<object?>(_properties.Count);
            foreach (var value in _properties.Values)
            {
                values.Add(value.ToObject());
            }
            return values;
        }
    }

    public int Count => _properties.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object? value) => SetObject(key, value);

    public bool ContainsKey(string key) => _properties.ContainsKey(key);

    public bool Remove(string key) => _properties.Remove(key);

    public bool TryGetValue(string key, out object? value)
    {
        if (_properties.TryGetValue(key, out var entry))
        {
            value = entry.ToObject();
            return true;
        }

        value = null;
        return false;
    }

    public void Add(KeyValuePair<string, object?> item) => SetObject(item.Key, item.Value);

    public void Clear() => _properties.Clear();

    public bool Contains(KeyValuePair<string, object?> item)
    {
        return TryGetValue(item.Key, out var value) && Equals(value, item.Value);
    }

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        foreach (var kvp in _properties)
        {
            array[arrayIndex++] = new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToObject());
        }
    }

    public bool Remove(KeyValuePair<string, object?> item)
    {
        if (!Contains(item))
        {
            return false;
        }

        return _properties.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        foreach (var kvp in _properties)
        {
            yield return new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToObject());
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly struct JsValue
    {
        private readonly byte _kind;
        private readonly double _numberValue;
        private readonly bool _boolValue;
        private readonly object? _objectValue;

        private JsValue(byte kind, double numberValue, bool boolValue, object? objectValue)
        {
            _kind = kind;
            _numberValue = numberValue;
            _boolValue = boolValue;
            _objectValue = objectValue;
        }

        public static JsValue FromNumber(double value) => new(kind: 1, numberValue: value, boolValue: false, objectValue: null);
        public static JsValue FromBoolean(bool value) => new(kind: 2, numberValue: 0d, boolValue: value, objectValue: null);
        public static JsValue FromObject(object? value) => value switch
        {
            double d => FromNumber(d),
            bool b => FromBoolean(b),
            _ => new JsValue(kind: 3, numberValue: 0d, boolValue: false, objectValue: value)
        };

        public object? ToObject() => _kind switch
        {
            1 => _numberValue,
            2 => _boolValue,
            _ => _objectValue
        };
    }
}
