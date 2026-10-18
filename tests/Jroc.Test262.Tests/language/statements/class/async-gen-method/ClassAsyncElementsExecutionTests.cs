namespace Jroc.Test262.Tests.language.statements.test_class.async_gen_method;

public sealed class ClassAsyncElementsExecutionTests : DiskExecutionTestsBase
{
    public ClassAsyncElementsExecutionTests() : base("language.statements.test_class.async_gen_method") { }

    [Fact(DisplayName = "yield-star-next-not-callable-null-throw.js")]
    public Task yield_star_next_not_callable_null_throw()
        => ExecutionTest("yield-star-next-not-callable-null-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-number-throw.js")]
    public Task yield_star_next_not_callable_number_throw()
        => ExecutionTest("yield-star-next-not-callable-number-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-object-throw.js")]
    public Task yield_star_next_not_callable_object_throw()
        => ExecutionTest("yield-star-next-not-callable-object-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-string-throw.js")]
    public Task yield_star_next_not_callable_string_throw()
        => ExecutionTest("yield-star-next-not-callable-string-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-symbol-throw.js")]
    public Task yield_star_next_not_callable_symbol_throw()
        => ExecutionTest("yield-star-next-not-callable-symbol-throw");

    [Fact(DisplayName = "yield-star-next-not-callable-undefined-throw.js")]
    public Task yield_star_next_not_callable_undefined_throw()
        => ExecutionTest("yield-star-next-not-callable-undefined-throw");

    [Fact(DisplayName = "yield-star-next-then-get-abrupt.js")]
    public Task yield_star_next_then_get_abrupt()
        => ExecutionTest("yield-star-next-then-get-abrupt");

    [Fact(DisplayName = "yield-star-next-then-non-callable-boolean-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_boolean_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-boolean-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-null-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_null_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-null-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-number-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_number_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-number-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-object-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_object_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-object-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-string-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_string_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-string-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-symbol-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_symbol_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-symbol-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-non-callable-undefined-fulfillpromise.js")]
    public Task yield_star_next_then_non_callable_undefined_fulfillpromise()
        => ExecutionTest("yield-star-next-then-non-callable-undefined-fulfillpromise");

    [Fact(DisplayName = "yield-star-next-then-returns-abrupt.js")]
    public Task yield_star_next_then_returns_abrupt()
        => ExecutionTest("yield-star-next-then-returns-abrupt");

    [Fact(DisplayName = "yield-star-sync-next.js")]
    public Task yield_star_sync_next()
        => ExecutionTest("yield-star-sync-next");

    [Fact(DisplayName = "yield-star-sync-return.js")]
    public Task yield_star_sync_return()
        => ExecutionTest("yield-star-sync-return");

    [Fact(DisplayName = "yield-star-sync-throw.js")]
    public Task yield_star_sync_throw()
        => ExecutionTest("yield-star-sync-throw");
}
