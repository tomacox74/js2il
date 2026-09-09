using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.lastIndexOf.BigInt;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.lastIndexOf.BigInt") { }

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-undefined")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_undefined() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-undefined");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-zero")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_zero() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-zero");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
