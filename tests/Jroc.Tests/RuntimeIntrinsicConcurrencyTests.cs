using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

/// <summary>
/// Concurrency regressions for the realm-owned intrinsic graph (GitHub issue #1824).
/// Covers the four hazards the lazy graph introduces: a half-initialized intrinsic
/// escaping to a second thread, a lock inversion between the intrinsic graph and
/// <see cref="BuiltinDelegateFunctionAdapter.InitializationLock"/>, an unstable
/// <see cref="RuntimeIntrinsics.Current"/> answer, and an unbootstrapped realm being
/// observed through the primitive prototype accessors.
/// </summary>
public sealed class RuntimeIntrinsicConcurrencyTests
{
    private static readonly TimeSpan DeadlockTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan SignalTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan BlockedProbe = TimeSpan.FromMilliseconds(500);

    private static readonly Func<object[], object?[]?, object?> LockInversionTarget =
        static (_, _) => null;

    /// <summary>
    /// Slot used by the tests that drive <see cref="RuntimeIntrinsics.GetOrCreate"/>
    /// directly. The runtime itself never materializes it in these realms.
    /// </summary>
    private const RuntimeIntrinsicSlot TestSlot = RuntimeIntrinsicSlot.UrlSearchParamsPrototype;

    [Fact]
    public async Task SimultaneousFirstAccess_BlocksUntilTheInitializerCompleted()
    {
        var intrinsics = new RuntimeIntrinsics();
        using var initializerEntered = new ManualResetEventSlim();
        using var releaseInitializer = new ManualResetEventSlim();

        var owner = Task.Run(() => intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            prototype =>
            {
                initializerEntered.Set();
                Assert.True(releaseInitializer.Wait(SignalTimeout));
                prototype["state"] = "ready";
            }));

        Assert.True(initializerEntered.Wait(SignalTimeout));

        var waiter = Task.Run(() => intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            static _ => throw new InvalidOperationException(
                "A second thread must not run the initializer for an owned slot.")));

        await Task.Delay(BlockedProbe);
        Assert.False(
            waiter.IsCompleted,
            "A second thread observed an intrinsic before its initializer completed.");

        releaseInitializer.Set();

        var owned = await Guarded(owner, "The owning thread never completed the initializer.");
        var observed = await Guarded(waiter, "The waiting thread never observed the intrinsic.");

        Assert.Same(owned, observed);
        Assert.Equal("ready", observed["state"]);
    }

    [Fact]
    public async Task SimultaneousFirstAccessToALazyPrototype_SharesOneFullyWiredInstance()
    {
        const int threadCount = 8;
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();
        using var ready = new Barrier(threadCount);

        var tasks = new Task<(object ArrayPrototype, object StringPrototype, object MapPrototype)>[threadCount];
        for (var index = 0; index < threadCount; index++)
        {
            tasks[index] = Task.Run(() =>
            {
                ready.SignalAndWait();

                var arrayPrototype = JavaScriptRuntime.Array.Prototype;
                var stringPrototype = JavaScriptRuntime.String.Prototype;
                var mapPrototype = JavaScriptRuntime.Map.Prototype;

                // Asserted at the moment of first observation: a half-built prototype
                // would be missing its method surface.
                AssertFullyWired(arrayPrototype, "map", "push", "filter");
                AssertFullyWired(stringPrototype, "slice", "indexOf", "replace");
                AssertFullyWired(mapPrototype, "get", "set", "has");

                return ((object)arrayPrototype, (object)stringPrototype, (object)mapPrototype);
            });
        }

        var results = await GuardedAll(tasks, "Concurrent lazy prototype creation deadlocked.");

        foreach (var result in results)
        {
            Assert.Same(results[0].ArrayPrototype, result.ArrayPrototype);
            Assert.Same(results[0].StringPrototype, result.StringPrototype);
            Assert.Same(results[0].MapPrototype, result.MapPrototype);
        }
    }

    [Fact]
    public void ReentrantResolutionFromAnInitializer_ObservesTheObjectUnderConstruction()
    {
        var intrinsics = new RuntimeIntrinsics();
        JsObject? reentrant = null;

        var value = intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            prototype =>
            {
                // The ECMA-262 intrinsic bootstrap cycles depend on this: the thread that
                // owns the slot resolves it to the object it is still wiring.
                reentrant = intrinsics.GetOrCreate(TestSlot, static () => new JsObject());
                prototype["state"] = "ready";
            });

        Assert.Same(value, reentrant);
        Assert.Equal("ready", value["state"]);
        Assert.Same(value, intrinsics.GetOrCreate(TestSlot, static () => new JsObject()));
    }

    [Fact]
    public void FailedInitializer_LeavesTheSlotRetryableInsteadOfHalfBuilt()
    {
        var intrinsics = new RuntimeIntrinsics();
        JsObject? abandoned = null;

        Assert.Throws<InvalidOperationException>(() => intrinsics.GetOrCreate(
            TestSlot,
            () =>
            {
                abandoned = new JsObject();
                return abandoned;
            },
            static _ => throw new InvalidOperationException("initializer failed")));

        var recovered = intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            static prototype => prototype["state"] = "ready");

        Assert.NotNull(abandoned);
        Assert.NotSame(abandoned, recovered);
        Assert.Equal("ready", recovered["state"]);
    }

    [Fact]
    public async Task FailedInitializer_IsNotObservedByAWaitingThread()
    {
        var intrinsics = new RuntimeIntrinsics();
        using var initializerEntered = new ManualResetEventSlim();
        using var releaseInitializer = new ManualResetEventSlim();

        var owner = Task.Run(() => intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            _ =>
            {
                initializerEntered.Set();
                Assert.True(releaseInitializer.Wait(SignalTimeout));
                throw new InvalidOperationException("initializer failed");
            }));

        Assert.True(initializerEntered.Wait(SignalTimeout));

        var waiter = Task.Run(() => intrinsics.GetOrCreate(
            TestSlot,
            static () => new JsObject(),
            static prototype => prototype["state"] = "ready"));

        await Task.Delay(BlockedProbe);
        Assert.False(
            waiter.IsCompleted,
            "A waiting thread observed the intrinsic while its initializer was still running.");

        releaseInitializer.Set();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => owner.WaitAsync(DeadlockTimeout));

        var recovered = await Guarded(waiter, "The retrying thread never made progress.");
        Assert.Equal("ready", recovered["state"]);
    }

    [Fact]
    public async Task IntrinsicInitializationAndAdapterInitialization_DoNotInvertLocks()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();

        var intrinsics = services.OwningRealm!.Intrinsics;
        var target = LockInversionTarget;
        var adapter = BuiltinDelegateFunctionAdapter.FromDelegate(target);

        using var adapterLockHeld = new ManualResetEventSlim();
        using var initializerEntered = new ManualResetEventSlim();

        // Adapter lock first, then intrinsics. %Function.prototype% already exists (the
        // adapter's own construction materialized it), so a prototype that this realm has
        // never touched is resolved as well.
        var adapterFirst = Task.Run(() =>
        {
            lock (adapter.InitializationLock)
            {
                adapterLockHeld.Set();
                Assert.True(initializerEntered.Wait(SignalTimeout));
                _ = JavaScriptRuntime.Map.Prototype;
                return (object)JavaScriptRuntime.Function.Prototype;
            }
        });

        // Intrinsic initialization first, then the same adapter lock. Before the fix the
        // initializer ran under a realm-wide intrinsic lock, so the task above could never
        // obtain %Function.prototype% and both threads hung.
        var intrinsicFirst = Task.Run(() =>
        {
            Assert.True(adapterLockHeld.Wait(SignalTimeout));
            return (object)intrinsics.GetOrCreate(
                TestSlot,
                static () => new JsObject(),
                prototype =>
                {
                    initializerEntered.Set();
                    JavaScriptRuntime.Function.InitializeFunctionInstance(target, 1d, "lockInversionTarget");
                    prototype["state"] = "ready";
                });
        });

        var results = await GuardedAll(
            [adapterFirst, intrinsicFirst],
            "The intrinsic graph and a builtin adapter deadlocked on inverted lock order.");

        Assert.Same(JavaScriptRuntime.Function.Prototype, results[0]);
        Assert.Same(
            JavaScriptRuntime.Function.Prototype,
            PrototypeChain.GetPrototypeOrNull(adapter));
    }

    [Fact]
    public async Task ConcurrentBootstrapAndAdapterInitialization_ProducesOneFullyWiredRealm()
    {
        const int threadCount = 8;
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();
        using var ready = new Barrier(threadCount);

        var tasks = new Task<Dictionary<string, object?>>[threadCount];
        for (var index = 0; index < threadCount; index++)
        {
            tasks[index] = Task.Run(() =>
            {
                ready.SignalAndWait();

                var global = GlobalThis.globalThis;
                var booleanAdapter = (object)BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean);
                var parseInt = GlobalThis.GetFunctionValue("parseInt");
                var numberPrototype = GlobalThis.NumberPrototypeValue;
                var typeErrorPrototype = GlobalThis.TypeErrorPrototypeValue;
                var objectPrototype = GlobalThis.ObjectPrototypeValue;

                // The realm bootstrap wires all of these, so observing any of them means
                // the bootstrap completed before this thread was allowed to continue.
                Assert.Same(objectPrototype, PrototypeChain.GetPrototypeOrNull(numberPrototype));
                Assert.Same(
                    GlobalThis.ErrorPrototypeValue,
                    PrototypeChain.GetPrototypeOrNull(typeErrorPrototype));
                Assert.Same(
                    JavaScriptRuntime.Function.Prototype,
                    PrototypeChain.GetPrototypeOrNull(booleanAdapter));
                Assert.True(PropertyDescriptorStore.TryGetOwn(booleanAdapter, "name", out _));
                Assert.NotNull(ObjectRuntime.GetItem(global, "Object"));
                Assert.NotNull(ObjectRuntime.GetItem(global, "JSON"));
                AssertFullyWired(objectPrototype, "hasOwnProperty", "toString", "valueOf");

                return new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["globalThis"] = global,
                    ["Boolean"] = booleanAdapter,
                    ["parseInt"] = parseInt,
                    ["Number.prototype"] = numberPrototype,
                    ["TypeError.prototype"] = typeErrorPrototype,
                    ["Object.prototype"] = objectPrototype,
                    ["Array.prototype"] = JavaScriptRuntime.Array.Prototype,
                };
            });
        }

        var results = await GuardedAll(tasks, "Concurrent realm bootstrap deadlocked.");

        foreach (var result in results)
        {
            foreach (var (name, value) in results[0])
            {
                Assert.NotNull(value);
                Assert.Same(value, result[name]);
            }
        }
    }

    [Fact]
    public async Task AmbientRealm_StaysStable_WhileAnotherRealmIsCreated()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();

        var ambient = services.OwningRealm!.Intrinsics;
        var global = GlobalThis.globalThis;
        var arrayPrototype = JavaScriptRuntime.Array.Prototype;
        var numberPrototype = GlobalThis.NumberPrototypeValue;

        using var otherRealmReady = new ManualResetEventSlim();
        using var releaseOtherRealm = new ManualResetEventSlim();

        var otherRealm = Task.Run(() =>
        {
            var otherServices = RuntimeServices.BuildServiceProvider();
            var otherContext = RuntimeExecutionContext.GetOrCreate(otherServices);
            using var otherScope = otherContext.EnterAsRoot();

            _ = GlobalThis.globalThis;
            var otherIntrinsics = otherServices.OwningRealm!.Intrinsics;
            Assert.Same(otherIntrinsics, RuntimeIntrinsics.Current);

            otherRealmReady.Set();
            Assert.True(releaseOtherRealm.Wait(SignalTimeout));
            return (object)otherIntrinsics;
        });

        Assert.True(otherRealmReady.Wait(SignalTimeout));

        // A second live realm must not change any answer this operation already received.
        for (var attempt = 0; attempt < 200; attempt++)
        {
            Assert.Same(ambient, RuntimeIntrinsics.Current);
            Assert.Same(global, GlobalThis.globalThis);
            Assert.Same(arrayPrototype, JavaScriptRuntime.Array.Prototype);
            Assert.Same(numberPrototype, GlobalThis.NumberPrototypeValue);
        }

        releaseOtherRealm.Set();

        var otherIntrinsics = await Guarded(otherRealm, "The second realm never completed.");
        Assert.NotSame(ambient, otherIntrinsics);
    }

    [Fact]
    public void ContextLessResolution_UsesADeterministicProcessDefaultGraph()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        try
        {
            var processDefault = RuntimeIntrinsics.Current;

            // A live realm's graph is never handed to a context-less caller, and creating
            // another realm does not change the context-less answer.
            Assert.NotSame(realm.Intrinsics, processDefault);
            var second = realm.Agent.CreateRealm();
            Assert.NotSame(second.Intrinsics, RuntimeIntrinsics.Current);
            Assert.Same(processDefault, RuntimeIntrinsics.Current);
            Assert.Same(processDefault.ObjectPrototype, RuntimeIntrinsics.Current.ObjectPrototype);
        }
        finally
        {
            realm.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void AmbientRealm_WinsOverTheProcessDefaultGraph()
    {
        var contextLess = RuntimeIntrinsics.Current;
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        var realmIntrinsics = services.OwningRealm!.Intrinsics;

        using (context.EnterAsRoot())
        {
            Assert.Same(realmIntrinsics, RuntimeIntrinsics.Current);
            Assert.Same(realmIntrinsics.ObjectPrototype, GlobalThis.ObjectPrototypeValue);
        }

        Assert.Same(contextLess, RuntimeIntrinsics.Current);
        Assert.NotSame(realmIntrinsics, RuntimeIntrinsics.Current);
    }

    [Fact]
    public async Task PrimitivePrototypeAccessors_NeverExposeAnUnbootstrappedRealm()
    {
        const int threadCount = 8;
        var services = RuntimeServices.BuildServiceProvider();
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();
        using var ready = new Barrier(threadCount);

        var tasks = new Task<bool>[threadCount];
        for (var index = 0; index < threadCount; index++)
        {
            var resolvesErrorFirst = index % 2 == 0;
            tasks[index] = Task.Run(() =>
            {
                ready.SignalAndWait();

                if (resolvesErrorFirst)
                {
                    // Error prototypes get their surface from the realm bootstrap alone,
                    // so this accessor must complete the bootstrap before it returns.
                    var errorPrototype = GlobalThis.ErrorPrototypeValue;
                    AssertFullyWired(errorPrototype, "toString", "message", "name");
                    Assert.Same(
                        GlobalThis.ObjectPrototypeValue,
                        PrototypeChain.GetPrototypeOrNull(errorPrototype));
                }
                else
                {
                    var numberPrototype = GlobalThis.NumberPrototypeValue;
                    AssertFullyWired(numberPrototype, "toFixed", "toString", "valueOf");
                    Assert.Same(
                        GlobalThis.ObjectPrototypeValue,
                        PrototypeChain.GetPrototypeOrNull(numberPrototype));
                }

                Assert.Same(
                    GlobalThis.TypeErrorPrototypeValue,
                    PrototypeChain.GetPrototypeOrNull(new TypeError("boom")));
                return true;
            });
        }

        var results = await GuardedAll(
            tasks,
            "Bootstrapping through a primitive prototype accessor deadlocked.");
        Assert.All(results, Assert.True);
    }

    private static void AssertFullyWired(object prototype, params string[] expectedProperties)
    {
        Assert.NotNull(prototype);
        foreach (var name in expectedProperties)
        {
            Assert.True(
                PropertyDescriptorStore.TryGetOwn(prototype, name, out _),
                $"Observed a partially initialized intrinsic: '{name}' is missing.");
        }
    }

    private static async Task<T> Guarded<T>(Task<T> task, string deadlockMessage)
    {
        try
        {
            return await task.WaitAsync(DeadlockTimeout);
        }
        catch (TimeoutException)
        {
            Assert.Fail(deadlockMessage);
            throw;
        }
    }

    private static async Task<T[]> GuardedAll<T>(Task<T>[] tasks, string deadlockMessage)
    {
        try
        {
            return await Task.WhenAll(tasks).WaitAsync(DeadlockTimeout);
        }
        catch (TimeoutException)
        {
            Assert.Fail(deadlockMessage);
            throw;
        }
    }
}
