using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.prefix_increment;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.prefix_increment") { }

    [Fact(DisplayName = "11.4.4-2-3-s")]
    public Task _11_4_4_2_3_s()
        => ExecutionTest("11.4.4-2-3-s");

    [Fact(DisplayName = "S11.4.4_A2.1_T2")]
    public Task S11_4_4_A2_1_T2()
        => ExecutionTest("S11.4.4_A2.1_T2");

    [Fact(DisplayName = "S11.4.4_A2.2_T1")]
    public Task S11_4_4_A2_2_T1()
        => ExecutionTest("S11.4.4_A2.2_T1");

    [Fact(DisplayName = "S11.4.4_A3_T2")]
    public Task S11_4_4_A3_T2()
        => ExecutionTest("S11.4.4_A3_T2");

    [Fact(DisplayName = "S11.4.4_A3_T3")]
    public Task S11_4_4_A3_T3()
        => ExecutionTest("S11.4.4_A3_T3");

    [Fact(DisplayName = "S11.4.4_A3_T4")]
    public Task S11_4_4_A3_T4()
        => ExecutionTest("S11.4.4_A3_T4");

    [Fact(DisplayName = "S11.4.4_A3_T5")]
    public Task S11_4_4_A3_T5()
        => ExecutionTest("S11.4.4_A3_T5");

    [Fact(DisplayName = "S11.4.4_A4_T1")]
    public Task S11_4_4_A4_T1()
        => ExecutionTest("S11.4.4_A4_T1");

    [Fact(DisplayName = "S11.4.4_A4_T2")]
    public Task S11_4_4_A4_T2()
        => ExecutionTest("S11.4.4_A4_T2");

    [Fact(DisplayName = "S11.4.4_A4_T3")]
    public Task S11_4_4_A4_T3()
        => ExecutionTest("S11.4.4_A4_T3");

    [Fact(DisplayName = "S11.4.4_A4_T4")]
    public Task S11_4_4_A4_T4()
        => ExecutionTest("S11.4.4_A4_T4");

    [Fact(DisplayName = "S11.4.4_A4_T5")]
    public Task S11_4_4_A4_T5()
        => ExecutionTest("S11.4.4_A4_T5");

    [Fact(DisplayName = "S11.4.4_A5_T3")]
    public Task S11_4_4_A5_T3()
        => ExecutionTest("S11.4.4_A5_T3");

    [Fact(DisplayName = "arguments-nostrict")]
    public Task arguments_nostrict()
        => ExecutionTest("arguments-nostrict");

    [Fact(DisplayName = "bigint")]
    public Task bigint()
        => ExecutionTest("bigint");

    [Fact(DisplayName = "eval-nostrict")]
    public Task eval_nostrict()
        => ExecutionTest("eval-nostrict");

    [Fact(DisplayName = "operator-prefix-increment-x-calls-putvalue-lhs-newvalue--1")]
    public Task operator_prefix_increment_x_calls_putvalue_lhs_newvalue__1()
        => ExecutionTest("operator-prefix-increment-x-calls-putvalue-lhs-newvalue--1");

    [Fact(DisplayName = "target-cover-id")]
    public Task target_cover_id()
        => ExecutionTest("target-cover-id");

    [Fact(DisplayName = "whitespace")]
    public Task whitespace()
        => ExecutionTest("whitespace");

}
