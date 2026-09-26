using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace JavaScriptRuntime;

internal readonly record struct PrimeMaskTraffic(
    long WordsRead,
    long WordsWritten,
    long MaskWordsInitialized,
    long MaskWordsConstructed);

internal static class PrimeRepeatingMaskPrototype
{
    public static bool TryApplyOriginal(
        Int32Array? words, int rangeStart, int step, int rangeStop, out PrimeMaskTraffic traffic)
    {
        traffic = default;
        if (!TryGetEligibleElements(words, rangeStart, step, rangeStop, out var elements))
        {
            return false;
        }

        var stopWord = rangeStop >> 5;
        long accesses = 0;
        for (var j = 0; j < 32; j++)
        {
            var index = (long)rangeStart + (long)j * step;
            var bit = 1 << ((int)index & 31);
            for (var word = index >> 5; word <= stopWord; word += step)
            {
                if (word >= elements.Length)
                {
                    break;
                }

                elements[(int)word] |= bit;
                accesses++;
            }
        }

        traffic = new PrimeMaskTraffic(accesses, accesses, 0, 0);
        return true;
    }

    public static bool TryApplyBlocks(
        Int32Array? words, int rangeStart, int step, int rangeStop, bool useVector,
        out PrimeMaskTraffic traffic)
    {
        traffic = default;
        if (!TryGetEligibleElements(words, rangeStart, step, rangeStop, out var elements))
        {
            return false;
        }

        var mask = BuildMask(rangeStart, step, out var secondBlockFirstWordBit);
        long reads = 0;
        long writes = 0;
        var firstWord = rangeStart >> 5;
        var lastWord = System.Math.Min(rangeStop >> 5, elements.Length - 1);
        for (long block = firstWord; block <= lastWord; block += step)
        {
            var count = (int)System.Math.Min((long)step, lastWord - block + 1);
            var offset = 0;
            if (block == firstWord && secondBlockFirstWordBit != 0)
            {
                var firstMask = mask[0] & ~secondBlockFirstWordBit;
                if (firstMask != 0)
                {
                    elements[(int)block] |= firstMask;
                    reads++;
                    writes++;
                }
                offset = 1;
            }

            if (useVector && Vector128.IsHardwareAccelerated)
            {
                ref var wordRef = ref MemoryMarshal.GetReference(elements);
                ref var maskRef = ref MemoryMarshal.GetArrayDataReference(mask);
                for (; offset <= count - Vector128<int>.Count; offset += Vector128<int>.Count)
                {
                    var wordIndex = (nuint)(block + offset);
                    var value = Vector128.LoadUnsafe(ref wordRef, wordIndex);
                    var pattern = Vector128.LoadUnsafe(ref maskRef, (nuint)offset);
                    Vector128.BitwiseOr(value, pattern).StoreUnsafe(ref wordRef, wordIndex);
                    reads += Vector128<int>.Count;
                    writes += Vector128<int>.Count;
                }
            }

            for (; offset < count; offset++)
            {
                var bitMask = mask[offset];
                if (bitMask != 0)
                {
                    elements[(int)(block + offset)] |= bitMask;
                    reads++;
                    writes++;
                }
            }
        }

        traffic = new PrimeMaskTraffic(reads, writes, step, 32);
        return true;
    }

    internal static bool TryBuildMask(int rangeStart, int step, int rangeStop, out int[] mask)
    {
        mask = [];
        if (!IsEligibleRange(rangeStart, step, rangeStop))
        {
            return false;
        }

        mask = BuildMask(rangeStart, step, out _);
        return true;
    }

    private static int[] BuildMask(int rangeStart, int step, out int secondBlockFirstWordBit)
    {
        var firstWord = rangeStart >> 5;
        var mask = new int[step];
        secondBlockFirstWordBit = 0;
        for (var j = 0; j < 32; j++)
        {
            var index = (long)rangeStart + (long)j * step;
            var word = (int)(index >> 5);
            var bit = 1 << ((int)index & 31);
            var offset = (word - firstWord) % step;
            mask[offset] |= bit;
            if (word >= (long)firstWord + step)
            {
                secondBlockFirstWordBit |= bit;
            }
        }

        return mask;
    }

    private static bool TryGetEligibleElements(
        Int32Array? words, int rangeStart, int step, int rangeStop, out Span<int> elements)
    {
        elements = default;
        return IsEligibleRange(rangeStart, step, rangeStop)
            && words is not null
            && words.GetType() == typeof(Int32Array)
            && words.buffer is not SharedArrayBuffer
            && words.TryGetContiguousElements(out elements);
    }

    private static bool IsEligibleRange(int rangeStart, int step, int rangeStop)
        => rangeStart >= 0 && step > 16
            && (long)rangeStart + 32L * step <= rangeStop;
}
