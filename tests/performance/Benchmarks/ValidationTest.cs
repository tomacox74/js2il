using Benchmarks.Runtimes;

namespace Benchmarks;

/// <summary>
/// Validation test for runtime adapters.
/// </summary>
public static class ValidationTest
{
    public static void RunValidation()
    {
        // Test runtime adapters with minimal.js
        var script = "\"use strict\";\nvar x = 1 + 1 === 2;";
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        var scenarios = BenchmarkScenarioCatalog.LoadScenarios(scriptsDir);
        var requiredScenarios = new[]
        {
            "minimal",
            "dromaeo-object-array",
            "linq-js",
            "stopwatch-modern"
        };

        Console.WriteLine("Testing runtime adapters...\n");

        Console.WriteLine($"Discovered {scenarios.Count} BenchmarkDotNet scenarios.");
        foreach (var requiredScenario in requiredScenarios)
        {
            if (!scenarios.Any(scenario => string.Equals(scenario.Key, requiredScenario, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"Expected benchmark scenario '{requiredScenario}' was not found.");
            }
        }
        Console.WriteLine($"Verified representative scenario discovery: {string.Join(", ", requiredScenarios)}");

        // Test Jint
        Console.WriteLine("1. Testing Jint...");
        try
        {
            var jintRuntime = new JintRuntime();
            var jintResult = jintRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {jintResult.Success}");
            Console.WriteLine($"   Execution Time: {jintResult.ExecutionTime.TotalMilliseconds}ms");
            if (!jintResult.Success)
            {
                Console.WriteLine($"   Error: {jintResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        // Test ClearScript
        Console.WriteLine("\n2. Testing ClearScript...");
        try
        {
            var clearScriptRuntime = new ClearScriptRuntime();
            var clearScriptResult = clearScriptRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {clearScriptResult.Success}");
            Console.WriteLine($"   Execution Time: {clearScriptResult.ExecutionTime.TotalMilliseconds}ms");
            if (!clearScriptResult.Success)
            {
                Console.WriteLine($"   Error: {clearScriptResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        // Test jroc
        Console.WriteLine("\n3. Testing jroc...");
        try
        {
            var jrocRuntime = new JrocRuntime();
            var jrocResult = jrocRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {jrocResult.Success}");
            Console.WriteLine($"   Compile Time: {jrocResult.CompileTime?.TotalMilliseconds ?? 0}ms");
            Console.WriteLine($"   Execution Time: {jrocResult.ExecutionTime.TotalMilliseconds}ms");
            if (!jrocResult.Success)
            {
                Console.WriteLine($"   Error: {jrocResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        // Test Okojo
        Console.WriteLine("\n4. Testing Okojo...");
        try
        {
            var okojoRuntime = new OkojoRuntime();
            var okojoResult = okojoRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {okojoResult.Success}");
            Console.WriteLine($"   Execution Time: {okojoResult.ExecutionTime.TotalMilliseconds}ms");
            if (!okojoResult.Success)
            {
                Console.WriteLine($"   Error: {okojoResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        Console.WriteLine("\nAll runtime adapters tested!");
    }
}
