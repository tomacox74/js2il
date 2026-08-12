using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.in_;

public class ExecutionTests
{
    [Fact(DisplayName = "S11.8.7_A4")]
    public Task S11_8_7_A4()
        => ExecutionTestFromFile("S11.8.7_A4");

    [Fact(DisplayName = "S11.8.7_A2.1_T1")]
    public Task S11_8_7_A2_1_T1()
        => ExecutionTestFromFile("S11.8.7_A2.1_T1");

    [Fact(DisplayName = "S11.8.7_A2.1_T2")]
    public Task S11_8_7_A2_1_T2()
        => ExecutionTestFromFile("S11.8.7_A2.1_T2");

    [Fact(DisplayName = "S11.8.7_A2.1_T3")]
    public Task S11_8_7_A2_1_T3()
        => ExecutionTestFromFile("S11.8.7_A2.1_T3");

    [Fact(DisplayName = "S11.8.7_A3")]
    public Task S11_8_7_A3()
        => ExecutionTestFromFile("S11.8.7_A3");

    [Fact(DisplayName = "private-field-presence-field.js")]
    public Task private_field_presence_field()
        => ExecutionTestFromFile("private-field-presence-field");

    [Fact(DisplayName = "private-field-presence-field-shadowed.js")]
    public Task private_field_presence_field_shadowed()
        => ExecutionTestFromFile("private-field-presence-field-shadowed");

    [Fact(DisplayName = "private-field-rhs-non-object.js")]
    public Task private_field_rhs_non_object()
        => ExecutionTestFromFile("private-field-rhs-non-object");

    private Task ExecutionTestFromFile(string testName, [CallerFilePath] string sourceFilePath = "")
    {
        var sourceDirectory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Could not resolve source directory.");
        var jsPath = Path.Combine(sourceDirectory, "JavaScript", testName + ".js");
        if (!File.Exists(jsPath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found: {jsPath}", jsPath);
        }

        var result = Test262SharedAssertHarness.CompileAndExecute(
            testName,
            "language.expressions.in",
            _ => (File.ReadAllText(jsPath), jsPath),
            sourceFilePath,
            enableIRMetrics: true);
        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }
}
