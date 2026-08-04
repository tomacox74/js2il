using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class OsModuleContractTests
{
    [Fact]
    public void IOsModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IOsModule);

        Assert.Equal(
            "os",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IOsModule_MapsOptionalPrefixAndArrayTypes()
    {
        Assert.Contains(GetMethods("getPriority"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("getPriority"), method => method.GetParameters().Length == 1);

        var setPriorityMethods = GetMethods("setPriority");
        Assert.Contains(
            setPriorityMethods,
            method => method.GetParameters().Select(parameter => parameter.Name)
                .SequenceEqual(["priority"]));
        Assert.Contains(
            setPriorityMethods,
            method => method.GetParameters().Select(parameter => parameter.Name)
                .SequenceEqual(["pid", "priority"]));
        Assert.Contains(GetMethods("userInfo"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("userInfo"), method => method.GetParameters().Length == 1);
        Assert.Equal(typeof(IJavaScriptArray), Assert.Single(GetMethods("cpus")).ReturnType);
        Assert.Equal(typeof(IJavaScriptArray), Assert.Single(GetMethods("loadavg")).ReturnType);
    }

    [Fact]
    public void IOsModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IOsModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Equal(
            20,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(3, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicOsModule_DelegatesAvailableMembers()
    {
        IOsModule module = new OS();

        Assert.False(string.IsNullOrWhiteSpace(module.tmpdir()));
        Assert.NotNull(module.homedir());
        Assert.Null(
            typeof(OS).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicOsModule_ThrowsForUnavailableMembers()
    {
        IOsModule module = new OS();

        var methodException = Assert.Throws<NotImplementedException>(() => module.arch());
        var propertyException = Assert.Throws<NotImplementedException>(() => _ = module.EOL);

        Assert.Equal(
            "The intrinsic node:os module does not implement 'os.arch'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:os module does not implement 'os.EOL'.",
            propertyException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IOsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
