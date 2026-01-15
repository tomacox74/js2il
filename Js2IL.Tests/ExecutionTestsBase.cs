using Js2IL.Services;
using Js2IL.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Runtime.Loader;
using JavaScriptRuntime;
using System.Runtime.ExceptionServices;

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

        protected Task ExecutionTest(string testName, bool allowUnhandledException = false, Action<VerifySettings>? configureSettings = null, bool preferOutOfProc = false, [CallerFilePath] string sourceFilePath = "", Action<IConsoleOutput> postTestProcessingAction = null!, string[]? additionalScripts = null, Action<JavaScriptRuntime.DependencyInjection.ServiceContainer>? addMocks = null)
        {
            var js = GetJavaScript(testName);
            var testFilePath = Path.Combine(_outputPath, $"{testName}.js");

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(testFilePath, js);

            // Add additional scripts to the mock file system
            if (additionalScripts != null)
            {
                foreach (var scriptName in additionalScripts)
                {
                    var scriptContent = GetJavaScript(scriptName);
                    var scriptPath = Path.Combine(_outputPath, $"{scriptName}.js");
                    mockFileSystem.AddFile(scriptPath, scriptContent);
                }
            }

            var options = new CompilerOptions
            {
                OutputDirectory = _outputPath
            };

            var testLogger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            
            if (!compiler.Compile(testFilePath))
            {
                var details = string.IsNullOrWhiteSpace(testLogger.Errors)
                    ? string.Empty
                    : $"\nErrors:\n{testLogger.Errors}";
                var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                    ? string.Empty
                    : $"\nWarnings:\n{testLogger.Warnings}";
                throw new InvalidOperationException($"Compilation failed for test {testName}.{details}{warnings}");
            }

            // Compiler outputs <entryFileBasename>.dll into OutputDirectory.
            // For nested-path test names (e.g. "CommonJS_Require_X/a"), the DLL will be "a.dll".
            var assemblyName = Path.GetFileNameWithoutExtension(testFilePath);
            var expectedPath = Path.Combine(_outputPath, $"{assemblyName}.dll");

            var il = preferOutOfProc
                ? ExecuteGeneratedAssembly(expectedPath, allowUnhandledException, testName)
                : ExecuteGeneratedAssemblyInProc(expectedPath, testName, postTestProcessingAction, addMocks: addMocks);

            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                var snapshotsDirectory = Path.Combine(directory, "Snapshots");
                Directory.CreateDirectory(snapshotsDirectory);
                settings.UseDirectory(snapshotsDirectory);
            }
            configureSettings?.Invoke(settings);
            return Verify(il, settings);
        }

        private string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException, string? testName = null, int timeoutMs = 30000)
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
            bool exited = process!.WaitForExit(timeoutMs);
            
            if (!exited)
            {
                process.Kill();
                throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
            }

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

        private string ExecuteGeneratedAssemblyInProc(string assemblyPath, string? testName = null, Action<IConsoleOutput>? postTestProcessingAction = null, int timeoutMs = 30000, Action<JavaScriptRuntime.DependencyInjection.ServiceContainer>? addMocks = null)
        {
            ArgumentNullException.ThrowIfNull(assemblyPath, nameof(assemblyPath));
            ArgumentNullException.ThrowIfNull(testName, nameof(testName));

            var assemblySimpleName = Path.GetFileNameWithoutExtension(assemblyPath);
            var needsIsolation = testName.Contains('/', StringComparison.Ordinal) || testName.Contains('\\', StringComparison.Ordinal);

            if (!needsIsolation)
            {
                foreach (var loaded in AssemblyLoadContext.Default.Assemblies)
                {
                    if (string.Equals(loaded.GetName().Name, assemblySimpleName, StringComparison.Ordinal))
                    {
                        needsIsolation = true;
                        break;
                    }
                }
            }

            var dir = Path.GetDirectoryName(assemblyPath)!;
            // IMPORTANT: do not load JavaScriptRuntime.dll from the per-test output directory.
            // Loading from that path can keep the file locked, and other tests concurrently
            // compiling into the same output directory may try to copy/overwrite it.
            // Always use the runtime assembly already loaded with the test host.
            var jsRuntimeAsm = typeof(JavaScriptRuntime.EnvironmentProvider).Assembly;

            var uniquePath = Path.Combine(dir, assemblySimpleName + $".run-{Guid.NewGuid():N}.dll");

            TestAssemblyLoadContext? alc = null;
            string outText;
            try
            {
                Assembly assembly;
                if (needsIsolation)
                {
                    File.Copy(assemblyPath, uniquePath, overwrite: true);

                    // Load the generated assembly into an isolated collectible ALC per test to avoid
                    // collisions when multiple tests compile to the same assembly name (e.g., many
                    // CommonJS tests have an entry module named "a").
                    // IMPORTANT: We must ensure the generated assembly binds to the already-loaded
                    // JavaScriptRuntime assembly, otherwise runtime statics/mocks won't match.
                    alc = new TestAssemblyLoadContext(jsRuntimeAsm, dir);
                    // Load from stream so we can delete the copied DLL without relying on GC.Collect
                    // to release file locks.
                    using var stream = File.OpenRead(uniquePath);
                    assembly = alc.LoadFromStream(stream);
                }
                else
                {
                    // Most tests produce a unique assembly name; running in the default ALC keeps behavior
                    // identical to out-of-proc execution and avoids subtle cross-ALC runtime edge cases.
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                }

                var entryPoint = assembly.EntryPoint ?? throw new InvalidOperationException("No entry point found in the generated assembly.");

                var modDir = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
                var file = assemblyPath;

                // mocks are created here
                var captured = new CapturingConsoleOutput();
                var capturedEnvironment = new CapturingEnvironment();

                var setupMocks = () =>
                {
                    JavaScriptRuntime.CommonJS.ModuleContext.SetModuleContext(modDir, file);
                    JavaScriptRuntime.EnvironmentProvider.SuppressExit = true;
                    var serviceProvider = JavaScriptRuntime.RuntimeServices.BuildServiceProvider();
                    serviceProvider.RegisterInstance(new ConsoleOutputSinks
                    {
                        Output = captured,
                        ErrorOutput = captured
                    });
                    serviceProvider.RegisterInstance<IEnvironment>(capturedEnvironment);

                    addMocks?.Invoke(serviceProvider);

                    JavaScriptRuntime.Engine._serviceProviderOverride.Value = serviceProvider;
                };


                // Run the entry point in a separate thread with timeout to prevent infinite hangs
                ExceptionDispatchInfo? threadException = null;
                var executionThread = new Thread(() =>
                {
                    setupMocks();

                    try
                    {
                        ((Action)Delegate.CreateDelegate(typeof(Action), entryPoint))();
                    }
                    catch (Exception ex)
                    {
                        threadException = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
                        JavaScriptRuntime.Engine._serviceProviderOverride.Value = null;
                    }
                });
                executionThread.Start();
                bool completed = executionThread.Join(timeoutMs);
                if (!completed)
                {
                    // Cannot safely abort managed threads, but we can at least fail the test
                    throw new TimeoutException($"In-proc test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
                }
                if (threadException != null)
                {
                    threadException.Throw();
                }
                
                postTestProcessingAction?.Invoke(captured);

                outText = captured.GetOutput();
                if (testName.StartsWith("Process_Exit_", StringComparison.Ordinal))
                {
                    var code = capturedEnvironment.ExitCalledWithCode ?? 0;
                    outText = $"exitCode {code}\n";
                }
            }
            finally
            {
                if (alc != null)
                {
                    alc.Unload();
                }

                try { File.Delete(uniquePath); } catch { }
            }

            return outText;
        }

        private sealed class TestAssemblyLoadContext : AssemblyLoadContext
        {
            private readonly Assembly _jsRuntimeAssembly;
            private readonly string _baseDirectory;

            public TestAssemblyLoadContext(Assembly jsRuntimeAssembly, string baseDirectory)
                : base(isCollectible: true)
            {
                _jsRuntimeAssembly = jsRuntimeAssembly;
                _baseDirectory = baseDirectory;
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                // Ensure the generated assembly uses the same JavaScriptRuntime assembly instance
                // as the test host, so shared statics (Engine overrides, ModuleContext, etc.) work.
                if (string.Equals(assemblyName.Name, _jsRuntimeAssembly.GetName().Name, StringComparison.Ordinal))
                {
                    return _jsRuntimeAssembly;
                }

                var candidatePath = Path.Combine(_baseDirectory, (assemblyName.Name ?? string.Empty) + ".dll");
                if (File.Exists(candidatePath))
                {
                    return LoadFromAssemblyPath(candidatePath);
                }

                return null;
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
            // Support nested module paths in tests (e.g., "CommonJS_Require_X/helpers/b").
            // Embedded resource names use '.' separators, so normalize path separators to '.'.
            var resourceKey = testName.Replace('\\', '.').Replace('/', '.');

            var categorySpecific = $"Js2IL.Tests.{GetType().Namespace?.Split('.').Last()}.JavaScript.{resourceKey}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{resourceKey}.js";
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
