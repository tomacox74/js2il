using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.directive_prologue;

public class ParseExecutionTests : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\directive-prologue", "language.directive_prologue") { }

    [Fact(DisplayName = "10.1.1-2gs")]
    public Task _10_1_1_2gs()
        => CompilationFailureTest("10.1.1-2gs");

    [Fact(DisplayName = "10.1.1-5gs")]
    public Task _10_1_1_5gs()
        => CompilationFailureTest("10.1.1-5gs");

    [Fact(DisplayName = "10.1.1-8gs")]
    public Task _10_1_1_8gs()
        => CompilationFailureTest("10.1.1-8gs");

    [Fact(DisplayName = "14.1-4gs")]
    public Task _14_1_4gs()
        => CompilationFailureTest("14.1-4gs");

    [Fact(DisplayName = "14.1-5gs")]
    public Task _14_1_5gs()
        => CompilationFailureTest("14.1-5gs");

    [Fact(DisplayName = "func-decl-inside-func-decl-parse")]
    public Task func_decl_inside_func_decl_parse()
        => CompilationFailureTest("func-decl-inside-func-decl-parse");

    [Fact(DisplayName = "func-decl-no-semi-parse")]
    public Task func_decl_no_semi_parse()
        => CompilationFailureTest("func-decl-no-semi-parse");

    [Fact(DisplayName = "func-decl-parse")]
    public Task func_decl_parse()
        => CompilationFailureTest("func-decl-parse");

    [Fact(DisplayName = "func-expr-inside-func-decl-parse")]
    public Task func_expr_inside_func_decl_parse()
        => CompilationFailureTest("func-expr-inside-func-decl-parse");

    [Fact(DisplayName = "func-expr-no-semi-parse")]
    public Task func_expr_no_semi_parse()
        => CompilationFailureTest("func-expr-no-semi-parse");

    [Fact(DisplayName = "func-expr-parse")]
    public Task func_expr_parse()
        => CompilationFailureTest("func-expr-parse");

}
