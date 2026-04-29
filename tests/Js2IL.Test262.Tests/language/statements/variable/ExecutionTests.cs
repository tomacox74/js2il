using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.variable;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.variable") { }

    [Fact(DisplayName = "binding-resolution")]
    public Task binding_resolution()
        => ExecutionTest("binding-resolution");
}
