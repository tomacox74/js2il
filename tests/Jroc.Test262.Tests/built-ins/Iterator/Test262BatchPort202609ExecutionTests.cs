using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Iterator") { }

    [Fact(DisplayName = "subclassable")]
    public Task subclassable()
        => ExecutionTestFromFile("subclassable");

}
