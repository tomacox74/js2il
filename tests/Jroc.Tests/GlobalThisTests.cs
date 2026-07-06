using JavaScriptRuntime;

namespace Jroc.Tests;

public class GlobalThisTests
{
    [Fact]
    public void GlobalObject_DoesNotExposeGcByDefault()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.False(globalObject.ContainsKey(nameof(GlobalThis.gc)));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_ExposesGcWhenEnabled()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();
        serviceProvider.Replace(new GlobalThisOptions
        {
            ExposeGc = true
        });

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.True(globalObject.ContainsKey(nameof(GlobalThis.gc)));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_ExposesKeyedCollectionConstructorsWithPrototypeBackReferences()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;

            Assert.All(
                new (string Name, object Constructor)[]
                {
                    (nameof(GlobalThis.Map), GlobalThis.Map),
                    (nameof(GlobalThis.Set), GlobalThis.Set),
                    (nameof(GlobalThis.WeakMap), GlobalThis.WeakMap),
                    (nameof(GlobalThis.WeakSet), GlobalThis.WeakSet)
                },
                pair =>
                {
                    Assert.True(globalObject.ContainsKey(pair.Name));
                    Assert.Same(pair.Constructor, globalObject[pair.Name]);

                    var prototype = ObjectRuntime.GetItem(pair.Constructor, "prototype");
                    Assert.NotNull(prototype);
                    Assert.Same(pair.Constructor, ObjectRuntime.GetItem(prototype!, "constructor"));

                    var descriptor = JavaScriptRuntime.Object.getOwnPropertyDescriptor(pair.Constructor, "prototype");
                    Assert.NotNull(descriptor);
                    Assert.False((bool)ObjectRuntime.GetItem(descriptor!, "writable")!);
                    Assert.False((bool)ObjectRuntime.GetItem(descriptor, "enumerable")!);
                    Assert.False((bool)ObjectRuntime.GetItem(descriptor, "configurable")!);

                    var error = Assert.Throws<TypeError>(() => Closure.InvokeWithArgs(pair.Constructor, RuntimeServices.EmptyScopes));
                    Assert.Equal($"Constructor {pair.Name} requires 'new'", error.Message);
                });
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_AppliesHostGlobalBindingsWithConfiguredPropertyAttributes()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();
        serviceProvider.Replace(new GlobalThisOptions
        {
            HostRuntimeIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
                .AddGlobalValue(
                    "assert",
                    "native assert",
                    new RuntimeGlobalPropertyAttributes(
                        Enumerable: true,
                        Configurable: false,
                        Writable: false))
                .Build()
        });

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.Equal("native assert", globalObject["assert"]);

            Assert.True(PropertyDescriptorStore.TryGetOwn(globalObject, "assert", out var descriptor));
            Assert.Equal("native assert", descriptor.Value);
            Assert.True(descriptor.Enumerable);
            Assert.False(descriptor.Configurable);
            Assert.False(descriptor.Writable);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_HostGlobalBindingsDoNotReplaceBuiltInsByDefault()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();
        serviceProvider.Replace(new GlobalThisOptions
        {
            HostRuntimeIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
                .AddGlobalValue(nameof(GlobalThis.Array), "host array")
                .Build()
        });

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.Same(GlobalThis.Array, globalObject[nameof(GlobalThis.Array)]);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_HostGlobalBindingsCanExplicitlyReplaceBuiltIns()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();
        serviceProvider.Replace(new GlobalThisOptions
        {
            HostRuntimeIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
                .AddGlobalValue(
                    nameof(GlobalThis.Array),
                    "host array",
                    overwritePolicy: RuntimeGlobalOverwritePolicy.ReplaceExisting)
                .Build()
        });

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.Equal("host array", globalObject[nameof(GlobalThis.Array)]);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_HostGlobalFactoriesCreateValuesPerRuntime()
    {
        var factoryCalls = 0;
        var hostIntrinsics = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalFactory("assert", () => ++factoryCalls)
            .Build();

        var firstServiceProvider = RuntimeServices.BuildServiceProvider();
        firstServiceProvider.Replace(new GlobalThisOptions
        {
            HostRuntimeIntrinsics = hostIntrinsics
        });

        var secondServiceProvider = RuntimeServices.BuildServiceProvider();
        secondServiceProvider.Replace(new GlobalThisOptions
        {
            HostRuntimeIntrinsics = hostIntrinsics
        });

        try
        {
            GlobalThis.ServiceProvider = firstServiceProvider;
            var firstGlobalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.Equal(1, firstGlobalObject["assert"]);

            GlobalThis.ServiceProvider = secondServiceProvider;
            var secondGlobalObject = (GlobalThis)GlobalThis.globalThis;
            Assert.Equal(2, secondGlobalObject["assert"]);
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }

    [Fact]
    public void GlobalObject_ExposesTypeErrorConstructor_WithPrototypeBackReferences()
    {
        var serviceProvider = RuntimeServices.BuildServiceProvider();

        try
        {
            GlobalThis.ServiceProvider = serviceProvider;

            var globalObject = (GlobalThis)GlobalThis.globalThis;

            Assert.True(globalObject.ContainsKey(nameof(GlobalThis.TypeError)));
            Assert.Same(GlobalThis.TypeError, globalObject[nameof(GlobalThis.TypeError)]);

            var prototype = ObjectRuntime.GetItem(GlobalThis.TypeError, "prototype");
            Assert.NotNull(prototype);
            Assert.Same(GlobalThis.TypeError, ObjectRuntime.GetItem(prototype!, "constructor"));
            Assert.Equal("TypeError", ObjectRuntime.GetItem(prototype, "name"));
            Assert.Same(ObjectRuntime.GetItem(GlobalThis.Error, "prototype"), PrototypeChain.GetPrototypeOrNull(prototype));

            var error = new TypeError("boom");
            Assert.True(Operators.InstanceOf(error, GlobalThis.TypeError));
            Assert.True(Operators.InstanceOf(error, GlobalThis.Error));
            Assert.Same(GlobalThis.TypeError, ObjectRuntime.GetItem(error, "constructor"));
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }
}
