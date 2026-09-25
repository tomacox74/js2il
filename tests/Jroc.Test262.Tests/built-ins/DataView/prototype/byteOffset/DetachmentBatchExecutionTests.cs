using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.byteOffset;

public class DetachmentBatchExecutionTests : InMemoryExecutionTestsBase
{
    public DetachmentBatchExecutionTests() : base("built_ins.DataView.prototype.byteOffset") { }

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");
}
