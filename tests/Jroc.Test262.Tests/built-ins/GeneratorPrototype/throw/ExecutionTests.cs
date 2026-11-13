using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.@throw;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorPrototype.throw") { }

    [Fact(DisplayName = "from-state-executing.js")]
    public Task Port200_from_state_executing()
        => ExecutionTestFromFile("from-state-executing");

    [Fact(DisplayName = "from-state-suspended-start.js")]
    public Task Port200_from_state_suspended_start()
        => ExecutionTestFromFile("from-state-suspended-start");

    [Fact(DisplayName = "length.js")]
    public Task Port200_length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "property-descriptor.js")]
    public Task Port200_property_descriptor()
        => ExecutionTestFromFile("property-descriptor");
}
