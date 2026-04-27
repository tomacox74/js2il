using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.logical_and;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.logical_and") { }

    [Fact(DisplayName = "symbol-logical-and-evaluation")]
    public Task symbol_logical_and_evaluation()
        => ExecutionTest("symbol-logical-and-evaluation");
}
