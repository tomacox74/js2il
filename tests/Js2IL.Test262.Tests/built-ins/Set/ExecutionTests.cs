using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Set;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set") { }

    [Fact(DisplayName = "bigint-number-same-value", Skip = "Known JS2IL defect")]
    public Task bigint_number_same_value()
        => ExecutionTest("bigint-number-same-value");
}
