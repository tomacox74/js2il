using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks;

/// <summary>
/// The Kraken performance benchmarks consist of a the test script and the data script.
/// compile time and script load time and data load time are all excluded from the measurements.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class KrackenBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios", "kracken-1.1");
        var astarTestScript = Path.Combine(scriptsDir,  "ai-astar.js");
        var astarDataScript = Path.Combine(scriptsDir,  "ai-astar-data.js");
        
    }
}