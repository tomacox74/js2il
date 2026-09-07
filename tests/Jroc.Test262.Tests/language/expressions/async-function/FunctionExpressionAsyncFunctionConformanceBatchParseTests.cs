using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.async_function;

public class FunctionExpressionAsyncFunctionConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionAsyncFunctionConformanceBatchParseTests() : base("language/expressions/async-function", "language.expressions.async_function") { }

    [Fact(DisplayName = "await-as-binding-identifier-escaped.js")]
    public Task await_as_binding_identifier_escaped()
        => CompilationFailureTest("await-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier.js")]
    public Task await_as_binding_identifier()
        => CompilationFailureTest("await-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference-escaped.js")]
    public Task await_as_identifier_reference_escaped()
        => CompilationFailureTest("await-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference.js")]
    public Task await_as_identifier_reference()
        => CompilationFailureTest("await-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier-escaped.js")]
    public Task await_as_label_identifier_escaped()
        => CompilationFailureTest("await-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier.js")]
    public Task await_as_label_identifier()
        => CompilationFailureTest("await-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-NSPL-with-USD.js")]
    public Task early_errors_expression_NSPL_with_USD()
        => CompilationFailureTest("early-errors-expression-NSPL-with-USD", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-binding-identifier-arguments.js")]
    public Task early_errors_expression_binding_identifier_arguments()
        => CompilationFailureTest("early-errors-expression-binding-identifier-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-body-contains-super-call.js")]
    public Task early_errors_expression_body_contains_super_call()
        => CompilationFailureTest("early-errors-expression-body-contains-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-body-contains-super-property.js")]
    public Task early_errors_expression_body_contains_super_property()
        => CompilationFailureTest("early-errors-expression-body-contains-super-property", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-eval-in-formal-parameters.js")]
    public Task early_errors_expression_eval_in_formal_parameters()
        => CompilationFailureTest("early-errors-expression-eval-in-formal-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-formals-body-duplicate.js")]
    public Task early_errors_expression_formals_body_duplicate()
        => CompilationFailureTest("early-errors-expression-formals-body-duplicate", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-formals-contains-super-call.js")]
    public Task early_errors_expression_formals_contains_super_call()
        => CompilationFailureTest("early-errors-expression-formals-contains-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-formals-contains-super-property.js")]
    public Task early_errors_expression_formals_contains_super_property()
        => CompilationFailureTest("early-errors-expression-formals-contains-super-property", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-expression-not-simple-assignment-target.js")]
    public Task early_errors_expression_not_simple_assignment_target()
        => CompilationFailureTest("early-errors-expression-not-simple-assignment-target", "Failed to parse JavaScript");

    [Fact(DisplayName = "escaped-async.js")]
    public Task escaped_async()
        => CompilationFailureTest("escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-array-destructuring-param-strict-body.js")]
    public Task named_array_destructuring_param_strict_body()
        => CompilationFailureTest("named-array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-binding-identifier-escaped.js")]
    public Task named_await_as_binding_identifier_escaped()
        => CompilationFailureTest("named-await-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-binding-identifier.js")]
    public Task named_await_as_binding_identifier()
        => CompilationFailureTest("named-await-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-identifier-reference-escaped.js")]
    public Task named_await_as_identifier_reference_escaped()
        => CompilationFailureTest("named-await-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-identifier-reference.js")]
    public Task named_await_as_identifier_reference()
        => CompilationFailureTest("named-await-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-label-identifier-escaped.js")]
    public Task named_await_as_label_identifier_escaped()
        => CompilationFailureTest("named-await-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-await-as-label-identifier.js")]
    public Task named_await_as_label_identifier()
        => CompilationFailureTest("named-await-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-dflt-params-duplicates.js")]
    public Task named_dflt_params_duplicates()
        => CompilationFailureTest("named-dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-dflt-params-rest.js")]
    public Task named_dflt_params_rest()
        => CompilationFailureTest("named-dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-object-destructuring-param-strict-body.js")]
    public Task named_object_destructuring_param_strict_body()
        => CompilationFailureTest("named-object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-rest-param-strict-body.js")]
    public Task named_rest_param_strict_body()
        => CompilationFailureTest("named-rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "named-rest-params-trailing-comma-early-error.js")]
    public Task named_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("named-rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-array-destructuring-param-strict-body.js")]
    public Task nameless_array_destructuring_param_strict_body()
        => CompilationFailureTest("nameless-array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-dflt-params-duplicates.js")]
    public Task nameless_dflt_params_duplicates()
        => CompilationFailureTest("nameless-dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-dflt-params-rest.js")]
    public Task nameless_dflt_params_rest()
        => CompilationFailureTest("nameless-dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-object-destructuring-param-strict-body.js")]
    public Task nameless_object_destructuring_param_strict_body()
        => CompilationFailureTest("nameless-object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-rest-param-strict-body.js")]
    public Task nameless_rest_param_strict_body()
        => CompilationFailureTest("nameless-rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "nameless-rest-params-trailing-comma-early-error.js")]
    public Task nameless_rest_params_trailing_comma_early_error()
        => CompilationFailureTest("nameless-rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

}
