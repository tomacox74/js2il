using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Benchmarks;

[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class PrototypeStorageBenchmarks
{
    private readonly JavaScriptRuntime.JsObject _prototype = new();

    [Benchmark(Description = "Array with initialized prototype")]
    public JavaScriptRuntime.Array ConstructArray()
        => new();

    [Benchmark(Description = "Ordinary object with initialized prototype")]
    public JavaScriptRuntime.JsObject ConstructOrdinaryObject()
    {
        var result = new JavaScriptRuntime.JsObject();
        JavaScriptRuntime.PrototypeChain.InitializePrototype(result, _prototype);
        return result;
    }
}
