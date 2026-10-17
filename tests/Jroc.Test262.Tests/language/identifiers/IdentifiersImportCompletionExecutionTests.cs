namespace Jroc.Test262.Tests.language.identifiers;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.identifiers") { }

    [Fact(DisplayName = "start-zwj-escaped.js")]
    public Task start_zwj_escaped()
        => CompilationFailureTest("start-zwj-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "start-zwnj-escaped.js")]
    public Task start_zwnj_escaped()
        => CompilationFailureTest("start-zwnj-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "unicode-escape-nls-err.js")]
    public Task unicode_escape_nls_err()
        => CompilationFailureTest("unicode-escape-nls-err", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-break-via-escape-hex.js")]
    public Task val_break_via_escape_hex()
        => CompilationFailureTest("val-break-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-break-via-escape-hex4.js")]
    public Task val_break_via_escape_hex4()
        => CompilationFailureTest("val-break-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-break.js")]
    public Task val_break()
        => CompilationFailureTest("val-break", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-case-via-escape-hex.js")]
    public Task val_case_via_escape_hex()
        => CompilationFailureTest("val-case-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-case-via-escape-hex4.js")]
    public Task val_case_via_escape_hex4()
        => CompilationFailureTest("val-case-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-case.js")]
    public Task val_case()
        => CompilationFailureTest("val-case", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-catch-via-escape-hex.js")]
    public Task val_catch_via_escape_hex()
        => CompilationFailureTest("val-catch-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-catch-via-escape-hex4.js")]
    public Task val_catch_via_escape_hex4()
        => CompilationFailureTest("val-catch-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-catch.js")]
    public Task val_catch()
        => CompilationFailureTest("val-catch", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-class-via-escape-hex.js")]
    public Task val_class_via_escape_hex()
        => CompilationFailureTest("val-class-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-class-via-escape-hex4.js")]
    public Task val_class_via_escape_hex4()
        => CompilationFailureTest("val-class-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-class.js")]
    public Task val_class()
        => CompilationFailureTest("val-class", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-const-via-escape-hex.js")]
    public Task val_const_via_escape_hex()
        => CompilationFailureTest("val-const-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-const-via-escape-hex4.js")]
    public Task val_const_via_escape_hex4()
        => CompilationFailureTest("val-const-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-const.js")]
    public Task val_const()
        => CompilationFailureTest("val-const", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-continue-via-escape-hex.js")]
    public Task val_continue_via_escape_hex()
        => CompilationFailureTest("val-continue-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-continue-via-escape-hex4.js")]
    public Task val_continue_via_escape_hex4()
        => CompilationFailureTest("val-continue-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-continue.js")]
    public Task val_continue()
        => CompilationFailureTest("val-continue", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-debugger-via-escape-hex.js")]
    public Task val_debugger_via_escape_hex()
        => CompilationFailureTest("val-debugger-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-debugger-via-escape-hex4.js")]
    public Task val_debugger_via_escape_hex4()
        => CompilationFailureTest("val-debugger-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-debugger.js")]
    public Task val_debugger()
        => CompilationFailureTest("val-debugger", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-default-via-escape-hex.js")]
    public Task val_default_via_escape_hex()
        => CompilationFailureTest("val-default-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-default-via-escape-hex4.js")]
    public Task val_default_via_escape_hex4()
        => CompilationFailureTest("val-default-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-default.js")]
    public Task val_default()
        => CompilationFailureTest("val-default", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-delete-via-escape-hex.js")]
    public Task val_delete_via_escape_hex()
        => CompilationFailureTest("val-delete-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-delete-via-escape-hex4.js")]
    public Task val_delete_via_escape_hex4()
        => CompilationFailureTest("val-delete-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-delete.js")]
    public Task val_delete()
        => CompilationFailureTest("val-delete", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-do-via-escape-hex.js")]
    public Task val_do_via_escape_hex()
        => CompilationFailureTest("val-do-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-do-via-escape-hex4.js")]
    public Task val_do_via_escape_hex4()
        => CompilationFailureTest("val-do-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-do.js")]
    public Task val_do()
        => CompilationFailureTest("val-do", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-else-via-escape-hex.js")]
    public Task val_else_via_escape_hex()
        => CompilationFailureTest("val-else-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-else-via-escape-hex4.js")]
    public Task val_else_via_escape_hex4()
        => CompilationFailureTest("val-else-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-else.js")]
    public Task val_else()
        => CompilationFailureTest("val-else", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-enum-via-escape-hex.js")]
    public Task val_enum_via_escape_hex()
        => CompilationFailureTest("val-enum-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-enum-via-escape-hex4.js")]
    public Task val_enum_via_escape_hex4()
        => CompilationFailureTest("val-enum-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-enum.js")]
    public Task val_enum()
        => CompilationFailureTest("val-enum", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-export-via-escape-hex.js")]
    public Task val_export_via_escape_hex()
        => CompilationFailureTest("val-export-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-export-via-escape-hex4.js")]
    public Task val_export_via_escape_hex4()
        => CompilationFailureTest("val-export-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-export.js")]
    public Task val_export()
        => CompilationFailureTest("val-export", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-extends-via-escape-hex.js")]
    public Task val_extends_via_escape_hex()
        => CompilationFailureTest("val-extends-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-extends-via-escape-hex4.js")]
    public Task val_extends_via_escape_hex4()
        => CompilationFailureTest("val-extends-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-extends.js")]
    public Task val_extends()
        => CompilationFailureTest("val-extends", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-false-via-escape-hex.js")]
    public Task val_false_via_escape_hex()
        => CompilationFailureTest("val-false-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-false-via-escape-hex4.js")]
    public Task val_false_via_escape_hex4()
        => CompilationFailureTest("val-false-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-false.js")]
    public Task val_false()
        => CompilationFailureTest("val-false", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-finally-via-escape-hex.js")]
    public Task val_finally_via_escape_hex()
        => CompilationFailureTest("val-finally-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-finally-via-escape-hex4.js")]
    public Task val_finally_via_escape_hex4()
        => CompilationFailureTest("val-finally-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-finally.js")]
    public Task val_finally()
        => CompilationFailureTest("val-finally", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-for-via-escape-hex.js")]
    public Task val_for_via_escape_hex()
        => CompilationFailureTest("val-for-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-for-via-escape-hex4.js")]
    public Task val_for_via_escape_hex4()
        => CompilationFailureTest("val-for-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-for.js")]
    public Task val_for()
        => CompilationFailureTest("val-for", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-function-via-escape-hex.js")]
    public Task val_function_via_escape_hex()
        => CompilationFailureTest("val-function-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-function-via-escape-hex4.js")]
    public Task val_function_via_escape_hex4()
        => CompilationFailureTest("val-function-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-function.js")]
    public Task val_function()
        => CompilationFailureTest("val-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-if-via-escape-hex.js")]
    public Task val_if_via_escape_hex()
        => CompilationFailureTest("val-if-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-if-via-escape-hex4.js")]
    public Task val_if_via_escape_hex4()
        => CompilationFailureTest("val-if-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-if.js")]
    public Task val_if()
        => CompilationFailureTest("val-if", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-import-via-escape-hex.js")]
    public Task val_import_via_escape_hex()
        => CompilationFailureTest("val-import-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-import-via-escape-hex4.js")]
    public Task val_import_via_escape_hex4()
        => CompilationFailureTest("val-import-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-import.js")]
    public Task val_import()
        => CompilationFailureTest("val-import", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-in-via-escape-hex.js")]
    public Task val_in_via_escape_hex()
        => CompilationFailureTest("val-in-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-in-via-escape-hex4.js")]
    public Task val_in_via_escape_hex4()
        => CompilationFailureTest("val-in-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-in.js")]
    public Task val_in()
        => CompilationFailureTest("val-in", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-instanceof-via-escape-hex.js")]
    public Task val_instanceof_via_escape_hex()
        => CompilationFailureTest("val-instanceof-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-instanceof-via-escape-hex4.js")]
    public Task val_instanceof_via_escape_hex4()
        => CompilationFailureTest("val-instanceof-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-instanceof.js")]
    public Task val_instanceof()
        => CompilationFailureTest("val-instanceof", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-new-via-escape-hex.js")]
    public Task val_new_via_escape_hex()
        => CompilationFailureTest("val-new-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-new-via-escape-hex4.js")]
    public Task val_new_via_escape_hex4()
        => CompilationFailureTest("val-new-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-new.js")]
    public Task val_new()
        => CompilationFailureTest("val-new", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-null-via-escape-hex.js")]
    public Task val_null_via_escape_hex()
        => CompilationFailureTest("val-null-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-null-via-escape-hex4.js")]
    public Task val_null_via_escape_hex4()
        => CompilationFailureTest("val-null-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-null.js")]
    public Task val_null()
        => CompilationFailureTest("val-null", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-return-via-escape-hex.js")]
    public Task val_return_via_escape_hex()
        => CompilationFailureTest("val-return-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-return-via-escape-hex4.js")]
    public Task val_return_via_escape_hex4()
        => CompilationFailureTest("val-return-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-return.js")]
    public Task val_return()
        => CompilationFailureTest("val-return", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-super-via-escape-hex.js")]
    public Task val_super_via_escape_hex()
        => CompilationFailureTest("val-super-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-super-via-escape-hex4.js")]
    public Task val_super_via_escape_hex4()
        => CompilationFailureTest("val-super-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-super.js")]
    public Task val_super()
        => CompilationFailureTest("val-super", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-switch-via-escape-hex.js")]
    public Task val_switch_via_escape_hex()
        => CompilationFailureTest("val-switch-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-switch-via-escape-hex4.js")]
    public Task val_switch_via_escape_hex4()
        => CompilationFailureTest("val-switch-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-switch.js")]
    public Task val_switch()
        => CompilationFailureTest("val-switch", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-this-via-escape-hex.js")]
    public Task val_this_via_escape_hex()
        => CompilationFailureTest("val-this-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-this-via-escape-hex4.js")]
    public Task val_this_via_escape_hex4()
        => CompilationFailureTest("val-this-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-this.js")]
    public Task val_this()
        => CompilationFailureTest("val-this", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-throw-via-escape-hex.js")]
    public Task val_throw_via_escape_hex()
        => CompilationFailureTest("val-throw-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-throw-via-escape-hex4.js")]
    public Task val_throw_via_escape_hex4()
        => CompilationFailureTest("val-throw-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-throw.js")]
    public Task val_throw()
        => CompilationFailureTest("val-throw", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-true-via-escape-hex.js")]
    public Task val_true_via_escape_hex()
        => CompilationFailureTest("val-true-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-true-via-escape-hex4.js")]
    public Task val_true_via_escape_hex4()
        => CompilationFailureTest("val-true-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-true.js")]
    public Task val_true()
        => CompilationFailureTest("val-true", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-try-via-escape-hex.js")]
    public Task val_try_via_escape_hex()
        => CompilationFailureTest("val-try-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-try-via-escape-hex4.js")]
    public Task val_try_via_escape_hex4()
        => CompilationFailureTest("val-try-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-try.js")]
    public Task val_try()
        => CompilationFailureTest("val-try", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-typeof-via-escape-hex.js")]
    public Task val_typeof_via_escape_hex()
        => CompilationFailureTest("val-typeof-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-typeof-via-escape-hex4.js")]
    public Task val_typeof_via_escape_hex4()
        => CompilationFailureTest("val-typeof-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-typeof.js")]
    public Task val_typeof()
        => CompilationFailureTest("val-typeof", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-var-via-escape-hex.js")]
    public Task val_var_via_escape_hex()
        => CompilationFailureTest("val-var-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-var-via-escape-hex4.js")]
    public Task val_var_via_escape_hex4()
        => CompilationFailureTest("val-var-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-var.js")]
    public Task val_var()
        => CompilationFailureTest("val-var", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-void-via-escape-hex.js")]
    public Task val_void_via_escape_hex()
        => CompilationFailureTest("val-void-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-void-via-escape-hex4.js")]
    public Task val_void_via_escape_hex4()
        => CompilationFailureTest("val-void-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-void.js")]
    public Task val_void()
        => CompilationFailureTest("val-void", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-while-via-escape-hex.js")]
    public Task val_while_via_escape_hex()
        => CompilationFailureTest("val-while-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-while-via-escape-hex4.js")]
    public Task val_while_via_escape_hex4()
        => CompilationFailureTest("val-while-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-while.js")]
    public Task val_while()
        => CompilationFailureTest("val-while", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-with-via-escape-hex.js")]
    public Task val_with_via_escape_hex()
        => CompilationFailureTest("val-with-via-escape-hex", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-with-via-escape-hex4.js")]
    public Task val_with_via_escape_hex4()
        => CompilationFailureTest("val-with-via-escape-hex4", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-with.js")]
    public Task val_with()
        => CompilationFailureTest("val-with", "Failed to parse JavaScript");

    [Fact(DisplayName = "val-yield-strict.js")]
    public Task val_yield_strict()
        => CompilationFailureTest("val-yield-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "vertical-tilde-continue-escaped.js")]
    public Task vertical_tilde_continue_escaped()
        => CompilationFailureTest("vertical-tilde-continue-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "vertical-tilde-continue.js")]
    public Task vertical_tilde_continue()
        => CompilationFailureTest("vertical-tilde-continue", "Failed to parse JavaScript");

    [Fact(DisplayName = "vertical-tilde-start-escaped.js")]
    public Task vertical_tilde_start_escaped()
        => CompilationFailureTest("vertical-tilde-start-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "vertical-tilde-start.js")]
    public Task vertical_tilde_start()
        => CompilationFailureTest("vertical-tilde-start", "Failed to parse JavaScript");
}
