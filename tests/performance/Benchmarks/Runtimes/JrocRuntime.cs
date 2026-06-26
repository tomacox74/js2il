using System.Diagnostics;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks.Runtimes;

/// <summary>
/// jroc runtime adapter - compiles JavaScript to .NET IL in memory and executes.
/// Separates compile time from execution time.
/// </summary>
public class JrocRuntime : IJavaScriptRuntime
{
    public string Name => "jroc";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        // Use a stable fake path; SourceText overlay means the file need not exist on disk.
        var entryPath = Path.Combine(Path.GetTempPath(), scriptName);
        var request = new JrocInMemoryCompileRequest(entryPath)
        {
            SourceText = scriptContent
        };

        JrocCompiledAssemblyArtifact artifact;

        // Measure compilation time
        var compileStopwatch = Stopwatch.StartNew();
        try
        {
            artifact = JrocInMemoryCompiler.Compile(request);
        }
        catch (Exception ex)
        {
            result.Error = $"jroc compilation failed: {ex.Message}";
            return result;
        }
        finally
        {
            compileStopwatch.Stop();
        }

        result.CompileTime = compileStopwatch.Elapsed;

        // Load the compiled assembly from memory and execute
        var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);
        try
        {
            var moduleId = artifact.ModuleIds[0];

            var executeStopwatch = Stopwatch.StartNew();
            using var exports = JsEngine.LoadModule(loadedAssembly.Assembly, moduleId);
            executeStopwatch.Stop();

            result.Success = true;
            result.ExecutionTime = executeStopwatch.Elapsed;
            result.Output = string.Empty;
        }
        catch (Exception ex)
        {
            result.Error = $"jroc execution failed: {ex.Message}";
        }
        finally
        {
            loadedAssembly.Dispose();
        }

        return result;
    }
}
