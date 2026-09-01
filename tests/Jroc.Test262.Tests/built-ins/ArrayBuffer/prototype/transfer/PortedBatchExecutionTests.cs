using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.transfer;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.transfer") { }

    [Fact(DisplayName = "this-is-sharedarraybuffer.js")]
    public Task this_is_sharedarraybuffer() => ExecutionTestFromFile("this-is-sharedarraybuffer");

}
