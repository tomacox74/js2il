using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.for_of;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.for_of") { }

    [Fact(DisplayName = "head-const-bound-names-fordecl-tdz")]
    public Task head_const_bound_names_fordecl_tdz()
        => ExecutionTest("head-const-bound-names-fordecl-tdz");

    [Fact(DisplayName = "head-const-fresh-binding-per-iteration")]
    public Task head_const_fresh_binding_per_iteration()
        => ExecutionTest("head-const-fresh-binding-per-iteration");

    [Fact(DisplayName = "head-let-bound-names-fordecl-tdz")]
    public Task head_let_bound_names_fordecl_tdz()
        => ExecutionTest("head-let-bound-names-fordecl-tdz");

    [Fact(DisplayName = "head-let-fresh-binding-per-iteration")]
    public Task head_let_fresh_binding_per_iteration()
        => ExecutionTest("head-let-fresh-binding-per-iteration");

    [Fact(DisplayName = "scope-body-lex-boundary")]
    public Task scope_body_lex_boundary()
        => ExecutionTest("scope-body-lex-boundary");
}
