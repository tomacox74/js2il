using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.unshift;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.unshift") { }

    [Fact(DisplayName = "S15.4.4.13_A1_T1")]
    public Task S15_4_4_13_A1_T1()
        => ExecutionTestFromFile("S15.4.4.13_A1_T1");
}
