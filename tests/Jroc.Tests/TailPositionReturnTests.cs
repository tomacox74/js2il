using Jroc.Tests.Utilities;

namespace Jroc.Tests;

public class TailPositionReturnTests
{
    [Theory]
    [InlineData("null ?? recurse(n - 1)", "12")]
    [InlineData("undefined ?? recurse(n - 1)", "12")]
    [InlineData("(visits++, recurse(n - 1))", "24")]
    [InlineData("(visits++, null ?? (visits++, recurse(n - 1)))", "36")]
    public void FinalCallsEmitTailPrefix(string expression, string expected)
    {
        var artifact = CompileRecursion(expression);

        Assert.Contains("tail.", AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
        Assert.Equal(expected, Execute(artifact));
    }

    [Theory]
    [InlineData("recurse(n - 1) ?? 'done'")]
    [InlineData("(recurse(n - 1), 'done')")]
    [InlineData("null ?? (recurse(n - 1), 'done')")]
    [InlineData("(recurse(n - 1), null) ?? 'done'")]
    public void NonFinalCallsDoNotEmitTailPrefix(string expression)
    {
        var artifact = CompileRecursion(expression);

        Assert.DoesNotContain("tail.", AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
        Assert.Equal("12", Execute(artifact));
    }

    [Fact]
    public void NullishTailBranchEvaluatesLeftOnceAndShortCircuits()
    {
        var artifact = Compile("""
            var reads = 0;
            function read(value) { reads++; return value; }
            function fallback() { throw new Error("unexpected call"); }
            function choose(value) { return read(value) ?? fallback(); }
            var marker = {};
            console.log(
                choose(marker) === marker &&
                choose(false) === false &&
                choose(0) === 0 &&
                choose("") === "",
                reads);
            """);

        Assert.Contains("tail.", AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
        Assert.Equal("true 4", Execute(artifact));
    }

    [Fact]
    public void AbruptSequencePrefixDoesNotInvokeTailCallee()
    {
        var artifact = Compile("""
            var calls = 0;
            function fail() { throw new Error("stop"); }
            function target() { calls++; }
            function sequence() { return (fail(), target()); }
            try { sequence(); } catch (error) { console.log(error.message, calls); }
            """);

        Assert.Contains("tail.", AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
        Assert.Equal("stop 0", Execute(artifact));
    }

    [Fact]
    public void CompositeReturnsPreserveTypedMethodAbi()
    {
        var artifact = Compile("""
            class Values {
                number() { return null ?? 42; }
                text() { return (0, "text"); }
                boolean() { return undefined ?? true; }
            }
            var values = new Values();
            console.log(values.number(), values.text(), values.boolean());
            """);

        Assert.DoesNotContain("tail.", AssemblyToText.ConvertToText(artifact.PeBytes, artifact.AssemblyName));
        Assert.Equal("42 text true", Execute(artifact));
    }

    private static JrocCompiledAssemblyArtifact CompileRecursion(string expression)
        => Compile($$"""
            "use strict";
            var visits = 0;
            (function recurse(n) {
                if (n === 0) { return; }
                visits++;
                return {{expression}};
            }(12));
            console.log(visits);
            """);

    private static JrocCompiledAssemblyArtifact Compile(string source)
        => JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(
            Path.Combine(Path.GetTempPath(), nameof(TailPositionReturnTests) + ".js"))
        {
            AssemblyName = nameof(TailPositionReturnTests),
            SourceText = source
        });

    private static string Execute(JrocCompiledAssemblyArtifact artifact)
        => InMemoryTestCompiler.ExecuteArtifact(
            artifact,
            Path.Combine(Path.GetTempPath(), nameof(TailPositionReturnTests) + ".js"),
            nameof(TailPositionReturnTests)).Output.Trim();
}
