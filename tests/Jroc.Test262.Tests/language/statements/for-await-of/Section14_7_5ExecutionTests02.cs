namespace Jroc.Test262.Tests.language.statements.for_await_of;

public class Section14_7_5ExecutionTests02 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests02() : base("language/statements/for-await-of", "language.statements.for_await_of") { }

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-empty.js")]
    public Task async_func_dstr_const_obj_ptrn_empty_251()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-get-value-err.js")]
    public Task async_func_dstr_const_obj_ptrn_id_get_value_err_252()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_id_init_fn_name_arrow_253()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_id_init_fn_name_class_254()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_id_init_fn_name_cover_255()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_id_init_fn_name_fn_256()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_id_init_fn_name_gen_257()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-skipped.js")]
    public Task async_func_dstr_const_obj_ptrn_id_init_skipped_258()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-throws.js")]
    public Task async_func_dstr_const_obj_ptrn_id_init_throws_259()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-init-unresolvable.js")]
    public Task async_func_dstr_const_obj_ptrn_id_init_unresolvable_260()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-id-trailing-comma.js")]
    public Task async_func_dstr_const_obj_ptrn_id_trailing_comma_261()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-list-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_list_err_262()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-list-err");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-ary-init.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_ary_init_263()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_ary_trailing_comma_264()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-ary-value-null.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_ary_value_null_265()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-ary.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_ary_266()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-eval-err.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_eval_err_267()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-get-value-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_get_value_err_268()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-init-skipped.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_init_skipped_269()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-init-throws.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_init_throws_270()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_init_unresolvable_271()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-init.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_init_272()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id-trailing-comma.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_trailing_comma_273()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-id.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_id_274()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-obj-init.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_obj_init_275()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-obj-value-null.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_obj_value_null_276()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-obj-value-undef.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_obj_value_undef_277()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-prop-obj.js")]
    public Task async_func_dstr_const_obj_ptrn_prop_obj_278()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-rest-getter.js", Skip = "Pending async object-accessor analysis.")]
    public Task async_func_dstr_const_obj_ptrn_rest_getter_279()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task async_func_dstr_const_obj_ptrn_rest_skip_non_enumerable_280()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-func-dstr-const-obj-ptrn-rest-val-obj.js")]
    public Task async_func_dstr_const_obj_ptrn_rest_val_obj_281()
        => ExecutionTest("async-func-dstr-const-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-func-dstr-let-ary-init-iter-close.js")]
    public Task async_func_dstr_let_ary_init_iter_close_282()
        => ExecutionTest("async-func-dstr-let-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-let-ary-init-iter-get-err.js")]
    public Task async_func_dstr_let_ary_init_iter_get_err_283()
        => ExecutionTest("async-func-dstr-let-ary-init-iter-get-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-init-iter-no-close.js")]
    public Task async_func_dstr_let_ary_init_iter_no_close_284()
        => ExecutionTest("async-func-dstr-let-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-let-ary-name-iter-val.js")]
    public Task async_func_dstr_let_ary_name_iter_val_285()
        => ExecutionTest("async-func-dstr-let-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_elem_init_286()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-elem-iter.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_elem_iter_287()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-elision-init.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_elision_init_288()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-elision-iter.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_elision_iter_289()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_empty_init_290()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_empty_iter_291()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-rest-init.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_rest_init_292()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_rest_iter_293()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-ary-val-null.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_ary_val_null_294()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-exhausted.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_exhausted_295()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_fn_name_arrow_296()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_fn_name_class_297()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_fn_name_cover_298()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_fn_name_fn_299()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_fn_name_gen_300()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-hole.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_hole_301()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-skipped.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_skipped_302()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_throws_303()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-undef.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_undef_304()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_init_unresolvable_305()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-iter-complete.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_iter_complete_306()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-iter-done.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_iter_done_307()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-iter-step-err.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_iter_step_err_308()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-iter-val-err.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_iter_val_err_309()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-id-iter-val.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_id_iter_val_310()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-id-init.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_id_init_311()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-id.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_id_312()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_prop_id_init_313()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-prop-id.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_prop_id_314()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-val-null.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_val_null_315()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elem-obj-val-undef.js")]
    public Task async_func_dstr_let_ary_ptrn_elem_obj_val_undef_316()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elision-exhausted.js")]
    public Task async_func_dstr_let_ary_ptrn_elision_exhausted_317()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elision-iter-close.js")]
    public Task async_func_dstr_let_ary_ptrn_elision_iter_close_318()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elision-step-err.js")]
    public Task async_func_dstr_let_ary_ptrn_elision_step_err_319()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-elision.js")]
    public Task async_func_dstr_let_ary_ptrn_elision_320()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-elision");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-empty.js")]
    public Task async_func_dstr_let_ary_ptrn_empty_321()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-ary-elem.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_ary_elem_322()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-ary-elision.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_ary_elision_323()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-ary-empty.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_ary_empty_324()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-ary-rest.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_ary_rest_325()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-elision-next-err.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_elision_next_err_326()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-elision.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_elision_327()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-exhausted.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_exhausted_328()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-iter-close.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_iter_close_329()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-iter-step-err.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_iter_step_err_330()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id-iter-val-err.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_iter_val_err_331()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-id.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_id_332()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-init-ary.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_init_ary_333()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-init-id.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_init_id_334()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-init-obj.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_init_obj_335()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-not-final-ary.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_not_final_ary_336()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-not-final-id.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_not_final_id_337()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-not-final-obj.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_not_final_obj_338()
        => CompilationFailureTest("async-func-dstr-let-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-obj-id.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_obj_id_339()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-func-dstr-let-ary-ptrn-rest-obj-prop-id.js")]
    public Task async_func_dstr_let_ary_ptrn_rest_obj_prop_id_340()
        => ExecutionTest("async-func-dstr-let-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-init-iter-close.js")]
    public Task async_func_dstr_let_async_ary_init_iter_close_341()
        => ExecutionTest("async-func-dstr-let-async-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-init-iter-no-close.js")]
    public Task async_func_dstr_let_async_ary_init_iter_no_close_342()
        => ExecutionTest("async-func-dstr-let-async-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-name-iter-val.js")]
    public Task async_func_dstr_let_async_ary_name_iter_val_343()
        => ExecutionTest("async-func-dstr-let-async-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_elem_init_344()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-elem-iter.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_elem_iter_345()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-elision-init.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_elision_init_346()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-elision-iter.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_elision_iter_347()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_empty_init_348()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_empty_iter_349()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-rest-init.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_rest_init_350()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_ary_rest_iter_351()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-exhausted.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_exhausted_352()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_fn_name_arrow_353()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_fn_name_class_354()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_fn_name_cover_355()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_fn_name_fn_356()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_fn_name_gen_357()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-hole.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_hole_358()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-skipped.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_skipped_359()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-init-undef.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_init_undef_360()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-iter-complete.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_iter_complete_361()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-iter-done.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_iter_done_362()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-id-iter-val.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_id_iter_val_363()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-obj-id-init.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_obj_id_init_364()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-obj-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_obj_id_365()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_obj_prop_id_init_366()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elem-obj-prop-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elem_obj_prop_id_367()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elision-exhausted.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elision_exhausted_368()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-elision.js")]
    public Task async_func_dstr_let_async_ary_ptrn_elision_369()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-elision");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-empty.js")]
    public Task async_func_dstr_let_async_ary_ptrn_empty_370()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-ary-elem.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_ary_elem_371()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-ary-elision.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_ary_elision_372()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-ary-empty.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_ary_empty_373()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-ary-rest.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_ary_rest_374()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-id-elision.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_id_elision_375()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-id-exhausted.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_id_exhausted_376()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_id_377()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-init-ary.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_init_ary_378()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-init-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_init_id_379()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-init-obj.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_init_obj_380()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-not-final-ary.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_not_final_ary_381()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-not-final-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_not_final_id_382()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-not-final-obj.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_not_final_obj_383()
        => CompilationFailureTest("async-func-dstr-let-async-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-obj-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_obj_id_384()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-func-dstr-let-async-ary-ptrn-rest-obj-prop-id.js")]
    public Task async_func_dstr_let_async_ary_ptrn_rest_obj_prop_id_385()
        => ExecutionTest("async-func-dstr-let-async-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-empty.js")]
    public Task async_func_dstr_let_async_obj_ptrn_empty_386()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_fn_name_arrow_387()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_fn_name_class_388()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_fn_name_cover_389()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_fn_name_fn_390()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_fn_name_gen_391()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-init-skipped.js")]
    public Task async_func_dstr_let_async_obj_ptrn_id_init_skipped_392()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-id-trailing-comma.js")]
    public Task async_func_dstr_let_async_obj_ptrn_id_trailing_comma_393()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-ary-init.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_ary_init_394()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_ary_trailing_comma_395()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-ary.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_ary_396()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-id-init-skipped.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_id_init_skipped_397()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-id-init.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_id_init_398()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-id-trailing-comma.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_id_trailing_comma_399()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-id.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_id_400()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-obj-init.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_obj_init_401()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-prop-obj.js")]
    public Task async_func_dstr_let_async_obj_ptrn_prop_obj_402()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-rest-getter.js")]
    public Task async_func_dstr_let_async_obj_ptrn_rest_getter_403()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task async_func_dstr_let_async_obj_ptrn_rest_skip_non_enumerable_404()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-func-dstr-let-async-obj-ptrn-rest-val-obj.js")]
    public Task async_func_dstr_let_async_obj_ptrn_rest_val_obj_405()
        => ExecutionTest("async-func-dstr-let-async-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-func-dstr-let-obj-init-null.js")]
    public Task async_func_dstr_let_obj_init_null_406()
        => ExecutionTest("async-func-dstr-let-obj-init-null");

    [Fact(DisplayName = "async-func-dstr-let-obj-init-undefined.js")]
    public Task async_func_dstr_let_obj_init_undefined_407()
        => ExecutionTest("async-func-dstr-let-obj-init-undefined");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-get-value-err.js")]
    public Task async_func_dstr_let_obj_ptrn_id_get_value_err_408()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_id_init_fn_name_arrow_409()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_id_init_fn_name_class_410()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_id_init_fn_name_cover_411()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_id_init_fn_name_fn_412()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_id_init_fn_name_gen_413()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-skipped.js")]
    public Task async_func_dstr_let_obj_ptrn_id_init_skipped_414()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-throws.js")]
    public Task async_func_dstr_let_obj_ptrn_id_init_throws_415()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-init-unresolvable.js")]
    public Task async_func_dstr_let_obj_ptrn_id_init_unresolvable_416()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-id-trailing-comma.js")]
    public Task async_func_dstr_let_obj_ptrn_id_trailing_comma_417()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-list-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_list_err_418()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-list-err");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-ary-init.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_ary_init_419()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_ary_trailing_comma_420()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-ary-value-null.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_ary_value_null_421()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-ary.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_ary_422()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-eval-err.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_eval_err_423()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-get-value-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_get_value_err_424()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-init-skipped.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_init_skipped_425()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-init-throws.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_init_throws_426()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_init_unresolvable_427()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-init.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_init_428()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id-trailing-comma.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_trailing_comma_429()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-id.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_id_430()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-obj-init.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_obj_init_431()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-obj-value-null.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_obj_value_null_432()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-obj-value-undef.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_obj_value_undef_433()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-prop-obj.js")]
    public Task async_func_dstr_let_obj_ptrn_prop_obj_434()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-rest-getter.js", Skip = "Pending async object-accessor analysis.")]
    public Task async_func_dstr_let_obj_ptrn_rest_getter_435()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task async_func_dstr_let_obj_ptrn_rest_skip_non_enumerable_436()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-rest-val-obj.js")]
    public Task async_func_dstr_let_obj_ptrn_rest_val_obj_437()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-func-dstr-var-ary-init-iter-close.js")]
    public Task async_func_dstr_var_ary_init_iter_close_438()
        => ExecutionTest("async-func-dstr-var-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-var-ary-init-iter-get-err.js")]
    public Task async_func_dstr_var_ary_init_iter_get_err_439()
        => ExecutionTest("async-func-dstr-var-ary-init-iter-get-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-init-iter-no-close.js")]
    public Task async_func_dstr_var_ary_init_iter_no_close_440()
        => ExecutionTest("async-func-dstr-var-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-var-ary-name-iter-val.js")]
    public Task async_func_dstr_var_ary_name_iter_val_441()
        => ExecutionTest("async-func-dstr-var-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_elem_init_442()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-elem-iter.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_elem_iter_443()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-elision-init.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_elision_init_444()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-elision-iter.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_elision_iter_445()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_empty_init_446()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_empty_iter_447()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-rest-init.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_rest_init_448()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_rest_iter_449()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-ary-val-null.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_ary_val_null_450()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-exhausted.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_exhausted_451()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_fn_name_arrow_452()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_fn_name_class_453()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_fn_name_cover_454()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_fn_name_fn_455()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_fn_name_gen_456()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-hole.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_hole_457()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-skipped.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_skipped_458()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_throws_459()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-undef.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_undef_460()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_init_unresolvable_461()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-iter-complete.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_iter_complete_462()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-iter-done.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_iter_done_463()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-iter-step-err.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_iter_step_err_464()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-iter-val-err.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_iter_val_err_465()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-id-iter-val.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_id_iter_val_466()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-id-init.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_id_init_467()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-id.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_id_468()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_prop_id_init_469()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-prop-id.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_prop_id_470()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-val-null.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_val_null_471()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elem-obj-val-undef.js")]
    public Task async_func_dstr_var_ary_ptrn_elem_obj_val_undef_472()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elision-exhausted.js")]
    public Task async_func_dstr_var_ary_ptrn_elision_exhausted_473()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elision-iter-close.js")]
    public Task async_func_dstr_var_ary_ptrn_elision_iter_close_474()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elision-step-err.js")]
    public Task async_func_dstr_var_ary_ptrn_elision_step_err_475()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-elision.js")]
    public Task async_func_dstr_var_ary_ptrn_elision_476()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-elision");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-empty.js")]
    public Task async_func_dstr_var_ary_ptrn_empty_477()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-ary-elem.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_ary_elem_478()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-ary-elision.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_ary_elision_479()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-ary-empty.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_ary_empty_480()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-ary-rest.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_ary_rest_481()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-elision-next-err.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_elision_next_err_482()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-elision.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_elision_483()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-exhausted.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_exhausted_484()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-iter-close.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_iter_close_485()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-iter-step-err.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_iter_step_err_486()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id-iter-val-err.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_iter_val_err_487()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-id.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_id_488()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-init-ary.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_init_ary_489()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-init-id.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_init_id_490()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-init-obj.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_init_obj_491()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-not-final-ary.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_not_final_ary_492()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-not-final-id.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_not_final_id_493()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-not-final-obj.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_not_final_obj_494()
        => CompilationFailureTest("async-func-dstr-var-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-obj-id.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_obj_id_495()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-func-dstr-var-ary-ptrn-rest-obj-prop-id.js")]
    public Task async_func_dstr_var_ary_ptrn_rest_obj_prop_id_496()
        => ExecutionTest("async-func-dstr-var-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-var-async-ary-init-iter-close.js")]
    public Task async_func_dstr_var_async_ary_init_iter_close_497()
        => ExecutionTest("async-func-dstr-var-async-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-var-async-ary-init-iter-no-close.js")]
    public Task async_func_dstr_var_async_ary_init_iter_no_close_498()
        => ExecutionTest("async-func-dstr-var-async-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-var-async-ary-name-iter-val.js")]
    public Task async_func_dstr_var_async_ary_name_iter_val_499()
        => ExecutionTest("async-func-dstr-var-async-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-var-async-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_var_async_ary_ptrn_elem_ary_elem_init_500()
        => ExecutionTest("async-func-dstr-var-async-ary-ptrn-elem-ary-elem-init");
}
