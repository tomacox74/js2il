using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.maxByteLength;

public class DetachmentBatchExecutionTests : DiskExecutionTestsBase
{
    public DetachmentBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.maxByteLength") { }

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "this-is-sharedarraybuffer.js")]
    public Task this_is_sharedarraybuffer() => ExecutionTestFromFile("this-is-sharedarraybuffer");
}
