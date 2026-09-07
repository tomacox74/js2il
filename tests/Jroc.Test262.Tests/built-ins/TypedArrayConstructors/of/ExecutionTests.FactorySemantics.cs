using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.of;

public class FactorySemanticsExecutionTests : DiskExecutionTestsBase
{
    public FactorySemanticsExecutionTests() : base("built_ins.TypedArrayConstructors.of") { }

    [Fact(DisplayName = "custom-ctor-does-not-instantiate-ta-throws.js")]
    public Task custom_ctor_does_not_instantiate_ta_throws()
        => ExecutionTestFromFile("custom-ctor-does-not-instantiate-ta-throws");

    [Fact(DisplayName = "custom-ctor-returns-smaller-instance-throws.js")]
    public Task custom_ctor_returns_smaller_instance_throws()
        => ExecutionTestFromFile("custom-ctor-returns-smaller-instance-throws");
}
