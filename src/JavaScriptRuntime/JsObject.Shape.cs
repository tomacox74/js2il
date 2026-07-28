using System;
using System.Collections.Generic;
using System.Threading;

namespace JavaScriptRuntime;

internal sealed class JsShape
{
    internal const int DictionaryPromotionThreshold = 2;

    private enum PropertyNameStorage
    {
        Interned,
        Direct
    }

    private static readonly ThreadLocal<JsShape> _empty = new(() => new JsShape());

    public static JsShape Empty => _empty.Value!;

    private readonly string[] _propertyNames;
    private readonly Dictionary<string, int>? _slotLookup;
    private Dictionary<string, WeakReference<JsShape>>? _transitions;

    public JsShape()
    {
        _propertyNames = System.Array.Empty<string>();
    }

    internal ReadOnlySpan<string> PropertyNamesInSlotOrder => _propertyNames;
    internal int PropertyCount => _propertyNames.Length;
    internal bool UsesDictionaryLookup => _slotLookup is not null;
    internal string GetPropertyNameAtSlot(int slot) => _propertyNames[slot];

    internal IEnumerable<string> EnumeratePropertyNamesSnapshot()
    {
        var propertyNames = _propertyNames;
        for (var slot = 0; slot < propertyNames.Length; slot++)
        {
            yield return propertyNames[slot];
        }
    }

    internal bool HasTransitionCache => _transitions is not null;
    internal int TransitionCacheCount => _transitions?.Count ?? 0;

    private JsShape(string newPropertyName, JsShape parent, PropertyNameStorage propertyNameStorage)
    {
        var storedPropertyName = propertyNameStorage == PropertyNameStorage.Interned
            ? string.Intern(newPropertyName)
            : newPropertyName;
        var parentPropertyNames = parent._propertyNames;
        _propertyNames = new string[parentPropertyNames.Length + 1];
        System.Array.Copy(parentPropertyNames, _propertyNames, parentPropertyNames.Length);
        _propertyNames[parentPropertyNames.Length] = storedPropertyName;

        if (_propertyNames.Length > DictionaryPromotionThreshold)
        {
            _slotLookup = parent._slotLookup is not null
                ? new Dictionary<string, int>(parent._slotLookup, StringComparer.Ordinal)
                : CreateSlotLookup(parentPropertyNames);
            _slotLookup[storedPropertyName] = parentPropertyNames.Length;
        }
    }

    private JsShape(string deadPropertyName, JsShape parent)
    {
        var deadSlot = parent.GetSlot(deadPropertyName);
        if (deadSlot < 0)
        {
            throw new InvalidOperationException($"Cannot remove missing shape property '{deadPropertyName}'.");
        }

        var parentPropertyNames = parent._propertyNames;
        _propertyNames = new string[parentPropertyNames.Length - 1];
        if (deadSlot > 0)
        {
            System.Array.Copy(parentPropertyNames, 0, _propertyNames, 0, deadSlot);
        }
        if (deadSlot < _propertyNames.Length)
        {
            System.Array.Copy(parentPropertyNames, deadSlot + 1, _propertyNames, deadSlot, _propertyNames.Length - deadSlot);
        }

        if (_propertyNames.Length > DictionaryPromotionThreshold)
        {
            _slotLookup = CreateSlotLookup(_propertyNames);
        }
    }

    private static Dictionary<string, int> CreateSlotLookup(ReadOnlySpan<string> propertyNames)
    {
        var slots = new Dictionary<string, int>(propertyNames.Length, StringComparer.Ordinal);
        for (var slot = 0; slot < propertyNames.Length; slot++)
        {
            slots[propertyNames[slot]] = slot;
        }
        return slots;
    }

    public JsShape TransitionTo(string newPropertyName)
    {
        if (_transitions is not null
            && _transitions.TryGetValue(newPropertyName, out var weakRef)
            && weakRef.TryGetTarget(out var existingShape))
        {
            return existingShape;
        }

        var newShape = new JsShape(newPropertyName, this, PropertyNameStorage.Interned);
        var transitions = _transitions ??= new Dictionary<string, WeakReference<JsShape>>();
        transitions[newPropertyName] = new WeakReference<JsShape>(newShape);
        return newShape;
    }

    /// <summary>
    /// Adds a property name without publishing it to the shared shape-transition cache.
    /// This is for objects populated from untrusted property keys.
    /// </summary>
    public JsShape TransitionToUncached(string newPropertyName)
        => new(newPropertyName, this, PropertyNameStorage.Direct);

    public JsShape TransitionAway(string deadPropertyName)
        => new(deadPropertyName, this);

    public int GetSlot(string propertyName)
    {
        if (_slotLookup is not null)
        {
            return _slotLookup.TryGetValue(propertyName, out var slot) ? slot : -1;
        }

        var propertyNames = _propertyNames;
        for (var slot = 0; slot < propertyNames.Length; slot++)
        {
            if (string.Equals(propertyNames[slot], propertyName, StringComparison.Ordinal))
            {
                return slot;
            }
        }
        return -1;
    }
}

public partial class JsObject
{
    private JsValue[] _properties = System.Array.Empty<JsValue>();
    private JsShape _shape = JsShape.Empty;
    private object? _prototype;
    private readonly bool _cacheShapeTransitions;

    internal bool TryGetInlinePrototype(out object? prototype)
        => (prototype = _prototype) is not null;

    internal void SetInlinePrototype(object? prototype) => _prototype = prototype;

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

    private void SetValue(string key, JsValue value)
    {
        if (HasSharedIntrinsicBaseline && !PropertyDescriptorStore.IsIntrinsicInitialization)
        {
            PropertyDescriptorStore.DefineOrUpdate(this, key, CreateDefaultDataDescriptor(value));
            return;
        }

        var slot = EnsurePropertySlot(key);
        _properties[slot] = value;
        ClearSlotMetadata(slot);
        AssertInlineInvariants();
    }

    private JsValue GetValue(string key)
    {
        var slot = _shape.GetSlot(key);
        if (slot == -1)
        {
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }
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

    private int EnsurePropertySlot(string key)
    {
        var slot = _shape.GetSlot(key);
        if (slot >= 0)
        {
            return slot;
        }

        _shape = _cacheShapeTransitions
            ? _shape.TransitionTo(key)
            : _shape.TransitionToUncached(key);
        slot = _shape.GetSlot(key);

        var newProperties = new JsValue[_properties.Length + 1];
        System.Array.Copy(_properties, newProperties, _properties.Length);
        _properties = newProperties;

        if (_descriptorState is { } state)
        {
            System.Array.Resize(ref state.Flags, _shape.PropertyCount);
            if (state.Accessors is not null)
            {
                System.Array.Resize(ref state.Accessors, _shape.PropertyCount);
            }
        }

        AssertInlineInvariants();
        return slot;
    }
}
