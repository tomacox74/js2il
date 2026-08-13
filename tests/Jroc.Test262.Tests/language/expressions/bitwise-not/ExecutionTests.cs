using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.bitwise_not;

public class ExecutionTests
{
    [Fact(DisplayName = "S11.4.8_A3_T3")]
    public Task S11_4_8_A3_T3()
        => ExecutionTestFromFile("S11.4.8_A3_T3");

    [Fact(DisplayName = "S11.4.8_A2.1_T1")]
    public Task S11_4_8_A2_1_T1()
        => ExecutionTestFromFile("S11.4.8_A2.1_T1");

    [Fact(DisplayName = "S11.4.8_A2.1_T2")]
    public Task S11_4_8_A2_1_T2()
        => ExecutionTestFromFile("S11.4.8_A2.1_T2");

    [Fact(DisplayName = "S11.4.8_A3_T1")]
    public Task S11_4_8_A3_T1()
        => ExecutionTestFromFile("S11.4.8_A3_T1");

    [Fact(DisplayName = "S11.4.8_A3_T2")]
    public Task S11_4_8_A3_T2()
        => ExecutionTestFromFile("S11.4.8_A3_T2");

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
            "language.expressions.bitwise-not",
            _ => (File.ReadAllText(jsPath), jsPath),
            enableIRMetrics: true);
        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "bigint")]
    public Task bigint()
        => ExecutionTestFromFile("bigint");
}
