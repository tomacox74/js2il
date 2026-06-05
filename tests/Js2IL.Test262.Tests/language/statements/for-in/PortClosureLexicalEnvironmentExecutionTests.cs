using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.for_in;

public class PortClosureLexicalEnvironmentExecutionTests : DiskExecutionTestsBase
{
    public PortClosureLexicalEnvironmentExecutionTests() : base("language.statements.for_in") { }

    [Fact(DisplayName = "scope-body-lex-boundary")]
    public Task scope_body_lex_boundary()
        => ExecutionTest("scope-body-lex-boundary");
}
