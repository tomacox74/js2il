namespace Jroc.Test262.Tests.built_ins.Object.defineProperty;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.defineProperty") { }

    [Fact(DisplayName = "15.2.3.6-4-531-12.js")]
    public Task _15_2_3_6_4_531_12() => ExecutionTestFromFile("15.2.3.6-4-531-12");

    [Fact(DisplayName = "15.2.3.6-4-531-3.js")]
    public Task _15_2_3_6_4_531_3() => ExecutionTestFromFile("15.2.3.6-4-531-3");

    [Fact(DisplayName = "coerced-P-grow.js")]
    public Task coerced_P_grow() => ExecutionTestFromFile("coerced-P-grow");

    [Fact(DisplayName = "coerced-P-shrink.js")]
    public Task coerced_P_shrink() => ExecutionTestFromFile("coerced-P-shrink");

    [Fact(DisplayName = "symbol-data-property-configurable.js")]
    public Task symbol_data_property_configurable() => ExecutionTestFromFile("symbol-data-property-configurable");

    [Fact(DisplayName = "symbol-data-property-default-non-strict.js")]
    public Task symbol_data_property_default_non_strict() => ExecutionTestFromFile("symbol-data-property-default-non-strict");
}
