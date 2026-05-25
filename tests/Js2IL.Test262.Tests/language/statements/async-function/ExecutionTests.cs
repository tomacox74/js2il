using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.async_function;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.async_function") { }

    [Fact(DisplayName = "declaration-returns-promise")]
    public Task declaration_returns_promise()
        => ExecutionTest("declaration-returns-promise");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

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

    [Fact(DisplayName = "try-throw-finally-reject")]
    public Task try_throw_finally_reject()
        => ExecutionTest("try-throw-finally-reject");

}
