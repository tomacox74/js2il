using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public class FunctionExpressionArrowFunctionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public FunctionExpressionArrowFunctionConformanceBatchExecutionTests() : base("language/expressions/arrow-function", "language.expressions.arrow_function") { }

    [Fact(DisplayName = "concisebody-lookahead-assignmentexpression-1.js")]
    public Task arrow_concisebody_lookahead_assignmentexpression_1()
        => ExecutionTest("arrow/concisebody-lookahead-assignmentexpression-1");

    [Fact(DisplayName = "concisebody-lookahead-assignmentexpression-2.js")]
    public Task arrow_concisebody_lookahead_assignmentexpression_2()
        => ExecutionTest("arrow/concisebody-lookahead-assignmentexpression-2");

    [Fact(DisplayName = "cannot-override-this-with-thisArg.js")]
    public Task cannot_override_this_with_thisArg()
        => ExecutionTest("cannot-override-this-with-thisArg");

    [Fact(DisplayName = "ary-init-iter-close.js")]
    public Task dstr_ary_init_iter_close()
        => ExecutionTest("dstr/ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype.js")]
    public Task dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-get-err.js")]
    public Task dstr_ary_init_iter_get_err()
        => ExecutionTest("dstr/ary-init-iter-get-err");

    [Fact(DisplayName = "ary-init-iter-no-close.js")]
    public Task dstr_ary_init_iter_no_close()
        => ExecutionTest("dstr/ary-init-iter-no-close");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init.js")]
    public Task dstr_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter.js")]
    public Task dstr_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init.js")]
    public Task dstr_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task dstr_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-step-err.js")]
    public Task dstr_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task dstr_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-err.js")]
    public Task dstr_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id-init.js")]
    public Task dstr_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id.js")]
    public Task dstr_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-null.js")]
    public Task dstr_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "ary-ptrn-elem-obj-val-undef.js")]
    public Task dstr_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "ary-ptrn-elision-exhausted.js")]
    public Task dstr_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "ary-ptrn-elision-step-err.js")]
    public Task dstr_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/ary-ptrn-elision-step-err");

    [Fact(DisplayName = "ary-ptrn-elision.js")]
    public Task dstr_ary_ptrn_elision()
        => ExecutionTest("dstr/ary-ptrn-elision");

    [Fact(DisplayName = "ary-ptrn-empty.js")]
    public Task dstr_ary_ptrn_empty()
        => ExecutionTest("dstr/ary-ptrn-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elem.js")]
    public Task dstr_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elision.js")]
    public Task dstr_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "ary-ptrn-rest-ary-empty.js")]
    public Task dstr_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-rest.js")]
    public Task dstr_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "ary-ptrn-rest-id-direct.js")]
    public Task dstr_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision-next-err.js")]
    public Task dstr_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision.js")]
    public Task dstr_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "ary-ptrn-rest-id-exhausted.js")]
    public Task dstr_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-step-err.js")]
    public Task dstr_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "ary-ptrn-rest-id-iter-val-err.js")]
    public Task dstr_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "ary-ptrn-rest-obj-id.js")]
    public Task dstr_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-prop-id.js")]
    public Task dstr_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-init-iter-close.js")]
    public Task dstr_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/dflt-ary-init-iter-close");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task dstr_dflt_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dflt-ary-init-iter-get-err.js")]
    public Task dstr_dflt_ary_init_iter_get_err()
        => ExecutionTest("dstr/dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "dflt-ary-init-iter-no-close.js")]
    public Task dstr_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "dflt-ary-name-iter-val.js")]
    public Task dstr_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/dflt-ary-name-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task dstr_dflt_ary_ptrn_elem_ary_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_throws()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task dstr_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_val_null()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task dstr_dflt_ary_ptrn_elem_obj_val_undef()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-exhausted.js")]
    public Task dstr_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-step-err.js")]
    public Task dstr_dflt_ary_ptrn_elision_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-elision.js")]
    public Task dstr_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-empty.js")]
    public Task dstr_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/dflt-ary-ptrn-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elem.js")]
    public Task dstr_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elision.js")]
    public Task dstr_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-empty.js")]
    public Task dstr_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-rest.js")]
    public Task dstr_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-direct.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_elision_next_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_iter_step_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task dstr_dflt_ary_ptrn_rest_id_iter_val_err()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task dstr_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-obj-init-null.js")]
    public Task dstr_dflt_obj_init_null()
        => ExecutionTest("dstr/dflt-obj-init-null");

    [Fact(DisplayName = "dflt-obj-init-undefined.js")]
    public Task dstr_dflt_obj_init_undefined()
        => ExecutionTest("dstr/dflt-obj-init-undefined");

    [Fact(DisplayName = "dflt-obj-ptrn-empty.js")]
    public Task dstr_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/dflt-obj-ptrn-empty");

    [Fact(DisplayName = "dflt-obj-ptrn-id-get-value-err.js")]
    public Task dstr_dflt_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task dstr_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task dstr_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task dstr_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task dstr_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task dstr_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-skipped.js")]
    public Task dstr_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-throws.js")]
    public Task dstr_dflt_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task dstr_dflt_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-id-trailing-comma.js")]
    public Task dstr_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-list-err.js")]
    public Task dstr_dflt_obj_ptrn_list_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-init.js")]
    public Task dstr_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task dstr_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task dstr_dflt_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary.js")]
    public Task dstr_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-eval-err.js")]
    public Task dstr_dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task dstr_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id.js")]
    public Task dstr_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-init.js")]
    public Task dstr_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task dstr_dflt_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task dstr_dflt_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj.js")]
    public Task dstr_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-getter.js")]
    public Task dstr_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task dstr_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-val-obj.js")]
    public Task dstr_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "obj-init-null.js")]
    public Task dstr_obj_init_null()
        => ExecutionTest("dstr/obj-init-null");

    [Fact(DisplayName = "obj-init-undefined.js")]
    public Task dstr_obj_init_undefined()
        => ExecutionTest("dstr/obj-init-undefined");

    [Fact(DisplayName = "obj-ptrn-empty.js")]
    public Task dstr_obj_ptrn_empty()
        => ExecutionTest("dstr/obj-ptrn-empty");

    [Fact(DisplayName = "obj-ptrn-id-get-value-err.js")]
    public Task dstr_obj_ptrn_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-arrow.js")]
    public Task dstr_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-class.js")]
    public Task dstr_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-cover.js")]
    public Task dstr_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-fn.js")]
    public Task dstr_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-gen.js")]
    public Task dstr_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "obj-ptrn-id-init-skipped.js")]
    public Task dstr_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-id-init-throws.js")]
    public Task dstr_obj_ptrn_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-id-init-unresolvable.js")]
    public Task dstr_obj_ptrn_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-id-trailing-comma.js")]
    public Task dstr_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-list-err.js")]
    public Task dstr_obj_ptrn_list_err()
        => ExecutionTest("dstr/obj-ptrn-list-err");

    [Fact(DisplayName = "obj-ptrn-prop-ary-trailing-comma.js")]
    public Task dstr_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary-value-null.js")]
    public Task dstr_obj_ptrn_prop_ary_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-eval-err.js")]
    public Task dstr_obj_ptrn_prop_eval_err()
        => ExecutionTest("dstr/obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-get-value-err.js")]
    public Task dstr_obj_ptrn_prop_id_get_value_err()
        => ExecutionTest("dstr/obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-skipped.js")]
    public Task dstr_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-throws.js")]
    public Task dstr_obj_ptrn_prop_id_init_throws()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-unresolvable.js")]
    public Task dstr_obj_ptrn_prop_id_init_unresolvable()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "obj-ptrn-prop-id-init.js")]
    public Task dstr_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init");

    [Fact(DisplayName = "obj-ptrn-prop-id-trailing-comma.js")]
    public Task dstr_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-id.js")]
    public Task dstr_obj_ptrn_prop_id()
        => ExecutionTest("dstr/obj-ptrn-prop-id");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-null.js")]
    public Task dstr_obj_ptrn_prop_obj_value_null()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "obj-ptrn-prop-obj-value-undef.js")]
    public Task dstr_obj_ptrn_prop_obj_value_undef()
        => ExecutionTest("dstr/obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "obj-ptrn-rest-getter.js")]
    public Task dstr_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/obj-ptrn-rest-getter");

    [Fact(DisplayName = "obj-ptrn-rest-skip-non-enumerable.js")]
    public Task dstr_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-ptrn-rest-val-obj.js")]
    public Task dstr_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "arrow-function-forbidden-ext-direct-access-prop-arguments.js")]
    public Task forbidden_ext_b1_arrow_function_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("forbidden-ext/b1/arrow-function-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "arrow-function-forbidden-ext-direct-access-prop-caller.js")]
    public Task forbidden_ext_b1_arrow_function_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("forbidden-ext/b1/arrow-function-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "arrow-function-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task forbidden_ext_b2_arrow_function_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("forbidden-ext/b2/arrow-function-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "arrow-function-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task forbidden_ext_b2_arrow_function_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("forbidden-ext/b2/arrow-function-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "arrow-function-forbidden-ext-indirect-access-prop-caller.js")]
    public Task forbidden_ext_b2_arrow_function_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("forbidden-ext/b2/arrow-function-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "lexical-new.target-closure-returned.js")]
    public Task lexical_new_target_closure_returned()
        => ExecutionTest("lexical-new.target-closure-returned");

    [Fact(DisplayName = "lexical-new.target.js")]
    public Task lexical_new_target()
        => ExecutionTest("lexical-new.target");

    [Fact(DisplayName = "param-dflt-yield-id-non-strict.js")]
    public Task param_dflt_yield_id_non_strict()
        => ExecutionTest("param-dflt-yield-id-non-strict");

    [Fact(DisplayName = "scope-paramsbody-var-open.js")]
    public Task scope_paramsbody_var_open()
        => ExecutionTest("scope-paramsbody-var-open");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-arguments.js")]
    public Task syntax_arrowparameters_bindingidentifier_arguments()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-arguments");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-concisebody-assignmentexpression.js")]
    public Task syntax_arrowparameters_bindingidentifier_concisebody_assignmentexpression()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-concisebody-assignmentexpression");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_bindingidentifier_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-eval.js")]
    public Task syntax_arrowparameters_bindingidentifier_eval()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-eval");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-lineterminator-concisebody-assignmentexpression.js")]
    public Task syntax_arrowparameters_bindingidentifier_lineterminator_concisebody_assignmentexpression()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-lineterminator-concisebody-assignmentexpression");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-lineterminator-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_bindingidentifier_lineterminator_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-lineterminator-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-bindingidentifier-yield.js")]
    public Task syntax_arrowparameters_bindingidentifier_yield()
        => ExecutionTest("syntax/arrowparameters-bindingidentifier-yield");

    [Fact(DisplayName = "arrowparameters-cover-concisebody-assignmentexpression.js")]
    public Task syntax_arrowparameters_cover_concisebody_assignmentexpression()
        => ExecutionTest("syntax/arrowparameters-cover-concisebody-assignmentexpression");

    [Fact(DisplayName = "arrowparameters-cover-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_cover_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-cover-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-cover-formalparameters-arguments.js")]
    public Task syntax_arrowparameters_cover_formalparameters_arguments()
        => ExecutionTest("syntax/arrowparameters-cover-formalparameters-arguments");

    [Fact(DisplayName = "arrowparameters-cover-formalparameters-eval.js")]
    public Task syntax_arrowparameters_cover_formalparameters_eval()
        => ExecutionTest("syntax/arrowparameters-cover-formalparameters-eval");

    [Fact(DisplayName = "arrowparameters-cover-formalparameters-yield.js")]
    public Task syntax_arrowparameters_cover_formalparameters_yield()
        => ExecutionTest("syntax/arrowparameters-cover-formalparameters-yield");

    [Fact(DisplayName = "arrowparameters-cover-includes-rest-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_cover_includes_rest_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-cover-includes-rest-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-cover-initialize-1.js")]
    public Task syntax_arrowparameters_cover_initialize_1()
        => ExecutionTest("syntax/arrowparameters-cover-initialize-1");

    [Fact(DisplayName = "arrowparameters-cover-initialize-2.js")]
    public Task syntax_arrowparameters_cover_initialize_2()
        => ExecutionTest("syntax/arrowparameters-cover-initialize-2");

    [Fact(DisplayName = "arrowparameters-cover-lineterminator-concisebody-assignmentexpression.js")]
    public Task syntax_arrowparameters_cover_lineterminator_concisebody_assignmentexpression()
        => ExecutionTest("syntax/arrowparameters-cover-lineterminator-concisebody-assignmentexpression");

    [Fact(DisplayName = "arrowparameters-cover-lineterminator-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_cover_lineterminator_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-cover-lineterminator-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-cover-rest-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_cover_rest_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-cover-rest-concisebody-functionbody");

    [Fact(DisplayName = "arrowparameters-cover-rest-lineterminator-concisebody-functionbody.js")]
    public Task syntax_arrowparameters_cover_rest_lineterminator_concisebody_functionbody()
        => ExecutionTest("syntax/arrowparameters-cover-rest-lineterminator-concisebody-functionbody");

    [Fact(DisplayName = "variations.js")]
    public Task syntax_variations()
        => ExecutionTest("syntax/variations");

}
