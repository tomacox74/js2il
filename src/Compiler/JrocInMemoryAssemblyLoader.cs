using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Jroc;

public static class JrocInMemoryAssemblyLoader
{
    public static JrocLoadedAssembly Load(JrocCompiledAssemblyArtifact artifact)
    {
        ArgumentNullException.ThrowIfNull(artifact);
        ArgumentNullException.ThrowIfNull(artifact.PeBytes);

        var runtimeAssembly = typeof(JavaScriptRuntime.Object).Assembly;
        var loadContext = new SharedRuntimeAssemblyLoadContext(artifact.AssemblyName, runtimeAssembly);

        try
        {
            using var peStream = new MemoryStream(artifact.PeBytes, writable: false);
            Assembly assembly;
            if (artifact.PdbBytes is { Length: > 0 } pdbBytes)
            {
                using var pdbStream = new MemoryStream(pdbBytes, writable: false);
                assembly = loadContext.LoadFromStream(peStream, pdbStream);
            }
            else
            {
                assembly = loadContext.LoadFromStream(peStream);
            }

            // Force type resolution while the loader still owns the boundary so missing runtime
            // dependencies fail deterministically here instead of later at first use.
            _ = assembly.GetTypes();

            return new JrocLoadedAssembly(loadContext, assembly, artifact.ModuleIds);
        }
        catch
        {
            loadContext.Unload();
            throw;
        }
    }

    private sealed class SharedRuntimeAssemblyLoadContext(string assemblyName, Assembly sharedRuntimeAssembly)
        : AssemblyLoadContext($"JROC:{assemblyName}:{Guid.NewGuid():N}", isCollectible: true)
    {
        private readonly string _sharedRuntimeAssemblyName = sharedRuntimeAssembly.GetName().Name
            ?? throw new InvalidOperationException("JavaScript runtime assembly name is required for in-memory loading.");

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return string.Equals(assemblyName.Name, _sharedRuntimeAssemblyName, StringComparison.Ordinal)
                ? sharedRuntimeAssembly
                : null;
        }
    }
}
