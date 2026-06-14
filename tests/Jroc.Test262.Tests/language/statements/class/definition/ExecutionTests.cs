using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.definition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.definition") { }

    [Fact(DisplayName = "accessors")]
    public Task accessors()
        => ExecutionTest("accessors");

    [Fact(DisplayName = "basics")]
    public Task basics()
        => ExecutionTest("basics");
}
