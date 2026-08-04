using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class TimersPromisesModuleContractTests
{
    [Fact]
    public void ITimersPromisesModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(ITimersPromisesModule);

        Assert.Equal(
            "timers/promises",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void ITimersPromisesModule_MapsCompleteTopLevelSurface()
    {
        var contractType = typeof(ITimersPromisesModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Equal(
            3,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal("scheduler", Assert.Single(contractType.GetProperties()).Name);
        Assert.DoesNotContain(
            contractType.GetMethods(),
            method => GetNodeMemberName(method)?.Contains('.', StringComparison.Ordinal) == true);
    }

    [Fact]
    public void ITimersPromisesModule_MapsPromiseAndAsyncIteratorTypes()
    {
        Assert.All(
            GetMethods("setTimeout"),
            method => Assert.Equal(typeof(IJavaScriptPromise), method.ReturnType));
        Assert.All(
            GetMethods("setImmediate"),
            method => Assert.Equal(typeof(IJavaScriptPromise), method.ReturnType));
        Assert.All(
            GetMethods("setInterval"),
            method => Assert.Equal(typeof(IJavaScriptAsyncIterator), method.ReturnType));
        Assert.Contains(GetMethods("setInterval"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("setInterval"), method => method.GetParameters().Length == 3);
    }

    [Fact]
    public void IntrinsicTimersPromisesModule_DelegatesAvailableMember()
    {
        var module = (ITimersPromisesModule)RuntimeHelpers.GetUninitializedObject(
            typeof(TimersPromises));

        Assert.IsAssignableFrom<IJavaScriptAsyncIterator>(module.setInterval());
        Assert.Null(
            typeof(TimersPromises).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicTimersPromisesModule_ThrowsForUnavailableScheduler()
    {
        var module = (ITimersPromisesModule)RuntimeHelpers.GetUninitializedObject(
            typeof(TimersPromises));

        var exception = Assert.Throws<NotImplementedException>(() => _ = module.scheduler);

        Assert.Equal(
            "The intrinsic node:timers/promises module does not implement 'timersPromises.scheduler'.",
            exception.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(ITimersPromisesModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
        => method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
}
