using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.indexOf;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.indexOf") { }

    [Fact(DisplayName = "S15.5.4.7_A10", Skip = "String.prototype.indexOf boxed-string handling is incomplete.")]
    public Task S15_5_4_7_A10()
        => ExecutionTest("S15.5.4.7_A10");

    [Fact(DisplayName = "S15.5.4.7_A11", Skip = "String.prototype.indexOf boxed-string handling is incomplete.")]
    public Task S15_5_4_7_A11()
        => ExecutionTest("S15.5.4.7_A11");

    [Fact(DisplayName = "S15.5.4.7_A1_T1", Skip = "String.prototype.indexOf boxed-string handling is incomplete.")]
    public Task S15_5_4_7_A1_T1()
        => ExecutionTest("S15.5.4.7_A1_T1");

    [Fact(DisplayName = "S15.5.4.7_A1_T10")]
    public Task S15_5_4_7_A1_T10()
        => ExecutionTest("S15.5.4.7_A1_T10");
}
