using System.Reflection;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Js2IL.Services;
using Js2IL.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Js2IL.Tests.Hosting;

public class GeneratedContractTests
{
    private const string HostingJavaScriptResourcePrefix = "Js2IL.Tests.Hosting.JavaScript.";

    [Fact]
    public void GeneratedContracts_Enable_LoadModule_NoArgs_And_InvokeMembers()
    {
        using var module = CompileAndLoadModuleAssemblyFromResource(
            rootModuleName: "hosting",
            scriptResourcePath: "Hosting_TypedExports.js");

        // Entry-module exports contract naming per docs/DotNetLibraryHosting.md:
        //   Js2IL.<AssemblyName>.I<AssemblyName>Exports
        var contractType = module.Assembly.GetType("Js2IL.hosting.IHostingExports", throwOnError: true)!;

        var attr = contractType.GetCustomAttribute<Js2IL.Runtime.JsModuleAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("hosting", attr!.ModuleId);

        var loadNoArgs = typeof(Js2IL.Runtime.JsEngine)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == nameof(Js2IL.Runtime.JsEngine.LoadModule)
                      && m.IsGenericMethodDefinition
                      && m.GetParameters().Length == 0);

        var exportsObj = loadNoArgs.MakeGenericMethod(contractType).Invoke(null, null);
        Assert.NotNull(exportsObj);

        using var exports = (IDisposable)exportsObj!;

        var version = (string)contractType.GetProperty("Version")!.GetValue(exportsObj)!;
        Assert.Equal("1.2.3", version);

        var add = contractType.GetMethod("Add")!;
        var sum = add.Invoke(exportsObj, new object?[] { 1.0, 2.0 });
        Assert.Equal(3.0, Convert.ToDouble(sum));

        // Exported class -> IJsConstructor<T> property + instance handle interface
        var counterCtor = contractType.GetProperty("Counter")!.GetValue(exportsObj);
        Assert.NotNull(counterCtor);

        var construct = counterCtor!.GetType().GetMethod("Construct")!;
        var counterObj = construct.Invoke(counterCtor, new object?[] { new object?[] { 10.0 } });
        Assert.NotNull(counterObj);

        var counterType = module.Assembly.GetType("Js2IL.hosting.ICounter", throwOnError: true)!;

        var addDelta = counterType.GetMethod("Add")!;
        var newValue = addDelta.Invoke(counterObj, new object?[] { 5.0 });
        Assert.Equal(15.0, Convert.ToDouble(newValue));

        var valueProp = counterType.GetProperty("Value");
        if (valueProp != null)
        {
            var current = valueProp.GetValue(counterObj);
            Assert.Equal(15.0, Convert.ToDouble(current));
        }
        else
        {
            var getValue = counterType.GetMethod("GetValue")!;
            var current = getValue.Invoke(counterObj, System.Array.Empty<object?>());
            Assert.Equal(15.0, Convert.ToDouble(current));
        }

        // Dispose the handle proxy and ensure it becomes unusable
        ((IDisposable)counterObj!).Dispose();
        var tie = Assert.Throws<TargetInvocationException>(() => addDelta.Invoke(counterObj, new object?[] { 1.0 }));
        Assert.IsType<ObjectDisposedException>(tie.InnerException);
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
            if (_alc.IsCollectible)
            {
                _alc.Unload();
            }
            try { File.Delete(_uniqueAssemblyPath); } catch { }
            try { Directory.Delete(_outputDir, recursive: true); } catch { }
        }
    }

    private static string LoadHostingJavaScript(string resourcePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourcePath);

        var normalized = resourcePath.Trim().Replace('\\', '/');
        var resourceName = HostingJavaScriptResourcePrefix + normalized.Replace("/", ".");

        var assembly = typeof(GeneratedContractTests).Assembly;
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

    private static CompiledModuleAssembly CompileAndLoadModuleAssemblyFromResource(string rootModuleName, string scriptResourcePath)
    {
        var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "GeneratedContracts", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var js = LoadHostingJavaScript(scriptResourcePath);

        var filePath = Path.Combine(outputDir, rootModuleName + ".js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(filePath, js);

        var options = new CompilerOptions { OutputDirectory = outputDir, GenerateModuleExportContracts = true };
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
            : base(isCollectible: false)
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

            var candidatePath = Path.Combine(_baseDirectory, (assemblyName.Name ?? "") + ".dll");
            if (File.Exists(candidatePath))
            {
                return LoadFromAssemblyPath(candidatePath);
            }

            return null;
        }
    }
}
