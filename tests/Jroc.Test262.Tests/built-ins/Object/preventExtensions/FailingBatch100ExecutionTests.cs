using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.preventExtensions;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.preventExtensions") { }

    [Fact(DisplayName = "15.2.3.10-1-1")]
    public Task _15_2_3_10_1_1() => ExecutionTestFromFile("15.2.3.10-1-1");

    [Fact(DisplayName = "15.2.3.10-1-2")]
    public Task _15_2_3_10_1_2() => ExecutionTestFromFile("15.2.3.10-1-2");

}
