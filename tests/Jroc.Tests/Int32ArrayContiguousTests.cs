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
}
