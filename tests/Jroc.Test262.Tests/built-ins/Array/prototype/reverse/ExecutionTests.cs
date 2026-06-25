using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.reverse;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.reverse") { }

    [Fact(DisplayName = "S15.4.4.8_A1_T1")]
    public Task S15_4_4_8_A1_T1()
        => ExecutionTestFromFile("S15.4.4.8_A1_T1");
}
