using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getUint8;

public class DetachmentBatchExecutionTests : DiskExecutionTestsBase
{
    public DetachmentBatchExecutionTests() : base("built_ins.DataView.prototype.getUint8") { }

    [Fact(DisplayName = "detached-buffer-after-toindex-byteoffset.js")]
    public Task detached_buffer_after_toindex_byteoffset() => ExecutionTestFromFile("detached-buffer-after-toindex-byteoffset");

    [Fact(DisplayName = "detached-buffer-before-outofrange-byteoffset.js")]
    public Task detached_buffer_before_outofrange_byteoffset() => ExecutionTestFromFile("detached-buffer-before-outofrange-byteoffset");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");
}
