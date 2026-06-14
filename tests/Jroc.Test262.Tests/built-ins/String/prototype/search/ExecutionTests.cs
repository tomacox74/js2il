using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.search;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.search") { }

    [Fact(DisplayName = "S15.5.4.12_A1_T1")]
    public Task S15_5_4_12_A1_T1()
        => ExecutionTestFromFile("S15.5.4.12_A1_T1");

    [Fact(DisplayName = "S15.5.4.12_A2_T1")]
    public Task S15_5_4_12_A2_T1()
        => ExecutionTestFromFile("S15.5.4.12_A2_T1");

    [Fact(DisplayName = "S15.5.4.12_A2_T3")]
    public Task S15_5_4_12_A2_T3()
        => ExecutionTestFromFile("S15.5.4.12_A2_T3");

    [Fact(DisplayName = "S15.5.4.12_A3_T2")]
    public Task S15_5_4_12_A3_T2()
        => ExecutionTestFromFile("S15.5.4.12_A3_T2");
    [Fact(DisplayName = "S15.5.4.12_A2_T4")]
    public Task S15_5_4_12_A2_T4()
        => ExecutionTestFromFile("S15.5.4.12_A2_T4");
    [Fact(DisplayName = "S15.5.4.12_A11")]
    public Task S15_5_4_12_A11()
        => ExecutionTestFromFile("S15.5.4.12_A11");
    [Fact(DisplayName = "S15.5.4.12_A1_T10")]
    public Task S15_5_4_12_A1_T10()
        => ExecutionTestFromFile("S15.5.4.12_A1_T10");
    [Fact(DisplayName = "S15.5.4.12_A2_T2")]
    public Task S15_5_4_12_A2_T2()
        => ExecutionTestFromFile("S15.5.4.12_A2_T2");

}
