using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.unary_plus;

public class ExecutionTests
{
    private readonly VerifySettings _verifySettings = new();

    public ExecutionTests()
    {
        _verifySettings.DisableDiff();
    }

    [Fact(DisplayName = "11.4.6-2-1")]
    public Task _11_4_6_2_1()
        => ExecutionTestFromFile("11.4.6-2-1");

    [Fact(DisplayName = "S11.4.6_A3_T3")]
    public Task S11_4_6_A3_T3()
        => ExecutionTestFromFile("S11.4.6_A3_T3");

    [Fact(DisplayName = "S11.4.6_A2.1_T1")]
    public Task S11_4_6_A2_1_T1()
        => ExecutionTestFromFile("S11.4.6_A2.1_T1");

    [Fact(DisplayName = "S11.4.6_A3_T1")]
    public Task S11_4_6_A3_T1()
        => ExecutionTestFromFile("S11.4.6_A3_T1");

    [Fact(DisplayName = "S11.4.6_A3_T2")]
    public Task S11_4_6_A3_T2()
        => ExecutionTestFromFile("S11.4.6_A3_T2");

    private async Task ExecutionTestFromFile(string testName, [CallerFilePath] string sourceFilePath = "")
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
            "language.expressions.unary-plus",
            _ => (File.ReadAllText(jsPath), jsPath),
            sourceFilePath,
            enableIRMetrics: true);
        await VerifyWithSnapshot(result.Output, sourceFilePath);
    }

    private Task VerifyWithSnapshot(string value, string sourceFilePath)
    {
        var settings = new VerifySettings(_verifySettings);
        var directory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Could not resolve source directory.");
        var snapshotsDirectory = Path.Combine(directory, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        return Verify(value, settings);
    }
}
