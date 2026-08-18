using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace JavaScriptRuntime;

/// <summary>
/// Identifies a well-known intrinsic object slot inside a realm's
/// <see cref="RuntimeIntrinsics"/> graph (ECMA-262 Realm Record [[Intrinsics]]).
/// </summary>
internal enum RuntimeIntrinsicSlot
{
    /// <summary>
    /// Pseudo-slot for the realm's one-time global bootstrap (see
    /// <see cref="RuntimeIntrinsics.EnsureBootstrapped"/>). Modelling the bootstrap as a
    /// slot keeps it inside the same publication/waiting protocol as every other
    /// intrinsic, so it can never invert the lock order against a lazily materialized
    /// prototype.
    /// </summary>
    RealmBootstrap,
    ObjectPrototype,
    FunctionPrototype,
    FunctionRestrictedPropertiesPrototype,
    ArrayImmutablePrototype,
    ArrayPrototype,
    StringPrototype,
    StringIteratorPrototype,
    NumberPrototype,
    BooleanPrototype,
    BigIntPrototype,
    SymbolPrototype,
    ErrorPrototype,
    EvalErrorPrototype,
    RangeErrorPrototype,
    ReferenceErrorPrototype,
    SyntaxErrorPrototype,
    TypeErrorPrototype,
    URIErrorPrototype,
    AggregateErrorPrototype,
    SuppressedErrorPrototype,
    Json,
    Intl,
    Atomics,
    GlobalPromisePrototype,
    PromisePrototype,
    ArrayBufferPrototype,
    SharedArrayBufferPrototype,
    TypedArrayPrototype,
    Float64ArrayPrototype,
    Float32ArrayPrototype,
    Int32ArrayPrototype,
    Int16ArrayPrototype,
    Int8ArrayPrototype,
    Uint32ArrayPrototype,
    Uint16ArrayPrototype,
    Uint8ArrayPrototype,
    Uint8ClampedArrayPrototype,
    MapPrototype,
    MapIteratorPrototype,
    SetPrototype,
    SetIteratorPrototype,
    WeakMapPrototype,
    WeakSetPrototype,
    WeakRefPrototype,
    FinalizationRegistryPrototype,
    DataViewPrototype,
    DatePrototype,
    RegExpPrototype,
    RegExpStringIteratorPrototype,
    IteratorPrototype,
    IteratorHelperPrototype,
    AsyncIteratorPrototype,
    GeneratorPrototype,
    GeneratorFunctionPrototype,
    AsyncGeneratorPrototype,
    AsyncGeneratorFunctionPrototype,
    AsyncFunctionPrototype,
    UrlPrototype,
    UrlSearchParamsPrototype,
    AbortControllerPrototype,
    AbortSignalPrototype,

    Count
}

/// <summary>
/// Realm-owned graph of well-known intrinsic objects. Mirrors the ECMA-262 Realm
/// Record's [[Intrinsics]] slot: every <see cref="RuntimeRealm"/> owns exactly one
/// <see cref="RuntimeIntrinsics"/> instance, and every JavaScript object reachable
/// from it (<c>Object.prototype</c>, every other built-in prototype, every built-in
/// function object, and every intrinsic property descriptor whose value is one of
/// those objects) is created for that realm alone.
/// </summary>
/// <remarks>
/// <para>
/// Slot creation is coordinated per slot. The thread that wins a slot runs the factory
/// and the initializer while every other thread waits for the fully initialized object,
/// so a half-wired intrinsic is never handed to a second thread. The winning thread
/// itself resolves the slot reentrantly to the object under construction, because the
/// mutually recursive intrinsic bootstrap cycles that ECMA-262 requires (for example
/// <c>Function.prototype.apply</c> being a function object whose own [[Prototype]] is
/// <c>Function.prototype</c>) must resolve against the partially wired object instead of
/// recursing forever. A failed initializer leaves the slot empty, so the next resolution
/// retries from scratch instead of publishing a permanently half-built object.
/// </para>
/// <para>
/// Lock order for the whole runtime (acquire left to right, never right to left):
/// <c>intrinsic slot gate</c> → <see cref="BuiltinDelegateFunctionAdapter.InitializationLock"/>.
/// The adapter lock is a leaf: any intrinsic an adapter-lock scope needs must be
/// materialized before the lock is taken (see <see cref="Function.InitializeFunctionInstance{T}(T)"/>).
/// The realm bootstrap is itself a slot (<see cref="RuntimeIntrinsicSlot.RealmBootstrap"/>),
/// so bootstrap and lazy slot creation share one protocol rather than two nested locks.
/// </para>
/// <para>
/// Besides the object slots this type owns the three realm-scoped side tables that
/// used to be process-wide and therefore leaked realm-created JavaScript objects
/// across realms:
/// <list type="bullet">
/// <item><description><see cref="BuiltinAdapters"/> — the stable
/// <see cref="BuiltinDelegateFunctionAdapter"/> identity for each runtime-owned
/// built-in delegate. The delegates themselves are immutable CLR metadata and stay
/// static; only the JavaScript-visible wrapper is realm-owned.</description></item>
/// <item><description><see cref="PrototypeSlots"/> — [[Prototype]] links for
/// JavaScript values that are not <see cref="JsObject"/> instances (for example
/// <c>Map</c>, <c>Set</c>, <c>RegExp</c>, <c>Date</c>, <c>Promise</c> and the
/// <see cref="System.Type"/> objects used as constructor identities).</description></item>
/// <item><description><see cref="IntrinsicDescriptors"/> — the intrinsic descriptor
/// baseline for those same non-<see cref="JsObject"/> targets.</description></item>
/// </list>
/// </para>
/// <para>
/// <see cref="Current"/> always resolves the ambient realm through
/// <see cref="RuntimeExecutionContext"/>; the ambient realm wins even when other realms
/// exist, so repeated resolutions inside one operation can never switch graphs. Only a
/// context-less caller (runtime unit tests that poke intrinsics directly) falls back to
/// a single deterministic process-default graph.
/// </para>
/// </remarks>
internal sealed class RuntimeIntrinsics
{
    private const int MaxWaitChainLength = 64;

    private static readonly TimeSpan WaitSlice = TimeSpan.FromMilliseconds(20);

    private static readonly object _processDefaultGate = new();

    /// <summary>
    /// Wait-for graph used to detect intrinsic initialization cycles that span threads.
    /// Maps a blocked thread to the slot it is blocked on.
    /// </summary>
    private static readonly ConcurrentDictionary<int, SlotEntry> _blockedThreads = new();

    private static RuntimeIntrinsics? _processDefault;
    private static long _nextId;

    [ThreadStatic]
    private static int _initializationDepth;

    private readonly object _slotTableGate = new();
    private readonly SlotEntry?[] _slots = new SlotEntry?[(int)RuntimeIntrinsicSlot.Count];

    /// <summary>
    /// Fully initialized slot values. Only written after the slot's initializer has
    /// completed, so the lock-free read path can never observe a half-built object.
    /// </summary>
    private readonly object?[] _published = new object?[(int)RuntimeIntrinsicSlot.Count];

    internal RuntimeIntrinsics()
    {
        Id = Interlocked.Increment(ref _nextId);
    }

    /// <summary>
    /// Process-unique identifier, usable as a realm-change marker by callers that must
    /// not keep the intrinsic graph alive (see <c>Array</c>'s cached prototype-chain state).
    /// </summary>
    internal long Id { get; }

    /// <summary>
    /// Per-realm stable adapter identities for runtime-owned built-in delegates.
    /// </summary>
    internal BuiltinAdapterCache BuiltinAdapters { get; } = new();

    /// <summary>
    /// Per-realm [[Prototype]] storage for values that are not <see cref="JsObject"/>
    /// instances (<see cref="JsObject"/> stores its own prototype inline).
    /// </summary>
    internal ConditionalWeakTable<object, PrototypeSlot> PrototypeSlots { get; } = new();

    /// <summary>
    /// Per-realm intrinsic descriptor baseline for non-<see cref="JsObject"/> targets.
    /// </summary>
    internal PropertyDescriptorStore.IntrinsicPropertyDescriptorStore IntrinsicDescriptors { get; } = new();

    /// <summary>
    /// Per-realm built-in function values exposed by <see cref="GlobalThis"/>
    /// (<c>setTimeout</c>, <c>parseInt</c>, ...).
    /// </summary>
    internal ConcurrentDictionary<string, JsFunctionObject> GlobalFunctionValues { get; } =
        new(StringComparer.Ordinal);

    /// <summary>
    /// The intrinsic graph for the ambient realm, or the process default graph when
    /// no realm is active (runtime unit tests that poke intrinsics directly).
    /// </summary>
    /// <remarks>
    /// The ambient realm always wins: there is no "only one realm exists" shortcut,
    /// because such a shortcut changes answer the moment a second realm appears and can
    /// therefore hand two different graphs to one logical operation.
    /// </remarks>
    internal static RuntimeIntrinsics Current
    {
        get
        {
            // Cheap ambient fast path: one AsyncLocal read.
            var context = RuntimeExecutionContext.Current;
            return context != null
                ? context.Realm.Intrinsics
                : ResolveWithoutAmbientFrame();
        }
    }

    /// <summary>
    /// True when this realm's global bootstrap has completed, or is running on the
    /// calling thread (the bootstrapping thread must observe the graph under
    /// construction instead of waiting for itself).
    /// </summary>
    internal bool IsBootstrapped
        => Volatile.Read(ref _published[(int)RuntimeIntrinsicSlot.RealmBootstrap]) != null
            || IsInitializingSlotOnCurrentThread(RuntimeIntrinsicSlot.RealmBootstrap);

    /// <summary>
    /// True while the calling thread is inside an intrinsic factory/initializer. Such a
    /// thread must never block on a bootstrap gate: it is already part of the graph
    /// being built and has to keep making progress.
    /// </summary>
    internal static bool IsInitializingOnCurrentThread => _initializationDepth > 0;

    internal object ObjectPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.ObjectPrototype, static () => new JsObject());

    internal object ErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.ErrorPrototype, static () => new JsObject());

    internal object EvalErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.EvalErrorPrototype, static () => new JsObject());

    internal object RangeErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.RangeErrorPrototype, static () => new JsObject());

    internal object ReferenceErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.ReferenceErrorPrototype, static () => new JsObject());

    internal object SyntaxErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.SyntaxErrorPrototype, static () => new JsObject());

    internal object TypeErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.TypeErrorPrototype, static () => new JsObject());

    internal object URIErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.URIErrorPrototype, static () => new JsObject());

    internal object AggregateErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.AggregateErrorPrototype, static () => new JsObject());

    internal object SuppressedErrorPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.SuppressedErrorPrototype, static () => new JsObject());

    internal object Json
        => GetOrCreate(RuntimeIntrinsicSlot.Json, static () => new JsObject());

    internal object Intl
        => GetOrCreate(RuntimeIntrinsicSlot.Intl, static () => new JsObject());

    internal object Atomics
        => GetOrCreate(RuntimeIntrinsicSlot.Atomics, static () => new JsObject());

    internal object NumberPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.NumberPrototype, static () => new JsObject());

    internal object BooleanPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.BooleanPrototype, static () => new JsObject());

    internal object BigIntPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.BigIntPrototype, static () => new JsObject());

    internal object SymbolPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.SymbolPrototype, static () => new JsObject());

    internal object GlobalPromisePrototype
        => GetOrCreate(RuntimeIntrinsicSlot.GlobalPromisePrototype, static () => new JsObject());

    internal object ArrayBufferPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.ArrayBufferPrototype, static () => new JsObject());

    internal object SharedArrayBufferPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.SharedArrayBufferPrototype, static () => new JsObject());

    internal object TypedArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.TypedArrayPrototype, static () => new JsObject());

    internal object Float64ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Float64ArrayPrototype, static () => new JsObject());

    internal object Float32ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Float32ArrayPrototype, static () => new JsObject());

    internal object Int32ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Int32ArrayPrototype, static () => new JsObject());

    internal object Int16ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Int16ArrayPrototype, static () => new JsObject());

    internal object Int8ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Int8ArrayPrototype, static () => new JsObject());

    internal object Uint32ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Uint32ArrayPrototype, static () => new JsObject());

    internal object Uint16ArrayPrototype
        => GetOrCreate(RuntimeIntrinsicSlot.Uint16ArrayPrototype, static () => new JsObject());

    /// <summary>
    /// Returns the intrinsic in <paramref name="slot"/>, creating it on first use.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <paramref name="create"/> must only allocate the bare object and must not resolve
    /// other intrinsics: the result is published to the owning thread before
    /// <paramref name="initialize"/> runs, so nested lookups of the same slot from within
    /// <paramref name="initialize"/> observe the object under construction instead of
    /// recursing.
    /// </para>
    /// <para>
    /// Other threads block until <paramref name="initialize"/> has completed and never
    /// observe the object under construction. If <paramref name="initialize"/> throws,
    /// the slot is reset to empty so the next resolution retries instead of publishing a
    /// permanently half-built intrinsic.
    /// </para>
    /// </remarks>
    internal T GetOrCreate<T>(
        RuntimeIntrinsicSlot slot,
        Func<T> create,
        Action<T>? initialize = null)
        where T : class
    {
        var index = (int)slot;
        if (Volatile.Read(ref _published[index]) is T ready)
        {
            return ready;
        }

        var entry = GetOrAddEntry(index);
        var threadId = Environment.CurrentManagedThreadId;

        lock (entry.Gate)
        {
            while (true)
            {
                if (entry.Status == SlotStatus.Initialized)
                {
                    return (T)entry.Value!;
                }

                if (entry.Status == SlotStatus.Empty)
                {
                    entry.Status = SlotStatus.Initializing;
                    entry.OwnerThreadId = threadId;
                    break;
                }

                // The owning thread resolves reentrantly; that is how the ECMA-262
                // intrinsic bootstrap cycles terminate without exposing the object under
                // construction to another thread.
                if (entry.OwnerThreadId == threadId)
                {
                    if (entry.Value is T underConstruction)
                    {
                        return underConstruction;
                    }

                    throw new InvalidOperationException(
                        $"Intrinsic slot '{slot}' was resolved reentrantly from its own factory. "
                        + "Factories must only allocate the bare object; move dependent work into the initializer.");
                }

                if (WouldDeadlock(entry, threadId))
                {
                    throw new InvalidOperationException(
                        $"Cross-thread intrinsic initialization cycle detected while resolving slot '{slot}'.");
                }

                Wait(entry, threadId);
            }
        }

        return RunInitializer(entry, index, create, initialize);
    }

    /// <summary>
    /// Runs this realm's one-time global bootstrap, blocking concurrent callers until it
    /// has completed. Reentrant calls from the bootstrapping thread are no-ops.
    /// </summary>
    internal void EnsureBootstrapped(Action bootstrap)
    {
        ArgumentNullException.ThrowIfNull(bootstrap);
        if (IsBootstrapped)
        {
            return;
        }

        GetOrCreate(
            RuntimeIntrinsicSlot.RealmBootstrap,
            static () => new object(),
            _ => bootstrap());
    }

    /// <summary>
    /// True while <paramref name="slot"/> is being initialized by the calling thread.
    /// </summary>
    internal bool IsInitializingSlotOnCurrentThread(RuntimeIntrinsicSlot slot)
    {
        var entry = Volatile.Read(ref _slots[(int)slot]);
        return entry != null
            && Volatile.Read(ref entry.OwnerThreadId) == Environment.CurrentManagedThreadId;
    }

    /// <summary>
    /// Releases the realm's intrinsic object graph (ECMA-262 realms are dropped whole).
    /// </summary>
    internal void Dispose()
    {
        lock (_slotTableGate)
        {
            System.Array.Clear(_slots);
            System.Array.Clear(_published);
        }

        BuiltinAdapters.Clear();
        GlobalFunctionValues.Clear();
    }

    private SlotEntry GetOrAddEntry(int index)
    {
        var existing = Volatile.Read(ref _slots[index]);
        if (existing != null)
        {
            return existing;
        }

        var candidate = new SlotEntry();
        return Interlocked.CompareExchange(ref _slots[index], candidate, null) ?? candidate;
    }

    private T RunInitializer<T>(
        SlotEntry entry,
        int index,
        Func<T> create,
        Action<T>? initialize)
        where T : class
    {
        T value;
        _initializationDepth++;
        try
        {
            value = create();

            lock (entry.Gate)
            {
                entry.Value = value;
                Monitor.PulseAll(entry.Gate);
            }

            initialize?.Invoke(value);
        }
        catch
        {
            lock (entry.Gate)
            {
                entry.Value = null;
                entry.Status = SlotStatus.Empty;
                entry.OwnerThreadId = 0;
                Monitor.PulseAll(entry.Gate);
            }

            throw;
        }
        finally
        {
            _initializationDepth--;
        }

        lock (entry.Gate)
        {
            entry.Status = SlotStatus.Initialized;
            entry.OwnerThreadId = 0;
            Monitor.PulseAll(entry.Gate);
        }

        // Only published once the initializer completed, and only while this entry is
        // still the realm's live entry for the slot (a realm disposed mid-initialization
        // must stay dropped).
        lock (_slotTableGate)
        {
            if (ReferenceEquals(_slots[index], entry))
            {
                Volatile.Write(ref _published[index], value);
            }
        }

        return value;
    }

    private static void Wait(SlotEntry entry, int threadId)
    {
        _blockedThreads[threadId] = entry;
        try
        {
            // Bounded so the cycle check above is re-evaluated periodically: a wait-for
            // edge that only appears after this thread blocked is then still noticed.
            Monitor.Wait(entry.Gate, WaitSlice);
        }
        finally
        {
            _blockedThreads.TryRemove(threadId, out _);
        }
    }

    /// <summary>
    /// Walks the wait-for graph from <paramref name="target"/>'s owner and reports
    /// whether blocking on it would close a cycle back to <paramref name="threadId"/>.
    /// </summary>
    private static bool WouldDeadlock(SlotEntry target, int threadId)
    {
        var current = target;
        for (var hop = 0; hop < MaxWaitChainLength; hop++)
        {
            var owner = Volatile.Read(ref current.OwnerThreadId);
            if (owner == 0)
            {
                return false;
            }

            if (owner == threadId)
            {
                return true;
            }

            if (!_blockedThreads.TryGetValue(owner, out var next))
            {
                return false;
            }

            current = next;
        }

        // Pathologically long chain: treat as a cycle rather than risk blocking forever.
        return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static RuntimeIntrinsics ResolveWithoutAmbientFrame()
    {
        var context = RuntimeExecutionContext.CurrentOrOverride;
        if (context != null)
        {
            return context.Realm.Intrinsics;
        }

        var processDefault = Volatile.Read(ref _processDefault);
        if (processDefault != null)
        {
            return processDefault;
        }

        lock (_processDefaultGate)
        {
            processDefault = _processDefault;
            if (processDefault == null)
            {
                processDefault = new RuntimeIntrinsics();
                Volatile.Write(ref _processDefault, processDefault);
            }

            return processDefault;
        }
    }

    private enum SlotStatus
    {
        Empty,
        Initializing,
        Initialized
    }

    /// <summary>
    /// Coordination state for a single intrinsic slot.
    /// </summary>
    private sealed class SlotEntry
    {
        internal readonly object Gate = new();

        /// <summary>
        /// The slot's object. Visible to other threads only once
        /// <see cref="Status"/> is <see cref="SlotStatus.Initialized"/>, except for a
        /// thread whose wait would close an initialization cycle.
        /// </summary>
        internal object? Value;

        internal SlotStatus Status;

        internal int OwnerThreadId;
    }

    /// <summary>
    /// [[Prototype]] storage for a single non-<see cref="JsObject"/> target.
    /// </summary>
    internal sealed class PrototypeSlot
    {
        internal object? Prototype;
    }

    /// <summary>
    /// Realm-owned stable adapter identities for runtime-owned built-in delegates.
    /// Keyed by immutable CLR metadata (declaring type / delegate target instance plus
    /// method handle and delegate type); only the produced adapter is realm-created.
    /// </summary>
    internal sealed class BuiltinAdapterCache
    {
        private sealed class Entries
        {
            internal ConcurrentDictionary<
                (RuntimeMethodHandle Method, Type DelegateType),
                BuiltinDelegateFunctionAdapter> Adapters { get; } = new();
        }

        private readonly ConditionalWeakTable<Type, Entries> _staticAdapters = new();
        private readonly ConditionalWeakTable<object, Entries> _instanceAdapters = new();

        internal BuiltinDelegateFunctionAdapter GetOrAdd(
            Delegate target,
            Func<Delegate, BuiltinDelegateFunctionAdapter> factory)
        {
            var entries = target.Target == null
                ? _staticAdapters.GetOrCreateValue(
                    target.Method.DeclaringType
                        ?? throw new InvalidOperationException(
                            "Runtime-owned static delegates require a declaring type."))
                : _instanceAdapters.GetOrCreateValue(target.Target);

            return entries.Adapters.GetOrAdd(
                (target.Method.MethodHandle, target.GetType()),
                _ => factory(target));
        }

        internal bool Contains(Delegate target)
        {
            if (target.Target == null)
            {
                return target.Method.DeclaringType is { } declaringType
                    && _staticAdapters.TryGetValue(declaringType, out var staticEntries)
                    && staticEntries.Adapters.ContainsKey(
                        (target.Method.MethodHandle, target.GetType()));
            }

            return _instanceAdapters.TryGetValue(target.Target, out var instanceEntries)
                && instanceEntries.Adapters.ContainsKey(
                    (target.Method.MethodHandle, target.GetType()));
        }

        internal void Clear()
        {
            _staticAdapters.Clear();
            _instanceAdapters.Clear();
        }
    }
}
