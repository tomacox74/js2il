using System.Runtime.CompilerServices;
using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.modules;

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
        string outputRoot = Path.Combine(
            projectRoot,
            "TestOutput",
            "Js2IL.Test262.Tests",
            SanitizePathSegment(_testCategory),
            SanitizePathSegment(testName),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(outputRoot);

        try
        {
            var compiled = TestCompiler.Compile(
                testName,
                _testCategory,
                outputRoot,
                name => GetJavaScriptAndSourcePath(projectRoot, name),
                additionalScripts: null,
                enableIRMetrics: true);

            string output = ExecuteGeneratedAssembly(compiled.AssemblyPath, allowUnhandledException);

            var settings = new VerifySettings(_verifySettings);
            string snapshotsDirectory = Path.Combine(Path.GetDirectoryName(sourceFilePath)!, "Snapshots");
            Directory.CreateDirectory(snapshotsDirectory);
            settings.UseDirectory(snapshotsDirectory);
            await Verify(output, settings);
        }
        finally
        {
            TryDeleteDirectory(outputRoot);
        }
    }

    protected async Task CompilationFailureTest(
        string testName,
        string? expectedFailureText = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        string projectRoot = FindProjectRoot(sourceFilePath);
        string outputRoot = Path.Combine(
            projectRoot,
            "TestOutput",
            "Js2IL.Test262.Tests",
            SanitizePathSegment(_testCategory),
            SanitizePathSegment(testName),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(outputRoot);

        try
        {
            Exception? failure = null;

            try
            {
                TestCompiler.Compile(
                    testName,
                    _testCategory,
                    outputRoot,
                    name => GetJavaScriptAndSourcePath(projectRoot, name),
                    additionalScripts: null,
                    enableIRMetrics: true);
            }
            catch (Exception ex)
            {
                failure = ex;
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

            var settings = new VerifySettings(_verifySettings);
            string snapshotsDirectory = Path.Combine(Path.GetDirectoryName(sourceFilePath)!, "Snapshots");
            Directory.CreateDirectory(snapshotsDirectory);
            settings.UseDirectory(snapshotsDirectory);
            await Verify("true" + Environment.NewLine, settings);
        }
        finally
        {
            TryDeleteDirectory(outputRoot);
        }
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

    private static string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException, int timeoutMs = 30000)
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = assemblyPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(processInfo)
            ?? throw new InvalidOperationException($"Failed to start dotnet for '{assemblyPath}'.");
        bool exited = process.WaitForExit(timeoutMs);
        if (!exited)
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0 && !allowUnhandledException)
        {
            throw new Exception($"dotnet execution failed:{Environment.NewLine}{standardError}");
        }

        return standardOutput;
    }

    private static string FindProjectRoot(string sourceFilePath)
    {
        string? directory = Path.GetDirectoryName(sourceFilePath);
        while (directory != null)
        {
            string candidate = Path.Combine(directory, "Js2IL.Test262.Tests.csproj");
            if (File.Exists(candidate))
            {
                return directory;
            }

            directory = Path.GetDirectoryName(directory);
        }

        throw new InvalidOperationException("Unable to locate Js2IL.Test262.Tests.csproj from source file path.");
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch));
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(path, recursive: true);
        }
        catch
        {
        }
    }
}
