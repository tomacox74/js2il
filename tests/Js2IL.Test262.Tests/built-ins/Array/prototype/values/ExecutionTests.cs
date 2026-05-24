using Js2IL.Test262.Tests.built_ins;


namespace Js2IL.Test262.Tests.built_ins.Array.prototype.values;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.values") { }

    [Fact(DisplayName = "iteration")]
    public Task iteration()
        => ExecutionTestFromFile("iteration");
}
