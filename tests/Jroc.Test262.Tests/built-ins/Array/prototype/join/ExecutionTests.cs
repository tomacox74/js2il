using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.join;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.join") { }

    [Fact(DisplayName = "S15.4.4.5_A1.1_T1")]
    public Task S15_4_4_5_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.5_A1.1_T1");

}
