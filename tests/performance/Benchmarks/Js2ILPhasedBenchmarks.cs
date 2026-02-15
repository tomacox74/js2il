using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using System.Diagnostics;
using Js2IL;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

/// <summary>
/// js2il-specific benchmark that separates compile and execute phases.
/// This provides detailed timing for AOT compilation vs execution.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
public class Js2ILPhasedBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly Dictionary<string, string> _compiledPaths = new();
    private string _tempDir = "";

    [GlobalSetup]
    public void Setup()
    {
        // Load all benchmark scripts
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        
        var scriptFiles = new[]
        {
            "minimal.js",
            "evaluation.js",
            "evaluation-modern.js",
            "stopwatch.js",
            "array-stress.js"
        };

        foreach (var scriptFile in scriptFiles)
        {
            var path = Path.Combine(scriptsDir, scriptFile);
            if (File.Exists(path))
            {
                var scriptName = Path.GetFileNameWithoutExtension(scriptFile);
                _scripts[scriptName] = File.ReadAllText(path);
            }
        }

        // Create temp directory for compiled outputs
        _tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmarks-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        // Pre-compile all scripts for execution-only benchmarks
        foreach (var kvp in _scripts)
        {
            var scriptName = kvp.Key;
            var scriptContent = kvp.Value;
            var tempScriptFile = Path.Combine(_tempDir, $"{scriptName}.js");
            File.WriteAllText(tempScriptFile, scriptContent);

            var outputPath = Path.Combine(_tempDir, scriptName);
            
            // Build compiler with service provider
            var options = new CompilerOptions { OutputDirectory = outputPath };
            var serviceProvider = CompilerServices.BuildServiceProvider(options);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            compiler.Compile(tempScriptFile, scriptName);

            // The DLL is placed directly in outputPath (not in a nested directory)
            // It's named after the input file (scriptName.js -> scriptName.dll)
            _compiledPaths[scriptName] = Path.Combine(outputPath, $"{scriptName}.dll");
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames() => _scripts.Keys;

    [Benchmark(Description = "js2il compile")]
    public void Js2IL_Compile()
    {
        var script = _scripts[ScriptName];
        var tempScriptFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");
        var tempOutputDir = Path.Combine(Path.GetTempPath(), $"js2il-compile-{Guid.NewGuid()}");

        try
        {
            File.WriteAllText(tempScriptFile, script);

            // Build compiler with service provider
            var options = new CompilerOptions { OutputDirectory = tempOutputDir };
            var serviceProvider = CompilerServices.BuildServiceProvider(options);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            compiler.Compile(tempScriptFile, ScriptName);
        }
        finally
        {
            try
            {
                if (File.Exists(tempScriptFile))
                    File.Delete(tempScriptFile);
                if (Directory.Exists(tempOutputDir))
                    Directory.Delete(tempOutputDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [Benchmark(Description = "js2il execute (pre-compiled)")]
    public void Js2IL_ExecuteOnly()
    {
        var dllPath = _compiledPaths[ScriptName];

        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{dllPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        if (process == null)
        {
            throw new Exception("Failed to start dotnet process");
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new Exception($"js2il execution failed: {error}");
        }
    }
}
