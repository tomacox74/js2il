using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.parseFloat;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.parseFloat") { }

    [Fact(DisplayName = "15.1.2.3-2-1")]
    public Task _15_1_2_3_2_1()
        => ExecutionTestFromFile("15.1.2.3-2-1");

    [Fact(DisplayName = "S15.1.2.3_A1_T1")]
    public Task S15_1_2_3_A1_T1()
        => ExecutionTestFromFile("S15.1.2.3_A1_T1");

    [Fact(DisplayName = "S15.1.2.3_A1_T3")]
    public Task S15_1_2_3_A1_T3()
        => ExecutionTestFromFile("S15.1.2.3_A1_T3");

    [Fact(DisplayName = "S15.1.2.3_A1_T4")]
    public Task S15_1_2_3_A1_T4()
        => ExecutionTestFromFile("S15.1.2.3_A1_T4");

    [Fact(DisplayName = "S15.1.2.3_A2_T1")]
    public Task S15_1_2_3_A2_T1()
        => ExecutionTestFromFile("S15.1.2.3_A2_T1");
}
