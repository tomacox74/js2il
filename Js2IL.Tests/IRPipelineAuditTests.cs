using Js2IL.IR;
using Js2IL.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Js2IL.Tests;

/// <summary>
/// Audit test to track progress of IR pipeline adoption.
/// Runs all execution tests and reports how many methods compiled via IR vs legacy.
/// </summary>
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
    /// Reports IR pipeline metrics after running a representative set of tests.
    /// This test always passes - it's for auditing purposes.
    /// Run with: dotnet test --filter "IRPipelineAuditTests" -v n
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
