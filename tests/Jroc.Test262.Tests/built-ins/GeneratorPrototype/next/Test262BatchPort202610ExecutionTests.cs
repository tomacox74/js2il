using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorPrototype.next;

public class Test262BatchPort202610ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202610ExecutionTests() : base("built_ins.GeneratorPrototype.next") { }

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");
}
