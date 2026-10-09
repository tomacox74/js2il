namespace Jroc.Test262.Tests.language.expressions.compound_assignment;

public class CompoundAssignmentCompletionExecutionTests : DiskExecutionTestsBase
{
    public CompoundAssignmentCompletionExecutionTests() : base("language.expressions.compound_assignment") { }

    [Fact(DisplayName = "11.13.2-1-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_1_s()
        => ExecutionTest("11.13.2-1-s");

    [Fact(DisplayName = "11.13.2-10-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_10_s()
        => ExecutionTest("11.13.2-10-s");

    [Fact(DisplayName = "11.13.2-11-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_11_s()
        => ExecutionTest("11.13.2-11-s");

    [Fact(DisplayName = "11.13.2-2-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_2_s()
        => ExecutionTest("11.13.2-2-s");

    [Fact(DisplayName = "11.13.2-4-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_4_s()
        => ExecutionTest("11.13.2-4-s");

    [Fact(DisplayName = "11.13.2-5-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_5_s()
        => ExecutionTest("11.13.2-5-s");

    [Fact(DisplayName = "11.13.2-6-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_6_s()
        => ExecutionTest("11.13.2-6-s");

    [Fact(DisplayName = "11.13.2-8-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_8_s()
        => ExecutionTest("11.13.2-8-s");

    [Fact(DisplayName = "11.13.2-9-s.js", Skip = "eval is not supported.")]
    public Task _11_13_2_9_s()
        => ExecutionTest("11.13.2-9-s");

    [Fact(DisplayName = "S11.13.2_A5.10_T1.js")]
    public Task S11_13_2_A5_10_T1()
        => ExecutionTest("S11.13.2_A5.10_T1");

    [Fact(DisplayName = "S11.13.2_A5.10_T2.js")]
    public Task S11_13_2_A5_10_T2()
        => ExecutionTest("S11.13.2_A5.10_T2");

    [Fact(DisplayName = "S11.13.2_A5.10_T3.js")]
    public Task S11_13_2_A5_10_T3()
        => ExecutionTest("S11.13.2_A5.10_T3");

    [Fact(DisplayName = "S11.13.2_A5.11_T1.js")]
    public Task S11_13_2_A5_11_T1()
        => ExecutionTest("S11.13.2_A5.11_T1");

    [Fact(DisplayName = "S11.13.2_A5.11_T2.js")]
    public Task S11_13_2_A5_11_T2()
        => ExecutionTest("S11.13.2_A5.11_T2");

    [Fact(DisplayName = "S11.13.2_A5.11_T3.js")]
    public Task S11_13_2_A5_11_T3()
        => ExecutionTest("S11.13.2_A5.11_T3");

    [Fact(DisplayName = "S11.13.2_A5.1_T1.js")]
    public Task S11_13_2_A5_1_T1()
        => ExecutionTest("S11.13.2_A5.1_T1");

    [Fact(DisplayName = "S11.13.2_A5.1_T2.js")]
    public Task S11_13_2_A5_1_T2()
        => ExecutionTest("S11.13.2_A5.1_T2");

    [Fact(DisplayName = "S11.13.2_A5.1_T3.js")]
    public Task S11_13_2_A5_1_T3()
        => ExecutionTest("S11.13.2_A5.1_T3");

    [Fact(DisplayName = "S11.13.2_A5.2_T1.js")]
    public Task S11_13_2_A5_2_T1()
        => ExecutionTest("S11.13.2_A5.2_T1");

    [Fact(DisplayName = "S11.13.2_A5.2_T2.js")]
    public Task S11_13_2_A5_2_T2()
        => ExecutionTest("S11.13.2_A5.2_T2");

    [Fact(DisplayName = "S11.13.2_A5.2_T3.js")]
    public Task S11_13_2_A5_2_T3()
        => ExecutionTest("S11.13.2_A5.2_T3");

    [Fact(DisplayName = "S11.13.2_A5.3_T1.js")]
    public Task S11_13_2_A5_3_T1()
        => ExecutionTest("S11.13.2_A5.3_T1");

    [Fact(DisplayName = "S11.13.2_A5.3_T2.js")]
    public Task S11_13_2_A5_3_T2()
        => ExecutionTest("S11.13.2_A5.3_T2");

    [Fact(DisplayName = "S11.13.2_A5.3_T3.js")]
    public Task S11_13_2_A5_3_T3()
        => ExecutionTest("S11.13.2_A5.3_T3");

    [Fact(DisplayName = "S11.13.2_A5.4_T1.js")]
    public Task S11_13_2_A5_4_T1()
        => ExecutionTest("S11.13.2_A5.4_T1");

    [Fact(DisplayName = "S11.13.2_A5.4_T2.js")]
    public Task S11_13_2_A5_4_T2()
        => ExecutionTest("S11.13.2_A5.4_T2");

    [Fact(DisplayName = "S11.13.2_A5.4_T3.js")]
    public Task S11_13_2_A5_4_T3()
        => ExecutionTest("S11.13.2_A5.4_T3");

    [Fact(DisplayName = "S11.13.2_A5.5_T1.js")]
    public Task S11_13_2_A5_5_T1()
        => ExecutionTest("S11.13.2_A5.5_T1");

    [Fact(DisplayName = "S11.13.2_A5.5_T2.js")]
    public Task S11_13_2_A5_5_T2()
        => ExecutionTest("S11.13.2_A5.5_T2");

    [Fact(DisplayName = "S11.13.2_A5.5_T3.js")]
    public Task S11_13_2_A5_5_T3()
        => ExecutionTest("S11.13.2_A5.5_T3");

    [Fact(DisplayName = "S11.13.2_A5.6_T1.js")]
    public Task S11_13_2_A5_6_T1()
        => ExecutionTest("S11.13.2_A5.6_T1");

    [Fact(DisplayName = "S11.13.2_A5.6_T2.js")]
    public Task S11_13_2_A5_6_T2()
        => ExecutionTest("S11.13.2_A5.6_T2");

    [Fact(DisplayName = "S11.13.2_A5.6_T3.js")]
    public Task S11_13_2_A5_6_T3()
        => ExecutionTest("S11.13.2_A5.6_T3");

    [Fact(DisplayName = "S11.13.2_A5.7_T1.js")]
    public Task S11_13_2_A5_7_T1()
        => ExecutionTest("S11.13.2_A5.7_T1");

    [Fact(DisplayName = "S11.13.2_A5.7_T2.js")]
    public Task S11_13_2_A5_7_T2()
        => ExecutionTest("S11.13.2_A5.7_T2");

    [Fact(DisplayName = "S11.13.2_A5.7_T3.js")]
    public Task S11_13_2_A5_7_T3()
        => ExecutionTest("S11.13.2_A5.7_T3");

    [Fact(DisplayName = "S11.13.2_A5.8_T1.js")]
    public Task S11_13_2_A5_8_T1()
        => ExecutionTest("S11.13.2_A5.8_T1");

    [Fact(DisplayName = "S11.13.2_A5.8_T2.js")]
    public Task S11_13_2_A5_8_T2()
        => ExecutionTest("S11.13.2_A5.8_T2");

    [Fact(DisplayName = "S11.13.2_A5.8_T3.js")]
    public Task S11_13_2_A5_8_T3()
        => ExecutionTest("S11.13.2_A5.8_T3");

    [Fact(DisplayName = "S11.13.2_A5.9_T1.js")]
    public Task S11_13_2_A5_9_T1()
        => ExecutionTest("S11.13.2_A5.9_T1");

    [Fact(DisplayName = "S11.13.2_A5.9_T2.js")]
    public Task S11_13_2_A5_9_T2()
        => ExecutionTest("S11.13.2_A5.9_T2");

    [Fact(DisplayName = "S11.13.2_A5.9_T3.js")]
    public Task S11_13_2_A5_9_T3()
        => ExecutionTest("S11.13.2_A5.9_T3");

    [Fact(DisplayName = "S11.13.2_A6.10_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_10_T1()
        => ExecutionTest("S11.13.2_A6.10_T1");

    [Fact(DisplayName = "S11.13.2_A6.11_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_11_T1()
        => ExecutionTest("S11.13.2_A6.11_T1");

    [Fact(DisplayName = "S11.13.2_A6.1_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_1_T1()
        => ExecutionTest("S11.13.2_A6.1_T1");

    [Fact(DisplayName = "S11.13.2_A6.2_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_2_T1()
        => ExecutionTest("S11.13.2_A6.2_T1");

    [Fact(DisplayName = "S11.13.2_A6.3_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_3_T1()
        => ExecutionTest("S11.13.2_A6.3_T1");

    [Fact(DisplayName = "S11.13.2_A6.4_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_4_T1()
        => ExecutionTest("S11.13.2_A6.4_T1");

    [Fact(DisplayName = "S11.13.2_A6.5_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_5_T1()
        => ExecutionTest("S11.13.2_A6.5_T1");

    [Fact(DisplayName = "S11.13.2_A6.6_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_6_T1()
        => ExecutionTest("S11.13.2_A6.6_T1");

    [Fact(DisplayName = "S11.13.2_A6.7_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_7_T1()
        => ExecutionTest("S11.13.2_A6.7_T1");

    [Fact(DisplayName = "S11.13.2_A6.8_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_8_T1()
        => ExecutionTest("S11.13.2_A6.8_T1");

    [Fact(DisplayName = "S11.13.2_A6.9_T1.js", Skip = "eval is not supported.")]
    public Task S11_13_2_A6_9_T1()
        => ExecutionTest("S11.13.2_A6.9_T1");

    [Fact(DisplayName = "S11.13.2_A7.10_T4.js")]
    public Task S11_13_2_A7_10_T4()
        => ExecutionTest("S11.13.2_A7.10_T4");

    [Fact(DisplayName = "S11.13.2_A7.11_T4.js")]
    public Task S11_13_2_A7_11_T4()
        => ExecutionTest("S11.13.2_A7.11_T4");

    [Fact(DisplayName = "S11.13.2_A7.1_T4.js")]
    public Task S11_13_2_A7_1_T4()
        => ExecutionTest("S11.13.2_A7.1_T4");

    [Fact(DisplayName = "S11.13.2_A7.2_T4.js")]
    public Task S11_13_2_A7_2_T4()
        => ExecutionTest("S11.13.2_A7.2_T4");

    [Fact(DisplayName = "S11.13.2_A7.3_T4.js")]
    public Task S11_13_2_A7_3_T4()
        => ExecutionTest("S11.13.2_A7.3_T4");

    [Fact(DisplayName = "S11.13.2_A7.4_T4.js")]
    public Task S11_13_2_A7_4_T4()
        => ExecutionTest("S11.13.2_A7.4_T4");

    [Fact(DisplayName = "S11.13.2_A7.5_T4.js")]
    public Task S11_13_2_A7_5_T4()
        => ExecutionTest("S11.13.2_A7.5_T4");

    [Fact(DisplayName = "S11.13.2_A7.6_T4.js")]
    public Task S11_13_2_A7_6_T4()
        => ExecutionTest("S11.13.2_A7.6_T4");

    [Fact(DisplayName = "S11.13.2_A7.7_T4.js")]
    public Task S11_13_2_A7_7_T4()
        => ExecutionTest("S11.13.2_A7.7_T4");

    [Fact(DisplayName = "S11.13.2_A7.8_T4.js")]
    public Task S11_13_2_A7_8_T4()
        => ExecutionTest("S11.13.2_A7.8_T4");

    [Fact(DisplayName = "S11.13.2_A7.9_T4.js")]
    public Task S11_13_2_A7_9_T4()
        => ExecutionTest("S11.13.2_A7.9_T4");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--10.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__10()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--10");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--12.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__12()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--12");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--14.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__14()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--14");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--16.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__16()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--16");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--18.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__18()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--18");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--2.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__2()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--2");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--20.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__20()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--20");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--4.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__4()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--4");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--6.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__6()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--6");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v--8.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v__8()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v--8");

    [Fact(DisplayName = "compound-assignment-operator-calls-putvalue-lref--v-.js")]
    public Task compound_assignment_operator_calls_putvalue_lref__v_()
        => ExecutionTest("compound-assignment-operator-calls-putvalue-lref--v-");

}
