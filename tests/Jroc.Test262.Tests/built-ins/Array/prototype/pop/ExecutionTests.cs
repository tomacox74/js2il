using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.pop;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.pop") { }

    [Fact(DisplayName = "S15.4.4.6_A1.1_T1")]
    public Task S15_4_4_6_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.6_A1.1_T1");

    [Fact(DisplayName = "S15.4.4.6_A1.2_T1")]
    public Task S15_4_4_6_A1_2_T1()
        => ExecutionTestFromFile("S15.4.4.6_A1.2_T1");
}
