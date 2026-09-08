using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.keywords;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/keywords", "language.keywords") { }

    [Fact(DisplayName = "ident-ref-break.js")]
    public Task ident_ref_break()
        => CompilationFailureTest("ident-ref-break");

    [Fact(DisplayName = "ident-ref-case.js")]
    public Task ident_ref_case()
        => CompilationFailureTest("ident-ref-case");

    [Fact(DisplayName = "ident-ref-catch.js")]
    public Task ident_ref_catch()
        => CompilationFailureTest("ident-ref-catch");

    [Fact(DisplayName = "ident-ref-continue.js")]
    public Task ident_ref_continue()
        => CompilationFailureTest("ident-ref-continue");

    [Fact(DisplayName = "ident-ref-default.js")]
    public Task ident_ref_default()
        => CompilationFailureTest("ident-ref-default");

    [Fact(DisplayName = "ident-ref-delete.js")]
    public Task ident_ref_delete()
        => CompilationFailureTest("ident-ref-delete");

    [Fact(DisplayName = "ident-ref-do.js")]
    public Task ident_ref_do()
        => CompilationFailureTest("ident-ref-do");

    [Fact(DisplayName = "ident-ref-else.js")]
    public Task ident_ref_else()
        => CompilationFailureTest("ident-ref-else");

    [Fact(DisplayName = "ident-ref-finally.js")]
    public Task ident_ref_finally()
        => CompilationFailureTest("ident-ref-finally");

    [Fact(DisplayName = "ident-ref-for.js")]
    public Task ident_ref_for()
        => CompilationFailureTest("ident-ref-for");

    [Fact(DisplayName = "ident-ref-function.js")]
    public Task ident_ref_function()
        => CompilationFailureTest("ident-ref-function");

    [Fact(DisplayName = "ident-ref-if.js")]
    public Task ident_ref_if()
        => CompilationFailureTest("ident-ref-if");

    [Fact(DisplayName = "ident-ref-in.js")]
    public Task ident_ref_in()
        => CompilationFailureTest("ident-ref-in");

    [Fact(DisplayName = "ident-ref-instanceof.js")]
    public Task ident_ref_instanceof()
        => CompilationFailureTest("ident-ref-instanceof");

    [Fact(DisplayName = "ident-ref-new.js")]
    public Task ident_ref_new()
        => CompilationFailureTest("ident-ref-new");

    [Fact(DisplayName = "ident-ref-return.js")]
    public Task ident_ref_return()
        => CompilationFailureTest("ident-ref-return");

    [Fact(DisplayName = "ident-ref-switch.js")]
    public Task ident_ref_switch()
        => CompilationFailureTest("ident-ref-switch");

    [Fact(DisplayName = "ident-ref-this.js")]
    public Task ident_ref_this()
        => CompilationFailureTest("ident-ref-this");

    [Fact(DisplayName = "ident-ref-throw.js")]
    public Task ident_ref_throw()
        => CompilationFailureTest("ident-ref-throw");

    [Fact(DisplayName = "ident-ref-try.js")]
    public Task ident_ref_try()
        => CompilationFailureTest("ident-ref-try");

    [Fact(DisplayName = "ident-ref-typeof.js")]
    public Task ident_ref_typeof()
        => CompilationFailureTest("ident-ref-typeof");

    [Fact(DisplayName = "ident-ref-var.js")]
    public Task ident_ref_var()
        => CompilationFailureTest("ident-ref-var");

    [Fact(DisplayName = "ident-ref-void.js")]
    public Task ident_ref_void()
        => CompilationFailureTest("ident-ref-void");

    [Fact(DisplayName = "ident-ref-while.js")]
    public Task ident_ref_while()
        => CompilationFailureTest("ident-ref-while");

    [Fact(DisplayName = "ident-ref-with.js")]
    public Task ident_ref_with()
        => CompilationFailureTest("ident-ref-with");
}
