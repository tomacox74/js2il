using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array.from;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.from") { }

    [Fact(DisplayName = "calling-from-valid-1-noStrict")]
    public Task calling_from_valid_1_noStrict()
        => ExecutionTest("calling-from-valid-1-noStrict");
}
