using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertyDescriptor;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "15.2.3.3-4-185")]
    public Task _15_2_3_3_4_185() => ExecutionTestFromFile("15.2.3.3-4-185");

    [Fact(DisplayName = "15.2.3.3-4-189")]
    public Task _15_2_3_3_4_189() => ExecutionTestFromFile("15.2.3.3-4-189");

}
