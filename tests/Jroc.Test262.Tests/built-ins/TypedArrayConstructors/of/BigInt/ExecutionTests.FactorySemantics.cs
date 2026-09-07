using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.of.BigInt;

public class FactorySemanticsExecutionTests : DiskExecutionTestsBase
{
    public FactorySemanticsExecutionTests() : base("built_ins.TypedArrayConstructors.of.BigInt") { }

    [Fact(DisplayName = "custom-ctor-does-not-instantiate-ta-throws.js")]
    public Task custom_ctor_does_not_instantiate_ta_throws()
        => ExecutionTestFromFile("custom-ctor-does-not-instantiate-ta-throws");

    [Fact(DisplayName = "custom-ctor-returns-other-instance.js")]
    public Task custom_ctor_returns_other_instance()
        => ExecutionTestFromFile("custom-ctor-returns-other-instance");
}
