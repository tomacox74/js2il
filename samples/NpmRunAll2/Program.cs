namespace NpmRunAll2;

internal static class Program
{
    private static void Main()
    {
        // The compiled assembly exposes two utilities from npm-run-all2:
        //   taskHeader(nameAndArgs) – formats a "> task" run header
        //   filterTasks(taskListCsv, patternsCsv) – glob-based task selection
        using var exports = global::NpmRunAll2Module.Import();

        // --- Task header formatting (npm-run-all2/lib/create-header) ---
        Console.WriteLine("=== task headers ===");
        Console.WriteLine(exports.TaskHeader("build"));
        Console.WriteLine(exports.TaskHeader("test:unit --reporter spec"));
        Console.WriteLine(exports.TaskHeader("lint"));

        // --- Pattern-based task filtering (npm-run-all2 glob rules) ---
        Console.WriteLine("\n=== pattern matching ===");
        string available = "build,test:unit,test:integration,test:e2e,lint,clean";

        PrintMatch("test:*");
        PrintMatch("lint");
        PrintMatch("build");
        PrintMatch("test:e2e");

        Console.WriteLine("done");

        void PrintMatch(string pattern)
        {
            var matched = exports.FilterTasks(available, pattern);
            Console.WriteLine($"  {pattern,-18} => [{matched}]");
        }
    }
}
