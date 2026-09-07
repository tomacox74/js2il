using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class FunctionExpressionArrowFunctionConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionArrowFunctionConformanceBatchParseTests() : base("language/expressions/arrow-function", "language.expressions.arrow_function") { }

    [Fact(DisplayName = "array-destructuring-param-strict-body.js")]
    public Task array_destructuring_param_strict_body()
        => CompilationFailureTest("array-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-duplicates.js")]
    public Task dflt_params_duplicates()
        => CompilationFailureTest("dflt-params-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-params-rest.js")]
    public Task dflt_params_rest()
        => CompilationFailureTest("dflt-params-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-ary.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-init-obj.js")]
    public Task dstr_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-break-escaped.js")]
    public Task dstr_syntax_error_ident_ref_break_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-break-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-case-escaped.js")]
    public Task dstr_syntax_error_ident_ref_case_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-case-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-catch-escaped.js")]
    public Task dstr_syntax_error_ident_ref_catch_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-catch-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-class-escaped.js")]
    public Task dstr_syntax_error_ident_ref_class_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-class-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-const-escaped.js")]
    public Task dstr_syntax_error_ident_ref_const_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-const-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-continue-escaped.js")]
    public Task dstr_syntax_error_ident_ref_continue_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-continue-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-debugger-escaped.js")]
    public Task dstr_syntax_error_ident_ref_debugger_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-debugger-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default-escaped-ext.js")]
    public Task dstr_syntax_error_ident_ref_default_escaped_ext()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default-escaped-ext", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default-escaped.js")]
    public Task dstr_syntax_error_ident_ref_default_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default.js")]
    public Task dstr_syntax_error_ident_ref_default()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-delete-escaped.js")]
    public Task dstr_syntax_error_ident_ref_delete_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-delete-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-do-escaped.js")]
    public Task dstr_syntax_error_ident_ref_do_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-do-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-else-escaped.js")]
    public Task dstr_syntax_error_ident_ref_else_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-else-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-enum-escaped.js")]
    public Task dstr_syntax_error_ident_ref_enum_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-enum-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-export-escaped.js")]
    public Task dstr_syntax_error_ident_ref_export_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-export-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends-escaped-ext.js")]
    public Task dstr_syntax_error_ident_ref_extends_escaped_ext()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends-escaped-ext", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends-escaped.js")]
    public Task dstr_syntax_error_ident_ref_extends_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends.js")]
    public Task dstr_syntax_error_ident_ref_extends()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-finally-escaped.js")]
    public Task dstr_syntax_error_ident_ref_finally_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-finally-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-for-escaped.js")]
    public Task dstr_syntax_error_ident_ref_for_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-for-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-function-escaped.js")]
    public Task dstr_syntax_error_ident_ref_function_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-function-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-if-escaped.js")]
    public Task dstr_syntax_error_ident_ref_if_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-if-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-implements-escaped.js")]
    public Task dstr_syntax_error_ident_ref_implements_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-implements-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-import-escaped.js")]
    public Task dstr_syntax_error_ident_ref_import_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-import-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-in-escaped.js")]
    public Task dstr_syntax_error_ident_ref_in_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-in-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-instanceof-escaped.js")]
    public Task dstr_syntax_error_ident_ref_instanceof_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-instanceof-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-interface-escaped.js")]
    public Task dstr_syntax_error_ident_ref_interface_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-interface-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-let-escaped.js")]
    public Task dstr_syntax_error_ident_ref_let_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-let-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-new-escaped.js")]
    public Task dstr_syntax_error_ident_ref_new_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-new-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-package-escaped.js")]
    public Task dstr_syntax_error_ident_ref_package_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-package-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-private-escaped.js")]
    public Task dstr_syntax_error_ident_ref_private_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-private-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-protected-escaped.js")]
    public Task dstr_syntax_error_ident_ref_protected_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-protected-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-public-escaped.js")]
    public Task dstr_syntax_error_ident_ref_public_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-public-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-return-escaped.js")]
    public Task dstr_syntax_error_ident_ref_return_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-return-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-static-escaped.js")]
    public Task dstr_syntax_error_ident_ref_static_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-static-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-super-escaped.js")]
    public Task dstr_syntax_error_ident_ref_super_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-super-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-switch-escaped.js")]
    public Task dstr_syntax_error_ident_ref_switch_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-switch-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-this-escaped.js")]
    public Task dstr_syntax_error_ident_ref_this_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-this-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-throw-escaped.js")]
    public Task dstr_syntax_error_ident_ref_throw_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-throw-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-try-escaped.js")]
    public Task dstr_syntax_error_ident_ref_try_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-try-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-typeof-escaped.js")]
    public Task dstr_syntax_error_ident_ref_typeof_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-typeof-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-var-escaped.js")]
    public Task dstr_syntax_error_ident_ref_var_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-var-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-void-escaped.js")]
    public Task dstr_syntax_error_ident_ref_void_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-void-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-while-escaped.js")]
    public Task dstr_syntax_error_ident_ref_while_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-while-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-with-escaped.js")]
    public Task dstr_syntax_error_ident_ref_with_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-with-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "object-destructuring-param-strict-body.js")]
    public Task object_destructuring_param_strict_body()
        => CompilationFailureTest("object-destructuring-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "param-dflt-yield-expr.js")]
    public Task param_dflt_yield_expr()
        => CompilationFailureTest("param-dflt-yield-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "param-dflt-yield-id-strict.js")]
    public Task param_dflt_yield_id_strict()
        => CompilationFailureTest("param-dflt-yield-id-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "params-duplicate.js")]
    public Task params_duplicate()
        => CompilationFailureTest("params-duplicate", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-param-strict-body.js")]
    public Task rest_param_strict_body()
        => CompilationFailureTest("rest-param-strict-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "rest-params-trailing-comma-early-error.js")]
    public Task rest_params_trailing_comma_early_error()
        => CompilationFailureTest("rest-params-trailing-comma-early-error", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding.js")]
    public Task static_init_await_binding()
        => CompilationFailureTest("static-init-await-binding", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-reference.js")]
    public Task static_init_await_reference()
        => CompilationFailureTest("static-init-await-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-identifier-futurereservedword.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_identifier_futurereservedword()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-identifier-futurereservedword", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-identifier-strict-futurereservedword.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_identifier_strict_futurereservedword()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-identifier-strict-futurereservedword", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-identifier.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_identifier()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-no-arguments.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_no_arguments()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-no-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-no-eval.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_no_eval()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-no-eval", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-no-yield.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_no_yield()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-no-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-rest.js")]
    public Task syntax_early_errors_arrowparameters_bindingidentifier_rest()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-bindingidentifier-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-arguments.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_arguments()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-array-1.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_array_1()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-array-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-array-2.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_array_2()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-array-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-array-3.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_array_3()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-array-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-1.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_1()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-2.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_2()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-3.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_3()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-4.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_4()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-4", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-5.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_5()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-5", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-binding-object-6.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_binding_object_6()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-binding-object-6", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates-rest.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates_rest()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-duplicates.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_duplicates()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-eval.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_eval()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-eval", "Failed to parse JavaScript");

    [Fact(DisplayName = "arrowparameters-cover-no-yield.js")]
    public Task syntax_early_errors_arrowparameters_cover_no_yield()
        => CompilationFailureTest("syntax/early-errors/arrowparameters-cover-no-yield", "Failed to parse JavaScript");

    [Fact(DisplayName = "asi-restriction-invalid-parenless-parameters-expression-body.js")]
    public Task syntax_early_errors_asi_restriction_invalid_parenless_parameters_expression_body()
        => CompilationFailureTest("syntax/early-errors/asi-restriction-invalid-parenless-parameters-expression-body", "Failed to parse JavaScript");

    [Fact(DisplayName = "asi-restriction-invalid-parenless-parameters.js")]
    public Task syntax_early_errors_asi_restriction_invalid_parenless_parameters()
        => CompilationFailureTest("syntax/early-errors/asi-restriction-invalid-parenless-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "asi-restriction-invalid.js")]
    public Task syntax_early_errors_asi_restriction_invalid()
        => CompilationFailureTest("syntax/early-errors/asi-restriction-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "use-strict-with-non-simple-param.js")]
    public Task syntax_early_errors_use_strict_with_non_simple_param()
        => CompilationFailureTest("syntax/early-errors/use-strict-with-non-simple-param", "Failed to parse JavaScript");

}
