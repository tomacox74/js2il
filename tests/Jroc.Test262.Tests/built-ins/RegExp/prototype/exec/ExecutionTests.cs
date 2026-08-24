using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.exec;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.exec") { }

    [Fact(DisplayName = "S15.10.6.2_A1_T1")]
    public Task S15_10_6_2_A1_T1()
        => ExecutionTestFromFile("S15.10.6.2_A1_T1");

    [Fact(DisplayName = "S15.10.6.2_A1_T2")]
    public Task S15_10_6_2_A1_T2()
        => ExecutionTestFromFile("S15.10.6.2_A1_T2");

    [Fact(DisplayName = "S15.10.6.2_A1_T3")]
    public Task S15_10_6_2_A1_T3()
        => ExecutionTestFromFile("S15.10.6.2_A1_T3");

    [Fact(DisplayName = "S15.10.6.2_A1_T10")]
    public Task S15_10_6_2_A1_T10()
        => ExecutionTestFromFile("S15.10.6.2_A1_T10");

    [Fact(DisplayName = "S15.10.6.2_A12")]
    public Task S15_10_6_2_A12()
        => ExecutionTestFromFile("S15.10.6.2_A12");
    [Fact(DisplayName = "S15.10.6.2_A1_T4")]
    public Task S15_10_6_2_A1_T4()
        => ExecutionTestFromFile("S15.10.6.2_A1_T4");
    [Fact(DisplayName = "S15.10.6.2_A1_T5")]
    public Task S15_10_6_2_A1_T5()
        => ExecutionTestFromFile("S15.10.6.2_A1_T5");
    [Fact(DisplayName = "S15.10.6.2_A1_T8")]
    public Task S15_10_6_2_A1_T8()
        => ExecutionTestFromFile("S15.10.6.2_A1_T8");
    [Fact(DisplayName = "S15.10.6.2_A1_T7")]
    public Task S15_10_6_2_A1_T7()
        => ExecutionTestFromFile("S15.10.6.2_A1_T7");
    [Fact(DisplayName = "S15.10.6.2_A1_T9")]
    public Task S15_10_6_2_A1_T9()
        => ExecutionTestFromFile("S15.10.6.2_A1_T9");

    [Fact(DisplayName = "S15.10.6.2_A3_T1")]
    public Task S15_10_6_2_A3_T1()
        => ExecutionTestFromFile("S15.10.6.2_A3_T1");

    [Fact(DisplayName = "S15.10.6.2_A4_T1")]
    public Task S15_10_6_2_A4_T1()
        => ExecutionTestFromFile("S15.10.6.2_A4_T1");

    [Fact(DisplayName = "S15.10.6.2_A4_T2")]
    public Task S15_10_6_2_A4_T2()
        => ExecutionTestFromFile("S15.10.6.2_A4_T2");

    [Fact(DisplayName = "S15.10.6.2_A5_T1")]
    public Task S15_10_6_2_A5_T1()
        => ExecutionTestFromFile("S15.10.6.2_A5_T1");

    [Fact(DisplayName = "S15.10.6.2_A1_T6")]
    public Task S15_10_6_2_A1_T6()
        => ExecutionTestFromFile("S15.10.6.2_A1_T6");

    [Fact(DisplayName = "S15.10.6.2_A4_T11")]
    public Task S15_10_6_2_A4_T11()
        => ExecutionTestFromFile("S15.10.6.2_A4_T11");

    [Fact(DisplayName = "failure-g-lastindex-reset")]
    public Task failure_g_lastindex_reset()
        => ExecutionTestFromFile("failure-g-lastindex-reset");

    [Fact(DisplayName = "failure-lastindex-access")]
    public Task failure_lastindex_access()
        => ExecutionTestFromFile("failure-lastindex-access");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "success-lastindex-access")]
    public Task success_lastindex_access()
        => ExecutionTestFromFile("success-lastindex-access");

    [Fact(DisplayName = "u-lastindex-adv")]
    public Task u_lastindex_adv()
        => ExecutionTestFromFile("u-lastindex-adv");

    [Fact(DisplayName = "y-fail-lastindex-no-write")]
    public Task y_fail_lastindex_no_write()
        => ExecutionTestFromFile("y-fail-lastindex-no-write");
}
