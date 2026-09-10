using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.dstr;

public class ClassDeclarationDestructuringConformance9BatchExecutionTests : FileSystemExecutionTestsBase
{
    public ClassDeclarationDestructuringConformance9BatchExecutionTests()
        : base("language/statements/class/dstr", "language.statements.class_.dstr") { }

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-close.js")]
    public Task meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task meth_static_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("meth-static-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-get-err.js")]
    public Task meth_static_dflt_ary_init_iter_get_err()
        => ExecutionTest("meth-static-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-static-dflt-ary-init-iter-no-close.js")]
    public Task meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-static-dflt-ary-name-iter-val.js")]
    public Task meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task meth_static_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task meth_static_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision-step-err.js")]
    public Task meth_static_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-elision.js")]
    public Task meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-empty.js")]
    public Task meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-static-dflt-obj-init-null.js")]
    public Task meth_static_dflt_obj_init_null()
        => ExecutionTest("meth-static-dflt-obj-init-null");

    [Fact(DisplayName = "meth-static-dflt-obj-init-undefined.js")]
    public Task meth_static_dflt_obj_init_undefined()
        => ExecutionTest("meth-static-dflt-obj-init-undefined");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-empty.js")]
    public Task meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-get-value-err.js")]
    public Task meth_static_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-throws.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task meth_static_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-list-err.js")]
    public Task meth_static_dflt_obj_ptrn_list_err()
        => ExecutionTest("meth-static-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task meth_static_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-eval-err.js")]
    public Task meth_static_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task meth_static_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task meth_static_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-ary-init-iter-close.js")]
    public Task private_gen_meth_ary_init_iter_close()
        => ExecutionTest("private-gen-meth-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-ary-init-iter-no-close.js")]
    public Task private_gen_meth_ary_init_iter_no_close()
        => ExecutionTest("private-gen-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-ary-name-iter-val.js")]
    public Task private_gen_meth_ary_name_iter_val()
        => ExecutionTest("private-gen-meth-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_gen_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task private_gen_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-id.js")]
    public Task private_gen_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_gen_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_gen_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-gen-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elision-exhausted.js")]
    public Task private_gen_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-gen-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-elision.js")]
    public Task private_gen_meth_ary_ptrn_elision()
        => ExecutionTest("private-gen-meth-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-empty.js")]
    public Task private_gen_meth_ary_ptrn_empty()
        => ExecutionTest("private-gen-meth-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-elem.js")]
    public Task private_gen_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-elision.js")]
    public Task private_gen_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-empty.js")]
    public Task private_gen_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-ary-rest.js")]
    public Task private_gen_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-direct.js")]
    public Task private_gen_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-elision.js")]
    public Task private_gen_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task private_gen_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-id.js")]
    public Task private_gen_meth_ary_ptrn_rest_id()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-obj-id.js")]
    public Task private_gen_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_gen_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-gen-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-init-iter-close.js")]
    public Task private_gen_meth_dflt_ary_init_iter_close()
        => ExecutionTest("private-gen-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-init-iter-no-close.js")]
    public Task private_gen_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("private-gen-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-name-iter-val.js")]
    public Task private_gen_meth_dflt_ary_name_iter_val()
        => ExecutionTest("private-gen-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-elision.js")]
    public Task private_gen_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-empty.js")]
    public Task private_gen_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-id.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-gen-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-empty.js")]
    public Task private_gen_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task private_gen_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-id.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task private_gen_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task private_gen_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_gen_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task private_gen_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-gen-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-empty.js")]
    public Task private_gen_meth_obj_ptrn_empty()
        => ExecutionTest("private-gen-meth-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_gen_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_gen_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_gen_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_gen_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-init-skipped.js")]
    public Task private_gen_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-id-trailing-comma.js")]
    public Task private_gen_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-gen-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_gen_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-gen-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_gen_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-gen-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-init.js")]
    public Task private_gen_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("private-gen-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_gen_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-gen-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-id.js")]
    public Task private_gen_meth_obj_ptrn_prop_id()
        => ExecutionTest("private-gen-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-getter.js")]
    public Task private_gen_meth_obj_ptrn_rest_getter()
        => ExecutionTest("private-gen-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_gen_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-gen-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-rest-val-obj.js")]
    public Task private_gen_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-gen-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-static-ary-init-iter-close.js")]
    public Task private_gen_meth_static_ary_init_iter_close()
        => ExecutionTest("private-gen-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-static-ary-init-iter-no-close.js")]
    public Task private_gen_meth_static_ary_init_iter_no_close()
        => ExecutionTest("private-gen-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-static-ary-name-iter-val.js")]
    public Task private_gen_meth_static_ary_name_iter_val()
        => ExecutionTest("private-gen-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_gen_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task private_gen_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-elision.js")]
    public Task private_gen_meth_static_ary_ptrn_elision()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-empty.js")]
    public Task private_gen_meth_static_ary_ptrn_empty()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-id.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-obj-id.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_gen_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-gen-meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-init-iter-close.js")]
    public Task private_gen_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("private-gen-meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-init-iter-no-close.js")]
    public Task private_gen_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("private-gen-meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-name-iter-val.js")]
    public Task private_gen_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("private-gen-meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-elision.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-empty.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-gen-meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-empty.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task private_gen_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-gen-meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-empty.js")]
    public Task private_gen_meth_static_obj_ptrn_empty()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-init-skipped.js")]
    public Task private_gen_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task private_gen_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_gen_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_gen_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-init.js")]
    public Task private_gen_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_gen_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-id.js")]
    public Task private_gen_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-getter.js")]
    public Task private_gen_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_gen_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-rest-val-obj.js")]
    public Task private_gen_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-gen-meth-static-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-meth-dflt-ary-init-iter-close.js")]
    public Task private_meth_dflt_ary_init_iter_close()
        => ExecutionTest("private-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-meth-dflt-ary-init-iter-no-close.js")]
    public Task private_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("private-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-meth-dflt-ary-name-iter-val.js")]
    public Task private_meth_dflt_ary_name_iter_val()
        => ExecutionTest("private-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task private_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task private_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task private_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-elision.js")]
    public Task private_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("private-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-empty.js")]
    public Task private_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("private-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task private_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task private_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task private_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task private_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task private_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task private_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task private_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-id.js")]
    public Task private_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task private_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-empty.js")]
    public Task private_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("private-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task private_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task private_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task private_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task private_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task private_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-id.js")]
    public Task private_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task private_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task private_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("private-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task private_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("private-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task private_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-meth-static-ary-init-iter-close.js")]
    public Task private_meth_static_ary_init_iter_close()
        => ExecutionTest("private-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "private-meth-static-ary-init-iter-no-close.js")]
    public Task private_meth_static_ary_init_iter_no_close()
        => ExecutionTest("private-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "private-meth-static-ary-name-iter-val.js")]
    public Task private_meth_static_ary_name_iter_val()
        => ExecutionTest("private-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task private_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task private_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task private_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task private_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task private_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task private_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-elision.js")]
    public Task private_meth_static_ary_ptrn_elision()
        => ExecutionTest("private-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-empty.js")]
    public Task private_meth_static_ary_ptrn_empty()
        => ExecutionTest("private-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task private_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task private_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task private_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task private_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task private_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task private_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task private_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-id.js")]
    public Task private_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-obj-id.js")]
    public Task private_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-meth-static-dflt-ary-init-iter-close.js")]
    public Task private_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("private-meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "private-meth-static-dflt-ary-init-iter-no-close.js")]
    public Task private_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("private-meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "private-meth-static-dflt-ary-name-iter-val.js")]
    public Task private_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("private-meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task private_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task private_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-elision.js")]
    public Task private_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-empty.js")]
    public Task private_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task private_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("private-meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-empty.js")]
    public Task private_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task private_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task private_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task private_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task private_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "private-meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task private_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("private-meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-empty.js")]
    public Task private_meth_static_obj_ptrn_empty()
        => ExecutionTest("private-meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task private_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task private_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task private_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task private_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task private_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-init-skipped.js")]
    public Task private_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("private-meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task private_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("private-meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task private_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task private_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-id-init.js")]
    public Task private_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task private_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-id.js")]
    public Task private_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-rest-getter.js")]
    public Task private_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("private-meth-static-obj-ptrn-rest-getter");
}
