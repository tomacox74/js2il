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

internal struct JsPropertyDescriptor
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

/// <summary>
/// Result of a single-probe own-descriptor lookup (perf #1418): answers
/// "deleted / descriptor / none" in one descriptor-store probe so hot read
/// paths don't pay for separate IsDeleted + TryGetOwn calls.
/// </summary>
internal enum PropertyDescriptorLookup
{
    None,
    Deleted,
    Found
}

internal sealed class PropertyDescriptorStore : IPropertyDescriptorStore
{
    // Perf (#1417): descriptor reads vastly outnumber writes on hot property paths, so
    // each slot publishes an immutable snapshot that readers access lock-free via a
    // volatile read. Writers serialize on the slot's WriteLock, build a modified copy
    // of the snapshot, and publish it atomically. Published snapshots are never
    // mutated, so readers can safely share them without locks or cloning.
    private sealed class DescriptorSnapshot
    {
        public static readonly DescriptorSnapshot Empty = new(
            new Dictionary<string, JsPropertyDescriptor>(StringComparer.Ordinal),
            System.Array.Empty<string>());

        public DescriptorSnapshot(Dictionary<string, JsPropertyDescriptor> descriptors, string[] keyOrder)
        {
            Descriptors = descriptors;
            KeyOrder = keyOrder;
        }

        public readonly Dictionary<string, JsPropertyDescriptor> Descriptors;
        public readonly string[] KeyOrder;
    }

    private sealed class DescriptorSlot
    {
        public readonly object WriteLock = new();
        private DescriptorSnapshot _snapshot = DescriptorSnapshot.Empty;

        public DescriptorSnapshot Read() => Volatile.Read(ref _snapshot);

        // Callers must hold WriteLock.
        public void Publish(DescriptorSnapshot snapshot) => Volatile.Write(ref _snapshot, snapshot);
    }

    private sealed class OverrideSnapshot
    {
        public static readonly OverrideSnapshot Empty = new(
            new Dictionary<string, PropertyDescriptorOverride>(StringComparer.Ordinal),
            System.Array.Empty<string>());

        public OverrideSnapshot(Dictionary<string, PropertyDescriptorOverride> overrides, string[] keyOrder)
        {
            Overrides = overrides;
            KeyOrder = keyOrder;
        }

        public readonly Dictionary<string, PropertyDescriptorOverride> Overrides;
        public readonly string[] KeyOrder;
    }

    private sealed class OverrideSlot
    {
        public readonly object WriteLock = new();
        private OverrideSnapshot _snapshot = OverrideSnapshot.Empty;

        public OverrideSnapshot Read() => Volatile.Read(ref _snapshot);

        // Callers must hold WriteLock.
        public void Publish(OverrideSnapshot snapshot) => Volatile.Write(ref _snapshot, snapshot);
    }

    private sealed class PropertyDescriptorOverride
    {
        public required PropertyDescriptorOverrideKind Kind { get; init; }
        public JsPropertyDescriptor Descriptor { get; init; }
    }

    private sealed class IntrinsicPropertyDescriptorStore : IPropertyDescriptorStore
    {
        private readonly ConditionalWeakTable<object, DescriptorSlot> _slots = new();

        public bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
        {
            ValidateTargetAndKey(target, key);

            if (_slots.TryGetValue(target, out var slot))
            {
                // Value-type descriptors are returned as copies, so readers cannot
                // mutate a published snapshot in place.
                return slot.Read().Descriptors.TryGetValue(key, out descriptor);
            }

            descriptor = default;
            return false;
        }

        public bool HasAny(object target)
        {
            ArgumentNullException.ThrowIfNull(target);

            return _slots.TryGetValue(target, out var slot)
                && slot.Read().Descriptors.Count != 0;
        }

        public void DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
        {
            ValidateTargetAndKey(target, key);

            DefineOrUpdateCore(target, key, descriptor);

            if (TryGetMirroredRawClassPrototype(target, TryGetOwn, out var mirroredTarget))
            {
                // Mirrored writes never sync the raw prototype's dictionary, so its
                // plain-object read fast path must be disabled regardless of shape.
                MarkJsObjectNonDataDescriptors(mirroredTarget);
                DefineOrUpdateCore(mirroredTarget, key, descriptor);
            }
        }

        public IEnumerable<string> GetOwnKeys(object target)
        {
            ArgumentNullException.ThrowIfNull(target);

            if (_slots.TryGetValue(target, out var slot))
            {
                return slot.Read().KeyOrder;
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
            // Intrinsic descriptors are defined during engine setup without a
            // guaranteed dictionary sync, so any intrinsic entry disables the
            // plain-object read fast path for JsObject targets.
            MarkJsObjectNonDataDescriptors(target);

            if (target is Delegate del)
            {
                Function.ClearDeletedMetadataProperty(del, key);
            }

            var slot = _slots.GetOrCreateValue(target);
            lock (slot.WriteLock)
            {
                var current = slot.Read();
                var descriptors = new Dictionary<string, JsPropertyDescriptor>(current.Descriptors, StringComparer.Ordinal);
                var keyOrder = current.KeyOrder;

                if (!descriptors.ContainsKey(key))
                {
                    var newOrder = new string[keyOrder.Length + 1];
                    System.Array.Copy(keyOrder, newOrder, keyOrder.Length);
                    newOrder[keyOrder.Length] = key;
                    keyOrder = newOrder;
                }

                descriptors[key] = CloneDescriptor(descriptor);
                slot.Publish(new DescriptorSnapshot(descriptors, keyOrder));
            }
        }

        private bool DeleteCore(object target, string key)
        {
            MarkJsObjectNonDataDescriptors(target);

            if (!_slots.TryGetValue(target, out var slot))
            {
                return false;
            }

            lock (slot.WriteLock)
            {
                var current = slot.Read();
                if (!current.Descriptors.ContainsKey(key))
                {
                    return false;
                }

                var descriptors = new Dictionary<string, JsPropertyDescriptor>(current.Descriptors, StringComparer.Ordinal);
                descriptors.Remove(key);
                var keyOrder = current.KeyOrder.Where(k => !string.Equals(k, key, StringComparison.Ordinal)).ToArray();
                slot.Publish(new DescriptorSnapshot(descriptors, keyOrder));
                return true;
            }
        }
    }

    private static readonly IntrinsicPropertyDescriptorStore _intrinsicStore = new();
    private static readonly ThreadLocal<IPropertyDescriptorStore?> _currentRuntimeStore = new(() => null);
    private static readonly ThreadLocal<int> _intrinsicInitializationDepth = new(() => 0);

    private readonly ConditionalWeakTable<object, OverrideSlot> _overrideSlots = new();
    private volatile bool _hasCanonicalIndexOverrides;

    /// <summary>
    /// Marks a <see cref="JsObject"/> target whose descriptor state can no longer be
    /// answered from its property dictionary alone, disabling the plain-object read
    /// fast path for that instance. See <see cref="JsObject.HasNonDataDescriptors"/>.
    /// </summary>
    private static void MarkJsObjectNonDataDescriptors(object target)
    {
        if (target is JsObject jsObject)
        {
            jsObject.MarkNonDataDescriptors();
        }
    }

    /// <summary>
    /// True for the descriptor shape produced by plain assignment/literal mirror
    /// writes (data, writable, enumerable, configurable). All writers of this shape
    /// keep the target dictionary in sync, so such descriptors never invalidate the
    /// plain-object read fast path.
    /// </summary>
    private static bool IsMirroredDefaultDataDescriptor(JsPropertyDescriptor descriptor)
        => descriptor.Kind == JsPropertyDescriptorKind.Data
            && descriptor.Writable
            && descriptor.Enumerable
            && descriptor.Configurable;

    public PropertyDescriptorStore()
    {
    }

    internal static void SetCurrentRuntimeStore(IPropertyDescriptorStore? store)
    {
        var previous = _currentRuntimeStore.Value;
        _currentRuntimeStore.Value = store;
        if (HasCanonicalIndexOverrides(previous) || HasCanonicalIndexOverrides(store))
        {
            Array.NotifyPrototypeMutation();
        }
    }

    internal static IDisposable BeginIntrinsicInitialization()
    {
        _intrinsicInitializationDepth.Value++;
        return new IntrinsicInitializationScope();
    }

    internal static JsPropertyDescriptor CloneDescriptor(JsPropertyDescriptor descriptor)
        => descriptor;

    public static bool TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
        => GetOwnLookup(target, key, out descriptor) == PropertyDescriptorLookup.Found;

    /// <summary>
    /// Unified single-probe own lookup (perf #1418). Combines the tombstone check
    /// (<see cref="IsDeleted"/>) and descriptor fetch (<see cref="TryGetOwn(object, string, out JsPropertyDescriptor)"/>)
    /// into one descriptor-store probe. Value-type descriptors are returned as copies.
    /// </summary>
    internal static PropertyDescriptorLookup GetOwnLookup(object target, string key, out JsPropertyDescriptor descriptor)
    {
        if (target is JsObject jsObject && jsObject is IExoticJsObject)
        {
            return jsObject.GetOwnPropertyDescriptor(key, out descriptor);
        }

        return GetOwnLookupCore(target, key, out descriptor);
    }

    /// <summary>
    /// Descriptor-store lookup used by the ordinary <see cref="JsObject"/>
    /// implementation. This deliberately bypasses virtual object operations to
    /// avoid recursion; exotic subclasses should call it before consulting their
    /// specialized storage.
    /// </summary>
    internal static PropertyDescriptorLookup GetOwnLookupCore(
        object target,
        string key,
        out JsPropertyDescriptor descriptor)
    {
        ValidateTargetAndKey(target, key);

        if (CurrentStore is PropertyDescriptorStore runtimeStore)
        {
            if (runtimeStore._overrideSlots.TryGetValue(target, out var slot)
                && slot.Read().Overrides.TryGetValue(key, out var entry))
            {
                if (entry.Kind == PropertyDescriptorOverrideKind.Delete)
                {
                    descriptor = default;
                    return PropertyDescriptorLookup.Deleted;
                }

                descriptor = entry.Descriptor;
                return PropertyDescriptorLookup.Found;
            }
        }

        return _intrinsicStore.TryGetOwn(target, key, out descriptor)
            ? PropertyDescriptorLookup.Found
            : PropertyDescriptorLookup.None;
    }

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
    {
        var store = CurrentStore;
        store.DefineOrUpdate(target, key, descriptor);
        NotifyCanonicalIndexMutation(store, key);
    }

    internal static void CopyOwnProperties(object source, object target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        // Descriptor copies do not sync the target's property dictionary, so a
        // JsObject target can no longer rely on the plain-object read fast path.
        MarkJsObjectNonDataDescriptors(target);

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
    {
        var store = CurrentStore;
        var deleted = store.Delete(target, key);
        NotifyCanonicalIndexMutation(store, key);
        return deleted;
    }

    internal static void Clear(object target)
    {
        CurrentStore.Clear(target);
        Array.NotifyPrototypeMutation();
    }

    private static bool HasCanonicalIndexOverrides(IPropertyDescriptorStore? store)
        => store is PropertyDescriptorStore runtimeStore
            && runtimeStore._hasCanonicalIndexOverrides;

    private static void NotifyCanonicalIndexMutation(IPropertyDescriptorStore store, string key)
    {
        if (key.Length == 0
            || (uint)(key[0] - '0') > 9
            || !ObjectRuntime.TryParseCanonicalArrayIndexUInt(key, out _))
        {
            return;
        }

        if (store is PropertyDescriptorStore runtimeStore)
        {
            runtimeStore._hasCanonicalIndexOverrides = true;
        }

        Array.NotifyPrototypeMutation();
    }

    public static bool IsEnumerableOrDefaultTrue(object target, string key)
        => !TryGetOwn(target, key, out var descriptor) || descriptor.Enumerable;

    bool IPropertyDescriptorStore.TryGetOwn(object target, string key, out JsPropertyDescriptor descriptor)
    {
        ValidateTargetAndKey(target, key);

        if (TryGetOverride(target, key, out var entry))
        {
            if (entry.Kind == PropertyDescriptorOverrideKind.Delete)
            {
                descriptor = default;
                return false;
            }

            descriptor = entry.Descriptor;
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

        var snapshot = slot.Read();
        foreach (var key in _intrinsicStore.GetOwnKeys(target))
        {
            if (!IsIntrinsicKeySuppressedByOverride(snapshot, key))
            {
                return true;
            }
        }

        foreach (var key in snapshot.KeyOrder)
        {
            if (snapshot.Overrides.TryGetValue(key, out var entry)
                && entry.Kind != PropertyDescriptorOverrideKind.Delete)
            {
                return true;
            }
        }

        return false;
    }

    void IPropertyDescriptorStore.DefineOrUpdate(object target, string key, JsPropertyDescriptor descriptor)
    {
        ValidateTargetAndKey(target, key);

        DefineOrUpdateOverride(target, key, descriptor);

        if (TryGetMirroredRawClassPrototype(target, ((IPropertyDescriptorStore)this).TryGetOwn, out var mirroredTarget))
        {
            // Mirrored writes never sync the raw prototype's dictionary, so its
            // plain-object read fast path must be disabled regardless of shape.
            MarkJsObjectNonDataDescriptors(mirroredTarget);
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
            // Perf (#1417): lock-free snapshot read.
            return slot.Read().Overrides.TryGetValue(key, out entry!);
        }

        entry = null!;
        return false;
    }

    private void DefineOrUpdateOverride(object target, string key, JsPropertyDescriptor descriptor)
    {
        // Non-default descriptors (accessors, restricted attributes) make the
        // dictionary non-authoritative for reads on JsObject targets.
        if (!IsMirroredDefaultDataDescriptor(descriptor))
        {
            MarkJsObjectNonDataDescriptors(target);
        }

        if (target is Delegate del)
        {
            Function.ClearDeletedMetadataProperty(del, key);
        }

        var hasIntrinsic = _intrinsicStore.TryGetOwn(target, key, out _);
        var slot = _overrideSlots.GetOrCreateValue(target);

        lock (slot.WriteLock)
        {
            var current = slot.Read();
            var hadDeleteOverride = current.Overrides.TryGetValue(key, out var existing)
                && existing.Kind == PropertyDescriptorOverrideKind.Delete;
            var shouldTrackOrder = (!hasIntrinsic || hadDeleteOverride)
                && System.Array.IndexOf(current.KeyOrder, key) < 0;

            var keyOrder = current.KeyOrder;
            if (shouldTrackOrder)
            {
                var newOrder = new string[keyOrder.Length + 1];
                System.Array.Copy(keyOrder, newOrder, keyOrder.Length);
                newOrder[keyOrder.Length] = key;
                keyOrder = newOrder;
            }

            var overrides = new Dictionary<string, PropertyDescriptorOverride>(current.Overrides, StringComparer.Ordinal);
            overrides[key] = new PropertyDescriptorOverride
            {
                Kind = hasIntrinsic && !hadDeleteOverride
                    ? PropertyDescriptorOverrideKind.Modify
                    : PropertyDescriptorOverrideKind.Add,
                Descriptor = CloneDescriptor(descriptor)
            };

            slot.Publish(new OverrideSnapshot(overrides, keyOrder));
        }
    }

    private bool DeleteOverride(object target, string key)
    {
        // Delete tombstones are only visible through the descriptor store.
        MarkJsObjectNonDataDescriptors(target);

        var hasIntrinsic = _intrinsicStore.TryGetOwn(target, key, out _);
        var slot = _overrideSlots.GetOrCreateValue(target);

        lock (slot.WriteLock)
        {
            var current = slot.Read();
            var hasOverride = current.Overrides.TryGetValue(key, out var existing)
                && existing.Kind != PropertyDescriptorOverrideKind.Delete;

            var keyOrder = current.KeyOrder;
            if (System.Array.IndexOf(keyOrder, key) >= 0)
            {
                keyOrder = keyOrder.Where(k => !string.Equals(k, key, StringComparison.Ordinal)).ToArray();
            }

            var overrides = new Dictionary<string, PropertyDescriptorOverride>(current.Overrides, StringComparer.Ordinal);
            overrides[key] = new PropertyDescriptorOverride
            {
                Kind = PropertyDescriptorOverrideKind.Delete
            };

            slot.Publish(new OverrideSnapshot(overrides, keyOrder));

            return hasIntrinsic || hasOverride;
        }
    }

    private IEnumerable<string> GetOwnKeysForRuntimeStore(object target)
    {
        var keys = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var snapshot = _overrideSlots.TryGetValue(target, out var slot)
            ? slot.Read()
            : OverrideSnapshot.Empty;

        foreach (var key in _intrinsicStore.GetOwnKeys(target))
        {
            if (!IsIntrinsicKeySuppressedByOverride(snapshot, key) && seen.Add(key))
            {
                keys.Add(key);
            }
        }

        foreach (var key in snapshot.KeyOrder)
        {
            if (!snapshot.Overrides.TryGetValue(key, out var entry)
                || entry.Kind == PropertyDescriptorOverrideKind.Delete)
            {
                continue;
            }

            if (seen.Add(key))
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    private bool IsDeletedOverride(object target, string key)
        => TryGetOverride(target, key, out var entry)
            && entry.Kind == PropertyDescriptorOverrideKind.Delete;

    private bool HasDeletedOverride(object target, string key)
        => IsDeletedOverride(target, key);

    private static bool IsIntrinsicKeySuppressedByOverride(OverrideSnapshot snapshot, string key)
        => snapshot.Overrides.TryGetValue(key, out var entry)
            && (entry.Kind == PropertyDescriptorOverrideKind.Delete
                || entry.Kind == PropertyDescriptorOverrideKind.Add);
}
