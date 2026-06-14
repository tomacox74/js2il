using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.logical_or;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.logical_or") { }

    [Fact(DisplayName = "symbol-logical-or-evaluation")]
    public Task symbol_logical_or_evaluation()
        => ExecutionTest("symbol-logical-or-evaluation");
}
