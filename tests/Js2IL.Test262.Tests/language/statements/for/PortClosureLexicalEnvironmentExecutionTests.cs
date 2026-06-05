using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.for_;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.for_") { }

    [Fact(DisplayName = "head-const-fresh-binding-per-iteration")]
    public Task head_const_fresh_binding_per_iteration()
        => ExecutionTest("head-const-fresh-binding-per-iteration");

    [Fact(DisplayName = "head-let-fresh-binding-per-iteration")]
    public Task head_let_fresh_binding_per_iteration()
        => ExecutionTest("head-let-fresh-binding-per-iteration");

    [Fact(DisplayName = "scope-body-lex-boundary")]
    public Task scope_body_lex_boundary()
        => ExecutionTest("scope-body-lex-boundary");

    [Fact(DisplayName = "scope-head-lex-close")]
    public Task scope_head_lex_close()
        => ExecutionTest("scope-head-lex-close");

    [Fact(DisplayName = "scope-head-lex-open")]
    public Task scope_head_lex_open()
        => ExecutionTest("scope-head-lex-open");
}
