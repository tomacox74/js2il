using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using Benchmarks.Runtimes;

namespace Benchmarks;

/// <summary>
/// Cross-runtime JavaScript benchmark suite comparing Node.js, Jint, and js2il.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class JavaScriptRuntimeBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly JintRuntime _jintRuntime = new();
    private readonly NodeJsRuntime _nodeRuntime = new();
    private readonly Js2ILRuntime _js2ilRuntime = new();

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
            else
            {
                Console.WriteLine($"Warning: Script not found: {path}");
            }
        }

        if (_scripts.Count == 0)
        {
            throw new InvalidOperationException("No benchmark scripts found!");
        }
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames()
    {
        if (_scripts.Count > 0)
        {
            return _scripts.Keys;
        }

        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        if (!Directory.Exists(scriptsDir))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(scriptsDir, "*.js")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name)!;
    }

    [Benchmark(Description = "Node.js")]
    public void NodeJs()
    {
        var script = _scripts[ScriptName];
        var result = _nodeRuntime.Execute(script, $"{ScriptName}.js");
        
        if (!result.Success)
        {
            throw new Exception($"Node.js execution failed: {result.Error}");
        }
    }

    [Benchmark(Description = "Jint")]
    public void Jint()
    {
        var script = _scripts[ScriptName];
        var result = _jintRuntime.Execute(script, $"{ScriptName}.js");
        
        if (!result.Success)
        {
            throw new Exception($"Jint execution failed: {result.Error}");
        }
    }

    [Benchmark(Description = "js2il (compile+execute)")]
    public void Js2IL_Total()
    {
        var script = _scripts[ScriptName];
        var result = _js2ilRuntime.Execute(script, $"{ScriptName}.js");
        
        if (!result.Success)
        {
            throw new Exception($"js2il execution failed: {result.Error}");
        }
    }
}
