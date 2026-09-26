using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[JsonExporterAttribute.FullCompressed]
[Config(typeof(Int32ArrayVectorOrBenchmarkConfig))]
public class Int32ArrayVectorOrBenchmarks
{
    private const int ElementCount = 4096;
    private const int Mask = unchecked((int)0x80000801);
    private Int32Array _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        _words = new Int32Array(ElementCount);
        for (var i = 0; i < ElementCount; i++)
        {
            _words[i] = i;
        }
    }

    [Benchmark(Baseline = true)]
    public double Scalar()
    {
        if (!Int32Array.TryOrRange(
            _words, 1, ElementCount - 1, Mask, allowVector: false))
        {
            throw new InvalidOperationException("Expected contiguous storage.");
        }
        return _words[ElementCount / 2];
    }

    [Benchmark]
    public double Accelerated()
    {
        if (!Int32Array.TryOrRange(_words, 1, ElementCount - 1, Mask))
        {
            throw new InvalidOperationException("Expected contiguous storage.");
        }
        return _words[ElementCount / 2];
    }
}

public sealed class Int32ArrayVectorOrBenchmarkConfig : ManualConfig
{
    public Int32ArrayVectorOrBenchmarkConfig()
    {
        AddJob(Job.ShortRun.WithId("IntrinsicsOn"));
        AddJob(Job.ShortRun.WithId("IntrinsicsOff")
            .WithEnvironmentVariable("DOTNET_EnableHWIntrinsic", "0"));
    }
}
