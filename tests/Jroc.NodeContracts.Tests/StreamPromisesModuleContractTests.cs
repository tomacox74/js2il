using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class StreamPromisesModuleContractTests
{
    [Fact]
    public void IStreamPromisesModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IStreamPromisesModule);

        Assert.Equal(
            "stream/promises",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IStreamPromisesModule_MapsCompletePromiseSurface()
    {
        Assert.Contains(GetMethods("finished"), method => method.GetParameters().Length == 1);
        Assert.Contains(GetMethods("finished"), method => method.GetParameters().Length == 2);
        Assert.All(
            typeof(IStreamPromisesModule).GetMethods(),
            method => Assert.Equal(typeof(IJavaScriptPromise), method.ReturnType));

        var pipeline = Assert.Single(GetMethods("pipeline"));
        Assert.NotNull(
            Assert.Single(pipeline.GetParameters()).GetCustomAttribute<ParamArrayAttribute>());
        Assert.Equal(
            2,
            typeof(IStreamPromisesModule).GetMethods()
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
    }

    [Fact]
    public void IntrinsicStreamPromisesModule_DelegatesEveryMember()
    {
        IStreamPromisesModule module = new StreamPromises();
        var writable = new Writable();

        var completion = module.finished(writable);
        writable.end("done");

        Assert.IsAssignableFrom<IJavaScriptPromise>(completion);
        Assert.Null(
            typeof(StreamPromises).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IStreamPromisesModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
