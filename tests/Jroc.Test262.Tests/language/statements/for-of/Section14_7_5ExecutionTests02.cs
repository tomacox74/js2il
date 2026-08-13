namespace Jroc.Test262.Tests.language.statements.for_of;

public class Section14_7_5ExecutionTests02 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests02() : base("language/statements/for-of", "language.statements.for_of") { }

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-throws.js")]
    public Task dstr_const_obj_ptrn_id_init_throws_251()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-init-unresolvable.js")]
    public Task dstr_const_obj_ptrn_id_init_unresolvable_252()
        => ExecutionTest("dstr/const-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dstr/const-obj-ptrn-id-trailing-comma.js")]
    public Task dstr_const_obj_ptrn_id_trailing_comma_253()
        => ExecutionTest("dstr/const-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/const-obj-ptrn-init-err.js")]
    public Task dstr_const_obj_ptrn_init_err_254()
        => CompilationFailureTest("dstr/const-obj-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/const-obj-ptrn-list-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_list_err_255()
        => ExecutionTest("dstr/const-obj-ptrn-list-err");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-ary-init.js")]
    public Task dstr_const_obj_ptrn_prop_ary_init_256()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task dstr_const_obj_ptrn_prop_ary_trailing_comma_257()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-ary-value-null.js")]
    public Task dstr_const_obj_ptrn_prop_ary_value_null_258()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-ary.js")]
    public Task dstr_const_obj_ptrn_prop_ary_259()
        => ExecutionTest("dstr/const-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-eval-err.js")]
    public Task dstr_const_obj_ptrn_prop_eval_err_260()
        => ExecutionTest("dstr/const-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-get-value-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_const_obj_ptrn_prop_id_get_value_err_261()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-init-skipped.js")]
    public Task dstr_const_obj_ptrn_prop_id_init_skipped_262()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-init-throws.js")]
    public Task dstr_const_obj_ptrn_prop_id_init_throws_263()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task dstr_const_obj_ptrn_prop_id_init_unresolvable_264()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-init.js")]
    public Task dstr_const_obj_ptrn_prop_id_init_265()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id-trailing-comma.js")]
    public Task dstr_const_obj_ptrn_prop_id_trailing_comma_266()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-id.js")]
    public Task dstr_const_obj_ptrn_prop_id_267()
        => ExecutionTest("dstr/const-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-obj-init.js")]
    public Task dstr_const_obj_ptrn_prop_obj_init_268()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-obj-value-null.js")]
    public Task dstr_const_obj_ptrn_prop_obj_value_null_269()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-obj-value-undef.js")]
    public Task dstr_const_obj_ptrn_prop_obj_value_undef_270()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "dstr/const-obj-ptrn-prop-obj.js")]
    public Task dstr_const_obj_ptrn_prop_obj_271()
        => ExecutionTest("dstr/const-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dstr/const-obj-ptrn-rest-getter.js", Skip = "Pending async object-accessor analysis.")]
    public Task dstr_const_obj_ptrn_rest_getter_272()
        => ExecutionTest("dstr/const-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/const-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task dstr_const_obj_ptrn_rest_skip_non_enumerable_273()
        => ExecutionTest("dstr/const-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/const-obj-ptrn-rest-val-obj.js")]
    public Task dstr_const_obj_ptrn_rest_val_obj_274()
        => ExecutionTest("dstr/const-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "dstr/let-ary-init-iter-close.js")]
    public Task dstr_let_ary_init_iter_close_275()
        => ExecutionTest("dstr/let-ary-init-iter-close");

    [Fact(DisplayName = "dstr/let-ary-init-iter-get-err-array-prototype.js")]
    public Task dstr_let_ary_init_iter_get_err_array_prototype_276()
        => ExecutionTest("dstr/let-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dstr/let-ary-init-iter-get-err.js")]
    public Task dstr_let_ary_init_iter_get_err_277()
        => ExecutionTest("dstr/let-ary-init-iter-get-err");

    [Fact(DisplayName = "dstr/let-ary-init-iter-no-close.js")]
    public Task dstr_let_ary_init_iter_no_close_278()
        => ExecutionTest("dstr/let-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/let-ary-name-iter-val.js")]
    public Task dstr_let_ary_name_iter_val_279()
        => ExecutionTest("dstr/let-ary-name-iter-val");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-elem-init.js")]
    public Task dstr_let_ary_ptrn_elem_ary_elem_init_280()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-elem-iter.js")]
    public Task dstr_let_ary_ptrn_elem_ary_elem_iter_281()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-elision-init.js")]
    public Task dstr_let_ary_ptrn_elem_ary_elision_init_282()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-elision-iter.js")]
    public Task dstr_let_ary_ptrn_elem_ary_elision_iter_283()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_ary_empty_init_284()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_ary_empty_iter_285()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-rest-init.js")]
    public Task dstr_let_ary_ptrn_elem_ary_rest_init_286()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_ary_rest_iter_287()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-ary-val-null.js")]
    public Task dstr_let_ary_ptrn_elem_ary_val_null_288()
        => ExecutionTest("dstr/let-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-exhausted.js")]
    public Task dstr_let_ary_ptrn_elem_id_init_exhausted_289()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_fn_name_arrow_290()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_fn_name_class_291()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_fn_name_cover_292()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_fn_name_fn_293()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_fn_name_gen_294()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-hole.js")]
    public Task dstr_let_ary_ptrn_elem_id_init_hole_295()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-skipped.js")]
    public Task dstr_let_ary_ptrn_elem_id_init_skipped_296()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_ary_ptrn_elem_id_init_throws_297()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-undef.js")]
    public Task dstr_let_ary_ptrn_elem_id_init_undef_298()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task dstr_let_ary_ptrn_elem_id_init_unresolvable_299()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-complete.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_complete_300()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-done.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_done_301()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-step-err.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_step_err_302()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_val_array_prototype_303()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-val-err.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_val_err_304()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-id-iter-val.js")]
    public Task dstr_let_ary_ptrn_elem_id_iter_val_305()
        => ExecutionTest("dstr/let-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-id-init.js")]
    public Task dstr_let_ary_ptrn_elem_obj_id_init_306()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-id.js")]
    public Task dstr_let_ary_ptrn_elem_obj_id_307()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task dstr_let_ary_ptrn_elem_obj_prop_id_init_308()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-prop-id.js")]
    public Task dstr_let_ary_ptrn_elem_obj_prop_id_309()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-val-null.js")]
    public Task dstr_let_ary_ptrn_elem_obj_val_null_310()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elem-obj-val-undef.js")]
    public Task dstr_let_ary_ptrn_elem_obj_val_undef_311()
        => ExecutionTest("dstr/let-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elision-exhausted.js")]
    public Task dstr_let_ary_ptrn_elision_exhausted_312()
        => ExecutionTest("dstr/let-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elision-iter-close.js")]
    public Task dstr_let_ary_ptrn_elision_iter_close_313()
        => ExecutionTest("dstr/let-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elision-step-err.js")]
    public Task dstr_let_ary_ptrn_elision_step_err_314()
        => ExecutionTest("dstr/let-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-elision.js")]
    public Task dstr_let_ary_ptrn_elision_315()
        => ExecutionTest("dstr/let-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/let-ary-ptrn-empty.js")]
    public Task dstr_let_ary_ptrn_empty_316()
        => ExecutionTest("dstr/let-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/let-ary-ptrn-init-err.js")]
    public Task dstr_let_ary_ptrn_init_err_317()
        => CompilationFailureTest("dstr/let-ary-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-ary-elem.js")]
    public Task dstr_let_ary_ptrn_rest_ary_elem_318()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-ary-elision.js")]
    public Task dstr_let_ary_ptrn_rest_ary_elision_319()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-ary-empty.js")]
    public Task dstr_let_ary_ptrn_rest_ary_empty_320()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-ary-rest.js")]
    public Task dstr_let_ary_ptrn_rest_ary_rest_321()
        => ExecutionTest("dstr/let-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-direct.js")]
    public Task dstr_let_ary_ptrn_rest_id_direct_322()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-elision-next-err.js")]
    public Task dstr_let_ary_ptrn_rest_id_elision_next_err_323()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-elision.js")]
    public Task dstr_let_ary_ptrn_rest_id_elision_324()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-exhausted.js")]
    public Task dstr_let_ary_ptrn_rest_id_exhausted_325()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-iter-close.js")]
    public Task dstr_let_ary_ptrn_rest_id_iter_close_326()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-iter-step-err.js")]
    public Task dstr_let_ary_ptrn_rest_id_iter_step_err_327()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id-iter-val-err.js")]
    public Task dstr_let_ary_ptrn_rest_id_iter_val_err_328()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-id.js")]
    public Task dstr_let_ary_ptrn_rest_id_329()
        => ExecutionTest("dstr/let-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-init-ary.js")]
    public Task dstr_let_ary_ptrn_rest_init_ary_330()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-init-id.js")]
    public Task dstr_let_ary_ptrn_rest_init_id_331()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-init-obj.js")]
    public Task dstr_let_ary_ptrn_rest_init_obj_332()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_let_ary_ptrn_rest_not_final_ary_333()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-not-final-id.js")]
    public Task dstr_let_ary_ptrn_rest_not_final_id_334()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_let_ary_ptrn_rest_not_final_obj_335()
        => CompilationFailureTest("dstr/let-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-obj-id.js")]
    public Task dstr_let_ary_ptrn_rest_obj_id_336()
        => ExecutionTest("dstr/let-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/let-ary-ptrn-rest-obj-prop-id.js")]
    public Task dstr_let_ary_ptrn_rest_obj_prop_id_337()
        => ExecutionTest("dstr/let-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/let-obj-init-null.js")]
    public Task dstr_let_obj_init_null_338()
        => ExecutionTest("dstr/let-obj-init-null");

    [Fact(DisplayName = "dstr/let-obj-init-undefined.js")]
    public Task dstr_let_obj_init_undefined_339()
        => ExecutionTest("dstr/let-obj-init-undefined");

    [Fact(DisplayName = "dstr/let-obj-ptrn-empty.js")]
    public Task dstr_let_obj_ptrn_empty_340()
        => ExecutionTest("dstr/let-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-get-value-err.js")]
    public Task dstr_let_obj_ptrn_id_get_value_err_341()
        => ExecutionTest("dstr/let-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_id_init_fn_name_arrow_342()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_id_init_fn_name_class_343()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_id_init_fn_name_cover_344()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_id_init_fn_name_fn_345()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_id_init_fn_name_gen_346()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-skipped.js")]
    public Task dstr_let_obj_ptrn_id_init_skipped_347()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-throws.js")]
    public Task dstr_let_obj_ptrn_id_init_throws_348()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-init-unresolvable.js")]
    public Task dstr_let_obj_ptrn_id_init_unresolvable_349()
        => ExecutionTest("dstr/let-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dstr/let-obj-ptrn-id-trailing-comma.js")]
    public Task dstr_let_obj_ptrn_id_trailing_comma_350()
        => ExecutionTest("dstr/let-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/let-obj-ptrn-init-err.js")]
    public Task dstr_let_obj_ptrn_init_err_351()
        => CompilationFailureTest("dstr/let-obj-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/let-obj-ptrn-list-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_list_err_352()
        => ExecutionTest("dstr/let-obj-ptrn-list-err");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-ary-init.js")]
    public Task dstr_let_obj_ptrn_prop_ary_init_353()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task dstr_let_obj_ptrn_prop_ary_trailing_comma_354()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-ary-value-null.js")]
    public Task dstr_let_obj_ptrn_prop_ary_value_null_355()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-ary.js")]
    public Task dstr_let_obj_ptrn_prop_ary_356()
        => ExecutionTest("dstr/let-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-eval-err.js")]
    public Task dstr_let_obj_ptrn_prop_eval_err_357()
        => ExecutionTest("dstr/let-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-get-value-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_let_obj_ptrn_prop_id_get_value_err_358()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-init-skipped.js")]
    public Task dstr_let_obj_ptrn_prop_id_init_skipped_359()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-init-throws.js")]
    public Task dstr_let_obj_ptrn_prop_id_init_throws_360()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task dstr_let_obj_ptrn_prop_id_init_unresolvable_361()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-init.js")]
    public Task dstr_let_obj_ptrn_prop_id_init_362()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id-trailing-comma.js")]
    public Task dstr_let_obj_ptrn_prop_id_trailing_comma_363()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-id.js")]
    public Task dstr_let_obj_ptrn_prop_id_364()
        => ExecutionTest("dstr/let-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-obj-init.js")]
    public Task dstr_let_obj_ptrn_prop_obj_init_365()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-obj-value-null.js")]
    public Task dstr_let_obj_ptrn_prop_obj_value_null_366()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-obj-value-undef.js")]
    public Task dstr_let_obj_ptrn_prop_obj_value_undef_367()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "dstr/let-obj-ptrn-prop-obj.js")]
    public Task dstr_let_obj_ptrn_prop_obj_368()
        => ExecutionTest("dstr/let-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dstr/let-obj-ptrn-rest-getter.js", Skip = "Pending async object-accessor analysis.")]
    public Task dstr_let_obj_ptrn_rest_getter_369()
        => ExecutionTest("dstr/let-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/let-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task dstr_let_obj_ptrn_rest_skip_non_enumerable_370()
        => ExecutionTest("dstr/let-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/let-obj-ptrn-rest-val-obj.js")]
    public Task dstr_let_obj_ptrn_rest_val_obj_371()
        => ExecutionTest("dstr/let-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "dstr/obj-empty-bool.js")]
    public Task dstr_obj_empty_bool_372()
        => ExecutionTest("dstr/obj-empty-bool");

    [Fact(DisplayName = "dstr/obj-empty-null.js")]
    public Task dstr_obj_empty_null_373()
        => ExecutionTest("dstr/obj-empty-null");

    [Fact(DisplayName = "dstr/obj-empty-num.js")]
    public Task dstr_obj_empty_num_374()
        => ExecutionTest("dstr/obj-empty-num");

    [Fact(DisplayName = "dstr/obj-empty-obj.js")]
    public Task dstr_obj_empty_obj_375()
        => ExecutionTest("dstr/obj-empty-obj");

    [Fact(DisplayName = "dstr/obj-empty-string.js")]
    public Task dstr_obj_empty_string_376()
        => ExecutionTest("dstr/obj-empty-string");

    [Fact(DisplayName = "dstr/obj-empty-symbol.js")]
    public Task dstr_obj_empty_symbol_377()
        => ExecutionTest("dstr/obj-empty-symbol");

    [Fact(DisplayName = "dstr/obj-empty-undef.js")]
    public Task dstr_obj_empty_undef_378()
        => ExecutionTest("dstr/obj-empty-undef");

    [Fact(DisplayName = "dstr/obj-id-identifier-resolution-first.js")]
    public Task dstr_obj_id_identifier_resolution_first_379()
        => ExecutionTest("dstr/obj-id-identifier-resolution-first");

    [Fact(DisplayName = "dstr/obj-id-identifier-resolution-last.js")]
    public Task dstr_obj_id_identifier_resolution_last_380()
        => ExecutionTest("dstr/obj-id-identifier-resolution-last");

    [Fact(DisplayName = "dstr/obj-id-identifier-resolution-lone.js")]
    public Task dstr_obj_id_identifier_resolution_lone_381()
        => ExecutionTest("dstr/obj-id-identifier-resolution-lone");

    [Fact(DisplayName = "dstr/obj-id-identifier-resolution-middle.js")]
    public Task dstr_obj_id_identifier_resolution_middle_382()
        => ExecutionTest("dstr/obj-id-identifier-resolution-middle");

    [Fact(DisplayName = "dstr/obj-id-identifier-resolution-trlng.js")]
    public Task dstr_obj_id_identifier_resolution_trlng_383()
        => ExecutionTest("dstr/obj-id-identifier-resolution-trlng");

    [Fact(DisplayName = "dstr/obj-id-identifier-yield-expr.js")]
    public Task dstr_obj_id_identifier_yield_expr_384()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-expr", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-identifier-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_identifier_yield_ident_invalid_385()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-identifier-yield-ident-valid.js")]
    public Task dstr_obj_id_identifier_yield_ident_valid_386()
        => ExecutionTest("dstr/obj-id-identifier-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-id-init-assignment-missing.js")]
    public Task dstr_obj_id_init_assignment_missing_387()
        => ExecutionTest("dstr/obj-id-init-assignment-missing");

    [Fact(DisplayName = "dstr/obj-id-init-assignment-null.js")]
    public Task dstr_obj_id_init_assignment_null_388()
        => ExecutionTest("dstr/obj-id-init-assignment-null");

    [Fact(DisplayName = "dstr/obj-id-init-assignment-truthy.js")]
    public Task dstr_obj_id_init_assignment_truthy_389()
        => ExecutionTest("dstr/obj-id-init-assignment-truthy");

    [Fact(DisplayName = "dstr/obj-id-init-assignment-undef.js")]
    public Task dstr_obj_id_init_assignment_undef_390()
        => ExecutionTest("dstr/obj-id-init-assignment-undef");

    [Fact(DisplayName = "dstr/obj-id-init-evaluation.js")]
    public Task dstr_obj_id_init_evaluation_391()
        => ExecutionTest("dstr/obj-id-init-evaluation");

    [Fact(DisplayName = "dstr/obj-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_id_init_fn_name_arrow_392()
        => ExecutionTest("dstr/obj-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/obj-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_id_init_fn_name_class_393()
        => ExecutionTest("dstr/obj-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/obj-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_id_init_fn_name_cover_394()
        => ExecutionTest("dstr/obj-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/obj-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_id_init_fn_name_fn_395()
        => ExecutionTest("dstr/obj-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/obj-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_id_init_fn_name_gen_396()
        => ExecutionTest("dstr/obj-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/obj-id-init-in.js")]
    public Task dstr_obj_id_init_in_397()
        => ExecutionTest("dstr/obj-id-init-in");

    [Fact(DisplayName = "dstr/obj-id-init-let.js")]
    public Task dstr_obj_id_init_let_398()
        => ExecutionTest("dstr/obj-id-init-let");

    [Fact(DisplayName = "dstr/obj-id-init-order.js")]
    public Task dstr_obj_id_init_order_399()
        => ExecutionTest("dstr/obj-id-init-order");

    [Fact(DisplayName = "dstr/obj-id-init-simple-no-strict.js")]
    public Task dstr_obj_id_init_simple_no_strict_400()
        => ExecutionTest("dstr/obj-id-init-simple-no-strict");

    [Fact(DisplayName = "dstr/obj-id-init-simple-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_init_simple_strict_401()
        => CompilationFailureTest("dstr/obj-id-init-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-init-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_id_init_yield_expr_402()
        => ExecutionTest("dstr/obj-id-init-yield-expr");

    [Fact(DisplayName = "dstr/obj-id-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_init_yield_ident_invalid_403()
        => CompilationFailureTest("dstr/obj-id-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-id-init-yield-ident-valid.js")]
    public Task dstr_obj_id_init_yield_ident_valid_404()
        => ExecutionTest("dstr/obj-id-init-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-id-put-const.js")]
    public Task dstr_obj_id_put_const_405()
        => ExecutionTest("dstr/obj-id-put-const");

    [Fact(DisplayName = "dstr/obj-id-put-let.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_id_put_let_406()
        => ExecutionTest("dstr/obj-id-put-let");

    [Fact(DisplayName = "dstr/obj-id-put-unresolvable-no-strict.js")]
    public Task dstr_obj_id_put_unresolvable_no_strict_407()
        => ExecutionTest("dstr/obj-id-put-unresolvable-no-strict");

    [Fact(DisplayName = "dstr/obj-id-put-unresolvable-strict.js")]
    public Task dstr_obj_id_put_unresolvable_strict_408()
        => ExecutionTest("dstr/obj-id-put-unresolvable-strict");

    [Fact(DisplayName = "dstr/obj-id-simple-no-strict.js")]
    public Task dstr_obj_id_simple_no_strict_409()
        => ExecutionTest("dstr/obj-id-simple-no-strict");

    [Fact(DisplayName = "dstr/obj-id-simple-strict.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_id_simple_strict_410()
        => CompilationFailureTest("dstr/obj-id-simple-strict", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-init-assignment-missing.js")]
    public Task dstr_obj_prop_elem_init_assignment_missing_411()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-missing");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-assignment-null.js")]
    public Task dstr_obj_prop_elem_init_assignment_null_412()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-null");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-assignment-truthy.js")]
    public Task dstr_obj_prop_elem_init_assignment_truthy_413()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-truthy");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-assignment-undef.js")]
    public Task dstr_obj_prop_elem_init_assignment_undef_414()
        => ExecutionTest("dstr/obj-prop-elem-init-assignment-undef");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-evaluation.js")]
    public Task dstr_obj_prop_elem_init_evaluation_415()
        => ExecutionTest("dstr/obj-prop-elem-init-evaluation");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_init_fn_name_arrow_416()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_init_fn_name_class_417()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-class");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_init_fn_name_cover_418()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-cover");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_init_fn_name_fn_419()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-fn");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_init_fn_name_gen_420()
        => ExecutionTest("dstr/obj-prop-elem-init-fn-name-gen");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-in.js")]
    public Task dstr_obj_prop_elem_init_in_421()
        => ExecutionTest("dstr/obj-prop-elem-init-in");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-let.js")]
    public Task dstr_obj_prop_elem_init_let_422()
        => ExecutionTest("dstr/obj-prop-elem-init-let");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_prop_elem_init_yield_expr_423()
        => ExecutionTest("dstr/obj-prop-elem-init-yield-expr");

    [Fact(DisplayName = "dstr/obj-prop-elem-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_elem_init_yield_ident_invalid_424()
        => CompilationFailureTest("dstr/obj-prop-elem-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-init-yield-ident-valid.js")]
    public Task dstr_obj_prop_elem_init_yield_ident_valid_425()
        => ExecutionTest("dstr/obj-prop-elem-init-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-prop-elem-target-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_memberexpr_optchain_prop_ref_init_426()
        => CompilationFailureTest("dstr/obj-prop-elem-target-memberexpr-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_optchain_prop_ref_init_427()
        => CompilationFailureTest("dstr/obj-prop-elem-target-obj-literal-optchain-prop-ref-init", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-obj-literal-prop-ref-init-active.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref_init_active_428()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref-init-active");

    [Fact(DisplayName = "dstr/obj-prop-elem-target-obj-literal-prop-ref-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref_init_429()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref-init");

    [Fact(DisplayName = "dstr/obj-prop-elem-target-obj-literal-prop-ref.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task dstr_obj_prop_elem_target_obj_literal_prop_ref_430()
        => ExecutionTest("dstr/obj-prop-elem-target-obj-literal-prop-ref");

    [Fact(DisplayName = "dstr/obj-prop-elem-target-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_prop_elem_target_yield_expr_431()
        => ExecutionTest("dstr/obj-prop-elem-target-yield-expr");

    [Fact(DisplayName = "dstr/obj-prop-elem-target-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_elem_target_yield_ident_invalid_432()
        => CompilationFailureTest("dstr/obj-prop-elem-target-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-elem-target-yield-ident-valid.js")]
    public Task dstr_obj_prop_elem_target_yield_ident_valid_433()
        => ExecutionTest("dstr/obj-prop-elem-target-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-prop-identifier-resolution-first.js")]
    public Task dstr_obj_prop_identifier_resolution_first_434()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-first");

    [Fact(DisplayName = "dstr/obj-prop-identifier-resolution-last.js")]
    public Task dstr_obj_prop_identifier_resolution_last_435()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-last");

    [Fact(DisplayName = "dstr/obj-prop-identifier-resolution-lone.js")]
    public Task dstr_obj_prop_identifier_resolution_lone_436()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-lone");

    [Fact(DisplayName = "dstr/obj-prop-identifier-resolution-middle.js")]
    public Task dstr_obj_prop_identifier_resolution_middle_437()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-middle");

    [Fact(DisplayName = "dstr/obj-prop-identifier-resolution-trlng.js")]
    public Task dstr_obj_prop_identifier_resolution_trlng_438()
        => ExecutionTest("dstr/obj-prop-identifier-resolution-trlng");

    [Fact(DisplayName = "dstr/obj-prop-name-evaluation-error.js")]
    public Task dstr_obj_prop_name_evaluation_error_439()
        => ExecutionTest("dstr/obj-prop-name-evaluation-error");

    [Fact(DisplayName = "dstr/obj-prop-name-evaluation.js")]
    public Task dstr_obj_prop_name_evaluation_440()
        => ExecutionTest("dstr/obj-prop-name-evaluation");

    [Fact(DisplayName = "dstr/obj-prop-nested-array-invalid.js")]
    public Task dstr_obj_prop_nested_array_invalid_441()
        => CompilationFailureTest("dstr/obj-prop-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-array-null.js")]
    public Task dstr_obj_prop_nested_array_null_442()
        => ExecutionTest("dstr/obj-prop-nested-array-null");

    [Fact(DisplayName = "dstr/obj-prop-nested-array-undefined-own.js")]
    public Task dstr_obj_prop_nested_array_undefined_own_443()
        => ExecutionTest("dstr/obj-prop-nested-array-undefined-own");

    [Fact(DisplayName = "dstr/obj-prop-nested-array-undefined.js")]
    public Task dstr_obj_prop_nested_array_undefined_444()
        => ExecutionTest("dstr/obj-prop-nested-array-undefined");

    [Fact(DisplayName = "dstr/obj-prop-nested-array-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_prop_nested_array_yield_expr_445()
        => ExecutionTest("dstr/obj-prop-nested-array-yield-expr");

    [Fact(DisplayName = "dstr/obj-prop-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_nested_array_yield_ident_invalid_446()
        => CompilationFailureTest("dstr/obj-prop-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-array-yield-ident-valid.js")]
    public Task dstr_obj_prop_nested_array_yield_ident_valid_447()
        => ExecutionTest("dstr/obj-prop-nested-array-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-prop-nested-array.js")]
    public Task dstr_obj_prop_nested_array_448()
        => ExecutionTest("dstr/obj-prop-nested-array");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-invalid.js")]
    public Task dstr_obj_prop_nested_obj_invalid_449()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-null.js")]
    public Task dstr_obj_prop_nested_obj_null_450()
        => ExecutionTest("dstr/obj-prop-nested-obj-null");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-undefined-own.js")]
    public Task dstr_obj_prop_nested_obj_undefined_own_451()
        => ExecutionTest("dstr/obj-prop-nested-obj-undefined-own");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-undefined.js")]
    public Task dstr_obj_prop_nested_obj_undefined_452()
        => ExecutionTest("dstr/obj-prop-nested-obj-undefined");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-yield-expr.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_prop_nested_obj_yield_expr_453()
        => ExecutionTest("dstr/obj-prop-nested-obj-yield-expr");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task dstr_obj_prop_nested_obj_yield_ident_invalid_454()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-prop-nested-obj-yield-ident-valid.js")]
    public Task dstr_obj_prop_nested_obj_yield_ident_valid_455()
        => ExecutionTest("dstr/obj-prop-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "dstr/obj-prop-nested-obj.js")]
    public Task dstr_obj_prop_nested_obj_456()
        => ExecutionTest("dstr/obj-prop-nested-obj");

    [Fact(DisplayName = "dstr/obj-prop-put-const.js")]
    public Task dstr_obj_prop_put_const_457()
        => ExecutionTest("dstr/obj-prop-put-const");

    [Fact(DisplayName = "dstr/obj-prop-put-let.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_prop_put_let_458()
        => ExecutionTest("dstr/obj-prop-put-let");

    [Fact(DisplayName = "dstr/obj-prop-put-order.js")]
    public Task dstr_obj_prop_put_order_459()
        => ExecutionTest("dstr/obj-prop-put-order");

    [Fact(DisplayName = "dstr/obj-prop-put-prop-ref-no-get.js")]
    public Task dstr_obj_prop_put_prop_ref_no_get_460()
        => ExecutionTest("dstr/obj-prop-put-prop-ref-no-get");

    [Fact(DisplayName = "dstr/obj-prop-put-prop-ref-user-err.js")]
    public Task dstr_obj_prop_put_prop_ref_user_err_461()
        => ExecutionTest("dstr/obj-prop-put-prop-ref-user-err");

    [Fact(DisplayName = "dstr/obj-prop-put-prop-ref.js")]
    public Task dstr_obj_prop_put_prop_ref_462()
        => ExecutionTest("dstr/obj-prop-put-prop-ref");

    [Fact(DisplayName = "dstr/obj-prop-put-unresolvable-no-strict.js")]
    public Task dstr_obj_prop_put_unresolvable_no_strict_463()
        => ExecutionTest("dstr/obj-prop-put-unresolvable-no-strict");

    [Fact(DisplayName = "dstr/obj-prop-put-unresolvable-strict.js")]
    public Task dstr_obj_prop_put_unresolvable_strict_464()
        => ExecutionTest("dstr/obj-prop-put-unresolvable-strict");

    [Fact(DisplayName = "dstr/obj-rest-computed-property-no-strict.js")]
    public Task dstr_obj_rest_computed_property_no_strict_465()
        => ExecutionTest("dstr/obj-rest-computed-property-no-strict");

    [Fact(DisplayName = "dstr/obj-rest-computed-property.js")]
    public Task dstr_obj_rest_computed_property_466()
        => ExecutionTest("dstr/obj-rest-computed-property");

    [Fact(DisplayName = "dstr/obj-rest-descriptors.js")]
    public Task dstr_obj_rest_descriptors_467()
        => ExecutionTest("dstr/obj-rest-descriptors");

    [Fact(DisplayName = "dstr/obj-rest-empty-obj.js")]
    public Task dstr_obj_rest_empty_obj_468()
        => ExecutionTest("dstr/obj-rest-empty-obj");

    [Fact(DisplayName = "dstr/obj-rest-getter-abrupt-get-error.js")]
    public Task dstr_obj_rest_getter_abrupt_get_error_469()
        => ExecutionTest("dstr/obj-rest-getter-abrupt-get-error");

    [Fact(DisplayName = "dstr/obj-rest-getter.js")]
    public Task dstr_obj_rest_getter_470()
        => ExecutionTest("dstr/obj-rest-getter");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_1_471()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-1dot.js")]
    public Task dstr_obj_rest_non_string_computed_property_1dot_472()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1dot");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-1dot0.js")]
    public Task dstr_obj_rest_non_string_computed_property_1dot0_473()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1dot0");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-1e0.js")]
    public Task dstr_obj_rest_non_string_computed_property_1e0_474()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-1e0");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-array-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_array_1_475()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-array-1");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-array-1e0.js")]
    public Task dstr_obj_rest_non_string_computed_property_array_1e0_476()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-array-1e0");

    [Fact(DisplayName = "dstr/obj-rest-non-string-computed-property-string-1.js")]
    public Task dstr_obj_rest_non_string_computed_property_string_1_477()
        => ExecutionTest("dstr/obj-rest-non-string-computed-property-string-1");

    [Fact(DisplayName = "dstr/obj-rest-not-last-element-invalid.js")]
    public Task dstr_obj_rest_not_last_element_invalid_478()
        => CompilationFailureTest("dstr/obj-rest-not-last-element-invalid", string.Empty);

    [Fact(DisplayName = "dstr/obj-rest-number.js")]
    public Task dstr_obj_rest_number_479()
        => ExecutionTest("dstr/obj-rest-number");

    [Fact(DisplayName = "dstr/obj-rest-order.js")]
    public Task dstr_obj_rest_order_480()
        => ExecutionTest("dstr/obj-rest-order");

    [Fact(DisplayName = "dstr/obj-rest-put-const.js")]
    public Task dstr_obj_rest_put_const_481()
        => ExecutionTest("dstr/obj-rest-put-const");

    [Fact(DisplayName = "dstr/obj-rest-same-name.js")]
    public Task dstr_obj_rest_same_name_482()
        => ExecutionTest("dstr/obj-rest-same-name");

    [Fact(DisplayName = "dstr/obj-rest-skip-non-enumerable.js")]
    public Task dstr_obj_rest_skip_non_enumerable_483()
        => ExecutionTest("dstr/obj-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/obj-rest-str-val.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task dstr_obj_rest_str_val_484()
        => ExecutionTest("dstr/obj-rest-str-val");

    [Fact(DisplayName = "dstr/obj-rest-symbol-val.js")]
    public Task dstr_obj_rest_symbol_val_485()
        => ExecutionTest("dstr/obj-rest-symbol-val");

    [Fact(DisplayName = "dstr/obj-rest-to-property-with-setter.js")]
    public Task dstr_obj_rest_to_property_with_setter_486()
        => ExecutionTest("dstr/obj-rest-to-property-with-setter");

    [Fact(DisplayName = "dstr/obj-rest-to-property.js")]
    public Task dstr_obj_rest_to_property_487()
        => ExecutionTest("dstr/obj-rest-to-property");

    [Fact(DisplayName = "dstr/obj-rest-val-null.js")]
    public Task dstr_obj_rest_val_null_488()
        => ExecutionTest("dstr/obj-rest-val-null");

    [Fact(DisplayName = "dstr/obj-rest-val-undefined.js")]
    public Task dstr_obj_rest_val_undefined_489()
        => ExecutionTest("dstr/obj-rest-val-undefined");

    [Fact(DisplayName = "dstr/obj-rest-valid-object.js")]
    public Task dstr_obj_rest_valid_object_490()
        => ExecutionTest("dstr/obj-rest-valid-object");

    [Fact(DisplayName = "dstr/var-ary-init-iter-close.js")]
    public Task dstr_var_ary_init_iter_close_491()
        => ExecutionTest("dstr/var-ary-init-iter-close");

    [Fact(DisplayName = "dstr/var-ary-init-iter-get-err-array-prototype.js")]
    public Task dstr_var_ary_init_iter_get_err_array_prototype_492()
        => ExecutionTest("dstr/var-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "dstr/var-ary-init-iter-get-err.js")]
    public Task dstr_var_ary_init_iter_get_err_493()
        => ExecutionTest("dstr/var-ary-init-iter-get-err");

    [Fact(DisplayName = "dstr/var-ary-init-iter-no-close.js")]
    public Task dstr_var_ary_init_iter_no_close_494()
        => ExecutionTest("dstr/var-ary-init-iter-no-close");

    [Fact(DisplayName = "dstr/var-ary-name-iter-val.js")]
    public Task dstr_var_ary_name_iter_val_495()
        => ExecutionTest("dstr/var-ary-name-iter-val");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-elem-init.js")]
    public Task dstr_var_ary_ptrn_elem_ary_elem_init_496()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-elem-iter.js")]
    public Task dstr_var_ary_ptrn_elem_ary_elem_iter_497()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-elision-init.js")]
    public Task dstr_var_ary_ptrn_elem_ary_elision_init_498()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-elision-iter.js")]
    public Task dstr_var_ary_ptrn_elem_ary_elision_iter_499()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_ary_empty_init_500()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-empty-init");
}
