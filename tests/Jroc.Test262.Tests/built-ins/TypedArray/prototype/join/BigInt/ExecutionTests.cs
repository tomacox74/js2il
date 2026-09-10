using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.join.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.join.BigInt") { }

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-single-comma.js")]
    public Task detached_buffer_during_fromIndex_returns_single_comma() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-single-comma");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
