using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JavaScriptRuntime;

public partial class JsObject
{
    [Flags]
    private enum JsSlotDescriptorFlags : byte
    {
        None = 0,
        HasMetadata = 1,
        IsAccessor = 2,
        Writable = 4,
        Enumerable = 8,
        Configurable = 16
    }

    private struct JsAccessorPair
    {
        public object? Get;
        public object? Set;
    }

    private sealed class JsObjectDescriptorState
    {
        public required byte[] Flags;
        public JsAccessorPair[]? Accessors;
        public Dictionary<string, JsPropertyDescriptor>? ExoticOverrides;
        public HashSet<string>? DeletedLazyClassMethods;
        public bool HasSharedIntrinsicBaseline;
    }

    private JsObjectDescriptorState? _descriptorState;

    // Sticky between ordinary mutations, but reset by Clear. While false, every
    // shape slot is an implicit writable/enumerable/configurable data descriptor.
    private bool _hasNonDataDescriptors;

    /// <summary>
    /// True when own reads can no longer be answered from the property dictionary
    /// alone (the object has accessors, deleted tombstones, or attribute-bearing
    /// descriptors). It remains sticky until the object is cleared.
    /// </summary>
    internal bool HasNonDataDescriptors => _hasNonDataDescriptors;

    internal void MarkNonDataDescriptors() => _hasNonDataDescriptors = true;
    internal bool HasInlineDescriptorState => _descriptorState is not null;
    internal bool HasSharedIntrinsicBaseline => _descriptorState?.HasSharedIntrinsicBaseline == true;
    internal bool HasInlineDescriptors => _shape.PropertyCount != 0 || _descriptorState?.ExoticOverrides?.Count > 0;
    internal bool HasInlineExoticDescriptors => _descriptorState?.ExoticOverrides?.Count > 0;

    internal virtual bool UsesInlineExoticDescriptorStorage(string key) => false;

    internal void MarkSharedIntrinsicBaseline()
    {
        var state = EnsureDescriptorState();
        state.HasSharedIntrinsicBaseline = true;
        MarkNonDataDescriptors();
        AssertInlineInvariants();
    }

    internal bool GetInlineOwnDescriptor(string key, out JsPropertyDescriptor descriptor)
    {
        var slot = _shape.GetSlot(key);
        if (slot < 0)
        {
            descriptor = default;
            return false;
        }

        var flags = _descriptorState is null
            ? JsSlotDescriptorFlags.None
            : (JsSlotDescriptorFlags)_descriptorState.Flags[slot];
        if (flags == JsSlotDescriptorFlags.None)
        {
            descriptor = CreateDefaultDataDescriptor(_properties[slot]);
            return true;
        }

        if ((flags & JsSlotDescriptorFlags.IsAccessor) != 0)
        {
            Debug.Assert(_descriptorState?.Accessors is not null);
            var accessor = _descriptorState!.Accessors![slot];
            descriptor = new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Accessor,
                Get = accessor.Get,
                Set = accessor.Set,
                Enumerable = (flags & JsSlotDescriptorFlags.Enumerable) != 0,
                Configurable = (flags & JsSlotDescriptorFlags.Configurable) != 0
            };
            return true;
        }

        descriptor = new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = _properties[slot].ToObject(),
            Writable = (flags & JsSlotDescriptorFlags.Writable) != 0,
            Enumerable = (flags & JsSlotDescriptorFlags.Enumerable) != 0,
            Configurable = (flags & JsSlotDescriptorFlags.Configurable) != 0
        };
        return true;
    }

    internal void DefineInlineOwnDescriptor(string key, JsPropertyDescriptor descriptor)
    {
        var slot = EnsurePropertySlot(key);
        if (descriptor.Kind == JsPropertyDescriptorKind.Data)
        {
            _properties[slot] = JsValue.FromObject(descriptor.Value);
            if (IsDefaultDataDescriptor(descriptor))
            {
                ClearSlotMetadata(slot);
                AssertInlineInvariants();
                return;
            }

            var state = EnsureDescriptorState();
            state.Flags[slot] = (byte)(
                JsSlotDescriptorFlags.HasMetadata
                | (descriptor.Writable ? JsSlotDescriptorFlags.Writable : 0)
                | (descriptor.Enumerable ? JsSlotDescriptorFlags.Enumerable : 0)
                | (descriptor.Configurable ? JsSlotDescriptorFlags.Configurable : 0));
            if (state.Accessors is not null)
            {
                state.Accessors[slot] = default;
            }
        }
        else
        {
            _properties[slot] = JsValue.Undefined;
            var state = EnsureDescriptorState();
            state.Accessors ??= new JsAccessorPair[_shape.PropertyCount];
            state.Flags[slot] = (byte)(
                JsSlotDescriptorFlags.HasMetadata
                | JsSlotDescriptorFlags.IsAccessor
                | (descriptor.Enumerable ? JsSlotDescriptorFlags.Enumerable : 0)
                | (descriptor.Configurable ? JsSlotDescriptorFlags.Configurable : 0));
            state.Accessors[slot] = new JsAccessorPair { Get = descriptor.Get, Set = descriptor.Set };
        }

        MarkNonDataDescriptors();
        AssertInlineInvariants();
    }

    internal bool DeleteInlineOwnDescriptor(string key)
    {
        var slot = _shape.GetSlot(key);
        if (slot < 0)
        {
            return false;
        }

        var newProperties = new JsValue[_properties.Length - 1];
        CopyAroundRemovedSlot(_properties, newProperties, slot);

        if (_descriptorState is { } state)
        {
            var newFlags = new byte[state.Flags.Length - 1];
            CopyAroundRemovedSlot(state.Flags, newFlags, slot);
            state.Flags = newFlags;

            if (state.Accessors is not null)
            {
                var newAccessors = new JsAccessorPair[state.Accessors.Length - 1];
                CopyAroundRemovedSlot(state.Accessors, newAccessors, slot);
                state.Accessors = newAccessors;
            }
        }

        _shape = _shape.TransitionAway(key);
        _properties = newProperties;
        AssertInlineInvariants();
        return true;
    }

    internal IEnumerable<string> GetInlineOwnDescriptorKeys() => _shape.EnumeratePropertyNamesSnapshot();
    internal IEnumerable<string> GetInlineExoticDescriptorKeys() => _descriptorState?.ExoticOverrides?.Keys.ToArray() ?? System.Array.Empty<string>();

    internal bool GetInlineExoticOwnDescriptor(string key, out JsPropertyDescriptor descriptor)
    {
        if (_descriptorState?.ExoticOverrides is { } overrides && overrides.TryGetValue(key, out descriptor))
        {
            return true;
        }

        descriptor = default;
        return false;
    }

    internal void DefineInlineExoticOwnDescriptor(string key, JsPropertyDescriptor descriptor)
    {
        var state = EnsureDescriptorState();
        state.ExoticOverrides ??= new Dictionary<string, JsPropertyDescriptor>(StringComparer.Ordinal);
        state.ExoticOverrides[key] = descriptor;
        MarkNonDataDescriptors();
        AssertInlineInvariants();
    }

    internal bool DeleteInlineExoticOwnDescriptor(string key)
    {
        var deleted = _descriptorState?.ExoticOverrides?.Remove(key) == true;
        AssertInlineInvariants();
        return deleted;
    }

    internal bool IsInlineLazyClassMethodDeleted(string key)
        => _descriptorState?.DeletedLazyClassMethods?.Contains(key) == true;

    internal void MarkInlineLazyClassMethodDeleted(string key)
    {
        var state = EnsureDescriptorState();
        state.DeletedLazyClassMethods ??= new HashSet<string>(StringComparer.Ordinal);
        state.DeletedLazyClassMethods.Add(key);
        MarkNonDataDescriptors();
        AssertInlineInvariants();
    }

    internal void ResetInlineDescriptorState()
    {
        Debug.Assert(!HasSharedIntrinsicBaseline);
        _descriptorState = null;
        _hasNonDataDescriptors = false;
        AssertInlineInvariants();
    }

    private JsObjectDescriptorState EnsureDescriptorState()
        => _descriptorState ??= new JsObjectDescriptorState { Flags = new byte[_shape.PropertyCount] };

    private void ClearSlotMetadata(int slot)
    {
        if (_descriptorState is not { } state)
        {
            return;
        }

        state.Flags[slot] = 0;
        if (state.Accessors is not null)
        {
            state.Accessors[slot] = default;
        }
    }

    private static bool IsDefaultDataDescriptor(JsPropertyDescriptor descriptor)
        => descriptor.Kind == JsPropertyDescriptorKind.Data
            && descriptor.Writable
            && descriptor.Enumerable
            && descriptor.Configurable;

    private static JsPropertyDescriptor CreateDefaultDataDescriptor(JsValue value)
        => new()
        {
            Kind = JsPropertyDescriptorKind.Data,
            Value = value.ToObject(),
            Writable = true,
            Enumerable = true,
            Configurable = true
        };

    private static void CopyAroundRemovedSlot<T>(T[] source, T[] destination, int slot)
    {
        if (slot > 0)
        {
            System.Array.Copy(source, 0, destination, 0, slot);
        }

        if (slot < destination.Length)
        {
            System.Array.Copy(source, slot + 1, destination, slot, destination.Length - slot);
        }
    }

    [Conditional("DEBUG")]
    private void AssertInlineInvariants()
    {
        Debug.Assert(_properties.Length == _shape.PropertyCount);
        if (_descriptorState is not { } state)
        {
            return;
        }

        Debug.Assert(state.Flags.Length == _shape.PropertyCount);
        Debug.Assert(state.Accessors is null || state.Accessors.Length == _shape.PropertyCount);
        for (var slot = 0; slot < state.Flags.Length; slot++)
        {
            var flags = (JsSlotDescriptorFlags)state.Flags[slot];
            Debug.Assert(flags == JsSlotDescriptorFlags.None || (flags & JsSlotDescriptorFlags.HasMetadata) != 0);
            Debug.Assert((flags & JsSlotDescriptorFlags.IsAccessor) == 0 || state.Accessors is not null);
        }

        if (state.ExoticOverrides is not null)
        {
            foreach (var key in state.ExoticOverrides.Keys)
            {
                Debug.Assert(UsesInlineExoticDescriptorStorage(key));
                Debug.Assert(_shape.GetSlot(key) < 0);
            }
        }
    }
}
