using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.byteOffset.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.byteOffset.BigInt") { }

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");
}
