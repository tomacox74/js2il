using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.global_code;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/global-code", "language.global_code") { }

    [Fact(DisplayName = "S10.1.7_A1_T1.js")]
    public Task S10_1_7_A1_T1()
        => ExecutionTest("S10.1.7_A1_T1");

    [Fact(DisplayName = "S10.4.1_A1_T1.js")]
    public Task S10_4_1_A1_T1()
        => ExecutionTest("S10.4.1_A1_T1");

    [Fact(DisplayName = "S10.4.1_A1_T2.js")]
    public Task S10_4_1_A1_T2()
        => ExecutionTest("S10.4.1_A1_T2");

    [Fact(DisplayName = "block-decl-strict.js")]
    public Task block_decl_strict()
        => ExecutionTest("block-decl-strict");

    [Fact(DisplayName = "decl-func-dup.js")]
    public Task decl_func_dup()
        => ExecutionTest("decl-func-dup");

    [Fact(DisplayName = "decl-lex-configurable-global.js")]
    public Task decl_lex_configurable_global()
        => ExecutionTest("decl-lex-configurable-global");

    [Fact(DisplayName = "decl-lex-deletion.js")]
    public Task decl_lex_deletion()
        => ExecutionTest("decl-lex-deletion");

    [Fact(DisplayName = "decl-lex.js")]
    public Task decl_lex()
        => ExecutionTest("decl-lex");

    [Fact(DisplayName = "decl-var.js")]
    public Task decl_var()
        => ExecutionTest("decl-var");

    [Fact(DisplayName = "import.js")]
    public Task import()
        => CompilationFailureTest("import");

    [Fact(DisplayName = "invalid-private-names-call-expression-bad-reference.js")]
    public Task invalid_private_names_call_expression_bad_reference()
        => CompilationFailureTest("invalid-private-names-call-expression-bad-reference");

    [Fact(DisplayName = "invalid-private-names-call-expression-this.js")]
    public Task invalid_private_names_call_expression_this()
        => CompilationFailureTest("invalid-private-names-call-expression-this");

    [Fact(DisplayName = "invalid-private-names-member-expression-bad-reference.js")]
    public Task invalid_private_names_member_expression_bad_reference()
        => CompilationFailureTest("invalid-private-names-member-expression-bad-reference");

    [Fact(DisplayName = "invalid-private-names-member-expression-this.js")]
    public Task invalid_private_names_member_expression_this()
        => CompilationFailureTest("invalid-private-names-member-expression-this");

    [Fact(DisplayName = "new.target-arrow.js")]
    public Task new_target_arrow()
        => CompilationFailureTest("new.target-arrow");

    [Fact(DisplayName = "new.target.js")]
    public Task new_target()
        => CompilationFailureTest("new.target");

    [Fact(DisplayName = "super-call-arrow.js")]
    public Task super_call_arrow()
        => CompilationFailureTest("super-call-arrow");

    [Fact(DisplayName = "super-call.js")]
    public Task super_call()
        => CompilationFailureTest("super-call");

    [Fact(DisplayName = "super-prop-arrow.js")]
    public Task super_prop_arrow()
        => CompilationFailureTest("super-prop-arrow");

    [Fact(DisplayName = "super-prop.js")]
    public Task super_prop()
        => CompilationFailureTest("super-prop");

    [Fact(DisplayName = "switch-case-decl-strict.js")]
    public Task switch_case_decl_strict()
        => ExecutionTest("switch-case-decl-strict");

    [Fact(DisplayName = "switch-dflt-decl-strict.js")]
    public Task switch_dflt_decl_strict()
        => ExecutionTest("switch-dflt-decl-strict");

    [Fact(DisplayName = "unscopables-ignored.js")]
    public Task unscopables_ignored()
        => ExecutionTest("unscopables-ignored");

    [Fact(DisplayName = "yield-non-strict.js")]
    public Task yield_non_strict()
        => ExecutionTest("yield-non-strict");

    [Fact(DisplayName = "yield-strict.js")]
    public Task yield_strict()
        => CompilationFailureTest("yield-strict");
}
