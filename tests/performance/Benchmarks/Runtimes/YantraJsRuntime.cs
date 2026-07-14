using System.Diagnostics;
using YantraJS.Core;

namespace Benchmarks.Runtimes;

/// <summary>
/// YantraJS runtime adapter - evaluates JavaScript in a fresh YantraJS context.
/// </summary>
public sealed class YantraJsRuntime : IJavaScriptRuntime
{
    public string Name => "YantraJS";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var context = new JSContext();
            context.Eval(scriptContent, scriptName);
            stopwatch.Stop();

            result.ExecutionTime = stopwatch.Elapsed;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Error = $"YantraJS execution failed: {ex.Message}";
        }

        return result;
    }
}
