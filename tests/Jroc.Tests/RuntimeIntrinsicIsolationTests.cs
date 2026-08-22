using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;

namespace Jroc.Tests;

/// <summary>
/// Covers GitHub issue #1824: globals, intrinsics, descriptors and prototypes are
/// realm-owned. See <see cref="RuntimeIntrinsics"/> for the object graph and
/// <see cref="RuntimeRealm"/> for ownership/lifecycle.
/// </summary>
public sealed class RuntimeIntrinsicIsolationTests
{
    [Fact]
    public void PlainObjectRead_DoesNotMaterializeFunctionPrototype()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var realm = services.OwningRealm!;
        var context = RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            using (context.EnterAsRoot())
            {
                var target = new JsObject();
                target.SetValue("value", "plain");

                Assert.False(
                    realm.Intrinsics.IsPublishedForTests(
                        RuntimeIntrinsicSlot.FunctionPrototype));
                Assert.Equal(
                    "plain",
                    ObjectRuntime.GetProperty(target, "value"));
                Assert.False(
                    realm.Intrinsics.IsPublishedForTests(
                        RuntimeIntrinsicSlot.FunctionPrototype));
            }

        }
        finally
        {
            realm.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public void FunctionPrototypeRestrictedProperties_StillThrow()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var realm = services.OwningRealm!;
        var context = RuntimeExecutionContext.GetOrCreate(services);

        try
        {
            using (context.EnterAsRoot())
            {
                var prototype = JavaScriptRuntime.Function.Prototype;

                Assert.Throws<TypeError>(
                    () => ObjectRuntime.GetProperty(
                        prototype,
                        "caller"));
                Assert.Throws<TypeError>(
                    () => ObjectRuntime.GetItem(
                        prototype,
                        "arguments"));
            }
        }
        finally
        {
            realm.Agent.Cluster.Dispose();
        }
    }

    /// <summary>
    /// Global bindings whose JavaScript-visible identity must differ between realms.
    /// </summary>
    private static readonly string[] RealmOwnedGlobalNames =
    [
        "globalThis",
        "Object",
        "Function",
        "Array",
        "String",
        "Number",
        "Boolean",
        "BigInt",
        "Symbol",
        "Map",
        "Set",
        "WeakMap",
        "WeakSet",
        "WeakRef",
        "FinalizationRegistry",
        "Promise",
        "RegExp",
        "Proxy",
        "Error",
        "TypeError",
        "RangeError",
        "SyntaxError",
        "ReferenceError",
        "EvalError",
        "URIError",
        "AggregateError",
        "ArrayBuffer",
        "SharedArrayBuffer",
        "DataView",
        "Uint8Array",
        "Int32Array",
        "Float64Array",
        "JSON",
        "Intl",
        "Atomics",
        "parseInt",
        "parseFloat",
        "isNaN",
        "isFinite",
        "decodeURI",
        "encodeURI",
        "setTimeout",
        "clearTimeout",
        "setInterval",
    ];

    [Fact]
    public void TwoRealms_HaveDistinctGlobalBindingIdentities()
    {
        var first = CaptureGlobalBindings();
        var second = CaptureGlobalBindings();

        foreach (var name in RealmOwnedGlobalNames)
        {
            Assert.True(first.ContainsKey(name), $"Missing global binding '{name}'.");
            Assert.True(first[name] is not null, $"Global binding '{name}' resolved to null.");
            Assert.True(
                !ReferenceEquals(first[name], second[name]),
                $"Global binding '{name}' has a process-shared identity across realms.");
        }
    }

    [Fact]
    public void TwoRealms_HaveDistinctIntrinsicPrototypeIdentities()
    {
        var first = CaptureIntrinsicPrototypes();
        var second = CaptureIntrinsicPrototypes();

        foreach (var (name, prototype) in first)
        {
            Assert.NotNull(prototype);
            Assert.NotSame(prototype, second[name]);
        }
    }

    [Fact]
    public void TwoRealms_HaveDistinctPrototypeMethodIdentities()
    {
        static Dictionary<string, object?> Capture()
            => WithRealm(static () =>
            {
                var global = GlobalThis.globalThis;
                return new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["Object.prototype.hasOwnProperty"] =
                        ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "hasOwnProperty"),
                    ["Object.prototype.toString"] =
                        ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "toString"),
                    ["Function.prototype.call"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, "call"),
                    ["Function.prototype.bind"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, "bind"),
                    ["Array.prototype.map"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "map"),
                    ["Array.prototype.push"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "push"),
                    ["Array.from"] = ObjectRuntime.GetItem(ObjectRuntime.GetItem(global, "Array")!, "from"),
                    ["String.prototype.slice"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.String.Prototype, "slice"),
                    ["Map.prototype.get"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "get"),
                    ["Promise.prototype.then"] =
                        ObjectRuntime.GetItem(JavaScriptRuntime.Promise.Prototype, "then"),
                    ["JSON.stringify"] =
                        ObjectRuntime.GetItem(ObjectRuntime.GetItem(global, "JSON")!, "stringify"),
                    ["Math.floor"] =
                        ObjectRuntime.GetItem(ObjectRuntime.GetItem(global, "Math")!, "floor"),
                };
            });

        var first = Capture();
        var second = Capture();

        foreach (var (name, method) in first)
        {
            Assert.True(method is JsFunctionObject, $"'{name}' is not a function object.");
            Assert.NotSame(method, second[name]);
        }
    }

    [Fact]
    public void TwoRealms_LinkEachRealmsPrototypeChainToItsOwnIntrinsics()
    {
        static (object ObjectPrototype, object FunctionPrototype, object ArrayPrototype,
            object ErrorPrototype, object TypeErrorPrototype, object? ArrayParent,
            object? TypeErrorParent, object? NumberParent, object? MapParent,
            object? DateConstructorParent) Capture()
            => WithRealm(static () =>
            {
                _ = GlobalThis.globalThis;
                var objectPrototype = GlobalThis.ObjectPrototypeValue;
                return (
                    objectPrototype,
                    JavaScriptRuntime.Function.Prototype,
                    JavaScriptRuntime.Array.Prototype,
                    GlobalThis.ErrorPrototypeValue,
                    GlobalThis.TypeErrorPrototypeValue,
                    PrototypeChain.GetPrototypeOrNull(JavaScriptRuntime.Array.Prototype),
                    PrototypeChain.GetPrototypeOrNull(GlobalThis.TypeErrorPrototypeValue),
                    PrototypeChain.GetPrototypeOrNull(GlobalThis.NumberPrototypeValue),
                    PrototypeChain.GetPrototypeOrNull(JavaScriptRuntime.Map.Prototype),
                    // typeof(Date) is a process-shared CLR handle; its [[Prototype]] link
                    // lives in the realm's fallback slot table and must still be this
                    // realm's %Function.prototype%.
                    PrototypeChain.GetPrototypeOrNull(GlobalThis.Date));
            });

        var first = Capture();
        var second = Capture();

        Assert.NotSame(first.ObjectPrototype, second.ObjectPrototype);
        Assert.NotSame(first.FunctionPrototype, second.FunctionPrototype);

        foreach (var realm in new[] { first, second })
        {
            Assert.Same(realm.ObjectPrototype, realm.ArrayParent);
            Assert.Same(realm.ObjectPrototype, realm.NumberParent);
            Assert.Same(realm.ObjectPrototype, realm.MapParent);
            Assert.Same(realm.ErrorPrototype, realm.TypeErrorParent);
            Assert.Same(realm.FunctionPrototype, realm.DateConstructorParent);
        }
    }

    [Fact]
    public void MutatingAnIntrinsicPrototype_IsInvisibleInAnotherRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();

        WithRealm(firstServices, () =>
        {
            _ = GlobalThis.globalThis;
            ObjectRuntime.SetItem(GlobalThis.ObjectPrototypeValue, "realmMarker", "first");
            ObjectRuntime.SetItem(JavaScriptRuntime.Array.Prototype, "realmMarker", "first");
            ObjectRuntime.SetItem(JavaScriptRuntime.Function.Prototype, "realmMarker", "first");
            ObjectRuntime.SetItem(JavaScriptRuntime.Map.Prototype, "realmMarker", "first");
            ObjectRuntime.SetItem(GlobalThis.NumberPrototypeValue, "realmMarker", "first");
            return true;
        });

        WithRealm(secondServices, () =>
        {
            _ = GlobalThis.globalThis;
            Assert.Null(ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "realmMarker"));
            Assert.Null(ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "realmMarker"));
            Assert.Null(ObjectRuntime.GetItem(JavaScriptRuntime.Function.Prototype, "realmMarker"));
            Assert.Null(ObjectRuntime.GetItem(JavaScriptRuntime.Map.Prototype, "realmMarker"));
            Assert.Null(ObjectRuntime.GetItem(GlobalThis.NumberPrototypeValue, "realmMarker"));
            return true;
        });

        // Round-trip: the mutation is still visible in the realm that made it.
        WithRealm(firstServices, () =>
        {
            Assert.Equal("first", ObjectRuntime.GetItem(GlobalThis.ObjectPrototypeValue, "realmMarker"));
            Assert.Equal("first", ObjectRuntime.GetItem(JavaScriptRuntime.Array.Prototype, "realmMarker"));
            return true;
        });
    }

    [Fact]
    public void RedefiningAnIntrinsicDescriptor_IsInvisibleInAnotherRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();

        var replacement = (Func<object[], object?[]?, object?>)((_, _) => "patched");

        WithRealm(firstServices, () =>
        {
            _ = GlobalThis.globalThis;
            PropertyDescriptorStore.DefineOrUpdate(
                JavaScriptRuntime.Array.Prototype,
                "join",
                new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Data,
                    Enumerable = false,
                    Configurable = true,
                    Writable = true,
                    Value = replacement
                });
            Assert.True(PropertyDescriptorStore.TryGetOwn(
                JavaScriptRuntime.Array.Prototype, "join", out var patched));
            Assert.Same(
                BuiltinDelegateFunctionAdapter.FromDelegate(replacement),
                patched.Value);
            return true;
        });

        WithRealm(secondServices, () =>
        {
            _ = GlobalThis.globalThis;
            Assert.True(PropertyDescriptorStore.TryGetOwn(
                JavaScriptRuntime.Array.Prototype, "join", out var pristine));
            Assert.NotSame(
                BuiltinDelegateFunctionAdapter.FromDelegate(replacement),
                pristine.Value);
            return true;
        });
    }

    [Fact]
    public void DeletingAnIntrinsicPrototypeProperty_IsInvisibleInAnotherRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();

        WithRealm(firstServices, () =>
        {
            _ = GlobalThis.globalThis;
            Assert.True(PropertyDescriptorStore.Delete(JavaScriptRuntime.Array.Prototype, "map"));
            Assert.False(PropertyDescriptorStore.TryGetOwn(
                JavaScriptRuntime.Array.Prototype, "map", out _));
            return true;
        });

        WithRealm(secondServices, () =>
        {
            _ = GlobalThis.globalThis;
            Assert.True(PropertyDescriptorStore.TryGetOwn(
                JavaScriptRuntime.Array.Prototype, "map", out _));
            return true;
        });
    }

    [Fact]
    public void ReassigningAnIntrinsicPrototypeLink_IsInvisibleInAnotherRealm()
    {
        var firstServices = RuntimeServices.BuildServiceProvider();
        var secondServices = RuntimeServices.BuildServiceProvider();

        WithRealm(firstServices, () =>
        {
            _ = GlobalThis.globalThis;
            // typeof(Date) is a process-shared CLR handle, so its [[Prototype]] link is the
            // strongest test that fallback prototype slots are realm-owned.
            PrototypeChain.SetPrototype(GlobalThis.Date, null);
            Assert.Null(PrototypeChain.GetPrototypeOrNull(GlobalThis.Date));
            return true;
        });

        WithRealm(secondServices, () =>
        {
            _ = GlobalThis.globalThis;
            Assert.Same(
                JavaScriptRuntime.Function.Prototype,
                PrototypeChain.GetPrototypeOrNull(GlobalThis.Date));
            return true;
        });
    }

    [Fact]
    public void SharedClrConstructorHandles_ExposeRealmOwnedPrototypeDescriptors()
    {
        static (object? DatePrototype, object? Uint8ArrayPrototype) Capture()
            => WithRealm(static () =>
            {
                _ = GlobalThis.globalThis;
                PropertyDescriptorStore.TryGetOwn(GlobalThis.Date, "prototype", out var date);
                PropertyDescriptorStore.TryGetOwn(typeof(Uint8Array), "prototype", out var uint8);
                return (date.Value, uint8.Value);
            });

        var first = Capture();
        var second = Capture();

        Assert.NotNull(first.DatePrototype);
        Assert.NotNull(first.Uint8ArrayPrototype);
        Assert.NotSame(first.DatePrototype, second.DatePrototype);
        Assert.NotSame(first.Uint8ArrayPrototype, second.Uint8ArrayPrototype);
    }

    [Fact]
    public void RepeatedLookupsWithinOneRealm_PreserveIntrinsicIdentity()
    {
        WithRealm(static () =>
        {
            var global = GlobalThis.globalThis;

            Assert.Same(global, GlobalThis.globalThis);
            Assert.Same(GlobalThis.ObjectPrototypeValue, GlobalThis.ObjectPrototypeValue);
            Assert.Same(JavaScriptRuntime.Function.Prototype, JavaScriptRuntime.Function.Prototype);
            Assert.Same(JavaScriptRuntime.Array.Prototype, JavaScriptRuntime.Array.Prototype);
            Assert.Same(
                ObjectRuntime.GetItem(global, "Map"),
                ObjectRuntime.GetItem(global, "Map"));
            Assert.Same(
                BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean),
                BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean));
            Assert.Same(
                GlobalThis.GetFunctionValue("parseInt"),
                GlobalThis.GetFunctionValue("parseInt"));
            return true;
        });
    }

    [Fact]
    public void TwoRealms_HaveDistinctBuiltinDelegateAdapterIdentities()
    {
        static (object Boolean, object Number, object ParseInt, object SetTimeout) Capture()
            => WithRealm(static () => (
                (object)BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean),
                BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Number),
                GlobalThis.GetFunctionValue("parseInt"),
                GlobalThis.GetFunctionValue("setTimeout")));

        var first = Capture();
        var second = Capture();

        Assert.NotSame(first.Boolean, second.Boolean);
        Assert.NotSame(first.Number, second.Number);
        Assert.NotSame(first.ParseInt, second.ParseInt);
        Assert.NotSame(first.SetTimeout, second.SetTimeout);
    }

    [Fact]
    public void EachRealm_OwnsADistinctIntrinsicsGraphInstance()
    {
        var first = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var second = first.Agent.CreateRealm();

        try
        {
            Assert.NotSame(first.Intrinsics, second.Intrinsics);
            Assert.NotEqual(first.Intrinsics.Id, second.Intrinsics.Id);
            Assert.NotSame(first.Intrinsics.ObjectPrototype, second.Intrinsics.ObjectPrototype);
            Assert.NotSame(first.Intrinsics.NumberPrototype, second.Intrinsics.NumberPrototype);
            Assert.NotSame(first.Intrinsics.ErrorPrototype, second.Intrinsics.ErrorPrototype);
            Assert.NotSame(first.Intrinsics.SymbolPrototype, second.Intrinsics.SymbolPrototype);
            Assert.NotSame(first.Intrinsics.Json, second.Intrinsics.Json);
            Assert.NotSame(first.Intrinsics.IntrinsicDescriptors, second.Intrinsics.IntrinsicDescriptors);
            Assert.NotSame(first.Intrinsics.PrototypeSlots, second.Intrinsics.PrototypeSlots);
            Assert.NotSame(first.Intrinsics.BuiltinAdapters, second.Intrinsics.BuiltinAdapters);
            Assert.NotSame(first.Intrinsics.GlobalFunctionValues, second.Intrinsics.GlobalFunctionValues);
        }
        finally
        {
            first.Agent.Cluster.Dispose();
        }
    }

    [Fact]
    public async Task ConcurrentRealmCreation_ProducesDeterministicIsolatedIntrinsics()
    {
        const int realmCount = 6;
        using var ready = new Barrier(realmCount);

        var tasks = new Task<Dictionary<string, object?>>[realmCount];
        for (var index = 0; index < realmCount; index++)
        {
            tasks[index] = Task.Run(() =>
            {
                var services = RuntimeServices.BuildServiceProvider();
                var context = RuntimeExecutionContext.GetOrCreate(services);
                using var scope = context.EnterAsRoot();

                ready.SignalAndWait();

                var global = (GlobalThis)GlobalThis.globalThis;
                var objectPrototype = GlobalThis.ObjectPrototypeValue;

                // Every realm must observe a fully and identically wired intrinsic graph,
                // regardless of creation order or interleaving.
                Assert.Same(global.Intrinsics.ObjectPrototype, objectPrototype);
                Assert.Same(objectPrototype, PrototypeChain.GetPrototypeOrNull(JavaScriptRuntime.Function.Prototype));
                Assert.Same(objectPrototype, PrototypeChain.GetPrototypeOrNull(JavaScriptRuntime.Array.Prototype));
                Assert.Same(objectPrototype, PrototypeChain.GetPrototypeOrNull(GlobalThis.NumberPrototypeValue));
                Assert.Same(
                    GlobalThis.ErrorPrototypeValue,
                    PrototypeChain.GetPrototypeOrNull(GlobalThis.TypeErrorPrototypeValue));
                Assert.True(PropertyDescriptorStore.TryGetOwn(
                    JavaScriptRuntime.Array.Prototype, "map", out _));

                return CaptureRealmIdentities();
            });
        }

        var results = await Task.WhenAll(tasks);

        for (var i = 0; i < realmCount; i++)
        {
            Assert.Equal(results[0].Keys.OrderBy(k => k, StringComparer.Ordinal),
                results[i].Keys.OrderBy(k => k, StringComparer.Ordinal));

            for (var j = i + 1; j < realmCount; j++)
            {
                foreach (var key in results[i].Keys)
                {
                    Assert.NotSame(results[i][key], results[j][key]);
                }
            }
        }
    }

    [Fact]
    public void RealmDisposal_ReleasesIntrinsicObjectGraph()
    {
        var references = CreateAndDisposeRealmIntrinsics();

        for (var attempt = 0;
            attempt < 10 && (references.Realm.IsAlive || references.ObjectPrototype.IsAlive
                || references.ArrayPrototype.IsAlive || references.BooleanAdapter.IsAlive
                || references.GlobalFunction.IsAlive);
            attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.False(references.Realm.IsAlive);
        Assert.False(references.ObjectPrototype.IsAlive);
        Assert.False(references.ArrayPrototype.IsAlive);
        Assert.False(references.BooleanAdapter.IsAlive);
        Assert.False(references.GlobalFunction.IsAlive);
    }

    [Fact]
    public void DisposedRealm_DropsItsIntrinsicSlots()
    {
        var realm = RuntimeOwnershipFactory.CreateIsolatedRealm();
        var intrinsics = realm.Intrinsics;
        var objectPrototype = intrinsics.ObjectPrototype;

        realm.Agent.Cluster.Dispose();

        Assert.NotSame(objectPrototype, intrinsics.ObjectPrototype);
        Assert.Empty(intrinsics.GlobalFunctionValues);
    }

    [Fact]
    public void PrototypeChain_NoLongerExposesAProcessWideBehaviorSwitch()
    {
        // Enabled is now a fixed compatibility shim (always true) rather than a
        // mutable process-wide switch, and Enable() is a no-op retained only for
        // existing call sites and compiler-emitted prologue calls.
        Assert.True(PrototypeChain.Enabled);
        PrototypeChain.Enable();
        Assert.True(PrototypeChain.Enabled);
    }

    private static Dictionary<string, object?> CaptureGlobalBindings()
        => WithRealm(static () =>
        {
            var global = GlobalThis.globalThis;
            var bindings = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var name in RealmOwnedGlobalNames)
            {
                bindings[name] = ObjectRuntime.GetItem(global, name);
            }

            return bindings;
        });

    private static Dictionary<string, object?> CaptureIntrinsicPrototypes()
        => WithRealm(static () =>
        {
            _ = GlobalThis.globalThis;
            return new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["Object.prototype"] = GlobalThis.ObjectPrototypeValue,
                ["Function.prototype"] = JavaScriptRuntime.Function.Prototype,
                ["Array.prototype"] = JavaScriptRuntime.Array.Prototype,
                ["%ArrayPrototypeTemplate%"] = JavaScriptRuntime.Array.ImmutablePrototype,
                ["String.prototype"] = JavaScriptRuntime.String.Prototype,
                ["%StringIteratorPrototype%"] = JavaScriptRuntime.String.StringIteratorPrototype,
                ["Number.prototype"] = GlobalThis.NumberPrototypeValue,
                ["Boolean.prototype"] = GlobalThis.BooleanPrototypeValue,
                ["Symbol.prototype"] = GlobalThis.SymbolPrototypeValue,
                ["Error.prototype"] = GlobalThis.ErrorPrototypeValue,
                ["TypeError.prototype"] = GlobalThis.TypeErrorPrototypeValue,
                ["Map.prototype"] = JavaScriptRuntime.Map.Prototype,
                ["Set.prototype"] = JavaScriptRuntime.Set.Prototype,
                ["WeakMap.prototype"] = JavaScriptRuntime.WeakMap.Prototype,
                ["WeakSet.prototype"] = JavaScriptRuntime.WeakSet.Prototype,
                ["WeakRef.prototype"] = JavaScriptRuntime.WeakRef.Prototype,
                ["FinalizationRegistry.prototype"] = JavaScriptRuntime.FinalizationRegistry.Prototype,
                ["Promise.prototype"] = JavaScriptRuntime.Promise.Prototype,
                ["RegExp.prototype"] = JavaScriptRuntime.RegExp.Prototype,
                ["Date.prototype"] = GlobalThis.DatePrototypeValue,
                ["DataView.prototype"] = JavaScriptRuntime.DataView.Prototype,
                ["Uint8Array.prototype"] = JavaScriptRuntime.Uint8Array.Prototype,
                ["%IteratorPrototype%"] = JavaScriptRuntime.Iterator.Prototype,
                ["%AsyncIteratorPrototype%"] = JavaScriptRuntime.AsyncIterator.Prototype,
                ["%AsyncFunction.prototype%"] = JavaScriptRuntime.AsyncFunction.Prototype,
                ["%AsyncGeneratorPrototype%"] = JavaScriptRuntime.AsyncGeneratorObject.PrototypeObject,
            };
        });

    private static Dictionary<string, object?> CaptureRealmIdentities()
    {
        var global = GlobalThis.globalThis;
        var identities = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Object.prototype"] = GlobalThis.ObjectPrototypeValue,
            ["Function.prototype"] = JavaScriptRuntime.Function.Prototype,
            ["Array.prototype"] = JavaScriptRuntime.Array.Prototype,
            ["Error.prototype"] = GlobalThis.ErrorPrototypeValue,
            ["Map.prototype"] = JavaScriptRuntime.Map.Prototype,
            ["Boolean"] = BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean),
            ["parseInt"] = GlobalThis.GetFunctionValue("parseInt"),
        };

        foreach (var name in new[] { "globalThis", "Object", "Array", "Map", "JSON" })
        {
            identities[$"global:{name}"] = ObjectRuntime.GetItem(global, name);
        }

        return identities;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static (WeakReference Realm, WeakReference ObjectPrototype, WeakReference ArrayPrototype,
        WeakReference BooleanAdapter, WeakReference GlobalFunction) CreateAndDisposeRealmIntrinsics()
    {
        var services = RuntimeServices.BuildServiceProvider();
        var realm = services.OwningRealm!;
        var context = RuntimeExecutionContext.GetOrCreate(services);

        WeakReference objectPrototype;
        WeakReference arrayPrototype;
        WeakReference booleanAdapter;
        WeakReference globalFunction;
        using (context.EnterAsRoot())
        {
            _ = GlobalThis.globalThis;
            objectPrototype = new WeakReference(GlobalThis.ObjectPrototypeValue);
            arrayPrototype = new WeakReference(JavaScriptRuntime.Array.Prototype);
            booleanAdapter = new WeakReference(
                BuiltinDelegateFunctionAdapter.FromDelegate(GlobalThis.Boolean));
            globalFunction = new WeakReference(GlobalThis.GetFunctionValue("parseInt"));
        }

        var realmReference = new WeakReference(realm);
        realm.Agent.Cluster.Dispose();

        return (realmReference, objectPrototype, arrayPrototype, booleanAdapter, globalFunction);
    }

    private static T WithRealm<T>(Func<T> body)
        => WithRealm(RuntimeServices.BuildServiceProvider(), body);

    private static T WithRealm<T>(ServiceContainer services, Func<T> body)
    {
        var context = RuntimeExecutionContext.GetOrCreate(services);
        using var scope = context.EnterAsRoot();
        return body();
    }
}
