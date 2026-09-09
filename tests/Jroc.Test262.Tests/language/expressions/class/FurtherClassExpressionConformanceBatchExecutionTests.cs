using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.expressions.class_;

public class FurtherClassExpressionConformanceBatchExecutionTests : FileSystemExecutionTestsBase
{
    public FurtherClassExpressionConformanceBatchExecutionTests() : base("language/expressions/class", "language.expressions.class_") { }

    [Fact(DisplayName = "dstr/private-meth-ary-init-iter-close.js")]
    public Task test_dstr_private_meth_ary_init_iter_close()
        => ExecutionTest("dstr/private-meth-ary-init-iter-close");

    [Fact(DisplayName = "dstr/private-meth-ary-init-iter-no-close.js")]
    public Task test_dstr_private_meth_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-meth-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/private-meth-ary-name-iter-val.js")]
    public Task test_dstr_private_meth_ary_name_iter_val()
        => ExecutionTest("dstr/private-meth-ary-name-iter-val");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-empty.js")]
    public Task test_dstr_private_meth_ary_ptrn_empty()
        => ExecutionTest("dstr/private-meth-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/private-meth-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_meth_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-meth-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-init-iter-close.js")]
    public Task test_dstr_private_meth_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/private-meth-dflt-ary-init-iter-close");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_private_meth_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-meth-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-name-iter-val.js")]
    public Task test_dstr_private_meth_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/private-meth-dflt-ary-name-iter-val");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_meth_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-meth-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/private-meth-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-empty.js")]
    public Task test_dstr_private_meth_obj_ptrn_empty()
        => ExecutionTest("dstr/private-meth-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_meth_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-meth-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/private-meth-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "dstr/private-meth-static-ary-init-iter-close.js")]
    public Task test_dstr_private_meth_static_ary_init_iter_close()
        => ExecutionTest("dstr/private-meth-static-ary-init-iter-close");

    [Fact(DisplayName = "dstr/private-meth-static-ary-init-iter-no-close.js")]
    public Task test_dstr_private_meth_static_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-meth-static-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/private-meth-static-ary-name-iter-val.js")]
    public Task test_dstr_private_meth_static_ary_name_iter_val()
        => ExecutionTest("dstr/private-meth-static-ary-name-iter-val");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-elision.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_elision()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-empty.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_empty()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/private-meth-static-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_meth_static_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-meth-static-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-init-iter-close.js")]
    public Task test_dstr_private_meth_static_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-init-iter-close");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_private_meth_static_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-name-iter-val.js")]
    public Task test_dstr_private_meth_static_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-name-iter-val");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-elision.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-empty.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_private_meth_static_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/private-meth-static-dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-empty.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-ary-init.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-ary.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-obj-init.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-prop-obj.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/private-meth-static-dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_static_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-static-dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-empty.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_empty()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-prop-id.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_prop_id()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-rest-getter.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/private-meth-static-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_private_meth_static_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/private-meth-static-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "elements/gen-private-method-static/yield-spread-arr-multiple.js")]
    public Task test_elements_gen_private_method_static_yield_spread_arr_multiple()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-arr-multiple");

    [Fact(DisplayName = "elements/gen-private-method-static/yield-spread-arr-single.js")]
    public Task test_elements_gen_private_method_static_yield_spread_arr_single()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-arr-single");

    [Fact(DisplayName = "elements/gen-private-method-static/yield-spread-obj.js")]
    public Task test_elements_gen_private_method_static_yield_spread_obj()
        => ExecutionTest("elements/gen-private-method-static/yield-spread-obj");

    [Fact(DisplayName = "elements/gen-private-method/yield-spread-arr-multiple.js")]
    public Task test_elements_gen_private_method_yield_spread_arr_multiple()
        => ExecutionTest("elements/gen-private-method/yield-spread-arr-multiple");

    [Fact(DisplayName = "elements/gen-private-method/yield-spread-arr-single.js")]
    public Task test_elements_gen_private_method_yield_spread_arr_single()
        => ExecutionTest("elements/gen-private-method/yield-spread-arr-single");

    [Fact(DisplayName = "elements/gen-private-method/yield-spread-obj.js")]
    public Task test_elements_gen_private_method_yield_spread_obj()
        => ExecutionTest("elements/gen-private-method/yield-spread-obj");

    [Fact(DisplayName = "elements/init-err-evaluation.js")]
    public Task test_elements_init_err_evaluation()
        => ExecutionTest("elements/init-err-evaluation");

    [Fact(DisplayName = "elements/new-no-sc-line-method-computed-symbol-names.js")]
    public Task test_elements_new_no_sc_line_method_computed_symbol_names()
        => ExecutionTest("elements/new-no-sc-line-method-computed-symbol-names");

    [Fact(DisplayName = "elements/new-no-sc-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_no_sc_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-no-sc-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/new-no-sc-line-method-private-field-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_field_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-field-usage");

    [Fact(DisplayName = "elements/new-no-sc-line-method-private-method-getter-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_method_getter_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-method-getter-usage");

    [Fact(DisplayName = "elements/new-no-sc-line-method-private-method-usage.js")]
    public Task test_elements_new_no_sc_line_method_private_method_usage()
        => ExecutionTest("elements/new-no-sc-line-method-private-method-usage");

    [Fact(DisplayName = "elements/new-no-sc-line-method-private-names.js")]
    public Task test_elements_new_no_sc_line_method_private_names()
        => ExecutionTest("elements/new-no-sc-line-method-private-names");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_new_no_sc_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-no-sc-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-field-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_field_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-field-identifier");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-getter-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-getter.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_getter()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-getter");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-method-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_method_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-method-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-method.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_method()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-method");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-setter-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-private-setter.js")]
    public Task test_elements_new_no_sc_line_method_rs_private_setter()
        => ExecutionTest("elements/new-no-sc-line-method-rs-private-setter");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-no-sc-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_no_sc_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-no-sc-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-no-sc-line-method-static-private-methods.js")]
    public Task test_elements_new_no_sc_line_method_static_private_methods()
        => ExecutionTest("elements/new-no-sc-line-method-static-private-methods");

    [Fact(DisplayName = "elements/new-sc-line-gen-computed-symbol-names.js")]
    public Task test_elements_new_sc_line_gen_computed_symbol_names()
        => ExecutionTest("elements/new-sc-line-gen-computed-symbol-names");

    [Fact(DisplayName = "elements/new-sc-line-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_sc_line_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-sc-line-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/new-sc-line-gen-private-field-usage.js")]
    public Task test_elements_new_sc_line_gen_private_field_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-field-usage");

    [Fact(DisplayName = "elements/new-sc-line-gen-private-method-getter-usage.js")]
    public Task test_elements_new_sc_line_gen_private_method_getter_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-method-getter-usage");

    [Fact(DisplayName = "elements/new-sc-line-gen-private-method-usage.js")]
    public Task test_elements_new_sc_line_gen_private_method_usage()
        => ExecutionTest("elements/new-sc-line-gen-private-method-usage");

    [Fact(DisplayName = "elements/new-sc-line-gen-private-names.js")]
    public Task test_elements_new_sc_line_gen_private_names()
        => ExecutionTest("elements/new-sc-line-gen-private-names");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-field-identifier-initializer.js")]
    public Task test_elements_new_sc_line_gen_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-field-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_field_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-field-identifier");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-getter-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_getter_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-getter-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-getter.js")]
    public Task test_elements_new_sc_line_gen_rs_private_getter()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-getter");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-method-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_method_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-method-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-method.js")]
    public Task test_elements_new_sc_line_gen_rs_private_method()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-method");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-setter-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_private_setter_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-setter-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-private-setter.js")]
    public Task test_elements_new_sc_line_gen_rs_private_setter()
        => ExecutionTest("elements/new-sc-line-gen-rs-private-setter");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-gen-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-gen-static-private-methods.js")]
    public Task test_elements_new_sc_line_gen_static_private_methods()
        => ExecutionTest("elements/new-sc-line-gen-static-private-methods");

    [Fact(DisplayName = "elements/new-sc-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_new_sc_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/new-sc-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/new-sc-line-method-private-field-usage.js")]
    public Task test_elements_new_sc_line_method_private_field_usage()
        => ExecutionTest("elements/new-sc-line-method-private-field-usage");

    [Fact(DisplayName = "elements/new-sc-line-method-private-method-getter-usage.js")]
    public Task test_elements_new_sc_line_method_private_method_getter_usage()
        => ExecutionTest("elements/new-sc-line-method-private-method-getter-usage");

    [Fact(DisplayName = "elements/new-sc-line-method-private-method-usage.js")]
    public Task test_elements_new_sc_line_method_private_method_usage()
        => ExecutionTest("elements/new-sc-line-method-private-method-usage");

    [Fact(DisplayName = "elements/new-sc-line-method-private-names.js")]
    public Task test_elements_new_sc_line_method_private_names()
        => ExecutionTest("elements/new-sc-line-method-private-names");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_new_sc_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-field-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_field_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-field-identifier");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-getter-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-getter.js")]
    public Task test_elements_new_sc_line_method_rs_private_getter()
        => ExecutionTest("elements/new-sc-line-method-rs-private-getter");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-method-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_method_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-method-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-method.js")]
    public Task test_elements_new_sc_line_method_rs_private_method()
        => ExecutionTest("elements/new-sc-line-method-rs-private-method");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-setter-alt.js")]
    public Task test_elements_new_sc_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-private-setter.js")]
    public Task test_elements_new_sc_line_method_rs_private_setter()
        => ExecutionTest("elements/new-sc-line-method-rs-private-setter");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_new_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/new-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/new-sc-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_new_sc_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/new-sc-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/new-sc-line-method-static-private-methods.js")]
    public Task test_elements_new_sc_line_method_static_private_methods()
        => ExecutionTest("elements/new-sc-line-method-static-private-methods");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-escape-sequence-ZWJ.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_ZWJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-ZWJ");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-escape-sequence-ZWNJ.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_ZWNJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-ZWNJ");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-escape-sequence-u2118.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_u2118()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-u2118");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-escape-sequence-u6F.js")]
    public Task test_elements_private_accessor_name_inst_private_escape_sequence_u6F()
        => ExecutionTest("elements/private-accessor-name/inst-private-escape-sequence-u6F");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-ZWJ.js")]
    public Task test_elements_private_accessor_name_inst_private_name_ZWJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-ZWJ");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-ZWNJ.js")]
    public Task test_elements_private_accessor_name_inst_private_name_ZWNJ()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-ZWNJ");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-common.js")]
    public Task test_elements_private_accessor_name_inst_private_name_common()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-common");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-dollar.js")]
    public Task test_elements_private_accessor_name_inst_private_name_dollar()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-dollar");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-u2118.js")]
    public Task test_elements_private_accessor_name_inst_private_name_u2118()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-u2118");

    [Fact(DisplayName = "elements/private-accessor-name/inst-private-name-underscore.js")]
    public Task test_elements_private_accessor_name_inst_private_name_underscore()
        => ExecutionTest("elements/private-accessor-name/inst-private-name-underscore");

    [Fact(DisplayName = "elements/private-async-generator-method-name.js")]
    public Task test_elements_private_async_generator_method_name()
        => ExecutionTest("elements/private-async-generator-method-name");

    [Fact(DisplayName = "elements/private-async-method-name.js")]
    public Task test_elements_private_async_method_name()
        => ExecutionTest("elements/private-async-method-name");

    [Fact(DisplayName = "elements/private-field-as-arrow-function.js")]
    public Task test_elements_private_field_as_arrow_function()
        => ExecutionTest("elements/private-field-as-arrow-function");

    [Fact(DisplayName = "elements/private-field-as-function.js")]
    public Task test_elements_private_field_as_function()
        => ExecutionTest("elements/private-field-as-function");

    [Fact(DisplayName = "elements/private-fields-proxy-default-handler-throws.js")]
    public Task test_elements_private_fields_proxy_default_handler_throws()
        => ExecutionTest("elements/private-fields-proxy-default-handler-throws");

    [Fact(DisplayName = "elements/private-generator-method-name.js")]
    public Task test_elements_private_generator_method_name()
        => ExecutionTest("elements/private-generator-method-name");

    [Fact(DisplayName = "elements/private-getter-is-not-a-own-property.js")]
    public Task test_elements_private_getter_is_not_a_own_property()
        => ExecutionTest("elements/private-getter-is-not-a-own-property");

    [Fact(DisplayName = "elements/private-method-comparison.js")]
    public Task test_elements_private_method_comparison()
        => ExecutionTest("elements/private-method-comparison");

    [Fact(DisplayName = "elements/private-method-is-not-a-own-property.js")]
    public Task test_elements_private_method_is_not_a_own_property()
        => ExecutionTest("elements/private-method-is-not-a-own-property");

    [Fact(DisplayName = "elements/private-method-length.js")]
    public Task test_elements_private_method_length()
        => ExecutionTest("elements/private-method-length");

    [Fact(DisplayName = "elements/private-setter-is-not-a-own-property.js")]
    public Task test_elements_private_setter_is_not_a_own_property()
        => ExecutionTest("elements/private-setter-is-not-a-own-property");

    [Fact(DisplayName = "elements/private-static-async-generator-method-name.js")]
    public Task test_elements_private_static_async_generator_method_name()
        => ExecutionTest("elements/private-static-async-generator-method-name");

    [Fact(DisplayName = "elements/private-static-async-method-name.js")]
    public Task test_elements_private_static_async_method_name()
        => ExecutionTest("elements/private-static-async-method-name");

    [Fact(DisplayName = "elements/private-static-generator-method-name.js")]
    public Task test_elements_private_static_generator_method_name()
        => ExecutionTest("elements/private-static-generator-method-name");

    [Fact(DisplayName = "elements/private-static-method-length.js")]
    public Task test_elements_private_static_method_length()
        => ExecutionTest("elements/private-static-method-length");

    [Fact(DisplayName = "elements/private-static-method-name.js")]
    public Task test_elements_private_static_method_name()
        => ExecutionTest("elements/private-static-method-name");

    [Fact(DisplayName = "elements/prod-private-getter-before-super-return-in-constructor.js")]
    public Task test_elements_prod_private_getter_before_super_return_in_constructor()
        => ExecutionTest("elements/prod-private-getter-before-super-return-in-constructor");

    [Fact(DisplayName = "elements/prod-private-getter-before-super-return-in-field-initializer.js")]
    public Task test_elements_prod_private_getter_before_super_return_in_field_initializer()
        => ExecutionTest("elements/prod-private-getter-before-super-return-in-field-initializer");

    [Fact(DisplayName = "elements/prod-private-method-before-super-return-in-constructor.js")]
    public Task test_elements_prod_private_method_before_super_return_in_constructor()
        => ExecutionTest("elements/prod-private-method-before-super-return-in-constructor");

    [Fact(DisplayName = "elements/prod-private-method-before-super-return-in-field-initializer.js")]
    public Task test_elements_prod_private_method_before_super_return_in_field_initializer()
        => ExecutionTest("elements/prod-private-method-before-super-return-in-field-initializer");

    [Fact(DisplayName = "elements/prod-private-setter-before-super-return-in-constructor.js")]
    public Task test_elements_prod_private_setter_before_super_return_in_constructor()
        => ExecutionTest("elements/prod-private-setter-before-super-return-in-constructor");

    [Fact(DisplayName = "elements/prod-private-setter-before-super-return-in-field-initializer.js")]
    public Task test_elements_prod_private_setter_before_super_return_in_field_initializer()
        => ExecutionTest("elements/prod-private-setter-before-super-return-in-field-initializer");

    [Fact(DisplayName = "elements/redeclaration-symbol.js")]
    public Task test_elements_redeclaration_symbol()
        => ExecutionTest("elements/redeclaration-symbol");

    [Fact(DisplayName = "elements/regular-definitions-computed-symbol-names.js")]
    public Task test_elements_regular_definitions_computed_symbol_names()
        => ExecutionTest("elements/regular-definitions-computed-symbol-names");

    [Fact(DisplayName = "elements/regular-definitions-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_regular_definitions_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/regular-definitions-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/regular-definitions-private-field-usage.js")]
    public Task test_elements_regular_definitions_private_field_usage()
        => ExecutionTest("elements/regular-definitions-private-field-usage");

    [Fact(DisplayName = "elements/regular-definitions-private-method-getter-usage.js")]
    public Task test_elements_regular_definitions_private_method_getter_usage()
        => ExecutionTest("elements/regular-definitions-private-method-getter-usage");

    [Fact(DisplayName = "elements/regular-definitions-private-method-usage.js")]
    public Task test_elements_regular_definitions_private_method_usage()
        => ExecutionTest("elements/regular-definitions-private-method-usage");

    [Fact(DisplayName = "elements/regular-definitions-private-names.js")]
    public Task test_elements_regular_definitions_private_names()
        => ExecutionTest("elements/regular-definitions-private-names");

    [Fact(DisplayName = "elements/regular-definitions-rs-field-identifier-initializer.js")]
    public Task test_elements_regular_definitions_rs_field_identifier_initializer()
        => ExecutionTest("elements/regular-definitions-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/regular-definitions-rs-field-identifier.js")]
    public Task test_elements_regular_definitions_rs_field_identifier()
        => ExecutionTest("elements/regular-definitions-rs-field-identifier");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-getter-alt.js")]
    public Task test_elements_regular_definitions_rs_private_getter_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-getter-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-getter.js")]
    public Task test_elements_regular_definitions_rs_private_getter()
        => ExecutionTest("elements/regular-definitions-rs-private-getter");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-method-alt.js")]
    public Task test_elements_regular_definitions_rs_private_method_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-method-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-method.js")]
    public Task test_elements_regular_definitions_rs_private_method()
        => ExecutionTest("elements/regular-definitions-rs-private-method");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-setter-alt.js")]
    public Task test_elements_regular_definitions_rs_private_setter_alt()
        => ExecutionTest("elements/regular-definitions-rs-private-setter-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-private-setter.js")]
    public Task test_elements_regular_definitions_rs_private_setter()
        => ExecutionTest("elements/regular-definitions-rs-private-setter");

    [Fact(DisplayName = "elements/regular-definitions-rs-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-privatename-identifier-initializer.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/regular-definitions-rs-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-privatename-identifier");

    [Fact(DisplayName = "elements/regular-definitions-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/regular-definitions-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_regular_definitions_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/regular-definitions-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/regular-definitions-rs-static-method-privatename-identifier.js")]
    public Task test_elements_regular_definitions_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/regular-definitions-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/regular-definitions-static-private-methods.js")]
    public Task test_elements_regular_definitions_static_private_methods()
        => ExecutionTest("elements/regular-definitions-static-private-methods");

    [Fact(DisplayName = "elements/same-line-gen-computed-symbol-names.js")]
    public Task test_elements_same_line_gen_computed_symbol_names()
        => ExecutionTest("elements/same-line-gen-computed-symbol-names");

    [Fact(DisplayName = "elements/same-line-gen-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_same_line_gen_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/same-line-gen-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/same-line-gen-private-field-usage.js")]
    public Task test_elements_same_line_gen_private_field_usage()
        => ExecutionTest("elements/same-line-gen-private-field-usage");

    [Fact(DisplayName = "elements/same-line-gen-private-method-getter-usage.js")]
    public Task test_elements_same_line_gen_private_method_getter_usage()
        => ExecutionTest("elements/same-line-gen-private-method-getter-usage");

    [Fact(DisplayName = "elements/same-line-gen-private-method-usage.js")]
    public Task test_elements_same_line_gen_private_method_usage()
        => ExecutionTest("elements/same-line-gen-private-method-usage");

    [Fact(DisplayName = "elements/same-line-gen-private-names.js")]
    public Task test_elements_same_line_gen_private_names()
        => ExecutionTest("elements/same-line-gen-private-names");

    [Fact(DisplayName = "elements/same-line-gen-rs-field-identifier-initializer.js")]
    public Task test_elements_same_line_gen_rs_field_identifier_initializer()
        => ExecutionTest("elements/same-line-gen-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/same-line-gen-rs-field-identifier.js")]
    public Task test_elements_same_line_gen_rs_field_identifier()
        => ExecutionTest("elements/same-line-gen-rs-field-identifier");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-getter-alt.js")]
    public Task test_elements_same_line_gen_rs_private_getter_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-getter-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-getter.js")]
    public Task test_elements_same_line_gen_rs_private_getter()
        => ExecutionTest("elements/same-line-gen-rs-private-getter");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-method-alt.js")]
    public Task test_elements_same_line_gen_rs_private_method_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-method-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-method.js")]
    public Task test_elements_same_line_gen_rs_private_method()
        => ExecutionTest("elements/same-line-gen-rs-private-method");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-setter-alt.js")]
    public Task test_elements_same_line_gen_rs_private_setter_alt()
        => ExecutionTest("elements/same-line-gen-rs-private-setter-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-private-setter.js")]
    public Task test_elements_same_line_gen_rs_private_setter()
        => ExecutionTest("elements/same-line-gen-rs-private-setter");

    [Fact(DisplayName = "elements/same-line-gen-rs-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-privatename-identifier-initializer.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/same-line-gen-rs-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-gen-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-gen-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_gen_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-gen-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-gen-rs-static-method-privatename-identifier.js")]
    public Task test_elements_same_line_gen_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/same-line-gen-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-gen-static-private-methods.js")]
    public Task test_elements_same_line_gen_static_private_methods()
        => ExecutionTest("elements/same-line-gen-static-private-methods");

    [Fact(DisplayName = "elements/same-line-method-computed-symbol-names.js")]
    public Task test_elements_same_line_method_computed_symbol_names()
        => ExecutionTest("elements/same-line-method-computed-symbol-names");

    [Fact(DisplayName = "elements/same-line-method-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_same_line_method_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/same-line-method-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/same-line-method-private-field-usage.js")]
    public Task test_elements_same_line_method_private_field_usage()
        => ExecutionTest("elements/same-line-method-private-field-usage");

    [Fact(DisplayName = "elements/same-line-method-private-method-getter-usage.js")]
    public Task test_elements_same_line_method_private_method_getter_usage()
        => ExecutionTest("elements/same-line-method-private-method-getter-usage");

    [Fact(DisplayName = "elements/same-line-method-private-method-usage.js")]
    public Task test_elements_same_line_method_private_method_usage()
        => ExecutionTest("elements/same-line-method-private-method-usage");

    [Fact(DisplayName = "elements/same-line-method-private-names.js")]
    public Task test_elements_same_line_method_private_names()
        => ExecutionTest("elements/same-line-method-private-names");

    [Fact(DisplayName = "elements/same-line-method-rs-field-identifier-initializer.js")]
    public Task test_elements_same_line_method_rs_field_identifier_initializer()
        => ExecutionTest("elements/same-line-method-rs-field-identifier-initializer");

    [Fact(DisplayName = "elements/same-line-method-rs-field-identifier.js")]
    public Task test_elements_same_line_method_rs_field_identifier()
        => ExecutionTest("elements/same-line-method-rs-field-identifier");

    [Fact(DisplayName = "elements/same-line-method-rs-private-getter-alt.js")]
    public Task test_elements_same_line_method_rs_private_getter_alt()
        => ExecutionTest("elements/same-line-method-rs-private-getter-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-private-getter.js")]
    public Task test_elements_same_line_method_rs_private_getter()
        => ExecutionTest("elements/same-line-method-rs-private-getter");

    [Fact(DisplayName = "elements/same-line-method-rs-private-method-alt.js")]
    public Task test_elements_same_line_method_rs_private_method_alt()
        => ExecutionTest("elements/same-line-method-rs-private-method-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-private-method.js")]
    public Task test_elements_same_line_method_rs_private_method()
        => ExecutionTest("elements/same-line-method-rs-private-method");

    [Fact(DisplayName = "elements/same-line-method-rs-private-setter-alt.js")]
    public Task test_elements_same_line_method_rs_private_setter_alt()
        => ExecutionTest("elements/same-line-method-rs-private-setter-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-private-setter.js")]
    public Task test_elements_same_line_method_rs_private_setter()
        => ExecutionTest("elements/same-line-method-rs-private-setter");

    [Fact(DisplayName = "elements/same-line-method-rs-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-privatename-identifier-initializer-alt.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_initializer_alt()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-privatename-identifier-initializer.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier_initializer()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier-initializer");

    [Fact(DisplayName = "elements/same-line-method-rs-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-method-rs-static-generator-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_static_generator_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-static-generator-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-static-generator-method-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_static_generator_method_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-static-generator-method-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-method-rs-static-method-privatename-identifier-alt.js")]
    public Task test_elements_same_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("elements/same-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "elements/same-line-method-rs-static-method-privatename-identifier.js")]
    public Task test_elements_same_line_method_rs_static_method_privatename_identifier()
        => ExecutionTest("elements/same-line-method-rs-static-method-privatename-identifier");

    [Fact(DisplayName = "elements/same-line-method-static-private-methods.js")]
    public Task test_elements_same_line_method_static_private_methods()
        => ExecutionTest("elements/same-line-method-static-private-methods");

    [Fact(DisplayName = "elements/static-private-getter.js")]
    public Task test_elements_static_private_getter()
        => ExecutionTest("elements/static-private-getter");

    [Fact(DisplayName = "elements/static-private-method-and-instance-method-brand-check.js")]
    public Task test_elements_static_private_method_and_instance_method_brand_check()
        => ExecutionTest("elements/static-private-method-and-instance-method-brand-check");

    [Fact(DisplayName = "elements/static-private-method-subclass-receiver.js")]
    public Task test_elements_static_private_method_subclass_receiver()
        => ExecutionTest("elements/static-private-method-subclass-receiver");

    [Fact(DisplayName = "elements/static-private-methods-proxy-default-handler-throws.js")]
    public Task test_elements_static_private_methods_proxy_default_handler_throws()
        => ExecutionTest("elements/static-private-methods-proxy-default-handler-throws");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-gen-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_gen_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-get-meth-prototype.js")]
    public Task test_elements_syntax_early_errors_grammar_static_get_meth_prototype()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-get-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-get-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_get_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-get-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-meth-prototype.js")]
    public Task test_elements_syntax_early_errors_grammar_static_meth_prototype()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-async-gen-meth-constructor.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_async_gen_meth_constructor()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-async-gen-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-async-gen-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_async_gen_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-async-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-async-meth-constructor.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_async_meth_constructor()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-async-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-async-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_async_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-async-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-gen-meth-constructor.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_gen_meth_constructor()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-gen-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-gen-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_gen_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-gen-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-meth-constructor.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_meth_constructor()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-meth-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-private-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_private_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-private-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-privatename-constructor.js")]
    public Task test_elements_syntax_early_errors_grammar_static_privatename_constructor()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-privatename-constructor", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-set-meth-prototype.js")]
    public Task test_elements_syntax_early_errors_grammar_static_set_meth_prototype()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-set-meth-prototype", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/grammar-static-set-meth-super.js")]
    public Task test_elements_syntax_early_errors_grammar_static_set_meth_super()
        => CompilationFailureTest("elements/syntax/early-errors/grammar-static-set-meth-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_fn_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_fn_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-fn-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_fn_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_fn_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-fn-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-heritage-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_heritage_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-heritage-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-heritage-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_heritage_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-heritage-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-heritage-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_heritage_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-heritage-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-heritage-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_heritage_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-heritage-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/field-init-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_field_init_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/field-init-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-fn-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_fn_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-fn-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-fn-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_fn_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-fn-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-fn-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_fn_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-fn-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-fn-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_fn_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-fn-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-heritage-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_heritage_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-heritage-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-heritage-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_heritage_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-heritage-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-heritage-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_heritage_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-heritage-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-heritage-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_heritage_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-heritage-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-outter-call-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_outter_call_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-outter-call-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-outter-call-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_outter_call_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-outter-call-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-outter-member-expression-bad-reference.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_outter_member_expression_bad_reference()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-outter-member-expression-bad-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/invalid-names/method-outter-member-expression-this.js")]
    public Task test_elements_syntax_early_errors_invalid_names_method_outter_member_expression_this()
        => CompilationFailureTest("elements/syntax/early-errors/invalid-names/method-outter-member-expression-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-async-generator-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_async_generator_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-async-generator-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-async-method-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_async_method_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-async-method-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-call-exp-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_call_exp_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-call-exp-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-field-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_field_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-field-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-generator-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_generator_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-generator-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-member-exp-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_member_exp_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-member-exp-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/private-method-cannot-escape-token.js")]
    public Task test_elements_syntax_early_errors_private_method_cannot_escape_token()
        => CompilationFailureTest("elements/syntax/early-errors/private-method-cannot-escape-token", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/early-errors/super-private-access-invalid.js")]
    public Task test_elements_syntax_early_errors_super_private_access_invalid()
        => CompilationFailureTest("elements/syntax/early-errors/super-private-access-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/syntax/valid/grammar-class-body-ctor-no-heritage.js")]
    public Task test_elements_syntax_valid_grammar_class_body_ctor_no_heritage()
        => ExecutionTest("elements/syntax/valid/grammar-class-body-ctor-no-heritage");

    [Fact(DisplayName = "elements/syntax/valid/grammar-field-classelementname-initializer-alt.js")]
    public Task test_elements_syntax_valid_grammar_field_classelementname_initializer_alt()
        => ExecutionTest("elements/syntax/valid/grammar-field-classelementname-initializer-alt");

    [Fact(DisplayName = "elements/syntax/valid/grammar-field-classelementname-initializer.js")]
    public Task test_elements_syntax_valid_grammar_field_classelementname_initializer()
        => ExecutionTest("elements/syntax/valid/grammar-field-classelementname-initializer");

    [Fact(DisplayName = "elements/syntax/valid/grammar-field-identifier-alt.js")]
    public Task test_elements_syntax_valid_grammar_field_identifier_alt()
        => ExecutionTest("elements/syntax/valid/grammar-field-identifier-alt");

    [Fact(DisplayName = "elements/syntax/valid/grammar-field-identifier.js")]
    public Task test_elements_syntax_valid_grammar_field_identifier()
        => ExecutionTest("elements/syntax/valid/grammar-field-identifier");

    [Fact(DisplayName = "elements/syntax/valid/grammar-fields-multi-line.js")]
    public Task test_elements_syntax_valid_grammar_fields_multi_line()
        => ExecutionTest("elements/syntax/valid/grammar-fields-multi-line");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatemeth-duplicate-get-set.js")]
    public Task test_elements_syntax_valid_grammar_privatemeth_duplicate_get_set()
        => ExecutionTest("elements/syntax/valid/grammar-privatemeth-duplicate-get-set");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatemeth-duplicate-meth-nestedclassmeth.js")]
    public Task test_elements_syntax_valid_grammar_privatemeth_duplicate_meth_nestedclassmeth()
        => ExecutionTest("elements/syntax/valid/grammar-privatemeth-duplicate-meth-nestedclassmeth");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatename-classelementname-initializer-alt.js")]
    public Task test_elements_syntax_valid_grammar_privatename_classelementname_initializer_alt()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-classelementname-initializer-alt");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatename-classelementname-initializer.js")]
    public Task test_elements_syntax_valid_grammar_privatename_classelementname_initializer()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-classelementname-initializer");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatename-identifier.js")]
    public Task test_elements_syntax_valid_grammar_privatename_identifier()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-identifier");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatename-no-initializer-with-method.js")]
    public Task test_elements_syntax_valid_grammar_privatename_no_initializer_with_method()
        => ExecutionTest("elements/syntax/valid/grammar-privatename-no-initializer-with-method");

    [Fact(DisplayName = "elements/syntax/valid/grammar-privatenames-multi-line.js")]
    public Task test_elements_syntax_valid_grammar_privatenames_multi_line()
        => ExecutionTest("elements/syntax/valid/grammar-privatenames-multi-line");

    [Fact(DisplayName = "elements/syntax/valid/grammar-special-prototype-accessor-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_accessor_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-accessor-meth-valid");

    [Fact(DisplayName = "elements/syntax/valid/grammar-special-prototype-async-gen-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_async_gen_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-async-gen-meth-valid");

    [Fact(DisplayName = "elements/syntax/valid/grammar-special-prototype-gen-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_gen_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-gen-meth-valid");

    [Fact(DisplayName = "elements/syntax/valid/grammar-special-prototype-meth-valid.js")]
    public Task test_elements_syntax_valid_grammar_special_prototype_meth_valid()
        => ExecutionTest("elements/syntax/valid/grammar-special-prototype-meth-valid");

    [Fact(DisplayName = "elements/syntax/valid/grammar-static-private-async-gen-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_async_gen_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-async-gen-meth-prototype");

    [Fact(DisplayName = "elements/syntax/valid/grammar-static-private-async-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_async_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-async-meth-prototype");

    [Fact(DisplayName = "elements/syntax/valid/grammar-static-private-gen-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_gen_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-gen-meth-prototype");

    [Fact(DisplayName = "elements/syntax/valid/grammar-static-private-meth-prototype.js")]
    public Task test_elements_syntax_valid_grammar_static_private_meth_prototype()
        => ExecutionTest("elements/syntax/valid/grammar-static-private-meth-prototype");

    [Fact(DisplayName = "elements/ternary-init-err-contains-arguments.js")]
    public Task test_elements_ternary_init_err_contains_arguments()
        => CompilationFailureTest("elements/ternary-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/ternary-init-err-contains-super.js")]
    public Task test_elements_ternary_init_err_contains_super()
        => CompilationFailureTest("elements/ternary-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/typeof-init-err-contains-arguments.js")]
    public Task test_elements_typeof_init_err_contains_arguments()
        => CompilationFailureTest("elements/typeof-init-err-contains-arguments", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/typeof-init-err-contains-super.js")]
    public Task test_elements_typeof_init_err_contains_super()
        => CompilationFailureTest("elements/typeof-init-err-contains-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "elements/wrapped-in-sc-computed-symbol-names.js")]
    public Task test_elements_wrapped_in_sc_computed_symbol_names()
        => ExecutionTest("elements/wrapped-in-sc-computed-symbol-names");

    [Fact(DisplayName = "elements/wrapped-in-sc-grammar-privatename-identifier-semantics-stringvalue.js")]
    public Task test_elements_wrapped_in_sc_grammar_privatename_identifier_semantics_stringvalue()
        => ExecutionTest("elements/wrapped-in-sc-grammar-privatename-identifier-semantics-stringvalue");

    [Fact(DisplayName = "elements/wrapped-in-sc-private-field-usage.js")]
    public Task test_elements_wrapped_in_sc_private_field_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-field-usage");

    [Fact(DisplayName = "elements/wrapped-in-sc-private-method-getter-usage.js")]
    public Task test_elements_wrapped_in_sc_private_method_getter_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-method-getter-usage");

    [Fact(DisplayName = "elements/wrapped-in-sc-private-method-usage.js")]
    public Task test_elements_wrapped_in_sc_private_method_usage()
        => ExecutionTest("elements/wrapped-in-sc-private-method-usage");

    [Fact(DisplayName = "elements/wrapped-in-sc-private-names.js")]
    public Task test_elements_wrapped_in_sc_private_names()
        => ExecutionTest("elements/wrapped-in-sc-private-names");
}
