using System.Reflection;
using System.Runtime.Loader;
using JavaScriptRuntime;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Js2IL.Tests;

public class LibraryHostingTests
{
    public interface IMathExports : IDisposable
    {
        string Version { get; }
        double Add(double x, double y);
    }

    [Fact]
    public void JsEngine_LoadModule_AllowsCallingExports()
    {
        var js = @"
const version = ""1.0.0"";
function add(x, y) {
  return x + y;
}
module.exports = { version, add };
";

        var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "LibraryHosting", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var filePath = Path.Combine(outputDir, "math.js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(filePath, js);

        var options = new CompilerOptions { OutputDirectory = outputDir };
        var logger = new TestLogger();
        var sp = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = sp.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(filePath), logger.Errors);

        var compiledPath = Path.Combine(outputDir, "math.dll");
        Assert.True(File.Exists(compiledPath), $"Expected compiled output at '{compiledPath}'");

        var jsRuntimeAsm = typeof(EnvironmentProvider).Assembly;
        var uniquePath = Path.Combine(outputDir, "math.run-" + Guid.NewGuid().ToString("N") + ".dll");
        File.Copy(compiledPath, uniquePath, overwrite: true);

        var alc = new HostingTestAssemblyLoadContext(jsRuntimeAsm, outputDir);
        try
        {
            Assembly compiledAssembly;
            using (var stream = File.OpenRead(uniquePath))
            {
                compiledAssembly = alc.LoadFromStream(stream);
            }

            using var exports = Js2IL.Runtime.JsEngine.LoadModule<IMathExports>(compiledAssembly, "math");

            Assert.Equal("1.0.0", exports.Version);
            Assert.Equal(3.0, exports.Add(1, 2));
        }
        finally
        {
            alc.Unload();
            try { File.Delete(uniquePath); } catch { }
        }
    }

    [Fact]
    public void JsEngine_LoadModule_Dynamic_AllowsCallingExports()
    {
        var js = @"
    const version = ""1.0.0"";
function add(x, y) {
  return x + y;
}
module.exports = { version, add };
";

        var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "LibraryHosting", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDir);

        var filePath = Path.Combine(outputDir, "math.js");
        var mockFs = new MockFileSystem();
        mockFs.AddFile(filePath, js);

        var options = new CompilerOptions { OutputDirectory = outputDir };
        var logger = new TestLogger();
        var sp = CompilerServices.BuildServiceProvider(options, mockFs, logger);
        var compiler = sp.GetRequiredService<Compiler>();

        Assert.True(compiler.Compile(filePath), logger.Errors);

        var compiledPath = Path.Combine(outputDir, "math.dll");
        Assert.True(File.Exists(compiledPath), $"Expected compiled output at '{compiledPath}'");

        var jsRuntimeAsm = typeof(EnvironmentProvider).Assembly;
        var uniquePath = Path.Combine(outputDir, "math.run-" + Guid.NewGuid().ToString("N") + ".dll");
        File.Copy(compiledPath, uniquePath, overwrite: true);

        var alc = new HostingTestAssemblyLoadContext(jsRuntimeAsm, outputDir);
        try
        {
            Assembly compiledAssembly;
            using (var stream = File.OpenRead(uniquePath))
            {
                compiledAssembly = alc.LoadFromStream(stream);
            }

            using var exportsObj = (IDisposable)Js2IL.Runtime.JsEngine.LoadModule(compiledAssembly, "math");
            dynamic exports = exportsObj;

            Assert.Equal("1.0.0", (string)exports.version);
            Assert.Equal(3.0, (double)exports.add(1, 2));
        }
        finally
        {
            alc.Unload();
            try { File.Delete(uniquePath); } catch { }
        }
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
