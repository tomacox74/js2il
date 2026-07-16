using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Benchmarks;

/// <summary>
/// Tracks the steady-state cost of ordinary-object reads and writes through the
/// generic runtime dispatch used by extensible JsObject internal operations.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[ShortRunJob]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "StdDev")]
public class ObjectInternalOperationsBenchmarks
{
    private readonly JavaScriptRuntime.JsObject _readTarget = new();
    private readonly JavaScriptRuntime.JsObject _writeTarget = new();

    [GlobalSetup]
    public void Setup()
    {
        JavaScriptRuntime.Object.SetProperty(_readTarget, "value", 42d);
        JavaScriptRuntime.Object.SetProperty(_writeTarget, "value", 0d);
    }

    [Benchmark(Description = "Ordinary JsObject read")]
    public object? Read()
        => JavaScriptRuntime.Object.GetProperty(_readTarget, "value");

    [Benchmark(Description = "Ordinary JsObject write")]
    public object? Write()
        => JavaScriptRuntime.Object.SetProperty(_writeTarget, "value", 42d);
}
