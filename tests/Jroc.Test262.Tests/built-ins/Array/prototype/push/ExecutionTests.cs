using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.push;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.push") { }

    [Fact(DisplayName = "S15.4.4.7_A1_T1")]
    public Task S15_4_4_7_A1_T1()
        => ExecutionTestFromFile("S15.4.4.7_A1_T1");
    [Fact(DisplayName = "S15.4.4.7_A1_T2")]
    public Task S15_4_4_7_A1_T2()
        => ExecutionTestFromFile("S15.4.4.7_A1_T2");

}
