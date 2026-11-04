namespace Jroc.Test262.Tests.language.statementList;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.statementList") { }

    [Fact(DisplayName = "eval-block-array-literal-with-item.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_array_literal_with_item()
        => ExecutionTest("eval-block-array-literal-with-item");

    [Fact(DisplayName = "eval-block-array-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_array_literal()
        => ExecutionTest("eval-block-array-literal");

    [Fact(DisplayName = "eval-block-arrow-function-assignment-expr.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_arrow_function_assignment_expr()
        => ExecutionTest("eval-block-arrow-function-assignment-expr");

    [Fact(DisplayName = "eval-block-arrow-function-functionbody.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_arrow_function_functionbody()
        => ExecutionTest("eval-block-arrow-function-functionbody");

    [Fact(DisplayName = "eval-block-block-with-labels.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_block_with_labels()
        => ExecutionTest("eval-block-block-with-labels");

    [Fact(DisplayName = "eval-block-block.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_block()
        => ExecutionTest("eval-block-block");

    [Fact(DisplayName = "eval-block-expr-arrow-function-boolean-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_expr_arrow_function_boolean_literal()
        => ExecutionTest("eval-block-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "eval-block-let-declaration.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_let_declaration()
        => ExecutionTest("eval-block-let-declaration");

    [Fact(DisplayName = "eval-block-regexp-literal-flags.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_regexp_literal_flags()
        => ExecutionTest("eval-block-regexp-literal-flags");

    [Fact(DisplayName = "eval-block-regexp-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_regexp_literal()
        => ExecutionTest("eval-block-regexp-literal");

    [Fact(DisplayName = "eval-block-with-statment-array-literal-with-item.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_array_literal_with_item()
        => ExecutionTest("eval-block-with-statment-array-literal-with-item");

    [Fact(DisplayName = "eval-block-with-statment-array-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_array_literal()
        => ExecutionTest("eval-block-with-statment-array-literal");

    [Fact(DisplayName = "eval-block-with-statment-arrow-function-assignment-expr.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_arrow_function_assignment_expr()
        => ExecutionTest("eval-block-with-statment-arrow-function-assignment-expr");

    [Fact(DisplayName = "eval-block-with-statment-arrow-function-functionbody.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_arrow_function_functionbody()
        => ExecutionTest("eval-block-with-statment-arrow-function-functionbody");

    [Fact(DisplayName = "eval-block-with-statment-block-with-labels.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_block_with_labels()
        => ExecutionTest("eval-block-with-statment-block-with-labels");

    [Fact(DisplayName = "eval-block-with-statment-block.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_block()
        => ExecutionTest("eval-block-with-statment-block");

    [Fact(DisplayName = "eval-block-with-statment-expr-arrow-function-boolean-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_expr_arrow_function_boolean_literal()
        => ExecutionTest("eval-block-with-statment-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "eval-block-with-statment-let-declaration.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_let_declaration()
        => ExecutionTest("eval-block-with-statment-let-declaration");

    [Fact(DisplayName = "eval-block-with-statment-regexp-literal-flags.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_regexp_literal_flags()
        => ExecutionTest("eval-block-with-statment-regexp-literal-flags");

    [Fact(DisplayName = "eval-block-with-statment-regexp-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_block_with_statment_regexp_literal()
        => ExecutionTest("eval-block-with-statment-regexp-literal");

    [Fact(DisplayName = "eval-class-array-literal-with-item.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_array_literal_with_item()
        => ExecutionTest("eval-class-array-literal-with-item");

    [Fact(DisplayName = "eval-class-array-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_array_literal()
        => ExecutionTest("eval-class-array-literal");

    [Fact(DisplayName = "eval-class-arrow-function-assignment-expr.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_arrow_function_assignment_expr()
        => ExecutionTest("eval-class-arrow-function-assignment-expr");

    [Fact(DisplayName = "eval-class-arrow-function-functionbody.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_arrow_function_functionbody()
        => ExecutionTest("eval-class-arrow-function-functionbody");

    [Fact(DisplayName = "eval-class-block-with-labels.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_block_with_labels()
        => ExecutionTest("eval-class-block-with-labels");

    [Fact(DisplayName = "eval-class-block.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_block()
        => ExecutionTest("eval-class-block");

    [Fact(DisplayName = "eval-class-expr-arrow-function-boolean-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_expr_arrow_function_boolean_literal()
        => ExecutionTest("eval-class-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "eval-class-let-declaration.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_let_declaration()
        => ExecutionTest("eval-class-let-declaration");

    [Fact(DisplayName = "eval-class-regexp-literal-flags.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_regexp_literal_flags()
        => ExecutionTest("eval-class-regexp-literal-flags");

    [Fact(DisplayName = "eval-class-regexp-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_class_regexp_literal()
        => ExecutionTest("eval-class-regexp-literal");

    [Fact(DisplayName = "eval-fn-array-literal-with-item.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_array_literal_with_item()
        => ExecutionTest("eval-fn-array-literal-with-item");

    [Fact(DisplayName = "eval-fn-array-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_array_literal()
        => ExecutionTest("eval-fn-array-literal");

    [Fact(DisplayName = "eval-fn-arrow-function-assignment-expr.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_arrow_function_assignment_expr()
        => ExecutionTest("eval-fn-arrow-function-assignment-expr");

    [Fact(DisplayName = "eval-fn-arrow-function-functionbody.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_arrow_function_functionbody()
        => ExecutionTest("eval-fn-arrow-function-functionbody");

    [Fact(DisplayName = "eval-fn-block-with-labels.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_block_with_labels()
        => ExecutionTest("eval-fn-block-with-labels");

    [Fact(DisplayName = "eval-fn-block.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_block()
        => ExecutionTest("eval-fn-block");

    [Fact(DisplayName = "eval-fn-expr-arrow-function-boolean-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_expr_arrow_function_boolean_literal()
        => ExecutionTest("eval-fn-expr-arrow-function-boolean-literal");

    [Fact(DisplayName = "eval-fn-let-declaration.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_let_declaration()
        => ExecutionTest("eval-fn-let-declaration");

    [Fact(DisplayName = "eval-fn-regexp-literal-flags.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_regexp_literal_flags()
        => ExecutionTest("eval-fn-regexp-literal-flags");

    [Fact(DisplayName = "eval-fn-regexp-literal.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task eval_fn_regexp_literal()
        => ExecutionTest("eval-fn-regexp-literal");

}
