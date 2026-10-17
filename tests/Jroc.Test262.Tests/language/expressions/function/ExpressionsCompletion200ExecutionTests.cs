namespace Jroc.Test262.Tests.language.expressions.function;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task eval_var_scope_syntax_err()
        => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task named_no_strict_reassign_fn_name_in_body_in_eval()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task named_strict_error_reassign_fn_name_in_body_in_eval()
        => ExecutionTest("named-strict-error-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "static-init-await-binding.js")]
    public Task static_init_await_binding()
        => ExecutionTest("static-init-await-binding");

    [Fact(DisplayName = "static-init-await-reference.js")]
    public Task static_init_await_reference()
        => ExecutionTest("static-init-await-reference");

    [Fact(DisplayName = "unscopables-with-in-nested-fn.js")]
    public Task unscopables_with_in_nested_fn()
        => ExecutionTest("unscopables-with-in-nested-fn");

    [Fact(DisplayName = "unscopables-with.js")]
    public Task unscopables_with()
        => ExecutionTest("unscopables-with");
}
