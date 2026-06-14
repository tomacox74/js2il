using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.async_arrow_function;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\expressions\async-arrow-function", "language.expressions.async_arrow_function") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body")]
    public Task array_destructuring_param_strict_body()
        => CompilationFailureTest("array-destructuring-param-strict-body");

    [Fact(DisplayName = "await-as-binding-identifier-escaped")]
    public Task await_as_binding_identifier_escaped()
        => CompilationFailureTest("await-as-binding-identifier-escaped");

    [Fact(DisplayName = "await-as-binding-identifier")]
    public Task await_as_binding_identifier()
        => CompilationFailureTest("await-as-binding-identifier");

    [Fact(DisplayName = "await-as-identifier-reference-escaped")]
    public Task await_as_identifier_reference_escaped()
        => CompilationFailureTest("await-as-identifier-reference-escaped");

    [Fact(DisplayName = "await-as-identifier-reference")]
    public Task await_as_identifier_reference()
        => CompilationFailureTest("await-as-identifier-reference");

    [Fact(DisplayName = "await-as-label-identifier-escaped")]
    public Task await_as_label_identifier_escaped()
        => CompilationFailureTest("await-as-label-identifier-escaped");

    [Fact(DisplayName = "await-as-label-identifier")]
    public Task await_as_label_identifier()
        => CompilationFailureTest("await-as-label-identifier");

    [Fact(DisplayName = "await-as-param-ident-nested-arrow-parameter-position")]
    public Task await_as_param_ident_nested_arrow_parameter_position()
        => CompilationFailureTest("await-as-param-ident-nested-arrow-parameter-position");

    [Fact(DisplayName = "await-as-param-nested-arrow-body-position")]
    public Task await_as_param_nested_arrow_body_position()
        => CompilationFailureTest("await-as-param-nested-arrow-body-position");

    [Fact(DisplayName = "await-as-param-nested-arrow-parameter-position")]
    public Task await_as_param_nested_arrow_parameter_position()
        => CompilationFailureTest("await-as-param-nested-arrow-parameter-position");

    [Fact(DisplayName = "await-as-param-rest-nested-arrow-parameter-position")]
    public Task await_as_param_rest_nested_arrow_parameter_position()
        => CompilationFailureTest("await-as-param-rest-nested-arrow-parameter-position");

    [Fact(DisplayName = "dflt-params-duplicates")]
    public Task dflt_params_duplicates()
        => CompilationFailureTest("dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest")]
    public Task dflt_params_rest()
        => CompilationFailureTest("dflt-params-rest");

    [Fact(DisplayName = "early-errors-arrow-await-in-formals-default")]
    public Task early_errors_arrow_await_in_formals_default()
        => CompilationFailureTest("early-errors-arrow-await-in-formals-default");

    [Fact(DisplayName = "early-errors-arrow-duplicate-parameters")]
    public Task early_errors_arrow_duplicate_parameters()
        => CompilationFailureTest("early-errors-arrow-duplicate-parameters");

    [Fact(DisplayName = "early-errors-arrow-body-contains-super-call")]
    public Task early_errors_arrow_body_contains_super_call()
        => CompilationFailureTest("early-errors-arrow-body-contains-super-call");

}
