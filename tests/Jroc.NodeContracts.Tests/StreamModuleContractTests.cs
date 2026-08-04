using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;
using NodeStream = JavaScriptRuntime.Node.Stream;

namespace Jroc.NodeContracts.Tests;

public class StreamModuleContractTests
{
    [Fact]
    public void IStreamModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IStreamModule);

        Assert.Equal(
            "stream",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IStreamModule_MapsCallbacksRestAndArrayTypes()
    {
        Assert.Contains(GetMethods("finished"), method => method.GetParameters().Length == 2);
        Assert.Contains(GetMethods("finished"), method => method.GetParameters().Length == 3);
        Assert.All(
            GetMethods("finished"),
            method => Assert.Equal(typeof(Delegate), method.ReturnType));

        var pipeline = Assert.Single(GetMethods("pipeline"));
        Assert.NotNull(
            Assert.Single(pipeline.GetParameters()).GetCustomAttribute<ParamArrayAttribute>());
        Assert.Equal(
            typeof(IJavaScriptArray),
            Assert.Single(GetMethods("duplexPair").Select(method => method.ReturnType).Distinct()));
        Assert.Equal(typeof(object), Assert.Single(GetMethods("addAbortSignal")).ReturnType);
    }

    [Fact]
    public void IStreamModule_MapsCompleteTopLevelRoster()
    {
        var contractType = typeof(IStreamModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Equal(
            10,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(6, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicStreamModule_DelegatesAvailableMembers()
    {
        IStreamModule module = new NodeStream();
        var writable = new Writable();
        Func<object[], object?[], object?> callback = (_, _) => null;

        Assert.Equal(typeof(Readable), module.Readable);
        Assert.Equal(typeof(Writable), module.Writable);
        var cleanup = module.finished(writable, callback);
        Assert.IsType<Action>(cleanup);
        cleanup.DynamicInvoke();
        Assert.Null(
            typeof(NodeStream).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicStreamModule_ThrowsForUnavailableMembers()
    {
        IStreamModule module = new NodeStream();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.compose());
        var propertyException = Assert.Throws<NotImplementedException>(
            () => _ = module.promises);

        Assert.Equal(
            "The intrinsic node:stream module does not implement 'stream.compose'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:stream module does not implement 'stream.promises'.",
            propertyException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IStreamModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
