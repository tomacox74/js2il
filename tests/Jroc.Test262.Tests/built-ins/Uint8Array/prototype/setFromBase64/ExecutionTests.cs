using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.prototype.setFromBase64;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Uint8Array.prototype.setFromBase64") { }

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTest("descriptor");

    [Fact(DisplayName = "results")]
    public Task results()
        => ExecutionTest("results");
}
