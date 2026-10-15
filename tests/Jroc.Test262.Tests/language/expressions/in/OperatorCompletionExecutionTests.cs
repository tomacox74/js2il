using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.in_;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.in") { }

    [Fact(DisplayName = "private-field-in-nested.js")]
    public Task test_private_field_in_nested() => CompilationFailureTest("private-field-in-nested", string.Empty);

    [Fact(DisplayName = "private-field-in.js")]
    public Task test_private_field_in() => CompilationFailureTest("private-field-in", string.Empty);

    [Fact(DisplayName = "private-field-invalid-assignment-target.js")]
    public Task test_private_field_invalid_assignment_target() => CompilationFailureTest("private-field-invalid-assignment-target", string.Empty);

    [Fact(DisplayName = "private-field-invalid-identifier-complex.js")]
    public Task test_private_field_invalid_identifier_complex() => CompilationFailureTest("private-field-invalid-identifier-complex", string.Empty);

    [Fact(DisplayName = "private-field-invalid-identifier-simple.js")]
    public Task test_private_field_invalid_identifier_simple() => CompilationFailureTest("private-field-invalid-identifier-simple", string.Empty);

    [Fact(DisplayName = "private-field-invalid-rhs.js")]
    public Task test_private_field_invalid_rhs() => CompilationFailureTest("private-field-invalid-rhs", string.Empty);

    [Fact(DisplayName = "private-field-presence-accessor-shadowed.js")]
    public Task test_private_field_presence_accessor_shadowed() => ExecutionTest("private-field-presence-accessor-shadowed");

    [Fact(DisplayName = "private-field-presence-accessor.js")]
    public Task test_private_field_presence_accessor() => ExecutionTest("private-field-presence-accessor");

    [Fact(DisplayName = "private-field-presence-method-shadowed.js")]
    public Task test_private_field_presence_method_shadowed() => ExecutionTest("private-field-presence-method-shadowed");

    [Fact(DisplayName = "private-field-presence-method.js")]
    public Task test_private_field_presence_method() => ExecutionTest("private-field-presence-method");

    [Fact(DisplayName = "private-field-rhs-await-absent.js")]
    public Task test_private_field_rhs_await_absent() => ExecutionTest("private-field-rhs-await-absent");

    [Fact(DisplayName = "private-field-rhs-await-present.js")]
    public Task test_private_field_rhs_await_present() => ExecutionTest("private-field-rhs-await-present");

    [Fact(DisplayName = "private-field-rhs-unresolvable.js")]
    public Task test_private_field_rhs_unresolvable() => ExecutionTest("private-field-rhs-unresolvable");

    [Fact(DisplayName = "private-field-rhs-yield-absent.js")]
    public Task test_private_field_rhs_yield_absent() => CompilationFailureTest("private-field-rhs-yield-absent", string.Empty);

    [Fact(DisplayName = "private-field-rhs-yield-present.js")]
    public Task test_private_field_rhs_yield_present() => ExecutionTest("private-field-rhs-yield-present");

    [Fact(DisplayName = "rhs-yield-absent-non-strict.js")]
    public Task test_rhs_yield_absent_non_strict() => ExecutionTest("rhs-yield-absent-non-strict");

    [Fact(DisplayName = "rhs-yield-absent-strict.js")]
    public Task test_rhs_yield_absent_strict() => CompilationFailureTest("rhs-yield-absent-strict", string.Empty);

    [Fact(DisplayName = "rhs-yield-present.js")]
    public Task test_rhs_yield_present() => ExecutionTest("rhs-yield-present");

    [Fact(DisplayName = "S11.8.7_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_8_7_A1() => ExecutionTest("S11.8.7_A1");

    [Fact(DisplayName = "S11.8.7_A2.4_T1.js")]
    public Task test_S11_8_7_A2_4_T1() => ExecutionTest("S11.8.7_A2.4_T1");

    [Fact(DisplayName = "S11.8.7_A2.4_T2.js")]
    public Task test_S11_8_7_A2_4_T2() => ExecutionTest("S11.8.7_A2.4_T2");

    [Fact(DisplayName = "S11.8.7_A2.4_T3.js")]
    public Task test_S11_8_7_A2_4_T3() => ExecutionTest("S11.8.7_A2.4_T3");

    [Fact(DisplayName = "S11.8.7_A2.4_T4.js")]
    public Task test_S11_8_7_A2_4_T4() => ExecutionTest("S11.8.7_A2.4_T4");

    [Fact(DisplayName = "S8.12.6_A1.js")]
    public Task test_S8_12_6_A1() => ExecutionTest("S8.12.6_A1");

    [Fact(DisplayName = "S8.12.6_A2_T1.js")]
    public Task test_S8_12_6_A2_T1() => ExecutionTest("S8.12.6_A2_T1");

    [Fact(DisplayName = "S8.12.6_A2_T2.js")]
    public Task test_S8_12_6_A2_T2() => ExecutionTest("S8.12.6_A2_T2");

    [Fact(DisplayName = "S8.12.6_A3.js")]
    public Task test_S8_12_6_A3() => ExecutionTest("S8.12.6_A3");
}
