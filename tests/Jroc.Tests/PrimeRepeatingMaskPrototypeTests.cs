using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class PrimeRepeatingMaskPrototypeTests
{
    [Theory]
    [InlineData(31, 17, 575, 20)]
    [InlineData(0, 17, 544, 18)]
    [InlineData(30, 31, 1_023, 33)]
    [InlineData(63, 32, 1_087, 35)]
    [InlineData(1, 33, 1_057, 32)]
    [InlineData(127, 63, 2_143, 65)]
    [InlineData(16, 65, 2_096, 64)]
    [InlineData(31, 19, 641, 1)]
    [InlineData(255, 127, 8_383, 0)]
    public void BoundaryAndClippedViewsMatchReferenceBytes(
        int start, int step, int stop, int wordCount)
    {
        AssertEquivalent(start, step, stop, wordCount, viewOffsetWords: 3);
    }

    [Fact]
    public void ScalarAndVectorMatchIndependentReferenceForVariedOffsetsAndTails()
    {
        var random = new Random(2164);
        for (var attempt = 0; attempt < 400; attempt++)
        {
            var start = random.Next(0, 128);
            var step = random.Next(17, 145);
            var stop = start + 32 * step + random.Next(0, 650);
            var wordCount = random.Next(0, (stop >> 5) + 8);
            AssertEquivalent(start, step, stop, wordCount, random.Next(0, 4));
        }
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1_000)]
    [InlineData(10_000)]
    [InlineData(100_000)]
    [InlineData(1_000_000)]
    [InlineData(10_000_000)]
    [InlineData(100_000_000)]
    public void PrimeValidationSizesMatchOriginalLargeStepPathWhereEligible(int sieveSize)
    {
        var stop = sieveSize / 2;
        var count = 1 + (stop >> 5);
        foreach (var step in new[] { 17, 19, 37, 97, 521 })
        {
            var factor = (step - 1) / 2;
            var start = 2 * factor * factor + 2 * factor;
            if (start + 32 * step <= stop)
            {
                AssertEquivalent(start, step, stop, count, viewOffsetWords: 1);
            }
            else
            {
                var words = new Int32Array((double)count);
                words[0] = 42;
                Assert.False(PrimeRepeatingMaskPrototype.TryApplyOriginal(
                    words, start, step, stop, out var originalTraffic));
                Assert.False(PrimeRepeatingMaskPrototype.TryApplyBlocks(
                    words, start, step, stop, true, out var blockTraffic));
                Assert.Equal(default, originalTraffic);
                Assert.Equal(default, blockTraffic);
                Assert.Equal(42d, words[0]);
            }
        }
    }

    [Theory]
    [InlineData(10, 4)]
    [InlineData(100, 25)]
    [InlineData(1_000, 168)]
    [InlineData(10_000, 1_229)]
    [InlineData(100_000, 9_592)]
    [InlineData(1_000_000, 78_498)]
    [InlineData(10_000_000, 664_579)]
    [InlineData(100_000_000, 5_761_455)]
    public void FullPrimeSieveRetainsEveryValidationCount(int sieveSize, int expectedCount)
    {
        var bitLength = sieveSize / 2;
        var words = new Int32Array(1d + ((1 + bitLength) >> 5));
        Assert.True(words.TryGetContiguousElements(out var elements));
        var factor = 1;
        var limit = (int)System.Math.Ceiling(System.Math.Sqrt(bitLength));
        while (factor < limit)
        {
            var step = factor * 2 + 1;
            var start = 2 * factor * factor + 2 * factor;
            if (!PrimeRepeatingMaskPrototype.TryApplyBlocks(
                    words, start, step, bitLength, useVector: true, out _))
            {
                for (var index = start; index < bitLength; index += step)
                {
                    elements[index >> 5] |= 1 << (index & 31);
                }
            }

            do
            {
                factor++;
            } while ((elements[factor >> 5] & (1 << (factor & 31))) != 0);
        }

        var count = 1;
        for (var index = 1; index < bitLength; index++)
        {
            if ((elements[index >> 5] & (1 << (index & 31))) == 0)
            {
                count++;
            }
        }
        Assert.Equal(expectedCount, count);
    }

    [Theory]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(63)]
    [InlineData(127)]
    [InlineData(255)]
    [InlineData(511)]
    public void BenchmarkStepsMatchAtFiveHundredThousandRepresentedBits(int step)
    {
        const int stop = 500_000;
        var factor = (step - 1) / 2;
        var start = 2 * factor * factor + 2 * factor;
        Assert.True(PrimeRepeatingMaskPrototype.TryBuildMask(start, step, stop, out var mask));
        Assert.Equal(step, mask.Length);
        AssertEquivalent(start, step, stop, 1 + (stop >> 5), viewOffsetWords: 0);
    }

    [Theory]
    [InlineData(17, 29_406, 15_622, 15_622)]
    [InlineData(31, 16_115, 15_611, 15_611)]
    [InlineData(61, 8_167, 8_167, 15_312)]
    [InlineData(127, 3_874, 3_874, 15_008)]
    [InlineData(251, 1_867, 1_867, 14_468)]
    [InlineData(509, 728, 728, 11_556)]
    public void BenchmarkTrafficCountsExposeZeroMaskScanCost(
        int step, long originalWords, long scalarWords, long acceleratedWords)
    {
        const int stop = 500_000;
        var factor = (step - 1) / 2;
        var start = 2 * factor * factor + 2 * factor;
        var wordCount = 1d + (stop >> 5);
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyOriginal(
            new Int32Array(wordCount), start, step, stop, out var original));
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyBlocks(
            new Int32Array(wordCount), start, step, stop, false, out var scalar));
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyBlocks(
            new Int32Array(wordCount), start, step, stop, true, out var vector));

        Assert.Equal(originalWords, original.WordsRead);
        Assert.Equal(originalWords, original.WordsWritten);
        Assert.Equal(scalarWords, scalar.WordsRead);
        Assert.Equal(scalarWords, scalar.WordsWritten);
        Assert.Equal(System.Runtime.Intrinsics.Vector128.IsHardwareAccelerated
            ? acceleratedWords : scalarWords, vector.WordsRead);
        Assert.Equal(vector.WordsRead, vector.WordsWritten);
        Assert.Equal(step, scalar.MaskWordsInitialized);
        Assert.Equal(32, scalar.MaskWordsConstructed);
    }

    [Fact]
    public void FirstPartialBlockOmitsSeedThatStartsInSecondBlock()
    {
        const int start = 31;
        const int step = 17;
        const int stop = start + 32 * step;
        Assert.True(PrimeRepeatingMaskPrototype.TryBuildMask(start, step, stop, out var mask));
        Assert.Equal(step, mask.Length);
        Assert.Equal((1 << 31) | (1 << 14), mask[0]);

        var original = new Int32Array(18d);
        var scalar = new Int32Array(18d);
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyOriginal(
            original, start, step, stop, out var originalTraffic));
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyBlocks(
            scalar, start, step, stop, false, out var scalarTraffic));
        Assert.Equal(1 << 31, (int)scalar[0]);
        Assert.Equal((1 << 31) | (1 << 14), (int)scalar[17]);
        Assert.Equal(original.buffer.RawBytes, scalar.buffer.RawBytes);
        Assert.Equal(new PrimeMaskTraffic(33, 33, 0, 0), originalTraffic);
        Assert.Equal(new PrimeMaskTraffic(18, 18, 17, 32), scalarTraffic);
    }

    [Fact]
    public void MaskSetupIsIndependentOfTargetAndRejectsIneligibleRanges()
    {
        Assert.True(PrimeRepeatingMaskPrototype.TryBuildMask(5, 33, 1_061, out var mask));
        Assert.Equal(33, mask.Length);
        Assert.Equal(32, mask.Sum(word => System.Numerics.BitOperations.PopCount((uint)word)));

        foreach (var (start, step, stop) in new[]
        {
            (-1, 17, 600), (0, 16, 1_000), (0, 0, 1_000),
            (0, 17, 543), (100, 17, 10), (int.MaxValue, 17, int.MaxValue),
            (0, int.MaxValue, int.MaxValue)
        })
        {
            Assert.False(PrimeRepeatingMaskPrototype.TryBuildMask(start, step, stop, out mask));
            Assert.Empty(mask);
            var words = new Int32Array(2d);
            words[0] = 123;
            Assert.False(PrimeRepeatingMaskPrototype.TryApplyOriginal(
                words, start, step, stop, out var originalTraffic));
            Assert.False(PrimeRepeatingMaskPrototype.TryApplyBlocks(
                words, start, step, stop, true, out var blockTraffic));
            Assert.Equal(default, originalTraffic);
            Assert.Equal(default, blockTraffic);
            Assert.Equal(123d, words[0]);
        }
    }

    [Fact]
    public void UnsafeTargetsAreRejectedWithoutMutation()
    {
        const int start = 31;
        const int step = 17;
        const int stop = 575;

        CheckRejected(null);
        CheckRejected(new DerivedInt32Array());

        var resizableBuffer = new ArrayBuffer(80d, new { maxByteLength = 160d });
        resizableBuffer.RawBytes.AsSpan().Fill(0x57);
        CheckRejected(new Int32Array(resizableBuffer, 4d, 12d));
        CheckRejected(new Int32Array(resizableBuffer, 4d));
        resizableBuffer.resize(4d);
        CheckRejected(new Int32Array(resizableBuffer, 0d));

        var sharedBuffer = new SharedArrayBuffer(80d);
        sharedBuffer.RawBytes.AsSpan().Fill(0x57);
        CheckRejected(new Int32Array(sharedBuffer));
        var growableBuffer = new SharedArrayBuffer(80d, new { maxByteLength = 160d });
        growableBuffer.RawBytes.AsSpan().Fill(0x57);
        CheckRejected(new Int32Array(growableBuffer));

        var detachedBuffer = new ArrayBuffer(80d);
        var detached = new Int32Array(detachedBuffer);
        detachedBuffer.Detach();
        CheckRejected(detached);

        void CheckRejected(Int32Array? words)
        {
            var before = words?.buffer.RawBytes.ToArray();
            Assert.False(PrimeRepeatingMaskPrototype.TryApplyOriginal(
                words, start, step, stop, out var originalTraffic));
            Assert.False(PrimeRepeatingMaskPrototype.TryApplyBlocks(
                words, start, step, stop, false, out var scalarTraffic));
            Assert.False(PrimeRepeatingMaskPrototype.TryApplyBlocks(
                words, start, step, stop, true, out var vectorTraffic));
            Assert.Equal(default, originalTraffic);
            Assert.Equal(default, scalarTraffic);
            Assert.Equal(default, vectorTraffic);
            if (words is not null)
            {
                Assert.Equal(before, words.buffer.RawBytes);
            }
        }
    }

    private static void AssertEquivalent(
        int start, int step, int stop, int wordCount, int viewOffsetWords)
    {
        var byteCount = (wordCount + viewOffsetWords + 2) * sizeof(int);
        var referenceBuffer = new ArrayBuffer((double)byteCount);
        referenceBuffer.RawBytes.AsSpan().Fill(0xA5);
        var expected = new Int32Array(
            referenceBuffer, (double)viewOffsetWords * sizeof(int), (double)wordCount);
        var originalBuffer = CopyBuffer(referenceBuffer);
        var scalarBuffer = CopyBuffer(referenceBuffer);
        var vectorBuffer = CopyBuffer(referenceBuffer);
        var original = new Int32Array(
            originalBuffer, (double)viewOffsetWords * sizeof(int), (double)wordCount);
        var scalar = new Int32Array(
            scalarBuffer, (double)viewOffsetWords * sizeof(int), (double)wordCount);
        var vector = new Int32Array(
            vectorBuffer, (double)viewOffsetWords * sizeof(int), (double)wordCount);

        long referenceAccesses = 0;
        var stopWord = stop >> 5;
        for (var j = 0; j < 32; j++)
        {
            var index = start + j * step;
            var bit = 1 << (index & 31);
            var word = index >> 5;
            do
            {
                if (word < wordCount)
                {
                    expected[word] = (int)expected[word] | bit;
                    referenceAccesses++;
                }
                word += step;
            } while (word <= stopWord);
        }

        Assert.True(PrimeRepeatingMaskPrototype.TryApplyOriginal(
            original, start, step, stop, out var originalTraffic));
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyBlocks(
            scalar, start, step, stop, false, out var scalarTraffic));
        Assert.True(PrimeRepeatingMaskPrototype.TryApplyBlocks(
            vector, start, step, stop, true, out var vectorTraffic));
        Assert.Equal(referenceBuffer.RawBytes, originalBuffer.RawBytes);
        Assert.Equal(referenceBuffer.RawBytes, scalarBuffer.RawBytes);
        Assert.Equal(referenceBuffer.RawBytes, vectorBuffer.RawBytes);
        Assert.Equal(new PrimeMaskTraffic(referenceAccesses, referenceAccesses, 0, 0),
            originalTraffic);
        Assert.Equal((long)step, scalarTraffic.MaskWordsInitialized);
        Assert.Equal(32L, scalarTraffic.MaskWordsConstructed);
        Assert.Equal(scalarTraffic.WordsRead, scalarTraffic.WordsWritten);
        Assert.Equal(vectorTraffic.WordsRead, vectorTraffic.WordsWritten);
        Assert.Equal((long)step, vectorTraffic.MaskWordsInitialized);
        Assert.Equal(32L, vectorTraffic.MaskWordsConstructed);
    }

    private static ArrayBuffer CopyBuffer(ArrayBuffer source)
    {
        var copy = new ArrayBuffer((double)source.RawBytes.Length);
        source.RawBytes.CopyTo(copy.RawBytes, 0);
        return copy;
    }

    private sealed class DerivedInt32Array : Int32Array
    {
        public DerivedInt32Array() : base(20d) { }
    }
}
