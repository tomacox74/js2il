using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.every;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.every") { }

    [Fact(DisplayName = "15.4.4.16-5-7")]
    public Task _15_4_4_16_5_7() => ExecutionTestFromFile("15.4.4.16-5-7");

}
