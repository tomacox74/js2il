namespace Jroc.Test262.Tests.language.expressions.dynamic_import.import_attributes;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.import_attributes") { }

    [Fact(DisplayName = "2nd-param-await-expr.js")]
    public Task test_2nd_param_await_expr()
        => ExecutionTest("2nd-param-await-expr");

    [Fact(DisplayName = "2nd-param-await-ident.js")]
    public Task test_2nd_param_await_ident()
        => ExecutionTest("2nd-param-await-ident");

    [Fact(DisplayName = "2nd-param-evaluation-abrupt-return.js")]
    public Task test_2nd_param_evaluation_abrupt_return()
        => ExecutionTest("2nd-param-evaluation-abrupt-return");

    [Fact(DisplayName = "2nd-param-evaluation-abrupt-throw.js")]
    public Task test_2nd_param_evaluation_abrupt_throw()
        => ExecutionTest("2nd-param-evaluation-abrupt-throw");

    [Fact(DisplayName = "2nd-param-evaluation-sequence.js")]
    public Task test_2nd_param_evaluation_sequence()
        => ExecutionTest("2nd-param-evaluation-sequence");

    [Fact(DisplayName = "2nd-param-get-with-error.js")]
    public Task test_2nd_param_get_with_error()
        => ExecutionTest("2nd-param-get-with-error");

    [Fact(DisplayName = "2nd-param-in.js")]
    public Task test_2nd_param_in()
        => ExecutionTest("2nd-param-in");

    [Fact(DisplayName = "2nd-param-non-object.js")]
    public Task test_2nd_param_non_object()
        => ExecutionTest("2nd-param-non-object");

    [Fact(DisplayName = "2nd-param-trailing-comma-fulfill.js")]
    public Task test_2nd_param_trailing_comma_fulfill()
        => ExecutionTest("2nd-param-trailing-comma-fulfill");

    [Fact(DisplayName = "2nd-param-trailing-comma-reject.js")]
    public Task test_2nd_param_trailing_comma_reject()
        => ExecutionTest("2nd-param-trailing-comma-reject");

    [Fact(DisplayName = "2nd-param-with-enumeration-abrupt.js")]
    public Task test_2nd_param_with_enumeration_abrupt()
        => ExecutionTest("2nd-param-with-enumeration-abrupt");

    [Fact(DisplayName = "2nd-param-with-enumeration-enumerable.js")]
    public Task test_2nd_param_with_enumeration_enumerable()
        => ExecutionTest("2nd-param-with-enumeration-enumerable");

    [Fact(DisplayName = "2nd-param-with-enumeration.js")]
    public Task test_2nd_param_with_enumeration()
        => ExecutionTest("2nd-param-with-enumeration");

    [Fact(DisplayName = "2nd-param-with-non-object.js")]
    public Task test_2nd_param_with_non_object()
        => ExecutionTest("2nd-param-with-non-object");

    [Fact(DisplayName = "2nd-param-with-type-text.js")]
    public Task test_2nd_param_with_type_text()
        => ExecutionTest("2nd-param-with-type-text");

    [Fact(DisplayName = "2nd-param-with-undefined.js")]
    public Task test_2nd_param_with_undefined()
        => ExecutionTest("2nd-param-with-undefined");

    [Fact(DisplayName = "2nd-param-with-value-abrupt.js")]
    public Task test_2nd_param_with_value_abrupt()
        => ExecutionTest("2nd-param-with-value-abrupt");

    [Fact(DisplayName = "2nd-param-with-value-non-string.js")]
    public Task test_2nd_param_with_value_non_string()
        => ExecutionTest("2nd-param-with-value-non-string");

    [Fact(DisplayName = "2nd-param-yield-expr.js")]
    public Task test_2nd_param_yield_expr()
        => ExecutionTest("2nd-param-yield-expr");

    [Fact(DisplayName = "2nd-param-yield-ident-invalid.js")]
    public Task test_2nd_param_yield_ident_invalid()
        => CompilationFailureTest("2nd-param-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "2nd-param-yield-ident-valid.js")]
    public Task test_2nd_param_yield_ident_valid()
        => ExecutionTest("2nd-param-yield-ident-valid");

    [Fact(DisplayName = "trailing-comma-fulfill.js")]
    public Task trailing_comma_fulfill()
        => ExecutionTest("trailing-comma-fulfill");

    [Fact(DisplayName = "trailing-comma-reject.js")]
    public Task trailing_comma_reject()
        => ExecutionTest("trailing-comma-reject");
}
