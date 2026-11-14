using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.@return;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.GeneratorPrototype.return") { }

    [Fact(DisplayName = "from-state-executing.js")]
    public Task Port200_from_state_executing()
        => ExecutionTestFromFile("from-state-executing");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "property-descriptor.js")]
    public Task Port200_property_descriptor()
        => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "try-finally-nested-try-catch-within-outer-try-before-nested.js")]
    public Task Port200_try_finally_nested_try_catch_within_outer_try_before_nested()
        => ExecutionTestFromFile("try-finally-nested-try-catch-within-outer-try-before-nested");
}
