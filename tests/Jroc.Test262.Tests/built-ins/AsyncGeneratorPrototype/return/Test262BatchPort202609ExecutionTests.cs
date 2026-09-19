using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorPrototype.@return;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.AsyncGeneratorPrototype.return") { }

    [Fact(DisplayName = "return-promise")]
    public Task return_promise()
        => ExecutionTestFromFile("return-promise");

}
