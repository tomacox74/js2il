using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.async_function;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.async_function") { }

    [Fact(DisplayName = "declaration-returns-promise")]
    public Task declaration_returns_promise()
        => ExecutionTest("declaration-returns-promise");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "evaluation-body-that-returns")]
    public Task evaluation_body_that_returns()
        => ExecutionTest("evaluation-body-that-returns");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "returns-async-arrow-returns-arguments-from-parent-function")]
    public Task returns_async_arrow_returns_arguments_from_parent_function()
        => ExecutionTest("returns-async-arrow-returns-arguments-from-parent-function");

    [Fact(DisplayName = "returns-async-arrow")]
    public Task returns_async_arrow()
        => ExecutionTest("returns-async-arrow");

    [Fact(DisplayName = "returns-async-function")]
    public Task returns_async_function()
        => ExecutionTest("returns-async-function");

    [Fact(DisplayName = "try-reject-finally-return")]
    public Task try_reject_finally_return()
        => ExecutionTest("try-reject-finally-return");

    [Fact(DisplayName = "try-reject-finally-throw")]
    public Task try_reject_finally_throw()
        => ExecutionTest("try-reject-finally-throw");

    [Fact(DisplayName = "try-return-finally-reject")]
    public Task try_return_finally_reject()
        => ExecutionTest("try-return-finally-reject");

    [Fact(DisplayName = "try-return-finally-return")]
    public Task try_return_finally_return()
        => ExecutionTest("try-return-finally-return");

    [Fact(DisplayName = "try-return-finally-throw")]
    public Task try_return_finally_throw()
        => ExecutionTest("try-return-finally-throw");

    [Fact(DisplayName = "try-throw-finally-reject")]
    public Task try_throw_finally_reject()
        => ExecutionTest("try-throw-finally-reject");

    [Fact(DisplayName = "try-throw-finally-return")]
    public Task try_throw_finally_return()
        => ExecutionTest("try-throw-finally-return");

    [Fact(DisplayName = "evaluation-body-that-returns-after-await")]
    public Task evaluation_body_that_returns_after_await()
        => ExecutionTest("evaluation-body-that-returns-after-await");
}
