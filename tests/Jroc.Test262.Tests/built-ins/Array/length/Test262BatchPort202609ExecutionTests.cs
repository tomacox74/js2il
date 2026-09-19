using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.length;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.length") { }

    [Fact(DisplayName = "S15.4.4_A1.3_T1")]
    public Task S15_4_4_A1_3_T1()
        => ExecutionTestFromFile("S15.4.4_A1.3_T1");

}
