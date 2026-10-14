namespace Jroc.Test262.Tests.language.expressions.async_generator.dstr;

public sealed class AsyncGeneratorExpressionDestructuringCompletionTests : DiskExecutionTestsBase
{
    public AsyncGeneratorExpressionDestructuringCompletionTests() : base("language.expressions.async_generator.dstr") { }

    [Fact(DisplayName = "ary-init-iter-close.js")]
    public Task test_ary_init_iter_close() => ExecutionTest("ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-no-close.js")]
    public Task test_ary_init_iter_no_close() => ExecutionTest("ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val.js")]
    public Task test_ary_name_iter_val() => ExecutionTest("ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init.js")]
    public Task test_ary_ptrn_elem_ary_elem_init() => ExecutionTest("ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init.js")]
    public Task test_ary_ptrn_elem_ary_elision_init() => ExecutionTest("ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init.js")]
    public Task test_ary_ptrn_elem_ary_empty_init() => ExecutionTest("ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init.js")]
    public Task test_ary_ptrn_elem_ary_rest_init() => ExecutionTest("ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole.js")]
    public Task test_ary_ptrn_elem_id_init_hole() => ExecutionTest("ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped.js")]
    public Task test_ary_ptrn_elem_id_init_skipped() => ExecutionTest("ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef.js")]
    public Task test_ary_ptrn_elem_id_init_undef() => ExecutionTest("ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete.js")]
    public Task test_ary_ptrn_elem_id_iter_complete() => ExecutionTest("ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done.js")]
    public Task test_ary_ptrn_elem_id_iter_done() => ExecutionTest("ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val.js")]
    public Task test_ary_ptrn_elem_id_iter_val() => ExecutionTest("ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id-init.js")]
    public Task test_ary_ptrn_elem_obj_id_init() => ExecutionTest("ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id.js")]
    public Task test_ary_ptrn_elem_obj_id() => ExecutionTest("ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id.js")]
    public Task test_ary_ptrn_elem_obj_prop_id() => ExecutionTest("ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "ary-ptrn-elision-exhausted.js")]
    public Task test_ary_ptrn_elision_exhausted() => ExecutionTest("ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "ary-ptrn-elision.js")]
    public Task test_ary_ptrn_elision() => ExecutionTest("ary-ptrn-elision");

    [Fact(DisplayName = "ary-ptrn-empty.js")]
    public Task test_ary_ptrn_empty() => ExecutionTest("ary-ptrn-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elem.js")]
    public Task test_ary_ptrn_rest_ary_elem() => ExecutionTest("ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elision.js")]
    public Task test_ary_ptrn_rest_ary_elision() => ExecutionTest("ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "ary-ptrn-rest-ary-empty.js")]
    public Task test_ary_ptrn_rest_ary_empty() => ExecutionTest("ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-rest.js")]
    public Task test_ary_ptrn_rest_ary_rest() => ExecutionTest("ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "ary-ptrn-rest-id-direct.js")]
    public Task test_ary_ptrn_rest_id_direct() => ExecutionTest("ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision.js")]
    public Task test_ary_ptrn_rest_id_elision() => ExecutionTest("ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "ary-ptrn-rest-id-exhausted.js")]
    public Task test_ary_ptrn_rest_id_exhausted() => ExecutionTest("ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "ary-ptrn-rest-id.js")]
    public Task test_ary_ptrn_rest_id() => ExecutionTest("ary-ptrn-rest-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-id.js")]
    public Task test_ary_ptrn_rest_obj_id() => ExecutionTest("ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-prop-id.js")]
    public Task test_ary_ptrn_rest_obj_prop_id() => ExecutionTest("ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-init-iter-close.js")]
    public Task test_dflt_ary_init_iter_close() => ExecutionTest("dflt-ary-init-iter-close");

    [Fact(DisplayName = "dflt-ary-init-iter-no-close.js")]
    public Task test_dflt_ary_init_iter_no_close() => ExecutionTest("dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "dflt-ary-name-iter-val.js")]
    public Task test_dflt_ary_name_iter_val() => ExecutionTest("dflt-ary-name-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dflt_ary_ptrn_elem_ary_elem_init() => ExecutionTest("dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dflt_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dflt_ary_ptrn_elem_ary_elision_init() => ExecutionTest("dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dflt_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dflt_ary_ptrn_elem_ary_empty_init() => ExecutionTest("dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dflt_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dflt_ary_ptrn_elem_ary_rest_init() => ExecutionTest("dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dflt_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_hole() => ExecutionTest("dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_skipped() => ExecutionTest("dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dflt_ary_ptrn_elem_id_init_undef() => ExecutionTest("dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dflt_ary_ptrn_elem_id_iter_complete() => ExecutionTest("dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dflt_ary_ptrn_elem_id_iter_done() => ExecutionTest("dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dflt_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dflt_ary_ptrn_elem_id_iter_val() => ExecutionTest("dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dflt_ary_ptrn_elem_obj_id_init() => ExecutionTest("dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dflt_ary_ptrn_elem_obj_id() => ExecutionTest("dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dflt_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dflt_ary_ptrn_elem_obj_prop_id() => ExecutionTest("dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dflt_ary_ptrn_elision_exhausted() => ExecutionTest("dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elision.js")]
    public Task test_dflt_ary_ptrn_elision() => ExecutionTest("dflt-ary-ptrn-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-empty.js")]
    public Task test_dflt_ary_ptrn_empty() => ExecutionTest("dflt-ary-ptrn-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dflt_ary_ptrn_rest_ary_elem() => ExecutionTest("dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dflt_ary_ptrn_rest_ary_elision() => ExecutionTest("dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dflt_ary_ptrn_rest_ary_empty() => ExecutionTest("dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dflt_ary_ptrn_rest_ary_rest() => ExecutionTest("dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dflt_ary_ptrn_rest_id_direct() => ExecutionTest("dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dflt_ary_ptrn_rest_id_elision() => ExecutionTest("dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dflt_ary_ptrn_rest_id_exhausted() => ExecutionTest("dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id.js")]
    public Task test_dflt_ary_ptrn_rest_id() => ExecutionTest("dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dflt_ary_ptrn_rest_obj_id() => ExecutionTest("dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dflt_ary_ptrn_rest_obj_prop_id() => ExecutionTest("dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-obj-ptrn-empty.js")]
    public Task test_dflt_obj_ptrn_empty() => ExecutionTest("dflt-obj-ptrn-empty");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dflt_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dflt_obj_ptrn_id_init_fn_name_class() => ExecutionTest("dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dflt_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dflt_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dflt_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dflt_obj_ptrn_id_init_skipped() => ExecutionTest("dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dflt_obj_ptrn_id_trailing_comma() => ExecutionTest("dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dflt_obj_ptrn_prop_ary_init() => ExecutionTest("dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dflt_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary.js")]
    public Task test_dflt_obj_ptrn_prop_ary() => ExecutionTest("dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dflt_obj_ptrn_prop_id_init_skipped() => ExecutionTest("dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dflt_obj_ptrn_prop_id_init() => ExecutionTest("dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dflt_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id.js")]
    public Task test_dflt_obj_ptrn_prop_id() => ExecutionTest("dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dflt_obj_ptrn_prop_obj_init() => ExecutionTest("dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj.js")]
    public Task test_dflt_obj_ptrn_prop_obj() => ExecutionTest("dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-getter.js")]
    public Task test_dflt_obj_ptrn_rest_getter() => ExecutionTest("dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dflt_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dflt_obj_ptrn_rest_val_obj() => ExecutionTest("dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "named-ary-init-iter-close.js")]
    public Task test_named_ary_init_iter_close() => ExecutionTest("named-ary-init-iter-close");

    [Fact(DisplayName = "named-ary-init-iter-no-close.js")]
    public Task test_named_ary_init_iter_no_close() => ExecutionTest("named-ary-init-iter-no-close");

    [Fact(DisplayName = "named-ary-name-iter-val.js")]
    public Task test_named_ary_name_iter_val() => ExecutionTest("named-ary-name-iter-val");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_named_ary_ptrn_elem_ary_elem_init() => ExecutionTest("named-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_named_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("named-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_named_ary_ptrn_elem_ary_elision_init() => ExecutionTest("named-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_named_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("named-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_named_ary_ptrn_elem_ary_empty_init() => ExecutionTest("named-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_named_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("named-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_named_ary_ptrn_elem_ary_rest_init() => ExecutionTest("named-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_named_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("named-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_named_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("named-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_named_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("named-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_named_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("named-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_named_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("named-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_named_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("named-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_named_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("named-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-hole.js")]
    public Task test_named_ary_ptrn_elem_id_init_hole() => ExecutionTest("named-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_named_ary_ptrn_elem_id_init_skipped() => ExecutionTest("named-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-init-undef.js")]
    public Task test_named_ary_ptrn_elem_id_init_undef() => ExecutionTest("named-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_named_ary_ptrn_elem_id_iter_complete() => ExecutionTest("named-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-done.js")]
    public Task test_named_ary_ptrn_elem_id_iter_done() => ExecutionTest("named-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_named_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("named-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "named-ary-ptrn-elem-id-iter-val.js")]
    public Task test_named_ary_ptrn_elem_id_iter_val() => ExecutionTest("named-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-id-init.js")]
    public Task test_named_ary_ptrn_elem_obj_id_init() => ExecutionTest("named-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-id.js")]
    public Task test_named_ary_ptrn_elem_obj_id() => ExecutionTest("named-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_named_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("named-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "named-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_named_ary_ptrn_elem_obj_prop_id() => ExecutionTest("named-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "named-ary-ptrn-elision-exhausted.js")]
    public Task test_named_ary_ptrn_elision_exhausted() => ExecutionTest("named-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "named-ary-ptrn-elision.js")]
    public Task test_named_ary_ptrn_elision() => ExecutionTest("named-ary-ptrn-elision");

    [Fact(DisplayName = "named-ary-ptrn-empty.js")]
    public Task test_named_ary_ptrn_empty() => ExecutionTest("named-ary-ptrn-empty");

    [Fact(DisplayName = "named-ary-ptrn-rest-ary-elem.js")]
    public Task test_named_ary_ptrn_rest_ary_elem() => ExecutionTest("named-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "named-ary-ptrn-rest-ary-elision.js")]
    public Task test_named_ary_ptrn_rest_ary_elision() => ExecutionTest("named-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "named-ary-ptrn-rest-ary-empty.js")]
    public Task test_named_ary_ptrn_rest_ary_empty() => ExecutionTest("named-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "named-ary-ptrn-rest-ary-rest.js")]
    public Task test_named_ary_ptrn_rest_ary_rest() => ExecutionTest("named-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-direct.js")]
    public Task test_named_ary_ptrn_rest_id_direct() => ExecutionTest("named-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-elision.js")]
    public Task test_named_ary_ptrn_rest_id_elision() => ExecutionTest("named-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "named-ary-ptrn-rest-id-exhausted.js")]
    public Task test_named_ary_ptrn_rest_id_exhausted() => ExecutionTest("named-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "named-ary-ptrn-rest-id.js")]
    public Task test_named_ary_ptrn_rest_id() => ExecutionTest("named-ary-ptrn-rest-id");

    [Fact(DisplayName = "named-ary-ptrn-rest-obj-id.js")]
    public Task test_named_ary_ptrn_rest_obj_id() => ExecutionTest("named-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "named-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_named_ary_ptrn_rest_obj_prop_id() => ExecutionTest("named-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "named-dflt-ary-init-iter-close.js")]
    public Task test_named_dflt_ary_init_iter_close() => ExecutionTest("named-dflt-ary-init-iter-close");

    [Fact(DisplayName = "named-dflt-ary-init-iter-no-close.js")]
    public Task test_named_dflt_ary_init_iter_no_close() => ExecutionTest("named-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "named-dflt-ary-name-iter-val.js")]
    public Task test_named_dflt_ary_name_iter_val() => ExecutionTest("named-dflt-ary-name-iter-val");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_elem_init() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_elision_init() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_empty_init() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_rest_init() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_named_dflt_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("named-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_hole() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_skipped() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_init_undef() => ExecutionTest("named-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_iter_complete() => ExecutionTest("named-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_iter_done() => ExecutionTest("named-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("named-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_named_dflt_ary_ptrn_elem_id_iter_val() => ExecutionTest("named-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_obj_id_init() => ExecutionTest("named-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_named_dflt_ary_ptrn_elem_obj_id() => ExecutionTest("named-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_named_dflt_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("named-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_named_dflt_ary_ptrn_elem_obj_prop_id() => ExecutionTest("named-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_named_dflt_ary_ptrn_elision_exhausted() => ExecutionTest("named-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "named-dflt-ary-ptrn-elision.js")]
    public Task test_named_dflt_ary_ptrn_elision() => ExecutionTest("named-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "named-dflt-ary-ptrn-empty.js")]
    public Task test_named_dflt_ary_ptrn_empty() => ExecutionTest("named-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_named_dflt_ary_ptrn_rest_ary_elem() => ExecutionTest("named-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_named_dflt_ary_ptrn_rest_ary_elision() => ExecutionTest("named-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_named_dflt_ary_ptrn_rest_ary_empty() => ExecutionTest("named-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_named_dflt_ary_ptrn_rest_ary_rest() => ExecutionTest("named-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_named_dflt_ary_ptrn_rest_id_direct() => ExecutionTest("named-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_named_dflt_ary_ptrn_rest_id_elision() => ExecutionTest("named-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_named_dflt_ary_ptrn_rest_id_exhausted() => ExecutionTest("named-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-id.js")]
    public Task test_named_dflt_ary_ptrn_rest_id() => ExecutionTest("named-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_named_dflt_ary_ptrn_rest_obj_id() => ExecutionTest("named-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "named-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_named_dflt_ary_ptrn_rest_obj_prop_id() => ExecutionTest("named-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "named-dflt-obj-ptrn-empty.js")]
    public Task test_named_dflt_obj_ptrn_empty() => ExecutionTest("named-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_named_dflt_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("named-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_named_dflt_obj_ptrn_id_init_fn_name_class() => ExecutionTest("named-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_named_dflt_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("named-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_named_dflt_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("named-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_named_dflt_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("named-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_named_dflt_obj_ptrn_id_init_skipped() => ExecutionTest("named-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "named-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_named_dflt_obj_ptrn_id_trailing_comma() => ExecutionTest("named-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_named_dflt_obj_ptrn_prop_ary_init() => ExecutionTest("named-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_named_dflt_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("named-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-ary.js")]
    public Task test_named_dflt_obj_ptrn_prop_ary() => ExecutionTest("named-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_named_dflt_obj_ptrn_prop_id_init_skipped() => ExecutionTest("named-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_named_dflt_obj_ptrn_prop_id_init() => ExecutionTest("named-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_named_dflt_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("named-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-id.js")]
    public Task test_named_dflt_obj_ptrn_prop_id() => ExecutionTest("named-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_named_dflt_obj_ptrn_prop_obj_init() => ExecutionTest("named-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "named-dflt-obj-ptrn-prop-obj.js")]
    public Task test_named_dflt_obj_ptrn_prop_obj() => ExecutionTest("named-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "named-dflt-obj-ptrn-rest-getter.js")]
    public Task test_named_dflt_obj_ptrn_rest_getter() => ExecutionTest("named-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "named-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_named_dflt_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("named-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "named-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_named_dflt_obj_ptrn_rest_val_obj() => ExecutionTest("named-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "named-obj-ptrn-empty.js")]
    public Task test_named_obj_ptrn_empty() => ExecutionTest("named-obj-ptrn-empty");

    [Fact(DisplayName = "named-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_named_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("named-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "named-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_named_obj_ptrn_id_init_fn_name_class() => ExecutionTest("named-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "named-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_named_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("named-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "named-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_named_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("named-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "named-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_named_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("named-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "named-obj-ptrn-id-init-skipped.js")]
    public Task test_named_obj_ptrn_id_init_skipped() => ExecutionTest("named-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "named-obj-ptrn-id-trailing-comma.js")]
    public Task test_named_obj_ptrn_id_trailing_comma() => ExecutionTest("named-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "named-obj-ptrn-prop-ary-init.js")]
    public Task test_named_obj_ptrn_prop_ary_init() => ExecutionTest("named-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "named-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_named_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("named-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "named-obj-ptrn-prop-ary.js")]
    public Task test_named_obj_ptrn_prop_ary() => ExecutionTest("named-obj-ptrn-prop-ary");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_named_obj_ptrn_prop_id_init_skipped() => ExecutionTest("named-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-init.js")]
    public Task test_named_obj_ptrn_prop_id_init() => ExecutionTest("named-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "named-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_named_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("named-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "named-obj-ptrn-prop-id.js")]
    public Task test_named_obj_ptrn_prop_id() => ExecutionTest("named-obj-ptrn-prop-id");

    [Fact(DisplayName = "named-obj-ptrn-prop-obj-init.js")]
    public Task test_named_obj_ptrn_prop_obj_init() => ExecutionTest("named-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "named-obj-ptrn-prop-obj.js")]
    public Task test_named_obj_ptrn_prop_obj() => ExecutionTest("named-obj-ptrn-prop-obj");

    [Fact(DisplayName = "named-obj-ptrn-rest-getter.js")]
    public Task test_named_obj_ptrn_rest_getter() => ExecutionTest("named-obj-ptrn-rest-getter");

    [Fact(DisplayName = "named-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_named_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("named-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "named-obj-ptrn-rest-val-obj.js")]
    public Task test_named_obj_ptrn_rest_val_obj() => ExecutionTest("named-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "obj-ptrn-empty.js")]
    public Task test_obj_ptrn_empty() => ExecutionTest("obj-ptrn-empty");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-class.js")]
    public Task test_obj_ptrn_id_init_fn_name_class() => ExecutionTest("obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "obj-ptrn-id-init-skipped.js")]
    public Task test_obj_ptrn_id_init_skipped() => ExecutionTest("obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-id-trailing-comma.js")]
    public Task test_obj_ptrn_id_trailing_comma() => ExecutionTest("obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary-init.js")]
    public Task test_obj_ptrn_prop_ary_init() => ExecutionTest("obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary.js")]
    public Task test_obj_ptrn_prop_ary() => ExecutionTest("obj-ptrn-prop-ary");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-skipped.js")]
    public Task test_obj_ptrn_prop_id_init_skipped() => ExecutionTest("obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-prop-id-init.js")]
    public Task test_obj_ptrn_prop_id_init() => ExecutionTest("obj-ptrn-prop-id-init");

    [Fact(DisplayName = "obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-id.js")]
    public Task test_obj_ptrn_prop_id() => ExecutionTest("obj-ptrn-prop-id");

    [Fact(DisplayName = "obj-ptrn-prop-obj-init.js")]
    public Task test_obj_ptrn_prop_obj_init() => ExecutionTest("obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "obj-ptrn-prop-obj.js")]
    public Task test_obj_ptrn_prop_obj() => ExecutionTest("obj-ptrn-prop-obj");

    [Fact(DisplayName = "obj-ptrn-rest-getter.js")]
    public Task test_obj_ptrn_rest_getter() => ExecutionTest("obj-ptrn-rest-getter");

    [Fact(DisplayName = "obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-ptrn-rest-val-obj.js")]
    public Task test_obj_ptrn_rest_val_obj() => ExecutionTest("obj-ptrn-rest-val-obj");
}
