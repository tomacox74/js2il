using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class Int32ArrayBackingBenchmarks
{
    private const int ElementCount = 1024;
    private JavaScriptRuntime.Int32Array _view = null!;

    [GlobalSetup]
    public void Setup()
    {
        _view = new JavaScriptRuntime.Int32Array(ElementCount);
        if (!_view.TryGetContiguousElements(out _))
        {
            throw new InvalidOperationException("Expected a fixed contiguous Int32Array.");
        }
    }

    [Benchmark(Baseline = true, Description = "Byte span + BinaryPrimitives")]
    public long ByteSpans()
    {
        var bytes = _view.buffer.RawBytes;
        long checksum = 0;
        for (var index = 0; index < ElementCount; index++)
        {
            var element = bytes.AsSpan(index * sizeof(int), sizeof(int));
            var value = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(element)
                : BinaryPrimitives.ReadInt32BigEndian(element);
            checksum += value;
            if (BitConverter.IsLittleEndian)
            {
                BinaryPrimitives.WriteInt32LittleEndian(element, value | (1 << (index & 31)));
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(element, value | (1 << (index & 31)));
            }
        }

        return checksum;
    }

    [Benchmark(Description = "Contiguous int span")]
    public long ContiguousSpan()
    {
        if (!_view.TryGetContiguousElements(out var elements))
        {
            throw new InvalidOperationException("Backing storage changed during the benchmark.");
        }

        long checksum = 0;
        for (var index = 0; index < ElementCount; index++)
        {
            var value = elements[index];
            checksum += value;
            elements[index] = value | (1 << (index & 31));
        }

        return checksum;
    }

    [Benchmark(Description = "Compiled-style indexed access")]
    public long IndexedAccess()
    {
        long checksum = 0;
        for (var index = 0; index < ElementCount; index++)
        {
            var value = _view[index];
            checksum += (int)value;
            _view[index] = (int)value | (1 << (index & 31));
        }

        return checksum;
    }
}
