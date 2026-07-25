using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.byteLength;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.byteLength") { }

    [Fact(DisplayName = "return-bytelength")]
    public Task return_bytelength()
        => ExecutionTest("return-bytelength");

}
