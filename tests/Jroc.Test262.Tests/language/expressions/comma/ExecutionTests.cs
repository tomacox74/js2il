using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.comma;

public class ExecutionTests
{
    [Fact(DisplayName = "S11.14_A2.1_T1")]
    public Task S11_14_A2_1_T1()
        => ExecutionTestFromFile("S11.14_A2.1_T1");

    [Fact(DisplayName = "S11.14_A2.1_T2")]
    public Task S11_14_A2_1_T2()
        => ExecutionTestFromFile("S11.14_A2.1_T2");

    [Fact(DisplayName = "S11.14_A2.1_T3")]
    public Task S11_14_A2_1_T3()
        => ExecutionTestFromFile("S11.14_A2.1_T3");

    [Fact(DisplayName = "S11.14_A3")]
    public Task S11_14_A3()
        => ExecutionTestFromFile("S11.14_A3");

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
            "language.expressions.comma",
            _ => (File.ReadAllText(jsPath), jsPath),
            sourceFilePath,
            enableIRMetrics: true);
        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }
}
