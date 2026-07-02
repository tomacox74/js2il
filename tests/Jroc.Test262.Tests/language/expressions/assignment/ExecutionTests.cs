using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.assignment;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.assignment") { }

    [Fact(DisplayName = "8.12.5-3-b_1")]
    public Task _8_12_5_3_b_1()
        => ExecutionTest("8.12.5-3-b_1");

    [Fact(DisplayName = "8.12.5-3-b_2")]
    public Task _8_12_5_3_b_2()
        => ExecutionTest("8.12.5-3-b_2");

    [Fact(DisplayName = "8.12.5-5-b_1")]
    public Task _8_12_5_5_b_1()
        => ExecutionTest("8.12.5-5-b_1");

    [Fact(DisplayName = "8.14.4-8-b_1")]
    public Task _8_14_4_8_b_1()
        => ExecutionTest("8.14.4-8-b_1");

    [Fact(DisplayName = "11.13.1-1-6-s")]
    public Task _11_13_1_1_6_s()
        => ExecutionTest("11.13.1-1-6-s");

    [Fact(DisplayName = "11.13.1-1-s")]
    public Task _11_13_1_1_s()
        => ExecutionTest("11.13.1-1-s");

    [Fact(DisplayName = "11.13.1-2-s")]
    public Task _11_13_1_2_s()
        => ExecutionTest("11.13.1-2-s");

    [Fact(DisplayName = "11.13.1-3-s")]
    public Task _11_13_1_3_s()
        => ExecutionTest("11.13.1-3-s");

    [Fact(DisplayName = "11.13.1-4-1")]
    public Task _11_13_1_4_1()
        => ExecutionTest("11.13.1-4-1");

    [Fact(DisplayName = "11.13.1-4-14-s")]
    public Task _11_13_1_4_14_s()
        => ExecutionTest("11.13.1-4-14-s");

    [Fact(DisplayName = "11.13.1-4-27-s")]
    public Task _11_13_1_4_27_s()
        => ExecutionTest("11.13.1-4-27-s");

    [Fact(DisplayName = "11.13.1-4-28gs")]
    public Task _11_13_1_4_28gs()
        => ExecutionTest("11.13.1-4-28gs");

    [Fact(DisplayName = "11.13.1-4-29gs")]
    public Task _11_13_1_4_29gs()
        => ExecutionTest("11.13.1-4-29gs");

    [Fact(DisplayName = "11.13.1-4-3-s")]
    public Task _11_13_1_4_3_s()
        => ExecutionTest("11.13.1-4-3-s");

    [Fact(DisplayName = "11.13.1-4-6-s")]
    public Task _11_13_1_4_6_s()
        => ExecutionTest("11.13.1-4-6-s");

    [Fact(DisplayName = "8.14.4-8-b_2")]
    public Task _8_14_4_8_b_2()
        => ExecutionTest("8.14.4-8-b_2");

    [Fact(DisplayName = "assignment-operator-calls-putvalue-lref--rval--1")]
    public Task assignment_operator_calls_putvalue_lref_rval_1()
        => ExecutionTest("assignment-operator-calls-putvalue-lref--rval--1");

    [Fact(DisplayName = "assignment-operator-calls-putvalue-lref--rval-")]
    public Task assignment_operator_calls_putvalue_lref_rval()
        => ExecutionTest("assignment-operator-calls-putvalue-lref--rval-");

    [Fact(DisplayName = "target-member-computed-reference")]
    public Task target_member_computed_reference()
        => ExecutionTest("target-member-computed-reference");

    [Fact(DisplayName = "target-member-identifier-reference-null")]
    public Task target_member_identifier_reference_null()
        => ExecutionTest("target-member-identifier-reference-null");
}
