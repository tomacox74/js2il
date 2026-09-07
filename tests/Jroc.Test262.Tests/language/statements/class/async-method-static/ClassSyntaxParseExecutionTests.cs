using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.async_method_static;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class/async-method-static", "language.statements.class_.async_method_static") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body")]
    public Task array_destructuring_param_strict_body()
        => CompilationFailureTest("array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier-escaped")]
    public Task await_as_binding_identifier_escaped()
        => CompilationFailureTest("await-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier")]
    public Task await_as_binding_identifier()
        => CompilationFailureTest("await-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference-escaped")]
    public Task await_as_identifier_reference_escaped()
        => CompilationFailureTest("await-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference")]
    public Task await_as_identifier_reference()
        => CompilationFailureTest("await-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier-escaped")]
    public Task await_as_label_identifier_escaped()
        => CompilationFailureTest("await-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier")]
    public Task await_as_label_identifier()
        => CompilationFailureTest("await-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates")]
    public Task dflt_params_duplicates()
        => CompilationFailureTest("dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest")]
    public Task dflt_params_rest()
        => CompilationFailureTest("dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body")]
    public Task object_destructuring_param_strict_body()
        => CompilationFailureTest("object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body")]
    public Task rest_param_strict_body()
        => CompilationFailureTest("rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error")]
    public Task rest_params_trailing_comma_early_error()
        => CompilationFailureTest("rest-params-trailing-comma-early-error", "Failed to parse JavaScript");
}
