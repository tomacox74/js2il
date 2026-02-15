using System.Diagnostics;

namespace Benchmarks.Runtimes;

/// <summary>
/// Node.js runtime adapter - executes JavaScript using Node.js process.
/// </summary>
public class NodeJsRuntime : IJavaScriptRuntime
{
    public string Name => "Node.js";

    public RuntimeExecutionResult Execute(string scriptContent, string scriptName = "script.js")
    {
        var result = new RuntimeExecutionResult { Success = false };

        // Create a temporary file for the script
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");

        try
        {
            // Write script to temp file
            File.WriteAllText(tempFile, scriptContent);

            var stopwatch = Stopwatch.StartNew();

            // Execute with Node.js
            var processInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"\"{tempFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                result.Error = "Failed to start Node.js process";
                return result;
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            stopwatch.Stop();

            if (process.ExitCode == 0)
            {
                result.Success = true;
                result.ExecutionTime = stopwatch.Elapsed;
                result.Output = output;
            }
            else
            {
                result.Success = false;
                result.Error = $"Node.js execution failed with exit code {process.ExitCode}: {error}";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"Node.js execution failed: {ex.Message}";
        }
        finally
        {
            // Clean up temp file
            try
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
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
