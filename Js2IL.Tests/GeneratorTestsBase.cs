using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Js2IL.Tests
{
    public abstract class GeneratorTestsBase
    {
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        protected GeneratorTestsBase(string testCategory)
        {
            _verifySettings.DisableDiff();

            // Create a temp directory for the generated assemblies.
            // Use a unique per-run directory to avoid file locks from Assembly.LoadFile() causing
            // intermittent failures when re-running tests on Windows.
            var root = Path.Combine(Path.GetTempPath(), "Js2IL.Tests");
            var runId = Guid.NewGuid().ToString("N");
            _outputPath = Path.Combine(root, $"{testCategory}.GeneratorTests", runId);
            Directory.CreateDirectory(_outputPath);
        }

        protected Task GenerateTest(string testName, string[]? additionalScripts, [CallerFilePath] string sourceFilePath = "")
            => GenerateTest(testName, configureSettings: null, additionalScripts: additionalScripts, sourceFilePath: sourceFilePath);

        protected Task GenerateTest(string testName, Action<VerifySettings>? configureSettings = null, string[]? additionalScripts = null, [CallerFilePath] string sourceFilePath = "", Action<System.Reflection.Assembly>? verifyAssembly = null)
        {
            async Task RunAsync()
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
                    var compileDetails = string.IsNullOrWhiteSpace(testLogger.Errors)
                        ? string.Empty
                        : $"\nErrors:\n{testLogger.Errors}";
                    var warnings = string.IsNullOrWhiteSpace(testLogger.Warnings)
                        ? string.Empty
                        : $"\nWarnings:\n{testLogger.Warnings}";
                    throw new InvalidOperationException($"Compilation failed for test {testName}.{compileDetails}{warnings}");
                }

                // Compiler outputs <entryFileBasename>.dll into OutputDirectory.
                // For nested-path test names (e.g. "CommonJS_Require_X/a"), the DLL will be "a.dll".
                var assemblyName = Path.GetFileNameWithoutExtension(testFilePath);
                var expectedPath = Path.Combine(_outputPath, $"{assemblyName}.dll");

                var il = Utilities.AssemblyToText.ConvertToText(expectedPath);

                if (verifyAssembly is not null)
                {
                    var assembly = Assembly.LoadFile(expectedPath);
                    verifyAssembly(assembly);
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

                await Verify(il, settings);
            }

            return RunAsync();
        }

        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var category = GetType().Namespace?.Split('.').Last();
            // Support nested module paths in tests (e.g., "CommonJS_Require_X/helpers/b").
            // Embedded resource names use '.' separators, so normalize path separators to '.'.
            var resourceKey = testName.Replace('\\', '.').Replace('/', '.');
            var categorySpecific = string.IsNullOrEmpty(category)
                ? null
                : $"Js2IL.Tests.{category}.JavaScript.{resourceKey}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{resourceKey}.js";
            using (var stream = (categorySpecific != null ? assembly.GetManifestResourceStream(categorySpecific) : null)
                               ?? assembly.GetManifestResourceStream(legacy))
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
