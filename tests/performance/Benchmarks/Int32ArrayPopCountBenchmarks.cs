using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[JsonExporterAttribute.FullCompressed]
public class Int32ArrayPopCountBenchmarks
{
    private const int BitCount = 1_000_003;
    private Int32Array _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        _words = new Int32Array((BitCount + 31) / 32d);
        for (var word = 0; word < _words.length; word++)
        {
            _words[word] = word % 3 == 0 ? -1 : word % 3 == 1 ? 0 : 0x7fffffff;
        }
        if (PerBit() != PopCount())
        {
            throw new InvalidOperationException("The PopCount control does not match per-bit counting.");
        }
    }

    [Benchmark(Baseline = true)]
    public double PerBit()
    {
        double clear = 0;
        for (var index = 1; index < BitCount; index++)
        {
            if (((int)_words[(double)((uint)index >> 5)] & (1 << (index & 31))) == 0)
            {
                clear++;
            }
        }
        return clear;
    }

    [Benchmark]
    public double PopCount() => Int32Array.CountZeroBitsOrNegative(_words, 1, BitCount);
}
