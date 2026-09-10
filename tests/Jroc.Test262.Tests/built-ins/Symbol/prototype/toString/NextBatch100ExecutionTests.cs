using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.toString;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Symbol.prototype.toString") { }

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "toString-default-attributes-non-strict")]
    public Task toString_default_attributes_non_strict() => ExecutionTestFromFile("toString-default-attributes-non-strict");

    [Fact(DisplayName = "toString-default-attributes-strict")]
    public Task toString_default_attributes_strict() => ExecutionTestFromFile("toString-default-attributes-strict");
}
