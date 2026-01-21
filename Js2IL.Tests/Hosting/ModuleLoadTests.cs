using System.Reflection;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Js2IL.Tests.Hosting;

public class ModuleLoadTests
{
    private const string HostingJavaScriptResourcePrefix = "Js2IL.Tests.Hosting.JavaScript.";

    public interface IMathExports : IDisposable
    {
        string Version { get; }
        double Add(double x, double y);
    }

    private sealed class CompiledModuleAssembly : IDisposable
    {
        private readonly string _outputDir;
        private readonly string _uniqueAssemblyPath;
        private readonly AssemblyLoadContext _alc;

        public Assembly Assembly { get; }

        public CompiledModuleAssembly(string outputDir, string uniqueAssemblyPath, AssemblyLoadContext alc, Assembly assembly)
        {
            _outputDir = outputDir;
            _uniqueAssemblyPath = uniqueAssemblyPath;
            _alc = alc;
            Assembly = assembly;
        }

        public void Dispose()
        {
            _alc.Unload();
            try { File.Delete(_uniqueAssemblyPath); } catch { }
            try { Directory.Delete(_outputDir, recursive: true); } catch { }
        }
    }

    [Fact]
    public void JsEngine_LoadModule_AllowsCallingExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");
        using var exports = Js2IL.Runtime.JsEngine.LoadModule<IMathExports>(module.Assembly, "math");

        Assert.Equal("1.0.0", exports.Version);
        Assert.Equal(3.0, exports.Add(1, 2));
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsCallingExports()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("math", "math.js");

        using var exportsObj = (IDisposable)Js2IL.Runtime.JsEngine.LoadModule(module.Assembly, "math");
        dynamic exports = exportsObj;

        Assert.Equal("1.0.0", (string)exports.version);
        Assert.Equal(3.0, (double)exports.add(1, 2));
    }

    [Fact]
    public void JsEngine_GetModuleIds_ReturnsExpectedModuleIds()
    {
        using var module = CompileAndLoadModuleAssemblyFromResources(
            rootModuleName: "main",
            rootScriptResourcePath: "main.js",
            additionalFiles: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["calculator/index.js"] = "calculator/index.js"
            });

        var moduleIds = Js2IL.Runtime.JsEngine.GetModuleIds(module.Assembly);

        Assert.Equal(new[] { "calculator/index", "main" }, moduleIds);
    }

    [Fact]
    public void JsEngine_LoadModule_WhenModuleThrowsDuringInitialization_PropagatesException()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource("boom", "boom.js");

        var ex = Assert.ThrowsAny<Exception>(() => Js2IL.Runtime.JsEngine.LoadModule(module.Assembly, "boom"));
        Assert.Contains("boom", ex.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static string LoadHostingJavaScript(string resourcePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourcePath);

        var normalized = resourcePath.Trim().Replace('\\', '/');
        var resourceName = HostingJavaScriptResourcePrefix + normalized.Replace("/", ".");

        var assembly = typeof(ModuleLoadTests).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var candidates = assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith(HostingJavaScriptResourcePrefix, StringComparison.Ordinal))
                .Order(StringComparer.Ordinal)
                .ToArray();

            throw new InvalidOperationException(
                $"Could not find embedded resource '{resourceName}' for script '{resourcePath}'. " +
                $"Available Hosting JavaScript resources: {string.Join(", ", candidates)}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static CompiledModuleAssembly CompileAndLoadModuleAssemblyFromResource(string moduleName, string scriptResourcePath)
    {
        return CompileAndLoadModuleAssemblyFromResources(
            rootModuleName: moduleName,
            rootScriptResourcePath: scriptResourcePath,
            additionalFiles: new Dictionary<string, string>(StringComparer.Ordinal));
    }

    private static CompiledModuleAssembly CompileAndLoadModuleAssemblyFromResources(
        string rootModuleName,
        string rootScriptResourcePath,
        IReadOnlyDictionary<string, string> additionalFiles)
    {
        var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "ModuleLoad", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var rootJs = LoadHostingJavaScript(rootScriptResourcePath);

        var filePath = Path.Combine(outputDir, rootModuleName + ".js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(filePath, rootJs);

        foreach (var kvp in additionalFiles)
        {
            var fullPath = Path.Combine(outputDir, kvp.Key.Replace('/', Path.DirectorySeparatorChar));
            var content = LoadHostingJavaScript(kvp.Value);
            mockFs.AddFile(fullPath, content);
        }

        var options = new CompilerOptions { OutputDirectory = outputDir };
        var logger = new TestLogger();
        var sp = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = sp.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(filePath), logger.Errors);

        var compiledPath = Path.Combine(outputDir, rootModuleName + ".dll");
        Assert.True(File.Exists(compiledPath), $"Expected compiled output at '{compiledPath}'");

        var jsRuntimeAsm = typeof(EnvironmentProvider).Assembly;
        var uniquePath = Path.Combine(outputDir, rootModuleName + ".run-" + Guid.NewGuid().ToString("N") + ".dll");
        File.Copy(compiledPath, uniquePath, overwrite: true);

        var alc = new HostingTestAssemblyLoadContext(jsRuntimeAsm, outputDir);
        Assembly compiledAssembly;
        using (var stream = File.OpenRead(uniquePath))
        {
            compiledAssembly = alc.LoadFromStream(stream);
        }

        return new CompiledModuleAssembly(outputDir, uniquePath, alc, compiledAssembly);
    }

    private sealed class HostingTestAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly Assembly _jsRuntimeAssembly;
        private readonly string _baseDirectory;

        public HostingTestAssemblyLoadContext(Assembly jsRuntimeAssembly, string baseDirectory)
            : base(isCollectible: true)
        {
            _jsRuntimeAssembly = jsRuntimeAssembly;
            _baseDirectory = baseDirectory;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (string.Equals(assemblyName.Name, _jsRuntimeAssembly.GetName().Name, StringComparison.Ordinal))
            {
                return _jsRuntimeAssembly;
            }

            var candidatePath = Path.Combine(_baseDirectory, (assemblyName.Name ?? string.Empty) + ".dll");
            if (File.Exists(candidatePath))
            {
                return LoadFromAssemblyPath(candidatePath);
            }

            return null;
        }
    }
}
