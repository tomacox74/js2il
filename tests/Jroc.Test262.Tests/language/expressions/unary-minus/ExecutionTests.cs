using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.unary_minus;

public class ExecutionTests
{
    [Fact(DisplayName = "11.4.7-4-1")]
    public Task _11_4_7_4_1()
        => ExecutionTestFromFile("11.4.7-4-1");

    [Fact(DisplayName = "S11.4.7_A3_T3")]
    public Task S11_4_7_A3_T3()
        => ExecutionTestFromFile("S11.4.7_A3_T3");

    [Fact(DisplayName = "S11.4.7_A2.1_T1")]
    public Task S11_4_7_A2_1_T1()
        => ExecutionTestFromFile("S11.4.7_A2.1_T1");

    [Fact(DisplayName = "S11.4.7_A3_T1")]
    public Task S11_4_7_A3_T1()
        => ExecutionTestFromFile("S11.4.7_A3_T1");

    [Fact(DisplayName = "S11.4.7_A3_T2")]
    public Task S11_4_7_A3_T2()
        => ExecutionTestFromFile("S11.4.7_A3_T2");

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
            "language.expressions.unary-minus",
            _ => (File.ReadAllText(jsPath), jsPath),
            enableIRMetrics: true);
        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }
}
