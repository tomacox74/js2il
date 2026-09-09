using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.includes.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.includes.BigInt") { }

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-false-for-zero")]
    public Task detached_buffer_during_fromIndex_returns_false_for_zero()
        => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-false-for-zero");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-true-for-undefined")]
    public Task detached_buffer_during_fromIndex_returns_true_for_undefined()
        => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-true-for-undefined");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "length-zero-returns-false")]
    public Task length_zero_returns_false()
        => ExecutionTestFromFile("length-zero-returns-false");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
