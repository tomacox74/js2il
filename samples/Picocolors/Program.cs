using Jroc.Runtime;
using System.Reflection;

namespace Picocolors;

internal static class Program
{
    private static void Main()
    {
        var compiledModulePath = Path.Combine(AppContext.BaseDirectory, "picocolors.dll");
        var asm = Assembly.LoadFrom(compiledModulePath);
        var moduleId = ResolvePicocolorsModuleId(asm);

        // module.exports from picocolors is the color-functions object itself.
        using dynamic pc = JsEngine.LoadModule(asm, moduleId);

        // Call a representative selection of picocolors color/style functions.
        // When ANSI color is supported the strings include ANSI escape codes;
        // when running without a TTY they are returned as plain text.
        string red    = Convert.ToString(pc.red("ERROR: something went wrong")) ?? string.Empty;
        string green  = Convert.ToString(pc.green("OK: all systems go"))        ?? string.Empty;
        string yellow = Convert.ToString(pc.yellow("WARN: check your config"))  ?? string.Empty;
        string cyan   = Convert.ToString(pc.cyan("INFO: picocolors via JROC"))  ?? string.Empty;
        string bold   = Convert.ToString(pc.bold("Bold text"))                  ?? string.Empty;

        Console.WriteLine($"red={red}");
        Console.WriteLine($"green={green}");
        Console.WriteLine($"yellow={yellow}");
        Console.WriteLine($"cyan={cyan}");
        Console.WriteLine($"bold={bold}");
        Console.WriteLine("done");
    }

    private static string ResolvePicocolorsModuleId(Assembly asm)
    {
        var moduleIds = JsEngine.GetModuleIds(asm);
        foreach (var candidate in moduleIds)
        {
            if (string.Equals(candidate, "picocolors", StringComparison.Ordinal))
            {
                return candidate;
            }
        }

        foreach (var candidate in moduleIds)
        {
            if (string.Equals(candidate, "picocolors/picocolors", StringComparison.Ordinal))
            {
                return candidate;
            }
        }

        if (moduleIds.Count > 0)
        {
            return moduleIds[0];
        }

        throw new InvalidOperationException("No compiled module IDs were found in picocolors.dll.");
    }
}
