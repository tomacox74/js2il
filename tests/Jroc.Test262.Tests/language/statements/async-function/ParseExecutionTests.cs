using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.statements.async_function;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\statements\async-function", "language.statements.async_function") { }

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

    [Fact(DisplayName = "dflt-params-duplicates")]
    public Task dflt_params_duplicates()
        => CompilationFailureTest("dflt-params-duplicates");

    [Fact(DisplayName = "dflt-params-rest")]
    public Task dflt_params_rest()
        => CompilationFailureTest("dflt-params-rest");

    [Fact(DisplayName = "early-errors-declaration-await-in-formals-default")]
    public Task early_errors_declaration_await_in_formals_default()
        => CompilationFailureTest("early-errors-declaration-await-in-formals-default");

    [Fact(DisplayName = "early-errors-declaration-await-in-formals")]
    public Task early_errors_declaration_await_in_formals()
        => CompilationFailureTest("early-errors-declaration-await-in-formals");

    [Fact(DisplayName = "early-errors-declaration-body-contains-super-call")]
    public Task early_errors_declaration_body_contains_super_call()
        => CompilationFailureTest("early-errors-declaration-body-contains-super-call");

    [Fact(DisplayName = "early-errors-declaration-body-contains-super-property")]
    public Task early_errors_declaration_body_contains_super_property()
        => CompilationFailureTest("early-errors-declaration-body-contains-super-property");

    [Fact(DisplayName = "early-errors-declaration-formals-body-duplicate")]
    public Task early_errors_declaration_formals_body_duplicate()
        => CompilationFailureTest("early-errors-declaration-formals-body-duplicate");

    [Fact(DisplayName = "early-errors-declaration-formals-contains-super-call")]
    public Task early_errors_declaration_formals_contains_super_call()
        => CompilationFailureTest("early-errors-declaration-formals-contains-super-call");

}
