using System.Diagnostics;
using Jint;

namespace Benchmarks.Runtimes;

/// <summary>
/// Jint runtime adapter - JavaScript interpreter for .NET.
/// </summary>
public class JintRuntime : IJavaScriptRuntime
{
    public string Name => "Jint";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        try
        {
            var stopwatch = Stopwatch.StartNew();

            // Create a new Jint engine with strict mode
            var engine = new Engine(options => options.Strict());

            // Execute the script
            engine.Execute(scriptContent, scriptName);

            stopwatch.Stop();

            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"Jint execution failed: {ex.Message}";
        }

        return result;
    }
}
