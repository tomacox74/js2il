using Js2IL.Services;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.Loader;
using JavaScriptRuntime;

namespace Js2IL.Tests
{
    public abstract class ExecutionTestsBase
    {
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        protected ExecutionTestsBase(string testCategory)
        {
            _parser = new JavaScriptParser();
            _validator = new JavaScriptAstValidator();
            _verifySettings.DisableDiff();

            // create a temp directory for the generated assemblies
            _outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            _outputPath = Path.Combine(_outputPath, $"{testCategory}.ExecutionTests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

    protected Task ExecutionTest(string testName, bool allowUnhandledException = false, [CallerFilePath] string sourceFilePath = "")
        {
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js, "test.js");
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();

            generator.Generate(ast, testName, _outputPath);

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            // Run in-proc to avoid process startup overhead and capture output.
            string il;
            bool usedFallback = false;
            string? fallbackReason = null;
            try
            {
                il = ExecuteGeneratedAssemblyInProc(expectedPath);
                if (string.IsNullOrWhiteSpace(il))
                {
                    // Fallback for environments where in-proc capture may miss output
                    usedFallback = true;
                    fallbackReason = "in-proc produced empty output";
                    il = ExecuteGeneratedAssembly(expectedPath, allowUnhandledException);
                }
            }
            catch (Exception ex)
            {
                // Fallback to out-of-proc execution on error (e.g., assembly binding issues)
                usedFallback = true;
                fallbackReason = $"in-proc error: {ex.GetType().Name}";
                il = ExecuteGeneratedAssembly(expectedPath, allowUnhandledException);
            }
            
            if (usedFallback)
            {
                // Write a diagnostic to the test output log instead of altering verified content.
                var reason = string.IsNullOrWhiteSpace(fallbackReason) ? "unknown reason" : fallbackReason;
                System.Console.WriteLine($"[ExecutionTestsBase] Fallback to out-of-proc execution; reason: {reason}");
            }
            
            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                settings.UseDirectory(directory);
            }
            return Verify(il, settings);
        }

    private string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{assemblyPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process!.WaitForExit();

            string stdOut = process.StandardOutput.ReadToEnd();
            string stdErr = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
            {
                if (!allowUnhandledException)
                {
                    throw new Exception($"dotnet execution failed:\n{stdErr}");
                }
                // When allowed, still verify what made it to stdout
                return stdOut;
            }

            return stdOut;
        }

        private string ExecuteGeneratedAssemblyInProc(string assemblyPath)
        {
            // Capture JavaScriptRuntime.Console output by swapping its IConsoleOutput implementation.
            var captured = new CapturingConsoleOutput();
            var consoleType = typeof(JavaScriptRuntime.Console);
            var outputField = consoleType.GetField("_output", BindingFlags.NonPublic | BindingFlags.Static);
            var previous = outputField?.GetValue(null) as IConsoleOutput;
            JavaScriptRuntime.Console.SetOutput(captured);
            try
            {
                // Preload JavaScriptRuntime dependency and load test assembly in Default ALC so framework resolves on the agent
                var dir = Path.GetDirectoryName(assemblyPath)!;
                var jsRuntimePath = Path.Combine(dir, "JavaScriptRuntime.dll");
                if (File.Exists(jsRuntimePath))
                {
                    try { AssemblyLoadContext.Default.LoadFromAssemblyPath(jsRuntimePath); } catch { }
                }
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                var entryPoint = assembly.EntryPoint ?? throw new InvalidOperationException("No entry point found in the generated assembly.");

                var paramInfos = entryPoint.GetParameters();
                object?[]? args = paramInfos.Length == 0 ? null : new object?[] { System.Array.Empty<string>() };
                entryPoint.Invoke(null, args);

                return captured.GetOutput();
            }
            finally
            {
                // Restore previous console output implementation (or default if missing)
                JavaScriptRuntime.Console.SetOutput(previous ?? new DefaultConsoleOutput());
            }
        }

        private sealed class CapturingConsoleOutput : IConsoleOutput
        {
            private readonly StringBuilder _sb = new();
            public void WriteLine(string line)
            {
                _sb.AppendLine(line);
            }
            public string GetOutput() => _sb.ToString();
        }


        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Js2IL.Tests.JavaScript.{testName}.js";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
