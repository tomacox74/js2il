using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Function.prototype.call;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype.call") { }

[Fact(DisplayName = "15.3.4.4-1-s")]
    public Task _15_3_4_4_1_s()
        => ExecutionTest("15.3.4.4-1-s");

[Fact(DisplayName = "15.3.4.4-2-s")]
    public Task _15_3_4_4_2_s()
        => ExecutionTest("15.3.4.4-2-s");

[Fact(DisplayName = "15.3.4.4-3-s")]
    public Task _15_3_4_4_3_s()
        => ExecutionTest("15.3.4.4-3-s");

[Fact(DisplayName = "S15.3.4.4_A10", Skip = "Function.prototype.call boxed receiver handling is incomplete.")]
    public Task S15_3_4_4_A10()
        => ExecutionTest("S15.3.4.4_A10");

[Fact(DisplayName = "S15.3.4.4_A11", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_3_4_4_A11()
        => ExecutionTest("S15.3.4.4_A11");

[Fact(DisplayName = "S15.3.4.4_A12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_3_4_4_A12()
        => ExecutionTest("S15.3.4.4_A12");

[Fact(DisplayName = "S15.3.4.4_A13")]
    public Task S15_3_4_4_A13()
        => ExecutionTest("S15.3.4.4_A13");

[Fact(DisplayName = "S15.3.4.4_A14")]
    public Task S15_3_4_4_A14()
        => ExecutionTest("S15.3.4.4_A14");
}
