using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.try_;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.try_") { }

    [Fact(DisplayName = "scope-catch-block-lex-close")]
    public Task scope_catch_block_lex_close()
        => ExecutionTest("scope-catch-block-lex-close");
}
