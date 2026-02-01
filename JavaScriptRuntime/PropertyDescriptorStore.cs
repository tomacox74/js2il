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
    }

    private static readonly ConditionalWeakTable<object, Slot> _slots = new();

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

        var slot = _slots.GetOrCreateValue(target);
        slot.Descriptors[key] = descriptor;
    }

    public static bool Delete(object target, string key)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (_slots.TryGetValue(target, out var slot))
        {
            return slot.Descriptors.Remove(key);
        }

        return false;
    }

    public static bool IsEnumerableOrDefaultTrue(object target, string key)
    {
        return TryGetOwn(target, key, out var desc) ? desc.Enumerable : true;
    }
}
