using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Js2IL.Tests;
using Js2IL.Runtime;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Tests.Hosting;

public class EventLoopKeepAliveTests
{
    [Fact]
    public async Task Timers_Fire_WhileHostIsIdle()
    {
        using var exports = LoadExports(out _);

        exports.StartTimer(25);

        // Regression coverage: timers should fire without the host having to pump the runtime.
        await Task.Delay(TimeSpan.FromMilliseconds(250));

        Assert.Equal(123, exports.GetState());
    }

    private static IKeepAliveExports LoadExports(out string moduleId)
    {
        var assembly = CompileModule("Hosting_EventLoopKeepAlive", out moduleId);
        return JsEngine.LoadModule<IKeepAliveExports>(assembly, moduleId);
    }

    private static Assembly CompileModule(string scriptName, out string moduleId)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Hosting", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var js = JavaScriptTestSource.EnsureUseStrict(GetJavaScript(scriptName));
        moduleId = $"{scriptName}_{Guid.NewGuid():N}";
        var testFilePath = Path.Combine(outputPath, moduleId + ".js");

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(testFilePath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = outputPath
        };

        var testLogger = new TestLogger();
        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
        var compiler = serviceProvider.GetRequiredService<Compiler>();

        if (!compiler.Compile(testFilePath))
        {
            var details = string.IsNullOrWhiteSpace(testLogger.Errors)
                ? string.Empty
                : $"\nErrors:\n{testLogger.Errors}";
            var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                ? string.Empty
                : $"\nWarnings:\n{testLogger.Warnings}";
            throw new InvalidOperationException($"Compilation failed for hosting test.{details}{warnings}");
        }

        var assemblyPath = Path.Combine(outputPath, moduleId + ".dll");
        return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
    }

    private static string GetJavaScript(string testName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceKey = $"Js2IL.Tests.Hosting.JavaScript.{testName}.js";

        using var stream = assembly.GetManifestResourceStream(resourceKey)
            ?? throw new InvalidOperationException($"Resource '{resourceKey}' not found in assembly '{assembly.FullName}'.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

public interface IKeepAliveExports : IDisposable
{
    void StartTimer(double ms);

    double GetState();
}
