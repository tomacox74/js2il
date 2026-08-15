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

    [Benchmark(Description = "Boolean wrapper with initialized prototype")]
    public JavaScriptRuntime.Boolean ConstructBooleanWrapper()
        => new(true);

    [Benchmark(Description = "Date wrapper with initialized prototype")]
    public JavaScriptRuntime.Date ConstructDateWrapper()
        => new(0d);

    [Benchmark(Description = "RegExp wrapper with initialized prototype")]
    public JavaScriptRuntime.RegExp ConstructRegExpWrapper()
        => new("a", "g");

    [Benchmark(Description = "Map with initialized prototype")]
    public JavaScriptRuntime.Map ConstructMap()
        => new();

    [Benchmark(Description = "Set with initialized prototype")]
    public JavaScriptRuntime.Set ConstructSet()
        => new();

    [Benchmark(Description = "WeakMap with initialized prototype")]
    public JavaScriptRuntime.WeakMap ConstructWeakMap()
        => new();

    [Benchmark(Description = "WeakSet with initialized prototype")]
    public JavaScriptRuntime.WeakSet ConstructWeakSet()
        => new();

    [Benchmark(Description = "Promise with initialized prototype")]
    public JavaScriptRuntime.Promise ConstructPromise()
        => (JavaScriptRuntime.Promise)JavaScriptRuntime.Promise.resolve(null)!;

    [Benchmark(Description = "Ordinary object with initialized prototype")]
    public JavaScriptRuntime.JsObject ConstructOrdinaryObject()
    {
        var result = new JavaScriptRuntime.JsObject();
        JavaScriptRuntime.PrototypeChain.InitializePrototype(result, _prototype);
        return result;
    }
}
