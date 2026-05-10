using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.arguments;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.arguments") { }

    [Fact(DisplayName = "access")]
    public Task access()
        => ExecutionTest("access");

    [Fact(DisplayName = "default-constructor")]
    public Task default_constructor()
        => ExecutionTest("default-constructor");
}
