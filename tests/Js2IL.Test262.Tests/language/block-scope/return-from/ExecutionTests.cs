using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.block_scope.return_from;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.return_from") { }

    [Fact(DisplayName = "block-const")]
    public Task block_const()
        => ExecutionTest("block-const");
}
