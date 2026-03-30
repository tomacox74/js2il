namespace Benchmarks.Runtimes;

/// <summary>
/// Represents the result of executing a JavaScript benchmark in a runtime.
/// </summary>
public class RuntimeExecutionResult
{
    /// <summary>
    /// Time taken to compile the JavaScript (for AOT runtimes like js2il).
    /// Null for interpreted or JIT runtimes.
    /// </summary>
    public TimeSpan? CompileTime { get; set; }

    /// <summary>
    /// Time taken to execute the JavaScript.
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// Any output produced by the script.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    /// Whether the execution was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if execution failed.
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Interface for JavaScript runtime adapters.
/// </summary>
public interface IJavaScriptRuntime
{
    /// <summary>
    /// Name of the runtime (e.g., "Node.js", "Jint", "js2il").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Execute JavaScript code and return timing and result information.
    /// </summary>
    /// <param name="scriptContent">The JavaScript source code to execute.</param>
    /// <param name="scriptName">Optional name for the script (for diagnostics).</param>
    /// <returns>Execution result with timing information.</returns>
    RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js");
}
