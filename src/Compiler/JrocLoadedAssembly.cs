using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Jroc;

public sealed class JrocLoadedAssembly : IDisposable
{
    private Assembly? _assembly;
    private AssemblyLoadContext? _loadContext;
    private int _disposed;

    internal JrocLoadedAssembly(
        AssemblyLoadContext loadContext,
        Assembly assembly,
        IReadOnlyList<string> moduleIds)
    {
        _loadContext = loadContext;
        _assembly = assembly;
        ModuleIds = moduleIds.ToArray();
        LoadContextWeakReference = new WeakReference(loadContext);
    }

    public Assembly Assembly => _assembly ?? throw new ObjectDisposedException(nameof(JrocLoadedAssembly));

    public IReadOnlyList<string> ModuleIds { get; }

    public WeakReference LoadContextWeakReference { get; }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _assembly = null;
        var loadContext = Interlocked.Exchange(ref _loadContext, null);
        loadContext?.Unload();
    }
}
