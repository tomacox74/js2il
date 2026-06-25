using Jroc.Runtime;

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

    [JsModule("typed-inmemory-module")]
    public interface ICalculatorExports : IDisposable
    {
        double add(double left, double right);
    }
}
