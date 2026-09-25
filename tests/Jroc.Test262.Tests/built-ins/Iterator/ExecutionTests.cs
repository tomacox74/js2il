using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
