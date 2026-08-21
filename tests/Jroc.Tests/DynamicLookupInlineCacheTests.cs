using System.Runtime.CompilerServices;
using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class DynamicLookupInlineCacheTests
{
    [Fact]
    public void PropertyRead_InvalidatesForOwnDescriptorAndPrototypeChanges()
    {
        WithRealm(
            () =>
            {
                var firstPrototype = new JsObject();
                firstPrototype.SetValue("value", "prototype:first");
                var receiver = new JsObject();
                PrototypeChain.SetPrototype(
                    receiver,
                    firstPrototype);

                Assert.Equal(
                    "prototype:first",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "read"));

                receiver.SetValue("value", "own");
                Assert.Equal(
                    "own",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "read"));

                Assert.True(receiver.Remove("value"));
                Assert.Equal(
                    "prototype:first",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "read"));

                firstPrototype.SetValue(
                    "value",
                    "prototype:updated");
                Assert.Equal(
                    "prototype:updated",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "read"));

                var secondPrototype = new JsObject();
                secondPrototype.SetValue(
                    "value",
                    "prototype:second");
                PrototypeChain.SetPrototype(
                    receiver,
                    secondPrototype);
                Assert.Equal(
                    "prototype:second",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "read"));
            });
    }

    [Fact]
    public void PropertyRead_PreservesGenericFallbackSemantics()
    {
        WithRealm(
            () =>
            {
                var prototype = new JsObject();
                var receiver = new JsObject();
                PrototypeChain.SetPrototype(receiver, prototype);

                Assert.Same(
                    prototype,
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "__proto__",
                        "legacy-prototype"));

                prototype.SetValue(
                    "__proto__",
                    "inherited-shadow");
                Assert.Same(
                    prototype,
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "__proto__",
                        "legacy-inherited-prototype"));

                receiver.SetValue("__proto__", "own-shadow");
                Assert.Equal(
                    "own-shadow",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "__proto__",
                        "own-prototype-property"));
                Assert.Null(
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "missing",
                        "missing"));
                Assert.Null(
                    DynamicLookupInlineCache
                        .GetSiteForTests("missing"));

                Assert.Throws<TypeError>(
                    () => DynamicLookupInlineCache.GetItem(
                        null!,
                        "value",
                        "null"));
                Assert.Throws<TypeError>(
                    () => DynamicLookupInlineCache.GetItem(
                        JsNull.Null,
                        "value",
                        "undefined"));
            });
    }

    [Fact]
    public void CallMember_InvalidatesForReplacementDeletionAndAccessor()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue(
                    "method",
                    CreateFunction("first"));

                Assert.Equal(
                    "first",
                    DynamicLookupInlineCache.CallMember0(
                        receiver,
                        "method",
                        "call"));

                receiver.SetValue(
                    "method",
                    CreateFunction("second"));
                Assert.Equal(
                    "second",
                    DynamicLookupInlineCache.CallMember0(
                        receiver,
                        "method",
                        "call"));

                var getterCalls = 0;
                BuiltinFunction0 getter = _ =>
                {
                    getterCalls++;
                    return CreateFunction("accessor");
                };
                PropertyDescriptorStore.DefineOrUpdate(
                    receiver,
                    "method",
                    new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Accessor,
                        Get = BuiltinDelegateFunctionAdapter
                            .FromDelegate(getter),
                        Enumerable = true,
                        Configurable = true
                    });

                Assert.Equal(
                    "accessor",
                    DynamicLookupInlineCache.CallMember0(
                        receiver,
                        "method",
                        "call"));
                Assert.Equal(1, getterCalls);

                Assert.True(receiver.Remove("method"));
                var prototype = new JsObject();
                prototype.SetValue(
                    "method",
                    CreateFunction("prototype"));
                PrototypeChain.SetPrototype(receiver, prototype);

                Assert.Equal(
                    "prototype",
                    DynamicLookupInlineCache.CallMember0(
                        receiver,
                        "method",
                        "call"));
            });
    }

    [Fact]
    public void Site_TransitionsToPolymorphicThenMegamorphic()
    {
        WithRealm(
            () =>
            {
                var receivers = Enumerable
                    .Range(
                        0,
                        DynamicLookupInlineCacheSite
                            .MaxPolymorphicEntries + 1)
                    .Select(
                        index =>
                        {
                            var receiver = new JsObject();
                            receiver.SetValue(
                                "value",
                                $"value:{index}");
                            return receiver;
                        })
                    .ToArray();

                Assert.Equal(
                    "value:0",
                    DynamicLookupInlineCache.GetItem(
                        receivers[0],
                        "value",
                        "transition"));
                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("transition"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);

                for (var index = 1;
                     index
                        < DynamicLookupInlineCacheSite
                            .MaxPolymorphicEntries;
                     index++)
                {
                    Assert.Equal(
                        $"value:{index}",
                        DynamicLookupInlineCache.GetItem(
                            receivers[index],
                            "value",
                            "transition"));
                }

                Assert.Equal(
                    DynamicLookupInlineCacheState.Polymorphic,
                    site.State);
                Assert.Equal(
                    DynamicLookupInlineCacheSite
                        .MaxPolymorphicEntries,
                    site.EntryCount);

                Assert.Equal(
                    "value:4",
                    DynamicLookupInlineCache.GetItem(
                        receivers[^1],
                        "value",
                        "transition"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Megamorphic,
                    site.State);
                Assert.Equal(0, site.EntryCount);

                receivers[0].SetValue(
                    "value",
                    "value:updated");
                Assert.Equal(
                    "value:updated",
                    DynamicLookupInlineCache.GetItem(
                        receivers[0],
                        "value",
                        "transition"));
            });
    }

    [Fact]
    public void Sites_AreRealmOwnedAndClearedAtDisposal()
    {
        var firstServices =
            RuntimeServices.BuildServiceProvider();
        var secondServices =
            RuntimeServices.BuildServiceProvider();
        var firstContext =
            RuntimeExecutionContext.GetOrCreate(firstServices);
        var secondContext =
            RuntimeExecutionContext.GetOrCreate(secondServices);
        DynamicLookupInlineCacheSite firstSite;
        DynamicLookupInlineCacheSite secondSite;

        try
        {
            using (firstContext.EnterAsRoot())
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "first");
                _ = DynamicLookupInlineCache.GetItem(
                    receiver,
                    "value",
                    "shared-key");
                firstSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("shared-key"));
            }

            using (secondContext.EnterAsRoot())
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "second");
                _ = DynamicLookupInlineCache.GetItem(
                    receiver,
                    "value",
                    "shared-key");
                secondSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("shared-key"));
            }

            Assert.NotSame(firstSite, secondSite);
            firstServices.OwningRealm!.Dispose();
            Assert.Empty(
                firstServices.OwningRealm.ValueCaches
                    .DynamicLookupInlineCaches);
            Assert.Single(
                secondServices.OwningRealm!.ValueCaches
                    .DynamicLookupInlineCaches);
        }
        finally
        {
            firstServices.OwningRealm!.Agent.Cluster.Dispose();
            secondServices.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void MonomorphicPropertyHit_AllocatesZeroBytes()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "cached");
                _ = DynamicLookupInlineCache.GetItem(
                    receiver,
                    "value",
                    "allocation");

                for (var index = 0; index < 100; index++)
                {
                    _ = DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "allocation");
                }

                var before =
                    GC.GetAllocatedBytesForCurrentThread();
                for (var index = 0; index < 1_000; index++)
                {
                    _ = DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "allocation");
                }
                var allocated =
                    GC.GetAllocatedBytesForCurrentThread()
                    - before;

                Assert.Equal(0, allocated);
            });
    }

    [Fact]
    public void DisposedRealmCache_DoesNotRetainReceiver()
    {
        var receiverReference =
            CreateDisposedRealmReceiverReference();

        for (var attempt = 0;
             attempt < 10 && receiverReference.IsAlive;
             attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.False(receiverReference.IsAlive);
    }

    [Fact]
    public void LiveRealmCache_DoesNotRetainReceiverOrValue()
    {
        var services =
            RuntimeServices.BuildServiceProvider();
        var context =
            RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            var references =
                PopulateLiveRealmCache(context);

            for (var attempt = 0;
                 attempt < 10
                    && (references.Receiver.IsAlive
                        || references.Value.IsAlive);
                 attempt++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            Assert.False(references.Receiver.IsAlive);
            Assert.False(references.Value.IsAlive);
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void CollectedEntries_DoNotConsumePolymorphicCapacity()
    {
        var services =
            RuntimeServices.BuildServiceProvider();
        var context =
            RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            var references =
                PopulateCollectibleCacheEntries(context);
            foreach (var reference in references)
            {
                for (var attempt = 0;
                     attempt < 10 && reference.IsAlive;
                     attempt++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                Assert.False(reference.IsAlive);
            }

            using (context.EnterAsRoot())
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "live");
                Assert.Equal(
                    "live",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "gc-capacity"));
                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "gc-capacity"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);
            }
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    private static object CreateFunction(string result)
    {
        BuiltinFunction0 function = _ => result;
        return BuiltinDelegateFunctionAdapter
            .FromDelegate(function);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference
        CreateDisposedRealmReceiverReference()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var realm = services.OwningRealm!;
        var context =
            RuntimeExecutionContext.GetOrCreate(services);
        var receiver = new JsObject();

        using (context.EnterAsRoot())
        {
            receiver.SetValue("value", "cached");
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                "value",
                "disposed");
        }

        var receiverReference =
            new WeakReference(receiver);
        realm.Agent.Cluster.Dispose();
        return receiverReference;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Receiver, WeakReference Value)
        PopulateLiveRealmCache(
            RuntimeExecutionContext context)
    {
        using (context.EnterAsRoot())
        {
            var receiver = new JsObject();
            var value = new JsObject();
            receiver.SetValue("value", value);
            _ = DynamicLookupInlineCache.GetItem(
                receiver,
                "value",
                "live-realm");
            return (
                new WeakReference(receiver),
                new WeakReference(value));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[]
        PopulateCollectibleCacheEntries(
            RuntimeExecutionContext context)
    {
        using (context.EnterAsRoot())
        {
            return Enumerable
                .Range(
                    0,
                    DynamicLookupInlineCacheSite
                        .MaxPolymorphicEntries)
                .Select(
                    index =>
                    {
                        var receiver = new JsObject();
                        receiver.SetValue("value", index);
                        _ = DynamicLookupInlineCache.GetItem(
                            receiver,
                            "value",
                            "gc-capacity");
                        return new WeakReference(receiver);
                    })
                .ToArray();
        }
    }

    private static void WithRealm(Action body)
    {
        var services =
            RuntimeServices.BuildServiceProvider();
        var context =
            RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            using (context.EnterAsRoot())
            {
                _ = GlobalThis.globalThis;
                body();
            }
        }
        finally
        {
            services.OwningRealm!.Agent.Cluster.Dispose();
        }
    }
}
