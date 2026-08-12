using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;

namespace Jroc.Tests
{
    public abstract class ExecutionTestsBase
    {
        private readonly string _testCategory;
        private readonly VerifySettings _verifySettings = new();

        protected ExecutionTestsBase(string testCategory)
        {
            _verifySettings.DisableDiff();
            _testCategory = testCategory;
        }

        protected async Task ExecutionTest(string testName, bool allowUnhandledException = false, Action<VerifySettings>? configureSettings = null, bool preferOutOfProc = false, [CallerFilePath] string sourceFilePath = "", Action<IConsoleOutput> postTestProcessingAction = null!, string[]? additionalScripts = null, Action<JavaScriptRuntime.DependencyInjection.ServiceContainer>? addMocks = null)
        {
            if (IsTest262ExecutionTest())
            {
                if (postTestProcessingAction != null)
                {
                    throw new NotSupportedException("Post-test console processing is not supported for in-memory test262 execution.");
                }

                var result = Test262SharedAssertHarness.CompileAndExecute(
                    testName,
                    _testCategory,
                    name => GetJavaScriptAndSourcePath(name, sourceFilePath),
                    sourceFilePath,
                    enableIRMetrics: true,
                    allowUnhandledException: allowUnhandledException,
                    addMocks: addMocks);

                Test262SharedAssertHarness.AssertNoOutput(testName, result.Output);
                return;
            }

            var compiled = SharedTestCompilation.GetOrCompile(
                _testCategory,
                testName,
                additionalScripts,
                outputDir => TestCompiler.Compile(
                    testName,
                    _testCategory,
                    outputDir,
                    name => GetJavaScriptAndSourcePath(name, sourceFilePath),
                    additionalScripts,
                    enableIRMetrics: true,
                    writeArtifacts: preferOutOfProc));

            string output;
            if (preferOutOfProc)
            {
                var materializedArtifact = compiled.MaterializedArtifact
                    ?? compiled.Artifact.Materialize(compiled.OutputDirectory);
                output = ExecuteGeneratedAssembly(materializedArtifact.AssemblyPath, allowUnhandledException, testName);
            }
            else
            {
                output = InMemoryTestCompiler.ExecuteArtifact(
                    compiled.Artifact,
                    compiled.TestFilePath,
                    testName,
                    allowUnhandledException,
                    addMocks,
                    postTestProcessingAction: postTestProcessingAction).Output;
            }

            var settings = new VerifySettings(_verifySettings);
            var directory = Path.GetDirectoryName(sourceFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                var snapshotsDirectory = Path.Combine(directory, "Snapshots");
                Directory.CreateDirectory(snapshotsDirectory);
                settings.UseDirectory(snapshotsDirectory);
            }

            configureSettings?.Invoke(settings);
            await Verify(output, settings);
        }

        private bool IsTest262ExecutionTest()
            => string.Equals(GetType().Assembly.GetName().Name, "Jroc.Test262.Tests", StringComparison.Ordinal);

        private static string ExecuteGeneratedAssembly(string assemblyPath, bool allowUnhandledException, string? testName = null, int timeoutMs = 30000)
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
            var exited = process!.WaitForExit(timeoutMs);
            if (!exited)
            {
                process.Kill();
                throw new TimeoutException($"Test execution timed out after {timeoutMs}ms. Test may have an infinite loop.");
            }

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(testName) && testName.StartsWith("Process_Exit_", StringComparison.Ordinal))
            {
                if (process.ExitCode < 0 && !allowUnhandledException)
                {
                    throw new Exception($"dotnet execution failed with exit code {process.ExitCode}:\nSTDERR:\n{standardError}\nSTDOUT:\n{standardOutput}");
                }

                return $"exitCode {process.ExitCode}\n";
            }

            if (process.ExitCode != 0)
            {
                if (!allowUnhandledException)
                {
                    throw new Exception($"dotnet execution failed:\n{standardError}");
                }

                return standardOutput;
            }

            return standardOutput;
        }

        private (string Script, string? SourcePath) GetJavaScriptAndSourcePath(string testName, string callerSourceFilePath)
        {
            var testType = GetType();
            var assembly = testType.Assembly;
            // Support nested module paths in tests (e.g., "CommonJS_Require_X/helpers/b").
            // Embedded resource names use '.' separators, so normalize path separators to '.'.
            var resourceKey = testName.Replace('\\', '.').Replace('/', '.');

            var category = TestProjectLayout.GetCategoryFromNamespace(testType);
            var resourceRoot = TestProjectLayout.GetResourceRoot(testType);
            var categorySpecific = $"{resourceRoot}.{category}.JavaScript.{resourceKey}.js";
            var legacy = $"{resourceRoot}.JavaScript.{resourceKey}.js";

            Stream? stream = assembly.GetManifestResourceStream(categorySpecific);
            var resolvedResourceName = categorySpecific;
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream(legacy);
                resolvedResourceName = legacy;
            }

            if (stream == null)
            {
                // Some build configurations produce manifest resource names with different casing
                // (e.g., "JROC.Tests" vs "Jroc.Tests"). Resolve case-insensitively.
                var names = assembly.GetManifestResourceNames();
                var match = names.FirstOrDefault(n => string.Equals(n, categorySpecific, StringComparison.OrdinalIgnoreCase))
                    ?? names.FirstOrDefault(n => string.Equals(n, legacy, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    stream = assembly.GetManifestResourceStream(match);
                    resolvedResourceName = match;
                }
            }

            using (stream)
            {
                if (stream == null)
                {
                    var sourceDirectory = Path.GetDirectoryName(callerSourceFilePath);
                    if (!string.IsNullOrWhiteSpace(sourceDirectory))
                    {
                        var scriptPath = Path.Combine(
                            sourceDirectory,
                            "JavaScript",
                            testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js");

                        if (File.Exists(scriptPath))
                        {
                            return (File.ReadAllText(scriptPath), scriptPath);
                        }
                    }

                    throw new InvalidOperationException($"Resource '{categorySpecific}' or '{legacy}' not found in assembly '{assembly.FullName}'.");
                }

                using var reader = new StreamReader(stream);
                var script = reader.ReadToEnd();
                var sourcePath = TryGetOriginalSourcePathFromEmbeddedResource(testType, assembly, resolvedResourceName, callerSourceFilePath);
                if (string.IsNullOrWhiteSpace(sourcePath) && !string.IsNullOrWhiteSpace(category))
                {
                    var projectRoot = TestProjectLayout.FindProjectRoot(testType, callerSourceFilePath);
                    if (projectRoot != null)
                    {
                        var categoryPath = category.Replace('.', Path.DirectorySeparatorChar);
                        var relative = Path.Combine(
                            categoryPath,
                            "JavaScript",
                            testName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar) + ".js");

                        var candidate = Path.GetFullPath(Path.Combine(projectRoot, relative));
                        if (File.Exists(candidate))
                        {
                            sourcePath = candidate;
                        }
                    }
                }

                return (script, sourcePath);
            }
        }

        private static string? TryGetOriginalSourcePathFromEmbeddedResource(Type testType, Assembly assembly, string jsResourceName, string callerSourceFilePath)
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

            var projectRoot = TestProjectLayout.FindProjectRoot(testType, callerSourceFilePath);
            if (projectRoot == null)
            {
                return null;
            }

            // Normalize separators to current OS.
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return Path.GetFullPath(Path.Combine(projectRoot, relativePath));
        }
    }
}
