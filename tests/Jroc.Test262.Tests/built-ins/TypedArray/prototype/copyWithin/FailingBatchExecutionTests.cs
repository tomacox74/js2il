using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.copyWithin;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.copyWithin") { }

    [Fact(DisplayName = "coerced-target-start-end-shrink.js")]
    public Task coerced_target_start_end_shrink() => ExecutionTestFromFile("coerced-target-start-end-shrink");

    [Fact(DisplayName = "coerced-target-start-grow.js")]
    public Task coerced_target_start_grow() => ExecutionTestFromFile("coerced-target-start-grow");

    [Fact(DisplayName = "coerced-values-end-detached-prototype.js")]
    public Task coerced_values_end_detached_prototype() => ExecutionTestFromFile("coerced-values-end-detached-prototype");

    [Fact(DisplayName = "coerced-values-end-detached.js")]
    public Task coerced_values_end_detached() => ExecutionTestFromFile("coerced-values-end-detached");

    [Fact(DisplayName = "coerced-values-start-detached.js")]
    public Task coerced_values_start_detached() => ExecutionTestFromFile("coerced-values-start-detached");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
