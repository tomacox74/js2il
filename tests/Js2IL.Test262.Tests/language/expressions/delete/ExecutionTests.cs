using Js2IL.Tests;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Js2IL.Test262.Tests.language.expressions.delete;

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

    [Fact(DisplayName = "11.4.1-2-2", Skip = "Delete of a function call result does not yet match test262 semantics.")]
    public Task _11_4_1_2_2()
        => ExecutionTestFromFile("11.4.1-2-2");

    [Fact(DisplayName = "11.4.1-3-1", Skip = "Expected runtime handling of an unresolved identifier, but JS2IL rejects unresolved globals during compilation.")]
    public Task _11_4_1_3_1()
        => ExecutionTestFromFile("11.4.1-3-1");

    [Fact(DisplayName = "11.4.1-3-2", Skip = "Expected runtime ReferenceError inspection, but JS2IL does not currently support this unresolved-reference scenario.")]
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

        var outputRoot = Path.Combine(sourceDirectory, ".execution-output", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputRoot);

        try
        {
            var compiled = TestCompiler.Compile(
                testName,
                "language.expressions.delete",
                outputRoot,
                _ => (File.ReadAllText(jsPath), jsPath),
                additionalScripts: null,
                enableIRMetrics: true);

            var output = ExecuteGeneratedAssembly(compiled.AssemblyPath);
            await VerifyWithSnapshot(output, sourceFilePath);
        }
        finally
        {
            try
            {
                if (Directory.Exists(outputRoot))
                {
                    Directory.Delete(outputRoot, recursive: true);
                }
            }
            catch
            {
            }
        }
    }

    private static string ExecuteGeneratedAssembly(string assemblyPath, int timeoutMs = 30000)
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

        using var process = Process.Start(processInfo)
            ?? throw new InvalidOperationException("Failed to start generated assembly.");
        bool exited = process.WaitForExit(timeoutMs);

        if (!exited)
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        string stdOut = process.StandardOutput.ReadToEnd();
        string stdErr = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            throw new Exception($"dotnet execution failed (exit {process.ExitCode}):\nSTDERR:\n{stdErr}\nSTDOUT:\n{stdOut}");
        }

        return stdOut;
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
