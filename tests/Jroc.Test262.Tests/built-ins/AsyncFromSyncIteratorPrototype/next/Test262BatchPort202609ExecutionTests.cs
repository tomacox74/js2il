using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncFromSyncIteratorPrototype.next;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.AsyncFromSyncIteratorPrototype.next") { }

    [Fact(DisplayName = "return-promise")]
    public Task return_promise()
        => ExecutionTestFromFile("return-promise");

}
