using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.JSON;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.JSON") { }

    [Fact(DisplayName = "15.12-0-3")]
    public Task _15_12_0_3()
        => ExecutionTestFromFile("15.12-0-3");

}
