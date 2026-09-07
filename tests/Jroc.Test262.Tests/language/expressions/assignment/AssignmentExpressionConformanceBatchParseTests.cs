using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.assignment;

public class AssignmentExpressionConformanceBatchParseTests : FileSystemExecutionTestsBase
{
    public AssignmentExpressionConformanceBatchParseTests() : base("language/expressions/assignment", "language.expressions.assignment") { }

    [Fact(DisplayName = "array-elem-init-yield-ident-invalid.js")]
    public Task dstr_array_elem_init_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-elem-init-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-nested-array-invalid.js")]
    public Task dstr_array_elem_nested_array_invalid()
        => CompilationFailureTest("dstr/array-elem-nested-array-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-nested-array-yield-ident-invalid.js")]
    public Task dstr_array_elem_nested_array_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-elem-nested-array-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-nested-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_nested_memberexpr_optchain_prop_ref_init()
        => CompilationFailureTest("dstr/array-elem-nested-memberexpr-optchain-prop-ref-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-nested-obj-invalid.js")]
    public Task dstr_array_elem_nested_obj_invalid()
        => CompilationFailureTest("dstr/array-elem-nested-obj-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-nested-obj-yield-ident-invalid.js")]
    public Task dstr_array_elem_nested_obj_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-elem-nested-obj-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-put-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_array_elem_put_obj_literal_optchain_prop_ref_init()
        => CompilationFailureTest("dstr/array-elem-put-obj-literal-optchain-prop-ref-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-target-simple-strict.js")]
    public Task dstr_array_elem_target_simple_strict()
        => CompilationFailureTest("dstr/array-elem-target-simple-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-elem-target-yield-invalid.js")]
    public Task dstr_array_elem_target_yield_invalid()
        => CompilationFailureTest("dstr/array-elem-target-yield-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-before-element.js")]
    public Task dstr_array_rest_before_element()
        => CompilationFailureTest("dstr/array-rest-before-element", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-before-elision.js")]
    public Task dstr_array_rest_before_elision()
        => CompilationFailureTest("dstr/array-rest-before-elision", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-before-rest.js")]
    public Task dstr_array_rest_before_rest()
        => CompilationFailureTest("dstr/array-rest-before-rest", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-elision-invalid.js")]
    public Task dstr_array_rest_elision_invalid()
        => CompilationFailureTest("dstr/array-rest-elision-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-init.js")]
    public Task dstr_array_rest_init()
        => CompilationFailureTest("dstr/array-rest-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-nested-array-invalid.js")]
    public Task dstr_array_rest_nested_array_invalid()
        => CompilationFailureTest("dstr/array-rest-nested-array-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-nested-array-yield-ident-invalid.js")]
    public Task dstr_array_rest_nested_array_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-rest-nested-array-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-nested-obj-invalid.js")]
    public Task dstr_array_rest_nested_obj_invalid()
        => CompilationFailureTest("dstr/array-rest-nested-obj-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-nested-obj-yield-ident-invalid.js")]
    public Task dstr_array_rest_nested_obj_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-rest-nested-obj-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "array-rest-yield-ident-invalid.js")]
    public Task dstr_array_rest_yield_ident_invalid()
        => CompilationFailureTest("dstr/array-rest-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-id-identifier-yield-expr.js")]
    public Task dstr_obj_id_identifier_yield_expr()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-id-identifier-yield-ident-invalid.js")]
    public Task dstr_obj_id_identifier_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-id-identifier-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-id-init-simple-strict.js")]
    public Task dstr_obj_id_init_simple_strict()
        => CompilationFailureTest("dstr/obj-id-init-simple-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-id-init-yield-ident-invalid.js")]
    public Task dstr_obj_id_init_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-id-init-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-id-simple-strict.js")]
    public Task dstr_obj_id_simple_strict()
        => CompilationFailureTest("dstr/obj-id-simple-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-elem-init-yield-ident-invalid.js")]
    public Task dstr_obj_prop_elem_init_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-prop-elem-init-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-elem-target-memberexpr-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_memberexpr_optchain_prop_ref_init()
        => CompilationFailureTest("dstr/obj-prop-elem-target-memberexpr-optchain-prop-ref-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-elem-target-obj-literal-optchain-prop-ref-init.js")]
    public Task dstr_obj_prop_elem_target_obj_literal_optchain_prop_ref_init()
        => CompilationFailureTest("dstr/obj-prop-elem-target-obj-literal-optchain-prop-ref-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-elem-target-yield-ident-invalid.js")]
    public Task dstr_obj_prop_elem_target_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-prop-elem-target-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-nested-array-invalid.js")]
    public Task dstr_obj_prop_nested_array_invalid()
        => CompilationFailureTest("dstr/obj-prop-nested-array-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-nested-array-yield-ident-invalid.js")]
    public Task dstr_obj_prop_nested_array_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-prop-nested-array-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-nested-obj-invalid.js")]
    public Task dstr_obj_prop_nested_obj_invalid()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-prop-nested-obj-yield-ident-invalid.js")]
    public Task dstr_obj_prop_nested_obj_yield_ident_invalid()
        => CompilationFailureTest("dstr/obj-prop-nested-obj-yield-ident-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-rest-not-last-element-invalid.js")]
    public Task dstr_obj_rest_not_last_element_invalid()
        => CompilationFailureTest("dstr/obj-rest-not-last-element-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-break-escaped.js")]
    public Task dstr_syntax_error_ident_ref_break_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-break-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-case-escaped.js")]
    public Task dstr_syntax_error_ident_ref_case_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-case-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-catch-escaped.js")]
    public Task dstr_syntax_error_ident_ref_catch_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-catch-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-class-escaped.js")]
    public Task dstr_syntax_error_ident_ref_class_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-class-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-const-escaped.js")]
    public Task dstr_syntax_error_ident_ref_const_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-const-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-continue-escaped.js")]
    public Task dstr_syntax_error_ident_ref_continue_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-continue-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-debugger-escaped.js")]
    public Task dstr_syntax_error_ident_ref_debugger_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-debugger-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default-escaped-ext.js")]
    public Task dstr_syntax_error_ident_ref_default_escaped_ext()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default-escaped-ext", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default-escaped.js")]
    public Task dstr_syntax_error_ident_ref_default_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-default.js")]
    public Task dstr_syntax_error_ident_ref_default()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-default", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-delete-escaped.js")]
    public Task dstr_syntax_error_ident_ref_delete_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-delete-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-do-escaped.js")]
    public Task dstr_syntax_error_ident_ref_do_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-do-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-else-escaped.js")]
    public Task dstr_syntax_error_ident_ref_else_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-else-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-enum-escaped.js")]
    public Task dstr_syntax_error_ident_ref_enum_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-enum-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-export-escaped.js")]
    public Task dstr_syntax_error_ident_ref_export_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-export-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends-escaped-ext.js")]
    public Task dstr_syntax_error_ident_ref_extends_escaped_ext()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends-escaped-ext", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends-escaped.js")]
    public Task dstr_syntax_error_ident_ref_extends_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-extends.js")]
    public Task dstr_syntax_error_ident_ref_extends()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-extends", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-finally-escaped.js")]
    public Task dstr_syntax_error_ident_ref_finally_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-finally-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-for-escaped.js")]
    public Task dstr_syntax_error_ident_ref_for_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-for-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-function-escaped.js")]
    public Task dstr_syntax_error_ident_ref_function_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-function-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-if-escaped.js")]
    public Task dstr_syntax_error_ident_ref_if_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-if-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-implements-escaped.js")]
    public Task dstr_syntax_error_ident_ref_implements_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-implements-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-import-escaped.js")]
    public Task dstr_syntax_error_ident_ref_import_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-import-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-in-escaped.js")]
    public Task dstr_syntax_error_ident_ref_in_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-in-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-instanceof-escaped.js")]
    public Task dstr_syntax_error_ident_ref_instanceof_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-instanceof-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-interface-escaped.js")]
    public Task dstr_syntax_error_ident_ref_interface_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-interface-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-let-escaped.js")]
    public Task dstr_syntax_error_ident_ref_let_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-let-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-new-escaped.js")]
    public Task dstr_syntax_error_ident_ref_new_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-new-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-package-escaped.js")]
    public Task dstr_syntax_error_ident_ref_package_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-package-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-private-escaped.js")]
    public Task dstr_syntax_error_ident_ref_private_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-private-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-protected-escaped.js")]
    public Task dstr_syntax_error_ident_ref_protected_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-protected-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-public-escaped.js")]
    public Task dstr_syntax_error_ident_ref_public_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-public-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-return-escaped.js")]
    public Task dstr_syntax_error_ident_ref_return_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-return-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-static-escaped.js")]
    public Task dstr_syntax_error_ident_ref_static_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-static-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-super-escaped.js")]
    public Task dstr_syntax_error_ident_ref_super_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-super-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-switch-escaped.js")]
    public Task dstr_syntax_error_ident_ref_switch_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-switch-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-this-escaped.js")]
    public Task dstr_syntax_error_ident_ref_this_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-this-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-throw-escaped.js")]
    public Task dstr_syntax_error_ident_ref_throw_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-throw-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-try-escaped.js")]
    public Task dstr_syntax_error_ident_ref_try_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-try-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-typeof-escaped.js")]
    public Task dstr_syntax_error_ident_ref_typeof_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-typeof-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-var-escaped.js")]
    public Task dstr_syntax_error_ident_ref_var_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-var-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-void-escaped.js")]
    public Task dstr_syntax_error_ident_ref_void_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-void-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-while-escaped.js")]
    public Task dstr_syntax_error_ident_ref_while_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-while-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "syntax-error-ident-ref-with-escaped.js")]
    public Task dstr_syntax_error_ident_ref_with_escaped()
        => CompilationFailureTest("dstr/syntax-error-ident-ref-with-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "id-arguments-strict.js")]
    public Task id_arguments_strict()
        => CompilationFailureTest("id-arguments-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "id-eval-strict.js")]
    public Task id_eval_strict()
        => CompilationFailureTest("id-eval-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "non-simple-target.js")]
    public Task non_simple_target()
        => CompilationFailureTest("non-simple-target", "Failed to parse JavaScript");

    [Fact(DisplayName = "target-assignment-inside-function.js")]
    public Task target_assignment_inside_function()
        => CompilationFailureTest("target-assignment-inside-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "target-assignment.js")]
    public Task target_assignment()
        => CompilationFailureTest("target-assignment", "Failed to parse JavaScript");

    [Fact(DisplayName = "target-boolean.js")]
    public Task target_boolean()
        => CompilationFailureTest("target-boolean", "Failed to parse JavaScript");

    [Fact(DisplayName = "target-cover-newtarget.js")]
    public Task target_cover_newtarget()
        => CompilationFailureTest("target-cover-newtarget", "Failed to parse JavaScript");
}
