using System.Reflection;
using System.Runtime.ExceptionServices;
using Jroc.IR;
using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace Jroc.Tests;

public static class InMemoryTestCompiler
{
    public static InMemoryTestExecutionResult CompileAndExecute(
        string testName,
        string testCategory,
        Func<string, (string Script, string? SourcePath)> getJavaScriptAndSourcePath,
        string[]? additionalScripts = null,
        bool executeAdditionalScriptsBeforeEntry = false,
        bool enableIRMetrics = false,
        bool allowUnhandledException = false,
        Action<ServiceContainer>? addMocks = null,
        int timeoutMs = 30000)
    {
        var (script, sourcePath) = getJavaScriptAndSourcePath(testName);
        var fileSystem = new MockFileSystem();
        string entryPath;
        string entrySourceText;

        if (executeAdditionalScriptsBeforeEntry && additionalScripts is { Length: > 0 })
        {
            var workingDirectory = sourcePath != null
                ? Path.Combine(Path.GetDirectoryName(sourcePath) ?? string.Empty, "__test262_harness__")
                : Path.Combine(
                    Path.GetTempPath(),
                    "Jroc.Tests",
                    "InMemory",
                    testCategory,
                    Guid.NewGuid().ToString("N"));

            var testLogicalPath = sourcePath != null
                ? Path.Combine(workingDirectory, Path.GetFileName(sourcePath))
                : Path.Combine(workingDirectory, "__test_entry__.js");
            fileSystem.AddFile(testLogicalPath, script, sourcePath);

            var bootstrapScript = new System.Text.StringBuilder();
            foreach (var scriptName in additionalScripts)
            {
                var (additionalScript, additionalSourcePath) = getJavaScriptAndSourcePath(scriptName);
                var normalizedModuleId = NormalizeModuleId(scriptName);
                var logicalPath = Path.Combine(
                    workingDirectory,
                    normalizedModuleId.Replace('/', Path.DirectorySeparatorChar) + ".js");
                fileSystem.AddFile(logicalPath, additionalScript, additionalSourcePath);
                if (string.Equals(normalizedModuleId, "node_modules/assert/index", StringComparison.Ordinal))
                {
                    bootstrapScript.AppendLine(additionalScript);
                    continue;
                }

                bootstrapScript.Append("require('./")
                    .Append(normalizedModuleId)
                    .AppendLine(".js');");
            }

            var entrySpecifier = "./" + Path.GetFileName(testLogicalPath).Replace('\\', '/');
            bootstrapScript.Append("require('")
                .Append(entrySpecifier)
                .AppendLine("');");

            entryPath = Path.Combine(workingDirectory, "__bootstrap__.js");
            entrySourceText = bootstrapScript.ToString();
            fileSystem.AddFile(entryPath, entrySourceText);
        }
        else
        {
            entryPath = sourcePath ?? Path.Combine(
                Path.GetTempPath(),
                "Jroc.Tests",
                "InMemory",
                testCategory,
                Guid.NewGuid().ToString("N"),
                $"{testName}.js");
            entrySourceText = script;
            fileSystem.AddFile(entryPath, script, sourcePath);

            if (additionalScripts != null)
            {
                foreach (var scriptName in additionalScripts)
                {
                    var (additionalScript, additionalSourcePath) = getJavaScriptAndSourcePath(scriptName);
                    var additionalPath = additionalSourcePath ?? Path.Combine(Path.GetDirectoryName(entryPath) ?? string.Empty, $"{scriptName}.js");
                    fileSystem.AddFile(additionalPath, additionalScript, additionalSourcePath);
                }
            }
        }

        var previousMetricsEnabled = false;
        if (enableIRMetrics)
        {
            previousMetricsEnabled = IRPipelineMetrics.Enabled;
            IRPipelineMetrics.Enabled = true;
            IRPipelineMetrics.Reset();
        }

        JrocCompiledAssemblyArtifact artifact;
        try
        {
            artifact = JrocInMemoryCompiler.Compile(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    SourceText = entrySourceText,
                    FileSystem = fileSystem,
                    RootModuleIdOverride = NormalizeModuleId(testName),
                    EmitPdb = true
                });
        }
        finally
        {
            if (enableIRMetrics)
            {
                IRPipelineMetrics.Enabled = previousMetricsEnabled;
            }
        }

        using var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);
        var output = ExecuteLoadedAssembly(loadedAssembly.Assembly, testName, allowUnhandledException, addMocks, timeoutMs);
        return new InMemoryTestExecutionResult(output, loadedAssembly.LoadContextWeakReference);
    }

    private static string ExecuteLoadedAssembly(
        Assembly assembly,
        string testName,
        bool allowUnhandledException,
        Action<ServiceContainer>? addMocks,
        int timeoutMs)
    {
        var output = new InMemoryConsoleOutput();
        var serviceProvider = RuntimeServices.BuildServiceProvider();
        serviceProvider.RegisterInstance(new ConsoleOutputSinks
        {
            Output = output,
            ErrorOutput = output
        });
        serviceProvider.RegisterInstance<IEnvironment>(new CapturingEnvironment());
        addMocks?.Invoke(serviceProvider);

        ExceptionDispatchInfo? threadException = null;
        var executionThread = new Thread(() =>
        {
            try
            {
                JavaScriptRuntime.Array.ResetPrototypeForTests();
                EnvironmentProvider.SuppressExit = true;
                Engine._serviceProviderOverride.Value = serviceProvider;

                var entryPoint = assembly.EntryPoint
                    ?? throw new InvalidOperationException("No entry point found in the generated assembly.");
                ((Action)Delegate.CreateDelegate(typeof(Action), entryPoint))();
            }
            catch (Exception ex)
            {
                threadException = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                if (serviceProvider.TryResolve<RuntimeExecutionContext>(out var runtimeContext)
                    && runtimeContext != null)
                {
                    RuntimeServices.UnregisterModuleRequires(runtimeContext.RegisteredModuleRequires);
                }

                GlobalThis.ServiceProvider = null;
                Engine._serviceProviderOverride.Value = null;
                RuntimeServices.SetCurrentThis(null);
                EnvironmentProvider.SuppressExit = false;
                JavaScriptRuntime.Array.ResetPrototypeForTests();
            }
        });

        executionThread.Start();
        if (!executionThread.Join(timeoutMs))
        {
            throw new TimeoutException($"In-memory test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
        }

        if (threadException != null && !allowUnhandledException)
        {
            threadException.Throw();
        }

        if (testName.StartsWith("Process_Exit_", StringComparison.Ordinal))
        {
            var environment = serviceProvider.Resolve<IEnvironment>() as CapturingEnvironment;
            return $"exitCode {environment?.ExitCalledWithCode ?? 0}{Environment.NewLine}";
        }

        return output.GetOutput();
    }

    private static string NormalizeModuleId(string moduleId)
        => moduleId.Trim().Replace('\\', '/').TrimStart('.', '/');
}

public sealed record InMemoryTestExecutionResult(string Output, WeakReference LoadContextWeakReference);

public sealed class InMemoryConsoleOutput : IConsoleOutput
{
    private readonly System.Text.StringBuilder _builder = new();

    public void Write(string text)
        => _builder.Append(text);

    public void WriteLine(string line)
        => _builder.AppendLine(line);

    public string GetOutput()
        => _builder.ToString();
}
