namespace Jroc.Test262.Tests.language.expressions.object_.method_definition;

public sealed class ObjectMethodCompletionTests : DiskExecutionTestsBase
{
    public ObjectMethodCompletionTests() : base("language.expressions.object_.method_definition") { }

    [Fact(DisplayName = "async-gen-meth-dflt-params-arg-val-not-undefined.js")]
    public Task test_async_gen_meth_dflt_params_arg_val_not_undefined() => ExecutionTest("async-gen-meth-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "async-gen-meth-dflt-params-arg-val-undefined.js")]
    public Task test_async_gen_meth_dflt_params_arg_val_undefined() => ExecutionTest("async-gen-meth-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "async-gen-meth-dflt-params-ref-prior.js")]
    public Task test_async_gen_meth_dflt_params_ref_prior() => ExecutionTest("async-gen-meth-dflt-params-ref-prior");

    [Fact(DisplayName = "async-gen-meth-dflt-params-trailing-comma.js")]
    public Task test_async_gen_meth_dflt_params_trailing_comma() => ExecutionTest("async-gen-meth-dflt-params-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_async_gen_meth_eval_var_scope_syntax_err() => ExecutionTest("async-gen-meth-eval-var-scope-syntax-err");

    [Fact(DisplayName = "async-gen-meth-params-trailing-comma-multiple.js")]
    public Task test_async_gen_meth_params_trailing_comma_multiple() => ExecutionTest("async-gen-meth-params-trailing-comma-multiple");

    [Fact(DisplayName = "async-gen-meth-params-trailing-comma-single.js")]
    public Task test_async_gen_meth_params_trailing_comma_single() => ExecutionTest("async-gen-meth-params-trailing-comma-single");

    [Fact(DisplayName = "async-gen-yield-identifier-non-strict.js")]
    public Task test_async_gen_yield_identifier_non_strict() => ExecutionTest("async-gen-yield-identifier-non-strict");

    [Fact(DisplayName = "async-gen-yield-identifier-spread-non-strict.js")]
    public Task test_async_gen_yield_identifier_spread_non_strict() => ExecutionTest("async-gen-yield-identifier-spread-non-strict");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next-catch.js")]
    public Task test_async_gen_yield_promise_reject_next_catch() => ExecutionTest("async-gen-yield-promise-reject-next-catch");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next-for-await-of-async-iterator.js")]
    public Task test_async_gen_yield_promise_reject_next_for_await_of_async_iterator() => ExecutionTest("async-gen-yield-promise-reject-next-for-await-of-async-iterator");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next-for-await-of-sync-iterator.js")]
    public Task test_async_gen_yield_promise_reject_next_for_await_of_sync_iterator() => ExecutionTest("async-gen-yield-promise-reject-next-for-await-of-sync-iterator");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next-yield-star-async-iterator.js")]
    public Task test_async_gen_yield_promise_reject_next_yield_star_async_iterator() => ExecutionTest("async-gen-yield-promise-reject-next-yield-star-async-iterator");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next-yield-star-sync-iterator.js")]
    public Task test_async_gen_yield_promise_reject_next_yield_star_sync_iterator() => ExecutionTest("async-gen-yield-promise-reject-next-yield-star-sync-iterator");

    [Fact(DisplayName = "async-gen-yield-promise-reject-next.js")]
    public Task test_async_gen_yield_promise_reject_next() => ExecutionTest("async-gen-yield-promise-reject-next");

    [Fact(DisplayName = "async-gen-yield-spread-arr-multiple.js")]
    public Task test_async_gen_yield_spread_arr_multiple() => ExecutionTest("async-gen-yield-spread-arr-multiple");

    [Fact(DisplayName = "async-gen-yield-spread-arr-single.js")]
    public Task test_async_gen_yield_spread_arr_single() => ExecutionTest("async-gen-yield-spread-arr-single");

    [Fact(DisplayName = "async-gen-yield-spread-obj.js")]
    public Task test_async_gen_yield_spread_obj() => ExecutionTest("async-gen-yield-spread-obj");

    [Fact(DisplayName = "async-gen-yield-star-async-next.js")]
    public Task test_async_gen_yield_star_async_next() => ExecutionTest("async-gen-yield-star-async-next");

    [Fact(DisplayName = "async-gen-yield-star-async-return.js")]
    public Task test_async_gen_yield_star_async_return() => ExecutionTest("async-gen-yield-star-async-return");

    [Fact(DisplayName = "async-gen-yield-star-async-throw.js")]
    public Task test_async_gen_yield_star_async_throw() => ExecutionTest("async-gen-yield-star-async-throw");

    [Fact(DisplayName = "async-gen-yield-star-expr-abrupt.js")]
    public Task test_async_gen_yield_star_expr_abrupt() => ExecutionTest("async-gen-yield-star-expr-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-get-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_async_get_abrupt() => ExecutionTest("async-gen-yield-star-getiter-async-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-not-callable-boolean-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_not_callable_boolean_throw() => ExecutionTest("async-gen-yield-star-getiter-async-not-callable-boolean-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-not-callable-number-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_not_callable_number_throw() => ExecutionTest("async-gen-yield-star-getiter-async-not-callable-number-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-not-callable-object-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_not_callable_object_throw() => ExecutionTest("async-gen-yield-star-getiter-async-not-callable-object-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-not-callable-string-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_not_callable_string_throw() => ExecutionTest("async-gen-yield-star-getiter-async-not-callable-string-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-not-callable-symbol-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_not_callable_symbol_throw() => ExecutionTest("async-gen-yield-star-getiter-async-not-callable-symbol-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-null-sync-get-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_async_null_sync_get_abrupt() => ExecutionTest("async-gen-yield-star-getiter-async-null-sync-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_abrupt() => ExecutionTest("async-gen-yield-star-getiter-async-returns-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-boolean-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_boolean_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-boolean-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-null-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_null_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-null-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-number-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_number_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-number-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-string-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_string_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-string-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-symbol-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_symbol_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-symbol-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-returns-undefined-throw.js")]
    public Task test_async_gen_yield_star_getiter_async_returns_undefined_throw() => ExecutionTest("async-gen-yield-star-getiter-async-returns-undefined-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-async-undefined-sync-get-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_async_undefined_sync_get_abrupt() => ExecutionTest("async-gen-yield-star-getiter-async-undefined-sync-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-get-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_sync_get_abrupt() => ExecutionTest("async-gen-yield-star-getiter-sync-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-not-callable-boolean-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_not_callable_boolean_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-not-callable-boolean-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-not-callable-number-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_not_callable_number_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-not-callable-number-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-not-callable-object-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_not_callable_object_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-not-callable-object-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-not-callable-string-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_not_callable_string_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-not-callable-string-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-not-callable-symbol-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_not_callable_symbol_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-not-callable-symbol-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-abrupt.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_abrupt() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-boolean-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_boolean_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-boolean-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-null-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_null_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-null-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-number-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_number_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-number-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-string-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_string_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-string-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-symbol-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_symbol_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-symbol-throw");

    [Fact(DisplayName = "async-gen-yield-star-getiter-sync-returns-undefined-throw.js")]
    public Task test_async_gen_yield_star_getiter_sync_returns_undefined_throw() => ExecutionTest("async-gen-yield-star-getiter-sync-returns-undefined-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-call-done-get-abrupt.js")]
    public Task test_async_gen_yield_star_next_call_done_get_abrupt() => ExecutionTest("async-gen-yield-star-next-call-done-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-next-call-returns-abrupt.js")]
    public Task test_async_gen_yield_star_next_call_returns_abrupt() => ExecutionTest("async-gen-yield-star-next-call-returns-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-next-call-value-get-abrupt.js")]
    public Task test_async_gen_yield_star_next_call_value_get_abrupt() => ExecutionTest("async-gen-yield-star-next-call-value-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-next-get-abrupt.js")]
    public Task test_async_gen_yield_star_next_get_abrupt() => ExecutionTest("async-gen-yield-star-next-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-next-non-object-ignores-then.js")]
    public Task test_async_gen_yield_star_next_non_object_ignores_then() => ExecutionTest("async-gen-yield-star-next-non-object-ignores-then");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-boolean-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_boolean_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-boolean-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-null-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_null_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-null-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-number-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_number_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-number-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-object-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_object_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-object-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-string-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_string_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-string-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-symbol-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_symbol_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-symbol-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-not-callable-undefined-throw.js")]
    public Task test_async_gen_yield_star_next_not_callable_undefined_throw() => ExecutionTest("async-gen-yield-star-next-not-callable-undefined-throw");

    [Fact(DisplayName = "async-gen-yield-star-next-then-get-abrupt.js")]
    public Task test_async_gen_yield_star_next_then_get_abrupt() => ExecutionTest("async-gen-yield-star-next-then-get-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-boolean-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_boolean_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-boolean-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-null-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_null_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-null-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-number-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_number_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-number-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-object-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_object_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-object-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-string-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_string_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-string-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-symbol-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_symbol_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-symbol-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-non-callable-undefined-fulfillpromise.js")]
    public Task test_async_gen_yield_star_next_then_non_callable_undefined_fulfillpromise() => ExecutionTest("async-gen-yield-star-next-then-non-callable-undefined-fulfillpromise");

    [Fact(DisplayName = "async-gen-yield-star-next-then-returns-abrupt.js")]
    public Task test_async_gen_yield_star_next_then_returns_abrupt() => ExecutionTest("async-gen-yield-star-next-then-returns-abrupt");

    [Fact(DisplayName = "async-gen-yield-star-sync-next.js")]
    public Task test_async_gen_yield_star_sync_next() => ExecutionTest("async-gen-yield-star-sync-next");

    [Fact(DisplayName = "async-gen-yield-star-sync-return.js")]
    public Task test_async_gen_yield_star_sync_return() => ExecutionTest("async-gen-yield-star-sync-return");

    [Fact(DisplayName = "async-gen-yield-star-sync-throw.js")]
    public Task test_async_gen_yield_star_sync_throw() => ExecutionTest("async-gen-yield-star-sync-throw");

    [Fact(DisplayName = "async-meth-dflt-params-abrupt.js")]
    public Task test_async_meth_dflt_params_abrupt() => ExecutionTest("async-meth-dflt-params-abrupt");

    [Fact(DisplayName = "async-meth-dflt-params-arg-val-not-undefined.js")]
    public Task test_async_meth_dflt_params_arg_val_not_undefined() => ExecutionTest("async-meth-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "async-meth-dflt-params-arg-val-undefined.js")]
    public Task test_async_meth_dflt_params_arg_val_undefined() => ExecutionTest("async-meth-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "async-meth-dflt-params-ref-later.js")]
    public Task test_async_meth_dflt_params_ref_later() => ExecutionTest("async-meth-dflt-params-ref-later");

    [Fact(DisplayName = "async-meth-dflt-params-ref-prior.js")]
    public Task test_async_meth_dflt_params_ref_prior() => ExecutionTest("async-meth-dflt-params-ref-prior");

    [Fact(DisplayName = "async-meth-dflt-params-ref-self.js")]
    public Task test_async_meth_dflt_params_ref_self() => ExecutionTest("async-meth-dflt-params-ref-self");

    [Fact(DisplayName = "async-meth-dflt-params-trailing-comma.js")]
    public Task test_async_meth_dflt_params_trailing_comma() => ExecutionTest("async-meth-dflt-params-trailing-comma");

    [Fact(DisplayName = "async-meth-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_async_meth_eval_var_scope_syntax_err() => ExecutionTest("async-meth-eval-var-scope-syntax-err");

    [Fact(DisplayName = "async-meth-params-trailing-comma-multiple.js")]
    public Task test_async_meth_params_trailing_comma_multiple() => ExecutionTest("async-meth-params-trailing-comma-multiple");

    [Fact(DisplayName = "async-meth-params-trailing-comma-single.js")]
    public Task test_async_meth_params_trailing_comma_single() => ExecutionTest("async-meth-params-trailing-comma-single");

    [Fact(DisplayName = "async-returns-async-arrow-returns-arguments-from-parent-function.js")]
    public Task test_async_returns_async_arrow_returns_arguments_from_parent_function() => ExecutionTest("async-returns-async-arrow-returns-arguments-from-parent-function");

    [Fact(DisplayName = "async-returns-async-arrow-returns-newtarget.js")]
    public Task test_async_returns_async_arrow_returns_newtarget() => ExecutionTest("async-returns-async-arrow-returns-newtarget");

    [Fact(DisplayName = "async-returns-async-arrow.js")]
    public Task test_async_returns_async_arrow() => ExecutionTest("async-returns-async-arrow");

    [Fact(DisplayName = "async-returns-async-function-returns-arguments-from-own-function.js")]
    public Task test_async_returns_async_function_returns_arguments_from_own_function() => ExecutionTest("async-returns-async-function-returns-arguments-from-own-function");

    [Fact(DisplayName = "async-returns-async-function-returns-newtarget.js")]
    public Task test_async_returns_async_function_returns_newtarget() => ExecutionTest("async-returns-async-function-returns-newtarget");

    [Fact(DisplayName = "async-returns-async-function.js")]
    public Task test_async_returns_async_function() => ExecutionTest("async-returns-async-function");

    [Fact(DisplayName = "async-super-call-body.js")]
    public Task test_async_super_call_body() => ExecutionTest("async-super-call-body");

    [Fact(DisplayName = "async-super-call-param.js")]
    public Task test_async_super_call_param() => ExecutionTest("async-super-call-param");

    [Fact(DisplayName = "gen-meth-dflt-params-abrupt.js")]
    public Task test_gen_meth_dflt_params_abrupt() => ExecutionTest("gen-meth-dflt-params-abrupt");

    [Fact(DisplayName = "gen-meth-dflt-params-ref-later.js")]
    public Task test_gen_meth_dflt_params_ref_later() => ExecutionTest("gen-meth-dflt-params-ref-later");

    [Fact(DisplayName = "gen-meth-dflt-params-ref-self.js")]
    public Task test_gen_meth_dflt_params_ref_self() => ExecutionTest("gen-meth-dflt-params-ref-self");

    [Fact(DisplayName = "gen-meth-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_gen_meth_eval_var_scope_syntax_err() => ExecutionTest("gen-meth-eval-var-scope-syntax-err");

    [Fact(DisplayName = "generator-prototype-prop.js")]
    public Task test_generator_prototype_prop() => ExecutionTest("generator-prototype-prop");

    [Fact(DisplayName = "meth-eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_meth_eval_var_scope_syntax_err() => ExecutionTest("meth-eval-var-scope-syntax-err");

    [Fact(DisplayName = "static-init-await-binding-accessor.js")]
    public Task test_static_init_await_binding_accessor() => ExecutionTest("static-init-await-binding-accessor");

    [Fact(DisplayName = "static-init-await-binding-generator.js")]
    public Task test_static_init_await_binding_generator() => ExecutionTest("static-init-await-binding-generator");

    [Fact(DisplayName = "static-init-await-binding-normal.js")]
    public Task test_static_init_await_binding_normal() => ExecutionTest("static-init-await-binding-normal");

    [Fact(DisplayName = "static-init-await-reference-accessor.js")]
    public Task test_static_init_await_reference_accessor() => ExecutionTest("static-init-await-reference-accessor");

    [Fact(DisplayName = "static-init-await-reference-generator.js")]
    public Task test_static_init_await_reference_generator() => ExecutionTest("static-init-await-reference-generator");

    [Fact(DisplayName = "static-init-await-reference-normal.js")]
    public Task test_static_init_await_reference_normal() => ExecutionTest("static-init-await-reference-normal");
}
