using JavaScriptRuntime;

namespace Jroc.Tests;

public sealed class Int32ArrayContiguousTests
{
    [Fact]
    public void FixedOffsetViewSharesNativeEndianContiguousStorage()
    {
        var buffer = new ArrayBuffer(16d);
        var view = new Int32Array(buffer, 4d, 2d);

        Assert.True(view.TryGetContiguousElements(out var elements));
        Assert.Equal(2, elements.Length);
        elements[0] = unchecked((int)0x89abcdef);
        Assert.Equal((double)elements[0], view[0]);
        Assert.Equal(BitConverter.GetBytes(elements[0]), buffer.RawBytes.AsSpan(4, 4).ToArray());

        view[1] = 0x12345678;
        Assert.Equal(0x12345678, elements[1]);
        Assert.Equal(0d, new Int32Array(buffer, 0d, 1d)[0]);

        view[0] = -1;
        view[1] = 4294967297d;
        Assert.Equal(-1, elements[0]);
        Assert.Equal(1, elements[1]);
    }

    [Fact]
    public void DetachedStorageCannotExposeAContiguousView()
    {
        var buffer = new ArrayBuffer(8d);
        var view = new Int32Array(buffer);
        view[0] = 17;
        Assert.True(view.TryGetContiguousElements(out _));

        buffer.Detach();

        Assert.False(view.TryGetContiguousElements(out _));
        Assert.Equal(0d, view.length);
        Assert.Equal(0d, view[0]);
        view[0] = 99;
        Assert.Equal(0d, view[0]);
    }

    [Fact]
    public void ResizableAndGrowableStorageAlwaysUseDynamicAccess()
    {
        var rab = new ArrayBuffer(16d, new { maxByteLength = 32d });
        var fixedView = new Int32Array(rab, 4d, 2d);
        var trackingView = new Int32Array(rab, 4d);
        Assert.False(fixedView.TryGetContiguousElements(out _));
        Assert.False(trackingView.TryGetContiguousElements(out _));
        fixedView[0] = 23;
        Assert.Equal(23d, trackingView[0]);

        rab.resize(4d);
        Assert.False(fixedView.TryGetContiguousElements(out _));
        Assert.Equal(0d, fixedView.length);
        fixedView[0] = 44;
        rab.resize(16d);
        Assert.False(fixedView.TryGetContiguousElements(out _));
        Assert.Equal(0d, fixedView[0]);
        fixedView[1] = 9;
        Assert.Equal(9d, trackingView[1]);

        var shared = new SharedArrayBuffer(8d, new { maxByteLength = 16d });
        var sharedView = new Int32Array(shared);
        Assert.False(sharedView.TryGetContiguousElements(out _));
        shared.grow(16d);
        Assert.False(sharedView.TryGetContiguousElements(out _));
        sharedView[0] = 12;
        Assert.Equal(12d, sharedView[0]);
    }

    [Fact]
    public void FindFirstZeroBitChecksEveryStartingOffsetAndWordBoundary()
    {
        var words = new Int32Array(3d);
        words[0] = -1;
        words[1] = -1;
        words[2] = unchecked((int)0x7fffffff);

        for (var start = 0; start < 32; start++)
        {
            Assert.Equal(95d, Int32Array.FindFirstZeroBitOrNegative(words, start));
        }

        words[0] = unchecked((int)0x7fffffff);
        Assert.Equal(31d, Int32Array.FindFirstZeroBitOrNegative(words, 0));
        Assert.Equal(95d, Int32Array.FindFirstZeroBitOrNegative(words, 32));
        words[1] = 0b1011;
        Assert.Equal(34d, Int32Array.FindFirstZeroBitOrNegative(words, 33));
        Assert.Equal(36d, Int32Array.FindFirstZeroBitOrNegative(words, 35));
        Assert.Equal(95d, Int32Array.FindFirstZeroBitOrNegative(words, 64));
        Assert.Equal(96d, Int32Array.FindFirstZeroBitOrNegative(words, 96));
    }

    [Fact]
    public void FindFirstZeroBitRejectsUnstableStorageAndNonIntegerIndices()
    {
        var buffer = new ArrayBuffer(8d);
        var words = new Int32Array(buffer);
        foreach (var index in new[] { -1d, BitConverter.Int64BitsToDouble(long.MinValue), 0.5, double.NaN, double.PositiveInfinity, 4294967296d })
        {
            Assert.Equal(-1d, Int32Array.FindFirstZeroBitOrNegative(words, index));
        }

        Assert.Equal(-1d, Int32Array.FindFirstZeroBitOrNegative(
            new Int32Array(new ArrayBuffer(8d, new { maxByteLength = 16d })), 0));
        Assert.Equal(-1d, Int32Array.FindFirstZeroBitOrNegative(
            new Int32Array(new SharedArrayBuffer(8d)), 0));
        buffer.Detach();
        Assert.Equal(-1d, Int32Array.FindFirstZeroBitOrNegative(words, 0));
    }

    [Fact]
    public void CountZeroBitsHandlesPartialAndCrossWordRanges()
    {
        var words = new Int32Array(3d);
        Assert.Equal(0d, Int32Array.CountZeroBitsOrNegative(words, 1, 1));
        Assert.Equal(31d, Int32Array.CountZeroBitsOrNegative(words, 1, 32));
        Assert.Equal(63d, Int32Array.CountZeroBitsOrNegative(words, 1, 64));

        words[0] = -1;
        words[1] = -1;
        words[2] = unchecked((int)0x7fffffff);
        Assert.Equal(0d, Int32Array.CountZeroBitsOrNegative(words, 1, 95));
        Assert.Equal(1d, Int32Array.CountZeroBitsOrNegative(words, 1, 96));
        Assert.Equal(6d, Int32Array.CountZeroBitsOrNegative(words, 1, 101));

        words[0] = 0b1011;
        words[1] = 0;
        Assert.Equal(6d, Int32Array.CountZeroBitsOrNegative(words, 1, 9));
        Assert.Equal(6d, Int32Array.CountZeroBitsOrNegative(words, 1, 9.0));
        Assert.Equal(2d, Int32Array.CountZeroBitsOrNegative(words, 31, 33));
        Assert.Equal(1d, Int32Array.CountZeroBitsOrNegative(words, 96, 97));
        Assert.Equal(5d, Int32Array.CountZeroBitsOrNegative(words, 96, 101));
    }

    [Fact]
    public void CountZeroBitsFallsBackForUnsafeInputsAndBacking()
    {
        var buffer = new ArrayBuffer(8d);
        var words = new Int32Array(buffer);
        foreach (var (start, end) in new[]
        {
            (-1d, 10d), (0.5, 10d), (0d, 1.5), (double.NaN, 10d),
            (0d, double.PositiveInfinity), (0d, 4294967297d),
            (BitConverter.Int64BitsToDouble(long.MinValue), 1d)
        })
        {
            Assert.Equal(-1d, Int32Array.CountZeroBitsOrNegative(words, start, end));
        }

        Assert.Equal(-1d, Int32Array.CountZeroBitsOrNegative(
            new Int32Array(new ArrayBuffer(8d, new { maxByteLength = 16d })), 0, 1));
        Assert.Equal(-1d, Int32Array.CountZeroBitsOrNegative(
            new Int32Array(new SharedArrayBuffer(8d)), 0, 1));
        buffer.Detach();
        Assert.Equal(-1d, Int32Array.CountZeroBitsOrNegative(words, 0, 1));
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
    public void CountZeroBitsCoversEveryPrimeValidationSize(int sieveSize)
    {
        var bitLength = sieveSize / 2;
        var words = new Int32Array(1d + ((bitLength + 1) >> 5));
        Assert.Equal(bitLength - 1d, Int32Array.CountZeroBitsOrNegative(words, 1, bitLength));
    }

    [Fact]
    public void CountZeroBitsDoesNotTreatArbitraryClrFieldsAsCompiledBitsets()
    {
        var owner = new UncompiledBitset();
        Assert.Equal(-1d, RuntimeServices.CountZeroBitsInCompiledBitsetOrNegative(
            owner, nameof(UncompiledBitset.wordArray), 1, 33));
    }

    private sealed class UncompiledBitset
    {
        public Int32Array wordArray = new(2d);
    }
}
