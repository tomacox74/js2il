using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce.BigInt;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.reduce.BigInt") { }

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
