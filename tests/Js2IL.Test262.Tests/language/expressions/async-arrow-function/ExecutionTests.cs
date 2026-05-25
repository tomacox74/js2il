using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.async_arrow_function;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "arrow-returns-promise")]
    public Task arrow_returns_promise()
        => ExecutionTest("arrow-returns-promise");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "try-return-finally-throw")]
    public Task try_return_finally_throw()
        => ExecutionTest("try-return-finally-throw");

    [Fact(DisplayName = "try-throw-finally-return")]
    public Task try_throw_finally_return()
        => ExecutionTest("try-throw-finally-return");

}
