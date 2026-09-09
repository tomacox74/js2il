using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.forEach;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.forEach") { }

    [Fact(DisplayName = "15.4.4.18-5-7")]
    public Task _15_4_4_18_5_7() => ExecutionTestFromFile("15.4.4.18-5-7");

}
