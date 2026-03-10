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
}
