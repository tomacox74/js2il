using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.for_;

public class ForDestructuringConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public ForDestructuringConformanceBatchExecutionTests()
        : base("language/statements/for", "language.statements.for_") { }

    [Fact(DisplayName = "const-ary-init-iter-get-err.js")]
    public Task test_const_ary_init_iter_get_err()
        => ExecutionTest("dstr/const-ary-init-iter-get-err");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_const_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_const_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_const_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-val-null.js")]
    public Task test_const_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/const-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_const_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_const_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_const_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_const_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_const_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_const_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-hole.js")]
    public Task test_const_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_const_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-throws.js")]
    public Task test_const_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-undef.js")]
    public Task test_const_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_const_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_const_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-done.js")]
    public Task test_const_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_const_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_const_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_const_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "const-ary-ptrn-elem-id-iter-val.js")]
    public Task test_const_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/const-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-id-init.js")]
    public Task test_const_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-id.js")]
    public Task test_const_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_const_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_const_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-val-null.js")]
    public Task test_const_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "const-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_const_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/const-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "const-ary-ptrn-elision-exhausted.js")]
    public Task test_const_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/const-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "const-ary-ptrn-elision-iter-close.js")]
    public Task test_const_ary_ptrn_elision_iter_close()
        => ExecutionTest("dstr/const-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "const-ary-ptrn-elision-step-err.js")]
    public Task test_const_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/const-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "const-ary-ptrn-elision.js")]
    public Task test_const_ary_ptrn_elision()
        => ExecutionTest("dstr/const-ary-ptrn-elision");

    [Fact(DisplayName = "const-ary-ptrn-empty.js")]
    public Task test_const_ary_ptrn_empty()
        => ExecutionTest("dstr/const-ary-ptrn-empty");

    [Fact(DisplayName = "const-ary-ptrn-rest-ary-elem.js")]
    public Task test_const_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "const-ary-ptrn-rest-ary-elision.js")]
    public Task test_const_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "const-ary-ptrn-rest-ary-empty.js")]
    public Task test_const_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "const-ary-ptrn-rest-ary-rest.js")]
    public Task test_const_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/const-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-direct.js")]
    public Task test_const_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_const_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-elision.js")]
    public Task test_const_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-exhausted.js")]
    public Task test_const_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-iter-close.js")]
    public Task test_const_ary_ptrn_rest_id_iter_close()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_const_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "const-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_const_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "const-ary-ptrn-rest-id.js")]
    public Task test_const_ary_ptrn_rest_id()
        => ExecutionTest("dstr/const-ary-ptrn-rest-id");

    [Fact(DisplayName = "const-ary-ptrn-rest-obj-id.js")]
    public Task test_const_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/const-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "const-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_const_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/const-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "const-obj-init-null.js")]
    public Task test_const_obj_init_null()
        => ExecutionTest("dstr/const-obj-init-null");

    [Fact(DisplayName = "const-obj-init-undefined.js")]
    public Task test_const_obj_init_undefined()
        => ExecutionTest("dstr/const-obj-init-undefined");

    [Fact(DisplayName = "const-obj-ptrn-empty.js")]
    public Task test_const_obj_ptrn_empty()
        => ExecutionTest("dstr/const-obj-ptrn-empty");

    [Fact(DisplayName = "const-obj-ptrn-id-get-value-err.js")]
    public Task test_const_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/const-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "const-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_const_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "const-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_const_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "const-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_const_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "const-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_const_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "const-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_const_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "const-obj-ptrn-id-init-skipped.js")]
    public Task test_const_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "const-obj-ptrn-id-init-throws.js")]
    public Task test_const_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "const-obj-ptrn-id-init-unresolvable.js")]
    public Task test_const_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "const-obj-ptrn-id-trailing-comma.js")]
    public Task test_const_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/const-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "const-obj-ptrn-list-err.js")]
    public Task test_const_obj_ptrn_list_err()
        => ExecutionTest("dstr/const-obj-ptrn-list-err");

    [Fact(DisplayName = "const-obj-ptrn-prop-ary-init.js")]
    public Task test_const_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "const-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_const_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "const-obj-ptrn-prop-ary-value-null.js")]
    public Task test_const_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "const-obj-ptrn-prop-ary.js")]
    public Task test_const_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary");

    [Fact(DisplayName = "const-obj-ptrn-prop-eval-err.js")]
    public Task test_const_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/const-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_const_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_const_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-init-throws.js")]
    public Task test_const_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_const_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-init.js")]
    public Task test_const_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "const-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_const_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "const-obj-ptrn-prop-id.js")]
    public Task test_const_obj_ptrn_prop_id()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id");

    [Fact(DisplayName = "const-obj-ptrn-prop-obj-init.js")]
    public Task test_const_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "const-obj-ptrn-prop-obj-value-null.js")]
    public Task test_const_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "const-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_const_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "const-obj-ptrn-prop-obj.js")]
    public Task test_const_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj");

    [Fact(DisplayName = "const-obj-ptrn-rest-getter.js")]
    public Task test_const_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/const-obj-ptrn-rest-getter");

    [Fact(DisplayName = "const-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_const_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/const-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "const-obj-ptrn-rest-val-obj.js")]
    public Task test_const_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/const-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "let-ary-init-iter-close.js")]
    public Task test_let_ary_init_iter_close()
        => ExecutionTest("dstr/let-ary-init-iter-close");

    [Fact(DisplayName = "let-ary-init-iter-get-err-array-prototype.js")]
    public Task test_let_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/let-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "let-ary-init-iter-get-err.js")]
    public Task test_let_ary_init_iter_get_err()
        => ExecutionTest("dstr/let-ary-init-iter-get-err");

    [Fact(DisplayName = "let-ary-init-iter-no-close.js")]
    public Task test_let_ary_init_iter_no_close()
        => ExecutionTest("dstr/let-ary-init-iter-no-close");

    [Fact(DisplayName = "let-ary-name-iter-val.js")]
    public Task test_let_ary_name_iter_val()
        => ExecutionTest("dstr/let-ary-name-iter-val");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_let_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_let_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_let_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_let_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_let_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_let_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_let_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_let_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "let-ary-ptrn-elem-ary-val-null.js")]
    public Task test_let_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_let_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_let_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_let_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_let_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_let_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_let_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-hole.js")]
    public Task test_let_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_let_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-throws.js")]
    public Task test_let_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-undef.js")]
    public Task test_let_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_let_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_let_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-done.js")]
    public Task test_let_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_let_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_let_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_let_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "let-ary-ptrn-elem-id-iter-val.js")]
    public Task test_let_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-id-init.js")]
    public Task test_let_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-id.js")]
    public Task test_let_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_let_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_let_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-val-null.js")]
    public Task test_let_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "let-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_let_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "let-ary-ptrn-elision-exhausted.js")]
    public Task test_let_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/let-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "let-ary-ptrn-elision-iter-close.js")]
    public Task test_let_ary_ptrn_elision_iter_close()
        => ExecutionTest("dstr/let-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "let-ary-ptrn-elision-step-err.js")]
    public Task test_let_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/let-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "let-ary-ptrn-elision.js")]
    public Task test_let_ary_ptrn_elision()
        => ExecutionTest("dstr/let-ary-ptrn-elision");

    [Fact(DisplayName = "let-ary-ptrn-empty.js")]
    public Task test_let_ary_ptrn_empty()
        => ExecutionTest("dstr/let-ary-ptrn-empty");

    [Fact(DisplayName = "let-ary-ptrn-rest-ary-elem.js")]
    public Task test_let_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "let-ary-ptrn-rest-ary-elision.js")]
    public Task test_let_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "let-ary-ptrn-rest-ary-empty.js")]
    public Task test_let_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "let-ary-ptrn-rest-ary-rest.js")]
    public Task test_let_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-direct.js")]
    public Task test_let_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_let_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-elision.js")]
    public Task test_let_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-exhausted.js")]
    public Task test_let_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-iter-close.js")]
    public Task test_let_ary_ptrn_rest_id_iter_close()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_let_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "let-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_let_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "let-ary-ptrn-rest-id.js")]
    public Task test_let_ary_ptrn_rest_id()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id");

    [Fact(DisplayName = "let-ary-ptrn-rest-obj-id.js")]
    public Task test_let_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/let-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "let-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_let_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/let-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "let-obj-init-null.js")]
    public Task test_let_obj_init_null()
        => ExecutionTest("dstr/let-obj-init-null");

    [Fact(DisplayName = "let-obj-init-undefined.js")]
    public Task test_let_obj_init_undefined()
        => ExecutionTest("dstr/let-obj-init-undefined");

    [Fact(DisplayName = "let-obj-ptrn-empty.js")]
    public Task test_let_obj_ptrn_empty()
        => ExecutionTest("dstr/let-obj-ptrn-empty");

    [Fact(DisplayName = "let-obj-ptrn-id-get-value-err.js")]
    public Task test_let_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/let-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "let-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_let_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "let-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_let_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "let-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_let_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "let-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_let_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "let-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_let_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "let-obj-ptrn-id-init-skipped.js")]
    public Task test_let_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "let-obj-ptrn-id-init-throws.js")]
    public Task test_let_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "let-obj-ptrn-id-init-unresolvable.js")]
    public Task test_let_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "let-obj-ptrn-id-trailing-comma.js")]
    public Task test_let_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/let-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "let-obj-ptrn-list-err.js")]
    public Task test_let_obj_ptrn_list_err()
        => ExecutionTest("dstr/let-obj-ptrn-list-err");

    [Fact(DisplayName = "let-obj-ptrn-prop-ary-init.js")]
    public Task test_let_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "let-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_let_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "let-obj-ptrn-prop-ary-value-null.js")]
    public Task test_let_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "let-obj-ptrn-prop-ary.js")]
    public Task test_let_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary");

    [Fact(DisplayName = "let-obj-ptrn-prop-eval-err.js")]
    public Task test_let_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/let-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_let_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_let_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-init-throws.js")]
    public Task test_let_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_let_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-init.js")]
    public Task test_let_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "let-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_let_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "let-obj-ptrn-prop-id.js")]
    public Task test_let_obj_ptrn_prop_id()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id");

    [Fact(DisplayName = "let-obj-ptrn-prop-obj-init.js")]
    public Task test_let_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "let-obj-ptrn-prop-obj-value-null.js")]
    public Task test_let_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "let-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_let_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "let-obj-ptrn-prop-obj.js")]
    public Task test_let_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj");

    [Fact(DisplayName = "let-obj-ptrn-rest-getter.js")]
    public Task test_let_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/let-obj-ptrn-rest-getter");

    [Fact(DisplayName = "let-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_let_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/let-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "let-obj-ptrn-rest-val-obj.js")]
    public Task test_let_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/let-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "var-ary-init-iter-close.js")]
    public Task test_var_ary_init_iter_close()
        => ExecutionTest("dstr/var-ary-init-iter-close");

    [Fact(DisplayName = "var-ary-init-iter-get-err-array-prototype.js")]
    public Task test_var_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/var-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "var-ary-init-iter-get-err.js")]
    public Task test_var_ary_init_iter_get_err()
        => ExecutionTest("dstr/var-ary-init-iter-get-err");

    [Fact(DisplayName = "var-ary-init-iter-no-close.js")]
    public Task test_var_ary_init_iter_no_close()
        => ExecutionTest("dstr/var-ary-init-iter-no-close");

    [Fact(DisplayName = "var-ary-name-iter-val.js")]
    public Task test_var_ary_name_iter_val()
        => ExecutionTest("dstr/var-ary-name-iter-val");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_var_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_var_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_var_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_var_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_var_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_var_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_var_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_var_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "var-ary-ptrn-elem-ary-val-null.js")]
    public Task test_var_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_var_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_var_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_var_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_var_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_var_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_var_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-hole.js")]
    public Task test_var_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_var_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-throws.js")]
    public Task test_var_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-undef.js")]
    public Task test_var_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task test_var_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_var_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-done.js")]
    public Task test_var_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-step-err.js")]
    public Task test_var_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_var_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-val-err.js")]
    public Task test_var_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "var-ary-ptrn-elem-id-iter-val.js")]
    public Task test_var_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-id-init.js")]
    public Task test_var_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-id.js")]
    public Task test_var_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_var_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_var_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-val-null.js")]
    public Task test_var_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "var-ary-ptrn-elem-obj-val-undef.js")]
    public Task test_var_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "var-ary-ptrn-elision-exhausted.js")]
    public Task test_var_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/var-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "var-ary-ptrn-elision-iter-close.js")]
    public Task test_var_ary_ptrn_elision_iter_close()
        => ExecutionTest("dstr/var-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "var-ary-ptrn-elision-step-err.js")]
    public Task test_var_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/var-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "var-ary-ptrn-elision.js")]
    public Task test_var_ary_ptrn_elision()
        => ExecutionTest("dstr/var-ary-ptrn-elision");

    [Fact(DisplayName = "var-ary-ptrn-empty.js")]
    public Task test_var_ary_ptrn_empty()
        => ExecutionTest("dstr/var-ary-ptrn-empty");

    [Fact(DisplayName = "var-ary-ptrn-rest-ary-elem.js")]
    public Task test_var_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "var-ary-ptrn-rest-ary-elision.js")]
    public Task test_var_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "var-ary-ptrn-rest-ary-empty.js")]
    public Task test_var_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "var-ary-ptrn-rest-ary-rest.js")]
    public Task test_var_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-direct.js")]
    public Task test_var_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-elision-next-err.js")]
    public Task test_var_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-elision.js")]
    public Task test_var_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-exhausted.js")]
    public Task test_var_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-iter-close.js")]
    public Task test_var_ary_ptrn_rest_id_iter_close()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-iter-step-err.js")]
    public Task test_var_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "var-ary-ptrn-rest-id-iter-val-err.js")]
    public Task test_var_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "var-ary-ptrn-rest-id.js")]
    public Task test_var_ary_ptrn_rest_id()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id");

    [Fact(DisplayName = "var-ary-ptrn-rest-obj-id.js")]
    public Task test_var_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/var-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "var-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_var_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/var-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "var-obj-init-null.js")]
    public Task test_var_obj_init_null()
        => ExecutionTest("dstr/var-obj-init-null");

    [Fact(DisplayName = "var-obj-init-undefined.js")]
    public Task test_var_obj_init_undefined()
        => ExecutionTest("dstr/var-obj-init-undefined");

    [Fact(DisplayName = "var-obj-ptrn-empty.js")]
    public Task test_var_obj_ptrn_empty()
        => ExecutionTest("dstr/var-obj-ptrn-empty");

    [Fact(DisplayName = "var-obj-ptrn-id-get-value-err.js")]
    public Task test_var_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/var-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "var-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_var_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "var-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_var_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "var-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_var_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "var-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_var_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "var-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_var_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "var-obj-ptrn-id-init-skipped.js")]
    public Task test_var_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "var-obj-ptrn-id-init-throws.js")]
    public Task test_var_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "var-obj-ptrn-id-init-unresolvable.js")]
    public Task test_var_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "var-obj-ptrn-id-trailing-comma.js")]
    public Task test_var_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/var-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "var-obj-ptrn-list-err.js")]
    public Task test_var_obj_ptrn_list_err()
        => ExecutionTest("dstr/var-obj-ptrn-list-err");

    [Fact(DisplayName = "var-obj-ptrn-prop-ary-init.js")]
    public Task test_var_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "var-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_var_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "var-obj-ptrn-prop-ary-value-null.js")]
    public Task test_var_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "var-obj-ptrn-prop-ary.js")]
    public Task test_var_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary");

    [Fact(DisplayName = "var-obj-ptrn-prop-eval-err.js")]
    public Task test_var_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/var-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-get-value-err.js")]
    public Task test_var_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_var_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-init-throws.js")]
    public Task test_var_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task test_var_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-init.js")]
    public Task test_var_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "var-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_var_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "var-obj-ptrn-prop-id.js")]
    public Task test_var_obj_ptrn_prop_id()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id");

    [Fact(DisplayName = "var-obj-ptrn-prop-obj-init.js")]
    public Task test_var_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "var-obj-ptrn-prop-obj-value-null.js")]
    public Task test_var_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "var-obj-ptrn-prop-obj-value-undef.js")]
    public Task test_var_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "var-obj-ptrn-prop-obj.js")]
    public Task test_var_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj");

    [Fact(DisplayName = "var-obj-ptrn-rest-getter.js")]
    public Task test_var_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/var-obj-ptrn-rest-getter");

    [Fact(DisplayName = "var-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_var_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/var-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "var-obj-ptrn-rest-val-obj.js")]
    public Task test_var_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/var-obj-ptrn-rest-val-obj");
}
