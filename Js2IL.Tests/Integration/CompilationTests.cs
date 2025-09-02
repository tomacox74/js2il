using System;
using System.IO;
using Js2IL.Services;

namespace Js2IL.Tests.Integration
{
    public class CompilationTests
    {
        [Fact(Skip = "Manual integration test: compiles scripts/generateFeatureCoverage.js; do not run in CI")]
        public void Compile_Scripts_GenerateFeatureCoverage()
        {
            var repoRoot = Directory.GetCurrentDirectory();
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
            var generator = new AssemblyGenerator();
            generator.Generate(ast, assemblyName, outputDir);

            var outputDll = Path.Combine(outputDir, assemblyName + ".dll");
            Assert.True(File.Exists(outputDll), $"Output assembly missing: {outputDll}");
            Assert.True(new FileInfo(outputDll).Length > 0, "Output assembly is empty");
        }
    }
}
