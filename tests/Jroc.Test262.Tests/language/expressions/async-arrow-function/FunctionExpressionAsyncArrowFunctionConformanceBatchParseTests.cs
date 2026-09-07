using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function;

public class FunctionExpressionAsyncArrowFunctionConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionAsyncArrowFunctionConformanceBatchParseTests() : base("language/expressions/async-arrow-function", "language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "early-errors-arrow-NSPL-with-USD.js")]
    public Task early_errors_arrow_NSPL_with_USD()
        => CompilationFailureTest("early-errors-arrow-NSPL-with-USD", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-arguments-in-formal-parameters.js")]
    public Task early_errors_arrow_arguments_in_formal_parameters()
        => CompilationFailureTest("early-errors-arrow-arguments-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-await-in-formals.js")]
    public Task early_errors_arrow_await_in_formals()
        => CompilationFailureTest("early-errors-arrow-await-in-formals", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-body-contains-super-property.js")]
    public Task early_errors_arrow_body_contains_super_property()
        => CompilationFailureTest("early-errors-arrow-body-contains-super-property", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-eval-in-formal-parameters.js")]
    public Task early_errors_arrow_eval_in_formal_parameters()
        => CompilationFailureTest("early-errors-arrow-eval-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-formals-body-duplicate.js")]
    public Task early_errors_arrow_formals_body_duplicate()
        => CompilationFailureTest("early-errors-arrow-formals-body-duplicate", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-formals-contains-super-call.js")]
    public Task early_errors_arrow_formals_contains_super_call()
        => CompilationFailureTest("early-errors-arrow-formals-contains-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-formals-contains-super-property.js")]
    public Task early_errors_arrow_formals_contains_super_property()
        => CompilationFailureTest("early-errors-arrow-formals-contains-super-property", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-arrow-formals-lineterminator.js")]
    public Task early_errors_arrow_formals_lineterminator()
        => CompilationFailureTest("early-errors-arrow-formals-lineterminator", "Failed to parse JavaScript");

    [Fact(DisplayName = "escaped-async.js")]
    public Task escaped_async()
        => CompilationFailureTest("escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task object_destructuring_param_strict_body()
        => CompilationFailureTest("object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task rest_param_strict_body()
        => CompilationFailureTest("rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task rest_params_trailing_comma_early_error()
        => CompilationFailureTest("rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

}
