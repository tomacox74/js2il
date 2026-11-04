namespace Jroc.Test262.Tests.language.statements.try_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.try_") { }

    [Fact(DisplayName = "catch-parameter-boundnames-restriction-arguments-eval-throws.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_catch_parameter_boundnames_restriction_arguments_eval_throws()
        => ExecutionTest("catch-parameter-boundnames-restriction-arguments-eval-throws");

    [Fact(DisplayName = "catch-parameter-boundnames-restriction-eval-eval-throws.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_catch_parameter_boundnames_restriction_eval_eval_throws()
        => ExecutionTest("catch-parameter-boundnames-restriction-eval-eval-throws");

    [Fact(DisplayName = "completion-values.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_completion_values()
        => ExecutionTest("completion-values");

    [Fact(DisplayName = "cptn-catch-empty-break.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_catch_empty_break()
        => ExecutionTest("cptn-catch-empty-break");

    [Fact(DisplayName = "cptn-catch-empty-continue.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_catch_empty_continue()
        => ExecutionTest("cptn-catch-empty-continue");

    [Fact(DisplayName = "cptn-catch-finally-empty-break.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_catch_finally_empty_break()
        => ExecutionTest("cptn-catch-finally-empty-break");

    [Fact(DisplayName = "cptn-catch-finally-empty-continue.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_catch_finally_empty_continue()
        => ExecutionTest("cptn-catch-finally-empty-continue");

    [Fact(DisplayName = "cptn-catch.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_catch()
        => ExecutionTest("cptn-catch");

    [Fact(DisplayName = "cptn-finally-empty-break.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_finally_empty_break()
        => ExecutionTest("cptn-finally-empty-break");

    [Fact(DisplayName = "cptn-finally-empty-continue.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_finally_empty_continue()
        => ExecutionTest("cptn-finally-empty-continue");

    [Fact(DisplayName = "cptn-finally-from-catch.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_finally_from_catch()
        => ExecutionTest("cptn-finally-from-catch");

    [Fact(DisplayName = "cptn-finally-skip-catch.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_finally_skip_catch()
        => ExecutionTest("cptn-finally-skip-catch");

    [Fact(DisplayName = "cptn-finally-wo-catch.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_finally_wo_catch()
        => ExecutionTest("cptn-finally-wo-catch");

    [Fact(DisplayName = "cptn-try.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_try()
        => ExecutionTest("cptn-try");

    [Fact(DisplayName = "scope-catch-block-lex-open.js")]
    public Task test_scope_catch_block_lex_open()
        => ExecutionTest("scope-catch-block-lex-open");

    [Fact(DisplayName = "scope-catch-param-lex-close.js")]
    public Task test_scope_catch_param_lex_close()
        => ExecutionTest("scope-catch-param-lex-close");

    [Fact(DisplayName = "scope-catch-param-lex-open.js")]
    public Task test_scope_catch_param_lex_open()
        => ExecutionTest("scope-catch-param-lex-open");

    [Fact(DisplayName = "scope-catch-param-var-none.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_scope_catch_param_var_none()
        => ExecutionTest("scope-catch-param-var-none");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task test_static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");

    [Fact(DisplayName = "tco-catch-finally.js")]
    public Task test_tco_catch_finally()
        => ExecutionTest("tco-catch-finally");

    [Fact(DisplayName = "tco-catch.js")]
    public Task test_tco_catch()
        => ExecutionTest("tco-catch");

    [Fact(DisplayName = "tco-finally.js")]
    public Task test_tco_finally()
        => ExecutionTest("tco-finally");

}
