namespace Jroc.Test262.Tests.language.expressions.dynamic_import.assignment_expression;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.assignment_expression") { }

    [Fact(DisplayName = "additive-expr.js")]
    public Task additive_expr()
        => ExecutionTest("additive-expr");

    [Fact(DisplayName = "array-literal.js")]
    public Task array_literal()
        => ExecutionTest("array-literal");

    [Fact(DisplayName = "arrow-function.js")]
    public Task arrow_function()
        => ExecutionTest("arrow-function");

    [Fact(DisplayName = "await-expr.js")]
    public Task await_expr()
        => ExecutionTest("await-expr");

    [Fact(DisplayName = "await-identifier.js")]
    public Task await_identifier()
        => ExecutionTest("await-identifier");

    [Fact(DisplayName = "call-expr-arguments.js")]
    public Task call_expr_arguments()
        => ExecutionTest("call-expr-arguments");

    [Fact(DisplayName = "call-expr-expr.js")]
    public Task call_expr_expr()
        => ExecutionTest("call-expr-expr");

    [Fact(DisplayName = "call-expr-identifier.js")]
    public Task call_expr_identifier()
        => ExecutionTest("call-expr-identifier");

    [Fact(DisplayName = "cover-call-expr.js")]
    public Task cover_call_expr()
        => ExecutionTest("cover-call-expr");

    [Fact(DisplayName = "cover-parenthesized-expr.js")]
    public Task cover_parenthesized_expr()
        => ExecutionTest("cover-parenthesized-expr");

    [Fact(DisplayName = "identifier.js")]
    public Task identifier()
        => ExecutionTest("identifier");

    [Fact(DisplayName = "import-meta.js")]
    public Task import_meta()
        => ExecutionTest("import-meta");

    [Fact(DisplayName = "lhs-assign-operator-assign-expr.js")]
    public Task lhs_assign_operator_assign_expr()
        => ExecutionTest("lhs-assign-operator-assign-expr");

    [Fact(DisplayName = "lhs-eq-assign-expr-nostrict.js")]
    public Task lhs_eq_assign_expr_nostrict()
        => ExecutionTest("lhs-eq-assign-expr-nostrict");

    [Fact(DisplayName = "lhs-eq-assign-expr.js")]
    public Task lhs_eq_assign_expr()
        => ExecutionTest("lhs-eq-assign-expr");

    [Fact(DisplayName = "logical-and-expr.js")]
    public Task logical_and_expr()
        => ExecutionTest("logical-and-expr");

    [Fact(DisplayName = "logical-or-expr.js")]
    public Task logical_or_expr()
        => ExecutionTest("logical-or-expr");

    [Fact(DisplayName = "member-expr.js")]
    public Task member_expr()
        => ExecutionTest("member-expr");

    [Fact(DisplayName = "new-target.js")]
    public Task new_target()
        => ExecutionTest("new-target");

    [Fact(DisplayName = "object-literal.js")]
    public Task object_literal()
        => ExecutionTest("object-literal");

    [Fact(DisplayName = "tagged-function-call.js")]
    public Task tagged_function_call()
        => ExecutionTest("tagged-function-call");

    [Fact(DisplayName = "ternary.js")]
    public Task ternary()
        => ExecutionTest("ternary");

    [Fact(DisplayName = "this.js")]
    public Task test_this()
        => ExecutionTest("this");

    [Fact(DisplayName = "unary-expr.js")]
    public Task unary_expr()
        => ExecutionTest("unary-expr");

    [Fact(DisplayName = "yield-assign-expr.js")]
    public Task yield_assign_expr()
        => ExecutionTest("yield-assign-expr");

    [Fact(DisplayName = "yield-expr.js")]
    public Task yield_expr()
        => ExecutionTest("yield-expr");

    [Fact(DisplayName = "yield-identifier.js")]
    public Task yield_identifier()
        => ExecutionTest("yield-identifier");
}
