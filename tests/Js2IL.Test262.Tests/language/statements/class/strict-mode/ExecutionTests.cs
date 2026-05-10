using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.strict_mode;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.strict_mode") { }

    [Fact(DisplayName = "arguments-callee", Skip = "Tracked by issue #1055: class-expression derived constructor super() lowering is incomplete.")]
    public Task arguments_callee()
        => ExecutionTest("arguments-callee");
}
