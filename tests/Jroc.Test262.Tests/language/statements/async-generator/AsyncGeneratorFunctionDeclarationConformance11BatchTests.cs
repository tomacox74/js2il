using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.async_generator;

public sealed class AsyncGeneratorFunctionDeclarationConformance11BatchTests : DiskExecutionTestsBase
{
    public AsyncGeneratorFunctionDeclarationConformance11BatchTests() : base("language.statements.async_generator") { }

    [Fact(DisplayName = "dflt-params-abrupt.js")]
    public Task test_dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-ref-later.js")]
    public Task test_dflt_params_ref_later()
        => ExecutionTest("dflt-params-ref-later");

    [Fact(DisplayName = "dflt-params-ref-self.js")]
    public Task test_dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-get-err.js")]
    public Task test_dstr_ary_init_iter_get_err()
        => ExecutionTest("dstr/ary-init-iter-get-err");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "ary-ptrn-elision-step-err.js")]
    public Task test_dstr_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/ary-ptrn-elision-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err.js")]
    public Task test_dstr_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-obj-init-null.js")]
    public Task test_dstr_dflt_obj_init_null()
        => ExecutionTest("dstr/dflt-obj-init-null");

    [Fact(DisplayName = "dflt-obj-init-undefined.js")]
    public Task test_dstr_dflt_obj_init_undefined()
        => ExecutionTest("dstr/dflt-obj-init-undefined");

    [Fact(DisplayName = "dflt-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-list-err.js")]
    public Task test_dstr_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "obj-init-null.js")]
    public Task test_dstr_obj_init_null()
        => ExecutionTest("dstr/obj-init-null");

    [Fact(DisplayName = "obj-init-undefined.js")]
    public Task test_dstr_obj_init_undefined()
        => ExecutionTest("dstr/obj-init-undefined");

    [Fact(DisplayName = "obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-id-init-throws.js")]
    public Task test_dstr_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-list-err.js")]
    public Task test_dstr_obj_ptrn_list_err()
        => ExecutionTest("dstr/obj-ptrn-list-err");

    [Fact(DisplayName = "obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "generator-created-after-decl-inst.js")]
    public Task test_generator_created_after_decl_inst()
        => ExecutionTest("generator-created-after-decl-inst");

}
