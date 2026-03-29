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
                });
        }
        finally
        {
            GlobalThis.ServiceProvider = null;
        }
    }
}
