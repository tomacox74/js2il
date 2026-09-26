using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[JsonExporterAttribute.FullCompressed]
[Config(typeof(PrimeRepeatingMaskBenchmarkConfig))]
public class PrimeRepeatingMaskBenchmarks
{
    private const int RangeStop = 500_000;
    private Int32Array _original = null!;
    private Int32Array _scalarBlocks = null!;
    private Int32Array _vectorBlocks = null!;
    private int _rangeStart;

    [Params(17, 31, 61, 127, 251, 509)]
    public int Step { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var factor = (Step - 1) / 2;
        _rangeStart = 2 * factor * factor + 2 * factor;
        var wordCount = 1 + (RangeStop >> 5);
        _original = new Int32Array(wordCount);
        _scalarBlocks = new Int32Array(wordCount);
        _vectorBlocks = new Int32Array(wordCount);
        if (!PrimeRepeatingMaskPrototype.TryApplyOriginal(
                _original, _rangeStart, Step, RangeStop, out _)
            || !PrimeRepeatingMaskPrototype.TryApplyBlocks(
                _scalarBlocks, _rangeStart, Step, RangeStop, useVector: false, out _)
            || !PrimeRepeatingMaskPrototype.TryApplyBlocks(
                _vectorBlocks, _rangeStart, Step, RangeStop, useVector: true, out _))
        {
            throw new InvalidOperationException("The repeating-mask prototype rejected a Prime workload.");
        }

        for (var index = 0; index < wordCount; index++)
        {
            if (_original[index] != _scalarBlocks[index]
                || _original[index] != _vectorBlocks[index])
            {
                throw new InvalidOperationException(
                    $"The Prime mask algorithms disagree at word {index}, step {Step}.");
            }
        }
    }

    [Benchmark(Baseline = true)]
    public double OriginalStrided()
    {
        if (!PrimeRepeatingMaskPrototype.TryApplyOriginal(
                _original, _rangeStart, Step, RangeStop, out _))
        {
            throw new InvalidOperationException("The original large-step range is ineligible.");
        }
        return _original[RangeStop >> 5];
    }

    [Benchmark]
    public double ScalarBlocks()
    {
        if (!PrimeRepeatingMaskPrototype.TryApplyBlocks(
                _scalarBlocks, _rangeStart, Step, RangeStop, useVector: false, out _))
        {
            throw new InvalidOperationException("The scalar block range is ineligible.");
        }
        return _scalarBlocks[RangeStop >> 5];
    }

    [Benchmark]
    public double VectorBlocks()
    {
        if (!PrimeRepeatingMaskPrototype.TryApplyBlocks(
                _vectorBlocks, _rangeStart, Step, RangeStop, useVector: true, out _))
        {
            throw new InvalidOperationException("The vector block range is ineligible.");
        }
        return _vectorBlocks[RangeStop >> 5];
    }

    [Benchmark]
    public int MaskConstruction()
    {
        if (!PrimeRepeatingMaskPrototype.TryBuildMask(
                _rangeStart, Step, RangeStop, out var mask))
        {
            throw new InvalidOperationException("The repeating mask could not be constructed.");
        }
        return mask.Length;
    }
}

public sealed class PrimeRepeatingMaskBenchmarkConfig : ManualConfig
{
    public PrimeRepeatingMaskBenchmarkConfig()
    {
        AddJob(Job.ShortRun.WithId("IntrinsicsOn"));
        AddJob(Job.ShortRun.WithId("IntrinsicsOff")
            .WithEnvironmentVariable("DOTNET_EnableHWIntrinsic", "0"));
    }
}
