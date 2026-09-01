using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.resize;

public class DetachmentBatchExecutionTests : DiskExecutionTestsBase
{
    public DetachmentBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.resize") { }

    [Fact(DisplayName = "coerced-new-length-detach.js")]
    public Task coerced_new_length_detach() => ExecutionTestFromFile("coerced-new-length-detach");

    [Fact(DisplayName = "this-is-detached.js")]
    public Task this_is_detached() => ExecutionTestFromFile("this-is-detached");
}
