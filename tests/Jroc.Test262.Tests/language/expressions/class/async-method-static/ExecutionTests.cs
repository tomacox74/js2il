using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.async_method_static;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.async-method-static") { }

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined.js")]
    public Task ported_dflt_params_arg_val_not_undefined() => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined.js")]
    public Task ported_dflt_params_arg_val_undefined() => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task ported_dflt_params_ref_later() => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-prior.js")]
    public Task ported_dflt_params_ref_prior() => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task ported_dflt_params_ref_self() => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task ported_dflt_params_trailing_comma() => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "params-trailing-comma-multiple.js")]
    public Task ported_params_trailing_comma_multiple() => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single.js")]
    public Task ported_params_trailing_comma_single() => ExecutionTest("params-trailing-comma-single");

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
