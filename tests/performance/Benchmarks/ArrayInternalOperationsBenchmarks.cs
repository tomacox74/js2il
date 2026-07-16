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
public class ArrayInternalOperationsBenchmarks
{
    private const int ElementCount = 256;
    private readonly object?[] _source = Enumerable.Range(0, ElementCount)
        .Select(static value => (object?)(double)value)
        .ToArray();
    private JavaScriptRuntime.Array _readTarget = null!;

    [GlobalSetup]
    public void Setup()
    {
        _readTarget = new JavaScriptRuntime.Array(_source);
    }

    [Benchmark(Description = "Dense Array construction")]
    public JavaScriptRuntime.Array Construct()
        => new(_source);

    [Benchmark(Description = "Dense Array indexed reads")]
    public object? Read()
    {
        object? value = null;
        for (int index = 0; index < ElementCount; index++)
        {
            value = JavaScriptRuntime.ObjectRuntime.GetItem(_readTarget, (double)index);
        }

        return value;
    }
}
