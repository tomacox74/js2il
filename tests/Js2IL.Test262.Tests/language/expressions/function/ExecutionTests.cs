using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.function;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "arguments-with-arguments-fn")]
    public Task arguments_with_arguments_fn()
        => ExecutionTest("arguments-with-arguments-fn");

    [Fact(DisplayName = "arguments-with-arguments-lex")]
    public Task arguments_with_arguments_lex()
        => ExecutionTest("arguments-with-arguments-lex");

    [Fact(DisplayName = "param-dflt-yield-non-strict")]
    public Task param_dflt_yield_non_strict()
        => ExecutionTest("param-dflt-yield-non-strict");

    [Fact(DisplayName = "param-eval-non-strict-is-correct-value")]
    public Task param_eval_non_strict_is_correct_value()
        => ExecutionTest("param-eval-non-strict-is-correct-value");
}
