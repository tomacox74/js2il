using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype") { }

    [Fact(DisplayName = "15.2.3.1")]
    public Task _15_2_3_1()
        => ExecutionTestFromFile("15.2.3.1");

    [Fact(DisplayName = "S15.2.3.1_A1")]
    public Task S15_2_3_1_A1()
        => ExecutionTestFromFile("S15.2.3.1_A1");

    [Fact(DisplayName = "S15.2.3.1_A3")]
    public Task S15_2_3_1_A3()
        => ExecutionTestFromFile("S15.2.3.1_A3");
}
