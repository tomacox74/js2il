using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.prototype") { }

    [Fact(DisplayName = "S15.3.5.2_A1_T1.js")]
    public Task Port200_S15_3_5_2_A1_T1()
        => ExecutionTestFromFile("S15.3.5.2_A1_T1");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "property-order.js")]
    public Task Port200_property_order()
        => ExecutionTestFromFile("property-order");
}
