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
        private readonly JavaScriptParser _parser;
        private readonly JavaScriptAstValidator _validator;
        private readonly string _outputPath;
        private readonly VerifySettings _verifySettings = new();

        protected GeneratorTestsBase(string testCategory)
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

            _outputPath = Path.Combine(_outputPath, $"{testCategory}.GeneratorTests");
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        protected Task GenerateTest(string testName, [CallerFilePath] string sourceFilePath = "")
            => GenerateTest(testName, configureSettings: null, sourceFilePath);

        protected Task GenerateTest(string testName, Action<VerifySettings>? configureSettings, [CallerFilePath] string sourceFilePath = "")
        {
            var js = GetJavaScript(testName);
            var testFilePath = Path.Combine(_outputPath, $"{testName}.js");

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(testFilePath, js);

            var options = new CompilerOptions
            {
                OutputDirectory = _outputPath
            };

            var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            
            if (!compiler.Compile(testFilePath))
            {
                throw new InvalidOperationException($"Compilation failed for test {testName}");
            }

            var expectedPath = Path.Combine(_outputPath, $"{testName}.dll");

            var il = Utilities.AssemblyToText.ConvertToText(expectedPath);
            
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

        private string GetJavaScript(string testName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var category = GetType().Namespace?.Split('.').Last();
            var categorySpecific = string.IsNullOrEmpty(category)
                ? null
                : $"Js2IL.Tests.{category}.JavaScript.{testName}.js";
            var legacy = $"Js2IL.Tests.JavaScript.{testName}.js";
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
