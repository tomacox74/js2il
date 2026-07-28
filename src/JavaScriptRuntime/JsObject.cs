using System;
using System.Collections;

namespace JavaScriptRuntime;

/// <summary>
/// Marks a <see cref="JsObject"/> subclass that overrides ECMAScript internal
/// object operations for specialized storage.
/// </summary>
/// <remarks>
/// Generic dispatch uses the virtual hooks on every <see cref="JsObject"/>. This
/// marker only identifies paths where ordinary objects can use specialized
/// non-virtual fast paths.
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
public partial class JsObject : IDictionary<string, object?>
{
    // -------------------------------------------------------------------------
    // ECMAScript internal object-operation hooks
    // -------------------------------------------------------------------------

    /// <summary>
    /// Looks up an own descriptor. Descriptors are value types, so callers receive
    /// an independent copy synthesized from inline storage or a runtime overlay.
    /// </summary>
    /// <remarks>
    /// Exotic subclasses must preserve descriptor-store tombstones and overrides
    /// before synthesizing descriptors for specialized storage.
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
            descriptor = default;
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
        => TryGetStoredBoxedValue(key, out value);

    /// <summary>
    /// Reads an own value whose property invariants make descriptor state irrelevant.
    /// Returning false defers to normal descriptor-aware resolution.
    /// </summary>
    internal virtual bool TryGetInvariantOwnPropertyValue(string key, out object? value)
    {
        value = null;
        return false;
    }

    /// <summary>Tests specialized backing storage for an own property without reading its value.</summary>
    internal virtual bool HasOwnPropertyValue(string key)
        => ContainsKey(key);

    /// <summary>
    /// Defines or updates an own property while keeping ordinary backing storage
    /// and descriptor state synchronized.
    /// </summary>
    internal virtual bool DefineOwnProperty(string key, JsPropertyDescriptor descriptor)
    {
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
        PropertyDescriptorStore.Delete(this, key);
        return true;
    }

    /// <summary>
    /// Returns every own key in ECMAScript encounter order, including keys held
    /// only in descriptor or specialized storage.
    /// </summary>
    /// <remarks>
    /// Subclasses with specialized storage must override this method and merge
    /// all of their key sources.
    /// </remarks>
    internal virtual IEnumerable<string> GetOwnPropertyKeys()
    {
        var keys = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var key in PropertyDescriptorStore.GetOwnKeys(this))
        {
            if (seen.Add(key))
            {
                keys.Add(key);
            }
        }

        foreach (var key in GetOwnPropertyNames())
        {
            if (!PropertyDescriptorStore.IsDeleted(this, key) && seen.Add(key))
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    // -------------------------------------------------------------------------
    // Typed initializer methods used from generated IL (no boxing at call site)
    // -------------------------------------------------------------------------

    /// <summary>Stores a numeric property without boxing the double value.</summary>
    public virtual void SetNumber(string key, double value)
        => SetValue(key, JsValue.FromNumber(value));

    /// <summary>Stores a boolean property without boxing the bool value.</summary>
    public virtual void SetBoolean(string key, bool value)
        => SetValue(key, JsValue.FromBoolean(value));

    /// <summary>Stores a string property.</summary>
    public virtual void SetString(string key, string? value)
        => SetValue(key, JsValue.FromString(value));

    /// <summary>Stores an arbitrary object value.</summary>
    public virtual void SetValue(string key, object? value)
        => SetValue(key, JsValue.FromObject(value));

    /// <summary>Stores an arbitrary object value (alias used by newer IL emit paths).</summary>
    public void SetObject(string key, object? value)
        => SetValue(key, value);

    // -------------------------------------------------------------------------
    // Read helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Tries to read this object's shape/slot storage as a boxed CLR object.
    /// </summary>
    public bool TryGetBoxedValue(string key, out object? value)
        => TryGetStoredBoxedValue(key, out value);

    /// <summary>
    /// Resolves an own JavaScript property while keeping descriptor and exotic-object
    /// semantics internal and preserving the original receiver for inherited accessors.
    /// </summary>
    internal virtual bool TryGetBoxedValue(
        string key,
        object receiverForAccessors,
        out object? value)
    {
        var isExotic = this is IExoticJsObject;
        if (isExotic && TryGetInvariantOwnPropertyValue(key, out value))
        {
            return true;
        }

        if (!HasNonDataDescriptors
            && TryGetOwnPropertyValue(key, out value))
        {
            return true;
        }

        // Value reads need only inline metadata and stored overlays. Calling the
        // semantic descriptor hook here would force exotic objects to materialize
        // synthetic descriptors for specialized backing storage.
        var lookup = PropertyDescriptorStore.GetOwnLookupCore(this, key, out var descriptor);
        if (lookup == PropertyDescriptorLookup.Deleted)
        {
            value = null;
            return false;
        }

        if (lookup == PropertyDescriptorLookup.Found)
        {
            if (descriptor.Kind == JsPropertyDescriptorKind.Accessor)
            {
                value = descriptor.Get is null || descriptor.Get is JsNull
                    ? null
                    : ObjectRuntime.InvokeCallable(
                        descriptor.Get,
                        receiverForAccessors,
                        System.Array.Empty<object>());
                return true;
            }

            value = descriptor.Value;
            return true;
        }

        if (TryGetOwnPropertyValue(key, out value))
        {
            return true;
        }

        if (RuntimeServices.TryEnsureLazyClassMethodDataProperty(
            this,
            key,
            out var lazyClassMethodDescriptor))
        {
            value = lazyClassMethodDescriptor.Value;
            return true;
        }

        value = null;
        return false;
    }

    private bool TryGetStoredBoxedValue(string key, out object? value)
    {
        if (TryGetJsValue(key, out var jsValue))
        {
            value = jsValue.ToObject();
            return true;
        }

        value = null;
        return false;
    }

    internal void SetBoxedValue(string key, object? value)
        => SetValue(key, JsValue.FromObject(value));

    /// <summary>Returns enumerable sequence of own property names.</summary>
    public IEnumerable<string> GetOwnPropertyNames()
        => _shape.EnumeratePropertyNamesSnapshot();

    /// <summary>Returns own property key-value pairs (values boxed as object).</summary>
    public IEnumerable<KeyValuePair<string, object?>> GetOwnProperties()
    {
        var shape = _shape;
        var properties = _properties;
        for (var slot = 0; slot < shape.PropertyCount; slot++)
        {
            yield return new KeyValuePair<string, object?>(
                shape.GetPropertyNameAtSlot(slot),
                properties[slot].ToObject());
        }
    }

    // -------------------------------------------------------------------------
    // IDictionary<string, object?> implementation
    // Values are converted to/from JsValue at the interface boundary.
    // -------------------------------------------------------------------------

    public virtual object? this[string key]
    {
        get => GetValue(key).ToObject();
        set => SetValue(key, JsValue.FromObject(value));
    }

    public ICollection<string> Keys
    {
        get
        {
            var propertyNames = _shape.PropertyNamesInSlotOrder;
            var keys = new List<string>(propertyNames.Length);
            foreach (var name in propertyNames)
            {
                keys.Add(name);
            }
            return keys;
        }
    }

    public ICollection<object?> Values
    {
        get
        {
            var values = new List<object?>(_properties.Length);
            foreach (var value in _properties)
            {
                values.Add(value.ToObject());
            }
            return values;
        }
    }

    public int Count => _properties!.Length;

    public bool IsReadOnly => false;

    public virtual void Add(string key, object? value)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException($"An item with the same key has already been added: {key}", nameof(key));
        }

        SetValue(key, JsValue.FromObject(value));
    }

    public virtual bool ContainsKey(string key) => _shape.GetSlot(key) != -1;

    public virtual bool Remove(string key)
        => RemoveBoxedValue(key);

    internal bool RemoveBoxedValue(string key)
    {
        if (HasSharedIntrinsicBaseline)
        {
            return PropertyDescriptorStore.Delete(this, key);
        }

        return DeleteInlineOwnDescriptor(key);
    }

    public virtual bool TryGetValue(string key, out object? value)
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
        if (HasSharedIntrinsicBaseline)
        {
            PropertyDescriptorStore.Clear(this);
            return;
        }

        _properties = System.Array.Empty<JsValue>();
        _shape = JsShape.Empty;
        ResetInlineDescriptorState();
        AssertInlineInvariants();
    }

    public virtual bool Contains(KeyValuePair<string, object?> item)
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
        var shape = _shape;
        var properties = _properties;
        for (var slot = 0; slot < shape.PropertyCount; slot++)
        {
            yield return new KeyValuePair<string, object?>(
                shape.GetPropertyNameAtSlot(slot),
                properties[slot].ToObject());
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
