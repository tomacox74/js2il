using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

internal enum JsPropertyDescriptorKind
{
    Data,
    Accessor
}

internal sealed class JsPropertyDescriptor
{
    public JsPropertyDescriptorKind Kind { get; set; }

    // Common attributes
    public bool Enumerable { get; set; }
    public bool Configurable { get; set; }

    // Data descriptor
    public object? Value { get; set; }
    public bool Writable { get; set; }

    // Accessor descriptor
    public object? Get { get; set; }
    public object? Set { get; set; }
}

internal static class PropertyDescriptorStore
{
    private sealed class Slot
    {
        public readonly Dictionary<string, JsPropertyDescriptor> Descriptors = new(StringComparer.Ordinal);
        public readonly List<string> KeyOrder = new();
    }

    private static readonly ConditionalWeakTable<object, Slot> _slots = new();

    private static bool TryGetMirroredRawClassPrototype(object target, out object mirroredTarget)
    {
        mirroredTarget = null!;

        if (!_slots.TryGetValue(target, out var slot)
            || !slot.Descriptors.TryGetValue("constructor", out var constructorDescriptor)
            || constructorDescriptor.Kind != JsPropertyDescriptorKind.Data
            || constructorDescriptor.Value is not ClassConstructorValue classConstructorValue)
        {
            return false;
        }

        var rawPrototype = Object.GetProperty(classConstructorValue.Type, "prototype");
        if (rawPrototype is not object
            || ReferenceEquals(rawPrototype, target))
        {
            return false;
        }

        mirroredTarget = rawPrototype;
        return true;
    }

    public static bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_slots.TryGetValue(target, out var slot))
        {
            return slot.Descriptors.TryGetValue(key, out descriptor!);
        }

        descriptor = null!;
        return false;
    }

    public static void DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

        if (target is Delegate del)
        {
            Function.ClearDeletedMetadataProperty(del, key);
        }

        var slot = _slots.GetOrCreateValue(target);
        if (!slot.Descriptors.ContainsKey(key))
        {
            slot.KeyOrder.Add(key);
        }
        slot.Descriptors[key] = descriptor;

        if (TryGetMirroredRawClassPrototype(target, out var mirroredTarget))
        {
            var mirroredSlot = _slots.GetOrCreateValue(mirroredTarget);
            if (!mirroredSlot.Descriptors.ContainsKey(key))
            {
                mirroredSlot.KeyOrder.Add(key);
            }

            mirroredSlot.Descriptors[key] = new JsPropertyDescriptor
            {
                Kind = descriptor.Kind,
                Enumerable = descriptor.Enumerable,
                Configurable = descriptor.Configurable,
                Value = descriptor.Value,
                Writable = descriptor.Writable,
                Get = descriptor.Get,
                Set = descriptor.Set
            };
        }
    }

    public static IEnumerable<string> GetOwnKeys(object target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));

        if (_slots.TryGetValue(target, out var slot))
        {
            var keys = slot.KeyOrder.ToArray();
            return keys;
        }

        return System.Array.Empty<string>();
    }

    public static bool Delete(object target, string key)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_slots.TryGetValue(target, out var slot))
        {
            var removed = slot.Descriptors.Remove(key);
            if (removed)
            {
                slot.KeyOrder.Remove(key);
            }

            if (removed && TryGetMirroredRawClassPrototype(target, out var mirroredTarget) && _slots.TryGetValue(mirroredTarget, out var mirroredSlot))
            {
                if (mirroredSlot.Descriptors.Remove(key))
                {
                    mirroredSlot.KeyOrder.Remove(key);
                }
            }

            return removed;
        }

        return false;
    }

    public static bool IsEnumerableOrDefaultTrue(object target, string key)
    {
        return !TryGetOwn(target, key, out var desc) || desc.Enumerable;
    }
}
