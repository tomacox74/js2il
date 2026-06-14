using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Jroc.Tests;

namespace Jroc.Test262.Tests.language;

public abstract class DiskExecutionTestsBase
{
    private readonly string _testCategory;
    private readonly VerifySettings _verifySettings = new();
    private static readonly ConcurrentDictionary<(string Category, string TestName), Lazy<CompilationResult>> Cache = new();
    private static readonly string OutputRoot = Path.Combine(Path.GetTempPath(), "Jroc.Test262.Tests");

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
        var compiled = GetOrCompile(testName, sourceFilePath);
        var output = await ExecuteGeneratedAssemblyOutOfProc(compiled.AssemblyPath, testName, allowUnhandledException);

        var settings = new VerifySettings(_verifySettings);
        var directory = Path.GetDirectoryName(sourceFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            var snapshotsDirectory = Path.Combine(directory, "Snapshots");
            Directory.CreateDirectory(snapshotsDirectory);
            settings.UseDirectory(snapshotsDirectory);
        }

        configureSettings?.Invoke(settings);
        await Verify(output, settings);
    }

    private CompiledAssembly GetOrCompile(string testName, string sourceFilePath)
    {
        var key = (_testCategory, testName);
        var lazy = Cache.GetOrAdd(key, _ => new Lazy<CompilationResult>(() =>
        {
            try
            {
                var outputDirectory = Path.Combine(
                    OutputRoot,
                    _testCategory + ".ExecutionTests",
                    Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(outputDirectory);

                return new CompilationResult(TestCompiler.Compile(
                    testName,
                    _testCategory,
                    outputDirectory,
                    _ => GetJavaScriptAndSourcePath(testName, sourceFilePath),
                    additionalScripts: null,
                    enableIRMetrics: true));
            }
            catch (Exception ex)
            {
                return new CompilationResult(ex);
            }
        }));

        var result = lazy.Value;
        if (result.Exception != null)
        {
            throw new InvalidOperationException($"Compilation failed for test {testName}", result.Exception);
        }

        return result.CompiledAssembly!;
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

    private static async Task<string> ExecuteGeneratedAssemblyOutOfProc(
        string assemblyPath,
        string testName,
        bool allowUnhandledException,
        int timeoutMs = 30000)
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
        var exited = await Task.Run(() => process.WaitForExit(timeoutMs));
        if (!exited)
        {
            process.Kill();
            throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        var standardOutput = await process.StandardOutput.ReadToEndAsync();
        var standardError = await process.StandardError.ReadToEndAsync();
        if (process.ExitCode != 0 && !allowUnhandledException)
        {
            throw new Exception($"dotnet execution failed for {testName}:{Environment.NewLine}{standardError}");
        }

        return standardOutput;
    }

    private sealed class CompilationResult
    {
        public CompilationResult(CompiledAssembly assembly)
        {
            CompiledAssembly = assembly;
        }

        public CompilationResult(Exception exception)
        {
            Exception = exception;
        }

        public CompiledAssembly? CompiledAssembly { get; }
        public Exception? Exception { get; }
    }

}
