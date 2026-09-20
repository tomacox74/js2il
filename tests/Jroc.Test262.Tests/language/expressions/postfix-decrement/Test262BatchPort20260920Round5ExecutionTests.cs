using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.postfix_decrement;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.postfix_decrement") { }

    [Fact(DisplayName = "11.3.2-2-3-s")]
    public Task _11_3_2_2_3_s()
        => ExecutionTest("11.3.2-2-3-s");

    [Fact(DisplayName = "S11.3.2_A2.1_T2")]
    public Task S11_3_2_A2_1_T2()
        => ExecutionTest("S11.3.2_A2.1_T2");

    [Fact(DisplayName = "S11.3.2_A2.2_T1")]
    public Task S11_3_2_A2_2_T1()
        => ExecutionTest("S11.3.2_A2.2_T1");

    [Fact(DisplayName = "S11.3.2_A3_T2")]
    public Task S11_3_2_A3_T2()
        => ExecutionTest("S11.3.2_A3_T2");

    [Fact(DisplayName = "S11.3.2_A3_T3")]
    public Task S11_3_2_A3_T3()
        => ExecutionTest("S11.3.2_A3_T3");

    [Fact(DisplayName = "S11.3.2_A3_T4")]
    public Task S11_3_2_A3_T4()
        => ExecutionTest("S11.3.2_A3_T4");

    [Fact(DisplayName = "S11.3.2_A3_T5")]
    public Task S11_3_2_A3_T5()
        => ExecutionTest("S11.3.2_A3_T5");

    [Fact(DisplayName = "S11.3.2_A4_T1")]
    public Task S11_3_2_A4_T1()
        => ExecutionTest("S11.3.2_A4_T1");

    [Fact(DisplayName = "S11.3.2_A4_T2")]
    public Task S11_3_2_A4_T2()
        => ExecutionTest("S11.3.2_A4_T2");

    [Fact(DisplayName = "S11.3.2_A4_T3")]
    public Task S11_3_2_A4_T3()
        => ExecutionTest("S11.3.2_A4_T3");

    [Fact(DisplayName = "S11.3.2_A4_T4")]
    public Task S11_3_2_A4_T4()
        => ExecutionTest("S11.3.2_A4_T4");

    [Fact(DisplayName = "S11.3.2_A4_T5")]
    public Task S11_3_2_A4_T5()
        => ExecutionTest("S11.3.2_A4_T5");

    [Fact(DisplayName = "S11.3.2_A5_T1")]
    public Task S11_3_2_A5_T1()
        => ExecutionTest("S11.3.2_A5_T1");

    [Fact(DisplayName = "S11.3.2_A5_T2")]
    public Task S11_3_2_A5_T2()
        => ExecutionTest("S11.3.2_A5_T2");

    [Fact(DisplayName = "S11.3.2_A5_T3")]
    public Task S11_3_2_A5_T3()
        => ExecutionTest("S11.3.2_A5_T3");

    [Fact(DisplayName = "S11.3.2_A6_T3")]
    public Task S11_3_2_A6_T3()
        => ExecutionTest("S11.3.2_A6_T3");

    [Fact(DisplayName = "arguments-nostrict")]
    public Task arguments_nostrict()
        => ExecutionTest("arguments-nostrict");

    [Fact(DisplayName = "bigint")]
    public Task bigint()
        => ExecutionTest("bigint");

    [Fact(DisplayName = "eval-nostrict")]
    public Task eval_nostrict()
        => ExecutionTest("eval-nostrict");

    [Fact(DisplayName = "operator-x-postfix-decrement-calls-putvalue-lhs-newvalue--1")]
    public Task operator_x_postfix_decrement_calls_putvalue_lhs_newvalue__1()
        => ExecutionTest("operator-x-postfix-decrement-calls-putvalue-lhs-newvalue--1");

    [Fact(DisplayName = "operator-x-postfix-decrement-calls-putvalue-lhs-newvalue-")]
    public Task operator_x_postfix_decrement_calls_putvalue_lhs_newvalue_()
        => ExecutionTest("operator-x-postfix-decrement-calls-putvalue-lhs-newvalue-");

    [Fact(DisplayName = "target-cover-id")]
    public Task target_cover_id()
        => ExecutionTest("target-cover-id");

    [Fact(DisplayName = "whitespace")]
    public Task whitespace()
        => ExecutionTest("whitespace");

}
