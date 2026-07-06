using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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

internal interface IPropertyDescriptorStore
{
    bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor);
    bool HasAny(object target);
    void DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor);
    IEnumerable<string> GetOwnKeys(object target);
    bool Delete(object target, string key);
    void Clear(object target);
    bool IsEnumerableOrDefaultTrue(object target, string key);
}

internal enum PropertyDescriptorOverrideKind
{
    Add,
    Modify,
    Delete
}

internal sealed class PropertyDescriptorStore : IPropertyDescriptorStore
{
    private sealed class DescriptorSlot
    {
        public readonly object SyncRoot = new();
        public readonly Dictionary<string, JsPropertyDescriptor> Descriptors = new(StringComparer.Ordinal);
        public readonly List<string> KeyOrder = new();
    }

    private sealed class OverrideSlot
    {
        public readonly object SyncRoot = new();
        public readonly Dictionary<string, PropertyDescriptorOverride> Overrides = new(StringComparer.Ordinal);
        public readonly List<string> KeyOrder = new();
    }

    private sealed class PropertyDescriptorOverride
    {
        public required PropertyDescriptorOverrideKind Kind { get; init; }
        public JsPropertyDescriptor? Descriptor { get; init; }
    }

    private sealed class IntrinsicPropertyDescriptorStore : IPropertyDescriptorStore
    {
        private readonly ConditionalWeakTable<object, DescriptorSlot> _slots = new();

        public bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
        {
            ValidateTargetAndKey(target, key);

            if (_slots.TryGetValue(target, out var slot))
            {
                lock (slot.SyncRoot)
                {
                    if (slot.Descriptors.TryGetValue(key, out var stored))
                    {
                        descriptor = CloneDescriptor(stored);
                        return true;
                    }
                }
            }

            descriptor = null!;
            return false;
        }

        public bool HasAny(object target)
        {
            ArgumentNullException.ThrowIfNull(target);

            if (!_slots.TryGetValue(target, out var slot))
            {
                return false;
            }

            lock (slot.SyncRoot)
            {
                return slot.Descriptors.Count != 0;
            }
        }

        public void DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
        {
            ValidateTargetAndKey(target, key);
            ArgumentNullException.ThrowIfNull(descriptor);

            DefineOrUpdateCore(target, key, descriptor);

            if (TryGetMirroredRawClassPrototype(target, TryGetOwn, out var mirroredTarget))
            {
                DefineOrUpdateCore(mirroredTarget, key, descriptor);
            }
        }

        public IEnumerable<string> GetOwnKeys(object target)
        {
            ArgumentNullException.ThrowIfNull(target);

            if (_slots.TryGetValue(target, out var slot))
            {
                lock (slot.SyncRoot)
                {
                    return slot.KeyOrder.ToArray();
                }
            }

            return System.Array.Empty<string>();
        }

        public bool Delete(object target, string key)
        {
            ValidateTargetAndKey(target, key);

            var removed = DeleteCore(target, key);
            if (removed
                && TryGetMirroredRawClassPrototype(target, TryGetOwn, out var mirroredTarget))
            {
                _ = DeleteCore(mirroredTarget, key);
            }

            return removed;
        }

        public void Clear(object target)
        {
            ArgumentNullException.ThrowIfNull(target);
            _slots.Remove(target);
        }

        public bool IsEnumerableOrDefaultTrue(object target, string key)
            => !TryGetOwn(target, key, out var desc) || desc.Enumerable;

        private void DefineOrUpdateCore(object target, string key, JsPropertyDescriptor descriptor)
        {
            if (target is Delegate del)
            {
                Function.ClearDeletedMetadataProperty(del, key);
            }

            var slot = _slots.GetOrCreateValue(target);
            lock (slot.SyncRoot)
            {
                if (!slot.Descriptors.ContainsKey(key))
                {
                    slot.KeyOrder.Add(key);
                }

                slot.Descriptors[key] = CloneDescriptor(descriptor);
            }
        }

        private bool DeleteCore(object target, string key)
        {
            if (!_slots.TryGetValue(target, out var slot))
            {
                return false;
            }

            lock (slot.SyncRoot)
            {
                var removed = slot.Descriptors.Remove(key);
                if (removed)
                {
                    slot.KeyOrder.Remove(key);
                }

                return removed;
            }
        }
    }

    private static readonly IntrinsicPropertyDescriptorStore _intrinsicStore = new();
    private static readonly ThreadLocal<IPropertyDescriptorStore?> _currentRuntimeStore = new(() => null);
    private static readonly ThreadLocal<int> _intrinsicInitializationDepth = new(() => 0);

    private readonly ConditionalWeakTable<object, OverrideSlot> _overrideSlots = new();

    public PropertyDescriptorStore()
    {
    }

    internal static void SetCurrentRuntimeStore(IPropertyDescriptorStore? store)
        => _currentRuntimeStore.Value = store;

    internal static IDisposable BeginIntrinsicInitialization()
    {
        _intrinsicInitializationDepth.Value++;
        return new IntrinsicInitializationScope();
    }

    internal static JsPropertyDescriptor CloneDescriptor(JsPropertyDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        return new JsPropertyDescriptor
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

    public static bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
        => CurrentStore.TryGetOwn(target, key, out descriptor);

    public static bool HasAny(object target)
        => CurrentStore.HasAny(target);

    internal static bool HasIntrinsicProperties(object target)
        => _intrinsicStore.HasAny(target);

    internal static bool IsDeleted(object target, string key)
    {
        ValidateTargetAndKey(target, key);
        return CurrentStore is PropertyDescriptorStore runtimeStore
            && runtimeStore.HasDeletedOverride(target, key);
    }

    public static void DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
        => CurrentStore.DefineOrUpdate(target, key, descriptor);

    internal static void CopyOwnProperties(object source, object target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        foreach (var key in GetOwnKeys(source))
        {
            if (TryGetOwn(source, key, out var descriptor))
            {
                DefineOrUpdate(target, key, descriptor);
            }
        }
    }

    public static IEnumerable<string> GetOwnKeys(object target)
        => CurrentStore.GetOwnKeys(target);

    public static bool Delete(object target, string key)
        => CurrentStore.Delete(target, key);

    internal static void Clear(object target)
        => CurrentStore.Clear(target);

    public static bool IsEnumerableOrDefaultTrue(object target, string key)
        => CurrentStore.IsEnumerableOrDefaultTrue(target, key);

    bool IPropertyDescriptorStore.TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
    {
        ValidateTargetAndKey(target, key);

        if (TryGetOverride(target, key, out var entry))
        {
            if (entry.Kind == PropertyDescriptorOverrideKind.Delete)
            {
                descriptor = null!;
                return false;
            }

            descriptor = CloneDescriptor(entry.Descriptor!);
            return true;
        }

        return _intrinsicStore.TryGetOwn(target, key, out descriptor);
    }

    bool IPropertyDescriptorStore.HasAny(object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (!_overrideSlots.TryGetValue(target, out var slot))
        {
            // No overrides for this target: intrinsic descriptors decide the answer
            // without materializing the ordered key list.
            return _intrinsicStore.HasAny(target);
        }

        foreach (var key in _intrinsicStore.GetOwnKeys(target))
        {
            if (!IsIntrinsicKeySuppressedByOverride(target, key))
            {
                return true;
            }
        }

        lock (slot.SyncRoot)
        {
            foreach (var key in slot.KeyOrder)
            {
                if (slot.Overrides.TryGetValue(key, out var entry)
                    && entry.Kind != PropertyDescriptorOverrideKind.Delete)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void IPropertyDescriptorStore.DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
    {
        ValidateTargetAndKey(target, key);
        ArgumentNullException.ThrowIfNull(descriptor);

        DefineOrUpdateOverride(target, key, descriptor);

        if (TryGetMirroredRawClassPrototype(target, ((IPropertyDescriptorStore)this).TryGetOwn, out var mirroredTarget))
        {
            DefineOrUpdateOverride(mirroredTarget, key, descriptor);
        }
    }

    IEnumerable<string> IPropertyDescriptorStore.GetOwnKeys(object target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return GetOwnKeysForRuntimeStore(target);
    }

    bool IPropertyDescriptorStore.Delete(object target, string key)
    {
        ValidateTargetAndKey(target, key);

        var removed = DeleteOverride(target, key);
        if (removed
            && TryGetMirroredRawClassPrototype(target, ((IPropertyDescriptorStore)this).TryGetOwn, out var mirroredTarget))
        {
            _ = DeleteOverride(mirroredTarget, key);
        }

        return removed;
    }

    void IPropertyDescriptorStore.Clear(object target)
    {
        ArgumentNullException.ThrowIfNull(target);
        _overrideSlots.Remove(target);
    }

    bool IPropertyDescriptorStore.IsEnumerableOrDefaultTrue(object target, string key)
        => !((IPropertyDescriptorStore)this).TryGetOwn(target, key, out var desc) || desc.Enumerable;

    private static IPropertyDescriptorStore CurrentStore
        => _intrinsicInitializationDepth.Value > 0
            ? _intrinsicStore
            : _currentRuntimeStore.Value ?? _intrinsicStore;

    private sealed class IntrinsicInitializationScope : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _intrinsicInitializationDepth.Value--;
        }
    }

    private static void ValidateTargetAndKey(object target, string key)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(key);
    }

    private static bool TryGetMirroredRawClassPrototype(
        object target,
        TryGetOwnDescriptor tryGetOwn,
        out object mirroredTarget)
    {
        mirroredTarget = null!;

        if (!tryGetOwn(target, "constructor", out var constructorDescriptor)
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

    private delegate bool TryGetOwnDescriptor(object target, string key, out JsPropertyDescriptor descriptor);

    private bool TryGetOverride(object target, string key, out PropertyDescriptorOverride entry)
    {
        if (_overrideSlots.TryGetValue(target, out var slot))
        {
            lock (slot.SyncRoot)
            {
                return slot.Overrides.TryGetValue(key, out entry!);
            }
        }

        entry = null!;
        return false;
    }

    private void DefineOrUpdateOverride(object target, string key, JsPropertyDescriptor descriptor)
    {
        if (target is Delegate del)
        {
            Function.ClearDeletedMetadataProperty(del, key);
        }

        var hasIntrinsic = _intrinsicStore.TryGetOwn(target, key, out _);
        var slot = _overrideSlots.GetOrCreateValue(target);

        lock (slot.SyncRoot)
        {
            var hadDeleteOverride = slot.Overrides.TryGetValue(key, out var existing)
                && existing.Kind == PropertyDescriptorOverrideKind.Delete;
            var shouldTrackOrder = (!hasIntrinsic || hadDeleteOverride)
                && (!slot.KeyOrder.Contains(key));

            if (shouldTrackOrder)
            {
                slot.KeyOrder.Add(key);
            }

            slot.Overrides[key] = new PropertyDescriptorOverride
            {
                Kind = hasIntrinsic && !hadDeleteOverride
                    ? PropertyDescriptorOverrideKind.Modify
                    : PropertyDescriptorOverrideKind.Add,
                Descriptor = CloneDescriptor(descriptor)
            };
        }
    }

    private bool DeleteOverride(object target, string key)
    {
        var hasIntrinsic = _intrinsicStore.TryGetOwn(target, key, out _);
        var slot = _overrideSlots.GetOrCreateValue(target);

        lock (slot.SyncRoot)
        {
            var hasOverride = slot.Overrides.TryGetValue(key, out var existing)
                && existing.Kind != PropertyDescriptorOverrideKind.Delete;

            if (!hasIntrinsic && !hasOverride)
            {
                slot.KeyOrder.Remove(key);
                slot.Overrides[key] = new PropertyDescriptorOverride
                {
                    Kind = PropertyDescriptorOverrideKind.Delete
                };
                return false;
            }

            if (hasIntrinsic)
            {
                slot.KeyOrder.Remove(key);
                slot.Overrides[key] = new PropertyDescriptorOverride
                {
                    Kind = PropertyDescriptorOverrideKind.Delete
                };
                return true;
            }

            slot.KeyOrder.Remove(key);
            slot.Overrides[key] = new PropertyDescriptorOverride
            {
                Kind = PropertyDescriptorOverrideKind.Delete
            };
            return true;
        }
    }

    private IEnumerable<string> GetOwnKeysForRuntimeStore(object target)
    {
        var keys = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var key in _intrinsicStore.GetOwnKeys(target))
        {
            if (!IsIntrinsicKeySuppressedByOverride(target, key) && seen.Add(key))
            {
                keys.Add(key);
            }
        }

        if (_overrideSlots.TryGetValue(target, out var slot))
        {
            lock (slot.SyncRoot)
            {
                foreach (var key in slot.KeyOrder.ToArray())
                {
                    if (!slot.Overrides.TryGetValue(key, out var entry)
                        || entry.Kind == PropertyDescriptorOverrideKind.Delete)
                    {
                        continue;
                    }

                    if (seen.Add(key))
                    {
                        keys.Add(key);
                    }
                }
            }
        }

        return keys;
    }

    private bool IsDeletedOverride(object target, string key)
        => TryGetOverride(target, key, out var entry)
            && entry.Kind == PropertyDescriptorOverrideKind.Delete;

    private bool HasDeletedOverride(object target, string key)
        => IsDeletedOverride(target, key);

    private bool IsIntrinsicKeySuppressedByOverride(object target, string key)
        => TryGetOverride(target, key, out var entry)
            && (entry.Kind == PropertyDescriptorOverrideKind.Delete
                || entry.Kind == PropertyDescriptorOverrideKind.Add);
}
