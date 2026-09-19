using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.enumerate;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Proxy.enumerate") { }

    [Fact(DisplayName = "removed-does-not-trigger")]
    public Task removed_does_not_trigger()
        => ExecutionTestFromFile("removed-does-not-trigger");

}
