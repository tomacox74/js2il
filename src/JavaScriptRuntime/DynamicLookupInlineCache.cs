using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

internal enum DynamicLookupInlineCacheState
{
    Empty,
    Monomorphic,
    Polymorphic,
    Megamorphic
}

internal enum DynamicLookupInlineCacheProbeResult
{
    Miss,
    Hit,
    Megamorphic
}

internal sealed class DynamicLookupInlineCacheSite
{
    internal const int MaxPolymorphicEntries = 4;

    private sealed class Snapshot
    {
        internal static readonly Snapshot Empty =
            new(DynamicLookupInlineCacheState.Empty, []);

        internal static readonly Snapshot Megamorphic =
            new(DynamicLookupInlineCacheState.Megamorphic, []);

        internal Snapshot(
            DynamicLookupInlineCacheState state,
            DynamicLookupInlineCacheEntry[] entries)
        {
            State = state;
            Entries = entries;
        }

        internal DynamicLookupInlineCacheState State { get; }

        internal DynamicLookupInlineCacheEntry[] Entries { get; }
    }

    private readonly object _writeLock = new();
    private Snapshot _snapshot = Snapshot.Empty;

    internal DynamicLookupInlineCacheState State
        => Volatile.Read(ref _snapshot).State;

    internal int EntryCount
        => Volatile.Read(ref _snapshot).Entries.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal DynamicLookupInlineCacheProbeResult Probe(
        JsObject receiver,
        string propertyName,
        out object? value)
    {
        var snapshot = Volatile.Read(ref _snapshot);
        if (snapshot.State
            == DynamicLookupInlineCacheState.Megamorphic)
        {
            value = null;
            return DynamicLookupInlineCacheProbeResult.Megamorphic;
        }

        foreach (var entry in snapshot.Entries)
        {
            if (entry.TryGetValue(receiver, propertyName, out value))
            {
                return DynamicLookupInlineCacheProbeResult.Hit;
            }
        }

        value = null;
        return DynamicLookupInlineCacheProbeResult.Miss;
    }

    internal void Observe(
        DynamicLookupInlineCacheEntry entry)
    {
        lock (_writeLock)
        {
            var current = Volatile.Read(ref _snapshot);
            if (current.State == DynamicLookupInlineCacheState.Megamorphic)
            {
                return;
            }

            var currentEntries =
                RemoveCollectedEntries(current.Entries);
            var replaceIndex = -1;
            for (var index = 0; index < currentEntries.Length; index++)
            {
                if (currentEntries[index].Matches(entry))
                {
                    replaceIndex = index;
                    break;
                }
            }

            if (replaceIndex >= 0)
            {
                var replacements =
                    (DynamicLookupInlineCacheEntry[])currentEntries.Clone();
                replacements[replaceIndex] = entry;
                Volatile.Write(
                    ref _snapshot,
                    new Snapshot(
                        GetState(replacements.Length),
                        replacements));
                return;
            }

            if (currentEntries.Length >= MaxPolymorphicEntries)
            {
                Volatile.Write(ref _snapshot, Snapshot.Megamorphic);
                return;
            }

            var entries =
                new DynamicLookupInlineCacheEntry[
                    currentEntries.Length + 1];
            System.Array.Copy(
                currentEntries,
                entries,
                currentEntries.Length);
            entries[^1] = entry;
            Volatile.Write(
                ref _snapshot,
                new Snapshot(
                    GetState(entries.Length),
                    entries));
        }
    }

    private static DynamicLookupInlineCacheEntry[]
        RemoveCollectedEntries(
            DynamicLookupInlineCacheEntry[] entries)
    {
        List<DynamicLookupInlineCacheEntry>? liveEntries =
            null;
        for (var index = 0; index < entries.Length; index++)
        {
            var entry = entries[index];
            if (entry.HasLiveShape)
            {
                liveEntries?.Add(entry);
                continue;
            }

            if (liveEntries is null)
            {
                liveEntries = new List<
                    DynamicLookupInlineCacheEntry>(
                    entries.Length);
                for (var previousIndex = 0;
                     previousIndex < index;
                     previousIndex++)
                {
                    liveEntries.Add(entries[previousIndex]);
                }
            }
        }

        return liveEntries?.ToArray() ?? entries;
    }

    private static DynamicLookupInlineCacheState GetState(
        int entryCount)
        => entryCount switch
        {
            0 => DynamicLookupInlineCacheState.Empty,
            1 => DynamicLookupInlineCacheState.Monomorphic,
            _ => DynamicLookupInlineCacheState.Polymorphic
        };
}

/// <summary>
/// A shape-keyed cache entry: a receiver's exact CLR shape identity, a property
/// name, and the slot resolved for that (shape, property) pair. The entry never
/// stores a receiver or a value; every hit re-reads the current receiver's live
/// slot value, so it is naturally correct across plain writes and shared by every
/// live receiver with the same <see cref="JsShape"/> reference.
/// </summary>
internal sealed class DynamicLookupInlineCacheEntry
{
    internal DynamicLookupInlineCacheEntry(
        JsShape shape,
        string propertyName,
        int slot)
    {
        _shape = new WeakReference<JsShape>(shape);
        PropertyName = propertyName;
        Slot = slot;
    }

    internal DynamicLookupInlineCacheEntry(
        JsShape receiverShape,
        string propertyName,
        JsObject prototype,
        long prototypeVersion,
        object value)
    {
        _shape = new WeakReference<JsShape>(receiverShape);
        _prototype = new WeakReference<JsObject>(prototype);
        _prototypeValue = new WeakReference<object>(value);
        _prototypeVersion = prototypeVersion;
        PropertyName = propertyName;
        Slot = -1;
    }

    private readonly WeakReference<JsShape> _shape;
    private readonly WeakReference<JsObject>? _prototype;
    private readonly WeakReference<object>? _prototypeValue;
    private readonly long _prototypeVersion;

    internal string PropertyName { get; }

    internal int Slot { get; }

    internal bool HasLiveShape
        => _shape.TryGetTarget(out _)
            && (_prototype is null
                || _prototype.TryGetTarget(out _)
                && _prototypeValue!.TryGetTarget(out _));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool Matches(DynamicLookupInlineCacheEntry other)
        => string.Equals(
                PropertyName,
                other.PropertyName,
                StringComparison.Ordinal)
            && _shape.TryGetTarget(out var shape)
            && other._shape.TryGetTarget(out var otherShape)
            && ReferenceEquals(shape, otherShape)
            && WeakTargetsMatch(_prototype, other._prototype);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetValue(
        JsObject receiver,
        string propertyName,
        out object? value)
    {
        if (!string.Equals(
                PropertyName,
                propertyName,
                StringComparison.Ordinal)
            || !_shape.TryGetTarget(out var shape))
        {
            value = null;
            return false;
        }

        if (_prototype is null)
        {
            return receiver.TryGetOwnPlainDataSlotValue(
                shape,
                Slot,
                out value);
        }

        if (!ReferenceEquals(receiver.Shape, shape)
            || !_prototype.TryGetTarget(out var prototype)
            || prototype.LookupVersion != _prototypeVersion
            || !_prototypeValue!.TryGetTarget(out value)
            || !receiver.TryGetInlinePrototype(out var currentPrototype)
            || !ReferenceEquals(currentPrototype, prototype))
        {
            value = null;
            return false;
        }

        return true;
    }

    private static bool WeakTargetsMatch<T>(
        WeakReference<T>? left,
        WeakReference<T>? right)
        where T : class
    {
        if (left is null || right is null)
        {
            return left is null && right is null;
        }

        return left.TryGetTarget(out var leftTarget)
            && right.TryGetTarget(out var rightTarget)
            && ReferenceEquals(leftTarget, rightTarget);
    }
}

public static class DynamicLookupInlineCache
{
    private const int RecentSiteCapacity = 8;

    [ThreadStatic]
    private static RecentSite[]? _recentSites;

    [ThreadStatic]
    private static int _nextRecentSite;

    [ThreadStatic]
    private static RecentSite _lastSite;

    [ThreadStatic]
    private static RecentSite _previousSite;

    [ThreadStatic]
    private static RuntimeRealmValueCacheState? _currentCaches;

    private readonly record struct RecentSite(
        RuntimeRealmValueCacheState Caches,
        string SiteKey,
        DynamicLookupInlineCacheSite Site);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object GetItem(
        object receiver,
        string propertyName,
        string siteKey)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (!CanCacheReceiver(receiver))
        {
            return ObjectRuntime.GetItem(receiver, propertyName);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        return GetItemCacheable(
            (JsObject)receiver,
            propertyName,
            siteKey,
            caches,
            out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object GetItem(
        object receiver,
        string propertyName,
        string siteKey,
        ref int terminalMegamorphic)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (Volatile.Read(ref terminalMegamorphic) != 0
            || !CanCacheReceiver(receiver))
        {
            return ObjectRuntime.GetItem(receiver, propertyName);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        var result = GetItemCacheable(
            (JsObject)receiver,
            propertyName,
            siteKey,
            caches,
            out var isMegamorphic);
        if (isMegamorphic)
        {
            Volatile.Write(ref terminalMegamorphic, 1);
        }

        return result;
    }

    private static object GetItemCacheable(
        JsObject receiver,
        string propertyName,
        string siteKey,
        RuntimeRealmValueCacheState caches,
        out bool isMegamorphic)
    {
        isMegamorphic = false;
        var site = GetSite(caches, siteKey);
        object? cachedValue = null;
        var probe = site is null
            ? DynamicLookupInlineCacheProbeResult.Miss
            : site.Probe(
                receiver,
                propertyName,
                out cachedValue);
        if (probe == DynamicLookupInlineCacheProbeResult.Hit)
        {
            return cachedValue!;
        }

        if (probe == DynamicLookupInlineCacheProbeResult.Megamorphic)
        {
            isMegamorphic = true;
            return ObjectRuntime.GetItem(receiver, propertyName);
        }

        if (!TryResolveOwnPlainDataProperty(
                receiver,
                propertyName,
                out var shape,
                out var slot,
                out var resolvedValue))
        {
            return ObjectRuntime.GetItem(receiver, propertyName);
        }

        site ??= caches.DynamicLookupInlineCaches.GetOrAdd(
            siteKey,
            static _ => new DynamicLookupInlineCacheSite());
        RememberSite(caches, siteKey, site);
        site.Observe(
            new DynamicLookupInlineCacheEntry(shape, propertyName, slot));
        if (site.State == DynamicLookupInlineCacheState.Megamorphic)
        {
            isMegamorphic = true;
        }

        return resolvedValue!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? CallMember0(
        object receiver,
        string propertyName,
        string siteKey)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (!CanCacheReceiver(receiver))
        {
            return ObjectRuntime.CallMember0(
                receiver,
                propertyName);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        return CallMember0Cacheable(
            (JsObject)receiver,
            propertyName,
            siteKey,
            caches,
            out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? CallMember0(
        object receiver,
        string propertyName,
        string siteKey,
        ref int terminalMegamorphic)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (Volatile.Read(ref terminalMegamorphic) != 0
            || !CanCacheReceiver(receiver))
        {
            return ObjectRuntime.CallMember0(
                receiver,
                propertyName);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        var result = CallMember0Cacheable(
            (JsObject)receiver,
            propertyName,
            siteKey,
            caches,
            out var isMegamorphic);
        if (isMegamorphic)
        {
            Volatile.Write(ref terminalMegamorphic, 1);
        }

        return result;
    }

    private static object? CallMember0Cacheable(
        JsObject receiver,
        string propertyName,
        string siteKey,
        RuntimeRealmValueCacheState caches,
        out bool isMegamorphic)
    {
        isMegamorphic = false;
        var site = GetSite(caches, siteKey);
        object? cachedValue = null;
        var probe = site is null
            ? DynamicLookupInlineCacheProbeResult.Miss
            : site.Probe(
                receiver,
                propertyName,
                out cachedValue);
        if (probe == DynamicLookupInlineCacheProbeResult.Hit
            && CallableOperations.IsCallable(cachedValue))
        {
            return CallableOperations.Call0(cachedValue!, receiver);
        }

        if (probe == DynamicLookupInlineCacheProbeResult.Megamorphic)
        {
            isMegamorphic = true;
            return ObjectRuntime.CallMember0(
                receiver,
                propertyName);
        }

        if (!TryResolveOwnPlainDataProperty(
                receiver,
                propertyName,
                out var shape,
                out var slot,
                out var resolvedValue)
            || !CallableOperations.IsCallable(resolvedValue))
        {
            return ObjectRuntime.CallMember0(receiver, propertyName);
        }

        site ??= caches.DynamicLookupInlineCaches.GetOrAdd(
            siteKey,
            static _ => new DynamicLookupInlineCacheSite());
        RememberSite(caches, siteKey, site);
        site.Observe(
            new DynamicLookupInlineCacheEntry(shape, propertyName, slot));
        if (site.State == DynamicLookupInlineCacheState.Megamorphic)
        {
            isMegamorphic = true;
        }

        return CallableOperations.Call0(resolvedValue!, receiver);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? CallMember1(
        object receiver,
        string propertyName,
        object? argument0,
        string siteKey)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (!CanCacheMemberReceiver(receiver))
        {
            return ObjectRuntime.CallMember1(
                receiver,
                propertyName,
                argument0);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        return CallMember1Cacheable(
            (JsObject)receiver,
            propertyName,
            argument0,
            siteKey,
            caches,
            out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? CallMember1(
        object receiver,
        string propertyName,
        object? argument0,
        string siteKey,
        ref int terminalMegamorphic)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(siteKey);

        if (Volatile.Read(ref terminalMegamorphic) != 0
            || !CanCacheMemberReceiver(receiver))
        {
            return ObjectRuntime.CallMember1(
                receiver,
                propertyName,
                argument0);
        }

        var caches = _currentCaches
            ?? GetCurrentRealmCaches();
        var result = CallMember1Cacheable(
            (JsObject)receiver,
            propertyName,
            argument0,
            siteKey,
            caches,
            out var isMegamorphic);
        if (isMegamorphic)
        {
            Volatile.Write(ref terminalMegamorphic, 1);
        }

        return result;
    }

    private static object? CallMember1Cacheable(
        JsObject receiver,
        string propertyName,
        object? argument0,
        string siteKey,
        RuntimeRealmValueCacheState caches,
        out bool isMegamorphic)
    {
        isMegamorphic = false;
        var site = GetSite(caches, siteKey);
        object? cachedValue = null;
        var probe = site is null
            ? DynamicLookupInlineCacheProbeResult.Miss
            : site.Probe(
                receiver,
                propertyName,
                out cachedValue);
        if (probe == DynamicLookupInlineCacheProbeResult.Hit
            && CallableOperations.IsCallable(cachedValue))
        {
            return CallableOperations.Call1(
                cachedValue!,
                receiver,
                argument0);
        }

        if (probe == DynamicLookupInlineCacheProbeResult.Megamorphic)
        {
            isMegamorphic = true;
            return ObjectRuntime.CallMember1(
                receiver,
                propertyName,
                argument0);
        }

        if (!TryResolvePlainDataMember(
                receiver,
                propertyName,
                out var entry,
                out var resolvedValue)
            || !CallableOperations.IsCallable(resolvedValue))
        {
            return ObjectRuntime.CallMember1(
                receiver,
                propertyName,
                argument0);
        }

        site ??= caches.DynamicLookupInlineCaches.GetOrAdd(
            siteKey,
            static _ => new DynamicLookupInlineCacheSite());
        RememberSite(caches, siteKey, site);
        site.Observe(entry);
        if (site.State == DynamicLookupInlineCacheState.Megamorphic)
        {
            isMegamorphic = true;
        }

        return CallableOperations.Call1(
            resolvedValue!,
            receiver,
            argument0);
    }

    internal static DynamicLookupInlineCacheSite? GetSiteForTests(
        string siteKey)
    {
        var caches = GetCurrentRealmCaches();
        return caches.DynamicLookupInlineCaches.TryGetValue(
            siteKey,
            out var site)
            ? site
            : null;
    }

    internal static bool RemoveSiteForBenchmarks(string siteKey)
    {
        var caches = GetCurrentRealmCaches();
        RemoveRecentSite(caches, siteKey);
        return caches.DynamicLookupInlineCaches
            .TryRemove(siteKey, out _);
    }

    internal static void OnExecutionContextChanged(
        RuntimeRealmValueCacheState? currentCaches)
    {
        _currentCaches = currentCaches;
        _recentSites = null;
        _nextRecentSite = 0;
        _lastSite = default;
        _previousSite = default;
    }

    private static RuntimeRealmValueCacheState GetCurrentRealmCaches()
        => RuntimeExecutionContext.CurrentOrOverride?.Realm.ValueCaches
            ?? throw new InvalidOperationException(
                "Dynamic lookup inline caches require an active JavaScript runtime.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DynamicLookupInlineCacheSite? GetSite(
        RuntimeRealmValueCacheState caches,
        string siteKey)
    {
        if (MatchesRecentSite(_lastSite, caches, siteKey))
        {
            return _lastSite.Site;
        }

        if (MatchesRecentSite(_previousSite, caches, siteKey))
        {
            (_lastSite, _previousSite) =
                (_previousSite, _lastSite);
            return _lastSite.Site;
        }

        var recentSites = _recentSites;
        if (recentSites is not null)
        {
            for (var index = 0;
                 index < recentSites.Length;
                 index++)
            {
                var recent = recentSites[index];
                if (MatchesRecentSite(
                        recent,
                        caches,
                        siteKey))
                {
                    _previousSite = _lastSite;
                    _lastSite = recent;
                    return recent.Site;
                }
            }
        }

        if (!caches.DynamicLookupInlineCaches.TryGetValue(
                siteKey,
                out var site))
        {
            return null;
        }

        RememberSite(caches, siteKey, site);
        return site;
    }

    private static void RememberSite(
        RuntimeRealmValueCacheState caches,
        string siteKey,
        DynamicLookupInlineCacheSite site)
    {
        var recent = new RecentSite(
            caches,
            siteKey,
            site);
        if (MatchesRecentSite(
                _lastSite,
                caches,
                siteKey))
        {
            _lastSite = recent;
            return;
        }

        _previousSite = _lastSite;
        _lastSite = recent;
        var recentSites = _recentSites
            ??= new RecentSite[RecentSiteCapacity];
        recentSites[_nextRecentSite] =
            recent;
        _nextRecentSite =
            (_nextRecentSite + 1) % recentSites.Length;
    }

    private static void RemoveRecentSite(
        RuntimeRealmValueCacheState caches,
        string siteKey)
    {
        if (MatchesRecentSite(
                _lastSite,
                caches,
                siteKey))
        {
            _lastSite = default;
        }

        if (MatchesRecentSite(
                _previousSite,
                caches,
                siteKey))
        {
            _previousSite = default;
        }

        var recentSites = _recentSites;
        if (recentSites is null)
        {
            return;
        }

        for (var index = 0;
             index < recentSites.Length;
             index++)
        {
            var recent = recentSites[index];
            if (MatchesRecentSite(
                    recent,
                    caches,
                    siteKey))
            {
                recentSites[index] = default;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchesRecentSite(
        RecentSite recent,
        RuntimeRealmValueCacheState caches,
        string siteKey)
        => recent.Caches is not null
            && ReferenceEquals(recent.Caches, caches)
            && SiteKeysEqual(recent.SiteKey, siteKey);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SiteKeysEqual(
        string? left,
        string right)
        => ReferenceEquals(left, right)
            || string.Equals(
                left,
                right,
                StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanCacheReceiver(object receiver)
        => receiver is not null
            && receiver.GetType() == typeof(JsObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanCacheMemberReceiver(object receiver)
        => receiver is not null
            && (receiver.GetType() == typeof(JsObject)
                || receiver.GetType() == typeof(Array));

    private static bool TryResolvePlainDataMember(
        JsObject receiver,
        string propertyName,
        out DynamicLookupInlineCacheEntry entry,
        out object? value)
    {
        entry = null!;
        value = null;
        if (ObjectRuntime.IsEncodedSymbolKey(propertyName))
        {
            return false;
        }

        if (receiver.TryGetOwnPlainDataSlot(
                propertyName,
                out var receiverShape,
                out var receiverSlot,
                out value))
        {
            entry = new DynamicLookupInlineCacheEntry(
                receiverShape,
                propertyName,
                receiverSlot);
            return true;
        }

        if (receiver.Shape.GetSlot(propertyName) >= 0
            || PropertyDescriptorStore.GetOwnLookup(
                receiver,
                propertyName,
                out _) != PropertyDescriptorLookup.None
            || !receiver.TryGetInlinePrototype(out var prototypeValue)
            || prototypeValue?.GetType() != typeof(JsObject))
        {
            return false;
        }

        var prototype = (JsObject)prototypeValue;
        if (prototype.GetOwnPropertyDescriptor(
                propertyName,
                out var descriptor)
                != PropertyDescriptorLookup.Found
            || descriptor.Kind != JsPropertyDescriptorKind.Data
            || !descriptor.Writable
            || !descriptor.Enumerable
            || !descriptor.Configurable
            || descriptor.Value is null)
        {
            return false;
        }

        value = descriptor.Value;
        entry = new DynamicLookupInlineCacheEntry(
            receiver.Shape,
            propertyName,
            prototype,
            prototype.LookupVersion,
            value);
        return true;
    }

    /// <summary>
    /// Resolves an own, plain (default-attributed) data-property slot eligible for
    /// the Tier 3 shape-keyed inline cache. Deliberately narrower than generic
    /// lookup: it never walks the prototype chain, never resolves accessors, and
    /// rejects encoded symbol keys and shared/override descriptor storage. All of
    /// those cases must keep using <see cref="ObjectRuntime"/> generic lookup.
    /// </summary>
    private static bool TryResolveOwnPlainDataProperty(
        JsObject receiver,
        string propertyName,
        out JsShape shape,
        out int slot,
        out object? value)
    {
        if (ObjectRuntime.IsEncodedSymbolKey(propertyName))
        {
            shape = null!;
            slot = -1;
            value = null;
            return false;
        }

        return receiver.TryGetOwnPlainDataSlot(
            propertyName,
            out shape,
            out slot,
            out value);
    }
}
