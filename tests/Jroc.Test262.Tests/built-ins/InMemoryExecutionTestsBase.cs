using System.Runtime.CompilerServices;
using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins;

[Collection(InMemoryExecutionTestsBase.CollectionName)]
public abstract class InMemoryExecutionTestsBase
{
    internal const string CollectionName = "Built-ins in-memory execution";

    private readonly string _testCategory;
    private readonly VerifySettings _verifySettings = new();

    protected InMemoryExecutionTestsBase(string testCategory)
    {
        _verifySettings.DisableDiff();
        _testCategory = testCategory;
    }

    protected Task ExecutionTest(string testName, [CallerFilePath] string sourceFilePath = "")
        => ExecutionTestFromFile(testName, sourceFilePath);

    protected async Task ExecutionTestFromFile(string testName, [CallerFilePath] string sourceFilePath = "")
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(name, sourceFilePath),
            enableIRMetrics: true);

        await VerifyWithSnapshot(result.Output, sourceFilePath);
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

[CollectionDefinition(InMemoryExecutionTestsBase.CollectionName)]
public sealed class InMemoryExecutionTestCollection
{
}
