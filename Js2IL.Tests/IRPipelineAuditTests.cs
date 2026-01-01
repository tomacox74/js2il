using Js2IL.IR;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests;

/// <summary>
/// Audit test to track progress of IR pipeline adoption.
/// Runs all execution tests and reports how many methods compiled via IR vs legacy.
/// Uses a unique collection to prevent parallel execution since it modifies global static state.
/// </summary>
[Collection("IRPipelineMetrics")]
public class IRPipelineAuditTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly string _outputPath;

    public IRPipelineAuditTests(ITestOutputHelper output)
    {
        _output = output;
        _outputPath = Path.Combine(Path.GetTempPath(), "Js2IL.Tests", "IRPipelineAudit");
        Directory.CreateDirectory(_outputPath);
        IRPipelineMetrics.Enabled = true;
        IRPipelineMetrics.Reset();
    }

    public void Dispose()
    {
        IRPipelineMetrics.Enabled = false;
    }

    /// <summary>
    /// Reports IR pipeline metrics after compiling ALL embedded JavaScript test files.
    /// This gives a comprehensive view of IR adoption across the entire test suite.
    /// Run with: dotnet test --filter "FullSuiteIRPipelineMetrics" --logger "console;verbosity=detailed"
    /// </summary>
    [Fact]
    public void FullSuiteIRPipelineMetrics()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(r => r.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
            .Where(r => !r.Contains(".expected.")) // Skip expected output files
            .OrderBy(r => r)
            .ToList();

        _output.WriteLine($"Found {resourceNames.Count} JavaScript test files");
        _output.WriteLine("");

        int compiled = 0;
        int failed = 0;
        var failures = new List<(string Resource, string Error)>();

        foreach (var resourceName in resourceNames)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                using var reader = new StreamReader(stream);
                var js = reader.ReadToEnd();

                // Extract a test name from the resource name
                var resourceWithoutPrefix = resourceName.StartsWith("Js2IL.Tests.", StringComparison.Ordinal)
                    ? resourceName.Substring("Js2IL.Tests.".Length)
                    : resourceName;
                var testName = Path.GetFileNameWithoutExtension(resourceWithoutPrefix);
                
                CompileJavaScript(js, testName);
                compiled++;
            }
            catch (Exception ex)
            {
                failed++;
                failures.Add((resourceName, ex.Message));
            }
        }

        var stats = IRPipelineMetrics.GetStats();
        
        _output.WriteLine("=".PadRight(60, '='));
        _output.WriteLine("IR PIPELINE ADOPTION REPORT - FULL TEST SUITE");
        _output.WriteLine("=".PadRight(60, '='));
        _output.WriteLine("");
        _output.WriteLine($"Test Files: {compiled} compiled, {failed} failed to compile");
        _output.WriteLine("");
        _output.WriteLine(stats.ToString());
        _output.WriteLine("");
        _output.WriteLine("Goal: 100% IR pipeline adoption (0 legacy fallbacks)");
        _output.WriteLine($"Current legacy fallbacks: {stats.TotalFallbacks}");
        _output.WriteLine("");
        
        if (failures.Count > 0 && failures.Count <= 10)
        {
            _output.WriteLine("Compilation failures:");
            foreach (var (resource, error) in failures)
            {
                var shortName = resource.Replace("Js2IL.Tests.", "");
                var firstLine = error.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? error;
                _output.WriteLine($"  - {shortName}: {firstLine}");
            }
        }
        else if (failures.Count > 10)
        {
            _output.WriteLine($"Compilation failures: {failures.Count} (showing first 10)");
            foreach (var (resource, error) in failures.Take(10))
            {
                var shortName = resource.Replace("Js2IL.Tests.", "");
                var firstLine = error.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? error;
                _output.WriteLine($"  - {shortName}: {firstLine}");
            }
        }

        // This test always passes - it's for reporting
        Assert.True(true);
    }

    /// <summary>
    /// Reports IR pipeline metrics after running a representative set of tests.
    /// This test always passes - it's for auditing purposes.
    /// Run with: dotnet test --filter "ReportIRPipelineMetrics" --logger "console;verbosity=detailed"
    /// </summary>
    [Fact]
    public void ReportIRPipelineMetrics()
    {
        // Compile a variety of JavaScript to exercise all code paths
        var testCases = new[]
        {
            // Main method / global scope
            ("console.log('hello');", "SimpleMain"),
            ("let x = 1 + 2; console.log(x);", "MainWithVariable"),
            
            // Functions
            ("function add(a, b) { return a + b; } console.log(add(1, 2));", "FunctionWithParams"),
            ("function greet() { console.log('hi'); } greet();", "FunctionNoParams"),
            
            // Arrow functions  
            ("const f = () => { console.log('arrow'); }; f();", "ArrowBlockBody"),
            ("const g = () => 42;", "ArrowConcise"),
            ("const h = (x) => { return x * 2; };", "ArrowWithParam"),
            
            // Classes
            ("class Foo { constructor() { } } new Foo();", "ClassEmptyCtor"),
            ("class Bar { constructor(x) { this.x = x; } } new Bar(1);", "ClassCtorWithParam"),
            ("class Baz { method() { return 1; } } new Baz().method();", "ClassMethod"),
            
            // Nested functions
            ("function outer() { function inner() { } inner(); } outer();", "NestedFunction"),
            
            // Closures
            ("function make() { let x = 1; return () => x; } make()();", "Closure"),
        };

        foreach (var (js, name) in testCases)
        {
            try
            {
                CompileJavaScript(js, name);
            }
            catch (Exception ex)
            {
                _output.WriteLine($"[{name}] Compilation failed: {ex.Message}");
            }
        }

        var stats = IRPipelineMetrics.GetStats();
        _output.WriteLine("");
        _output.WriteLine(stats.ToString());
        _output.WriteLine("");
        
        // Report detailed breakdown
        _output.WriteLine("Goal: 100% IR pipeline adoption (0 legacy fallbacks)");
        _output.WriteLine($"Current legacy fallbacks: {stats.TotalFallbacks}");
        
        // This test always passes - it's for reporting
        Assert.True(true);
    }

    private void CompileJavaScript(string js, string testName)
    {
        var testFilePath = Path.Combine(_outputPath, $"{testName}.js");
        
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(testFilePath, js);

        var options = new CompilerOptions
        {
            OutputDirectory = _outputPath
        };

        var serviceProvider = CompilerServices.BuildServiceProvider(options, mockFileSystem);
        var compiler = serviceProvider.GetRequiredService<Compiler>();
        
        compiler.Compile(testFilePath);
    }
}
