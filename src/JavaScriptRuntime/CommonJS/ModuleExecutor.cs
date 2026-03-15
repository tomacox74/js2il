using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Js2IL.Runtime;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime.CommonJS;

/// <summary>
/// Executes JavaScript code using the CommonJS module system.
/// </summary>
internal sealed class ModuleExecutor
{
    private readonly ServiceContainer _serviceProvider;

    public ModuleExecutor(ServiceContainer serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static bool ShouldPreserveRawEntryRequire(Require requireService, string requestedSpecifier)
    {
        var normalized = requestedSpecifier.Trim().Replace('\\', '/');
        if (!normalized.StartsWith("./", StringComparison.Ordinal) && !normalized.StartsWith("../", StringComparison.Ordinal))
        {
            return false;
        }

        var hasParentTraversal = normalized
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Any(segment => string.Equals(segment, "..", StringComparison.Ordinal));
        if (!hasParentTraversal)
        {
            return false;
        }

        return requireService.CanResolveLocalModule(requestedSpecifier);
    }

    private static string ResolveMainModuleId(ModuleMainDelegate scriptEntryPoint, string fallbackModuleId)
    {
        var declaringTypeName = scriptEntryPoint.Method.DeclaringType?.FullName;
        if (string.IsNullOrWhiteSpace(declaringTypeName))
        {
            return fallbackModuleId;
        }

        var canonicalModuleId = scriptEntryPoint.Method.Module.Assembly
            .GetCustomAttributes<JsCompiledModuleTypeAttribute>()
            .Where(attr => string.Equals(attr.TypeName, declaringTypeName, StringComparison.Ordinal))
            .Select(attr => attr.CanonicalModuleId)
            .FirstOrDefault(id => !string.IsNullOrWhiteSpace(id));

        return string.IsNullOrWhiteSpace(canonicalModuleId) ? fallbackModuleId : canonicalModuleId;
    }

    /// <summary>
    /// Executes the script entry point using CommonJS module semantics.
    /// </summary>
    public void Execute([NotNull] ModuleMainDelegate scriptEntryPoint)
    {
        ArgumentNullException.ThrowIfNull(scriptEntryPoint);

        var moduleContext = ModuleContext.CreateModuleContext(_serviceProvider);
        var requireService = _serviceProvider.Resolve<Require>();

        var fallbackMainModuleId = ".";
        var mainModuleId = ResolveMainModuleId(scriptEntryPoint, fallbackMainModuleId);

        // Create a require delegate for the main module
        RequireDelegate mainRequire = (moduleId) =>
        {
            if (moduleId is not string moduleName || moduleName == null)
            {
                throw new TypeError("The \"id\" argument must be of type string.");
            }

            // Entry wrappers can intentionally keep parent-traversal specifiers as the published
            // local module ids (for example the packed canary shims under scripts/differential-test).
            // Preserve those raw ids when they already resolve in the compiled local-module manifest;
            // otherwise keep the canonical module-id-relative behavior needed for package roots.
            return ShouldPreserveRawEntryRequire(requireService, moduleName)
                ? requireService.RequireModule(moduleName)
                : requireService.RequireModuleFrom(mainModuleId, moduleName);
        };

        // Create the main Module object
        // Main module has id of "." in Node.js, but we use the filename for consistency
        var mainModule = new Module(
            id: mainModuleId,
            filename: moduleContext.__filename,
            parent: null,  // Main module has no parent
            requireDelegate: mainRequire
        );

        // Node semantics: require.main is the entry module.
        requireService.SetMainModule(mainModule);
        JavaScriptRuntime.ObjectRuntime.SetProperty(mainRequire, "main", mainModule);
        RuntimeServices.RegisterModuleRequire(mainModule.id, mainRequire);
        if (!string.Equals(mainModule.filename, mainModule.id, StringComparison.OrdinalIgnoreCase))
        {
            RuntimeServices.RegisterModuleRequire(mainModule.filename, mainRequire);
        }

        // Set the main module as the current parent for require() calls
        requireService.SetCurrentParent(mainModule);

        // Invoke script with module parameters
        // exports is initially the same object as module.exports
        // Parameters: exports, require, module, __filename, __dirname
        scriptEntryPoint(mainModule.exports, mainRequire, mainModule, moduleContext.__filename, moduleContext.__dirname);

        // Mark main module as loaded
        mainModule.MarkLoaded();
    }
}
