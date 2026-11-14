using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.module_code;

public sealed class NativePortBatch_Port700 : DiskExecutionTestsBase
{
    public NativePortBatch_Port700() : base("Jroc.Test262.Tests.language.module_code") { }

    [Fact(DisplayName = "comment-multi-line-html-close")]
    public Task Port700_comment_multi_line_html_close() => CompilationFailureTest("comment-multi-line-html-close");

    [Fact(DisplayName = "comment-single-line-html-close")]
    public Task Port700_comment_single_line_html_close() => CompilationFailureTest("comment-single-line-html-close");

    [Fact(DisplayName = "comment-single-line-html-open")]
    public Task Port700_comment_single_line_html_open() => CompilationFailureTest("comment-single-line-html-open");

    [Fact(DisplayName = "early-dup-lables")]
    public Task Port700_early_dup_lables() => CompilationFailureTest("early-dup-lables");

    [Fact(DisplayName = "early-dup-top-function-async-generator")]
    public Task Port700_early_dup_top_function_async_generator() => CompilationFailureTest("early-dup-top-function-async-generator");

    [Fact(DisplayName = "early-dup-top-function-async")]
    public Task Port700_early_dup_top_function_async() => CompilationFailureTest("early-dup-top-function-async");

    [Fact(DisplayName = "early-dup-top-function-generator")]
    public Task Port700_early_dup_top_function_generator() => CompilationFailureTest("early-dup-top-function-generator");

    [Fact(DisplayName = "early-dup-top-function")]
    public Task Port700_early_dup_top_function() => CompilationFailureTest("early-dup-top-function");

    [Fact(DisplayName = "early-export-ill-formed-string")]
    public Task Port700_early_export_ill_formed_string() => CompilationFailureTest("early-export-ill-formed-string");

    [Fact(DisplayName = "early-import-arguments")]
    public Task Port700_early_import_arguments() => CompilationFailureTest("early-import-arguments");

    [Fact(DisplayName = "early-import-as-arguments")]
    public Task Port700_early_import_as_arguments() => CompilationFailureTest("early-import-as-arguments");

    [Fact(DisplayName = "early-import-as-eval")]
    public Task Port700_early_import_as_eval() => CompilationFailureTest("early-import-as-eval");

    [Fact(DisplayName = "early-import-eval")]
    public Task Port700_early_import_eval() => CompilationFailureTest("early-import-eval");

    [Fact(DisplayName = "early-strict-mode")]
    public Task Port700_early_strict_mode() => CompilationFailureTest("early-strict-mode");

    [Fact(DisplayName = "export-default-asyncfunction-declaration-binding-exists")]
    public Task Port700_export_default_asyncfunction_declaration_binding_exists() => CompilationFailureTest("export-default-asyncfunction-declaration-binding-exists");

    [Fact(DisplayName = "export-default-asyncgenerator-declaration-binding-exists")]
    public Task Port700_export_default_asyncgenerator_declaration_binding_exists() => CompilationFailureTest("export-default-asyncgenerator-declaration-binding-exists");

    [Fact(DisplayName = "export-default-function-declaration-binding-exists")]
    public Task Port700_export_default_function_declaration_binding_exists() => CompilationFailureTest("export-default-function-declaration-binding-exists");

    [Fact(DisplayName = "export-default-generator-declaration-binding-exists")]
    public Task Port700_export_default_generator_declaration_binding_exists() => CompilationFailureTest("export-default-generator-declaration-binding-exists");

    [Fact(DisplayName = "export-expname-from-as-unpaired-surrogate")]
    public Task Port700_export_expname_from_as_unpaired_surrogate() => CompilationFailureTest("export-expname-from-as-unpaired-surrogate");

    [Fact(DisplayName = "export-expname-from-star-unpaired-surrogate")]
    public Task Port700_export_expname_from_star_unpaired_surrogate() => CompilationFailureTest("export-expname-from-star-unpaired-surrogate");

    [Fact(DisplayName = "export-expname-from-unpaired-surrogate")]
    public Task Port700_export_expname_from_unpaired_surrogate() => CompilationFailureTest("export-expname-from-unpaired-surrogate");

    [Fact(DisplayName = "export-expname-import-unpaired-surrogate")]
    public Task Port700_export_expname_import_unpaired_surrogate() => CompilationFailureTest("export-expname-import-unpaired-surrogate");

    [Fact(DisplayName = "export-expname-string-binding")]
    public Task Port700_export_expname_string_binding() => CompilationFailureTest("export-expname-string-binding");

    [Fact(DisplayName = "export-expname-unpaired-surrogate")]
    public Task Port700_export_expname_unpaired_surrogate() => CompilationFailureTest("export-expname-unpaired-surrogate");

    [Fact(DisplayName = "invalid-private-names-call-expression-bad-reference")]
    public Task Port700_invalid_private_names_call_expression_bad_reference() => CompilationFailureTest("invalid-private-names-call-expression-bad-reference");

    [Fact(DisplayName = "invalid-private-names-call-expression-this")]
    public Task Port700_invalid_private_names_call_expression_this() => CompilationFailureTest("invalid-private-names-call-expression-this");

    [Fact(DisplayName = "invalid-private-names-member-expression-bad-reference")]
    public Task Port700_invalid_private_names_member_expression_bad_reference() => CompilationFailureTest("invalid-private-names-member-expression-bad-reference");

    [Fact(DisplayName = "invalid-private-names-member-expression-this")]
    public Task Port700_invalid_private_names_member_expression_this() => CompilationFailureTest("invalid-private-names-member-expression-this");

    [Fact(DisplayName = "parse-err-decl-pos-export-arrow-function")]
    public Task Port700_parse_err_decl_pos_export_arrow_function() => CompilationFailureTest("parse-err-decl-pos-export-arrow-function");

    [Fact(DisplayName = "parse-err-decl-pos-export-block-stmt-list")]
    public Task Port700_parse_err_decl_pos_export_block_stmt_list() => CompilationFailureTest("parse-err-decl-pos-export-block-stmt-list");

    [Fact(DisplayName = "parse-err-decl-pos-export-block-stmt")]
    public Task Port700_parse_err_decl_pos_export_block_stmt() => CompilationFailureTest("parse-err-decl-pos-export-block-stmt");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-decl-meth-static")]
    public Task Port700_parse_err_decl_pos_export_class_decl_meth_static() => CompilationFailureTest("parse-err-decl-pos-export-class-decl-meth-static");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-decl-meth")]
    public Task Port700_parse_err_decl_pos_export_class_decl_meth() => CompilationFailureTest("parse-err-decl-pos-export-class-decl-meth");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-decl-method-gen-static")]
    public Task Port700_parse_err_decl_pos_export_class_decl_method_gen_static() => CompilationFailureTest("parse-err-decl-pos-export-class-decl-method-gen-static");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-decl-method-gen")]
    public Task Port700_parse_err_decl_pos_export_class_decl_method_gen() => CompilationFailureTest("parse-err-decl-pos-export-class-decl-method-gen");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-expr-meth-gen-static")]
    public Task Port700_parse_err_decl_pos_export_class_expr_meth_gen_static() => CompilationFailureTest("parse-err-decl-pos-export-class-expr-meth-gen-static");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-expr-meth-gen")]
    public Task Port700_parse_err_decl_pos_export_class_expr_meth_gen() => CompilationFailureTest("parse-err-decl-pos-export-class-expr-meth-gen");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-expr-meth-static")]
    public Task Port700_parse_err_decl_pos_export_class_expr_meth_static() => CompilationFailureTest("parse-err-decl-pos-export-class-expr-meth-static");

    [Fact(DisplayName = "parse-err-decl-pos-export-class-expr-meth")]
    public Task Port700_parse_err_decl_pos_export_class_expr_meth() => CompilationFailureTest("parse-err-decl-pos-export-class-expr-meth");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-const")]
    public Task Port700_parse_err_decl_pos_export_for_const() => CompilationFailureTest("parse-err-decl-pos-export-for-const");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-in-const")]
    public Task Port700_parse_err_decl_pos_export_for_in_const() => CompilationFailureTest("parse-err-decl-pos-export-for-in-const");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-in-let")]
    public Task Port700_parse_err_decl_pos_export_for_in_let() => CompilationFailureTest("parse-err-decl-pos-export-for-in-let");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-in-lhs")]
    public Task Port700_parse_err_decl_pos_export_for_in_lhs() => CompilationFailureTest("parse-err-decl-pos-export-for-in-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-in-var")]
    public Task Port700_parse_err_decl_pos_export_for_in_var() => CompilationFailureTest("parse-err-decl-pos-export-for-in-var");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-let")]
    public Task Port700_parse_err_decl_pos_export_for_let() => CompilationFailureTest("parse-err-decl-pos-export-for-let");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-lhs")]
    public Task Port700_parse_err_decl_pos_export_for_lhs() => CompilationFailureTest("parse-err-decl-pos-export-for-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-of-const")]
    public Task Port700_parse_err_decl_pos_export_for_of_const() => CompilationFailureTest("parse-err-decl-pos-export-for-of-const");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-of-let")]
    public Task Port700_parse_err_decl_pos_export_for_of_let() => CompilationFailureTest("parse-err-decl-pos-export-for-of-let");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-of-lhs")]
    public Task Port700_parse_err_decl_pos_export_for_of_lhs() => CompilationFailureTest("parse-err-decl-pos-export-for-of-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-of-var")]
    public Task Port700_parse_err_decl_pos_export_for_of_var() => CompilationFailureTest("parse-err-decl-pos-export-for-of-var");

    [Fact(DisplayName = "parse-err-decl-pos-export-for-var")]
    public Task Port700_parse_err_decl_pos_export_for_var() => CompilationFailureTest("parse-err-decl-pos-export-for-var");

    [Fact(DisplayName = "parse-err-decl-pos-export-function-decl")]
    public Task Port700_parse_err_decl_pos_export_function_decl() => CompilationFailureTest("parse-err-decl-pos-export-function-decl");

    [Fact(DisplayName = "parse-err-decl-pos-export-function-expr")]
    public Task Port700_parse_err_decl_pos_export_function_expr() => CompilationFailureTest("parse-err-decl-pos-export-function-expr");

    [Fact(DisplayName = "parse-err-decl-pos-export-generator-decl")]
    public Task Port700_parse_err_decl_pos_export_generator_decl() => CompilationFailureTest("parse-err-decl-pos-export-generator-decl");

    [Fact(DisplayName = "parse-err-decl-pos-export-generator-expr")]
    public Task Port700_parse_err_decl_pos_export_generator_expr() => CompilationFailureTest("parse-err-decl-pos-export-generator-expr");

    [Fact(DisplayName = "parse-err-decl-pos-export-if-else")]
    public Task Port700_parse_err_decl_pos_export_if_else() => CompilationFailureTest("parse-err-decl-pos-export-if-else");

    [Fact(DisplayName = "parse-err-decl-pos-export-labeled")]
    public Task Port700_parse_err_decl_pos_export_labeled() => CompilationFailureTest("parse-err-decl-pos-export-labeled");

    [Fact(DisplayName = "parse-err-decl-pos-export-object-gen-method")]
    public Task Port700_parse_err_decl_pos_export_object_gen_method() => CompilationFailureTest("parse-err-decl-pos-export-object-gen-method");

    [Fact(DisplayName = "parse-err-decl-pos-export-object-getter")]
    public Task Port700_parse_err_decl_pos_export_object_getter() => CompilationFailureTest("parse-err-decl-pos-export-object-getter");

    [Fact(DisplayName = "parse-err-decl-pos-export-object-method")]
    public Task Port700_parse_err_decl_pos_export_object_method() => CompilationFailureTest("parse-err-decl-pos-export-object-method");

    [Fact(DisplayName = "parse-err-decl-pos-export-object-setter")]
    public Task Port700_parse_err_decl_pos_export_object_setter() => CompilationFailureTest("parse-err-decl-pos-export-object-setter");

    [Fact(DisplayName = "parse-err-decl-pos-export-switch-case-dflt")]
    public Task Port700_parse_err_decl_pos_export_switch_case_dflt() => CompilationFailureTest("parse-err-decl-pos-export-switch-case-dflt");

    [Fact(DisplayName = "parse-err-decl-pos-export-switch-case")]
    public Task Port700_parse_err_decl_pos_export_switch_case() => CompilationFailureTest("parse-err-decl-pos-export-switch-case");

    [Fact(DisplayName = "parse-err-decl-pos-export-switch-dftl")]
    public Task Port700_parse_err_decl_pos_export_switch_dftl() => CompilationFailureTest("parse-err-decl-pos-export-switch-dftl");

    [Fact(DisplayName = "parse-err-decl-pos-export-try-catch-finally")]
    public Task Port700_parse_err_decl_pos_export_try_catch_finally() => CompilationFailureTest("parse-err-decl-pos-export-try-catch-finally");

    [Fact(DisplayName = "parse-err-decl-pos-export-try-catch")]
    public Task Port700_parse_err_decl_pos_export_try_catch() => CompilationFailureTest("parse-err-decl-pos-export-try-catch");

    [Fact(DisplayName = "parse-err-decl-pos-export-try-finally")]
    public Task Port700_parse_err_decl_pos_export_try_finally() => CompilationFailureTest("parse-err-decl-pos-export-try-finally");

    [Fact(DisplayName = "parse-err-decl-pos-export-try-try")]
    public Task Port700_parse_err_decl_pos_export_try_try() => CompilationFailureTest("parse-err-decl-pos-export-try-try");

    [Fact(DisplayName = "parse-err-decl-pos-import-arrow-function")]
    public Task Port700_parse_err_decl_pos_import_arrow_function() => CompilationFailureTest("parse-err-decl-pos-import-arrow-function");

    [Fact(DisplayName = "parse-err-decl-pos-import-block-stmt-list")]
    public Task Port700_parse_err_decl_pos_import_block_stmt_list() => CompilationFailureTest("parse-err-decl-pos-import-block-stmt-list");

    [Fact(DisplayName = "parse-err-decl-pos-import-block-stmt")]
    public Task Port700_parse_err_decl_pos_import_block_stmt() => CompilationFailureTest("parse-err-decl-pos-import-block-stmt");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-decl-meth-static")]
    public Task Port700_parse_err_decl_pos_import_class_decl_meth_static() => CompilationFailureTest("parse-err-decl-pos-import-class-decl-meth-static");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-decl-meth")]
    public Task Port700_parse_err_decl_pos_import_class_decl_meth() => CompilationFailureTest("parse-err-decl-pos-import-class-decl-meth");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-decl-method-gen-static")]
    public Task Port700_parse_err_decl_pos_import_class_decl_method_gen_static() => CompilationFailureTest("parse-err-decl-pos-import-class-decl-method-gen-static");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-decl-method-gen")]
    public Task Port700_parse_err_decl_pos_import_class_decl_method_gen() => CompilationFailureTest("parse-err-decl-pos-import-class-decl-method-gen");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-expr-meth-gen-static")]
    public Task Port700_parse_err_decl_pos_import_class_expr_meth_gen_static() => CompilationFailureTest("parse-err-decl-pos-import-class-expr-meth-gen-static");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-expr-meth-gen")]
    public Task Port700_parse_err_decl_pos_import_class_expr_meth_gen() => CompilationFailureTest("parse-err-decl-pos-import-class-expr-meth-gen");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-expr-meth-static")]
    public Task Port700_parse_err_decl_pos_import_class_expr_meth_static() => CompilationFailureTest("parse-err-decl-pos-import-class-expr-meth-static");

    [Fact(DisplayName = "parse-err-decl-pos-import-class-expr-meth")]
    public Task Port700_parse_err_decl_pos_import_class_expr_meth() => CompilationFailureTest("parse-err-decl-pos-import-class-expr-meth");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-const")]
    public Task Port700_parse_err_decl_pos_import_for_const() => CompilationFailureTest("parse-err-decl-pos-import-for-const");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-in-const")]
    public Task Port700_parse_err_decl_pos_import_for_in_const() => CompilationFailureTest("parse-err-decl-pos-import-for-in-const");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-in-let")]
    public Task Port700_parse_err_decl_pos_import_for_in_let() => CompilationFailureTest("parse-err-decl-pos-import-for-in-let");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-in-lhs")]
    public Task Port700_parse_err_decl_pos_import_for_in_lhs() => CompilationFailureTest("parse-err-decl-pos-import-for-in-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-in-var")]
    public Task Port700_parse_err_decl_pos_import_for_in_var() => CompilationFailureTest("parse-err-decl-pos-import-for-in-var");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-let")]
    public Task Port700_parse_err_decl_pos_import_for_let() => CompilationFailureTest("parse-err-decl-pos-import-for-let");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-lhs")]
    public Task Port700_parse_err_decl_pos_import_for_lhs() => CompilationFailureTest("parse-err-decl-pos-import-for-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-of-const")]
    public Task Port700_parse_err_decl_pos_import_for_of_const() => CompilationFailureTest("parse-err-decl-pos-import-for-of-const");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-of-let")]
    public Task Port700_parse_err_decl_pos_import_for_of_let() => CompilationFailureTest("parse-err-decl-pos-import-for-of-let");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-of-lhs")]
    public Task Port700_parse_err_decl_pos_import_for_of_lhs() => CompilationFailureTest("parse-err-decl-pos-import-for-of-lhs");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-of-var")]
    public Task Port700_parse_err_decl_pos_import_for_of_var() => CompilationFailureTest("parse-err-decl-pos-import-for-of-var");

    [Fact(DisplayName = "parse-err-decl-pos-import-for-var")]
    public Task Port700_parse_err_decl_pos_import_for_var() => CompilationFailureTest("parse-err-decl-pos-import-for-var");

    [Fact(DisplayName = "parse-err-decl-pos-import-function-decl")]
    public Task Port700_parse_err_decl_pos_import_function_decl() => CompilationFailureTest("parse-err-decl-pos-import-function-decl");

    [Fact(DisplayName = "parse-err-decl-pos-import-function-expr")]
    public Task Port700_parse_err_decl_pos_import_function_expr() => CompilationFailureTest("parse-err-decl-pos-import-function-expr");

    [Fact(DisplayName = "parse-err-decl-pos-import-generator-decl")]
    public Task Port700_parse_err_decl_pos_import_generator_decl() => CompilationFailureTest("parse-err-decl-pos-import-generator-decl");

    [Fact(DisplayName = "parse-err-decl-pos-import-generator-expr")]
    public Task Port700_parse_err_decl_pos_import_generator_expr() => CompilationFailureTest("parse-err-decl-pos-import-generator-expr");

    [Fact(DisplayName = "parse-err-decl-pos-import-if-else")]
    public Task Port700_parse_err_decl_pos_import_if_else() => CompilationFailureTest("parse-err-decl-pos-import-if-else");

    [Fact(DisplayName = "parse-err-decl-pos-import-labeled")]
    public Task Port700_parse_err_decl_pos_import_labeled() => CompilationFailureTest("parse-err-decl-pos-import-labeled");

    [Fact(DisplayName = "parse-err-decl-pos-import-object-gen-method")]
    public Task Port700_parse_err_decl_pos_import_object_gen_method() => CompilationFailureTest("parse-err-decl-pos-import-object-gen-method");

    [Fact(DisplayName = "parse-err-decl-pos-import-object-getter")]
    public Task Port700_parse_err_decl_pos_import_object_getter() => CompilationFailureTest("parse-err-decl-pos-import-object-getter");

    [Fact(DisplayName = "parse-err-decl-pos-import-object-method")]
    public Task Port700_parse_err_decl_pos_import_object_method() => CompilationFailureTest("parse-err-decl-pos-import-object-method");

    [Fact(DisplayName = "parse-err-decl-pos-import-object-setter")]
    public Task Port700_parse_err_decl_pos_import_object_setter() => CompilationFailureTest("parse-err-decl-pos-import-object-setter");

    [Fact(DisplayName = "parse-err-decl-pos-import-switch-case-dflt")]
    public Task Port700_parse_err_decl_pos_import_switch_case_dflt() => CompilationFailureTest("parse-err-decl-pos-import-switch-case-dflt");

    [Fact(DisplayName = "parse-err-decl-pos-import-switch-case")]
    public Task Port700_parse_err_decl_pos_import_switch_case() => CompilationFailureTest("parse-err-decl-pos-import-switch-case");

    [Fact(DisplayName = "parse-err-decl-pos-import-switch-dftl")]
    public Task Port700_parse_err_decl_pos_import_switch_dftl() => CompilationFailureTest("parse-err-decl-pos-import-switch-dftl");

    [Fact(DisplayName = "parse-err-decl-pos-import-try-catch-finally")]
    public Task Port700_parse_err_decl_pos_import_try_catch_finally() => CompilationFailureTest("parse-err-decl-pos-import-try-catch-finally");

    [Fact(DisplayName = "parse-err-decl-pos-import-try-catch")]
    public Task Port700_parse_err_decl_pos_import_try_catch() => CompilationFailureTest("parse-err-decl-pos-import-try-catch");

    [Fact(DisplayName = "parse-err-decl-pos-import-try-finally")]
    public Task Port700_parse_err_decl_pos_import_try_finally() => CompilationFailureTest("parse-err-decl-pos-import-try-finally");

    [Fact(DisplayName = "parse-err-decl-pos-import-try-try")]
    public Task Port700_parse_err_decl_pos_import_try_try() => CompilationFailureTest("parse-err-decl-pos-import-try-try");

    [Fact(DisplayName = "parse-err-export-dflt-const")]
    public Task Port700_parse_err_export_dflt_const() => CompilationFailureTest("parse-err-export-dflt-const");

    [Fact(DisplayName = "parse-err-export-dflt-expr")]
    public Task Port700_parse_err_export_dflt_expr() => CompilationFailureTest("parse-err-export-dflt-expr");

    [Fact(DisplayName = "parse-err-export-dflt-let")]
    public Task Port700_parse_err_export_dflt_let() => CompilationFailureTest("parse-err-export-dflt-let");

    [Fact(DisplayName = "parse-err-export-dflt-var")]
    public Task Port700_parse_err_export_dflt_var() => CompilationFailureTest("parse-err-export-dflt-var");

    [Fact(DisplayName = "parse-err-hoist-lex-fun")]
    public Task Port700_parse_err_hoist_lex_fun() => CompilationFailureTest("parse-err-hoist-lex-fun");

    [Fact(DisplayName = "parse-err-hoist-lex-gen")]
    public Task Port700_parse_err_hoist_lex_gen() => CompilationFailureTest("parse-err-hoist-lex-gen");

    [Fact(DisplayName = "parse-err-invoke-anon-fun-decl")]
    public Task Port700_parse_err_invoke_anon_fun_decl() => CompilationFailureTest("parse-err-invoke-anon-fun-decl");

    [Fact(DisplayName = "parse-err-invoke-anon-gen-decl")]
    public Task Port700_parse_err_invoke_anon_gen_decl() => CompilationFailureTest("parse-err-invoke-anon-gen-decl");

    [Fact(DisplayName = "parse-err-return")]
    public Task Port700_parse_err_return() => CompilationFailureTest("parse-err-return");

    [Fact(DisplayName = "parse-err-semi-dflt-expr")]
    public Task Port700_parse_err_semi_dflt_expr() => CompilationFailureTest("parse-err-semi-dflt-expr");

    [Fact(DisplayName = "parse-err-yield")]
    public Task Port700_parse_err_yield() => CompilationFailureTest("parse-err-yield");

    [Fact(DisplayName = "private-identifiers-not-empty")]
    public Task Port700_private_identifiers_not_empty() => CompilationFailureTest("private-identifiers-not-empty");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-1")]
    public Task Port700_privatename_not_valid_earlyerr_module_1() => CompilationFailureTest("privatename-not-valid-earlyerr-module-1");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-2")]
    public Task Port700_privatename_not_valid_earlyerr_module_2() => CompilationFailureTest("privatename-not-valid-earlyerr-module-2");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-3")]
    public Task Port700_privatename_not_valid_earlyerr_module_3() => CompilationFailureTest("privatename-not-valid-earlyerr-module-3");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-4")]
    public Task Port700_privatename_not_valid_earlyerr_module_4() => CompilationFailureTest("privatename-not-valid-earlyerr-module-4");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-5")]
    public Task Port700_privatename_not_valid_earlyerr_module_5() => CompilationFailureTest("privatename-not-valid-earlyerr-module-5");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-6")]
    public Task Port700_privatename_not_valid_earlyerr_module_6() => CompilationFailureTest("privatename-not-valid-earlyerr-module-6");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-7")]
    public Task Port700_privatename_not_valid_earlyerr_module_7() => CompilationFailureTest("privatename-not-valid-earlyerr-module-7");

    [Fact(DisplayName = "privatename-not-valid-earlyerr-module-8")]
    public Task Port700_privatename_not_valid_earlyerr_module_8() => CompilationFailureTest("privatename-not-valid-earlyerr-module-8");

}
