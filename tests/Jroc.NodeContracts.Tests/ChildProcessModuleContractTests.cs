using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class ChildProcessModuleContractTests
{
    [Fact]
    public void IChildProcessModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IChildProcessModule);

        Assert.Equal(
            "child_process",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IChildProcessModule_MapsRequiredOptionalUnionAndArrayCallForms()
    {
        var spawnMethods = GetMethods("spawn");

        Assert.Contains(
            spawnMethods,
            method => method.GetParameters().Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(string)]));
        Assert.Contains(
            spawnMethods,
            method => method.GetParameters().Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(string), typeof(object), typeof(object)]));

        var forkWithModulePathOnly = Assert.Single(
            GetMethods("fork"),
            method => method.GetParameters().Length == 1);
        Assert.Equal(typeof(object), forkWithModulePathOnly.GetParameters()[0].ParameterType);

        Assert.All(
            typeof(IChildProcessModule).GetMethods(),
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
    public void IChildProcessModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IChildProcessModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            7,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Single(contractType.GetProperties());
    }

    [Fact]
    public void IntrinsicChildProcessModule_DelegatesAvailableMembers()
    {
        IChildProcessModule module = new ChildProcess();

        var result = Assert.IsAssignableFrom<IDictionary<string, object?>>(
            module.spawnSync("__jroc_missing_child_process_executable__"));

        Assert.Equal(-1d, result["status"]);
        Assert.Null(
            typeof(ChildProcess).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicChildProcessModule_ThrowsForUnavailableMembers()
    {
        IChildProcessModule module = new ChildProcess();

        var exception = Assert.Throws<NotImplementedException>(() => _ = module.ChildProcess);

        Assert.Equal(
            "The intrinsic node:child_process module does not implement 'child_process.ChildProcess'.",
            exception.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IChildProcessModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
