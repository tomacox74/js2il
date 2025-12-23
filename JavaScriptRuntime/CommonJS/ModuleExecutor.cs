using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Executes the script entry point using CommonJS module semantics.
    /// </summary>
    public void Execute([NotNull] ModuleMainDelegate scriptEntryPoint)
    {
        ArgumentNullException.ThrowIfNull(scriptEntryPoint);

        var moduleContext = ModuleContext.CreateModuleContext(_serviceProvider);
        var requireService = _serviceProvider.Resolve<Require>();

        // Create a require delegate for the main module
        RequireDelegate mainRequire = (moduleId) =>
        {
            if (moduleId is not string moduleName || moduleName == null)
            {
                throw new TypeError("The \"id\" argument must be of type string.");
            }
            return requireService.RequireModule(moduleName);
        };

        // Create the main Module object
        // Main module has id of "." in Node.js, but we use the filename for consistency
        var mainModule = new Module(
            id: moduleContext.__filename.Length > 0 ? moduleContext.__filename : ".",
            filename: moduleContext.__filename,
            parent: null,  // Main module has no parent
            requireDelegate: mainRequire
        );

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
