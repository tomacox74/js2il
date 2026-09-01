using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView;

public class DetachmentBatchExecutionTests : DiskExecutionTestsBase
{
    public DetachmentBatchExecutionTests() : base("built_ins.DataView") { }

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");
}
