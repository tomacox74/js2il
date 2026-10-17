namespace Jroc.Test262.Tests.language.expressions.call;

public sealed class ExpressionsRemainderExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsRemainderExecutionTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "tco-non-eval-function-dynamic.js", Skip = "eval is not supported.")]
    public Task tco_non_eval_function_dynamic()
        => ExecutionTest("tco-non-eval-function-dynamic");

    [Fact(DisplayName = "tco-non-eval-function.js", Skip = "Calls through a binding named eval are not supported.")]
    public Task tco_non_eval_function()
        => ExecutionTest("tco-non-eval-function");

    [Fact(DisplayName = "tco-non-eval-global.js", Skip = "Calls through a binding named eval are not supported.")]
    public Task tco_non_eval_global()
        => ExecutionTest("tco-non-eval-global");

    [Fact(DisplayName = "tco-non-eval-with.js", Skip = "Calls through a binding named eval are not supported.")]
    public Task tco_non_eval_with()
        => ExecutionTest("tco-non-eval-with");
}
