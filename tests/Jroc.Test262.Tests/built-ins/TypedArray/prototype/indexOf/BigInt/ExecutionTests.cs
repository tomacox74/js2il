using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.indexOf.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.indexOf.BigInt") { }

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-undefined.js")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_undefined() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-undefined");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-zero.js")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_zero() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-zero");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
