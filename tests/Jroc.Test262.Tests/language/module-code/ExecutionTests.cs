using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\module-code", "language.module_code") { }

    [Fact(DisplayName = "early-dup-export-as-star-as")]
    public Task early_dup_export_as_star_as()
        => CompilationFailureTest("early-dup-export-as-star-as");

    [Fact(DisplayName = "early-dup-export-decl")]
    public Task early_dup_export_decl()
        => CompilationFailureTest("early-dup-export-decl");

    [Fact(DisplayName = "early-dup-export-dflt-id")]
    public Task early_dup_export_dflt_id()
        => CompilationFailureTest("early-dup-export-dflt-id");

    [Fact(DisplayName = "early-dup-export-dflt")]
    public Task early_dup_export_dflt()
        => CompilationFailureTest("early-dup-export-dflt");

    [Fact(DisplayName = "early-dup-export-id-as")]
    public Task early_dup_export_id_as()
        => CompilationFailureTest("early-dup-export-id-as");

    [Fact(DisplayName = "early-dup-export-id")]
    public Task early_dup_export_id()
        => CompilationFailureTest("early-dup-export-id");

    [Fact(DisplayName = "early-dup-export-star-as-dflt")]
    public Task early_dup_export_star_as_dflt()
        => CompilationFailureTest("early-dup-export-star-as-dflt");

    [Fact(DisplayName = "early-dup-lex")]
    public Task early_dup_lex()
        => CompilationFailureTest("early-dup-lex");

    [Fact(DisplayName = "early-export-global")]
    public Task early_export_global()
        => CompilationFailureTest("early-export-global");

    [Fact(DisplayName = "early-export-unresolvable")]
    public Task early_export_unresolvable()
        => CompilationFailureTest("early-export-unresolvable");

    [Fact(DisplayName = "early-lex-and-var")]
    public Task early_lex_and_var()
        => CompilationFailureTest("early-lex-and-var");

    [Fact(DisplayName = "early-new-target")]
    public Task early_new_target()
        => CompilationFailureTest("early-new-target");

    [Fact(DisplayName = "early-super")]
    public Task early_super()
        => CompilationFailureTest("early-super");

    [Fact(DisplayName = "early-undef-break")]
    public Task early_undef_break()
        => CompilationFailureTest("early-undef-break");

    [Fact(DisplayName = "early-undef-continue")]
    public Task early_undef_continue()
        => CompilationFailureTest("early-undef-continue");

    [Fact(DisplayName = "parse-err-decl-pos-export-do-while")]
    public Task parse_err_decl_pos_export_do_while()
        => CompilationFailureTest("parse-err-decl-pos-export-do-while");

    [Fact(DisplayName = "parse-err-decl-pos-export-if-if")]
    public Task parse_err_decl_pos_export_if_if()
        => CompilationFailureTest("parse-err-decl-pos-export-if-if");

    [Fact(DisplayName = "parse-err-decl-pos-export-while")]
    public Task parse_err_decl_pos_export_while()
        => CompilationFailureTest("parse-err-decl-pos-export-while");

    [Fact(DisplayName = "parse-err-decl-pos-import-do-while")]
    public Task parse_err_decl_pos_import_do_while()
        => CompilationFailureTest("parse-err-decl-pos-import-do-while");

    [Fact(DisplayName = "parse-err-decl-pos-import-if-if")]
    public Task parse_err_decl_pos_import_if_if()
        => CompilationFailureTest("parse-err-decl-pos-import-if-if");

    [Fact(DisplayName = "parse-err-decl-pos-import-while")]
    public Task parse_err_decl_pos_import_while()
        => CompilationFailureTest("parse-err-decl-pos-import-while");

    [Fact(DisplayName = "parse-err-semi-export-star")]
    public Task parse_err_semi_export_star()
        => CompilationFailureTest("parse-err-semi-export-star");

    [Fact(DisplayName = "parse-err-semi-name-space-export")]
    public Task parse_err_semi_name_space_export()
        => CompilationFailureTest("parse-err-semi-name-space-export");

    [Fact(DisplayName = "parse-err-semi-named-export")]
    public Task parse_err_semi_named_export()
        => CompilationFailureTest("parse-err-semi-named-export");

    [Fact(DisplayName = "parse-err-semi-named-export-from")]
    public Task parse_err_semi_named_export_from()
        => CompilationFailureTest("parse-err-semi-named-export-from");

    [Fact(DisplayName = "parse-err-syntax-1")]
    public Task parse_err_syntax_1()
        => CompilationFailureTest("parse-err-syntax-1");

    [Fact(DisplayName = "parse-err-syntax-2")]
    public Task parse_err_syntax_2()
        => CompilationFailureTest("parse-err-syntax-2");

    [Fact(DisplayName = "eval-export-cls-semi")]
    public Task eval_export_cls_semi() => ExecutionTest("eval-export-cls-semi");
    [Fact(DisplayName = "eval-export-dflt-cls-anon-semi")]
    public Task eval_export_dflt_cls_anon_semi() => ExecutionTest("eval-export-dflt-cls-anon-semi");
    [Fact(DisplayName = "eval-export-dflt-cls-anon")]
    public Task eval_export_dflt_cls_anon() => ExecutionTest("eval-export-dflt-cls-anon");
    [Fact(DisplayName = "eval-export-dflt-cls-name-meth")]
    public Task eval_export_dflt_cls_name_meth() => ExecutionTest("eval-export-dflt-cls-name-meth");
    [Fact(DisplayName = "eval-export-dflt-cls-named-semi")]
    public Task eval_export_dflt_cls_named_semi() => ExecutionTest("eval-export-dflt-cls-named-semi");
    [Fact(DisplayName = "eval-export-dflt-cls-named")]
    public Task eval_export_dflt_cls_named() => ExecutionTest("eval-export-dflt-cls-named");
    [Fact(DisplayName = "eval-export-dflt-expr-cls-anon")]
    public Task eval_export_dflt_expr_cls_anon() => ExecutionTest("eval-export-dflt-expr-cls-anon");
    [Fact(DisplayName = "eval-export-dflt-expr-cls-name-meth")]
    public Task eval_export_dflt_expr_cls_name_meth() => ExecutionTest("eval-export-dflt-expr-cls-name-meth");
    [Fact(DisplayName = "eval-export-dflt-expr-cls-named")]
    public Task eval_export_dflt_expr_cls_named() => ExecutionTest("eval-export-dflt-expr-cls-named");
    [Fact(DisplayName = "eval-export-dflt-expr-err-eval")]
    public Task eval_export_dflt_expr_err_eval() => ExecutionTest("eval-export-dflt-expr-err-eval");
    [Fact(DisplayName = "eval-export-dflt-expr-err-get-value")]
    public Task eval_export_dflt_expr_err_get_value() => ExecutionTest("eval-export-dflt-expr-err-get-value");
    [Fact(DisplayName = "eval-export-dflt-expr-fn-anon")]
    public Task eval_export_dflt_expr_fn_anon() => ExecutionTest("eval-export-dflt-expr-fn-anon");
    [Fact(DisplayName = "eval-export-dflt-expr-fn-named")]
    public Task eval_export_dflt_expr_fn_named() => ExecutionTest("eval-export-dflt-expr-fn-named");
    [Fact(DisplayName = "eval-export-dflt-expr-gen-anon")]
    public Task eval_export_dflt_expr_gen_anon() => ExecutionTest("eval-export-dflt-expr-gen-anon");
    [Fact(DisplayName = "eval-export-dflt-expr-gen-named")]
    public Task eval_export_dflt_expr_gen_named() => ExecutionTest("eval-export-dflt-expr-gen-named");
    [Fact(DisplayName = "eval-export-dflt-expr-in")]
    public Task eval_export_dflt_expr_in() => ExecutionTest("eval-export-dflt-expr-in");
    [Fact(DisplayName = "eval-export-dflt-fun-anon-semi")]
    public Task eval_export_dflt_fun_anon_semi() => ExecutionTest("eval-export-dflt-fun-anon-semi");
    [Fact(DisplayName = "eval-export-dflt-fun-named-semi")]
    public Task eval_export_dflt_fun_named_semi() => ExecutionTest("eval-export-dflt-fun-named-semi");
    [Fact(DisplayName = "eval-export-dflt-gen-anon-semi")]
    public Task eval_export_dflt_gen_anon_semi() => ExecutionTest("eval-export-dflt-gen-anon-semi");
    [Fact(DisplayName = "eval-export-dflt-gen-named-semi")]
    public Task eval_export_dflt_gen_named_semi() => ExecutionTest("eval-export-dflt-gen-named-semi");
    [Fact(DisplayName = "eval-export-fun-semi")]
    public Task eval_export_fun_semi() => ExecutionTest("eval-export-fun-semi");
    [Fact(DisplayName = "eval-export-gen-semi")]
    public Task eval_export_gen_semi() => ExecutionTest("eval-export-gen-semi");
    [Fact(DisplayName = "eval-gtbndng-indirect-trlng-comma")]
    public Task eval_gtbndng_indirect_trlng_comma() => ExecutionTest("eval-gtbndng-indirect-trlng-comma");
    [Fact(DisplayName = "eval-gtbndng-indirect-update-as")]
    public Task eval_gtbndng_indirect_update_as() => ExecutionTest("eval-gtbndng-indirect-update-as");
    [Fact(DisplayName = "eval-gtbndng-indirect-update-dflt")]
    public Task eval_gtbndng_indirect_update_dflt() => ExecutionTest("eval-gtbndng-indirect-update-dflt");
    [Fact(DisplayName = "eval-gtbndng-indirect-update")]
    public Task eval_gtbndng_indirect_update() => ExecutionTest("eval-gtbndng-indirect-update");
    [Fact(DisplayName = "eval-gtbndng-local-bndng-cls")]
    public Task eval_gtbndng_local_bndng_cls() => ExecutionTest("eval-gtbndng-local-bndng-cls");
    [Fact(DisplayName = "eval-gtbndng-local-bndng-const")]
    public Task eval_gtbndng_local_bndng_const() => ExecutionTest("eval-gtbndng-local-bndng-const");
    [Fact(DisplayName = "eval-gtbndng-local-bndng-let")]
    public Task eval_gtbndng_local_bndng_let() => ExecutionTest("eval-gtbndng-local-bndng-let");
    [Fact(DisplayName = "eval-gtbndng-local-bndng-var")]
    public Task eval_gtbndng_local_bndng_var() => ExecutionTest("eval-gtbndng-local-bndng-var");
    [Fact(DisplayName = "eval-rqstd-abrupt")]
    public Task eval_rqstd_abrupt() => ExecutionTest("eval-rqstd-abrupt");
    [Fact(DisplayName = "eval-rqstd-once")]
    public Task eval_rqstd_once() => ExecutionTest("eval-rqstd-once");
    [Fact(DisplayName = "eval-rqstd-order")]
    public Task eval_rqstd_order() => ExecutionTest("eval-rqstd-order");
    [Fact(DisplayName = "eval-self-abrupt")]
    public Task eval_self_abrupt() => ExecutionTest("eval-self-abrupt");
    [Fact(DisplayName = "eval-self-once")]
    public Task eval_self_once() => ExecutionTest("eval-self-once");
    [Fact(DisplayName = "eval-this")]
    public Task eval_this() => ExecutionTest("eval-this");
    [Fact(DisplayName = "export-default-asyncfunction-declaration-binding")]
    public Task export_default_asyncfunction_declaration_binding() => ExecutionTest("export-default-asyncfunction-declaration-binding");
    [Fact(DisplayName = "export-default-asyncgenerator-declaration-binding")]
    public Task export_default_asyncgenerator_declaration_binding() => ExecutionTest("export-default-asyncgenerator-declaration-binding");
    [Fact(DisplayName = "export-default-function-declaration-binding")]
    public Task export_default_function_declaration_binding() => ExecutionTest("export-default-function-declaration-binding");
    [Fact(DisplayName = "export-default-generator-declaration-binding")]
    public Task export_default_generator_declaration_binding() => ExecutionTest("export-default-generator-declaration-binding");
    [Fact(DisplayName = "export-expname-binding-index")]
    public Task export_expname_binding_index() => ExecutionTest("export-expname-binding-index");
    [Fact(DisplayName = "export-expname-binding-string")]
    public Task export_expname_binding_string() => ExecutionTest("export-expname-binding-string");
    [Fact(DisplayName = "export-expname-from-binding-string")]
    public Task export_expname_from_binding_string() => ExecutionTest("export-expname-from-binding-string");
    [Fact(DisplayName = "export-expname-from-star-string")]
    public Task export_expname_from_star_string() => ExecutionTest("export-expname-from-star-string");
    [Fact(DisplayName = "export-expname-from-star")]
    public Task export_expname_from_star() => ExecutionTest("export-expname-from-star");
    [Fact(DisplayName = "export-expname-from-string-binding")]
    public Task export_expname_from_string_binding() => ExecutionTest("export-expname-from-string-binding");
    [Fact(DisplayName = "export-expname-from-string-string")]
    public Task export_expname_from_string_string() => ExecutionTest("export-expname-from-string-string");
    [Fact(DisplayName = "export-expname-from-string")]
    public Task export_expname_from_string() => ExecutionTest("export-expname-from-string");
    [Fact(DisplayName = "export-expname-import-string-binding")]
    public Task export_expname_import_string_binding() => ExecutionTest("export-expname-import-string-binding");
    [Fact(DisplayName = "export-star-as-dflt")]
    public Task export_star_as_dflt() => ExecutionTest("export-star-as-dflt");
    [Fact(DisplayName = "instn-iee-bndng-cls")]
    public Task instn_iee_bndng_cls() => ExecutionTest("instn-iee-bndng-cls");
    [Fact(DisplayName = "instn-iee-bndng-const")]
    public Task instn_iee_bndng_const() => ExecutionTest("instn-iee-bndng-const");
    [Fact(DisplayName = "instn-iee-bndng-fun")]
    public Task instn_iee_bndng_fun() => ExecutionTest("instn-iee-bndng-fun");
    [Fact(DisplayName = "instn-iee-bndng-gen")]
    public Task instn_iee_bndng_gen() => ExecutionTest("instn-iee-bndng-gen");
    [Fact(DisplayName = "instn-iee-bndng-let")]
    public Task instn_iee_bndng_let() => ExecutionTest("instn-iee-bndng-let");
    [Fact(DisplayName = "instn-iee-bndng-var")]
    public Task instn_iee_bndng_var() => ExecutionTest("instn-iee-bndng-var");
    [Fact(DisplayName = "instn-iee-iee-cycle")]
    public Task instn_iee_iee_cycle() => ExecutionTest("instn-iee-iee-cycle");
    [Fact(DisplayName = "instn-iee-star-cycle")]
    public Task instn_iee_star_cycle() => ExecutionTest("instn-iee-star-cycle");
    [Fact(DisplayName = "instn-iee-trlng-comma")]
    public Task instn_iee_trlng_comma() => ExecutionTest("instn-iee-trlng-comma");
    [Fact(DisplayName = "instn-local-bndng-cls")]
    public Task instn_local_bndng_cls() => ExecutionTest("instn-local-bndng-cls");
    [Fact(DisplayName = "instn-local-bndng-const")]
    public Task instn_local_bndng_const() => ExecutionTest("instn-local-bndng-const");
    [Fact(DisplayName = "instn-local-bndng-export-cls")]
    public Task instn_local_bndng_export_cls() => ExecutionTest("instn-local-bndng-export-cls");
    [Fact(DisplayName = "instn-local-bndng-export-const")]
    public Task instn_local_bndng_export_const() => ExecutionTest("instn-local-bndng-export-const");
    [Fact(DisplayName = "instn-local-bndng-export-fun")]
    public Task instn_local_bndng_export_fun() => ExecutionTest("instn-local-bndng-export-fun");
    [Fact(DisplayName = "instn-local-bndng-export-gen")]
    public Task instn_local_bndng_export_gen() => ExecutionTest("instn-local-bndng-export-gen");
    [Fact(DisplayName = "instn-local-bndng-export-let")]
    public Task instn_local_bndng_export_let() => ExecutionTest("instn-local-bndng-export-let");
    [Fact(DisplayName = "instn-local-bndng-export-var")]
    public Task instn_local_bndng_export_var() => ExecutionTest("instn-local-bndng-export-var");
    [Fact(DisplayName = "instn-local-bndng-for-dup")]
    public Task instn_local_bndng_for_dup() => ExecutionTest("instn-local-bndng-for-dup");
    [Fact(DisplayName = "instn-local-bndng-for")]
    public Task instn_local_bndng_for() => ExecutionTest("instn-local-bndng-for");
    [Fact(DisplayName = "instn-local-bndng-fun")]
    public Task instn_local_bndng_fun() => ExecutionTest("instn-local-bndng-fun");
    [Fact(DisplayName = "instn-local-bndng-gen")]
    public Task instn_local_bndng_gen() => ExecutionTest("instn-local-bndng-gen");
    [Fact(DisplayName = "instn-local-bndng-let")]
    public Task instn_local_bndng_let() => ExecutionTest("instn-local-bndng-let");
    [Fact(DisplayName = "instn-local-bndng-var-dup")]
    public Task instn_local_bndng_var_dup() => ExecutionTest("instn-local-bndng-var-dup");
    [Fact(DisplayName = "instn-local-bndng-var")]
    public Task instn_local_bndng_var() => ExecutionTest("instn-local-bndng-var");
    [Fact(DisplayName = "instn-named-bndng-cls")]
    public Task instn_named_bndng_cls() => ExecutionTest("instn-named-bndng-cls");
    [Fact(DisplayName = "instn-named-bndng-const")]
    public Task instn_named_bndng_const() => ExecutionTest("instn-named-bndng-const");
    [Fact(DisplayName = "instn-named-bndng-dflt-cls")]
    public Task instn_named_bndng_dflt_cls() => ExecutionTest("instn-named-bndng-dflt-cls");
    [Fact(DisplayName = "instn-named-bndng-dflt-expr")]
    public Task instn_named_bndng_dflt_expr() => ExecutionTest("instn-named-bndng-dflt-expr");
    [Fact(DisplayName = "instn-named-bndng-dflt-fun-anon")]
    public Task instn_named_bndng_dflt_fun_anon() => ExecutionTest("instn-named-bndng-dflt-fun-anon");
    [Fact(DisplayName = "instn-named-bndng-dflt-fun-named")]
    public Task instn_named_bndng_dflt_fun_named() => ExecutionTest("instn-named-bndng-dflt-fun-named");
    [Fact(DisplayName = "instn-named-bndng-dflt-gen-anon")]
    public Task instn_named_bndng_dflt_gen_anon() => ExecutionTest("instn-named-bndng-dflt-gen-anon");
    [Fact(DisplayName = "instn-named-bndng-dflt-gen-named")]
    public Task instn_named_bndng_dflt_gen_named() => ExecutionTest("instn-named-bndng-dflt-gen-named");
    [Fact(DisplayName = "instn-named-bndng-dflt-named")]
    public Task instn_named_bndng_dflt_named() => ExecutionTest("instn-named-bndng-dflt-named");
    [Fact(DisplayName = "instn-named-bndng-dflt-star")]
    public Task instn_named_bndng_dflt_star() => ExecutionTest("instn-named-bndng-dflt-star");
    [Fact(DisplayName = "instn-named-bndng-fun")]
    public Task instn_named_bndng_fun() => ExecutionTest("instn-named-bndng-fun");
    [Fact(DisplayName = "instn-named-bndng-gen")]
    public Task instn_named_bndng_gen() => ExecutionTest("instn-named-bndng-gen");
    [Fact(DisplayName = "instn-named-bndng-let")]
    public Task instn_named_bndng_let() => ExecutionTest("instn-named-bndng-let");
    [Fact(DisplayName = "instn-named-bndng-trlng-comma")]
    public Task instn_named_bndng_trlng_comma() => ExecutionTest("instn-named-bndng-trlng-comma");
    [Fact(DisplayName = "instn-named-bndng-var")]
    public Task instn_named_bndng_var() => ExecutionTest("instn-named-bndng-var");
    [Fact(DisplayName = "instn-named-id-name")]
    public Task instn_named_id_name() => ExecutionTest("instn-named-id-name");
    [Fact(DisplayName = "instn-named-iee-cycle")]
    public Task instn_named_iee_cycle() => ExecutionTest("instn-named-iee-cycle");
    [Fact(DisplayName = "instn-named-star-cycle")]
    public Task instn_named_star_cycle() => ExecutionTest("instn-named-star-cycle");
    [Fact(DisplayName = "instn-once")]
    public Task instn_once() => ExecutionTest("instn-once");
    [Fact(DisplayName = "instn-same-global")]
    public Task instn_same_global() => ExecutionTest("instn-same-global");
    [Fact(DisplayName = "instn-star-as-props-dflt-skip")]
    public Task instn_star_as_props_dflt_skip() => ExecutionTest("instn-star-as-props-dflt-skip");
    [Fact(DisplayName = "instn-star-binding")]
    public Task instn_star_binding() => ExecutionTest("instn-star-binding");
    [Fact(DisplayName = "instn-star-equality")]
    public Task instn_star_equality() => ExecutionTest("instn-star-equality");
    [Fact(DisplayName = "instn-star-id-name")]
    public Task instn_star_id_name() => ExecutionTest("instn-star-id-name");
    [Fact(DisplayName = "instn-star-iee-cycle")]
    public Task instn_star_iee_cycle() => ExecutionTest("instn-star-iee-cycle");
    [Fact(DisplayName = "instn-star-props-circular")]
    public Task instn_star_props_circular() => ExecutionTest("instn-star-props-circular");
    [Fact(DisplayName = "instn-star-props-dflt-keep-indirect")]
    public Task instn_star_props_dflt_keep_indirect() => ExecutionTest("instn-star-props-dflt-keep-indirect");
    [Fact(DisplayName = "instn-star-props-dflt-keep-local")]
    public Task instn_star_props_dflt_keep_local() => ExecutionTest("instn-star-props-dflt-keep-local");
    [Fact(DisplayName = "instn-star-props-dflt-skip")]
    public Task instn_star_props_dflt_skip() => ExecutionTest("instn-star-props-dflt-skip");
    [Fact(DisplayName = "instn-star-props-nrml")]
    public Task instn_star_props_nrml() => ExecutionTest("instn-star-props-nrml");
    [Fact(DisplayName = "instn-star-star-cycle")]
    public Task instn_star_star_cycle() => ExecutionTest("instn-star-star-cycle");
    [Fact(DisplayName = "instn-uniq-env-rec")]
    public Task instn_uniq_env_rec() => ExecutionTest("instn-uniq-env-rec");
    [Fact(DisplayName = "parse-export-empty")]
    public Task parse_export_empty() => ExecutionTest("parse-export-empty");
    [Fact(DisplayName = "privatename-valid-no-earlyerr")]
    public Task privatename_valid_no_earlyerr() => ExecutionTest("privatename-valid-no-earlyerr");
}
