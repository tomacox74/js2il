using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.assignment;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.assignment") { }

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

    [Fact(DisplayName = "assignment-operator-calls-putvalue-lref--rval--1", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
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
