using System.Diagnostics;
using Okojo;
using Okojo.Runtime;

namespace Benchmarks.Runtimes;

/// <summary>
/// Okojo runtime adapter - executes JavaScript through the managed Okojo runtime.
/// </summary>
public sealed class OkojoRuntime : IJavaScriptRuntime
{
    public string Name => "Okojo";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        try
        {
            var stopwatch = Stopwatch.StartNew();

            using var runtime = JsRuntime.CreateBuilder().Build();
            runtime.MainRealm.Evaluate(scriptContent);

            stopwatch.Stop();

            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = true;
            result.Output = string.Empty;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"Okojo execution failed: {ex.Message}";
        }

        return result;
    }
}
