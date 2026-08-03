using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class ConsoleModuleContractTests
{
    [Fact]
    public void IConsoleModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IConsoleModule);

        Assert.Equal(
            "console",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
        Assert.Equal(
            "Console",
            contractType.GetProperty("Console")
                ?.GetCustomAttribute<NodeModuleMemberAttribute>()
                ?.MemberName);
    }

    [Fact]
    public void IConsoleModule_MapsOptionalAndRestParameters()
    {
        var logMethods = GetMethods("log");

        Assert.Contains(logMethods, method => method.GetParameters().Length == 0);
        var variadicLog = Assert.Single(
            logMethods,
            method => method.GetParameters().Length == 2);
        Assert.NotNull(
            variadicLog.GetParameters()[1].GetCustomAttribute<ParamArrayAttribute>());

        var assert = Assert.Single(GetMethods("assert"));
        Assert.False(assert.GetParameters()[0].HasDefaultValue);
        Assert.NotNull(assert.GetParameters()[1].GetCustomAttribute<ParamArrayAttribute>());
    }

    [Fact]
    public void IConsoleModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IConsoleModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            22,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Single(contractType.GetProperties());
    }

    [Fact]
    public void IntrinsicConsoleModule_DelegatesAvailableMembers()
    {
        var output = new RecordingConsoleOutput();
        var errorOutput = new RecordingConsoleOutput();
        IConsoleModule module = new ConsoleModule(new ConsoleOutputSinks
        {
            Output = output,
            ErrorOutput = errorOutput
        });

        module.log("hello", "world");
        module.error("failure");
        module.log();

        Assert.Equal(["hello world", string.Empty], output.Lines);
        Assert.Equal(["failure"], errorOutput.Lines);
        Assert.Equal(typeof(global::JavaScriptRuntime.Console), module.Console);
        Assert.Null(
            typeof(ConsoleModule).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicConsoleModule_ThrowsForUnavailableMembers()
    {
        IConsoleModule module = new ConsoleModule();

        var exception = Assert.Throws<NotImplementedException>(() => module.count());

        Assert.Equal(
            "The intrinsic node:console module does not implement 'console.count'.",
            exception.Message);

        Assert.Throws<NotImplementedException>(
            () => module.table(new object(), new JavaScriptRuntime.Array()));
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IConsoleModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }

    private sealed class RecordingConsoleOutput : IConsoleOutput
    {
        public List<string> Lines { get; } = [];

        public void Write(string text)
        {
            if (Lines.Count == 0)
            {
                Lines.Add(text);
            }
            else
            {
                Lines[^1] += text;
            }
        }

        public void WriteLine(string line)
        {
            Lines.Add(line);
        }
    }
}
