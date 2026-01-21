using System.Reflection;

namespace Js2IL.Runtime;

public static class JsEngine
{
    /// <summary>
    /// Loads a module using metadata associated with <typeparamref name="TExports"/>.
    /// This requires <see cref="JsModuleAttribute"/> to be present on <typeparamref name="TExports"/>.
    /// </summary>
    public static TExports LoadModule<TExports>()
        where TExports : class
    {
        var contractType = typeof(TExports);
        var moduleAttr = contractType.GetCustomAttribute<JsModuleAttribute>();
        if (moduleAttr == null)
        {
            throw new InvalidOperationException(
                $"{contractType.FullName} does not have {nameof(JsModuleAttribute)}. " +
                $"Call {nameof(LoadModule)}<{contractType.Name}>(moduleId) or {nameof(LoadModule)}(compiledAssembly, moduleId) instead.");
        }

        return LoadModule<TExports>(contractType.Assembly, moduleAttr.ModuleId);
    }

    /// <summary>
    /// Loads a module by id, inferring the target compiled assembly from <typeparamref name="TExports"/>'s assembly.
    /// </summary>
    public static TExports LoadModule<TExports>(string moduleId)
        where TExports : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);
        return LoadModule<TExports>(typeof(TExports).Assembly, moduleId);
    }

    /// <summary>
    /// Dynamic / reflection-friendly form: returns a dynamic exports proxy (also <see cref="IDisposable"/>).
    /// </summary>
    public static object LoadModule(Assembly compiledAssembly, string moduleId)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        var runtime = new JsRuntimeInstance(compiledAssembly, moduleId);
        return new JsDynamicExports(runtime);
    }

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
