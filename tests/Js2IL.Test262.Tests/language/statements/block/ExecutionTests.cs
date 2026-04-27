using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.block;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.block") { }

    [Fact(DisplayName = "scope-lex-close", Skip = "Known JS2IL defect")]
    public Task scope_lex_close()
        => ExecutionTest("scope-lex-close");

    [Fact(DisplayName = "scope-lex-open", Skip = "Known JS2IL defect")]
    public Task scope_lex_open()
        => ExecutionTest("scope-lex-open");

    [Fact(DisplayName = "scope-var-none")]
    public Task scope_var_none()
        => ExecutionTest("scope-var-none");
}
