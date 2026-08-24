using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Error.prototype.toString") { }

    [Fact(DisplayName = "invalid-receiver")]
    public Task invalid_receiver()
        => ExecutionTestFromFile("invalid-receiver");
}
