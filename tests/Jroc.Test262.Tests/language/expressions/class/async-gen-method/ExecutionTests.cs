using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.async_gen_method;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.async-gen-method") { }

    [Fact(DisplayName = "dflt-params-arg-val-undefined.js")]
    public Task ported_dflt_params_arg_val_undefined() => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior.js")]
    public Task ported_dflt_params_ref_prior() => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task ported_dflt_params_trailing_comma() => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "params-trailing-comma-multiple.js")]
    public Task ported_params_trailing_comma_multiple() => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single.js")]
    public Task ported_params_trailing_comma_single() => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "yield-promise-reject-next-catch.js")]
    public Task ported_yield_promise_reject_next_catch() => ExecutionTest("yield-promise-reject-next-catch");

    [Fact(DisplayName = "yield-promise-reject-next-for-await-of-async-iterator.js")]
    public Task ported_yield_promise_reject_next_for_await_of_async_iterator() => ExecutionTest("yield-promise-reject-next-for-await-of-async-iterator");

    [Fact(DisplayName = "yield-promise-reject-next-for-await-of-sync-iterator.js")]
    public Task ported_yield_promise_reject_next_for_await_of_sync_iterator() => ExecutionTest("yield-promise-reject-next-for-await-of-sync-iterator");

    [Fact(DisplayName = "yield-promise-reject-next-yield-star-async-iterator.js")]
    public Task ported_yield_promise_reject_next_yield_star_async_iterator() => ExecutionTest("yield-promise-reject-next-yield-star-async-iterator");

    [Fact(DisplayName = "yield-promise-reject-next-yield-star-sync-iterator.js")]
    public Task ported_yield_promise_reject_next_yield_star_sync_iterator() => ExecutionTest("yield-promise-reject-next-yield-star-sync-iterator");

    [Fact(DisplayName = "yield-promise-reject-next.js")]
    public Task ported_yield_promise_reject_next() => ExecutionTest("yield-promise-reject-next");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task ported_yield_spread_arr_multiple() => ExecutionTest("yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task ported_yield_spread_arr_single() => ExecutionTest("yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task ported_yield_spread_obj() => ExecutionTest("yield-spread-obj");

    [Fact(DisplayName = "yield-star-async-next.js")]
    public Task ported_yield_star_async_next() => ExecutionTest("yield-star-async-next");

    [Fact(DisplayName = "yield-star-async-return.js")]
    public Task ported_yield_star_async_return() => ExecutionTest("yield-star-async-return");

    [Fact(DisplayName = "yield-star-async-throw.js")]
    public Task ported_yield_star_async_throw() => ExecutionTest("yield-star-async-throw");

    [Fact(DisplayName = "yield-star-expr-abrupt.js")]
    public Task ported_yield_star_expr_abrupt() => ExecutionTest("yield-star-expr-abrupt");

    [Fact(DisplayName = "yield-star-getiter-async-get-abrupt.js")]
    public Task ported_yield_star_getiter_async_get_abrupt() => ExecutionTest("yield-star-getiter-async-get-abrupt");

    [Fact(DisplayName = "yield-star-getiter-async-not-callable-boolean-throw.js")]
    public Task ported_yield_star_getiter_async_not_callable_boolean_throw() => ExecutionTest("yield-star-getiter-async-not-callable-boolean-throw");

    [Fact(DisplayName = "yield-star-getiter-async-not-callable-number-throw.js")]
    public Task ported_yield_star_getiter_async_not_callable_number_throw() => ExecutionTest("yield-star-getiter-async-not-callable-number-throw");

    [Fact(DisplayName = "yield-star-getiter-async-not-callable-object-throw.js")]
    public Task ported_yield_star_getiter_async_not_callable_object_throw() => ExecutionTest("yield-star-getiter-async-not-callable-object-throw");

    [Fact(DisplayName = "yield-star-getiter-async-not-callable-string-throw.js")]
    public Task ported_yield_star_getiter_async_not_callable_string_throw() => ExecutionTest("yield-star-getiter-async-not-callable-string-throw");

    [Fact(DisplayName = "yield-star-getiter-async-not-callable-symbol-throw.js")]
    public Task ported_yield_star_getiter_async_not_callable_symbol_throw() => ExecutionTest("yield-star-getiter-async-not-callable-symbol-throw");

    [Fact(DisplayName = "yield-star-getiter-async-null-sync-get-abrupt.js")]
    public Task ported_yield_star_getiter_async_null_sync_get_abrupt() => ExecutionTest("yield-star-getiter-async-null-sync-get-abrupt");

    [Fact(DisplayName = "yield-star-getiter-async-returns-abrupt.js")]
    public Task ported_yield_star_getiter_async_returns_abrupt() => ExecutionTest("yield-star-getiter-async-returns-abrupt");

    [Fact(DisplayName = "yield-star-getiter-async-returns-boolean-throw.js")]
    public Task ported_yield_star_getiter_async_returns_boolean_throw() => ExecutionTest("yield-star-getiter-async-returns-boolean-throw");

    [Fact(DisplayName = "yield-star-getiter-async-returns-null-throw.js")]
    public Task ported_yield_star_getiter_async_returns_null_throw() => ExecutionTest("yield-star-getiter-async-returns-null-throw");

    [Fact(DisplayName = "yield-star-getiter-async-returns-number-throw.js")]
    public Task ported_yield_star_getiter_async_returns_number_throw() => ExecutionTest("yield-star-getiter-async-returns-number-throw");

    [Fact(DisplayName = "yield-star-getiter-async-returns-string-throw.js")]
    public Task ported_yield_star_getiter_async_returns_string_throw() => ExecutionTest("yield-star-getiter-async-returns-string-throw");

    [Fact(DisplayName = "yield-star-getiter-async-returns-symbol-throw.js")]
    public Task ported_yield_star_getiter_async_returns_symbol_throw() => ExecutionTest("yield-star-getiter-async-returns-symbol-throw");

    [Fact(DisplayName = "yield-star-getiter-async-returns-undefined-throw.js")]
    public Task ported_yield_star_getiter_async_returns_undefined_throw() => ExecutionTest("yield-star-getiter-async-returns-undefined-throw");

    [Fact(DisplayName = "yield-star-getiter-async-undefined-sync-get-abrupt.js")]
    public Task ported_yield_star_getiter_async_undefined_sync_get_abrupt() => ExecutionTest("yield-star-getiter-async-undefined-sync-get-abrupt");

    [Fact(DisplayName = "yield-star-getiter-sync-get-abrupt.js")]
    public Task ported_yield_star_getiter_sync_get_abrupt() => ExecutionTest("yield-star-getiter-sync-get-abrupt");

    [Fact(DisplayName = "yield-star-getiter-sync-not-callable-boolean-throw.js")]
    public Task ported_yield_star_getiter_sync_not_callable_boolean_throw() => ExecutionTest("yield-star-getiter-sync-not-callable-boolean-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-not-callable-number-throw.js")]
    public Task ported_yield_star_getiter_sync_not_callable_number_throw() => ExecutionTest("yield-star-getiter-sync-not-callable-number-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-not-callable-object-throw.js")]
    public Task ported_yield_star_getiter_sync_not_callable_object_throw() => ExecutionTest("yield-star-getiter-sync-not-callable-object-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-not-callable-string-throw.js")]
    public Task ported_yield_star_getiter_sync_not_callable_string_throw() => ExecutionTest("yield-star-getiter-sync-not-callable-string-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-not-callable-symbol-throw.js")]
    public Task ported_yield_star_getiter_sync_not_callable_symbol_throw() => ExecutionTest("yield-star-getiter-sync-not-callable-symbol-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-abrupt.js")]
    public Task ported_yield_star_getiter_sync_returns_abrupt() => ExecutionTest("yield-star-getiter-sync-returns-abrupt");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-boolean-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_boolean_throw() => ExecutionTest("yield-star-getiter-sync-returns-boolean-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-null-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_null_throw() => ExecutionTest("yield-star-getiter-sync-returns-null-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-number-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_number_throw() => ExecutionTest("yield-star-getiter-sync-returns-number-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-string-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_string_throw() => ExecutionTest("yield-star-getiter-sync-returns-string-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-symbol-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_symbol_throw() => ExecutionTest("yield-star-getiter-sync-returns-symbol-throw");

    [Fact(DisplayName = "yield-star-getiter-sync-returns-undefined-throw.js")]
    public Task ported_yield_star_getiter_sync_returns_undefined_throw() => ExecutionTest("yield-star-getiter-sync-returns-undefined-throw");

    [Fact(DisplayName = "yield-star-next-call-done-get-abrupt.js")]
    public Task ported_yield_star_next_call_done_get_abrupt() => ExecutionTest("yield-star-next-call-done-get-abrupt");

    [Fact(DisplayName = "yield-star-next-call-returns-abrupt.js")]
    public Task ported_yield_star_next_call_returns_abrupt() => ExecutionTest("yield-star-next-call-returns-abrupt");

    [Fact(DisplayName = "yield-star-next-call-value-get-abrupt.js")]
    public Task ported_yield_star_next_call_value_get_abrupt() => ExecutionTest("yield-star-next-call-value-get-abrupt");

    [Fact(DisplayName = "yield-star-next-get-abrupt.js")]
    public Task ported_yield_star_next_get_abrupt() => ExecutionTest("yield-star-next-get-abrupt");

    [Fact(DisplayName = "yield-star-next-non-object-ignores-then.js")]
    public Task ported_yield_star_next_non_object_ignores_then() => ExecutionTest("yield-star-next-non-object-ignores-then");

    [Fact(DisplayName = "yield-star-next-not-callable-boolean-throw.js")]
    public Task ported_yield_star_next_not_callable_boolean_throw() => ExecutionTest("yield-star-next-not-callable-boolean-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-null-throw.js")]
    public Task ported_yield_star_next_not_callable_null_throw() => ExecutionTest("yield-star-next-not-callable-null-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-number-throw.js")]
    public Task ported_yield_star_next_not_callable_number_throw() => ExecutionTest("yield-star-next-not-callable-number-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-object-throw.js")]
    public Task ported_yield_star_next_not_callable_object_throw() => ExecutionTest("yield-star-next-not-callable-object-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-string-throw.js")]
    public Task ported_yield_star_next_not_callable_string_throw() => ExecutionTest("yield-star-next-not-callable-string-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-symbol-throw.js")]
    public Task ported_yield_star_next_not_callable_symbol_throw() => ExecutionTest("yield-star-next-not-callable-symbol-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-undefined-throw.js")]
    public Task ported_yield_star_next_not_callable_undefined_throw() => ExecutionTest("yield-star-next-not-callable-undefined-throw");

    [Fact(DisplayName = "yield-star-next-then-get-abrupt.js")]
    public Task ported_yield_star_next_then_get_abrupt() => ExecutionTest("yield-star-next-then-get-abrupt");

    [Fact(DisplayName = "yield-star-next-then-non-callable-boolean-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_boolean_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-boolean-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-null-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_null_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-null-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-number-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_number_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-number-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-object-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_object_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-object-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-string-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_string_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-string-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-symbol-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_symbol_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-symbol-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-undefined-fulfillpromise.js")]
    public Task ported_yield_star_next_then_non_callable_undefined_fulfillpromise() => ExecutionTest("yield-star-next-then-non-callable-undefined-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-returns-abrupt.js")]
    public Task ported_yield_star_next_then_returns_abrupt() => ExecutionTest("yield-star-next-then-returns-abrupt");

    [Fact(DisplayName = "yield-star-sync-next.js")]
    public Task ported_yield_star_sync_next() => ExecutionTest("yield-star-sync-next");

    [Fact(DisplayName = "yield-star-sync-return.js")]
    public Task ported_yield_star_sync_return() => ExecutionTest("yield-star-sync-return");

    [Fact(DisplayName = "yield-star-sync-throw.js")]
    public Task ported_yield_star_sync_throw() => ExecutionTest("yield-star-sync-throw");

}
