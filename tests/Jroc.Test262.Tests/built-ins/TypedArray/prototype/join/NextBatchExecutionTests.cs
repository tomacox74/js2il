using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.join;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.join") { }

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-single-comma")]
    public Task detached_buffer_during_fromIndex_returns_single_comma() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-single-comma");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
