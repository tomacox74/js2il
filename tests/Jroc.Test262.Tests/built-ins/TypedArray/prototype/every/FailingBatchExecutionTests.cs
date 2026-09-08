using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.every;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.every") { }

    [Fact(DisplayName = "callbackfn-detachbuffer.js")]
    public Task callbackfn_detachbuffer() => ExecutionTestFromFile("callbackfn-detachbuffer");

    [Fact(DisplayName = "callbackfn-resize.js")]
    public Task callbackfn_resize() => ExecutionTestFromFile("callbackfn-resize");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "returns-true-if-every-cb-returns-true.js")]
    public Task returns_true_if_every_cb_returns_true() => ExecutionTestFromFile("returns-true-if-every-cb-returns-true");
}
