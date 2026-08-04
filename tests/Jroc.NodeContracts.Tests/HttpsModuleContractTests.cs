using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class HttpsModuleContractTests
{
    [Fact]
    public void IHttpsModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(IHttpsModule), "https", 3, 11, 3);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IHttpsModule));
    }

    [Fact]
    public void IHttpsModule_MapsRequestOverloadsAndExports()
    {
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IHttpsModule), "get").Length);
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(IHttpsModule), "request").Length);
        Assert.NotNull(typeof(IHttpsModule).GetProperty("Agent"));
        Assert.NotNull(typeof(IHttpsModule).GetProperty("Server"));
        Assert.True(typeof(IHttpsModule).GetProperty("globalAgent")?.CanWrite);
    }

    [Fact]
    public void IntrinsicHttps_DelegatesAndThrowsExplicitly()
    {
        IHttpsModule module = new Https();

        Assert.NotNull(module.Server);
        var exception = Assert.Throws<NotImplementedException>(() => _ = module.globalAgent);
        Assert.Equal(
            "The intrinsic node:https module does not implement 'https.globalAgent'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Https));
    }
}
