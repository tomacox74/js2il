using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Map") { }

    [Fact(DisplayName = "valid-keys")]
    public Task valid_keys()
        => ExecutionTestFromFile("valid-keys");

}
