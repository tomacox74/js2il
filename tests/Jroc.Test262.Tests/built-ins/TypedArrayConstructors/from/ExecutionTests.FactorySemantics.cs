using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.from;

public class FactorySemanticsExecutionTests : DiskExecutionTestsBase
{
    public FactorySemanticsExecutionTests() : base("built_ins.TypedArrayConstructors.from") { }

    [Fact(DisplayName = "custom-ctor-does-not-instantiate-ta-throws.js")]
    public Task custom_ctor_does_not_instantiate_ta_throws()
        => ExecutionTestFromFile("custom-ctor-does-not-instantiate-ta-throws");

    [Fact(DisplayName = "custom-ctor-returns-smaller-instance-throws.js")]
    public Task custom_ctor_returns_smaller_instance_throws()
        => ExecutionTestFromFile("custom-ctor-returns-smaller-instance-throws");

    [Fact(DisplayName = "mapfn-is-not-callable.js")]
    public Task mapfn_is_not_callable()
        => ExecutionTestFromFile("mapfn-is-not-callable");
}
