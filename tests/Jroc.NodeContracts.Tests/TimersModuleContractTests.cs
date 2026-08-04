using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class TimersModuleContractTests
{
    [Fact]
    public void ITimersModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(ITimersModule);

        Assert.Equal(
            "timers",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void ITimersModule_MapsCompleteFunctionRoster()
    {
        var contractType = typeof(ITimersModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Empty(contractType.GetProperties());
        Assert.Equal(
            6,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
    }

    [Fact]
    public void ITimersModule_MapsCallbacksOptionalDelayAndRestArguments()
    {
        Assert.Contains(GetMethods("setTimeout"), method => method.GetParameters().Length == 1);
        var timeoutWithArgs = Assert.Single(
            GetMethods("setTimeout"),
            method => method.GetParameters().Length == 3);
        Assert.Equal(typeof(Delegate), timeoutWithArgs.GetParameters()[0].ParameterType);
        Assert.NotNull(timeoutWithArgs.GetParameters()[2].GetCustomAttribute<ParamArrayAttribute>());
        Assert.All(GetMethods("clearTimeout"), method => Assert.Equal(typeof(void), method.ReturnType));
    }

    [Fact]
    public void IntrinsicTimersModule_UsesStaticAdapters()
    {
        var module = new TimersModule();

        Assert.NotNull(module.setTimeout);
        Assert.NotNull(module.setImmediate);
        Assert.NotNull(module.setInterval);
        Assert.IsAssignableFrom<ITimersModule>(module);
        Assert.Null(
            typeof(TimersModule).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(ITimersModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
        => method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
}
