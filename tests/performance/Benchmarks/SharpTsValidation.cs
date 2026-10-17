using Benchmarks.Runtimes;

namespace Benchmarks;

internal static class SharpTsValidation
{
    public static void Run()
    {
        // A module initializer can appear to work but become a no-op on reuse.
        // Verify real execution, persistent callable state, and reset on Main.
        using (var script = new SharpTsCompiledScript("""
            var count = 0;
            function next() { count++; return count === 1 ? 'first' : 'again'; }
            """, "lifecycle.js"))
        {
            var context = SynchronizationContext.Current;
            script.Execute();
            var next = script.GetFunction("next");
            Require(next().ToString() == "first", "Initial state was not initialized.");
            Require(next().ToString() == "again", "Callable state did not persist.");
            script.Execute();
            Require(next().ToString() == "first", "Repeated execution skipped the script body.");
            Require(ReferenceEquals(SynchronizationContext.Current, context), "Synchronization context leaked.");
        }

        var dromaeo = new DromaeoExecutionBenchmarks();
        foreach (var scenario in dromaeo.ScriptNames())
        {
            dromaeo.ScriptName = scenario;
            try
            {
                dromaeo.SetupSharpTs();
                dromaeo.SharpTs_ExecuteOnly();
                dromaeo.SharpTs_ExecuteOnly();
                Console.WriteLine($"SharpTS passed: {scenario}");
            }
            finally
            {
                dromaeo.CleanupSharpTs();
            }
        }

        // The remaining Kraken fixtures currently generate invalid IL in SharpTS
        // 1.0.12 and are exposed only through --comprehensive.
        var kracken = new KrackenExecutionBenchmarks { ScriptName = "kracken-json-parse-financial.js" };
        try
        {
            kracken.SetupSharpTs();
            kracken.RunSharpTsTest();
            kracken.RunSharpTsTest();
            Console.WriteLine($"SharpTS passed: {kracken.ScriptName}");
        }
        finally
        {
            kracken.CleanupSharpTs();
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
