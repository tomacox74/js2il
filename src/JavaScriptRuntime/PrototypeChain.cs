using System;
using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

/// <summary>
/// Minimal runtime feature gate and storage for ECMAScript-like [[Prototype]] chains.
///
/// This is intentionally opt-in: JS2IL emits a prologue call to <see cref="Enable"/> only
/// when the compiler detects prototype-related usage (or when forced via compiler options).
/// </summary>
public static class PrototypeChain
{
    private sealed class PrototypeSlot
    {
        public object? Prototype;
    }

    private static readonly ConditionalWeakTable<object, PrototypeSlot> _slots = new();

    // Volatile ensures other threads observe the enabled flag without additional locking.
    private static volatile bool _enabled;

    public static bool Enabled => _enabled;

    public static void Enable()
    {
        _enabled = true;
    }

    public static bool TryGetPrototype(object obj, out object? prototype)
    {
        if (!_enabled)
        {
            prototype = null;
            return false;
        }

        if (obj == null) throw new ArgumentNullException(nameof(obj));

        if (_slots.TryGetValue(obj, out var slot))
        {
            prototype = slot.Prototype;
            return true;
        }

        prototype = null;
        return false;
    }

    public static object? GetPrototypeOrNull(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return _enabled && _slots.TryGetValue(obj, out var slot)
            ? slot.Prototype
            : null;
    }

    public static void SetPrototype(object obj, object? prototype)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        // If someone calls SetPrototype directly, treat it as explicit opt-in.
        Enable();

        var slot = _slots.GetOrCreateValue(obj);
        slot.Prototype = prototype;
    }
}
