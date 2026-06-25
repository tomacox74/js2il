using System.Reflection;
using System.Threading;

namespace Jroc;

public class JrocInMemoryModule : IDisposable
{
    private object? _exports;
    private IDisposable? _exportsDisposable;
    private JrocLoadedAssembly? _loadedAssembly;
    private int _disposed;

    internal JrocInMemoryModule(JrocLoadedAssembly loadedAssembly, object exports, IDisposable exportsDisposable)
    {
        _loadedAssembly = loadedAssembly;
        _exports = exports;
        _exportsDisposable = exportsDisposable;
    }

    public object Exports => _exports ?? throw new ObjectDisposedException(nameof(JrocInMemoryModule));

    public Assembly Assembly => LoadedAssembly.Assembly;

    public string AssemblyName => Assembly.GetName().Name ?? string.Empty;

    public IReadOnlyList<string> ModuleIds => LoadedAssembly.ModuleIds;

    public WeakReference LoadContextWeakReference => LoadedAssembly.LoadContextWeakReference;

    protected JrocLoadedAssembly LoadedAssembly => _loadedAssembly ?? throw new ObjectDisposedException(nameof(JrocInMemoryModule));

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Exception? disposeException = null;

        try
        {
            _exportsDisposable?.Dispose();
        }
        catch (Exception ex)
        {
            disposeException = ex;
        }
        finally
        {
            _exportsDisposable = null;
            _exports = null;
        }

        try
        {
            _loadedAssembly?.Dispose();
        }
        finally
        {
            _loadedAssembly = null;
        }

        if (disposeException is not null)
        {
            throw disposeException;
        }
    }
}
