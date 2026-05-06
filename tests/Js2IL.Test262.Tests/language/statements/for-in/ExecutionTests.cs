using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_in;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_in") { }

    [Fact(DisplayName = "12.6.4-1")]
    public Task _12_6_4_1()
        => ExecutionTest("12.6.4-1");

    [Fact(DisplayName = "12.6.4-2")]
    public Task _12_6_4_2()
        => ExecutionTest("12.6.4-2");

    [Fact(DisplayName = "head-const-fresh-binding-per-iteration")]
    public Task head_const_fresh_binding_per_iteration()
        => ExecutionTest("head-const-fresh-binding-per-iteration");

    [Fact(DisplayName = "S12.6.4_A1", Skip = "for-in null/undefined handling is incomplete.")]
    public Task S12_6_4_A1()
        => ExecutionTest("S12.6.4_A1");

    [Fact(DisplayName = "S12.6.4_A14_T2")]
    public Task S12_6_4_A14_T2()
        => ExecutionTest("S12.6.4_A14_T2");

    [Fact(DisplayName = "S12.6.4_A2", Skip = "for-in null/undefined handling is incomplete.")]
    public Task S12_6_4_A2()
        => ExecutionTest("S12.6.4_A2");

    [Fact(DisplayName = "S12.6.4_A3.1", Skip = "eval is not supported by JS2IL.")]
    public Task S12_6_4_A3_1()
        => ExecutionTest("S12.6.4_A3.1");

}
