using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class StringDecoderModuleContractTests
{
    [Fact]
    public void IStringDecoderModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IStringDecoderModule);

        Assert.Equal(
            "string_decoder",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IStringDecoderModule_MapsConstructorOnlySurface()
    {
        var contractType = typeof(IStringDecoderModule);
        var constructor = Assert.Single(contractType.GetProperties());

        Assert.DoesNotContain(contractType.GetMethods(), method => !method.IsSpecialName);
        Assert.Equal("StringDecoder", constructor.Name);
        Assert.Equal(
            "StringDecoder",
            constructor.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName);
    }

    [Fact]
    public void IntrinsicStringDecoderModule_DelegatesConstructor()
    {
        IStringDecoderModule module = new StringDecoderModule();

        Assert.Equal(typeof(StringDecoder), module.StringDecoder);
        Assert.Null(
            typeof(StringDecoderModule).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }
}
