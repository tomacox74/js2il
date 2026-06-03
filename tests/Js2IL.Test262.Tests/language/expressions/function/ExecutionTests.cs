using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.function;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "arguments-with-arguments-fn")]
    public Task arguments_with_arguments_fn()
        => ExecutionTest("arguments-with-arguments-fn");

    [Fact(DisplayName = "arguments-with-arguments-lex")]
    public Task arguments_with_arguments_lex()
        => ExecutionTest("arguments-with-arguments-lex");

    [Fact(DisplayName = "param-dflt-yield-non-strict")]
    public Task param_dflt_yield_non_strict()
        => ExecutionTest("param-dflt-yield-non-strict");

    [Fact(DisplayName = "param-eval-non-strict-is-correct-value")]
    public Task param_eval_non_strict_is_correct_value()
        => ExecutionTest("param-eval-non-strict-is-correct-value");

    [Fact(DisplayName = "S10.1.1_A1_T2")]
    public Task S10_1_1_A1_T2()
        => ExecutionTest("S10.1.1_A1_T2");

    [Fact(DisplayName = "length-dflt")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "name-arguments-non-strict")]
    public Task name_arguments_non_strict()
        => ExecutionTest("name-arguments-non-strict");

    [Fact(DisplayName = "name-eval-non-strict")]
    public Task name_eval_non_strict()
        => ExecutionTest("name-eval-non-strict");

    [Fact(DisplayName = "name-eval-stricteval", Skip = "Blocked: eval is not supported yet.")]
    public Task name_eval_stricteval()
        => ExecutionTest("name-eval-stricteval");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "param-arguments-non-strict")]
    public Task param_arguments_non_strict()
        => ExecutionTest("param-arguments-non-strict");

    [Fact(DisplayName = "param-duplicated-non-strict")]
    public Task param_duplicated_non_strict()
        => ExecutionTest("param-duplicated-non-strict");

    [Fact(DisplayName = "param-eval-non-strict")]
    public Task param_eval_non_strict()
        => ExecutionTest("param-eval-non-strict");

    [Fact(DisplayName = "param-eval-stricteval", Skip = "Blocked: eval is not supported yet.")]
    public Task param_eval_stricteval()
        => ExecutionTest("param-eval-stricteval");

    [Fact(DisplayName = "params-dflt-args-unmapped")]
    public Task params_dflt_args_unmapped()
        => ExecutionTest("params-dflt-args-unmapped");

    [Fact(DisplayName = "params-dflt-ref-arguments")]
    public Task params_dflt_ref_arguments()
        => ExecutionTest("params-dflt-ref-arguments");

    [Fact(DisplayName = "scope-body-lex-distinct", Skip = "Blocked: eval is not supported yet.")]
    public Task scope_body_lex_distinct()
        => ExecutionTest("scope-body-lex-distinct");

    [Fact(DisplayName = "scope-name-var-close")]
    public Task scope_name_var_close()
        => ExecutionTest("scope-name-var-close");

    [Fact(DisplayName = "scope-name-var-open-non-strict")]
    public Task scope_name_var_open_non_strict()
        => ExecutionTest("scope-name-var-open-non-strict");

    [Fact(DisplayName = "scope-name-var-open-strict")]
    public Task scope_name_var_open_strict()
        => ExecutionTest("scope-name-var-open-strict");

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

    [Fact(DisplayName = "dflt-params-trailing-comma")]
    public Task dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body")]
    public Task named_no_strict_reassign_fn_name_in_body()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-no-strict-reassign-fn-name-in-body-in-arrow")]
    public Task named_no_strict_reassign_fn_name_in_body_in_arrow()
        => ExecutionTest("named-no-strict-reassign-fn-name-in-body-in-arrow");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body")]
    public Task named_strict_error_reassign_fn_name_in_body()
        => ExecutionTest("named-strict-error-reassign-fn-name-in-body");

    [Fact(DisplayName = "named-strict-error-reassign-fn-name-in-body-in-arrow")]
    public Task named_strict_error_reassign_fn_name_in_body_in_arrow()
        => ExecutionTest("named-strict-error-reassign-fn-name-in-body-in-arrow");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "scope-param-elem-var-close")]
    public Task scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open")]
    public Task scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");

    [Fact(DisplayName = "scope-param-rest-elem-var-close")]
    public Task scope_param_rest_elem_var_close()
        => ExecutionTest("scope-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-param-rest-elem-var-open")]
    public Task scope_param_rest_elem_var_open()
        => ExecutionTest("scope-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");

    [Fact(DisplayName = "scope-paramsbody-var-open")]
    public Task scope_paramsbody_var_open()
        => ExecutionTest("scope-paramsbody-var-open");
}
