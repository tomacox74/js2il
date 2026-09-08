using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.findIndex.BigInt;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.findIndex.BigInt") { }

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "predicate-may-detach-buffer.js")]
    public Task predicate_may_detach_buffer() => ExecutionTestFromFile("predicate-may-detach-buffer");
}
