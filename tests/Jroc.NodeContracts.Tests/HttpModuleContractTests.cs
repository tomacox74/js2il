using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class HttpModuleContractTests
{
    [Fact]
    public void IHttpModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(IHttpModule), "http", 7, 17, 11);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IHttpModule));
    }

    [Fact]
    public void IHttpModule_MapsRequestOverloadsAndExports()
    {
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IHttpModule), "get").Length);
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IHttpModule), "request").Length);
        Assert.Equal(typeof(IJavaScriptArray), typeof(IHttpModule).GetProperty("METHODS")?.PropertyType);
        Assert.True(typeof(IHttpModule).GetProperty("globalAgent")?.CanWrite);
    }

    [Fact]
    public void IntrinsicHttp_DelegatesAndThrowsExplicitly()
    {
        IHttpModule module = new Http();

        Assert.IsType<HttpServer>(module.createServer());
        Assert.NotNull(module.globalAgent);
        var exception = Assert.Throws<NotImplementedException>(() => _ = module.METHODS);
        Assert.Equal(
            "The intrinsic node:http module does not implement 'http.METHODS'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Http));
    }
}
