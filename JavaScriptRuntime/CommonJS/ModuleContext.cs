using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JavaScriptRuntime.DependencyInjection;

namespace JavaScriptRuntime.CommonJS;
public class ModuleContext
{
    /// <summary>
    /// temporary statics
    /// </summary>

    private readonly static ThreadLocal<string> dirname = new ThreadLocal<string>();
    private readonly static ThreadLocal<string> filename = new ThreadLocal<string>();

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

    static ModuleContext()
    {
            // Provide sensible defaults when running out-of-proc: resolve to the entry assembly path.
            try
            {
                var entry = Assembly.GetEntryAssembly();
                var file = entry?.Location;
                if (!string.IsNullOrEmpty(file))
                {
                    filename.Value = file!;
                    dirname.Value = System.IO.Path.GetDirectoryName(file!) ?? string.Empty;
                    // argv is resolved on-demand by Process.argv from the environment provider.
                }
            }
            catch
            {
                // Best-effort; leave defaults if anything goes wrong.
            }    
    }

    public static void SetModuleContext(string dir, string file)
    {
        dirname.Value = dir;
        filename.Value = file;
    }

    public static void ClearModuleContext()
    {
        dirname.Value = string.Empty;
        filename.Value = string.Empty;
    }

    public static ModuleContext CreateModuleContext([NotNull] ServiceContainer serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        var requireService = serviceProvider.Resolve<Require>();
        var context = new ModuleContext
        {
            require = CreateRequireDelegate(requireService),
            __dirname = dirname.Value!,
            __filename = filename.Value!
        };
        return context;
    }

    public static ModuleContext CreateModuleContext()
    {
        // Fallback for legacy call sites (e.g. Node.Process.argv). Prefer passing a container.
        return CreateModuleContext(JavaScriptRuntime.RuntimeServices.BuildServiceProvider());
    }

    public object? Exports { get; set; }

    public required string __filename;

    public required string __dirname;

    public required RequireDelegate require { get; set; }
}