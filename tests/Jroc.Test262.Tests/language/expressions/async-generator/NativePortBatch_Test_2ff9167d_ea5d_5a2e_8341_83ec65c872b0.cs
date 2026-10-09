using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_generator;

public sealed class NativePortBatch_Test_2ff9167d_ea5d_5a2e_8341_83ec65c872b0 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_2ff9167d_ea5d_5a2e_8341_83ec65c872b0() : base("Jroc.Test262.Tests.language.expressions.async_generator") { }

    [Fact(DisplayName = "named-yield-identifier-non-strict")]
    public Task named_yield_identifier_non_strict() => ExecutionTestFromFile("named-yield-identifier-non-strict");

    [Fact(DisplayName = "named-yield-identifier-spread-non-strict")]
    public Task named_yield_identifier_spread_non_strict() => ExecutionTestFromFile("named-yield-identifier-spread-non-strict");

    [Fact(DisplayName = "named-yield-promise-reject-next")]
    public Task named_yield_promise_reject_next() => ExecutionTestFromFile("named-yield-promise-reject-next");

    [Fact(DisplayName = "named-yield-promise-reject-next-catch")]
    public Task named_yield_promise_reject_next_catch() => ExecutionTestFromFile("named-yield-promise-reject-next-catch");

    [Fact(DisplayName = "named-yield-promise-reject-next-for-await-of-async-iterator")]
    public Task named_yield_promise_reject_next_for_await_of_async_iterator() => ExecutionTestFromFile("named-yield-promise-reject-next-for-await-of-async-iterator");

    [Fact(DisplayName = "named-yield-promise-reject-next-for-await-of-sync-iterator")]
    public Task named_yield_promise_reject_next_for_await_of_sync_iterator() => ExecutionTestFromFile("named-yield-promise-reject-next-for-await-of-sync-iterator");

    [Fact(DisplayName = "named-yield-promise-reject-next-yield-star-async-iterator")]
    public Task named_yield_promise_reject_next_yield_star_async_iterator() => ExecutionTestFromFile("named-yield-promise-reject-next-yield-star-async-iterator");

    [Fact(DisplayName = "named-yield-promise-reject-next-yield-star-sync-iterator")]
    public Task named_yield_promise_reject_next_yield_star_sync_iterator() => ExecutionTestFromFile("named-yield-promise-reject-next-yield-star-sync-iterator");

    [Fact(DisplayName = "named-yield-spread-arr-multiple")]
    public Task named_yield_spread_arr_multiple() => ExecutionTestFromFile("named-yield-spread-arr-multiple");

    [Fact(DisplayName = "named-yield-spread-arr-single")]
    public Task named_yield_spread_arr_single() => ExecutionTestFromFile("named-yield-spread-arr-single");

    [Fact(DisplayName = "named-yield-spread-obj")]
    public Task named_yield_spread_obj() => ExecutionTestFromFile("named-yield-spread-obj");

    [Fact(DisplayName = "named-yield-star-async-next")]
    public Task named_yield_star_async_next() => ExecutionTestFromFile("named-yield-star-async-next");

    [Fact(DisplayName = "named-yield-star-async-return")]
    public Task named_yield_star_async_return() => ExecutionTestFromFile("named-yield-star-async-return");

    [Fact(DisplayName = "named-yield-star-async-throw")]
    public Task named_yield_star_async_throw() => ExecutionTestFromFile("named-yield-star-async-throw");

    [Fact(DisplayName = "named-yield-star-expr-abrupt")]
    public Task named_yield_star_expr_abrupt() => ExecutionTestFromFile("named-yield-star-expr-abrupt");

    [Fact(DisplayName = "named-yield-star-getiter-async-get-abrupt")]
    public Task named_yield_star_getiter_async_get_abrupt() => ExecutionTestFromFile("named-yield-star-getiter-async-get-abrupt");

    [Fact(DisplayName = "named-yield-star-getiter-async-not-callable-boolean-throw")]
    public Task named_yield_star_getiter_async_not_callable_boolean_throw() => ExecutionTestFromFile("named-yield-star-getiter-async-not-callable-boolean-throw");

    [Fact(DisplayName = "named-yield-star-getiter-async-not-callable-number-throw")]
    public Task named_yield_star_getiter_async_not_callable_number_throw() => ExecutionTestFromFile("named-yield-star-getiter-async-not-callable-number-throw");

    [Fact(DisplayName = "named-yield-star-getiter-async-not-callable-object-throw")]
    public Task named_yield_star_getiter_async_not_callable_object_throw() => ExecutionTestFromFile("named-yield-star-getiter-async-not-callable-object-throw");

    [Fact(DisplayName = "named-yield-star-getiter-async-not-callable-string-throw")]
    public Task named_yield_star_getiter_async_not_callable_string_throw() => ExecutionTestFromFile("named-yield-star-getiter-async-not-callable-string-throw");

}
