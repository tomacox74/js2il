using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Jroc.Runtime;

public static class JsEngine
{
    /// <summary>
    /// Returns the canonical entry module id recorded in a compiled JROC assembly.
    /// </summary>
    public static string GetEntryModuleId(Assembly compiledAssembly)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);

        var entryModules = compiledAssembly
            .GetCustomAttributes<JsCompiledEntryModuleAttribute>()
            .Select(attribute => attribute.ModuleId)
            .Where(moduleId => !string.IsNullOrWhiteSpace(moduleId))
            .ToArray();

        return entryModules.Length switch
        {
            1 => entryModules[0],
            0 => throw new InvalidOperationException(
                "The assembly does not contain JROC entry-module metadata."),
            _ => throw new InvalidOperationException(
                $"The assembly contains {entryModules.Length} entry-module declarations; exactly one is required.")
        };
    }

    /// <summary>
    /// Returns module ids present in a compiled JROC assembly.
    /// Prefer this over scanning types directly; compiled assemblies emitted by JROC include
    /// an assembly-level manifest via <see cref="JsCompiledModuleAttribute"/>.
    /// </summary>
    public static IReadOnlyList<string> GetModuleIds(Assembly compiledAssembly)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);

        var fromManifest = compiledAssembly
            .GetCustomAttributes<JsCompiledModuleAttribute>()
            .Select(a => a.ModuleId)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        if (fromManifest.Length != 0)
        {
            return fromManifest;
        }

        // Back-compat: older compiled assemblies won't have the manifest.
        // Fall back to listing type names in well-known namespaces.
        // NOTE: these are sanitized ids (e.g. "calculator_index") and may not match original path-like ids.
        return compiledAssembly
            .GetTypes()
            .Where(t => string.Equals(t.Namespace, "Modules", StringComparison.Ordinal) || string.Equals(t.Namespace, "Scripts", StringComparison.Ordinal))
            .Where(t => t.IsClass && !t.IsNested)
            .Select(t => t.Name)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>
    /// Loads a module using metadata associated with <typeparamref name="TExports"/>.
    /// This requires <see cref="JsModuleAttribute"/> to be present on <typeparamref name="TExports"/>.
    /// </summary>
    public static TExports LoadModule<TExports>()
        where TExports : class
    {
        var contractType = typeof(TExports);
        var moduleId = GeneratedContractMetadata.GetModuleId(contractType);
        if (moduleId == null)
        {
            throw new JsContractProjectionException(
                $"{contractType.FullName} does not have {nameof(JsModuleAttribute)}. " +
                $"Call {nameof(LoadModule)}<{contractType.Name}>(moduleId) or {nameof(LoadDynamicModule)}(compiledAssembly, moduleId) instead.",
                contractType: contractType);
        }

        return LoadModule<TExports>(contractType.Assembly, moduleId);
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
    /// Compatibility form: returns a dynamic exports proxy through its historical
    /// <see cref="IDisposable"/> return type.
    /// </summary>
    public static IDisposable LoadModule(Assembly compiledAssembly, string moduleId)
        => LoadModule(compiledAssembly, moduleId, options: null);

    /// <summary>
    /// Compatibility form: returns a dynamic exports proxy through its historical
    /// <see cref="IDisposable"/> return type.
    /// </summary>
    public static IDisposable LoadModule(
        Assembly compiledAssembly,
        string moduleId,
        JsModuleLoadOptions? options)
        => LoadDynamicModule(compiledAssembly, moduleId, options);

    /// <summary>
    /// Loads a module with a strongly typed dynamic/reflection-friendly exports surface.
    /// </summary>
    public static JsDynamicExports LoadDynamicModule(
        Assembly compiledAssembly,
        string moduleId)
        => LoadDynamicModule(compiledAssembly, moduleId, options: null);

    /// <summary>
    /// Loads a module with a strongly typed dynamic/reflection-friendly exports surface.
    /// </summary>
    public static JsDynamicExports LoadDynamicModule(
        Assembly compiledAssembly,
        string moduleId,
        JsModuleLoadOptions? options)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        try
        {
            var runtime = new JsRuntimeInstance(compiledAssembly, moduleId, options);
            return new JsDynamicExports(runtime);
        }
        catch (Exception ex)
        {
            var translated = JsHostingExceptionTranslator.TranslateModuleLoad(ex, compiledAssembly, moduleId);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }

    public static TExports LoadModule<TExports>(Assembly compiledAssembly, string moduleId)
        where TExports : class
        => LoadModule<TExports>(compiledAssembly, moduleId, options: null);

    public static TExports LoadModule<TExports>(Assembly compiledAssembly, string moduleId, JsModuleLoadOptions? options)
        where TExports : class
        => (TExports)LoadModule(typeof(TExports), compiledAssembly, moduleId, options);

    internal static object LoadModule(Type exportsContractType, Assembly compiledAssembly, string moduleId, JsModuleLoadOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(exportsContractType);
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        if (!exportsContractType.IsInterface)
        {
            throw new JsContractProjectionException(
                $"{exportsContractType.FullName} must be an interface generated by JROC.",
                moduleId: moduleId,
                contractType: exportsContractType,
                compiledAssemblyName: compiledAssembly.GetName().Name);
        }

        if (!typeof(IDisposable).IsAssignableFrom(exportsContractType))
        {
            throw new JsContractProjectionException(
                $"{exportsContractType.FullName} must implement IDisposable so the module runtime can be shut down deterministically.",
                moduleId: moduleId,
                contractType: exportsContractType,
                compiledAssemblyName: compiledAssembly.GetName().Name);
        }

        JsRuntimeInstance? runtime = null;
        try
        {
            runtime = new JsRuntimeInstance(compiledAssembly, moduleId, options);
            var proxy = DispatchProxy.Create(exportsContractType, typeof(JsExportsProxy));
            ((JsExportsProxy)(object)proxy).Initialize(runtime);
            runtime = null;
            return proxy;
        }
        catch (Exception ex)
        {
            runtime?.Dispose();
            var translated = JsHostingExceptionTranslator.TranslateModuleLoad(ex, compiledAssembly, moduleId, contractType: exportsContractType);
            ExceptionDispatchInfo.Capture(translated).Throw();
            throw;
        }
    }
}
