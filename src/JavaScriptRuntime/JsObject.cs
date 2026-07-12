using System;
using System.Collections;
using System.Collections.Frozen;

namespace JavaScriptRuntime;

internal sealed class JsShape
{

    private static readonly ThreadLocal<JsShape> _empty = new (() => new JsShape());

    public static JsShape Empty
    {
        get => _empty.Value!;
    }

    private FrozenDictionary<string, int> _slots;

    private Dictionary<string, WeakReference<JsShape>>? _transitions = new Dictionary<string, WeakReference<JsShape>>();

    public JsShape()
    {
        _slots = new Dictionary<string, int>().ToFrozenDictionary();
    }

    public IEnumerable<string> PropertyNames
    {
        get
        {
            // FrozenDictionary does not guarantee enumeration order; property names
            // must be reported in slot order to preserve JS insertion-order semantics.
            var names = new string[_slots.Count];
            foreach (var kvp in _slots)
            {
                names[kvp.Value] = kvp.Key;
            }
            return names;
        }
    }

    private JsShape(string newPropertyName, JsShape parent)
    {
        var newSlots = parent._slots.ToDictionary();
        newSlots[string.Intern(newPropertyName)] = newSlots.Count;
        _slots = newSlots.ToFrozenDictionary();
    }

    private JsShape(string deadPropertyName, JsShape parent, bool delete)
    {
        // Rebuild slots in the parent's slot order so surviving properties keep their
        // relative order and stay aligned with the compacted value array.
        var newSlots = new Dictionary<string, int>();
        foreach (var name in parent.PropertyNames)
        {
            if (!string.Equals(name, deadPropertyName, StringComparison.Ordinal))
            {
                newSlots[name] = newSlots.Count;
            }
        }
        _slots = newSlots.ToFrozenDictionary();
    }

    public JsShape TransitionTo(string newPropertyName)
    {
        if (_transitions != null && _transitions.TryGetValue(newPropertyName, out var weakRef) && weakRef.TryGetTarget(out var existingShape))
        {
            return existingShape;
        }
        var newShape = new JsShape(newPropertyName, this);
        _transitions![newPropertyName] = new WeakReference<JsShape>(newShape);
        return newShape;
    }

    public JsShape TransitionAway(string deadPropertyName)
    {
        return new JsShape(deadPropertyName, this, true);
    }

    public int GetSlot(string propertyName)
        => _slots.TryGetValue(propertyName, out var slot) ? slot : -1;
}

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
public class JsObject : IDictionary<string, object?>
{
    private JsValue[] _properties = System.Array.Empty<JsValue>();

    private JsShape _shape = JsShape.Empty;

    // Perf (#1418 follow-up): sticky flag set when this object gains descriptor
    // state that the plain dictionary cannot answer (accessors, delete tombstones,
    // non-default attributes from defineProperty/seal/freeze, intrinsic descriptors).
    // While the flag is clear, every own descriptor is a mirrored default data
    // descriptor whose value matches the dictionary, so hot read paths can go
    // straight to the dictionary and skip the descriptor store probe entirely.
    private volatile bool _hasNonDataDescriptors;

    /// <summary>
    /// True when own reads can no longer be answered from the property dictionary
    /// alone (the object has accessors, deleted tombstones, or attribute-bearing
    /// descriptors). Sticky: once set it is never cleared.
    /// </summary>
    internal bool HasNonDataDescriptors => _hasNonDataDescriptors;

    internal void MarkNonDataDescriptors() => _hasNonDataDescriptors = true;

    // -------------------------------------------------------------------------
    // Typed initializer methods used from generated IL (no boxing at call site)
    // -------------------------------------------------------------------------

    /// <summary>Stores a numeric property without boxing the double value.</summary>
    public void SetNumber(string key, double value)
    {
        SetValue(key, JsValue.FromNumber(value));
        DefineDataDescriptor(key, value);
    }

    /// <summary>Stores a boolean property without boxing the bool value.</summary>
    public void SetBoolean(string key, bool value)
    {
        SetValue(key, JsValue.FromBoolean(value));
        DefineDataDescriptor(key, value);
    }

    /// <summary>Stores a string property.</summary>
    public void SetString(string key, string? value)
    {
        SetValue(key, JsValue.FromString(value));
        DefineDataDescriptor(key, value);
    }

    /// <summary>Stores an arbitrary object value.</summary>
    public void SetValue(string key, object? value)
    {
        SetValue(key, JsValue.FromObject(value));
        DefineDataDescriptor(key, value);
    }

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
        if (TryGetJsValue(key, out var jv))
        {
            value = jv.ToObject();
            return true;
        }
        value = null;
        return false;
    }

    internal void SetBoxedValue(string key, object? value)
        => SetValue(key, JsValue.FromObject(value));

    /// <summary>Returns enumerable sequence of own property names.</summary>
    public IEnumerable<string> GetOwnPropertyNames()
        => _shape.PropertyNames;

    /// <summary>Returns own property key-value pairs (values boxed as object).</summary>
    public IEnumerable<KeyValuePair<string, object?>> GetOwnProperties()
    {
        for (int i = 0; i < _shape.PropertyNames.Count(); i++)
        {
            var key = _shape.PropertyNames.ElementAt(i);
            var value = _properties[i];
            yield return new KeyValuePair<string, object?>(key, value.ToObject());
        }
    }

    // -------------------------------------------------------------------------
    // IDictionary<string, object?> implementation
    // Values are converted to/from JsValue at the interface boundary.
    // -------------------------------------------------------------------------

    public object? this[string key]
    {
        get => GetValue(key).ToObject();
        set => SetValue(key, JsValue.FromObject(value));
    }

    public ICollection<string> Keys => _shape.PropertyNames.ToList();

    public ICollection<object?> Values => _properties!.Select(v => v.ToObject()).ToList();

    public int Count => _properties!.Length;

    public bool IsReadOnly => false;

    public void Add(string key, object? value)
        => SetValue(key, JsValue.FromObject(value));

    public bool ContainsKey(string key) => _shape.GetSlot(key) != -1;

    public bool Remove(string key)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
            return false;

        _shape = _shape.TransitionAway(key);
        _properties = _properties.Where((v, i) => i != slot).ToArray();

        return true;
    }

    public bool TryGetValue(string key, out object? value)
    {
        if (this.TryGetJsValue(key, out var jv))
        {
            value = jv.ToObject();
            return true;
        }
        value = null;
        return false;
    }

    public void Add(KeyValuePair<string, object?> item)
    {
        SetValue(item.Key, JsValue.FromObject(item.Value));
    }

    public void Clear()
    {
        _properties = System.Array.Empty<JsValue>();
        _shape = JsShape.Empty;
    }

    public bool Contains(KeyValuePair<string, object?> item)
        => _properties is not null
           && TryGetJsValue(item.Key, out var jv)
           && Equals(jv.ToObject(), item.Value);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    public bool Remove(KeyValuePair<string, object?> item)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        foreach (var key in _shape.PropertyNames)
        {
            if (TryGetJsValue(key, out var jv))
                yield return new KeyValuePair<string, object?>(key, jv.ToObject());
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------
    private void SetValue(string key, JsValue value)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
        {
            _shape = _shape.TransitionTo(key);
            slot = _shape.GetSlot(key);

            var newProperties = new JsValue[_properties!.Length + 1];
            System.Array.Copy(_properties, newProperties, _properties.Length);
            _properties = newProperties;
        }

        _properties[slot] = value;
    }

    private JsValue GetValue(string key)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
            throw new KeyNotFoundException($"Key '{key}' not found.");
        return _properties[slot];
    }

    private bool TryGetJsValue(string key, out JsValue value)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
        {
            value = JsValue.Undefined;
            return false;
        }
        value = _properties[slot];
        return true;
    }

    private void DefineDataDescriptor(string key, object? value)
        => PropertyDescriptorStore.DefineOrUpdate(this, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value,
            Writable = true,
            Enumerable = true,
            Configurable = true
        });
}
