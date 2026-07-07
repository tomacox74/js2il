using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object") { }

    [Fact(DisplayName = "S15.2.1.1_A1_T1")]
    public Task S15_2_1_1_A1_T1()
        => ExecutionTestFromFile("S15.2.1.1_A1_T1");

    [Fact(DisplayName = "S15.2.1.1_A1_T2")]
    public Task S15_2_1_1_A1_T2()
        => ExecutionTestFromFile("S15.2.1.1_A1_T2");

    [Fact(DisplayName = "S15.2.1.1_A1_T3")]
    public Task S15_2_1_1_A1_T3()
        => ExecutionTestFromFile("S15.2.1.1_A1_T3");

    [Fact(DisplayName = "S15.2.2.1_A2_T5")]
    public Task S15_2_2_1_A2_T5()
        => ExecutionTestFromFile("S15.2.2.1_A2_T5");

    [Fact(DisplayName = "S15.2.3_A2")]
    public Task S15_2_3_A2()
        => ExecutionTestFromFile("S15.2.3_A2");

    [Fact(DisplayName = "bigint")]
    public Task bigint()
        => ExecutionTestFromFile("bigint");
}
