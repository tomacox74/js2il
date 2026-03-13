using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptRuntime;

/// <summary>
/// A JavaScript plain object backed by a <see cref="Dictionary{TKey,TValue}"/> of
/// <see cref="JsValue"/> entries. Numeric and boolean property values are stored
/// without heap boxing; boxing is deferred until the value is accessed as
/// <see cref="object"/> (e.g., via the <see cref="IDictionary{TKey,TValue}"/> interface).
/// <para>
/// Typed initializer methods (<see cref="SetNumber"/>, <see cref="SetBoolean"/>,
/// <see cref="SetString"/>, <see cref="SetValue"/>) are called from generated IL
/// for object literal property initialization to avoid the <c>box</c> instruction.
/// </para>
/// </summary>
public sealed class JsObject : IDictionary<string, object?>
{
    private Dictionary<string, JsValue>? _properties;

    // -------------------------------------------------------------------------
    // Typed initializer methods used from generated IL (no boxing at call site)
    // -------------------------------------------------------------------------

    /// <summary>Stores a numeric property without boxing the double value.</summary>
    public void SetNumber(string key, double value)
        => GetOrCreateDict()[key] = JsValue.FromNumber(value);

    /// <summary>Stores a boolean property without boxing the bool value.</summary>
    public void SetBoolean(string key, bool value)
        => GetOrCreateDict()[key] = JsValue.FromBoolean(value);

    /// <summary>Stores a string property.</summary>
    public void SetString(string key, string? value)
        => GetOrCreateDict()[key] = JsValue.FromString(value);

    /// <summary>Stores an arbitrary object value.</summary>
    public void SetValue(string key, object? value)
        => GetOrCreateDict()[key] = JsValue.FromObject(value);

    /// <summary>Stores an arbitrary object value (alias used by newer IL emit paths).</summary>
    public void SetObject(string key, object? value)
        => SetValue(key, value);

    // -------------------------------------------------------------------------
    // Read helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tries to get the value for <paramref name="key"/> as a boxed CLR object.
    /// Returns <c>true</c> and sets <paramref name="value"/> when found.
    /// </summary>
    public bool TryGetBoxedValue(string key, out object? value)
    {
        if (_properties is not null && _properties.TryGetValue(key, out var jv))
        {
            value = jv.ToObject();
            return true;
        }
        value = null;
        return false;
    }

    /// <summary>Returns enumerable sequence of own property names.</summary>
    public IEnumerable<string> GetOwnPropertyNames()
        => _properties?.Keys ?? Enumerable.Empty<string>();

    /// <summary>Returns own property key-value pairs (values boxed as object).</summary>
    public IEnumerable<KeyValuePair<string, object?>> GetOwnProperties()
    {
        if (_properties is null)
            yield break;
        foreach (var kvp in _properties)
            yield return new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToObject());
    }

    // -------------------------------------------------------------------------
    // IDictionary<string, object?> implementation
    // Values are converted to/from JsValue at the interface boundary.
    // -------------------------------------------------------------------------

    public object? this[string key]
    {
        get
        {
            var dict = _properties;
            if (dict is not null && dict.TryGetValue(key, out var jv))
                return jv.ToObject();
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }
        set => GetOrCreateDict()[key] = JsValue.FromObject(value);
    }

    public ICollection<string> Keys => _properties?.Keys ?? (ICollection<string>)System.Array.Empty<string>();

    public ICollection<object?> Values
    {
        get
        {
            if (_properties is null)
                return System.Array.Empty<object?>();
            return _properties.Values.Select(v => v.ToObject()).ToList();
        }
    }

    public int Count => _properties?.Count ?? 0;

    public bool IsReadOnly => false;

    public void Add(string key, object? value) => GetOrCreateDict().Add(key, JsValue.FromObject(value));

    public bool ContainsKey(string key) => _properties?.ContainsKey(key) ?? false;

    public bool Remove(string key) => _properties?.Remove(key) ?? false;

    public bool TryGetValue(string key, out object? value)
    {
        if (_properties is not null && _properties.TryGetValue(key, out var jv))
        {
            value = jv.ToObject();
            return true;
        }
        value = null;
        return false;
    }

    public void Add(KeyValuePair<string, object?> item) => GetOrCreateDict().Add(item.Key, JsValue.FromObject(item.Value));

    public void Clear() => _properties?.Clear();

    public bool Contains(KeyValuePair<string, object?> item)
        => _properties is not null
           && _properties.TryGetValue(item.Key, out var jv)
           && Equals(jv.ToObject(), item.Value);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        if (_properties is null) return;
        int i = arrayIndex;
        foreach (var kvp in _properties)
            array[i++] = new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToObject());
    }

    public bool Remove(KeyValuePair<string, object?> item)
    {
        if (_properties is null) return false;
        if (_properties.TryGetValue(item.Key, out var jv) && Equals(jv.ToObject(), item.Value))
            return _properties.Remove(item.Key);
        return false;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        if (_properties is null)
            yield break;
        foreach (var kvp in _properties)
            yield return new KeyValuePair<string, object?>(kvp.Key, kvp.Value.ToObject());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private Dictionary<string, JsValue> GetOrCreateDict()
        => _properties ??= new Dictionary<string, JsValue>(StringComparer.Ordinal);
}
