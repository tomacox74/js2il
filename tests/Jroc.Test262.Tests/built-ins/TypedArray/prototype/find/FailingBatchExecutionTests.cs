using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.find;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.find") { }

    [Fact(DisplayName = "callbackfn-resize.js")]
    public Task callbackfn_resize() => ExecutionTestFromFile("callbackfn-resize");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "predicate-may-detach-buffer.js")]
    public Task predicate_may_detach_buffer() => ExecutionTestFromFile("predicate-may-detach-buffer");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");
}
