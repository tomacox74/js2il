using System.Runtime.CompilerServices;
using Jroc;
using Jroc.IR;
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
        var additionalScripts = GetAdditionalScripts(testName, sourceFilePath);
        var result = InMemoryTestCompiler.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(name, sourceFilePath),
            additionalScripts: additionalScripts,
            enableIRMetrics: true);

        await VerifyWithSnapshot(result.Output, sourceFilePath);
    }

    protected async Task CompilationFailureTest(
        string testName,
        string? expectedFailureText = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        var (script, sourcePath) = GetJavaScriptAndSourcePath(testName, sourceFilePath);
        sourcePath ??= Path.Combine(
            Path.GetTempPath(),
            "Jroc.Test262.Tests",
            "CompilationFailure",
            testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js");
        Exception? failure = null;

        var previousMetricsEnabled = IRPipelineMetrics.Enabled;
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();
        try
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(sourcePath, script, sourcePath);
            JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(sourcePath)
            {
                SourceText = script,
                FileSystem = fileSystem,
                EmitPdb = true
            });
        }
        catch (Exception ex)
        {
            failure = ex;
        }
        finally
        {
            IRPipelineMetrics.Enabled = previousMetricsEnabled;
        }

        if (failure == null)
        {
            throw new InvalidOperationException($"Expected compilation to fail for test {testName}.");
        }

        if (!string.IsNullOrWhiteSpace(expectedFailureText)
            && !failure.ToString().Contains(expectedFailureText, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Compilation failed for test {testName}, but the failure did not contain '{expectedFailureText}'.\nActual failure:\n{failure}");
        }

        await VerifyWithSnapshot("true" + Environment.NewLine, sourceFilePath);
    }

    private static (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName, string callerSourceFilePath)
    {
        if (string.Equals(testName, "node_modules\\assert\\index", StringComparison.Ordinal)
            || string.Equals(testName, "node_modules/assert/index", StringComparison.Ordinal))
        {
            return (File.ReadAllText(GetAssertHarnessSourcePath(callerSourceFilePath)), null);
        }

        var relativePath = testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js";
        var sourceDirectory = Path.GetDirectoryName(callerSourceFilePath)
            ?? throw new InvalidOperationException("Unable to determine test source directory.");
        var scriptPath = Path.Combine(sourceDirectory, "JavaScript", relativePath);

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found at '{scriptPath}'.", scriptPath);
        }

        var script = File.ReadAllText(scriptPath);
        if (ShouldInjectAssertHarness(scriptPath))
        {
            script = "const { assert } = require('assert');" + Environment.NewLine + script;
        }

        return (script, scriptPath);
    }

    private static string[]? GetAdditionalScripts(string testName, string callerSourceFilePath)
    {
        var sourceDirectory = Path.GetDirectoryName(callerSourceFilePath)
            ?? throw new InvalidOperationException("Unable to determine test source directory.");
        var scriptPath = Path.Combine(
            sourceDirectory,
            "JavaScript",
            testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js");

        return ShouldInjectAssertHarness(scriptPath)
            ? ["node_modules/assert/index"]
            : null;
    }

    private static bool ShouldInjectAssertHarness(string scriptPath)
    {
        var normalizedPath = Path.GetFullPath(scriptPath);
        var sectionDirectory = Path.Combine(
            "tests",
            "Jroc.Test262.Tests",
            "built-ins",
            "Date",
            "Section21_4",
            "");

        return normalizedPath.Contains(sectionDirectory, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetAssertHarnessSourcePath(string callerSourceFilePath)
    {
        var repoRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(callerSourceFilePath)!, "..", "..", "..", ".."));
        return Path.Combine(repoRoot, "Harness", "assert.js");
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
