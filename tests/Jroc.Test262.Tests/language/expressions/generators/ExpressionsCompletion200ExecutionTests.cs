namespace Jroc.Test262.Tests.language.expressions.generators;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.generators") { }

    [Fact(DisplayName = "default-proto.js")]
    public Task default_proto()
        => ExecutionTest("default-proto");

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "eval-body-proto-realm.js", Skip = "eval is not supported.")]
    public Task eval_body_proto_realm()
        => ExecutionTest("eval-body-proto-realm");

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task eval_var_scope_syntax_err()
        => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body-in-arrow.js")]
    public Task named_no_strict_reassign_fn_name_in_body_in_arrow()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body-in-arrow");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task named_no_strict_reassign_fn_name_in_body_in_eval()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body.js")]
    public Task named_no_strict_reassign_fn_name_in_body()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task named_strict_error_reassign_fn_name_in_body_in_eval()
        => ExecutionTest("named-strict-error-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "scope-body-lex-distinct.js", Skip = "eval is not supported.")]
    public Task scope_body_lex_distinct()
        => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "scope-name-var-close.js")]
    public Task scope_name_var_close()
        => ExecutionTest("scope-name-var-close");

    [Fact(DisplayName = "scope-name-var-open-non-strict.js")]
    public Task scope_name_var_open_non_strict()
        => ExecutionTest("scope-name-var-open-non-strict");

    [Fact(DisplayName = "scope-name-var-open-strict.js")]
    public Task scope_name_var_open_strict()
        => ExecutionTest("scope-name-var-open-strict");

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
