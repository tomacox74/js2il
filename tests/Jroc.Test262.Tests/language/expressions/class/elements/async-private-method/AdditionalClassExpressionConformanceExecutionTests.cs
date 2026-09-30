using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.elements.async_private_method;

public class AdditionalClassExpressionConformanceExecutionTests : ExecutionTestsBase
{
    public AdditionalClassExpressionConformanceExecutionTests() : base("language.expressions.class_.elements.async-private-method") { }

    [Fact(DisplayName = "returns-async-arrow-returns-arguments-from-parent-function.js")]
    public Task ported_returns_async_arrow_returns_arguments_from_parent_function() => ExecutionTest("returns-async-arrow-returns-arguments-from-parent-function");

    [Fact(DisplayName = "returns-async-arrow-returns-newtarget.js")]
    public Task ported_returns_async_arrow_returns_newtarget() => ExecutionTest("returns-async-arrow-returns-newtarget");

    [Fact(DisplayName = "returns-async-arrow.js")]
    public Task ported_returns_async_arrow() => ExecutionTest("returns-async-arrow");

    [Fact(DisplayName = "returns-async-function-returns-arguments-from-own-function.js")]
    public Task ported_returns_async_function_returns_arguments_from_own_function() => ExecutionTest("returns-async-function-returns-arguments-from-own-function");

    [Fact(DisplayName = "returns-async-function-returns-newtarget.js")]
    public Task ported_returns_async_function_returns_newtarget() => ExecutionTest("returns-async-function-returns-newtarget");

    [Fact(DisplayName = "returns-async-function.js")]
    public Task ported_returns_async_function() => ExecutionTest("returns-async-function");

}
