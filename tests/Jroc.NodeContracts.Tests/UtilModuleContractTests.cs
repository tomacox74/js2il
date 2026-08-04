using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class UtilModuleContractTests
{
    [Fact]
    public void IUtilModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IUtilModule);

        Assert.Equal(
            "util",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IUtilModule_MapsOptionalRestAndPromiseTypes()
    {
        var format = Assert.Single(GetMethods("format"));
        Assert.Equal(typeof(string), format.ReturnType);
        Assert.NotNull(
            format.GetParameters()[1].GetCustomAttribute<ParamArrayAttribute>());
        Assert.Contains(GetMethods("inspect"), method => method.GetParameters().Length == 1);
        Assert.Contains(GetMethods("inspect"), method => method.GetParameters().Length == 4);
        Assert.Equal(
            typeof(IJavaScriptPromise),
            Assert.Single(GetMethods("aborted")).ReturnType);
        Assert.Equal(
            typeof(IJavaScriptArray),
            Assert.Single(GetMethods("diff")).ReturnType);
    }

    [Fact]
    public void IUtilModule_MapsCompleteTopLevelRoster()
    {
        var contractType = typeof(IUtilModule);

        Assert.All(
            contractType.GetMethods().Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.Equal(
            27,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(5, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicUtilModule_DelegatesAvailableMembers()
    {
        IUtilModule module = new Util();

        Assert.IsAssignableFrom<IUtilTypesModule>(module.types);
        Assert.Equal("value: 3", module.format("value: %d", 3d));
        Assert.Equal("{ value: 1 }", module.inspect(new JsObject { ["value"] = 1d }));
        Assert.Null(
            typeof(Util).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicUtilModule_ThrowsForUnavailableMembers()
    {
        IUtilModule module = new Util();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.callbackify(new Action(() => { })));
        var propertyException = Assert.Throws<NotImplementedException>(
            () => _ = module.MIMEType);

        Assert.Equal(
            "The intrinsic node:util module does not implement 'util.callbackify'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:util module does not implement 'util.MIMEType'.",
            propertyException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IUtilModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
