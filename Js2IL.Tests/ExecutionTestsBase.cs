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
using Js2IL.IR;

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

            // Use a unique per-run directory to avoid file locks from in-proc AssemblyLoadContext
            // execution causing intermittent failures when re-running tests on Windows.
            var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            var runId = Guid.NewGuid().ToString("N");
            _outputPath = Path.Combine(root, $"{testCategory}.ExecutionTests", runId);
            Directory.CreateDirectory(_outputPath);
        }

        protected Task ExecutionTest(string testName, bool allowUnhandledException = false, Action<VerifySettings>? configureSettings = null, bool preferOutOfProc = false, [CallerFilePath] string sourceFilePath = "", Action<IConsoleOutput> postTestProcessingAction = null!, string[]? additionalScripts = null, Action<JavaScriptRuntime.DependencyInjection.ServiceContainer>? addMocks = null)
        {
            var (js, jsSourcePath) = GetJavaScriptAndSourcePath(testName, sourceFilePath);
            var testFilePath = Path.Combine(_outputPath, $"{testName}.js");

            // Prefer a stable on-disk path (within the repo) as the logical module path.
            // This ensures the PDB "document" points at a file VS Code can open when clicking stack frames.
            var entryPathForCompilation = jsSourcePath ?? testFilePath;

            // If we couldn't resolve a stable on-disk source path, write the JS to the temp location
            // so debuggers can still open the file when the PDB points at it.
            if (entryPathForCompilation == testFilePath && !File.Exists(testFilePath))
            {
                var testDir = Path.GetDirectoryName(testFilePath);
                if (!string.IsNullOrWhiteSpace(testDir))
                {
                    Directory.CreateDirectory(testDir);
                }
                File.WriteAllText(testFilePath, js);
            }

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(entryPathForCompilation, js, jsSourcePath);

            // Add additional scripts to the mock file system
            if (additionalScripts != null)
            {
                foreach (var scriptName in additionalScripts)
                {
                    var (scriptContent, scriptSourcePath) = GetJavaScriptAndSourcePath(scriptName, sourceFilePath);

                    // Same rule as the entry file: prefer the stable source path so module resolution + PDB
                    // documents stay rooted in the repo.
                    var scriptTempPath = Path.Combine(_outputPath, $"{scriptName}.js");
                    var scriptPathForCompilation = scriptSourcePath ?? scriptTempPath;

                    if (scriptPathForCompilation == scriptTempPath && !File.Exists(scriptTempPath))
                    {
                        var scriptDir = Path.GetDirectoryName(scriptTempPath);
                        if (!string.IsNullOrWhiteSpace(scriptDir))
                        {
                            Directory.CreateDirectory(scriptDir);
                        }
                        File.WriteAllText(scriptTempPath, scriptContent);
                    }

                    mockFileSystem.AddFile(scriptPathForCompilation, scriptContent, scriptSourcePath);
                }
            }

            var options = new CompilerOptions
            {
                OutputDirectory = _outputPath,
                EmitPdb = true
            };

            var testLogger = new TestLogger();
            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem, testLogger);
            var compiler = serviceProvider.GetRequiredService<Compiler>();

            var prevMetricsEnabled = IRPipelineMetrics.Enabled;
            IRPipelineMetrics.Enabled = true;
            IRPipelineMetrics.Reset();
            try
            {
                if (!compiler.Compile(entryPathForCompilation))
                {
                    var details = string.IsNullOrWhiteSpace(testLogger.Errors)
                        ? string.Empty
                        : $"\nErrors:\n{testLogger.Errors}";
                    var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                        ? string.Empty
                        : $"\nWarnings:\n{testLogger.Warnings}";
                    var lastFailure = IRPipelineMetrics.GetLastFailure();
                    var failureDetails = string.IsNullOrWhiteSpace(lastFailure)
                        ? string.Empty
                        : $"\nIR failure: {lastFailure}";
                    throw new InvalidOperationException($"Compilation failed for test {testName}.{failureDetails}{details}{warnings}");
                }
            }
            finally
            {
                IRPipelineMetrics.Enabled = prevMetricsEnabled;
            }

            // Compiler outputs <entryFileBasename>.dll into OutputDirectory.
            // For nested-path test names (e.g. "CommonJS_Require_X/a"), the DLL will be "a.dll".
            var assemblyName = Path.GetFileNameWithoutExtension(testFilePath);
            var expectedPath = Path.Combine(_outputPath, $"{assemblyName}.dll");

            if (options.EmitPdb)
            {
                var expectedPdbPath = Path.Combine(_outputPath, $"{assemblyName}.pdb");
                Assert.True(File.Exists(expectedPdbPath), $"Expected PDB to be emitted at '{expectedPdbPath}'.");
            }

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

                    // Copy PDB alongside the copied DLL so we can load symbols from a stream.
                    // This enables source/line info even when the assembly itself is loaded from bytes.
                    var sourcePdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
                    var uniquePdbPath = Path.ChangeExtension(uniquePath, ".pdb");
                    if (File.Exists(sourcePdbPath))
                    {
                        File.Copy(sourcePdbPath, uniquePdbPath, overwrite: true);
                    }

                    // Load the generated assembly into an isolated collectible ALC per test to avoid
                    // collisions when multiple tests compile to the same assembly name (e.g., many
                    // CommonJS tests have an entry module named "a").
                    // IMPORTANT: We must ensure the generated assembly binds to the already-loaded
                    // JavaScriptRuntime assembly, otherwise runtime statics/mocks won't match.
                    alc = new TestAssemblyLoadContext(jsRuntimeAsm, dir);
                    // Load from stream so we can delete the copied DLL without relying on GC.Collect
                    // to release file locks.
                    using var stream = File.OpenRead(uniquePath);
                    if (File.Exists(uniquePdbPath))
                    {
                        using var pdbStream = File.OpenRead(uniquePdbPath);
                        assembly = alc.LoadFromStream(stream, pdbStream);
                    }
                    else
                    {
                        assembly = alc.LoadFromStream(stream);
                    }
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

                TryDeleteFile(uniquePath);
                TryDeleteFile(Path.ChangeExtension(uniquePath, ".pdb"));
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

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete temp file '{path}': {ex.Message}");
            }
        }

        private (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName, string callerSourceFilePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // Support nested module paths in tests (e.g., "CommonJS_Require_X/helpers/b").
            // Embedded resource names use '.' separators, so normalize path separators to '.'.
            var resourceKey = testName.Replace('\\', '.').Replace('/', '.');

            var categorySpecific = $"Js2IL.Tests.{GetType().Namespace?.Split('.').Last()}.JavaScript.{resourceKey}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{resourceKey}.js";

            Stream? stream = assembly.GetManifestResourceStream(categorySpecific);
            var resolvedResourceName = categorySpecific;
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream(legacy);
                resolvedResourceName = legacy;
            }

            using (stream)
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{categorySpecific}' or '{legacy}' not found in assembly '{assembly.FullName}'.");
                }
                using (var reader = new StreamReader(stream))
                {
                    var script = reader.ReadToEnd();
                    var sourcePath = TryGetOriginalSourcePathFromEmbeddedResource(assembly, resolvedResourceName, callerSourceFilePath);
                    if (string.IsNullOrWhiteSpace(sourcePath))
                    {
                        // Fallback: derive the repo path from known test layout:
                        // Js2IL.Tests/<Category>/JavaScript/<testName>.js
                        var category = GetType().Namespace?.Split('.').Last();
                        if (!string.IsNullOrWhiteSpace(category))
                        {
                            var projectRoot = FindDirectoryContainingFile(Path.GetDirectoryName(callerSourceFilePath) ?? string.Empty, "Js2IL.Tests.csproj");
                            if (projectRoot != null)
                            {
                                var relative = Path.Combine(
                                    category,
                                    "JavaScript",
                                    testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js");

                                var candidate = Path.GetFullPath(Path.Combine(projectRoot, relative));
                                if (File.Exists(candidate))
                                {
                                    sourcePath = candidate;
                                }
                            }
                        }
                    }
                    return (script, sourcePath);
                }
            }
        }

        private static string? TryGetOriginalSourcePathFromEmbeddedResource(Assembly assembly, string jsResourceName, string callerSourceFilePath)
        {
            // For each embedded "*.js" test script, we also embed a "*.path" text resource
            // containing the project-relative path to the original on-disk JS file.
            var pathResourceName = jsResourceName.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                ? jsResourceName.Substring(0, jsResourceName.Length - 3) + ".path"
                : jsResourceName + ".path";

            using var pathStream = assembly.GetManifestResourceStream(pathResourceName);
            if (pathStream == null)
            {
                return null;
            }

            using var reader = new StreamReader(pathStream);
            var relativePath = reader.ReadToEnd().Trim();
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            var projectRoot = FindDirectoryContainingFile(Path.GetDirectoryName(callerSourceFilePath) ?? string.Empty, "Js2IL.Tests.csproj");
            if (projectRoot == null)
            {
                return null;
            }

            // Normalize separators to current OS.
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.GetFullPath(Path.Combine(projectRoot, relativePath));
        }

        private static string? FindDirectoryContainingFile(string startDirectory, string fileName)
        {
            var current = startDirectory;
            while (!string.IsNullOrWhiteSpace(current))
            {
                var candidate = Path.Combine(current, fileName);
                if (File.Exists(candidate))
                {
                    return current;
                }

                var parent = Directory.GetParent(current);
                if (parent == null)
                {
                    break;
                }
                current = parent.FullName;
            }

            return null;
        }
    }
}
