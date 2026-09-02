using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.buffer.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.buffer.BigInt") { }

    [Fact(DisplayName = "return-buffer.js")]
    public Task return_buffer() => ExecutionTestFromFile("return-buffer");

}
