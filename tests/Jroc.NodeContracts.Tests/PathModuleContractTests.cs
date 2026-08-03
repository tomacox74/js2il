using System.CodeDom.Compiler;
using System.Reflection;
using JavaScriptRuntime.Node;
using Jroc.Runtime.Node.Contracts;

namespace Jroc.NodeContracts.Tests;

public class PathModuleContractTests
{
    [Fact]
    public void IPathModule_IdentifiesPinnedGeneratedContract()
    {
        var contractType = typeof(IPathModule);

        Assert.Equal(
            "path",
            contractType.GetCustomAttribute<NodeModuleInterfaceAttribute>()?.ModuleName);
        var generatedCode = contractType.GetCustomAttribute<GeneratedCodeAttribute>();
        Assert.NotNull(generatedCode);
        Assert.Equal("generateNodeModuleInterface.js", generatedCode.Tool);
        Assert.Matches("^sha256:[0-9a-f]{64}$", generatedCode.Version);
    }

    [Fact]
    public void IPathModule_MapsOptionalAndRestCallForms()
    {
        var basenameMethods = GetMethods("basename");

        Assert.Contains(
            basenameMethods,
            method => method.GetParameters().Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(string)]));
        Assert.Contains(
            basenameMethods,
            method => method.GetParameters().Select(parameter => parameter.ParameterType)
                .SequenceEqual([typeof(string), typeof(object)]));

        var join = Assert.Single(GetMethods("join"));
        Assert.NotNull(join.GetParameters()[0].GetCustomAttribute<ParamArrayAttribute>());
    }

    [Fact]
    public void IPathModule_MapsEveryGeneratedMemberToItsJavaScriptName()
    {
        var contractType = typeof(IPathModule);

        Assert.All(
            contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => !method.IsSpecialName),
            method => Assert.NotNull(method.GetCustomAttribute<NodeModuleMemberAttribute>()));
        Assert.All(
            contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            property => Assert.NotNull(property.GetCustomAttribute<NodeModuleMemberAttribute>()));

        Assert.Equal(
            12,
            contractType.GetMethods()
                .Where(method => !method.IsSpecialName)
                .Select(GetNodeMemberName)
                .Distinct(StringComparer.Ordinal)
                .Count());
        Assert.Equal(4, contractType.GetProperties().Length);
    }

    [Fact]
    public void IntrinsicPathModule_DelegatesAvailableMembers()
    {
        IPathModule module = new JavaScriptRuntime.Node.Path();

        Assert.Equal("example", module.basename("/tmp/example.txt", ".txt"));
        Assert.Equal("example.txt", module.basename("/tmp/example.txt/"));
        Assert.Equal(".", module.dirname("example.txt"));
        Assert.EndsWith(module.sep, module.normalize($"one{module.sep}two{module.sep}"));
        Assert.Equal($"one{module.sep}two", module.join("one", "two"));
        Assert.Equal($"one{module.sep}two", module.join("one", $"{module.sep}two"));
        Assert.True(module.isAbsolute(System.IO.Path.GetFullPath(".")));
        Assert.NotNull(module.parse("/tmp/example.txt"));
        var parsed = Assert.IsAssignableFrom<IDictionary<string, object?>>(
            module.parse("/tmp/example.txt/"));
        Assert.Equal("example.txt", parsed["base"]);
        var parsedRoot = Assert.IsAssignableFrom<IDictionary<string, object?>>(
            module.parse(System.IO.Path.GetPathRoot(System.IO.Path.GetFullPath("."))!));
        Assert.Equal(parsedRoot["root"], parsedRoot["dir"]);
        var formatInput = new JavaScriptRuntime.JsObject();
        ((IDictionary<string, object?>)formatInput)["name"] = "example";
        ((IDictionary<string, object?>)formatInput)["ext"] = "txt";
        Assert.Equal("example.txt", module.format(formatInput));
        Assert.NotNull(module.posix);
        Assert.NotNull(module.win32);
        Assert.Null(
            typeof(JavaScriptRuntime.Node.Path).GetMethod(
                "InvokeContractMember",
                BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void IntrinsicPathModule_ThrowsForUnavailableMembers()
    {
        IPathModule module = new JavaScriptRuntime.Node.Path();

        var exception = Assert.Throws<NotImplementedException>(
            () => module.matchesGlob("index.js", "*.js"));

        Assert.Equal(
            "The intrinsic node:path module does not implement 'path.matchesGlob'.",
            exception.Message);
    }

    private static MethodInfo[] GetMethods(string memberName)
    {
        return typeof(IPathModule)
            .GetMethods()
            .Where(method => GetNodeMemberName(method) == memberName)
            .ToArray();
    }

    private static string? GetNodeMemberName(MethodInfo method)
    {
        return method.GetCustomAttribute<NodeModuleMemberAttribute>()?.MemberName;
    }
}
