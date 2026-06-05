using Js2IL.Test262.Tests.language.modules;

namespace Js2IL.Test262.Tests.language.module_code;

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

    [Fact(DisplayName = "early-import-arguments")]
    public Task early_import_arguments()
        => CompilationFailureTest("early-import-arguments");

    [Fact(DisplayName = "early-import-as-arguments")]
    public Task early_import_as_arguments()
        => CompilationFailureTest("early-import-as-arguments");

    [Fact(DisplayName = "early-import-as-eval")]
    public Task early_import_as_eval()
        => CompilationFailureTest("early-import-as-eval");

    [Fact(DisplayName = "early-import-eval")]
    public Task early_import_eval()
        => CompilationFailureTest("early-import-eval");

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

    [Fact(DisplayName = "parse-err-semi-named-export")]
    public Task parse_err_semi_named_export()
        => CompilationFailureTest("parse-err-semi-named-export");

}
