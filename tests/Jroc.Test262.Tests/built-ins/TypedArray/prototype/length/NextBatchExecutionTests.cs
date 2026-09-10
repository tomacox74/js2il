using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.length;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.length") { }

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
