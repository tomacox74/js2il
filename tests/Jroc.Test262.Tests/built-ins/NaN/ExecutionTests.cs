using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NaN;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.NaN") { }

    [Fact(DisplayName = "15.1.1.1-0.js")]
    public Task Port400_15_1_1_1_0()
        => ExecutionTestFromFile("15.1.1.1-0");

    [Fact(DisplayName = "S15.1.1.1_A1.js")]
    public Task Port400_S15_1_1_1_A1()
        => ExecutionTestFromFile("S15.1.1.1_A1");

    [Fact(DisplayName = "S15.1.1.1_A3_T2.js")]
    public Task Port400_S15_1_1_1_A3_T2()
        => ExecutionTestFromFile("S15.1.1.1_A3_T2");
}
