using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.resize;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.resize") { }

    [Fact(DisplayName = "new-length-excessive.js")]
    public Task new_length_excessive() => ExecutionTestFromFile("new-length-excessive");

    [Fact(DisplayName = "new-length-negative.js")]
    public Task new_length_negative() => ExecutionTestFromFile("new-length-negative");

    [Fact(DisplayName = "new-length-non-number.js")]
    public Task new_length_non_number() => ExecutionTestFromFile("new-length-non-number");

    [Fact(DisplayName = "this-is-sharedarraybuffer.js")]
    public Task this_is_sharedarraybuffer() => ExecutionTestFromFile("this-is-sharedarraybuffer");

}
