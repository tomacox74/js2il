using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.isExtensible;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.isExtensible") { }

    [Fact(DisplayName = "15.2.3.13-1-1")]
    public Task _15_2_3_13_1_1() => ExecutionTestFromFile("15.2.3.13-1-1");

    [Fact(DisplayName = "15.2.3.13-1-2")]
    public Task _15_2_3_13_1_2() => ExecutionTestFromFile("15.2.3.13-1-2");

}
