using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.subarray;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.subarray") { }

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");

}
