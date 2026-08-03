using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class ProcessModuleContractTests
{
    [Fact]
    public void IProcessModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IProcessModule);

        Assert.Equal(
            "process",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IProcessModule_MapsOptionalRestArrayAndBigIntTypes()
    {
        Assert.Contains(GetMethods("exit"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("exit"), method => method.GetParameters().Length == 1);
        Assert.Contains(GetMethods("loadEnvFile"), method => method.GetParameters().Length == 0);
        Assert.Contains(GetMethods("loadEnvFile"), method => method.GetParameters().Length == 1);

        var nextTick = Assert.Single(GetMethods("nextTick"));
        Assert.Equal(typeof(Delegate), nextTick.GetParameters()[0].ParameterType);
        Assert.NotNull(nextTick.GetParameters()[1].GetCustomAttribute<ParamArrayAttribute>());

        Assert.Equal(
            typeof(IJavaScriptArray),
            Assert.Single(GetMethods("getActiveResourcesInfo")).ReturnType);
        Assert.Equal(
            typeof(IJavaScriptArray),
            Assert.Single(GetMethods("hrtime"), method => method.GetParameters().Length == 0).ReturnType);
        Assert.Equal(typeof(double), Assert.Single(GetMethods("getegid")).ReturnType);
        Assert.Equal(typeof(double), Assert.Single(GetMethods("geteuid")).ReturnType);
        Assert.Equal(typeof(double), Assert.Single(GetMethods("getgid")).ReturnType);
        Assert.All(GetMethods("kill"), method => Assert.Equal(typeof(bool), method.ReturnType));
        Assert.All(GetMethods("umask"), method => Assert.Equal(typeof(double), method.ReturnType));

        Assert.All(
            typeof(IProcessModule).GetMethods(),
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
            typeof(IProcessModule).GetProperties(),
            property =>
            {
                Assert.NotEqual(typeof(JavaScriptRuntime.Array), property.PropertyType);
                Assert.NotEqual(typeof(Promise), property.PropertyType);
            });

        Assert.True(typeof(IProcessModule).GetProperty("exitCode")?.CanWrite);
        Assert.True(typeof(IProcessModule).GetProperty("debugPort")?.CanWrite);
        Assert.False(typeof(IProcessModule).GetProperty("platform")?.CanWrite);
    }

    [Fact]
    public void IProcessModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IProcessModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            39,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(32, contractType.GetProperties().Length);
        Assert.NotNull(contractType.GetProperty("features"));
        Assert.NotNull(contractType.GetProperty("finalization"));
        Assert.Null(contractType.GetProperty("cached_builtins"));
        Assert.Empty(GetMethods("finalization.register"));
        Assert.Empty(GetMethods("hrtime.bigint"));
        Assert.Empty(GetMethods("memoryUsage.rss"));
    }

    [Fact]
    public void IntrinsicProcessModule_DelegatesAvailableMembers()
    {
        var environment = new CapturingEnvironment();
        IProcessModule module = new Process(environment);

        Assert.Null(module.exitCode);
        module.exitCode = 7d;
        Assert.Equal(7, environment.ExitCode);
        Assert.Equal(7d, module.exitCode);
        module.exitCode = "8";
        Assert.Equal(8, environment.ExitCode);
        Assert.Equal(8d, module.exitCode);
        module.exitCode = JsNull.Null;
        Assert.Equal(0, environment.ExitCode);
        Assert.Null(module.exitCode);
        Assert.False(string.IsNullOrWhiteSpace(module.cwd()));
        Assert.NotEmpty(module.platform);
        Assert.IsAssignableFrom<IJavaScriptArray>(module.argv);
        Assert.NotNull(module.env);

        module.exit(4d);
        Assert.True(environment.ExitCalled);
        Assert.Equal(4, environment.ExitCalledWithCode);
        Assert.Equal(4d, module.exitCode);
        Assert.Null(
            typeof(Process).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicProcessModule_ThrowsForUnavailableMembers()
    {
        IProcessModule module = new Process(new NonTerminatingEnvironment());

        var methodException = Assert.Throws<NotImplementedException>(
            () => module.availableMemory());
        var propertyException = Assert.Throws<NotImplementedException>(() => _ = module.debugPort);
        var setterException = Assert.Throws<NotImplementedException>(() => module.debugPort = 9229d);
        var argvSetterException = Assert.Throws<NotImplementedException>(
            () => module.argv = new JavaScriptRuntime.Array());
        var envSetterException = Assert.Throws<NotImplementedException>(
            () => module.env = new object());
        Assert.Throws<TypeError>(() => module.exitCode = true);
        Assert.Throws<TypeError>(() => module.exitCode = "not-an-integer");
        Assert.Throws<RangeError>(() => module.exitCode = 1.5d);
        Assert.Throws<RangeError>(() => module.exitCode = 9007199254740992d);
        Assert.Throws<TypeError>(() => module.exit(true));
        Assert.Throws<RangeError>(() => module.exit(1.5d));

        Assert.Equal(
            "The intrinsic node:process module does not implement 'process.availableMemory'.",
            methodException.Message);
        Assert.Equal(
            "The intrinsic node:process module does not implement 'process.debugPort'.",
            propertyException.Message);
        Assert.Equal(propertyException.Message, setterException.Message);
        Assert.Equal(
            "The intrinsic node:process module does not implement 'process.argv'.",
            argvSetterException.Message);
        Assert.Equal(
            "The intrinsic node:process module does not implement 'process.env'.",
            envSetterException.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IProcessModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
