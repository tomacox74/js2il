using System;
using System.IO;
using Js2IL.Services;

namespace Js2IL.Tests.Integration
{
    public class CompilationTests
    {
        [Fact]
        public void Compile_Scripts_GenerateFeatureCoverage()
        {
            if (!string.Equals(Environment.GetEnvironmentVariable("RUN_INTEGRATION"), "1", StringComparison.Ordinal))
                return; // treat as no-op unless explicitly enabled

            // Resolve repo root by walking up from the test assembly directory
            static string FindRepoRoot()
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    if (File.Exists(Path.Combine(dir.FullName, "js2il.sln")))
                        return dir.FullName;

                    var scriptsDir = Path.Combine(dir.FullName, "scripts");
                    if (Directory.Exists(scriptsDir) && File.Exists(Path.Combine(scriptsDir, "generateFeatureCoverage.js")))
                        return dir.FullName;
                    dir = dir.Parent;
                }
                throw new InvalidOperationException("Repository root not found from test base directory.");
            }

            var repoRoot = FindRepoRoot();
            var scriptPath = Path.Combine(repoRoot, "scripts", "generateFeatureCoverage.js");
            Assert.True(File.Exists(scriptPath), $"Script file not found: {scriptPath}");

            var js = File.ReadAllText(scriptPath);

            var parser = new JavaScriptParser();
            var ast = parser.ParseJavaScript(js, scriptPath);

            var validator = new JavaScriptAstValidator();
            var validation = validator.Validate(ast);
            Assert.True(validation.IsValid, "Validation failed: " + string.Join("; ", validation.Errors));

            var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Integration", "Compilation");
            Directory.CreateDirectory(outputDir);

            var assemblyName = "GenerateFeatureCoverage_Script";
            var module = new ModuleDefinition
            {
                Ast = ast,
                Path = "test.js"
            };

            var generator = new AssemblyGenerator();
            generator.Generate(module, assemblyName, outputDir);

            var outputDll = Path.Combine(outputDir, assemblyName + ".dll");
            Assert.True(File.Exists(outputDll), $"Output assembly missing: {outputDll}");
            Assert.True(new FileInfo(outputDll).Length > 0, "Output assembly is empty");
        }
    }
}
