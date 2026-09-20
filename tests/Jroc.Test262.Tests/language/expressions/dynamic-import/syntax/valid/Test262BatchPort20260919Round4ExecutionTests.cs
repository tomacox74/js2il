using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.dynamic_import.syntax.valid;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.expressions.dynamic_import.syntax.valid") { }

    [Fact(DisplayName = "nested-arrow-assignment-expression-empty-str-is-valid-assign-expr")]
    public Task nested_arrow_assignment_expression_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-assignment-expression-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-arrow-empty-str-is-valid-assign-expr")]
    public Task nested_arrow_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-arrow-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-await-empty-str-is-valid-assign-expr")]
    public Task nested_async_arrow_function_await_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-await-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-arrow-function-return-await-empty-str-is-valid-assign-expr")]
    public Task nested_async_arrow_function_return_await_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-arrow-function-return-await-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-await-empty-str-is-valid-assign-expr")]
    public Task nested_async_function_await_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-await-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-empty-str-is-valid-assign-expr")]
    public Task nested_async_function_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-function-return-await-empty-str-is-valid-assign-expr")]
    public Task nested_async_function_return_await_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-function-return-await-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-async-gen-await-empty-str-is-valid-assign-expr")]
    public Task nested_async_gen_await_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-async-gen-await-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-empty-str-is-valid-assign-expr")]
    public Task nested_function_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-empty-str-is-valid-assign-expr");

    [Fact(DisplayName = "nested-function-return-empty-str-is-valid-assign-expr")]
    public Task nested_function_return_empty_str_is_valid_assign_expr()
        => ExecutionTest("nested-function-return-empty-str-is-valid-assign-expr");

}
