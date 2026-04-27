using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Map;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map") { }

    [Fact(DisplayName = "bigint-number-same-value")]
    public Task bigint_number_same_value()
        => ExecutionTest("bigint-number-same-value");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTest("constructor");
}
