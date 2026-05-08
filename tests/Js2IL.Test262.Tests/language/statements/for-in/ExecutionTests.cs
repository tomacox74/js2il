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

    [Fact(DisplayName = "S12.6.4_A1")]
    public Task S12_6_4_A1()
        => ExecutionTest("S12.6.4_A1");

    [Fact(DisplayName = "S12.6.4_A14_T2")]
    public Task S12_6_4_A14_T2()
        => ExecutionTest("S12.6.4_A14_T2");

    [Fact(DisplayName = "S12.6.4_A2")]
    public Task S12_6_4_A2()
        => ExecutionTest("S12.6.4_A2");

    [Fact(DisplayName = "S12.6.4_A3.1", Skip = "eval is not supported by JS2IL.")]
    public Task S12_6_4_A3_1()
        => ExecutionTest("S12.6.4_A3.1");

    [Fact(DisplayName = "S12.6.4_A3", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S12_6_4_A3()
        => ExecutionTest("S12.6.4_A3");

    [Fact(DisplayName = "S12.6.4_A4.1", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S12_6_4_A4_1()
        => ExecutionTest("S12.6.4_A4.1");

    [Fact(DisplayName = "S12.6.4_A4", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S12_6_4_A4()
        => ExecutionTest("S12.6.4_A4");

    [Fact(DisplayName = "S12.6.4_A5.1")]
    public Task S12_6_4_A5_1()
        => ExecutionTest("S12.6.4_A5.1");

    [Fact(DisplayName = "S12.6.4_A5")]
    public Task S12_6_4_A5()
        => ExecutionTest("S12.6.4_A5");

    [Fact(DisplayName = "S12.6.4_A6.1")]
    public Task S12_6_4_A6_1()
        => ExecutionTest("S12.6.4_A6.1");

    [Fact(DisplayName = "S12.6.4_A6")]
    public Task S12_6_4_A6()
        => ExecutionTest("S12.6.4_A6");

    [Fact(DisplayName = "S12.6.4_A7_T1")]
    public Task S12_6_4_A7_T1()
        => ExecutionTest("S12.6.4_A7_T1");

    [Fact(DisplayName = "S12.6.4_A7_T2")]
    public Task S12_6_4_A7_T2()
        => ExecutionTest("S12.6.4_A7_T2");

    [Fact(DisplayName = "cptn-decl-abrupt-empty", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task cptn_decl_abrupt_empty()
        => ExecutionTest("cptn-decl-abrupt-empty");

    [Fact(DisplayName = "cptn-decl-itr", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task cptn_decl_itr()
        => ExecutionTest("cptn-decl-itr");

    [Fact(DisplayName = "cptn-decl-skip-itr", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task cptn_decl_skip_itr()
        => ExecutionTest("cptn-decl-skip-itr");
}
