using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "ArrowFunction_restricted-properties")]
    public Task ArrowFunction_restricted_properties()
        => ExecutionTest("ArrowFunction_restricted-properties");

    [Fact(DisplayName = "ArrowFunction_cannot-override-this-with-thisArg")]
    public Task ArrowFunction_cannot_override_this_with_thisArg()
        => ExecutionTest("ArrowFunction_cannot-override-this-with-thisArg");

    [Fact(DisplayName = "ArrowFunction_default-parameter-abrupt-initializer")]
    public Task ArrowFunction_default_parameter_abrupt_initializer()
        => ExecutionTest("ArrowFunction_default-parameter-abrupt-initializer");

    [Fact(DisplayName = "dflt-params-abrupt")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-later")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-ref-self")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "length-dflt")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "lexical-bindings-overriden-by-formal-parameters-non-strict")]
    public Task lexical_bindings_overriden_by_formal_parameters_non_strict()
        => ExecutionTest("lexical-bindings-overriden-by-formal-parameters-non-strict");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest("dstr/ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-rest-id")]
    public Task dstr_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "low-precedence-expression-body-no-parens")]
    public Task low_precedence_expression_body_no_parens()
        => ExecutionTest("low-precedence-expression-body-no-parens");
    [Fact(DisplayName = "non-strict")]
    public Task non_strict()
        => ExecutionTest("non-strict");
    [Fact(DisplayName = "strict")]
    public Task strict()
        => ExecutionTest("strict");
    [Fact(DisplayName = "throw-new")]
    public Task throw_new()
        => ExecutionTest("throw-new");
    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");
    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "statement-body-requires-braces-must-return-explicitly-missing")]
    public Task statement_body_requires_braces_must_return_explicitly_missing()
        => ExecutionTest("statement-body-requires-braces-must-return-explicitly-missing");
    [Fact(DisplayName = "statement-body-requires-braces-must-return-explicitly")]
    public Task statement_body_requires_braces_must_return_explicitly()
        => ExecutionTest("statement-body-requires-braces-must-return-explicitly");

    [Fact(DisplayName = "scope-param-elem-var-close")]
    public Task scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open")]
    public Task scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");

    [Fact(DisplayName = "binding-tests-1")]
    public Task arrow_binding_tests_1()
        => ExecutionTest("arrow/binding-tests-1");

    [Fact(DisplayName = "binding-tests-2")]
    public Task arrow_binding_tests_2()
        => ExecutionTest("arrow/binding-tests-2");

    [Fact(DisplayName = "binding-tests-3")]
    public Task arrow_binding_tests_3()
        => ExecutionTest("arrow/binding-tests-3");

    [Fact(DisplayName = "capturing-closure-variables-1")]
    public Task arrow_capturing_closure_variables_1()
        => ExecutionTest("arrow/capturing-closure-variables-1");

    [Fact(DisplayName = "capturing-closure-variables-2")]
    public Task arrow_capturing_closure_variables_2()
        => ExecutionTest("arrow/capturing-closure-variables-2");

    [Fact(DisplayName = "empty-function-body-returns-undefined")]
    public Task empty_function_body_returns_undefined()
        => ExecutionTest("empty-function-body-returns-undefined");

    [Fact(DisplayName = "expression-body-implicit-return")]
    public Task expression_body_implicit_return()
        => ExecutionTest("expression-body-implicit-return");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTest("extensibility");

    [Fact(DisplayName = "lexical-arguments")]
    public Task lexical_arguments()
        => ExecutionTest("lexical-arguments");

    [Fact(DisplayName = "lexical-this")]
    public Task lexical_this()
        => ExecutionTest("lexical-this");

    [Fact(DisplayName = "object-literal-return-requires-body-parens")]
    public Task object_literal_return_requires_body_parens()
        => ExecutionTest("object-literal-return-requires-body-parens");

    [Fact(DisplayName = "prototype-rules")]
    public Task prototype_rules()
        => ExecutionTest("prototype-rules");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init")]
    public Task dstr_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter")]
    public Task dstr_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter")]
    public Task dstr_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init")]
    public Task dstr_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter")]
    public Task dstr_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null")]
    public Task dstr_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted")]
    public Task dstr_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole")]
    public Task dstr_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped")]
    public Task dstr_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws")]
    public Task dstr_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef")]
    public Task dstr_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete")]
    public Task dstr_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done")]
    public Task dstr_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val")]
    public Task dstr_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id-init")]
    public Task dstr_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id")]
    public Task dstr_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id");
}
