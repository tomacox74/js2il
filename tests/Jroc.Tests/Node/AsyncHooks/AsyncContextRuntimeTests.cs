using JavaScriptRuntime.Node;

namespace Jroc.Tests.Node.AsyncHooks;

public sealed class AsyncContextRuntimeTests
{
    [Fact]
    public void InactiveCaptureReturnsOriginalActionWithoutAllocating()
    {
        var runtime = new AsyncContextRuntime();
        Action callback = static () => { };

        Assert.Same(callback, runtime.CaptureAction(callback));

        _ = runtime.CaptureAction(callback);
        var captured = callback;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < 10_000; index++)
        {
            captured = runtime.CaptureAction(callback);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Same(callback, captured);
        Assert.Equal(0, allocated);
    }
}
