using Jroc.Runtime;
using System.Reflection;

namespace NpmRunAll2;

internal static class Program
{
    private static void Main()
    {
        var compiledModulePath = Path.Combine(AppContext.BaseDirectory, "index.dll");
        var asm = Assembly.LoadFrom(compiledModulePath);

        // The compiled assembly exposes two utilities from npm-run-all2:
        //   taskHeader(nameAndArgs) – formats a "> task" run header
        //   filterTasks(taskListCsv, patternsCsv) – glob-based task selection
        using dynamic exports = JsEngine.LoadModule(asm, moduleId: "npm-run-all2-match");

        // --- Task header formatting (npm-run-all2/lib/create-header) ---
        Console.WriteLine("=== task headers ===");
        Console.WriteLine(Convert.ToString(exports.taskHeader("build")) ?? "");
        Console.WriteLine(Convert.ToString(exports.taskHeader("test:unit --reporter spec")) ?? "");
        Console.WriteLine(Convert.ToString(exports.taskHeader("lint")) ?? "");

        // --- Pattern-based task filtering (npm-run-all2 glob rules) ---
        Console.WriteLine("\n=== pattern matching ===");
        string available = "build,test:unit,test:integration,test:e2e,lint,clean";

        PrintMatch(exports, "test:*",   available);
        PrintMatch(exports, "lint",     available);
        PrintMatch(exports, "build",    available);
        PrintMatch(exports, "test:e2e", available);

        Console.WriteLine("done");
    }

    private static void PrintMatch(dynamic exports, string pattern, string available)
    {
        string matched = Convert.ToString(exports.filterTasks(available, pattern)) ?? string.Empty;
        Console.WriteLine($"  {pattern,-18} => [{matched}]");
    }
}
