using Jroc.Tests;
using System.Runtime.CompilerServices;

namespace Jroc.Test262.Tests.language.expressions.delete;

public class ExecutionTests
{
    private readonly VerifySettings _verifySettings = new();

    public ExecutionTests()
    {
        _verifySettings.DisableDiff();
    }

    [Fact(DisplayName = "11.4.1-0-1")]
    public Task _11_4_1_0_1()
        => ExecutionTestFromFile("11.4.1-0-1");

    [Fact(DisplayName = "11.4.1-2-2")]
    public Task _11_4_1_2_2()
        => ExecutionTestFromFile("11.4.1-2-2");

    [Fact(DisplayName = "11.4.1-3-1")]
    public Task _11_4_1_3_1()
        => ExecutionTestFromFile("11.4.1-3-1");

    [Fact(DisplayName = "11.4.1-3-2")]
    public Task _11_4_1_3_2()
        => ExecutionTestFromFile("11.4.1-3-2");

    [Fact(DisplayName = "11.4.1-3-3")]
    public Task _11_4_1_3_3()
        => ExecutionTestFromFile("11.4.1-3-3");

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
            "language.expressions.delete",
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
