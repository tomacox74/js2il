using System.Runtime.CompilerServices;
using Jroc.Tests;

namespace Jroc.Test262.Tests.language;

public abstract class DiskExecutionTestsBase
{
    private readonly string _testCategory;
    protected DiskExecutionTestsBase(string testCategory)
    {
        _testCategory = testCategory;
    }

    protected Task ExecutionTest(
        string testName,
        bool allowUnhandledException = false,
        [CallerFilePath] string sourceFilePath = "")
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(name, sourceFilePath),
            sourceFilePath,
            enableIRMetrics: true,
            allowUnhandledException: allowUnhandledException);

        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
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

}
