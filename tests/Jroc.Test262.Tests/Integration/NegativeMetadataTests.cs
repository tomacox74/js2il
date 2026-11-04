using Jroc.Tests;

namespace Jroc.Test262.Tests.Integration;

public class NegativeMetadataTests
{
    [Fact]
    public void RuntimeNegativeAutomaticallyCapturesDeclaredError()
    {
        var result = ExecuteRuntimeNegative("throw new TypeError('expected');");
        var error = Assert.IsType<JavaScriptRuntime.TypeError>(result.UnhandledException);
        Assert.Equal("expected", error.message);
        Assert.Empty(result.Output);
    }

    [Fact]
    public void RuntimeNegativeRejectsWrongErrorType()
    {
        var error = Assert.Throws<InvalidOperationException>(
            () => ExecuteRuntimeNegative("throw new RangeError('wrong');"));
        Assert.Contains("expected runtime exception 'TypeError', but got 'RangeError'", error.Message);
    }

    [Fact]
    public void RuntimeNegativeRejectsNormalCompletion()
    {
        var error = Assert.Throws<InvalidOperationException>(
            () => ExecuteRuntimeNegative("var completed = true;"));
        Assert.Contains("expected a runtime TypeError, but completed successfully", error.Message);
    }

    [Fact]
    public void ParseNegativeValidatesSyntaxErrorWithoutExecutingEval()
        => Test262CompilationAssertions.AssertParseSyntaxError("parse", ParseSource("eval(''); var = ;"), "parse.js");

    [Fact]
    public void ParseNegativeRejectsRuntimeSyntaxError()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            Test262CompilationAssertions.AssertParseSyntaxError(
                "parse", ParseSource("throw new SyntaxError('too late');"), "parse.js"));
        Assert.Contains("Expected a parse SyntaxError", error.Message);
    }

    [Fact]
    public void ParseNegativeUsesModuleGoal()
        => Test262CompilationAssertions.AssertParseSyntaxError(
            "parse", ParseSource("var await;", "flags: [module]\n"), "parse.js");

    [Fact]
    public void ParseNegativeHonorsOnlyStrict()
        => Test262CompilationAssertions.AssertParseSyntaxError(
            "parse", ParseSource("var yield;", "flags: [onlyStrict]\n"), "parse.js");

    private static string ParseSource(string body, string flags = "")
        => "/*---\n" + flags + "negative:\n  phase: parse\n  type: SyntaxError\n---*/\n" + body;

    private static InMemoryTestExecutionResult ExecuteRuntimeNegative(string body)
        => Test262SharedAssertHarness.CompileAndExecute(
            "runtime", "NegativeMetadataTests",
            _ => ("/*---\nnegative:\n  phase: runtime\n  type: TypeError\n---*/\n" + body,
                  Path.Combine(Directory.GetCurrentDirectory(), "runtime.js")));
}
