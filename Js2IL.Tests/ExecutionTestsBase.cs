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

            _outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", $"{testCategory}.ExecutionTests");
            Directory.CreateDirectory(_outputPath);
        }

        protected Task ExecutionTest(string testName, bool allowUnhandledException = false, Action<VerifySettings>? configureSettings = null, bool preferOutOfProc = false, [CallerFilePath] string sourceFilePath = "")
        {
            var js = GetJavaScript(testName);
            var ast = _parser.ParseJavaScript(js, "test.js");
            _validator.Validate(ast);

            var generator = new AssemblyGenerator();
            generator.Generate(ast, testName, _outputPath);

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            string il;
            bool usedFallback = false;
            string? fallbackReason = null;

            try
            {
                if (preferOutOfProc)
                {
                    il = ExecuteGeneratedAssembly(expectedPath, allowUnhandledException, testName);
                }
                else
                {
                    il = ExecuteGeneratedAssemblyInProc(expectedPath, testName);
                }
                if (string.IsNullOrWhiteSpace(il))
                {
                    usedFallback = true;
                    fallbackReason = "in-proc produced empty output";
                    il = ExecuteGeneratedAssembly(expectedPath, allowUnhandledException, testName);
                }
            }
            catch (Exception ex)
            {
                usedFallback = true;
                fallbackReason = $"in-proc error: {ex.GetType().Name}";
                il = ExecuteGeneratedAssembly(expectedPath, allowUnhandledException, testName);
            }

            if (usedFallback)
            {
                var reason = string.IsNullOrWhiteSpace(fallbackReason) ? "unknown reason" : fallbackReason;
                System.Console.WriteLine($"[ExecutionTestsBase] Fallback to out-of-proc execution; reason: {reason}");
            }

            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                settings.UseDirectory(directory);
            }
            configureSettings?.Invoke(settings);
            return Verify(il, settings);
        }

        private string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException, string? testName = null)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = assemblyPath,
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

            if (!string.IsNullOrEmpty(testName) && testName.StartsWith("Process_Exit_", StringComparison.Ordinal))
            {
                if (process.ExitCode < 0 && !allowUnhandledException)
                {
                    throw new Exception($"dotnet execution failed with exit code {process.ExitCode}:\nSTDERR:\n{stdErr}\nSTDOUT:\n{stdOut}");
                }
                return $"exitCode {process.ExitCode}\n";
            }

            if (process.ExitCode != 0)
            {
                if (!allowUnhandledException)
                {
                    throw new Exception($"dotnet execution failed:\n{stdErr}");
                }
                return stdOut;
            }

            return stdOut;
        }

        private string ExecuteGeneratedAssemblyInProc(string assemblyPath, string? testName = null)
        {
            var prevOut = System.Console.Out;
            var prevErr = System.Console.Error;
            var swOut = new StringWriter();
            var swErr = new StringWriter();
            System.Console.SetOut(swOut);
            System.Console.SetError(swErr);

            var captured = new CapturingConsoleOutput();
            var consoleType = typeof(JavaScriptRuntime.Console);
            var outputField = consoleType.GetField("_output", BindingFlags.NonPublic | BindingFlags.Static);
            var previous = outputField?.GetValue(null) as IConsoleOutput;
            JavaScriptRuntime.Console.SetOutput(captured);
            try
            {
                var dir = Path.GetDirectoryName(assemblyPath)!;
                var jsRuntimePath = Path.Combine(dir, "JavaScriptRuntime.dll");
                Assembly? jsRuntimeAsm = null;
                if (File.Exists(jsRuntimePath))
                {
                    try { jsRuntimeAsm = AssemblyLoadContext.Default.LoadFromAssemblyPath(jsRuntimePath); } catch { }
                }
                jsRuntimeAsm ??= typeof(JavaScriptRuntime.EnvironmentProvider).Assembly;

                var uniquePath = Path.Combine(dir, Path.GetFileNameWithoutExtension(assemblyPath) + $".run-{Guid.NewGuid():N}.dll");
                File.Copy(assemblyPath, uniquePath, overwrite: true);
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(uniquePath);
                var entryPoint = assembly.EntryPoint ?? throw new InvalidOperationException("No entry point found in the generated assembly.");

                var modDir = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
                var file = assemblyPath;
                // Attempt to set module context on all plausible runtime assemblies pre-run
                try
                {
                    // 1) The discovered jsRuntimeAsm (file-based or fallback)
                    var gvType = jsRuntimeAsm?.GetType("JavaScriptRuntime.GlobalVariables");
                    var setCtx = gvType?.GetMethod("SetModuleContext", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string), typeof(string) });
                    if (gvType != null && setCtx != null)
                    {
                        setCtx.Invoke(null, new object?[] { modDir, file });
                    }
                }
                catch { }
                try
                {
                    // 2) The compile-time runtime assembly
                    JavaScriptRuntime.GlobalVariables.SetModuleContext(modDir, file);
                }
                catch { }
                try
                {
                    // 3) Any already-loaded runtime assemblies in the default context
                    foreach (var asm in AssemblyLoadContext.Default.Assemblies)
                    {
                        if (!string.Equals(asm.GetName().Name, "JavaScriptRuntime", StringComparison.Ordinal)) continue;
                        try
                        {
                            var gvt = asm.GetType("JavaScriptRuntime.GlobalVariables");
                            var sc = gvt?.GetMethod("SetModuleContext", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string), typeof(string) });
                            sc?.Invoke(null, new object?[] { modDir, file });
                        }
                        catch { }
                    }
                }
                catch { }

                // Rebind jsRuntimeAsm to the actual loaded JavaScriptRuntime used by the generated assembly
                try
                {
                    var loaded = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a => string.Equals(a.GetName().Name, "JavaScriptRuntime", StringComparison.Ordinal));
                    if (loaded != null)
                    {
                        jsRuntimeAsm = loaded;
                    }
                }
                catch { }

                // Ensure module context is set on the actual runtime assembly instance used by the program
                try
                {
                    if (jsRuntimeAsm != null)
                    {
                        var gvType2 = jsRuntimeAsm.GetType("JavaScriptRuntime.GlobalVariables");
                        if (gvType2 != null)
                        {
                            var setCtx2 = gvType2.GetMethod("SetModuleContext", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string), typeof(string) });
                            if (setCtx2 != null)
                            {
                                setCtx2.Invoke(null, new object?[] { modDir, file });
                            }
                            else
                            {
                                // Fallback: set __dirname/__filename directly if method not found
                                var dirProp = gvType2.GetProperty("__dirname", BindingFlags.Public | BindingFlags.Static);
                                var fileProp = gvType2.GetProperty("__filename", BindingFlags.Public | BindingFlags.Static);
                                if (dirProp != null && dirProp.CanWrite)
                                {
                                    dirProp.SetValue(null, modDir);
                                }
                                if (fileProp != null && fileProp.CanWrite)
                                {
                                    fileProp.SetValue(null, file);
                                }
                                else
                                {
                                    var dirField = gvType2.GetField("__dirname", BindingFlags.NonPublic | BindingFlags.Static);
                                    var fileField = gvType2.GetField("__filename", BindingFlags.NonPublic | BindingFlags.Static);
                                    if (dirField != null) dirField.SetValue(null, modDir);
                                    if (fileField != null) fileField.SetValue(null, file);
                                }
                            }
                        }
                    }
                }
                catch { }

                // Install a CapturingEnvironment so we can deterministically read the exit code without terminating
                object? capEnvInstance = null;
                Type? capturingEnvType = null;
                try
                {
                    if (jsRuntimeAsm != null)
                    {
                        var envProvType = jsRuntimeAsm.GetType("JavaScriptRuntime.EnvironmentProvider");
                        var suppressProp = envProvType?.GetProperty("SuppressExit", BindingFlags.Public | BindingFlags.Static);
                        suppressProp?.SetValue(null, true);
                        var ienvType = jsRuntimeAsm.GetType("JavaScriptRuntime.IEnvironment");
                        capturingEnvType = jsRuntimeAsm.GetType("JavaScriptRuntime.CapturingEnvironment");
                        var setEnv = envProvType?.GetMethod("SetEnvironment", BindingFlags.Public | BindingFlags.Static, new Type[] { ienvType! });
                        if (setEnv != null && capturingEnvType != null)
                        {
                            capEnvInstance = Activator.CreateInstance(capturingEnvType);
                            setEnv.Invoke(null, new object?[] { capEnvInstance });
                        }
                        // Reset process-wide exit code to a known state
                        try { System.Environment.ExitCode = 0; } catch { }
                    }
                }
                catch { }

                var paramInfos = entryPoint.GetParameters();
                object?[]? args = paramInfos.Length == 0 ? null : new object?[] { System.Array.Empty<string>() };
                try { entryPoint.Invoke(null, args); } catch { }

                var outText = swOut.ToString();
                if (!string.IsNullOrEmpty(testName) && testName.StartsWith("Process_Exit_", StringComparison.Ordinal))
                {
                    int code = System.Environment.ExitCode;
                    try
                    {
                        if (jsRuntimeAsm != null)
                        {
                            var envProvType = jsRuntimeAsm.GetType("JavaScriptRuntime.EnvironmentProvider");
                            var lastProp = envProvType?.GetProperty("LastExitCodeSet", BindingFlags.Public | BindingFlags.Static);
                            var lastVal = lastProp?.GetValue(null);
                            if (lastVal is int last)
                            {
                                code = last;
                            }
                            else if (capEnvInstance != null && capturingEnvType != null)
                            {
                                var exitCalledWithCodeProp = capturingEnvType.GetProperty("ExitCalledWithCode", BindingFlags.Public | BindingFlags.Instance);
                                var exitCodeProp = capturingEnvType.GetProperty("ExitCode", BindingFlags.Public | BindingFlags.Instance);
                                var val = exitCalledWithCodeProp?.GetValue(capEnvInstance);
                                if (val is int i)
                                {
                                    code = i;
                                }
                                else if (exitCodeProp != null)
                                {
                                    var ecVal = exitCodeProp.GetValue(capEnvInstance);
                                    if (ecVal is int j)
                                    {
                                        code = j;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                    outText = $"exitCode {code}\n";
                }
                if (string.IsNullOrEmpty(outText))
                {
                    outText = captured.GetOutput();
                }
                return outText;
            }
            finally
            {
                try
                {
                    JavaScriptRuntime.EnvironmentProvider.SetEnvironment(new JavaScriptRuntime.DefaultEnvironment());
                    JavaScriptRuntime.EnvironmentProvider.SuppressExit = false;
                }
                catch { }
                JavaScriptRuntime.Console.SetOutput(previous ?? new DefaultConsoleOutput());
                System.Console.SetOut(prevOut);
                System.Console.SetError(prevErr);
            }
        }

        private sealed class CapturingConsoleOutput : IConsoleOutput
        {
            private readonly StringBuilder _sb = new();
            public void WriteLine(string line) => _sb.AppendLine(line);
            public string GetOutput() => _sb.ToString();
        }

        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var categorySpecific = $"Js2IL.Tests.{GetType().Namespace?.Split('.').Last()}.JavaScript.{testName}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{testName}.js";
            using (var stream = assembly.GetManifestResourceStream(categorySpecific) ?? assembly.GetManifestResourceStream(legacy))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{categorySpecific}' or '{legacy}' not found in assembly '{assembly.FullName}'.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
