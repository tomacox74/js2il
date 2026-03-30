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

        Console.WriteLine("Testing runtime adapters...\n");

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

        // Test Node.js
        Console.WriteLine("\n2. Testing Node.js...");
        try
        {
            var nodeRuntime = new NodeJsRuntime();
            var nodeResult = nodeRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {nodeResult.Success}");
            Console.WriteLine($"   Execution Time: {nodeResult.ExecutionTime.TotalMilliseconds}ms");
            if (!nodeResult.Success)
            {
                Console.WriteLine($"   Error: {nodeResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        // Test js2il
        Console.WriteLine("\n3. Testing js2il...");
        try
        {
            var js2ilRuntime = new Js2ILRuntime();
            var js2ilResult = js2ilRuntime.Execute(script, "minimal.js");
            Console.WriteLine($"   Success: {js2ilResult.Success}");
            Console.WriteLine($"   Compile Time: {js2ilResult.CompileTime?.TotalMilliseconds ?? 0}ms");
            Console.WriteLine($"   Execution Time: {js2ilResult.ExecutionTime.TotalMilliseconds}ms");
            if (!js2ilResult.Success)
            {
                Console.WriteLine($"   Error: {js2ilResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
        }

        Console.WriteLine("\nAll runtime adapters tested!");
    }
}
