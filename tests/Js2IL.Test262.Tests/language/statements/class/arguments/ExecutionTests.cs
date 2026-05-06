using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.arguments;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.arguments") { }

    [Fact(DisplayName = "access", Skip = "Class body arguments semantics are not implemented yet.")]
    public Task access()
        => ExecutionTest("access");
}
