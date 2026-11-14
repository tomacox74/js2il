using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Infinity;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Infinity") { }

    [Fact(DisplayName = "S15.1.1.2_A3_T2.js")]
    public Task Port400_S15_1_1_2_A3_T2()
        => ExecutionTestFromFile("S15.1.1.2_A3_T2");
}
