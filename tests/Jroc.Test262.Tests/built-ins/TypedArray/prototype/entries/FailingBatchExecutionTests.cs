using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.entries;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.entries") { }

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
