namespace Jroc.Test262.Tests.language.expressions.async_generator;

public class PortFunctionAsyncGeneratorBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public PortFunctionAsyncGeneratorBatchParseTests() : base(@"language/expressions/async-generator", "language.expressions.async_generator") { }

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

    [Fact(DisplayName = "ary-ptrn-rest-init-ary")]
    public Task dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-ary");

    [Fact(DisplayName = "ary-ptrn-rest-init-id")]
    public Task dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-id");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj")]
    public Task dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-obj");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary")]
    public Task dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-ary");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id")]
    public Task dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-id");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj")]
    public Task dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-obj");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-ary")]
    public Task dstr_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-ary");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-id")]
    public Task dstr_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-obj")]
    public Task dstr_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-obj");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-ary")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-ary");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-id")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-obj")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-obj");

    [Fact(DisplayName = "named-ary-ptrn-rest-init-ary")]
    public Task dstr_named_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-init-ary");

    [Fact(DisplayName = "named-ary-ptrn-rest-init-id")]
    public Task dstr_named_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-init-id");

    [Fact(DisplayName = "named-ary-ptrn-rest-init-obj")]
    public Task dstr_named_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-init-obj");

    [Fact(DisplayName = "named-ary-ptrn-rest-not-final-ary")]
    public Task dstr_named_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-not-final-ary");

    [Fact(DisplayName = "named-ary-ptrn-rest-not-final-id")]
    public Task dstr_named_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-not-final-id");

    [Fact(DisplayName = "named-ary-ptrn-rest-not-final-obj")]
    public Task dstr_named_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/named-ary-ptrn-rest-not-final-obj");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-init-ary")]
    public Task dstr_named_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-init-ary");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-init-id")]
    public Task dstr_named_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-init-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-init-obj")]
    public Task dstr_named_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-init-obj");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-not-final-ary")]
    public Task dstr_named_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-not-final-ary");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-not-final-id")]
    public Task dstr_named_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-not-final-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-not-final-obj")]
    public Task dstr_named_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/named-dflt-ary-ptrn-rest-not-final-obj");

    [Fact(DisplayName = "early-errors-expression-NSPL-with-USD")]
    public Task early_errors_expression_NSPL_with_USD()
        => CompilationFailureTest("early-errors-expression-NSPL-with-USD");

    [Fact(DisplayName = "early-errors-expression-arguments-in-formal-parameters")]
    public Task early_errors_expression_arguments_in_formal_parameters()
        => CompilationFailureTest("early-errors-expression-arguments-in-formal-parameters");

    [Fact(DisplayName = "early-errors-expression-await-as-function-binding-identifier")]
    public Task early_errors_expression_await_as_function_binding_identifier()
        => CompilationFailureTest("early-errors-expression-await-as-function-binding-identifier");

    [Fact(DisplayName = "early-errors-expression-binding-identifier-arguments")]
    public Task early_errors_expression_binding_identifier_arguments()
        => CompilationFailureTest("early-errors-expression-binding-identifier-arguments");
}
