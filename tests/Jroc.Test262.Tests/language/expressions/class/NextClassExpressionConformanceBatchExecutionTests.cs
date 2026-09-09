using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.class_;

public class NextClassExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public NextClassExpressionConformanceBatchExecutionTests() : base("language/expressions/class", "language.expressions.class_") { }

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-obj-init-null.js")]
    public Task test_dstr_meth_obj_init_null()
        => ExecutionTest("dstr/meth-obj-init-null");

    [Fact(DisplayName = "meth-obj-init-undefined.js")]
    public Task test_dstr_meth_obj_init_undefined()
        => ExecutionTest("dstr/meth-obj-init-undefined");

    [Fact(DisplayName = "meth-obj-ptrn-empty.js")]
    public Task test_dstr_meth_obj_ptrn_empty()
        => ExecutionTest("dstr/meth-obj-ptrn-empty");

    [Fact(DisplayName = "meth-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_meth_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/meth-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_meth_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/meth-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-list-err.js")]
    public Task test_dstr_meth_obj_ptrn_list_err()
        => ExecutionTest("dstr/meth-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_meth_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_meth_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id.js")]
    public Task test_dstr_meth_obj_ptrn_prop_id()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_meth_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_meth_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/meth-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-obj-ptrn-rest-getter.js")]
    public Task test_dstr_meth_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-static-ary-init-iter-close.js")]
    public Task test_dstr_meth_static_ary_init_iter_close()
        => ExecutionTest("dstr/meth-static-ary-init-iter-close");

    [Fact(DisplayName = "meth-static-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_meth_static_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/meth-static-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-static-ary-init-iter-get-err.js")]
    public Task test_dstr_meth_static_ary_init_iter_get_err()
        => ExecutionTest("dstr/meth-static-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-static-ary-init-iter-no-close.js")]
    public Task test_dstr_meth_static_ary_init_iter_no_close()
        => ExecutionTest("dstr/meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-static-ary-name-iter-val.js")]
    public Task test_dstr_meth_static_ary_name_iter_val()
        => ExecutionTest("dstr/meth-static-ary-name-iter-val");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-static-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_meth_static_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-static-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-static-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-elision.js")]
    public Task test_dstr_meth_static_ary_ptrn_elision()
        => ExecutionTest("dstr/meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "meth-static-ary-ptrn-empty.js")]
    public Task test_dstr_meth_static_ary_ptrn_empty()
        => ExecutionTest("dstr/meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-id.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-close.js")]
    public Task test_dstr_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task test_dstr_meth_static_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/meth-static-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-get-err.js")]
    public Task test_dstr_meth_static_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-static-dflt-ary-name-iter-val.js")]
    public Task test_dstr_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision-step-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-static-dflt-obj-init-null.js")]
    public Task test_dstr_meth_static_dflt_obj_init_null()
        => ExecutionTest("dstr/meth-static-dflt-obj-init-null");

    [Fact(DisplayName = "meth-static-dflt-obj-init-undefined.js")]
    public Task test_dstr_meth_static_dflt_obj_init_undefined()
        => ExecutionTest("dstr/meth-static-dflt-obj-init-undefined");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-list-err.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-static-obj-init-null.js")]
    public Task test_dstr_meth_static_obj_init_null()
        => ExecutionTest("dstr/meth-static-obj-init-null");

    [Fact(DisplayName = "meth-static-obj-init-undefined.js")]
    public Task test_dstr_meth_static_obj_init_undefined()
        => ExecutionTest("dstr/meth-static-obj-init-undefined");

    [Fact(DisplayName = "meth-static-obj-ptrn-empty.js")]
    public Task test_dstr_meth_static_obj_ptrn_empty()
        => ExecutionTest("dstr/meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-static-obj-ptrn-list-err.js")]
    public Task test_dstr_meth_static_obj_ptrn_list_err()
        => ExecutionTest("dstr/meth-static-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-ary-value-null.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-eval-err.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-init-throws.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-id.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-obj-value-null.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_dstr_meth_static_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/meth-static-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-static-obj-ptrn-rest-getter.js")]
    public Task test_dstr_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/meth-static-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-static-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/meth-static-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-ary-init-iter-close.js")]
    public Task test_dstr_private_gen_meth_ary_init_iter_close()
        => ExecutionTest("dstr/private-gen-meth-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-ary-init-iter-no-close.js")]
    public Task test_dstr_private_gen_meth_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-gen-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-ary-name-iter-val.js")]
    public Task test_dstr_private_gen_meth_ary_name_iter_val()
        => ExecutionTest("dstr/private-gen-meth-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elision.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_elision()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-init-iter-close.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-name-iter-val.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_gen_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-gen-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_gen_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-gen-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-static-ary-init-iter-close.js")]
    public Task test_dstr_private_gen_meth_static_ary_init_iter_close()
        => ExecutionTest("dstr/private-gen-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-static-ary-init-iter-no-close.js")]
    public Task test_dstr_private_gen_meth_static_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-gen-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-static-ary-name-iter-val.js")]
    public Task test_dstr_private_gen_meth_static_ary_name_iter_val()
        => ExecutionTest("dstr/private-gen-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elision.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_elision()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-init-iter-close.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-name-iter-val.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_gen_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-gen-meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-empty.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_empty()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_gen_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-gen-meth-static-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "after-same-line-gen-computed-symbol-names.js")]
    public Task test_elements_after_same_line_gen_computed_symbol_names()
        => ExecutionTest("elements/after-same-line-gen-computed-symbol-names");

    [Fact(DisplayName = "after-same-line-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_after_same_line_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/after-same-line-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "after-same-line-gen-private-field-usage.js")]
    public Task test_elements_after_same_line_gen_private_field_usage()
        => ExecutionTest("elements/after-same-line-gen-private-field-usage");

    [Fact(DisplayName = "after-same-line-gen-private-method-usage.js")]
    public Task test_elements_after_same_line_gen_private_method_usage()
        => ExecutionTest("elements/after-same-line-gen-private-method-usage");

    [Fact(DisplayName = "after-same-line-gen-private-names.js")]
    public Task test_elements_after_same_line_gen_private_names()
        => ExecutionTest("elements/after-same-line-gen-private-names");

    [Fact(DisplayName = "after-same-line-gen-rs-field-identifier-initializer.js")]
    public Task test_elements_after_same_line_gen_rs_field_identifier_initializer()
        => ExecutionTest("elements/after-same-line-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "after-same-line-gen-rs-field-identifier.js")]
    public Task test_elements_after_same_line_gen_rs_field_identifier()
        => ExecutionTest("elements/after-same-line-gen-rs-field-identifier");

    [Fact(DisplayName = "after-same-line-gen-rs-private-getter-alt.js")]
    public Task test_elements_after_same_line_gen_rs_private_getter_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-private-getter-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-private-getter.js")]
    public Task test_elements_after_same_line_gen_rs_private_getter()
        => ExecutionTest("elements/after-same-line-gen-rs-private-getter");

    [Fact(DisplayName = "after-same-line-gen-rs-private-method-alt.js")]
    public Task test_elements_after_same_line_gen_rs_private_method_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-private-method-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-private-method.js")]
    public Task test_elements_after_same_line_gen_rs_private_method()
        => ExecutionTest("elements/after-same-line-gen-rs-private-method");

    [Fact(DisplayName = "after-same-line-gen-rs-private-setter-alt.js")]
    public Task test_elements_after_same_line_gen_rs_private_setter_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-private-setter-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-private-setter.js")]
    public Task test_elements_after_same_line_gen_rs_private_setter()
        => ExecutionTest("elements/after-same-line-gen-rs-private-setter");

    [Fact(DisplayName = "after-same-line-gen-rs-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_gen_rs_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_after_same_line_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-privatename-identifier-initializer.js")]
    public Task test_elements_after_same_line_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/after-same-line-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-gen-rs-privatename-identifier.js")]
    public Task test_elements_after_same_line_gen_rs_privatename_identifier()
        => ExecutionTest("elements/after-same-line-gen-rs-privatename-identifier");

    [Fact(DisplayName = "after-same-line-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_after_same_line_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/after-same-line-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-static-method-privatename-identifier.js")]
    public Task test_elements_after_same_line_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/after-same-line-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-gen-static-private-methods.js")]
    public Task test_elements_after_same_line_gen_static_private_methods()
        => ExecutionTest("elements/after-same-line-gen-static-private-methods");

    [Fact(DisplayName = "after-same-line-method-computed-symbol-names.js")]
    public Task test_elements_after_same_line_method_computed_symbol_names()
        => ExecutionTest("elements/after-same-line-method-computed-symbol-names");

    [Fact(DisplayName = "after-same-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_after_same_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/after-same-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "after-same-line-method-private-field-usage.js")]
    public Task test_elements_after_same_line_method_private_field_usage()
        => ExecutionTest("elements/after-same-line-method-private-field-usage");

    [Fact(DisplayName = "after-same-line-method-private-method-usage.js")]
    public Task test_elements_after_same_line_method_private_method_usage()
        => ExecutionTest("elements/after-same-line-method-private-method-usage");

    [Fact(DisplayName = "after-same-line-method-private-names.js")]
    public Task test_elements_after_same_line_method_private_names()
        => ExecutionTest("elements/after-same-line-method-private-names");

    [Fact(DisplayName = "after-same-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_after_same_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/after-same-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "after-same-line-method-rs-field-identifier.js")]
    public Task test_elements_after_same_line_method_rs_field_identifier()
        => ExecutionTest("elements/after-same-line-method-rs-field-identifier");

    [Fact(DisplayName = "after-same-line-method-rs-private-getter-alt.js")]
    public Task test_elements_after_same_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/after-same-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "after-same-line-method-rs-private-getter.js")]
    public Task test_elements_after_same_line_method_rs_private_getter()
        => ExecutionTest("elements/after-same-line-method-rs-private-getter");

    [Fact(DisplayName = "after-same-line-method-rs-private-method-alt.js")]
    public Task test_elements_after_same_line_method_rs_private_method_alt()
        => ExecutionTest("elements/after-same-line-method-rs-private-method-alt");

    [Fact(DisplayName = "after-same-line-method-rs-private-method.js")]
    public Task test_elements_after_same_line_method_rs_private_method()
        => ExecutionTest("elements/after-same-line-method-rs-private-method");

    [Fact(DisplayName = "after-same-line-method-rs-private-setter-alt.js")]
    public Task test_elements_after_same_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/after-same-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "after-same-line-method-rs-private-setter.js")]
    public Task test_elements_after_same_line_method_rs_private_setter()
        => ExecutionTest("elements/after-same-line-method-rs-private-setter");

    [Fact(DisplayName = "after-same-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_after_same_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/after-same-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_after_same_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/after-same-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-method-rs-privatename-identifier.js")]
    public Task test_elements_after_same_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/after-same-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "after-same-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_after_same_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/after-same-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_after_same_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/after-same-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_after_same_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/after-same-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "after-same-line-method-static-private-methods.js")]
    public Task test_elements_after_same_line_method_static_private_methods()
        => ExecutionTest("elements/after-same-line-method-static-private-methods");

    [Fact(DisplayName = "computed-name-toprimitive-symbol.js")]
    public Task test_elements_computed_name_toprimitive_symbol()
        => ExecutionTest("elements/computed-name-toprimitive-symbol");

    [Fact(DisplayName = "ctor-called-after-fields-init.js")]
    public Task test_elements_ctor_called_after_fields_init()
        => ExecutionTest("elements/ctor-called-after-fields-init");

    [Fact(DisplayName = "fields-anonymous-function-length.js")]
    public Task test_elements_fields_anonymous_function_length()
        => ExecutionTest("elements/fields-anonymous-function-length");

    [Fact(DisplayName = "fields-run-once-on-double-super.js")]
    public Task test_elements_fields_run_once_on_double_super()
        => ExecutionTest("elements/fields-run-once-on-double-super");
}
