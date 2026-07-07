using Jroc.Test262.Tests.intl402;

namespace Jroc.Test262.Tests.intl402.BigInt.prototype.toLocaleString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("intl402.BigInt.prototype.toLocaleString") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");
}
