using System.Runtime.CompilerServices;
using Jroc.Tests;

namespace Jroc.Test262.Tests.language;

public abstract class DiskExecutionTestsBase
{
    private readonly string _testCategory;
    private readonly VerifySettings _verifySettings = new();

    protected DiskExecutionTestsBase(string testCategory)
    {
        _verifySettings.DisableDiff();
        _testCategory = testCategory;
    }

    protected async Task ExecutionTest(
        string testName,
        bool allowUnhandledException = false,
        Action<VerifySettings>? configureSettings = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(name, sourceFilePath),
            sourceFilePath,
            enableIRMetrics: true,
            allowUnhandledException: allowUnhandledException);

        await VerifyWithSnapshot(result.Output, sourceFilePath, configureSettings);
    }

    private static (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName, string callerSourceFilePath)
    {
        var relativePath = testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js";
        var sourceDirectory = Path.GetDirectoryName(callerSourceFilePath)
            ?? throw new InvalidOperationException("Unable to determine test source directory.");
        var scriptPath = Path.Combine(sourceDirectory, "JavaScript", relativePath);

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found at '{scriptPath}'.", scriptPath);
        }

        return (File.ReadAllText(scriptPath), scriptPath);
    }

    private Task VerifyWithSnapshot(
        string value,
        string sourceFilePath,
        Action<VerifySettings>? configureSettings)
    {
        var settings = new VerifySettings(_verifySettings);
        var directory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Could not resolve source directory.");
        var snapshotsDirectory = Path.Combine(directory, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        configureSettings?.Invoke(settings);
        return Verify(value, settings);
    }
}
