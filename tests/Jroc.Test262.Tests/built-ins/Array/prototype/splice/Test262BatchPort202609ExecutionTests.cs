using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.splice;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.splice") { }

    [Fact(DisplayName = "create-species-length-exceeding-integer-limit")]
    public Task create_species_length_exceeding_integer_limit()
        => ExecutionTestFromFile("create-species-length-exceeding-integer-limit");

    [Fact(DisplayName = "property-traps-order-with-species")]
    public Task property_traps_order_with_species()
        => ExecutionTestFromFile("property-traps-order-with-species");

}
