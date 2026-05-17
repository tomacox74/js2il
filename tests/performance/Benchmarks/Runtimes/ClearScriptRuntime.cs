using System.Diagnostics;
using Microsoft.ClearScript.V8;

namespace Benchmarks.Runtimes;

/// <summary>
/// ClearScript runtime adapter - executes JavaScript through a hosted V8 engine in-process.
/// </summary>
public sealed class ClearScriptRuntime : IJavaScriptRuntime
{
    public string Name => "ClearScript";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        try
        {
            var stopwatch = Stopwatch.StartNew();

            using var engine = new V8ScriptEngine();
            engine.Execute(scriptName, scriptContent);

            stopwatch.Stop();

            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"ClearScript execution failed: {ex.Message}";
        }

        return result;
    }
}
