using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.function;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "language/expressions/function/name-eval-strict-body.js")]
    public Task test_name_eval_strict_body()
        => CompilationFailureTest("name-eval-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/name-eval-strict.js")]
    public Task test_name_eval_strict()
        => CompilationFailureTest("name-eval-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-1.js")]
    public Task test_param_duplicated_strict_1()
        => CompilationFailureTest("param-duplicated-strict-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-2.js")]
    public Task test_param_duplicated_strict_2()
        => CompilationFailureTest("param-duplicated-strict-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-3.js")]
    public Task test_param_duplicated_strict_3()
        => CompilationFailureTest("param-duplicated-strict-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-body-1.js")]
    public Task test_param_duplicated_strict_body_1()
        => CompilationFailureTest("param-duplicated-strict-body-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-body-2.js")]
    public Task test_param_duplicated_strict_body_2()
        => CompilationFailureTest("param-duplicated-strict-body-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-duplicated-strict-body-3.js")]
    public Task test_param_duplicated_strict_body_3()
        => CompilationFailureTest("param-duplicated-strict-body-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/param-eval-strict-body.js")]
    public Task test_param_eval_strict_body()
        => CompilationFailureTest("param-eval-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/rest-param-strict-body.js")]
    public Task test_rest_param_strict_body()
        => CompilationFailureTest("rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/rest-params-trailing-comma-early-error.js")]
    public Task test_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "language/expressions/function/use-strict-with-non-simple-param.js")]
    public Task test_use_strict_with_non_simple_param()
        => CompilationFailureTest("use-strict-with-non-simple-param", "Failed to parse JavaScript");
}
