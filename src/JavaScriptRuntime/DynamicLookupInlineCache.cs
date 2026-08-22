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
        object receiver,
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
        DynamicLookupInlineCacheEntry entry,
        object receiver)
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
                if (currentEntries[index].MatchesIdentity(
                        receiver,
                        entry.PropertyName))
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
            if (entry.HasLiveReceiver)
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

internal sealed class DynamicLookupInlineCacheEntry
{
    internal DynamicLookupInlineCacheEntry(
        object receiver,
        string propertyName,
        object? value,
        JsObject[] prototypeChain,
        long[] lookupVersions)
    {
        _receiver = new WeakReference<object>(receiver);
        PropertyName = propertyName;
        _value = value is null
            ? null
            : new WeakReference<object>(value);
        _valueIsNull = value is null;
        PrototypeChain = prototypeChain
            .Select(static item => new WeakReference<JsObject>(item))
            .ToArray();
        LookupVersions = lookupVersions;
    }

    private readonly WeakReference<object> _receiver;

    private readonly WeakReference<object>? _value;

    private readonly bool _valueIsNull;

    internal string PropertyName { get; }

    private WeakReference<JsObject>[] PrototypeChain { get; }

    private long[] LookupVersions { get; }

    internal bool HasLiveReceiver
        => _receiver.TryGetTarget(out _);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool MatchesIdentity(
        object receiver,
        string propertyName)
        => _receiver.TryGetTarget(out var cachedReceiver)
            && ReferenceEquals(cachedReceiver, receiver)
            && string.Equals(
                PropertyName,
                propertyName,
                StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetValue(
        object receiver,
        string propertyName,
        out object? value)
    {
        if (!MatchesIdentity(receiver, propertyName))
        {
            value = null;
            return false;
        }

        for (var index = 0; index < PrototypeChain.Length; index++)
        {
            if (!PrototypeChain[index].TryGetTarget(out var chainObject)
                || chainObject.LookupVersion
                != LookupVersions[index])
            {
                value = null;
                return false;
            }
        }

        if (_valueIsNull)
        {
            value = null;
            return true;
        }

        return _value!.TryGetTarget(out value);
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

        if (!TryResolveOrdinaryDataProperty(
                receiver,
                propertyName,
                out var entry))
        {
            return ObjectRuntime.GetItem(receiver, propertyName);
        }

        site ??= caches.DynamicLookupInlineCaches.GetOrAdd(
            siteKey,
            static _ => new DynamicLookupInlineCacheSite());
        RememberSite(caches, siteKey, site);
        site.Observe(entry, receiver);
        if (site.State == DynamicLookupInlineCacheState.Megamorphic)
        {
            isMegamorphic = true;
        }

        entry.TryGetValue(receiver, propertyName, out var resolvedValue);
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

        if (!TryResolveOrdinaryDataProperty(
                receiver,
                propertyName,
                out var entry)
            || !entry.TryGetValue(
                receiver,
                propertyName,
                out var resolvedValue)
            || !CallableOperations.IsCallable(resolvedValue))
        {
            return ObjectRuntime.CallMember0(receiver, propertyName);
        }

        site ??= caches.DynamicLookupInlineCaches.GetOrAdd(
            siteKey,
            static _ => new DynamicLookupInlineCacheSite());
        RememberSite(caches, siteKey, site);
        site.Observe(entry, receiver);
        if (site.State == DynamicLookupInlineCacheState.Megamorphic)
        {
            isMegamorphic = true;
        }

        return CallableOperations.Call0(resolvedValue!, receiver);
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

    private static bool TryResolveOrdinaryDataProperty(
        object receiver,
        string propertyName,
        out DynamicLookupInlineCacheEntry entry)
    {
        entry = null!;
        if (receiver is null
            || receiver is JsNull
            || receiver.GetType() != typeof(JsObject))
        {
            return false;
        }

        var chain = new List<JsObject>(4);
        var versions = new List<long>(4);
        object? current = receiver;
        object? value = null;
        var found = false;

        while (current is not null and not JsNull)
        {
            if (current.GetType() != typeof(JsObject))
            {
                return false;
            }

            var jsObject = (JsObject)current;
            chain.Add(jsObject);
            versions.Add(jsObject.LookupVersion);

            var lookup = jsObject.GetOwnPropertyDescriptor(
                propertyName,
                out var descriptor);
            if (lookup == PropertyDescriptorLookup.Found)
            {
                if (chain.Count > 1
                    && string.Equals(
                        propertyName,
                        "__proto__",
                        StringComparison.Ordinal))
                {
                    return false;
                }

                if (descriptor.Kind != JsPropertyDescriptorKind.Data)
                {
                    return false;
                }

                value = descriptor.Value;
                found = true;
                break;
            }

            current = PrototypeChain.GetPrototypeOrNull(jsObject);
        }

        if (!found)
        {
            return false;
        }

        var candidate = new DynamicLookupInlineCacheEntry(
            receiver,
            propertyName,
            value,
            chain.ToArray(),
            versions.ToArray());
        if (!candidate.TryGetValue(
                receiver,
                propertyName,
                out _))
        {
            return false;
        }

        entry = candidate;
        return true;
    }
}
