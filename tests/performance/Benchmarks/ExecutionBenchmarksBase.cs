using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public abstract class ExecutionBenchmarksBase
{
    public static string? ScenarioFilter { get; set; }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public abstract IEnumerable<string> ScriptNames();

    protected static bool MatchesScenarioFilter(params string[] scenarioNames)
    {
        if (string.IsNullOrWhiteSpace(ScenarioFilter))
        {
            return true;
        }

        return scenarioNames.Any(scenarioName =>
            string.Equals(scenarioName, ScenarioFilter, StringComparison.Ordinal));
    }
}
