using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.arguments;

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
