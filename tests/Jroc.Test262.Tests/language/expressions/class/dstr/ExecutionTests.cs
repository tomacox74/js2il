using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_.dstr;

public partial class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.dstr") { }

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-close.js")]
    public Task ported_async_gen_meth_ary_init_iter_close() => ExecutionTest("async-gen-meth-ary-init-iter-close");

    [Fact(DisplayName = "async-gen-meth-ary-init-iter-no-close.js")]
    public Task ported_async_gen_meth_ary_init_iter_no_close() => ExecutionTest("async-gen-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "async-gen-meth-ary-name-iter-val.js")]
    public Task ported_async_gen_meth_ary_name_iter_val() => ExecutionTest("async-gen-meth-ary-name-iter-val");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_elem_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_elision_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_empty_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_rest_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("async-gen-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_hole() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_skipped() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_init_undef() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_iter_complete() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_iter_done() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_id_iter_val() => ExecutionTest("async-gen-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_obj_id_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-id.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_obj_id() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task ported_async_gen_meth_ary_ptrn_elem_obj_prop_id() => ExecutionTest("async-gen-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elision-exhausted.js")]
    public Task ported_async_gen_meth_ary_ptrn_elision_exhausted() => ExecutionTest("async-gen-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-elision.js")]
    public Task ported_async_gen_meth_ary_ptrn_elision() => ExecutionTest("async-gen-meth-ary-ptrn-elision");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-empty.js")]
    public Task ported_async_gen_meth_ary_ptrn_empty() => ExecutionTest("async-gen-meth-ary-ptrn-empty");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-ary-elem.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_ary_elem() => ExecutionTest("async-gen-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-ary-elision.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_ary_elision() => ExecutionTest("async-gen-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-ary-empty.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_ary_empty() => ExecutionTest("async-gen-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-ary-rest.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_ary_rest() => ExecutionTest("async-gen-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-direct.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_id_direct() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-elision.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_id_elision() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_id_exhausted() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-id.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_id() => ExecutionTest("async-gen-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-obj-id.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_obj_id() => ExecutionTest("async-gen-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task ported_async_gen_meth_ary_ptrn_rest_obj_prop_id() => ExecutionTest("async-gen-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-close.js")]
    public Task ported_async_gen_meth_dflt_ary_init_iter_close() => ExecutionTest("async-gen-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-init-iter-no-close.js")]
    public Task ported_async_gen_meth_dflt_ary_init_iter_no_close() => ExecutionTest("async-gen-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-name-iter-val.js")]
    public Task ported_async_gen_meth_dflt_ary_name_iter_val() => ExecutionTest("async-gen-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_elem_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_elision_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_empty_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_rest_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_hole() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_skipped() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_init_undef() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_complete() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_done() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_id_iter_val() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_obj_id_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_obj_id() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elem_obj_prop_id() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elision_exhausted() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-elision.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_elision() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-empty.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_empty() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_ary_elem() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_ary_elision() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_ary_empty() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_ary_rest() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_id_direct() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_id_elision() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_id_exhausted() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-id.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_id() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_obj_id() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task ported_async_gen_meth_dflt_ary_ptrn_rest_obj_prop_id() => ExecutionTest("async-gen-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-empty.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_empty() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_fn_name_class() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_init_skipped() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_id_trailing_comma() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_ary_init() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_ary() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_id_init_skipped() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_id_init() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-id.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_id() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_obj_init() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_prop_obj() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_rest_getter() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-gen-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task ported_async_gen_meth_dflt_obj_ptrn_rest_val_obj() => ExecutionTest("async-gen-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-empty.js")]
    public Task ported_async_gen_meth_obj_ptrn_empty() => ExecutionTest("async-gen-meth-obj-ptrn-empty");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_fn_name_arrow() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_fn_name_class() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_fn_name_cover() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_fn_name_fn() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_fn_name_gen() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-init-skipped.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_init_skipped() => ExecutionTest("async-gen-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-id-trailing-comma.js")]
    public Task ported_async_gen_meth_obj_ptrn_id_trailing_comma() => ExecutionTest("async-gen-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-ary-init.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_ary_init() => ExecutionTest("async-gen-meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_ary_trailing_comma() => ExecutionTest("async-gen-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-ary.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_ary() => ExecutionTest("async-gen-meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_id_init_skipped() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-init.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_id_init() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_id_trailing_comma() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-id.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_id() => ExecutionTest("async-gen-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj-init.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_obj_init() => ExecutionTest("async-gen-meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-prop-obj.js")]
    public Task ported_async_gen_meth_obj_ptrn_prop_obj() => ExecutionTest("async-gen-meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-rest-getter.js")]
    public Task ported_async_gen_meth_obj_ptrn_rest_getter() => ExecutionTest("async-gen-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task ported_async_gen_meth_obj_ptrn_rest_skip_non_enumerable() => ExecutionTest("async-gen-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-gen-meth-obj-ptrn-rest-val-obj.js")]
    public Task ported_async_gen_meth_obj_ptrn_rest_val_obj() => ExecutionTest("async-gen-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-gen-meth-static-ary-init-iter-close.js")]
    public Task ported_async_gen_meth_static_ary_init_iter_close() => ExecutionTest("async-gen-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "async-gen-meth-static-ary-init-iter-no-close.js")]
    public Task ported_async_gen_meth_static_ary_init_iter_no_close() => ExecutionTest("async-gen-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "async-gen-meth-static-ary-name-iter-val.js")]
    public Task ported_async_gen_meth_static_ary_name_iter_val() => ExecutionTest("async-gen-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_elem_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_elem_iter() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_elision_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_elision_iter() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_empty_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_empty_iter() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_rest_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_ary_rest_iter() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_exhausted() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_fn_name_arrow() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_fn_name_class() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_fn_name_cover() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_fn_name_fn() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_fn_name_gen() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_hole() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_skipped() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_init_undef() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_iter_complete() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_iter_done() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_iter_val_array_prototype() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_id_iter_val() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_obj_id_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_obj_id() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_obj_prop_id_init() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elem_obj_prop_id() => ExecutionTest("async-gen-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elision_exhausted() => ExecutionTest("async-gen-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-elision.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_elision() => ExecutionTest("async-gen-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-empty.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_empty() => ExecutionTest("async-gen-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_ary_elem() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_ary_elision() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_ary_empty() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_ary_rest() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_id_direct() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task ported_async_gen_meth_static_ary_ptrn_rest_id_elision() => ExecutionTest("async-gen-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "gen-meth-ary-init-iter-get-err-array-prototype.js")]
    public Task ported_gen_meth_ary_init_iter_get_err_array_prototype() => ExecutionTest("gen-meth-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "gen-meth-ary-init-iter-get-err.js")]
    public Task ported_gen_meth_ary_init_iter_get_err() => ExecutionTest("gen-meth-ary-init-iter-get-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-ary-val-null.js")]
    public Task ported_gen_meth_ary_ptrn_elem_ary_val_null() => ExecutionTest("gen-meth-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-throws.js")]
    public Task ported_gen_meth_ary_ptrn_elem_id_init_throws() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task ported_gen_meth_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("gen-meth-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-step-err.js")]
    public Task ported_gen_meth_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-id-iter-val-err.js")]
    public Task ported_gen_meth_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("gen-meth-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-val-null.js")]
    public Task ported_gen_meth_ary_ptrn_elem_obj_val_null() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elem-obj-val-undef.js")]
    public Task ported_gen_meth_ary_ptrn_elem_obj_val_undef() => ExecutionTest("gen-meth-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "gen-meth-ary-ptrn-elision-step-err.js")]
    public Task ported_gen_meth_ary_ptrn_elision_step_err() => ExecutionTest("gen-meth-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-elision-next-err.js")]
    public Task ported_gen_meth_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("gen-meth-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-iter-step-err.js")]
    public Task ported_gen_meth_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("gen-meth-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-id-iter-val-err.js")]
    public Task ported_gen_meth_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("gen-meth-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task ported_gen_meth_dflt_ary_init_iter_get_err_array_prototype() => ExecutionTest("gen-meth-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "gen-meth-dflt-ary-init-iter-get-err.js")]
    public Task ported_gen_meth_dflt_ary_init_iter_get_err() => ExecutionTest("gen-meth-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_ary_val_null() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_obj_val_null() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elem_obj_val_undef() => ExecutionTest("gen-meth-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-elision-step-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_elision_step_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task ported_gen_meth_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("gen-meth-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-dflt-obj-init-null.js")]
    public Task ported_gen_meth_dflt_obj_init_null() => ExecutionTest("gen-meth-dflt-obj-init-null");

    [Fact(DisplayName = "gen-meth-dflt-obj-init-undefined.js")]
    public Task ported_gen_meth_dflt_obj_init_undefined() => ExecutionTest("gen-meth-dflt-obj-init-undefined");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-get-value-err.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_id_get_value_err() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-throws.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_id_init_throws() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTest("gen-meth-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-list-err.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_list_err() => ExecutionTest("gen-meth-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_ary_value_null() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-eval-err.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_eval_err() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_obj_value_null() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "gen-meth-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task ported_gen_meth_dflt_obj_ptrn_prop_obj_value_undef() => ExecutionTest("gen-meth-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-obj-init-null.js")]
    public Task ported_gen_meth_obj_init_null() => ExecutionTest("gen-meth-obj-init-null");

    [Fact(DisplayName = "gen-meth-obj-init-undefined.js")]
    public Task ported_gen_meth_obj_init_undefined() => ExecutionTest("gen-meth-obj-init-undefined");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-get-value-err.js")]
    public Task ported_gen_meth_obj_ptrn_id_get_value_err() => ExecutionTest("gen-meth-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-throws.js")]
    public Task ported_gen_meth_obj_ptrn_id_init_throws() => ExecutionTest("gen-meth-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "gen-meth-obj-ptrn-id-init-unresolvable.js")]
    public Task ported_gen_meth_obj_ptrn_id_init_unresolvable() => ExecutionTest("gen-meth-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-obj-ptrn-list-err.js")]
    public Task ported_gen_meth_obj_ptrn_list_err() => ExecutionTest("gen-meth-obj-ptrn-list-err");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-ary-init.js")]
    public Task ported_gen_meth_obj_ptrn_prop_ary_init() => ExecutionTest("gen-meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-ary-value-null.js")]
    public Task ported_gen_meth_obj_ptrn_prop_ary_value_null() => ExecutionTest("gen-meth-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-ary.js")]
    public Task ported_gen_meth_obj_ptrn_prop_ary() => ExecutionTest("gen-meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-eval-err.js")]
    public Task ported_gen_meth_obj_ptrn_prop_eval_err() => ExecutionTest("gen-meth-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-get-value-err.js")]
    public Task ported_gen_meth_obj_ptrn_prop_id_get_value_err() => ExecutionTest("gen-meth-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init-throws.js")]
    public Task ported_gen_meth_obj_ptrn_prop_id_init_throws() => ExecutionTest("gen-meth-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task ported_gen_meth_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("gen-meth-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-obj-init.js")]
    public Task ported_gen_meth_obj_ptrn_prop_obj_init() => ExecutionTest("gen-meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-obj-value-null.js")]
    public Task ported_gen_meth_obj_ptrn_prop_obj_value_null() => ExecutionTest("gen-meth-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-obj-value-undef.js")]
    public Task ported_gen_meth_obj_ptrn_prop_obj_value_undef() => ExecutionTest("gen-meth-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-obj-ptrn-prop-obj.js")]
    public Task ported_gen_meth_obj_ptrn_prop_obj() => ExecutionTest("gen-meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "gen-meth-static-ary-init-iter-get-err-array-prototype.js")]
    public Task ported_gen_meth_static_ary_init_iter_get_err_array_prototype() => ExecutionTest("gen-meth-static-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "gen-meth-static-ary-init-iter-get-err.js")]
    public Task ported_gen_meth_static_ary_init_iter_get_err() => ExecutionTest("gen-meth-static-ary-init-iter-get-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-ary-val-null.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_ary_val_null() => ExecutionTest("gen-meth-static-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-throws.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_id_init_throws() => ExecutionTest("gen-meth-static-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("gen-meth-static-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-step-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("gen-meth-static-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-id-iter-val-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("gen-meth-static-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-val-null.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_obj_val_null() => ExecutionTest("gen-meth-static-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elem-obj-val-undef.js")]
    public Task ported_gen_meth_static_ary_ptrn_elem_obj_val_undef() => ExecutionTest("gen-meth-static-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-elision-step-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_elision_step_err() => ExecutionTest("gen-meth-static-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-elision-next-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("gen-meth-static-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-iter-step-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("gen-meth-static-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-id-iter-val-err.js")]
    public Task ported_gen_meth_static_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("gen-meth-static-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-init-iter-get-err-array-prototype.js")]
    public Task ported_gen_meth_static_dflt_ary_init_iter_get_err_array_prototype() => ExecutionTest("gen-meth-static-dflt-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-init-iter-get-err.js")]
    public Task ported_gen_meth_static_dflt_ary_init_iter_get_err() => ExecutionTest("gen-meth-static-dflt-ary-init-iter-get-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-ary-val-null.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_ary_val_null() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-throws.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_id_init_throws() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_id_init_unresolvable() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-step-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_id_iter_step_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_id_iter_val_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-val-null.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_obj_val_null() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elem-obj-val-undef.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elem_obj_val_undef() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-elision-step-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_elision_step_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-elision-next-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_rest_id_elision_next_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-iter-step-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_rest_id_iter_step_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-id-iter-val-err.js")]
    public Task ported_gen_meth_static_dflt_ary_ptrn_rest_id_iter_val_err() => ExecutionTest("gen-meth-static-dflt-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-init-null.js")]
    public Task ported_gen_meth_static_dflt_obj_init_null() => ExecutionTest("gen-meth-static-dflt-obj-init-null");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-init-undefined.js")]
    public Task ported_gen_meth_static_dflt_obj_init_undefined() => ExecutionTest("gen-meth-static-dflt-obj-init-undefined");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-get-value-err.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_id_get_value_err() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-throws.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_id_init_throws() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_id_init_unresolvable() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-list-err.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_list_err() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-list-err");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-ary-value-null.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_ary_value_null() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-eval-err.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_eval_err() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-get-value-err.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_id_get_value_err() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-init-throws.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_id_init_throws() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-obj-value-null.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_obj_value_null() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "gen-meth-static-dflt-obj-ptrn-prop-obj-value-undef.js")]
    public Task ported_gen_meth_static_dflt_obj_ptrn_prop_obj_value_undef() => ExecutionTest("gen-meth-static-dflt-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-static-obj-init-null.js")]
    public Task ported_gen_meth_static_obj_init_null() => ExecutionTest("gen-meth-static-obj-init-null");

    [Fact(DisplayName = "gen-meth-static-obj-init-undefined.js")]
    public Task ported_gen_meth_static_obj_init_undefined() => ExecutionTest("gen-meth-static-obj-init-undefined");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-get-value-err.js")]
    public Task ported_gen_meth_static_obj_ptrn_id_get_value_err() => ExecutionTest("gen-meth-static-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-throws.js")]
    public Task ported_gen_meth_static_obj_ptrn_id_init_throws() => ExecutionTest("gen-meth-static-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_obj_ptrn_id_init_unresolvable() => ExecutionTest("gen-meth-static-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-list-err.js")]
    public Task ported_gen_meth_static_obj_ptrn_list_err() => ExecutionTest("gen-meth-static-obj-ptrn-list-err");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-ary-init.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_ary_init() => ExecutionTest("gen-meth-static-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-ary-value-null.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_ary_value_null() => ExecutionTest("gen-meth-static-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-ary.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_ary() => ExecutionTest("gen-meth-static-obj-ptrn-prop-ary");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-eval-err.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_eval_err() => ExecutionTest("gen-meth-static-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-get-value-err.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_id_get_value_err() => ExecutionTest("gen-meth-static-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-init-throws.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_id_init_throws() => ExecutionTest("gen-meth-static-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_id_init_unresolvable() => ExecutionTest("gen-meth-static-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-obj-init.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_obj_init() => ExecutionTest("gen-meth-static-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-obj-value-null.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_obj_value_null() => ExecutionTest("gen-meth-static-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-obj-value-undef.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_obj_value_undef() => ExecutionTest("gen-meth-static-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "gen-meth-static-obj-ptrn-prop-obj.js")]
    public Task ported_gen_meth_static_obj_ptrn_prop_obj() => ExecutionTest("gen-meth-static-obj-ptrn-prop-obj");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary-init.js")]
    public Task ported_meth_obj_ptrn_prop_ary_init() => ExecutionTest("meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "meth-obj-ptrn-prop-ary.js")]
    public Task ported_meth_obj_ptrn_prop_ary() => ExecutionTest("meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj-init.js")]
    public Task ported_meth_obj_ptrn_prop_obj_init() => ExecutionTest("meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "meth-obj-ptrn-prop-obj.js")]
    public Task ported_meth_obj_ptrn_prop_obj() => ExecutionTest("meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-ary-init.js")]
    public Task ported_meth_static_obj_ptrn_prop_ary_init() => ExecutionTest("meth-static-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-ary.js")]
    public Task ported_meth_static_obj_ptrn_prop_ary() => ExecutionTest("meth-static-obj-ptrn-prop-ary");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-obj-init.js")]
    public Task ported_meth_static_obj_ptrn_prop_obj_init() => ExecutionTest("meth-static-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "meth-static-obj-ptrn-prop-obj.js")]
    public Task ported_meth_static_obj_ptrn_prop_obj() => ExecutionTest("meth-static-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-ary-init.js")]
    public Task ported_private_gen_meth_obj_ptrn_prop_ary_init() => ExecutionTest("private-gen-meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-ary.js")]
    public Task ported_private_gen_meth_obj_ptrn_prop_ary() => ExecutionTest("private-gen-meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-obj-init.js")]
    public Task ported_private_gen_meth_obj_ptrn_prop_obj_init() => ExecutionTest("private-gen-meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-obj-ptrn-prop-obj.js")]
    public Task ported_private_gen_meth_obj_ptrn_prop_obj() => ExecutionTest("private-gen-meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-ary-init.js")]
    public Task ported_private_gen_meth_static_obj_ptrn_prop_ary_init() => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-ary.js")]
    public Task ported_private_gen_meth_static_obj_ptrn_prop_ary() => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-obj-init.js")]
    public Task ported_private_gen_meth_static_obj_ptrn_prop_obj_init() => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-gen-meth-static-obj-ptrn-prop-obj.js")]
    public Task ported_private_gen_meth_static_obj_ptrn_prop_obj() => ExecutionTest("private-gen-meth-static-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-ary-init.js")]
    public Task ported_private_meth_obj_ptrn_prop_ary_init() => ExecutionTest("private-meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-ary.js")]
    public Task ported_private_meth_obj_ptrn_prop_ary() => ExecutionTest("private-meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-obj-init.js")]
    public Task ported_private_meth_obj_ptrn_prop_obj_init() => ExecutionTest("private-meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-obj.js")]
    public Task ported_private_meth_obj_ptrn_prop_obj() => ExecutionTest("private-meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-obj-init.js")]
    public Task ported_private_meth_static_obj_ptrn_prop_obj_init() => ExecutionTest("private-meth-static-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-obj.js")]
    public Task ported_private_meth_static_obj_ptrn_prop_obj() => ExecutionTest("private-meth-static-obj-ptrn-prop-obj");

}
