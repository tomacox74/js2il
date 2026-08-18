using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.propertyIsEnumerable;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.propertyIsEnumerable") { }

    [Fact(DisplayName = "S15.2.4.7_A11")]
    public Task S15_2_4_7_A11()
        => ExecutionTestFromFile("S15.2.4.7_A11");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "symbol_property_toPrimitive")]
    public Task symbol_property_toPrimitive()
        => ExecutionTestFromFile("symbol_property_toPrimitive");

    [Fact(DisplayName = "symbol_property_toString")]
    public Task symbol_property_toString()
        => ExecutionTestFromFile("symbol_property_toString");

    [Fact(DisplayName = "symbol_property_valueOf")]
    public Task symbol_property_valueOf()
        => ExecutionTestFromFile("symbol_property_valueOf");
}
