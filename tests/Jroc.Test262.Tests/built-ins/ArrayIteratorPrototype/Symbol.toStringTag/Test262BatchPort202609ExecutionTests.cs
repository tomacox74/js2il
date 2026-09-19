using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayIteratorPrototype.Symbol.toStringTag;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.ArrayIteratorPrototype.Symbol.toStringTag") { }

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "value-direct")]
    public Task value_direct()
        => ExecutionTestFromFile("value-direct");

    [Fact(DisplayName = "value-from-to-string")]
    public Task value_from_to_string()
        => ExecutionTestFromFile("value-from-to-string");

}
