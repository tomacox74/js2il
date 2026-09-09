using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduceRight;

public class NextBatchExecutionTests : DiskExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.reduceRight") { }

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

}
