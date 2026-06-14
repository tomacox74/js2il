using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.toString") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
