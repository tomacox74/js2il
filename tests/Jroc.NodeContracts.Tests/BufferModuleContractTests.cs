using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;
using NodeBuffer = JavaScriptRuntime.Node.Buffer;

namespace Jroc.NodeContracts.Tests;

public class BufferModuleContractTests
{
    [Fact]
    public void IBufferModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IBufferModule);

        Assert.Equal(
            "buffer",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IBufferModule_MapsDocumentedTypesAndAccess()
    {
        Assert.Equal(typeof(string), Assert.Single(GetMethods("atob")).ReturnType);
        Assert.Equal(typeof(string), Assert.Single(GetMethods("btoa")).ReturnType);
        Assert.Equal(typeof(bool), Assert.Single(GetMethods("isUtf8")).ReturnType);
        Assert.Equal(typeof(NodeBuffer), Assert.Single(GetMethods("transcode")).ReturnType);
        Assert.True(typeof(IBufferModule).GetProperty("INSPECT_MAX_BYTES")?.CanWrite);
        Assert.False(typeof(IBufferModule).GetProperty("kMaxLength")?.CanWrite);

        AssertNoConcreteCollectionAbi(typeof(IBufferModule));
    }

    [Fact]
    public void IBufferModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        AssertGeneratedMemberMap(typeof(IBufferModule));

        Assert.Equal(
            6,
            typeof(IBufferModule).GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(8, typeof(IBufferModule).GetProperties().Length);
    }

    [Fact]
    public void IntrinsicBufferModule_DelegatesAvailableMembers()
    {
        IBufferModule module = new BufferModule();

        Assert.Equal(typeof(NodeBuffer), module.Buffer);
        Assert.True(module.isUtf8(new NodeBuffer("hello")));
        Assert.False(module.isUtf8(new NodeBuffer(new byte[] { 0xff })));
        Assert.Null(module.resolveObjectURL("blob:nodedata:missing"));
        Assert.Null(
            typeof(BufferModule).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicBufferModule_ThrowsForUnavailableMembers()
    {
        IBufferModule module = new BufferModule();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.isAscii(new NodeBuffer("ascii")));
        var propertyException = Assert.Throws<NotImplementedException>(() => _ = module.Blob);
        var setterException = Assert.Throws<NotImplementedException>(
            () => module.INSPECT_MAX_BYTES = 80d);

        Assert.Equal(
            "The intrinsic node:buffer module does not implement 'buffer.isAscii'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:buffer module does not implement 'buffer.Blob'.",
            propertyException.Message);
        Assert.Equal(
            "The intrinsic node:buffer module does not implement 'buffer.INSPECT_MAX_BYTES'.",
            setterException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IBufferModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }

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
