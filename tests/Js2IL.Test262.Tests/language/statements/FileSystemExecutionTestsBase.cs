using System.Diagnostics;
using System.Runtime.CompilerServices;
using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements;

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

    private (string Script, string SourcePath) GetJavaScriptAndSourcePath(string projectRoot, string testName)
    {
        string sourcePath = Path.Combine(projectRoot, _relativeCategoryPath, "JavaScript", testName + ".js");
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"JavaScript fixture not found: '{sourcePath}'.", sourcePath);
        }

        return (File.ReadAllText(sourcePath), sourcePath);
    }

    private static string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException, int timeoutMs = 30000)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = assemblyPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo) ?? throw new InvalidOperationException("Failed to start dotnet process.");
        if (!process.WaitForExit(timeoutMs))
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        string stdOut = process.StandardOutput.ReadToEnd();
        string stdErr = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            if (!allowUnhandledException)
            {
                throw new Exception($"dotnet execution failed:\n{stdErr}\n{stdOut}");
            }

            return stdOut;
        }

        return stdOut;
    }

    private static string FindProjectRoot(string sourceFilePath)
    {
        string? current = Path.GetDirectoryName(sourceFilePath);
        while (!string.IsNullOrWhiteSpace(current))
        {
            string candidate = Path.Combine(current, "Js2IL.Test262.Tests.csproj");
            if (File.Exists(candidate))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new InvalidOperationException($"Could not find project root from '{sourceFilePath}'.");
    }

    private static string SanitizePathSegment(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length);
        foreach (char ch in value)
        {
            builder.Append(Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch);
        }

        return builder.ToString().Replace('.', '_').Replace('\\', '_').Replace('/', '_');
    }

    private static void TryDeleteDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return;
        }

        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                Directory.Delete(path, recursive: true);
                return;
            }
            catch (IOException) when (attempt < 2)
            {
                Thread.Sleep(50);
            }
            catch (UnauthorizedAccessException) when (attempt < 2)
            {
                Thread.Sleep(50);
            }
        }
    }
}
