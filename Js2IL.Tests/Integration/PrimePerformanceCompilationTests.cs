using System;
using System.IO;
using Js2IL.Services;

namespace Js2IL.Tests.Integration;

public class PrimePerformanceCompilationTests
{
    /// <summary>
    /// Reproduces the current failure when attempting to compile the performance script
    /// tests/performance/PrimeJavaScript.js which uses ES class syntax. The ClassesGenerator
    /// attempts to emit methods but the symbol table lacks method-level scopes, resulting
    /// in an InvalidOperationException: "Scope 'setBitTrue' not found in local slots".
    ///
    /// This test captures the existing buggy behavior so we have a regression target.
    /// Once class method scopes are implemented, this test should be updated to assert
    /// successful generation instead of expecting an exception.
    /// </summary>
    [Fact]
    public void Compile_PrimeJavaScript_PerformanceScript_Reproduces_ClassMethodScopeBug()
    {
        // Optional integration: skip unless explicitly enabled (mirrors CompilationTests)
        if (!string.Equals(Environment.GetEnvironmentVariable("RUN_INTEGRATION"), "1", StringComparison.Ordinal))
            return; // no-op when integration tests disabled

        var repoRoot = FindRepoRoot();
        var scriptPath = Path.Combine(repoRoot, "tests", "performance", "PrimeJavaScript.js");
        Assert.True(File.Exists(scriptPath), $"Script file not found: {scriptPath}");

        var js = File.ReadAllText(scriptPath);

        var parser = new JavaScriptParser();
        var ast = parser.ParseJavaScript(js, scriptPath);

        var validator = new JavaScriptAstValidator();
        var validation = validator.Validate(ast);
        // Classes are currently experimental warnings — ensure no hard validation errors.
        Assert.True(validation.IsValid, "Validation unexpectedly failed: " + string.Join("; ", validation.Errors));

        var outputDir = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "Integration", "PrimePerformance");
        Directory.CreateDirectory(outputDir);

        var assemblyName = "PrimePerformanceScript";
        var generator = new AssemblyGenerator();

        var ex = Assert.Throws<InvalidOperationException>(() => generator.Generate(ast, assemblyName, outputDir));
        Assert.Contains("setBitTrue", ex.Message); // Key part of current failure message
    }

    private static string FindRepoRoot()
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
}
