using System.Reflection;

namespace Js2IL.Runtime;

public static class JsEngine
{
    public static TExports LoadModule<TExports>(Assembly compiledAssembly, string moduleId)
        where TExports : class
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        if (!typeof(IDisposable).IsAssignableFrom(typeof(TExports)))
        {
            throw new NotSupportedException($"{typeof(TExports).FullName} must implement IDisposable so the module runtime can be shut down deterministically.");
        }

        var runtime = new JsRuntimeInstance(compiledAssembly, moduleId);
        var proxy = DispatchProxy.Create<TExports, JsExportsProxy>();
        ((JsExportsProxy)(object)proxy).Initialize(runtime);
        return proxy;
    }
}
