using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Function.prototype.call;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.call") { }

    [Fact(DisplayName = "15.3.4.4-1-s")]
    public Task _15_3_4_4_1_s()
        => ExecutionTestFromFile("15.3.4.4-1-s");

    [Fact(DisplayName = "15.3.4.4-2-s")]
    public Task _15_3_4_4_2_s()
        => ExecutionTestFromFile("15.3.4.4-2-s");

    [Fact(DisplayName = "15.3.4.4-3-s")]
    public Task _15_3_4_4_3_s()
        => ExecutionTestFromFile("15.3.4.4-3-s");

    [Fact(DisplayName = "S15.3.4.4_A10", Skip = "Blocked: inherited Object.prototype methods on function objects are incomplete.")]
    public Task S15_3_4_4_A10()
        => ExecutionTestFromFile("S15.3.4.4_A10");

    [Fact(DisplayName = "S15.3.4.4_A11", Skip = "Blocked: function object property enumeration is incomplete.")]
    public Task S15_3_4_4_A11()
        => ExecutionTestFromFile("S15.3.4.4_A11");

    [Fact(DisplayName = "S15.3.4.4_A12")]
    public Task S15_3_4_4_A12()
        => ExecutionTestFromFile("S15.3.4.4_A12");

    [Fact(DisplayName = "S15.3.4.4_A13")]
    public Task S15_3_4_4_A13()
        => ExecutionTestFromFile("S15.3.4.4_A13");

    [Fact(DisplayName = "S15.3.4.4_A14")]
    public Task S15_3_4_4_A14()
        => ExecutionTestFromFile("S15.3.4.4_A14");

    [Fact(DisplayName = "S15.3.4.4_A1_T1")]
    public Task S15_3_4_4_A1_T1()
        => ExecutionTestFromFile("S15.3.4.4_A1_T1");

    [Fact(DisplayName = "S15.3.4.4_A1_T2")]
    public Task S15_3_4_4_A1_T2()
        => ExecutionTestFromFile("S15.3.4.4_A1_T2");

    [Fact(DisplayName = "S15.3.4.4_A15")]
    public Task S15_3_4_4_A15()
        => ExecutionTestFromFile("S15.3.4.4_A15");

    [Fact(DisplayName = "S15.3.4.4_A16")]
    public Task S15_3_4_4_A16()
        => ExecutionTestFromFile("S15.3.4.4_A16");

    [Fact(DisplayName = "S15.3.4.4_A2_T1")]
    public Task S15_3_4_4_A2_T1()
        => ExecutionTestFromFile("S15.3.4.4_A2_T1");

    [Fact(DisplayName = "S15.3.4.4_A2_T2")]
    public Task S15_3_4_4_A2_T2()
        => ExecutionTestFromFile("S15.3.4.4_A2_T2");

    [Fact(DisplayName = "S15.3.4.4_A3_T1", Skip = "Blocked: non-strict Function.prototype.call global-this substitution is incomplete.")]
    public Task S15_3_4_4_A3_T1()
        => ExecutionTestFromFile("S15.3.4.4_A3_T1");

    [Fact(DisplayName = "S15.3.4.4_A3_T10", Skip = "Blocked: eval is not supported yet.")]
    public Task S15_3_4_4_A3_T10()
        => ExecutionTestFromFile("S15.3.4.4_A3_T10");
}
