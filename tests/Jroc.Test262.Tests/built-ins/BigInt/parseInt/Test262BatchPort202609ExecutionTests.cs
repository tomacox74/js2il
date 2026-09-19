using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.parseInt;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.BigInt.parseInt") { }

    [Fact(DisplayName = "nonexistent")]
    public Task nonexistent()
        => ExecutionTestFromFile("nonexistent");

}
