namespace Jroc.Test262.Tests.language.statements.for_of;

public class Section14_7_5ExecutionTests03 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests03() : base("language/statements/for-of", "language.statements.for_of") { }

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_ary_empty_iter_501()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-rest-init.js")]
    public Task dstr_var_ary_ptrn_elem_ary_rest_init_502()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_ary_rest_iter_503()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-ary-val-null.js")]
    public Task dstr_var_ary_ptrn_elem_ary_val_null_504()
        => ExecutionTest("dstr/var-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-exhausted.js")]
    public Task dstr_var_ary_ptrn_elem_id_init_exhausted_505()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_fn_name_arrow_506()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_fn_name_class_507()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_fn_name_cover_508()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_fn_name_fn_509()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_fn_name_gen_510()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-hole.js")]
    public Task dstr_var_ary_ptrn_elem_id_init_hole_511()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-skipped.js")]
    public Task dstr_var_ary_ptrn_elem_id_init_skipped_512()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_ary_ptrn_elem_id_init_throws_513()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-undef.js")]
    public Task dstr_var_ary_ptrn_elem_id_init_undef_514()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task dstr_var_ary_ptrn_elem_id_init_unresolvable_515()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-complete.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_complete_516()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-done.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_done_517()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-step-err.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_step_err_518()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-val-array-prototype.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_val_array_prototype_519()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val-array-prototype");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-val-err.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_val_err_520()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-id-iter-val.js")]
    public Task dstr_var_ary_ptrn_elem_id_iter_val_521()
        => ExecutionTest("dstr/var-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-id-init.js")]
    public Task dstr_var_ary_ptrn_elem_obj_id_init_522()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-id.js")]
    public Task dstr_var_ary_ptrn_elem_obj_id_523()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task dstr_var_ary_ptrn_elem_obj_prop_id_init_524()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-prop-id.js")]
    public Task dstr_var_ary_ptrn_elem_obj_prop_id_525()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-val-null.js")]
    public Task dstr_var_ary_ptrn_elem_obj_val_null_526()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elem-obj-val-undef.js")]
    public Task dstr_var_ary_ptrn_elem_obj_val_undef_527()
        => ExecutionTest("dstr/var-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elision-exhausted.js")]
    public Task dstr_var_ary_ptrn_elision_exhausted_528()
        => ExecutionTest("dstr/var-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elision-iter-close.js")]
    public Task dstr_var_ary_ptrn_elision_iter_close_529()
        => ExecutionTest("dstr/var-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elision-step-err.js")]
    public Task dstr_var_ary_ptrn_elision_step_err_530()
        => ExecutionTest("dstr/var-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-elision.js")]
    public Task dstr_var_ary_ptrn_elision_531()
        => ExecutionTest("dstr/var-ary-ptrn-elision");

    [Fact(DisplayName = "dstr/var-ary-ptrn-empty.js")]
    public Task dstr_var_ary_ptrn_empty_532()
        => ExecutionTest("dstr/var-ary-ptrn-empty");

    [Fact(DisplayName = "dstr/var-ary-ptrn-init-err.js")]
    public Task dstr_var_ary_ptrn_init_err_533()
        => CompilationFailureTest("dstr/var-ary-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-ary-elem.js")]
    public Task dstr_var_ary_ptrn_rest_ary_elem_534()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-ary-elision.js")]
    public Task dstr_var_ary_ptrn_rest_ary_elision_535()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-ary-empty.js")]
    public Task dstr_var_ary_ptrn_rest_ary_empty_536()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-ary-rest.js")]
    public Task dstr_var_ary_ptrn_rest_ary_rest_537()
        => ExecutionTest("dstr/var-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-direct.js")]
    public Task dstr_var_ary_ptrn_rest_id_direct_538()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-direct");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-elision-next-err.js")]
    public Task dstr_var_ary_ptrn_rest_id_elision_next_err_539()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-elision.js")]
    public Task dstr_var_ary_ptrn_rest_id_elision_540()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-exhausted.js")]
    public Task dstr_var_ary_ptrn_rest_id_exhausted_541()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-iter-close.js")]
    public Task dstr_var_ary_ptrn_rest_id_iter_close_542()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-iter-step-err.js")]
    public Task dstr_var_ary_ptrn_rest_id_iter_step_err_543()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id-iter-val-err.js")]
    public Task dstr_var_ary_ptrn_rest_id_iter_val_err_544()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-id.js")]
    public Task dstr_var_ary_ptrn_rest_id_545()
        => ExecutionTest("dstr/var-ary-ptrn-rest-id");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-init-ary.js")]
    public Task dstr_var_ary_ptrn_rest_init_ary_546()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-init-id.js")]
    public Task dstr_var_ary_ptrn_rest_init_id_547()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-init-obj.js")]
    public Task dstr_var_ary_ptrn_rest_init_obj_548()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-not-final-ary.js")]
    public Task dstr_var_ary_ptrn_rest_not_final_ary_549()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-not-final-id.js")]
    public Task dstr_var_ary_ptrn_rest_not_final_id_550()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-not-final-obj.js")]
    public Task dstr_var_ary_ptrn_rest_not_final_obj_551()
        => CompilationFailureTest("dstr/var-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-obj-id.js")]
    public Task dstr_var_ary_ptrn_rest_obj_id_552()
        => ExecutionTest("dstr/var-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "dstr/var-ary-ptrn-rest-obj-prop-id.js")]
    public Task dstr_var_ary_ptrn_rest_obj_prop_id_553()
        => ExecutionTest("dstr/var-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "dstr/var-obj-init-null.js")]
    public Task dstr_var_obj_init_null_554()
        => ExecutionTest("dstr/var-obj-init-null");

    [Fact(DisplayName = "dstr/var-obj-init-undefined.js")]
    public Task dstr_var_obj_init_undefined_555()
        => ExecutionTest("dstr/var-obj-init-undefined");

    [Fact(DisplayName = "dstr/var-obj-ptrn-empty.js")]
    public Task dstr_var_obj_ptrn_empty_556()
        => ExecutionTest("dstr/var-obj-ptrn-empty");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-get-value-err.js")]
    public Task dstr_var_obj_ptrn_id_get_value_err_557()
        => ExecutionTest("dstr/var-obj-ptrn-id-get-value-err");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_id_init_fn_name_arrow_558()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_id_init_fn_name_class_559()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_id_init_fn_name_cover_560()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_id_init_fn_name_fn_561()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_id_init_fn_name_gen_562()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-skipped.js")]
    public Task dstr_var_obj_ptrn_id_init_skipped_563()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-throws.js")]
    public Task dstr_var_obj_ptrn_id_init_throws_564()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-throws");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-init-unresolvable.js")]
    public Task dstr_var_obj_ptrn_id_init_unresolvable_565()
        => ExecutionTest("dstr/var-obj-ptrn-id-init-unresolvable");

    [Fact(DisplayName = "dstr/var-obj-ptrn-id-trailing-comma.js")]
    public Task dstr_var_obj_ptrn_id_trailing_comma_566()
        => ExecutionTest("dstr/var-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "dstr/var-obj-ptrn-init-err.js")]
    public Task dstr_var_obj_ptrn_init_err_567()
        => CompilationFailureTest("dstr/var-obj-ptrn-init-err", string.Empty);

    [Fact(DisplayName = "dstr/var-obj-ptrn-list-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_list_err_568()
        => ExecutionTest("dstr/var-obj-ptrn-list-err");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-ary-init.js")]
    public Task dstr_var_obj_ptrn_prop_ary_init_569()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task dstr_var_obj_ptrn_prop_ary_trailing_comma_570()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-ary-value-null.js")]
    public Task dstr_var_obj_ptrn_prop_ary_value_null_571()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary-value-null");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-ary.js")]
    public Task dstr_var_obj_ptrn_prop_ary_572()
        => ExecutionTest("dstr/var-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-eval-err.js")]
    public Task dstr_var_obj_ptrn_prop_eval_err_573()
        => ExecutionTest("dstr/var-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-get-value-err.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task dstr_var_obj_ptrn_prop_id_get_value_err_574()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-get-value-err");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-init-skipped.js")]
    public Task dstr_var_obj_ptrn_prop_id_init_skipped_575()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-init-throws.js")]
    public Task dstr_var_obj_ptrn_prop_id_init_throws_576()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-throws");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-init-unresolvable.js")]
    public Task dstr_var_obj_ptrn_prop_id_init_unresolvable_577()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init-unresolvable");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-init.js")]
    public Task dstr_var_obj_ptrn_prop_id_init_578()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id-trailing-comma.js")]
    public Task dstr_var_obj_ptrn_prop_id_trailing_comma_579()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-id.js")]
    public Task dstr_var_obj_ptrn_prop_id_580()
        => ExecutionTest("dstr/var-obj-ptrn-prop-id");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-obj-init.js")]
    public Task dstr_var_obj_ptrn_prop_obj_init_581()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-obj-value-null.js")]
    public Task dstr_var_obj_ptrn_prop_obj_value_null_582()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-value-null");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-obj-value-undef.js")]
    public Task dstr_var_obj_ptrn_prop_obj_value_undef_583()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj-value-undef");

    [Fact(DisplayName = "dstr/var-obj-ptrn-prop-obj.js")]
    public Task dstr_var_obj_ptrn_prop_obj_584()
        => ExecutionTest("dstr/var-obj-ptrn-prop-obj");

    [Fact(DisplayName = "dstr/var-obj-ptrn-rest-getter.js")]
    public Task dstr_var_obj_ptrn_rest_getter_585()
        => ExecutionTest("dstr/var-obj-ptrn-rest-getter");

    [Fact(DisplayName = "dstr/var-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task dstr_var_obj_ptrn_rest_skip_non_enumerable_586()
        => ExecutionTest("dstr/var-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "dstr/var-obj-ptrn-rest-val-obj.js")]
    public Task dstr_var_obj_ptrn_rest_val_obj_587()
        => ExecutionTest("dstr/var-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "escaped-of.js")]
    public Task escaped_of_588()
        => CompilationFailureTest("escaped-of", string.Empty);

    [Fact(DisplayName = "generator-next-error.js")]
    public Task generator_next_error_589()
        => ExecutionTest("generator-next-error");

    [Fact(DisplayName = "head-await-using-bound-names-fordecl-tdz.js")]
    public Task head_await_using_bound_names_fordecl_tdz_590()
        => ExecutionTest("head-await-using-bound-names-fordecl-tdz");

    [Fact(DisplayName = "head-await-using-bound-names-let.js")]
    public Task head_await_using_bound_names_let_591()
        => CompilationFailureTest("head-await-using-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-await-using-init.js")]
    public Task head_await_using_init_592()
        => CompilationFailureTest("head-await-using-init", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-dup.js")]
    public Task head_const_bound_names_dup_593()
        => CompilationFailureTest("head-const-bound-names-dup", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-in-stmt.js")]
    public Task head_const_bound_names_in_stmt_594()
        => CompilationFailureTest("head-const-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-const-bound-names-let.js")]
    public Task head_const_bound_names_let_595()
        => CompilationFailureTest("head-const-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-const-init.js")]
    public Task head_const_init_596()
        => CompilationFailureTest("head-const-init", string.Empty);

    [Fact(DisplayName = "head-decl-no-expr.js")]
    public Task head_decl_no_expr_597()
        => CompilationFailureTest("head-decl-no-expr", string.Empty);

    [Fact(DisplayName = "head-expr-no-expr.js")]
    public Task head_expr_no_expr_598()
        => CompilationFailureTest("head-expr-no-expr", string.Empty);

    [Fact(DisplayName = "head-expr-obj-iterator-method.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task head_expr_obj_iterator_method_599()
        => ExecutionTest("head-expr-obj-iterator-method");

    [Fact(DisplayName = "head-expr-primitive-iterator-method.js")]
    public Task head_expr_primitive_iterator_method_600()
        => ExecutionTest("head-expr-primitive-iterator-method");

    [Fact(DisplayName = "head-expr-to-obj.js")]
    public Task head_expr_to_obj_601()
        => ExecutionTest("head-expr-to-obj");

    [Fact(DisplayName = "head-let-bound-names-dup.js")]
    public Task head_let_bound_names_dup_602()
        => CompilationFailureTest("head-let-bound-names-dup", string.Empty);

    [Fact(DisplayName = "head-let-bound-names-in-stmt.js")]
    public Task head_let_bound_names_in_stmt_603()
        => CompilationFailureTest("head-let-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-let-bound-names-let.js")]
    public Task head_let_bound_names_let_604()
        => CompilationFailureTest("head-let-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-let-init.js")]
    public Task head_let_init_605()
        => CompilationFailureTest("head-let-init", string.Empty);

    [Fact(DisplayName = "head-lhs-async-dot.js")]
    public Task head_lhs_async_dot_606()
        => ExecutionTest("head-lhs-async-dot");

    [Fact(DisplayName = "head-lhs-async-escaped.js")]
    public Task head_lhs_async_escaped_607()
        => ExecutionTest("head-lhs-async-escaped");

    [Fact(DisplayName = "head-lhs-async-invalid.js")]
    public Task head_lhs_async_invalid_608()
        => CompilationFailureTest("head-lhs-async-invalid", string.Empty);

    [Fact(DisplayName = "head-lhs-async-parens.js")]
    public Task head_lhs_async_parens_609()
        => ExecutionTest("head-lhs-async-parens");

    [Fact(DisplayName = "head-lhs-cover-non-asnmt-trgt.js")]
    public Task head_lhs_cover_non_asnmt_trgt_610()
        => CompilationFailureTest("head-lhs-cover-non-asnmt-trgt", string.Empty);

    [Fact(DisplayName = "head-lhs-invalid-asnmt-ptrn-ary.js")]
    public Task head_lhs_invalid_asnmt_ptrn_ary_611()
        => CompilationFailureTest("head-lhs-invalid-asnmt-ptrn-ary", string.Empty);

    [Fact(DisplayName = "head-lhs-invalid-asnmt-ptrn-obj.js")]
    public Task head_lhs_invalid_asnmt_ptrn_obj_612()
        => CompilationFailureTest("head-lhs-invalid-asnmt-ptrn-obj", string.Empty);

    [Fact(DisplayName = "head-lhs-let.js")]
    public Task head_lhs_let_613()
        => CompilationFailureTest("head-lhs-let", string.Empty);

    [Fact(DisplayName = "head-lhs-non-asnmt-trgt.js")]
    public Task head_lhs_non_asnmt_trgt_614()
        => CompilationFailureTest("head-lhs-non-asnmt-trgt", string.Empty);

    [Fact(DisplayName = "head-using-bound-names-fordecl-tdz.js")]
    public Task head_using_bound_names_fordecl_tdz_615()
        => ExecutionTest("head-using-bound-names-fordecl-tdz");

    [Fact(DisplayName = "head-using-bound-names-in-stmt.js")]
    public Task head_using_bound_names_in_stmt_616()
        => CompilationFailureTest("head-using-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-using-bound-names-let.js")]
    public Task head_using_bound_names_let_617()
        => CompilationFailureTest("head-using-bound-names-let", string.Empty);

    [Fact(DisplayName = "head-using-fresh-binding-per-iteration.js")]
    public Task head_using_fresh_binding_per_iteration_618()
        => ExecutionTest("head-using-fresh-binding-per-iteration");

    [Fact(DisplayName = "head-using-init.js")]
    public Task head_using_init_619()
        => CompilationFailureTest("head-using-init", string.Empty);

    [Fact(DisplayName = "head-var-init.js")]
    public Task head_var_init_620()
        => CompilationFailureTest("head-var-init", string.Empty);

    [Fact(DisplayName = "head-var-no-expr.js")]
    public Task head_var_no_expr_621()
        => CompilationFailureTest("head-var-no-expr", string.Empty);

    [Fact(DisplayName = "iterator-close-non-object.js")]
    public Task iterator_close_non_object_622()
        => ExecutionTest("iterator-close-non-object");

    [Fact(DisplayName = "iterator-close-non-throw-get-method-abrupt.js")]
    public Task iterator_close_non_throw_get_method_abrupt_623()
        => ExecutionTest("iterator-close-non-throw-get-method-abrupt");

    [Fact(DisplayName = "iterator-close-non-throw-get-method-is-null.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task iterator_close_non_throw_get_method_is_null_624()
        => ExecutionTest("iterator-close-non-throw-get-method-is-null");

    [Fact(DisplayName = "iterator-close-non-throw-get-method-non-callable.js")]
    public Task iterator_close_non_throw_get_method_non_callable_625()
        => ExecutionTest("iterator-close-non-throw-get-method-non-callable");

    [Fact(DisplayName = "iterator-close-throw-get-method-abrupt.js")]
    public Task iterator_close_throw_get_method_abrupt_626()
        => ExecutionTest("iterator-close-throw-get-method-abrupt");

    [Fact(DisplayName = "iterator-close-throw-get-method-non-callable.js")]
    public Task iterator_close_throw_get_method_non_callable_627()
        => ExecutionTest("iterator-close-throw-get-method-non-callable");

    [Fact(DisplayName = "iterator-close-via-break.js")]
    public Task iterator_close_via_break_628()
        => ExecutionTest("iterator-close-via-break");

    [Fact(DisplayName = "iterator-close-via-continue.js")]
    public Task iterator_close_via_continue_629()
        => ExecutionTest("iterator-close-via-continue");

    [Fact(DisplayName = "iterator-close-via-return.js")]
    public Task iterator_close_via_return_630()
        => ExecutionTest("iterator-close-via-return");

    [Fact(DisplayName = "iterator-close-via-throw.js")]
    public Task iterator_close_via_throw_631()
        => ExecutionTest("iterator-close-via-throw");

    [Fact(DisplayName = "iterator-next-error.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task iterator_next_error_632()
        => ExecutionTest("iterator-next-error");

    [Fact(DisplayName = "iterator-next-reference.js")]
    public Task iterator_next_reference_633()
        => ExecutionTest("iterator-next-reference");

    [Fact(DisplayName = "iterator-next-result-type.js")]
    public Task iterator_next_result_type_634()
        => ExecutionTest("iterator-next-result-type");

    [Fact(DisplayName = "iterator-next-result-value-attr-error.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task iterator_next_result_value_attr_error_635()
        => ExecutionTest("iterator-next-result-value-attr-error");

    [Fact(DisplayName = "iterator-next-result-value-attr.js")]
    public Task iterator_next_result_value_attr_636()
        => ExecutionTest("iterator-next-result-value-attr");

    [Fact(DisplayName = "labelled-fn-stmt-const.js")]
    public Task labelled_fn_stmt_const_637()
        => CompilationFailureTest("labelled-fn-stmt-const", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-let.js")]
    public Task labelled_fn_stmt_let_638()
        => CompilationFailureTest("labelled-fn-stmt-let", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-lhs.js")]
    public Task labelled_fn_stmt_lhs_639()
        => CompilationFailureTest("labelled-fn-stmt-lhs", string.Empty);

    [Fact(DisplayName = "labelled-fn-stmt-var.js")]
    public Task labelled_fn_stmt_var_640()
        => CompilationFailureTest("labelled-fn-stmt-var", string.Empty);

    [Fact(DisplayName = "let-array-with-newline.js")]
    public Task let_array_with_newline_641()
        => CompilationFailureTest("let-array-with-newline", string.Empty);

    [Fact(DisplayName = "let-block-with-newline.js")]
    public Task let_block_with_newline_642()
        => ExecutionTest("let-block-with-newline");

    [Fact(DisplayName = "let-identifier-with-newline.js")]
    public Task let_identifier_with_newline_643()
        => ExecutionTest("let-identifier-with-newline");

    [Fact(DisplayName = "map-contract-expand.js")]
    public Task map_contract_expand_644()
        => ExecutionTest("map-contract-expand");

    [Fact(DisplayName = "map-expand-contract.js")]
    public Task map_expand_contract_645()
        => ExecutionTest("map-expand-contract");

    [Fact(DisplayName = "return-from-catch.js")]
    public Task return_from_catch_646()
        => ExecutionTest("return-from-catch");

    [Fact(DisplayName = "return-from-finally.js")]
    public Task return_from_finally_647()
        => ExecutionTest("return-from-finally");

    [Fact(DisplayName = "return-from-try.js")]
    public Task return_from_try_648()
        => ExecutionTest("return-from-try");

    [Fact(DisplayName = "return.js")]
    public Task return_649()
        => ExecutionTest("return");

    [Fact(DisplayName = "scope-body-lex-close.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task scope_body_lex_close_650()
        => ExecutionTest("scope-body-lex-close");

    [Fact(DisplayName = "scope-body-lex-open.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_body_lex_open_651()
        => ExecutionTest("scope-body-lex-open");

    [Fact(DisplayName = "scope-body-var-none.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_body_var_none_652()
        => ExecutionTest("scope-body-var-none");

    [Fact(DisplayName = "scope-head-lex-close.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_head_lex_close_653()
        => ExecutionTest("scope-head-lex-close");

    [Fact(DisplayName = "scope-head-lex-open.js", Skip = "Pending complete loop lexical-environment lowering.")]
    public Task scope_head_lex_open_654()
        => ExecutionTest("scope-head-lex-open");

    [Fact(DisplayName = "scope-head-var-none.js", Skip = "eval is not supported by JROC.")]
    public Task scope_head_var_none_655()
        => ExecutionTest("scope-head-var-none");

    [Fact(DisplayName = "set-contract-expand.js")]
    public Task set_contract_expand_656()
        => ExecutionTest("set-contract-expand");

    [Fact(DisplayName = "set-expand-contract.js")]
    public Task set_expand_contract_657()
        => ExecutionTest("set-expand-contract");

    [Fact(DisplayName = "string-astral-truncated.js")]
    public Task string_astral_truncated_658()
        => ExecutionTest("string-astral-truncated");

    [Fact(DisplayName = "throw-from-catch.js")]
    public Task throw_from_catch_659()
        => ExecutionTest("throw-from-catch");

    [Fact(DisplayName = "throw-from-finally.js")]
    public Task throw_from_finally_660()
        => ExecutionTest("throw-from-finally");

    [Fact(DisplayName = "throw.js")]
    public Task throw_661()
        => ExecutionTest("throw");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer-grow-before-end.js", Skip = "Resizable ArrayBuffer is not supported by JROC.")]
    public Task typedarray_backed_by_resizable_buffer_grow_before_end_662()
        => ExecutionTest("typedarray-backed-by-resizable-buffer-grow-before-end");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer-grow-mid-iteration.js", Skip = "Resizable ArrayBuffer is not supported by JROC.")]
    public Task typedarray_backed_by_resizable_buffer_grow_mid_iteration_663()
        => ExecutionTest("typedarray-backed-by-resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer-shrink-mid-iteration.js", Skip = "Resizable ArrayBuffer is not supported by JROC.")]
    public Task typedarray_backed_by_resizable_buffer_shrink_mid_iteration_664()
        => ExecutionTest("typedarray-backed-by-resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer-shrink-to-zero-mid-iteration.js", Skip = "Resizable ArrayBuffer is not supported by JROC.")]
    public Task typedarray_backed_by_resizable_buffer_shrink_to_zero_mid_iteration_665()
        => ExecutionTest("typedarray-backed-by-resizable-buffer-shrink-to-zero-mid-iteration");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer.js", Skip = "Resizable ArrayBuffer is not supported by JROC.")]
    public Task typedarray_backed_by_resizable_buffer_666()
        => ExecutionTest("typedarray-backed-by-resizable-buffer");

    [Fact(DisplayName = "uint16array-mutate.js")]
    public Task uint16array_mutate_667()
        => ExecutionTest("uint16array-mutate");

    [Fact(DisplayName = "uint16array.js")]
    public Task uint16array_668()
        => ExecutionTest("uint16array");

    [Fact(DisplayName = "uint32array-mutate.js")]
    public Task uint32array_mutate_669()
        => ExecutionTest("uint32array-mutate");

    [Fact(DisplayName = "uint32array.js")]
    public Task uint32array_670()
        => ExecutionTest("uint32array");

    [Fact(DisplayName = "yield-from-catch.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_from_catch_671()
        => ExecutionTest("yield-from-catch");

    [Fact(DisplayName = "yield-from-finally.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_from_finally_672()
        => ExecutionTest("yield-from-finally");

    [Fact(DisplayName = "yield-from-try.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_from_try_673()
        => ExecutionTest("yield-from-try");

    [Fact(DisplayName = "yield-star-from-catch.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_star_from_catch_674()
        => ExecutionTest("yield-star-from-catch");

    [Fact(DisplayName = "yield-star-from-finally.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_star_from_finally_675()
        => ExecutionTest("yield-star-from-finally");

    [Fact(DisplayName = "yield-star-from-try.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_star_from_try_676()
        => ExecutionTest("yield-star-from-try");

    [Fact(DisplayName = "yield-star.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_star_677()
        => ExecutionTest("yield-star");

    [Fact(DisplayName = "yield.js", Skip = "Pending complete iterator protocol and abrupt-completion semantics.")]
    public Task yield_678()
        => ExecutionTest("yield");
}
