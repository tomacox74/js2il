using System.Reflection;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class DnsModuleContractTests
{
    [Fact]
    public void IDnsModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(IDnsModule), "dns", 21, 25, 29);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IDnsModule));

        var resolverType = typeof(IResolver);
        Assert.Equal(
            "Resolver",
            resolverType.GetCustomAttribute<NodeModuleTypeAttribute>()?.TypeName);
        Assert.Equal(24, resolverType.GetMethods().Length);
    }

    [Fact]
    public void IntrinsicDns_DelegatesAndThrowsExplicitly()
    {
        IDnsModule module = new Dns();
        var originalOrder = module.getDefaultResultOrder();
        try
        {
            module.setDefaultResultOrder("ipv6first");
            Assert.Equal("ipv6first", module.getDefaultResultOrder());
        }
        finally
        {
            module.setDefaultResultOrder(originalOrder);
        }

        var exception = Assert.Throws<NotImplementedException>(
            () => module.resolveAny("localhost", (Action)(() => { })));
        Assert.Equal(
            "The intrinsic node:dns module does not implement 'dns.resolveAny'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Dns));
    }
}
