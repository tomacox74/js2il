using BenchmarkDotNet.Attributes;
using JavaScriptRuntime;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class Int32ArrayFirstZeroBitBenchmarks
{
    private Int32Array _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        _words = new Int32Array(32d);
        for (var word = 0; word < 31; word++)
        {
            _words[word] = -1;
        }
        _words[31] = 0x7fffffff;
    }

    [Benchmark(Baseline = true)]
    public double PerBit()
    {
        double index = 7;
        while ((((int)_words[(double)((uint)index >> 5)]) & (1 << ((int)index & 31))) != 0)
        {
            index++;
        }
        return index;
    }

    [Benchmark]
    public double WordAtATime() => RuntimeServices.FindFirstZeroBitOrNegative(_words, 7);
}
