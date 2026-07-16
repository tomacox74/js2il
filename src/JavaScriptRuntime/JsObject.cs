using System;
using System.Collections;
using System.Collections.Frozen;
using System.Dynamic;

namespace JavaScriptRuntime;

internal sealed class JsShape
{
    private enum PropertyNameStorage
    {
        Interned,
        Direct
    }

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

    private JsShape(string newPropertyName, JsShape parent, PropertyNameStorage propertyNameStorage)
    {
        var newSlots = parent._slots.ToDictionary();
        newSlots[propertyNameStorage == PropertyNameStorage.Interned ? string.Intern(newPropertyName) : newPropertyName] = newSlots.Count;
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
        var newShape = new JsShape(newPropertyName, this, PropertyNameStorage.Interned);
        _transitions![newPropertyName] = new WeakReference<JsShape>(newShape);
        return newShape;
    }

    /// <summary>
    /// Adds a property name without publishing it to the shared shape-transition cache.
    /// This is for objects populated from untrusted input where neither global string
    /// interning nor long-lived transition-cache keys are appropriate.
    /// </summary>
    public JsShape TransitionToUncached(string newPropertyName)
        => new JsShape(newPropertyName, this, PropertyNameStorage.Direct);

    public JsShape TransitionAway(string deadPropertyName)
    {
        return new JsShape(deadPropertyName, this, true);
    }

    public int GetSlot(string propertyName)
        => _slots.TryGetValue(propertyName, out var slot) ? slot : -1;
}

/// <summary>
/// Marks a <see cref="JsObject"/> subclass that overrides ECMAScript internal
/// object operations for specialized storage.
/// </summary>
/// <remarks>
/// Keeping this opt-in explicit lets ordinary and generated JsObject subclasses
/// retain direct, non-virtual hot paths.
/// </remarks>
internal interface IExoticJsObject
{
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
public class JsObject : DynamicObject, IDictionary<string, object?>
{
    private JsValue[] _properties = System.Array.Empty<JsValue>();

    private JsShape _shape = JsShape.Empty;

    private readonly bool _cacheShapeTransitions;

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

    /// <summary>Creates an ordinary object using the shared shape-transition cache.</summary>
    public JsObject()
    {
        _cacheShapeTransitions = true;
    }

    /// <summary>
    /// Creates an ordinary object whose property names are appended without using shared
    /// shape transitions. Intended for records populated from untrusted property keys.
    /// </summary>
    internal JsObject(bool cacheShapeTransitions)
    {
        _cacheShapeTransitions = cacheShapeTransitions;
    }

    // -------------------------------------------------------------------------
    // ECMAScript internal object-operation hooks
    // -------------------------------------------------------------------------

    /// <summary>
    /// Looks up an own descriptor without cloning it. The returned descriptor is
    /// shared descriptor-store state and must be cloned before mutation.
    /// </summary>
    /// <remarks>
    /// Exotic subclasses must preserve descriptor-store tombstones and overrides
    /// before synthesizing descriptors for specialized storage and implement
    /// <see cref="IExoticJsObject"/> to opt generic dispatch into these hooks.
    /// </remarks>
    internal virtual PropertyDescriptorLookup GetOwnPropertyDescriptor(
        string key,
        out JsPropertyDescriptor descriptor)
    {
        var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out descriptor);
        if (lookup != PropertyDescriptorLookup.None)
        {
            return lookup;
        }

        if (!TryGetOwnPropertyValue(key, out var value))
        {
            descriptor = null!;
            return PropertyDescriptorLookup.None;
        }

        descriptor = new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value,
            Writable = true,
            Enumerable = true,
            Configurable = true
        };
        return PropertyDescriptorLookup.Found;
    }

    /// <summary>
    /// Reads an own value from this object's specialized backing storage.
    /// Keys are canonical runtime property-key strings; symbols remain encoded
    /// keys and are never converted to display strings by this contract.
    /// </summary>
    internal virtual bool TryGetOwnPropertyValue(string key, out object? value)
        => TryGetBoxedValue(key, out value);

    /// <summary>
    /// Defines or updates an own property while keeping ordinary backing storage
    /// and descriptor state synchronized.
    /// </summary>
    internal virtual bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (!PropertyDescriptorStore.HasIntrinsicProperties(this))
        {
            if (!SetOwnPropertyValue(
                key,
                descriptor.Kind == JsPropertyDescriptorKind.Accessor ? null : descriptor.Value))
            {
                return false;
            }
        }

        PropertyDescriptorStore.DefineOrUpdate(this, key, descriptor);
        return true;
    }

    /// <summary>Writes an own value to this object's specialized backing storage.</summary>
    internal virtual bool SetOwnPropertyValue(string key, object? value)
    {
        SetBoxedValue(key, value);
        return true;
    }

    /// <summary>Deletes an own property from backing and descriptor storage.</summary>
    internal virtual bool DeleteOwnProperty(string key)
    {
        if (!PropertyDescriptorStore.HasIntrinsicProperties(this))
        {
            Remove(key);
        }

        PropertyDescriptorStore.Delete(this, key);
        return true;
    }

    /// <summary>
    /// Returns every own key in ECMAScript encounter order, including keys held
    /// only in descriptor or specialized storage.
    /// </summary>
    /// <remarks>
    /// <see cref="IExoticJsObject"/> implementations with specialized storage
    /// must override this method and merge all of their key sources.
    /// </remarks>
    internal virtual IEnumerable<string> GetOwnPropertyKeys()
        => GetOwnPropertyNames();

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
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException($"An item with the same key has already been added: {key}", nameof(key));
        }

        SetValue(key, JsValue.FromObject(value));
    }

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
        Add(item.Key, item.Value);
    }

    public virtual void Clear()
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
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("The destination array does not have enough space.", nameof(array));
        }

        foreach (var property in this)
        {
            array[arrayIndex++] = property;
        }
    }

    public bool Remove(KeyValuePair<string, object?> item)
    {
        return TryGetValue(item.Key, out var value)
            && EqualityComparer<object?>.Default.Equals(value, item.Value)
            && Remove(item.Key);
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

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        => TryGetValue(binder.Name, out result);

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        this[binder.Name] = value;
        return true;
    }

    public override bool TryDeleteMember(DeleteMemberBinder binder)
        => Remove(binder.Name);

    public override IEnumerable<string> GetDynamicMemberNames()
        => GetOwnPropertyNames();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------
    private void SetValue(string key, JsValue value)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
        {
            _shape = _cacheShapeTransitions
                ? _shape.TransitionTo(key)
                : _shape.TransitionToUncached(key);
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
