using System.Runtime.Loader;

namespace Jroc.Tests;

public sealed class JrocInMemoryAssemblyLoaderTests
{
    [Fact]
    public void Load_WithCompiledArtifact_LoadsIntoCollectibleContext_AndSharesRuntimeAssembly()
    {
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(Path.Combine(Path.GetTempPath(), "loader-entry.js"))
        {
            SourceText = "\"use strict\";\nmodule.exports = 123;\n",
            EmitPdb = true
        });

        using var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);

        Assert.Equal(artifact.AssemblyName, loadedAssembly.Assembly.GetName().Name);
        Assert.Equal(artifact.ModuleIds, loadedAssembly.ModuleIds);

        var loadContext = AssemblyLoadContext.GetLoadContext(loadedAssembly.Assembly);
        Assert.NotNull(loadContext);
        Assert.True(loadContext!.IsCollectible);

        var runtimeAssemblyName = typeof(JavaScriptRuntime.Object).Assembly.GetName().Name;
        Assert.DoesNotContain(
            loadContext.Assemblies,
            assembly => !ReferenceEquals(assembly, loadedAssembly.Assembly)
                && string.Equals(assembly.GetName().Name, runtimeAssemblyName, StringComparison.Ordinal));
    }

    [Fact]
    public void Dispose_AllowsLoadContextToBecomeCollectible()
    {
        var weakReference = LoadAndDispose();

        for (int i = 0; weakReference.IsAlive && i < 10; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Assert.False(weakReference.IsAlive);
    }

    private static WeakReference LoadAndDispose()
    {
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(Path.Combine(Path.GetTempPath(), "loader-unload-entry.js"))
        {
            SourceText = "\"use strict\";\nmodule.exports = 456;\n"
        });

        using var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);
        return loadedAssembly.LoadContextWeakReference;
    }
}
