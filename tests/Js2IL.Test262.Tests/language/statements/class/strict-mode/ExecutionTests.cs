using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.strict_mode;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.strict_mode") { }

    [Fact(DisplayName = "arguments-callee")]
    public Task arguments_callee()
        => ExecutionTest("arguments-callee");
}
