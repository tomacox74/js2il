namespace Jroc.Test262.Tests.language.statements.async_function;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.async_function") { }

    [Fact(DisplayName = "cptn-decl.js", Skip = "Requires unsupported eval.")]
    public Task cptn_decl()
        => ExecutionTest("cptn-decl");

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "Requires unsupported eval.")]
    public Task eval_var_scope_syntax_err()
        => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "evaluation-body-that-throws-after-await.js")]
    public Task evaluation_body_that_throws_after_await()
        => ExecutionTest("evaluation-body-that-throws-after-await");

    [Fact(DisplayName = "evaluation-body-that-throws.js")]
    public Task evaluation_body_that_throws()
        => ExecutionTest("evaluation-body-that-throws");

    [Fact(DisplayName = "evaluation-body.js")]
    public Task evaluation_body()
        => ExecutionTest("evaluation-body");

    [Fact(DisplayName = "evaluation-default-that-throws.js")]
    public Task evaluation_default_that_throws()
        => ExecutionTest("evaluation-default-that-throws");

    [Fact(DisplayName = "evaluation-mapped-arguments.js")]
    public Task evaluation_mapped_arguments()
        => ExecutionTest("evaluation-mapped-arguments");

    [Fact(DisplayName = "evaluation-this-value-global.js")]
    public Task evaluation_this_value_global()
        => ExecutionTest("evaluation-this-value-global");

    [Fact(DisplayName = "evaluation-this-value-passed.js")]
    public Task evaluation_this_value_passed()
        => ExecutionTest("evaluation-this-value-passed");

    [Fact(DisplayName = "evaluation-unmapped-arguments.js")]
    public Task evaluation_unmapped_arguments()
        => ExecutionTest("evaluation-unmapped-arguments");

    [Fact(DisplayName = "returns-async-arrow-returns-newtarget.js")]
    public Task returns_async_arrow_returns_newtarget()
        => ExecutionTest("returns-async-arrow-returns-newtarget");

    [Fact(DisplayName = "returns-async-function-returns-arguments-from-own-function.js")]
    public Task returns_async_function_returns_arguments_from_own_function()
        => ExecutionTest("returns-async-function-returns-arguments-from-own-function");

    [Fact(DisplayName = "returns-async-function-returns-newtarget.js")]
    public Task returns_async_function_returns_newtarget()
        => ExecutionTest("returns-async-function-returns-newtarget");

    [Fact(DisplayName = "syntax-declaration.js")]
    public Task syntax_declaration()
        => ExecutionTest("syntax-declaration");

    [Fact(DisplayName = "try-reject-finally-reject.js")]
    public Task try_reject_finally_reject()
        => ExecutionTest("try-reject-finally-reject");
}
