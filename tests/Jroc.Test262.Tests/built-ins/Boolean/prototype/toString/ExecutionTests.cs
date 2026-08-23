using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Boolean.prototype.toString") { }

    [Fact(DisplayName = "S15.6.4.2_A1_T1")]
    public Task S15_6_4_2_A1_T1()
        => ExecutionTestFromFile("S15.6.4.2_A1_T1");

    [Fact(DisplayName = "S15.6.4.2_A1_T2")]
    public Task S15_6_4_2_A1_T2()
        => ExecutionTestFromFile("S15.6.4.2_A1_T2");
}
