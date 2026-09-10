using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.generators;

public sealed class GeneratorFunctionDeclarationConformance11BatchTests : DiskExecutionTestsBase
{
    public GeneratorFunctionDeclarationConformance11BatchTests() : base("language.statements.generators") { }

    [Fact(DisplayName = "arguments-with-arguments-fn.js")]
    public Task test_arguments_with_arguments_fn()
        => ExecutionTest("arguments-with-arguments-fn");

    [Fact(DisplayName = "arguments-with-arguments-lex.js")]
    public Task test_arguments_with_arguments_lex()
        => ExecutionTest("arguments-with-arguments-lex");

    [Fact(DisplayName = "declaration.js")]
    public Task test_declaration()
        => ExecutionTest("declaration");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined.js")]
    public Task test_dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined.js")]
    public Task test_dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior.js")]
    public Task test_dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-trailing-comma.js")]
    public Task test_dflt_params_trailing_comma()
        => ExecutionTest("dflt-params-trailing-comma");

    [Fact(DisplayName = "ary-init-iter-close.js")]
    public Task test_dstr_ary_init_iter_close()
        => ExecutionTest("dstr/ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-no-close.js")]
    public Task test_dstr_ary_init_iter_no_close()
        => ExecutionTest("dstr/ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val.js")]
    public Task test_dstr_ary_name_iter_val()
        => ExecutionTest("dstr/ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "ary-ptrn-elision.js")]
    public Task test_dstr_ary_ptrn_elision()
        => ExecutionTest("dstr/ary-ptrn-elision");

    [Fact(DisplayName = "ary-ptrn-empty.js")]
    public Task test_dstr_ary_ptrn_empty()
        => ExecutionTest("dstr/ary-ptrn-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "ary-ptrn-rest-id.js")]
    public Task test_dstr_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-init-iter-close.js")]
    public Task test_dstr_dflt_ary_init_iter_close()
        => ExecutionTest("dstr/dflt-ary-init-iter-close");

    [Fact(DisplayName = "dflt-ary-init-iter-no-close.js")]
    public Task test_dstr_dflt_ary_init_iter_no_close()
        => ExecutionTest("dstr/dflt-ary-init-iter-no-close");

    [Fact(DisplayName = "dflt-ary-name-iter-val.js")]
    public Task test_dstr_dflt_ary_name_iter_val()
        => ExecutionTest("dstr/dflt-ary-name-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elem-iter.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-elision-iter.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-empty-iter.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-ary-rest-iter.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-exhausted.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-arrow.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_fn_name_arrow()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-class.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_fn_name_class()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-cover.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_fn_name_cover()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-fn.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_fn_name_fn()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-fn-name-gen.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_fn_name_gen()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-hole.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_hole()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-skipped.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_skipped()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-init-undef.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_init_undef()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-complete.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_complete()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-done.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_done()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_val_array_prototype()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-id-iter-val.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_id_iter_val()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_id_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-id.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_prop_id_init()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dflt-ary-ptrn-elem-obj-prop-id.js")]
    public Task test_dstr_dflt_ary_ptrn_elem_obj_prop_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dflt-ary-ptrn-elision-exhausted.js")]
    public Task test_dstr_dflt_ary_ptrn_elision_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-elision.js")]
    public Task test_dstr_dflt_ary_ptrn_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-empty.js")]
    public Task test_dstr_dflt_ary_ptrn_empty()
        => ExecutionTest("dstr/dflt-ary-ptrn-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elem.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_ary_elem()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-elision.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_ary_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-empty.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_ary_empty()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-ary-rest.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_ary_rest()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-direct.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_direct()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-elision.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_elision()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id-exhausted.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id_exhausted()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-id.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-id.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_obj_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dflt-ary-ptrn-rest-obj-prop-id.js")]
    public Task test_dstr_dflt_ary_ptrn_rest_obj_prop_id()
        => ExecutionTest("dstr/dflt-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dflt-obj-ptrn-empty.js")]
    public Task test_dstr_dflt_obj_ptrn_empty()
        => ExecutionTest("dstr/dflt-obj-ptrn-empty");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dflt-obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_dflt_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_dflt_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-init.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-id.js")]
    public Task test_dstr_dflt_obj_ptrn_prop_id()
        => ExecutionTest("dstr/dflt-obj-ptrn-prop-id");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-getter.js")]
    public Task test_dstr_dflt_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_dflt_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dflt-obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_dflt_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/dflt-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "obj-ptrn-empty.js")]
    public Task test_dstr_obj_ptrn_empty()
        => ExecutionTest("dstr/obj-ptrn-empty");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-arrow.js")]
    public Task test_dstr_obj_ptrn_id_init_fn_name_arrow()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-class.js")]
    public Task test_dstr_obj_ptrn_id_init_fn_name_class()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-cover.js")]
    public Task test_dstr_obj_ptrn_id_init_fn_name_cover()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-fn.js")]
    public Task test_dstr_obj_ptrn_id_init_fn_name_fn()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-gen.js")]
    public Task test_dstr_obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("dstr/obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "obj-ptrn-id-init-skipped.js")]
    public Task test_dstr_obj_ptrn_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-id-trailing-comma.js")]
    public Task test_dstr_obj_ptrn_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-ary-trailing-comma.js")]
    public Task test_dstr_obj_ptrn_prop_ary_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-id-init-skipped.js")]
    public Task test_dstr_obj_ptrn_prop_id_init_skipped()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "obj-ptrn-prop-id-init.js")]
    public Task test_dstr_obj_ptrn_prop_id_init()
        => ExecutionTest("dstr/obj-ptrn-prop-id-init");

    [Fact(DisplayName = "obj-ptrn-prop-id-trailing-comma.js")]
    public Task test_dstr_obj_ptrn_prop_id_trailing_comma()
        => ExecutionTest("dstr/obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "obj-ptrn-prop-id.js")]
    public Task test_dstr_obj_ptrn_prop_id()
        => ExecutionTest("dstr/obj-ptrn-prop-id");

    [Fact(DisplayName = "obj-ptrn-rest-getter.js")]
    public Task test_dstr_obj_ptrn_rest_getter()
        => ExecutionTest("dstr/obj-ptrn-rest-getter");

    [Fact(DisplayName = "obj-ptrn-rest-skip-non-enumerable.js")]
    public Task test_dstr_obj_ptrn_rest_skip_non_enumerable()
        => ExecutionTest("dstr/obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "obj-ptrn-rest-val-obj.js")]
    public Task test_dstr_obj_ptrn_rest_val_obj()
        => ExecutionTest("dstr/obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "gen-func-decl-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_forbidden_ext_b1_gen_func_decl_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("forbidden-ext/b1/gen-func-decl-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "gen-func-decl-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_forbidden_ext_b1_gen_func_decl_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("forbidden-ext/b1/gen-func-decl-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_forbidden_ext_b2_gen_func_decl_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("forbidden-ext/b2/gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_forbidden_ext_b2_gen_func_decl_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("forbidden-ext/b2/gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "gen-func-decl-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_forbidden_ext_b2_gen_func_decl_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("forbidden-ext/b2/gen-func-decl-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "generator-created-after-decl-inst.js")]
    public Task test_generator_created_after_decl_inst()
        => ExecutionTest("generator-created-after-decl-inst");

    [Fact(DisplayName = "has-instance.js")]
    public Task test_has_instance()
        => ExecutionTest("has-instance");

    [Fact(DisplayName = "invoke-as-constructor.js")]
    public Task test_invoke_as_constructor()
        => ExecutionTest("invoke-as-constructor");

    [Fact(DisplayName = "length-dflt.js")]
    public Task test_length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "length-property-descriptor.js")]
    public Task test_length_property_descriptor()
        => ExecutionTest("length-property-descriptor");

    [Fact(DisplayName = "name.js")]
    public Task test_name()
        => ExecutionTest("name");

    [Fact(DisplayName = "no-yield.js")]
    public Task test_no_yield()
        => ExecutionTest("no-yield");

    [Fact(DisplayName = "params-dflt-args-unmapped.js")]
    public Task test_params_dflt_args_unmapped()
        => ExecutionTest("params-dflt-args-unmapped");

    [Fact(DisplayName = "params-dflt-ref-arguments.js")]
    public Task test_params_dflt_ref_arguments()
        => ExecutionTest("params-dflt-ref-arguments");

    [Fact(DisplayName = "params-trailing-comma-multiple.js")]
    public Task test_params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single.js")]
    public Task test_params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "prototype-own-properties.js")]
    public Task test_prototype_own_properties()
        => ExecutionTest("prototype-own-properties");

    [Fact(DisplayName = "prototype-property-descriptor.js")]
    public Task test_prototype_property_descriptor()
        => ExecutionTest("prototype-property-descriptor");

    [Fact(DisplayName = "prototype-relation-to-function.js")]
    public Task test_prototype_relation_to_function()
        => ExecutionTest("prototype-relation-to-function");

    [Fact(DisplayName = "prototype-typeof.js")]
    public Task test_prototype_typeof()
        => ExecutionTest("prototype-typeof");

    [Fact(DisplayName = "prototype-uniqueness.js")]
    public Task test_prototype_uniqueness()
        => ExecutionTest("prototype-uniqueness");

    [Fact(DisplayName = "prototype-value.js")]
    public Task test_prototype_value()
        => ExecutionTest("prototype-value");

    [Fact(DisplayName = "restricted-properties.js")]
    public Task test_restricted_properties()
        => ExecutionTest("restricted-properties");

    [Fact(DisplayName = "return.js")]
    public Task test_return()
        => ExecutionTest("return");

    [Fact(DisplayName = "scope-param-elem-var-close.js")]
    public Task test_scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open.js")]
    public Task test_scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");

    [Fact(DisplayName = "scope-param-rest-elem-var-close.js")]
    public Task test_scope_param_rest_elem_var_close()
        => ExecutionTest("scope-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-param-rest-elem-var-open.js")]
    public Task test_scope_param_rest_elem_var_open()
        => ExecutionTest("scope-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-paramsbody-var-close.js")]
    public Task test_scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");

    [Fact(DisplayName = "scope-paramsbody-var-open.js")]
    public Task test_scope_paramsbody_var_open()
        => ExecutionTest("scope-paramsbody-var-open");

    [Fact(DisplayName = "yield-as-function-expression-binding-identifier.js")]
    public Task test_yield_as_function_expression_binding_identifier()
        => ExecutionTest("yield-as-function-expression-binding-identifier");

    [Fact(DisplayName = "yield-as-generator-declaration-binding-identifier.js")]
    public Task test_yield_as_generator_declaration_binding_identifier()
        => ExecutionTest("yield-as-generator-declaration-binding-identifier");

    [Fact(DisplayName = "yield-as-identifier-in-nested-function.js")]
    public Task test_yield_as_identifier_in_nested_function()
        => ExecutionTest("yield-as-identifier-in-nested-function");

    [Fact(DisplayName = "yield-as-literal-property-name.js")]
    public Task test_yield_as_literal_property_name()
        => ExecutionTest("yield-as-literal-property-name");

    [Fact(DisplayName = "yield-as-property-name.js")]
    public Task test_yield_as_property_name()
        => ExecutionTest("yield-as-property-name");

    [Fact(DisplayName = "yield-as-statement.js")]
    public Task test_yield_as_statement()
        => ExecutionTest("yield-as-statement");

    [Fact(DisplayName = "yield-as-yield-operand.js")]
    public Task test_yield_as_yield_operand()
        => ExecutionTest("yield-as-yield-operand");

    [Fact(DisplayName = "yield-identifier-non-strict.js")]
    public Task test_yield_identifier_non_strict()
        => ExecutionTest("yield-identifier-non-strict");

    [Fact(DisplayName = "yield-identifier-spread-non-strict.js")]
    public Task test_yield_identifier_spread_non_strict()
        => ExecutionTest("yield-identifier-spread-non-strict");

    [Fact(DisplayName = "yield-newline.js")]
    public Task test_yield_newline()
        => ExecutionTest("yield-newline");

    [Fact(DisplayName = "yield-spread-arr-multiple.js")]
    public Task test_yield_spread_arr_multiple()
        => ExecutionTest("yield-spread-arr-multiple");

    [Fact(DisplayName = "yield-spread-arr-single.js")]
    public Task test_yield_spread_arr_single()
        => ExecutionTest("yield-spread-arr-single");

    [Fact(DisplayName = "yield-spread-obj.js")]
    public Task test_yield_spread_obj()
        => ExecutionTest("yield-spread-obj");

    [Fact(DisplayName = "yield-star-before-newline.js")]
    public Task test_yield_star_before_newline()
        => ExecutionTest("yield-star-before-newline");

}
