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

[Fact(DisplayName = "S10.1.1_A1_T2")]
    public Task S10_1_1_A1_T2()
        => ExecutionTest("S10.1.1_A1_T2");

[Fact(DisplayName = "length-dflt", Skip = "Function length descriptors with default parameters are incorrect.")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

[Fact(DisplayName = "name-arguments-non-strict")]
    public Task name_arguments_non_strict()
        => ExecutionTest("name-arguments-non-strict");

[Fact(DisplayName = "name-eval-non-strict")]
    public Task name_eval_non_strict()
        => ExecutionTest("name-eval-non-strict");

[Fact(DisplayName = "name-eval-stricteval", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task name_eval_stricteval()
        => ExecutionTest("name-eval-stricteval");

[Fact(DisplayName = "name", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task name()
        => ExecutionTest("name");

[Fact(DisplayName = "param-arguments-non-strict")]
    public Task param_arguments_non_strict()
        => ExecutionTest("param-arguments-non-strict");

[Fact(DisplayName = "param-duplicated-non-strict")]
    public Task param_duplicated_non_strict()
        => ExecutionTest("param-duplicated-non-strict");
}
