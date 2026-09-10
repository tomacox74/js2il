using System.Runtime.CompilerServices;
using Jroc;
using Jroc.IR;
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
            enableIRMetrics: true,
            allowUnhandledException: allowUnhandledException);

        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }

    protected Task CompilationFailureTest(
        string testName,
        string? expectedFailureText = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        var (script, sourcePath) = GetJavaScriptAndSourcePath(testName, sourceFilePath);
        var preparedScript = Test262SharedAssertHarness.PrepareEntryScript(script);
        Exception? failure = null;

        var previousMetricsEnabled = IRPipelineMetrics.Enabled;
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();
        try
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(sourcePath, preparedScript, sourcePath);
            JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(sourcePath)
            {
                SourceText = preparedScript,
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

        return Task.CompletedTask;
    }

    private static (string Script, string SourcePath) GetJavaScriptAndSourcePath(string testName, string callerSourceFilePath)
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
