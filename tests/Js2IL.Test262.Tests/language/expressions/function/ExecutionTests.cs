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

    [Fact(DisplayName = "length-dflt")]
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

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "param-arguments-non-strict")]
    public Task param_arguments_non_strict()
        => ExecutionTest("param-arguments-non-strict");

    [Fact(DisplayName = "param-duplicated-non-strict")]
    public Task param_duplicated_non_strict()
        => ExecutionTest("param-duplicated-non-strict");

    [Fact(DisplayName = "param-eval-non-strict")]
    public Task param_eval_non_strict()
        => ExecutionTest("param-eval-non-strict");

    [Fact(DisplayName = "param-eval-stricteval", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task param_eval_stricteval()
        => ExecutionTest("param-eval-stricteval");

    [Fact(DisplayName = "params-dflt-args-unmapped")]
    public Task params_dflt_args_unmapped()
        => ExecutionTest("params-dflt-args-unmapped");

    [Fact(DisplayName = "params-dflt-ref-arguments")]
    public Task params_dflt_ref_arguments()
        => ExecutionTest("params-dflt-ref-arguments");

    [Fact(DisplayName = "scope-body-lex-distinct", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task scope_body_lex_distinct()
        => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "scope-name-var-close", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task scope_name_var_close()
        => ExecutionTest("scope-name-var-close");

    [Fact(DisplayName = "scope-name-var-open-non-strict", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task scope_name_var_open_non_strict()
        => ExecutionTest("scope-name-var-open-non-strict");

    [Fact(DisplayName = "scope-name-var-open-strict", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task scope_name_var_open_strict()
        => ExecutionTest("scope-name-var-open-strict");
}
