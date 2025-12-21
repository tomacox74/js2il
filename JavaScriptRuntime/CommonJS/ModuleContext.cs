using System.Reflection;

namespace JavaScriptRuntime.CommonJS;
public class ModuleContext
{
    /// <summary>
    /// temporary statics
    /// </summary>
    private static string dirname;
    private static string filename;

    private static readonly Func<object?, object?> defaultRequire = (object? id) =>
    {
        if (id is not string moduleName || id == null)
        {
            throw new TypeError("The \"id\" argument must be of type string.");
        }

        return Require.require(moduleName);
    };

    static ModuleContext()
    {
            // Provide sensible defaults when running out-of-proc: resolve to the entry assembly path.
            try
            {
                var entry = Assembly.GetEntryAssembly();
                var file = entry?.Location;
                if (!string.IsNullOrEmpty(file))
                {
                    dirname = file!;
                    dirname = System.IO.Path.GetDirectoryName(file!) ?? string.Empty;
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
        dirname = dir;
        filename = file;
    }

    public static void ClearModuleContext()
    {
        dirname = string.Empty;
        filename = string.Empty;
    }

    public static ModuleContext CreateModuleContext()
    {
        var context = new ModuleContext
        {
            require = defaultRequire,
            __dirname = dirname,
            __filename = filename
        };
        return context;
    }

    public object? Exports { get; set; }

    public required string __filename;

    public required string __dirname;

    public required Func<object?, object?> require { get; set; }
}