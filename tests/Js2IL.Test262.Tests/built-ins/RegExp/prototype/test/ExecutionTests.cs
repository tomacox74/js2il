using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.RegExp.prototype.test;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.test") { }

    [Fact(DisplayName = "S15.10.6.3_A1_T1")]
    public Task S15_10_6_3_A1_T1()
        => ExecutionTestFromFile("S15.10.6.3_A1_T1");

    [Fact(DisplayName = "S15.10.6.3_A1_T2")]
    public Task S15_10_6_3_A1_T2()
        => ExecutionTestFromFile("S15.10.6.3_A1_T2");

    [Fact(DisplayName = "S15.10.6.3_A1_T10")]
    public Task S15_10_6_3_A1_T10()
        => ExecutionTestFromFile("S15.10.6.3_A1_T10");

    [Fact(DisplayName = "S15.10.6.3_A1_T11")]
    public Task S15_10_6_3_A1_T11()
        => ExecutionTestFromFile("S15.10.6.3_A1_T11");

    [Fact(DisplayName = "S15.10.6.3_A1_T3")]
    public Task S15_10_6_3_A1_T3()
        => ExecutionTestFromFile("S15.10.6.3_A1_T3");
}
