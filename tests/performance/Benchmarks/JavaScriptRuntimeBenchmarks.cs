using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using Benchmarks.Runtimes;

namespace Benchmarks;

/// <summary>
/// Cross-runtime JavaScript benchmark suite comparing hosted .NET JavaScript runtimes and jroc.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class JavaScriptRuntimeBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly Dictionary<string, string> _scenarioKeyToScriptName = new(StringComparer.Ordinal);
    private readonly JintRuntime _jintRuntime = new();
    private readonly OkojoRuntime _okojoRuntime = new();
    private readonly ClearScriptRuntime _clearScriptRuntime = new();
    private readonly JrocRuntime _jrocRuntime = new();

    [GlobalSetup]
    public void Setup()
    {
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");

        foreach (var scenario in BenchmarkScenarioCatalog.LoadScenarios(scriptsDir))
        {
            _scripts[scenario.Key] = scenario.Content;
            _scenarioKeyToScriptName[scenario.Key] = scenario.ScriptName;
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
            return _scripts.Keys.OrderBy(name => name, StringComparer.Ordinal);
        }

        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        return BenchmarkScenarioCatalog.LoadScenarios(scriptsDir)
            .Select(scenario => scenario.Key);
    }

    [Benchmark(Description = "ClearScript")]
    public void ClearScript()
    {
        var script = _scripts[ScriptName];
        var result = _clearScriptRuntime.Execute(script, $"{ResolveScriptName(ScriptName)}.js");
        
        if (!result.Success)
        {
            throw new Exception($"ClearScript execution failed: {result.Error}");
        }
    }

    [Benchmark(Description = "Jint")]
    public void Jint()
    {
        var script = _scripts[ScriptName];
        var result = _jintRuntime.Execute(script, $"{ResolveScriptName(ScriptName)}.js");
        
        if (!result.Success)
        {
            throw new Exception($"Jint execution failed: {result.Error}");
        }
    }

    [Benchmark(Description = "Okojo")]
    public void Okojo()
    {
        var script = _scripts[ScriptName];
        var result = _okojoRuntime.Execute(script, $"{ResolveScriptName(ScriptName)}.js");
        
        if (!result.Success)
        {
            throw new Exception($"Okojo execution failed: {result.Error}");
        }
    }

    [Benchmark(Description = "jroc (compile+execute)")]
    public void Jroc_Total()
    {
        var script = _scripts[ScriptName];
        var result = _jrocRuntime.Execute(script, $"{ResolveScriptName(ScriptName)}.js");
        
        if (!result.Success)
        {
            throw new Exception($"jroc execution failed: {result.Error}");
        }
    }

    private string ResolveScriptName(string scenarioKey)
    {
        return _scenarioKeyToScriptName.TryGetValue(scenarioKey, out var scriptName)
            ? scriptName
            : scenarioKey;
    }
}
