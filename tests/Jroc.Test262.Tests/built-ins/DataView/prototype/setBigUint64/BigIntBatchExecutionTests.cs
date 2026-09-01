using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.setBigUint64;

public class BigIntBatchExecutionTests : DiskExecutionTestsBase
{
    public BigIntBatchExecutionTests() : base("built_ins.DataView.prototype.setBigUint64") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");
}
