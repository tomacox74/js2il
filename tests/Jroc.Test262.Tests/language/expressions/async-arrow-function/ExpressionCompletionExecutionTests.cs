using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task test_dflt_params_abrupt() => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task test_dflt_params_ref_later() => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task test_dflt_params_ref_self() => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task test_dflt_params_trailing_comma() => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "eval-var-scope-syntax-err.js", Skip = "eval is not supported.")]
    public Task test_eval_var_scope_syntax_err() => ExecutionTest("eval-var-scope-syntax-err");

    [Fact(DisplayName = "prototype.js")]
    public Task test_prototype() => ExecutionTest("prototype");

    [Fact(DisplayName = "try-throw-finally-throw.js")]
    public Task test_try_throw_finally_throw() => ExecutionTest("try-throw-finally-throw");

    [Fact(DisplayName = "unscopables-with-in-nested-fn.js")]
    public Task test_unscopables_with_in_nested_fn() => ExecutionTest("unscopables-with-in-nested-fn");

    [Fact(DisplayName = "unscopables-with.js")]
    public Task test_unscopables_with() => ExecutionTest("unscopables-with");
}
