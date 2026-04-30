using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.switch_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.switch_") { }

    [Fact(DisplayName = "scope-lex-close-case")]
    public Task scope_lex_close_case()
        => ExecutionTest("scope-lex-close-case");

    [Fact(DisplayName = "scope-lex-open-dflt")]
    public Task scope_lex_open_dflt()
        => ExecutionTest("scope-lex-open-dflt");
}
