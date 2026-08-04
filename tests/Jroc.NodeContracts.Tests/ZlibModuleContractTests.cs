using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;
using NodeBuffer = JavaScriptRuntime.Node.Buffer;

namespace Jroc.NodeContracts.Tests;

public class ZlibModuleContractTests
{
    [Fact]
    public void IZlibModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IZlibModule);

        Assert.Equal(
            "zlib",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IZlibModule_MapsCompleteTopLevelRoster()
    {
        var contractType = typeof(IZlibModule);

        AssertGeneratedMemberMap(contractType);
        Assert.Equal(
            34,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(52, contractType.GetProperties().Length);
        Assert.NotNull(contractType.GetProperty("Z_NO_FLUSH")?.GetCustomAttribute<ObsoleteAttribute>());
    }

    [Fact]
    public void IZlibModule_MapsDocumentedReturnAndOptionalTypes()
    {
        Assert.All(GetMethods("crc32"), method => Assert.Equal(typeof(double), method.ReturnType));
        Assert.All(GetMethods("gzipSync"), method => Assert.Equal(typeof(NodeBuffer), method.ReturnType));
        Assert.All(GetMethods("gzip"), method => Assert.Equal(typeof(void), method.ReturnType));
        Assert.Contains(GetMethods("createGzip"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("createGzip"), method => method.GetParameters().Length == 1);
        AssertNoConcreteCollectionAbi(typeof(IZlibModule));
    }

    [Fact]
    public void IntrinsicZlibModule_DelegatesAvailableMembers()
    {
        IZlibModule module = new Zlib();

        Assert.IsType<NodeBuffer>(module.gzipSync(new NodeBuffer("contract")));
        Assert.IsAssignableFrom<Transform>(module.createGzip());
        Assert.Null(
            typeof(Zlib).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicZlibModule_ThrowsForUnavailableMembers()
    {
        IZlibModule module = new Zlib();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.deflateSync(new NodeBuffer("contract")));
        var propertyException = Assert.Throws<NotImplementedException>(
            () => _ = module.constants);

        Assert.Equal(
            "The intrinsic node:zlib module does not implement 'zlib.deflateSync'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:zlib module does not implement 'zlib.constants'.",
            propertyException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IZlibModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
        => method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;

    private static void AssertGeneratedMemberMap(Type contractType)
    {
        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
    }

    private static void AssertNoConcreteCollectionAbi(Type contractType)
    {
        Assert.All(
            contractType.GetMethods(),
            method =>
            {
                Assert.NotEqual(typeof(JavaScriptRuntime.Array), method.ReturnType);
                Assert.NotEqual(typeof(Promise), method.ReturnType);
                Assert.DoesNotContain(
                    method.GetParameters(),
                    parameter => parameter.ParameterType == typeof(JavaScriptRuntime.Array)
                        || parameter.ParameterType == typeof(Promise));
            });
    }
}
