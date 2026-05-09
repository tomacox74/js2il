using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.subclass;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.subclass") { }

    [Fact(DisplayName = "binding")]
    public Task binding()
        => ExecutionTest("binding");
}
