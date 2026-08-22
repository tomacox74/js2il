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
    public void PropertyRead_SameShapeReceiversShareOneMonomorphicEntryAcrossThousandsOfInstances()
    {
        WithRealm(
            () =>
            {
                // Mirrors the GraphNode.pos scenario: thousands of distinct
                // JsObject instances share one JsShape (same own property
                // added in the same order) and one own plain-data "pos" slot
                // holding a distinct reference value per instance.
                const int count = 5_000;
                var receivers = new JsObject[count];
                var expectedValues = new object[count];

                for (var index = 0; index < count; index++)
                {
                    var receiver = new JsObject();
                    var pos = new JsObject();
                    pos.SetValue("x", (double)index);
                    receiver.SetValue("pos", pos);
                    receivers[index] = receiver;
                    expectedValues[index] = pos;
                }

                for (var index = 0; index < count; index++)
                {
                    var value = DynamicLookupInlineCache.GetItem(
                        receivers[index],
                        "pos",
                        "same-shape");
                    Assert.Same(expectedValues[index], value);
                }

                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("same-shape"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);
            });
    }

    [Fact]
    public void PropertyRead_ExistingValueWriteRemainsMonomorphicHitAndReturnsLatestValue()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "first");

                Assert.Equal(
                    "first",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "write-hit"));
                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("write-hit"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);

                // Plain writes to an already-cached slot must not invalidate
                // the entry (no shape transition, no descriptor change) and
                // every subsequent read must observe the latest value.
                for (var index = 0; index < 25; index++)
                {
                    receiver.SetValue("value", $"value:{index}");
                    Assert.Equal(
                        $"value:{index}",
                        DynamicLookupInlineCache.GetItem(
                            receiver,
                            "value",
                            "write-hit"));
                    Assert.Equal(
                        DynamicLookupInlineCacheState.Monomorphic,
                        site.State);
                    Assert.Equal(1, site.EntryCount);
                }
            });
    }

    [Fact]
    public void PropertyRead_AddDeleteReaddAndAccessorRedefinition_NeverReturnsStaleData()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "first");
                Assert.Equal(
                    "first",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));

                Assert.True(receiver.Remove("value"));
                Assert.Null(
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));

                receiver.SetValue("value", "second");
                Assert.Equal(
                    "second",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));

                var getterCalls = 0;
                BuiltinFunction0 getter = _ =>
                {
                    getterCalls++;
                    return "accessor:value";
                };
                PropertyDescriptorStore.DefineOrUpdate(
                    receiver,
                    "value",
                    new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Accessor,
                        Get = BuiltinDelegateFunctionAdapter
                            .FromDelegate(getter),
                        Enumerable = true,
                        Configurable = true
                    });

                Assert.Equal(
                    "accessor:value",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));
                Assert.Equal(1, getterCalls);

                PropertyDescriptorStore.DefineOrUpdate(
                    receiver,
                    "value",
                    new JsPropertyDescriptor
                    {
                        Kind = JsPropertyDescriptorKind.Data,
                        Value = "third",
                        Writable = true,
                        Enumerable = true,
                        Configurable = true
                    });
                Assert.Equal(
                    "third",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));

                receiver.SetValue("value", "fourth");
                Assert.Equal(
                    "fourth",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "lifecycle"));
            });
    }

    [Fact]
    public void PropertyRead_ProxyReceiverBypassesShapeCache()
    {
        WithRealm(
            () =>
            {
                var target = new JsObject();
                target.SetValue("value", "target-value");
                var handler = new JsObject();
                var proxy = new JavaScriptRuntime.Proxy(target, handler);

                Assert.Equal(
                    "target-value",
                    DynamicLookupInlineCache.GetItem(
                        proxy,
                        "value",
                        "proxy-bypass"));

                target.SetValue("value", "target-value-updated");
                Assert.Equal(
                    "target-value-updated",
                    DynamicLookupInlineCache.GetItem(
                        proxy,
                        "value",
                        "proxy-bypass"));

                // Proxy receivers are excluded before any site/entry is
                // ever created (CanCacheReceiver requires an exact JsObject).
                Assert.Null(
                    DynamicLookupInlineCache
                        .GetSiteForTests("proxy-bypass"));
            });
    }

    [Fact]
    public void PropertyRead_SymbolKeyBypassesShapeCache()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                var symbol = new Symbol("marker");
                var symbolKey =
                    ObjectRuntime.ToPropertyKeyString(symbol);
                receiver.SetValue(symbolKey, "symbol-value");

                Assert.Equal(
                    "symbol-value",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        symbolKey,
                        "symbol-bypass"));

                receiver.SetValue(symbolKey, "symbol-value-updated");
                Assert.Equal(
                    "symbol-value-updated",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        symbolKey,
                        "symbol-bypass"));

                // Encoded symbol keys are rejected before an entry is built,
                // so no site is ever created for this site key.
                Assert.Null(
                    DynamicLookupInlineCache
                        .GetSiteForTests("symbol-bypass"));
            });
    }

    [Fact]
    public void PropertyRead_CachesNullAndUndefinedValuesAcrossRepeatedHits()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue("nullValue", JsNull.Null);
                receiver.SetValue("undefinedValue", null);

                for (var attempt = 0; attempt < 3; attempt++)
                {
                    Assert.Equal(
                        JsNull.Null,
                        DynamicLookupInlineCache.GetItem(
                            receiver,
                            "nullValue",
                            "null-read"));
                    Assert.Null(
                        DynamicLookupInlineCache.GetItem(
                            receiver,
                            "undefinedValue",
                            "undefined-read"));
                }

                var nullSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("null-read"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    nullSite.State);
                var undefinedSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("undefined-read"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    undefinedSite.State);
            });
    }

    [Fact]
    public void CallMember0_SharesOneMonomorphicEntryAcrossSameShapeReceivers()
    {
        WithRealm(
            () =>
            {
                const int count = 500;
                var receivers = new JsObject[count];

                for (var index = 0; index < count; index++)
                {
                    var receiver = new JsObject();
                    receiver.SetValue(
                        "method",
                        CreateFunction($"result:{index}"));
                    receivers[index] = receiver;
                }

                for (var index = 0; index < count; index++)
                {
                    Assert.Equal(
                        $"result:{index}",
                        DynamicLookupInlineCache.CallMember0(
                            receivers[index],
                            "method",
                            "call-same-shape"));
                }

                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache
                        .GetSiteForTests("call-same-shape"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);
            });
    }

    [Fact]
    public void CallMember1_SharesPrototypeEntryAcrossArraysAndPreservesThisAndArgument()
    {
        WithRealm(
            () =>
            {
                const int count = 500;
                var prototype = new JsObject();
                var receivers = Enumerable
                    .Range(0, count)
                    .Select(
                        _ =>
                        {
                            var receiver = new JavaScriptRuntime.Array();
                            PrototypeChain.SetPrototype(receiver, prototype);
                            return receiver;
                        })
                    .ToArray();
                BuiltinFunction1 method = (thisArgument, argument0) =>
                {
                    var index = (int)(double)argument0!;
                    return ReferenceEquals(
                        thisArgument,
                        receivers[index]);
                };
                prototype.SetValue(
                    "method",
                    BuiltinDelegateFunctionAdapter.FromDelegate(method));

                for (var index = 0; index < count; index++)
                {
                    Assert.Equal(
                        true,
                        DynamicLookupInlineCache.CallMember1(
                            receivers[index],
                            "method",
                            (double)index,
                            "call1-prototype-same-shape"));
                }

                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "call1-prototype-same-shape"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);
            });
    }

    [Fact]
    public void CallMember1_CachesUserMethodOnSharedArrayPrototype()
    {
        WithRealm(
            () =>
            {
                BuiltinFunction1 method =
                    static (_, argument0) => argument0;
                JavaScriptRuntime.Array.Prototype.SetValue(
                    "phase4Method",
                    BuiltinDelegateFunctionAdapter
                        .FromDelegate(method));
                var receiver =
                    new JavaScriptRuntime.Array();

                Assert.Equal(
                    "argument",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "phase4Method",
                        "argument",
                        "call1-shared-array-prototype"));
                Assert.Equal(
                    "updated",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "phase4Method",
                        "updated",
                        "call1-shared-array-prototype"));

                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "call1-shared-array-prototype"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    site.State);
                Assert.Equal(1, site.EntryCount);
            });
    }

    [Fact]
    public void CallMember1_InvalidatesForShadowingReassignmentDescriptorsAndPrototypeMutation()
    {
        WithRealm(
            () =>
            {
                var prototype = new JsObject();
                prototype.SetValue(
                    "method",
                    CreateFunction1("prototype:first"));
                var receiver = new JavaScriptRuntime.Array();
                PrototypeChain.SetPrototype(receiver, prototype);

                Assert.Equal(
                    "prototype:first:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));

                prototype.SetValue(
                    "method",
                    CreateFunction1("prototype:updated"));
                Assert.Equal(
                    "prototype:updated:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));

                receiver.SetValue(
                    "method",
                    CreateFunction1("own"));
                Assert.Equal(
                    "own:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));

                Assert.True(receiver.Remove("method"));
                Assert.Equal(
                    "prototype:updated:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));

                var getterCalls = 0;
                BuiltinFunction0 getter = _ =>
                {
                    getterCalls++;
                    return CreateFunction1("accessor");
                };
                PropertyDescriptorStore.DefineOrUpdate(
                    prototype,
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
                    "accessor:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));
                Assert.Equal(1, getterCalls);

                var replacementPrototype = new JsObject();
                replacementPrototype.SetValue(
                    "method",
                    CreateFunction1("replacement"));
                PrototypeChain.SetPrototype(
                    receiver,
                    replacementPrototype);
                Assert.Equal(
                    "replacement:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-invalidation"));
            });
    }

    [Fact]
    public void CallMember1_TransitionsToPolymorphicThenMegamorphic()
    {
        WithRealm(
            () =>
            {
                var prototype = new JsObject();
                prototype.SetValue(
                    "method",
                    CreateFunction1("prototype"));
                var receivers = Enumerable
                    .Range(
                        0,
                        DynamicLookupInlineCacheSite
                            .MaxPolymorphicEntries + 1)
                    .Select(
                        index =>
                        {
                            var receiver = new JavaScriptRuntime.Array();
                            receiver.SetValue($"shape{index}", true);
                            PrototypeChain.SetPrototype(receiver, prototype);
                            return receiver;
                        })
                    .ToArray();

                foreach (var receiver in receivers)
                {
                    Assert.Equal(
                        "prototype:arg",
                        DynamicLookupInlineCache.CallMember1(
                            receiver,
                            "method",
                            "arg",
                            "call1-transition"));
                }

                var site = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "call1-transition"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Megamorphic,
                    site.State);
                Assert.Equal(0, site.EntryCount);
            });
    }

    [Fact]
    public void CallMember1_ProxyBypassesCache()
    {
        WithRealm(
            () =>
            {
                var target = new JsObject();
                target.SetValue(
                    "method",
                    CreateFunction1("target"));
                var proxy = new JavaScriptRuntime.Proxy(
                    target,
                    new JsObject());

                Assert.Equal(
                    "target:arg",
                    DynamicLookupInlineCache.CallMember1(
                        proxy,
                        "method",
                        "arg",
                        "call1-proxy"));
                Assert.Null(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "call1-proxy"));
            });
    }

    [Fact]
    public void CallMember1_SymbolKeyBypassesCache()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                var symbolKey = ObjectRuntime.ToPropertyKeyString(
                    new Symbol("method"));
                receiver.SetValue(
                    symbolKey,
                    CreateFunction1("symbol"));

                Assert.Equal(
                    "symbol:arg",
                    DynamicLookupInlineCache.CallMember1(
                        receiver,
                        symbolKey,
                        "arg",
                        "call1-symbol"));
                Assert.Null(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "call1-symbol"));
            });
    }

    [Fact]
    public void Site_TransitionsToPolymorphicThenMegamorphic()
    {
        WithRealm(
            () =>
            {
                // Shape-keyed entries are shared across same-shape receivers, so
                // exercising real polymorphic/megamorphic transitions requires
                // receivers whose shapes are genuinely distinct from each other.
                var receivers = CreateReceiversWithDistinctShapes(
                    DynamicLookupInlineCacheSite
                        .MaxPolymorphicEntries + 1);

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
    public void GeneratedTerminalFlag_BypassesMegamorphicSite()
    {
        WithRealm(
            () =>
            {
                var terminal = 0;
                var receivers = CreateReceiversWithDistinctShapes(
                    DynamicLookupInlineCacheSite
                        .MaxPolymorphicEntries + 1);

                foreach (var receiver in receivers)
                {
                    _ = DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "generated-terminal",
                        ref terminal);
                }

                Assert.Equal(1, terminal);
                receivers[0].SetValue(
                    "value",
                    "updated");
                Assert.Equal(
                    "updated",
                    DynamicLookupInlineCache.GetItem(
                        receivers[0],
                        "value",
                        "generated-terminal",
                        ref terminal));
            });
    }

    [Fact]
    public void GeneratedTerminalFlag_DoesNotShareRealmCacheState()
    {
        var firstServices =
            RuntimeServices.BuildServiceProvider();
        var secondServices =
            RuntimeServices.BuildServiceProvider();
        var firstContext =
            RuntimeExecutionContext.GetOrCreate(firstServices);
        var secondContext =
            RuntimeExecutionContext.GetOrCreate(secondServices);
        var terminal = 0;

        try
        {
            using (firstContext.EnterAsRoot())
            {
                foreach (var index in Enumerable.Range(
                    0,
                    DynamicLookupInlineCacheSite
                        .MaxPolymorphicEntries + 1))
                {
                    var receiver = new JsObject();
                    receiver.SetValue($"shape{index}", true);
                    receiver.SetValue("value", index);
                    _ = DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "generated-realms",
                        ref terminal);
                }

                Assert.Equal(1, terminal);
                var firstSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "generated-realms"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Megamorphic,
                    firstSite.State);
            }

            using (secondContext.EnterAsRoot())
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "second");
                Assert.Equal(
                    "second",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "generated-realms",
                        ref terminal));
                Assert.Null(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "generated-realms"));
            }
        }
        finally
        {
            firstServices.OwningRealm!.Agent.Cluster.Dispose();
            secondServices.OwningRealm!.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void RemovedSite_IsNotServedFromRecentSiteCache()
    {
        WithRealm(
            () =>
            {
                var receiver = new JsObject();
                receiver.SetValue("value", "first");
                Assert.Equal(
                    "first",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "remove-recent"));

                Assert.True(
                    DynamicLookupInlineCache.RemoveSiteForBenchmarks(
                        "remove-recent"));
                receiver.SetValue("value", "second");
                Assert.Equal(
                    "second",
                    DynamicLookupInlineCache.GetItem(
                        receiver,
                        "value",
                        "remove-recent"));

                var replacement = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "remove-recent"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Monomorphic,
                    replacement.State);
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
    public void MonomorphicCallMember1PrototypeHit_AllocatesZeroBytes()
    {
        WithRealm(
            () =>
            {
                var prototype = new JsObject();
                BuiltinFunction1 method =
                    static (_, _) => "cached";
                prototype.SetValue(
                    "method",
                    BuiltinDelegateFunctionAdapter
                        .FromDelegate(method));
                var receiver = new JavaScriptRuntime.Array();
                PrototypeChain.SetPrototype(receiver, prototype);
                _ = DynamicLookupInlineCache.CallMember1(
                    receiver,
                    "method",
                    "arg",
                    "call1-allocation");

                for (var index = 0; index < 100; index++)
                {
                    _ = DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-allocation");
                }

                var before =
                    GC.GetAllocatedBytesForCurrentThread();
                for (var index = 0; index < 1_000; index++)
                {
                    _ = DynamicLookupInlineCache.CallMember1(
                        receiver,
                        "method",
                        "arg",
                        "call1-allocation");
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
    public void LiveRealmCallMember1Cache_DoesNotRetainPrototypeOrCallable()
    {
        var services =
            RuntimeServices.BuildServiceProvider();
        var context =
            RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            var references =
                PopulateCollectibleCallMember1Entry(context);

            for (var attempt = 0;
                 attempt < 10
                    && (references.Prototype.IsAlive
                        || references.Callable.IsAlive);
                 attempt++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            Assert.False(references.Prototype.IsAlive);
            Assert.False(references.Callable.IsAlive);
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

            using (context.EnterAsRoot())
            {
                // Confirm real polymorphism was reached (distinct shapes)
                // before the receivers/shapes are collected below.
                var populatedSite = Assert.IsType<
                    DynamicLookupInlineCacheSite>(
                    DynamicLookupInlineCache.GetSiteForTests(
                        "gc-capacity"));
                Assert.Equal(
                    DynamicLookupInlineCacheState.Polymorphic,
                    populatedSite.State);
                Assert.Equal(
                    DynamicLookupInlineCacheSite.MaxPolymorphicEntries,
                    populatedSite.EntryCount);
            }

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

    private static object CreateFunction1(string result)
    {
        BuiltinFunction1 function =
            (_, argument0) => $"{result}:{argument0}";
        return BuiltinDelegateFunctionAdapter
            .FromDelegate(function);
    }

    /// <summary>
    /// Creates receivers whose shapes are pairwise distinct (via a unique marker
    /// property per index) while every receiver also owns a "value" data slot.
    /// Used to exercise genuine polymorphic/megamorphic transitions, since
    /// shape-keyed entries otherwise share one monomorphic entry across
    /// same-shape receivers.
    /// </summary>
    private static JsObject[] CreateReceiversWithDistinctShapes(int count)
        => Enumerable
            .Range(0, count)
            .Select(
                index =>
                {
                    var receiver = new JsObject();
                    receiver.SetValue($"shape{index}", true);
                    receiver.SetValue(
                        "value",
                        $"value:{index}");
                    return receiver;
                })
            .ToArray();

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
                        // Distinct marker property per receiver produces a
                        // distinct JsShape, so this exercises genuine
                        // polymorphic capacity rather than shared monomorphic
                        // reuse.
                        receiver.SetValue($"gc-shape{index}", true);
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Prototype, WeakReference Callable)
        PopulateCollectibleCallMember1Entry(
            RuntimeExecutionContext context)
    {
        using (context.EnterAsRoot())
        {
            var prototype = new JsObject();
            BuiltinFunction1 first =
                static (_, _) => null;
            BuiltinFunction1 second =
                static (_, argument0) => argument0;
            var function = (BuiltinFunction1)Delegate.Combine(
                first,
                second);
            var callable =
                BuiltinDelegateFunctionAdapter
                    .FromDelegate(function);
            prototype.SetValue("method", callable);
            var receiver =
                new JavaScriptRuntime.Array();
            PrototypeChain.SetPrototype(receiver, prototype);
            _ = DynamicLookupInlineCache.CallMember1(
                receiver,
                "method",
                "argument",
                "call1-gc");
            return (
                new WeakReference(prototype),
                new WeakReference(callable));
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
