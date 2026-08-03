using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class FsModuleContractTests
{
    [Theory]
    [InlineData("fs", "fs")]
    [InlineData(" node:fs ", "fs")]
    [InlineData("NODE:fs", "fs")]
    public void NodeModuleInterfaceAttribute_NormalizesModuleName(string value, string expected)
    {
        var attribute = new NodeModuleInterfaceAttribute(value);

        Assert.Equal(expected, attribute.ModuleName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("node:")]
    public void NodeModuleInterfaceAttribute_RejectsEmptyModuleName(string value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new NodeModuleInterfaceAttribute(value));
    }

    [Fact]
    public void IFsModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IFsModule);

        Assert.Equal(
            "fs",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateFsModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);

        var deprecatedConstant = contractType.GetProperty("F_OK");
        Assert.Equal(typeof(double), deprecatedConstant?.PropertyType);
        Assert.NotNull(deprecatedConstant?.GetCustomAttribute<ObsoleteAttribute>());
        Assert.All(
            contractType.GetMethods().Where(method => GetNodeMemberName(method) == "exists"),
            method => Assert.NotNull(method.GetCustomAttribute<ObsoleteAttribute>()));

        var memberNames = contractType
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Select(member => member.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName)
            .Where(memberName => memberName is not null)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains("constants", memberNames);
        Assert.Contains("promises", memberNames);
        Assert.Contains("F_OK", memberNames);
        Assert.Contains("realpath.native", memberNames);
        Assert.Contains("openSync", memberNames);
    }

    [Fact]
    public void IFsModule_ProvidesRepresentativeCallbackAndSynchronousOverloads()
    {
        var accessParameterTypes = typeof(IFsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == "access")
            .Select(method => method.GetParameters().Select(parameter => parameter.ParameterType).ToArray())
            .ToArray();

        Assert.Contains(
            accessParameterTypes,
            parameters => parameters.SequenceEqual([typeof(object), typeof(Delegate)]));
        Assert.Contains(
            accessParameterTypes,
            parameters => parameters.SequenceEqual([typeof(object), typeof(object), typeof(Delegate)]));

        var closeMethods = typeof(IFsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == "close")
            .ToArray();

        Assert.Contains(
            closeMethods,
            method => method.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(double)]));
        Assert.Contains(
            closeMethods,
            method => method.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(double), typeof(object)]));

        var openSyncMethods = typeof(IFsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == "openSync")
            .ToArray();

        Assert.Equal(3, openSyncMethods.Length);
        Assert.All(openSyncMethods, method => Assert.Equal(typeof(double), method.ReturnType));

        var realpathNativeMethods = typeof(IFsModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == "realpath.native")
            .ToArray();

        Assert.Equal(2, realpathNativeMethods.Length);
        Assert.All(realpathNativeMethods, method => Assert.Equal("realpathNative", method.Name));
    }

    [Fact]
    public void IFsModule_UsesRuntimeContractsInsteadOfImplementations()
    {
        Assert.Contains(
            typeof(IFsModule).GetMethods(),
            method => method.ReturnType == typeof(IJavaScriptArray));
        Assert.DoesNotContain(
            typeof(IFsModule).GetMethods(),
            method => method.ReturnType == typeof(JavaScriptRuntime.Array)
                || method.GetParameters().Any(
                    parameter => parameter.ParameterType == typeof(JavaScriptRuntime.Array)));
        Assert.True(typeof(IJavaScriptArray).IsAssignableFrom(typeof(JavaScriptRuntime.Array)));

        Assert.Contains(
            typeof(IFsModule).GetMethods(),
            method => method.ReturnType == typeof(IJavaScriptPromise));
        Assert.DoesNotContain(
            typeof(IFsModule).GetMethods(),
            method => method.ReturnType == typeof(Promise)
                || method.GetParameters().Any(parameter => parameter.ParameterType == typeof(Promise)));
        Assert.True(typeof(IJavaScriptPromise).IsAssignableFrom(typeof(Promise)));
    }

    [Fact]
    public void IntrinsicFsModule_ImplementsAvailableContractMembers()
    {
        IFsModule module = new FS();

        Assert.Null(
            typeof(FS).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.NotNull(module.constants);
        Assert.True(module.existsSync(Environment.CurrentDirectory));
        Assert.IsAssignableFrom<IJavaScriptArray>(
            module.readdirSync(Environment.CurrentDirectory));
    }

    [Fact]
    public void IntrinsicFsModule_ThrowsForUnavailableContractMembers()
    {
        IFsModule module = new FS();

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.chmodSync("file.txt", 384d));

        Assert.Equal(
            "The intrinsic node:fs module does not implement 'fs.chmodSync'.",
            methodException.Message);

        var propertyException = Assert.Throws<TargetInvocationException>(
            () => typeof(IFsModule).GetProperty("F_OK")!.GetValue(module));

        Assert.IsType<NotImplementedException>(propertyException.InnerException);
    }

    [Fact]
    public void IFsModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IFsModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            95,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(6, contractType.GetProperties().Length);
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
