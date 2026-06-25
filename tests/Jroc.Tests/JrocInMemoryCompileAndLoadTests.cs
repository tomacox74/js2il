using Jroc.Runtime;
using System.Runtime.CompilerServices;

namespace Jroc.Tests;

public sealed class JrocInMemoryCompileAndLoadTests
{
    [Fact]
    public void CompileAndLoadModule_WithTypedExports_LoadsAndInvokesExportsWithoutDiskAssembly()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "typed-inmemory-module.js");

        using var module = JrocInMemoryCompiler.CompileAndLoadModule<ICalculatorExports>(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = "\"use strict\";\nexports.add = (left, right) => left + right;\n"
            });

        Assert.Equal(5d, module.Exports.add(2, 3));
        Assert.Equal("typed-inmemory-module", module.AssemblyName);
        Assert.Contains("typed-inmemory-module", module.ModuleIds, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompileAndLoadModule_WithTypedExportsAndEmitPdb_LoadsExportsWithoutDiskAssembly()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Jroc.Tests", "InMemoryCompileAndLoadPdb", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var entryPath = Path.Combine(tempRoot, "typed-inmemory-module.js");

            using var module = JrocInMemoryCompiler.CompileAndLoadModule<ICalculatorExports>(
                new JrocInMemoryCompileRequest(entryPath)
                {
                    SourceText = "\"use strict\";\nexports.add = (left, right) => left + right;\n",
                    EmitPdb = true
                });

            Assert.Equal(11d, module.Exports.add(5, 6));
            Assert.Equal("typed-inmemory-module", module.AssemblyName);
            Assert.Equal(string.Empty, module.Assembly.Location);
            Assert.Empty(Directory.EnumerateFileSystemEntries(tempRoot));
        }
        finally
        {
            try { Directory.Delete(tempRoot, recursive: true); } catch { }
        }
    }

    [Fact]
    public void CompileAndLoadModule_WithDynamicExports_UsesInferredModuleId()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "dynamic-inmemory-module.js");

        using var module = JrocInMemoryCompiler.CompileAndLoadModule(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = "\"use strict\";\nexports.answer = () => 42;\n"
            });

        dynamic exports = module.Exports;
        Assert.Equal(42d, (double)exports.answer());
        Assert.Equal("dynamic-inmemory-module", module.AssemblyName);
        Assert.Contains("dynamic-inmemory-module", module.ModuleIds, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompileAndLoadModule_Dispose_ClosesHostedAccessAndLoadBoundary()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "typed-inmemory-unload.js");
        var module = JrocInMemoryCompiler.CompileAndLoadModule<ICalculatorExports>(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = "\"use strict\";\nexports.add = (left, right) => left + right;\n"
            });

        Assert.Equal(9d, module.Exports.add(4, 5));

        module.Dispose();

        Assert.Throws<ObjectDisposedException>(() => _ = module.Exports);
        Assert.Throws<ObjectDisposedException>(() => _ = module.Assembly);
    }

    [Fact]
    public void CompileAndLoadModule_Dispose_AllowsCollectibleLoadContextToUnload()
    {
        var weakReference = LoadAndDisposeHostedModule();

        for (int i = 0; weakReference.IsAlive && i < 10; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.False(weakReference.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndDisposeHostedModule()
    {
        var entryPath = Path.Combine(Path.GetTempPath(), "typed-inmemory-hosted-unload.js");
        var module = JrocInMemoryCompiler.CompileAndLoadModule<ICalculatorExports>(
            new JrocInMemoryCompileRequest(entryPath)
            {
                SourceText = "\"use strict\";\nexports.add = (left, right) => left + right;\n"
            });

        try
        {
            Assert.Equal(17d, module.Exports.add(8, 9));
            return module.LoadContextWeakReference;
        }
        finally
        {
            module.Dispose();
        }
    }

    [JsModule("typed-inmemory-module")]
    public interface ICalculatorExports : IDisposable
    {
        double add(double left, double right);
    }
}
