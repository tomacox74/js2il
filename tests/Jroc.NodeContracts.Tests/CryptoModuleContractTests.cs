using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;
using NodeBuffer = JavaScriptRuntime.Node.Buffer;

namespace Jroc.NodeContracts.Tests;

public class CryptoModuleContractTests
{
    [Fact]
    public void ICryptoModule_MapsCompletePinnedRoster()
    {
        NodeContractTestHelpers.AssertGeneratedContract(
            typeof(ICryptoModule),
            "crypto",
            55,
            91,
            16);
        NodeContractTestHelpers.AssertNoConcreteCollectionAbi(typeof(ICryptoModule));
    }

    [Fact]
    public void ICryptoModule_PreservesSyncAndCallbackReturnTypes()
    {
        var randomInt = NodeContractTestHelpers.GetMethods(typeof(ICryptoModule), "randomInt");
        Assert.Equal(4, randomInt.Length);
        Assert.Equal(2, randomInt.Count(method => method.ReturnType == typeof(double)));
        Assert.Equal(2, randomInt.Count(method => method.ReturnType == typeof(void)));

        var sign = NodeContractTestHelpers.GetMethods(typeof(ICryptoModule), "sign");
        Assert.Contains(sign, method => method.ReturnType == typeof(NodeBuffer));
        Assert.Contains(sign, method => method.ReturnType == typeof(void));
        Assert.Equal(
            typeof(IJavaScriptArray),
            NodeContractTestHelpers.GetMethods(typeof(ICryptoModule), "getHashes").Single().ReturnType);
    }

    [Fact]
    public void IntrinsicCrypto_DelegatesAndThrowsExplicitly()
    {
        ICryptoModule module = new Crypto();

        Assert.IsType<Hash>(module.createHash("sha256"));
        Assert.Equal(4, Assert.IsType<NodeBuffer>(module.randomBytes(4)).length);
        Assert.NotNull(module.subtle);
        var exception = Assert.Throws<NotImplementedException>(() => module.randomUUID());
        Assert.Equal(
            "The intrinsic node:crypto module does not implement 'crypto.randomUUID'.",
            exception.Message);
        NodeContractTestHelpers.AssertUsesStaticAdapters(typeof(Crypto));
    }
}
