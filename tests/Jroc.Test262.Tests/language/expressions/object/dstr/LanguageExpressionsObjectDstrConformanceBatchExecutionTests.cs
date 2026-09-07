using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.object_.dstr;

public class LanguageExpressionsObjectDstrConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public LanguageExpressionsObjectDstrConformanceBatchExecutionTests() : base("language.expressions.object.dstr") { }

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-get-err-array-prototype.js")]
    public Task async_gen_meth_ary_init_iter_get_err_array_prototype() => ExecutionTest("async-gen-meth-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-get-err.js")]
    public Task async_gen_meth_ary_init_iter_get_err() => ExecutionTest("async-gen-meth-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-val-null.js")]
    public Task async_gen_meth_ary_ptrn_elem_ary_val_null() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-throws.js")]
    public Task async_gen_meth_ary_ptrn_elem_id_init_throws() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task async_gen_meth_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-step-err.js")]
    public Task async_gen_meth_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-val-err.js")]
    public Task async_gen_meth_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-val-null.js")]
    public Task async_gen_meth_ary_ptrn_elem_obj_val_null() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-val-undef.js")]
    public Task async_gen_meth_ary_ptrn_elem_obj_val_undef() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elision-step-err.js")]
    public Task async_gen_meth_ary_ptrn_elision_step_err() => ExecutionTest("async-gen-meth-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-elision-next-err.js")]
    public Task async_gen_meth_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-iter-step-err.js")]
    public Task async_gen_meth_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-iter-val-err.js")]
    public Task async_gen_meth_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task async_gen_meth_dflt_ary_init_iter_get_err_array_prototype() => ExecutionTest("async-gen-meth-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-get-err.js")]
    public Task async_gen_meth_dflt_ary_init_iter_get_err() => ExecutionTest("async-gen-meth-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_ary_val_null() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_obj_val_null() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elem_obj_val_undef() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elision-step-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_elision_step_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-init-null.js")]
    public Task async_gen_meth_dflt_obj_init_null() => ExecutionTest("async-gen-meth-dflt-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-init-undefined.js")]
    public Task async_gen_meth_dflt_obj_init_undefined() => ExecutionTest("async-gen-meth-dflt-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-get-value-err.js")]
    public Task async_gen_meth_dflt_obj_ptrn_id_get_value_err() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-throws.js")]
    public Task async_gen_meth_dflt_obj_ptrn_id_init_throws() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task async_gen_meth_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-list-err.js")]
    public Task async_gen_meth_dflt_obj_ptrn_list_err() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_ary_value_null() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-eval-err.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_eval_err() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_obj_value_null() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task async_gen_meth_dflt_obj_ptrn_prop_obj_value_undef() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-gen-meth-obj-init-null.js")]
    public Task async_gen_meth_obj_init_null() => ExecutionTest("async-gen-meth-obj-init-null");

    [Fact(DisplayName = "async-gen-meth-obj-init-undefined.js")]
    public Task async_gen_meth_obj_init_undefined() => ExecutionTest("async-gen-meth-obj-init-undefined");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-get-value-err.js")]
    public Task async_gen_meth_obj_ptrn_id_get_value_err() => ExecutionTest("async-gen-meth-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-throws.js")]
    public Task async_gen_meth_obj_ptrn_id_init_throws() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-unresolvable.js")]
    public Task async_gen_meth_obj_ptrn_id_init_unresolvable() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-list-err.js")]
    public Task async_gen_meth_obj_ptrn_list_err() => ExecutionTest("async-gen-meth-obj-ptrn-list-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-ary-value-null.js")]
    public Task async_gen_meth_obj_ptrn_prop_ary_value_null() => ExecutionTest("async-gen-meth-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-eval-err.js")]
    public Task async_gen_meth_obj_ptrn_prop_eval_err() => ExecutionTest("async-gen-meth-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-get-value-err.js")]
    public Task async_gen_meth_obj_ptrn_prop_id_get_value_err() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init-throws.js")]
    public Task async_gen_meth_obj_ptrn_prop_id_init_throws() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task async_gen_meth_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj-value-null.js")]
    public Task async_gen_meth_obj_ptrn_prop_obj_value_null() => ExecutionTest("async-gen-meth-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj-value-undef.js")]
    public Task async_gen_meth_obj_ptrn_prop_obj_value_undef() => ExecutionTest("async-gen-meth-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-ary-init-iter-close.js")]
    public Task gen_meth_ary_init_iter_close() => ExecutionTest("gen-meth-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-ary-init-iter-no-close.js")]
    public Task gen_meth_ary_init_iter_no_close() => ExecutionTest("gen-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-ary-name-iter-val.js")]
    public Task gen_meth_ary_name_iter_val() => ExecutionTest("gen-meth-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task gen_meth_ary_ptrn_elem_ary_elem_init() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task gen_meth_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task gen_meth_ary_ptrn_elem_ary_elision_init() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task gen_meth_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task gen_meth_ary_ptrn_elem_ary_empty_init() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task gen_meth_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task gen_meth_ary_ptrn_elem_ary_rest_init() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task gen_meth_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_hole() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_skipped() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task gen_meth_ary_ptrn_elem_id_init_undef() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task gen_meth_ary_ptrn_elem_id_iter_complete() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task gen_meth_ary_ptrn_elem_id_iter_done() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task gen_meth_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task gen_meth_ary_ptrn_elem_id_iter_val() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task gen_meth_ary_ptrn_elem_obj_id_init() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-id.js")]
    public Task gen_meth_ary_ptrn_elem_obj_id() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task gen_meth_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task gen_meth_ary_ptrn_elem_obj_prop_id() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elision-exhausted.js")]
    public Task gen_meth_ary_ptrn_elision_exhausted() => ExecutionTest("gen-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elision.js")]
    public Task gen_meth_ary_ptrn_elision() => ExecutionTest("gen-meth-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-empty.js")]
    public Task gen_meth_ary_ptrn_empty() => ExecutionTest("gen-meth-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-elem.js")]
    public Task gen_meth_ary_ptrn_rest_ary_elem() => ExecutionTest("gen-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-elision.js")]
    public Task gen_meth_ary_ptrn_rest_ary_elision() => ExecutionTest("gen-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-empty.js")]
    public Task gen_meth_ary_ptrn_rest_ary_empty() => ExecutionTest("gen-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-ary-rest.js")]
    public Task gen_meth_ary_ptrn_rest_ary_rest() => ExecutionTest("gen-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-direct.js")]
    public Task gen_meth_ary_ptrn_rest_id_direct() => ExecutionTest("gen-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-elision.js")]
    public Task gen_meth_ary_ptrn_rest_id_elision() => ExecutionTest("gen-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task gen_meth_ary_ptrn_rest_id_exhausted() => ExecutionTest("gen-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id.js")]
    public Task gen_meth_ary_ptrn_rest_id() => ExecutionTest("gen-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-obj-id.js")]
    public Task gen_meth_ary_ptrn_rest_obj_id() => ExecutionTest("gen-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task gen_meth_ary_ptrn_rest_obj_prop_id() => ExecutionTest("gen-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-close.js")]
    public Task gen_meth_dflt_ary_init_iter_close() => ExecutionTest("gen-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-no-close.js")]
    public Task gen_meth_dflt_ary_init_iter_no_close() => ExecutionTest("gen-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "gen-meth-dflt-ary-name-iter-val.js")]
    public Task gen_meth_dflt_ary_name_iter_val() => ExecutionTest("gen-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_elem_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_elision_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_empty_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_rest_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_hole() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_skipped() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_init_undef() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_iter_complete() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_iter_done() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_id_iter_val() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_obj_id_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_obj_id() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task gen_meth_dflt_ary_ptrn_elem_obj_prop_id() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task gen_meth_dflt_ary_ptrn_elision_exhausted() => ExecutionTest("gen-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elision.js")]
    public Task gen_meth_dflt_ary_ptrn_elision() => ExecutionTest("gen-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-empty.js")]
    public Task gen_meth_dflt_ary_ptrn_empty() => ExecutionTest("gen-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_ary_elem() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_ary_elision() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_ary_empty() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_ary_rest() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_id_direct() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_id_elision() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_id_exhausted() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_id() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_obj_id() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task gen_meth_dflt_ary_ptrn_rest_obj_prop_id() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-empty.js")]
    public Task gen_meth_dflt_obj_ptrn_empty() => ExecutionTest("gen-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_fn_name_class() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task gen_meth_dflt_obj_ptrn_id_init_skipped() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task gen_meth_dflt_obj_ptrn_id_trailing_comma() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task gen_meth_dflt_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task gen_meth_dflt_obj_ptrn_prop_id_init_skipped() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task gen_meth_dflt_obj_ptrn_prop_id_init() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task gen_meth_dflt_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id.js")]
    public Task gen_meth_dflt_obj_ptrn_prop_id() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task gen_meth_dflt_obj_ptrn_rest_getter() => ExecutionTest("gen-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task gen_meth_dflt_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task gen_meth_dflt_obj_ptrn_rest_val_obj() => ExecutionTest("gen-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "gen-meth-obj-ptrn-empty.js")]
    public Task gen_meth_obj_ptrn_empty() => ExecutionTest("gen-meth-obj-ptrn-empty");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task gen_meth_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("gen-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task gen_meth_obj_ptrn_id_init_fn_name_class() => ExecutionTest("gen-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task gen_meth_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("gen-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task gen_meth_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("gen-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task gen_meth_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("gen-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-skipped.js")]
    public Task gen_meth_obj_ptrn_id_init_skipped() => ExecutionTest("gen-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-trailing-comma.js")]
    public Task gen_meth_obj_ptrn_id_trailing_comma() => ExecutionTest("gen-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task gen_meth_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("gen-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task gen_meth_obj_ptrn_prop_id_init_skipped() => ExecutionTest("gen-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init.js")]
    public Task gen_meth_obj_ptrn_prop_id_init() => ExecutionTest("gen-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task gen_meth_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("gen-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id.js")]
    public Task gen_meth_obj_ptrn_prop_id() => ExecutionTest("gen-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-getter.js")]
    public Task gen_meth_obj_ptrn_rest_getter() => ExecutionTest("gen-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task gen_meth_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("gen-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "gen-meth-obj-ptrn-rest-val-obj.js")]
    public Task gen_meth_obj_ptrn_rest_val_obj() => ExecutionTest("gen-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-ary-init-iter-close.js")]
    public Task meth_ary_init_iter_close() => ExecutionTest("meth-ary-init-iter-close");

    [Fact(DisplayName = "meth-ary-init-iter-get-err-array-prototype.js")]
    public Task meth_ary_init_iter_get_err_array_prototype() => ExecutionTest("meth-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-ary-init-iter-get-err.js")]
    public Task meth_ary_init_iter_get_err() => ExecutionTest("meth-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-ary-init-iter-no-close.js")]
    public Task meth_ary_init_iter_no_close() => ExecutionTest("meth-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-ary-name-iter-val.js")]
    public Task meth_ary_name_iter_val() => ExecutionTest("meth-ary-name-iter-val");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task meth_ary_ptrn_elem_ary_elem_init() => ExecutionTest("meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task meth_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task meth_ary_ptrn_elem_ary_elision_init() => ExecutionTest("meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task meth_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task meth_ary_ptrn_elem_ary_empty_init() => ExecutionTest("meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task meth_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task meth_ary_ptrn_elem_ary_rest_init() => ExecutionTest("meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task meth_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-ary-ptrn-elem-ary-val-null.js")]
    public Task meth_ary_ptrn_elem_ary_val_null() => ExecutionTest("meth-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task meth_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task meth_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task meth_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task meth_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task meth_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task meth_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-hole.js")]
    public Task meth_ary_ptrn_elem_id_init_hole() => ExecutionTest("meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task meth_ary_ptrn_elem_id_init_skipped() => ExecutionTest("meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-throws.js")]
    public Task meth_ary_ptrn_elem_id_init_throws() => ExecutionTest("meth-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-undef.js")]
    public Task meth_ary_ptrn_elem_id_init_undef() => ExecutionTest("meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task meth_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("meth-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task meth_ary_ptrn_elem_id_iter_complete() => ExecutionTest("meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-done.js")]
    public Task meth_ary_ptrn_elem_id_iter_done() => ExecutionTest("meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-step-err.js")]
    public Task meth_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("meth-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task meth_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val-err.js")]
    public Task meth_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("meth-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-ary-ptrn-elem-id-iter-val.js")]
    public Task meth_ary_ptrn_elem_id_iter_val() => ExecutionTest("meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-id-init.js")]
    public Task meth_ary_ptrn_elem_obj_id_init() => ExecutionTest("meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-id.js")]
    public Task meth_ary_ptrn_elem_obj_id() => ExecutionTest("meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task meth_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task meth_ary_ptrn_elem_obj_prop_id() => ExecutionTest("meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-val-null.js")]
    public Task meth_ary_ptrn_elem_obj_val_null() => ExecutionTest("meth-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-ary-ptrn-elem-obj-val-undef.js")]
    public Task meth_ary_ptrn_elem_obj_val_undef() => ExecutionTest("meth-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-ary-ptrn-elision-exhausted.js")]
    public Task meth_ary_ptrn_elision_exhausted() => ExecutionTest("meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-elision-step-err.js")]
    public Task meth_ary_ptrn_elision_step_err() => ExecutionTest("meth-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-elision.js")]
    public Task meth_ary_ptrn_elision() => ExecutionTest("meth-ary-ptrn-elision");

    [Fact(DisplayName = "meth-ary-ptrn-empty.js")]
    public Task meth_ary_ptrn_empty() => ExecutionTest("meth-ary-ptrn-empty");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-elem.js")]
    public Task meth_ary_ptrn_rest_ary_elem() => ExecutionTest("meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-elision.js")]
    public Task meth_ary_ptrn_rest_ary_elision() => ExecutionTest("meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-empty.js")]
    public Task meth_ary_ptrn_rest_ary_empty() => ExecutionTest("meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-ary-ptrn-rest-ary-rest.js")]
    public Task meth_ary_ptrn_rest_ary_rest() => ExecutionTest("meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-direct.js")]
    public Task meth_ary_ptrn_rest_id_direct() => ExecutionTest("meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-elision-next-err.js")]
    public Task meth_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("meth-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-elision.js")]
    public Task meth_ary_ptrn_rest_id_elision() => ExecutionTest("meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-exhausted.js")]
    public Task meth_ary_ptrn_rest_id_exhausted() => ExecutionTest("meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-iter-step-err.js")]
    public Task meth_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("meth-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id-iter-val-err.js")]
    public Task meth_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("meth-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-ary-ptrn-rest-id.js")]
    public Task meth_ary_ptrn_rest_id() => ExecutionTest("meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-ary-ptrn-rest-obj-id.js")]
    public Task meth_ary_ptrn_rest_obj_id() => ExecutionTest("meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task meth_ary_ptrn_rest_obj_prop_id() => ExecutionTest("meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-close.js")]
    public Task meth_dflt_ary_init_iter_close() => ExecutionTest("meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task meth_dflt_ary_init_iter_get_err_array_prototype() => ExecutionTest("meth-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-get-err.js")]
    public Task meth_dflt_ary_init_iter_get_err() => ExecutionTest("meth-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "meth-dflt-ary-init-iter-no-close.js")]
    public Task meth_dflt_ary_init_iter_no_close() => ExecutionTest("meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "meth-dflt-ary-name-iter-val.js")]
    public Task meth_dflt_ary_name_iter_val() => ExecutionTest("meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_elem_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_elision_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_empty_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_rest_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task meth_dflt_ary_ptrn_elem_ary_val_null() => ExecutionTest("meth-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_hole() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_skipped() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_undef() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task meth_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_complete() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_done() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task meth_dflt_ary_ptrn_elem_id_iter_val() => ExecutionTest("meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_id_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_id() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_prop_id() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_val_null() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task meth_dflt_ary_ptrn_elem_obj_val_undef() => ExecutionTest("meth-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task meth_dflt_ary_ptrn_elision_exhausted() => ExecutionTest("meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision-step-err.js")]
    public Task meth_dflt_ary_ptrn_elision_step_err() => ExecutionTest("meth-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-elision.js")]
    public Task meth_dflt_ary_ptrn_elision() => ExecutionTest("meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-empty.js")]
    public Task meth_dflt_ary_ptrn_empty() => ExecutionTest("meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task meth_dflt_ary_ptrn_rest_ary_elem() => ExecutionTest("meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task meth_dflt_ary_ptrn_rest_ary_elision() => ExecutionTest("meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task meth_dflt_ary_ptrn_rest_ary_empty() => ExecutionTest("meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task meth_dflt_ary_ptrn_rest_ary_rest() => ExecutionTest("meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task meth_dflt_ary_ptrn_rest_id_direct() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task meth_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task meth_dflt_ary_ptrn_rest_id_elision() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task meth_dflt_ary_ptrn_rest_id_exhausted() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task meth_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task meth_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("meth-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-id.js")]
    public Task meth_dflt_ary_ptrn_rest_id() => ExecutionTest("meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task meth_dflt_ary_ptrn_rest_obj_id() => ExecutionTest("meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task meth_dflt_ary_ptrn_rest_obj_prop_id() => ExecutionTest("meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "meth-dflt-obj-init-null.js")]
    public Task meth_dflt_obj_init_null() => ExecutionTest("meth-dflt-obj-init-null");

    [Fact(DisplayName = "meth-dflt-obj-init-undefined.js")]
    public Task meth_dflt_obj_init_undefined() => ExecutionTest("meth-dflt-obj-init-undefined");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-empty.js")]
    public Task meth_dflt_obj_ptrn_empty() => ExecutionTest("meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-get-value-err.js")]
    public Task meth_dflt_obj_ptrn_id_get_value_err() => ExecutionTest("meth-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task meth_dflt_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task meth_dflt_obj_ptrn_id_init_fn_name_class() => ExecutionTest("meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task meth_dflt_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task meth_dflt_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task meth_dflt_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task meth_dflt_obj_ptrn_id_init_skipped() => ExecutionTest("meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-throws.js")]
    public Task meth_dflt_obj_ptrn_id_init_throws() => ExecutionTest("meth-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task meth_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTest("meth-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task meth_dflt_obj_ptrn_id_trailing_comma() => ExecutionTest("meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-list-err.js")]
    public Task meth_dflt_obj_ptrn_list_err() => ExecutionTest("meth-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task meth_dflt_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task meth_dflt_obj_ptrn_prop_ary_value_null() => ExecutionTest("meth-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-eval-err.js")]
    public Task meth_dflt_obj_ptrn_prop_eval_err() => ExecutionTest("meth-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task meth_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task meth_dflt_obj_ptrn_prop_id_init_skipped() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task meth_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task meth_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task meth_dflt_obj_ptrn_prop_id_init() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task meth_dflt_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-id.js")]
    public Task meth_dflt_obj_ptrn_prop_id() => ExecutionTest("meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task meth_dflt_obj_ptrn_prop_obj_value_null() => ExecutionTest("meth-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task meth_dflt_obj_ptrn_prop_obj_value_undef() => ExecutionTest("meth-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-getter.js")]
    public Task meth_dflt_obj_ptrn_rest_getter() => ExecutionTest("meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task meth_dflt_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task meth_dflt_obj_ptrn_rest_val_obj() => ExecutionTest("meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "meth-obj-init-null.js")]
    public Task meth_obj_init_null() => ExecutionTest("meth-obj-init-null");

    [Fact(DisplayName = "meth-obj-init-undefined.js")]
    public Task meth_obj_init_undefined() => ExecutionTest("meth-obj-init-undefined");

    [Fact(DisplayName = "meth-obj-ptrn-empty.js")]
    public Task meth_obj_ptrn_empty() => ExecutionTest("meth-obj-ptrn-empty");

    [Fact(DisplayName = "meth-obj-ptrn-id-get-value-err.js")]
    public Task meth_obj_ptrn_id_get_value_err() => ExecutionTest("meth-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task meth_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task meth_obj_ptrn_id_init_fn_name_class() => ExecutionTest("meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task meth_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task meth_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task meth_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-skipped.js")]
    public Task meth_obj_ptrn_id_init_skipped() => ExecutionTest("meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-throws.js")]
    public Task meth_obj_ptrn_id_init_throws() => ExecutionTest("meth-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "meth-obj-ptrn-id-init-unresolvable.js")]
    public Task meth_obj_ptrn_id_init_unresolvable() => ExecutionTest("meth-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "meth-obj-ptrn-id-trailing-comma.js")]
    public Task meth_obj_ptrn_id_trailing_comma() => ExecutionTest("meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-list-err.js")]
    public Task meth_obj_ptrn_list_err() => ExecutionTest("meth-obj-ptrn-list-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task meth_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary-value-null.js")]
    public Task meth_obj_ptrn_prop_ary_value_null() => ExecutionTest("meth-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "meth-obj-ptrn-prop-eval-err.js")]
    public Task meth_obj_ptrn_prop_eval_err() => ExecutionTest("meth-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-get-value-err.js")]
    public Task meth_obj_ptrn_prop_id_get_value_err() => ExecutionTest("meth-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task meth_obj_ptrn_prop_id_init_skipped() => ExecutionTest("meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-throws.js")]
    public Task meth_obj_ptrn_prop_id_init_throws() => ExecutionTest("meth-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task meth_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("meth-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-init.js")]
    public Task meth_obj_ptrn_prop_id_init() => ExecutionTest("meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task meth_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "meth-obj-ptrn-prop-id.js")]
    public Task meth_obj_ptrn_prop_id() => ExecutionTest("meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj-value-null.js")]
    public Task meth_obj_ptrn_prop_obj_value_null() => ExecutionTest("meth-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj-value-undef.js")]
    public Task meth_obj_ptrn_prop_obj_value_undef() => ExecutionTest("meth-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "meth-obj-ptrn-rest-getter.js")]
    public Task meth_obj_ptrn_rest_getter() => ExecutionTest("meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task meth_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "meth-obj-ptrn-rest-val-obj.js")]
    public Task meth_obj_ptrn_rest_val_obj() => ExecutionTest("meth-obj-ptrn-rest-val-obj");

}
