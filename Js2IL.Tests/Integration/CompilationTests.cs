using System;
using System.IO;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;

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

            var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Integration", "Compilation");
            Directory.CreateDirectory(outputDir);

            var options = new CompilerOptions
            {
                OutputDirectory = outputDir
            };

            var serviceProvider = CompilerServices.BuildServiceProvider(options, fileSystem: null);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            
            if (!compiler.Compile(scriptPath))
            {
                throw new InvalidOperationException($"Compilation failed for script {scriptPath}");
            }

            var outputDll = Path.Combine(outputDir, "generateFeatureCoverage.dll");
            Assert.True(File.Exists(outputDll), $"Output assembly missing: {outputDll}");
            Assert.True(new FileInfo(outputDll).Length > 0, "Output assembly is empty");
        }
    }
}
