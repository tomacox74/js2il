using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.shift;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.shift") { }

    [Fact(DisplayName = "S15.4.4.9_A1.1_T1")]
    public Task S15_4_4_9_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.9_A1.1_T1");
}
