using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class PerfHooksModuleContractTests
{
    [Fact]
    public void IPerfHooksModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IPerfHooksModule);

        Assert.Equal(
            "perf_hooks",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IPerfHooksModule_MapsOptionalFunctionAndCorrectedReturnTypes()
    {
        Assert.Contains(
            GetMethods("createHistogram"),
            method => method.GetParameters().Length == 0);
        Assert.Contains(
            GetMethods("eventLoopUtilization"),
            method => method.GetParameters().Length == 2);

        var timerify = Assert.Single(
            GetMethods("timerify"),
            method => method.GetParameters().Length == 1);
        Assert.Equal(typeof(Delegate), timerify.ReturnType);
        Assert.Equal(typeof(Delegate), timerify.GetParameters()[0].ParameterType);

        Assert.All(
            typeof(IPerfHooksModule).GetMethods(),
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

    [Fact]
    public void IPerfHooksModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IPerfHooksModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            4,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(9, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicPerfHooksModule_DelegatesAvailableMembers()
    {
        IPerfHooksModule module = new PerfHooks();

        var performance = Assert.IsType<PerfHooks.Performance>(module.performance);

        Assert.IsType<double>(performance.now());
        Assert.Null(
            typeof(PerfHooks).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicPerfHooksModule_ThrowsForUnavailableMembers()
    {
        IPerfHooksModule module = new PerfHooks();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.createHistogram());
        var propertyException = Assert.Throws<NotImplementedException>(() => _ = module.constants);

        Assert.Equal(
            "The intrinsic node:perf_hooks module does not implement 'perf_hooks.createHistogram'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:perf_hooks module does not implement 'perf_hooks.constants'.",
            propertyException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IPerfHooksModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
