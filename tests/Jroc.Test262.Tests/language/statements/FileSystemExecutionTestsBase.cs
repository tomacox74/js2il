using System.Runtime.CompilerServices;
using Jroc;
using Jroc.IR;
using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements;

public abstract class FileSystemExecutionTestsBase
{
    private readonly string _relativeCategoryPath;
    private readonly string _testCategory;
    protected FileSystemExecutionTestsBase(string relativeCategoryPath, string testCategory)
    {
        _relativeCategoryPath = relativeCategoryPath;
        _testCategory = testCategory;
    }

    protected Task ExecutionTest(string testName, bool allowUnhandledException = false, [CallerFilePath] string sourceFilePath = "")
    {
        string projectRoot = FindProjectRoot(sourceFilePath);
        var result = Test262SharedAssertHarness.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(projectRoot, name),
            enableIRMetrics: true,
            allowUnhandledException: allowUnhandledException);

        Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
        return Task.CompletedTask;
    }

    protected Task CompilationFailureTest(string testName, string expectedFailureText, [CallerFilePath] string sourceFilePath = "")
    {
        string projectRoot = FindProjectRoot(sourceFilePath);
        var (script, sourcePath) = GetJavaScriptAndSourcePath(projectRoot, testName);
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

        if (!failure.ToString().Contains(expectedFailureText, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Compilation failed for test {testName}, but the failure did not contain '{expectedFailureText}'.\nActual failure:\n{failure}");
        }

        return Task.CompletedTask;
    }

    private (string Script, string SourcePath) GetJavaScriptAndSourcePath(string projectRoot, string testName)
    {
        string normalizedCategoryPath = _relativeCategoryPath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        string normalizedTestName = testName
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        string sourcePath = Path.Combine(projectRoot, normalizedCategoryPath, "JavaScript", normalizedTestName + ".js");
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found: '{sourcePath}'.", sourcePath);
        }

        return (File.ReadAllText(sourcePath), sourcePath);
    }

    private static string FindProjectRoot(string sourceFilePath)
    {
        string? current = Path.GetDirectoryName(sourceFilePath);
        while (!string.IsNullOrWhiteSpace(current))
        {
            string candidate = Path.Combine(current, "Jroc.Test262.Tests.csproj");
            if (File.Exists(candidate))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new InvalidOperationException($"Could not find project root from '{sourceFilePath}'.");
    }
}
