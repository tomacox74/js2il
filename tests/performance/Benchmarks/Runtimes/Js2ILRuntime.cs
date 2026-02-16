using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using Js2IL;
using Js2IL.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks.Runtimes;

/// <summary>
/// js2il runtime adapter - compiles JavaScript to .NET IL and executes.
/// Separates compile time from execution time.
/// </summary>
public class Js2ILRuntime : IJavaScriptRuntime
{
    public string Name => "js2il";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        // Create temp directory for compilation output
        var tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmark-{Guid.NewGuid()}");
        var tempScriptFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");
        var outputName = "benchmark";

        try
        {
            Directory.CreateDirectory(tempDir);

            // Write script to temp file
            File.WriteAllText(tempScriptFile, scriptContent);

            // Measure compilation time
            var compileStopwatch = Stopwatch.StartNew();

            try
            {
                // Build compiler with service provider
                var options = new CompilerOptions { OutputDirectory = tempDir };
                var serviceProvider = CompilerServices.BuildServiceProvider(options);
                var compiler = serviceProvider.GetRequiredService<Compiler>();

                // Compile the JavaScript
                if (!compiler.Compile(tempScriptFile, outputName))
                {
                    result.Success = false;
                    result.Error = "js2il compilation failed";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = $"js2il compilation failed: {ex.Message}";
                return result;
            }

            compileStopwatch.Stop();
            result.CompileTime = compileStopwatch.Elapsed;

            // Find the generated DLL (it's named after the input file, not the outputName parameter)
            var dllFiles = Directory.GetFiles(tempDir, "*.dll", SearchOption.TopDirectoryOnly)
                .Where(f => !f.Contains("JavaScriptRuntime"))  // Exclude the runtime DLL
                .ToArray();
            
            if (dllFiles.Length == 0)
            {
                result.Success = false;
                result.Error = $"js2il compiled output not found in: {tempDir}";
                return result;
            }

            var dllPath = dllFiles[0];

            var moduleLoadContext = new AssemblyLoadContext($"js2il-benchmark-runtime-{Guid.NewGuid():N}", isCollectible: true);

            try
            {
                var assembly = moduleLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
                var moduleId = ResolveModuleId(assembly, outputName);

                // Measure execution time
                var executeStopwatch = Stopwatch.StartNew();
                using var exports = JsEngine.LoadModule(assembly, moduleId);
                executeStopwatch.Stop();

                result.Success = true;
                result.ExecutionTime = executeStopwatch.Elapsed;
                result.Output = string.Empty;
            }
            finally
            {
                moduleLoadContext.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"js2il execution failed: {ex.Message}";
        }
        finally
        {
            // Clean up temp files
            try
            {
                if (File.Exists(tempScriptFile))
                {
                    File.Delete(tempScriptFile);
                }
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        return result;
    }

    private static string ResolveModuleId(Assembly assembly, string fallback)
    {
        var moduleIds = assembly
            .GetCustomAttributes<JsCompiledModuleAttribute>()
            .Select(a => a.ModuleId)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (moduleIds.Length == 0)
        {
            return fallback;
        }

        if (moduleIds.Contains(fallback, StringComparer.Ordinal))
        {
            return fallback;
        }

        return moduleIds[0];
    }
}
