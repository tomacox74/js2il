using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_function;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.async_function") { }

    [Fact(DisplayName = "named-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_named_eval_var_scope_syntax_err() => ExecutionTest("named-eval-var-scope-syntax-err");

    [Fact(DisplayName = "named-params-trailing-comma-single.js")]
    public Task test_named_params_trailing_comma_single() => ExecutionTest("named-params-trailing-comma-single");

    [Fact(DisplayName = "named-reassign-fn-name-in-body-in-arrow.js")]
    public Task test_named_reassign_fn_name_in_body_in_arrow() => ExecutionTest("named-reassign-fn-name-in-body-in-arrow");

    [Fact(DisplayName = "named-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task test_named_reassign_fn_name_in_body_in_eval() => ExecutionTest("named-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "named-reassign-fn-name-in-body.js")]
    public Task test_named_reassign_fn_name_in_body() => ExecutionTest("named-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-returns-async-arrow-returns-arguments-from-parent-function.js")]
    public Task test_named_returns_async_arrow_returns_arguments_from_parent_function() => ExecutionTest("named-returns-async-arrow-returns-arguments-from-parent-function");

    [Fact(DisplayName = "named-returns-async-arrow-returns-newtarget.js")]
    public Task test_named_returns_async_arrow_returns_newtarget() => ExecutionTest("named-returns-async-arrow-returns-newtarget");

    [Fact(DisplayName = "named-returns-async-arrow.js")]
    public Task test_named_returns_async_arrow() => ExecutionTest("named-returns-async-arrow");

    [Fact(DisplayName = "named-returns-async-function-returns-arguments-from-own-function.js")]
    public Task test_named_returns_async_function_returns_arguments_from_own_function() => ExecutionTest("named-returns-async-function-returns-arguments-from-own-function");

    [Fact(DisplayName = "named-returns-async-function-returns-newtarget.js")]
    public Task test_named_returns_async_function_returns_newtarget() => ExecutionTest("named-returns-async-function-returns-newtarget");

    [Fact(DisplayName = "named-returns-async-function.js")]
    public Task test_named_returns_async_function() => ExecutionTest("named-returns-async-function");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body-in-arrow.js")]
    public Task test_named_strict_error_reassign_fn_name_in_body_in_arrow() => ExecutionTest("named-strict-error-reassign-fn-name-in-body-in-arrow");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body-in-eval.js", Skip = "eval is not supported.")]
    public Task test_named_strict_error_reassign_fn_name_in_body_in_eval() => ExecutionTest("named-strict-error-reassign-fn-name-in-body-in-eval");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body.js")]
    public Task test_named_strict_error_reassign_fn_name_in_body() => ExecutionTest("named-strict-error-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-unscopables-with-in-nested-fn.js")]
    public Task test_named_unscopables_with_in_nested_fn() => ExecutionTest("named-unscopables-with-in-nested-fn");

    [Fact(DisplayName = "named-unscopables-with.js")]
    public Task test_named_unscopables_with() => ExecutionTest("named-unscopables-with");

    [Fact(DisplayName = "nameless-dflt-params-abrupt.js")]
    public Task test_nameless_dflt_params_abrupt() => ExecutionTest("nameless-dflt-params-abrupt");

    [Fact(DisplayName = "nameless-dflt-params-arg-val-not-undefined.js")]
    public Task test_nameless_dflt_params_arg_val_not_undefined() => ExecutionTest("nameless-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "nameless-dflt-params-arg-val-undefined.js")]
    public Task test_nameless_dflt_params_arg_val_undefined() => ExecutionTest("nameless-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "nameless-dflt-params-ref-later.js")]
    public Task test_nameless_dflt_params_ref_later() => ExecutionTest("nameless-dflt-params-ref-later");

    [Fact(DisplayName = "nameless-dflt-params-ref-prior.js")]
    public Task test_nameless_dflt_params_ref_prior() => ExecutionTest("nameless-dflt-params-ref-prior");

    [Fact(DisplayName = "nameless-dflt-params-ref-self.js")]
    public Task test_nameless_dflt_params_ref_self() => ExecutionTest("nameless-dflt-params-ref-self");

    [Fact(DisplayName = "nameless-dflt-params-trailing-comma.js")]
    public Task test_nameless_dflt_params_trailing_comma() => ExecutionTest("nameless-dflt-params-trailing-comma");

    [Fact(DisplayName = "nameless-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_nameless_eval_var_scope_syntax_err() => ExecutionTest("nameless-eval-var-scope-syntax-err");

    [Fact(DisplayName = "nameless-params-trailing-comma-multiple.js")]
    public Task test_nameless_params_trailing_comma_multiple() => ExecutionTest("nameless-params-trailing-comma-multiple");

    [Fact(DisplayName = "nameless-params-trailing-comma-single.js")]
    public Task test_nameless_params_trailing_comma_single() => ExecutionTest("nameless-params-trailing-comma-single");

    [Fact(DisplayName = "nameless-unscopables-with-in-nested-fn.js")]
    public Task test_nameless_unscopables_with_in_nested_fn() => ExecutionTest("nameless-unscopables-with-in-nested-fn");

    [Fact(DisplayName = "nameless-unscopables-with.js")]
    public Task test_nameless_unscopables_with() => ExecutionTest("nameless-unscopables-with");

    [Fact(DisplayName = "try-reject-finally-reject.js")]
    public Task test_try_reject_finally_reject() => ExecutionTest("try-reject-finally-reject");

    [Fact(DisplayName = "try-reject-finally-return.js")]
    public Task test_try_reject_finally_return() => ExecutionTest("try-reject-finally-return");

    [Fact(DisplayName = "try-reject-finally-throw.js")]
    public Task test_try_reject_finally_throw() => ExecutionTest("try-reject-finally-throw");

    [Fact(DisplayName = "try-return-finally-reject.js")]
    public Task test_try_return_finally_reject() => ExecutionTest("try-return-finally-reject");

    [Fact(DisplayName = "try-return-finally-return.js")]
    public Task test_try_return_finally_return() => ExecutionTest("try-return-finally-return");

    [Fact(DisplayName = "try-return-finally-throw.js")]
    public Task test_try_return_finally_throw() => ExecutionTest("try-return-finally-throw");

    [Fact(DisplayName = "try-throw-finally-reject.js")]
    public Task test_try_throw_finally_reject() => ExecutionTest("try-throw-finally-reject");

    [Fact(DisplayName = "try-throw-finally-return.js")]
    public Task test_try_throw_finally_return() => ExecutionTest("try-throw-finally-return");

    [Fact(DisplayName = "try-throw-finally-throw.js")]
    public Task test_try_throw_finally_throw() => ExecutionTest("try-throw-finally-throw");
}
