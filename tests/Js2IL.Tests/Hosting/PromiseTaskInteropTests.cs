using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Js2IL.Tests;
using Js2IL.Runtime;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Tests.Hosting;

public class PromiseTaskInteropTests
{
    [Fact]
    public async Task PromiseToTask_ImmediateResolve_ConvertsValue()
    {
        using var exports = LoadExports(out _);

        var result = await exports.ImmediateResolve(123);
        Assert.Equal(123, result);
    }

    [Fact]
    public async Task PromiseToTask_ImmediateReject_StringBecomesJsThrownValueException()
    {
        using var exports = LoadExports(out _);

        var ex = await Assert.ThrowsAsync<JavaScriptRuntime.JsThrownValueException>(() => exports.ImmediateReject("nope"));
        Assert.Equal("nope", ex.Value);
    }

    [Fact]
    public async Task PromiseToTask_ImmediateReject_ErrorPropagates()
    {
        using var exports = LoadExports(out _);

        await Assert.ThrowsAsync<JavaScriptRuntime.Error>(() => exports.ImmediateRejectError());
    }

    [Fact]
    public async Task PromiseToTask_TimerResolve_DoesNotDeadlock()
    {
        using var exports = LoadExports(out _);

        var task = exports.TimeoutResolve(25, 7);

        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.Same(task, completed);

        Assert.Equal(7, await task);
    }

    [Fact]
    public async Task PromiseToTask_TimerReject_DoesNotDeadlock()
    {
        using var exports = LoadExports(out _);

        var task = exports.TimeoutReject(25, "later");

        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.Same(task, completed);

        var ex = await Assert.ThrowsAsync<JavaScriptRuntime.JsThrownValueException>(() => task);
        Assert.Equal("later", ex.Value);
    }

    private static IPromiseExports LoadExports(out string moduleId)
    {
        var assembly = CompileModule("Hosting_PromiseTaskInterop", out moduleId);
        return JsEngine.LoadModule<IPromiseExports>(assembly, moduleId);
    }

    private static Assembly CompileModule(string scriptName, out string moduleId)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Hosting", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputPath);

        var js = GetJavaScript(scriptName);
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

public interface IPromiseExports : IDisposable
{
    Task<int> ImmediateResolve(int value);

    Task<int> ImmediateReject(string reason);

    Task<int> ImmediateRejectError();

    Task<int> TimeoutResolve(int ms, int value);

    Task<int> TimeoutReject(int ms, string reason);
}
