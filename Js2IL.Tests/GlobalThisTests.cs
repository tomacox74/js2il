using JavaScriptRuntime;

namespace Js2IL.Tests;

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
}
