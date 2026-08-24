using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Boolean.prototype") { }

    [Fact(DisplayName = "S15.6.3.1_A1")]
    public Task S15_6_3_1_A1()
        => ExecutionTestFromFile("S15.6.3.1_A1");

}
