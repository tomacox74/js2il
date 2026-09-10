using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_;

public class ClassDeclarationConformance10BatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassDeclarationConformance10BatchExecutionTests()
        : base("language/statements/class", "language.statements.class_") { }

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-obj-init-null.js")]
    public Task test_dstr_meth_dflt_obj_init_null()
        => ExecutionTest("dstr/meth-dflt-obj-init-null");

    [Fact(DisplayName = "meth-dflt-obj-init-undefined.js")]
    public Task test_dstr_meth_dflt_obj_init_undefined()
        => ExecutionTest("dstr/meth-dflt-obj-init-undefined");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-get-value-err.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-throws.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-list-err.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/meth-dflt-obj-ptrn-prop-ary-trailing-comma");

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

    [Fact(DisplayName = "private-meth-ary-init-iter-close.js")]
    public Task test_dstr_private_meth_ary_init_iter_close()
        => ExecutionTest("dstr/private-meth-ary-init-iter-close");

    [Fact(DisplayName = "private-meth-ary-init-iter-no-close.js")]
    public Task test_dstr_private_meth_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "private-meth-ary-name-iter-val.js")]
    public Task test_dstr_private_meth_ary_name_iter_val()
        => ExecutionTest("dstr/private-meth-ary-name-iter-val");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-meth-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-meth-ary-ptrn-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elision");

    [Fact(DisplayName = "private-meth-ary-ptrn-empty.js")]
    public Task test_dstr_private_meth_ary_ptrn_empty()
        => ExecutionTest("dstr/private-meth-ary-ptrn-empty");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-meth-obj-ptrn-empty.js")]
    public Task test_dstr_private_meth_obj_ptrn_empty()
        => ExecutionTest("dstr/private-meth-obj-ptrn-empty");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-meth-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-meth-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-meth-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-rest-val-obj");

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

    [Fact(DisplayName = "fielddefinition-initializer-abrupt-completion.js")]
    public Task test_elements_fielddefinition_initializer_abrupt_completion()
        => ExecutionTest("elements/fielddefinition-initializer-abrupt-completion");

    [Fact(DisplayName = "fields-anonymous-function-length.js")]
    public Task test_elements_fields_anonymous_function_length()
        => ExecutionTest("elements/fields-anonymous-function-length");

    [Fact(DisplayName = "fields-asi-1.js")]
    public Task test_elements_fields_asi_1()
        => ExecutionTest("elements/fields-asi-1");

    [Fact(DisplayName = "fields-asi-2.js")]
    public Task test_elements_fields_asi_2()
        => ExecutionTest("elements/fields-asi-2");

    [Fact(DisplayName = "fields-asi-5.js")]
    public Task test_elements_fields_asi_5()
        => ExecutionTest("elements/fields-asi-5");

    [Fact(DisplayName = "fields-computed-name-propname-constructor.js")]
    public Task test_elements_fields_computed_name_propname_constructor()
        => ExecutionTest("elements/fields-computed-name-propname-constructor");

    [Fact(DisplayName = "fields-computed-name-static-computed-var-propname-prototype.js")]
    public Task test_elements_fields_computed_name_static_computed_var_propname_prototype()
        => ExecutionTest("elements/fields-computed-name-static-computed-var-propname-prototype");

    [Fact(DisplayName = "fields-computed-name-static-propname-prototype.js")]
    public Task test_elements_fields_computed_name_static_propname_prototype()
        => ExecutionTest("elements/fields-computed-name-static-propname-prototype");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task test_elements_gen_private_method_static_yield_spread_arr_multiple()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task test_elements_gen_private_method_static_yield_spread_arr_single()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task test_elements_gen_private_method_static_yield_spread_obj()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-obj");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task test_elements_gen_private_method_yield_spread_arr_multiple()
        => ExecutionTest("elements/gen-private-method/yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task test_elements_gen_private_method_yield_spread_arr_single()
        => ExecutionTest("elements/gen-private-method/yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task test_elements_gen_private_method_yield_spread_obj()
        => ExecutionTest("elements/gen-private-method/yield-spread-obj");

    [Fact(DisplayName = "init-err-evaluation.js")]
    public Task test_elements_init_err_evaluation()
        => ExecutionTest("elements/init-err-evaluation");

    [Fact(DisplayName = "new-no-sc-line-method-computed-symbol-names.js")]
    public Task test_elements_new_no_sc_line_method_computed_symbol_names()
        => ExecutionTest("elements/new-no-sc-line-method-computed-symbol-names");

    [Fact(DisplayName = "new-no-sc-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_no_sc_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-no-sc-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "new-no-sc-line-method-private-field-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_field_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-field-usage");

    [Fact(DisplayName = "new-no-sc-line-method-private-method-getter-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_method_getter_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-method-getter-usage");

    [Fact(DisplayName = "new-no-sc-line-method-private-method-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_method_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-method-usage");

    [Fact(DisplayName = "new-no-sc-line-method-private-names.js")]
    public Task test_elements_new_no_sc_line_method_private_names()
        => ExecutionTest("elements/new-no-sc-line-method-private-names");

    [Fact(DisplayName = "new-no-sc-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_new_no_sc_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-no-sc-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "new-no-sc-line-method-rs-field-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_field_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-field-identifier");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-getter-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-getter.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_getter()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-getter");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-method-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_method_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-method-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-method.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_method()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-method");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-setter-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-private-setter.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_setter()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-setter");

    [Fact(DisplayName = "new-no-sc-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "new-no-sc-line-method-rs-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "new-no-sc-line-method-static-private-methods.js")]
    public Task test_elements_new_no_sc_line_method_static_private_methods()
        => ExecutionTest("elements/new-no-sc-line-method-static-private-methods");

    [Fact(DisplayName = "new-sc-line-gen-computed-symbol-names.js")]
    public Task test_elements_new_sc_line_gen_computed_symbol_names()
        => ExecutionTest("elements/new-sc-line-gen-computed-symbol-names");

    [Fact(DisplayName = "new-sc-line-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_sc_line_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-sc-line-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "new-sc-line-gen-private-field-usage.js")]
    public Task test_elements_new_sc_line_gen_private_field_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-field-usage");

    [Fact(DisplayName = "new-sc-line-gen-private-method-getter-usage.js")]
    public Task test_elements_new_sc_line_gen_private_method_getter_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-method-getter-usage");

    [Fact(DisplayName = "new-sc-line-gen-private-method-usage.js")]
    public Task test_elements_new_sc_line_gen_private_method_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-method-usage");

    [Fact(DisplayName = "new-sc-line-gen-private-names.js")]
    public Task test_elements_new_sc_line_gen_private_names()
        => ExecutionTest("elements/new-sc-line-gen-private-names");

    [Fact(DisplayName = "new-sc-line-gen-rs-field-identifier-initializer.js")]
    public Task test_elements_new_sc_line_gen_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-gen-rs-field-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_field_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-field-identifier");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-getter-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_getter_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-getter-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-getter.js")]
    public Task test_elements_new_sc_line_gen_rs_private_getter()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-getter");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-method-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_method_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-method-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-method.js")]
    public Task test_elements_new_sc_line_gen_rs_private_method()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-method");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-setter-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_setter_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-setter-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-private-setter.js")]
    public Task test_elements_new_sc_line_gen_rs_private_setter()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-setter");

    [Fact(DisplayName = "new-sc-line-gen-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-gen-rs-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-gen-static-private-methods.js")]
    public Task test_elements_new_sc_line_gen_static_private_methods()
        => ExecutionTest("elements/new-sc-line-gen-static-private-methods");

    [Fact(DisplayName = "new-sc-line-method-computed-symbol-names.js")]
    public Task test_elements_new_sc_line_method_computed_symbol_names()
        => ExecutionTest("elements/new-sc-line-method-computed-symbol-names");

    [Fact(DisplayName = "new-sc-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_sc_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-sc-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "new-sc-line-method-private-field-usage.js")]
    public Task test_elements_new_sc_line_method_private_field_usage()
        => ExecutionTest("elements/new-sc-line-method-private-field-usage");

    [Fact(DisplayName = "new-sc-line-method-private-method-getter-usage.js")]
    public Task test_elements_new_sc_line_method_private_method_getter_usage()
        => ExecutionTest("elements/new-sc-line-method-private-method-getter-usage");

    [Fact(DisplayName = "new-sc-line-method-private-method-usage.js")]
    public Task test_elements_new_sc_line_method_private_method_usage()
        => ExecutionTest("elements/new-sc-line-method-private-method-usage");

    [Fact(DisplayName = "new-sc-line-method-private-names.js")]
    public Task test_elements_new_sc_line_method_private_names()
        => ExecutionTest("elements/new-sc-line-method-private-names");

    [Fact(DisplayName = "new-sc-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_new_sc_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-method-rs-field-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_field_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-field-identifier");

    [Fact(DisplayName = "new-sc-line-method-rs-private-getter-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-private-getter.js")]
    public Task test_elements_new_sc_line_method_rs_private_getter()
        => ExecutionTest("elements/new-sc-line-method-rs-private-getter");

    [Fact(DisplayName = "new-sc-line-method-rs-private-method-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_method_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-method-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-private-method.js")]
    public Task test_elements_new_sc_line_method_rs_private_method()
        => ExecutionTest("elements/new-sc-line-method-rs-private-method");

    [Fact(DisplayName = "new-sc-line-method-rs-private-setter-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-private-setter.js")]
    public Task test_elements_new_sc_line_method_rs_private_setter()
        => ExecutionTest("elements/new-sc-line-method-rs-private-setter");

    [Fact(DisplayName = "new-sc-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-method-rs-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-method-static-private-methods.js")]
    public Task test_elements_new_sc_line_method_static_private_methods()
        => ExecutionTest("elements/new-sc-line-method-static-private-methods");

    [Fact(DisplayName = "inst-private-escape-sequence-ZWJ.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_ZWJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-ZWJ");

    [Fact(DisplayName = "inst-private-escape-sequence-ZWNJ.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_ZWNJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-ZWNJ");

    [Fact(DisplayName = "inst-private-escape-sequence-u2118.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_u2118()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-u2118");

    [Fact(DisplayName = "inst-private-escape-sequence-u6F.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_u6F()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-u6F");

    [Fact(DisplayName = "inst-private-name-ZWJ.js")]
    public Task test_elements_private_accessor_name_inst_private_name_ZWJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-ZWJ");

    [Fact(DisplayName = "inst-private-name-ZWNJ.js")]
    public Task test_elements_private_accessor_name_inst_private_name_ZWNJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-ZWNJ");

    [Fact(DisplayName = "inst-private-name-common.js")]
    public Task test_elements_private_accessor_name_inst_private_name_common()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-common");

    [Fact(DisplayName = "inst-private-name-dollar.js")]
    public Task test_elements_private_accessor_name_inst_private_name_dollar()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-dollar");

    [Fact(DisplayName = "inst-private-name-u2118.js")]
    public Task test_elements_private_accessor_name_inst_private_name_u2118()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-u2118");

    [Fact(DisplayName = "inst-private-name-underscore.js")]
    public Task test_elements_private_accessor_name_inst_private_name_underscore()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-underscore");

    [Fact(DisplayName = "private-async-generator-method-name.js")]
    public Task test_elements_private_async_generator_method_name()
        => ExecutionTest("elements/private-async-generator-method-name");

    [Fact(DisplayName = "private-async-method-name.js")]
    public Task test_elements_private_async_method_name()
        => ExecutionTest("elements/private-async-method-name");

    [Fact(DisplayName = "private-field-as-arrow-function.js")]
    public Task test_elements_private_field_as_arrow_function()
        => ExecutionTest("elements/private-field-as-arrow-function");

    [Fact(DisplayName = "private-field-as-function.js")]
    public Task test_elements_private_field_as_function()
        => ExecutionTest("elements/private-field-as-function");

    [Fact(DisplayName = "private-field-is-not-clobbered-by-computed-property.js")]
    public Task test_elements_private_field_is_not_clobbered_by_computed_property()
        => ExecutionTest("elements/private-field-is-not-clobbered-by-computed-property");

    [Fact(DisplayName = "private-generator-method-name.js")]
    public Task test_elements_private_generator_method_name()
        => ExecutionTest("elements/private-generator-method-name");

    [Fact(DisplayName = "private-getter-brand-check-super-class.js")]
    public Task test_elements_private_getter_brand_check_super_class()
        => ExecutionTest("elements/private-getter-brand-check-super-class");

    [Fact(DisplayName = "private-getter-is-not-a-own-property.js")]
    public Task test_elements_private_getter_is_not_a_own_property()
        => ExecutionTest("elements/private-getter-is-not-a-own-property");

    [Fact(DisplayName = "private-getter-is-not-clobbered-by-computed-property.js")]
    public Task test_elements_private_getter_is_not_clobbered_by_computed_property()
        => ExecutionTest("elements/private-getter-is-not-clobbered-by-computed-property");

    [Fact(DisplayName = "private-method-brand-check-super-class.js")]
    public Task test_elements_private_method_brand_check_super_class()
        => ExecutionTest("elements/private-method-brand-check-super-class");

    [Fact(DisplayName = "private-method-comparison.js")]
    public Task test_elements_private_method_comparison()
        => ExecutionTest("elements/private-method-comparison");

    [Fact(DisplayName = "private-method-is-not-a-own-property.js")]
    public Task test_elements_private_method_is_not_a_own_property()
        => ExecutionTest("elements/private-method-is-not-a-own-property");

    [Fact(DisplayName = "private-method-is-not-clobbered-by-computed-property.js")]
    public Task test_elements_private_method_is_not_clobbered_by_computed_property()
        => ExecutionTest("elements/private-method-is-not-clobbered-by-computed-property");

    [Fact(DisplayName = "private-method-length.js")]
    public Task test_elements_private_method_length()
        => ExecutionTest("elements/private-method-length");

    [Fact(DisplayName = "private-method-not-writable.js")]
    public Task test_elements_private_method_not_writable()
        => ExecutionTest("elements/private-method-not-writable");

    [Fact(DisplayName = "private-setter-brand-check-super-class.js")]
    public Task test_elements_private_setter_brand_check_super_class()
        => ExecutionTest("elements/private-setter-brand-check-super-class");

    [Fact(DisplayName = "private-setter-is-not-a-own-property.js")]
    public Task test_elements_private_setter_is_not_a_own_property()
        => ExecutionTest("elements/private-setter-is-not-a-own-property");

    [Fact(DisplayName = "private-setter-is-not-clobbered-by-computed-property.js")]
    public Task test_elements_private_setter_is_not_clobbered_by_computed_property()
        => ExecutionTest("elements/private-setter-is-not-clobbered-by-computed-property");

    [Fact(DisplayName = "private-static-async-generator-method-name.js")]
    public Task test_elements_private_static_async_generator_method_name()
        => ExecutionTest("elements/private-static-async-generator-method-name");

    [Fact(DisplayName = "private-static-async-method-name.js")]
    public Task test_elements_private_static_async_method_name()
        => ExecutionTest("elements/private-static-async-method-name");

    [Fact(DisplayName = "private-static-generator-method-name.js")]
    public Task test_elements_private_static_generator_method_name()
        => ExecutionTest("elements/private-static-generator-method-name");

    [Fact(DisplayName = "private-static-getter-abrupt-completition.js")]
    public Task test_elements_private_static_getter_abrupt_completition()
        => ExecutionTest("elements/private-static-getter-abrupt-completition");

    [Fact(DisplayName = "private-static-method-length.js")]
    public Task test_elements_private_static_method_length()
        => ExecutionTest("elements/private-static-method-length");

    [Fact(DisplayName = "private-static-method-name.js")]
    public Task test_elements_private_static_method_name()
        => ExecutionTest("elements/private-static-method-name");

    [Fact(DisplayName = "private-static-method-not-writable.js")]
    public Task test_elements_private_static_method_not_writable()
        => ExecutionTest("elements/private-static-method-not-writable");

    [Fact(DisplayName = "private-static-setter-abrupt-completition.js")]
    public Task test_elements_private_static_setter_abrupt_completition()
        => ExecutionTest("elements/private-static-setter-abrupt-completition");

    [Fact(DisplayName = "privatefieldget-success-2.js")]
    public Task test_elements_privatefieldget_success_2()
        => ExecutionTest("elements/privatefieldget-success-2");

    [Fact(DisplayName = "privatefieldget-success-3.js")]
    public Task test_elements_privatefieldget_success_3()
        => ExecutionTest("elements/privatefieldget-success-3");

    [Fact(DisplayName = "privatefieldget-success-4.js")]
    public Task test_elements_privatefieldget_success_4()
        => ExecutionTest("elements/privatefieldget-success-4");

    [Fact(DisplayName = "privatefieldget-success-5.js")]
    public Task test_elements_privatefieldget_success_5()
        => ExecutionTest("elements/privatefieldget-success-5");

    [Fact(DisplayName = "privatefieldget-typeerror-4.js")]
    public Task test_elements_privatefieldget_typeerror_4()
        => ExecutionTest("elements/privatefieldget-typeerror-4");

    [Fact(DisplayName = "privatefieldget-typeerror-5.js")]
    public Task test_elements_privatefieldget_typeerror_5()
        => ExecutionTest("elements/privatefieldget-typeerror-5");

    [Fact(DisplayName = "privatefieldset-typeerror-4.js")]
    public Task test_elements_privatefieldset_typeerror_4()
        => ExecutionTest("elements/privatefieldset-typeerror-4");

    [Fact(DisplayName = "privatefieldset-typeerror-5.js")]
    public Task test_elements_privatefieldset_typeerror_5()
        => ExecutionTest("elements/privatefieldset-typeerror-5");

    [Fact(DisplayName = "public-class-field-initialization-is-visible-to-proxy.js")]
    public Task test_elements_public_class_field_initialization_is_visible_to_proxy()
        => ExecutionTest("elements/public-class-field-initialization-is-visible-to-proxy");

    [Fact(DisplayName = "redeclaration-symbol.js")]
    public Task test_elements_redeclaration_symbol()
        => ExecutionTest("elements/redeclaration-symbol");

    [Fact(DisplayName = "regular-definitions-computed-symbol-names.js")]
    public Task test_elements_regular_definitions_computed_symbol_names()
        => ExecutionTest("elements/regular-definitions-computed-symbol-names");

    [Fact(DisplayName = "regular-definitions-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_regular_definitions_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/regular-definitions-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "regular-definitions-private-field-usage.js")]
    public Task test_elements_regular_definitions_private_field_usage()
        => ExecutionTest("elements/regular-definitions-private-field-usage");

    [Fact(DisplayName = "regular-definitions-private-method-getter-usage.js")]
    public Task test_elements_regular_definitions_private_method_getter_usage()
        => ExecutionTest("elements/regular-definitions-private-method-getter-usage");

    [Fact(DisplayName = "regular-definitions-private-method-usage.js")]
    public Task test_elements_regular_definitions_private_method_usage()
        => ExecutionTest("elements/regular-definitions-private-method-usage");

    [Fact(DisplayName = "regular-definitions-private-names.js")]
    public Task test_elements_regular_definitions_private_names()
        => ExecutionTest("elements/regular-definitions-private-names");

    [Fact(DisplayName = "regular-definitions-rs-field-identifier-initializer.js")]
    public Task test_elements_regular_definitions_rs_field_identifier_initializer()
        => ExecutionTest("elements/regular-definitions-rs-field-identifier-initializer");

    [Fact(DisplayName = "regular-definitions-rs-field-identifier.js")]
    public Task test_elements_regular_definitions_rs_field_identifier()
        => ExecutionTest("elements/regular-definitions-rs-field-identifier");

    [Fact(DisplayName = "regular-definitions-rs-private-getter-alt.js")]
    public Task test_elements_regular_definitions_rs_private_getter_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-getter-alt");

    [Fact(DisplayName = "regular-definitions-rs-private-getter.js")]
    public Task test_elements_regular_definitions_rs_private_getter()
        => ExecutionTest("elements/regular-definitions-rs-private-getter");

    [Fact(DisplayName = "regular-definitions-rs-private-method-alt.js")]
    public Task test_elements_regular_definitions_rs_private_method_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-method-alt");

    [Fact(DisplayName = "regular-definitions-rs-private-method.js")]
    public Task test_elements_regular_definitions_rs_private_method()
        => ExecutionTest("elements/regular-definitions-rs-private-method");

    [Fact(DisplayName = "regular-definitions-rs-private-setter-alt.js")]
    public Task test_elements_regular_definitions_rs_private_setter_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-setter-alt");

    [Fact(DisplayName = "regular-definitions-rs-private-setter.js")]
    public Task test_elements_regular_definitions_rs_private_setter()
        => ExecutionTest("elements/regular-definitions-rs-private-setter");

    [Fact(DisplayName = "regular-definitions-rs-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-alt");

    [Fact(DisplayName = "regular-definitions-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "regular-definitions-rs-privatename-identifier-initializer.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "regular-definitions-rs-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier");

    [Fact(DisplayName = "regular-definitions-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "regular-definitions-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "regular-definitions-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "regular-definitions-rs-static-method-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "regular-definitions-static-private-methods.js")]
    public Task test_elements_regular_definitions_static_private_methods()
        => ExecutionTest("elements/regular-definitions-static-private-methods");

    [Fact(DisplayName = "same-line-gen-computed-symbol-names.js")]
    public Task test_elements_same_line_gen_computed_symbol_names()
        => ExecutionTest("elements/same-line-gen-computed-symbol-names");

    [Fact(DisplayName = "same-line-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_same_line_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/same-line-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "same-line-gen-private-field-usage.js")]
    public Task test_elements_same_line_gen_private_field_usage()
        => ExecutionTest("elements/same-line-gen-private-field-usage");

    [Fact(DisplayName = "same-line-gen-private-method-getter-usage.js")]
    public Task test_elements_same_line_gen_private_method_getter_usage()
        => ExecutionTest("elements/same-line-gen-private-method-getter-usage");

    [Fact(DisplayName = "same-line-gen-private-method-usage.js")]
    public Task test_elements_same_line_gen_private_method_usage()
        => ExecutionTest("elements/same-line-gen-private-method-usage");

    [Fact(DisplayName = "same-line-gen-private-names.js")]
    public Task test_elements_same_line_gen_private_names()
        => ExecutionTest("elements/same-line-gen-private-names");

    [Fact(DisplayName = "same-line-gen-rs-field-identifier-initializer.js")]
    public Task test_elements_same_line_gen_rs_field_identifier_initializer()
        => ExecutionTest("elements/same-line-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "same-line-gen-rs-field-identifier.js")]
    public Task test_elements_same_line_gen_rs_field_identifier()
        => ExecutionTest("elements/same-line-gen-rs-field-identifier");

    [Fact(DisplayName = "same-line-gen-rs-private-getter-alt.js")]
    public Task test_elements_same_line_gen_rs_private_getter_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-getter-alt");

    [Fact(DisplayName = "same-line-gen-rs-private-getter.js")]
    public Task test_elements_same_line_gen_rs_private_getter()
        => ExecutionTest("elements/same-line-gen-rs-private-getter");

    [Fact(DisplayName = "same-line-gen-rs-private-method-alt.js")]
    public Task test_elements_same_line_gen_rs_private_method_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-method-alt");

    [Fact(DisplayName = "same-line-gen-rs-private-method.js")]
    public Task test_elements_same_line_gen_rs_private_method()
        => ExecutionTest("elements/same-line-gen-rs-private-method");

    [Fact(DisplayName = "same-line-gen-rs-private-setter-alt.js")]
    public Task test_elements_same_line_gen_rs_private_setter_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-setter-alt");

    [Fact(DisplayName = "same-line-gen-rs-private-setter.js")]
    public Task test_elements_same_line_gen_rs_private_setter()
        => ExecutionTest("elements/same-line-gen-rs-private-setter");

    [Fact(DisplayName = "same-line-gen-rs-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "same-line-gen-rs-privatename-identifier-initializer.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "same-line-gen-rs-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier");

    [Fact(DisplayName = "same-line-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "same-line-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-gen-rs-static-method-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "same-line-gen-static-private-methods.js")]
    public Task test_elements_same_line_gen_static_private_methods()
        => ExecutionTest("elements/same-line-gen-static-private-methods");

    [Fact(DisplayName = "same-line-method-computed-symbol-names.js")]
    public Task test_elements_same_line_method_computed_symbol_names()
        => ExecutionTest("elements/same-line-method-computed-symbol-names");

    [Fact(DisplayName = "same-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_same_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/same-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "same-line-method-private-field-usage.js")]
    public Task test_elements_same_line_method_private_field_usage()
        => ExecutionTest("elements/same-line-method-private-field-usage");

    [Fact(DisplayName = "same-line-method-private-method-getter-usage.js")]
    public Task test_elements_same_line_method_private_method_getter_usage()
        => ExecutionTest("elements/same-line-method-private-method-getter-usage");

    [Fact(DisplayName = "same-line-method-private-method-usage.js")]
    public Task test_elements_same_line_method_private_method_usage()
        => ExecutionTest("elements/same-line-method-private-method-usage");

    [Fact(DisplayName = "same-line-method-private-names.js")]
    public Task test_elements_same_line_method_private_names()
        => ExecutionTest("elements/same-line-method-private-names");

    [Fact(DisplayName = "same-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_same_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/same-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "same-line-method-rs-field-identifier.js")]
    public Task test_elements_same_line_method_rs_field_identifier()
        => ExecutionTest("elements/same-line-method-rs-field-identifier");

    [Fact(DisplayName = "same-line-method-rs-private-getter-alt.js")]
    public Task test_elements_same_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/same-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "same-line-method-rs-private-getter.js")]
    public Task test_elements_same_line_method_rs_private_getter()
        => ExecutionTest("elements/same-line-method-rs-private-getter");

    [Fact(DisplayName = "same-line-method-rs-private-method-alt.js")]
    public Task test_elements_same_line_method_rs_private_method_alt()
        => ExecutionTest("elements/same-line-method-rs-private-method-alt");

    [Fact(DisplayName = "same-line-method-rs-private-method.js")]
    public Task test_elements_same_line_method_rs_private_method()
        => ExecutionTest("elements/same-line-method-rs-private-method");

    [Fact(DisplayName = "same-line-method-rs-private-setter-alt.js")]
    public Task test_elements_same_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/same-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "same-line-method-rs-private-setter.js")]
    public Task test_elements_same_line_method_rs_private_setter()
        => ExecutionTest("elements/same-line-method-rs-private-setter");

    [Fact(DisplayName = "same-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "same-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "same-line-method-rs-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "same-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "same-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "same-line-method-static-private-methods.js")]
    public Task test_elements_same_line_method_static_private_methods()
        => ExecutionTest("elements/same-line-method-static-private-methods");

    [Fact(DisplayName = "set-access-of-missing-private-setter.js")]
    public Task test_elements_set_access_of_missing_private_setter()
        => ExecutionTest("elements/set-access-of-missing-private-setter");

    [Fact(DisplayName = "set-access-of-missing-private-static-setter.js")]
    public Task test_elements_set_access_of_missing_private_static_setter()
        => ExecutionTest("elements/set-access-of-missing-private-static-setter");

    [Fact(DisplayName = "set-access-of-private-method.js")]
    public Task test_elements_set_access_of_private_method()
        => ExecutionTest("elements/set-access-of-private-method");

    [Fact(DisplayName = "static-field-initializer-error.js")]
    public Task test_elements_static_field_initializer_error()
        => ExecutionTest("elements/static-field-initializer-error");

    [Fact(DisplayName = "static-fielddefinition-initializer-abrupt-completion.js")]
    public Task test_elements_static_fielddefinition_initializer_abrupt_completion()
        => ExecutionTest("elements/static-fielddefinition-initializer-abrupt-completion");

    [Fact(DisplayName = "static-private-getter.js")]
    public Task test_elements_static_private_getter()
        => ExecutionTest("elements/static-private-getter");

    [Fact(DisplayName = "static-private-method-and-instance-method-brand-check.js")]
    public Task test_elements_static_private_method_and_instance_method_brand_check()
        => ExecutionTest("elements/static-private-method-and-instance-method-brand-check");

    [Fact(DisplayName = "static-private-method-subclass-receiver.js")]
    public Task test_elements_static_private_method_subclass_receiver()
        => ExecutionTest("elements/static-private-method-subclass-receiver");

    [Fact(DisplayName = "super-access-inside-a-private-getter.js")]
    public Task test_elements_super_access_inside_a_private_getter()
        => ExecutionTest("elements/super-access-inside-a-private-getter");

    [Fact(DisplayName = "super-access-inside-a-private-setter.js")]
    public Task test_elements_super_access_inside_a_private_setter()
        => ExecutionTest("elements/super-access-inside-a-private-setter");

    [Fact(DisplayName = "super-fielddefinition-initializer-abrupt-completion.js")]
    public Task test_elements_super_fielddefinition_initializer_abrupt_completion()
        => ExecutionTest("elements/super-fielddefinition-initializer-abrupt-completion");

    [Fact(DisplayName = "grammar-class-body-ctor-no-heritage.js")]
    public Task test_elements_syntax_valid_grammar_class_body_ctor_no_heritage()
        => ExecutionTest("elements/syntax/valid/grammar-class-body-ctor-no-heritage");

    [Fact(DisplayName = "grammar-field-classelementname-initializer-alt.js")]
    public Task test_elements_syntax_valid_grammar_field_classelementname_initializer_alt()
        => ExecutionTest("elements/syntax/valid/grammar-field-classelementname-initializer-alt");

    [Fact(DisplayName = "grammar-field-classelementname-initializer.js")]
    public Task test_elements_syntax_valid_grammar_field_classelementname_initializer()
        => ExecutionTest("elements/syntax/valid/grammar-field-classelementname-initializer");

    [Fact(DisplayName = "grammar-field-identifier-alt.js")]
    public Task test_elements_syntax_valid_grammar_field_identifier_alt()
        => ExecutionTest("elements/syntax/valid/grammar-field-identifier-alt");

    [Fact(DisplayName = "grammar-field-identifier.js")]
    public Task test_elements_syntax_valid_grammar_field_identifier()
        => ExecutionTest("elements/syntax/valid/grammar-field-identifier");

    [Fact(DisplayName = "grammar-fields-multi-line.js")]
    public Task test_elements_syntax_valid_grammar_fields_multi_line()
        => ExecutionTest("elements/syntax/valid/grammar-fields-multi-line");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-get-set.js")]
    public Task test_elements_syntax_valid_grammar_privatemeth_duplicate_get_set()
        => ExecutionTest("elements/syntax/valid/grammar-privatemeth-duplicate-get-set");

    [Fact(DisplayName = "grammar-privatemeth-duplicate-meth-nestedclassmeth.js")]
    public Task test_elements_syntax_valid_grammar_privatemeth_duplicate_meth_nestedclassmeth()
        => ExecutionTest("elements/syntax/valid/grammar-privatemeth-duplicate-meth-nestedclassmeth");

    [Fact(DisplayName = "grammar-privatename-classelementname-initializer-alt.js")]
    public Task test_elements_syntax_valid_grammar_privatename_classelementname_initializer_alt()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-classelementname-initializer-alt");

    [Fact(DisplayName = "grammar-privatename-classelementname-initializer.js")]
    public Task test_elements_syntax_valid_grammar_privatename_classelementname_initializer()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-classelementname-initializer");

    [Fact(DisplayName = "grammar-privatename-identifier.js")]
    public Task test_elements_syntax_valid_grammar_privatename_identifier()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-identifier");

    [Fact(DisplayName = "grammar-privatename-no-initializer-with-method.js")]
    public Task test_elements_syntax_valid_grammar_privatename_no_initializer_with_method()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-no-initializer-with-method");

    [Fact(DisplayName = "grammar-privatenames-multi-line.js")]
    public Task test_elements_syntax_valid_grammar_privatenames_multi_line()
        => ExecutionTest("elements/syntax/valid/grammar-privatenames-multi-line");

    [Fact(DisplayName = "grammar-special-prototype-accessor-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_accessor_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-accessor-meth-valid");

    [Fact(DisplayName = "grammar-special-prototype-async-gen-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_async_gen_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-async-gen-meth-valid");

    [Fact(DisplayName = "grammar-special-prototype-gen-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_gen_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-gen-meth-valid");

    [Fact(DisplayName = "grammar-special-prototype-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-meth-valid");

    [Fact(DisplayName = "grammar-static-private-async-gen-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_async_gen_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-async-gen-meth-prototype");

    [Fact(DisplayName = "grammar-static-private-async-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_async_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-async-meth-prototype");

    [Fact(DisplayName = "grammar-static-private-gen-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_gen_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-gen-meth-prototype");

    [Fact(DisplayName = "grammar-static-private-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-meth-prototype");

    [Fact(DisplayName = "wrapped-in-sc-computed-symbol-names.js")]
    public Task test_elements_wrapped_in_sc_computed_symbol_names()
        => ExecutionTest("elements/wrapped-in-sc-computed-symbol-names");

    [Fact(DisplayName = "wrapped-in-sc-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_wrapped_in_sc_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/wrapped-in-sc-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "wrapped-in-sc-private-field-usage.js")]
    public Task test_elements_wrapped_in_sc_private_field_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-field-usage");

    [Fact(DisplayName = "wrapped-in-sc-private-method-getter-usage.js")]
    public Task test_elements_wrapped_in_sc_private_method_getter_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-method-getter-usage");

    [Fact(DisplayName = "wrapped-in-sc-private-method-usage.js")]
    public Task test_elements_wrapped_in_sc_private_method_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-method-usage");

    [Fact(DisplayName = "wrapped-in-sc-private-names.js")]
    public Task test_elements_wrapped_in_sc_private_names()
        => ExecutionTest("elements/wrapped-in-sc-private-names");

    [Fact(DisplayName = "wrapped-in-sc-rs-field-identifier-initializer.js")]
    public Task test_elements_wrapped_in_sc_rs_field_identifier_initializer()
        => ExecutionTest("elements/wrapped-in-sc-rs-field-identifier-initializer");

    [Fact(DisplayName = "wrapped-in-sc-rs-field-identifier.js")]
    public Task test_elements_wrapped_in_sc_rs_field_identifier()
        => ExecutionTest("elements/wrapped-in-sc-rs-field-identifier");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-getter-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_getter_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-getter-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-getter.js")]
    public Task test_elements_wrapped_in_sc_rs_private_getter()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-getter");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-method-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_method_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-method-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-method.js")]
    public Task test_elements_wrapped_in_sc_rs_private_method()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-method");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-setter-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_private_setter_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-setter-alt");

    [Fact(DisplayName = "wrapped-in-sc-rs-private-setter.js")]
    public Task test_elements_wrapped_in_sc_rs_private_setter()
        => ExecutionTest("elements/wrapped-in-sc-rs-private-setter");

    [Fact(DisplayName = "wrapped-in-sc-rs-privatename-identifier-alt.js")]
    public Task test_elements_wrapped_in_sc_rs_privatename_identifier_alt()
        => ExecutionTest("elements/wrapped-in-sc-rs-privatename-identifier-alt");
}
