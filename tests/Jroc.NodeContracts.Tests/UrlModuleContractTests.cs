using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class UrlModuleContractTests
{
    [Fact]
    public void IUrlModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(IUrlModule), "url", 9, 15, 3);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(IUrlModule));
    }

    [Fact]
    public void IUrlModule_MapsLegacyAndWhatwgOverloads()
    {
        Assert.Equal(2, NodeContractTestHelpers.GetMethods(typeof(IUrlModule), "fileURLToPath").Length);
        Assert.Equal(3, NodeContractTestHelpers.GetMethods(typeof(IUrlModule), "parse").Length);
        Assert.Equal(typeof(string), NodeContractTestHelpers
            .GetMethods(typeof(IUrlModule), "domainToASCII").Single().ReturnType);
    }

    [Fact]
    public void IntrinsicUrl_DelegatesAndThrowsExplicitly()
    {
        IUrlModule module = new Url();

        Assert.Equal("/tmp/contract", module.fileURLToPath("file:///tmp/contract"));
        Assert.IsType<URL>(module.pathToFileURL("/tmp/contract"));
        var exception = Assert.Throws<NotImplementedException>(
            () => module.domainToASCII("example.com"));
        Assert.Equal(
            "The intrinsic node:url module does not implement 'url.domainToASCII'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Url));
    }
}
