using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime.Modules.CommonJS;

public class ModuleContext
{
    private static readonly string DefaultFilename = GetDefaultFilename();
    private static readonly string DefaultDirectory =
        Path.GetDirectoryName(DefaultFilename) ?? string.Empty;

    private static RequireDelegate CreateRequireDelegate(Require requireService)
    {
        return (object? id) =>
        {
            if (id is not string moduleName || id == null)
            {
                throw new TypeError("The \"id\" argument must be of type string.");
            }

            return requireService.RequireModule(moduleName);
        };
    }

    public static void SetModuleContext(string dir, string file)
    {
        var context = RuntimeExecutionContext.CurrentOrOverride
            ?? throw new InvalidOperationException(
                "A runtime execution context is required to set module location.");
        context.SetModuleLocation(dir, file);
    }

    public static void ClearModuleContext()
    {
        RuntimeExecutionContext.CurrentOrOverride?.SetModuleLocation(
            string.Empty,
            string.Empty);
    }

    public static ModuleContext CreateModuleContext([NotNull] ServiceContainer serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        var requireService = serviceProvider.Resolve<Require>();
        var executionContext = RuntimeExecutionContext.GetOrCreate(serviceProvider);
        var (directory, filename) = executionContext.GetModuleLocation();
        var context = new ModuleContext
        {
            require = CreateRequireDelegate(requireService),
            __dirname = string.IsNullOrEmpty(directory)
                ? DefaultDirectory
                : directory,
            __filename = string.IsNullOrEmpty(filename)
                ? DefaultFilename
                : filename
        };
        return context;
    }

    public static ModuleContext CreateModuleContext()
    {
        var services = RuntimeExecutionContext.CurrentOrOverride?.Services
            ?? JavaScriptRuntime.RuntimeServices.BuildServiceProvider();
        return CreateModuleContext(services);
    }

    private static string GetDefaultFilename()
    {
        try
        {
            return Assembly.GetEntryAssembly()?.Location ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public object? Exports { get; set; }

    public required string __filename;

    public required string __dirname;

    public required RequireDelegate require { get; set; }
}