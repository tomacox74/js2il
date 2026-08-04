using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class TlsModuleContractTests
{
    [Fact]
    public void ITlsModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(typeof(ITlsModule), "tls", 7, 25, 7);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(ITlsModule));
    }

    [Fact]
    public void ITlsModule_MapsConnectionAndCertificateTypes()
    {
        Assert.Equal(14, NodeContractTestHelpers.GetMethods(typeof(ITlsModule), "connect").Length);
        Assert.All(
            NodeContractTestHelpers.GetMethods(typeof(ITlsModule), "getCiphers"),
            method => Assert.Equal(typeof(IJavaScriptArray), method.ReturnType));
        Assert.Equal(
            typeof(void),
            NodeContractTestHelpers.GetMethods(
                typeof(ITlsModule),
                "setDefaultCACertificates").Single().ReturnType);
    }

    [Fact]
    public void IntrinsicTls_DelegatesAndThrowsExplicitly()
    {
        ITlsModule module = new Tls();

        Assert.IsType<TlsSecureContext>(module.createSecureContext());
        Assert.NotNull(module.TLSSocket);
        var exception = Assert.Throws<NotImplementedException>(() => module.getCiphers());
        Assert.Equal(
            "The intrinsic node:tls module does not implement 'tls.getCiphers'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Tls));
    }
}
