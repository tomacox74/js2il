namespace Jroc.Test262.Tests.language.statements.for_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.for_") { }

    [Fact(DisplayName = "S12.6.3_A5.js", Skip = "eval is not supported.")]
    public Task test_S12_6_3_A5()
        => ExecutionTest("S12.6.3_A5");

    [Fact(DisplayName = "cptn-decl-expr-iter.js", Skip = "eval is not supported.")]
    public Task test_cptn_decl_expr_iter()
        => ExecutionTest("cptn-decl-expr-iter");

    [Fact(DisplayName = "cptn-decl-expr-no-iter.js", Skip = "eval is not supported.")]
    public Task test_cptn_decl_expr_no_iter()
        => ExecutionTest("cptn-decl-expr-no-iter");

    [Fact(DisplayName = "cptn-expr-expr-iter.js", Skip = "eval is not supported.")]
    public Task test_cptn_expr_expr_iter()
        => ExecutionTest("cptn-expr-expr-iter");

    [Fact(DisplayName = "cptn-expr-expr-no-iter.js", Skip = "eval is not supported.")]
    public Task test_cptn_expr_expr_no_iter()
        => ExecutionTest("cptn-expr-expr-no-iter");

    [Fact(DisplayName = "head-init-expr-check-empty-inc-empty-completion.js", Skip = "eval is not supported.")]
    public Task test_head_init_expr_check_empty_inc_empty_completion()
        => ExecutionTest("head-init-expr-check-empty-inc-empty-completion");

    [Fact(DisplayName = "head-init-var-check-empty-inc-empty-completion.js", Skip = "eval is not supported.")]
    public Task test_head_init_var_check_empty_inc_empty_completion()
        => ExecutionTest("head-init-var-check-empty-inc-empty-completion");

    [Fact(DisplayName = "scope-body-lex-open.js")]
    public Task test_scope_body_lex_open()
        => ExecutionTest("scope-body-lex-open");

    [Fact(DisplayName = "scope-body-var-none.js", Skip = "eval is not supported.")]
    public Task test_scope_body_var_none()
        => ExecutionTest("scope-body-var-none");

    [Fact(DisplayName = "scope-head-var-none.js", Skip = "eval is not supported.")]
    public Task test_scope_head_var_none()
        => ExecutionTest("scope-head-var-none");

}
