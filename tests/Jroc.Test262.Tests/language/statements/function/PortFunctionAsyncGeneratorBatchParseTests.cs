namespace Jroc.Test262.Tests.language.statements.function;

public class PortFunctionAsyncGeneratorBatchParseTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public PortFunctionAsyncGeneratorBatchParseTests() : base(@"language\statements\function", "language.statements.function_") { }

    [Fact(DisplayName = "13.1-13gs")]
    public Task case_13_1_13gs()
        => CompilationFailureTest("13.1-13gs");

    [Fact(DisplayName = "13.1-1gs")]
    public Task case_13_1_1gs()
        => CompilationFailureTest("13.1-1gs");

    [Fact(DisplayName = "13.1-4gs")]
    public Task case_13_1_4gs()
        => CompilationFailureTest("13.1-4gs");

    [Fact(DisplayName = "13.1-5gs")]
    public Task case_13_1_5gs()
        => CompilationFailureTest("13.1-5gs");

    [Fact(DisplayName = "13.1-8gs")]
    public Task case_13_1_8gs()
        => CompilationFailureTest("13.1-8gs");

    [Fact(DisplayName = "S13_A7_T3")]
    public Task S13_A7_T3()
        => CompilationFailureTest("S13_A7_T3");

    [Fact(DisplayName = "array-destructuring-param-strict-body")]
    public Task array_destructuring_param_strict_body()
        => CompilationFailureTest("array-destructuring-param-strict-body");

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

    [Fact(DisplayName = "early-body-super-call")]
    public Task early_body_super_call()
        => CompilationFailureTest("early-body-super-call");

    [Fact(DisplayName = "early-body-super-prop")]
    public Task early_body_super_prop()
        => CompilationFailureTest("early-body-super-prop");

    [Fact(DisplayName = "invalid-names-call-expression-bad-reference")]
    public Task early_errors_invalid_names_call_expression_bad_reference()
        => CompilationFailureTest("early-errors/invalid-names-call-expression-bad-reference");

    [Fact(DisplayName = "invalid-names-call-expression-this")]
    public Task early_errors_invalid_names_call_expression_this()
        => CompilationFailureTest("early-errors/invalid-names-call-expression-this");

    [Fact(DisplayName = "invalid-names-member-expression-bad-reference")]
    public Task early_errors_invalid_names_member_expression_bad_reference()
        => CompilationFailureTest("early-errors/invalid-names-member-expression-bad-reference");

    [Fact(DisplayName = "invalid-names-member-expression-this")]
    public Task early_errors_invalid_names_member_expression_this()
        => CompilationFailureTest("early-errors/invalid-names-member-expression-this");

    [Fact(DisplayName = "early-params-super-call")]
    public Task early_params_super_call()
        => CompilationFailureTest("early-params-super-call");

    [Fact(DisplayName = "early-params-super-prop")]
    public Task early_params_super_prop()
        => CompilationFailureTest("early-params-super-prop");

    [Fact(DisplayName = "enable-strict-via-body")]
    public Task enable_strict_via_body()
        => CompilationFailureTest("enable-strict-via-body");

    [Fact(DisplayName = "enable-strict-via-outer-body")]
    public Task enable_strict_via_outer_body()
        => CompilationFailureTest("enable-strict-via-outer-body");

    [Fact(DisplayName = "enable-strict-via-outer-script")]
    public Task enable_strict_via_outer_script()
        => CompilationFailureTest("enable-strict-via-outer-script");

    [Fact(DisplayName = "invalid-2-names")]
    public Task invalid_2_names()
        => CompilationFailureTest("invalid-2-names");

    [Fact(DisplayName = "invalid-3-names")]
    public Task invalid_3_names()
        => CompilationFailureTest("invalid-3-names");

    [Fact(DisplayName = "invalid-function-body-1")]
    public Task invalid_function_body_1()
        => CompilationFailureTest("invalid-function-body-1");

    [Fact(DisplayName = "invalid-function-body-2")]
    public Task invalid_function_body_2()
        => CompilationFailureTest("invalid-function-body-2");

    [Fact(DisplayName = "invalid-function-body-3")]
    public Task invalid_function_body_3()
        => CompilationFailureTest("invalid-function-body-3");

    [Fact(DisplayName = "invalid-name-dot")]
    public Task invalid_name_dot()
        => CompilationFailureTest("invalid-name-dot");

    [Fact(DisplayName = "invalid-name-two-dots")]
    public Task invalid_name_two_dots()
        => CompilationFailureTest("invalid-name-two-dots");

    [Fact(DisplayName = "name-arguments-strict-body")]
    public Task name_arguments_strict_body()
        => CompilationFailureTest("name-arguments-strict-body");

    [Fact(DisplayName = "name-arguments-strict")]
    public Task name_arguments_strict()
        => CompilationFailureTest("name-arguments-strict");

    [Fact(DisplayName = "name-eval-strict-body")]
    public Task name_eval_strict_body()
        => CompilationFailureTest("name-eval-strict-body");

    [Fact(DisplayName = "name-eval-strict")]
    public Task name_eval_strict()
        => CompilationFailureTest("name-eval-strict");

    [Fact(DisplayName = "object-destructuring-param-strict-body")]
    public Task object_destructuring_param_strict_body()
        => CompilationFailureTest("object-destructuring-param-strict-body");

    [Fact(DisplayName = "param-arguments-strict-body")]
    public Task param_arguments_strict_body()
        => CompilationFailureTest("param-arguments-strict-body");

    [Fact(DisplayName = "param-arguments-strict")]
    public Task param_arguments_strict()
        => CompilationFailureTest("param-arguments-strict");

    [Fact(DisplayName = "param-dflt-yield-strict")]
    public Task param_dflt_yield_strict()
        => CompilationFailureTest("param-dflt-yield-strict");

    [Fact(DisplayName = "param-duplicated-strict-1")]
    public Task param_duplicated_strict_1()
        => CompilationFailureTest("param-duplicated-strict-1");

    [Fact(DisplayName = "param-duplicated-strict-2")]
    public Task param_duplicated_strict_2()
        => CompilationFailureTest("param-duplicated-strict-2");

    [Fact(DisplayName = "param-duplicated-strict-3")]
    public Task param_duplicated_strict_3()
        => CompilationFailureTest("param-duplicated-strict-3");

    [Fact(DisplayName = "param-duplicated-strict-body-1")]
    public Task param_duplicated_strict_body_1()
        => CompilationFailureTest("param-duplicated-strict-body-1");

    [Fact(DisplayName = "param-duplicated-strict-body-2")]
    public Task param_duplicated_strict_body_2()
        => CompilationFailureTest("param-duplicated-strict-body-2");

    [Fact(DisplayName = "param-duplicated-strict-body-3")]
    public Task param_duplicated_strict_body_3()
        => CompilationFailureTest("param-duplicated-strict-body-3");

    [Fact(DisplayName = "param-eval-strict-body")]
    public Task param_eval_strict_body()
        => CompilationFailureTest("param-eval-strict-body");

    [Fact(DisplayName = "param-eval-strict")]
    public Task param_eval_strict()
        => CompilationFailureTest("param-eval-strict");

    [Fact(DisplayName = "rest-param-strict-body")]
    public Task rest_param_strict_body()
        => CompilationFailureTest("rest-param-strict-body");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error")]
    public Task rest_params_trailing_comma_early_error()
        => CompilationFailureTest("rest-params-trailing-comma-early-error");

    [Fact(DisplayName = "static-init-await-binding-invalid")]
    public Task static_init_await_binding_invalid()
        => CompilationFailureTest("static-init-await-binding-invalid");

    [Fact(DisplayName = "use-strict-with-non-simple-param")]
    public Task use_strict_with_non_simple_param()
        => CompilationFailureTest("use-strict-with-non-simple-param");
}
