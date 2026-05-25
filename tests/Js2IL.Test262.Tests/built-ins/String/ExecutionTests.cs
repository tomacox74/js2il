using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String") { }

    [Fact(DisplayName = "S15.5.1.1_A1_T10")]
    public Task S15_5_1_1_A1_T10()
        => ExecutionTestFromFile("S15.5.1.1_A1_T10");

    [Fact(DisplayName = "S15.5.1.1_A1_T11")]
    public Task S15_5_1_1_A1_T11()
        => ExecutionTestFromFile("S15.5.1.1_A1_T11");

    [Fact(DisplayName = "S15.5.1.1_A1_T12")]
    public Task S15_5_1_1_A1_T12()
        => ExecutionTestFromFile("S15.5.1.1_A1_T12");

    [Fact(DisplayName = "S15.5.1.1_A1_T13")]
    public Task S15_5_1_1_A1_T13()
        => ExecutionTestFromFile("S15.5.1.1_A1_T13");

    [Fact(DisplayName = "15.5.5.5.2-1-1")]
    public Task _15_5_5_5_2_1_1()
        => ExecutionTestFromFile("15.5.5.5.2-1-1");

    [Fact(DisplayName = "15.5.5.5.2-3-7")]
    public Task _15_5_5_5_2_3_7()
        => ExecutionTestFromFile("15.5.5.5.2-3-7");
}
