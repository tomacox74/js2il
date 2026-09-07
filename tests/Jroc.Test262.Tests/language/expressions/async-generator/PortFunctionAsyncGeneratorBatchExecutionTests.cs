using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_generator;

public class PortFunctionAsyncGeneratorBatchExecutionTests : DiskExecutionTestsBase
{
    public PortFunctionAsyncGeneratorBatchExecutionTests() : base("language.expressions.async-generator") { }

    [Fact(DisplayName = "dflt-params-abrupt")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later")]
    public Task dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype")]
    public Task dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-get-err")]
    public Task dstr_ary_init_iter_get_err()
        => ExecutionTest("dstr/ary-init-iter-get-err");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null")]
    public Task dstr_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws")]
    public Task dstr_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err")]
    public Task dstr_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-err")]
    public Task dstr_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-null")]
    public Task dstr_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-undef")]
    public Task dstr_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "ary-ptrn-elision-step-err")]
    public Task dstr_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/ary-ptrn-elision-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision-next-err")]
    public Task dstr_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-step-err")]
    public Task dstr_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-val-err")]
    public Task dstr_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err-array-prototype")]
    public Task dstr_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err")]
    public Task dstr_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-val-null")]
    public Task dstr_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-throws")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-step-err")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-err")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-null")]
    public Task dstr_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-undef")]
    public Task dstr_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-step-err")]
    public Task dstr_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision-next-err")]
    public Task dstr_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-step-err")]
    public Task dstr_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-val-err")]
    public Task dstr_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-obj-init-null")]
    public Task dstr_dflt_obj_init_null()
        => ExecutionTest("dstr/dflt-obj-init-null");

    [Fact(DisplayName = "dflt-obj-init-undefined")]
    public Task dstr_dflt_obj_init_undefined()
        => ExecutionTest("dstr/dflt-obj-init-undefined");

    [Fact(DisplayName = "dflt-obj-ptrn-id-get-value-err")]
    public Task dstr_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-throws")]
    public Task dstr_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-unresolvable")]
    public Task dstr_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-list-err")]
    public Task dstr_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-value-null")]
    public Task dstr_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-eval-err")]
    public Task dstr_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-get-value-err")]
    public Task dstr_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-throws")]
    public Task dstr_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-unresolvable")]
    public Task dstr_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-null")]
    public Task dstr_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-undef")]
    public Task dstr_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "named-ary-init-iter-get-err-array-prototype")]
    public Task dstr_named_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/named-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "named-ary-init-iter-get-err")]
    public Task dstr_named_ary_init_iter_get_err()
        => ExecutionTest("dstr/named-ary-init-iter-get-err");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-val-null")]
    public Task dstr_named_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/named-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-throws")]
    public Task dstr_named_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/named-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_named_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/named-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-step-err")]
    public Task dstr_named_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/named-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-val-err")]
    public Task dstr_named_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/named-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-val-null")]
    public Task dstr_named_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/named-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-val-undef")]
    public Task dstr_named_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/named-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "named-ary-ptrn-elision-step-err")]
    public Task dstr_named_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/named-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-elision-next-err")]
    public Task dstr_named_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/named-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-iter-step-err")]
    public Task dstr_named_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/named-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-iter-val-err")]
    public Task dstr_named_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/named-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "named-dflt-ary-init-iter-get-err-array-prototype")]
    public Task dstr_named_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/named-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "named-dflt-ary-init-iter-get-err")]
    public Task dstr_named_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/named-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-val-null")]
    public Task dstr_named_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-throws")]
    public Task dstr_named_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-unresolvable")]
    public Task dstr_named_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-step-err")]
    public Task dstr_named_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-val-err")]
    public Task dstr_named_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-val-null")]
    public Task dstr_named_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-val-undef")]
    public Task dstr_named_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elision-step-err")]
    public Task dstr_named_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-elision-next-err")]
    public Task dstr_named_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-iter-step-err")]
    public Task dstr_named_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-iter-val-err")]
    public Task dstr_named_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/named-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "named-dflt-obj-init-null")]
    public Task dstr_named_dflt_obj_init_null()
        => ExecutionTest("dstr/named-dflt-obj-init-null");

    [Fact(DisplayName = "named-dflt-obj-init-undefined")]
    public Task dstr_named_dflt_obj_init_undefined()
        => ExecutionTest("dstr/named-dflt-obj-init-undefined");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-get-value-err")]
    public Task dstr_named_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-throws")]
    public Task dstr_named_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-unresolvable")]
    public Task dstr_named_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "named-dflt-obj-ptrn-list-err")]
    public Task dstr_named_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-ary-value-null")]
    public Task dstr_named_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-eval-err")]
    public Task dstr_named_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-get-value-err")]
    public Task dstr_named_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-init-throws")]
    public Task dstr_named_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-init-unresolvable")]
    public Task dstr_named_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-obj-value-null")]
    public Task dstr_named_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/named-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "named-obj-init-null")]
    public Task dstr_named_obj_init_null()
        => ExecutionTest("dstr/named-obj-init-null");

    [Fact(DisplayName = "named-obj-init-undefined")]
    public Task dstr_named_obj_init_undefined()
        => ExecutionTest("dstr/named-obj-init-undefined");

    [Fact(DisplayName = "named-obj-ptrn-id-get-value-err")]
    public Task dstr_named_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/named-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "named-obj-ptrn-id-init-throws")]
    public Task dstr_named_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/named-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "named-obj-ptrn-id-init-unresolvable")]
    public Task dstr_named_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/named-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "named-obj-ptrn-list-err")]
    public Task dstr_named_obj_ptrn_list_err()
        => ExecutionTest("dstr/named-obj-ptrn-list-err");

    [Fact(DisplayName = "named-obj-ptrn-prop-ary-value-null")]
    public Task dstr_named_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/named-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "named-obj-ptrn-prop-eval-err")]
    public Task dstr_named_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/named-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-get-value-err")]
    public Task dstr_named_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/named-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-init-throws")]
    public Task dstr_named_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/named-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-init-unresolvable")]
    public Task dstr_named_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/named-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "named-obj-ptrn-prop-obj-value-null")]
    public Task dstr_named_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/named-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "named-obj-ptrn-prop-obj-value-undef")]
    public Task dstr_named_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/named-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "obj-init-null")]
    public Task dstr_obj_init_null()
        => ExecutionTest("dstr/obj-init-null");

    [Fact(DisplayName = "obj-init-undefined")]
    public Task dstr_obj_init_undefined()
        => ExecutionTest("dstr/obj-init-undefined");

    [Fact(DisplayName = "obj-ptrn-id-get-value-err")]
    public Task dstr_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-id-init-throws")]
    public Task dstr_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-id-init-unresolvable")]
    public Task dstr_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-list-err")]
    public Task dstr_obj_ptrn_list_err()
        => ExecutionTest("dstr/obj-ptrn-list-err");

    [Fact(DisplayName = "obj-ptrn-prop-ary-value-null")]
    public Task dstr_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-eval-err")]
    public Task dstr_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-get-value-err")]
    public Task dstr_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-throws")]
    public Task dstr_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-unresolvable")]
    public Task dstr_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-null")]
    public Task dstr_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-undef")]
    public Task dstr_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-undef");
}
