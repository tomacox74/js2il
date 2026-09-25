namespace Jroc.Test262.Tests.language.statements.generators.dstr;

public sealed class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.generators.dstr") { }

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype.js")]
    public Task Ported_ary_init_iter_get_err_array_prototype() => ExecutionTestFromFile("ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-get-err.js")]
    public Task Ported_ary_init_iter_get_err() => ExecutionTestFromFile("ary-init-iter-get-err");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null.js")]
    public Task Ported_ary_ptrn_elem_ary_val_null() => ExecutionTestFromFile("ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-throws.js")]
    public Task Ported_ary_ptrn_elem_id_init_throws() => ExecutionTestFromFile("ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable.js")]
    public Task Ported_ary_ptrn_elem_id_init_unresolvable() => ExecutionTestFromFile("ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err.js")]
    public Task Ported_ary_ptrn_elem_id_iter_step_err() => ExecutionTestFromFile("ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-err.js")]
    public Task Ported_ary_ptrn_elem_id_iter_val_err() => ExecutionTestFromFile("ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-null.js")]
    public Task Ported_ary_ptrn_elem_obj_val_null() => ExecutionTestFromFile("ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-undef.js")]
    public Task Ported_ary_ptrn_elem_obj_val_undef() => ExecutionTestFromFile("ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "ary-ptrn-elision-step-err.js")]
    public Task Ported_ary_ptrn_elision_step_err() => ExecutionTestFromFile("ary-ptrn-elision-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision-next-err.js")]
    public Task Ported_ary_ptrn_rest_id_elision_next_err() => ExecutionTestFromFile("ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-step-err.js")]
    public Task Ported_ary_ptrn_rest_id_iter_step_err() => ExecutionTestFromFile("ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-val-err.js")]
    public Task Ported_ary_ptrn_rest_id_iter_val_err() => ExecutionTestFromFile("ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task Ported_dflt_ary_init_iter_get_err_array_prototype() => ExecutionTestFromFile("dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err.js")]
    public Task Ported_dflt_ary_init_iter_get_err() => ExecutionTestFromFile("dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task Ported_dflt_ary_ptrn_elem_ary_val_null() => ExecutionTestFromFile("dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task Ported_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTestFromFile("dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task Ported_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTestFromFile("dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task Ported_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTestFromFile("dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task Ported_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTestFromFile("dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task Ported_dflt_ary_ptrn_elem_obj_val_null() => ExecutionTestFromFile("dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task Ported_dflt_ary_ptrn_elem_obj_val_undef() => ExecutionTestFromFile("dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-step-err.js")]
    public Task Ported_dflt_ary_ptrn_elision_step_err() => ExecutionTestFromFile("dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task Ported_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTestFromFile("dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task Ported_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTestFromFile("dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task Ported_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTestFromFile("dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-obj-init-null.js")]
    public Task Ported_dflt_obj_init_null() => ExecutionTestFromFile("dflt-obj-init-null");

    [Fact(DisplayName = "dflt-obj-init-undefined.js")]
    public Task Ported_dflt_obj_init_undefined() => ExecutionTestFromFile("dflt-obj-init-undefined");

    [Fact(DisplayName = "dflt-obj-ptrn-id-get-value-err.js")]
    public Task Ported_dflt_obj_ptrn_id_get_value_err() => ExecutionTestFromFile("dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-throws.js")]
    public Task Ported_dflt_obj_ptrn_id_init_throws() => ExecutionTestFromFile("dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task Ported_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTestFromFile("dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-list-err.js")]
    public Task Ported_dflt_obj_ptrn_list_err() => ExecutionTestFromFile("dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task Ported_dflt_obj_ptrn_prop_ary_value_null() => ExecutionTestFromFile("dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task Ported_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTestFromFile("dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task Ported_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTestFromFile("dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task Ported_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTestFromFile("dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task Ported_dflt_obj_ptrn_prop_obj_value_null() => ExecutionTestFromFile("dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task Ported_dflt_obj_ptrn_prop_obj_value_undef() => ExecutionTestFromFile("dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "obj-init-null.js")]
    public Task Ported_obj_init_null() => ExecutionTestFromFile("obj-init-null");

    [Fact(DisplayName = "obj-init-undefined.js")]
    public Task Ported_obj_init_undefined() => ExecutionTestFromFile("obj-init-undefined");

    [Fact(DisplayName = "obj-ptrn-id-get-value-err.js")]
    public Task Ported_obj_ptrn_id_get_value_err() => ExecutionTestFromFile("obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-id-init-throws.js")]
    public Task Ported_obj_ptrn_id_init_throws() => ExecutionTestFromFile("obj-ptrn-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-id-init-unresolvable.js")]
    public Task Ported_obj_ptrn_id_init_unresolvable() => ExecutionTestFromFile("obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-list-err.js")]
    public Task Ported_obj_ptrn_list_err() => ExecutionTestFromFile("obj-ptrn-list-err");

    [Fact(DisplayName = "obj-ptrn-prop-ary-value-null.js")]
    public Task Ported_obj_ptrn_prop_ary_value_null() => ExecutionTestFromFile("obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-id-get-value-err.js")]
    public Task Ported_obj_ptrn_prop_id_get_value_err() => ExecutionTestFromFile("obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-throws.js")]
    public Task Ported_obj_ptrn_prop_id_init_throws() => ExecutionTestFromFile("obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-unresolvable.js")]
    public Task Ported_obj_ptrn_prop_id_init_unresolvable() => ExecutionTestFromFile("obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-null.js")]
    public Task Ported_obj_ptrn_prop_obj_value_null() => ExecutionTestFromFile("obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-undef.js")]
    public Task Ported_obj_ptrn_prop_obj_value_undef() => ExecutionTestFromFile("obj-ptrn-prop-obj-value-undef");

}
