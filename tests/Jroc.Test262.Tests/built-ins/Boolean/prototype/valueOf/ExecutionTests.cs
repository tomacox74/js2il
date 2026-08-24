using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype.valueOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Boolean.prototype.valueOf") { }

    [Fact(DisplayName = "S15.6.4.3_A1_T1")]
    public Task S15_6_4_3_A1_T1()
        => ExecutionTestFromFile("S15.6.4.3_A1_T1");

    [Fact(DisplayName = "S15.6.4.3_A1_T2")]
    public Task S15_6_4_3_A1_T2()
        => ExecutionTestFromFile("S15.6.4.3_A1_T2");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
