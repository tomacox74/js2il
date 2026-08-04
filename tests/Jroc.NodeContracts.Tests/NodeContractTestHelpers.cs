using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

internal static class NodeContractTestHelpers
{
    internal static void AssertGeneratedContract(
        Type contractType,
        string moduleName,
        int uniqueMethodCount,
        int methodOverloadCount,
        int propertyCount)
    {
        Assert.Equal(
            moduleName,
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);

        var methods = contractType.GetMethods().Where(method => !method.IsSpecialName).ToArray();
        Assert.Equal(methodOverloadCount, methods.Length);
        Assert.Equal(
            uniqueMethodCount,
            methods.Select(GetNodeMemberName).Distinct(StringComparer.Ordinal).Count());
        Assert.All(
            methods,
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));

        var properties = contractType.GetProperties();
        Assert.Equal(propertyCount, properties.Length);
        Assert.All(
            properties,
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));
    }

    internal static MethodInfo[] GetMethods(Type contractType, string memberName)
    {
        return contractType
            .GetMethods()
            .Where(method => !method.IsSpecialName && GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    internal static void AssertNoConcreteCollectionAbi(Type contractType)
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
        Assert.All(
            contractType.GetProperties(),
            property =>
            {
                Assert.NotEqual(typeof(JavaScriptRuntime.Array), property.PropertyType);
                Assert.NotEqual(typeof(Promise), property.PropertyType);
            });
    }

    internal static void AssertUsesStaticAdapters(Type intrinsicType)
    {
        Assert.Null(
            intrinsicType.GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    private static string? GetNodeMemberName(MemberInfo member)
        => member.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
}
