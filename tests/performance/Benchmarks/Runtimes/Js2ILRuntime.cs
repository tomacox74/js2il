using System.Diagnostics;
using Js2IL;
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

            // Measure execution time
            var executeStopwatch = Stopwatch.StartNew();

            var dllPath = Path.Combine(tempDir, outputName, $"{outputName}.dll");
            if (!File.Exists(dllPath))
            {
                result.Success = false;
                result.Error = $"js2il compiled output not found: {dllPath}";
                return result;
            }

            // Execute the compiled assembly
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{dllPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                result.Error = "Failed to start dotnet process";
                return result;
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            executeStopwatch.Stop();

            if (process.ExitCode == 0)
            {
                result.Success = true;
                result.ExecutionTime = executeStopwatch.Elapsed;
                result.Output = output;
            }
            else
            {
                result.Success = false;
                result.Error = $"js2il execution failed with exit code {process.ExitCode}: {error}";
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
}
