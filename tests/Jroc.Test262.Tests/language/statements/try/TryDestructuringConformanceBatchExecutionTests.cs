using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.try_;

public class TryDestructuringConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public TryDestructuringConformanceBatchExecutionTests()
        : base("language/statements/try", "language.statements.try_") { }

    [Fact(DisplayName = "ary-init-iter-get-err.js")]
    public Task test_ary_init_iter_get_err()
        => ExecutionTest("dstr/ary-init-iter-get-err");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init.js")]
    public Task test_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init.js")]
    public Task test_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init.js")]
    public Task test_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-val-null.js")]
    public Task test_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole.js")]
    public Task test_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped.js")]
    public Task test_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef.js")]
    public Task test_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete.js")]
    public Task test_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done.js")]
    public Task test_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val.js")]
    public Task test_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id-init.js")]
    public Task test_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id.js")]
    public Task test_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id.js")]
    public Task test_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-null.js")]
    public Task test_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-undef.js")]
    public Task test_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "ary-ptrn-elision-exhausted.js")]
    public Task test_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "ary-ptrn-elision-step-err.js")]
    public Task test_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/ary-ptrn-elision-step-err");

    [Fact(DisplayName = "ary-ptrn-elision.js")]
    public Task test_ary_ptrn_elision()
        => ExecutionTest("dstr/ary-ptrn-elision");

    [Fact(DisplayName = "ary-ptrn-empty.js")]
    public Task test_ary_ptrn_empty()
        => ExecutionTest("dstr/ary-ptrn-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elem.js")]
    public Task test_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elision.js")]
    public Task test_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "ary-ptrn-rest-ary-empty.js")]
    public Task test_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-rest.js")]
    public Task test_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "ary-ptrn-rest-id-direct.js")]
    public Task test_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision.js")]
    public Task test_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "ary-ptrn-rest-id-exhausted.js")]
    public Task test_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-rest-id.js")]
    public Task test_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-id.js")]
    public Task test_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-prop-id.js")]
    public Task test_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "obj-init-undefined.js")]
    public Task test_obj_init_undefined()
        => ExecutionTest("dstr/obj-init-undefined");

    [Fact(DisplayName = "obj-ptrn-empty.js")]
    public Task test_obj_ptrn_empty()
        => ExecutionTest("dstr/obj-ptrn-empty");

    [Fact(DisplayName = "obj-ptrn-id-get-value-err.js")]
    public Task test_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-id-init-skipped.js")]
    public Task test_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-id-init-unresolvable.js")]
    public Task test_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-id-trailing-comma.js")]
    public Task test_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary-init.js")]
    public Task test_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary-value-null.js")]
    public Task test_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-ary.js")]
    public Task test_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/obj-ptrn-prop-ary");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-skipped.js")]
    public Task test_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-prop-id-init.js")]
    public Task test_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init");

    [Fact(DisplayName = "obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-id.js")]
    public Task test_obj_ptrn_prop_id()
        => ExecutionTest("dstr/obj-ptrn-prop-id");

    [Fact(DisplayName = "obj-ptrn-prop-obj-init.js")]
    public Task test_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-null.js")]
    public Task test_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-undef.js")]
    public Task test_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "obj-ptrn-prop-obj.js")]
    public Task test_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/obj-ptrn-prop-obj");

    [Fact(DisplayName = "obj-ptrn-rest-getter.js")]
    public Task test_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/obj-ptrn-rest-getter");

    [Fact(DisplayName = "obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-ptrn-rest-val-obj.js")]
    public Task test_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/obj-ptrn-rest-val-obj");
}
