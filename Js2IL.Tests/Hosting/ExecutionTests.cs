using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Js2IL.Tests;
using Js2IL.Runtime;
using Js2IL.Services;
using JavaScriptRuntime;
using Microsoft.Extensions.DependencyInjection;

namespace Js2IL.Tests.Hosting;

public class ExecutionTests
{
    [Fact]
    public void TypedExports_PropertyAndFunctionCall()
    {
        using var exports = LoadExports(out _);

        Assert.Equal("1.2.3", exports.Version);
        Assert.Equal(3, exports.Add(1, 2));
    }

    [Fact]
    public async Task TypedExports_CrossThreadCall_Marshals()
    {
        using var exports = LoadExports(out _);

        var result = await Task.Run(() => exports.Add(4, 6));

        Assert.Equal(10, result);
    }

    [Fact]
    public void TypedExports_InvocationError_Propagates()
    {
        using var exports = LoadExports(out _);

        var ex = Assert.Throws<JsInvocationException>(() => exports.Fail());
        var jsError = Assert.IsType<JsErrorException>(ex.InnerException);
        _ = Assert.IsType<Error>(jsError.InnerException);
    }

    [Fact]
    public void TypedExports_Dispose_PreventsFurtherCalls()
    {
        var exports = LoadExports(out _);
        exports.Dispose();

        Assert.Throws<ObjectDisposedException>(() => exports.Add(1, 1));
    }

    [Fact]
    public void Handles_ConstructAndDispose_InstanceCalls()
    {
        using var exports = LoadExports(out _);
        using var constructor = exports.Counter;

        var counter = constructor.Construct(10);
        var created = exports.CreateCounter(2);

        Assert.Equal(15, counter.Add(5));
        Assert.Equal(15, counter.GetValue());
        Assert.Equal(15, counter.Value);

        Assert.Equal(3, created.Add(1));

        counter.Dispose();
        created.Dispose();

        Assert.Throws<ObjectDisposedException>(() => counter.Add(1));
        Assert.Throws<ObjectDisposedException>(() => created.Add(1));
    }

    private static IHostingExports LoadExports(out string moduleId)
    {
        var assembly = CompileModule("Hosting_TypedExports", out moduleId);
        return JsEngine.LoadModule<IHostingExports>(assembly, moduleId);
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

public interface IHostingExports : IDisposable
{
    string Version { get; }

    double Add(double x, double y);

    void Fail();

    IJsConstructor<ICounter> Counter { get; }

    ICounter CreateCounter(double start);
}

public interface ICounter : IJsHandle
{
    double Add(double delta);

    double GetValue();

    double Value { get; }
}
