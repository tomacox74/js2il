using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class NetModuleContractTests
{
    [Fact]
    public void INetModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(INetModule), "net", 10, 27, 4);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(INetModule));
    }

    [Fact]
    public void INetModule_MapsNetworkCallForms()
    {
        Assert.Equal(8, NodeContractTestHelpers.GetMethods(typeof(INetModule), "connect").Length);
        Assert.Equal(4, NodeContractTestHelpers.GetMethods(typeof(INetModule), "createServer").Length);
        Assert.Equal(
            typeof(double),
            NodeContractTestHelpers.GetMethods(typeof(INetModule), "isIP").Single().ReturnType);
    }

    [Fact]
    public void IntrinsicNet_DelegatesAndThrowsExplicitly()
    {
        INetModule module = new Net();

        Assert.IsType<NetServer>(module.createServer());
        Assert.NotNull(module.Server);
        var exception = Assert.Throws<NotImplementedException>(() => module.isIP("127.0.0.1"));
        Assert.Equal(
            "The intrinsic node:net module does not implement 'net.isIP'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Net));
    }
}
