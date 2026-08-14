namespace Jroc.Test262.Tests.language.statements.for_await_of;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/statements/for-await-of", "language.statements.for_await_of") { }

    [Fact(DisplayName = "async-from-sync-iterator-continuation-abrupt-completion-get-constructor.js")]
    public Task async_from_sync_iterator_continuation_abrupt_completion_get_constructor_1()
        => ExecutionTest("async-from-sync-iterator-continuation-abrupt-completion-get-constructor");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-assignment.js")]
    public Task async_func_decl_dstr_array_elem_init_assignment_2()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-assignment");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-evaluation.js")]
    public Task async_func_decl_dstr_array_elem_init_evaluation_3()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-evaluation");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_array_elem_init_fn_name_arrow_4()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_array_elem_init_fn_name_class_5()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-fn-name-class");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_array_elem_init_fn_name_cover_6()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-fn-name-cover");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_array_elem_init_fn_name_fn_7()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-fn-name-fn");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_array_elem_init_fn_name_gen_8()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-fn-name-gen");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-in.js")]
    public Task async_func_decl_dstr_array_elem_init_in_9()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-in");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-order.js")]
    public Task async_func_decl_dstr_array_elem_init_order_10()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-order");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-simple-no-strict.js")]
    public Task async_func_decl_dstr_array_elem_init_simple_no_strict_11()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-simple-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task async_func_decl_dstr_array_elem_init_yield_ident_invalid_12()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-init-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-init-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_elem_init_yield_ident_valid_13()
        => ExecutionTest("async-func-decl-dstr-array-elem-init-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-iter-nrml-close.js")]
    public Task async_func_decl_dstr_array_elem_iter_nrml_close_14()
        => ExecutionTest("async-func-decl-dstr-array-elem-iter-nrml-close");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-invalid.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_invalid_15()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-nested-array-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-null.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_null_16()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array-null");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-undefined-hole.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_undefined_hole_17()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array-undefined-hole");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-undefined-own.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_undefined_own_18()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array-undefined-own");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-undefined.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_undefined_19()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array-undefined");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task async_func_decl_dstr_array_elem_nested_array_yield_ident_invalid_20()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-nested-array-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_yield_ident_valid_21()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-array.js")]
    public Task async_func_decl_dstr_array_elem_nested_array_22()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-array");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-invalid.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_invalid_23()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-nested-obj-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-null.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_null_24()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj-null");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-undefined-hole.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_undefined_hole_25()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj-undefined-hole");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-undefined-own.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_undefined_own_26()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj-undefined-own");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-undefined.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_undefined_27()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj-undefined");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-yield-ident-invalid.js", Skip = "Pending complete for-in/of early-error validation.")]
    public Task async_func_decl_dstr_array_elem_nested_obj_yield_ident_invalid_28()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-nested-obj-yield-ident-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_yield_ident_valid_29()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-nested-obj.js")]
    public Task async_func_decl_dstr_array_elem_nested_obj_30()
        => ExecutionTest("async-func-decl-dstr-array-elem-nested-obj");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-const.js")]
    public Task async_func_decl_dstr_array_elem_put_const_31()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-const");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-prop-ref-no-get.js")]
    public Task async_func_decl_dstr_array_elem_put_prop_ref_no_get_32()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-prop-ref-no-get");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-prop-ref-user-err.js")]
    public Task async_func_decl_dstr_array_elem_put_prop_ref_user_err_33()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-prop-ref-user-err");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-prop-ref.js")]
    public Task async_func_decl_dstr_array_elem_put_prop_ref_34()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-prop-ref");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-unresolvable-no-strict.js")]
    public Task async_func_decl_dstr_array_elem_put_unresolvable_no_strict_35()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-unresolvable-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-put-unresolvable-strict.js")]
    public Task async_func_decl_dstr_array_elem_put_unresolvable_strict_36()
        => ExecutionTest("async-func-decl-dstr-array-elem-put-unresolvable-strict");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-target-identifier.js")]
    public Task async_func_decl_dstr_array_elem_target_identifier_37()
        => ExecutionTest("async-func-decl-dstr-array-elem-target-identifier");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-target-simple-strict.js")]
    public Task async_func_decl_dstr_array_elem_target_simple_strict_38()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-target-simple-strict", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-target-yield-invalid.js")]
    public Task async_func_decl_dstr_array_elem_target_yield_invalid_39()
        => CompilationFailureTest("async-func-decl-dstr-array-elem-target-yield-invalid", string.Empty);

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-target-yield-valid.js")]
    public Task async_func_decl_dstr_array_elem_target_yield_valid_40()
        => ExecutionTest("async-func-decl-dstr-array-elem-target-yield-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-elem-trlg-iter-elision-iter-nrml-close-null.js")]
    public Task async_func_decl_dstr_array_elem_trlg_iter_elision_iter_nrml_close_null_41()
        => ExecutionTest("async-func-decl-dstr-array-elem-trlg-iter-elision-iter-nrml-close-null");

    [Fact(DisplayName = "async-func-decl-dstr-array-elision-val-array.js")]
    public Task async_func_decl_dstr_array_elision_val_array_42()
        => ExecutionTest("async-func-decl-dstr-array-elision-val-array");

    [Fact(DisplayName = "async-func-decl-dstr-array-elision-val-string.js")]
    public Task async_func_decl_dstr_array_elision_val_string_43()
        => ExecutionTest("async-func-decl-dstr-array-elision-val-string");

    [Fact(DisplayName = "async-func-decl-dstr-array-empty-val-array.js")]
    public Task async_func_decl_dstr_array_empty_val_array_44()
        => ExecutionTest("async-func-decl-dstr-array-empty-val-array");

    [Fact(DisplayName = "async-func-decl-dstr-array-empty-val-string.js")]
    public Task async_func_decl_dstr_array_empty_val_string_45()
        => ExecutionTest("async-func-decl-dstr-array-empty-val-string");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-after-element.js")]
    public Task async_func_decl_dstr_array_rest_after_element_46()
        => ExecutionTest("async-func-decl-dstr-array-rest-after-element");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-after-elision.js")]
    public Task async_func_decl_dstr_array_rest_after_elision_47()
        => ExecutionTest("async-func-decl-dstr-array-rest-after-elision");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-elision.js")]
    public Task async_func_decl_dstr_array_rest_elision_48()
        => ExecutionTest("async-func-decl-dstr-array-rest-elision");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array-null.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_null_49()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array-null");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array-undefined-hole.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_undefined_hole_50()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array-undefined-hole");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array-undefined-own.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_undefined_own_51()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array-undefined-own");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array-undefined.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_undefined_52()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array-undefined");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_yield_ident_valid_53()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-array.js")]
    public Task async_func_decl_dstr_array_rest_nested_array_54()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-array");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj-null.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_null_55()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj-null");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj-undefined-hole.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_undefined_hole_56()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj-undefined-hole");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj-undefined-own.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_undefined_own_57()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj-undefined-own");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj-undefined.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_undefined_58()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj-undefined");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_yield_ident_valid_59()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-nested-obj.js")]
    public Task async_func_decl_dstr_array_rest_nested_obj_60()
        => ExecutionTest("async-func-decl-dstr-array-rest-nested-obj");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-put-prop-ref-no-get.js")]
    public Task async_func_decl_dstr_array_rest_put_prop_ref_no_get_61()
        => ExecutionTest("async-func-decl-dstr-array-rest-put-prop-ref-no-get");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-put-prop-ref.js")]
    public Task async_func_decl_dstr_array_rest_put_prop_ref_62()
        => ExecutionTest("async-func-decl-dstr-array-rest-put-prop-ref");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-put-unresolvable-no-strict.js")]
    public Task async_func_decl_dstr_array_rest_put_unresolvable_no_strict_63()
        => ExecutionTest("async-func-decl-dstr-array-rest-put-unresolvable-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-yield-ident-valid.js")]
    public Task async_func_decl_dstr_array_rest_yield_ident_valid_64()
        => ExecutionTest("async-func-decl-dstr-array-rest-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-empty-symbol.js")]
    public Task async_func_decl_dstr_obj_empty_symbol_65()
        => ExecutionTest("async-func-decl-dstr-obj-empty-symbol");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-identifier-resolution-first.js")]
    public Task async_func_decl_dstr_obj_id_identifier_resolution_first_66()
        => ExecutionTest("async-func-decl-dstr-obj-id-identifier-resolution-first");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-identifier-resolution-last.js")]
    public Task async_func_decl_dstr_obj_id_identifier_resolution_last_67()
        => ExecutionTest("async-func-decl-dstr-obj-id-identifier-resolution-last");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-identifier-resolution-lone.js")]
    public Task async_func_decl_dstr_obj_id_identifier_resolution_lone_68()
        => ExecutionTest("async-func-decl-dstr-obj-id-identifier-resolution-lone");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-identifier-resolution-middle.js")]
    public Task async_func_decl_dstr_obj_id_identifier_resolution_middle_69()
        => ExecutionTest("async-func-decl-dstr-obj-id-identifier-resolution-middle");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-identifier-resolution-trlng.js")]
    public Task async_func_decl_dstr_obj_id_identifier_resolution_trlng_70()
        => ExecutionTest("async-func-decl-dstr-obj-id-identifier-resolution-trlng");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-assignment-missing.js")]
    public Task async_func_decl_dstr_obj_id_init_assignment_missing_71()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-assignment-missing");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-assignment-null.js")]
    public Task async_func_decl_dstr_obj_id_init_assignment_null_72()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-assignment-null");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-assignment-truthy.js")]
    public Task async_func_decl_dstr_obj_id_init_assignment_truthy_73()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-assignment-truthy");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-assignment-undef.js")]
    public Task async_func_decl_dstr_obj_id_init_assignment_undef_74()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-assignment-undef");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-evaluation.js")]
    public Task async_func_decl_dstr_obj_id_init_evaluation_75()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-evaluation");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_id_init_fn_name_arrow_76()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_id_init_fn_name_class_77()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_id_init_fn_name_cover_78()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_id_init_fn_name_fn_79()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_id_init_fn_name_gen_80()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-in.js")]
    public Task async_func_decl_dstr_obj_id_init_in_81()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-in");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-order.js")]
    public Task async_func_decl_dstr_obj_id_init_order_82()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-order");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-simple-no-strict.js")]
    public Task async_func_decl_dstr_obj_id_init_simple_no_strict_83()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-simple-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-init-yield-ident-valid.js")]
    public Task async_func_decl_dstr_obj_id_init_yield_ident_valid_84()
        => ExecutionTest("async-func-decl-dstr-obj-id-init-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-put-unresolvable-no-strict.js")]
    public Task async_func_decl_dstr_obj_id_put_unresolvable_no_strict_85()
        => ExecutionTest("async-func-decl-dstr-obj-id-put-unresolvable-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-obj-id-simple-no-strict.js")]
    public Task async_func_decl_dstr_obj_id_simple_no_strict_86()
        => ExecutionTest("async-func-decl-dstr-obj-id-simple-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-assignment-missing.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_assignment_missing_87()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-assignment-missing");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-assignment-null.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_assignment_null_88()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-assignment-null");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-assignment-truthy.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_assignment_truthy_89()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-assignment-truthy");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-assignment-undef.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_assignment_undef_90()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-assignment-undef");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-evaluation.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_evaluation_91()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-evaluation");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_prop_elem_init_fn_name_arrow_92()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_prop_elem_init_fn_name_class_93()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-fn-name-class");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_prop_elem_init_fn_name_cover_94()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-fn-name-cover");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_prop_elem_init_fn_name_fn_95()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-fn-name-fn");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_decl_dstr_obj_prop_elem_init_fn_name_gen_96()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-fn-name-gen");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-in.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_in_97()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-in");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-init-yield-ident-valid.js")]
    public Task async_func_decl_dstr_obj_prop_elem_init_yield_ident_valid_98()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-init-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-elem-target-yield-ident-valid.js")]
    public Task async_func_decl_dstr_obj_prop_elem_target_yield_ident_valid_99()
        => ExecutionTest("async-func-decl-dstr-obj-prop-elem-target-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-identifier-resolution-first.js")]
    public Task async_func_decl_dstr_obj_prop_identifier_resolution_first_100()
        => ExecutionTest("async-func-decl-dstr-obj-prop-identifier-resolution-first");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-identifier-resolution-last.js")]
    public Task async_func_decl_dstr_obj_prop_identifier_resolution_last_101()
        => ExecutionTest("async-func-decl-dstr-obj-prop-identifier-resolution-last");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-identifier-resolution-lone.js")]
    public Task async_func_decl_dstr_obj_prop_identifier_resolution_lone_102()
        => ExecutionTest("async-func-decl-dstr-obj-prop-identifier-resolution-lone");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-identifier-resolution-middle.js")]
    public Task async_func_decl_dstr_obj_prop_identifier_resolution_middle_103()
        => ExecutionTest("async-func-decl-dstr-obj-prop-identifier-resolution-middle");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-identifier-resolution-trlng.js")]
    public Task async_func_decl_dstr_obj_prop_identifier_resolution_trlng_104()
        => ExecutionTest("async-func-decl-dstr-obj-prop-identifier-resolution-trlng");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-name-evaluation.js")]
    public Task async_func_decl_dstr_obj_prop_name_evaluation_105()
        => ExecutionTest("async-func-decl-dstr-obj-prop-name-evaluation");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-nested-array-yield-ident-valid.js")]
    public Task async_func_decl_dstr_obj_prop_nested_array_yield_ident_valid_106()
        => ExecutionTest("async-func-decl-dstr-obj-prop-nested-array-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-nested-array.js")]
    public Task async_func_decl_dstr_obj_prop_nested_array_107()
        => ExecutionTest("async-func-decl-dstr-obj-prop-nested-array");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-nested-obj-yield-ident-valid.js")]
    public Task async_func_decl_dstr_obj_prop_nested_obj_yield_ident_valid_108()
        => ExecutionTest("async-func-decl-dstr-obj-prop-nested-obj-yield-ident-valid");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-nested-obj.js")]
    public Task async_func_decl_dstr_obj_prop_nested_obj_109()
        => ExecutionTest("async-func-decl-dstr-obj-prop-nested-obj");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-put-order.js")]
    public Task async_func_decl_dstr_obj_prop_put_order_110()
        => ExecutionTest("async-func-decl-dstr-obj-prop-put-order");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-put-prop-ref-no-get.js")]
    public Task async_func_decl_dstr_obj_prop_put_prop_ref_no_get_111()
        => ExecutionTest("async-func-decl-dstr-obj-prop-put-prop-ref-no-get");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-put-prop-ref.js")]
    public Task async_func_decl_dstr_obj_prop_put_prop_ref_112()
        => ExecutionTest("async-func-decl-dstr-obj-prop-put-prop-ref");

    [Fact(DisplayName = "async-func-decl-dstr-obj-prop-put-unresolvable-no-strict.js")]
    public Task async_func_decl_dstr_obj_prop_put_unresolvable_no_strict_113()
        => ExecutionTest("async-func-decl-dstr-obj-prop-put-unresolvable-no-strict");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-descriptors.js")]
    public Task async_func_decl_dstr_obj_rest_descriptors_114()
        => ExecutionTest("async-func-decl-dstr-obj-rest-descriptors");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-empty-obj.js")]
    public Task async_func_decl_dstr_obj_rest_empty_obj_115()
        => ExecutionTest("async-func-decl-dstr-obj-rest-empty-obj");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-getter.js")]
    public Task async_func_decl_dstr_obj_rest_getter_116()
        => ExecutionTest("async-func-decl-dstr-obj-rest-getter");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-number.js")]
    public Task async_func_decl_dstr_obj_rest_number_117()
        => ExecutionTest("async-func-decl-dstr-obj-rest-number");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-same-name.js")]
    public Task async_func_decl_dstr_obj_rest_same_name_118()
        => ExecutionTest("async-func-decl-dstr-obj-rest-same-name");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-skip-non-enumerable.js")]
    public Task async_func_decl_dstr_obj_rest_skip_non_enumerable_119()
        => ExecutionTest("async-func-decl-dstr-obj-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-str-val.js")]
    public Task async_func_decl_dstr_obj_rest_str_val_120()
        => ExecutionTest("async-func-decl-dstr-obj-rest-str-val");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-symbol-val.js")]
    public Task async_func_decl_dstr_obj_rest_symbol_val_121()
        => ExecutionTest("async-func-decl-dstr-obj-rest-symbol-val");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-to-property-with-setter.js")]
    public Task async_func_decl_dstr_obj_rest_to_property_with_setter_122()
        => ExecutionTest("async-func-decl-dstr-obj-rest-to-property-with-setter");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-to-property.js")]
    public Task async_func_decl_dstr_obj_rest_to_property_123()
        => ExecutionTest("async-func-decl-dstr-obj-rest-to-property");

    [Fact(DisplayName = "async-func-decl-dstr-obj-rest-valid-object.js")]
    public Task async_func_decl_dstr_obj_rest_valid_object_124()
        => ExecutionTest("async-func-decl-dstr-obj-rest-valid-object");

    [Fact(DisplayName = "async-func-dstr-const-ary-init-iter-close.js")]
    public Task async_func_dstr_const_ary_init_iter_close_125()
        => ExecutionTest("async-func-dstr-const-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-const-ary-init-iter-get-err.js")]
    public Task async_func_dstr_const_ary_init_iter_get_err_126()
        => ExecutionTest("async-func-dstr-const-ary-init-iter-get-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-init-iter-no-close.js")]
    public Task async_func_dstr_const_ary_init_iter_no_close_127()
        => ExecutionTest("async-func-dstr-const-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-const-ary-name-iter-val.js")]
    public Task async_func_dstr_const_ary_name_iter_val_128()
        => ExecutionTest("async-func-dstr-const-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_elem_init_129()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-elem-iter.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_elem_iter_130()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-elision-init.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_elision_init_131()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-elision-iter.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_elision_iter_132()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_empty_init_133()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_empty_iter_134()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-rest-init.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_rest_init_135()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_rest_iter_136()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-ary-val-null.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_ary_val_null_137()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-ary-val-null");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-exhausted.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_exhausted_138()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_fn_name_arrow_139()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_fn_name_class_140()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_fn_name_cover_141()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_fn_name_fn_142()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_fn_name_gen_143()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-hole.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_hole_144()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-skipped.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_skipped_145()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-throws.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_throws_146()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-throws");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-undef.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_undef_147()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-init-unresolvable.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_init_unresolvable_148()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-init-unresolvable");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-iter-complete.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_iter_complete_149()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-iter-done.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_iter_done_150()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-iter-step-err.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_iter_step_err_151()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-iter-val-err.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_iter_val_err_152()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-id-iter-val.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_id_iter_val_153()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-id-init.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_id_init_154()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-id.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_id_155()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_prop_id_init_156()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-prop-id.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_prop_id_157()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-val-null.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_val_null_158()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-val-null");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elem-obj-val-undef.js")]
    public Task async_func_dstr_const_ary_ptrn_elem_obj_val_undef_159()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elem-obj-val-undef");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elision-exhausted.js")]
    public Task async_func_dstr_const_ary_ptrn_elision_exhausted_160()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elision-iter-close.js")]
    public Task async_func_dstr_const_ary_ptrn_elision_iter_close_161()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elision-iter-close");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elision-step-err.js")]
    public Task async_func_dstr_const_ary_ptrn_elision_step_err_162()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elision-step-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-elision.js")]
    public Task async_func_dstr_const_ary_ptrn_elision_163()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-elision");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-empty.js")]
    public Task async_func_dstr_const_ary_ptrn_empty_164()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-ary-elem.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_ary_elem_165()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-ary-elision.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_ary_elision_166()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-ary-empty.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_ary_empty_167()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-ary-rest.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_ary_rest_168()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-elision-next-err.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_elision_next_err_169()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-elision-next-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-elision.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_elision_170()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-exhausted.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_exhausted_171()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-iter-close.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_iter_close_172()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-iter-close");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-iter-step-err.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_iter_step_err_173()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-iter-step-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id-iter-val-err.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_iter_val_err_174()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id-iter-val-err");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-id.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_id_175()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-init-ary.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_init_ary_176()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-init-id.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_init_id_177()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-init-obj.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_init_obj_178()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-not-final-ary.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_not_final_ary_179()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-not-final-id.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_not_final_id_180()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-not-final-obj.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_not_final_obj_181()
        => CompilationFailureTest("async-func-dstr-const-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-obj-id.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_obj_id_182()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-func-dstr-const-ary-ptrn-rest-obj-prop-id.js")]
    public Task async_func_dstr_const_ary_ptrn_rest_obj_prop_id_183()
        => ExecutionTest("async-func-dstr-const-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-init-iter-close.js")]
    public Task async_func_dstr_const_async_ary_init_iter_close_184()
        => ExecutionTest("async-func-dstr-const-async-ary-init-iter-close");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-init-iter-no-close.js")]
    public Task async_func_dstr_const_async_ary_init_iter_no_close_185()
        => ExecutionTest("async-func-dstr-const-async-ary-init-iter-no-close");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-name-iter-val.js")]
    public Task async_func_dstr_const_async_ary_name_iter_val_186()
        => ExecutionTest("async-func-dstr-const-async-ary-name-iter-val");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-elem-init.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_elem_init_187()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-elem-iter.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_elem_iter_188()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-elision-init.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_elision_init_189()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-elision-iter.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_elision_iter_190()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-empty-init.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_empty_init_191()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-empty-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_empty_iter_192()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-rest-init.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_rest_init_193()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-ary-rest-iter.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_ary_rest_iter_194()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-ary-rest-iter");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-exhausted.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_exhausted_195()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_fn_name_arrow_196()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_fn_name_class_197()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_fn_name_cover_198()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_fn_name_fn_199()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_fn_name_gen_200()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-hole.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_hole_201()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-hole");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-skipped.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_skipped_202()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-init-undef.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_init_undef_203()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-init-undef");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-iter-complete.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_iter_complete_204()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-iter-complete");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-iter-done.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_iter_done_205()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-iter-done");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-id-iter-val.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_id_iter_val_206()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-id-iter-val");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-obj-id-init.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_obj_id_init_207()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-obj-id-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-obj-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_obj_id_208()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-obj-id");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-obj-prop-id-init.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_obj_prop_id_init_209()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-obj-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elem-obj-prop-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elem_obj_prop_id_210()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elem-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elision-exhausted.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elision_exhausted_211()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elision-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-elision.js")]
    public Task async_func_dstr_const_async_ary_ptrn_elision_212()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-elision");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-empty.js")]
    public Task async_func_dstr_const_async_ary_ptrn_empty_213()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-ary-elem.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_ary_elem_214()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-ary-elem");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-ary-elision.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_ary_elision_215()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-ary-elision");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-ary-empty.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_ary_empty_216()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-ary-empty");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-ary-rest.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_ary_rest_217()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-ary-rest");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-id-elision.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_id_elision_218()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-id-elision");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-id-exhausted.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_id_exhausted_219()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-id-exhausted");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_id_220()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-id");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-init-ary.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_init_ary_221()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-init-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-init-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_init_id_222()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-init-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-init-obj.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_init_obj_223()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-init-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-not-final-ary.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_not_final_ary_224()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-not-final-ary", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-not-final-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_not_final_id_225()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-not-final-id", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-not-final-obj.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_not_final_obj_226()
        => CompilationFailureTest("async-func-dstr-const-async-ary-ptrn-rest-not-final-obj", string.Empty);

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-obj-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_obj_id_227()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-obj-id");

    [Fact(DisplayName = "async-func-dstr-const-async-ary-ptrn-rest-obj-prop-id.js")]
    public Task async_func_dstr_const_async_ary_ptrn_rest_obj_prop_id_228()
        => ExecutionTest("async-func-dstr-const-async-ary-ptrn-rest-obj-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-empty.js")]
    public Task async_func_dstr_const_async_obj_ptrn_empty_229()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-empty");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-fn-name-arrow.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_fn_name_arrow_230()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-fn-name-arrow");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-fn-name-class.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_fn_name_class_231()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-fn-name-class");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-fn-name-cover.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_fn_name_cover_232()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-fn-name-cover");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-fn-name-fn.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_fn_name_fn_233()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-fn-name-fn");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-fn-name-gen.js", Skip = "Pending complete for-in/of destructuring lowering.")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_fn_name_gen_234()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-fn-name-gen");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-init-skipped.js")]
    public Task async_func_dstr_const_async_obj_ptrn_id_init_skipped_235()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-id-trailing-comma.js")]
    public Task async_func_dstr_const_async_obj_ptrn_id_trailing_comma_236()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-ary-init.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_ary_init_237()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-ary-trailing-comma.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_ary_trailing_comma_238()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-ary-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-ary.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_ary_239()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-ary");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-id-init-skipped.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_id_init_skipped_240()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-id-init-skipped");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-id-init.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_id_init_241()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-id-init");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-id-trailing-comma.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_id_trailing_comma_242()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-id-trailing-comma");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-id.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_id_243()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-id");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-obj-init.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_obj_init_244()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-prop-obj.js")]
    public Task async_func_dstr_const_async_obj_ptrn_prop_obj_245()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-prop-obj");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-rest-getter.js")]
    public Task async_func_dstr_const_async_obj_ptrn_rest_getter_246()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-rest-getter");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-rest-skip-non-enumerable.js")]
    public Task async_func_dstr_const_async_obj_ptrn_rest_skip_non_enumerable_247()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-rest-skip-non-enumerable");

    [Fact(DisplayName = "async-func-dstr-const-async-obj-ptrn-rest-val-obj.js")]
    public Task async_func_dstr_const_async_obj_ptrn_rest_val_obj_248()
        => ExecutionTest("async-func-dstr-const-async-obj-ptrn-rest-val-obj");

    [Fact(DisplayName = "async-func-dstr-const-obj-init-null.js")]
    public Task async_func_dstr_const_obj_init_null_249()
        => ExecutionTest("async-func-dstr-const-obj-init-null");

    [Fact(DisplayName = "async-func-dstr-const-obj-init-undefined.js")]
    public Task async_func_dstr_const_obj_init_undefined_250()
        => ExecutionTest("async-func-dstr-const-obj-init-undefined");
}
