using System.Runtime.CompilerServices;
using Jroc;
using Jroc.IR;
using Jroc.Tests;

namespace Jroc.Test262.Tests.language.modules;

public abstract class FileSystemExecutionTestsBase
{
    private readonly string _relativeCategoryPath;
    private readonly string _testCategory;
    private readonly VerifySettings _verifySettings = new();

    protected FileSystemExecutionTestsBase(string relativeCategoryPath, string testCategory)
    {
        _relativeCategoryPath = relativeCategoryPath;
        _testCategory = testCategory;
        _verifySettings.DisableDiff();
    }

    protected async Task ExecutionTest(string testName, bool allowUnhandledException = false, [CallerFilePath] string sourceFilePath = "")
    {
        string projectRoot = FindProjectRoot(sourceFilePath);
        var result = InMemoryTestCompiler.CompileAndExecute(
            testName,
            _testCategory,
            name => GetJavaScriptAndSourcePath(projectRoot, name),
            enableIRMetrics: true,
            allowUnhandledException: allowUnhandledException);

        await VerifyWithSnapshot(result.Output, sourceFilePath);
    }

    protected async Task CompilationFailureTest(
        string testName,
        string? expectedFailureText = null,
        [CallerFilePath] string sourceFilePath = "")
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

        if (!string.IsNullOrWhiteSpace(expectedFailureText)
            && !failure.ToString().Contains(expectedFailureText, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Compilation failed for test {testName}, but the failure did not contain '{expectedFailureText}'.\nActual failure:\n{failure}");
        }

        await VerifyWithSnapshot("true" + Environment.NewLine, sourceFilePath);
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

    private Task VerifyWithSnapshot(string value, string sourceFilePath)
    {
        var settings = new VerifySettings(_verifySettings);
        string snapshotsDirectory = Path.Combine(Path.GetDirectoryName(sourceFilePath)!, "Snapshots");
        Directory.CreateDirectory(snapshotsDirectory);
        settings.UseDirectory(snapshotsDirectory);
        return Verify(value, settings);
    }

    private static string FindProjectRoot(string sourceFilePath)
    {
        string? directory = Path.GetDirectoryName(sourceFilePath);
        while (directory != null)
        {
            string candidate = Path.Combine(directory, "Jroc.Test262.Tests.csproj");
            if (File.Exists(candidate))
            {
                return directory;
            }

            directory = Path.GetDirectoryName(directory);
        }

        throw new InvalidOperationException("Unable to locate Jroc.Test262.Tests.csproj from source file path.");
    }
}
