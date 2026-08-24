using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.round;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math.round") { }

    [Fact(DisplayName = "S15.8.2.15_A7")]
    public Task S15_8_2_15_A7()
        => ExecutionTestFromFile("S15.8.2.15_A7");
}
